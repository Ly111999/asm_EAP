using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelServices.Models
{
    public class BodyPostMe
    {
        public Post Post { get; set; }
        public List<Tag> Tag { get; set; }
    }
}