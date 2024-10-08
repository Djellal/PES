using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using System.IO;
using Pes.Models.DMdel;
using Microsoft.AspNetCore.Components;

namespace Pes.Layouts
{
    public partial class MainLayoutComponent
    {
      
        protected async System.Threading.Tasks.Task EnsureRolesExists()
        {
            var roles = await Security.GetRoles();

            if (!roles.Any(r => r.Name == Constants.admin)) await Security.CreateRole(new Microsoft.AspNetCore.Identity.IdentityRole(Constants.admin));
            if (!roles.Any(r => r.Name == Constants.coordinateur)) await Security.CreateRole(new Microsoft.AspNetCore.Identity.IdentityRole(Constants.coordinateur));
            if (!roles.Any(r => r.Name == Constants.president_jury)) await Security.CreateRole(new Microsoft.AspNetCore.Identity.IdentityRole(Constants.president_jury));
            if (!roles.Any(r => r.Name == Constants.membre_jury)) await Security.CreateRole(new Microsoft.AspNetCore.Identity.IdentityRole(Constants.membre_jury));
            if (!roles.Any(r => r.Name == Constants.expert)) await Security.CreateRole(new Microsoft.AspNetCore.Identity.IdentityRole(Constants.expert));

            

            if (!Directory.Exists(Constants.DocsPath)) Directory.CreateDirectory(Constants.DocsPath);
        }

        protected async System.Threading.Tasks.Task<Session> GetActiveSession()
        {
            return await DMdel.GetActiveSession();
        }

    }
}
