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
    public partial class StagiairesEvaluationsComponent : ComponentBase, IDisposable
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
            var dMdelGetEtablissementsResult = await DMdel.GetEtablissements();
            getEtablissementsResult = dMdelGetEtablissementsResult;

            getStagiairesResult = null;

            await LoadStagiaires();
        }

        protected async System.Threading.Tasks.Task Splitbutton0Click(RadzenSplitButtonItem args)
        {
            await ExecuteAction(args.Value);
        }

        protected async System.Threading.Tasks.Task Dropdown0Change(dynamic args)
        {
            await LoadStagiaires();
        }

        protected async System.Threading.Tasks.Task Switch0Change(bool args, dynamic data)
        {
            grid0.SelectRow(data);
        }

        protected async System.Threading.Tasks.Task Switch1Change(bool args)
        {
            await ChangeSelectionStatus(args);
        }

        protected async System.Threading.Tasks.Task Splitbutton1Click(RadzenSplitButtonItem args, dynamic data)
        {
            await AttestatioAction($"{args.Value}",data);
        }

        protected async System.Threading.Tasks.Task EditButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                await DMdel.CreateEvals(data.Id);
            }
            catch (System.Exception exception)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = exception.Message });
            };

            var dialogResult = await DialogService.OpenAsync<EditStagiaire>($"Evaluation Stagiaire", new Dictionary<string, object>() { {"Id", data.Id} }, new DialogOptions(){ Width = $"{1300}px",CloseDialogOnOverlayClick = true,Resizable = true,Draggable = true });
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }
    }
}
