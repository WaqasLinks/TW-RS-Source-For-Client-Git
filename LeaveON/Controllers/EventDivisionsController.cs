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
using LeaveON.Models;
using System.Text;
using Microsoft.AspNet.Identity;
using System.Windows.Input;
using System.Web.Script.Serialization;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User,TournamentDirector")]
  public class EventDivisionsController : Controller
  {
    private TourneyWizardEntities db = new TourneyWizardEntities();
    //private List<BeltViewModel> beltsCollection = new List<BeltViewModel>() { new BeltViewModel { Id = 1, Name = "White" }, new BeltViewModel { Id = 2, Name = "Yellow" }, new BeltViewModel { Id = 3, Name = "Orange" }, new BeltViewModel { Id = 4, Name = "Green" }, new BeltViewModel { Id = 5, Name = "Blue" }, new BeltViewModel { Id = 6, Name = "Purple" }, new BeltViewModel { Id = 7, Name = "Brown" }, new BeltViewModel { Id = 8, Name = "Red" }, new BeltViewModel { Id = 9, Name = "Black" } };
    private List<BeltViewModel> beltsCollection = new List<BeltViewModel>();
    private List<GenderViewModel> gendersCollection = new List<GenderViewModel>() { new GenderViewModel { Id = 2, Name = "Male & Female" }, new GenderViewModel { Id = 1, Name = "Male" }, new GenderViewModel { Id = 0, Name = "Female" } };
    private List<CriteriaViewModel> criteriaCollection = new List<CriteriaViewModel>() { new CriteriaViewModel { Value = "true", Name = "Strict" }, new CriteriaViewModel { Value = "false", Name = "Loose" } };
    private List<string> combinations = new List<string>();
    JavaScriptSerializer serializer = new JavaScriptSerializer();

    
    [HttpPost]
    public async Task<ActionResult> Index2(List<int> checkboxData = null)
    {
      string userId = User.Identity.GetUserId();
      var eventDivisions = new List<EventDivisionViewModel>();
      if (User.IsInRole("Admin"))
      {
        if (checkboxData != null && checkboxData.Count > 0 && checkboxData[0] != 0)
        {
          string serCheckboxData = serializer.Serialize(checkboxData);
          Response.Cookies.Add(new HttpCookie("DivisionFilters", serCheckboxData));

          eventDivisions = await db.EventDivisions.Where(x => checkboxData.Contains(x.TournamentEvent.Id)).Select(x => new EventDivisionViewModel
          {
            AgeRange = x.AgeRange,
            BeltRange = x.BeltRange,
            GenderRange = x.GenderRange,
            Id = x.Id,
            Name = x.Name,
            TournamentEventId = x.TournamentEventId,
            TournamentEventName = x.TournamentEvent.Name,
            TournamentName = "",
            StrictCriteria = x.StrictCriteria,
            GroupLimit = x.GroupLimit,
            Combinations = x.Combinations,
            IsCustom = x.IsCustom
          }).ToListAsync();
        }
        else
        {
          Response.Cookies.Add(new HttpCookie("DivisionFilters", ""));

          //eventDivisions = await db.EventDivisions.Select(x => new EventDivisionViewModel
          //{
          //  AgeRange = x.AgeRange,
          //  BeltRange = x.BeltRange,
          //  GenderRange = x.GenderRange,
          //  Id = x.Id,
          //  Name = x.Name,
          //  TournamentEventId = x.TournamentEventId,
          //  TournamentEventName = x.TournamentEvent.Name,
          //  TournamentName = x.Tournament.Name,
          //  StrictCriteria = x.StrictCriteria,
          //  GroupLimit = x.GroupLimit,
          //  Combinations = x.Combinations,
          //  IsCustom = x.IsCustom
          //}).ToListAsync();
        }
      }
      else
      {
        if (checkboxData != null && checkboxData.Count > 0 && checkboxData[0] != 0)
        {
          string serCheckboxData = serializer.Serialize(checkboxData);
          Response.Cookies.Add(new HttpCookie("DivisionFilters", serCheckboxData));

          eventDivisions = await db.EventDivisions.Where(x => x.CreatedBy == userId && checkboxData.Contains(x.TournamentEvent.Id)).Select(x => new EventDivisionViewModel
          {
            AgeRange = x.AgeRange,
            BeltRange = x.BeltRange,
            GenderRange = x.GenderRange,
            Id = x.Id,
            Name = x.Name,
            TournamentEventId = x.TournamentEventId,
            TournamentEventName = x.TournamentEvent.Name,
            TournamentName = "",
            StrictCriteria = x.StrictCriteria,
            GroupLimit = x.GroupLimit,
            Combinations = x.Combinations,
            IsCustom = x.IsCustom
          }).ToListAsync();
        }
        else
        {
          Response.Cookies.Add(new HttpCookie("DivisionFilters", ""));

          //eventDivisions = await db.EventDivisions.Where(x => x.CreatedBy == userId).Select(x => new EventDivisionViewModel
          //{
          //  AgeRange = x.AgeRange,
          //  BeltRange = x.BeltRange,
          //  GenderRange = x.GenderRange,
          //  Id = x.Id,
          //  Name = x.Name,
          //  TournamentEventId = x.TournamentEventId,
          //  TournamentEventName = x.TournamentEvent.Name,
          //  TournamentName = x.Tournament.Name,
          //  StrictCriteria = x.StrictCriteria,
          //  GroupLimit = x.GroupLimit,
          //  Combinations = x.Combinations,
          //  IsCustom = x.IsCustom
          //}).ToListAsync();
        }
      }

      ////var eventDivisions = await db.EventDivisions.Select(x => new EventDivisionViewModel
      ////{
      ////  AgeRange = x.AgeRange,
      ////  BeltRange = x.BeltRange,
      ////  GenderRange = x.GenderRange,
      ////  Id = x.Id,
      ////  Name = x.Name,
      ////  TournamentEventId = x.TournamentEventId,
      ////  TournamentEventName = x.TournamentEvent.Name,
      ////  TournamentName = x.Tournament.Name,
      ////  StrictCriteria = x.StrictCriteria,
      ////  GroupLimit = x.GroupLimit,
      ////  Combinations = x.Combinations,
      ////  IsCustom = x.IsCustom
      ////}).ToListAsync();

      if (eventDivisions != null && eventDivisions.Any())
      {
        eventDivisions.ForEach(x =>
        {
          x.TotalCompetitors = GetDivisionCount(x.AgeRange, x.BeltRange, x.WeightRange, x.GenderRange, x.StrictCriteria.GetValueOrDefault(), x.GroupLimit.GetValueOrDefault(), x.Combinations, x.TournamentEventName, (int)x.Id, x.IsCustom.GetValueOrDefault(), 1);
        });
      }

      var Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();

      var index = 1;
      var response = "";
      foreach (var item in eventDivisions)
      {
        response += "<tr>";
        response += "<td>" + (index++) + "</td>";
        string displayAgeRange;
        if (item.AgeRange != null && item.AgeRange.Contains("-"))
        {
          string[] ages = item.AgeRange?.Split('-');
          int firstAge = int.Parse(ages[0]);
          int secondAge = int.Parse(ages[1]);

          if (secondAge >= 100)
          {
            displayAgeRange = $"{firstAge} years & above";
          }
          else if (firstAge == 1)
          {
            displayAgeRange = $"{secondAge} years & below";
          }
          else
          {
            displayAgeRange = item.AgeRange;
          }
        }
        else
        {
          displayAgeRange = item.AgeRange; 
        }
        response += "<td data-sort='" + (item.AgeRange?.Replace("-", "")) + "'>" + displayAgeRange + "</td>";
        response += "<td>";
        if (!string.IsNullOrWhiteSpace(item.GenderRange))
        {
          switch (item.GenderRange)
          {
            case "0":
              item.GenderRange = "Female";
              break;
            case "1":
              item.GenderRange = "Male";
              break;
            default:
              item.GenderRange = "Male & Female";
              break;
          }
        }
        response += item.GenderRange + "</td>";
        response += "<td data-sort='" + (!string.IsNullOrWhiteSpace(item.BeltRange) ? item.BeltRange.Split(' ')[0] : item.BeltRange) + "'>";
        if (!string.IsNullOrWhiteSpace(item.BeltRange))
          //response += (item.BeltRange.Split('-').First() == "1" ? "White" : item.BeltRange.Split('-').First() == "2" ? "Yellow" : item.BeltRange.Split('-').First() == "3" ? "Orange" : item.BeltRange.Split('-').First() == "4" ? "Green" : item.BeltRange.Split('-').First() == "5" ? "Blue" : item.BeltRange.Split('-').First() == "6" ? "Purple" : item.BeltRange.Split('-').First() == "7" ? "Brown" : item.BeltRange.Split('-').First() == "8" ? "Red" : "Black")
          response += (Belts.FirstOrDefault(y => y.BeltId.ToString() == item.BeltRange.Split('-').First())?.BeltName)
          + " - " +
          //(item.BeltRange.Split('-').Last() == "1" ? "White" : item.BeltRange.Split('-').Last() == "2" ? "Yellow" : item.BeltRange.Split('-').Last() == "3" ? "Orange" : item.BeltRange.Split('-').Last() == "4" ? "Green" : item.BeltRange.Split('-').Last() == "5" ? "Blue" : item.BeltRange.Split('-').Last() == "6" ? "Purple" : item.BeltRange.Split('-').Last() == "7" ? "Brown" : item.BeltRange.Split('-').Last() == "8" ? "Red" : "Black");
          (Belts.FirstOrDefault(y => y.BeltId.ToString() == item.BeltRange.Split('-').Last())?.BeltName);



        response += "</td>";
        response += "<td>" + (string.IsNullOrEmpty(item.WeightRange) ? "All" : item.WeightRange) + "</td>";
        response += "<td>" + item.Name + "</td>";
        response += "<td>" + item.TotalCompetitors + "</td>";
        //if(item.GroupLimit == null || item.GroupLimit == 0) response += "<td>1</td>";
        if((string.IsNullOrEmpty(item.Combinations) || item.Combinations == "0")) response += "<td>0</td>";
        //else response += "<td>" + ((Convert.ToInt32(item.TotalCompetitors) / Convert.ToInt32(item.GroupLimit))+1).ToString() + "</td>";
        else response += "<td>" + item.Combinations.Split(',').Count() + "</td>";
        response += "<td>";
        if (User.IsInRole("Admin") || User.IsInRole("TournamentDirector"))
        {

          response += "<a class='btn btn-info' href='/EventDivisions/Edit/" + item.Id + "'>View Division</a>";
          response += "  <a class='btn btn-danger' href='/EventDivisions/Delete/" + item.Id + "'>Delete</a>";
        }




        response += "</td></tr>";
      }

      return Json(response, JsonRequestBehavior.AllowGet);
    }

    // GET: EventDivisions
    public async Task<ActionResult> Index()
    {
      var checkboxData = new List<int>();
      string serCheckBoxesData = Request.Cookies["DivisionFilters"]?.Value;
      if (!string.IsNullOrEmpty(serCheckBoxesData)) checkboxData = serializer.Deserialize<List<int>>(serCheckBoxesData);
      
      string userId = User.Identity.GetUserId();
      var eventDivisions = new List<EventDivisionViewModel>();
      if (User.IsInRole("Admin"))
      {
        if (checkboxData.Count == 0) {
          eventDivisions = await db.EventDivisions.Where(x => x.TournamentEvent.Id==1).Select(x => new EventDivisionViewModel
          {
            AgeRange = x.AgeRange,
            BeltRange = x.BeltRange,
            GenderRange = x.GenderRange,
            Id = x.Id,
            Name = x.Name,
            TournamentEventId = x.TournamentEventId,
            TournamentEventName = x.TournamentEvent.Name,
            TournamentName = "",
            StrictCriteria = x.StrictCriteria,
            GroupLimit = x.GroupLimit,
            Combinations = x.Combinations,
            IsCustom = x.IsCustom
          }).ToListAsync();
        }
        else {
          eventDivisions = await db.EventDivisions.Where(x => checkboxData.Contains(x.TournamentEvent.Id)).Select(x => new EventDivisionViewModel
          {
            AgeRange = x.AgeRange,
            BeltRange = x.BeltRange,
            GenderRange = x.GenderRange,
            Id = x.Id,
            Name = x.Name,
            TournamentEventId = x.TournamentEventId,
            TournamentEventName = x.TournamentEvent.Name,
            TournamentName = "",
            StrictCriteria = x.StrictCriteria,
            GroupLimit = x.GroupLimit,
            Combinations = x.Combinations,
            IsCustom = x.IsCustom
          }).ToListAsync();
        }
        
      }
      else
      {
        if (checkboxData.Count == 0)
        {
          eventDivisions = await db.EventDivisions.Where(x => x.CreatedBy == userId && x.TournamentEvent.Id==db.TournamentEvents.FirstOrDefault(y=>y.CreatedBy==userId).Id).Select(x => new EventDivisionViewModel
          {
            AgeRange = x.AgeRange,
            BeltRange = x.BeltRange,
            GenderRange = x.GenderRange,
            Id = x.Id,
            Name = x.Name,
            TournamentEventId = x.TournamentEventId,
            TournamentEventName = x.TournamentEvent.Name,
            TournamentName = "",
            StrictCriteria = x.StrictCriteria,
            GroupLimit = x.GroupLimit,
            Combinations = x.Combinations,
            IsCustom = x.IsCustom
          }).ToListAsync();
        }
        else {
          eventDivisions = await db.EventDivisions.Where(x => x.CreatedBy == userId && checkboxData.Contains(x.TournamentEvent.Id)).Select(x => new EventDivisionViewModel
          {
            AgeRange = x.AgeRange,
            BeltRange = x.BeltRange,
            GenderRange = x.GenderRange,
            Id = x.Id,
            Name = x.Name,
            TournamentEventId = x.TournamentEventId,
            TournamentEventName = x.TournamentEvent.Name,
            TournamentName = "",
            StrictCriteria = x.StrictCriteria,
            GroupLimit = x.GroupLimit,
            Combinations = x.Combinations,
            IsCustom = x.IsCustom
          }).ToListAsync();
        }
          
      }

      ////var eventDivisions = await db.EventDivisions.Select(x => new EventDivisionViewModel
      ////{
      ////  AgeRange = x.AgeRange,
      ////  BeltRange = x.BeltRange,
      ////  GenderRange = x.GenderRange,
      ////  Id = x.Id,
      ////  Name = x.Name,
      ////  TournamentEventId = x.TournamentEventId,
      ////  TournamentEventName = x.TournamentEvent.Name,
      ////  TournamentName = x.Tournament.Name,
      ////  StrictCriteria = x.StrictCriteria,
      ////  GroupLimit = x.GroupLimit,
      ////  Combinations = x.Combinations,
      ////  IsCustom = x.IsCustom
      ////}).ToListAsync();

      if (eventDivisions != null && eventDivisions.Any())
      {
        eventDivisions.ForEach(x =>
        {
          x.TotalCompetitors = GetDivisionCount(x.AgeRange, x.BeltRange, x.WeightRange, x.GenderRange, x.StrictCriteria.GetValueOrDefault(), x.GroupLimit.GetValueOrDefault(), x.Combinations, x.TournamentEventName, (int)x.Id, x.IsCustom.GetValueOrDefault(), 1);
        });
      }
      if (User.IsInRole("Admin")) ViewBag.EventTournaments = db.TournamentEvents.ToList();
      else ViewBag.EventTournaments = db.TournamentEvents.Where(x=>x.CreatedBy==userId).ToList();
      ViewBag.SelectedET = checkboxData;
      ViewBag.SelectedFT = db.TournamentEvents.FirstOrDefault(y => y.CreatedBy == userId)?.Id;
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      return View(eventDivisions);
    }

    public async Task<ActionResult> UpdateCompetitorDevisions(int Id, string EventDevisionIds, int EventId)
    {
      if (string.IsNullOrWhiteSpace(EventDevisionIds))
      {
        var divisions = await db.DivisionGroups.Where(x => x.CompetitorId == Id && x.TournamentEventId == EventId).ToListAsync();
        foreach (var division in divisions)
        {
          db.DivisionGroups.Remove(division);
        }
        await db.SaveChangesAsync();

        db.DivisionGroups.Add(new DivisionGroup()
        {
          CompetitorId = Id,
          TournamentEventId = EventId,
          GroupNo = 1
        });

        await db.SaveChangesAsync();
        return Content("updated successfully.");
      }
      var eventDivisionIds = EventDevisionIds.Split(',').Select(x => int.Parse(x)).ToList();
      var oldDivisions = await db.DivisionGroups.Where(x => x.CompetitorId == Id && x.TournamentEventId == EventId).ToListAsync();
      var oldSelectedDiv = new List<DivisionGroup>();

      foreach (var division in oldDivisions)
      {
        if (eventDivisionIds.Contains(division.EventDevisionId.GetValueOrDefault()))
        {
          oldSelectedDiv.Add(new DivisionGroup
          {
            EventDevisionId = division.EventDevisionId,
            GroupNo = division.GroupNo
          });
        }
        db.DivisionGroups.Remove(division);
      }
      await db.SaveChangesAsync();
      foreach (var eventDevision in eventDivisionIds)
      {
        int groupNo = 1;
        var oldDivGroup = oldSelectedDiv.FirstOrDefault(x => x.EventDevisionId == eventDevision);
        var tournamentEvent = db.EventDivisions.Find(eventDevision);
        if (oldDivGroup != null)
        {
          groupNo = oldDivGroup.GroupNo.GetValueOrDefault();
        }

        db.DivisionGroups.Add(new DivisionGroup()
        {
          CompetitorId = Id,
          EventDevisionId = eventDevision,
          GroupNo = groupNo,
          TournamentEventId = tournamentEvent?.TournamentEventId
        });
      }

      await db.SaveChangesAsync();
      return Content("updated successfully.");
    }

    public async Task<ActionResult> AddCompetitorDevisions(int Id)
    {
      var eventDivisions = await db.EventDivisions.Where(x => x.TournamentEventId == Id).Select(x => x.Id).ToListAsync();
      var oldDivisionCompetitors = await db.DivisionGroups.Where(x => x.EventDevisionId != null && eventDivisions.Contains(x.EventDevisionId.Value)).Select(x => x.CompetitorId).ToListAsync();
      var eventCompetitros = await db.DivisionGroups.Where(x => x.EventDevisionId == null && x.TournamentEventId != null).Select(x => x.CompetitorId).ToListAsync();
      var excludeCompetitorIds = new List<int?>();
      excludeCompetitorIds.AddRange(eventCompetitros);
      excludeCompetitorIds.AddRange(oldDivisionCompetitors);
      var competitors = await db.Competitors.Where(x => !excludeCompetitorIds.Contains(x.Id)).ToListAsync();
      var selectList = new SelectList(competitors, "Id", "Name");

      ViewBag.Competitors = selectList;
      return PartialView("CompetitorList");
    }

    public async Task<ActionResult> UpdateCompetitorED(int Id, int EventId)
    {
      var oldDivisionCompetitors = await db.DivisionGroups.FirstOrDefaultAsync(x => x.TournamentEventId == EventId && x.CompetitorId == Id);
      if (oldDivisionCompetitors == null)
      {
        db.DivisionGroups.Add(new DivisionGroup()
        {
          GroupNo = 1,
          CompetitorId = Id,
          TournamentEventId = EventId
        });
        await db.SaveChangesAsync();
      }
      return Content("updated successfully;");
    }

    public async Task<ActionResult> DeleteCompetitorDevisionGroup(decimal EventDevisionGroupId)
    {
      var eventDivGrp = await db.DivisionGroups.FirstOrDefaultAsync(x => x.Id == EventDevisionGroupId);
      if (eventDivGrp != null)
      {
        db.DivisionGroups.Remove(eventDivGrp);
      }

      await db.SaveChangesAsync();
      return Content("deleted successfully.");
    }


    public async Task<ActionResult> DeleteCompetitorDevisions(int Id, string EventDevisionIds)
    {
      if (string.IsNullOrWhiteSpace(EventDevisionIds))
      {
        var divisions = await db.DivisionGroups.Where(x => x.CompetitorId == Id).ToListAsync();
        foreach (var division in divisions)
        {
          db.DivisionGroups.Remove(division);
        }
        await db.SaveChangesAsync();
        return Content("deleted successfully.");
      }
      var eventDivisionIds = EventDevisionIds.Split(',').Select(x => decimal.Parse(x)).ToList();
      var oldDivisions = await db.DivisionGroups.Where(x => x.CompetitorId == Id).ToListAsync();
      var oldSelectedDiv = new List<DivisionGroup>();

      foreach (var division in oldDivisions)
      {
        if (eventDivisionIds.Contains(division.EventDevisionId.GetValueOrDefault()))
        {
          db.DivisionGroups.Remove(division);
        }
      }

      await db.SaveChangesAsync();
      return Content("deleted successfully.");
    }

    public async Task<ActionResult> ViewCompetitors(int id)
    {
      var eventDivision = await db.EventDivisions.Where(x => x.TournamentEventId == id).ToListAsync();
      var eventDivisionIds = eventDivision.Select(x => x.Id);
      var eventDivisionGroups = await db.DivisionGroups.Where(e => eventDivisionIds.Contains(e.EventDevisionId.Value)).Include(e => e.Competitor).ToListAsync();
      var divisions = await db.EventDivisions.Where(x => x.TournamentEventId == id).Select(x => new DivisionDropdownVM { Id = x.Id, Name = x.Name }).ToListAsync();

      var competitorDevisions = new List<CompetitorDivisionsVM>();
      if (eventDivisionGroups != null)
      {
        foreach (var divisionGroup in eventDivisionGroups)
        {
          var selectedDivisions = await db.DivisionGroups.Where(x => x.CompetitorId == divisionGroup.CompetitorId).Select(x => x.EventDevisionId).ToListAsync();
          var compDivisions = new List<SelectListItem>();
          foreach (var division in divisions)
          {
            if (selectedDivisions.Contains(division.Id))
            {
              compDivisions.Add(new SelectListItem { Text = division.Name, Value = division.Id.ToString(), Selected = true });

            }
            else
            {
              compDivisions.Add(new SelectListItem { Text = division.Name, Value = division.Id.ToString() });
            }
          }

          var eventDivisionId = divisionGroup.EventDevisionId.GetValueOrDefault();
          var ed = await db.EventDivisions.FindAsync(eventDivisionId);
          if (!competitorDevisions.Any(x => x.Competitor.Id == divisionGroup.CompetitorId))
          {
            competitorDevisions.Add(new CompetitorDivisionsVM
            {
              EventDivisionId = eventDivisionId,
              Competitor = divisionGroup.Competitor,
              Divisions = compDivisions,
              SelectedDivisionIds = selectedDivisions,
              TournamentEventId = ed?.TournamentEventId
            });
          }
        }
      }

      var eventCompetitorDGs = await db.DivisionGroups.Where(e => e.EventDevisionId == null && e.TournamentEventId == id).Include(e => e.Competitor).ToListAsync();
      if (eventCompetitorDGs != null)
      {
        foreach (var eventCompetitorDG in eventCompetitorDGs)
        {
          var compDivisions = new List<SelectListItem>();
          foreach (var division in divisions)
          {
            compDivisions.Add(new SelectListItem { Text = division.Name, Value = division.Id.ToString() });
          }

          if (!competitorDevisions.Any(x => x.Competitor.Id == eventCompetitorDG.CompetitorId))
          {
            competitorDevisions.Add(new CompetitorDivisionsVM
            {
              Competitor = eventCompetitorDG.Competitor,
              Divisions = compDivisions,
              SelectedDivisionIds = new List<int?>()
            });
          }
        }
      }

      ////ViewBag.Tournament = eventDivision.FirstOrDefault()?.Tournament?.Name;
      ViewBag.Event = eventDivision.FirstOrDefault()?.TournamentEvent?.Name;
      ViewBag.EventId = id;
      string userId = User.Identity.GetUserId();
      ViewBag.Belts =  db.Belts.Where(x => x.CreatedBy == userId).ToList();
      return View(competitorDevisions.Distinct());
    }

    public async Task<ActionResult> TournamentViewer(string Age, string Belt, string Weight, string Gender, string Event, int id, string Combination, int Group, int TotalGroup, int EventId, bool IsCustom)
    {
      beltsCollection = new List<BeltViewModel>();
      string curUserId = User.Identity.GetUserId();
      var Dbbelts = db.Belts.Where(x => x.CreatedBy == curUserId).ToList();
      foreach (var item in Dbbelts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }

      var competitors = db.Competitors.ToList();
      var filteredCompetitors = new List<CompetitorViewModel>();
      ////var eventDevisionInfo = db.EventDivisions.Include(e => e.Tournament).Include(e => e.TournamentEvent).FirstOrDefault(x => x.Id == EventId);
      var eventDevisionInfo = db.EventDivisions.Include(e => e.TournamentEvent).FirstOrDefault(x => x.Id == EventId);
      var eventDivisionGroup = db.DivisionGroups.Where(x => x.EventDevisionId == EventId).ToList();

      if (IsCustom)
      {
        if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
        {
          foreach (var divisionGroup in eventDivisionGroup)
          {
            var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
            if (competitorInfo != null)
            {
              filteredCompetitors.Add(new CompetitorViewModel
              {
                GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                Address = competitorInfo.Address,
                Age = competitorInfo.Age,
                Belt = competitorInfo.Belt,
                City = competitorInfo.City,
                Email = competitorInfo.Email,
                Event = competitorInfo.Event,
                Gender = competitorInfo.Gender,
                Id = competitorInfo.Id,
                Name = competitorInfo.Name,
                Phone = competitorInfo.Phone,
                School = competitorInfo.School,
                Serial = competitorInfo.Serial,
                State = competitorInfo.State,
                Weight = competitorInfo.Weight,
                Zip = competitorInfo.Zip
              });
            }
          }
        }
      }
      else
      {
        if (eventDivisionGroup == null || eventDivisionGroup.Count <= 0 || eventDevisionInfo == null || string.IsNullOrWhiteSpace(eventDevisionInfo.Combinations) || eventDevisionInfo.Combinations != Combination)
        {
          if (!string.IsNullOrEmpty(Age))
          {
            var ageRange = Age.Split('-');
            int.TryParse(ageRange[0], out var fromAge);
            int.TryParse(ageRange[1], out var toAge);

            filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
            {
              Address = x.Address,
              Age = x.Age,
              Belt = x.Belt,
              City = x.City,
              Email = x.Email,
              Event = x.Event,
              Gender = x.Gender,
              Id = x.Id,
              Name = x.Name,
              Phone = x.Phone,
              School = x.School,
              Serial = x.Serial,
              State = x.State,
              Weight = x.Weight,
              Zip = x.Zip
            }));
          }

          if (!string.IsNullOrEmpty(Belt))
          {
            var beltRange = Belt.Split('-');
            int.TryParse(beltRange[0], out var frombelt);
            int.TryParse(beltRange[1], out var toBelt);

            if (filteredCompetitors.Count > 0)
            {

              filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).ToList();
            }
            else
            {
              filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }
          }

          if (!string.IsNullOrEmpty(Weight))
          {
            var weightRange = Weight.Split('-');
            int.TryParse(weightRange[0], out var fromWeight);
            int.TryParse(weightRange[1], out var toWeight);

            if (filteredCompetitors.Count > 0)
            {
              filteredCompetitors = filteredCompetitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).ToList();
            }
            else
            {
              filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }
          }

          if (!string.IsNullOrEmpty(Gender))
          {
            switch (Gender)
            {
              case "0":
              case "1":
                {
                  bool gender = Gender.Equals("0") ? false : true;
                  if (filteredCompetitors.Count > 0)
                  {
                    filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && x.Gender == gender).ToList();
                  }
                  else
                  {
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                }
                break;
              case "2":
                {
                  if (filteredCompetitors.Count > 0)
                  {
                    filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).ToList();
                  }
                  else
                  {
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                }
                break;
              default:
                break;
            }
          }

          if (!string.IsNullOrWhiteSpace(Event))
          {
            filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Event) && x.Event.Contains(Event)).ToList();
          }

          if (Combination != null && !Combination.Equals("null") && !string.IsNullOrWhiteSpace(Combination))
          {
            int lastGroupAssigned = 1;
            int lastStudentGroupAssigned = 0;
            var groups = Combination.Split(',');
            foreach (var grp in groups)
            {
              var group = int.Parse(grp);
              for (int i = 0; i < group; i++)
              {
                filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                lastStudentGroupAssigned++;
              }
              lastGroupAssigned++;
            }
          }
        }
        else
        {
          if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
          {
            foreach (var divisionGroup in eventDivisionGroup)
            {
              var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
              if (competitorInfo != null)
              {
                filteredCompetitors.Add(new CompetitorViewModel
                {
                  GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                  Address = competitorInfo.Address,
                  Age = competitorInfo.Age,
                  Belt = competitorInfo.Belt,
                  City = competitorInfo.City,
                  Email = competitorInfo.Email,
                  Event = competitorInfo.Event,
                  Gender = competitorInfo.Gender,
                  Id = competitorInfo.Id,
                  Name = competitorInfo.Name,
                  Phone = competitorInfo.Phone,
                  School = competitorInfo.School,
                  Serial = competitorInfo.Serial,
                  State = competitorInfo.State,
                  Weight = competitorInfo.Weight,
                  Zip = competitorInfo.Zip
                });
              }
            }
          }
        }
      }


      var schoolGroupedCompetitor = filteredCompetitors.Where(x => x.GroupId == Group).GroupBy(info => info.School.Trim())
                        .Select(group => new
                        {
                          Key = group.Key,
                          Count = group.Count()
                        });
      var sortedGroupedCompetitors = filteredCompetitors.Where(x => x.GroupId == Group).ToList();
      foreach (var sortedGroupedCompetitor in sortedGroupedCompetitors)
      {
        sortedGroupedCompetitor.Count = schoolGroupedCompetitor.First(y => y.Key == sortedGroupedCompetitor.School.Trim()).Count;
      }

      var competitorNames = sortedGroupedCompetitors.OrderByDescending(x => x.Count).Select(x => new Tuple<string, string>(x.Name.Replace("'", "").Trim(), string.IsNullOrEmpty(x.School) ? "\'-----\'" : $"{x.School.Replace("'", "").Trim()}")).ToList();
      var totalPlayers = competitorNames.Count;

      //for (int i = 0; i <= competitorNames.Count - 2; i++)
      //{
      //  if (competitorNames[i].Item2.Equals(competitorNames[i + 1].Item2))
      //  {
      //    for (int j = i + 1; j <= competitorNames.Count - 1; j++)
      //    {
      //      if (!competitorNames[i].Item2.Equals(competitorNames[j].Item2))
      //      {
      //        var temp = competitorNames[i + 1];
      //        competitorNames[i + 1] = competitorNames[j];
      //        competitorNames[j] = temp;
      //      }
      //    }
      //  }
      //}

      var competitorGenders = filteredCompetitors.Select(x => x.Gender).ToList();
      var competitorBelts = filteredCompetitors.Select(x => x.Belt).ToList();

      if (competitorNames.Count() % 2 == 1)
      {
        competitorNames.Add(new Tuple<string, string>("--BYE--", "\'-----\'"));
      }

      //      1,8,4,5,2,7,3,6
      //and when inserting participants data insert accordingly
      //same as use 1,4,2,3 for 8 person brackets

      var teamsSchoolWise = Split(competitorNames);
      var teamsWithBye = new List<List<Tuple<string, string>>>();
      var teams = new List<List<Tuple<string, string>>>();
      var competorpairs = 4;
      var competitorsToForward = 0;
      var totalCompetitorsCompete = teamsSchoolWise.SelectMany(x => x.Select(y => y.Item1)).Count();
      if (totalCompetitorsCompete <= competorpairs)
      {
        competitorsToForward = competorpairs - totalCompetitorsCompete;
      }
      else
      {
        competorpairs += competorpairs;
        while (competorpairs < totalCompetitorsCompete)
        {
          competorpairs += competorpairs;
        }
        competitorsToForward = competorpairs - totalCompetitorsCompete;
      }

      teams.AddRange(teamsSchoolWise);

      competitorsToForward += teams.SelectMany(x => x.Where(y => y.Item1.Equals("--BYE--")).Select(y => y.Item1)).Count();

      var forwardedCompetitors = teams.SelectMany(x => x.Where(y => y.Item1.Equals("--BYE--")).Select(y => y.Item1)).Count();
      for (int j = 0; j < competitorsToForward; j++)
      {
        if (forwardedCompetitors == competitorsToForward)
        {
          break;
        }
        for (int i = teams.Count - 1; i >= 0; i--)
        {
          if (!teams[i][0].Item1.Equals("--BYE--") && !teams[i][1].Item1.Equals("--BYE--"))
          {
            teams[i] = new List<Tuple<string, string>> { teamsSchoolWise[i][0], new Tuple<string, string>("--BYE--", "\'-----\'") };
            teams.Insert(i, new List<Tuple<string, string>> { teamsSchoolWise[i][1], new Tuple<string, string>("--BYE--", "\'-----\'") });
            forwardedCompetitors += 2;
            break;
          }
        }
      }

      var pairList = new List<int>() { 1, teams.Count };
      var midPoint = new List<int>();
      var edgePoint = new List<int>() { 1, teams.Count };
      var midPointIndex = (1 + (teams.Count)) / 2;
      if (midPointIndex > 1)
      {
        pairList.Add(midPointIndex);
        pairList.Add(midPointIndex + 1);
        midPoint = new List<int>() { midPointIndex, midPointIndex + 1 };
        while (true)
        {
          if ((pairList.Contains(edgePoint[0] + 1) && pairList.Contains(edgePoint[1] - 1)) || (pairList.Contains(midPoint[0] - 1) && pairList.Contains(midPoint[1] + 1)))
          {
            break;
          }
          else
          {
            pairList.Add(edgePoint[0] + 1);
            pairList.Add(edgePoint[1] - 1);
            pairList.Add(midPoint[0] - 1);
            pairList.Add(midPoint[1] + 1);
            edgePoint = new List<int>() { edgePoint[0] + 1, edgePoint[1] - 1 };
            midPoint = new List<int>() { midPoint[0] - 1, midPoint[1] + 1 };
          }
        }
      }

      for (int i = 0; i <= teams.Count - 1; i++)
      {
        teams[i] = new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") };
      }

      int competitorIndex = 0;
      while (competitorIndex > -1)
      {
        for (int i = 0; i <= pairList.Count - 1; i++)
        {
          if (competitorIndex > competitorNames.Count - 1)
          {
            competitorIndex = -1;
            break;
          }
          else
          {
            if (teams[pairList[i] - 1][0].Item1 == "--BYE--")
            {
              teams[pairList[i] - 1][0] = competitorNames[competitorIndex];
            }
            else
            {
              teams[pairList[i] - 1][1] = competitorNames[competitorIndex];
            }
            competitorIndex++;
          }
        }
      }


      if (id == 1)
      {
        var totalCompetitors = teams.SelectMany(x => x.Select(y => y.Item1)).Count();
        if (totalCompetitors <= 4)
        {
          var teamsToAdd = (4 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors <= 8)
        {
          var teamsToAdd = (8 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 8 && totalCompetitors <= 16)
        {
          var teamsToAdd = (16 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 16 && totalCompetitors <= 32)
        {
          var teamsToAdd = (32 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 32 && totalCompetitors <= 64)
        {
          var teamsToAdd = (64 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 64)
        {
          if (totalCompetitors % 8 != 0)
          {
            var teamsToAdd = 0;
            while (true)
            {
              if (totalCompetitors % 8 != 0)
              {
                teamsToAdd++;
                totalCompetitors++;
              }
              else
              {
                break;
              }
            }

            teamsToAdd = teamsToAdd / 2;
            for (int i = 0; i < teamsToAdd; i++)
            {
              teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
            }
          }
        }
      }
      else
      {
        var totalCompetitors = teams.SelectMany(x => x.Select(y => y.Item1)).Count();
        if (totalCompetitors > 4 && totalCompetitors % 8 != 0)
        {
          var teamsToAdd = 0;
          while (true)
          {
            if (totalCompetitors % 8 != 0)
            {
              teamsToAdd++;
              totalCompetitors++;
            }
            else
            {
              break;
            }
          }

          teamsToAdd = teamsToAdd / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
      }

      competitorNames = teams.SelectMany(x => x).ToList();

      var teamsJS = new StringBuilder();
      foreach (var team in teams)
      {
        teamsJS.Append($"[\'{team[0].Item1}\',\'{team[1].Item1}\'],");
      }

      ////ViewBag.Tournament = eventDevisionInfo?.Tournament?.Name;
      ViewBag.Event = eventDevisionInfo?.TournamentEvent?.Name;
      ViewBag.Division = eventDevisionInfo?.Name;
      ViewBag.RingAssignment = eventDevisionInfo?.RingAssignment;

      string genderText = string.Empty;
      if (competitorGenders.Distinct().All(x => x == true))
      {
        genderText = "Boys";
      }
      else if (competitorGenders.All(x => x == false))
      {
        genderText = "Girls";
      }
      else
      {
        genderText = "Boys & Girls";
      }

      var belts = new List<BeltViewModel>();
      foreach (var belt in competitorBelts)
      {
        if (belt != null && !belts.Any(x => x.Id == int.Parse(belt)))
        {
          belts.Add(new BeltViewModel { Id = int.Parse(belt), Name = beltsCollection.FirstOrDefault(y => y.Id == int.Parse(belt))?.Name });
        }
      }

      string beltText = "-";
      if (belts.Count > 0)
      {
        var orderedbelts = belts.OrderBy(x => x.Id).ToList();
        beltText = $"{orderedbelts.First().Name} - {orderedbelts.Last().Name}";
      }

      switch (id)
      {
        case 1:
          {
            var firstRound = teams;
            var firstRoundJS = new StringBuilder();
            firstRoundJS.Append("[");
            foreach (var team in teams)
            {
              if (
                //(!team[0].Item2.Equals("'-----'") && !team[1].Item2.Equals("'-----'") && team[0].Item2.Equals(team[1].Item2)) ||
                (team[0].Item2.Equals("'-----'") && !team[1].Item2.Equals("'-----'")))
              {
                firstRoundJS.Append("[0,1],");
              }
              else if (!team[0].Item2.Equals("'-----'") && team[1].Item2.Equals("'-----'"))
              {
                firstRoundJS.Append("[1,0],");
              }
              else
              {
                firstRoundJS.Append("[null,null],");
              }
            }
            firstRoundJS.Append("],");

            var roundResults = new StringBuilder();
            roundResults.Append(firstRoundJS.ToString());

            var remaining = competitorNames.Count / 2;
            while (true)
            {
              if (remaining % 2 == 1)
              {
                remaining++;
              }

              if (remaining == 2)
              {
                roundResults.Append("[[null,null],[null,null]]");
                break;
              }

              var remTeams = Split(Enumerable.Range(1, remaining).ToList());

              var roundString = new StringBuilder();
              roundString.Append("[");
              foreach (var remTeam in remTeams)
              {
                roundString.Append("[null,null],");
              }
              roundString.Append("],");
              roundResults.Append(roundString.ToString());
              remaining = remaining / 2;
            }
            ViewBag.BracketEl = "Single Elimination";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", roundResults.ToString(), "]");
          }
          break;
        case 2:
          {
            var winnerBrackets = new StringBuilder();
            var loserBrackets = new StringBuilder();
            var winRemaining = competitorNames.Count() / 2;
            bool isWinnerSet = false;
            while (true)
            {
              bool isMatchedCompeForwward = false;
              int j = 0;
              if (winRemaining == 1)
              {
                winnerBrackets.Append("[[null,null]]");
                loserBrackets.Append("[[null,null]]]");
                break;
              }

              winnerBrackets.Append("[");
              loserBrackets.Append("[");
              for (int i = 0; i < winRemaining; i++)
              {
                if (!isWinnerSet)
                {
                  var firstCompSchool = competitorNames.ElementAt(j).Item2;
                  var secCompSchool = competitorNames.ElementAt(j + 1).Item2;
                  if (
                    //(!firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'") && firstCompSchool.Equals(secCompSchool)) ||
                    (firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'")))
                  {
                    winnerBrackets.Append("[0,1],");
                    loserBrackets.Append("[null,null],");
                  }
                  else if (!firstCompSchool.Equals("'-----'") && secCompSchool.Equals("'-----'"))
                  {
                    winnerBrackets.Append("[1,0],");
                    loserBrackets.Append("[null,null],");
                  }
                  else
                  {
                    winnerBrackets.Append("[null,null],");
                    loserBrackets.Append("[null,null],");
                  }
                  isMatchedCompeForwward = true;
                  j += 2;
                }
                else
                {
                  winnerBrackets.Append("[null,null],");
                  loserBrackets.Append("[null,null],");
                }
              }

              if (isMatchedCompeForwward)
              {
                isWinnerSet = true;
              }

              winnerBrackets.Append("],");
              loserBrackets.Append("],");
              winRemaining = winRemaining / 2;
            }

            ViewBag.BracketEl = "Double Elimination";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", winnerBrackets.ToString(), "]", ",", "[", loserBrackets.ToString());
          }
          break;
        case 3:
          {
            ViewBag.GroupMessage = $"Group {Group} of {TotalGroup}";
            ViewBag.Bracket = "Round Robin Elimination";
            ViewBag.GenderText = genderText;
            ViewBag.BeltDevision = beltText;
            return View("TournamentViewerRoundRobin", ListMatches(competitorNames.Select(x => x.Item1).ToList(), competitorNames.Select(x => x.Item2).ToList(), competitorNames.Count));
          }
        case 4:
          {
            ViewBag.GroupMessage = $"Group {Group} of {TotalGroup}";
            ViewBag.Bracket = "List Elimination";
            ViewBag.GenderText = genderText;
            ViewBag.BeltDevision = beltText;
            return View("TournamentViewerList", competitorNames.Select(x => x.Item1).ToList());
          }
        case 5:
          {
            var winnerBrackets = new StringBuilder();
            var loserBrackets = new StringBuilder();
            var winRemaining = competitorNames.Count() / 2;
            bool isWinnerSet = false;
            while (true)
            {
              bool isMatchedCompeForwward = false;
              int j = 0;
              if (winRemaining == 1)
              {
                winnerBrackets.Append("[[null,null]]");
                loserBrackets.Append("[[null,null]]]");
                break;
              }

              winnerBrackets.Append("[");
              loserBrackets.Append("[");
              for (int i = 0; i < winRemaining; i++)
              {
                if (!isWinnerSet)
                {
                  var firstCompSchool = competitorNames.ElementAt(j).Item2;
                  var secCompSchool = competitorNames.ElementAt(j + 1).Item2;
                  if (
                    //(!firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'") && firstCompSchool.Equals(secCompSchool)) ||
                    (firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'")))
                  {
                    winnerBrackets.Append("[0,1],");
                    loserBrackets.Append("[null,null],");
                  }
                  else if (!firstCompSchool.Equals("'-----'") && secCompSchool.Equals("'-----'"))
                  {
                    winnerBrackets.Append("[1,0],");
                    loserBrackets.Append("[null,null],");
                  }
                  else
                  {
                    winnerBrackets.Append("[null,null],");
                    loserBrackets.Append("[null,null],");
                  }
                  isMatchedCompeForwward = true;
                  j += 2;
                }
                else
                {
                  winnerBrackets.Append("[null,null],");
                  loserBrackets.Append("[null,null],");
                }
              }

              if (isMatchedCompeForwward)
              {
                isWinnerSet = true;
              }

              winnerBrackets.Append("],");
              loserBrackets.Append("],");
              winRemaining = winRemaining / 2;
            }

            ViewBag.BracketEl = "2-Match Gurantee";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", winnerBrackets.ToString(), "]", ",", "[", loserBrackets.ToString());
          }
          break;
        default:
          break;
      }

      var schoolNamesArray = new StringBuilder();
      schoolNamesArray.Append("[");
      schoolNamesArray.Append(string.Join(",", competitorNames.Select(x => x.Item2.Equals("'-----'") ? x.Item2 : string.Concat("'", x.Item2, "'")).ToList()));
      schoolNamesArray.Append("]");

      var comSchoolNamesArray = new StringBuilder();
      comSchoolNamesArray.Append("{");

      foreach (var item in competitorNames)
      {
        comSchoolNamesArray.Append($"'{item.Item1}':{(item.Item2.Equals("'-----'") ? item.Item2 : string.Concat("'", item.Item2, "'"))},");
      }
      comSchoolNamesArray.Append("}");

      var competitorTBC = comSchoolNamesArray.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
      ////var existCompSortOrd = await db.CompetitorTournamentOrders.Where(x => x.Division.Equals(eventDevisionInfo.Name) && x.Tournament.Equals(eventDevisionInfo.Tournament.Name) && x.Event.Equals(eventDevisionInfo.TournamentEvent.Name) && competitorTBC.Contains(x.Competitor)).Select(x => x.Competitor).ToListAsync();
      var existCompSortOrd = await db.CompetitorTournamentOrders.Where(x => x.Division.Equals(eventDevisionInfo.Name)  && x.Event.Equals(eventDevisionInfo.TournamentEvent.Name) && competitorTBC.Contains(x.Competitor)).Select(x => x.Competitor).ToListAsync();
      if (existCompSortOrd != null && competitorTBC.All(existCompSortOrd.Contains) && competitorTBC.Count == existCompSortOrd.Count)
      {
        ////return await TournamentViewerV2(string.Join(",", existCompSortOrd), eventDevisionInfo?.Tournament?.Name, eventDevisionInfo?.TournamentEvent?.Name, eventDevisionInfo?.Name, eventDevisionInfo?.RingAssignment, id, genderText, beltText, $"Group {Group} of {TotalGroup}");
        return await TournamentViewerV2(string.Join(",", existCompSortOrd), "", eventDevisionInfo?.TournamentEvent?.Name, eventDevisionInfo?.Name, eventDevisionInfo?.RingAssignment, id, genderText, beltText, $"Group {Group} of {TotalGroup}");
      }

      ViewBag.ComSchoolArray = comSchoolNamesArray.ToString();
      ViewBag.SchoolArray = schoolNamesArray.ToString();
      ViewBag.GenderText = genderText;
      ViewBag.BeltDevision = beltText;
      ViewBag.GroupMessage = $"Group {Group} of {TotalGroup}";
      ViewBag.Bracket = id;

      if (totalPlayers == 3 && competitorNames.Count == 4)
      {
        ViewBag.AreThreePlayersInTournament = "true";
        var SecondMatchCompetitorInfo = teams.First(x => x.Any(y => y.Item1.Equals("--BYE--"))).First(x => !x.Item1.Equals("--BYE--"));
        ViewBag.SecondMatchCompetitor = SecondMatchCompetitorInfo.Item1;
        ViewBag.SecondMatchCompetitorSchool = SecondMatchCompetitorInfo.Item2;
      }
      else if (totalPlayers == 2 && competitorNames.Count == 4)
      {
        return View("TournamentViewerTwoPlayers", competitorNames.Where(y => !y.Item1.Equals("--BYE--")).ToList());
      }
      return View();
    }

    public async Task<ActionResult> TournamentViewerV1(string Age, string Belt, string Weight, string Gender, string Event, int id, int EventId, bool IsCustom)
    {
      beltsCollection = new List<BeltViewModel>();
      string curUserId = User.Identity.GetUserId();
      var Dbbelts = db.Belts.Where(x => x.CreatedBy == curUserId).ToList();
      foreach (var item in Dbbelts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }
      var competitors = db.Competitors.ToList();
      var filteredCompetitors = new List<CompetitorViewModel>();
      ////var eventDevisionInfo = db.EventDivisions.Include(e => e.Tournament).Include(e => e.TournamentEvent).FirstOrDefault(x => x.Id == EventId);
      var eventDevisionInfo = db.EventDivisions.Include(e => e.TournamentEvent).FirstOrDefault(x => x.Id == EventId);
      var eventDivisionGroup = db.DivisionGroups.Where(x => x.EventDevisionId == EventId).ToList();

      if (IsCustom)
      {
        if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
        {
          foreach (var divisionGroup in eventDivisionGroup)
          {
            var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
            if (competitorInfo != null)
            {
              filteredCompetitors.Add(new CompetitorViewModel
              {
                GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                Address = competitorInfo.Address,
                Age = competitorInfo.Age,
                Belt = competitorInfo.Belt,
                City = competitorInfo.City,
                Email = competitorInfo.Email,
                Event = competitorInfo.Event,
                Gender = competitorInfo.Gender,
                Id = competitorInfo.Id,
                Name = competitorInfo.Name,
                Phone = competitorInfo.Phone,
                School = competitorInfo.School,
                Serial = competitorInfo.Serial,
                State = competitorInfo.State,
                Weight = competitorInfo.Weight,
                Zip = competitorInfo.Zip
              });
            }
          }
        }
      }
      else
      {
        if (eventDivisionGroup == null || eventDivisionGroup.Count <= 0 || eventDevisionInfo == null || string.IsNullOrWhiteSpace(eventDevisionInfo.Combinations))
        {
          if (!string.IsNullOrEmpty(Age))
          {
            var ageRange = Age.Split('-');
            int.TryParse(ageRange[0], out var fromAge);
            int.TryParse(ageRange[1], out var toAge);

            filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
            {
              Address = x.Address,
              Age = x.Age,
              Belt = x.Belt,
              City = x.City,
              Email = x.Email,
              Event = x.Event,
              Gender = x.Gender,
              Id = x.Id,
              Name = x.Name,
              Phone = x.Phone,
              School = x.School,
              Serial = x.Serial,
              State = x.State,
              Weight = x.Weight,
              Zip = x.Zip
            }));
          }

          if (!string.IsNullOrEmpty(Belt))
          {
            var beltRange = Belt.Split('-');
            int.TryParse(beltRange[0], out var frombelt);
            int.TryParse(beltRange[1], out var toBelt);

            if (filteredCompetitors.Count > 0)
            {

              filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).ToList();
            }
            else
            {
              filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }
          }

          if (!string.IsNullOrEmpty(Weight))
          {
            var weightRange = Weight.Split('-');
            int.TryParse(weightRange[0], out var fromWeight);
            int.TryParse(weightRange[1], out var toWeight);

            if (filteredCompetitors.Count > 0)
            {
              filteredCompetitors = filteredCompetitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).ToList();
            }
            else
            {
              filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }
          }

          if (!string.IsNullOrEmpty(Gender))
          {
            switch (Gender)
            {
              case "0":
              case "1":
                {
                  bool gender = Gender.Equals("0") ? false : true;
                  if (filteredCompetitors.Count > 0)
                  {
                    filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && x.Gender == gender).ToList();
                  }
                  else
                  {
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                }
                break;
              case "2":
                {
                  if (filteredCompetitors.Count > 0)
                  {
                    filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).ToList();
                  }
                  else
                  {
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                }
                break;
              default:
                break;
            }
          }

          if (!string.IsNullOrWhiteSpace(Event))
          {
            filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Event) && x.Event.Contains(Event)).ToList();
          }

          filteredCompetitors.ForEach(x =>
          {
            x.GroupId = 1;
          });
        }
        else
        {
          if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
          {
            foreach (var divisionGroup in eventDivisionGroup)
            {
              var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
              if (competitorInfo != null)
              {
                filteredCompetitors.Add(new CompetitorViewModel
                {
                  GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                  Address = competitorInfo.Address,
                  Age = competitorInfo.Age,
                  Belt = competitorInfo.Belt,
                  City = competitorInfo.City,
                  Email = competitorInfo.Email,
                  Event = competitorInfo.Event,
                  Gender = competitorInfo.Gender,
                  Id = competitorInfo.Id,
                  Name = competitorInfo.Name,
                  Phone = competitorInfo.Phone,
                  School = competitorInfo.School,
                  Serial = competitorInfo.Serial,
                  State = competitorInfo.State,
                  Weight = competitorInfo.Weight,
                  Zip = competitorInfo.Zip
                });
              }
            }
          }
        }
      }

      var schoolGroupedCompetitor = filteredCompetitors.GroupBy(info => info.School.Trim())
                        .Select(group => new
                        {
                          Key = group.Key,
                          Count = group.Count()
                        });
      var sortedGroupedCompetitors = filteredCompetitors.ToList();
      foreach (var sortedGroupedCompetitor in sortedGroupedCompetitors)
      {
        sortedGroupedCompetitor.Count = schoolGroupedCompetitor.First(y => y.Key == sortedGroupedCompetitor.School.Trim()).Count;
      }

      var competitorNames = sortedGroupedCompetitors.OrderByDescending(x => x.Count).Select(x => new Tuple<string, string>(x.Name.Replace("'", "").Trim(), string.IsNullOrEmpty(x.School) ? "\'-----\'" : $"{x.School.Replace("'", "").Trim()}")).ToList();
      var totalPlayers = competitorNames.Count;

      //for (int i = 0; i <= competitorNames.Count - 2; i++)
      //{
      //  if (competitorNames[i].Item2.Equals(competitorNames[i + 1].Item2))
      //  {
      //    for (int j = i + 1; j <= competitorNames.Count - 1; j++)
      //    {
      //      if (!competitorNames[i].Item2.Equals(competitorNames[j].Item2))
      //      {
      //        var temp = competitorNames[i + 1];
      //        competitorNames[i + 1] = competitorNames[j];
      //        competitorNames[j] = temp;
      //      }
      //    }
      //  }
      //}

      var competitorGenders = filteredCompetitors.Select(x => x.Gender).ToList();
      var competitorBelts = filteredCompetitors.Select(x => x.Belt).ToList();

      if (competitorNames.Count() % 2 == 1)
      {
        competitorNames.Add(new Tuple<string, string>("--BYE--", "\'-----\'"));
      }

      var teamsSchoolWise = Split(competitorNames);
      var teamsWithBye = new List<List<Tuple<string, string>>>();
      var teams = new List<List<Tuple<string, string>>>();
      var competorpairs = 4;
      var competitorsToForward = 0;
      var totalCompetitorsCompete = teamsSchoolWise.SelectMany(x => x.Select(y => y.Item1)).Count();
      if (totalCompetitorsCompete <= competorpairs)
      {
        competitorsToForward = competorpairs - totalCompetitorsCompete;
      }
      else
      {
        competorpairs += competorpairs;
        while (competorpairs < totalCompetitorsCompete)
        {
          competorpairs += competorpairs;
        }
        competitorsToForward = competorpairs - totalCompetitorsCompete;
      }

      teams.AddRange(teamsSchoolWise);

      competitorsToForward += teams.SelectMany(x => x.Where(y => y.Item1.Equals("--BYE--")).Select(y => y.Item1)).Count();

      var forwardedCompetitors = teams.SelectMany(x => x.Where(y => y.Item1.Equals("--BYE--")).Select(y => y.Item1)).Count();
      for (int j = 0; j < competitorsToForward; j++)
      {
        if (forwardedCompetitors == competitorsToForward)
        {
          break;
        }
        for (int i = teams.Count - 1; i >= 0; i--)
        {
          if (!teams[i][0].Item1.Equals("--BYE--") && !teams[i][1].Item1.Equals("--BYE--"))
          {
            teams[i] = new List<Tuple<string, string>> { teamsSchoolWise[i][0], new Tuple<string, string>("--BYE--", "\'-----\'") };
            teams.Insert(i, new List<Tuple<string, string>> { teamsSchoolWise[i][1], new Tuple<string, string>("--BYE--", "\'-----\'") });
            forwardedCompetitors += 2;
            break;
          }
        }
      }

      var pairList = new List<int>() { 1, teams.Count };
      var midPoint = new List<int>();
      var edgePoint = new List<int>() { 1, teams.Count };
      var midPointIndex = (1 + (teams.Count)) / 2;
      if (midPointIndex > 1)
      {
        pairList.Add(midPointIndex);
        pairList.Add(midPointIndex + 1);
        midPoint = new List<int>() { midPointIndex, midPointIndex + 1 };
        while (true)
        {
          if ((pairList.Contains(edgePoint[0] + 1) && pairList.Contains(edgePoint[1] - 1)) || (pairList.Contains(midPoint[0] - 1) && pairList.Contains(midPoint[1] + 1)))
          {
            break;
          }
          else
          {
            pairList.Add(edgePoint[0] + 1);
            pairList.Add(edgePoint[1] - 1);
            pairList.Add(midPoint[0] - 1);
            pairList.Add(midPoint[1] + 1);
            edgePoint = new List<int>() { edgePoint[0] + 1, edgePoint[1] - 1 };
            midPoint = new List<int>() { midPoint[0] - 1, midPoint[1] + 1 };
          }
        }
      }

      for (int i = 0; i <= teams.Count - 1; i++)
      {
        teams[i] = new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") };
      }

      int competitorIndex = 0;
      while (competitorIndex > -1)
      {
        for (int i = 0; i <= pairList.Count - 1; i++)
        {
          if (competitorIndex > competitorNames.Count - 1)
          {
            competitorIndex = -1;
            break;
          }
          else
          {
            if (teams[pairList[i] - 1][0].Item1 == "--BYE--")
            {
              teams[pairList[i] - 1][0] = competitorNames[competitorIndex];
            }
            else
            {
              teams[pairList[i] - 1][1] = competitorNames[competitorIndex];
            }
            competitorIndex++;
          }
        }
      }

      if (id == 1)
      {
        var totalCompetitors = teams.SelectMany(x => x.Select(y => y.Item1)).Count();
        if (totalCompetitors <= 4)
        {
          var teamsToAdd = (4 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors <= 8)
        {
          var teamsToAdd = (8 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 8 && totalCompetitors <= 16)
        {
          var teamsToAdd = (16 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 16 && totalCompetitors <= 32)
        {
          var teamsToAdd = (32 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 32 && totalCompetitors <= 64)
        {
          var teamsToAdd = (64 - totalCompetitors) / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
        else if (totalCompetitors > 64)
        {
          if (totalCompetitors % 8 != 0)
          {
            var teamsToAdd = 0;
            while (true)
            {
              if (totalCompetitors % 8 != 0)
              {
                teamsToAdd++;
                totalCompetitors++;
              }
              else
              {
                break;
              }
            }

            teamsToAdd = teamsToAdd / 2;
            for (int i = 0; i < teamsToAdd; i++)
            {
              teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
            }
          }
        }
      }
      else
      {
        var totalCompetitors = teams.SelectMany(x => x.Select(y => y.Item1)).Count();
        if (totalCompetitors > 4 && totalCompetitors % 8 != 0)
        {
          var teamsToAdd = 0;
          while (true)
          {
            if (totalCompetitors % 8 != 0)
            {
              teamsToAdd++;
              totalCompetitors++;
            }
            else
            {
              break;
            }
          }

          teamsToAdd = teamsToAdd / 2;
          for (int i = 0; i < teamsToAdd; i++)
          {
            teams.Add(new List<Tuple<string, string>> { new Tuple<string, string>("--BYE--", "\'-----\'"), new Tuple<string, string>("--BYE--", "\'-----\'") });
          }
        }
      }

      competitorNames = teams.SelectMany(x => x).ToList();

      var teamsJS = new StringBuilder();
      foreach (var team in teams)
      {
        teamsJS.Append($"[\'{team[0].Item1}\',\'{team[1].Item1}\'],");
      }

      ////ViewBag.Tournament = eventDevisionInfo?.Tournament?.Name;
      ViewBag.Event = eventDevisionInfo?.TournamentEvent?.Name;
      ViewBag.Division = eventDevisionInfo?.Name;
      ViewBag.RingAssignment = eventDevisionInfo?.RingAssignment;

      string genderText = string.Empty;
      if (competitorGenders.Distinct().All(x => x == true))
      {
        genderText = "Boys";
      }
      else if (competitorGenders.All(x => x == false))
      {
        genderText = "Girls";
      }
      else
      {
        genderText = "Boys & Girls";
      }

      var belts = new List<BeltViewModel>();
      foreach (var belt in competitorBelts)
      {
        if (belt != null && !belts.Any(x => x.Id == int.Parse(belt)))
        {
          belts.Add(new BeltViewModel { Id = int.Parse(belt), Name = beltsCollection.FirstOrDefault(y => y.Id == int.Parse(belt))?.Name });
        }
      }

      string beltText = "-";
      if (belts.Count > 0)
      {
        var orderedbelts = belts.OrderBy(x => x.Id).ToList();
        beltText = $"{orderedbelts.First().Name} - {orderedbelts.Last().Name}";
      }

      switch (id)
      {
        case 1:
          {
            var firstRound = teams;
            var firstRoundJS = new StringBuilder();
            firstRoundJS.Append("[");
            foreach (var team in teams)
            {
              if (
                //(!team[0].Item2.Equals("'-----'") && !team[1].Item2.Equals("'-----'") && team[0].Item2.Equals(team[1].Item2)) ||
                (team[0].Item2.Equals("'-----'") && !team[1].Item2.Equals("'-----'")))
              {
                firstRoundJS.Append("[0,1],");
              }
              else if (!team[0].Item2.Equals("'-----'") && team[1].Item2.Equals("'-----'"))
              {
                firstRoundJS.Append("[1,0],");
              }
              else
              {
                firstRoundJS.Append("[null,null],");
              }
            }
            firstRoundJS.Append("],");

            var roundResults = new StringBuilder();
            roundResults.Append(firstRoundJS.ToString());

            var remaining = competitorNames.Count / 2;
            while (true)
            {
              if (remaining % 2 == 1)
              {
                remaining++;
              }

              if (remaining == 2)
              {
                roundResults.Append("[[null,null],[null,null]]");
                break;
              }

              var remTeams = Split(Enumerable.Range(1, remaining).ToList());

              var roundString = new StringBuilder();
              roundString.Append("[");
              foreach (var remTeam in remTeams)
              {
                roundString.Append("[null,null],");
              }
              roundString.Append("],");
              roundResults.Append(roundString.ToString());
              remaining = remaining / 2;
            }
            ViewBag.BracketEl = "Single Elimination";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", roundResults.ToString(), "]");
          }
          break;
        case 2:
          {
            var winnerBrackets = new StringBuilder();
            var loserBrackets = new StringBuilder();
            var winRemaining = competitorNames.Count() / 2;
            bool isWinnerSet = false;
            while (true)
            {
              bool isMatchedCompeForwward = false;
              int j = 0;
              if (winRemaining == 1)
              {
                winnerBrackets.Append("[[null,null]]");
                loserBrackets.Append("[[null,null]]]");
                break;
              }

              winnerBrackets.Append("[");
              loserBrackets.Append("[");
              for (int i = 0; i < winRemaining; i++)
              {
                if (!isWinnerSet)
                {
                  var firstCompSchool = competitorNames.ElementAt(j).Item2;
                  var secCompSchool = competitorNames.ElementAt(j + 1).Item2;
                  if (
                    //(!firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'") && firstCompSchool.Equals(secCompSchool)) || 
                    (firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'")))
                  {
                    winnerBrackets.Append("[0,1],");
                    loserBrackets.Append("[null,null],");
                  }
                  else if (!firstCompSchool.Equals("'-----'") && secCompSchool.Equals("'-----'"))
                  {
                    winnerBrackets.Append("[1,0],");
                    loserBrackets.Append("[null,null],");
                  }
                  else
                  {
                    winnerBrackets.Append("[null,null],");
                    loserBrackets.Append("[null,null],");
                  }
                  isMatchedCompeForwward = true;
                  j += 2;
                }
                else
                {
                  winnerBrackets.Append("[null,null],");
                  loserBrackets.Append("[null,null],");
                }
              }

              if (isMatchedCompeForwward)
              {
                isWinnerSet = true;
              }

              winnerBrackets.Append("],");
              loserBrackets.Append("],");
              winRemaining = winRemaining / 2;
            }

            ViewBag.BracketEl = "Double Elimination";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", winnerBrackets.ToString(), "]", ",", "[", loserBrackets.ToString());
          }
          break;
        case 3:
          {
            ViewBag.GroupMessage = $"Group 1 of 1";
            ViewBag.Bracket = "Round Robin Elimination";
            ViewBag.GenderText = genderText;
            ViewBag.BeltDevision = beltText;
            return View("TournamentViewerRoundRobin", ListMatches(competitorNames.Select(x => x.Item1).ToList(), competitorNames.Select(x => x.Item2).ToList(), competitorNames.Count));
          }
        case 4:
          {
            ViewBag.GroupMessage = $"Group 1 of 1";
            ViewBag.Bracket = "List Elimination";
            ViewBag.GenderText = genderText;
            ViewBag.BeltDevision = beltText;
            return View("TournamentViewerList", competitorNames.Select(x => x.Item1).ToList());
          }
        case 5:
          {
            var winnerBrackets = new StringBuilder();
            var loserBrackets = new StringBuilder();
            var winRemaining = competitorNames.Count() / 2;
            bool isWinnerSet = false;
            while (true)
            {
              bool isMatchedCompeForwward = false;
              int j = 0;
              if (winRemaining == 1)
              {
                winnerBrackets.Append("[[null,null]]");
                loserBrackets.Append("[[null,null]]]");
                break;
              }

              winnerBrackets.Append("[");
              loserBrackets.Append("[");
              for (int i = 0; i < winRemaining; i++)
              {
                if (!isWinnerSet)
                {
                  var firstCompSchool = competitorNames.ElementAt(j).Item2;
                  var secCompSchool = competitorNames.ElementAt(j + 1).Item2;
                  if (
                    //(!firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'") && firstCompSchool.Equals(secCompSchool)) ||
                    (firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'")))
                  {
                    winnerBrackets.Append("[0,1],");
                    loserBrackets.Append("[null,null],");
                  }
                  else if (!firstCompSchool.Equals("'-----'") && secCompSchool.Equals("'-----'"))
                  {
                    winnerBrackets.Append("[1,0],");
                    loserBrackets.Append("[null,null],");
                  }
                  else
                  {
                    winnerBrackets.Append("[null,null],");
                    loserBrackets.Append("[null,null],");
                  }
                  isMatchedCompeForwward = true;
                  j += 2;
                }
                else
                {
                  winnerBrackets.Append("[null,null],");
                  loserBrackets.Append("[null,null],");
                }
              }

              if (isMatchedCompeForwward)
              {
                isWinnerSet = true;
              }

              winnerBrackets.Append("],");
              loserBrackets.Append("],");
              winRemaining = winRemaining / 2;
            }

            ViewBag.BracketEl = "2-Match Gurantee";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", winnerBrackets.ToString(), "]", ",", "[", loserBrackets.ToString());
          }
          break;
        default:
          break;
      }

      var schoolNamesArray = new StringBuilder();
      schoolNamesArray.Append("[");
      schoolNamesArray.Append(string.Join(",", competitorNames.Select(x => x.Item2.Equals("'-----'") ? x.Item2 : string.Concat("'", x.Item2, "'")).ToList()));
      schoolNamesArray.Append("]");

      var comSchoolNamesArray = new StringBuilder();
      comSchoolNamesArray.Append("{");

      foreach (var item in competitorNames)
      {
        comSchoolNamesArray.Append($"'{item.Item1}':{(item.Item2.Equals("'-----'") ? item.Item2 : string.Concat("'", item.Item2, "'"))},");
      }
      comSchoolNamesArray.Append("}");

      var competitorTBC = comSchoolNamesArray.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
      ////var existCompSortOrd = await db.CompetitorTournamentOrders.Where(x => x.Division.Equals(eventDevisionInfo.Name) && x.Tournament.Equals(eventDevisionInfo.Tournament.Name) && x.Event.Equals(eventDevisionInfo.TournamentEvent.Name) && competitorTBC.Contains(x.Competitor)).Select(x => x.Competitor).ToListAsync();
      var existCompSortOrd = await db.CompetitorTournamentOrders.Where(x => x.Division.Equals(eventDevisionInfo.Name)  && x.Event.Equals(eventDevisionInfo.TournamentEvent.Name) && competitorTBC.Contains(x.Competitor)).Select(x => x.Competitor).ToListAsync();
      if (existCompSortOrd != null && competitorTBC.All(existCompSortOrd.Contains) && competitorTBC.Count == existCompSortOrd.Count)
      {
        ////return await TournamentViewerV2(string.Join(",", existCompSortOrd), eventDevisionInfo?.Tournament?.Name, eventDevisionInfo?.TournamentEvent?.Name, eventDevisionInfo?.Name, eventDevisionInfo?.RingAssignment, id, genderText, beltText, $"Group 1 of 1");
        return await TournamentViewerV2(string.Join(",", existCompSortOrd), "",eventDevisionInfo?.TournamentEvent?.Name, eventDevisionInfo?.Name, eventDevisionInfo?.RingAssignment, id, genderText, beltText, $"Group 1 of 1");
      }

      ViewBag.ComSchoolArray = comSchoolNamesArray.ToString();
      ViewBag.SchoolArray = schoolNamesArray.ToString();
      ViewBag.GenderText = genderText;
      ViewBag.BeltDevision = beltText;
      ViewBag.GroupMessage = $"Group 1 of 1";
      ViewBag.Bracket = id;

      if (totalPlayers == 3 && competitorNames.Count == 4)
      {
        ViewBag.AreThreePlayersInTournament = "true";
        var SecondMatchCompetitorInfo = teams.First(x => x.Any(y => y.Item1.Equals("--BYE--"))).First(x => !x.Item1.Equals("--BYE--"));
        ViewBag.SecondMatchCompetitor = SecondMatchCompetitorInfo.Item1;
        ViewBag.SecondMatchCompetitorSchool = SecondMatchCompetitorInfo.Item2;
      }
      else if (totalPlayers == 2 && competitorNames.Count == 4)
      {
        return View("TournamentViewerTwoPlayers", competitorNames.Where(y => !y.Item1.Equals("--BYE--")).ToList());
      }

      return View("TournamentViewer");
    }

    [HttpPost]
    public async Task<ActionResult> TournamentViewerV2(string Competitors, string Tournament, string Event, string Division, string RingAssignment, int id, string GenderText, string BeltText, string GroupMessage)
    {
      var competitors = Competitors.Split(',');
      var competitorNames = await UpdateCompetitorsSortOrder(Tournament, Event, Division, competitors);
      var totalPlayers = competitorNames.Where(y => !y.Contains("--BYE--")).ToList().Count();

      var teams = Split(competitorNames);

      var teamsJS = new StringBuilder();
      foreach (var team in teams)
      {
        teamsJS.Append($"[{team[0].Split(':')[0]},{team[1].Split(':')[0]}],");
      }

      ViewBag.Tournament = Tournament;
      ViewBag.Event = Event;
      ViewBag.Division = Division;
      ViewBag.RingAssignment = RingAssignment;

      switch (id)
      {
        case 1:
          {
            var firstRound = teams;
            var firstRoundJS = new StringBuilder();
            firstRoundJS.Append("[");
            foreach (var team in teams)
            {
              if (
                //(!team[0].Item2.Equals("'-----'") && !team[1].Item2.Equals("'-----'") && team[0].Item2.Equals(team[1].Item2)) ||
                (team[0].Split(':')[1].Equals("'-----'") && !team[1].Split(':')[1].Equals("'-----'")))
              {
                firstRoundJS.Append("[0,1],");
              }
              else if (!team[0].Split(':')[1].Equals("'-----'") && team[1].Split(':')[1].Equals("'-----'"))
              {
                firstRoundJS.Append("[1,0],");
              }
              else
              {
                firstRoundJS.Append("[null,null],");
              }
            }
            firstRoundJS.Append("],");

            var roundResults = new StringBuilder();
            roundResults.Append(firstRoundJS.ToString());

            var remaining = competitorNames.Count() / 2;
            while (true)
            {
              if (remaining % 2 == 1)
              {
                remaining++;
              }

              if (remaining == 2)
              {
                roundResults.Append("[[null,null],[null,null]]");
                break;
              }

              var remTeams = Split(Enumerable.Range(1, remaining).ToList());

              var roundString = new StringBuilder();
              roundString.Append("[");
              foreach (var remTeam in remTeams)
              {
                roundString.Append("[null,null],");
              }
              roundString.Append("],");
              roundResults.Append(roundString.ToString());
              remaining = remaining / 2;
            }
            ViewBag.BracketEl = "Single Elimination";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", roundResults.ToString(), "]");
          }
          break;
        case 2:
          {
            var winnerBrackets = new StringBuilder();
            var loserBrackets = new StringBuilder();
            var winRemaining = competitorNames.Count() / 2;
            bool isWinnerSet = false;
            while (true)
            {
              bool isMatchedCompeForwward = false;
              int j = 0;
              if (winRemaining == 1)
              {
                winnerBrackets.Append("[[null,null]]");
                loserBrackets.Append("[[null,null]]]");
                break;
              }

              winnerBrackets.Append("[");
              loserBrackets.Append("[");
              for (int i = 0; i < winRemaining; i++)
              {
                if (!isWinnerSet)
                {
                  var firstCompSchool = competitorNames.ElementAt(j).Split(':')[1];
                  var secCompSchool = competitorNames.ElementAt(j + 1).Split(':')[1];
                  if (
                    //(!firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'") && firstCompSchool.Equals(secCompSchool)) || 
                    (firstCompSchool.Equals("'-----'") && !secCompSchool.Equals("'-----'")))
                  {
                    winnerBrackets.Append("[0,1],");
                    loserBrackets.Append("[null,null],");
                  }
                  else if (!firstCompSchool.Equals("'-----'") && secCompSchool.Equals("'-----'"))
                  {
                    winnerBrackets.Append("[1,0],");
                    loserBrackets.Append("[null,null],");
                  }
                  else
                  {
                    winnerBrackets.Append("[null,null],");
                    loserBrackets.Append("[null,null],");
                  }
                  isMatchedCompeForwward = true;
                  j += 2;
                }
                else
                {
                  winnerBrackets.Append("[null,null],");
                  loserBrackets.Append("[null,null],");
                }
              }

              if (isMatchedCompeForwward)
              {
                isWinnerSet = true;
              }

              winnerBrackets.Append("],");
              loserBrackets.Append("],");
              winRemaining = winRemaining / 2;
            }

            ViewBag.BracketEl = "Double Elimination";
            ViewBag.Teams = teamsJS.ToString();
            ViewBag.Results = string.Concat("[", winnerBrackets.ToString(), "]", ",", "[", loserBrackets.ToString());
          }
          break;
        default:
          break;
      }

      var schoolNamesArray = new StringBuilder();
      schoolNamesArray.Append("[");
      schoolNamesArray.Append(string.Join(",", competitorNames.Select(x => x.Split(':')[1].Equals("'-----'") ? x.Split(':')[1] : string.Concat("", x.Split(':')[1], "")).ToList()));
      schoolNamesArray.Append("]");

      var comSchoolNamesArray = new StringBuilder();
      comSchoolNamesArray.Append("{");

      foreach (var item in competitorNames)
      {
        comSchoolNamesArray.Append($"{item.Split(':')[0]}:{(item.Split(':')[1].Equals("'-----'") ? item.Split(':')[1] : string.Concat("", item.Split(':')[1], ""))},");
      }
      comSchoolNamesArray.Append("}");

      ViewBag.ComSchoolArray = comSchoolNamesArray.ToString();
      ViewBag.SchoolArray = schoolNamesArray.ToString();
      ViewBag.GenderText = GenderText;
      ViewBag.BeltDevision = BeltText;
      ViewBag.GroupMessage = GroupMessage;
      ViewBag.Bracket = id;

      if (totalPlayers == 3 && competitorNames.Count() == 4)
      {
        ViewBag.AreThreePlayersInTournament = "true";
        var SecondMatchCompetitorInfo = teams.First(x => x.Any(y => y.Split(':')[0].Contains("--BYE--"))).First(x => !x.Split(':')[0].Contains("--BYE--"));
        ViewBag.SecondMatchCompetitor = SecondMatchCompetitorInfo.Split(':')[0].Replace("'", "");
        ViewBag.SecondMatchCompetitorSchool = SecondMatchCompetitorInfo.Split(':')[1].Replace("'", ""); ;
      }
      else if (totalPlayers == 2 && competitorNames.Count() == 4)
      {
        return View("TournamentViewerTwoPlayers", competitorNames.Where(y => !y.Split(':')[0].Contains("--BYE--")).Select(x => new Tuple<string, string>(x.Split(':')[0].Replace("'", ""), x.Split(':')[1].Replace("'", ""))).ToList());
      }

      return View("TournamentViewer");
    }

    private async Task<string[]> UpdateCompetitorsSortOrder(string Tournament, string Event, string Division, string[] competitors)
    {
      var existCompSortOrd = await db.CompetitorTournamentOrders.Where(x => x.Division.Equals(Division) && x.Tournament.Equals(Tournament) && x.Event.Equals(x.Event) && competitors.Contains(x.Competitor)).ToListAsync();
      if (existCompSortOrd != null && existCompSortOrd.Count > 0)
      {
        db.CompetitorTournamentOrders.RemoveRange(existCompSortOrd);
      }

      var competitorNames = competitors;
      for (int i = 0; i < competitorNames.Count(); i++)
      {
        db.CompetitorTournamentOrders.Add(new CompetitorTournamentOrder { SortOrder = i, Competitor = competitorNames[i], Division = Division, Event = Event, Tournament = Tournament });
      }
      await db.SaveChangesAsync();
      return competitorNames;
    }

    private List<RoundRobinEliminationVM> ListMatches(List<string> ListTeam, List<string> ListTeamSchools, int numTeams)
    {
      int numDays = (numTeams - 1);
      int halfSize = numTeams / 2;

      List<string> teams = new List<string>();
      List<string> teamsSchool = new List<string>();
      List<RoundRobinEliminationVM> teamsTournament = new List<RoundRobinEliminationVM>();

      teams.AddRange(ListTeam.Skip(halfSize).Take(halfSize));
      teams.AddRange(ListTeam.Skip(1).Take(halfSize - 1).ToArray().Reverse());
      teamsSchool.AddRange(ListTeamSchools.Skip(halfSize).Take(halfSize));
      teamsSchool.AddRange(ListTeamSchools.Skip(1).Take(halfSize - 1).ToArray().Reverse());

      int teamsSize = teams.Count;
      int teamsSchoolSize = teamsSchool.Count;

      for (int round = 0; round < numDays; round++)
      {

        int teamIdx = round % teamsSize;
        int teamSchoolIdx = round % teamsSchoolSize;

        int teamRound = (round + 1);

        teamsTournament.Add(new RoundRobinEliminationVM()
        {
          Round = teamRound,
          Team1 = teams[teamIdx],
          Team1School = teamsSchool[teamSchoolIdx],
          Team2 = ListTeam[0],
          Team2School = ListTeamSchools[0]
        });

        for (int idx = 1; idx < halfSize; idx++)
        {
          int firstTeam = (round + idx) % teamsSize;
          int secondTeam = (round + teamsSize - idx) % teamsSize;

          teamsTournament.Add(new RoundRobinEliminationVM()
          {
            Round = teamRound,
            Team1 = teams[firstTeam],
            Team1School = teamsSchool[firstTeam],
            Team2 = teams[secondTeam],
            Team2School = teamsSchool[secondTeam]
          });

        }
      }
      return teamsTournament;
    }


    public List<List<T>> Split<T>(IList<T> source)
    {
      return source
          .Select((x, i) => new { Index = i, Value = x })
          .GroupBy(x => x.Index / 2)
          .Select(x => x.Select(v => v.Value).ToList())
          .ToList();
    }

    // GET: EventDivisions/Details/5
    public async Task<ActionResult> Details(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EventDivision eventDivision = await db.EventDivisions.FindAsync(id);
      if (eventDivision == null)
      {
        return HttpNotFound();
      }
      return View(eventDivision);
    }

    [HttpGet]
    public ActionResult GetGroupCombinations(string Age, string Belt, string Weight, string Gender, int GroupLimit, string Event, string Combination)
    {
      var competitors = db.Competitors.ToList();
      var filteredCompetitors = new List<CompetitorViewModel>();

      if (!string.IsNullOrEmpty(Age))
      {
        var ageRange = Age.Split('-');
        int.TryParse(ageRange[0], out var fromAge);
        int.TryParse(ageRange[1], out var toAge);

        filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
        {
          Address = x.Address,
          Age = x.Age,
          Belt = x.Belt,
          City = x.City,
          Email = x.Email,
          Event = x.Event,
          Gender = x.Gender,
          Id = x.Id,
          Name = x.Name,
          Phone = x.Phone,
          School = x.School,
          Serial = x.Serial,
          State = x.State,
          Weight = x.Weight,
          Zip = x.Zip
        }));
      }

      if (!string.IsNullOrEmpty(Belt))
      {
        var beltRange = Belt.Split('-');
        int.TryParse(beltRange[0], out var frombelt);
        int.TryParse(beltRange[1], out var toBelt);

        if (filteredCompetitors.Count > 0)
        {

          filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).ToList();
        }
        else
        {
          filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
          {
            Address = x.Address,
            Age = x.Age,
            Belt = x.Belt,
            City = x.City,
            Email = x.Email,
            Event = x.Event,
            Gender = x.Gender,
            Id = x.Id,
            Name = x.Name,
            Phone = x.Phone,
            School = x.School,
            Serial = x.Serial,
            State = x.State,
            Weight = x.Weight,
            Zip = x.Zip
          }));
        }
      }

      if (!string.IsNullOrEmpty(Weight))
      {
        var weightRange = Weight.Split('-');
        int.TryParse(weightRange[0], out var fromWeight);
        int.TryParse(weightRange[1], out var toWeight);

        if (filteredCompetitors.Count > 0)
        {
          filteredCompetitors = filteredCompetitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).ToList();
        }
        else
        {
          filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
          {
            Address = x.Address,
            Age = x.Age,
            Belt = x.Belt,
            City = x.City,
            Email = x.Email,
            Event = x.Event,
            Gender = x.Gender,
            Id = x.Id,
            Name = x.Name,
            Phone = x.Phone,
            School = x.School,
            Serial = x.Serial,
            State = x.State,
            Weight = x.Weight,
            Zip = x.Zip
          }));
        }
      }

      if (!string.IsNullOrEmpty(Gender))
      {
        switch (Gender)
        {
          case "0":
          case "1":
            {
              bool gender = Gender.Equals("0") ? false : true;
              if (filteredCompetitors.Count > 0)
              {
                filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && x.Gender == gender).ToList();
              }
              else
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
            }
            break;
          case "2":
            {
              if (filteredCompetitors.Count > 0)
              {
                filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).ToList();
              }
              else
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
            }
            break;
          default:
            break;
        }
      }

      if (!string.IsNullOrWhiteSpace(Event))
      {
        filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Event) && x.Event.Contains(Event)).ToList();
      }

      var groupList = new List<string>();

      if (GroupLimit <= 1)
      {
        ViewBag.Combinations = new SelectList(new List<CombinationViewModel>() { new CombinationViewModel { Name = filteredCompetitors.Count.ToString(), Value = filteredCompetitors.Count.ToString() } }, "Value", "Name"); ;
        ViewBag.Combination = Combination;
      }
      else
      {
        GetCombinations("", 0, filteredCompetitors.Count, filteredCompetitors.Count - 1);
        var filteredComb = combinations.Where(x => GetCharacterCount(x) == GroupLimit).Select(x => new CombinationViewModel() { Name = x, Value = x }).ToList();
        if (filteredComb == null || filteredComb.Count() < 1)
        {
          ViewBag.Combinations = new SelectList(new List<CombinationViewModel>() { new CombinationViewModel { Name = filteredCompetitors.Count.ToString(), Value = filteredCompetitors.Count.ToString() } }, "Value", "Name"); ;
          ViewBag.Combination = Combination;
        }
        else
        {
          filteredComb = new List<CombinationViewModel>();
          var numberOFGroups = filteredCompetitors.Count / GroupLimit;
          if (filteredCompetitors.Count % GroupLimit == 0)
          {
            var group = new StringBuilder();
            for (int i = 0; i < numberOFGroups; i++)
            {
              if (i > 0)
              {
                group.Append($" , {GroupLimit}");
              }
              else
              {
                group.Append($"{GroupLimit}");
              }
            }
            filteredComb.Add(new CombinationViewModel() { Name = group.ToString(), Value = group.ToString() });
          }
          else if (filteredCompetitors.Count / GroupLimit == 1)
          {
            filteredComb = combinations.Where(x => GetCharacterCount(x) == 2).Select(x => new CombinationViewModel() { Name = x, Value = x }).ToList();
          }
          else
          {
            filteredComb = combinations.Where(x => GetCharacterCount(x) == numberOFGroups).Select(x => new CombinationViewModel() { Name = x, Value = x }).ToList();
          }

          var selectList = new SelectList(filteredComb, "Value", "Name");

          ViewBag.Combinations = selectList;
          ViewBag.Combination = Combination;
        }
      }

      ViewBag.TotalCombinations = filteredCompetitors.Distinct().Count();
      return PartialView("GeneratedCombinations", filteredCompetitors.Distinct());
    }

    [HttpGet]
    public ActionResult GetCustomGroupCombinations(int GroupLimit, int Id, string Combination)
    {
      var competitors = db.Competitors.ToList();
      var filteredCompetitors = new List<CompetitorViewModel>();
      var eventDivisionGroup = db.DivisionGroups.Where(x => x.EventDevisionId == Id).ToList();
      if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
      {
        foreach (var divisionGroup in eventDivisionGroup)
        {
          var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
          if (competitorInfo != null)
          {
            filteredCompetitors.Add(new CompetitorViewModel
            {
              GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
              Address = competitorInfo.Address,
              Age = competitorInfo.Age,
              Belt = competitorInfo.Belt,
              City = competitorInfo.City,
              Email = competitorInfo.Email,
              Event = competitorInfo.Event,
              Gender = competitorInfo.Gender,
              Id = competitorInfo.Id,
              Name = competitorInfo.Name,
              Phone = competitorInfo.Phone,
              School = competitorInfo.School,
              Serial = competitorInfo.Serial,
              State = competitorInfo.State,
              Weight = competitorInfo.Weight,
              Zip = competitorInfo.Zip
            });
          }
        }
      }

      var groupList = new List<string>();

      GetCombinations("", 0, filteredCompetitors.Count, filteredCompetitors.Count - 1);
      var filteredComb = combinations.Where(x => GetCharacterCount(x) == GroupLimit).Select(x => new CombinationViewModel() { Name = x, Value = x }).ToList();
      if (GroupLimit <= 1 || (filteredComb == null || filteredComb.Count() < 1))
      {
        ViewBag.Combinations = new SelectList(new List<CombinationViewModel>() { new CombinationViewModel { Name = filteredCompetitors.Count.ToString(), Value = filteredCompetitors.Count.ToString() } }, "Value", "Name"); ;
        ViewBag.Combination = Combination;
      }
      else
      {
        filteredComb = new List<CombinationViewModel>();
        var numberOFGroups = filteredCompetitors.Count / GroupLimit;
        if (filteredCompetitors.Count % GroupLimit == 0)
        {
          var group = new StringBuilder();
          for (int i = 0; i < numberOFGroups; i++)
          {
            if (i > 0)
            {
              group.Append($" , {GroupLimit}");
            }
            else
            {
              group.Append($"{GroupLimit}");
            }
          }
          filteredComb.Add(new CombinationViewModel() { Name = group.ToString(), Value = group.ToString() });
        }
        else if (filteredCompetitors.Count / GroupLimit == 1)
        {
          filteredComb = combinations.Where(x => GetCharacterCount(x) == 2).Select(x => new CombinationViewModel() { Name = x, Value = x }).ToList();
        }
        else
        {
          filteredComb = combinations.Where(x => GetCharacterCount(x) == numberOFGroups).Select(x => new CombinationViewModel() { Name = x, Value = x }).ToList();
        }

        var selectList = new SelectList(filteredComb, "Value", "Name");

        ViewBag.Combinations = selectList;
        ViewBag.Combination = Combination;
      }

      ViewBag.TotalCombinations = filteredCompetitors.Distinct().Count();
      return PartialView("GeneratedCombinations", filteredCompetitors.Distinct());
    }


    private int GetCharacterCount(string combination)
    {
      int count = 1;
      char charToCount = ',';
      foreach (char c in combination)
      {
        if (c == charToCount)
        {
          count++;
        }
      }
      return count;
    }
    private void GetCombinations(string listStart, int startSum, int n, int max)
    {
      for (int i = 1; i <= max; i++)
      {
        string list = listStart.Length > 0
            ? listStart + " , " + i.ToString()
            : i.ToString();
        int sum = startSum + i;
        if (sum == n)
        {
          combinations.Add(list);
        }
        else if (sum < n)
        {
          GetCombinations(list, sum, n, i);
        }
      }
    }

    [HttpGet]
    public ActionResult GetRecord(string Age, string Belt, string Weight, string Gender, bool Criteria, int GroupLimit, string Combination, string Event, int EventId, bool IsCustom, int IsEdit)
    {
      var competitors = db.Competitors.ToList();
      var filteredCompetitors = new List<CompetitorViewModel>();
      var eventDevisionInfo = db.EventDivisions.FirstOrDefault(x => x.Id == EventId);
      var eventDivisionGroup = db.DivisionGroups.Where(x => x.EventDevisionId == EventId).ToList();
      if (IsCustom)
      {
        if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
        {
          foreach (var divisionGroup in eventDivisionGroup)
          {
            var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
            if (competitorInfo != null)
            {
              filteredCompetitors.Add(new CompetitorViewModel
              {
                EventDivisoinGroupId = divisionGroup.Id,
                GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                Address = competitorInfo.Address,
                Age = competitorInfo.Age,
                Belt = competitorInfo.Belt,
                City = competitorInfo.City,
                Email = competitorInfo.Email,
                Event = competitorInfo.Event,
                Gender = competitorInfo.Gender,
                Id = competitorInfo.Id,
                Name = competitorInfo.Name,
                Phone = competitorInfo.Phone,
                School = competitorInfo.School,
                Serial = competitorInfo.Serial,
                State = competitorInfo.State,
                Weight = competitorInfo.Weight,
                Zip = competitorInfo.Zip,
              });
            }
          }

          if (IsEdit == 0 && Combination != null && !Combination.Equals("null") && !string.IsNullOrWhiteSpace(Combination))
          {
            int lastGroupAssigned = 1;
            int lastStudentGroupAssigned = 0;
            var groups = Combination.Split(',');
            foreach (var grp in groups)
            {
              var group = int.Parse(grp);
              for (int i = 0; i < group; i++)
              {
                filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                lastStudentGroupAssigned++;
              }
              lastGroupAssigned++;
            }
          }

        }
      }
      else
      {
        if (eventDivisionGroup == null || eventDivisionGroup.Count <= 0 || eventDevisionInfo == null || IsEdit == 0 || string.IsNullOrWhiteSpace(eventDevisionInfo.Combinations) || eventDevisionInfo.Combinations != Combination)
        {
          if (Criteria)
          {
            var fromAge = 0;
            var toAge = 0;
            var frombelt = 0;
            var toBelt = 0;
            var fromWeight = 0;
            var toWeight = 0;
            if (!string.IsNullOrEmpty(Age))
            {
              var ageRange = Age.Split('-');
              int.TryParse(ageRange[0], out fromAge);
              int.TryParse(ageRange[1], out toAge);

              filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Belt))
            {
              var beltRange = Belt.Split('-');
              int.TryParse(beltRange[0], out frombelt);
              int.TryParse(beltRange[1], out toBelt);

              if (filteredCompetitors.Count > 0)
              {
                filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).ToList();
              }
              else if (!string.IsNullOrEmpty(Age))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else
              {
                filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
            }

            if (!string.IsNullOrEmpty(Weight))
            {
              var weightRange = Weight.Split('-');
              int.TryParse(weightRange[0], out fromWeight);
              int.TryParse(weightRange[1], out toWeight);

              if (filteredCompetitors.Count > 0)
              {
                filteredCompetitors = filteredCompetitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).ToList();
              }
              else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else if (!string.IsNullOrEmpty(Age))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else if (!string.IsNullOrEmpty(Belt))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
            }

            if (!string.IsNullOrEmpty(Gender))
            {
              switch (Gender)
              {
                case "0":
                case "1":
                  {
                    bool gender = Gender.Equals("0") ? false : true;
                    if (filteredCompetitors.Count > 0)
                    {
                      filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && x.Gender == gender).ToList();
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                  }
                  break;
                case "2":
                  {
                    if (filteredCompetitors.Count > 0)
                    {
                      filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).ToList();
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false) && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false) && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                  }
                  break;
                default:
                  break;
              }
            }
          }
          else
          {

            if (!string.IsNullOrEmpty(Age))
            {
              var ageRange = Age.Split('-');
              int.TryParse(ageRange[0], out var fromAge);
              int.TryParse(ageRange[1], out var toAge);

              filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Belt))
            {
              var beltRange = Belt.Split('-');
              int.TryParse(beltRange[0], out var frombelt);
              int.TryParse(beltRange[1], out var toBelt);

              filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Weight))
            {
              var weightRange = Weight.Split('-');
              int.TryParse(weightRange[0], out var fromWeight);
              int.TryParse(weightRange[1], out var toWeight);

              filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Gender))
            {
              switch (Gender)
              {
                case "0":
                case "1":
                  {
                    bool gender = Gender.Equals("0") ? false : true;
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                  break;
                case "2":
                  {
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                  break;
                default:
                  break;
              }
            }
          }

          if (!string.IsNullOrWhiteSpace(Event))
          {
            //Hasnain
            filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Event) && x.Event.Contains(Event)).ToList();
          }

          if (Combination != null && !Combination.Equals("null") && !string.IsNullOrWhiteSpace(Combination) && filteredCompetitors.Count > 0)
          {
            int lastGroupAssigned = 1;
            int lastStudentGroupAssigned = 0;
            var groups = Combination.Split(',');
            foreach (var grp in groups)
            {
              var group = int.Parse(grp);
              for (int i = 0; i < group; i++)
              {
                filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                lastStudentGroupAssigned++;
              }
              lastGroupAssigned++;
            }
          }
        }
        else
        {
          if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
          {
            foreach (var divisionGroup in eventDivisionGroup)
            {
              var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
              if (competitorInfo != null)
              {
                filteredCompetitors.Add(new CompetitorViewModel
                {
                  EventDivisoinGroupId = divisionGroup.Id,
                  GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                  Address = competitorInfo.Address,
                  Age = competitorInfo.Age,
                  Belt = competitorInfo.Belt,
                  City = competitorInfo.City,
                  Email = competitorInfo.Email,
                  Event = competitorInfo.Event,
                  Gender = competitorInfo.Gender,
                  Id = competitorInfo.Id,
                  Name = competitorInfo.Name,
                  Phone = competitorInfo.Phone,
                  School = competitorInfo.School,
                  Serial = competitorInfo.Serial,
                  State = competitorInfo.State,
                  Weight = competitorInfo.Weight,
                  Zip = competitorInfo.Zip
                });
              }
            }

            if (IsEdit == 0 && Combination != null && !Combination.Equals("null") && !string.IsNullOrWhiteSpace(Combination))
            {
              int lastGroupAssigned = 1;
              int lastStudentGroupAssigned = 0;
              var groups = Combination.Split(',');
              foreach (var grp in groups)
              {
                var group = int.Parse(grp);
                for (int i = 0; i < group; i++)
                {
                  filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                  lastStudentGroupAssigned++;
                }
                lastGroupAssigned++;
              }
            }
          }
        }
      }

      foreach (var filteredCompetitor in filteredCompetitors)
      {
        filteredCompetitor.CompetitorEvents = new Dictionary<decimal, string>();
        var competitrorEvents = db.DivisionGroups.Where(x => x.CompetitorId == filteredCompetitor.Id).ToList();
        if (competitrorEvents != null && competitrorEvents.Count > 0)
        {
          foreach (var competitrorEvent in competitrorEvents)
          {
            var eventInfo = db.TournamentEvents.Find(competitrorEvent.TournamentEventId);
            if (eventInfo != null)
            {
              if (!filteredCompetitor.CompetitorEvents.ContainsKey(eventInfo.Id))
              {
                filteredCompetitor.CompetitorEvents.Add(eventInfo.Id, eventInfo.Name);
              }
            }
          }
        }
      }

      string userId = User.Identity.GetUserId();
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      return PartialView("GeneratedRecords", filteredCompetitors.Distinct());
    }

    public int GetDivisionCount(string Age, string Belt, string Weight, string Gender, bool Criteria, int GroupLimit, string Combination, string Event, int EventId, bool IsCustom, int IsEdit)
    {
      var competitors = db.Competitors.ToList();
      var filteredCompetitors = new List<CompetitorViewModel>();
      var eventDevisionInfo = db.EventDivisions.FirstOrDefault(x => x.Id == EventId);
      var eventDivisionGroup = db.DivisionGroups.Where(x => x.EventDevisionId == EventId).ToList();
      if (IsCustom)
      {
        if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
        {
          foreach (var divisionGroup in eventDivisionGroup)
          {
            var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
            if (competitorInfo != null)
            {
              filteredCompetitors.Add(new CompetitorViewModel
              {
                EventDivisoinGroupId = divisionGroup.Id,
                GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                Address = competitorInfo.Address,
                Age = competitorInfo.Age,
                Belt = competitorInfo.Belt,
                City = competitorInfo.City,
                Email = competitorInfo.Email,
                Event = competitorInfo.Event,
                Gender = competitorInfo.Gender,
                Id = competitorInfo.Id,
                Name = competitorInfo.Name,
                Phone = competitorInfo.Phone,
                School = competitorInfo.School,
                Serial = competitorInfo.Serial,
                State = competitorInfo.State,
                Weight = competitorInfo.Weight,
                Zip = competitorInfo.Zip,
              });
            }
          }

          if (IsEdit == 0 && Combination != null && !Combination.Equals("null") && !string.IsNullOrWhiteSpace(Combination))
          {
            int lastGroupAssigned = 1;
            int lastStudentGroupAssigned = 0;
            var groups = Combination.Split(',');
            foreach (var grp in groups)
            {
              var group = int.Parse(grp);
              for (int i = 0; i < group; i++)
              {
                filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                lastStudentGroupAssigned++;
              }
              lastGroupAssigned++;
            }
          }

        }
      }
      else
      {
        //if (eventDivisionGroup == null || eventDivisionGroup.Count <= 0 || eventDevisionInfo == null || IsEdit == 0 || string.IsNullOrWhiteSpace(eventDevisionInfo.Combinations) || eventDevisionInfo.Combinations != Combination)
        //{
        //  if (Criteria)
        //  {
        //    if (!string.IsNullOrEmpty(Age))
        //    {
        //      var ageRange = Age.Split('-');
        //      int.TryParse(ageRange[0], out var fromAge);
        //      int.TryParse(ageRange[1], out var toAge);

        //      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
        //      {
        //        Address = x.Address,
        //        Age = x.Age,
        //        Belt = x.Belt,
        //        City = x.City,
        //        Email = x.Email,
        //        Event = x.Event,
        //        Gender = x.Gender,
        //        Id = x.Id,
        //        Name = x.Name,
        //        Phone = x.Phone,
        //        School = x.School,
        //        Serial = x.Serial,
        //        State = x.State,
        //        Weight = x.Weight,
        //        Zip = x.Zip
        //      }));
        //    }

        //    if (!string.IsNullOrEmpty(Belt))
        //    {
        //      var beltRange = Belt.Split('-');
        //      int.TryParse(beltRange[0], out var frombelt);
        //      int.TryParse(beltRange[1], out var toBelt);

        //      if (filteredCompetitors.Count > 0)
        //      {

        //        filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).ToList();
        //      }
        //      else
        //      {
        //        filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
        //        {
        //          Address = x.Address,
        //          Age = x.Age,
        //          Belt = x.Belt,
        //          City = x.City,
        //          Email = x.Email,
        //          Event = x.Event,
        //          Gender = x.Gender,
        //          Id = x.Id,
        //          Name = x.Name,
        //          Phone = x.Phone,
        //          School = x.School,
        //          Serial = x.Serial,
        //          State = x.State,
        //          Weight = x.Weight,
        //          Zip = x.Zip
        //        }));
        //      }
        //    }

        //    if (!string.IsNullOrEmpty(Weight))
        //    {
        //      var weightRange = Weight.Split('-');
        //      int.TryParse(weightRange[0], out var fromWeight);
        //      int.TryParse(weightRange[1], out var toWeight);

        //      if (filteredCompetitors.Count > 0)
        //      {
        //        filteredCompetitors = filteredCompetitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).ToList();
        //      }
        //      else
        //      {
        //        filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
        //        {
        //          Address = x.Address,
        //          Age = x.Age,
        //          Belt = x.Belt,
        //          City = x.City,
        //          Email = x.Email,
        //          Event = x.Event,
        //          Gender = x.Gender,
        //          Id = x.Id,
        //          Name = x.Name,
        //          Phone = x.Phone,
        //          School = x.School,
        //          Serial = x.Serial,
        //          State = x.State,
        //          Weight = x.Weight,
        //          Zip = x.Zip
        //        }));
        //      }
        //    }

        //    if (!string.IsNullOrEmpty(Gender))
        //    {
        //      switch (Gender)
        //      {
        //        case "0":
        //        case "1":
        //          {
        //            bool gender = Gender.Equals("0") ? false : true;
        //            if (filteredCompetitors.Count > 0)
        //            {
        //              filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && x.Gender == gender).ToList();
        //            }
        //            else
        //            {
        //              filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
        //              {
        //                Address = x.Address,
        //                Age = x.Age,
        //                Belt = x.Belt,
        //                City = x.City,
        //                Email = x.Email,
        //                Event = x.Event,
        //                Gender = x.Gender,
        //                Id = x.Id,
        //                Name = x.Name,
        //                Phone = x.Phone,
        //                School = x.School,
        //                Serial = x.Serial,
        //                State = x.State,
        //                Weight = x.Weight,
        //                Zip = x.Zip
        //              }));
        //            }
        //          }
        //          break;
        //        case "2":
        //          {
        //            if (filteredCompetitors.Count > 0)
        //            {
        //              filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).ToList();
        //            }
        //            else
        //            {
        //              filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
        //              {
        //                Address = x.Address,
        //                Age = x.Age,
        //                Belt = x.Belt,
        //                City = x.City,
        //                Email = x.Email,
        //                Event = x.Event,
        //                Gender = x.Gender,
        //                Id = x.Id,
        //                Name = x.Name,
        //                Phone = x.Phone,
        //                School = x.School,
        //                Serial = x.Serial,
        //                State = x.State,
        //                Weight = x.Weight,
        //                Zip = x.Zip
        //              }));
        //            }
        //          }
        //          break;
        //        default:
        //          break;
        //      }
        //    }
        //  }
        //  else
        //  {

        //    if (!string.IsNullOrEmpty(Age))
        //    {
        //      var ageRange = Age.Split('-');
        //      int.TryParse(ageRange[0], out var fromAge);
        //      int.TryParse(ageRange[1], out var toAge);

        //      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
        //      {
        //        Address = x.Address,
        //        Age = x.Age,
        //        Belt = x.Belt,
        //        City = x.City,
        //        Email = x.Email,
        //        Event = x.Event,
        //        Gender = x.Gender,
        //        Id = x.Id,
        //        Name = x.Name,
        //        Phone = x.Phone,
        //        School = x.School,
        //        Serial = x.Serial,
        //        State = x.State,
        //        Weight = x.Weight,
        //        Zip = x.Zip
        //      }));
        //    }

        //    if (!string.IsNullOrEmpty(Belt))
        //    {
        //      var beltRange = Belt.Split('-');
        //      int.TryParse(beltRange[0], out var frombelt);
        //      int.TryParse(beltRange[1], out var toBelt);

        //      filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
        //      {
        //        Address = x.Address,
        //        Age = x.Age,
        //        Belt = x.Belt,
        //        City = x.City,
        //        Email = x.Email,
        //        Event = x.Event,
        //        Gender = x.Gender,
        //        Id = x.Id,
        //        Name = x.Name,
        //        Phone = x.Phone,
        //        School = x.School,
        //        Serial = x.Serial,
        //        State = x.State,
        //        Weight = x.Weight,
        //        Zip = x.Zip
        //      }));
        //    }

        //    if (!string.IsNullOrEmpty(Weight))
        //    {
        //      var weightRange = Weight.Split('-');
        //      int.TryParse(weightRange[0], out var fromWeight);
        //      int.TryParse(weightRange[1], out var toWeight);

        //      filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
        //      {
        //        Address = x.Address,
        //        Age = x.Age,
        //        Belt = x.Belt,
        //        City = x.City,
        //        Email = x.Email,
        //        Event = x.Event,
        //        Gender = x.Gender,
        //        Id = x.Id,
        //        Name = x.Name,
        //        Phone = x.Phone,
        //        School = x.School,
        //        Serial = x.Serial,
        //        State = x.State,
        //        Weight = x.Weight,
        //        Zip = x.Zip
        //      }));
        //    }

        //    if (!string.IsNullOrEmpty(Gender))
        //    {
        //      switch (Gender)
        //      {
        //        case "0":
        //        case "1":
        //          {
        //            bool gender = Gender.Equals("0") ? false : true;
        //            filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
        //            {
        //              Address = x.Address,
        //              Age = x.Age,
        //              Belt = x.Belt,
        //              City = x.City,
        //              Email = x.Email,
        //              Event = x.Event,
        //              Gender = x.Gender,
        //              Id = x.Id,
        //              Name = x.Name,
        //              Phone = x.Phone,
        //              School = x.School,
        //              Serial = x.Serial,
        //              State = x.State,
        //              Weight = x.Weight,
        //              Zip = x.Zip
        //            }));
        //          }
        //          break;
        //        case "2":
        //          {
        //            filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
        //            {
        //              Address = x.Address,
        //              Age = x.Age,
        //              Belt = x.Belt,
        //              City = x.City,
        //              Email = x.Email,
        //              Event = x.Event,
        //              Gender = x.Gender,
        //              Id = x.Id,
        //              Name = x.Name,
        //              Phone = x.Phone,
        //              School = x.School,
        //              Serial = x.Serial,
        //              State = x.State,
        //              Weight = x.Weight,
        //              Zip = x.Zip
        //            }));
        //          }
        //          break;
        //        default:
        //          break;
        //      }
        //    }
        //  }

        //  if (!string.IsNullOrWhiteSpace(Event))
        //  {
        //    filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Event) && x.Event.Contains(Event)).ToList();
        //  }

        //}
        if (eventDivisionGroup == null || eventDivisionGroup.Count <= 0 || eventDevisionInfo == null || IsEdit == 0 || string.IsNullOrWhiteSpace(eventDevisionInfo.Combinations) || eventDevisionInfo.Combinations != Combination)
        {
          if (Criteria)
          {
            var fromAge = 0;
            var toAge = 0;
            var frombelt = 0;
            var toBelt = 0;
            var fromWeight = 0;
            var toWeight = 0;
            if (!string.IsNullOrEmpty(Age))
            {
              var ageRange = Age.Split('-');
              int.TryParse(ageRange[0], out fromAge);
              int.TryParse(ageRange[1], out toAge);

              filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Belt))
            {
              var beltRange = Belt.Split('-');
              int.TryParse(beltRange[0], out frombelt);
              int.TryParse(beltRange[1], out toBelt);

              if (filteredCompetitors.Count > 0)
              {
                filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).ToList();
              }
              else if (!string.IsNullOrEmpty(Age))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else
              {
                filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
            }

            if (!string.IsNullOrEmpty(Weight))
            {
              var weightRange = Weight.Split('-');
              int.TryParse(weightRange[0], out fromWeight);
              int.TryParse(weightRange[1], out toWeight);

              if (filteredCompetitors.Count > 0)
              {
                filteredCompetitors = filteredCompetitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).ToList();
              }
              else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else if (!string.IsNullOrEmpty(Age))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else if (!string.IsNullOrEmpty(Belt))
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
              else
              {
                filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                {
                  Address = x.Address,
                  Age = x.Age,
                  Belt = x.Belt,
                  City = x.City,
                  Email = x.Email,
                  Event = x.Event,
                  Gender = x.Gender,
                  Id = x.Id,
                  Name = x.Name,
                  Phone = x.Phone,
                  School = x.School,
                  Serial = x.Serial,
                  State = x.State,
                  Weight = x.Weight,
                  Zip = x.Zip
                }));
              }
            }

            if (!string.IsNullOrEmpty(Gender))
            {
              switch (Gender)
              {
                case "0":
                case "1":
                  {
                    bool gender = Gender.Equals("0") ? false : true;
                    if (filteredCompetitors.Count > 0)
                    {
                      filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && x.Gender == gender).ToList();
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                  }
                  break;
                case "2":
                  {
                    if (filteredCompetitors.Count > 0)
                    {
                      filteredCompetitors = filteredCompetitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).ToList();
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt) && !string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Age))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge && x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Belt))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false) && !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else if (!string.IsNullOrEmpty(Weight))
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false) && x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                    else
                    {
                      filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                      {
                        Address = x.Address,
                        Age = x.Age,
                        Belt = x.Belt,
                        City = x.City,
                        Email = x.Email,
                        Event = x.Event,
                        Gender = x.Gender,
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        School = x.School,
                        Serial = x.Serial,
                        State = x.State,
                        Weight = x.Weight,
                        Zip = x.Zip
                      }));
                    }
                  }
                  break;
                default:
                  break;
              }
            }
          }
          else
          {

            if (!string.IsNullOrEmpty(Age))
            {
              var ageRange = Age.Split('-');
              int.TryParse(ageRange[0], out var fromAge);
              int.TryParse(ageRange[1], out var toAge);

              filteredCompetitors.AddRange(competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Belt))
            {
              var beltRange = Belt.Split('-');
              int.TryParse(beltRange[0], out var frombelt);
              int.TryParse(beltRange[1], out var toBelt);

              filteredCompetitors.AddRange(competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Weight))
            {
              var weightRange = Weight.Split('-');
              int.TryParse(weightRange[0], out var fromWeight);
              int.TryParse(weightRange[1], out var toWeight);

              filteredCompetitors.AddRange(competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
              {
                Address = x.Address,
                Age = x.Age,
                Belt = x.Belt,
                City = x.City,
                Email = x.Email,
                Event = x.Event,
                Gender = x.Gender,
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                School = x.School,
                Serial = x.Serial,
                State = x.State,
                Weight = x.Weight,
                Zip = x.Zip
              }));
            }

            if (!string.IsNullOrEmpty(Gender))
            {
              switch (Gender)
              {
                case "0":
                case "1":
                  {
                    bool gender = Gender.Equals("0") ? false : true;
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                  break;
                case "2":
                  {
                    filteredCompetitors.AddRange(competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
                    {
                      Address = x.Address,
                      Age = x.Age,
                      Belt = x.Belt,
                      City = x.City,
                      Email = x.Email,
                      Event = x.Event,
                      Gender = x.Gender,
                      Id = x.Id,
                      Name = x.Name,
                      Phone = x.Phone,
                      School = x.School,
                      Serial = x.Serial,
                      State = x.State,
                      Weight = x.Weight,
                      Zip = x.Zip
                    }));
                  }
                  break;
                default:
                  break;
              }
            }
          }

          if (!string.IsNullOrWhiteSpace(Event))
          {
            //Hasnain
            filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Event) && x.Event.Contains(Event)).ToList();
          }

          if (Combination != null && !Combination.Equals("null") && !string.IsNullOrWhiteSpace(Combination) && filteredCompetitors.Count > 0)
          {
            int lastGroupAssigned = 1;
            int lastStudentGroupAssigned = 0;
            var groups = Combination.Split(',');
            foreach (var grp in groups)
            {
              var group = int.Parse(grp);
              for (int i = 0; i < group; i++)
              {
                if(filteredCompetitors.Count > i) { 
                  filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                  lastStudentGroupAssigned++;
                }
              }
              lastGroupAssigned++;
            }
          }
        }

        else
        {
          if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
          {
            foreach (var divisionGroup in eventDivisionGroup)
            {
              var competitorInfo = competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
              if (competitorInfo != null)
              {
                filteredCompetitors.Add(new CompetitorViewModel
                {
                  EventDivisoinGroupId = divisionGroup.Id,
                  GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
                  Address = competitorInfo.Address,
                  Age = competitorInfo.Age,
                  Belt = competitorInfo.Belt,
                  City = competitorInfo.City,
                  Email = competitorInfo.Email,
                  Event = competitorInfo.Event,
                  Gender = competitorInfo.Gender,
                  Id = competitorInfo.Id,
                  Name = competitorInfo.Name,
                  Phone = competitorInfo.Phone,
                  School = competitorInfo.School,
                  Serial = competitorInfo.Serial,
                  State = competitorInfo.State,
                  Weight = competitorInfo.Weight,
                  Zip = competitorInfo.Zip
                });
              }
            }

            if (IsEdit == 0 && Combination != null && !Combination.Equals("null") && !string.IsNullOrWhiteSpace(Combination) && filteredCompetitors.Count > 0)
            {
              int lastGroupAssigned = 1;
              int lastStudentGroupAssigned = 0;
              var groups = Combination.Split(',');
              foreach (var grp in groups)
              {
                var group = int.Parse(grp);
                for (int i = 0; i < group; i++)
                {
                  if (filteredCompetitors.Count > i)
                  {
                    filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                    lastStudentGroupAssigned++;
                  }
                }
                lastGroupAssigned++;
              }
            }
          }
        }
      }

      return filteredCompetitors.Distinct().Count();
    }


    // GET: EventDivisions/Create
    public ActionResult Create()
    {
      var currentUserId = User.Identity.GetUserId();
      ////var filteredTournaments = db.Tournaments.Where(x => x.CreatedBy == currentUserId).ToList();
      ////ViewBag.TournamentId = new SelectList(filteredTournaments, "Id", "Name");
      if(User.IsInRole("Admin")) ViewBag.TournamentEventId = new SelectList(db.TournamentEvents, "Id", "Name");
      else ViewBag.TournamentEventId = new SelectList(db.TournamentEvents.Where(x=> x.CreatedBy==currentUserId), "Id", "Name");
      var competitors = db.Competitors.ToList();
      //var ages = competitors.Select(x => x.Age).Distinct().OrderBy(x => x);
      //var weigts = competitors.Select(x => x.Weight).Distinct().OrderBy(x => x);
      var ages = Enumerable.Range(1, 100);
      var weigts = Enumerable.Range(1, 1000);

      //var belts = new List<BeltViewModel>();
      //foreach (var competitor in competitors)
      //{
      //  if (competitor.Belt != null && !belts.Any(x => x.Id == int.Parse(competitor.Belt)))
      //  {
      //    belts.Add(new BeltViewModel { Id = int.Parse(competitor.Belt), Name = beltsCollection.FirstOrDefault(y => y.Id == int.Parse(competitor.Belt))?.Name });
      //  }
      //}

      ViewBag.Ages = ages;
      ViewBag.Weights = weigts;
      ViewBag.Belts = beltsCollection;//belts;
      ViewBag.Genders = gendersCollection;
      ViewBag.Criterias = criteriaCollection;
      ViewBag.Combinations = new SelectList(new List<CombinationViewModel>());
      ViewBag.Combination = string.Empty;
      ViewBag.Criteria = "true";
      ViewBag.IsCustom = false;
      ViewBag.IsGenderFilter = "1";
      return View();
    }

    // POST: EventDivisions/Create  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,AgeRange,BeltRange,GenderRange,WeightRange,DateCreated,DateModified,TournamentId,TournamentEventId,Name,Description,StrictCriteria,GroupLimit,Combinations,RingAssignment,IsCustom")] EventDivision eventDivision, string GroupIds, string CompetitorIds)
    {
      beltsCollection = new List<BeltViewModel>();
      string curUserId = User.Identity.GetUserId();
      var Dbbelts = db.Belts.Where(x => x.CreatedBy == curUserId).ToList();
      foreach (var item in Dbbelts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }

      var lastEvId = db.EventDivisions.OrderByDescending(x => x.Id).FirstOrDefault()?.Id;
      eventDivision.Id = ((lastEvId.HasValue ? lastEvId.Value : 0) + 1);
      //bool isError = false;
      if (eventDivision != null && db.EventDivisions.Any(x => x.Name.ToUpper() == eventDivision.Name.ToUpper()))
      {
        ModelState.AddModelError("Name", $"Division with name {eventDivision.Name} already exists");
        //isError = true;
      }
      if (ModelState.IsValid)
      {
        eventDivision.GenderRange = string.Empty;
        eventDivision.CreatedBy = User.Identity.GetUserId();
        db.EventDivisions.Add(eventDivision);
        await db.SaveChangesAsync();

        if (!string.IsNullOrEmpty(CompetitorIds))
        {
          var groupCompetitors = CompetitorIds.Split(',');
          var groupNumbers = GroupIds.Split(',');
          for (int i = 0; i < groupCompetitors.Count(); i++)
          {
            db.DivisionGroups.Add(new DivisionGroup()
            {
              CompetitorId = int.Parse(groupCompetitors[i]),
              EventDevisionId = eventDivision.Id,
              GroupNo = int.Parse(groupNumbers[i])
            });
          }
          await db.SaveChangesAsync();
        }

        //return RedirectToAction("Index");
        eventDivision = db.EventDivisions.FirstOrDefault(x => x.Name == eventDivision.Name);
        return RedirectToAction("Edit", new { Id = eventDivision.Id });
      }
      //ViewBag.IsError = isError;


      ////////------------------------------------------////////
      var competitors = db.Competitors.ToList();
      var ages = competitors.Select(x => x.Age).Distinct().OrderBy(x => x);
      var weigts = competitors.Select(x => x.Weight).Distinct().OrderBy(x => x);

      var belts = new List<BeltViewModel>();
      foreach (var competitor in competitors)
      {
        if (competitor.Belt != null && !belts.Any(x => x.Id == int.Parse(competitor.Belt)))
        {
          belts.Add(new BeltViewModel { Id = int.Parse(competitor.Belt), Name = beltsCollection.FirstOrDefault(y => y.Id == int.Parse(competitor.Belt))?.Name });
        }
      }

      if (!string.IsNullOrEmpty(eventDivision.AgeRange))
      {
        var ageRange = eventDivision.AgeRange.Split('-');
        int.TryParse(ageRange[0], out var fromAge);
        int.TryParse(ageRange[1], out var toAge);

        ViewBag.FromAge = fromAge;
        ViewBag.ToAge = toAge;
        ViewBag.Ages = ages;
        ViewBag.IsAgeFilter = "1";
      }
      else
      {
        ViewBag.Ages = ages;
        ViewBag.IsAgeFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.BeltRange))
      {
        var beltRange = eventDivision.BeltRange.Split('-');
        int.TryParse(beltRange[0], out var frombelt);
        int.TryParse(beltRange[1], out var toBelt);

        ViewBag.FromBelt = frombelt;
        ViewBag.ToBelt = toBelt;
        ViewBag.Belts = belts;
        ViewBag.IsBeltFilter = "1";
      }
      else
      {
        ViewBag.Belts = belts;
        ViewBag.IsBeltFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.WeightRange))
      {
        var weightRange = eventDivision.WeightRange.Split('-');
        int.TryParse(weightRange[0], out var fromWeight);
        int.TryParse(weightRange[1], out var toWeight);

        ViewBag.FromWeight = fromWeight;
        ViewBag.ToWeight = toWeight;
        ViewBag.Weights = weigts;
        ViewBag.IsWeightFilter = "1";
      }
      else
      {
        ViewBag.Weights = weigts;
        ViewBag.IsWeightFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.GenderRange))
      {
        ViewBag.Gender = eventDivision.GenderRange;
        ViewBag.Genders = gendersCollection;
        ViewBag.IsGenderFilter = "1";
      }
      else
      {
        ViewBag.Genders = gendersCollection;
        ViewBag.IsGenderFilter = "0";
      }

      if (eventDivision.StrictCriteria == null)
      {
        ViewBag.Criterias = criteriaCollection;
        ViewBag.Criteria = "true";
      }
      else
      {
        ViewBag.Criterias = criteriaCollection;
        ViewBag.Criteria = eventDivision.StrictCriteria.ToString().ToLower();
      }

      ////ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", eventDivision.TournamentId);
      ViewBag.TournamentEventId = new SelectList(db.TournamentEvents, "Id", "Name", eventDivision.TournamentEventId);
      ViewBag.Combinations = new SelectList(new List<CombinationViewModel>());
      ViewBag.Combination = eventDivision.Combinations;
      ViewBag.IsCustom = eventDivision.IsCustom;
      ViewBag.IsEdit = "1";
      return View(eventDivision);
    }

    public async Task<ActionResult> UpdateGroupAndGetRecord(string CompetitorIds, string GroupIds, int EventId)
    {
      var divisonGroups = await db.DivisionGroups.Where(x => x.EventDevisionId == EventId).ToListAsync();
      if (divisonGroups != null && divisonGroups.Count > 0)
      {
        if (!string.IsNullOrEmpty(CompetitorIds))
        {
          foreach (var divisionGroup in divisonGroups)
          {
            db.DivisionGroups.Remove(divisionGroup);
          }
          await db.SaveChangesAsync();

          var groupCompetitors = CompetitorIds.Split(',');
          var groupNumbers = GroupIds.Split(',');
          for (int i = 0; i < groupCompetitors.Count(); i++)
          {
            db.DivisionGroups.Add(new DivisionGroup()
            {
              CompetitorId = int.Parse(groupCompetitors[i]),
              EventDevisionId = EventId,
              GroupNo = int.Parse(groupNumbers[i])
            });
          }

        }
      }
      await db.SaveChangesAsync();

      var filteredCompetitors = new List<CompetitorViewModel>();
      var eventDivisionGroup = await db.DivisionGroups.Where(x => x.EventDevisionId == EventId).ToListAsync();
      foreach (var divisionGroup in eventDivisionGroup)
      {
        var competitorInfo = db.Competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
        if (competitorInfo != null)
        {
          filteredCompetitors.Add(new CompetitorViewModel
          {
            EventDivisoinGroupId = divisionGroup.Id,
            GroupId = divisionGroup.GroupNo.GetValueOrDefault(),
            Address = competitorInfo.Address,
            Age = competitorInfo.Age,
            Belt = competitorInfo.Belt,
            City = competitorInfo.City,
            Email = competitorInfo.Email,
            Event = competitorInfo.Event,
            Gender = competitorInfo.Gender,
            Id = competitorInfo.Id,
            Name = competitorInfo.Name,
            Phone = competitorInfo.Phone,
            School = competitorInfo.School,
            Serial = competitorInfo.Serial,
            State = competitorInfo.State,
            Weight = competitorInfo.Weight,
            Zip = competitorInfo.Zip
          });
        }
      }

      foreach (var filteredCompetitor in filteredCompetitors)
      {
        filteredCompetitor.CompetitorEvents = new Dictionary<decimal, string>();
        var competitrorEvents = db.DivisionGroups.Where(x => x.CompetitorId == filteredCompetitor.Id).ToList();
        if (competitrorEvents != null && competitrorEvents.Count > 0)
        {
          foreach (var competitrorEvent in competitrorEvents)
          {
            var eventInfo = db.TournamentEvents.Find(competitrorEvent.TournamentEventId);
            if (eventInfo != null)
            {
              if (!filteredCompetitor.CompetitorEvents.ContainsKey(eventInfo.Id))
              {
                filteredCompetitor.CompetitorEvents.Add(eventInfo.Id, eventInfo.Name);
              }
            }
          }
        }
      }

      string userId = User.Identity.GetUserId();
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      return PartialView("GeneratedRecords", filteredCompetitors.Distinct());
    }


    // GET: EventDivisions/Edit/5
    public async Task<ActionResult> Edit(decimal id)
    {
      beltsCollection = new List<BeltViewModel>();
      string curUserId = User.Identity.GetUserId();
      var Dbbelts = db.Belts.Where(x => x.CreatedBy == curUserId).ToList();
      foreach (var item in Dbbelts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }

      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EventDivision eventDivision = await db.EventDivisions.FindAsync(id);
      if (eventDivision == null)
      {
        return HttpNotFound();
      }

      var competitors = db.Competitors.ToList();
      //var ages = competitors.Select(x => x.Age).Distinct().OrderBy(x => x);
      //var weigts = competitors.Select(x => x.Weight).Distinct().OrderBy(x => x);
      var ages = Enumerable.Range(1, 100);
      var weigts = Enumerable.Range(1, 1000);

      //var belts = new List<BeltViewModel>();
      //foreach (var competitor in competitors)
      //{
      //  if (competitor.Belt != null && !belts.Any(x => x.Id == int.Parse(competitor.Belt)))
      //  {
      //    belts.Add(new BeltViewModel { Id = int.Parse(competitor.Belt), Name = beltsCollection.FirstOrDefault(y => y.Id == int.Parse(competitor.Belt))?.Name });
      //  }
      //}

      if (!string.IsNullOrEmpty(eventDivision.AgeRange))
      {
        var ageRange = eventDivision.AgeRange.Split('-');
        int.TryParse(ageRange[0], out var fromAge);
        int.TryParse(ageRange[1], out var toAge);

        ViewBag.FromAge = fromAge;
        ViewBag.ToAge = toAge;
        ViewBag.Ages = ages;
        ViewBag.IsAgeFilter = "1";
      }
      else
      {
        ViewBag.Ages = ages;
        ViewBag.IsAgeFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.BeltRange))
      {
        var beltRange = eventDivision.BeltRange.Split('-');
        int.TryParse(beltRange[0], out var frombelt);
        int.TryParse(beltRange[1], out var toBelt);

        ViewBag.FromBelt = frombelt;
        ViewBag.ToBelt = toBelt;
        ViewBag.Belts = beltsCollection;//belts;
        ViewBag.IsBeltFilter = "1";
      }
      else
      {
        ViewBag.Belts = beltsCollection;//belts;
        ViewBag.IsBeltFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.WeightRange))
      {
        var weightRange = eventDivision.WeightRange.Split('-');
        int.TryParse(weightRange[0], out var fromWeight);
        int.TryParse(weightRange[1], out var toWeight);

        ViewBag.FromWeight = fromWeight;
        ViewBag.ToWeight = toWeight;
        ViewBag.Weights = weigts;
        ViewBag.IsWeightFilter = "1";
      }
      else
      {
        ViewBag.Weights = weigts;
        ViewBag.IsWeightFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.GenderRange))
      {
        ViewBag.Gender = eventDivision.GenderRange;
        ViewBag.Genders = gendersCollection;
        ViewBag.IsGenderFilter = "1";
      }
      else
      {
        ViewBag.Genders = gendersCollection;
        ViewBag.IsGenderFilter = "0";
      }

      if (eventDivision.StrictCriteria == null)
      {
        ViewBag.Criterias = criteriaCollection;
        ViewBag.Criteria = "true";
      }
      else
      {
        ViewBag.Criterias = criteriaCollection;
        ViewBag.Criteria = eventDivision.StrictCriteria.ToString().ToLower();
      }

      var currentUserId = User.Identity.GetUserId();
      ////var filteredTournaments = db.Tournaments.Where(x => x.CreatedBy == currentUserId).ToList();
      ////ViewBag.TournamentId = new SelectList(filteredTournaments, "Id", "Name", eventDivision.TournamentId);
      if(User.IsInRole("Admin")) ViewBag.TournamentEventId = new SelectList(db.TournamentEvents, "Id", "Name", eventDivision.TournamentEventId);
      else ViewBag.TournamentEventId = new SelectList(db.TournamentEvents.Where(x=> x.CreatedBy==currentUserId), "Id", "Name", eventDivision.TournamentEventId);
      ViewBag.Combinations = new SelectList(new List<string>());
      ViewBag.Combination = string.Empty;
      ViewBag.Combinations = new SelectList(new List<CombinationViewModel>());
      ViewBag.Combination = eventDivision.Combinations;
      ViewBag.IsEdit = "1";
      return View(eventDivision);
    }

    // POST: EventDivisions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,AgeRange,BeltRange,GenderRange,WeightRange,DateCreated,DateModified,TournamentId,TournamentEventId,Name,Description,StrictCriteria,GroupLimit,Combinations,RingAssignment,IsCustom,CreatedBy")] EventDivision eventDivision, string GroupIds, string CompetitorIds)
    {
      beltsCollection = new List<BeltViewModel>();
      string curUserId = User.Identity.GetUserId();
      var Dbbelts = db.Belts.Where(x => x.CreatedBy == curUserId).ToList();
      foreach (var item in Dbbelts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }

      if (ModelState.IsValid)
      {
        var oldDivisionGroups = await db.DivisionGroups.Where(x => x.EventDevisionId == eventDivision.Id).ToListAsync();
        if (oldDivisionGroups != null && oldDivisionGroups.Count > 0)
        {
          foreach (var divisionGroup in oldDivisionGroups)
          {
            db.DivisionGroups.Remove(divisionGroup);
          }
          await db.SaveChangesAsync();
        }

        if (!string.IsNullOrEmpty(CompetitorIds))
        {
          var groupCompetitors = CompetitorIds.Split(',');
          var groupNumbers = GroupIds.Split(',');
          for (int i = 0; i < groupCompetitors.Count(); i++)
          {
            db.DivisionGroups.Add(new DivisionGroup()
            {
              CompetitorId = int.Parse(groupCompetitors[i]),
              EventDevisionId = eventDivision.Id,
              GroupNo = int.Parse(groupNumbers[i]),
              TournamentEventId = eventDivision.TournamentEventId
            });
          }
        }

        eventDivision.CreatedBy = db.EventDivisions.AsNoTracking().FirstOrDefault(x => x.Id == eventDivision.Id)?.CreatedBy;
        db.Entry(eventDivision).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      var competitors = db.Competitors.ToList();
      var ages = competitors.Select(x => x.Age).Distinct().OrderBy(x => x);
      var weigts = competitors.Select(x => x.Weight).Distinct().OrderBy(x => x);

      var belts = new List<BeltViewModel>();
      foreach (var competitor in competitors)
      {
        if (competitor.Belt != null && !belts.Any(x => x.Id == int.Parse(competitor.Belt)))
        {
          belts.Add(new BeltViewModel { Id = int.Parse(competitor.Belt), Name = beltsCollection.FirstOrDefault(y => y.Id == int.Parse(competitor.Belt))?.Name });
        }
      }

      if (!string.IsNullOrEmpty(eventDivision.AgeRange))
      {
        var ageRange = eventDivision.AgeRange.Split('-');
        int.TryParse(ageRange[0], out var fromAge);
        int.TryParse(ageRange[1], out var toAge);

        ViewBag.FromAge = fromAge;
        ViewBag.ToAge = toAge;
        ViewBag.Ages = ages;
        ViewBag.IsAgeFilter = "1";
      }
      else
      {
        ViewBag.Ages = ages;
        ViewBag.IsAgeFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.BeltRange))
      {
        var beltRange = eventDivision.BeltRange.Split('-');
        int.TryParse(beltRange[0], out var frombelt);
        int.TryParse(beltRange[1], out var toBelt);

        ViewBag.FromBelt = frombelt;
        ViewBag.ToBelt = toBelt;
        ViewBag.Belts = belts;
        ViewBag.IsBeltFilter = "1";
      }
      else
      {
        ViewBag.Belts = belts;
        ViewBag.IsBeltFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.WeightRange))
      {
        var weightRange = eventDivision.WeightRange.Split('-');
        int.TryParse(weightRange[0], out var fromWeight);
        int.TryParse(weightRange[1], out var toWeight);

        ViewBag.FromWeight = fromWeight;
        ViewBag.ToWeight = toWeight;
        ViewBag.Weights = weigts;
        ViewBag.IsWeightFilter = "1";
      }
      else
      {
        ViewBag.Weights = weigts;
        ViewBag.IsWeightFilter = "0";
      }

      if (!string.IsNullOrEmpty(eventDivision.GenderRange))
      {
        ViewBag.Gender = eventDivision.GenderRange;
        ViewBag.Genders = gendersCollection;
        ViewBag.IsGenderFilter = "1";
      }
      else
      {
        ViewBag.Genders = gendersCollection;
        ViewBag.IsGenderFilter = "0";
      }

      if (eventDivision.StrictCriteria == null)
      {
        ViewBag.Criterias = criteriaCollection;
        ViewBag.Criteria = "true";
      }
      else
      {
        ViewBag.Criterias = criteriaCollection;
        ViewBag.Criteria = eventDivision.StrictCriteria.ToString().ToLower();
      }

      ////ViewBag.TournamentId = new SelectList(db.Tournaments, "Id", "Name", eventDivision.TournamentId);
      ViewBag.TournamentEventId = new SelectList(db.TournamentEvents, "Id", "Name", eventDivision.TournamentEventId);
      ViewBag.Combinations = new SelectList(new List<CombinationViewModel>());
      ViewBag.Combination = eventDivision.Combinations;

      ViewBag.IsEdit = "1";
      return View(eventDivision);
    }

    // GET: EventDivisions/Delete/5
    public async Task<ActionResult> Delete(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      EventDivision eventDivision = await db.EventDivisions.FindAsync(id);
      if (eventDivision == null)
      {
        return HttpNotFound();
      }
      return View(eventDivision);
    }

    // POST: EventDivisions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      var attachedGroups = db.DivisionGroups.Where(x => x.EventDevisionId == id).ToList();
      for (int i = 0; i < attachedGroups.Count; i++)
      {
        attachedGroups[i].EventDevisionId = null;
        db.Entry(attachedGroups[i]).State = EntityState.Modified;
      }
      
      EventDivision eventDivision = await db.EventDivisions.FindAsync(id);
      db.EventDivisions.Remove(eventDivision);
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
