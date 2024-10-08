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
    public partial class EditEtablissementsComponent : ComponentBase, IDisposable
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

        Pes.Models.DMdel.Etablissement _etablissement;
        protected Pes.Models.DMdel.Etablissement etablissement
        {
            get
            {
                return _etablissement;
            }
            set
            {
                if (!object.Equals(_etablissement, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "etablissement", NewValue = value, OldValue = _etablissement };
                    _etablissement = value;
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
            var dMdelGetEtablissementByIdResult = await DMdel.GetEtablissementById(Id);
            etablissement = dMdelGetEtablissementByIdResult;

            var dMdelGetRegionsResult = await DMdel.GetRegions();
            getRegionsResult = dMdelGetRegionsResult;
        }

        protected async System.Threading.Tasks.Task Form0Submit(Pes.Models.DMdel.Etablissement args)
        {
            try
            {
                var dMdelUpdateEtablissementResult = await DMdel.UpdateEtablissement(Id, etablissement);
                DialogService.Close(etablissement);
            }
            catch (System.Exception dMdelUpdateEtablissementException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to update Etablissement" });
            }
        }

        protected async System.Threading.Tasks.Task Button2Click(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
