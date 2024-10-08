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
    public partial class EditApplicationUserComponent : ComponentBase, IDisposable
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

        ApplicationUser _user;
        protected ApplicationUser user
        {
            get
            {
                return _user;
            }
            set
            {
                if (!object.Equals(_user, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "user", NewValue = value, OldValue = _user };
                    _user = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<IdentityRole> _roles;
        protected IEnumerable<IdentityRole> roles
        {
            get
            {
                return _roles;
            }
            set
            {
                if (!object.Equals(_roles, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "roles", NewValue = value, OldValue = _roles };
                    _roles = value;
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

        IEnumerable<Pes.Models.DMdel.Faculte> _getFacultesResult;
        protected IEnumerable<Pes.Models.DMdel.Faculte> getFacultesResult
        {
            get
            {
                return _getFacultesResult;
            }
            set
            {
                if (!object.Equals(_getFacultesResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getFacultesResult", NewValue = value, OldValue = _getFacultesResult };
                    _getFacultesResult = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        IEnumerable<Pes.Models.DMdel.Departement> _getDepartementsResult;
        protected IEnumerable<Pes.Models.DMdel.Departement> getDepartementsResult
        {
            get
            {
                return _getDepartementsResult;
            }
            set
            {
                if (!object.Equals(_getDepartementsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getDepartementsResult", NewValue = value, OldValue = _getDepartementsResult };
                    _getDepartementsResult = value;
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
            var securityGetUserByIdResult = await Security.GetUserById($"{Id}");
            user = securityGetUserByIdResult;

            var securityGetRolesResult = await Security.GetRoles();
            roles = securityGetRolesResult;

            var dMdelGetRegionsResult = await DMdel.GetRegions();
            getRegionsResult = dMdelGetRegionsResult;

            var dMdelGetEtablissementsResult = await DMdel.GetEtablissements();
            getEtablissementsResult = dMdelGetEtablissementsResult;

            var dMdelGetFacultesResult = await DMdel.GetFacultes();
            getFacultesResult = dMdelGetFacultesResult;
        }

        protected async System.Threading.Tasks.Task Form0Submit(ApplicationUser args)
        {
            try
            {
                var securityUpdateUserResult = await Security.UpdateUser($"{Id}", args);
                DialogService.Close(null);
            }
            catch (System.Exception securityUpdateUserException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Cannot update user",Detail = $"{securityUpdateUserException.Message}" });
            }
        }

        protected async System.Threading.Tasks.Task RegiondropdownChange(dynamic args)
        {
            var dMdelGetEtablissementsResult = await DMdel.GetEtablissements(new Query() { Filter = $@"et=>et.Regid == {args}" });
            getEtablissementsResult = dMdelGetEtablissementsResult;
        }

        protected async System.Threading.Tasks.Task FacultedropdownChange(dynamic args)
        {
            var dMdelGetDepartementsResult = await DMdel.GetDepartements(new Query() { Filter = $@"d=>d.Facid == {args}" });
            getDepartementsResult = dMdelGetDepartementsResult;
        }

        protected async System.Threading.Tasks.Task Button2Click(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
