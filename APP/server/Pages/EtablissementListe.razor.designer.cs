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
    public partial class EtablissementListeComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<Pes.Models.DMdel.Etablissement> grid0;

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
            var dMdelGetRegionsResult = await DMdel.GetRegions();
            getRegionsResult = dMdelGetRegionsResult;

            etablissement = new Pes.Models.DMdel.Etablissement(){};

            var dMdelGetEtablissementsResult = await DMdel.GetEtablissements(new Query() { Expand = "Region" });
            getEtablissementsResult = dMdelGetEtablissementsResult;
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            await this.grid0.InsertRow(new Pes.Models.DMdel.Etablissement());
        }

        protected async System.Threading.Tasks.Task Grid0RowCreate(dynamic args)
        {
            var dMdelCreateEtablissementResult = await DMdel.CreateEtablissement(args);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Grid0RowUpdate(dynamic args)
        {
            var dMdelUpdateEtablissementResult = await DMdel.UpdateEtablissement(args.Id, args);
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

            var dMdelCancelEtablissementChangesResult = await DMdel.CancelEtablissementChanges(data);
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Voulez-vous vraiment supprimer cet établissement ?") == true)
                {
                    var dMdelDeleteEtablissementResult = await DMdel.DeleteEtablissement(data.Id);
                    if (dMdelDeleteEtablissementResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteEtablissementException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Etablissement" });
            }
        }
    }
}
