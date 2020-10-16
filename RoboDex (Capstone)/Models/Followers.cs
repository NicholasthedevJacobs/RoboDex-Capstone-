using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class Followers
    {
        public int Id { get; set; }
        [ForeignKey("RoboDexer")]
        public int RoboDexerId { get; set; }
        [ForeignKey("RoboDexer")]
        public int FollowerId { get; set; }

        //Maybe Add UserName property here
    }
}
