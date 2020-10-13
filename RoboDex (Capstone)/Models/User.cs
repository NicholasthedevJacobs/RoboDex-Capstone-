using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int Inbox { get; set; }
        public int Inventory{ get; set; }
        public int ShoppingCart { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AboutMe { get; set; }
        public string ContactInfo { get; set; }
        public string IMGUrl { get; set; }

        [ForeignKey("IdentityUser")]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }


    }
}
