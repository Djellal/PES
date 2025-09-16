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
                    if (SelectedEtab != null)
                    {
                        users = (await Security.GetUsersOfEtab(SelectedEtab)).ToList();

                    }
                    else
                    {
                        users = (await Security.GetUsers()).ToList();
                    }
                }
                else if (Security.IsInRole(new string[] { Constants.coordinateur }))
                {
                    users = (await Security.GetUsersOfEtab(Security.User.Etabid)).ToList();
                }

                await AttribuerRoles(users);

            }
            catch (Exception ex)
            {

                throw ex;
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
            catch (Exception)
            {


            }
        }
        protected string GetEtabName(int? etabId)
        {
            return getEtablissementsResult.FirstOrDefault(e=>e.Id == etabId)?.NomEtablissement;
        }

        protected async System.Threading.Tasks.Task ExportUsersToExcel()
        {
            // Navigate to the export URL
            UriHelper.NavigateTo("/export/applicationusers/excel(fileName='Utilisateurs')", forceLoad: true);
        }
    }
}
