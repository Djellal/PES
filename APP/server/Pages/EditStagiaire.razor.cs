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

        protected async System.Threading.Tasks.Task LoadEvalutions()
        {
            try
            {
                Membresjury = await Security.GetUsersInRoleAndEtab(new string[] { Constants.membre_jury, Constants.president_jury, Constants.expert }, stagiaire.Etabid);

                if (Security.IsInRole(new String[] { Constants.expert,Constants.membre_jury, Constants.president_jury }))
                {
                    Evals = DMdel.DMContext.Evaluations.Include(ev => ev.Critere).Where(ev => ev.Stagid == stagiaire.Id && !ev.EstSynthese && ev.MembreId == Security.User.Id).OrderBy(ev => ev.NomRubrique).ToList();
                }
                else if (Security.IsInRole(new String[] {  Constants.coordinateur,Constants.admin }))
                {
                    Evals = DMdel.DMContext.Evaluations.Include(ev => ev.Critere).Where(ev => ev.Stagid == stagiaire.Id && ev.EstSynthese).OrderBy(ev => ev.NomRubrique).ToList();
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
                var AllEvals = DMdel.DMContext.Evaluations.Include(ev => ev.Echelle).Where(ev => ev.Stagid == stagiaire.Id && ev.Criterid == criterid).ToList();
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
                    synthese.NoteSynthese = somme / count;
                    await DMdel.UpdateEvaluation(synthese.Id, synthese);

                }


              
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "CalculerMoyeneCritere : \r\n" + ex.Message });
            }
        }

        protected async System.Threading.Tasks.Task CalculerNote()
        {
            
            try
            {
               var rubriques  = DMdel.DMContext.Rubriques.Where(r=>r.Sessionid == Globals.ActiveSession.Id).ToList();
                var AllSyntheseEvals = DMdel.DMContext.Evaluations.Include(ev=>ev.Echelle).Include(ev => ev.Critere).ThenInclude(c=>c.Element).ThenInclude(ec => ec.Rubrique).Where(ev => ev.Stagid == stagiaire.Id && ev.EstSynthese).ToList();

                

                if (AllSyntheseEvals.Count() == 0) { stagiaire.Note = 0; return; }

                double s = 0, cnt = 0;
                foreach (var item in AllSyntheseEvals)
                {
                    cnt+= (double)item.Critere?.Element?.Rubrique?.Coeff;

                    if (!Double.IsNaN(item.NoteSynthese))
                        s += (double)(item.NoteSynthese * item.Critere?.Element?.Rubrique?.Coeff);
                    else
                        s += 0;
                }

                 
               

                if (stagiaire.CourEnligne != null)
                if ((bool)stagiaire.CourEnligne && cnt != 0) 
                        stagiaire.Note = s / (cnt * 5);
                else stagiaire.Note = 0;

                stagiaire.NoteFinale = ((stagiaire.Note + stagiaire.NoteCC) / 2);

                await DMdel.UpdateStagiaire(stagiaire.Id, stagiaire);
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

        }
}
