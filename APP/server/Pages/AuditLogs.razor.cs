using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Pes.Models;
using Radzen;
using Radzen.Blazor;

namespace Pes.Pages
{
    public partial class AuditLogsComponent
    {
        protected string SelectedUserName { get; set; }
        protected DateTime? FilterDateFrom { get; set; }
        protected DateTime? FilterDateTo { get; set; }

        IEnumerable<AuditLog> _allRowsSource;
        List<AuditLog> _allRows;

        /// <summary>
        /// Copie matérialisée du jeu de données complet (non filtré).
        /// Recalculée uniquement quand Load() remplace getAuditLogsResult,
        /// afin d'énumérer l'IQueryable EF une seule fois par chargement.
        /// </summary>
        protected List<AuditLog> AllRows
        {
            get
            {
                var source = getAuditLogsResult;
                if (!ReferenceEquals(source, _allRowsSource))
                {
                    _allRowsSource = source;
                    _allRows = source?.ToList() ?? new List<AuditLog>();
                }
                return _allRows;
            }
        }

        /// <summary>
        /// Noms d'utilisateurs distincts présents dans le journal (sans filtre appliqué,
        /// pour que les options du dropdown ne disparaissent pas lors du filtrage).
        /// </summary>
        protected IEnumerable<string> UserNames
        {
            get
            {
                return AllRows
                    .Where(a => !string.IsNullOrEmpty(a.UserName))
                    .Select(a => a.UserName)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(n => n, StringComparer.Ordinal)
                    .ToList();
            }
        }

        /// <summary>
        /// Données filtrées par utilisateur et par plage de dates affichées dans la grille.
        /// </summary>
        protected IEnumerable<AuditLog> FilteredAuditLogs
        {
            get
            {
                IEnumerable<AuditLog> rows = AllRows;

                if (!string.IsNullOrEmpty(SelectedUserName))
                {
                    rows = rows.Where(a => a.UserName == SelectedUserName);
                }

                if (FilterDateFrom.HasValue)
                {
                    var from = FilterDateFrom.Value.Date;
                    rows = rows.Where(a => a.Timestamp.Date >= from);
                }

                if (FilterDateTo.HasValue)
                {
                    var to = FilterDateTo.Value.Date;
                    rows = rows.Where(a => a.Timestamp.Date <= to);
                }

                return rows.ToList();
            }
        }

        protected void FilterChange(dynamic args)
        {
            SelectedUserName = args == null ? null : args.ToString();
            OnFilterChanged();
        }

        protected void OnFilterChanged()
        {
            Reload();
            grid0?.Reload();
        }

        protected void ClearFiltersClick(MouseEventArgs args)
        {
            SelectedUserName = null;
            FilterDateFrom = null;
            FilterDateTo = null;
            OnFilterChanged();
        }

        protected async System.Threading.Tasks.Task DetailsButtonClick(MouseEventArgs args, AuditLog data)
        {
            if (data == null)
            {
                return;
            }

            var header = $"Date       : {data.Timestamp:dd/MM/yyyy HH:mm:ss}\n"
                       + $"Utilisateur: {data.UserName}\n"
                       + $"Source     : {data.Source}\n"
                       + $"Adresse IP : {data.IpAddress}\n"
                       + $"Action     : {data.Action}\n"
                       + $"Entité     : {data.EntityType} {data.EntityId}\n"
                       + $"Résumé     : {data.Summary}\n"
                       + "\nDétails :\n";

            var details = string.IsNullOrEmpty(data.DetailsJson) ? "(aucun détail)" : data.DetailsJson;

            await DialogService.OpenAsync("Détail de l'action",
                ds =>
                {
                    return builder =>
                    {
                        builder.OpenElement(0, "pre");
                        builder.AddAttribute(1, "style", "white-space: pre-wrap; word-break: break-all; margin: 0; font-size: 13px; max-height: 60vh; overflow: auto;");
                        builder.AddContent(2, header + details);
                        builder.CloseElement();
                    };
                },
                new DialogOptions { Width = "700px", CloseDialogOnOverlayClick = true });
        }
    }
}
