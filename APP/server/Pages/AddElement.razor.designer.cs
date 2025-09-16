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
    public partial class AddElementComponent : ComponentBase, IDisposable
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

        IEnumerable<Pes.Models.DMdel.Rubrique> _getRubriquesResult;
        protected IEnumerable<Pes.Models.DMdel.Rubrique> getRubriquesResult
        {
            get
            {
                return _getRubriquesResult;
            }
            set
            {
                if (!object.Equals(_getRubriquesResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getRubriquesResult", NewValue = value, OldValue = _getRubriquesResult };
                    _getRubriquesResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        Pes.Models.DMdel.Element _element;
        protected Pes.Models.DMdel.Element element
        {
            get
            {
                return _element;
            }
            set
            {
                if (!object.Equals(_element, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "element", NewValue = value, OldValue = _element };
                    _element = value;
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
            var dMdelGetRubriquesResult = await DMdel.GetRubriques(new Query() { Filter = $@"r=>r.Sessionid = {Globals.ActiveSession?.Id}" });
            getRubriquesResult = dMdelGetRubriquesResult;

            element = new Pes.Models.DMdel.Element(){Sessionid = Globals.ActiveSession?.Id};
        }

        protected async System.Threading.Tasks.Task Form0Submit(Pes.Models.DMdel.Element args)
        {
            try
            {
                var dMdelCreateElementResult = await DMdel.CreateElement(element);
                DialogService.Close(element);
            }
            catch (System.Exception dMdelCreateElementException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to create new Element!" });
            }
        }

        protected async System.Threading.Tasks.Task Button2Click(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
