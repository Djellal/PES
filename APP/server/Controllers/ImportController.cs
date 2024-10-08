using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using Pes.Models.DMdel;
using DocumentFormat.OpenXml.InkML;
using Pes.Data;
using DocumentFormat.OpenXml.Office2010.Ink;
using System.IO.Pipes;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;

namespace Pes
{
    public partial class ImportController : Controller
    {
        private DMdelService service;
        private DMdelContext context;
        private IWebHostEnvironment environment;

        public ImportController(IWebHostEnvironment environment, DMdelContext context, DMdelService service)
        {
            this.service = service;
            this.context = context;

        }

        // Single file upload
        [HttpPost("import/stagiaires/{etabid}/{sessionid}")]
        public  IActionResult stagiaires(IFormFile file, int etabid, int sessionid)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Aucun fichier n'est téléchargé.");


                IWorkbook workbook;

                workbook = new XSSFWorkbook(file.OpenReadStream());


                ISheet sheet = workbook.GetSheetAt(0);
                Stagiaire stg;
                List<Stagiaire> lstStags = new List<Stagiaire>();
                ICell cell;
                DateTime dt;
                int? facid, deptid;
                double cc;
                Stagiaire stagiare;

                var session = context.Sessions.FirstOrDefault(s=>s.Id == sessionid);
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        if (row.GetCell(0) == null || row.GetCell(1) == null) continue;


                        //facid = GetFaculteByName(etabid, row.GetCell(2)?.ToString());
                        //deptid = GetDepartementByName(facid, row.GetCell(3)?.ToString());


                       
                        

                        stg = new Stagiaire { Nom = row.GetCell(0)?.ToString(), Prenom = row.GetCell(1)?.ToString(), email= row.GetCell(2)?.ToString(), portefolio="", DateNaissance = DateTime.Now, Etabid = etabid,/*Facid = facid, Departid = deptid,*/ URLcour = row.GetCell(3)?.ToString(),CourEnligne=false, Sessionid = sessionid };

                       



                        if (Double.TryParse(row.GetCell(4)?.ToString(), out cc))
                        {
                            stg.NoteCC = cc;
                            if (stg.NoteCC > 1 && stg.NoteCC <= 100) stg.NoteCC = stg.NoteCC / 100;
                        }

                        stagiare = context.Stagiaires.FirstOrDefault(s => s.Nom == stg.Nom && s.Prenom == stg.Prenom && s.Etabid == stg.Etabid && s.Sessionid == stg.Sessionid);
                         
                        if (stagiare == null)
                        {
                            //int i = context.Stagiaires.Count(s => s.Sessionid == session.Id) + 1;
                            //stg.RefAttestation = session.CodeSession + "/" + i.ToString("D4");
                            //while (context.Stagiaires.Any(s => s.Sessionid == session.Id && s.RefAttestation == stg.RefAttestation))
                            //    stg.RefAttestation = session.CodeSession + "/" + i++.ToString("D4");

                            lstStags.Add(stg);
                        }
                        else
                        {
                            stagiare.Nom = stg.Nom; stagiare.Prenom = stg.Prenom; stagiare.email=stg.email; stagiare.Etabid = stg.Etabid; stagiare.portefolio = stg.portefolio;
                            stagiare.Sessionid = stg.Sessionid; stagiare.Facid = stg.Facid; stagiare.Departid = stg.Departid; stagiare.DateNaissance = stg.DateNaissance; 
                            stagiare.URLcour = stg.URLcour; stagiare.CourEnligne = stg.CourEnligne;stagiare.NoteCC = stg.NoteCC; stagiare.RefAttestation = stg.RefAttestation;
                            
                        }


                    }
                }

                workbook.Close();



                context.AddRange(lstStags);
                context.SaveChanges();




                return StatusCode(200);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, ex.Message);
            }
        }

        private int? GetFaculteByName(int etabid, string nomfac)
        {
            var res = context.Facultes.FirstOrDefault(f => f.Etabid == etabid && f.NomFaculte == nomfac);

            if (res != null) return res.Id;
            else return null;
        }

        private int? GetDepartementByName(int? facid, string nomDept)
        {
            var res = context.Departements.FirstOrDefault(f => f.Facid == facid && f.NomDepartement == nomDept);

            if (res != null) return res.Id;
            else return null;
        }
    }
}