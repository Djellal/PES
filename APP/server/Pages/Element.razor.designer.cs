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
    public partial class ElementComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<Pes.Models.DMdel.Element> grid0;
        protected RadzenDataGrid<Pes.Models.DMdel.Critere> grid1;

        IEnumerable<Pes.Models.DMdel.Element> _getElementsResult;
        protected IEnumerable<Pes.Models.DMdel.Element> getElementsResult
        {
            get
            {
                return _getElementsResult;
            }
            set
            {
                if (!object.Equals(_getElementsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getElementsResult", NewValue = value, OldValue = _getElementsResult };
                    _getElementsResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        Pes.Models.DMdel.Element _master;
        protected Pes.Models.DMdel.Element master
        {
            get
            {
                return _master;
            }
            set
            {
                if (!object.Equals(_master, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "master", NewValue = value, OldValue = _master };
                    _master = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        string _lastFilter;
        protected string lastFilter
        {
            get
            {
                return _lastFilter;
            }
            set
            {
                if (!object.Equals(_lastFilter, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "lastFilter", NewValue = value, OldValue = _lastFilter };
                    _lastFilter = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Critere> _Criteres;
        protected IEnumerable<Pes.Models.DMdel.Critere> Criteres
        {
            get
            {
                return _Criteres;
            }
            set
            {
                if (!object.Equals(_Criteres, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "Criteres", NewValue = value, OldValue = _Criteres };
                    _Criteres = value;
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
            if(Globals.ActiveSession == null)
{
    NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Attention",Detail = $"Aucune session active. Veuillez activer une cession avant de gérer les rubriques." });
    return;
};

            var dMdelGetElementsResult = await DMdel.GetElements();
            getElementsResult = dMdelGetElementsResult;
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            if(Globals.ActiveSession == null)
{
    NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Attention",Detail = $"Aucune session active. Veuillez activer une cession avant de gérer les rubriques." });
    return;
};

            var dialogResult = await DialogService.OpenAsync<AddElement>("Add Element", null);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async void Grid0Render(DataGridRenderEventArgs<Pes.Models.DMdel.Element> args)
        {
            if (grid0.Query.Filter != lastFilter) {
                master = grid0.View.FirstOrDefault();
            }

            if (grid0.Query.Filter != lastFilter)
            {
                await grid0.SelectRow(master);
            }

            lastFilter = grid0.Query.Filter;
        }

        protected async System.Threading.Tasks.Task Grid0RowDoubleClick(DataGridRowMouseEventArgs<Pes.Models.DMdel.Element> args)
        {
            await DialogService.OpenAsync<EditElement>("Edit Element", new Dictionary<string, object>() { {"Id", args.Data.Id} });
        }

        protected async System.Threading.Tasks.Task Grid0RowSelect(Pes.Models.DMdel.Element args)
        {
            master = args;

            if (args == null) {
                Criteres = null;
            }

            if (args != null)
            {
                var dMdelGetCriteresResult = await DMdel.GetCriteres(new Query() { Filter = $@"i => i.Elementid == {args.Id}" });
                Criteres = dMdelGetCriteresResult;
            }
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Voulez-vous vraiment supprimer cette ligne ?") == true)
                {
                    var dMdelDeleteElementResult = await DMdel.DeleteElement(data.Id);
                    if (dMdelDeleteElementResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteElementException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Element" });
            }
        }

        protected async System.Threading.Tasks.Task CritereAddButtonClick(MouseEventArgs args)
        {
            if(Globals.ActiveSession == null)
{
    NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Attention",Detail = $"Aucune session active. Veuillez activer une cession avant de gérer les rubriques." });
    return;
};

            var dialogResult = await DialogService.OpenAsync<AddCritere>("Add Critere", new Dictionary<string, object>() { {"Elementid", master.Id} });
            await grid1.Reload();
        }

        protected async System.Threading.Tasks.Task Grid1RowSelect(Pes.Models.DMdel.Critere args)
        {
            var dialogResult = await DialogService.OpenAsync<EditCritere>("Edit Critere", new Dictionary<string, object>() { {"Id", args.Id} });
            await grid1.Reload();
        }

        protected async System.Threading.Tasks.Task CritereDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Voulez-vous vraiment supprimer cette ligne ?") == true)
                {
                    var dMdelDeleteCritereResult = await DMdel.DeleteCritere(data.Id);
                    if (dMdelDeleteCritereResult != null)
                    {
                        await grid1.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteCritereException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Element" });
            }
        }
    }
}
