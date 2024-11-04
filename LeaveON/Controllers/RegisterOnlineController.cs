using LeaveON.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.BuilderProperties;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using TourneyRepo.Models;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace LeaveON.Controllers
{
  public class RegisterOnlineController : Controller
  {
    public TourneyWizardEntities db = new TourneyWizardEntities();


    public ActionResult Index()
    {
      return View(db.Competitors.ToList());
    }

    [HttpGet]
    public ActionResult Registration(int id,int CompetitorId)
    {
      string userId = User.Identity.GetUserId();

      var records = db.ScheduledEvents.FirstOrDefault(x => x.Id == id);
      if (records != null)
      {
        //ViewBag.Events = db.ScheduledEvents.Where(x => x.TournamentId == records.TournamentId).ToList();
        //ViewBag.Event = records;
        ViewBag.Description = records.Description;
      }

      //ViewBag.CurrentEventId = id;
      // Competitor competitor= db.Competitors.Find(CompetitorId);
      // ViewBag.Belts = new SelectList(db.Belts.Where(x => x.CreatedBy == userId), "BeltId", "BeltName");

      ViewBag.recaptchaKey = db.Configurations.Where(x => x.Key == "RecaptchaKey").FirstOrDefault().Value;
      ViewBag.TournamentEvents = db.TournamentEventRealtions.Where(x => x.ScheduleEventId == id).ToList();
      var existingCompetitors = db.DivisionGroups.Where(x => x.TournamentId == id).Select(y => y.CompetitorId).ToList();
      db.Configuration.ProxyCreationEnabled = false;
      ViewBag.AllProfiles = db.Competitors.Where(x => !existingCompetitors.Contains(x.Id) && x.CreatedBy == userId).ToList();
      ViewBag.CurrentCompetId = CompetitorId;
      ViewBag.TournamentId = id;
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();

      ViewBag.EventFee = db.Configurations.FirstOrDefault(x => x.Key == "EventFee")?.Value;
      ViewBag.OtherEventFee = db.Configurations.FirstOrDefault(x => x.Key == "OtherEventFee")?.Value;

      //return View(competitor);
      return View();
    }

    public class Card
    {
      public string CardNo { get; set; }
      public int ExpMonth { get; set; }
      public int ExpYear { get; set; }
      public string CVC { get; set; }
      public string PayMethod { get; set; }
    }



    [HttpPost]
    public ActionResult Registration(Competitor _competitor, Card _card, List<TournamentRegisteration> _competitorEvents)
    {



      var payMethodservice = new PaymentMethodService();
      var payIntentservice = new PaymentIntentService();
      PaymentIntent paymentIntent = null;

      try
      {
        ////var payment = Convert.ToInt32(db.Configurations.FirstOrDefault(x => x.Key.Equals("EventFee")).Value) * _competitor.Event.Split(',').Length;
        int payment = 0;
        var eventFee = Convert.ToInt32(db.Configurations.FirstOrDefault(x => x.Key == "EventFee")?.Value);
        var otherEventFee = Convert.ToInt32(db.Configurations.FirstOrDefault(x => x.Key == "OtherEventFee")?.Value);

        //int totalMembersJoining = 0;
        foreach (var registration in _competitorEvents)
        { 
          if (registration.EventIds.Count > 1) payment += (eventFee + ((registration.EventIds.Count - 1) * otherEventFee));
          else if (registration.EventIds.Count == 1) payment+=eventFee;
          //totalMembersJoining += registration.EventIds.Count;
        }


        //var payment = Convert.ToInt32(db.Configurations.FirstOrDefault(x => x.Key.Equals("EventFee")).Value) * totalMembersJoining;
        var paySO = (Convert.ToDouble(db.Configurations.FirstOrDefault(x => x.Key.Equals("SiteOwnerPercentage")).Value) / 100) * payment;
        var payTD = ((100 - Convert.ToDouble(db.Configurations.FirstOrDefault(x => x.Key.Equals("SiteOwnerPercentage")).Value)) / 100) * payment;

        var payMetodCO = new PaymentMethodCreateOptions
        {
          Type = "card",
          Card = new PaymentMethodCardOptions
          {
            Number = _card.CardNo,
            ExpMonth = _card.ExpMonth,
            ExpYear = _card.ExpYear,
            Cvc = _card.CVC,
          },
        };

        StripeConfiguration.ApiKey = db.Configurations.FirstOrDefault(x => x.Key.Equals("SiteOwnerStripeKey")).Value;

        // For Tournament Director 
        var currTournamentDirectorId = db.ScheduledEvents.ToList().FirstOrDefault(x => x.Id == Convert.ToInt32(_competitor.Event.Split(',')[0])).TournamentDirectorId;
        ////var currTournamentId = db.ScheduledEvents.ToList().FirstOrDefault(x => x.Id == Convert.ToInt32(_competitor.Event.Split(',')[0])).TournamentId;
        ////var currTournamentDirectorId = db.Tournaments.FirstOrDefault(y => y.Id == currTournamentId).TournamentDirectorId;
        var currTournamentDirectorKey = db.TournamentDirectors.FirstOrDefault(z => z.Id == currTournamentDirectorId).StripeCustomerId;

        try
        {
          var service = new CustomerService().Get(currTournamentDirectorKey);

          if (service!=null)
          {
            // For Site Owner
            var res = payMethodservice.Create(payMetodCO);
            var options = new PaymentIntentCreateOptions
            {
              Description = "Payment For Site Owner",
              PaymentMethod = res.Id,

              #region Comment Shipping Code
              //Shipping = new ChargeShippingOptions
              //{
              //    Name = giver.Name,
              //    Address = new AddressOptions
              //    {
              //        Line1 = "510 Townsend St",
              //        PostalCode = "98140",
              //        City = "San Francisco",
              //        State = "CA",
              //        Country = "US",
              //    },
              //},
              //ReceiptEmail = email,
              #endregion

              Amount = (long)payment * 100,
              Currency = "USD",
              Confirm = true,
            };
            paymentIntent = payIntentservice.Create(options);



            var options3 = new ChargeCreateOptions
            {

              Amount = (long)payTD * 100,

              Currency = "USD",


              Customer = currTournamentDirectorKey,

              Description = "Payment For Tournament Director", //Optional  

            };

            //and Create Method of this object is doing the payment execution.  

            var service3 = new ChargeService();

            Charge charge = service3.Create(options3);

            #region Comment Code
            //StripeConfiguration.ApiKey = currTournamentDirectorKey;
            //var res2 = payMethodservice.Create(payMetodCO);

            //var options2 = new PaymentIntentCreateOptions
            //{
            //  Description = "Payment For Tournament Director",
            //  PaymentMethod = res2.Id,
            //  //Shipping = new ChargeShippingOptions
            //  //{
            //  //    Name = giver.Name,
            //  //    Address = new AddressOptions
            //  //    {
            //  //        Line1 = "510 Townsend St",
            //  //        PostalCode = "98140",
            //  //        City = "San Francisco",
            //  //        State = "CA",
            //  //        Country = "US",
            //  //    },
            //  //},
            //  //ReceiptEmail = email,
            //  Amount = (long)payTD * 100,
            //  Currency = "USD",
            //  Confirm = true,
            //};
            //paymentIntent = payIntentservice.Create(options2);
            #endregion
          }
        }
        catch (Exception ex)
        {
          return Json(new { success = false, error = "Internal Server Error"});
        }

      }
      catch (StripeException ex)
      {
        return Json(new { success = false, error = ex.StripeError.Message });
      }

      if (paymentIntent.Status.ToString().ToLower() == "succeeded")
      {

        ////Competitor obj = new Competitor();

        ////obj = _competitor;

        ////obj.RegistrationDate = DateTime.Now.ToString();

        ////obj.DateCreated = DateTime.Now;

        ////obj.DateModified = DateTime.Now;

        ////obj.CreatedBy = User.Identity.GetUserId();

        ////db.Entry(obj).State = EntityState.Modified;

        ////db.SaveChanges();

        ////for (int k = 0; k < _competitor.Event.Split(',').Count(); k++)
        ////{
        ////  db.CompetitorEventRelations.Add(new CompetitorEventRelation {
        ////    EventId = Convert.ToInt32(_competitor.Event.Split(',')[k]),
        ////    CompetitorId = _competitor.Id
        ////  });
        ////}
        ////db.SaveChanges();
        ///

        for (int k = 0; k < _competitorEvents.Count; k++)
        {
          var currCompetitor = db.Competitors.Find(_competitorEvents[k].CompetitorId);
          for (int m = 0; m < _competitorEvents[k].EventIds.Count; m++)
          {
            db.CompetitorEventRelations.Add(new CompetitorEventRelation
            {
              EventId = _competitorEvents[k].EventIds[m],
              CompetitorId = _competitorEvents[k].CompetitorId,
              TournamentId = Convert.ToInt32(_competitor.Event)
            });
            db.SaveChanges();

            currCompetitor.Event += "<br>" + db.TournamentEvents.Find(_competitorEvents[k].EventIds[m])?.Name;
            db.Entry(currCompetitor).State = EntityState.Modified;
            db.DivisionGroups.Add(new DivisionGroup
            {
              GroupNo = 1,
              //EventDevisionId = 0,
              TournamentEventId = _competitorEvents[k].EventIds[m],
              CompetitorId = _competitorEvents[k].CompetitorId,
              TournamentId = Convert.ToInt32(_competitor.Event)
            });
          }
        }
        db.SaveChanges();

        ////EmailServiceModel.SendEmail(_competitor.Email, "Test Email", "This is a test email.");
        foreach (var item in _competitorEvents)
        {
          var comptEmail = db.Competitors.FirstOrDefault(x => x.Id == item.CompetitorId).Email;
          EmailServiceModel.SendEmail(comptEmail, "Tournery Wizard", "Congratulations! You've joined the Tournament.");
        }


        return Json(new { success = true });
      }

      else {

        return Json(new { success = false, error = paymentIntent.StripeResponse.Content.ToString() });
      
      }


  
    }
  

    [HttpGet]
    public async Task<ActionResult> Edit(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Competitor competitor = await db.Competitors.FindAsync(id);

      if (competitor == null)
      {
        return HttpNotFound();
      }

      string userId = User.Identity.GetUserId();
      ViewBag.Belts = new SelectList(db.Belts.Where(x=>x.CreatedBy==userId), "BeltId", "BeltName");
      return View(competitor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Serial,Name,Age,Weight,Gender,State,City,Address,Zip,Email,Phone,School,Belt,Event,RegistrationDate,DateCreated,DateModified,Path")] Competitor competitor)
    {
      if (ModelState.IsValid)
      {
        db.Entry(competitor).State = EntityState.Modified;

        competitor.Path = competitor.Path;

        await db.SaveChangesAsync();

        return RedirectToAction("Index");
      }
      return View(competitor);
    }

    public async Task<ActionResult> Delete(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Competitor competitor = await db.Competitors.FindAsync(id);
      if (competitor == null)
      {
        return HttpNotFound();
      }
      return View(competitor);
    }

    // POST: Tournaments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      Competitor competitor = await db.Competitors.FindAsync(id);
      db.Competitors.Remove(competitor);
      await db.SaveChangesAsync();
      return RedirectToAction("Index");
    }

    static string path = @"C:\Users\Funzoft\Desktop\tourneyImages\"; 

    [HttpPost]
    public ActionResult SignatureSave(string imageData)
    {
      string Path = path + DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";
     
      TempData["imagePath"] = Path;
      
      using (FileStream fs = new FileStream(Path, FileMode.Create))
      {

        using (BinaryWriter bw = new BinaryWriter(fs))
        {

          byte[] data = Convert.FromBase64String(imageData);

          bw.Write(data);

          bw.Close();
        }

      }
      return RedirectToAction("Registration");

    }

  }

}
