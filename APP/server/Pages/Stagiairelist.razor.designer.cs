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
    public partial class StagiairelistComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<Pes.Models.DMdel.Stagiaire> grid0;

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

        Pes.Models.DMdel.Stagiaire _stagiaire;
        protected Pes.Models.DMdel.Stagiaire stagiaire
        {
            get
            {
                return _stagiaire;
            }
            set
            {
                if (!object.Equals(_stagiaire, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "stagiaire", NewValue = value, OldValue = _stagiaire };
                    _stagiaire = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Stagiaire> _getStagiairesResult;
        protected IEnumerable<Pes.Models.DMdel.Stagiaire> getStagiairesResult
        {
            get
            {
                return _getStagiairesResult;
            }
            set
            {
                if (!object.Equals(_getStagiairesResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getStagiairesResult", NewValue = value, OldValue = _getStagiairesResult };
                    _getStagiairesResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

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

        IEnumerable<Pes.Models.DMdel.Departement> _getDepartementsResult;
        protected IEnumerable<Pes.Models.DMdel.Departement> getDepartementsResult
        {
            get
            {
                return _getDepartementsResult;
            }
            set
            {
                if (!object.Equals(_getDepartementsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getDepartementsResult", NewValue = value, OldValue = _getDepartementsResult };
                    _getDepartementsResult = value;
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
            SelectedEtab = null;

            getEtablissementsResult = null;

            stagiaire = new Pes.Models.DMdel.Stagiaire(){};

            getStagiairesResult = null;

            getFacultesResult = null;

            getDepartementsResult = null;

            await LoadStagiaires();
        }

        protected async System.Threading.Tasks.Task Splitbutton0Click(RadzenSplitButtonItem args)
        {
            await ExecuteAction(args.Value);
        }

        protected async System.Threading.Tasks.Task UploadListMouseEnter(ElementReference args)
        {
            TooltipService.Open(args, $"Le nom de la faculté et de département doit exister dans la liste des faculté et départements de l'établissement", new TooltipOptions(){ Duration = 5000,Position = TooltipPosition.Bottom });
        }

        protected async System.Threading.Tasks.Task UploadListComplete(UploadCompleteEventArgs args)
        {
            await LoadStagiaires();
        }

        protected async System.Threading.Tasks.Task UploadListError(UploadErrorEventArgs args)
        {
            if (SelectedEtab == null)
            {
                await DialogService.Alert("Il faut séléctionner l'établissement");
            }
        }

        protected async System.Threading.Tasks.Task Dropdown0Change(dynamic args)
        {
            await LoadStagiaires();
        }

        protected async System.Threading.Tasks.Task Grid0RowCreate(dynamic args)
        {
            var dMdelCreateStagiaireResult = await DMdel.CreateStagiaire(args);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Grid0RowUpdate(dynamic args)
        {
            var dMdelUpdateStagiaireResult = await DMdel.UpdateStagiaire(args.Id, args);
        }

        protected async System.Threading.Tasks.Task Switch0Change(bool args, dynamic data)
        {
            grid0.SelectRow(data);
        }

        protected async System.Threading.Tasks.Task Switch1Change(bool args)
        {
            await ChangeSelectionStatus(args);
        }

        protected async System.Threading.Tasks.Task FacultedropdownChange(dynamic args, dynamic data)
        {
            var dMdelGetDepartementsResult = await DMdel.GetDepartements(new Query() { Filter = $@"d=>d.Facid == {args}" });
            getDepartementsResult = dMdelGetDepartementsResult;
        }

        protected async System.Threading.Tasks.Task EditButtonClick(MouseEventArgs args, dynamic data)
        {
            this.grid0.EditRow(data);
        }

        protected async System.Threading.Tasks.Task SaveButtonClick(MouseEventArgs args, dynamic data)
        {
            this.grid0.UpdateRow(data);
        }

        protected async System.Threading.Tasks.Task CancelButtonClick(MouseEventArgs args, dynamic data)
        {
            this.grid0.CancelEditRow(data);

            var dMdelCancelStagiaireChangesResult = await DMdel.CancelStagiaireChanges(data);
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            await GridDeleteButtonClicked(data.Id);
        }
    }
}
