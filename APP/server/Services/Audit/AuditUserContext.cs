using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Pes
{
    /// <summary>
    /// Informations sur l'utilisateur à associer à une entrée du journal d'audit.
    /// </summary>
    public class AuditUserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// Résout l'utilisateur courant pour l'audit.
    /// Priorité au SecurityService (circuits Blazor), puis aux revendications
    /// HTTP (requêtes MVC). Ne dépend jamais de SecurityService au moment de la
    /// construction afin d'éviter toute dépendance circulaire avec les
    /// DbContexts (résolution paresseuse via IServiceProvider).
    /// </summary>
    public partial class AuditUserContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IServiceProvider serviceProvider;

        public AuditUserContext(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.serviceProvider = serviceProvider;
        }

        public AuditUserInfo GetCurrentUser()
        {
            var info = new AuditUserInfo { UserName = "Anonymous", Source = "Système" };

            try
            {
                // Attention : le "utilisateur anonyme" retourné par SecurityService
                // possède un Id auto-généré — on ne le accepte que si le nom
                // d'utilisateur est réellement renseigné.
                var security = serviceProvider.GetService<SecurityService>();
                var user = security?.User;

                if (user?.UserName != null && user.UserName != "Anonymous")
                {
                    info.UserId = user.Id;
                    info.UserName = user.UserName;
                    info.Source = "Blazor";
                }
            }
            catch
            {
                // SecurityService indisponible (ex. hors circuit Blazor) : on retombe sur HttpContext.
            }

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                if (info.Source == "Système")
                {
                    info.Source = "MVC";

                    if (httpContext.User?.Identity?.IsAuthenticated == true)
                    {
                        info.UserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        info.UserName = httpContext.User.Identity.Name ?? info.UserName;
                    }
                }

                info.IpAddress = httpContext.Connection?.RemoteIpAddress?.ToString();
            }

            return info;
        }
    }
}
