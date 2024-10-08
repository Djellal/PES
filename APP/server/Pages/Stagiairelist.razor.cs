using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using Pes.Models.DMdel;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Bibliography;

namespace Pes.Pages
{
    public partial class StagiairelistComponent
    {
        public string uploadUrl { get; set; } = "";
        public int? etabid { get; set; } = 0;

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; }

        public IList<Pes.Models.DMdel.Stagiaire> SelectedStagiares;

        public bool SelectedStagiairesIsNullOrEmpty { get { return SelectedStagiares == null || !SelectedStagiares.Any(); } set { } }

        protected async System.Threading.Tasks.Task LoadStagiaires()
        {
            try
            {


                if (Security.IsInRole(new String[] { Constants.coordinateur }))
                {
                    getEtablissementsResult = await DMdel.GetEtablissements(new Query() { Filter = $@"s=>s.Id == {Security.User.Etabid}" });
                    etabid = Security.User.Etabid;
                }

                if (Security.IsInRole(new String[] { Constants.admin }))
                {
                    getEtablissementsResult = await DMdel.GetEtablissements();

                    etabid = SelectedEtab;
                }


                if (Globals.ActiveSession?.Id != null)
                {


                    if (etabid != null)
                    {
                        getFacultesResult = await DMdel.GetFacultes(new Query() { Filter = $@"et=>et.Etabid == {etabid}" });
                        getStagiairesResult = await DMdel.GetStagiaires(new Query() { Filter = $@"s=>s.Etabid == {etabid} && s.Sessionid == {Globals.ActiveSession?.Id}", Expand = "Etablissement" });
                    }
                    else
                    {
                        getFacultesResult = await DMdel.GetFacultes();
                        getStagiairesResult = await DMdel.GetStagiaires(new Query() { Filter = $@"s => s.Sessionid == {Globals.ActiveSession?.Id}", Expand = "Etablissement" });

                    }


                    await GiveRefAttestation();
                }
                else
                    getStagiairesResult = new List<Stagiaire>();



                uploadUrl = $"import/stagiaires/{etabid}/{Globals.ActiveSession?.Id}";


                if (grid0 != null) await grid0.Reload();


               
            }
            catch (Exception ex)
            {

                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = $"{ex.Message}" });
            }

        }

        protected async System.Threading.Tasks.Task PrepareNewStagiaire()
        {
            try
            {
                Stagiaire s = new Stagiaire();

                s.Sessionid = Globals.ActiveSession?.Id;
                s.email = "";
                s.portefolio = "";
                s.URLcour = "";
                s.CourEnligne = false;

                if (Security.IsInRole(Constants.coordinateur))
                {
                    s.Etabid = Security.User.Etabid;
                }

                await this.grid0.InsertRow(s);
            }
            catch (Exception ex)
            {

                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = $"{ex.Message}" });
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
                switch (act)
                {
                    case "0":
                        await PrepareNewStagiaire();
                        break;

                    case "1":
                        await ExportToExcel();
                        break;

                    case "2":
                        await DeleteSelectedStagiaires();
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

        private async Task DeleteSelectedStagiaires()
        {
            var resp = await DialogService.Confirm("La suppression est définitive et il n'y aura pas de possibilité de les récupérer.", "Voulez-vous vraiment supprimer la liste des stagiaires ?", new ConfirmOptions() { OkButtonText = "Oui", CancelButtonText = "Non" });
            if (resp == null) { return; }
            if ((bool)!resp) { return; }

            foreach (var item in SelectedStagiares)
            {
                await DMdel.DeleteEvalsOfStagiaire(item.Id);
                await DMdel.DeleteStagiaire(item.Id);
            }

            NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = $"Suppression", Detail = SelectedStagiares.Count + " stagiaire(s) supprimée(s) avec succès" });


            await Load();

        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClicked(int Id)
        {
            try
            {
                if (await DialogService.Confirm("La suppression est définitive et il n'y aura pas de possibilité de le récupérer.", "Voulez-vous vraiment supprimer ce stagiaire ?", new ConfirmOptions() { OkButtonText = "Oui", CancelButtonText = "Non" }) == true)
                {
                    await DMdel.DeleteEvalsOfStagiaire(Id);
                    var dMdelDeleteStagiaireResult = await DMdel.DeleteStagiaire(Id);
                    if (dMdelDeleteStagiaireResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteStagiaireException)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = $"Erreur lors de la suppression du Stagiaire" });
            }
        }


        protected async System.Threading.Tasks.Task ExportToExcel()
        {
            try
            {
                Query query;


                if (Globals.ActiveSession?.Id != null)
                {
                    if (etabid != null)
                    {
                        query = new Query() { Filter = $@"s=>s.Etabid == {etabid} && s.Sessionid == {Globals.ActiveSession?.Id}", Expand = "Etablissement,Faculte,Departement", Select = "Nom,Prenom,Etablissement.NomEtablissement,Faculte.NomFaculte,Departement.NomDepartement,URLcour,NoteCC,Note ,NoteFinale" };
                    }
                    else
                    {
                        query = new Query() { Filter = $@"s=>s.Sessionid == {Globals.ActiveSession?.Id}", Expand = "Etablissement,Faculte,Departement", Select = "Nom,Prenom,Etablissement.NomEtablissement,Faculte.NomFaculte,Departement.NomDepartement,URLcour,NoteCC,Note as Note_evalution,NoteFinale" };

                    }

                    await DMdel.ExportStagiairesToExcel(query, $"listestagiaires.xlsx");
                }



            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = ex.Message });
            }
        }
        protected async System.Threading.Tasks.Task GiveRefAttestation()
        {

            try
            {
                var lst = DMdel.DMContext.Stagiaires.Where( s => s.Sessionid ==Globals.ActiveSession.Id &&  String.IsNullOrEmpty(s.RefAttestation.Trim())).ToList();

                await DMdel.SetRefAttestation(lst, Globals.ActiveSession);
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = ex.Message });
            }



        }

    }
}
