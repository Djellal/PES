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
    public partial class FacultesComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<Pes.Models.DMdel.Faculte> grid0;
        protected RadzenDataGrid<Pes.Models.DMdel.Departement> grid1;

        IEnumerable<Pes.Models.DMdel.Faculte> _getFacultesResult;
        protected IEnumerable<Pes.Models.DMdel.Faculte> getFacultesResult
        {
            get
            {
                return _getFacultesResult;
            }
            set
            {
                if (!object.Equals(_getFacultesResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getFacultesResult", NewValue = value, OldValue = _getFacultesResult };
                    _getFacultesResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        int? _SelectedEtab;
        protected int? SelectedEtab
        {
            get
            {
                return _SelectedEtab;
            }
            set
            {
                if (!object.Equals(_SelectedEtab, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "SelectedEtab", NewValue = value, OldValue = _SelectedEtab };
                    _SelectedEtab = value;
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

        dynamic _master;
        protected dynamic master
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
            var dMdelGetFacultesResult = await DMdel.GetFacultes(new Query() { Expand = "Etablissement" });
            getFacultesResult = dMdelGetFacultesResult;

            SelectedEtab = null;

            getEtablissementsResult = new List<Pes.Models.DMdel.Etablissement>();

            await LoadFacultes();
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            var dialogResult = await DialogService.OpenAsync<AddFacultes>($"Ajouter Faculté/Institut", null);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Dropdown0Change(dynamic args)
        {
            await LoadFacultes();
        }

        protected async System.Threading.Tasks.Task Grid0RowExpand(Pes.Models.DMdel.Faculte args)
        {
            master = args;

            var dMdelGetDepartementsResult = await DMdel.GetDepartements(new Query() { Filter = $@"i => i.Facid == {args.Id}" });
            if (dMdelGetDepartementsResult != null)
            {
                args.Departements = dMdelGetDepartementsResult.ToList();
            }
        }

        protected async System.Threading.Tasks.Task Grid0RowSelect(Pes.Models.DMdel.Faculte args)
        {
            var dialogResult = await DialogService.OpenAsync<EditFacultes>($"Modifier Faulté/Institut", new Dictionary<string, object>() { {"Id", args.Id} });
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Voulez-vous vraiment supprimer cette ligne ?") == true)
                {
                    var dMdelDeleteFaculteResult = await DMdel.DeleteFaculte(data.Id);
                    if (dMdelDeleteFaculteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteFaculteException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Faculte" });
            }
        }

        protected async System.Threading.Tasks.Task DepartementAddButtonClick(MouseEventArgs args, dynamic data)
        {
            var dialogResult = await DialogService.OpenAsync<AddDepartement>($"Ajouter Département", new Dictionary<string, object>() { {"Facid", data.Id} });
            await Grid0RowExpand(master);

            await grid1.Reload();
        }

        protected async System.Threading.Tasks.Task Grid1RowSelect(Pes.Models.DMdel.Departement args, dynamic data)
        {
            var dialogResult = await DialogService.OpenAsync<EditDepartement>($"Modifier Département", new Dictionary<string, object>() { {"Id", args.Id} });
            await Grid0RowExpand(master);

            await grid1.Reload();
        }

        protected async System.Threading.Tasks.Task DepartementDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Voulez-vous vraiment supprimer cette ligne ?") == true)
                {
                    var dMdelDeleteDepartementResult = await DMdel.DeleteDepartement(data.Id);
                    await Grid0RowExpand(master);

                    if (dMdelDeleteDepartementResult != null)
                    {
                        await grid1.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteDepartementException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Faculte" });
            }
        }
    }
}
