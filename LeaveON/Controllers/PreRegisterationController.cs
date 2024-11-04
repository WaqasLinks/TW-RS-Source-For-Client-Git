using LeaveON.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TourneyRepo.Models;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User,TournamentDirector")]
  public class PreRegisterationController : Controller
  {
    private TourneyWizardEntities db = new TourneyWizardEntities();
    // GET: PreRegisteration
    public ActionResult Index()
    {
      string userId = User.Identity.GetUserId();
      ViewBag.Belts = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      return View();
    }
    [HttpPost]
    public ActionResult GetFilteredData()
    {
      
      List<CompetitorChildViewModel> listobj = new List<CompetitorChildViewModel>();

      var School = (from C in db.Competitors
                    where C.School != null
                    select new
                    {
                      SchoolName = C.School,
                      CoachName = C.CoachName,
                    }).GroupBy(x => x.SchoolName)?.ToList();

      foreach (var item in School)
      {

        CompetitorChildViewModel obj = new CompetitorChildViewModel();

        var SchoolName = item.Select(x => x.SchoolName).FirstOrDefault();
        var CoachName = item.Select(x => x.CoachName).FirstOrDefault();


        var Child = (from C in db.Competitors
                     where C.School == SchoolName
                     select new CompetitorChild
                     {
                       Id = C.Id,
                       Name = C.Name,
                       Age = (int)C.Age,
                       Events = C.Event,
                       Belt = C.Belt,
                       School = C.School,
                       IsCheckin = (bool)((bool)C.IsCheckin == null ? false : C.IsCheckin),
                       Remarks = C.Remarks==null?"": C.Remarks,
                     }).ToList();



        obj.SchoolName = SchoolName;
        obj.CoachName = CoachName;
        obj.TotalCompititor = Child.Count();
        obj.competitors = Child.ToList();

        listobj.Add(obj);


      }

      return Json(listobj, JsonRequestBehavior.AllowGet);

    }

    [HttpPost]

    public async Task<ActionResult> UpdateCompititor(Competitor competitor, string FormatedDate, string[] EventDivisions)
    {
      if (!string.IsNullOrEmpty(FormatedDate))
      {
        var date = FormatedDate.Split('/');
        competitor.RegistrationDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(int.Parse(date[2])), int.Parse(date[0]), int.Parse(date[1])).ToString();
      }

      competitor.DateModified = DateTime.Now;

      Competitor obj = new Competitor();


      obj = db.Competitors.Where(x => x.Id == competitor.Id).FirstOrDefault();

      obj.Id = competitor.Id;
      obj.Name = competitor.Name;
      obj.Age = competitor.Age;
      obj.Belt = competitor.Belt;
      obj.IsCheckin = competitor.IsCheckin;
      obj.Remarks = competitor.Remarks;



      db.Entry(obj).State = EntityState.Modified;

      await db.SaveChangesAsync();



      return Json(new { success = true, message = "Saved Successfully", JsonRequestBehavior.AllowGet });
    }

    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      Competitor competitor = await db.Competitors.FindAsync(id);
      db.Competitors.Remove(competitor);
      await db.SaveChangesAsync();
      return Json(new { success = true, message = "Delete Successfully", JsonRequestBehavior.AllowGet });
    }



  }
}
