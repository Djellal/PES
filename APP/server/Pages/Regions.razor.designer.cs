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
    public partial class RegionsComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<Pes.Models.DMdel.Region> grid0;

        Pes.Models.DMdel.Region _region;
        protected Pes.Models.DMdel.Region region
        {
            get
            {
                return _region;
            }
            set
            {
                if (!object.Equals(_region, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "region", NewValue = value, OldValue = _region };
                    _region = value;
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

        Pes.Models.DMdel.Region _editItem;
        protected Pes.Models.DMdel.Region editItem
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
            region = new Pes.Models.DMdel.Region(){};

            var dMdelGetRegionsResult = await DMdel.GetRegions();
            getRegionsResult = dMdelGetRegionsResult;
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            await this.grid0.InsertRow(new Pes.Models.DMdel.Region());
        }

        protected async System.Threading.Tasks.Task Grid0RowCreate(dynamic args)
        {
            var dMdelCreateRegionResult = await DMdel.CreateRegion(args);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Grid0RowUpdate(dynamic args)
        {
            var dMdelUpdateRegionResult = await DMdel.UpdateRegion(args.Id, args);
        }

        protected async System.Threading.Tasks.Task EditButtonClick(MouseEventArgs args, dynamic data)
        {
            if (editItem != null)
            {
                var dMdelCancelRegionChangesResult = await DMdel.CancelRegionChanges(editItem);

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

            var dMdelCancelRegionChangesResult = await DMdel.CancelRegionChanges(data);
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var dMdelDeleteRegionResult = await DMdel.DeleteRegion(data.Id);
                    if (dMdelDeleteRegionResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteRegionException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Region" });
            }
        }
    }
}
