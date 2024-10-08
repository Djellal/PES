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


        public async Task<IEnumerable<ApplicationUser>> GetUsersOfEtab(int? etabid)
        {
           

            List<ApplicationUser> users =((await GetUsers()).Where(u => u.Etabid == etabid).ToList());



            return await Task.FromResult(users);
        }

        public async Task<IEnumerable<string>> GetRolesOfUser(ApplicationUser user)
        {
            var res = await userManager.GetRolesAsync(user);


            return await Task.FromResult(res);
        }
    }
}
