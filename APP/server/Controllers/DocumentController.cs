using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.IO;
using Syncfusion.Drawing;
using System.Globalization;
using System.Collections.Generic;
using Pes.Models.DMdel;
using System;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;


namespace Pes
{
    public class DocumentController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly DocumentService documentService;
        private readonly DMdelService context;
        public DocumentController(IWebHostEnvironment environment, DMdelService context, DocumentService documentService)
        {
            this.context = context;
            this.environment = environment;
            this.documentService = documentService;

        }
        [HttpGet("/Document/Attestation")]
        public FileStreamResult Attestation([FromQuery] int Stagid)
        {
            try
            {
                                
                return documentService.Attestation(Stagid); ;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

       

        [HttpGet("/Document/AttestationsList(ids='{ids}')")]
        public IActionResult AttestationsList(string ids = null)
        {
            try
            {

                return documentService.AttestationsList(ids); ;
            }
            catch (Exception ex)
            {

                return null;
            }
            
        }


        public IActionResult SendEmailWithAttachment(int Stagid)
        {
            try
            {
                var stagiaire = context.DMContext.Stagiaires.Find(Stagid);

                if (stagiaire == null) return StatusCode(404);


                var fileStream = Attestation(Stagid).FileStream;

                SendMail(stagiaire, fileStream);

                return StatusCode(200);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        private static void SendMail(Stagiaire stagiaire, Stream fileStream)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MESRS - Formation TIC et pratiques pédagogiques", "digiweek@dual-mesrs.dz"));
            message.To.Add(new MailboxAddress(stagiaire.Nom + " " + stagiaire.Prenom, stagiaire.email));
            message.Subject = "Attestation de formation";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Bonjour <strong>{stagiaire.Nom} {stagiaire.Prenom}</strong>,<br><br>Félicitations !<br>Vous avez réussi votre formation <strong>TIC et pratiques pédagogiques</strong>.<br>Veuillez trouver ci-joint votre attestation.<br><br>Bon courage.<br><br><em>Ne pas répondre à ce message automatique</em>.</p>"
            };

            // Add the FileStream as an attachment
            bodyBuilder.Attachments.Add("attestation.pdf", fileStream, new ContentType("application", "pdf"));

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
            {
                smtpClient.Connect("mail.cloud.cerist.dz", 587, SecureSocketOptions.StartTls);

                smtpClient.Authenticate("digiweek@dual-mesrs.dz", "jAc(R6B2A5");
                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }
        }



        [HttpGet("/Document/SendAttestationsList(ids='{ids}')")]
        public IActionResult SendAttestationsList(string ids = null)
        {
            string StatatusMsg = string.Empty;
            Stagiaire stagiaire;
            var lst = ids.Split(';');
            foreach (var item in lst)
            {

                try
                {
                    stagiaire = context.DMContext.Stagiaires.Find(Int32.Parse(item));

                    if (stagiaire == null) continue;


                    var fileStream = Attestation(stagiaire.Id).FileStream;

                    SendMail(stagiaire, fileStream);
                }
                catch (Exception ex)
                {

                    StatatusMsg+= ex.Message+"\r\n";
                }
            }


            return StatusCode(200, StatatusMsg);
        }


        
    }
}
