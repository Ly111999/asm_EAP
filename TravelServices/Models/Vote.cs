//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TravelServices.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vote
    {
        public int id { get; set; }
        public int vote1 { get; set; }
        public int Post_id { get; set; }
        public int Traveler_id { get; set; }
    
        public virtual Post Post { get; set; }
        public virtual Traveler Traveler { get; set; }
    }
}
