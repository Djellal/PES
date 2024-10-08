using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Pes.Models.DMdel;
using Syncfusion.Drawing;

namespace Pes
{
    public class DocumentService
    {
        private DMdelService context;
        private IWebHostEnvironment environment;

        public DocumentService(IWebHostEnvironment environment, DMdelService context)
        {
            this.context = context;
            this.environment = environment;
        }
        public FileStreamResult Attestation(int Stagid)
        {
            var stagiaire = context.DMContext.Stagiaires.Find(Stagid);

            if (stagiaire == null) return null;



            PdfDocument document = new PdfDocument();
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;
            document.PageSettings.Margins.All = 10;
            PdfPage page = document.Pages.Add();



            var headerpath = Path.Combine(environment.WebRootPath, "assets", "images", "bg.png");
            FileStream imageStream = new FileStream(headerpath, FileMode.Open, FileAccess.Read);
            PdfImage image = PdfImage.FromStream(imageStream);
            //Draws the image to the PDF page

            DrawPage(page, image, stagiaire);



            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            //Download the PDF document in the browser
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
            fileStreamResult.FileDownloadName = "liste_attestations.pdf";
            return fileStreamResult;
        }

        private void DrawPage(PdfPage page, PdfImage image, Stagiaire stagiaire)
        {
            var etab = context.DMContext.Etablissements.Find(stagiaire.Etabid);
            var sess = context.DMContext.Sessions.Find(stagiaire.Sessionid);
            PdfGraphics g = page.Graphics;

            RectangleF rec = new RectangleF(0, 0, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height);


            page.Graphics.DrawImage(image, rec);


            PdfStringFormat format = new PdfStringFormat();


            PdfLayoutResult result = new PdfLayoutResult(page, new RectangleF(0, 0, page.Graphics.ClientSize.Width / 2, 95));
            PdfFont HeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 13, PdfFontStyle.Bold);
            PdfFont NormalFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 13);

            format.Alignment = PdfTextAlignment.Left;
            g.DrawString("Réf: " + stagiaire.RefAttestation, NormalFont, PdfBrushes.Black, new RectangleF(40, 150, page.Graphics.ClientSize.Width, 100), format);


            format.Alignment = PdfTextAlignment.Center;

            g.DrawString($"{stagiaire.Nom} {stagiaire.Prenom}", HeadingFont, PdfBrushes.Black, new RectangleF(0, 280, page.Graphics.ClientSize.Width, 100), format);

            g.DrawString(etab?.NomEtablissement, HeadingFont, PdfBrushes.Black, new RectangleF(0, 310, page.Graphics.ClientSize.Width, 100), format);

            format.Alignment = PdfTextAlignment.Left;

            g.DrawString("durant l'année universitaire " + sess?.NomSession + " (programme en annexe).", NormalFont, PdfBrushes.Black, new RectangleF(50, 375, page.Graphics.ClientSize.Width, 100), format);


            format.Alignment = PdfTextAlignment.Right;
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), NormalFont, PdfBrushes.Black, new RectangleF(0, 420, page.Graphics.ClientSize.Width - 130, 100), format);

        }

        public FileStreamResult AttestationsList(string ids = null)
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;
            document.PageSettings.Margins.All = 10;



            var headerpath = Path.Combine(environment.WebRootPath, "assets", "images", "bg.png");
            FileStream imageStream = new FileStream(headerpath, FileMode.Open, FileAccess.Read);
            PdfImage image = PdfImage.FromStream(imageStream);

            Stagiaire stagiaire;
            var lst = ids.Split('-');
            foreach (var item in lst)
            {
                PdfPage page = document.Pages.Add();
                PdfGraphics g = page.Graphics;

                stagiaire = context.DMContext.Stagiaires.Find(Int32.Parse(item));

                if (stagiaire == null) continue;


                DrawPage(page, image, stagiaire);
            }



            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            //Download the PDF document in the browser
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
            fileStreamResult.FileDownloadName = "liste_attestations.pdf";
            return fileStreamResult;
        }
    }
}
