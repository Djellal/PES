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
        public async Task<FileStreamResult> ExportApplicationUsersToExcel(
            string fileName = null,
            [FromQuery] string[] roles = null,
            [FromQuery] int? etabid = null)
        {
            IEnumerable<ApplicationUser> users;

            // Apply filtering based on provided roles and Etabid
            if ((roles != null && roles.Length > 0) && etabid != null)
            {
                users = await _securityService.GetUsersInRoleAndEtab(roles, etabid);
            }
            else if (etabid != null)
            {
                users = await _securityService.GetUsersOfEtab(etabid, null);
            }
            else if (roles != null && roles.Length > 0)
            {
                // If multiple roles, aggregate users from all roles
                var usersList = new List<ApplicationUser>();
                foreach (var role in roles)
                {
                    var roleUsers = await _securityService.GetUsersInRole(role);
                    usersList.AddRange(roleUsers);
                }
                users = usersList.Distinct();
            }
            else
            {
                users = await _securityService.GetUsers();
            }

            List<UserExportDTO> exportData = new List<UserExportDTO>();
            foreach (var u in users)
            {
                var userRoles = await _securityService.GetRolesOfUser(u);
                exportData.Add(new UserExportDTO
                {
                    UserName = u.UserName,
                    Etablissement = GetEtablissementName(u.Etabid),
                    RoleNames = userRoles != null ? string.Join(", ", userRoles) : string.Empty
                });
            }

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