using LeaveON.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TourneyRepo.Models;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User,TournamentDirector")]
  public class PrintBracketsController : Controller
  {
    private TourneyWizardEntities db = new TourneyWizardEntities();
    //private List<BeltViewModel> beltsCollection = new List<BeltViewModel>() { new BeltViewModel { Id = 1, Name = "White" }, new BeltViewModel { Id = 2, Name = "Yellow" }, new BeltViewModel { Id = 3, Name = "Orange" }, new BeltViewModel { Id = 4, Name = "Green" }, new BeltViewModel { Id = 5, Name = "Blue" }, new BeltViewModel { Id = 6, Name = "Purple" }, new BeltViewModel { Id = 7, Name = "Brown" }, new BeltViewModel { Id = 8, Name = "Red" }, new BeltViewModel { Id = 9, Name = "Black" } };
    private List<BeltViewModel> beltsCollection = new List<BeltViewModel>();
    private List<GenderViewModel> gendersCollection = new List<GenderViewModel>() { new GenderViewModel { Id = 2, Name = "Male & Female" }, new GenderViewModel { Id = 1, Name = "Male" }, new GenderViewModel { Id = 0, Name = "Female" } };

    public ActionResult Index()
    {
      beltsCollection = new List<BeltViewModel>();
      string userId = User.Identity.GetUserId();
      var belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      foreach (var item in belts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }
      
      if (User.IsInRole("Admin")) ViewBag.TournamentEventId = new SelectList(db.TournamentEvents, "Id", "Name");
      else ViewBag.TournamentEventId = new SelectList(db.TournamentEvents.Where(x => x.CreatedBy == userId), "Id", "Name");
    ////  ViewBag.TournamentEventId = new SelectList(db.TournamentEvents, "Id", "Name");
      var ages = Enumerable.Range(1, 100);
      var weigts = Enumerable.Range(1, 1000);
      ViewBag.Ages = ages;
      ViewBag.Belts = beltsCollection;//belts;
      ViewBag.Genders = gendersCollection;

      return View();
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
    private List<PrintEventDivisionInfoViewModel> CustomLoadEventDivisionInfo(string[] Ids)
    {
      var eventDivisionInfos = new List<PrintEventDivisionInfoViewModel>();
      var divisions = db.EventDivisions.Where(x => Ids.Any(c => c == x.Id.ToString())).ToList();

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

    public async Task<ActionResult> TournamentPrintViewer(string Age, string Belt, string Gender, int Event, int id, string extras)
    {
      var eventDivisions = LoadEventDivisionInfo(Event);

      var filteredEventDivisions = new List<PrintEventDivisionInfoViewModel>();
      var eventDivisionsToPrint = new List<PrintBracketsViewModel>();
      var competitors = await db.Competitors.ToListAsync();

      if (string.IsNullOrEmpty(Gender) && string.IsNullOrEmpty(Age) && string.IsNullOrEmpty(Belt))
      {
        filteredEventDivisions.AddRange(eventDivisions);

      }
      else
      {
        if (!string.IsNullOrEmpty(Age))
        {
          var ageRange = Age.Split('-');
          filteredEventDivisions.AddRange(eventDivisions.Where(x => x.FromAge >= int.Parse(ageRange[0]) && x.ToAge <= int.Parse(ageRange[1])).ToList());
        }

        if (!string.IsNullOrEmpty(Belt))
        {
          var beltRange = Belt.Split('-');
          if (filteredEventDivisions.Count > 0)
          {
            filteredEventDivisions = filteredEventDivisions.Where(x => x.FromBelt >= int.Parse(beltRange[0]) && x.ToBelt <= int.Parse(beltRange[1])).ToList();
          }
          else
          {
            filteredEventDivisions.AddRange(eventDivisions.Where(x => x.FromBelt >= int.Parse(beltRange[0]) && x.ToBelt <= int.Parse(beltRange[1])).ToList());
          }
        }

        if (!string.IsNullOrEmpty(Gender))
        {
          if (filteredEventDivisions.Count > 0)
          {
            filteredEventDivisions = filteredEventDivisions.Where(x => x.GenderRange == Gender).ToList();
          }
          else
          {
            filteredEventDivisions.AddRange(eventDivisions.Where(x => x.GenderRange == Gender).ToList());
          }
        }
      }
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
            id = id,
            IsCustom = eventDivision.IsCustom.GetValueOrDefault(),
            IsParticipantLimited = false,
            TotalGroup = 1,
            Weight = eventDivision.WeightRange,
            //Competitors = competitors,
            Competitors = db.Competitors.Where(x => db.DivisionGroups.Where(z => z.EventDevisionId == eventDivision.Id).Select(z => z.CompetitorId).ToList().Contains(x.Id)).ToList(),
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
              id = id,
              IsCustom = eventDivision.IsCustom.GetValueOrDefault(),
              IsParticipantLimited = true,
              TotalGroup = groups.Count(),
              Weight = eventDivision.WeightRange,
              ////Competitors = competitors,
              Competitors = db.Competitors.Where(x=>  db.DivisionGroups.Where(z=>z.EventDevisionId==eventDivision.Id).Select(z=>z.CompetitorId).ToList().Contains(x.Id)).ToList(),
              EventDivisions = eventDivisionInfo,
              DivisionGroups = eventDivisionGroupsInfo
            });
            grpNo++;
          }
        }
      }

      var result = await CustomTournamentPrintViewer(Age, Belt, Gender, extras);

      eventDivisionsToPrint.AddRange(result);

      return View(eventDivisionsToPrint);
    }


    public async Task<List<PrintBracketsViewModel>> CustomTournamentPrintViewer(string Age, string Belt, string Gender, string extras)
    {

      var data = JsonConvert.DeserializeObject<List<ExtraEventBracket>>(extras);

      var eventDivisions = new List<PrintEventDivisionInfoViewModel>();

      var filteredEventDivisions = new List<PrintEventDivisionInfoViewModel>();
      var eventDivisionsToPrint = new List<PrintBracketsViewModel>();

      var indexId = 0;

      foreach (var item in data)
      {


        eventDivisions.AddRange(CustomLoadEventDivisionInfo(item.EventDivisions));


        var competitors = await db.Competitors.ToListAsync();
        if (string.IsNullOrEmpty(Gender) && string.IsNullOrEmpty(Age) && string.IsNullOrEmpty(Belt))
        {
          filteredEventDivisions.AddRange(eventDivisions);

        }
        else
        {
          if (!string.IsNullOrEmpty(Age))
          {
            var ageRange = Age.Split('-');
            filteredEventDivisions.AddRange(eventDivisions.Where(x => x.FromAge >= int.Parse(ageRange[0]) && x.ToAge <= int.Parse(ageRange[1])).ToList());
          }

          if (!string.IsNullOrEmpty(Belt))
          {
            var beltRange = Belt.Split('-');
            if (filteredEventDivisions.Count > 0)
            {
              filteredEventDivisions = filteredEventDivisions.Where(x => x.FromBelt >= int.Parse(beltRange[0]) && x.ToBelt <= int.Parse(beltRange[1])).ToList();
            }
            else
            {
              filteredEventDivisions.AddRange(eventDivisions.Where(x => x.FromBelt >= int.Parse(beltRange[0]) && x.ToBelt <= int.Parse(beltRange[1])).ToList());
            }
          }

          if (!string.IsNullOrEmpty(Gender))
          {
            if (filteredEventDivisions.Count > 0)
            {
              filteredEventDivisions = filteredEventDivisions.Where(x => x.GenderRange == Gender).ToList();
            }
            else
            {
              filteredEventDivisions.AddRange(eventDivisions.Where(x => x.GenderRange == Gender).ToList());
            }
          }
        }
        var eventDivIds = filteredEventDivisions.Select(x => x.Id).ToList();
        ////var eventDivisionInfo = await db.EventDivisions.Where(x => eventDivIds.Contains(x.Id)).Include(e => e.Tournament).Include(e => e.TournamentEvent).ToListAsync();
        var eventDivisionInfo = await db.EventDivisions.Where(x => eventDivIds.Contains(x.Id)).Include(e => e.TournamentEvent).ToListAsync();
        var eventDivisionGroupsInfo = await db.DivisionGroups.Where(x => x.EventDevisionId != null && eventDivIds.Contains(x.EventDevisionId.Value)).ToListAsync();


        var BracketId = data[indexId].BracketId;

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
              id = BracketId,
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
                id = BracketId,
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

        indexId++;

      }



      return eventDivisionsToPrint;
    }



    //public async Task<ActionResult> TournamentPrintViewer(string Age, string Belt, string Gender, int Event, int id)
    //{
    //  var eventDivisions = LoadEventDivisionInfo(Event);
    //  var filteredEventDivisions = new List<PrintEventDivisionInfoViewModel>();
    //  var eventDivisionsToPrint = new List<PrintBracketsViewModel>();
    //  var competitors = await db.Competitors.ToListAsync();
    //  if (string.IsNullOrEmpty(Gender) && string.IsNullOrEmpty(Age) && string.IsNullOrEmpty(Belt))
    //  {
    //    filteredEventDivisions.AddRange(eventDivisions);

    //  }
    //  else
    //  {
    //    if (!string.IsNullOrEmpty(Age))
    //    {
    //      var ageRange = Age.Split('-');
    //      filteredEventDivisions.AddRange(eventDivisions.Where(x => x.FromAge >= int.Parse(ageRange[0]) && x.ToAge <= int.Parse(ageRange[1])).ToList());
    //    }

    //    if (!string.IsNullOrEmpty(Belt))
    //    {
    //      var beltRange = Belt.Split('-');
    //      if (filteredEventDivisions.Count > 0)
    //      {
    //        filteredEventDivisions = filteredEventDivisions.Where(x => x.FromBelt >= int.Parse(beltRange[0]) && x.ToBelt <= int.Parse(beltRange[1])).ToList();
    //      }
    //      else
    //      {
    //        filteredEventDivisions.AddRange(eventDivisions.Where(x => x.FromBelt >= int.Parse(beltRange[0]) && x.ToBelt <= int.Parse(beltRange[1])).ToList());
    //      }
    //    }

    //    if (!string.IsNullOrEmpty(Gender))
    //    {
    //      if (filteredEventDivisions.Count > 0)
    //      {
    //        filteredEventDivisions = filteredEventDivisions.Where(x => x.GenderRange == Gender).ToList();
    //      }
    //      else
    //      {
    //        filteredEventDivisions.AddRange(eventDivisions.Where(x => x.GenderRange == Gender).ToList());
    //      }
    //    }
    //  }
    //  var eventDivIds = filteredEventDivisions.Select(x => x.Id).ToList();
    //  var eventDivisionInfo = await db.EventDivisions.Where(x => eventDivIds.Contains(x.Id)).Include(e => e.Tournament).Include(e => e.TournamentEvent).ToListAsync();
    //  var eventDivisionGroupsInfo = await db.DivisionGroups.Where(x => x.EventDevisionId != null && eventDivIds.Contains(x.EventDevisionId.Value)).ToListAsync();

    //  foreach (var eventDivision in filteredEventDivisions)
    //  {
    //    if (string.IsNullOrWhiteSpace(eventDivision.Combinations))
    //    {
    //      eventDivisionsToPrint.Add(new PrintBracketsViewModel
    //      {
    //        Age = eventDivision.AgeRange,
    //        Belt = eventDivision.BeltRange,
    //        Combination = eventDivision.Combinations,
    //        Event = eventDivision.Name,
    //        EventId = eventDivision.Id,
    //        Gender = eventDivision.GenderRange,
    //        Group = 1,
    //        id = id,
    //        IsCustom = eventDivision.IsCustom.GetValueOrDefault(),
    //        IsParticipantLimited = false,
    //        TotalGroup = 1,
    //        Weight = eventDivision.WeightRange,
    //        Competitors = competitors,
    //        EventDivisions = eventDivisionInfo,
    //        DivisionGroups = eventDivisionGroupsInfo
    //      });
    //    }
    //    else
    //    {
    //      var groups = eventDivision.Combinations.Split(',');
    //      var grpNo = 1;
    //      foreach (var group in groups)
    //      {
    //        eventDivisionsToPrint.Add(new PrintBracketsViewModel
    //        {
    //          Age = eventDivision.AgeRange,
    //          Belt = eventDivision.BeltRange,
    //          Combination = eventDivision.Combinations,
    //          Event = eventDivision.Name,
    //          EventId = eventDivision.Id,
    //          Gender = eventDivision.GenderRange,
    //          Group = grpNo,
    //          id = id,
    //          IsCustom = eventDivision.IsCustom.GetValueOrDefault(),
    //          IsParticipantLimited = true,
    //          TotalGroup = groups.Count(),
    //          Weight = eventDivision.WeightRange,
    //          Competitors = competitors,
    //          EventDivisions = eventDivisionInfo,
    //          DivisionGroups = eventDivisionGroupsInfo
    //        });
    //        grpNo++;
    //      }
    //    }
    //  }

    //  return View(eventDivisionsToPrint);
    //}

    [AllowAnonymous]
    public async Task<ActionResult> TournamentViewer(string Age, string Belt, string Weight, string Gender, string Event, int id, string Combination, int Group, int TotalGroup, int EventId, bool IsCustom, List<Competitor> Competitors, List<EventDivision> EventDivisions, List<DivisionGroup> DivisionGroups)
    {
      beltsCollection = new List<BeltViewModel>();
      string userId = User.Identity.GetUserId();
      var Dbbelts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      foreach (var item in Dbbelts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }
      var filteredCompetitors = new List<CompetitorViewModel>();
      var eventDevisionInfo = EventDivisions.FirstOrDefault(x => x.Id == EventId);
      var eventDivisionGroup = DivisionGroups.Where(x => x.EventDevisionId == EventId).ToList();

      if (IsCustom)
      {
        if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
        {
          foreach (var divisionGroup in eventDivisionGroup)
          {
            var competitorInfo = Competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
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

            filteredCompetitors.AddRange(Competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
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
              filteredCompetitors.AddRange(Competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
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
              filteredCompetitors.AddRange(Competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
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
                    filteredCompetitors.AddRange(Competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
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
                    filteredCompetitors.AddRange(Competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
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
                //Hasnain
                if(filteredCompetitors.Count > i)
                {
                  filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
                }
                ////filteredCompetitors[lastStudentGroupAssigned].GroupId = lastGroupAssigned;
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
              var competitorInfo = Competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
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


      var schoolGroupedCompetitor = filteredCompetitors.Where(x => x.GroupId == Group).GroupBy(info => info.School?.Trim())
                        .Select(group => new
                        {
                          Key = group.Key,
                          Count = group.Count()
                        });
      var sortedGroupedCompetitors = filteredCompetitors.Where(x => x.GroupId == Group).ToList();
      foreach (var sortedGroupedCompetitor in sortedGroupedCompetitors)
      {
        sortedGroupedCompetitor.Count = schoolGroupedCompetitor.First(y => y.Key == sortedGroupedCompetitor.School?.Trim()).Count;
      }

      var competitorNames = sortedGroupedCompetitors.OrderByDescending(x => x.Count).Select(x => new Tuple<string, string>(x.Name.Replace("'", "").Trim(), string.IsNullOrEmpty(x.School) ? "\'-----\'" : $"{x.School.Replace("'", "").Trim()}")).ToList();
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

              if (winRemaining == 0)
              {
                break;
              }

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
            ViewBag.EventDivisionId = EventId;

            if (competitorNames.All(x => x.Item1.Equals("--BYE--")))
            {
              return Content(string.Empty);
            }

            return PartialView("_TournamentViewerRoundRobin", ListMatches(competitorNames.Select(x => x.Item1).ToList(), competitorNames.Select(x => x.Item2).ToList(), competitorNames.Count));
          }
        case 4:
          {
            ViewBag.GroupMessage = $"Group {Group} of {TotalGroup}";
            ViewBag.Bracket = "High Score List";
            ViewBag.GenderText = genderText;
            ViewBag.BeltDevision = beltText;
            ViewBag.EventDivisionId = EventId;

            if (competitorNames.All(x => x.Item1.Equals("--BYE--")))
            {
              return Content(string.Empty);
            }

            return PartialView("_TournamentViewerList", competitorNames.Select(x => x.Item1).ToList());
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

              if (winRemaining == 0)
              {
                break;
              }

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

      ViewBag.ComSchoolArray = comSchoolNamesArray.ToString();
      ViewBag.SchoolArray = schoolNamesArray.ToString();
      ViewBag.GenderText = genderText;
      ViewBag.BeltDevision = beltText;
      ViewBag.GroupMessage = $"Group {Group} of {TotalGroup}";
      ViewBag.Bracket = id;
      ViewBag.UiId = Guid.NewGuid().ToString();
      ViewBag.EventDivisionId = EventId;

      if (competitorNames.All(x => x.Item1.Equals("--BYE--")))
      {
        return Content(string.Empty);
      }

      return PartialView("_TournamentView");
    }

    [AllowAnonymous]
    public async Task<ActionResult> TournamentViewerV1(string Age, string Belt, string Weight, string Gender, string Event, int id, int EventId, bool IsCustom, List<Competitor> Competitors, List<EventDivision> EventDivisions, List<DivisionGroup> DivisionGroups)
    {
      var filteredCompetitors = new List<CompetitorViewModel>();
      var eventDevisionInfo = EventDivisions.FirstOrDefault(x => x.Id == EventId);
      var eventDivisionGroup = DivisionGroups.Where(x => x.EventDevisionId == EventId).ToList();

      if (IsCustom)
      {
        if (eventDivisionGroup != null && eventDivisionGroup.Count > 0)
        {
          foreach (var divisionGroup in eventDivisionGroup)
          {
            var competitorInfo = Competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
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

            filteredCompetitors.AddRange(Competitors.Where(x => x.Age != null && x.Age >= fromAge && x.Age <= toAge).Select(x => new CompetitorViewModel
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
              filteredCompetitors.AddRange(Competitors.Where(x => !string.IsNullOrWhiteSpace(x.Belt) && int.Parse(x.Belt) >= frombelt && int.Parse(x.Belt) <= toBelt).Select(x => new CompetitorViewModel
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
              filteredCompetitors.AddRange(Competitors.Where(x => x.Weight != null && x.Weight >= fromWeight && x.Weight <= toWeight).Select(x => new CompetitorViewModel
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
                    filteredCompetitors.AddRange(Competitors.Where(x => x.Gender != null && x.Gender == gender).Select(x => new CompetitorViewModel
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
                    filteredCompetitors.AddRange(Competitors.Where(x => x.Gender != null && (x.Gender == true || x.Gender == false)).Select(x => new CompetitorViewModel
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
              var competitorInfo = Competitors.FirstOrDefault(x => x.Id == divisionGroup.CompetitorId);
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

              if (winRemaining == 0)
              {
                break;
              }

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
            ViewBag.EventDivisionId = EventId;
            if (competitorNames.All(x => x.Item1.Equals("--BYE--")))
            {
              return Content(string.Empty);
            }
            return PartialView("_TournamentViewerRoundRobin", ListMatches(competitorNames.Select(x => x.Item1).ToList(), competitorNames.Select(x => x.Item2).ToList(), competitorNames.Count));
          }
        case 4:
          {
            ViewBag.GroupMessage = $"Group 1 of 1";
            ViewBag.Bracket = "High Score List";
            ViewBag.GenderText = genderText;
            ViewBag.BeltDevision = beltText;
            ViewBag.EventDivisionId = EventId;
            if (competitorNames.All(x => x.Item1.Equals("--BYE--")))
            {
              return Content(string.Empty);
            }
            return PartialView("_TournamentViewerList", competitorNames.Select(x => x.Item1).ToList());
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

      ViewBag.ComSchoolArray = comSchoolNamesArray.ToString();
      ViewBag.SchoolArray = schoolNamesArray.ToString();
      ViewBag.GenderText = genderText;
      ViewBag.BeltDevision = beltText;
      ViewBag.GroupMessage = $"Group 1 of 1";
      ViewBag.Bracket = id;
      ViewBag.UiId = Guid.NewGuid().ToString();
      ViewBag.EventDivisionId = EventId;

      if (competitorNames.All(x=> x.Item1.Equals("--BYE--")))
      {
        return Content(string.Empty);
      }

      return PartialView("_TournamentView");
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
