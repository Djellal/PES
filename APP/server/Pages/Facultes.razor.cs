using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Radzen;
using Radzen.Blazor;
using Pes.Models.DMdel;

namespace Pes.Pages
{
    public partial class FacultesComponent
    {
        public string uploadUrl { get; set; } = "";

        private int? GetImportEtabid()
        {
            if (Security.IsInRole(Constants.coordinateur)) return Security.User.Etabid;
            return SelectedEtab;
        }

        protected async System.Threading.Tasks.Task Upload0Complete(UploadCompleteEventArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args.RawResponse))
                {
                    using var doc = JsonDocument.Parse(args.RawResponse);
                    var root = doc.RootElement;
                    int facs = root.GetProperty("facs").GetInt32();
                    int depts = root.GetProperty("depts").GetInt32();
                    int skipped = root.GetProperty("skipped").GetInt32();

                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Success, Summary = "Import terminé", Detail = $"{facs} faculté(s) et {depts} département(s) importé(s). {skipped} doublon(s) ignoré(s)." });
                }

                await LoadFacultes();
                await grid0.Reload();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = "Erreur", Detail = ex.Message });
            }
        }

        protected async System.Threading.Tasks.Task Upload0Error(UploadErrorEventArgs args)
        {
            if (!Security.IsInRole(Constants.coordinateur) && SelectedEtab == null)
            {
                await DialogService.Alert("Il faut séléctionner l'établissement");
            }
            else
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = "Erreur", Detail = args.Message });
            }
        }

        protected async System.Threading.Tasks.Task LoadFacultes()
        {
            string filter = null;

            if (Security.IsInRole(Constants.admin))
            {
                getEtablissementsResult = await DMdel.GetEtablissements();
                if (SelectedEtab != null) filter = $@"f=>f.Etabid == {SelectedEtab}";
            }
            else if (Security.IsInRole(Constants.admin_regional))
            {
                var regionEtabs = await DMdel.GetEtablissements(new Query() { Filter = $@"e=>e.Regid == {Security.User.Regid}" });
                getEtablissementsResult = regionEtabs != null ? regionEtabs.ToList() : new List<Etablissement>();

                if (SelectedEtab != null)
                {
                    filter = $@"f=>f.Etabid == {SelectedEtab}";
                }
                else if (regionEtabs != null && regionEtabs.Any())
                {
                    var regionFilter = string.Join(" || ", regionEtabs.Select(e => $"f.Etabid == {e.Id}"));
                    filter = $@"f=>{regionFilter}";
                }
            }
            else if (Security.IsInRole(Constants.coordinateur))
            {
                filter = $@"f=>f.Etabid == {Security.User.Etabid}";
            }

            getFacultesResult = filter != null
                ? await DMdel.GetFacultes(new Query() { Expand = "Etablissement", Filter = filter })
                : await DMdel.GetFacultes(new Query() { Expand = "Etablissement" });

            uploadUrl = GetImportEtabid() != null ? $"import/facultes/{GetImportEtabid()}" : "";
        }
    }
}
