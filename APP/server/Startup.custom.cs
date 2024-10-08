using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Pes
{
    public partial class Startup
    {
        partial void OnConfigureServices(IServiceCollection services)
        {

            var cultureInfo = new CultureInfo("fr-FR");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            services.Configure<RequestLocalizationOptions>(options =>
            {

                options.SupportedCultures[0].NumberFormat.NumberDecimalSeparator = ".";

            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddControllers();

            services.AddScoped<CircuitHandler, CustomCircuitHandler>();

            services.Configure<SmtpSettings>(Configuration.GetSection("Smtp"));

            services.AddScoped<EmailSender>();
            services.AddScoped<DocumentService>();
        }

        partial void OnConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }

    }
}
