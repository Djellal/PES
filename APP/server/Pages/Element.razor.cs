using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Pes.Pages
{
    public partial class ElementComponent
    {
        protected async System.Threading.Tasks.Task ImportButtonClick(MouseEventArgs args)
        {
            if (Globals.ActiveSession == null)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Warning, Summary = $"Attention", Detail = $"Aucune session active. Veuillez activer une session avant d'importer des éléments." });
                return;
            }

            var dialogResult = await DialogService.OpenAsync<ImportElements>("Importer éléments & critères", null);
            if (dialogResult == true)
            {
                await this.Load();
                await this.grid0.Reload();

                if (master != null)
                {
                    Criteres = await DMdel.GetCriteres(new Query() { Filter = $@"i => i.Elementid == {master.Id}" });
                }
            }
        }
    }
}
