using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Pes.Data;
using Pes.Models;
using Pes.Models.DMdel;
using Pes.Pages;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pes
{
    public partial class DMdelService
    {
        public DMdelService(DMdelContext context, NavigationManager navigationManager, SecurityService security)
        {
            this.context = context;
            this.navigationManager = navigationManager;
            this.Security = security;
        }

        private readonly SecurityService Security;
        public DMdelContext DMContext
        {
            get
            {
                return this.context;
            }
        }


        public async System.Threading.Tasks.Task<Models.DMdel.Session> GetActiveSession()
        {
            return context.Sessions.FirstOrDefault(s => s.EnCours);
        }
        public async System.Threading.Tasks.Task CalculerMoyenneGlobale(int SessionId)
        {
            var stags = context.Stagiaires.Where(s => s.Sessionid == SessionId).ToList();

            var session = context.Sessions.FirstOrDefault(s => s.Id == SessionId);
            if (session == null) return;

            double moyenne = stags.Any() ? stags.Average(s => s.NoteFinale) : 0;

            session.MoyenneGlobale = moyenne;

            await context.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task ReintEvals(int StagId, int SessionId)
        {
            var evals = Context.Evaluations.Where(ev => ev.Stagid == StagId).ToList();

            Context.Evaluations.RemoveRange(evals);

            await  Context.SaveChangesAsync();

            await CreateEvals(StagId, SessionId);
        }

        public async System.Threading.Tasks.Task CreateEvals(int StagId,int SessionId)
        {

            var stagiaire = await GetStagiaireById(StagId);


            var criteres = Context.Criteres.Where(c=>c.Sessionid == SessionId).Include(ev => ev.Element).ThenInclude(e => e.Rubrique).OrderBy(c=>c.NomCritere).ToList(); //(await DMdel. GetCriteres()).ToList();

            var membresjury = (await Security.GetUsersInRoleAndEtab(new string[] { Constants.expert, Constants.membre_jury, Constants.president_jury }, stagiaire.Etabid))?.ToList() ?? new List<ApplicationUser>();
            membresjury = membresjury.GroupBy(m => m.Id).Select(g => g.First()).ToList();


            string NomElem, NomRubrique;

            List<Evaluation> evaluations = new List<Evaluation>();

            var Evals = Context.Evaluations.Where(ev => ev.Stagid == StagId).ToList();

            foreach (var critr in criteres)
            {

                NomElem = critr.Element?.NomElement;
                NomRubrique = critr.Element?.Rubrique?.NomRubrique;

                if (!Evals.Any(v => v.Criterid == critr.Id && v.EstSynthese)
                    && !evaluations.Any(v => v.Criterid == critr.Id && v.EstSynthese))
                {
                    evaluations.Add(new Models.DMdel.Evaluation { Criterid = critr.Id, Stagid = StagId, MembreId = string.Empty, NomElement = NomElem, NomRubrique = NomRubrique, EstSynthese = true });

                }

                foreach (var membre in membresjury)
                {
                    if (!Evals.Any(v => v.Criterid == critr.Id && v.MembreId == membre.Id && !v.EstSynthese)
                        && !evaluations.Any(v => v.Criterid == critr.Id && v.MembreId == membre.Id && !v.EstSynthese))
                    {
                        evaluations.Add(new Models.DMdel.Evaluation { Criterid = critr.Id, Stagid = StagId, MembreId = membre.Id, NomElement = NomElem, NomRubrique = NomRubrique, EstSynthese = false });
                    }
                }
            }

            foreach (var item in Evals)
            {
                if (!criteres.Any(c => c.Id == item.Criterid)) Context.Remove(item);
            }

            await Context.AddRangeAsync(evaluations);
            await Context.SaveChangesAsync();

        }

        protected async System.Threading.Tasks.Task CalculerMoyeneCritere(int stagiairId, int criterid, List<ApplicationUser> Membresjury)
        {
            if (Membresjury == null) Membresjury = new List<ApplicationUser>();

            var AllEvals = Context.Evaluations.Include(ev => ev.Echelle).Where(ev => ev.Stagid == stagiairId && ev.Criterid == criterid);
            if (!AllEvals.Any()) return;
            double somme = 0;
            double count = 0;
            foreach (var item in AllEvals)
            {

                if (item.Echelle != null)
                    if (!item.EstSynthese && Membresjury.Any(m => m.Id == item.MembreId))
                    {
                        somme += item.Echelle.Val;
                        count++;
                    }
            }

            var synthese = AllEvals.FirstOrDefault(s => s.EstSynthese);

            if (synthese != null)
            {
                // Aucun membre du jury n'a note ce critere -> NULL (« non evalue »), jamais NaN.
                synthese.NoteSynthese = count == 0 ? (double?)null : somme / count;
                await Context.SaveChangesAsync();

            }

        }

        /// <summary>
        /// Valeur maximale de l'echelle de la session. 0 si l'echelle est vide : la note vaut alors 0.
        /// </summary>
        public static double GetEchelleMax(DMdelContext context, int sessionId)
        {
            var max = context.Echelles
                .Where(e => e.Sessionid == sessionId || (e.Sessionid == null && !context.Echelles.Any(x => x.Sessionid == sessionId)))
                .Select(e => (double?)e.Val)
                .Max();

            return max ?? 0d;
        }

        /// <summary>
        /// Moyenne des notes de jury pour un critere. null = aucun membre du jury n'a note
        /// (criteres « non evalue »). Calcul pur, sans ecriture.
        /// </summary>
        private static double? MoyenneCritere(IEnumerable<Evaluation> allEvals, List<ApplicationUser> membresjury)
        {
            if (membresjury == null) membresjury = new List<ApplicationUser>();
            var ids = membresjury.Select(m => m.Id).ToHashSet();

            double somme = 0, count = 0;
            foreach (var item in allEvals)
            {
                if (item.EstSynthese || item.Echelle == null) continue;
                if (!ids.Contains(item.MembreId)) continue;
                somme += item.Echelle.Val;
                count++;
            }

            return count == 0 ? (double?)null : somme / count;
        }

        public async System.Threading.Tasks.Task CalculerNote(int StagId, int SessionId)
        {
            await CalculerNoteInterne(StagId, SessionId, true);
        }

        /// <summary>Note recalculee sans rien enregistrer (diagnostic de la page de reprise).</summary>
        public async System.Threading.Tasks.Task<double> CalculerNoteSimulation(int StagId, int SessionId)
        {
            return await CalculerNoteInterne(StagId, SessionId, false);
        }

        /// <summary>
        /// Calcul de la note d'un stagiaire, en deux niveaux (identique a EditStagiaire.razor.cs) :
        ///   Moy_rubrique = somme des syntheses evaluables / (nb de criteres de la rubrique * echelle max)
        ///   MG           = somme(coeff_rubrique * Moy_rubrique) / somme(coeff_rubrique)
        /// Un critere sans aucune note de jury compte pour 0 au numerateur mais garde son poids au
        /// denominateur : l'etudiant est penalise tant que le jury n'a pas tout saisi.
        /// </summary>
        private async System.Threading.Tasks.Task<double> CalculerNoteInterne(int StagId, int SessionId, bool enregistrer)
        {
            var stagiaire = Context.Stagiaires.FirstOrDefault(s => s.Id == StagId);
            if (stagiaire == null) return 0;

            var criteres = Context.Criteres.Where(r => r.Sessionid == SessionId).ToList();

            var Membresjury = (await Security.GetUsersInRoleAndEtab(new string[] { Constants.membre_jury, Constants.president_jury, Constants.expert }, stagiaire.Etabid))?.ToList() ?? new List<ApplicationUser>();
            Membresjury = Membresjury.GroupBy(m => m.Id).Select(g => g.First()).ToList();

            var AllEvals = Context.Evaluations
                .Include(ev => ev.Echelle)
                .Where(ev => ev.Stagid == stagiaire.Id)
                .ToList();

            // elementId -> rubriqueId, chargee explicitement : pas de lazy loading dans ce projet.
            var rubIdParElement = await Context.Elements
                .Where(el => el.Rubid != null)
                .Select(el => new { el.Id, el.Rubid })
                .ToListAsync();
            var rubIdParElementDict = rubIdParElement.ToDictionary(x => x.Id, x => (int?)x.Rubid);
            var rubIdParCritere = new Dictionary<int, int?>();
            foreach (var c in criteres)
            {
                rubIdParCritere[c.Id] = c.Elementid.HasValue && rubIdParElementDict.TryGetValue(c.Elementid.Value, out var rid) ? rid : null;
            }

            // Synthese de chaque critere. En simulation le resultat reste dans un dictionnaire,
            // aucune entite suivie n'est modifiee.
            var syntheseParCritere = new Dictionary<int, double?>();
            foreach (var item in criteres)
            {
                var synthese = AllEvals.FirstOrDefault(ev => ev.EstSynthese && ev.Criterid == item.Id);
                if (synthese == null) continue;
                double? valeur = MoyenneCritere(AllEvals.Where(ev => ev.Criterid == item.Id), Membresjury);
                syntheseParCritere[item.Id] = valeur;
                if (enregistrer) synthese.NoteSynthese = valeur;
            }

            var syntheses = criteres
                .Where(c => syntheseParCritere.ContainsKey(c.Id))
                .Select(c => new { c.Id, Valeur = syntheseParCritere[c.Id] })
                .ToList();

            var echelleMax = GetEchelleMax(Context, SessionId);

            if (syntheses.Count == 0 || echelleMax <= 0)
            {
                if (enregistrer)
                {
                    stagiaire.Note = 0;
                    stagiaire.NoteFinale = stagiaire.NoteCC / 2;
                    await Context.SaveChangesAsync();
                }
                return 0;
            }

            // Niveau 1 : moyenne de chaque rubrique. Le denominateur compte TOUS les criteres
            // de la rubrique, y compris ceux que le jury n'a pas notes.
            // Un critere dont l'element n'est rattache a aucune rubrique est ignore (comme dans EditStagiaire).
            var moyennesRubrique = syntheses
                .Where(s => rubIdParCritere.TryGetValue(s.Id, out var r0) && r0.HasValue)
                .GroupBy(s => rubIdParCritere[s.Id].Value)
                .Select(g =>
                {
                    var rubId = g.Key;
                    var nbCriteres = criteres.Count(c => rubIdParCritere.TryGetValue(c.Id, out var r2) && r2.HasValue && r2.Value == rubId);
                    var somme = g.Sum(s => s.Valeur ?? 0d);
                    var poidsMax = nbCriteres * echelleMax;
                    var coeff = Context.Rubriques.Where(r => r.Id == rubId).Select(r => (double?)r.Coeff).FirstOrDefault() ?? 0d;
                    return new
                    {
                        Moyenne = poidsMax > 0 ? somme / poidsMax : 0d,
                        Coeff = coeff
                    };
                })
                .ToList();

            double s = 0, cnt = 0;
            foreach (var item in moyennesRubrique)
            {
                var coeff = item.Coeff;
                if (coeff <= 0) coeff = 1; // coefficient nul : on neutralise plutot que de fausser la moyenne
                s += coeff * item.Moyenne;
                cnt += coeff;
            }

            // Niveau 2 : moyenne ponderee de la session.
            double mg = cnt > 0 ? s / cnt : 0;

            // Un cours hors ligne n'a pas de note de jury : Note = 0, NoteFinale repose sur NoteCC.
            double note = (stagiaire.CourEnligne == true) ? mg : 0;
            double noteFinale = (note + stagiaire.NoteCC) / 2;

            if (enregistrer)
            {
                stagiaire.Note = note;
                stagiaire.NoteFinale = noteFinale;
                await Context.SaveChangesAsync();
            }

            return note;
        }

        /// <summary>
        /// Supprime les evaluations en double : un membre de jury portant plusieurs des roles
        /// demandes etait enregistre une fois par role, ce qui comptait sa note plusieurs fois.
        /// Conserve la ligne portant une note si elle existe, sinon la plus ancienne.
        /// </summary>
        public async System.Threading.Tasks.Task<int> SupprimerDoublonsEvaluations(int SessionId)
        {
            var evals = Context.Evaluations
                .Where(ev => ev.EstSynthese == false && ev.Criterid != null)
                .Where(ev => Context.Criteres.Any(c => c.Id == ev.Criterid && c.Sessionid == SessionId))
                .ToList();

            var doublons = evals
                .GroupBy(ev => new { ev.Stagid, ev.Criterid, ev.MembreId })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g
                    .OrderByDescending(ev => ev.Echellid.HasValue)
                    .ThenBy(ev => ev.Id)
                    .Skip(1))
                .ToList();

            if (doublons.Count == 0) return 0;

            Context.Evaluations.RemoveRange(doublons);
            await Context.SaveChangesAsync();

            return doublons.Count;
        }

        /// <summary>
        /// Duplique l'echelle par defaut pour une nouvelle session, afin que chaque session
        /// dispose de sa propre echelle modifiable sans affecter les sessions precedentes.
        /// Ne fait rien si la session possede deja une echelle.
        /// </summary>
        public async System.Threading.Tasks.Task<int> CopyEchelleDefautPourSession(int sessionId)
        {
            if (sessionId <= 0) return 0;

            bool existe = await Context.Echelles.AnyAsync(e => e.Sessionid == sessionId);
            if (existe) return 0;

            var defauts = await Context.Echelles
                .Where(e => e.Sessionid == null)
                .OrderBy(e => e.Val)
                .ToListAsync();

            if (defauts.Count == 0) return 0;

            foreach (var d in defauts)
            {
                Context.Echelles.Add(new Pes.Models.DMdel.Echelle
                {
                    Id = d.Id,
                    Val = d.Val,
                    Sessionid = sessionId
                });
            }

            await Context.SaveChangesAsync();
            return defauts.Count;
        }

        /// <summary>
        /// Empeche la suppression d'une echelle encore utilisee par des evaluations.
        /// </summary>
        public async System.Threading.Tasks.Task<bool> SupprimerEchelleSiLibre(int idScale)
        {
            int utilisee = await Context.Evaluations.CountAsync(ev => ev.Echellid == idScale);
            if (utilisee > 0) return false;

            var echelle = await Context.Echelles.FirstOrDefaultAsync(e => e.IdScale == idScale);
            if (echelle == null) return false;

            Context.Echelles.Remove(echelle);
            await Context.SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task DeleteEvalsOfStagiaire(int id)
        {
            
                var evals = Context.Evaluations.Where(ev => ev.Stagid == id);
                Context.Evaluations.RemoveRange(evals);

                await DMContext.SaveChangesAsync();
            
        }


        public async System.Threading.Tasks.Task<int> CopyRubriques(int sourceSessionId, List<int> rubriqueIds, int targetSessionId)
        {
            int copied = 0;

            var sourceRubriques = await Context.Rubriques
                .Include(r => r.Elements).ThenInclude(e => e.Criteres)
                .Where(r => r.Sessionid == sourceSessionId && rubriqueIds.Contains(r.Id))
                .ToListAsync();

            foreach (var srcRubrique in sourceRubriques)
            {
                bool rubriqueExists = await Context.Rubriques.AnyAsync(r => r.Sessionid == targetSessionId && r.NomRubrique == srcRubrique.NomRubrique);
                if (rubriqueExists) continue;

                var newRubrique = new Rubrique { NomRubrique = srcRubrique.NomRubrique, Coeff = srcRubrique.Coeff, Sessionid = targetSessionId };
                Context.Rubriques.Add(newRubrique);
                await Context.SaveChangesAsync();

                if (srcRubrique.Elements == null) continue;

                foreach (var srcElement in srcRubrique.Elements)
                {
                    var newElement = new Pes.Models.DMdel.Element { NomElement = srcElement.NomElement, Rubid = newRubrique.Id, Sessionid = targetSessionId };
                    Context.Elements.Add(newElement);
                    await Context.SaveChangesAsync();

                    if (srcElement.Criteres == null) continue;

                    foreach (var srcCritere in srcElement.Criteres)
                    {
                        Context.Criteres.Add(new Critere { NomCritere = srcCritere.NomCritere, Elementid = newElement.Id, Sessionid = targetSessionId });
                    }
                }

                await Context.SaveChangesAsync();
                copied++;
            }

            return copied;
        }

        public async System.Threading.Tasks.Task<int> CopyElements(int sourceSessionId, List<int> elementIds, int targetSessionId)
        {
            int copied = 0;

            var sourceElements = await Context.Elements
                .Include(e => e.Rubrique)
                .Include(e => e.Criteres)
                .Where(e => e.Sessionid == sourceSessionId && elementIds.Contains(e.Id))
                .ToListAsync();

            foreach (var srcElement in sourceElements)
            {
                Rubrique targetRubrique = null;
                if (srcElement.Rubrique != null)
                {
                    targetRubrique = await Context.Rubriques.FirstOrDefaultAsync(r => r.Sessionid == targetSessionId && r.NomRubrique == srcElement.Rubrique.NomRubrique);
                    if (targetRubrique == null)
                    {
                        targetRubrique = new Rubrique { NomRubrique = srcElement.Rubrique.NomRubrique, Coeff = srcElement.Rubrique.Coeff, Sessionid = targetSessionId };
                        Context.Rubriques.Add(targetRubrique);
                        await Context.SaveChangesAsync();
                    }
                }

                bool elementExists = await Context.Elements.AnyAsync(e => e.Sessionid == targetSessionId && e.NomElement == srcElement.NomElement
                    && (srcElement.Rubrique == null || e.Rubid == targetRubrique.Id));
                if (elementExists) continue;

                var newElement = new Pes.Models.DMdel.Element { NomElement = srcElement.NomElement, Rubid = targetRubrique?.Id, Sessionid = targetSessionId };
                Context.Elements.Add(newElement);
                await Context.SaveChangesAsync();

                if (srcElement.Criteres != null)
                {
                    foreach (var srcCritere in srcElement.Criteres)
                    {
                        Context.Criteres.Add(new Critere { NomCritere = srcCritere.NomCritere, Elementid = newElement.Id, Sessionid = targetSessionId });
                    }
                }

                await Context.SaveChangesAsync();
                copied++;
            }

            return copied;
        }

        public async System.Threading.Tasks.Task SetRefAttestation(List<Stagiaire> lst, Session session)
        {
            if (session == null) return ;
            
            int i = Context.Stagiaires.Count(s=>s.Sessionid == session.Id) - lst.Count+1;
           
            foreach (var item in lst)
            {
                item.RefAttestation = session.CodeSession + "/" + i++.ToString("D4");
                while (Context.Stagiaires.Any(s => s.Sessionid == session.Id && s.RefAttestation == item.RefAttestation))
                    item.RefAttestation = session.CodeSession + "/" + i++.ToString("D4");

                Context.Stagiaires.Entry(item).State = EntityState.Modified;
            }


           await Context.SaveChangesAsync() ;

        }
    }
}
