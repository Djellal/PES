using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Web;
using Pes.Models.DMdel;

namespace Pes.Pages
{
    public partial class SessionsComponent
    {
        protected async System.Threading.Tasks.Task CalculerMoyenneClick(MouseEventArgs args, Session sess)
        {
            await DMdel.CalculerMoyenneGlobale(sess.Id);
            await grid0.Reload();
            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task SetActiveSession(Session sesscour)
        {
            if(!sesscour.EnCours ) return;

            var sessions = DMdel.DMContext.Sessions;

            foreach( var session in sessions ) {session.EnCours = false;}

            var ss=  sessions.FirstOrDefault(s => s.Id == sesscour.Id);
            ss.EnCours= true;


            DMdel.DMContext.SaveChanges();

            Globals.ActiveSession = ss;


            Reload();


        }
    }
}
