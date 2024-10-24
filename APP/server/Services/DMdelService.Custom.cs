using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Pes.Data;
using Pes.Models;
using Pes.Models.DMdel;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pes
{
    public partial class DMdelService
    {
        public DMdelService(DMdelContext context, NavigationManager navigationManager, SecurityService security)
        {
            this.context = context;
            this.navigationManager = navigationManager;
            this.Security = security;
        }

        private readonly SecurityService Security;
        public DMdelContext DMContext
        {
            get
            {
                return this.context;
            }
        }


        public async System.Threading.Tasks.Task<Models.DMdel.Session> GetActiveSession()
        {
            return context.Sessions.FirstOrDefault(s => s.EnCours);
        }

        public async System.Threading.Tasks.Task CreateEvals(int StagId)
        {

            var stagiaire = await GetStagiaireById(StagId);


            var criteres = Context.Criteres.Include(ev => ev.Element).ThenInclude(e => e.Rubrique).ToList(); //(await DMdel. GetCriteres()).ToList();

            var membresjury = await Security.GetUsersInRoleAndEtab(new string[] { Constants.expert, Constants.membre_jury, Constants.president_jury }, stagiaire.Etabid);


            string NomElem, NomRubrique;

            List<Evaluation> evaluations = new List<Evaluation>();

            var Evals = Context.Evaluations.Where(ev => ev.Stagid == StagId).ToList();

            foreach (var critr in criteres)
            {

                NomElem = critr.Element?.NomElement;
                NomRubrique = critr.Element?.Rubrique?.NomRubrique;

                if (!Evals.Any(v => v.Criterid == critr.Id && v.EstSynthese))
                {
                    evaluations.Add(new Models.DMdel.Evaluation { Criterid = critr.Id, Stagid = StagId, MembreId = string.Empty, NomElement = NomElem, NomRubrique = NomRubrique, EstSynthese = true });

                }

                foreach (var membre in membresjury)
                {
                    if (!Evals.Any(v => v.Criterid == critr.Id && v.MembreId == membre.Id && !v.EstSynthese))
                    {
                        evaluations.Add(new Models.DMdel.Evaluation { Criterid = critr.Id, Stagid = StagId, MembreId = membre.Id, NomElement = NomElem, NomRubrique = NomRubrique, EstSynthese = false });
                    }
                }
            }

            foreach (var item in Evals)
            {
                if (!criteres.Any(c => c.Id == item.Criterid)) Context.Remove(item);
            }

            await Context.AddRangeAsync(evaluations);
            await Context.SaveChangesAsync();

        }

        protected async System.Threading.Tasks.Task CalculerMoyeneCritere(int stagiairId, int criterid, List<ApplicationUser> Membresjury)
        {
            var AllEvals = Context.Evaluations.Include(ev => ev.Echelle).Where(ev => ev.Stagid == stagiairId && ev.Criterid == criterid);
            if (!AllEvals.Any()) return;
            double somme = 0;
            double count = 0;
            foreach (var item in AllEvals)
            {

                if (item.Echelle != null)
                    if (!item.EstSynthese && Membresjury.Any(m => m.Id == item.MembreId))
                    {
                        somme += item.Echelle.Val;
                        count++;
                    }
            }

            var synthese = AllEvals.FirstOrDefault(s => s.EstSynthese);

            if (synthese != null)
            {
                synthese.NoteSynthese = somme / count;
                await Context.SaveChangesAsync();

            }

        }

        public async System.Threading.Tasks.Task CalculerNote(int StagId)
        {
           
                var stagiaire = Context.Stagiaires.FirstOrDefault(s=>s.Id == StagId);
                if (stagiaire == null) return;

                var criteres = Context.Criteres.ToList();

               var rubriques = Context.Rubriques.ToList();



                var Membresjury = (await Security.GetUsersInRoleAndEtab(new string[] { Constants.membre_jury, Constants.president_jury, Constants.expert }, stagiaire.Etabid)).ToList();

                foreach ( var item in criteres)
                {
                    await CalculerMoyeneCritere(stagiaire.Id, item.Id, Membresjury);
                }

                var AllSyntheseEvals = Context.Evaluations.Include(ev => ev.Echelle).Include(ev => ev.Critere).Where(ev => ev.Stagid == stagiaire.Id && ev.EstSynthese).ToList();


                if (AllSyntheseEvals.Count() == 0) { stagiaire.Note = 0; return; }

                double s = 0, cnt = 0;
                foreach (var item in AllSyntheseEvals)
                {
                    cnt++;
                    if (!Double.IsNaN(item.NoteSynthese))
                        s += item.NoteSynthese;
                    else
                        s += 0;
                }

                if (stagiaire.CourEnligne != null)
                    if ((bool)stagiaire.CourEnligne && cnt != 0)
                        stagiaire.Note = s / (cnt * 5);
                    else stagiaire.Note = 0;

                stagiaire.NoteFinale = ((stagiaire.Note + stagiaire.NoteCC) / 2);



            await Context.SaveChangesAsync();           
        }

        public async System.Threading.Tasks.Task DeleteEvalsOfStagiaire(int id)
        {
            
                var evals = Context.Evaluations.Where(ev => ev.Stagid == id);
                Context.Evaluations.RemoveRange(evals);

                await DMContext.SaveChangesAsync();
            
        }


        public async System.Threading.Tasks.Task SetRefAttestation(List<Stagiaire> lst, Session session)
        {
            if (session == null) return ;
            
            int i = Context.Stagiaires.Count(s=>s.Sessionid == session.Id) - lst.Count+1;
           
            foreach (var item in lst)
            {
                item.RefAttestation = session.CodeSession + "/" + i++.ToString("D4");
                while (Context.Stagiaires.Any(s => s.Sessionid == session.Id && s.RefAttestation == item.RefAttestation))
                    item.RefAttestation = session.CodeSession + "/" + i++.ToString("D4");

                Context.Stagiaires.Entry(item).State = EntityState.Modified;
            }


           await Context.SaveChangesAsync() ;

        }
    }
}
