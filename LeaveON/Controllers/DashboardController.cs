using TourneyRepo.Models;
using LeaveON.UtilityClasses;
using Microsoft.AspNet.Identity;
//using InventoryRepo.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeaveON.Models;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User,TournamentDirector")]

  public class DashboardController : Controller
  {
    // GET: Dashboard
    //private InventoryPortalEntities db = new InventoryPortalEntities();
    private TourneyWizardEntities db = new TourneyWizardEntities();
    //private List<BeltViewModel> beltsCollection = new List<BeltViewModel>() { new BeltViewModel { Id = 1, Name = "white" }, new BeltViewModel { Id = 2, Name = "Yellow" }, new BeltViewModel { Id = 3, Name = "Orange" }, new BeltViewModel { Id = 4, Name = "Green" }, new BeltViewModel { Id = 5, Name = "Blue" }, new BeltViewModel { Id = 6, Name = "Brown" }, new BeltViewModel { Id = 7, Name = "Red" }, new BeltViewModel { Id = 8, Name = "Black" } };
    private List<BeltViewModel> beltsCollection = new List<BeltViewModel>();
    private List<GenderViewModel> gendersCollection = new List<GenderViewModel>() { new GenderViewModel { Id = 1, Name = "Male" }, new GenderViewModel { Id = 0, Name = "Female" }, new GenderViewModel { Id = 2, Name = "Male & Female" } };

    Dashboard dashboard = new Dashboard();


    public ActionResult Index1()
    {
      FillData();

      return View(dashboard);

    }
    public ActionResult Index()
    {
      FillData();
      return View(dashboard);

    }


    public void FillData()
    {
      string userId = User.Identity.GetUserId();
      var belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      foreach (var item in belts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }

      // db.Leaves.Sum(x=>x..LeaveTypeId!=0)


      //int policyId = db.AspNetUsers.FirstOrDefault(x => x.Id == userId).UserLeavePolicyId.GetValueOrDefault();
      ////var data=db.Tournaments.ToList();
      if (User.IsInRole("Admin")) dashboard.totalCompetitors = db.Competitors.Count();
      else
      {
        var userTournamentsIds = db.ScheduledEvents.Where(x => x.CreatedBy == userId).Select(y => y.Id).ToList();
        var competitorExistsTournaments = db.CompetitorEventRelations.Where(x => userTournamentsIds.Contains(x.TournamentId.Value)).Select(y => y.CompetitorId).ToList();
        dashboard.totalCompetitors = db.Competitors.Where(x => x.CreatedBy == userId || competitorExistsTournaments.Contains(x.Id)).Count();
      }
      ////if (User.IsInRole("Admin")) dashboard.totalTournaments = db.Tournaments.Count();
      ////else dashboard.totalTournaments = db.Tournaments.Where(x => x.CreatedBy == userId).Count();
      if (User.IsInRole("Admin")) dashboard.totalEvents = db.TournamentEvents.Count();
      else dashboard.totalEvents = db.TournamentEvents.Where(x => x.CreatedBy == userId).Count();
      if (User.IsInRole("Admin")) dashboard.totalDivisions = db.EventDivisions.Count();
      else dashboard.totalDivisions = db.EventDivisions.Where(x => x.CreatedBy == userId).Count();

      ////dashboard.totalCompetitors = db.Competitors.Count();
      ////dashboard.totalTournaments = db.Tournaments.Count();
      ////dashboard.totalEvents = db.ScheduledEvents.Count();
      ////dashboard.totalDivisions = db.EventDivisions.Count();
      ViewBag.isRunVideo = TempData["isRunVideo"];

      //List<Competitor> missingCompetitors = db.Competitors.ToList();
      //Competitor competitor;
      //foreach (DivisionGroup division in db.DivisionGroups.ToList())
      //{
      //  if (division.GroupNo>1)
      //  {
      //    //competitor=missingCompetitors.Where(x=>x.)
      //    //missingCompetitors.(division.CompetitorId)
      //  }
      //}
      //List<Accounting> Items = await _context.Accounting.Where(q => q.Accounting.Where(r => r.ParentAccountId == q.AccountId).count() == 0).ToListAsync();

      //List<Competitor> missingCompetitors = db.Competitors.Where(x => x.DivisionGroups.Where(y => y.GroupNo.Value == 0).Count() == 0).ToList();

      List<Competitor> missingCompetitors = new List<Competitor>();

      List<Competitor> DuplicateNames = new List<Competitor>();
      if (User.IsInRole("Admin"))
      {
        List<Competitor> ToaddLst = new List<Competitor>();
        foreach (Competitor comp in db.Competitors.Where(x => x.CreatedBy == userId).ToList())
        {
          ToaddLst = db.Competitors.Where(x => x.CreatedBy == userId && x.Name.ToUpper().Trim() == comp.Name.ToUpper().Trim()).ToList();
          if (ToaddLst.Count > 1) 
          {
            DuplicateNames.AddRange(ToaddLst);
          }
        }
      }

      if (User.IsInRole("Admin")) missingCompetitors = db.Competitors.Where(x => x.DivisionGroups.Count() == 0).ToList();
      else missingCompetitors = db.Competitors.Where(x => x.CreatedBy == userId && x.DivisionGroups.Count() == 0).ToList();
      List<EventDivision> missingDivisions = new List<EventDivision>();
      if (User.IsInRole("Admin")) missingDivisions = db.EventDivisions.Where(y => y.DivisionGroups.Count() == 0).ToList();
      else missingDivisions = db.EventDivisions.Where(y => y.CreatedBy == userId && y.DivisionGroups.Count() == 0).ToList();
      ////List<Competitor> missingCompetitors = db.Competitors.Where(x => x.DivisionGroups.Count() == 0).ToList();
      ////List<EventDivision> missingDivisions = db.EventDivisions.Where(y => y.DivisionGroups.Count() == 0).ToList();
      missingDivisions.ForEach(x =>
      {
        if (!string.IsNullOrWhiteSpace(x.BeltRange))
        {
          var beltRange = x.BeltRange.Split('-');
          x.BeltRange = string.Concat(beltsCollection.FirstOrDefault(y => y.Id == int.Parse(beltRange[0]))?.Id, " ", beltsCollection.FirstOrDefault(y => y.Id == int.Parse(beltRange[0]))?.Name, "-", beltsCollection.FirstOrDefault(y => y.Id == int.Parse(beltRange[1]))?.Name);
        }

        if (!string.IsNullOrWhiteSpace(x.GenderRange))
        {
          switch (x.GenderRange)
          {
            case "0":
              x.GenderRange = "Female";
              break;
            case "1":
              x.GenderRange = "Male";
              break;
            default:
              x.GenderRange = "Male & Female";
              break;
          }
        }
      });


      var eventDivisions = db.EventDivisions.Select(x => new EventDivisionViewModel
      {
        AgeRange = x.AgeRange,
        BeltRange = x.BeltRange,
        GenderRange = x.GenderRange,
        Id = x.Id,
        Name = x.Name,
        TournamentEventId = x.TournamentEventId,
        TournamentEventName = x.TournamentEvent.Name,
        ////TournamentName = x.Tournament.Name,
        TournamentName = "",
        StrictCriteria = x.StrictCriteria,
        GroupLimit = x.GroupLimit,
        Combinations = x.Combinations,
        IsCustom = x.IsCustom,
        CreatedBy = x.CreatedBy

      }).ToList();

      if (eventDivisions != null && eventDivisions.Any())
      {
        eventDivisions.ForEach(x =>
        {
          x.TotalCompetitors = GetDivisionCount(x.AgeRange, x.BeltRange, x.WeightRange, x.GenderRange, x.StrictCriteria.GetValueOrDefault(), x.GroupLimit.GetValueOrDefault(), x.Combinations, x.TournamentEventName, (int)x.Id, x.IsCustom.GetValueOrDefault(), 1);
        });
      }
      ViewBag.DuplicateNames = DuplicateNames;
      ViewBag.missingCompetitors = missingCompetitors;
      ViewBag.missingDivisions = missingDivisions;
      
      //// ViewBag.groupContainingOneCompetitor = eventDivisions?.Where(x => x.TotalCompetitors == 1).ToList();
      if (User.IsInRole("Admin"))
        ViewBag.groupContainingOneCompetitor = eventDivisions?.Where(x => x.TotalCompetitors == 1).ToList();
      else
        ViewBag.groupContainingOneCompetitor = eventDivisions?.Where(x => x.CreatedBy == userId && x.TotalCompetitors == 1).ToList();

      ViewBag.Belts = belts;
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
        if (eventDivisionGroup == null || eventDivisionGroup.Count <= 0 || eventDevisionInfo == null || IsEdit == 0 || string.IsNullOrWhiteSpace(eventDevisionInfo.Combinations) || eventDevisionInfo.Combinations != Combination)
        {
          if (Criteria)
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
            filteredCompetitors = filteredCompetitors.Where(x => !string.IsNullOrWhiteSpace(x.Event) && x.Event.Contains(Event)).ToList();
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

      return filteredCompetitors.Distinct().Count();
    }
  }
}
