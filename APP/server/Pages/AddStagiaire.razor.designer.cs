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
    public partial class AddStagiaireComponent : ComponentBase, IDisposable
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
            getEtablissementsForEtabidResult = dMdelGetEtablissementsResult;

            stagiaire = new Pes.Models.DMdel.Stagiaire(){};
        }

        protected async System.Threading.Tasks.Task Form0Submit(Pes.Models.DMdel.Stagiaire args)
        {
            try
            {
                var dMdelCreateStagiaireResult = await DMdel.CreateStagiaire(stagiaire);
                DialogService.Close(stagiaire);
            }
            catch (System.Exception dMdelCreateStagiaireException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to create new Stagiaire!" });
            }
        }

        protected async System.Threading.Tasks.Task Button1Click(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
