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
    public partial class SessionsComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<Pes.Models.DMdel.Session> grid0;

        Pes.Models.DMdel.Session _session;
        protected Pes.Models.DMdel.Session session
        {
            get
            {
                return _session;
            }
            set
            {
                if (!object.Equals(_session, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "session", NewValue = value, OldValue = _session };
                    _session = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Session> _getSessionsResult;
        protected IEnumerable<Pes.Models.DMdel.Session> getSessionsResult
        {
            get
            {
                return _getSessionsResult;
            }
            set
            {
                if (!object.Equals(_getSessionsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getSessionsResult", NewValue = value, OldValue = _getSessionsResult };
                    _getSessionsResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        Pes.Models.DMdel.Session _editItem;
        protected Pes.Models.DMdel.Session editItem
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
            session = new Pes.Models.DMdel.Session(){};

            var dMdelGetSessionsResult = await DMdel.GetSessions();
            getSessionsResult = dMdelGetSessionsResult;
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            await this.grid0.InsertRow(new Pes.Models.DMdel.Session());
        }

        protected async System.Threading.Tasks.Task Grid0RowCreate(dynamic args)
        {
            var dMdelCreateSessionResult = await DMdel.CreateSession(args);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Grid0RowUpdate(dynamic args)
        {
            var dMdelUpdateSessionResult = await DMdel.UpdateSession(args.Id, args);

            if (args.EnCours)
            {
                await SetActiveSession(args.Id);
            }
        }

        protected async System.Threading.Tasks.Task EnCourswitchChange(bool args, dynamic data)
        {
            await SetActiveSession(data);
        }

        protected async System.Threading.Tasks.Task EnCourswitcheditChange(bool args, dynamic data)
        {
            try
            {
                await SetActiveSession(data);
            }
            catch (System.Exception setActiveSessionException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Erreur" });
            }
        }

        protected async System.Threading.Tasks.Task EditButtonClick(MouseEventArgs args, dynamic data)
        {
            if (editItem != null)
            {
                var dMdelCancelSessionChangesResult = await DMdel.CancelSessionChanges(editItem);

            }

            await this.Load();

            await this.grid0.Reload();

            this.grid0.EditRow(data);

            editItem = data;
        }

        protected async System.Threading.Tasks.Task SaveButtonClick(MouseEventArgs args, dynamic data)
        {
            this.grid0.UpdateRow(data);

            if (data.EnCours)
            {
                await SetActiveSession(data.Id);
            }
        }

        protected async System.Threading.Tasks.Task CancelButtonClick(MouseEventArgs args, dynamic data)
        {
            this.grid0.CancelEditRow(data);

            var dMdelCancelSessionChangesResult = await DMdel.CancelSessionChanges(data);
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Voulez-vous vraiment supprimer cette ligne ?") == true)
                {
                    var dMdelDeleteSessionResult = await DMdel.DeleteSession(data.Id);
                    if (dMdelDeleteSessionResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception dMdelDeleteSessionException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Session" });
            }
        }
    }
}
