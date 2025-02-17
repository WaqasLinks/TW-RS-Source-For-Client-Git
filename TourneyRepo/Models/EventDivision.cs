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
    
    public partial class EventDivision
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EventDivision()
        {
            this.DivisionGroups = new HashSet<DivisionGroup>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AgeRange { get; set; }
        public string BeltRange { get; set; }
        public string GenderRange { get; set; }
        public string WeightRange { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<int> TournamentId { get; set; }
        public Nullable<int> TournamentEventId { get; set; }
        public Nullable<bool> StrictCriteria { get; set; }
        public Nullable<int> GroupLimit { get; set; }
        public string Combinations { get; set; }
        public string RingAssignment { get; set; }
        public Nullable<bool> IsCustom { get; set; }
        public string CreatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DivisionGroup> DivisionGroups { get; set; }
        public virtual TournamentEvent TournamentEvent { get; set; }
    }
}
