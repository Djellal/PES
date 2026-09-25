using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pes.Models;

namespace Pes
{
    public partial class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment env;
        private readonly AuditService audit;

        public AccountController(IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AuditService audit)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.env = env;
            this.audit = audit;
        }

        private IActionResult RedirectWithError(string error, string redirectUrl)
        {
             if (!string.IsNullOrEmpty(redirectUrl))
             {
                 return Redirect($"~/Login?error={error}&redirectUrl={Uri.EscapeDataString(redirectUrl)}");
             }
             else
             {
                 return Redirect($"~/Login?error={error}");
             }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string redirectUrl)
        {
            if (env.EnvironmentName == "Development" && userName == "admin" && password == "admin")
            {
                var claims = new List<Claim>()
                {
                        new Claim(ClaimTypes.Name, "admin"),
                        new Claim(ClaimTypes.Email, "admin")
                };

                roleManager.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
                await signInManager.SignInWithClaimsAsync(new ApplicationUser { UserName = userName, Email = userName }, isPersistent: false, claims);

                await audit.LogAsync(AuditActions.Login, "Utilisateur", userName, $"Connexion de {userName} (dev)", new { userName, redirectUrl }, userName: userName);

                return Redirect($"~/{redirectUrl}");
            }

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {

                var result = await signInManager.PasswordSignInAsync(userName, password, false, false);

                if (result.Succeeded)
                {
                    await audit.LogAsync(AuditActions.Login, "Utilisateur", userName, $"Connexion de {userName}", new { userName, redirectUrl }, userName: userName);

                    return Redirect($"~/{redirectUrl}");
                }

                await audit.LogAsync(AuditActions.LoginFailed, "Utilisateur", userName, $"Échec de connexion de {userName}", new { userName, redirectUrl }, userName: userName);
            }

            return RedirectWithError("Invalid user or password", redirectUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Redirect("~/Login?error=Invalid user or password");
            }

            var user = new ApplicationUser { UserName = userName, Email = userName };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);

                await audit.LogAsync(AuditActions.Register, "Utilisateur", user.Id, $"Inscription de {userName}", new { userName }, userName: userName);

                return Redirect("~/");
            }

            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            await audit.LogAsync(AuditActions.Register, "Utilisateur", userName, $"Échec d'inscription de {userName} : {message}", new { userName, error = message }, userName: userName);

            return Redirect($"~/Login?error={message}");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            if (oldPassword == null || newPassword == null)
            {
                return Redirect($"~/Profile?error=Invalid old or new password");
            }

            var id = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await userManager.FindByIdAsync(id);

            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true);

                await audit.LogAsync(AuditActions.PasswordChange, "Utilisateur", id, $"Changement de mot de passe de {user.Email}", new { user.Email });

                return Redirect("~/");
            }

            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            await audit.LogAsync(AuditActions.PasswordChange, "Utilisateur", id, $"Échec de changement de mot de passe de {user.Email} : {message}", new { user.Email, error = message });

            return Redirect($"~/Profile?error={message}");
        }

        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name;

            await audit.LogAsync(AuditActions.Logout, "Utilisateur", userName, $"Déconnexion de {userName}", new { userName }, userName: userName);

            await signInManager.SignOutAsync();

            return Redirect("~/");
        }
    }
}
