using System;
using System.Collections.Generic;
using System.Linq;
using LeaveON.UtilityClasses;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(LeaveON.Startup))]

namespace LeaveON
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      ConfigureAuth(app);
      //new ScheduledTasks().InitTimerForScheduleTasks();
      //reset.LeavePolicyValues();
      app.UseCookieAuthentication(new CookieAuthenticationOptions
      {
        ExpireTimeSpan = TimeSpan.FromDays(7),
        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        LoginPath = new PathString("/Account/Login")
      });
    }

  }
}
