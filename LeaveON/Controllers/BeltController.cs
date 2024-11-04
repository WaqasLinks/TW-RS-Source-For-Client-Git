using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TourneyRepo.Models;

namespace LeaveON.Controllers
{
  [System.Web.Mvc.Authorize(Roles = "Admin,Manager,TournamentDirector")]
  public class BeltController : Controller
  {
    private TourneyWizardEntities db = new TourneyWizardEntities();
    public ActionResult Index()
    {
      string userId = User.Identity.GetUserId();
      var data = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      return View(data);
    }
    public async Task<ActionResult> AddBelt([FromBody] List<Belt> belts)
    {
      string userId = User.Identity.GetUserId();
      var existingRecords = db.Belts.Where(x => x.CreatedBy == userId).ToList();
      if (existingRecords.Count > 0) {
        db.Belts.RemoveRange(existingRecords);
        db.SaveChanges();
      }
      belts.ForEach(item => item.CreatedBy = userId);
      db.Belts.AddRange(belts);
      await db.SaveChangesAsync();
      return Json(new { success = true, message = "Saved Successfully", JsonRequestBehavior.AllowGet });
    }
  }
}
