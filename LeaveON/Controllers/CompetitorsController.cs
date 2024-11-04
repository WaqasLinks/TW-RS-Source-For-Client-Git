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
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing.Drawing2D;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using System.Web.Http.Results;
using Microsoft.Owin;
using LeaveON.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class CompetitorsController : Controller
  {
    public string myConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    private List<GenderViewModel> gendersCollection = new List<GenderViewModel>() { new GenderViewModel { Value = true, Name = "Male" }, new GenderViewModel { Value = false, Name = "Female" } };
    private TourneyWizardEntities db = new TourneyWizardEntities();
    //private List<BeltViewModel> beltsCollection = new List<BeltViewModel>() { new BeltViewModel { Id = 1, Name = "White" }, new BeltViewModel { Id = 2, Name = "Yellow" }, new BeltViewModel { Id = 3, Name = "Orange" }, new BeltViewModel { Id = 4, Name = "Green" }, new BeltViewModel { Id = 5, Name = "Blue" }, new BeltViewModel { Id = 6, Name = "Purple" }, new BeltViewModel { Id = 7, Name = "Brown" }, new BeltViewModel { Id = 8, Name = "Red" }, new BeltViewModel { Id = 9, Name = "Black" } };
    private List<BeltViewModel> beltsCollection = new List<BeltViewModel>();
    private List<EventDivisionInfoViewModel> eventDivisionInfos = new List<EventDivisionInfoViewModel>();

    private List<EventDivisionInfoViewModel> LoadEventDivisionInfo()
    {
      var divisions = db.EventDivisions.Where(x => x.IsCustom == false).ToList();
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

          eventDivisionInfos.Add(new EventDivisionInfoViewModel
          {
            Id = x.Id,
            FromAge = fromAge,
            ToAge = toAge,
            FromBelt = frombelt,
            ToBelt = toBelt,
            FromWeight = fromWeight,
            ToWeight = toWeight,
            Gender = gender,
            TournamentEventId = x.TournamentEventId
          });
        });
      }

      return new List<EventDivisionInfoViewModel>();
    }

    public async Task<ActionResult> GetEventDevisions(int Id, string UId)
    {
      ViewBag.EventDivisions = await db.EventDivisions.Where(x => x.TournamentEventId == Id).Select(x => new SelectListItem { Value = x.TournamentEventId + "_" + x.Id, Text = x.Name }).ToListAsync();
      ViewBag.Id = string.Concat(Id, UId);
      return PartialView();
    }

    public async Task<ActionResult> GetCompetitorEventDevisions(int CompetitorId, int EventId)
    {
      return PartialView(await db.DivisionGroups.Where(x => x.EventDevisionId != null && x.TournamentEventId == EventId && x.CompetitorId == CompetitorId).Select(x => x.EventDivision.Name).ToListAsync());
    }

    // GET: Competitors
    public async Task<ActionResult> Index()
    {
      string userId = User.Identity.GetUserId();
      var userTournamentsIds = new List<int>();
      var Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      var competitors = new List<Competitor>();
      if (User.IsInRole("Admin")) competitors = await db.Competitors.ToListAsync();
      else {
        userTournamentsIds = db.ScheduledEvents.Where(x => x.CreatedBy == userId).Select(y => y.Id).ToList();
        var competitorExistsTournaments = db.CompetitorEventRelations.Where(x => userTournamentsIds.Contains(x.TournamentId.Value)).Select(y => y.CompetitorId).ToList();
        competitors = await db.Competitors.Where(x => x.CreatedBy == userId || competitorExistsTournaments.Contains(x.Id)).ToListAsync(); 
      }
      var competitorswithEvents = new List<CompetitorIndexViewModel>();
      if (competitors != null && competitors.Count > 0)
      {
        foreach (var item in competitors)
        {


          //DateTime date;
          //string format = "dd/MM/yy hh:mm:ss";

          //if (DateTime.TryParseExact(item.RegistrationDate, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
          //{
          //  Console.WriteLine($"The date {item.RegistrationDate} is valid in the format {format}. Parsed date: {date}");
          //}
          //else
          //{
          //  Console.WriteLine($"The date {item.RegistrationDate} is not valid in the format {format}.");
          //}

          var competitorswithEvent = new CompetitorIndexViewModel
          {
            Address = item.Address,
            Age = item.Age,
            Belt = Belts.FirstOrDefault(y => y.BeltId == Convert.ToInt32(item.Belt))?.BeltName,
            City = item.City,
            Email = item.Email,
            Gender = item.Gender,
            Id = item.Id,
            Name = item.Name,
            Phone = item.Phone,
            School = item.School,
            Serial = item.Serial,
            State = item.State,
            Weight = item.Weight,
            Zip = item.Zip,
            RegistrationDate = DateTime.Now,
            RegistrationDates = string.IsNullOrEmpty(item.RegistrationDate) ? "" : item.RegistrationDate.Split(' ')[0],
             //RegistrationDate = string.IsNullOrEmpty(item.RegistrationDate) ? DateTime.Now:  DateTime.ParseExact(item?.RegistrationDate, "MM/dd/yy", System.Globalization.CultureInfo.InvariantCulture),

            Event = new Dictionary<decimal, string>()
          };
          var competitrorEvents = await db.DivisionGroups.Where(x => x.CompetitorId == item.Id).ToListAsync();
          if (competitrorEvents != null && competitrorEvents.Count > 0)
          {
            foreach (var competitrorEvent in competitrorEvents)
            {
              var eventInfo = await db.TournamentEvents.FindAsync(competitrorEvent.TournamentEventId);
              if (eventInfo != null)
              {
                if (!competitorswithEvent.Event.ContainsKey(eventInfo.Id))
                {
                  competitorswithEvent.Event.Add(eventInfo.Id, eventInfo.Name);
                }
              }
            }
          }
          competitorswithEvents.Add(competitorswithEvent);
        }
      }
      return View(competitorswithEvents);
    }

    public async Task<ActionResult> TournamentDetail()
    {
      ////var tournamentDetails = from tournament in db.Tournaments
      ////                        join eventDivision in db.EventDivisions on tournament.Id equals eventDivision.TournamentId
      ////                        join te in db.TournamentEvents on eventDivision.TournamentEventId equals te.Id
      ////                        join dg in db.DivisionGroups on eventDivision.Id equals dg.EventDevisionId
      ////                        join comp in db.Competitors on dg.CompetitorId equals comp.Id
      ////                        select new TournamentDetailViM() { Competitor = comp.Name, Division = eventDivision.Name, Event = te.Name, GroupNo = dg.GroupNo, Tournament = tournament.Name };

      ////return View(await tournamentDetails.Distinct().ToListAsync());
      return View();
    }

    // GET: Competitors/Create
    public ActionResult Create()
    {
      string userId = User.Identity.GetUserId();
      ViewBag.TournamentEventId = new SelectList(db.TournamentEvents.Where(x => x.CreatedBy == userId), "Id", "Name");
      ViewBag.Belts = new SelectList(db.Belts.Where(x => x.CreatedBy == userId), "BeltId", "BeltName");
      ViewBag.Genders = gendersCollection;
      return View();
    }

    // POST: Competitors/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Serial,Name,Age,Weight,Gender,State,City,Address,Zip,Email,Phone,School,Belt,Event,RegistrationDate,DateCreated,DateModified")] Competitor competitor, string FormatedDate, string[] TournamentEvents, string[] EventDivisions)
    {
      if (!string.IsNullOrEmpty(FormatedDate))
      {
        var date = FormatedDate.Split('/');
        competitor.RegistrationDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(int.Parse(date[2])), int.Parse(date[0]), int.Parse(date[1])).ToString();
      }

      //competitor.Id = Guid.NewGuid().ToString();
      competitor.DateCreated = DateTime.Now;
      competitor.CreatedBy = User.Identity.GetUserId();
      if (ModelState.IsValid)
      {
        db.Competitors.Add(competitor);
        await db.SaveChangesAsync();

        if (EventDivisions != null && EventDivisions.Count() > 0)
        {
          foreach (var eventDivi in EventDivisions.Distinct())
          {
            var evntDivInfo = eventDivi.Split('_');
            var eventId = int.Parse(evntDivInfo[0]);
            var divId = int.Parse(evntDivInfo[1]);
            var eventInfo = await db.TournamentEvents.FindAsync(eventId);
            competitor.Event = competitor.Event + "<br>" + eventInfo.Name;
            db.DivisionGroups.Add(new DivisionGroup() { CompetitorId = competitor.Id, TournamentEventId = eventId, EventDevisionId = divId, GroupNo = 1 });
          }
          db.Entry(competitor).State = EntityState.Modified;
          await db.SaveChangesAsync();
        }

        if (TournamentEvents != null && TournamentEvents.Count() > 0)
        {
          foreach (var evntid in TournamentEvents.Distinct())
          {
            if (string.IsNullOrEmpty(evntid)) break;
            //var evntDivInfo = eventDivi.Split('_');
            var teventId = int.Parse(evntid);
            //var divId = int.Parse(evntDivInfo[1]);
            var eventInfo = await db.TournamentEvents.FindAsync(teventId);
            competitor.Event = competitor.Event + "<br>" + eventInfo.Name;
            //competitor.Event = eventInfo.Name + "<br>";
            List<DivisionGroup> LstDivisionGroups = db.DivisionGroups.Where(x => x.CompetitorId == competitor.Id && x.TournamentEventId == teventId).ToList();
            if (LstDivisionGroups.Count == 0)
            {
              db.DivisionGroups.Add(new DivisionGroup() { CompetitorId = competitor.Id, TournamentEventId = teventId });
              await db.SaveChangesAsync();
            }

          }
        }

        return Redirect("/Competitors/Index");
      }
      ViewBag.Genders = gendersCollection;
      //ViewBag.FormatedDate = competitor.RegistrationDate == null? string.Empty :competitor.RegistrationDate.GetValueOrDefault().ToString("MM/dd/yy");
      //ViewBag.FormatedDate = competitor.RegistrationDate == null ? string.Empty : competitor.RegistrationDate.ToString("MM/dd/yy");
      ViewBag.TournamentEventId = new SelectList(db.TournamentEvents, "Id", "Name");
      return View(competitor);
    }

    // GET: Competitors/Edit/5
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
      ViewBag.Belts = new SelectList(db.Belts.Where(x => x.CreatedBy == userId), "BeltId", "BeltName");

      ViewBag.Genders = gendersCollection;
      //ViewBag.FormatedDate = competitor.RegistrationDate == null ? string.Empty : competitor.RegistrationDate.ToString("MM/dd/yy");
      var tournamentEvents = await db.TournamentEvents.Where(x => x.CreatedBy == userId).ToListAsync();
      ViewBag.TournamentEventId = new SelectList(tournamentEvents, "Id", "Name");

      var competitorEventDiv = await db.DivisionGroups.Where(x => x.CompetitorId == competitor.Id).ToListAsync();
      if (competitorEventDiv != null && competitorEventDiv.Count > 0)
      {
        var eventDivInfo = new Dictionary<decimal?, CompetitorEventDivisionViewModel>();
        foreach (var item in competitorEventDiv)
        {
          var eventId = item.TournamentEventId.GetValueOrDefault();
          if (eventDivInfo.ContainsKey(eventId))
          {
            eventDivInfo[eventId].SelectedDivisionIds.Add(item.EventDevisionId);
          }
          else
          {
            eventDivInfo.Add(eventId, new CompetitorEventDivisionViewModel { TournamentEvents = tournamentEvents, Divisions = await db.EventDivisions.Where(x => x.TournamentEventId == item.TournamentEventId).Select(x => new SelectListItem { Value = x.TournamentEventId + "_" + x.Id, Text = x.Name }).ToListAsync(), SelectedDivisionIds = new List<decimal?>() { item.EventDevisionId } });
          }
        }

        foreach (var item in eventDivInfo)
        {
          var selectedDiv = item.Value.Divisions.Where(x => item.Value.SelectedDivisionIds.Contains(decimal.Parse(x.Value.Split('_')[1]))).ToList();
          if (selectedDiv != null)
          {
            foreach (var div in selectedDiv)
            {
              div.Selected = true;
            }
          }
        }

        ViewBag.CompetitorEventDivisionInfo = eventDivInfo;
      }
      return View(competitor);
    }

    // POST: Competitors/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Serial,Name,Age,Weight,Gender,State,City,Address,Zip,Email,Phone,School,Belt,Event,RegistrationDate,DateCreated,DateModified")] Competitor competitor, string FormatedDate, string[] TournamentEvents, string[] EventDivisions)
    {

      if (!string.IsNullOrEmpty(FormatedDate))
      {
        var date = FormatedDate.Split('/');
        competitor.RegistrationDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(int.Parse(date[2])), int.Parse(date[0]), int.Parse(date[1])).ToString();
      }

      competitor.DateModified = DateTime.Now;
      var oldProfileData = db.Competitors.AsNoTracking().FirstOrDefault(x => x.Id == competitor.Id);
      competitor.CreatedBy = oldProfileData.CreatedBy;
      competitor.ProfilePath = oldProfileData?.ProfilePath;
      
      string userId = User.Identity.GetUserId();
      var userTournamentsIds = db.ScheduledEvents.Where(x => x.CreatedBy == userId).Select(y => y.Id).ToList();
      var currUserTournament = db.CompetitorEventRelations.FirstOrDefault(x => userTournamentsIds.Contains(x.TournamentId.Value));
      
      if (ModelState.IsValid)
      {

        //await db.SaveChangesAsync();  

        var eventDiv = await db.DivisionGroups.Where(x => x.CompetitorId == competitor.Id).ToListAsync();
        if (eventDiv != null && eventDiv.Count > 0)
        {
          db.DivisionGroups.RemoveRange(eventDiv);
          db.SaveChanges();
        }

        if (EventDivisions != null && EventDivisions.Count() > 0)
        {
          foreach (var eventDivi in EventDivisions.Distinct())
          {
            var evntDivInfo = eventDivi.Split('_');
            var eventId = int.Parse(evntDivInfo[0]);
            var divId = int.Parse(evntDivInfo[1]);
            var eventInfo = await db.TournamentEvents.FindAsync(eventId);
            //competitor.Event = competitor.Event + "<br>" + eventInfo.Name;

            if (currUserTournament != null && currUserTournament.TournamentId != null)
            {
              db.DivisionGroups.Add(new DivisionGroup() { CompetitorId = competitor.Id, TournamentEventId = eventId, EventDevisionId = divId, GroupNo = 1, TournamentId= currUserTournament.TournamentId });
            }
            else
            {
              db.DivisionGroups.Add(new DivisionGroup() { CompetitorId = competitor.Id, TournamentEventId = eventId, EventDevisionId = divId, GroupNo = 1 });
            }
            
          }

          db.Entry(competitor).State = EntityState.Modified;
          await db.SaveChangesAsync();
        }
        competitor.Event = "";

  

        if (TournamentEvents != null && TournamentEvents.Count() > 0)
        {
          foreach (var evntid in TournamentEvents.Distinct())
          {
            if (string.IsNullOrEmpty(evntid)) break;
            //var evntDivInfo = eventDivi.Split('_');
            var teventId = int.Parse(evntid);
            //var divId = int.Parse(evntDivInfo[1]);
            var eventInfo = await db.TournamentEvents.FindAsync(teventId);
            competitor.Event = competitor.Event + "<br>" + eventInfo.Name;
            //competitor.Event = eventInfo.Name + "<br>";
            List<DivisionGroup> LstDivisionGroups = db.DivisionGroups.Where(x => x.CompetitorId == competitor.Id && x.TournamentEventId == teventId).ToList();
            if (LstDivisionGroups.Count == 0)
            {
              if (currUserTournament != null && currUserTournament.TournamentId != null)
              {
                db.DivisionGroups.Add(new DivisionGroup() { CompetitorId = competitor.Id, TournamentEventId = teventId, TournamentId = currUserTournament.TournamentId });
              }
              else
              {
                db.DivisionGroups.Add(new DivisionGroup() { CompetitorId = competitor.Id, TournamentEventId = teventId });
              }
              
            }

          }
        }

        db.Entry(competitor).State = EntityState.Modified;
        await db.SaveChangesAsync();


        return Redirect("/Competitors/Index");
      }
      ViewBag.Genders = gendersCollection;
      //ViewBag.FormatedDate = competitor.RegistrationDate == null ? string.Empty : competitor.RegistrationDate.ToString("MM/dd/yy");
      var tournamentEvents = await db.TournamentEvents.ToListAsync();
      ViewBag.TournamentEventId = new SelectList(tournamentEvents, "Id", "Name");

      var competitorEventDiv = await db.DivisionGroups.Where(x => x.CompetitorId == competitor.Id).ToListAsync();
      if (competitorEventDiv != null && competitorEventDiv.Count > 0)
      {
        var eventDivInfo = new Dictionary<decimal, CompetitorEventDivisionViewModel>();
        foreach (var item in competitorEventDiv)
        {
          var eventId = item.TournamentEventId.GetValueOrDefault();
          if (eventDivInfo.ContainsKey(eventId))
          {
            eventDivInfo[eventId].SelectedDivisionIds.Add(item.EventDevisionId);
          }
          else
          {
            eventDivInfo.Add(eventId, new CompetitorEventDivisionViewModel { TournamentEvents = tournamentEvents, Divisions = await db.EventDivisions.Where(x => x.TournamentEventId == item.TournamentEventId).Select(x => new SelectListItem { Value = x.TournamentEventId + "_" + x.Id, Text = x.Name }).ToListAsync(), SelectedDivisionIds = new List<decimal?>() { item.EventDevisionId } });
          }
        }
        ViewBag.CompetitorEventDivisionInfo = eventDivInfo;
      }
      return View(competitor);
    }


    // GET: Competitors/Delete/5
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

    // POST: Competitors/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      var comptGrp = db.DivisionGroups.Where(x => x.CompetitorId == id).ToList();
      db.DivisionGroups.RemoveRange(comptGrp);
      var comptEvents = db.CompetitorEventRelations.Where(x => x.CompetitorId == id).ToList();
      db.CompetitorEventRelations.RemoveRange(comptEvents);

      Competitor competitor = await db.Competitors.FindAsync(id);
      db.Competitors.Remove(competitor);
      await db.SaveChangesAsync();
      return Redirect("/Competitors/Index");
    }

    // GET: Competitors/Delete/allRecords
    public async Task<ActionResult> DeleteAllRecords()
    {

      return View();
    }
    [HttpPost, ActionName("DeleteAllRecords")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteAllConfirmed()
    {
      //Competitor competitor = await db.Competitors.FirstAsync();
      db.DivisionGroups.RemoveRange(db.DivisionGroups);
      db.CompetitorEventRelations.RemoveRange(db.CompetitorEventRelations);
      db.Competitors.RemoveRange(db.Competitors);
      await db.SaveChangesAsync();
      db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Competitor', RESEED, 0)");
      return Redirect("/Competitors/Index");
    }

    private ActionResult ActionResult(string v)
    {
      throw new NotImplementedException();
    }

    public ActionResult Upload()
    {
      return View();
    }

    [HttpPost]
    public ActionResult UploadExcelFile()
    {
      string userId = User.Identity.GetUserId();
      var belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      foreach (var item in belts)
      {
        beltsCollection.Add(new BeltViewModel { Id = item.BeltId.Value, Name = item.BeltName });
      }
      if (Request.Files.Count > 0)
      {
        LoadEventDivisionInfo();
        List<string> allColumns = new List<string>();
        List<string> allRows = new List<string>();
        List<string> allErrors = new List<string>();
        var tableName = "Competitor";


        try
        {
          HttpFileCollectionBase postedFiles = Request.Files;
          HttpPostedFileBase postedFile = postedFiles[0];
          string filePath = string.Empty;
          var noOfCol = 0;
          var noOfRow = 0;
          var tableType = new List<TableType>();

          if (postedFile != null)
          {
            string path = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(path))
            {
              Directory.CreateDirectory(path);
            }

            filePath = path + Path.GetFileName(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
            postedFile.SaveAs(filePath);



            using (SqlConnection conn = new SqlConnection(myConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' and UPPER(COLUMN_NAME) <> 'Id' and UPPER(COLUMN_NAME) <> 'DateCreated'  and UPPER(COLUMN_NAME) <> 'DateModified'", conn))
            {
              SqlDataAdapter adapt = new SqlDataAdapter(cmd);
              adapt.SelectCommand.CommandType = CommandType.Text;

              DataTable dt = new DataTable();
              adapt.Fill(dt);

              if (dt.Rows.Count > 0)
              {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                  var tableTpe = new TableType();
                  tableTpe.ColumnName = dt.Rows[i][0].ToString();
                  tableTpe.DataType = dt.Rows[i][1].ToString();
                  tableType.Add(tableTpe);
                }

              }
            }


            string fileName = postedFile.FileName;

            string fileContentType = postedFile.ContentType;
            byte[] fileBytes = new byte[postedFile.ContentLength];
            var data = postedFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(postedFile.ContentLength));

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(postedFile.InputStream))
            {
              var currentSheet = package.Workbook.Worksheets;
              var workSheet = currentSheet.First();
              noOfCol = workSheet.Dimension.End.Column;
              noOfRow = workSheet.Dimension.End.Row;

              for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
              {
                for (int colIterator = 1; colIterator <= noOfCol; colIterator++)
                {
                  if (rowIterator == 1)
                    allColumns.Add(Convert.ToString(workSheet.Cells[1, colIterator].Value));
                  else
                  {
                    allRows.Add(Convert.ToString(workSheet.Cells[rowIterator, colIterator].Value));

                    //var dataType = tableType.FirstOrDefault(x => x.ColumnName.ToUpper() == allColumns[colIterator - 1].ToUpper()).DataType;

                    //if (dataType == "nvarchar")
                    //{
                    //  try
                    //  {
                    //    string test = Convert.ToString(workSheet.Cells[rowIterator, colIterator].Value);
                    //  }
                    //  catch (Exception ex)
                    //  {
                    //    allErrors.Add("Value must be of type string at Col: " + colIterator + " Row: " + rowIterator);
                    //  }
                    //}
                    //else if (dataType == "int")
                    //{
                    //  try
                    //  {
                    //    int test = Convert.ToInt32(workSheet.Cells[rowIterator, colIterator].Value);
                    //  }
                    //  catch (Exception ex)
                    //  {
                    //    allErrors.Add("Value must be of type int at Col: " + colIterator + " Row: " + rowIterator);
                    //  }
                    //}
                    //else if (dataType == "datetime")
                    //{
                    //  try
                    //  {
                    //    DateTime test = Convert.ToDateTime(workSheet.Cells[rowIterator, colIterator].Value);
                    //  }
                    //  catch (Exception ex)
                    //  {
                    //    allErrors.Add("Value must be of type datetime at Col: " + colIterator + " Row: " + rowIterator);
                    //  }
                    //}
                    //else if (dataType == "bit")
                    //{
                    //  try
                    //  {
                    //    int test = Convert.ToInt32(workSheet.Cells[rowIterator, colIterator].Value);
                    //    if (test != 0 && test != 1)
                    //      allErrors.Add("Value must be of type boolean at Col: " + colIterator + " Row: " + rowIterator);
                    //  }
                    //  catch (Exception ex)
                    //  {
                    //    allErrors.Add("Value must be of type boolean at Col: " + colIterator + " Row: " + rowIterator);
                    //  }
                    //}
                    //else if (dataType == "decimal")
                    //{
                    //  try
                    //  {
                    //    decimal test = Convert.ToDecimal(workSheet.Cells[rowIterator, colIterator].Value);
                    //  }
                    //  catch (Exception ex)
                    //  {
                    //    allErrors.Add("Value must be of type decimal at Col: " + colIterator + " Row: " + rowIterator);
                    //  }
                    //}

                  }
                }


              }
            }

          }

          CultureInfo culture = new CultureInfo("en-US");
          DateTime mydatatime;
          BeltViewModel beltViewModel;
          string Events = string.Empty;
          bool isColEmpty = false;
          string myStr = string.Empty;
          if (allErrors.Count == 0)
          {

            int curcol = 0;
            for (int u = 0; u < (noOfRow - 1); u++)
            {
              isColEmpty = false;
              //string query = "insert into " + tableName + " select '" + Guid.NewGuid() + "', ";
              string query = "insert into " + tableName + " (Name,Age,Weight,Gender,State,City,Address,Zip,Email,Phone,School,Belt,Event,RegistrationDate,DateCreated,DateModified,CreatedBy) Values (";
              int colNo = 1;
              for (int o = colNo; o < noOfCol; o++)
              {
                curcol = u * 15 + colNo;
                //var dataType = tableType.FirstOrDefault(x => x.ColumnName.ToUpper() == allColumns[o].ToUpper()).DataType;
                //if (dataType == "nvarchar") query += "'" + Convert.ToString(allRows[colNo]).Replace("'", "''") + "' ,";
                //if (dataType == "bit") query += "CAST('" + allRows[colNo] + "' as bit) ,";
                //if (dataType == "datetime") query += "CAST('" + allRows[colNo] + "' as datetime) ,";
                //if (dataType == "int") query += "CAST('" + allRows[colNo] + "' as int) ,";
                //if (dataType == "decimal") query += "CAST('" + allRows[colNo] + "' as decimal(18,2)) ,";

                if (/*allColumns[o] == "Serial" || allColumns[o] == "Id" ||*/ allColumns[o] == "Age" || allColumns[o] == "Belt" || allColumns[o] == "Weight" || allColumns[o] == "Registration Date" || allColumns[o] == "Gender" || allColumns[o] == "Event(s)")
                {
                  //if (allColumns[o] == "Serial")
                  //{
                  //  string result = Regex.Match(allColumns[o], @"\d+").Value;

                  //  query += "CAST('" + result + "' as decimal(18,0)), ";
                  //}
                  //if (allColumns[o] == "Id")
                  //{
                  //  string result = Regex.Replace(allRows[colNo], @"[^\d]", "");

                  //  query += "CAST('" + result + "' as decimal(18,0)), ";
                  //}
                  if (allColumns[o] == "Age")
                  {
                    //if (string.IsNullOrEmpty(allRows[curcol]) == true) { isColEmpty = true; break; };

                    string result = Regex.Replace(allRows[curcol], @"[^\d]", "");

                    query += "CAST('" + result + "' as decimal(18,0)), ";
                  }
                  if (allColumns[o] == "Weight")
                  {
                    //if (string.IsNullOrEmpty(allRows[curcol]) == true) { isColEmpty = true; break; };
                    string result = Regex.Replace(allRows[curcol], @"[^\d]", "");

                    query += "CAST('" + result + "' as decimal(18,0)), ";
                  }

                  if (allColumns[o] == "Registration Date")
                  {
                    //if (string.IsNullOrEmpty(allRows[curcol]) == true) { isColEmpty = true; break; };

                    //query += "CAST('" + result + "' as datetime), ";
                    //mydatatime = ParseDate(allRows[curcol]);
                    //mydatatime = Convert.ToDateTime(allRows[curcol], new CultureInfo("en-US"));
                    //query += "'" + mydatatime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', ";
                    query += "'" + allRows[curcol] + "', ";

                  }

                  if (allColumns[o] == "Belt")
                  {
                    beltViewModel = beltsCollection.FirstOrDefault(x => x.Name.ToLower() == allRows[curcol].ToLower());
                    //if (beltViewModel == null) { isColEmpty = true; break; };
                    query += "'" + beltViewModel.Id + "', ";
                  }
                  if (allColumns[o] == "Event(s)")
                  {
                    Events = allRows[curcol];
                    //if (string.IsNullOrEmpty(Events) == true) { isColEmpty = true; break; };
                    query += "'" + allRows[curcol] + "', ";
                  }

                  if (allColumns[o] == "Gender")
                  {
                    if (allRows[curcol].ToLower() == "male")
                    {
                      query += "CAST('" + 1 + "' as bit), ";
                    }
                    if (allRows[curcol].ToLower() == "female")
                    {
                      query += "CAST('" + 0 + "' as bit), ";
                    }
                  }

                }
                else
                {
                  myStr = allRows[curcol].Replace("'", "''");
                  query += "'" + myStr + "', ";
                }


                ++colNo;
              }


              //if (!isColEmpty)
              //{


              //query += "convert(datetime,'" + DateTime.Now + "',5)" + ", " + "convert(datetime,'" + DateTime.Now + "',5)" + ",";
              query += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', " + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' ,";
              query += "'"+User.Identity.GetUserId() + "'),";

              //query += "CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' as DATETIME), " + "CAST('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' as DATETIME),";
              //query += "0";
              //Inserting into DB
              query = query.Trim().TrimEnd(',');
              try
              {
                using (SqlConnection con = new SqlConnection(myConnectionString))

                using (SqlCommand cmnd = new SqlCommand(query, con))
                {
                  SqlDataAdapter adapter = new SqlDataAdapter(cmnd);
                  adapter.SelectCommand.CommandType = CommandType.Text;

                  DataTable dtble = new DataTable();
                  adapter.Fill(dtble);

                }
              }
              catch (Exception ex)
              {
                throw ex;
              }

              try
              {

                string[] EventsList = Events.Split(new[] { "<br>" }, StringSplitOptions.None);
                TournamentEvent tournamentEvent;
                foreach (string evnt in EventsList)
                {
                  tournamentEvent = db.TournamentEvents.FirstOrDefault(x => x.CreatedBy == userId && x.Name.Trim().ToLower() == evnt.Trim().ToLower());
                  if (tournamentEvent != null)
                  {
                    decimal maxId = db.Competitors.Max(g => g.Id);
                    int? groupNo = null;
                    var competitor = db.Competitors.Find(maxId);
                    if (competitor != null)
                    {
                      var eventDivision = new List<decimal>();
                      int.TryParse(competitor.Belt, out var belt);
                      int gender = competitor.Gender == true ? 1 : 0;
                      var tournamentEventDivision = eventDivisionInfos.Where(x => x.TournamentEventId == tournamentEvent.Id).ToList();
                      foreach (var eD in tournamentEventDivision)
                      {
                        int totalCriteria = 0;
                        int totalMatchedCriteria = 0;
                        if (eD.FromAge > 0 && eD.ToAge > 0)
                        {
                          totalCriteria++;
                          if (competitor.Age >= eD.FromAge && competitor.Age <= eD.ToAge)
                          {
                            totalMatchedCriteria++;
                          }
                        }

                        if (eD.FromBelt > 0 && eD.ToBelt > 0)
                        {
                          totalCriteria++;
                          if (belt >= eD.FromBelt && belt <= eD.ToBelt)
                          {
                            totalMatchedCriteria++;
                          }
                        }

                        if (eD.FromWeight > 0 && eD.ToWeight > 0)
                        {
                          totalCriteria++;
                          if (competitor.Weight >= eD.FromWeight && competitor.Weight <= eD.ToWeight)
                          {
                            totalMatchedCriteria++;
                          }
                        }

                        if (eD.Gender < 3)
                        {
                          totalCriteria++;
                          if (eD.Gender == gender || eD.Gender == 2)
                          {
                            totalMatchedCriteria++;
                          }
                        }

                        if (totalCriteria == totalMatchedCriteria)
                        {
                          eventDivision.Add(eD.Id);
                        }
                      }

                      if (eventDivision != null && eventDivision.Count > 0)
                      {
                        foreach (var ed in eventDivision.Distinct())
                        {
                          groupNo = 1;
                          query = "insert into DivisionGroup (CompetitorId,TournamentEventId, EventDevisionId, GroupNo) Values (" + maxId + ", " + tournamentEvent.Id + ", " + ed + ", " + groupNo + ")";
                          using (SqlConnection con = new SqlConnection(myConnectionString))

                          using (SqlCommand cmnd = new SqlCommand(query, con))
                          {
                            SqlDataAdapter adapter = new SqlDataAdapter(cmnd);
                            adapter.SelectCommand.CommandType = CommandType.Text;

                            DataTable dtble = new DataTable();
                            adapter.Fill(dtble);

                          }
                        }
                      }
                      else
                      {
                        query = "insert into DivisionGroup (CompetitorId,TournamentEventId) Values (" + maxId + ", " + tournamentEvent.Id + ")";
                        using (SqlConnection con = new SqlConnection(myConnectionString))

                        using (SqlCommand cmnd = new SqlCommand(query, con))
                        {
                          SqlDataAdapter adapter = new SqlDataAdapter(cmnd);
                          adapter.SelectCommand.CommandType = CommandType.Text;

                          DataTable dtble = new DataTable();
                          adapter.Fill(dtble);

                        }
                      }
                    }
                    else
                    {
                      query = "insert into DivisionGroup (CompetitorId,TournamentEventId) Values (" + maxId + ", " + tournamentEvent.Id + ")";
                      using (SqlConnection con = new SqlConnection(myConnectionString))

                      using (SqlCommand cmnd = new SqlCommand(query, con))
                      {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmnd);
                        adapter.SelectCommand.CommandType = CommandType.Text;

                        DataTable dtble = new DataTable();
                        adapter.Fill(dtble);

                      }
                    }

                  }
                }



              }
              catch (Exception ex)
              {
                throw ex;
              }

              //}



            }
          }


          return Json(new { allColumns = allColumns, allRows = allRows, allErrors = allErrors, noOfRows = noOfRow }, JsonRequestBehavior.AllowGet);

        }
        catch (Exception ex)
        {
          return Json("Error occurred. Error details: " + ex.Message);
        }
      }
      else
      {
        return Json("No file selected.");
      }
    }
    //public static DateTime ParseDate(string input)
    //{
    //  DateTime result;

    //  if (DateTime.TryParseExact(input, "yy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out result)) return result;
    //  if (DateTime.TryParseExact(input, "dd/MM/yy HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out result)) return result;

    //  throw new FormatException();
    //}

    public class TableType
    {
      public string ColumnName { get; set; }
      public string DataType { get; set; }
    }

    [AllowAnonymous]
    public async Task<ActionResult> Athletes(string searchString)
    {
      List<Competitor> competitor = await db.Competitors.ToListAsync();
      string userId = User.Identity.GetUserId();
      var Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();

      if (searchString != null)
      {
        competitor = competitor.Where(x => x.Name.ToLower().ToString().Contains(searchString.ToLower().ToString())).ToList();
      }

      ViewBag.CurrentFilter = searchString;
      for (int i = 0; i < competitor.Count; i++)
      {
        competitor[i].Belt = Belts.FirstOrDefault(x => x.BeltId == Convert.ToInt32(competitor[i].Belt))?.BeltName;
      }
      return View(competitor);
    }


    [Authorize]
    public JsonResult GetProfile()
    {
      var CurrentUserId = User.Identity.GetUserId();
      var Data = db.Competitors.Where(x => x.CreatedBy == CurrentUserId).Take(2).Select(x =>
      new
      {
        Id = x.Id,
        CreatedBy = x.CreatedBy,
        Name = x.Name,
        Profile = x.ProfilePath

      }).OrderBy(x => x.Id).ToList();
      return Json(Data, JsonRequestBehavior.AllowGet);
    }

    [Authorize]
    public async Task<JsonResult> QRCodeEventPassDetails(int EventId, int CompetitorId)
    {

      Random rnd = new Random();
      int randomNumber = rnd.Next(10000000, 99999999);

      bool CheckQRCodeExits = db.QRCodeDetails.Any(x => x.EventId == EventId && x.CompetitorId == CompetitorId);

      var EventName = await db.ScheduledEvents.Where(x => x.Id == EventId).Select(x => x.Name).FirstOrDefaultAsync();
      string userId = User.Identity.GetUserId();
      var data = await db.Competitors.Where(x => x.Id == CompetitorId).Select(o => new
      {
        Profile = o.ProfilePath,
        Name = o.Name,
        Age = o.Age,
        Belt = db.Belts.FirstOrDefault(y => y.CreatedBy == userId && y.BeltId.ToString() == o.Belt).BeltName,
        Weight = o.Weight,
        Gender = o.Gender,
        EventName = EventName,
        QRCodeId = CheckQRCodeExits ? db.QRCodeDetails.Where(x => x.EventId == EventId && x.CompetitorId == CompetitorId).FirstOrDefault().QRCodeId : "#" + randomNumber.ToString()
      }).FirstOrDefaultAsync();


      if (data != null)
      {


        if (!CheckQRCodeExits)
        {
          QRCodeDetail obj = new QRCodeDetail();
          obj.QRCodeId = "#" + randomNumber.ToString();
          obj.EventId = EventId;
          obj.CompetitorId = CompetitorId;
          db.QRCodeDetails.Add(obj);
          await db.SaveChangesAsync();
        }
      }
      return Json(data, JsonRequestBehavior.AllowGet);

    }


    [Authorize]
    public async Task<ActionResult> ProfileSetting(string Id)
    {

      // var data = await db.Competitors.Where(x => x.CreatedBy == Id && x.IsDefault == true).FirstOrDefaultAsync();

      var data = await db.Competitors.Where(x => x.Id.ToString() == Id).FirstOrDefaultAsync();
      string userId = User.Identity.GetUserId();
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      return View(data);
    }

    public async Task<ActionResult> GetProfileById(string Id)
    {
      db.Configuration.ProxyCreationEnabled = false;
      var data = await db.Competitors.Where(x => x.Id.ToString() == Id).FirstOrDefaultAsync();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    [Authorize]
    public async Task<ActionResult> MyAccount(int id)
    {
      var EventIds = await db.DivisionGroups.Where(x => x.CompetitorId == id).Select(x => x.TournamentId).ToListAsync();

      var EventList = await db.ScheduledEvents.Where(x => EventIds.Any(c => c == x.Id && x.EndDate >= DateTime.Now)).ToListAsync();

      ViewBag.EventList = EventList;

      var Data = await db.Competitors.Where(x => x.Id == id).FirstOrDefaultAsync();

      //var _User = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(Data.CreatedBy);

      //await HttpContext.GetOwinContext().GetUserManager<ApplicationSignInManager>().SignInAsync(_User, false, false);

      return View(Data);

      //return View(await db.Competitors.Where(x => x.Id == id).FirstOrDefaultAsync());

    }

    [Authorize]
    public async Task<ActionResult> UserProfile(string Id)
    {
      //  return View(await db.Competitors.Where(x => x.CreatedBy == Id && x.IsDefault==true).FirstOrDefaultAsync());
      var data = await db.Competitors.Where(x => x.Id.ToString() == Id).FirstOrDefaultAsync();
      string userId = User.Identity.GetUserId();
      data.Belt = db.Belts.FirstOrDefault(x => x.CreatedBy == userId && x.BeltId.ToString() == data.Belt)?.BeltName;
      return View(data);
    }

    [Authorize]
    public async Task<ActionResult> Profile(int? id)
    {
      var CurrentUserId = User.Identity.GetUserId();
      var data = new List<Competitor>();
      if (id != null && id > 0)
      {
        var existingCompetitors = db.DivisionGroups.Where(x => x.TournamentId == id).Select(y => y.CompetitorId).ToList();
        data = db.Competitors.Where(x => !existingCompetitors.Contains(x.Id) && x.CreatedBy == CurrentUserId).ToList();
      }
      else
      {
        data = db.Competitors.Where(x => x.CreatedBy == CurrentUserId).ToList();
      }
      
      return View(data);
    }

    [AllowAnonymous]
    public ActionResult AddProfile()
    {
      ViewBag.recaptchaKey = db.Configurations.Where(x => x.Key == "RecaptchaKey").FirstOrDefault().Value;
      return View();
    }


    [Authorize]
    public async Task<ActionResult> AddInfo()
    {
      string userId = User.Identity.GetUserId();
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      ViewBag.Email = db.AspNetUsers.FirstOrDefault(x => x.Id == userId)?.Email;
      return View();
    }

    [Authorize]
    public async Task<ActionResult> UpdateInfo()
    {
      string userId = User.Identity.GetUserId();
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      ViewBag.Email = db.AspNetUsers.FirstOrDefault(x => x.Id == userId)?.Email;
      var data = db.Competitors.FirstOrDefault(x => x.CreatedBy == userId && x.IsDefault == true);
      return View(data);
    }

    [Authorize]
    public async Task<ActionResult> AddMember(int? ProfileId)
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

      var existingData = db.Competitors.AsNoTracking().FirstOrDefault(x => x.CreatedBy == userId && x.IsDefault == true);
      if (existingData == null) return RedirectToAction("AddInfo", "Competitors");
      

      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      ViewBag.Id = (ProfileId != null ? ProfileId : 0);
     return View();
      
    }

    [Authorize]
    [HttpPost]
    public async Task<JsonResult> AddUpdateMember([System.Web.Http.FromBody] Competitor competitor)
    {
      string userId = User.Identity.GetUserId();
      competitor.CreatedBy = userId;
      competitor.DateModified = DateTime.Now;

      var existingData = db.Competitors.AsNoTracking().FirstOrDefault(x => x.CreatedBy == userId && x.IsDefault==true);
      competitor.Address = existingData.Address;
      competitor.Country = existingData.Country;
      competitor.State = existingData.State;
      competitor.City = existingData.City;
      competitor.Zip = existingData.Zip;
      competitor.Phone = existingData.Phone;
      competitor.Email = existingData.Email;
      competitor.IsDefault = existingData.IsDefault;
      competitor.Event = existingData.Event;
      competitor.IsCheckin = existingData.IsCheckin;
      competitor.RegistrationDate = existingData.RegistrationDate;
      competitor.Remarks = existingData.Remarks;
      competitor.Path = existingData.Path;
      competitor.Serial = existingData.Serial;
      if (string.IsNullOrEmpty(competitor.ProfilePath)) competitor.ProfilePath = existingData.ProfilePath;

      var newCompetitor = competitor;

     if(newCompetitor.Id == 0)
      {
        db.Competitors.Add(newCompetitor);
      }
      else
      {
        db.Entry(newCompetitor).State = EntityState.Modified;
      }
      await db.SaveChangesAsync();
      return Json(new { success = true, message = "Update Successfully", JsonRequestBehavior.AllowGet });

    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<JsonResult> AddProfile([System.Web.Http.FromBody] Competitor competitor)
    {
      competitor.CreatedBy = User.Identity.GetUserId();
      if (!db.Competitors.Any(x => x.CreatedBy == competitor.CreatedBy)) competitor.IsDefault = true;
      db.Competitors.Add(competitor);
      await db.SaveChangesAsync();
      return Json(new { success = true, message = "Saved Successfully", competitorId=competitor.Id, JsonRequestBehavior.AllowGet });

    }  
    
    [HttpPost]
    public async Task<JsonResult> UpdateInfoProfile([System.Web.Http.FromBody] Competitor competitor)
    {
      var userId = db.Competitors.FirstOrDefault(x => x.Id == competitor.Id)?.CreatedBy;
      var competList = db.Competitors.Where(x => x.CreatedBy == userId).ToList();
      for (int i = 0; i < competList.Count; i++)
      {
        var currCompet = competList[i];
        currCompet.Address = competitor.Address;
        currCompet.Country = competitor.Country;
        currCompet.State = competitor.State;
        currCompet.City = competitor.City;
        currCompet.Zip = competitor.Zip;
        currCompet.Phone = competitor.Phone;

        db.Entry(currCompet).State = EntityState.Modified;
        db.SaveChanges();
      }

      return Json(new { success = true, message = "Updated Successfully", competitorId=competitor.Id, JsonRequestBehavior.AllowGet });

    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<JsonResult> UpdateProfile([System.Web.Http.FromBody] Competitor competitor)
    {
      competitor.CreatedBy = User.Identity.GetUserId();
      competitor.DateModified = DateTime.Now;
      db.Entry(competitor).State = EntityState.Modified;
      await db.SaveChangesAsync();
      return Json(new { success = true, message = "Update Successfully", JsonRequestBehavior.AllowGet });

    }


    [Authorize]
    [HttpPost]
    public async Task<JsonResult> CheckCutOffDate(int Id)
    {

      var EventId = db.DivisionGroups.Where(x => x.CompetitorId == Id).FirstOrDefault()?.TournamentId;

      var Events = db.ScheduledEvents.Where(x => x.Id == EventId).FirstOrDefault();

      if (Events != null && Events.CutOffdate < DateTime.Now && DateTime.Now < Events.EndDate)
      {

        var messsage = "Your profile is locked due to Event" + "(" + Events.Name + ")" + " until " + "(" + Events.EndDate.Value.ToString("yyyy-MM-dd") + ").";

        return Json(new { success = true, message = messsage, JsonRequestBehavior.AllowGet });

      }

      return Json(new { success = false, message = "", JsonRequestBehavior.AllowGet });


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
