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
using LeaveON.Models;
using System.IO;
using System.Web.UI.WebControls;
using Stripe;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class TournamentDirectorsController : Controller
  {

    private TourneyWizardEntities db = new TourneyWizardEntities();


    [AllowAnonymous]
    public async Task<ActionResult> TournamentDirectors()
    {
      return View(await db.TournamentDirectors.ToListAsync());
    }
     
    [Authorize]
    public ActionResult AddTournamentDirectors()
    {
      string userId = User.Identity.GetUserId();
      ViewBag.User =  db.AspNetUsers.FirstOrDefault(x => x.Id == userId);
      ViewBag.recaptchaKey = db.Configurations.Where(x => x.Key == "RecaptchaKey").FirstOrDefault().Value;
      return View();
    }

    [AllowAnonymous]

    [HttpPost]
    public async Task<ActionResult> AddTournamentDirectors(TournamentDirectorsModel model)
    {
      var customerId = "";
      try

      {
        StripeConfiguration.ApiKey = db.Configurations.FirstOrDefault(x => x.Key.Equals("SiteOwnerStripeKey")).Value;
        var optionscus = new CustomerCreateOptions
        {
          Email =model.Email,
          Name = model.AccountHolderName
        };

        var serviceCus = new CustomerService();
        var customer = serviceCus.Create(optionscus);

        customerId=customer.Id; // Store this ID for future reference

        // Create TOken
        var optionsToken = new TokenCreateOptions
        {
          BankAccount = new TokenBankAccountOptions
          {
            Country = "US",
            Currency = "usd",
            AccountHolderName = model.AccountHolderName,
            AccountHolderType = "individual",

            AccountNumber = model.AccountNumber, //"000123456789",
            RoutingNumber = model.RoutingNumber //"110000000",
          },
        };
        var serviceToken = new TokenService();
        var token = serviceToken.Create(optionsToken);


        // Create Bank Account
        var options = new BankAccountCreateOptions
        {

          Source = token.Id,
        };

        var service = new BankAccountService();
        var res = service.Create(customerId, options);

        //Verify Bank Account
        var optionsVeri = new BankAccountVerifyOptions
        {
          Amounts = new List<long> { 32, 45 },
        };
        var serviceVeri = new BankAccountService();
        var verify = serviceVeri.Verify(
          customerId,
          res.Id,
          optionsVeri);
      }
      catch (Exception ex)
      {
        var service = new CustomerService();
        service.Delete(customerId);

        return Json(new { success = false, message = ex.Message });
      }
     

        if (model.ImageFile != null && model.ImageFile.ContentLength > 0)
        {
          // Process the uploaded image file
          var fileName = Path.GetFileName(model.ImageFile.FileName);
          var path = Path.Combine(Server.MapPath("~/Images"), fileName);
          model.ImageFile.SaveAs(path);
        }
       
        TournamentDirector obj = new TournamentDirector();

      var UserId = User.Identity.GetUserId();

      var result = db.TournamentDirectors.FirstOrDefault(x => x.UserId == UserId);

        obj.Id =model.Id;
        obj.Address = model.Address;
        obj.DateCreated = DateTime.Now;
        obj.City = model.City;
        obj.Country = model.Country;
        obj.Email = model.Email;
        obj.FirstName = model.FirstName;
        obj.LastName = model.LastName;
        obj.StripeKey = model.StripeKey;
        obj.Zip = model.Zip;
        obj.Phone = model.Phone;
        obj.StripeCustomerId = customerId;
        obj.UserId= User.Identity.GetUserId();
        obj.Profile = "http://gsmartin67-001-site1.ctempurl.com/Images/" + model.ImageFile?.FileName;

     

      if (result!=null)
      {

        TournamentDirector Newobj = new TournamentDirector();

        Newobj.Id = result.Id;
        Newobj.Address = model.Address;
        Newobj.DateCreated = DateTime.Now;
        Newobj.City = model.City;
        Newobj.Country = model.Country;
        Newobj.Email = model.Email;
        Newobj.FirstName = model.FirstName;
        Newobj.LastName = model.LastName;
        Newobj.StripeKey = model.StripeKey;
        Newobj.Zip = model.Zip;
        Newobj.Phone = model.Phone;
        Newobj.StripeCustomerId = customerId;
        Newobj.UserId = User.Identity.GetUserId();
        Newobj.Profile = "https://localhost:44380/Images/" + model.ImageFile.FileName;

        db.Entry(result).State = EntityState.Detached; 
        
        db.TournamentDirectors.Attach(Newobj);

        db.Entry(Newobj).State = EntityState.Modified;

        db.SaveChanges();

      }
      else
      {
        await HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().AddToRoleAsync(User.Identity.GetUserId(), "TournamentDirector");
        db.TournamentDirectors.Add(obj);
        db.SaveChanges();

        var _User = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
        await HttpContext.GetOwinContext().GetUserManager<ApplicationSignInManager>().SignInAsync(_User, false, false);

      }
      return Json(new { success = true, message ="Add Successfully"});

    }


    public async Task<ActionResult> Index()
    {
      return View(await db.TournamentDirectors.ToListAsync());
    }

    // GET: TournamentDirectorss/Details/5
    public async Task<ActionResult> Details(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TournamentDirector tournamentDirector = await db.TournamentDirectors.FindAsync(id);
      if (tournamentDirector == null)
      {
        return HttpNotFound();
      }
      return View(tournamentDirector);
    }

    // GET: Tournaments/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: Tournaments/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public ActionResult Create(TournamentDirector tournamentDirector)
    {

        tournamentDirector.DateCreated = DateTime.Now;

        tournamentDirector.DateModified = DateTime.Now;

        db.TournamentDirectors.Add(tournamentDirector);

        db.SaveChanges();


        return RedirectToAction("Index");
     
    

    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> Create([Bind(Include = "Id,Name,DateCreated,DateModified,Age,Weight,Gender,State,City,Address,Zip,Email,Phone,School,Belt,Role")] TournamentDirector tournamentDirector)
    //{ 
    //  if (ModelState.IsValid)
    //  {
    //    db.TournamentDirectors.Add(tournamentDirector);
    //    await db.SaveChangesAsync();
    //    return RedirectToAction("Index");
    //  }

    //  return View(tournamentDirector);
    //}

    // GET: Tournaments/Edit/5
    public async Task<ActionResult> Edit(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TournamentDirector tournamentDirectors = await db.TournamentDirectors.FindAsync(id);

      if (tournamentDirectors == null)
      {
        return HttpNotFound();
      }
      //ViewBag.FormatedDate = tournament.TournamentDate.ToString("MM/dd/yy");
      return View(tournamentDirectors);
    }

    // POST: Tournaments/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Address,Zip,City,Country,Email,Phone,DateCreated,DateModified")] TournamentDirector tournamentDirectors)
    {
      //if (!string.IsNullOrEmpty(FormatedDate))
      //{
      //  var date = FormatedDate.Split('/');
      //  tournament.TournamentDate = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(int.Parse(date[2])), int.Parse(date[0]), int.Parse(date[1]));
      //}

      if (ModelState.IsValid)
      {
        db.Entry(tournamentDirectors).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(tournamentDirectors);
    }

    // GET: Tournaments/Delete/5
    public async Task<ActionResult> Delete(decimal id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      TournamentDirector tournamentDirector = await db.TournamentDirectors.FindAsync(id);
      if (tournamentDirector == null)
      {
        return HttpNotFound();
      }
      return View(tournamentDirector);
    }

    // POST: Tournaments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(decimal id)
    {
      TournamentDirector tournamentDirector = await db.TournamentDirectors.FindAsync(id);
      db.TournamentDirectors.Remove(tournamentDirector);
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
