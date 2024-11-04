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
using System.Globalization;
using System.Data.Common;
using Microsoft.AspNet.Identity;


namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User,TournamentDirector")]
  public class TournamentsController : Controller
  {
  //  private TourneyWizardEntities db = new TourneyWizardEntities();

  //  // GET: Tournaments
  //  [AllowAnonymous]
  //  public async Task<ActionResult> Tournament(string search, DateTime? StartDate, DateTime? EndDate)
  //  {
  //    List<Tournament> ListTournament = await db.Tournaments.ToListAsync();
  //    List<Tournament> FilterListTournament = new List<Tournament>();
  //    FilterListTournament = ListTournament;

  //    if (ListTournament == null)
  //    {
  //        return HttpNotFound();
  //    }

  //    if (search != null)
  //    {
  //      ViewData["search"] = search;

  //      FilterListTournament = ListTournament.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();

  //    }

  //    if (StartDate != null)
  //    {
  //      ViewData["startdate"] = StartDate.Value.ToString("yyyy-MM-dd");

  //      FilterListTournament = ListTournament.Where(x => x.TournamentDate.ToString("yyyy-MM-dd").Contains(StartDate.Value.ToString("yyyy-MM-dd"))).ToList();

  //    }
  //    if (EndDate != null)
  //    {
  //      ViewData["enddate"] = EndDate.Value.ToString("yyyy-MM-dd");

  //      FilterListTournament = ListTournament.Where(x => x.TournamentDate.ToString("yyyy-MM-dd").Contains(EndDate.Value.ToString("yyyy-MM-dd"))).ToList();

  //    }

  //    if (StartDate != null && EndDate != null)
  //    {
  //      FilterListTournament = ListTournament.Where(x => x.TournamentDate.Date >= StartDate.Value.Date && x.TournamentDate.Date <= EndDate.Value.Date).ToList();

  //    }

  //    return View(FilterListTournament);
      
  //  }  
    
  //  [AllowAnonymous]
  //  public async Task<ActionResult> TournamentEvents(string Id,string search, DateTime? StartDate, DateTime? EndDate)
  //  {

  //    Tournament tournament = await db.Tournaments.Where(x => x.Name.ToLower() == Id.ToLower()).FirstOrDefaultAsync();


  //    List<ScheduledEvent> ListScheduledEvent = await db.ScheduledEvents.Where(x => x.TournamentId == tournament.Id).ToListAsync();


  //    List<ScheduledEvent> FilterListScheduledEvent = new List<ScheduledEvent>();
    
 

  //    ViewBag.image = tournament.Image;
  //    ViewBag.tournamentDate = tournament.TournamentDate;
  //    ViewBag.name = tournament.Name;
  //    ViewBag.Id = tournament.Name;
  //    ViewBag.tournamentId = tournament.Id;
  //    ViewBag.CreatedBy = tournament.CreatedBy;

  //    FilterListScheduledEvent = ListScheduledEvent;

  //    if (ListScheduledEvent == null)
  //    {
  //      return HttpNotFound();
  //    }

  //    if (search != null)
  //    {
  //      ViewData["search"] = search;

  //      FilterListScheduledEvent = ListScheduledEvent.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();

  //    }

  //    if (StartDate != null)
  //    {
  //      ViewData["startdate"] = StartDate.Value.ToString("yyyy-MM-dd");

  //      FilterListScheduledEvent = ListScheduledEvent.Where(x => x.StartDate.Value.ToString("yyyy-MM-dd").Contains(StartDate.Value.ToString("yyyy-MM-dd"))).ToList();

  //    }
  //    if (EndDate != null)
  //    {
  //      ViewData["enddate"] = EndDate.Value.ToString("yyyy-MM-dd");

  //      FilterListScheduledEvent = ListScheduledEvent.Where(x => x.EndDate.Value.ToString("yyyy-MM-dd").Contains(EndDate.Value.ToString("yyyy-MM-dd"))).ToList();

  //    }

  //    if (StartDate != null && EndDate != null)
  //    {
  //      FilterListScheduledEvent = ListScheduledEvent.Where(x => x.StartDate >= StartDate && x.EndDate <= EndDate).ToList();

  //    }

  //    return View(FilterListScheduledEvent);

  //  }



  //  [AllowAnonymous]
  //  public async Task<ActionResult> TournamentCompetitors(string Id, string SearchString)
  //  {
  //    Tournament tournament = await db.Tournaments.Where(x => x.Name.ToLower() == Id.ToLower()).FirstOrDefaultAsync();


  //    var EventIds = await db.ScheduledEvents.Where(x => x.TournamentId == tournament.Id).Select(x=>x.Id).ToListAsync();

  //    var CompetitorIds = await db.CompetitorEventRelations.Where(x => EventIds.Any(c => c == x.EventId)).Select(x => x.CompetitorId).ToListAsync();

  //    List<Competitor> competitors= await db.Competitors.Where(x => CompetitorIds.Any(c => c == x.Id)).ToListAsync();

  //    ViewBag.image = tournament.Image;
  //    ViewBag.tournamentDate = tournament.TournamentDate;
  //    ViewBag.name = tournament.Name;
  //    ViewBag.Id = tournament.Name;
  //    ViewBag.tournamentId = tournament.Id;
  //    ViewBag.CreatedBy = tournament.CreatedBy;

  //    if (SearchString != null)
  //    {
  //       ViewBag.CurrentFilter = SearchString;

  //       competitors = competitors.Where(x => x.Name.ToLower().Contains(SearchString.ToLower())).ToList();

  //    }
      
  //    return View(competitors);
  //  }

  //  [Authorize]
  //  public async Task<ActionResult> AddTournament()
  //  {
  //    if (User.IsInRole("TournamentDirector"))
  //    {
  //      ViewBag.recaptchaKey = db.Configurations.Where(x => x.Key == "RecaptchaKey").FirstOrDefault().Value;

  //      //return View(await db.TournamentDirectors.ToListAsync());

  //      return View();

       
  //    }
  //    else
  //    {
  //      return RedirectToAction("AddTournamentDirectors", "TournamentDirectors");

  //    }
  //  }

  //  [AllowAnonymous]
  //  public async Task<ActionResult> TournamentDetails(string Id)
  //  {
  //    if (Id == null)
  //    {
  //      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
  //    }
  //    Tournament obj = await db.Tournaments.Where(x=>x.Name.ToLower()==Id.ToLower()).FirstOrDefaultAsync();


  //    if (obj == null)
  //    {
  //      return HttpNotFound();
  //    }

  //    return View(obj);
  //  }  
    
  //  [AllowAnonymous]
  //  public async Task<ActionResult> UpdateTournament(int? Id)
  //  {
  //    if (Id == null)
  //    {
  //      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
  //    }
  //    Tournament obj = await db.Tournaments.FindAsync(Id);

  //    ViewBag.tournamentDirectors = db.TournamentDirectors.ToList();


  //    if (obj == null)
  //    {
  //      return HttpNotFound();
  //    }

  //    return View(obj);
  //  }

  

  //[HttpPost]
  //  [AllowAnonymous]
  //  public async Task<JsonResult> AddTournament([System.Web.Http.FromBody] Tournament tournament)
  //  {

  //    if (db.Tournaments.Any(x => x.Name.ToLower().Contains(tournament.Name.ToLower())))
  //    {

  //      return Json(new { success = false, message = "This Tournament " + tournament.Name + " Already Exits", JsonRequestBehavior.AllowGet });
  //    }
  //    else
  //    {

  //      var userId = User.Identity.GetUserId();

  //      var TDId =db.TournamentDirectors?.Where(x => x.UserId == userId).FirstOrDefault()?.Id;
  //      tournament.TournamentDirectorId = TDId;
  //      tournament.TournamentDate = DateTime.Now;
  //      tournament.DateCreated = DateTime.Now;
  //      tournament.CreatedBy = User.Identity.GetUserId();
  //      db.Tournaments.Add(tournament);
  //      await db.SaveChangesAsync();

  //      return Json(new { success = true, message = "Saved Successfully", JsonRequestBehavior.AllowGet });
  //    }
  //  }
    
  //  [HttpGet]
  //  [AllowAnonymous]
  //  public async Task<ActionResult> DeleteTournament(int? Id)
  //  {

  //    if (Id == null)
  //    {
  //      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
  //    }

  //    Tournament tournament = await db.Tournaments.FindAsync(Id);
  //    ScheduledEvent scheduledEvent = await db.ScheduledEvents.Where(x => x.TournamentId == Id).FirstOrDefaultAsync();
  //    CompetitorEventRelation competitorEventRelation = await db.CompetitorEventRelations.Where(x => x.EventId == scheduledEvent.Id).FirstOrDefaultAsync();
  //    db.Tournaments.Remove(tournament);
  //    db.ScheduledEvents.Remove(scheduledEvent);
     
  //    if(competitorEventRelation!=null) db.CompetitorEventRelations.Remove(competitorEventRelation);

  //    await db.SaveChangesAsync();

  //    return RedirectToAction("Tournament");
  //  }

    
  //  [HttpPost]
  //  [AllowAnonymous]
  //  public async Task<JsonResult> UpdateTournament([System.Web.Http.FromBody] Tournament tournament)
  //  {

  //    if (db.Tournaments.Any(x => x.Name.ToLower().Contains(tournament.Name.ToLower()) && x.Id!=tournament.Id))
  //    {

  //      return Json(new { success = false, message = "This Tournament " + tournament.Name + " Already Exits", JsonRequestBehavior.AllowGet });
  //    }
  //    else
  //    {
  //      tournament.TournamentDate = DateTime.Now;
  //      tournament.DateCreated = DateTime.Now;
  //      tournament.CreatedBy = User.Identity.GetUserId();
  //      db.Entry(tournament).State = EntityState.Modified;
  //      await db.SaveChangesAsync();

  //      return Json(new { success = true, message = "Update Successfully", JsonRequestBehavior.AllowGet });
  //    }
  //  }

  //  public async Task<ActionResult> Index()
  //  {
  //    string userId = User.Identity.GetUserId();
  //    var tournaments = new List<Tournament>();
  //    if (User.IsInRole("Admin")) tournaments = await db.Tournaments.ToListAsync();
  //    else tournaments = await db.Tournaments.Where(x => x.CreatedBy == userId).ToListAsync();
  //    return View(tournaments);
  //  }

  //  [HttpGet]
  //  public JsonResult Copy(int id , string nme , DateTime dte)
  //  {
  //    int maxId = db.Tournaments.Max(p => p.Id);

  //    Tournament tournament = new Tournament
  //    {
  //      Id = maxId,
  //      Name = nme,
  //      TournamentDate = dte,
  //      DateCreated = DateTime.Now,
  //      DateModified = DateTime.Now
  //    };

  //    db.Tournaments.Add(tournament);

  //    List<EventDivision> eventDivisions = db.EventDivisions.Where(x => x.TournamentId == id).ToList();

  //    foreach (EventDivision eventDivision in eventDivisions)
  //    {
  //      EventDivision eventDivision1 = new EventDivision
  //      {
  //        Name = eventDivision.Name,
  //        Description = eventDivision.Description,
  //        AgeRange = eventDivision.AgeRange,
  //        BeltRange = eventDivision.BeltRange,
  //        GenderRange = eventDivision.GenderRange,
  //        WeightRange = eventDivision.WeightRange,
  //        DateCreated= eventDivision.DateCreated,
  //        DateModified= eventDivision.DateModified,
  //        TournamentId = maxId,
  //        TournamentEventId = eventDivision.TournamentEventId,
  //        StrictCriteria = eventDivision.StrictCriteria,
  //        GroupLimit = eventDivision.GroupLimit,
  //        Combinations = eventDivision.Combinations,
  //        RingAssignment = eventDivision.RingAssignment,
  //        IsCustom = eventDivision.IsCustom,
  //      };

  //      db.EventDivisions.Add(eventDivision1);
  //    }

  //    db.SaveChanges();
  //    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
  //  }


  //  // GET: Tournaments/Details/5
  //  public async Task<ActionResult> Details(decimal id)
  //  {
  //    if(id == null)
  //    {
  //      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
  //    }
  //    Tournament tournament = await db.Tournaments.FindAsync(id);
  //    if (tournament == null)
  //    {
  //      return HttpNotFound();
  //    }
  //    return View(tournament);
  //  }

  //  // GET: Tournaments/Create
  //  public ActionResult Create()
  //  {
  //    return View();
  //  }

  //  // POST: Tournaments/Create
  //  // To protect from overposting attacks, enable the specific properties you want to bind to, for 
  //  // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
  //  [HttpPost]
  //  [ValidateAntiForgeryToken]
  //  public async Task<ActionResult> Create([Bind(Include = "Id,Name,TournamentDate,DateCreated,DateModified")] Tournament tournament, string FormatedDate)
  //  {

  //    if (!string.IsNullOrEmpty(FormatedDate))
  //    {
  //      var date = FormatedDate.Split('/');
  //      tournament.TournamentDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(int.Parse(date[2])), int.Parse(date[0]), int.Parse(date[1]));
  //    }
  //    //tournament.Id = Guid.NewGuid().ToString();
  //    if (ModelState.IsValid)
  //    {
  //      db.Tournaments.Add(tournament);
  //      await db.SaveChangesAsync();
  //      return RedirectToAction("Index");
  //    }

  //    return View(tournament);
  //  }

  //  // GET: Tournaments/Edit/5
  //  public async Task<ActionResult> Edit(decimal id)
  //  {
  //    if (id == null)
  //    {
  //      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
  //    }
  //    Tournament tournament = await db.Tournaments.FindAsync(id);
  //    if (tournament == null)
  //    {
  //      return HttpNotFound();
  //    }
  //    ViewBag.FormatedDate = tournament.TournamentDate.ToString("MM/dd/yy");
  //    return View(tournament);
  //  }

  //  // POST: Tournaments/Edit/5
  //  // To protect from overposting attacks, enable the specific properties you want to bind to, for 
  //  // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
  //  [HttpPost]
  //  [ValidateAntiForgeryToken]
  //  public async Task<ActionResult> Edit([Bind(Include = "Id,Name,TournamentDate,DateCreated,DateModified")] Tournament tournament, string FormatedDate)
  //  {
  //    if (!string.IsNullOrEmpty(FormatedDate))
  //    {
  //      dynamic date = FormatedDate.Split('/');
  //      tournament.TournamentDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(int.Parse(date[2])), int.Parse(date[0]), int.Parse(date[1]));
  //    }

  //    if (ModelState.IsValid)
  //    {
  //      db.Entry(tournament).State = EntityState.Modified;
  //      await db.SaveChangesAsync();
  //      return RedirectToAction("Index");
  //    }
  //    return View(tournament);
  //  }

  //  // GET: Tournaments/Delete/5
  //  public async Task<ActionResult> Delete(decimal id)
  //  {
  //    if (id == null)
  //    {
  //      return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
  //    }
  //    Tournament tournament = await db.Tournaments.FindAsync(id);
  //    if (tournament == null)
  //    {
  //      return HttpNotFound();
  //    }
  //    return View(tournament);
  //  }

  //  // POST: Tournaments/Delete/5
  //  [HttpPost, ActionName("Delete")]
  //  [ValidateAntiForgeryToken]
  //  public async Task<ActionResult> DeleteConfirmed(decimal id)
  //  {
  //    Tournament tournament = await db.Tournaments.FindAsync(id);
  //    db.Tournaments.Remove(tournament);
  //    await db.SaveChangesAsync();
  //    return RedirectToAction("Index");
  //  }

  //  protected override void Dispose(bool disposing)
  //  {
  //    if (disposing)
  //    {
  //      db.Dispose();
  //    }
  //    base.Dispose(disposing);
  //  }
  }
}
