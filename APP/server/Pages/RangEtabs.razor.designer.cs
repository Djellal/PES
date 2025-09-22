using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Pes.Models.DMdel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Pes.Models;

namespace Pes.Pages
{
    public partial class RangEtabsComponent : ComponentBase, IDisposable
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, dynamic> Attributes { get; set; }

        [Inject]
        protected GlobalsService Globals { get; set; }

        partial void OnDispose();

        public void Dispose()
        {
            Globals.PropertyChanged -= OnPropertyChanged;
            OnDispose();
        }

        public void Reload()
        {
            InvokeAsync(StateHasChanged);
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
        }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager UriHelper { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected DMdelService DMdel { get; set; }
        protected RadzenDataGrid<Pes.Models.DMdel.RangsEtab> grid0;

        IEnumerable<Pes.Models.DMdel.Session> _getSessionsResult;
        protected IEnumerable<Pes.Models.DMdel.Session> getSessionsResult
        {
            get
            {
                return _getSessionsResult;
            }
            set
            {
                if (!object.Equals(_getSessionsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getSessionsResult", NewValue = value, OldValue = _getSessionsResult };
                    _getSessionsResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Etablissement> _getEtablissementsResult;
        protected IEnumerable<Pes.Models.DMdel.Etablissement> getEtablissementsResult
        {
            get
            {
                return _getEtablissementsResult;
            }
            set
            {
                if (!object.Equals(_getEtablissementsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getEtablissementsResult", NewValue = value, OldValue = _getEtablissementsResult };
                    _getEtablissementsResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        Pes.Models.DMdel.RangsEtab _rangsetab;
        protected Pes.Models.DMdel.RangsEtab rangsetab
        {
            get
            {
                return _rangsetab;
            }
            set
            {
                if (!object.Equals(_rangsetab, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "rangsetab", NewValue = value, OldValue = _rangsetab };
                    _rangsetab = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.RangsEtab> _getRangsEtabsResult;
        protected IEnumerable<Pes.Models.DMdel.RangsEtab> getRangsEtabsResult
        {
            get
            {
                return _getRangsEtabsResult;
            }
            set
            {
                if (!object.Equals(_getRangsEtabsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getRangsEtabsResult", NewValue = value, OldValue = _getRangsEtabsResult };
                    _getRangsEtabsResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Region> _getRegionsResult;
        protected IEnumerable<Pes.Models.DMdel.Region> getRegionsResult
        {
            get
            {
                return _getRegionsResult;
            }
            set
            {
                if (!object.Equals(_getRegionsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getRegionsResult", NewValue = value, OldValue = _getRegionsResult };
                    _getRegionsResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        int? _SelectedRegion;
        protected int? SelectedRegion
        {
            get
            {
                return _SelectedRegion;
            }
            set
            {
                if (!object.Equals(_SelectedRegion, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "SelectedRegion", NewValue = value, OldValue = _SelectedRegion };
                    _SelectedRegion = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            Globals.PropertyChanged += OnPropertyChanged;
            await Security.InitializeAsync(AuthenticationStateProvider);
            if (!Security.IsAuthenticated())
            {
                UriHelper.NavigateTo("Login", true);
            }
            else
            {
                await Load();
            }
        }
        protected async System.Threading.Tasks.Task Load()
        {
            var dMdelGetSessionsResult = await DMdel.GetSessions();
            getSessionsResult = dMdelGetSessionsResult;

            var dMdelGetEtablissementsResult = await DMdel.GetEtablissements();
            getEtablissementsResult = dMdelGetEtablissementsResult;

            rangsetab = new Pes.Models.DMdel.RangsEtab(){};

            getRangsEtabsResult = null;

            if (Globals.ActiveSession != null)
            {
                await GenerateRangs(Globals.ActiveSession.Id);
            }

            var dMdelGetRegionsResult = await DMdel.GetRegions();
            getRegionsResult = dMdelGetRegionsResult;

            SelectedRegion = null;
        }

        protected async System.Threading.Tasks.Task Dropdown0Change(dynamic args)
        {
            await SelectedRegionChanged();
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            if (Globals.ActiveSession != null)
            {
                await GenerateRangs(Globals.ActiveSession.Id);
            }
        }

        protected async System.Threading.Tasks.Task Button1Click(MouseEventArgs args)
        {
            if (Globals.ActiveSession != null)
            {
                await DMdel.ExportRangsEtabsToExcel(new Query() { Filter = $@"r => r.Sessionid == {Globals.ActiveSession.Id}", OrderBy = $"r => r.Rang", Expand = "Etablissement", Select = "Rang,Etablissement.NomEtablissement,Moyenne" }, $"rangs-etabs");

            }
        }

        protected async System.Threading.Tasks.Task Grid0RowCreate(dynamic args)
        {
            var dMdelCreateRangsEtabResult = await DMdel.CreateRangsEtab(args);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Grid0RowUpdate(dynamic args)
        {
            var dMdelUpdateRangsEtabResult = await DMdel.UpdateRangsEtab(args.Id, args);
        }
    }
}
