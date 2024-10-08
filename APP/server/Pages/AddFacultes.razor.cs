using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;

namespace Pes.Pages
{
    public partial class AddFacultesComponent
    {
        protected async System.Threading.Tasks.Task LoadEtablissement()
        {
            

            if (Security.IsInRole(new String[] { Constants.coordinateur }))
            {
                
                getEtablissementsForEtabidResult = await DMdel.GetEtablissements(new Query() { Filter = $@"f=>f.Id == {Security.User.Etabid}" });

            }

            if (Security.IsInRole(new String[] { Constants.admin }))
            {
                getEtablissementsForEtabidResult = await DMdel.GetEtablissements();
                
            }


        }
    }
}
