using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using Microsoft.EntityFrameworkCore;
using Pes.Models.DMdel;
using Pes.Models;

namespace Pes.Pages
{
    public partial class StagiaireEvaluationsDetailComponent
    {
        public IEnumerable<Pes.Models.DMdel.Evaluation> Evals { get; set; } = new List<Pes.Models.DMdel.Evaluation>();

        public IEnumerable<ApplicationUser> Membresjury { get; private set; }

        public Dictionary<string, string> MembresNames { get; private set; } = new Dictionary<string, string>();

        protected async System.Threading.Tasks.Task LoadEvals()
        {
            try
            {
                if (stagiaire == null) return;

                Membresjury = await Security.GetUsersInRoleAndEtab(new string[] { Constants.membre_jury, Constants.president_jury, Constants.expert }, stagiaire.Etabid);
                MembresNames = (Membresjury ?? Enumerable.Empty<ApplicationUser>())
                    .Where(m => !string.IsNullOrEmpty(m.Id))
                    .ToDictionary(m => m.Id, m => m.Name);

                Evals = DMdel.DMContext.Evaluations
                    .Include(ev => ev.Critere)
                    .Include(ev => ev.Echelle)
                    .Where(ev => ev.Stagid == stagiaire.Id)
                    .OrderBy(ev => ev.NomRubrique)
                    .ThenBy(ev => ev.NomElement)
                    .ThenBy(ev => ev.Critere.NomCritere)
                    .ThenBy(ev => ev.EstSynthese ? 0 : 1)
                    .ToList();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "LoadEvals : \r\n" + ex.Message });
            }
        }

        public string GetMembreName(string membreId)
        {
            if (string.IsNullOrEmpty(membreId)) return "";
            return MembresNames.TryGetValue(membreId, out var name) ? name : membreId;
        }

        public string FormatNote(Evaluation ev)
        {
            return ev.Echelle?.Val.ToString("F2") ?? "-";
        }

        protected async System.Threading.Tasks.Task BackToListClick()
        {
            DialogService.Close(null);
        }
    }
}