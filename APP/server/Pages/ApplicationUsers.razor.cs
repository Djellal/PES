using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using Pes.Models;

namespace Pes.Pages
{
    public partial class ApplicationUsersComponent
    {
        protected async System.Threading.Tasks.Task LoadUsers()
        {
            try
            {
                if (Security.IsInRole(new string[] { Constants.admin }))
                {
                   
                        users = (await Security.GetUsersOfEtab(SelectedEtab,SelectedRole)).ToList();

                    
                }
                else if (Security.IsInRole(new string[] { Constants.coordinateur }))
                {
                    users = (await Security.GetUsersOfEtab(Security.User.Etabid, SelectedRole)).ToList();
                }

                await AttribuerRoles(users);

            }
            catch (Exception ex)
            {

                    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "LoadUsers : \r\n" + ex.Message });
            }


        }

        private async Task AttribuerRoles(IEnumerable<ApplicationUser> users)
        {
            try
            {
                foreach (var u in users)
                {
                    u.RoleNames = await Security.GetRolesOfUser(u);

                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "LoadUsers : \r\n" + ex.Message });


            }
        }
        protected string GetEtabName(int? etabId)
        {
            return getEtablissementsResult.FirstOrDefault(e=>e.Id == etabId)?.NomEtablissement;
        }

        protected async System.Threading.Tasks.Task ExportUsersToExcel()
        {
            // Navigate to the export URL
            UriHelper.NavigateTo($"/export/applicationusers/excel?roles={SelectedRole}&etabid={SelectedEtab}", forceLoad: true);
        }
    }
}
