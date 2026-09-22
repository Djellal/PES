using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;

namespace Pes.Pages
{
    public partial class ProfileComponent
    {
        public string RegionName { get; private set; } = "—";
        public string EtabName { get; private set; } = "—";
        public string FaculteName { get; private set; } = "—";
        public string DepartementName { get; private set; } = "—";

        protected string UserInitials
        {
            get
            {
                var name = user?.Name;
                if (string.IsNullOrWhiteSpace(name)) return "?";

                var parts = name.Split(new[] { ' ', '.', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return "?";

                return string.Concat(parts.Take(2).Select(p => char.ToUpperInvariant(p[0])).ToArray());
            }
        }

        protected BadgeStyle GetRoleStyle(string role)
        {
            if (role == Constants.admin) return BadgeStyle.Danger;
            if (role == Constants.admin_regional) return BadgeStyle.Warning;
            if (role == Constants.coordinateur) return BadgeStyle.Info;
            if (role == Constants.president_jury) return BadgeStyle.Primary;
            if (role == Constants.membre_jury) return BadgeStyle.Secondary;
            if (role == Constants.expert) return BadgeStyle.Success;
            return BadgeStyle.Light;
        }

        protected override async System.Threading.Tasks.Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && user != null)
            {
                await ResolveUserDetails();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async System.Threading.Tasks.Task ResolveUserDetails()
        {
            try
            {
                if (user.Regid != null)
                    RegionName = (await DMdel.GetRegionById(user.Regid))?.NomRegion ?? "—";

                if (user.Etabid != null)
                    EtabName = (await DMdel.GetEtablissementById(user.Etabid))?.NomEtablissement ?? "—";

                if (user.Facultid != null)
                    FaculteName = (await DMdel.GetFaculteById(user.Facultid))?.NomFaculte ?? "—";

                if (user.Departid != null)
                    DepartementName = (await DMdel.GetDepartementById(user.Departid))?.NomDepartement ?? "—";
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Erreur", Detail = $"ResolveUserDetails : \r\n{ex.Message}" });
            }
        }
    }
}
