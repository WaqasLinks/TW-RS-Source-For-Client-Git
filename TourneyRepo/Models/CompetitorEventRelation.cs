//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TourneyRepo.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CompetitorEventRelation
    {
        public int Id { get; set; }
        public Nullable<int> EventId { get; set; }
        public Nullable<int> CompetitorId { get; set; }
        public Nullable<int> TournamentId { get; set; }
    
        public virtual Competitor Competitor { get; set; }
        public virtual ScheduledEvent ScheduledEvent { get; set; }
        public virtual TournamentEvent TournamentEvent { get; set; }
    }
}
