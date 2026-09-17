using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Microsoft.EntityFrameworkCore;
using Pes.Models.DMdel;
using Pes.Models;

namespace Pes.Pages
{
    public class EtabStat
    {
        public int EtabId { get; set; }
        public string Nom { get; set; }
        public string Region { get; set; }
        public int Count { get; set; }
        public double Moyenne { get; set; }
        public double TauxReussite { get; set; }
        public int Rang { get; set; }
    }

    public class MentionStat
    {
        public string Mention { get; set; }
        public int Count { get; set; }
    }

    public partial class StatistiquesComponent
    {
        public int TotalStagiaires { get; set; }
        public int StagiairesEvalues { get; set; }
        public double TauxReussite { get; set; }
        public double MoyenneNoteFinale { get; set; }
        public double MoyenneNoteCours { get; set; }
        public double MoyenneNoteCC { get; set; }

        public List<EtabStat> EtabStats { get; set; } = new List<EtabStat>();
        public List<MentionStat> MentionStats { get; set; } = new List<MentionStat>();

        public bool HasData { get { return TotalStagiaires > 0; } }

        protected async System.Threading.Tasks.Task LoadStats()
        {
            try
            {
                if (Globals.ActiveSession?.Id == null) return;

                var sessionId = Globals.ActiveSession.Id;
                var scopedEtabIds = await GetScopedEtabIds();

                var query = DMdel.DMContext.Stagiaires
                    .AsNoTracking()
                    .Include(s => s.Etablissement).ThenInclude(e => e.Region)
                    .Where(s => s.Sessionid == sessionId);

                if (scopedEtabIds != null)
                {
                    var ids = scopedEtabIds;
                    query = query.Where(s => s.Etabid != null && ids.Contains(s.Etabid.Value));
                }

                var stagiaires = await query.ToListAsync();

                TotalStagiaires = stagiaires.Count;

                if (TotalStagiaires == 0)
                {
                    EtabStats = new List<EtabStat>();
                    MentionStats = new List<MentionStat>();
                    StagiairesEvalues = 0;
                    TauxReussite = 0;
                    MoyenneNoteFinale = 0;
                    MoyenneNoteCours = 0;
                    MoyenneNoteCC = 0;
                    return;
                }

                StagiairesEvalues = await CountStagiairesEvalues(stagiaires.Select(s => s.Id).ToList());

                MoyenneNoteFinale = stagiaires.Average(s => s.NoteFinale);
                MoyenneNoteCours = stagiaires.Average(s => s.Note);
                MoyenneNoteCC = stagiaires.Average(s => s.NoteCC);
                TauxReussite = (double)stagiaires.Count(s => s.NoteFinale >= 0.5) / TotalStagiaires;

                MentionStats = BuildMentions(stagiaires);
                EtabStats = BuildEtabStats(stagiaires);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = "LoadStats : \r\n" + ex.Message });
            }
        }

        private async Task<List<int?>> GetScopedEtabIds()
        {
            if (Security.IsInRole(Constants.admin))
            {
                return null;
            }

            if (Security.IsInRole(Constants.admin_regional))
            {
                if (Security.User?.Regid == null) return new List<int?>();
                return await DMdel.DMContext.Etablissements
                    .AsNoTracking()
                    .Where(e => e.Regid == Security.User.Regid)
                    .Select(e => (int?)e.Id)
                    .ToListAsync();
            }

            if (Security.User?.Etabid != null)
            {
                return new List<int?> { Security.User.Etabid };
            }

            return new List<int?>();
        }

        private async Task<int> CountStagiairesEvalues(List<int> stagIds)
        {
            if (stagIds.Count == 0) return 0;

            var liste = await DMdel.DMContext.Evaluations
                .AsNoTracking()
                .Where(ev => ev.Stagid != null && stagIds.Contains(ev.Stagid.Value) && !ev.EstSynthese)
                .Select(ev => ev.Stagid.Value)
                .Distinct()
                .ToListAsync();

            return liste.Count;
        }

        private List<MentionStat> BuildMentions(List<Stagiaire> stagiaires)
        {
            var result = new List<MentionStat>();
            var bands = new (string nom, double min, double max)[]
            {
                ("Excellent", 0.9, double.MaxValue),
                ("Très Bien", 0.8, 0.9),
                ("Bien", 0.7, 0.8),
                ("Assez Bien", 0.6, 0.7),
                ("Passable", 0.5, 0.6),
                ("Insuffisant", double.MinValue, 0.5)
            };

            foreach (var band in bands)
            {
                var count = stagiaires.Count(s => s.NoteFinale >= band.min && s.NoteFinale < band.max);
                result.Add(new MentionStat() { Mention = band.nom, Count = Math.Max(count, 0) });
            }

            return result;
        }

        private List<EtabStat> BuildEtabStats(List<Stagiaire> stagiaires)
        {
            var result = new List<EtabStat>();

            foreach (var group in stagiaires.GroupBy(s => s.Etabid))
            {
                var first = group.First();
                result.Add(new EtabStat()
                {
                    EtabId = group.Key ?? 0,
                    Nom = first.Etablissement?.NomEtablissement ?? $"Établissement {group.Key}",
                    Region = first.Etablissement?.Region?.NomRegion ?? "",
                    Count = group.Count(),
                    Moyenne = group.Average(s => s.NoteFinale),
                    TauxReussite = (double)group.Count(s => s.NoteFinale >= 0.5) / group.Count()
                });
            }

            int rank = 0;
            int counter = 0;
            double prevMoyenne = double.NaN;
            foreach (var item in result.OrderByDescending(x => x.Moyenne))
            {
                counter++;
                if (item.Moyenne != prevMoyenne) rank = counter;
                item.Rang = rank;
                prevMoyenne = item.Moyenne;
            }

            return result.OrderBy(x => x.Rang).ThenByDescending(x => x.Moyenne).ToList();
        }
    }
}