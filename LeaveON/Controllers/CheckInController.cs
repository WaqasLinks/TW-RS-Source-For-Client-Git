using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TourneyRepo.Models;
using System.Net.Mail;
using LeaveON.Models;
using Microsoft.AspNet.Identity;

namespace LeaveON.Controllers
{
  public class CheckInController : Controller
  {
    private TourneyWizardEntities db = new TourneyWizardEntities();

    // GET: Configurations
    [Authorize]
    public async Task<ActionResult> Index()
    {
      string userId = User.Identity.GetUserId();
      var events = new List<ScheduledEvent>();
      if (User.IsInRole("Admin")) events = await db.ScheduledEvents.ToListAsync();
      else events = await db.ScheduledEvents.Where(x => x.CreatedBy == userId).ToListAsync();
      return View(events);
    }

    public async Task<JsonResult> EventDataBYTournamentById(int Id)
    {

      dynamic data;

      if (Id == 0)
      {

        data = await db.ScheduledEvents.Select(x => new { Id = x.Id, Name = x.Name }).ToListAsync();

      }
      else
      {

        data = await db.ScheduledEvents.Where(x => x.TournamentId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToListAsync();
      }

      return Json(data, JsonRequestBehavior.AllowGet);
    }


    public JsonResult GetAllCompetitorDataByEventId(int Id, int? TournamentId)
    {

      dynamic Data="";
      string userId = User.Identity.GetUserId();

      //if (Id == 0 && TournamentId > 0)
      //{

      //  var EventId = db.ScheduledEvents.Where(x => x.TournamentId == TournamentId).Select(x => x.Id).ToList();

      //  var CompetitorsId = db.CompetitorEventRelations.Where(x => EventId.Any(c => c == x.EventId)).Select(x => x.CompetitorId).ToList();

      //  Data = (from C in db.Competitors.Where(x => CompetitorsId.Any(C => C == x.Id)).ToList()
      //          join Q in db.QRCodeDetails
      //          on C.Id equals Q.CompetitorId
      //          select new
      //          {
      //            Id = C.Id,
      //            QRID = Q.QRCodeId,
      //            Name = C.Name,
      //            Age = C.Age,
      //            Weight = C.Weight,
      //            Gender = C.Gender,
      //            State = C.State,
      //            Email = C.Email,
      //            IsVerify = Q.IsVerify
      //          })?.ToList();


      //}
      if (Id == 0)
      {

        var Competitors__Id = new List<int?>();
        if (User.IsInRole("Admin")) Competitors__Id = db.DivisionGroups.Select(x => x.CompetitorId).ToList();
        else Competitors__Id = db.DivisionGroups.Where(x => x.ScheduledEvent.CreatedBy == userId).Select(x => x.CompetitorId).ToList();


        Data = (from C in db.Competitors.Where(x => Competitors__Id.Any(C => C == x.Id))
                join Q in db.QRCodeDetails
                on C.Id equals Q.CompetitorId into qrGroup
                from Q in qrGroup.DefaultIfEmpty()
                select new
                {
                  Id = C.Id,
                  QRID = Q.QRCodeId,
                  Name = C.Name,
                  Age = C.Age,
                  Weight = C.Weight,
                  Gender = C.Gender,
                  State = C.State,
                  Email = C.Email,
                  IsVerify = Q.IsVerify
                })?.ToList();
      }
      else
      {
        var Competitor_Id = new List<int?>();
        if (User.IsInRole("Admin")) Competitor_Id = db.DivisionGroups.Where(x => x.TournamentId == Id).Select(x => x.CompetitorId).ToList();
        else Competitor_Id = db.DivisionGroups.Where(x => x.TournamentId == Id && x.ScheduledEvent.CreatedBy==userId).Select(x => x.CompetitorId).ToList();

        Data = (from C in db.Competitors.Where(x => Competitor_Id.Any(c => c == x.Id))
                join Q in db.QRCodeDetails 
                on C.Id equals Q.CompetitorId into qrGroup
                from Q in qrGroup.DefaultIfEmpty()
                select new
                {
                  Id = C.Id,
                  QRID = Q.QRCodeId,
                  Name = C.Name,
                  Age = C.Age,
                  Weight = C.Weight,
                  Gender = C.Gender,
                  State = C.State,
                  Email = C.Email,
                  IsVerify = Q.IsVerify
                }).ToList();
      }
      return Json(Data, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetQRCodeDetailsBy(string QRCodeId)
    {
      string userId = User.Identity.GetUserId();
      

      QRCodeId = "#" + QRCodeId;

      var Data = (from C in db.Competitors
                  join Q in db.QRCodeDetails
                  on C.Id equals Q.CompetitorId
                  where (Q.QRCodeId == QRCodeId)
                  select new
                  {
                    Profile = C.ProfilePath,
                    Name = C.Name,
                    Age = C.Age,
                    Belt = db.Belts.FirstOrDefault(x => x.CreatedBy == userId && x.BeltId == Convert.ToInt32(C.Belt)).BeltName,
                    Weight = C.Weight,
                    Gender = C.Gender,
                    EventName = db.ScheduledEvents.Where(x => x.Id == Q.EventId).FirstOrDefault().Name,
                    QRCodeId = Q.QRCodeId
                  }).FirstOrDefault();

     
      if (Data != null)
      {
        QRCodeDetail QRDetails = db.QRCodeDetails.Where(x => x.QRCodeId == Data.QRCodeId).FirstOrDefault();

        if (QRDetails.IsVerify == true)
        {

          return Json(new { success = true, data = Data, message = "This Competitor is Already Verified" }, JsonRequestBehavior.AllowGet);


        }
        else
        {

          QRDetails.IsVerify = true;

          db.Entry(QRDetails).State = EntityState.Modified;

          db.SaveChanges();

          return Json(new { success = true, data = Data, message = "This Competitor is Verified" }, JsonRequestBehavior.AllowGet);
        }
      }

      return Json(new { success = true, data = Data, message = "" }, JsonRequestBehavior.AllowGet);


    }

    public JsonResult ISVerifyUpdate(int Id)
    {

      var QRDetails = db.QRCodeDetails.Where(x => x.CompetitorId == Id).FirstOrDefault();

      QRDetails.IsVerify = true;

      db.Entry(QRDetails).State = EntityState.Modified;

      db.SaveChanges();

      return Json(new { success = true, message = " IsVery QR Code IUpdate" }, JsonRequestBehavior.AllowGet);
    }

    public JsonResult SendEmail()
    {

      var Data = (from C in db.Competitors
                  join Q in db.QRCodeDetails
                  on C.Id equals Q.CompetitorId
                  where (C.Event == Q.EventId.ToString() && C.Id == Q.CompetitorId && Q.IsVerify == false)
                  select new
                  {
                    Email = C.Email,

                  }).ToList();

      if (Data.Count > 0)
      {
        foreach (var item in Data)
        {
          if (item.Email != null || item.Email != "")
          {
            //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            //smtpClient.Port = 587; // Set the SMTP port (e.g., 587 for Gmail)
            //smtpClient.UseDefaultCredentials = false;
            //smtpClient.EnableSsl = true; // Set SSL/TLS encryption

            //// Set your credentials (username and password)
            //smtpClient.Credentials = new NetworkCredential("khasnainali8@gmail.com", "tubnmcxkpvpxpzjm");

            //// Create the email message
            //MailMessage mailMessage = new MailMessage();
            //mailMessage.From = new MailAddress(item.Email);
            //mailMessage.To.Add(item.Email);
            //mailMessage.Subject = "Do not Attende Event";
            //mailMessage.Body = "Do not Attende Event.";

            //// Send the email
            //smtpClient.Send(mailMessage);
            EmailServiceModel.SendEmail(item.Email, "Do not Attende Event", "Do not Attende Event.");
          }

        }

        return Json(new { success = true, message = "Email Send Successfully" }, JsonRequestBehavior.AllowGet);

      }

      else
      {

        return Json(new { success = false, message = "No Atheletes Found." }, JsonRequestBehavior.AllowGet);


      }




    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
