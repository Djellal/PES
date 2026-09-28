using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Pes.Data;
using Pes.Models;
using Pes.Models.DMdel;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pes.Pages
{
    public partial class RepriseNotesComponent : ComponentBase
    {
        [Inject]
        public NotificationService NotificationService { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        [Inject]
        public GlobalsService Globals { get; set; }

        [Inject]
        public DMdelService DMdel { get; set; }

        public class RapportLigne
        {
            public string Libelle { get; set; }
            public string Valeur { get; set; }
        }

        public int sessionId { get; set; }

        public IEnumerable<Session> Sessions { get; set; } = new List<Session>();

        public bool enCours { get; set; }
        public string enCoursMessage { get; set; }
        public bool diagnosticFait { get; set; }
        public bool applique { get; set; }
        public string messageFinal { get; set; }

        public List<RapportLigne> rapport { get; set; } = new List<RapportLigne>();

        private const int TailleLot = 50;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            base.OnInitialized();
            Sessions = (await DMdel.GetSessions(new Query() { OrderBy = $"c=>c.DateDebut desc" }))?.ToList() ?? new List<Session>();
            sessionId = Globals.ActiveSession?.Id ?? 0;
        }

        private DMdelContext Ctx => DMdel.DMContext;

        protected async System.Threading.Tasks.Task DiagnosticAsync()
        {
            enCours = true;
            diagnosticFait = false;
            applique = false;
            rapport = new List<RapportLigne>();
            StateHasChanged();
            await InvokeAsync(StateHasChanged);

            try
            {
                int doublons = 0;
                foreach (var g in await Ctx.Evaluations
                             .Where(ev => ev.EstSynthese == false && ev.Criterid != null)
                             .Where(ev => Ctx.Criteres.Any(c => c.Id == ev.Criterid && c.Sessionid == sessionId))
                             .GroupBy(ev => new { ev.Stagid, ev.Criterid, ev.MembreId })
                             .Where(g => g.Count() > 1)
                             .ToListAsync())
                {
                    doublons += g.Count() - 1;
                }

                var syntheseIds = await Ctx.Evaluations
                    .Where(ev => ev.EstSynthese && ev.Stagid != null)
                    .Where(ev => Ctx.Stagiaires.Any(s => s.Id == ev.Stagid && s.Sessionid == sessionId))
                    .Select(ev => ev.Id)
                    .ToListAsync();

                int synthesesNonEvaluees = await Ctx.Evaluations
                    .CountAsync(ev => syntheseIds.Contains(ev.Id) && ev.NoteSynthese == null);

                int nbStagiaires = await Ctx.Stagiaires.CountAsync(s => s.Sessionid == sessionId);

                // Combien de notes changent avec la nouvelle formule (simulation, aucune ecriture).
                int notesChangees = 0;
                double minAvant = 1, maxAvant = 0, minApres = 1, maxApres = 0;
                bool premiere = true;

                var ids = await Ctx.Stagiaires
                    .Where(s => s.Sessionid == sessionId)
                    .OrderBy(s => s.Id)
                    .Select(s => new { s.Id, s.Note })
                    .ToListAsync();

                foreach (var st in ids)
                {
                    double apres = await DMdel.CalculerNoteSimulation(st.Id, sessionId);

                    if (premiere)
                    {
                        minAvant = maxAvant = st.Note;
                        minApres = maxApres = apres;
                        premiere = false;
                    }
                    else
                    {
                        minAvant = Math.Min(minAvant, st.Note);
                        maxAvant = Math.Max(maxAvant, st.Note);
                        minApres = Math.Min(minApres, apres);
                        maxApres = Math.Max(maxApres, apres);
                    }

                    if (Math.Abs(st.Note - apres) > 0.0001) notesChangees++;
                }

                rapport = new List<RapportLigne>
                {
                    new RapportLigne { Libelle = "Stagiaires de la session", Valeur = nbStagiaires.ToString("N0") },
                    new RapportLigne { Libelle = "Évaluations en double à supprimer", Valeur = doublons.ToString("N0") },
                    new RapportLigne { Libelle = "Synthèses « non évalué » (aucune note de jury)", Valeur = synthesesNonEvaluees.ToString("N0") },
                    new RapportLigne { Libelle = "Notes qui changent avec la nouvelle formule", Valeur = notesChangees.ToString("N0") },
                    new RapportLigne { Libelle = "Note du cours — avant (min / max)", Valeur = $"{minAvant:P2} / {maxAvant:P2}" },
                    new RapportLigne { Libelle = "Note du cours — après (min / max)", Valeur = $"{minApres:P2} / {maxApres:P2}" },
                };

                diagnosticFait = true;

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Diagnostic terminé",
                    Detail = $"{nbStagiaires} stagiaires analysés, {notesChangees} notes changent."
                });
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Erreur",
                    Detail = "Diagnostic : " + ex.Message
                });
            }
            finally
            {
                enCours = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async System.Threading.Tasks.Task AppliquerAsync()
        {
            var confirm = await DialogService.Confirm(
                "Cette opération est définitive : les évaluations en double seront supprimées et toutes les notes de la session recalculées. Continuer ?",
                "Appliquer la reprise", new ConfirmOptions() { Width = "450px" });

            if (confirm != true) return;

            enCours = true;
            applique = false;
            enCoursMessage = "Dédoublonnage des évaluations...";
            StateHasChanged();
            await InvokeAsync(StateHasChanged);

            try
            {
                int supprimes = await DMdel.SupprimerDoublonsEvaluations(sessionId);
                await InvokeAsync(StateHasChanged);

                var ids = await Ctx.Stagiaires
                    .Where(s => s.Sessionid == sessionId)
                    .OrderBy(s => s.Id)
                    .Select(s => s.Id)
                    .ToListAsync();

                int traites = 0;
                for (int i = 0; i < ids.Count; i += TailleLot)
                {
                    var lot = ids.Skip(i).Take(TailleLot);
                    foreach (int id in lot)
                    {
                        await DMdel.CalculerNote(id, sessionId);
                        traites++;
                    }

                    enCoursMessage = $"Recalcul {traites} / {ids.Count} stagiaires...";
                    await InvokeAsync(StateHasChanged);
                }

                await DMdel.CalculerMoyenneGlobale(sessionId);

                messageFinal = $"Reprise terminée : {supprimes} évaluations en double supprimées, {traites} notes recalculées.";
                applique = true;

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Reprise terminée",
                    Detail = messageFinal
                });
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Erreur",
                    Detail = "Reprise : " + ex.Message
                });
            }
            finally
            {
                enCours = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
