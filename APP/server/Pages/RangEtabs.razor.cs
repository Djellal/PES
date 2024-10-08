using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using Pes.Models.DMdel;

namespace Pes.Pages
{
    public partial class RangEtabsComponent
    {
        

        protected async System.Threading.Tasks.Task GenerateRangs(int sessionid)
        {



            try
            {
                RangsEtab retab;
                var RangsEtabList = DMdel.DMContext.RangsEtabs.Where(r => r.Sessionid == sessionid).ToList();

                foreach (var et in getEtablissementsResult.ToList())
                {
                    retab = RangsEtabList.FirstOrDefault(r => r.Etabid == et.Id);

                    if (retab == null)
                    {
                        retab = new RangsEtab { Etabid = et.Id, Sessionid = sessionid };
                        await CalculerMoy(retab);
                        RangsEtabList.Add(retab);

                        await DMdel.CreateRangsEtab(retab);

                    }
                    else
                    {
                        await CalculerMoy(retab);
                        await DMdel.UpdateRangsEtab(retab.Id, retab);
                    }
                }
                RangsEtabList = RangsEtabList.OrderByDescending(r => r.Moyenne).ToList();
                int rang = 1;
                int counter = 1;
                double prevMov = 0;
                foreach (var item in RangsEtabList)
                {
                    if (item.Moyenne != prevMov) rang = counter;
                   
                    item.Rang = rang;
                   

                    prevMov = item.Moyenne;
                    counter++;

                }
                
                getRangsEtabsResult = RangsEtabList;
            }
            catch (Exception ex)
            {

                //throw ex;
            }
        }

        private async Task CalculerMoy(RangsEtab rangsetab)
        {
            var stags = DMdel.DMContext.Stagiaires.Where(s => s.Etabid == rangsetab.Etabid && s.Sessionid == rangsetab.Sessionid).ToList();

           // DMdel.GetStagiaires(Query=)


            rangsetab.Moyenne = stags.Sum(s => s.NoteCC)/stags.Count;

            if (Double.IsNaN( rangsetab.Moyenne )) rangsetab.Moyenne =0;
                
        }
    }
}
