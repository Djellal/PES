using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Pes.Models.DMdel;
using DocumentFormat.OpenXml.Drawing;
using Pes.Models;
using NPOI.SS.Formula.Functions;

namespace Pes.Pages
{
    public partial class EditStagiaireComponent
    {

        public IEnumerable<Pes.Models.DMdel.Evaluation> Evals { get; set; }
        public IEnumerable<ApplicationUser> Membresjury { get; private set; }

        /// <summary>Valeur maximale de l'echelle de la session en cours.</summary>
        public double EchelleMax { get; set; }
        protected async System.Threading.Tasks.Task LoadEvalutions()
        {
            try
            {
                Membresjury = (await Security.GetUsersInRoleAndEtab(new string[] { Constants.membre_jury, Constants.president_jury, Constants.expert }, stagiaire.Etabid))?.ToList() ?? new List<ApplicationUser>();
                Membresjury = Membresjury.GroupBy(m => m.Id).Select(g => g.First()).ToList();

                if (Security.IsInRole(new String[] { Constants.expert,Constants.membre_jury, Constants.president_jury }))
                {
                    Evals = DMdel.DMContext.Evaluations.Include(ev => ev.Critere).Where(ev => ev.Stagid == stagiaire.Id && !ev.EstSynthese && ev.MembreId == Security.User.Id).OrderBy(ev => ev.NomRubrique).ThenBy(c => c.Critere.NomCritere).ToList();
                }
                else if (Security.IsInRole(new String[] {  Constants.coordinateur,Constants.vice_recteur,Constants.admin,Constants.admin_regional }))
                {
                    Evals = DMdel.DMContext.Evaluations.Include(ev => ev.Critere).Where(ev => ev.Stagid == stagiaire.Id && ev.EstSynthese).OrderBy(ev => ev.NomRubrique).ThenBy(c=>c.Critere.NomCritere).ToList();
                }


            }
            catch (Exception ex)
            {

                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "LoadEvalutions : \r\n" + ex.Message });
            }
        }

        protected async System.Threading.Tasks.Task CalculerMoyeneCritere(int criterid)
        {
            try
            {
                if (Membresjury == null) Membresjury = new List<ApplicationUser>();

                var AllEvals = DMdel.DMContext.Evaluations.Include(ev => ev.Echelle).Where(ev => ev.Stagid == stagiaire.Id && ev.Criterid == criterid).OrderBy(ev => ev.NomRubrique).ThenBy(c => c.Critere.NomCritere).ToList();
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

                var synthese = AllEvals.Find(s => s.EstSynthese);

                if (synthese != null)
                {
                    // Aucun membre du jury n'a note ce critere -> NULL (« non evalue »), jamais NaN.
                    synthese.NoteSynthese = count == 0 ? (double?)null : somme / count;
                    await DMdel.UpdateEvaluation(synthese.Id, synthese);

                }



            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "CalculerMoyeneCritere : \r\n" + ex.Message });
            }
        }

        /// <summary>
        /// Calcul de la note du stagiaire, en deux niveaux (identique a DMdelService.CalculerNote) :
        ///   Moy_rubrique = somme des syntheses evaluables / (nb de criteres de la rubrique * echelle max)
        ///   MG           = somme(coeff_rubrique * Moy_rubrique) / somme(coeff_rubrique)
        /// Un critere sans aucune note de jury compte pour 0 au numerateur mais garde son poids au
        /// denominateur : l'etudiant est penalise tant que le jury n'a pas tout saisi.
        /// </summary>
        protected async System.Threading.Tasks.Task CalculerNote()
        {

            try
            {
                int sessionId = Globals.ActiveSession.Id;

                var criteres = DMdel.DMContext.Criteres.Include(c => c.Element).ThenInclude(e => e.Rubrique)
                    .Where(c => c.Sessionid == sessionId).ToList();

                var AllSyntheseEvals = DMdel.DMContext.Evaluations
                    .Include(ev => ev.Critere).ThenInclude(c => c.Element).ThenInclude(e => e.Rubrique)
                    .Where(ev => ev.Stagid == stagiaire.Id && ev.EstSynthese)
                    .ToList();

                var echelleMax = DMdelService.GetEchelleMax(DMdel.DMContext, sessionId);

                if (AllSyntheseEvals.Count == 0 || echelleMax <= 0)
                {
                    stagiaire.Note = 0;
                    stagiaire.NoteFinale = stagiaire.NoteCC / 2;
                    return;
                }

                // Niveau 1 : moyenne de chaque rubrique. Le denominateur compte TOUS les criteres
                // de la rubrique, y compris ceux que le jury n'a pas notes.
                var moyennesRubrique = AllSyntheseEvals
                    .Where(ev => ev.Critere?.Element?.Rubrique != null)
                    .GroupBy(ev => ev.Critere.Element.Rubrique.Id)
                    .Select(g =>
                    {
                        var nbCriteres = criteres.Count(c => c.Element?.Rubid == g.Key);
                        var somme = g.Sum(ev => ev.NoteSynthese ?? 0d);
                        var poidsMax = nbCriteres * echelleMax;
                        return new
                        {
                            Coeff = g.First().Critere.Element.Rubrique.Coeff,
                            Moyenne = poidsMax > 0 ? somme / poidsMax : 0d
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
                stagiaire.Note = (stagiaire.CourEnligne == true) ? mg : 0;
                stagiaire.NoteFinale = (stagiaire.Note + stagiaire.NoteCC) / 2;

                //await DMdel.UpdateStagiaire(stagiaire.Id, stagiaire);
            }
            catch (Exception ex)
            {
               // throw ex;
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "CalculerNote : \r\n"+ex.Message });
            }

        }

        protected async System.Threading.Tasks.Task Calculer()
        {
            try
            {
                var lst = getCriteresResult.ToList();

                foreach (var item in lst)
                {
                   await CalculerMoyeneCritere(item.Id);
                }


               await CalculerNote();
            }
            catch (Exception ex)
            {
                 //throw ex;
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "CalculerNote : \r\n" + ex.Message });
            }
        }

        protected async System.Threading.Tasks.Task DetailbuttonClick()
        {
            await DialogService.OpenAsync<StagiaireEvaluationsDetail>(
                $"Détails évaluations",
                new Dictionary<string, object>() { { "Id", stagiaire.Id } },
                new DialogOptions() { Width = "1300px", CloseDialogOnOverlayClick = true, Resizable = true, Draggable = true });
        }

        protected async System.Threading.Tasks.Task Reinit()
        {
            try
            {

                await DMdel.ReintEvals(stagiaire.Id, Globals.ActiveSession.Id);
                await Load();
                await Calculer();
            }
            catch (System.Exception calculerNoteException)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = $"{calculerNoteException.Message}" });
            }
        }

        }
}
