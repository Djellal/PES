using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;

namespace Pes.Pages
{
    public partial class EditApplicationUserComponent
    {
        public bool RegionEtabVisible
        {
            get
            {

                if (user == null) return false;
                if (user.RoleNames == null) return false;


                if ((user.RoleNames.Contains(Constants.coordinateur) || user.RoleNames.Contains(Constants.membre_jury) || user.RoleNames.Contains(Constants.expert) || user.RoleNames.Contains(Constants.president_jury))

                && Security.IsInRole(Constants.admin)) return true;
                else return false;

            }
        }

        public bool FacDepVisible
        {
            get
            {

                if (user == null) return false;
                if (user.RoleNames == null) return false;


                if ((user.RoleNames.Contains(Constants.membre_jury) || user.RoleNames.Contains(Constants.expert) || user.RoleNames.Contains(Constants.president_jury))
                    && Security.IsInRole(Constants.admin)) return true;
                else return false;

            }
        }

        public bool QualiteVisible
        {
            get
            {

                if (user == null) return false;
                if (user.RoleNames == null) return false;


                if ((user.RoleNames.Contains(Constants.membre_jury) || user.RoleNames.Contains(Constants.president_jury))
                    && Security.IsInRole(Constants.admin)) return true;
                else return false;

            }
        }


        protected async System.Threading.Tasks.Task ApplyRestrictions()
        {
            if (Security.User.Name == "admin")
            {
                return;
            }
            if (Security.IsInRole(Constants.coordinateur))
            {
                roles = roles.Where(r => r.Name != Constants.coordinateur && r.Name != Constants.admin);
                getEtablissementsResult = getEtablissementsResult.Where(e => e.Id == Security.User.Etabid);

                user.Etabid = Security.User.Etabid;

            }
            


        }
    }
}
