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

            if (Security.IsInRole(new String[] { Constants.admin_regional }))
            {
                var regionEtabs = await DMdel.GetEtablissements(new Query() { Filter = $@"e=>e.Regid == {Security.User.Regid}" });
                getEtablissementsResult = regionEtabs;

                if (regionEtabs != null && regionEtabs.Any())
                {
                    var regionFilter = string.Join(" || ", regionEtabs.Select(e => $"f.Etabid == {e.Id}"));
                    getFacultesResult = await DMdel.GetFacultes(new Query() { Expand = "Etablissement", Filter = $@"f=>{regionFilter}" });
                }
                else
                {
                    getFacultesResult = new List<Faculte>();
                }

                etabid = SelectedEtab;
            }            


        }
    }
}
