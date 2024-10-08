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
    public partial class EditStagiaireComponent : ComponentBase, IDisposable
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

        [Parameter]
        public dynamic Id { get; set; }
        protected RadzenDataGrid<Pes.Models.DMdel.Evaluation> grid0;

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

        IEnumerable<Pes.Models.DMdel.Etablissement> _getEtablissementsForEtabidResult;
        protected IEnumerable<Pes.Models.DMdel.Etablissement> getEtablissementsForEtabidResult
        {
            get
            {
                return _getEtablissementsForEtabidResult;
            }
            set
            {
                if (!object.Equals(_getEtablissementsForEtabidResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getEtablissementsForEtabidResult", NewValue = value, OldValue = _getEtablissementsForEtabidResult };
                    _getEtablissementsForEtabidResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Echelle> _getEchellesResult;
        protected IEnumerable<Pes.Models.DMdel.Echelle> getEchellesResult
        {
            get
            {
                return _getEchellesResult;
            }
            set
            {
                if (!object.Equals(_getEchellesResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getEchellesResult", NewValue = value, OldValue = _getEchellesResult };
                    _getEchellesResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Critere> _getCriteresResult;
        protected IEnumerable<Pes.Models.DMdel.Critere> getCriteresResult
        {
            get
            {
                return _getCriteresResult;
            }
            set
            {
                if (!object.Equals(_getCriteresResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getCriteresResult", NewValue = value, OldValue = _getCriteresResult };
                    _getCriteresResult = value;
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
            var dMdelGetStagiaireByIdResult = await DMdel.GetStagiaireById(Id);
            stagiaire = dMdelGetStagiaireByIdResult;

            var dMdelGetEtablissementsResult = await DMdel.GetEtablissements();
            getEtablissementsForEtabidResult = dMdelGetEtablissementsResult;

            var dMdelGetEchellesResult = await DMdel.GetEchelles(new Query() { OrderBy = $"c=>c.Val" });
            getEchellesResult = dMdelGetEchellesResult;

            var dMdelGetCriteresResult = await DMdel.GetCriteres();
            getCriteresResult = dMdelGetCriteresResult;

            await LoadEvalutions();

            await Calculer();
        }

        protected async System.Threading.Tasks.Task Form0Submit(Pes.Models.DMdel.Stagiaire args)
        {
            try
            {
                var dMdelUpdateStagiaireResult = await DMdel.UpdateStagiaire(Id, stagiaire);
                DialogService.Close(stagiaire);
            }
            catch (System.Exception dMdelUpdateStagiaireException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to update Stagiaire" });
            }

            await Calculer();
        }

        protected async System.Threading.Tasks.Task Checkbox0Change(bool? args)
        {
            try
            {
                await CalculerNote();
            }
            catch (System.Exception calculerNoteException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Erreur",Detail = $"{calculerNoteException.Message}" });
            }
        }

        protected async void Grid0Render(DataGridRenderEventArgs<Pes.Models.DMdel.Evaluation> args)
        {
            if (args.FirstRender)
{
    args.Grid.Groups.Add(new GroupDescriptor() { Property = "NomRubrique", SortOrder = SortOrder.Descending, Title = "Rubrique" });
    //args.Grid.Groups.Add(new GroupDescriptor() { Property = "NomElement", SortOrder = SortOrder.Descending,Title= "Element" });
    StateHasChanged();
};
        }

        protected async System.Threading.Tasks.Task Grid0RowCreate(dynamic args)
        {
            var dMdelCreateEvaluationResult = await DMdel.CreateEvaluation(args);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Grid0RowUpdate(dynamic args)
        {
            var dMdelUpdateEvaluationResult = await DMdel.UpdateEvaluation(args.Id, args);

            await Calculer();
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

            var dMdelCancelEvaluationChangesResult = await DMdel.CancelEvaluationChanges(data);
        }

        protected async System.Threading.Tasks.Task Button1Click(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
