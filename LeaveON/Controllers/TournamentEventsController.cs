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
using Microsoft.AspNet.Identity;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User,TournamentDirector")]
  public class TournamentEventsController : Controller
  {
    private TourneyWizardEntities db = new TourneyWizardEntities();

    // GET: TournamentEvents
    public async Task<ActionResult> Index()
    {
      string userId = User.Identity.GetUserId();
      var tournamentEvents = new List<TournamentEvent>();
      if (User.IsInRole("Admin")) tournamentEvents = await db.TournamentEvents.ToListAsync();
      else tournamentEvents = await db.TournamentEvents.Where(x => x.CreatedBy == userId).ToListAsync();
      return View(tournamentEvents);
    }

    // GET: TournamentEvents/Details/5
    public async Task<ActionResult> Details(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TournamentEvent tournamentEvent = await db.TournamentEvents.FindAsync(id);
      if (tournamentEvent == null)
      {
        return HttpNotFound();
      }
      return View(tournamentEvent);
    }

    // GET: TournamentEvents/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: TournamentEvents/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Name,DateCreated,DateModified,Description")] TournamentEvent tournamentEvent)
    {
      //tournamentEvent.Id = Guid.NewGuid().ToString();
      if (ModelState.IsValid)
      {
        tournamentEvent.CreatedBy = User.Identity.GetUserId();

        db.TournamentEvents.Add(tournamentEvent);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      return View(tournamentEvent);
    }

    // GET: TournamentEvents/Edit/5
    public async Task<ActionResult> Edit(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TournamentEvent tournamentEvent = await db.TournamentEvents.FindAsync(id);
      if (tournamentEvent == null)
      {
        return HttpNotFound();
      }
      return View(tournamentEvent);
    }

    // POST: TournamentEvents/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,DateCreated,DateModified,Description")] TournamentEvent tournamentEvent)
    {
      if (ModelState.IsValid)
      {
        tournamentEvent.CreatedBy = User.Identity.GetUserId();
        db.Entry(tournamentEvent).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(tournamentEvent);
    }

    // GET: TournamentEvents/Delete/5
    public async Task<ActionResult> Delete(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TournamentEvent tournamentEvent = await db.TournamentEvents.FindAsync(id);
      if (tournamentEvent == null)
      {
        return HttpNotFound();
      }
      return View(tournamentEvent);
    }

    // POST: TournamentEvents/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      TournamentEvent tournamentEvent = await db.TournamentEvents.FindAsync(id);
      db.TournamentEvents.Remove(tournamentEvent);
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
