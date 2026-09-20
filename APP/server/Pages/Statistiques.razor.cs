using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Pes.Models.DMdel;
using Pes.Models;

namespace Pes.Pages
{
    public class EtabStatItem
    {
        public int EtabId { get; set; }
        public string Nom { get; set; } = "";
        public string Region { get; set; } = "";
        public int Count { get; set; }
        public double Moyenne { get; set; }
        public double TauxReussite { get; set; }
        public int Rang { get; set; }
    }

    public class FaculteStatItem
    {
        public int FacId { get; set; }
        public string Nom { get; set; } = "";
        public int Count { get; set; }
        public double Moyenne { get; set; }
        public double TauxReussite { get; set; }
        public int Rang { get; set; }
    }

    public class MentionStatItem
    {
        public string Mention { get; set; } = "";
        public int Count { get; set; }
    }

    public partial class Statistiques : ComponentBase, IDisposable
    {
        [Inject] protected DMdelService DMdel { get; set; }
        [Inject] protected SecurityService Security { get; set; }
        [Inject] protected GlobalsService Globals { get; set; }
        [Inject] protected NavigationManager UriHelper { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        // State
        public bool IsLoading { get; set; } = true;
        public bool HasData => TotalStagiaires > 0;

        // Visibility helpers used by the template. The page content (grids/charts) is always
        // rendered and only hidden with CSS/Visible so that RadzenDataGrid/RadzenChart are never
        // disposed and re-created, which previously caused System.ObjectDisposedException on render.
        public bool ShowContent => !IsLoading && HasData;
        public bool ShowEmptyState => !IsLoading && !HasData;
        public bool ShowFaculteChart => IsSingleEtab && FaculteStats.Count > 1;
        public bool ShowFaculteTable => IsSingleEtab && FaculteStats.Count > 0;
        public string StatsChartTitle => ShowFaculteChart
            ? "Moyenne finale par faculté"
            : (IsSingleEtab ? "Moyenne finale de l'établissement" : "Moyenne finale par établissement");
        public string StatsTableTitle => ShowFaculteTable ? "Détail par faculté" : "Détail par établissement";

        // Formatters used by the chart value axes. In Radzen 4.30.x the data labels inherit the
        // ValueAxis formatting (see AxisBase.Formatter), so label percentages/counts are set here.
        private string FormatPercent(object value) => Convert.ToDouble(value).ToString("P0");
        private string FormatCount(object value) => Convert.ToDouble(value).ToString("N0");

        // KPI metrics
        public int TotalStagiaires { get; set; }
        public int StagiairesEvalues { get; set; }
        public double TauxReussite { get; set; }
        public double MoyenneNoteFinale { get; set; }
        public double MoyenneNoteCours { get; set; }
        public double MoyenneNoteCC { get; set; }

        // Data tables & charts
        public List<EtabStatItem> EtabStats { get; set; } = new();
        public List<FaculteStatItem> FaculteStats { get; set; } = new();
        public List<MentionStatItem> MentionStats { get; set; } = new();

        // Roles & Scope
        public bool IsAdmin { get; set; }
        public bool IsAdminRegional { get; set; }
        public bool IsCoordinateur { get; set; }
        public bool IsSingleEtab { get; set; }

        public string ScopeTitle { get; set; } = "";
        public int? UserRegId { get; set; }
        public int? UserEtabId { get; set; }
        public string UserEtabNom { get; set; } = "";
        public string UserRegNom { get; set; } = "";

        // Filters
        public List<Session> SessionsList { get; set; } = new();
        public int? SelectedSessionId { get; set; }

        public List<Region> RegionsList { get; set; } = new();
        public int? SelectedRegionId { get; set; }

        public List<Etablissement> FilterEtabsList { get; set; } = new();
        public int? SelectedEtabId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Security.InitializeAsync(AuthenticationStateProvider);
            if (!Security.IsAuthenticated())
            {
                UriHelper.NavigateTo("Login", true);
                return;
            }

            await InitializeComponentDataAsync();
        }

        private async Task InitializeComponentDataAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                IsLoading = true;

                // 1. Resolve roles
                IsAdmin = Security.IsInRole(Constants.admin)
                          || string.Equals(Security.Principal?.Identity?.Name, "admin", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(Security.User?.UserName, "admin", StringComparison.OrdinalIgnoreCase);

                IsAdminRegional = !IsAdmin && Security.IsInRole(Constants.admin_regional);
                IsCoordinateur = !IsAdmin && !IsAdminRegional;

                // 2. Resolve Region and Etab IDs
                await ResolveScopeIdsAsync();

                // 3. Load all sessions
                SessionsList = await DMdel.DMContext.Sessions
                    .AsNoTracking()
                    .OrderByDescending(s => s.EnCours)
                    .ThenByDescending(s => s.DateDebut)
                    .ToListAsync();

                // Pick active session by default, or most recent
                var activeSess = SessionsList.FirstOrDefault(s => s.EnCours) ?? SessionsList.FirstOrDefault();
                SelectedSessionId = Globals.ActiveSession?.Id ?? activeSess?.Id;

                // 4. Load filter lists according to role
                if (IsAdmin)
                {
                    RegionsList = await DMdel.DMContext.Regions.AsNoTracking().OrderBy(r => r.NomRegion).ToListAsync();
                    FilterEtabsList = await DMdel.DMContext.Etablissements.AsNoTracking().OrderBy(e => e.NomEtablissement).ToListAsync();
                }
                else if (IsAdminRegional && UserRegId.HasValue)
                {
                    FilterEtabsList = await DMdel.DMContext.Etablissements.AsNoTracking()
                        .Where(e => e.Regid == UserRegId.Value)
                        .OrderBy(e => e.NomEtablissement)
                        .ToListAsync();
                }

                // 5. Load statistics for selected session
                await LoadStatsInternalAsync();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Erreur", Detail = ex.Message });
            }
            finally
            {
                IsLoading = false;
                _semaphore.Release();
            }
        }

        private async Task ResolveScopeIdsAsync()
        {
            var currentUserName = Security.Principal?.Identity?.Name;

            if (IsAdmin)
            {
                ScopeTitle = "National (Tous les établissements)";
                return;
            }

            if (IsAdminRegional)
            {
                UserRegId = Security.User?.Regid;
                if (!UserRegId.HasValue && !string.IsNullOrEmpty(currentUserName))
                {
                    UserRegId = await Security.context.Users
                        .Where(u => u.UserName == currentUserName || u.Email == currentUserName)
                        .Select(u => u.Regid)
                        .FirstOrDefaultAsync();
                }

                if (!UserRegId.HasValue)
                {
                    var etabId = Security.User?.Etabid ?? Globals.Etab?.Id;
                    if (etabId.HasValue)
                    {
                        UserRegId = await DMdel.DMContext.Etablissements
                            .Where(e => e.Id == etabId.Value)
                            .Select(e => e.Regid)
                            .FirstOrDefaultAsync();
                    }
                }

                if (UserRegId.HasValue)
                {
                    var reg = await DMdel.DMContext.Regions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == UserRegId.Value);
                    UserRegNom = reg?.NomRegion ?? $"Région #{UserRegId}";
                    ScopeTitle = $"Région {UserRegNom}";
                }
                else
                {
                    ScopeTitle = "Région (Non définie)";
                }
                return;
            }

            // Coordinateur & Jury: strictly own establishment
            UserEtabId = Security.User?.Etabid ?? Globals.Etab?.Id;
            if (!UserEtabId.HasValue && !string.IsNullOrEmpty(currentUserName))
            {
                UserEtabId = await Security.context.Users
                    .Where(u => u.UserName == currentUserName || u.Email == currentUserName)
                    .Select(u => u.Etabid)
                    .FirstOrDefaultAsync();
            }

            if (UserEtabId.HasValue)
            {
                var etab = await DMdel.DMContext.Etablissements.AsNoTracking().FirstOrDefaultAsync(e => e.Id == UserEtabId.Value);
                UserEtabNom = etab?.NomEtablissement ?? $"Établissement #{UserEtabId}";
                ScopeTitle = UserEtabNom;
                if (Globals.Etab == null && etab != null)
                {
                    Globals.Etab = etab;
                }
            }
            else
            {
                ScopeTitle = "Établissement (Non défini)";
            }
        }

        private async Task<List<int>> GetScopedEtabIdsAsync()
        {
            if (IsAdmin)
            {
                if (SelectedEtabId.HasValue)
                {
                    return new List<int> { SelectedEtabId.Value };
                }
                if (SelectedRegionId.HasValue)
                {
                    return await DMdel.DMContext.Etablissements
                        .AsNoTracking()
                        .Where(e => e.Regid == SelectedRegionId.Value)
                        .Select(e => e.Id)
                        .ToListAsync();
                }
                return null; // Global!
            }

            if (IsAdminRegional)
            {
                if (!UserRegId.HasValue) return new List<int>();

                if (SelectedEtabId.HasValue)
                {
                    return new List<int> { SelectedEtabId.Value };
                }

                return await DMdel.DMContext.Etablissements
                    .AsNoTracking()
                    .Where(e => e.Regid == UserRegId.Value)
                    .Select(e => e.Id)
                    .ToListAsync();
            }

            // Coordinateur & jury: only related establishment
            if (UserEtabId.HasValue)
            {
                return new List<int> { UserEtabId.Value };
            }

            return new List<int>();
        }

        private async Task LoadStatsInternalAsync()
        {
            if (!SelectedSessionId.HasValue)
            {
                ResetStats();
                return;
            }

            var sessionId = SelectedSessionId.Value;
            var scopedEtabIds = await GetScopedEtabIdsAsync();

            if (scopedEtabIds != null && scopedEtabIds.Count == 0)
            {
                ResetStats();
                return;
            }

            IsSingleEtab = scopedEtabIds != null && scopedEtabIds.Count == 1;

            var query = DMdel.DMContext.Stagiaires
                .AsNoTracking()
                .Include(s => s.Etablissement).ThenInclude(e => e.Region)
                .Include(s => s.Faculte)
                .Where(s => s.Sessionid == sessionId);

            if (scopedEtabIds != null)
            {
                query = query.Where(s => s.Etabid.HasValue && scopedEtabIds.Contains(s.Etabid.Value));
            }

            var stagiaires = await query.ToListAsync();
            TotalStagiaires = stagiaires.Count;

            if (TotalStagiaires == 0)
            {
                ResetStats();
                return;
            }

            var evalQuery = DMdel.DMContext.Evaluations
                .AsNoTracking()
                .Where(ev => ev.Stagid.HasValue && ev.Stagiaire.Sessionid == sessionId && !ev.EstSynthese);

            if (scopedEtabIds != null)
            {
                evalQuery = evalQuery.Where(ev => ev.Stagiaire.Etabid.HasValue && scopedEtabIds.Contains(ev.Stagiaire.Etabid.Value));
            }

            StagiairesEvalues = await evalQuery
                .Select(ev => ev.Stagid.Value)
                .Distinct()
                .CountAsync();

            MoyenneNoteFinale = stagiaires.Average(s => s.NoteFinale);
            MoyenneNoteCours = stagiaires.Average(s => s.Note);
            MoyenneNoteCC = stagiaires.Average(s => s.NoteCC);
            TauxReussite = (double)stagiaires.Count(s => s.NoteFinale >= 0.5) / TotalStagiaires;

            MentionStats = BuildMentions(stagiaires);
            EtabStats = BuildEtabStats(stagiaires);
            FaculteStats = BuildFaculteStats(stagiaires);
        }

        private void ResetStats()
        {
            TotalStagiaires = 0;
            StagiairesEvalues = 0;
            TauxReussite = 0;
            MoyenneNoteFinale = 0;
            MoyenneNoteCours = 0;
            MoyenneNoteCC = 0;
            EtabStats = new();
            FaculteStats = new();
            MentionStats = new();
        }

        private List<MentionStatItem> BuildMentions(List<Stagiaire> stagiaires)
        {
            var result = new List<MentionStatItem>();
            var bands = new (string nom, double min, double max)[]
            {
                ("Excellent (≥ 90%)", 0.9, double.MaxValue),
                ("Très Bien (80% - 90%)", 0.8, 0.9),
                ("Bien (70% - 80%)", 0.7, 0.8),
                ("Assez Bien (60% - 70%)", 0.6, 0.7),
                ("Passable (50% - 60%)", 0.5, 0.6),
                ("Insuffisant (< 50%)", double.MinValue, 0.5)
            };

            foreach (var band in bands)
            {
                var count = stagiaires.Count(s => s.NoteFinale >= band.min && s.NoteFinale < band.max);
                result.Add(new MentionStatItem { Mention = band.nom, Count = Math.Max(count, 0) });
            }

            return result;
        }

        private List<EtabStatItem> BuildEtabStats(List<Stagiaire> stagiaires)
        {
            var result = new List<EtabStatItem>();

            foreach (var group in stagiaires.GroupBy(s => s.Etabid))
            {
                var first = group.First();
                result.Add(new EtabStatItem
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

        private List<FaculteStatItem> BuildFaculteStats(List<Stagiaire> stagiaires)
        {
            var result = new List<FaculteStatItem>();

            foreach (var group in stagiaires.GroupBy(s => s.Facid))
            {
                var first = group.First();
                string nomFaculte = first.Faculte?.NomFaculte;
                if (string.IsNullOrWhiteSpace(nomFaculte))
                {
                    nomFaculte = "Non assigné";
                }

                result.Add(new FaculteStatItem
                {
                    FacId = group.Key ?? 0,
                    Nom = nomFaculte,
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

        public async Task OnSessionChanged(object args)
        {
            await _semaphore.WaitAsync();
            try
            {
                IsLoading = true;
                await LoadStatsInternalAsync();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Erreur", Detail = ex.Message });
            }
            finally
            {
                IsLoading = false;
                _semaphore.Release();
            }
        }

        public async Task OnRegionChanged(object args)
        {
            await _semaphore.WaitAsync();
            try
            {
                IsLoading = true;
                SelectedEtabId = null;
                if (SelectedRegionId.HasValue)
                {
                    FilterEtabsList = await DMdel.DMContext.Etablissements.AsNoTracking()
                        .Where(e => e.Regid == SelectedRegionId.Value)
                        .OrderBy(e => e.NomEtablissement)
                        .ToListAsync();
                }
                else
                {
                    FilterEtabsList = await DMdel.DMContext.Etablissements.AsNoTracking()
                        .OrderBy(e => e.NomEtablissement)
                        .ToListAsync();
                }
                await LoadStatsInternalAsync();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Erreur", Detail = ex.Message });
            }
            finally
            {
                IsLoading = false;
                _semaphore.Release();
            }
        }

        public async Task OnEtabChanged(object args)
        {
            await _semaphore.WaitAsync();
            try
            {
                IsLoading = true;
                await LoadStatsInternalAsync();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Erreur", Detail = ex.Message });
            }
            finally
            {
                IsLoading = false;
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}