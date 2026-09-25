using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pes.Data;
using Pes.Models;
using Radzen;

namespace Pes
{
    /// <summary>
    /// Écriture et lecture du journal d'audit (table AuditLogs).
    /// Les écritures automatiques (CRUD métier) sont assurées par AuditInterceptor ;
    /// ce service permet de journaliser explicitement les événements sans
    /// SaveChanges propre (connexions, déconnexions, gestion des comptes...).
    /// </summary>
    public partial class AuditService
    {
        private readonly ApplicationIdentityDbContext context;
        private readonly AuditUserContext auditUser;

        public AuditService(ApplicationIdentityDbContext context, AuditUserContext auditUser)
        {
            this.context = context;
            this.auditUser = auditUser;
        }

        public async Task LogAsync(string action, string entityType = null, string entityId = null, string summary = null, object details = null, string userName = null)
        {
            var info = auditUser.GetCurrentUser();

            // Lors d'une connexion, HttpContext.User n'est pas encore mis à jour
            // dans la même requête : on fournit explicitement le nom d'utilisateur.
            if (!string.IsNullOrEmpty(userName))
            {
                info.UserName = userName;
            }

            string detailsJson = null;
            if (details != null)
            {
                try
                {
                    detailsJson = System.Text.Json.JsonSerializer.Serialize(details);
                }
                catch
                {
                    detailsJson = null;
                }
            }

            var log = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                UserId = info.UserId,
                UserName = info.UserName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Summary = summary,
                DetailsJson = detailsJson,
                IpAddress = info.IpAddress,
                Source = info.Source
            };

            context.AuditLogs.Add(log);
            await context.SaveChangesAsync();
        }

        partial void OnAuditLogsRead(ref IQueryable<Models.AuditLog> items);

        public async Task<IQueryable<Models.AuditLog>> GetAuditLogs(Query query = null)
        {
            var items = Context.AuditLogs.AsQueryable();

            if (string.IsNullOrEmpty(query?.OrderBy))
            {
                items = items.OrderByDescending(a => a.Timestamp);
            }

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnAuditLogsRead(ref items);

            return await Task.FromResult(items);
        }

        private ApplicationIdentityDbContext Context
        {
            get
            {
                return this.context;
            }
        }
    }
}
