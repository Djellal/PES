using DocumentFormat.OpenXml.InkML;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Pes.Models.DMdel;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Pes
{
    

    public class EmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly DMdelService context;
        private readonly DocumentService documentService;

        public EmailSender(IOptions<SmtpSettings> smtpSettings, DMdelService context, DocumentService documentService)
        {
            _smtpSettings = smtpSettings.Value;
            this.context = context;
            this.documentService = documentService;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
               
            };
           

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        


        public  async Task<string> SendEmailWithAttachment(int Stagid)
        {
            
                var stagiaire = context.DMContext.Stagiaires.Find(Stagid);

                if (stagiaire == null) return "Stagiaire introuvable";


                var fileStream = documentService.Attestation(Stagid).FileStream;

               return await SendMail(stagiaire, fileStream);               
           
        }

        public static async Task<string> SendMail(Stagiaire stagiaire, Stream fileStream)
        {
            string res = "";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MESRS - Formation TIC et pratiques pédagogiques", "digiweek@dual-mesrs.dz"));
            message.To.Add(new MailboxAddress(stagiaire.Nom + " " + stagiaire.Prenom, stagiaire.email.Trim()));
            message.Subject = "Attestation de formation";

            var bodyBuilder = new BodyBuilder
            {
                
                HtmlBody = 
                $"<p style=\"font-family: Arial, sans-serif; color: #333; line-height: 1.8; font-size: 1.1em;\">\r\n    Bonjour <strong style=\"color: #0056b3; font-size: 1.2em;\">{stagiaire.Nom} {stagiaire.Prenom}</strong>,<br><br>\r\n    <span style=\"font-size: 1.4em; color: #28a745;\">Félicitations !</span><br>\r\n    Vous avez réussi votre formation <strong style=\"color: #0056b3; font-size: 1.2em;\">TIC et pratiques pédagogiques</strong>.<br>\r\n    Veuillez trouver ci-joint votre attestation.<br><br>\r\n    Bon courage.<br><br>\r\n    <em style=\"color: #6c757d; font-size: 1em;\">Ne pas répondre à ce message automatique</em>.\r\n</p>"
            };

            // Add the FileStream as an attachment
            bodyBuilder.Attachments.Add("attestation.pdf", fileStream, new ContentType("application", "pdf"));

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
            {
                smtpClient.Connect("mail.cloud.cerist.dz", 587, SecureSocketOptions.StartTls);

                smtpClient.Authenticate("digiweek@dual-mesrs.dz", "jAc(R6B2A5");
                res= smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }

            return res;
        }




        public async Task<string> SendAttestationsList(string ids = null)
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


                    var fileStream = documentService.Attestation(stagiaire.Id).FileStream;

                    StatatusMsg+= await SendMail(stagiaire, fileStream)+ "\r\n";
                }
                catch (Exception ex)
                {

                    StatatusMsg += ex.Message + "\r\n";
                }

                
            }

            return StatatusMsg;

        }
    }

}


