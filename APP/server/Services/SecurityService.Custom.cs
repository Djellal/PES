using Microsoft.EntityFrameworkCore;
using NPOI.Util;
using Pes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
namespace Pes
{
    public partial class SecurityService
    {
        public async Task<IEnumerable<ApplicationUser>> GetUsersInRole(string role)
        {
            // roleManager
            var users =  await  userManager.GetUsersInRoleAsync(role);
            return await Task.FromResult(users);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAndEtab(string[] roles,int? etabid)
        {
            // roleManager
            List<ApplicationUser> users = new List<ApplicationUser>();

            foreach (var r in roles.ToList())
            {
                users.AddRange((await userManager.GetUsersInRoleAsync(r)).Where(u => u.Etabid == etabid).ToList());
            }
                   
            
            return await Task.FromResult(users);
        }


        public async Task<IEnumerable<ApplicationUser>> GetUsersOfEtab(int? etabid,string role)
        {
            List<ApplicationUser> users;
            if (string.IsNullOrEmpty(role))
            {
                users = ((await GetUsers()).ToList());
            }
            
            else

                users =((await GetUsersInRole(role)).ToList());

            if(etabid.HasValue)
                users = users.Where(u => u.Etabid == etabid).ToList();



            return await Task.FromResult(users);
        }

        public async Task<IEnumerable<string>> GetRolesOfUser(ApplicationUser user)
        {
            var res = await userManager.GetRolesAsync(user);


            return await Task.FromResult(res);
        }

        /// <summary>
        /// Retourne true si l'utilisateur courant est admin global OU admin_regional
        /// </summary>
        public bool IsAdminOrRegional()
            => IsInRole(Constants.admin) || IsInRole(Constants.admin_regional);

        /// <summary>
        /// Retourne le Regid de l'admin_regional, ou null pour l'admin global (= pas de filtre région)
        /// </summary>
        public int? GetAdminRegionId()
            => IsInRole(Constants.admin) ? null : User?.Regid;

        /// <summary>
        /// Retourne les utilisateurs dont la propre Regid correspond à la région donnée,
        /// optionnellement filtrés par rôle.
        /// </summary>
        public async Task<IEnumerable<ApplicationUser>> GetUsersOfRegion(int? regid, string role)
        {
            List<ApplicationUser> users;
            if (string.IsNullOrEmpty(role))
                users = (await GetUsers()).ToList();
            else
                users = (await GetUsersInRole(role)).ToList();

            if (regid.HasValue)
                users = users.Where(u => u.Regid == regid).ToList();

            return await Task.FromResult(users);
        }
    }
}
