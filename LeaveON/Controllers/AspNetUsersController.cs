using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using LeaveON.Models;
using Microsoft.AspNet.Identity.EntityFramework;
//using InventoryRepo.Models;
using TourneyRepo.Models;

namespace LeaveON.Controllers
{

  [Authorize(Roles = "Admin")]
  public class AspNetUsersController : Controller
  {
    //private LeaveONEntities db = new LeaveONEntities();
    //private jsaosorioEntities db = new jsaosorioEntities();
    //private InventoryPortalEntities db = new InventoryPortalEntities();
    private TourneyWizardEntities db = new TourneyWizardEntities();
    // GET: AspNetUsers
    public ActionResult Index()
    {
      //var ab = db.AspNetUsers.Count();
      //List<AspNetUser> aspNetUsers = db.AspNetUsers;//.Include(a => a.Department);
      var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
      List<AspNetUser2> LstAspNetUsers2 = new List<AspNetUser2>();
      foreach (AspNetUser aspNetUser1 in db.AspNetUsers)
      {
        //var user = userManager.FindByIdAsync(user1.Id);
        string rolename = userManager.GetRoles(aspNetUser1.Id).FirstOrDefault();

        LstAspNetUsers2.Add(new AspNetUser2 { Id = aspNetUser1.Id, Email = aspNetUser1.Email, Type = rolename });
      }
      //.Where(x=>x.Email== "ashir.aslam@tricast.com")
      return View(LstAspNetUsers2);
    }

    // GET: AspNetUsers/Details/5
    public async Task<ActionResult> Details(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
      if (aspNetUser == null)
      {
        return HttpNotFound();
      }
      return View(aspNetUser);
    }

    // GET: AspNetUsers/Create
    public ActionResult Create()
    {
      //ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name");
      return View();
    }

    // POST: AspNetUsers/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Hometown,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,DateCreated,DateModified,Remarks,DepartmentId")] AspNetUser aspNetUser)
    {
      if (ModelState.IsValid)
      {
        db.AspNetUsers.Add(aspNetUser);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      //ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", aspNetUser.DepartmentId);
      return View(aspNetUser);
    }

    // GET: AspNetUsers/Edit/5
    public async Task<ActionResult> Edit(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
      if (aspNetUser == null)
      {
        return HttpNotFound();
      }
      //ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", aspNetUser.CountryId);
      //ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", aspNetUser.DepartmentId);
      //ViewBag.UserLeavePolicyId = new SelectList(db.UserLeavePolicies, "Id", "Description", aspNetUser.UserLeavePolicyId);


      List<SelectListItem> UserTypes = new List<SelectListItem>()
      {
          new SelectListItem{Text = "Admin", Value = "1"},
          //new SelectListItem{Text = "Manager", Value = "2"},
          new SelectListItem{Text = "User", Value = "3"}
      };

      //ViewBag.UserTypes = UserTypes;
      var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));


      string RoleId = "3";
      if (userManager.IsInRole(id, "Admin"))
      {
        RoleId = "1";
      }
      else if (userManager.IsInRole(id, "Manager"))
      {
        RoleId = "2";
      }
      else if (userManager.IsInRole(id, "User"))
      {
        RoleId = "3";
      }

      ViewBag.UserTypesWithSelected = new SelectList(UserTypes, "Value", "Text", RoleId);


      return View(aspNetUser);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(string UserType, [Bind(Include = "Id,Hometown,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,DateCreated,DateModified,Remarks,DepartmentId,CountryId,UserLeavePolicyId,BioStarEmpNum")] AspNetUser aspNetUser)//,int CountryId)
    {
      aspNetUser.DateModified = DateTime.Now;

      if (ModelState.IsValid)
      {

        db.AspNetUsers.Attach(aspNetUser);
        db.Entry(aspNetUser).Property(x => x.Email).IsModified = false;
        db.Entry(aspNetUser).Property(x => x.UserName).IsModified = false;
        db.Entry(aspNetUser).Property(x => x.DateModified).IsModified = true;
        db.Entry(aspNetUser).Property(x => x.Remarks).IsModified = false;
        db.Entry(aspNetUser).Property(x => x.DepartmentId).IsModified = false;
        db.Entry(aspNetUser).Property(x => x.CountryId).IsModified = false;
        db.Entry(aspNetUser).Property(x => x.UserLeavePolicyId).IsModified = false;
        db.Entry(aspNetUser).Property(x => x.BioStarEmpNum).IsModified = false;

        await db.SaveChangesAsync();

        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        //bool trueOrFalse = userManager.IsInRole(id, "Admin");

        await userManager.RemoveFromRoleAsync(aspNetUser.Id, "Admin");
        await userManager.RemoveFromRoleAsync(aspNetUser.Id, "Manager");
        await userManager.RemoveFromRoleAsync(aspNetUser.Id, "User");

        //switch (UserType)
        //{
        //  case "1"://Admin
        //    await userManager.RemoveFromRoleAsync(aspNetUser.Id, "Admin");
        //    await userManager.RemoveFromRoleAsync(aspNetUser.Id, "Manager");
        //    await userManager.RemoveFromRoleAsync(aspNetUser.Id, "User");
        //    break;
        //case "2"://Manager
        //  await userManager.RemoveFromRoleAsync(aspNetUser.Id, "Manager");
        //  await userManager.RemoveFromRoleAsync(aspNetUser.Id, "User");
        //  break;
        //case "3"://User
        //  await userManager.RemoveFromRoleAsync(aspNetUser.Id, "User");
        //  break;
        //}
        switch (UserType)
        {
          case "1"://Admin
            await userManager.AddToRoleAsync(aspNetUser.Id, "Admin");
            await userManager.AddToRoleAsync(aspNetUser.Id, "Manager");
            await userManager.AddToRoleAsync(aspNetUser.Id, "User");
            break;
          case "2"://Manager
            await userManager.AddToRoleAsync(aspNetUser.Id, "Manager");
            await userManager.AddToRoleAsync(aspNetUser.Id, "User");
            break;
          case "3"://User
            await userManager.AddToRoleAsync(aspNetUser.Id, "User");
            break;
        }

        return RedirectToAction("Index");
      }
      //ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", aspNetUser.DepartmentId);
      return View(aspNetUser);
    }

    //public async Task<ActionResult> ResetPasword(string id)
    //{
    //  return View();
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> ResetPassword(string id)
    //{
    //  return RedirectToAction("Index");
    //}


    //
    // GET: /Account/ResetPassword
    //[AllowAnonymous]
    public ActionResult ResetPassword(string code)
    {
      return code == null ? View("Error") : View();
    }

    //
    // POST: /Account/ResetPassword

    //[AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ResetPassword2ViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
      //var userFound= userManager.FindById(model.Code);
      //var user = await userManager.FindByNameAsync(userFound.Email);
      //---
      //if (user == null)
      //{
      //// Don't reveal that the user does not exist
      ////return RedirectToAction("ResetPasswordConfirmation", "Account");
      //}
      //--
      //var result = await userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
      //if (result.Succeeded)
      //{
      //  return RedirectToAction("ResetPasswordConfirmation", "Account");
      //}
      //AddErrors(result);
      //-**************
      //AspNetUser aspNetUser = db.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault();
      //aspNetUser.PasswordHash = userManager.PasswordHasher.HashPassword(model.Password);
      //aspNetUser.SecurityStamp = Guid.NewGuid().ToString();
      //aspNetUser.DateModified = DateTime.Now;
      //db.AspNetUsers.Attach(aspNetUser);
      //db.Entry(aspNetUser).Property(x => x.PasswordHash).IsModified = true;
      //db.Entry(aspNetUser).Property(x => x.SecurityStamp).IsModified = true;
      //db.Entry(aspNetUser).Property(x => x.DateModified).IsModified = true;
      //await db.SaveChangesAsync();
      //******************
      userManager.RemovePassword(model.Code);

      userManager.AddPassword(model.Code, model.Password);


      return RedirectToAction("Index");
    }


    // POST: AspNetUsers/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> Edit([Bind(Include = "Id,Hometown,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,DateCreated,DateModified,Remarks,DepartmentId,CountryId,UserLeavePolicyId,BioStarEmpNum")] AspNetUser aspNetUser)//,int CountryId)
    //{
    //  aspNetUser.DateModified = DateTime.Now;

    //  if (ModelState.IsValid)
    //  {
    //    //db.Entry(aspNetUser).State = EntityState.Modified;
    //    db.AspNetUsers.Attach(aspNetUser);

    //    db.Entry(aspNetUser).Property(x => x.Email).IsModified = true;
    //    db.Entry(aspNetUser).Property(x => x.UserName).IsModified = true;
    //    db.Entry(aspNetUser).Property(x => x.DateModified).IsModified = true;
    //    db.Entry(aspNetUser).Property(x => x.Remarks).IsModified = true;
    //    db.Entry(aspNetUser).Property(x => x.DepartmentId).IsModified = true;
    //    db.Entry(aspNetUser).Property(x => x.CountryId).IsModified = true;
    //    db.Entry(aspNetUser).Property(x => x.UserLeavePolicyId).IsModified = true;
    //    db.Entry(aspNetUser).Property(x => x.BioStarEmpNum).IsModified = true;
    //    //UserLeavePolicyId


    //    await db.SaveChangesAsync();
    //    return RedirectToAction("Index");
    //  }
    //  ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", aspNetUser.DepartmentId);
    //  return View(aspNetUser);
    //}


    // GET: AspNetUsers/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
      if (aspNetUser == null)
      {
        return HttpNotFound();
      }
      return View(aspNetUser);
    }

    // POST: AspNetUsers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteConfirmed(string id)
    {
      AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
      db.AspNetUsers.Remove(aspNetUser);
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
