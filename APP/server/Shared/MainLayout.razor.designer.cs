﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Pes.Models.DMdel;
using Microsoft.AspNetCore.Identity;
using Pes.Models;
using Pes.Pages;

namespace Pes.Layouts
{
    public partial class MainLayoutComponent : LayoutComponentBase
    {
        [Inject]
        protected GlobalsService Globals { get; set; }

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
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected DMdelService DMdel { get; set; }

        protected RadzenBody body0;

        private void Authenticated()
        {
             StateHasChanged();
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
             if (Security != null)
             {
                  Security.Authenticated += Authenticated;

                  await Security.InitializeAsync(AuthenticationStateProvider);
             }
             await Load();
        }
        protected async System.Threading.Tasks.Task Load()
        {
            await EnsureRolesExists();

            var getActiveSessionResult = await GetActiveSession();
            Globals.ActiveSession = getActiveSessionResult;

            if (Security.User != null)
            {
                var dMdelGetEtablissementByIdResult = await DMdel.GetEtablissementById(Security.User?.Etabid);
                Globals.Etab = dMdelGetEtablissementByIdResult;
            }
        }

        protected async System.Threading.Tasks.Task Profilemenu0Click(dynamic args)
        {
            if (args.Value == "Logout")
            {
                await Security.Logout();
            }
        }
    }
}
