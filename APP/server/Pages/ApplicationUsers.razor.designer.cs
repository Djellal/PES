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
    public partial class ApplicationUsersComponent : ComponentBase, IDisposable
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
        protected RadzenDataGrid<ApplicationUser> grid0;

        int? _SelectedEtab;
        protected int? SelectedEtab
        {
            get
            {
                return _SelectedEtab;
            }
            set
            {
                if (!object.Equals(_SelectedEtab, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "SelectedEtab", NewValue = value, OldValue = _SelectedEtab };
                    _SelectedEtab = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.ApplicationUser> _users;
        protected IEnumerable<Pes.Models.ApplicationUser> users
        {
            get
            {
                return _users;
            }
            set
            {
                if (!object.Equals(_users, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "users", NewValue = value, OldValue = _users };
                    _users = value;
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
            SelectedEtab = null;

            users = null;

            var dMdelGetEtablissementsResult = await DMdel.GetEtablissements();
            getEtablissementsResult = dMdelGetEtablissementsResult;

            await LoadUsers();
        }

        protected async System.Threading.Tasks.Task Dropdown0Change(dynamic args)
        {
            await LoadUsers();
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            var dialogResult = await DialogService.OpenAsync<AddApplicationUser>($"Ajouter Un utilisateur", null, new DialogOptions(){ Width = $"{800}px",CloseDialogOnOverlayClick = true,Resizable = true,Draggable = true });
            await Load();

            await grid0.Reload();
        }

        protected async System.Threading.Tasks.Task Grid0RowSelect(ApplicationUser args)
        {
            var dialogResult = await DialogService.OpenAsync<EditApplicationUser>($"Modifier utilisateur", new Dictionary<string, object>() { {"Id", args.Id} }, new DialogOptions(){ Width = $"{800}px",CloseDialogOnOverlayClick = true,Resizable = true,Draggable = true });
            await Load();

            await grid0.Reload();
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, ApplicationUser data)
        {
            try
            {
                var securityDeleteUserResult = await Security.DeleteUser($"{data.Id}");
                await Load();

                if (securityDeleteUserResult != null)
                {
                    await grid0.Reload();
                }
            }
            catch (System.Exception securityDeleteUserException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete user" });
            }
        }
    }
}
