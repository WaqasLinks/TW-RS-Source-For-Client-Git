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
using Microsoft.Ajax.Utilities;
using System.Data.Entity.Infrastructure;
using System.Web.Http;
using LeaveON.Migrations;
using Microsoft.AspNet.Identity;

namespace LeaveON.Controllers
{
    public class ScheduledEventsController : Controller
    {
        private TourneyWizardEntities db = new TourneyWizardEntities();


    ////public async Task<JsonResult> GetAllTournaments()
    ////{
    ////  string CurrentUserId = User.Identity.GetUserId();

    ////  var data = Json(await db.Tournaments.Where(x=>x.CreatedBy== CurrentUserId).Select(o => new
    ////  {
    ////    Id = o.Id,
    ////    Name = o.Name
    ////  }).ToListAsync(), JsonRequestBehavior.AllowGet);

    ////  return data;
    ////}

    [System.Web.Http.HttpPost]

    public async Task<JsonResult> AddEvent([FromBody] ScheduledEvent scheduledEvent)
    {
      string userId = User.Identity.GetUserId();
      using (TourneyWizardEntities obj = new TourneyWizardEntities())
      {
        if (obj.ScheduledEvents.Any(x => x.Name.ToLower().Contains(scheduledEvent.Name.ToLower())))
        {
          return Json(new { success = false, message = "This Tournament " + scheduledEvent.Name + " Already Exits", JsonRequestBehavior.AllowGet });
        }
        else
        {
          var events = db.TournamentEvents.Where(x => x.CreatedBy == userId).ToList();

          if (events == null || events.Count == 0)
          {
            return Json(new { success = false, message = "No Events Found, Please Create an Event", JsonRequestBehavior.AllowGet });
          }

          scheduledEvent.CreatedBy = userId;
          scheduledEvent.TournamentDirectorId = db.TournamentDirectors.FirstOrDefault(x => x.UserId == userId)?.Id;
          obj.ScheduledEvents.Add(scheduledEvent);
          obj.SaveChanges();

          //Adding Events to Tournaments
          
          foreach (var item in events)
          {
            obj.TournamentEventRealtions.Add(new TournamentEventRealtion { ScheduleEventId = scheduledEvent.Id, EventId = item.Id, EventName = item.Name, CreatedBy = userId, CreatedOn = DateTime.Now });
          }
          obj.SaveChanges();

          return Json(new { success = true, message = "Saved Successfully", JsonRequestBehavior.AllowGet });

        }
         
        
       
      }

    }  
    
    [System.Web.Http.HttpPost]

    public async Task<JsonResult> UpdateEvent([FromBody] ScheduledEvent scheduledEvent)
    {
      using (TourneyWizardEntities obj = new TourneyWizardEntities())
      {
        if (obj.ScheduledEvents.Any(x => x.Name.ToLower().Contains(scheduledEvent.Name.ToLower()) && x.Id!=scheduledEvent.Id))
        {


          return Json(new { success = false, message = "This Tournament " + scheduledEvent.Name + " Already Exits", JsonRequestBehavior.AllowGet });
        }
        else
        {
          string userId = User.Identity.GetUserId();
          scheduledEvent.CreatedBy = userId;
          scheduledEvent.TournamentDirectorId = db.TournamentDirectors.FirstOrDefault(x => x.UserId == userId)?.Id;
          obj.Entry(scheduledEvent).State = EntityState.Modified;
          await obj.SaveChangesAsync();
          return Json(new { success = true, message = "Update Successfully", JsonRequestBehavior.AllowGet });

        }
         
        
       
      }

    }

    public async Task<ActionResult> Index()
        {
      //var scheduledEvents = db.ScheduledEvents.Include(s => s.Tournament);
      //await scheduledEvents.ToListAsync()
            return View();
        }

 
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledEvent scheduledEvent = await db.ScheduledEvents.FindAsync(id);
            if (scheduledEvent == null)
            {
                return HttpNotFound();
            }
            return View(scheduledEvent);
        }

        public ActionResult Create()
        {
            ////ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name");
            return View();
        }


      [System.Web.Http.HttpPost]
      [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,City,State,Country,EventDate,Description,DateCreated,DateModified,TournamentId")] ScheduledEvent scheduledEvent)
        {
            if (ModelState.IsValid)
            {
                db.ScheduledEvents.Add(scheduledEvent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //// ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", scheduledEvent.TournamentId);
            return View(scheduledEvent);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledEvent scheduledEvent = await db.ScheduledEvents.FindAsync(id);
            if (scheduledEvent == null)
            {
                return HttpNotFound();
            }
            ////ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", scheduledEvent.TournamentId);
            return View(scheduledEvent);
        }

    [System.Web.Http.HttpPost]
    [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,City,State,Country,EventDate,Description,DateCreated,DateModified,TournamentId")] ScheduledEvent scheduledEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduledEvent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ////ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", scheduledEvent.TournamentId);
            return View(scheduledEvent);
        }

        // GET: ScheduledEvents1/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledEvent scheduledEvent = await db.ScheduledEvents.FindAsync(id);
            if (scheduledEvent == null)
            {
                return HttpNotFound();
            }
            return View(scheduledEvent);
        }

    // POST: ScheduledEvents1/Delete/5
        [System.Web.Http.HttpPost, System.Web.Http.ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ScheduledEvent scheduledEvent = await db.ScheduledEvents.FindAsync(id);
            db.ScheduledEvents.Remove(scheduledEvent);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
