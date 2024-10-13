using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Pes.Models.DMdel;
using System.Text.RegularExpressions;

namespace Pes.Pages
{
    public partial class StagiairesEvaluationsComponent
    {
        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected EmailSender EmailSender { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; }
        public int? SelectedEtab { get; set; } = null;

        public IList<Pes.Models.DMdel.Stagiaire> SelectedStagiares;

        public bool  SelectedStagiairesIsNullOrEmpty { get{ return SelectedStagiares == null || !SelectedStagiares.Any(); } set { } }

        protected async System.Threading.Tasks.Task LoadStagiaires()
        {
            try
            {


                if (Globals.ActiveSession == null)
                {
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "Pas de session active." });
                    return;
                }

                if (Security.IsInRole(Constants.coordinateur, Constants.president_jury, Constants.membre_jury, Constants.expert))
                {
                    SelectedEtab = Security.User.Etabid;
                    var res = await DMdel.GetStagiaires(new Query() { Filter = $@"s=>s.Etabid == {Security.User.Etabid} && s.Sessionid == {Globals.ActiveSession?.Id}", Expand = "Etablissement" });
                    getStagiairesResult = res.OrderBy(s => s.Nom); ;
                }
                else //if (Security.IsInRole(Constants.admin))
                {
                    if (SelectedEtab == null)
                    {
                        var res = await DMdel.GetStagiaires(new Query() { Filter = $@"s=>s.Sessionid == {Globals.ActiveSession?.Id} ", Expand = "Etablissement" });
                        getStagiairesResult = res.OrderBy(s => s.Nom);
                    }
                    else
                    {
                        var res = await DMdel.GetStagiaires(new Query() { Filter = $@"s=>s.Sessionid == {Globals.ActiveSession?.Id}&& s.Etabid == {SelectedEtab}", Expand = "Etablissement" });
                        getStagiairesResult = res.Where(s => s.Etabid == SelectedEtab).OrderBy(s => s.Nom);
                    }
                }
            }
            catch (System.Exception exception)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = exception.Message });
            }


        }


        protected async System.Threading.Tasks.Task GetAttestation(int stagid)
        {
            var uri = $"Document/Attestation?stagid={stagid}";
            UriHelper.NavigateTo(uri, true);


        }


        protected async System.Threading.Tasks.Task GetAttestationsList()
        {
            var stagwin = SelectedStagiares.Where(s => s.NoteFinale >= 0.5);

            if (!stagwin.Any())
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "Il faut d'abord sélectionner des stagiaires dont la moyennes >= 50% " });
                return;
            }


            string queryString = string.Join("-", stagwin.Select(x => x.Id));

            var uri = $"Document/AttestationsList(ids='{(UrlEncoder.Default.Encode(queryString))}')";

            UriHelper.NavigateTo(uri, true);


        }

        protected async System.Threading.Tasks.Task SendAttestationsList()
        {
            try
            {
                var stagwin = SelectedStagiares.Where(s => s.NoteFinale >= 0.5);

                if (!stagwin.Any())
                {
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "Il faut d'abord sélectionner des stagiaires dont la moyennes >= 50% " });
                    return;
                }




                string queryString = string.Join(";", stagwin.Select(x => x.Id));



                var res = await EmailSender.SendAttestationsList(queryString);

                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Info, Summary = $"Erreur", Detail = res });
            }
            catch (Exception)
            {

                
            }
        }


        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Define a regex pattern for a valid email address
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

                // Return true if the email matches the pattern
                return regex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                // In case of a timeout while matching the regex
                return false;
            }
        }




        protected async System.Threading.Tasks.Task ChangeSelectionStatus(bool args)
        {
            if (args) SelectedStagiares = getStagiairesResult.ToList();
            else SelectedStagiares = null;
        }

        protected async System.Threading.Tasks.Task ExecuteAction(string act)
        {
            try
            {

               
                    if (SelectedStagiairesIsNullOrEmpty)
                    {
                        NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "Il faut d'abord sélectionner des stagiaires dont la moyennes >= 50% " });
                        return;
                    }
               
                switch (act)
                {
                    case "0":

                        await GetAttestationsList();

                        break;

                        case "1":
                        await ExportToExcel();
                        break;

                    case "2":
                        await SendAttestationsList();
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception exception)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = exception.Message });
            }

        }

        protected async System.Threading.Tasks.Task ExportToExcel()
        {
            try
            {


                Query query;


                if (Globals.ActiveSession?.Id != null)
                {
                    if (SelectedEtab != null)
                    {
                        query = new Query() { Filter = $@"s=>s.Etabid == {SelectedEtab} && s.Sessionid == {Globals.ActiveSession?.Id}", Expand = "Etablissement,Faculte,Departement", Select = "Nom,Prenom as Prénom,Etablissement.NomEtablissement as Établissement,Faculte.NomFaculte as Faculté,Departement.NomDepartement as Département,URLcour as URL_Cours,NoteCC as Note_Évaluation_Continue,Note as Note_Cours ,NoteFinale as Note_Finale" };
                    }
                    else
                    {
                        query = new Query() { Filter = $@"s=>s.Sessionid == {Globals.ActiveSession?.Id}", Expand = "Etablissement,Faculte,Departement", Select = "Nom,Prenom as Prénom,Etablissement.NomEtablissement as Établissement,Faculte.NomFaculte as Faculté,Departement.NomDepartement as Département,URLcour as URL_Cours,NoteCC as Note_Évaluation_Continue,Note as Note_Cours ,NoteFinale as Note_Finale" };

                    }

                    await DMdel.ExportStagiairesToExcel(query, $"listestagiaires.xlsx");
                }



            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = ex.Message });
            }
        }

        protected async System.Threading.Tasks.Task AttestatioAction(string act, Stagiaire data)
        {
            try
            {

                switch (act)
                {
                    

                    case "0":
                        await GetAttestation(data.Id);
                        break;

                   
                    
                    case "1":

                        await SendAttestation(data.Id);

                        break; 
                    
                    default:
                        break;
                }


            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = ex.Message });
            }
        }

        protected async System.Threading.Tasks.Task SendAttestation(int stagid)
        {
            try
            {
                var stg = await DMdel.GetStagiaireById(stagid);
                if (stg == null) return;
                if (!IsValidEmail(stg.email))
                {
                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "Addresse e-mail invalide: " + stg.email });

                    return;
                }

                var res = await EmailSender.SendEmailWithAttachment(stagid);

                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = res, Detail = "" });
            }
            catch (Exception)
            {

                
            }

        }
    }
}
