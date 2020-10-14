using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class LocationPlace
    {
        [Key]
        public int LocationId { get; set; }
        public string MainLocation { get; set; }
        public string SecondaryLocation { get; set; }//make nullable

    }
}
