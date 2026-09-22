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
            var stagiaire = await GetStagiaireById(StagId);


            var Evals = Context.Evaluations.Where(ev => ev.Stagid == StagId);

            Context.Evaluations.RemoveRange(Evals);

            await  Context.SaveChangesAsync();

            await CreateEvals(StagId, SessionId);
        }

        public async System.Threading.Tasks.Task CreateEvals(int StagId,int SessionId)
        {

            var stagiaire = await GetStagiaireById(StagId);


            var criteres = Context.Criteres.Where(c=>c.Sessionid == SessionId).Include(ev => ev.Element).ThenInclude(e => e.Rubrique).OrderBy(c=>c.NomCritere).ToList(); //(await DMdel. GetCriteres()).ToList();

            var membresjury = await Security.GetUsersInRoleAndEtab(new string[] { Constants.expert, Constants.membre_jury, Constants.president_jury }, stagiaire.Etabid);


            string NomElem, NomRubrique;

            List<Evaluation> evaluations = new List<Evaluation>();

            var Evals = Context.Evaluations.Where(ev => ev.Stagid == StagId).ToList();

            foreach (var critr in criteres)
            {

                NomElem = critr.Element?.NomElement;
                NomRubrique = critr.Element?.Rubrique?.NomRubrique;

                if (!Evals.Any(v => v.Criterid == critr.Id && v.EstSynthese))
                {
                    evaluations.Add(new Models.DMdel.Evaluation { Criterid = critr.Id, Stagid = StagId, MembreId = string.Empty, NomElement = NomElem, NomRubrique = NomRubrique, EstSynthese = true });

                }

                foreach (var membre in membresjury)
                {
                    if (!Evals.Any(v => v.Criterid == critr.Id && v.MembreId == membre.Id && !v.EstSynthese))
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
                synthese.NoteSynthese = somme / count;
                await Context.SaveChangesAsync();

            }

        }

        public async System.Threading.Tasks.Task CalculerNote(int StagId, int SessionId)
        {
           
                var stagiaire = Context.Stagiaires.FirstOrDefault(s=>s.Id == StagId);
                if (stagiaire == null) return;

                var criteres = Context.Criteres.Where(r=>r.Sessionid == SessionId).ToList();

               var rubriques = Context.Rubriques.Where(r => r.Sessionid == SessionId).ToList();



                var Membresjury = (await Security.GetUsersInRoleAndEtab(new string[] { Constants.membre_jury, Constants.president_jury, Constants.expert }, stagiaire.Etabid)).ToList();

                foreach ( var item in criteres)
                {
                    await CalculerMoyeneCritere(stagiaire.Id, item.Id, Membresjury);
                }

                var AllSyntheseEvals = Context.Evaluations.Include(ev => ev.Echelle).Include(ev => ev.Critere).Where(ev => ev.Stagid == stagiaire.Id && ev.EstSynthese).ToList();


                if (AllSyntheseEvals.Count() == 0) { stagiaire.Note = 0; return; }

                double s = 0, cnt = 0;
                foreach (var item in AllSyntheseEvals)
                {
                    cnt++;
                    if (!Double.IsNaN(item.NoteSynthese))
                        s += item.NoteSynthese;
                    else
                        s += 0;
                }

                if (stagiaire.CourEnligne != null)
                    if ((bool)stagiaire.CourEnligne && cnt != 0)
                        stagiaire.Note = s / (cnt * 5);
                    else stagiaire.Note = 0;

                stagiaire.NoteFinale = ((stagiaire.Note + stagiaire.NoteCC) / 2);



            await Context.SaveChangesAsync();           
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
