using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using Pes.Models.DMdel;

namespace Pes.Pages
{
    public partial class FacultesComponent
    {
        protected async System.Threading.Tasks.Task LoadFacultes()
        {
            int? etabid = 0;

            if (Security.IsInRole(new String[] { Constants.coordinateur }))
            {
                getFacultesResult = await DMdel.GetFacultes(new Query() { Expand = "Etablissement",Filter = $@"f=>f.Etabid == {Security.User.Etabid}" });
                etabid = Security.User.Etabid;

            }

            if (Security.IsInRole(new String[] { Constants.admin }))
            {
                getFacultesResult = await DMdel.GetFacultes(new Query() { Expand = "Etablissement"});

                etabid = SelectedEtab;
            }            


        }
    }
}
