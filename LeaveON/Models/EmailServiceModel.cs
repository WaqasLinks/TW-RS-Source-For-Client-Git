using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;

namespace LeaveON.Models
{
  public class EmailServiceModel
  {
    public static void SendEmail(string Email, string Subject, string Body)

    {
      try
      {
        SmtpSection Obj = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

        // Create the SMTP client
        SmtpClient smtpClient = new SmtpClient(Obj.Network.Host);
        smtpClient.Port = Obj.Network.Port; // Set the SMTP port (e.g., 587 for Gmail)
                                            smtpClient.UseDefaultCredentials = false;
                                            smtpClient.EnableSsl = Obj.Network.EnableSsl; // Set SSL/TLS encryption
        

        // Set your credentials (username and password)
        smtpClient.Credentials = new NetworkCredential(Obj.Network.UserName, Obj.Network.Password);

        // Create the email message
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(Obj.From);
        mailMessage.To.Add(Email);
        mailMessage.Subject = Subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = Body;

        // Send the email
        smtpClient.Send(mailMessage);
      }
      catch (Exception ex)
      {

      }
    }
  }
}
