using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pes.Data;

namespace Pes
{
    public partial class ExportDMdelController : ExportController
    {
        private readonly DMdelContext context;
        private readonly DMdelService service;
        public ExportDMdelController(DMdelContext context, DMdelService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/DMdel/criteres/csv")]
        [HttpGet("/export/DMdel/criteres/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportCriteresToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCriteres(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/criteres/excel")]
        [HttpGet("/export/DMdel/criteres/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportCriteresToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCriteres(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/departements/csv")]
        [HttpGet("/export/DMdel/departements/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportDepartementsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDepartements(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/departements/excel")]
        [HttpGet("/export/DMdel/departements/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportDepartementsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDepartements(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/echelles/csv")]
        [HttpGet("/export/DMdel/echelles/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportEchellesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEchelles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/echelles/excel")]
        [HttpGet("/export/DMdel/echelles/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportEchellesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEchelles(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/elements/csv")]
        [HttpGet("/export/DMdel/elements/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportElementsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetElements(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/elements/excel")]
        [HttpGet("/export/DMdel/elements/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportElementsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetElements(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/etablissements/csv")]
        [HttpGet("/export/DMdel/etablissements/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportEtablissementsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEtablissements(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/etablissements/excel")]
        [HttpGet("/export/DMdel/etablissements/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportEtablissementsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEtablissements(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/evaluations/csv")]
        [HttpGet("/export/DMdel/evaluations/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportEvaluationsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEvaluations(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/evaluations/excel")]
        [HttpGet("/export/DMdel/evaluations/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportEvaluationsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEvaluations(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/facultes/csv")]
        [HttpGet("/export/DMdel/facultes/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportFacultesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFacultes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/facultes/excel")]
        [HttpGet("/export/DMdel/facultes/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportFacultesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFacultes(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/rangsetabs/csv")]
        [HttpGet("/export/DMdel/rangsetabs/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportRangsEtabsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRangsEtabs(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/rangsetabs/excel")]
        [HttpGet("/export/DMdel/rangsetabs/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportRangsEtabsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRangsEtabs(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/regions/csv")]
        [HttpGet("/export/DMdel/regions/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportRegionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRegions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/regions/excel")]
        [HttpGet("/export/DMdel/regions/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportRegionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRegions(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/rubriques/csv")]
        [HttpGet("/export/DMdel/rubriques/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportRubriquesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetRubriques(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/rubriques/excel")]
        [HttpGet("/export/DMdel/rubriques/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportRubriquesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetRubriques(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/sessions/csv")]
        [HttpGet("/export/DMdel/sessions/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportSessionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSessions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/sessions/excel")]
        [HttpGet("/export/DMdel/sessions/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportSessionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSessions(), Request.Query, false), fileName);
        }
        [HttpGet("/export/DMdel/stagiaires/csv")]
        [HttpGet("/export/DMdel/stagiaires/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportStagiairesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetStagiaires(), Request.Query, false), fileName);
        }

        [HttpGet("/export/DMdel/stagiaires/excel")]
        [HttpGet("/export/DMdel/stagiaires/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportStagiairesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetStagiaires(), Request.Query, false), fileName);
        }
    }
}
