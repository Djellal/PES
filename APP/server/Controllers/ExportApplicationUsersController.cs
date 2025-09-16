using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pes.Data;
using Pes.Models;


namespace Pes
{
    public partial class ExportApplicationUsersController : ExportController
    {
        private readonly SecurityService _securityService;
        private readonly DMdelContext _context;

        public ExportApplicationUsersController(SecurityService securityService, DMdelContext context)
        {
            _securityService = securityService;
            _context = context;
        }

        [HttpGet("/export/applicationusers/excel")]
        [HttpGet("/export/applicationusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportApplicationUsersToExcel(string fileName = null)
        {
            // Get all users with their roles
            var users = await _securityService.GetUsers();

            List<UserExportDTO> exportData = new List<UserExportDTO>();
            IEnumerable<string> Roles;
            foreach (var u in users)
            {
                Roles = await _securityService.GetRolesOfUser(u);
                exportData.Add(new UserExportDTO
                {
                    UserName = u.UserName,
                    Etablissement = GetEtablissementName(u.Etabid),
                    RoleNames = Roles != null ? string.Join(", ", Roles) : string.Empty

                });
            }
            
            // Transform to DTO with username, etablissement, and role names
            //var exportData = users.Select(u => new UserExportDTO
            //{
            //    UserName = u.UserName,
            //    Etablissement = GetEtablissementName(u.Etabid),
            //    RoleNames = u.RoleNames != null ? string.Join(", ", u.RoleNames) : string.Empty
            //}).AsQueryable();



            return ToExcel(exportData.AsQueryable(), fileName);
        }

        private string GetEtablissementName(int? etabId)
        {
            if (etabId == null) return string.Empty;
            
            var etablissement = _context.Etablissements
                .FirstOrDefault(e => e.Id == etabId);
                
            return etablissement?.NomEtablissement ?? string.Empty;
        }
    }
}