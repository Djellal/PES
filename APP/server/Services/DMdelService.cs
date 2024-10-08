using Radzen;
using System;
using System.Web;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Data;
using System.Text.Encodings.Web;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Pes.Data;

namespace Pes
{
    public partial class DMdelService
    {
        DMdelContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly DMdelContext context;
        private readonly NavigationManager navigationManager;

        public DMdelService(DMdelContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public async Task ExportCriteresToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/criteres/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/criteres/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCriteresToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/criteres/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/criteres/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCriteresRead(ref IQueryable<Models.DMdel.Critere> items);

        public async Task<IQueryable<Models.DMdel.Critere>> GetCriteres(Query query = null)
        {
            var items = Context.Criteres.AsQueryable();

            items = items.Include(i => i.Element);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnCriteresRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCritereCreated(Models.DMdel.Critere item);
        partial void OnAfterCritereCreated(Models.DMdel.Critere item);

        public async Task<Models.DMdel.Critere> CreateCritere(Models.DMdel.Critere critere)
        {
            OnCritereCreated(critere);

            var existingItem = Context.Criteres
                              .Where(i => i.Id == critere.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Criteres.Add(critere);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(critere).State = EntityState.Detached;
                critere.Element = null;
                throw;
            }

            OnAfterCritereCreated(critere);

            return critere;
        }
        public async Task ExportDepartementsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/departements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/departements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDepartementsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/departements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/departements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDepartementsRead(ref IQueryable<Models.DMdel.Departement> items);

        public async Task<IQueryable<Models.DMdel.Departement>> GetDepartements(Query query = null)
        {
            var items = Context.Departements.AsQueryable();

            items = items.Include(i => i.Faculte);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnDepartementsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDepartementCreated(Models.DMdel.Departement item);
        partial void OnAfterDepartementCreated(Models.DMdel.Departement item);

        public async Task<Models.DMdel.Departement> CreateDepartement(Models.DMdel.Departement departement)
        {
            OnDepartementCreated(departement);

            var existingItem = Context.Departements
                              .Where(i => i.Id == departement.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Departements.Add(departement);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(departement).State = EntityState.Detached;
                departement.Faculte = null;
                throw;
            }

            OnAfterDepartementCreated(departement);

            return departement;
        }
        public async Task ExportEchellesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/echelles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/echelles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEchellesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/echelles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/echelles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEchellesRead(ref IQueryable<Models.DMdel.Echelle> items);

        public async Task<IQueryable<Models.DMdel.Echelle>> GetEchelles(Query query = null)
        {
            var items = Context.Echelles.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnEchellesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEchelleCreated(Models.DMdel.Echelle item);
        partial void OnAfterEchelleCreated(Models.DMdel.Echelle item);

        public async Task<Models.DMdel.Echelle> CreateEchelle(Models.DMdel.Echelle echelle)
        {
            OnEchelleCreated(echelle);

            var existingItem = Context.Echelles
                              .Where(i => i.Id == echelle.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Echelles.Add(echelle);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(echelle).State = EntityState.Detached;
                throw;
            }

            OnAfterEchelleCreated(echelle);

            return echelle;
        }
        public async Task ExportElementsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/elements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/elements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportElementsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/elements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/elements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnElementsRead(ref IQueryable<Models.DMdel.Element> items);

        public async Task<IQueryable<Models.DMdel.Element>> GetElements(Query query = null)
        {
            var items = Context.Elements.AsQueryable();

            items = items.Include(i => i.Rubrique);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnElementsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnElementCreated(Models.DMdel.Element item);
        partial void OnAfterElementCreated(Models.DMdel.Element item);

        public async Task<Models.DMdel.Element> CreateElement(Models.DMdel.Element element)
        {
            OnElementCreated(element);

            var existingItem = Context.Elements
                              .Where(i => i.Id == element.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Elements.Add(element);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(element).State = EntityState.Detached;
                element.Rubrique = null;
                throw;
            }

            OnAfterElementCreated(element);

            return element;
        }
        public async Task ExportEtablissementsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/etablissements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/etablissements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEtablissementsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/etablissements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/etablissements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEtablissementsRead(ref IQueryable<Models.DMdel.Etablissement> items);

        public async Task<IQueryable<Models.DMdel.Etablissement>> GetEtablissements(Query query = null)
        {
            var items = Context.Etablissements.AsQueryable();

            items = items.Include(i => i.Region);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnEtablissementsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEtablissementCreated(Models.DMdel.Etablissement item);
        partial void OnAfterEtablissementCreated(Models.DMdel.Etablissement item);

        public async Task<Models.DMdel.Etablissement> CreateEtablissement(Models.DMdel.Etablissement etablissement)
        {
            OnEtablissementCreated(etablissement);

            var existingItem = Context.Etablissements
                              .Where(i => i.Id == etablissement.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Etablissements.Add(etablissement);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(etablissement).State = EntityState.Detached;
                etablissement.Region = null;
                throw;
            }

            OnAfterEtablissementCreated(etablissement);

            return etablissement;
        }
        public async Task ExportEvaluationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/evaluations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/evaluations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEvaluationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/evaluations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/evaluations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEvaluationsRead(ref IQueryable<Models.DMdel.Evaluation> items);

        public async Task<IQueryable<Models.DMdel.Evaluation>> GetEvaluations(Query query = null)
        {
            var items = Context.Evaluations.AsQueryable();

            items = items.Include(i => i.Stagiaire);

            items = items.Include(i => i.Critere);

            items = items.Include(i => i.Echelle);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnEvaluationsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEvaluationCreated(Models.DMdel.Evaluation item);
        partial void OnAfterEvaluationCreated(Models.DMdel.Evaluation item);

        public async Task<Models.DMdel.Evaluation> CreateEvaluation(Models.DMdel.Evaluation evaluation)
        {
            OnEvaluationCreated(evaluation);

            var existingItem = Context.Evaluations
                              .Where(i => i.Id == evaluation.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Evaluations.Add(evaluation);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(evaluation).State = EntityState.Detached;
                evaluation.Stagiaire = null;
                evaluation.Critere = null;
                evaluation.Echelle = null;
                throw;
            }

            OnAfterEvaluationCreated(evaluation);

            return evaluation;
        }
        public async Task ExportFacultesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/facultes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/facultes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFacultesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/facultes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/facultes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFacultesRead(ref IQueryable<Models.DMdel.Faculte> items);

        public async Task<IQueryable<Models.DMdel.Faculte>> GetFacultes(Query query = null)
        {
            var items = Context.Facultes.AsQueryable();

            items = items.Include(i => i.Etablissement);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnFacultesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFaculteCreated(Models.DMdel.Faculte item);
        partial void OnAfterFaculteCreated(Models.DMdel.Faculte item);

        public async Task<Models.DMdel.Faculte> CreateFaculte(Models.DMdel.Faculte faculte)
        {
            OnFaculteCreated(faculte);

            var existingItem = Context.Facultes
                              .Where(i => i.Id == faculte.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Facultes.Add(faculte);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(faculte).State = EntityState.Detached;
                faculte.Etablissement = null;
                throw;
            }

            OnAfterFaculteCreated(faculte);

            return faculte;
        }
        public async Task ExportRangsEtabsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/rangsetabs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/rangsetabs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRangsEtabsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/rangsetabs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/rangsetabs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRangsEtabsRead(ref IQueryable<Models.DMdel.RangsEtab> items);

        public async Task<IQueryable<Models.DMdel.RangsEtab>> GetRangsEtabs(Query query = null)
        {
            var items = Context.RangsEtabs.AsQueryable();

            items = items.Include(i => i.Etablissement);

            items = items.Include(i => i.Session);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnRangsEtabsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRangsEtabCreated(Models.DMdel.RangsEtab item);
        partial void OnAfterRangsEtabCreated(Models.DMdel.RangsEtab item);

        public async Task<Models.DMdel.RangsEtab> CreateRangsEtab(Models.DMdel.RangsEtab rangsEtab)
        {
            OnRangsEtabCreated(rangsEtab);

            var existingItem = Context.RangsEtabs
                              .Where(i => i.Id == rangsEtab.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.RangsEtabs.Add(rangsEtab);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(rangsEtab).State = EntityState.Detached;
                rangsEtab.Etablissement = null;
                rangsEtab.Session = null;
                throw;
            }

            OnAfterRangsEtabCreated(rangsEtab);

            return rangsEtab;
        }
        public async Task ExportRegionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/regions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/regions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRegionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/regions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/regions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRegionsRead(ref IQueryable<Models.DMdel.Region> items);

        public async Task<IQueryable<Models.DMdel.Region>> GetRegions(Query query = null)
        {
            var items = Context.Regions.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnRegionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRegionCreated(Models.DMdel.Region item);
        partial void OnAfterRegionCreated(Models.DMdel.Region item);

        public async Task<Models.DMdel.Region> CreateRegion(Models.DMdel.Region region)
        {
            OnRegionCreated(region);

            var existingItem = Context.Regions
                              .Where(i => i.Id == region.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Regions.Add(region);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(region).State = EntityState.Detached;
                throw;
            }

            OnAfterRegionCreated(region);

            return region;
        }
        public async Task ExportRubriquesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/rubriques/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/rubriques/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRubriquesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/rubriques/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/rubriques/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRubriquesRead(ref IQueryable<Models.DMdel.Rubrique> items);

        public async Task<IQueryable<Models.DMdel.Rubrique>> GetRubriques(Query query = null)
        {
            var items = Context.Rubriques.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnRubriquesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRubriqueCreated(Models.DMdel.Rubrique item);
        partial void OnAfterRubriqueCreated(Models.DMdel.Rubrique item);

        public async Task<Models.DMdel.Rubrique> CreateRubrique(Models.DMdel.Rubrique rubrique)
        {
            OnRubriqueCreated(rubrique);

            var existingItem = Context.Rubriques
                              .Where(i => i.Id == rubrique.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Rubriques.Add(rubrique);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(rubrique).State = EntityState.Detached;
                throw;
            }

            OnAfterRubriqueCreated(rubrique);

            return rubrique;
        }
        public async Task ExportSessionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/sessions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/sessions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSessionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/sessions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/sessions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSessionsRead(ref IQueryable<Models.DMdel.Session> items);

        public async Task<IQueryable<Models.DMdel.Session>> GetSessions(Query query = null)
        {
            var items = Context.Sessions.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnSessionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSessionCreated(Models.DMdel.Session item);
        partial void OnAfterSessionCreated(Models.DMdel.Session item);

        public async Task<Models.DMdel.Session> CreateSession(Models.DMdel.Session session)
        {
            OnSessionCreated(session);

            var existingItem = Context.Sessions
                              .Where(i => i.Id == session.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Sessions.Add(session);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(session).State = EntityState.Detached;
                throw;
            }

            OnAfterSessionCreated(session);

            return session;
        }
        public async Task ExportStagiairesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/stagiaires/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/stagiaires/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportStagiairesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/dmdel/stagiaires/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/dmdel/stagiaires/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnStagiairesRead(ref IQueryable<Models.DMdel.Stagiaire> items);

        public async Task<IQueryable<Models.DMdel.Stagiaire>> GetStagiaires(Query query = null)
        {
            var items = Context.Stagiaires.AsQueryable();

            items = items.Include(i => i.Etablissement);

            items = items.Include(i => i.Faculte);

            items = items.Include(i => i.Departement);

            items = items.Include(i => i.Session);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

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

            OnStagiairesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnStagiaireCreated(Models.DMdel.Stagiaire item);
        partial void OnAfterStagiaireCreated(Models.DMdel.Stagiaire item);

        public async Task<Models.DMdel.Stagiaire> CreateStagiaire(Models.DMdel.Stagiaire stagiaire)
        {
            OnStagiaireCreated(stagiaire);

            var existingItem = Context.Stagiaires
                              .Where(i => i.Id == stagiaire.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Stagiaires.Add(stagiaire);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(stagiaire).State = EntityState.Detached;
                stagiaire.Etablissement = null;
                stagiaire.Faculte = null;
                stagiaire.Departement = null;
                stagiaire.Session = null;
                throw;
            }

            OnAfterStagiaireCreated(stagiaire);

            return stagiaire;
        }

        partial void OnCritereDeleted(Models.DMdel.Critere item);
        partial void OnAfterCritereDeleted(Models.DMdel.Critere item);

        public async Task<Models.DMdel.Critere> DeleteCritere(int? id)
        {
            var itemToDelete = Context.Criteres
                              .Where(i => i.Id == id)
                              .Include(i => i.Evaluations)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCritereDeleted(itemToDelete);

            Context.Criteres.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCritereDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnCritereGet(Models.DMdel.Critere item);

        public async Task<Models.DMdel.Critere> GetCritereById(int? id)
        {
            var items = Context.Criteres
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Element);

            var itemToReturn = items.FirstOrDefault();

            OnCritereGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Critere> CancelCritereChanges(Models.DMdel.Critere item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCritereUpdated(Models.DMdel.Critere item);
        partial void OnAfterCritereUpdated(Models.DMdel.Critere item);

        public async Task<Models.DMdel.Critere> UpdateCritere(int? id, Models.DMdel.Critere critere)
        {
            OnCritereUpdated(critere);

            var itemToUpdate = Context.Criteres
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(critere);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterCritereUpdated(critere);

            return critere;
        }

        partial void OnDepartementDeleted(Models.DMdel.Departement item);
        partial void OnAfterDepartementDeleted(Models.DMdel.Departement item);

        public async Task<Models.DMdel.Departement> DeleteDepartement(int? id)
        {
            var itemToDelete = Context.Departements
                              .Where(i => i.Id == id)
                              .Include(i => i.Stagiaires)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnDepartementDeleted(itemToDelete);

            Context.Departements.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDepartementDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnDepartementGet(Models.DMdel.Departement item);

        public async Task<Models.DMdel.Departement> GetDepartementById(int? id)
        {
            var items = Context.Departements
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Faculte);

            var itemToReturn = items.FirstOrDefault();

            OnDepartementGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Departement> CancelDepartementChanges(Models.DMdel.Departement item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDepartementUpdated(Models.DMdel.Departement item);
        partial void OnAfterDepartementUpdated(Models.DMdel.Departement item);

        public async Task<Models.DMdel.Departement> UpdateDepartement(int? id, Models.DMdel.Departement departement)
        {
            OnDepartementUpdated(departement);

            var itemToUpdate = Context.Departements
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(departement);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterDepartementUpdated(departement);

            return departement;
        }

        partial void OnEchelleDeleted(Models.DMdel.Echelle item);
        partial void OnAfterEchelleDeleted(Models.DMdel.Echelle item);

        public async Task<Models.DMdel.Echelle> DeleteEchelle(string id)
        {
            var itemToDelete = Context.Echelles
                              .Where(i => i.Id == id)
                              .Include(i => i.Evaluations)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEchelleDeleted(itemToDelete);

            Context.Echelles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEchelleDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnEchelleGet(Models.DMdel.Echelle item);

        public async Task<Models.DMdel.Echelle> GetEchelleById(string id)
        {
            var items = Context.Echelles
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            var itemToReturn = items.FirstOrDefault();

            OnEchelleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Echelle> CancelEchelleChanges(Models.DMdel.Echelle item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEchelleUpdated(Models.DMdel.Echelle item);
        partial void OnAfterEchelleUpdated(Models.DMdel.Echelle item);

        public async Task<Models.DMdel.Echelle> UpdateEchelle(string id, Models.DMdel.Echelle echelle)
        {
            OnEchelleUpdated(echelle);

            var itemToUpdate = Context.Echelles
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(echelle);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterEchelleUpdated(echelle);

            return echelle;
        }

        partial void OnElementDeleted(Models.DMdel.Element item);
        partial void OnAfterElementDeleted(Models.DMdel.Element item);

        public async Task<Models.DMdel.Element> DeleteElement(int? id)
        {
            var itemToDelete = Context.Elements
                              .Where(i => i.Id == id)
                              .Include(i => i.Criteres)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnElementDeleted(itemToDelete);

            Context.Elements.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterElementDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnElementGet(Models.DMdel.Element item);

        public async Task<Models.DMdel.Element> GetElementById(int? id)
        {
            var items = Context.Elements
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Rubrique);

            var itemToReturn = items.FirstOrDefault();

            OnElementGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Element> CancelElementChanges(Models.DMdel.Element item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnElementUpdated(Models.DMdel.Element item);
        partial void OnAfterElementUpdated(Models.DMdel.Element item);

        public async Task<Models.DMdel.Element> UpdateElement(int? id, Models.DMdel.Element element)
        {
            OnElementUpdated(element);

            var itemToUpdate = Context.Elements
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(element);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterElementUpdated(element);

            return element;
        }

        partial void OnEtablissementDeleted(Models.DMdel.Etablissement item);
        partial void OnAfterEtablissementDeleted(Models.DMdel.Etablissement item);

        public async Task<Models.DMdel.Etablissement> DeleteEtablissement(int? id)
        {
            var itemToDelete = Context.Etablissements
                              .Where(i => i.Id == id)
                              .Include(i => i.Facultes)
                              .Include(i => i.RangsEtabs)
                              .Include(i => i.Stagiaires)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEtablissementDeleted(itemToDelete);

            Context.Etablissements.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEtablissementDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnEtablissementGet(Models.DMdel.Etablissement item);

        public async Task<Models.DMdel.Etablissement> GetEtablissementById(int? id)
        {
            var items = Context.Etablissements
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Region);

            var itemToReturn = items.FirstOrDefault();

            OnEtablissementGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Etablissement> CancelEtablissementChanges(Models.DMdel.Etablissement item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEtablissementUpdated(Models.DMdel.Etablissement item);
        partial void OnAfterEtablissementUpdated(Models.DMdel.Etablissement item);

        public async Task<Models.DMdel.Etablissement> UpdateEtablissement(int? id, Models.DMdel.Etablissement etablissement)
        {
            OnEtablissementUpdated(etablissement);

            var itemToUpdate = Context.Etablissements
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(etablissement);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterEtablissementUpdated(etablissement);

            return etablissement;
        }

        partial void OnEvaluationDeleted(Models.DMdel.Evaluation item);
        partial void OnAfterEvaluationDeleted(Models.DMdel.Evaluation item);

        public async Task<Models.DMdel.Evaluation> DeleteEvaluation(int? id)
        {
            var itemToDelete = Context.Evaluations
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEvaluationDeleted(itemToDelete);

            Context.Evaluations.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEvaluationDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnEvaluationGet(Models.DMdel.Evaluation item);

        public async Task<Models.DMdel.Evaluation> GetEvaluationById(int? id)
        {
            var items = Context.Evaluations
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Stagiaire);

            items = items.Include(i => i.Critere);

            items = items.Include(i => i.Echelle);

            var itemToReturn = items.FirstOrDefault();

            OnEvaluationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Evaluation> CancelEvaluationChanges(Models.DMdel.Evaluation item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEvaluationUpdated(Models.DMdel.Evaluation item);
        partial void OnAfterEvaluationUpdated(Models.DMdel.Evaluation item);

        public async Task<Models.DMdel.Evaluation> UpdateEvaluation(int? id, Models.DMdel.Evaluation evaluation)
        {
            OnEvaluationUpdated(evaluation);

            var itemToUpdate = Context.Evaluations
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(evaluation);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterEvaluationUpdated(evaluation);

            return evaluation;
        }

        partial void OnFaculteDeleted(Models.DMdel.Faculte item);
        partial void OnAfterFaculteDeleted(Models.DMdel.Faculte item);

        public async Task<Models.DMdel.Faculte> DeleteFaculte(int? id)
        {
            var itemToDelete = Context.Facultes
                              .Where(i => i.Id == id)
                              .Include(i => i.Departements)
                              .Include(i => i.Stagiaires)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnFaculteDeleted(itemToDelete);

            Context.Facultes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFaculteDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnFaculteGet(Models.DMdel.Faculte item);

        public async Task<Models.DMdel.Faculte> GetFaculteById(int? id)
        {
            var items = Context.Facultes
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Etablissement);

            var itemToReturn = items.FirstOrDefault();

            OnFaculteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Faculte> CancelFaculteChanges(Models.DMdel.Faculte item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFaculteUpdated(Models.DMdel.Faculte item);
        partial void OnAfterFaculteUpdated(Models.DMdel.Faculte item);

        public async Task<Models.DMdel.Faculte> UpdateFaculte(int? id, Models.DMdel.Faculte faculte)
        {
            OnFaculteUpdated(faculte);

            var itemToUpdate = Context.Facultes
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(faculte);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterFaculteUpdated(faculte);

            return faculte;
        }

        partial void OnRangsEtabDeleted(Models.DMdel.RangsEtab item);
        partial void OnAfterRangsEtabDeleted(Models.DMdel.RangsEtab item);

        public async Task<Models.DMdel.RangsEtab> DeleteRangsEtab(int? id)
        {
            var itemToDelete = Context.RangsEtabs
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRangsEtabDeleted(itemToDelete);

            Context.RangsEtabs.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRangsEtabDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnRangsEtabGet(Models.DMdel.RangsEtab item);

        public async Task<Models.DMdel.RangsEtab> GetRangsEtabById(int? id)
        {
            var items = Context.RangsEtabs
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Etablissement);

            items = items.Include(i => i.Session);

            var itemToReturn = items.FirstOrDefault();

            OnRangsEtabGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.RangsEtab> CancelRangsEtabChanges(Models.DMdel.RangsEtab item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRangsEtabUpdated(Models.DMdel.RangsEtab item);
        partial void OnAfterRangsEtabUpdated(Models.DMdel.RangsEtab item);

        public async Task<Models.DMdel.RangsEtab> UpdateRangsEtab(int? id, Models.DMdel.RangsEtab rangsEtab)
        {
            OnRangsEtabUpdated(rangsEtab);

            var itemToUpdate = Context.RangsEtabs
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rangsEtab);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterRangsEtabUpdated(rangsEtab);

            return rangsEtab;
        }

        partial void OnRegionDeleted(Models.DMdel.Region item);
        partial void OnAfterRegionDeleted(Models.DMdel.Region item);

        public async Task<Models.DMdel.Region> DeleteRegion(int? id)
        {
            var itemToDelete = Context.Regions
                              .Where(i => i.Id == id)
                              .Include(i => i.Etablissements)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRegionDeleted(itemToDelete);

            Context.Regions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRegionDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnRegionGet(Models.DMdel.Region item);

        public async Task<Models.DMdel.Region> GetRegionById(int? id)
        {
            var items = Context.Regions
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            var itemToReturn = items.FirstOrDefault();

            OnRegionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Region> CancelRegionChanges(Models.DMdel.Region item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRegionUpdated(Models.DMdel.Region item);
        partial void OnAfterRegionUpdated(Models.DMdel.Region item);

        public async Task<Models.DMdel.Region> UpdateRegion(int? id, Models.DMdel.Region region)
        {
            OnRegionUpdated(region);

            var itemToUpdate = Context.Regions
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(region);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterRegionUpdated(region);

            return region;
        }

        partial void OnRubriqueDeleted(Models.DMdel.Rubrique item);
        partial void OnAfterRubriqueDeleted(Models.DMdel.Rubrique item);

        public async Task<Models.DMdel.Rubrique> DeleteRubrique(int? id)
        {
            var itemToDelete = Context.Rubriques
                              .Where(i => i.Id == id)
                              .Include(i => i.Elements)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRubriqueDeleted(itemToDelete);

            Context.Rubriques.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRubriqueDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnRubriqueGet(Models.DMdel.Rubrique item);

        public async Task<Models.DMdel.Rubrique> GetRubriqueById(int? id)
        {
            var items = Context.Rubriques
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            var itemToReturn = items.FirstOrDefault();

            OnRubriqueGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Rubrique> CancelRubriqueChanges(Models.DMdel.Rubrique item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRubriqueUpdated(Models.DMdel.Rubrique item);
        partial void OnAfterRubriqueUpdated(Models.DMdel.Rubrique item);

        public async Task<Models.DMdel.Rubrique> UpdateRubrique(int? id, Models.DMdel.Rubrique rubrique)
        {
            OnRubriqueUpdated(rubrique);

            var itemToUpdate = Context.Rubriques
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(rubrique);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterRubriqueUpdated(rubrique);

            return rubrique;
        }

        partial void OnSessionDeleted(Models.DMdel.Session item);
        partial void OnAfterSessionDeleted(Models.DMdel.Session item);

        public async Task<Models.DMdel.Session> DeleteSession(int? id)
        {
            var itemToDelete = Context.Sessions
                              .Where(i => i.Id == id)
                              .Include(i => i.RangsEtabs)
                              .Include(i => i.Stagiaires)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSessionDeleted(itemToDelete);

            Context.Sessions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSessionDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnSessionGet(Models.DMdel.Session item);

        public async Task<Models.DMdel.Session> GetSessionById(int? id)
        {
            var items = Context.Sessions
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            var itemToReturn = items.FirstOrDefault();

            OnSessionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Session> CancelSessionChanges(Models.DMdel.Session item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSessionUpdated(Models.DMdel.Session item);
        partial void OnAfterSessionUpdated(Models.DMdel.Session item);

        public async Task<Models.DMdel.Session> UpdateSession(int? id, Models.DMdel.Session session)
        {
            OnSessionUpdated(session);

            var itemToUpdate = Context.Sessions
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(session);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterSessionUpdated(session);

            return session;
        }

        partial void OnStagiaireDeleted(Models.DMdel.Stagiaire item);
        partial void OnAfterStagiaireDeleted(Models.DMdel.Stagiaire item);

        public async Task<Models.DMdel.Stagiaire> DeleteStagiaire(int? id)
        {
            var itemToDelete = Context.Stagiaires
                              .Where(i => i.Id == id)
                              .Include(i => i.Evaluations)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnStagiaireDeleted(itemToDelete);

            Context.Stagiaires.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterStagiaireDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnStagiaireGet(Models.DMdel.Stagiaire item);

        public async Task<Models.DMdel.Stagiaire> GetStagiaireById(int? id)
        {
            var items = Context.Stagiaires
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Etablissement);

            items = items.Include(i => i.Faculte);

            items = items.Include(i => i.Departement);

            items = items.Include(i => i.Session);

            var itemToReturn = items.FirstOrDefault();

            OnStagiaireGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.DMdel.Stagiaire> CancelStagiaireChanges(Models.DMdel.Stagiaire item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnStagiaireUpdated(Models.DMdel.Stagiaire item);
        partial void OnAfterStagiaireUpdated(Models.DMdel.Stagiaire item);

        public async Task<Models.DMdel.Stagiaire> UpdateStagiaire(int? id, Models.DMdel.Stagiaire stagiaire)
        {
            OnStagiaireUpdated(stagiaire);

            var itemToUpdate = Context.Stagiaires
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(stagiaire);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterStagiaireUpdated(stagiaire);

            return stagiaire;
        }
    }
}
