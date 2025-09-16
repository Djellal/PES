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
    public partial class RubriquesComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<Pes.Models.DMdel.Rubrique> grid0;

        Pes.Models.DMdel.Rubrique _rubrique;
        protected Pes.Models.DMdel.Rubrique rubrique
        {
            get
            {
                return _rubrique;
            }
            set
            {
                if (!object.Equals(_rubrique, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "rubrique", NewValue = value, OldValue = _rubrique };
                    _rubrique = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

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

        Pes.Models.DMdel.Rubrique _editItem;
        protected Pes.Models.DMdel.Rubrique editItem
        {
            get
            {
                return _editItem;
            }
            set
            {
                if (!object.Equals(_editItem, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "editItem", NewValue = value, OldValue = _editItem };
                    _editItem = value;
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

            rubrique = new Pes.Models.DMdel.Rubrique(){};

            var dMdelGetRubriquesResult = await DMdel.GetRubriques(new Query() { Filter = $@"r=>r.Sessionid == {Globals.ActiveSession?.Id}" });
            getRubriquesResult = dMdelGetRubriquesResult;
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            if (Globals.ActiveSession != null)
{
    await this.grid0.InsertRow(new Pes.Models.DMdel.Rubrique { NomRubrique = "Nouvelle rubrique", Coeff = 1, Sessionid = Globals.ActiveSession?.Id });
}
else
{
    NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Warning, Summary = $"Attention", Detail = $"Aucune session active. Veuillez activer une session avant de créer une rubrique." });
};
        }

        protected async System.Threading.Tasks.Task Grid0RowCreate(dynamic args)
        {
            var dMdelCreateRubriqueResult = await DMdel.CreateRubrique(args);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Grid0RowUpdate(dynamic args)
        {
            var dMdelUpdateRubriqueResult = await DMdel.UpdateRubrique(args.Id, args);
        }

        protected async System.Threading.Tasks.Task EditButtonClick(MouseEventArgs args, dynamic data)
        {
            if (editItem != null)
            {
                var dMdelCancelRubriqueChangesResult = await DMdel.CancelRubriqueChanges(editItem);

            }

            await this.Load();

            await this.grid0.Reload();

            this.grid0.EditRow(data);

            editItem = data;
        }

        protected async System.Threading.Tasks.Task SaveButtonClick(MouseEventArgs args, dynamic data)
        {
            this.grid0.UpdateRow(data);
        }

        protected async System.Threading.Tasks.Task CancelButtonClick(MouseEventArgs args, dynamic data)
        {
            this.grid0.CancelEditRow(data);

            var dMdelCancelRubriqueChangesResult = await DMdel.CancelRubriqueChanges(data);
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Voulez-vous vraiment supprimer cette ligne ?") == true)
                {
                    var dMdelDeleteRubriqueResult = await DMdel.DeleteRubrique(data.Id);
                    if (dMdelDeleteRubriqueResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteRubriqueException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Rubrique" });
            }
        }
    }
}
