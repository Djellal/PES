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
    public partial class AddFacultesComponent : ComponentBase, IDisposable
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

        Pes.Models.DMdel.Faculte _faculte;
        protected Pes.Models.DMdel.Faculte faculte
        {
            get
            {
                return _faculte;
            }
            set
            {
                if (!object.Equals(_faculte, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "faculte", NewValue = value, OldValue = _faculte };
                    _faculte = value;
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
            faculte = new Pes.Models.DMdel.Faculte(){};

            getEtablissementsForEtabidResult = null;

            await LoadEtablissement();
        }

        protected async System.Threading.Tasks.Task Form0Submit(Pes.Models.DMdel.Faculte args)
        {
            try
            {
                var dMdelCreateFaculteResult = await DMdel.CreateFaculte(faculte);
                DialogService.Close(faculte);
            }
            catch (System.Exception dMdelCreateFaculteException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to create new Faculte!" });
            }
        }

        protected async System.Threading.Tasks.Task Button1Click(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
