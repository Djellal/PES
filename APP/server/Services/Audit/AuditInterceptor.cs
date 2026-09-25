using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pes.Models;

namespace Pes
{
    /// <summary>
    /// Capture automatique des créations / modifications / suppressions d'entités
    /// métier à chaque SaveChanges et leur écrit dans la table AuditLogs, dans la
    /// même transaction que le changement métier (atomicité).
    /// Les entités d'identité (AspNet*) sont ignorées : elles sont journalisées
    /// explicitement par AccountController / SecurityService avec un résumé
    /// métier lisible.
    /// </summary>
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private const int MaxDetailsLength = 16000;

        private readonly AuditUserContext auditUser;

        public AuditInterceptor(AuditUserContext auditUser)
        {
            this.auditUser = auditUser;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            AddAuditEntries(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            AddAuditEntries(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            DetachAuditEntries(eventData.Context);
            base.SaveChangesFailed(eventData);
        }

        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            DetachAuditEntries(eventData.Context);
            return base.SaveChangesFailedAsync(eventData, cancellationToken);
        }

        private void AddAuditEntries(DbContext data)
        {
            if (data == null)
            {
                return;
            }

            List<EntityEntry> entries;

            try
            {
                entries = data.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added
                             || e.State == EntityState.Modified
                             || e.State == EntityState.Deleted)
                    .Where(e => !(e.Entity is AuditLog) && !IsExplicitlyLoggedEntity(e))
                    .ToList();
            }
            catch (InvalidOperationException)
            {
                return;
            }

            if (entries.Count == 0)
            {
                return;
            }

            var info = auditUser.GetCurrentUser();
            var timestamp = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                var audit = BuildAuditEntry(entry, info, timestamp);
                if (audit != null)
                {
                    data.Set<AuditLog>().Add(audit);
                }
            }
        }

        private static AuditLog BuildAuditEntry(EntityEntry entry, AuditUserInfo info, DateTime timestamp)
        {
            var entityType = entry.Metadata.ClrType.Name;
            var entityId = string.Join("|", entry.Metadata.FindPrimaryKey().Properties
                .Select(p => entry.Property(p.Name)?.CurrentValue?.ToString()));
            string action;
            object details;

            switch (entry.State)
            {
                case EntityState.Added:
                    action = AuditActions.Create;
                    details = entry.Properties.ToDictionary(
                        p => p.Metadata.Name,
                        p => Mask(p.Metadata.Name, p.CurrentValue));
                    break;
                case EntityState.Deleted:
                    action = AuditActions.Delete;
                    details = entry.Properties.ToDictionary(
                        p => p.Metadata.Name,
                        p => Mask(p.Metadata.Name, p.OriginalValue));
                    break;
                case EntityState.Modified:
                    var changedProperties = entry.Properties
                        .Where(p => !Equals(p.OriginalValue, p.CurrentValue))
                        .ToList();

                    if (changedProperties.Count == 0)
                    {
                        // Mise à jour sans réel changement : inutile de journaliser.
                        return null;
                    }

                    action = AuditActions.Update;
                    details = changedProperties.ToDictionary(
                        p => p.Metadata.Name,
                        p => (object)new Dictionary<string, object>
                        {
                            ["old"] = Mask(p.Metadata.Name, p.OriginalValue),
                            ["new"] = Mask(p.Metadata.Name, p.CurrentValue)
                        });
                    break;
                default:
                    return null;
            }

            return new AuditLog
            {
                Timestamp = timestamp,
                UserId = info.UserId,
                UserName = info.UserName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Summary = $"{entityType} ({entityId})",
                DetailsJson = SerializeDetails(details),
                IpAddress = info.IpAddress,
                Source = info.Source
            };
        }

        private static string SerializeDetails(object details)
        {
            try
            {
                var json = JsonSerializer.Serialize(details);

                if (json.Length > MaxDetailsLength)
                {
                    return JsonSerializer.Serialize(new
                    {
                        truncated = true,
                        note = "Détails trop volumineux",
                        length = json.Length
                    });
                }

                return json;
            }
            catch
            {
                return null;
            }
        }

        private static object Mask(string propertyName, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (propertyName.Contains("Password", StringComparison.OrdinalIgnoreCase))
            {
                return "[MASQUÉ]";
            }

            return value;
        }

        private static bool IsExplicitlyLoggedEntity(EntityEntry entry)
        {
            var name = entry.Metadata.ClrType.Name;
            return name == nameof(ApplicationUser) || name.StartsWith("Identity");
        }

        private static void DetachAuditEntries(DbContext data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var entry in data.ChangeTracker.Entries<AuditLog>()
                .Where(e => e.State == EntityState.Added)
                .ToList())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
