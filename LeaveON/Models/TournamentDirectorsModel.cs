using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class TournamentDirectorsModel
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string StripeKey { get; set; }
    public string AccountHolderName { get; set; }
    public string AccountNumber { get; set; }
    public string RoutingNumber { get; set; }
    public DateTime DateModified { get; set; }
    public DateTime DateCreated { get; set; }

    public HttpPostedFileBase ImageFile { get; set; }

  }
}

