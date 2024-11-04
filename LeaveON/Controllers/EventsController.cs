using LeaveON.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TourneyRepo.Models;

namespace LeaveON.Controllers
{
  public class EventsController : Controller
  {
    private TourneyWizardEntities db = new TourneyWizardEntities();

    public ActionResult Index()
    {
      return View();
    }
    public ActionResult UpcomingEvent()
    {
      return View();
    }

    [Authorize]
    public ActionResult AddEvent()
    {
      if (User.IsInRole("TournamentDirector"))
      {

        ViewBag.recaptchaKey = db.Configurations.Where(x => x.Key == "RecaptchaKey").FirstOrDefault().Value;
        return View();
      }
      else
      {

        return RedirectToAction("AddTournamentDirectors", "TournamentDirectors");
       
     }
    }

    public ActionResult LogIn()
    {
      return View();
    }

    public ActionResult Register()
    {
      return View();
    }    
    public async Task<ActionResult> Participant(string Id,string searchString)
    {
      string userId = User.Identity.GetUserId();
      ScheduledEvent scheduledEvent = await db.ScheduledEvents.Where(x => x.Name.ToLower() == Id.ToLower()).FirstOrDefaultAsync();

      var CompetitorId = await db.DivisionGroups.Where(x=>x.TournamentId== scheduledEvent.Id).Select(c=>c.CompetitorId).ToListAsync();

      List<Competitor> competitor = await db.Competitors.Where(x=> CompetitorId.Any(r=>r.Value == x.Id)).ToListAsync();
      ViewBag.isAnyCompetitorRemaining = competitor.Where(a => a.CreatedBy == userId).Select(b => b.Id).ToList().OrderBy(x => x).SequenceEqual(db.Competitors.Where(x => x.CreatedBy == userId).Select(y => y.Id).ToList().OrderBy(x => x));

      if (searchString != null)
      {
        competitor = competitor.Where(x => x.Name.ToLower().ToString().Contains(searchString.ToLower().ToString())).ToList();
      }

      
      ViewBag.isCompetitorExist = (competitor.Any(x => x.CreatedBy == userId) ? true : false);
      ViewBag.image = scheduledEvent.Image;
      ViewBag.eventDate = scheduledEvent.StartDate.Value.ToString("dd MMM") + "-" + scheduledEvent.EndDate.Value.ToString("dd MMM");
      ViewBag.name = scheduledEvent.Name;
      ViewBag.Id = scheduledEvent.Name;
      ViewBag.EventId = scheduledEvent.Id;
      ViewBag.CreatedBy = scheduledEvent.CreatedBy;
      ViewBag.totalCount = competitor.Count();
      ViewBag.CurrentFilter = searchString;

      ViewBag.Belts = db.Belts.Where(y => y.CreatedBy == userId).ToList();
      return View(competitor);
    }


    public async Task<ActionResult> Events(string search,DateTime? StartDate,DateTime? EndDate, string EventType)
    {
      string userId = User.Identity.GetUserId();
      if (!string.IsNullOrEmpty(userId) && db.Belts.Where(x => x.CreatedBy == userId).ToList().Count == 0)
      {
        var builtBelts = new List<Belt>();
        builtBelts.Add(new Belt { BeltId = 1, BeltName = "White", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 2, BeltName = "Yellow", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 3, BeltName = "Orange", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 4, BeltName = "Green", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 5, BeltName = "Blue", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 6, BeltName = "Purple", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 7, BeltName = "Brown", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 8, BeltName = "Red", CreatedBy = userId });
        builtBelts.Add(new Belt { BeltId = 9, BeltName = "Black", CreatedBy = userId });
        db.Belts.AddRange(builtBelts);
        db.SaveChanges();
      }
        
      List<ScheduledEvent> scheduledEvent = await db.ScheduledEvents.ToListAsync();
      List<ScheduledEvent> FilterscheduledEvent = new List<ScheduledEvent>();
      
      FilterscheduledEvent = scheduledEvent;

      if (scheduledEvent == null)
        if (scheduledEvent == null)
      {
        return HttpNotFound();
      }


      if (EventType != null && EventType== "Upcoming" || EventType==null)
      {
        ViewData["eventtype"] = "Upcoming";

        FilterscheduledEvent = scheduledEvent.Where(x => x.EndDate >=DateTime.Now).ToList();

      }

      if (EventType != null && EventType == "Pastevents")
      {
        ViewData["eventtype"] = "Pastevents";

        FilterscheduledEvent = scheduledEvent.Where(x => x.EndDate < DateTime.Now).ToList();

      } 
      
      if (EventType != null && EventType == "Yourevents")
      {
       
        ViewData["eventtype"] = "Yourevents";
        
        var UserId = "";

        if (User.Identity.IsAuthenticated)
        {
          UserId = User.Identity.GetUserId();
        }

        FilterscheduledEvent = scheduledEvent.Where(x => x.CreatedBy== UserId).ToList();

      }
   
      if (search!=null && !string.IsNullOrEmpty(search))
      {
        ViewData["search"] = search;
        FilterscheduledEvent = scheduledEvent.Where(x => x.Name.ToLower().Contains(search.ToLower()) 
                || x.Country.ToLower().Contains(search.ToLower())
                || x.State.ToLower().Contains(search.ToLower())
                || x.City.ToLower().Contains(search.ToLower())
                ).ToList();
        if(FilterscheduledEvent.Count == 0)
        {
          FilterscheduledEvent = scheduledEvent.Where(x =>
          {
            string cleanText = Regex.Replace(x.Description, "<.*?>", string.Empty);
            return cleanText.IndexOf(search.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0;
          }).ToList();
        }
      }

      if (StartDate!=null)
      {
        ViewData["startdate"] = StartDate.Value.ToString("yyyy-MM-dd");

        FilterscheduledEvent = scheduledEvent.Where(x => x.StartDate.Value.ToString("yyyy-MM-dd").Contains(StartDate.Value.ToString("yyyy-MM-dd"))).ToList();

      }
      if (EndDate != null)
      {
         ViewData["enddate"] = EndDate.Value.ToString("yyyy-MM-dd");

        FilterscheduledEvent = scheduledEvent.Where(x => x.EndDate.Value.ToString("yyyy-MM-dd").Contains(EndDate.Value.ToString("yyyy-MM-dd"))).ToList();

      }

      if (StartDate != null && EndDate != null)
      {
        FilterscheduledEvent = scheduledEvent.Where(x => x.StartDate >= StartDate && x.StartDate <= EndDate).ToList();

      }

      return View(FilterscheduledEvent);
    }   
    public async Task<ActionResult> UpdateEvent(int? Id)
    {
      if (Id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
     
      ScheduledEvent scheduledEvent = await db.ScheduledEvents.FindAsync(Id);


      if (scheduledEvent == null)
      {
        return HttpNotFound();
      }
      ViewBag.recaptchaKey = db.Configurations.Where(x => x.Key == "RecaptchaKey").FirstOrDefault().Value;
      return View(scheduledEvent);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> DeleteEvents(int? Id)
    {

      if (Id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      ScheduledEvent scheduledEvent = await db.ScheduledEvents.Where(x => x.Id == Id).FirstOrDefaultAsync();

      DivisionGroup competitorEventRelation = await db.DivisionGroups.Where(x => x.TournamentId == scheduledEvent.Id).FirstOrDefaultAsync();

      
      db.ScheduledEvents.Remove(scheduledEvent);

      if(competitorEventRelation!=null) db.DivisionGroups.Remove(competitorEventRelation);

      await db.SaveChangesAsync();

      return RedirectToAction("Events");
    }

   
    public async Task<ActionResult> EventDetails(string Id)
    {
      if (Id == null)
      {
        //return RedirectToAction("Index", "Events");
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      string userId = User.Identity.GetUserId();
      ScheduledEvent scheduledEvent = await db.ScheduledEvents.Where(x => x.Name.ToLower().Replace(" ","") == Id.ToLower()).FirstOrDefaultAsync();
      if (scheduledEvent == null) return Redirect(HttpContext.Request.Url.AbsoluteUri + "/Index");
      var CompetitorId = new List<int?>();
      if (scheduledEvent == null) { CompetitorId = new List<int?>(); }
      else  CompetitorId = await db.DivisionGroups.Where(x => x.TournamentId == scheduledEvent.Id).Select(c => c.CompetitorId).ToListAsync();

      List<Competitor> competitor = await db.Competitors.Where(x => CompetitorId.Any(r => r.Value == x.Id)).ToListAsync();

      ViewBag.totalCount = competitor.Count();
      ViewBag.isCompetitorExist = (competitor.Any(x=>x.CreatedBy==userId) ? true : false);
      ViewBag.isAnyCompetitorRemaining = (!string.IsNullOrEmpty(userId) ? competitor.Where(a => a.CreatedBy == userId).Select(b=>b.Id).ToList().OrderBy(x => x).SequenceEqual(db.Competitors.Where(x => x.CreatedBy == userId).Select(y => y.Id).ToList().OrderBy(x => x)) : false);

      ViewBag.EventFee = db.Configurations.FirstOrDefault(x => x.Key == "EventFee").Value;
      ViewBag.OtherEventFee = db.Configurations.FirstOrDefault(x => x.Key == "OtherEventFee").Value;

      ViewBag.TournamentDirector = db.TournamentDirectors.FirstOrDefault(x => x.Id == scheduledEvent.TournamentDirectorId);
      if (scheduledEvent == null)
      {
        return HttpNotFound();
      }
    
      return View(scheduledEvent);
    }

    private List<PrintEventDivisionInfoViewModel> LoadEventDivisionInfo(int Event)
    {
      var eventDivisionInfos = new List<PrintEventDivisionInfoViewModel>();
      var divisions = db.EventDivisions.Where(x => x.TournamentEventId == Event).ToList();
      if (divisions != null && divisions.Count > 0)
      {
        divisions.ForEach(x =>
        {
          int fromAge = 0, toAge = 0;
          if (!string.IsNullOrEmpty(x.AgeRange))
          {
            var ageRange = x.AgeRange.Split('-');
            int.TryParse(ageRange[0], out fromAge);
            int.TryParse(ageRange[1], out toAge);
          }

          int frombelt = 0, toBelt = 0;
          if (!string.IsNullOrEmpty(x.BeltRange))
          {
            var beltRange = x.BeltRange.Split('-');
            int.TryParse(beltRange[0], out frombelt);
            int.TryParse(beltRange[1], out toBelt);
          }

          int fromWeight = 0, toWeight = 0;
          if (!string.IsNullOrEmpty(x.WeightRange))
          {
            var weightRange = x.WeightRange.Split('-');
            int.TryParse(weightRange[0], out fromWeight);
            int.TryParse(weightRange[1], out toWeight);
          }

          int gender = 7;
          if (!string.IsNullOrEmpty(x.GenderRange))
          {
            int.TryParse(x.GenderRange, out gender);
          }

          eventDivisionInfos.Add(new PrintEventDivisionInfoViewModel
          {
            Id = x.Id,
            FromAge = fromAge,
            ToAge = toAge,
            FromBelt = frombelt,
            ToBelt = toBelt,
            FromWeight = fromWeight,
            ToWeight = toWeight,
            Gender = gender,
            TournamentEventId = x.TournamentEventId,
            AgeRange = x.AgeRange,
            BeltRange = x.BeltRange,
            Combinations = x.Combinations,
            Description = x.Description,
            GenderRange = x.GenderRange,
            GroupLimit = x.GroupLimit,
            IsCustom = x.IsCustom,
            Name = x.TournamentEvent.Name,
            RingAssignment = x.RingAssignment,
            StrictCriteria = x.StrictCriteria,
            TournamentId = x.TournamentId,
            WeightRange = x.WeightRange
          });
        });
      }
      return eventDivisionInfos;
    }

    public async Task<ActionResult> Brackets(string Id, int? type)
    {
      ScheduledEvent scheduledEvent = await db.ScheduledEvents.Where(x => x.Name.ToLower() == Id.ToLower()).FirstOrDefaultAsync();
      var CompetitorId = await db.DivisionGroups.Where(x => x.TournamentId == scheduledEvent.Id).Select(c => c.CompetitorId).ToListAsync();
    List<Competitor> competitor = await db.Competitors.Where(x => CompetitorId.Any(r => r.Value == x.Id)).ToListAsync();

      string userId = User.Identity.GetUserId();
      ViewBag.isAnyCompetitorRemaining = competitor.Where(a => a.CreatedBy == userId).Select(b => b.Id).ToList().OrderBy(x => x).SequenceEqual(db.Competitors.Where(x => x.CreatedBy == userId).Select(y => y.Id).ToList().OrderBy(x => x));

      ViewBag.isCompetitorExist = (competitor.Any(x => x.CreatedBy == userId) ? true : false);
      if (type == null) type = 1;
      ViewBag.image = scheduledEvent.Image;
      ViewBag.eventDate = scheduledEvent.StartDate.Value.ToString("dd MMM") + "-" + scheduledEvent.EndDate.Value.ToString("dd MMM");
      ViewBag.name = scheduledEvent.Name;
      ViewBag.Id = scheduledEvent.Name;
      ViewBag.EventId = scheduledEvent.Id;
      ViewBag.CreatedBy = scheduledEvent.CreatedBy;
      ViewBag.totalCount = competitor.Count;
      ViewBag.typeId = type;


      var eventDivisions = LoadEventDivisionInfo(scheduledEvent.Id);
      var filteredEventDivisions = new List<PrintEventDivisionInfoViewModel>();
      var eventDivisionsToPrint = new List<PrintBracketsViewModel>();
      var competitors = await db.Competitors.ToListAsync();

      filteredEventDivisions.AddRange(eventDivisions);

      var eventDivIds = filteredEventDivisions.Select(x => x.Id).ToList();
      ////var eventDivisionInfo = await db.EventDivisions.Where(x => eventDivIds.Contains(x.Id)).Include(e => e.Tournament).Include(e => e.TournamentEvent).ToListAsync();
      var eventDivisionInfo = await db.EventDivisions.Where(x => eventDivIds.Contains(x.Id)).Include(e => e.TournamentEvent).ToListAsync();
      var eventDivisionGroupsInfo = await db.DivisionGroups.Where(x => x.EventDevisionId != null && eventDivIds.Contains(x.EventDevisionId.Value)).ToListAsync();

      foreach (var eventDivision in filteredEventDivisions)
      {
        if (string.IsNullOrWhiteSpace(eventDivision.Combinations))
        {
          eventDivisionsToPrint.Add(new PrintBracketsViewModel
          {
            Age = eventDivision.AgeRange,
            Belt = eventDivision.BeltRange,
            Combination = eventDivision.Combinations,
            Event = eventDivision.Name,
            EventId = eventDivision.Id,
            Gender = eventDivision.GenderRange,
            Group = 1,
            id = type.Value,
            IsCustom = eventDivision.IsCustom.GetValueOrDefault(),
            IsParticipantLimited = false,
            TotalGroup = 1,
            Weight = eventDivision.WeightRange,
            Competitors = competitors,
            EventDivisions = eventDivisionInfo,
            DivisionGroups = eventDivisionGroupsInfo
          });
        }
        else
        {
          var groups = eventDivision.Combinations.Split(',');
          var grpNo = 1;
          foreach (var group in groups)
          {
            eventDivisionsToPrint.Add(new PrintBracketsViewModel
            {
              Age = eventDivision.AgeRange,
              Belt = eventDivision.BeltRange,
              Combination = eventDivision.Combinations,
              Event = eventDivision.Name,
              EventId = eventDivision.Id,
              Gender = eventDivision.GenderRange,
              Group = grpNo,
              id = type.Value,
              IsCustom = eventDivision.IsCustom.GetValueOrDefault(),
              IsParticipantLimited = true,
              TotalGroup = groups.Count(),
              Weight = eventDivision.WeightRange,
              Competitors = competitors,
              EventDivisions = eventDivisionInfo,
              DivisionGroups = eventDivisionGroupsInfo
            });
            grpNo++;
          }
        }
      }

      return View(eventDivisionsToPrint);
    }


  }
}
