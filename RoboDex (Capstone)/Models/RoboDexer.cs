using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class RoboDexer
    {
        [Key]
        public int RoboDexerId { get; set; }
        [ForeignKey("Inbox")]
        public int InboxId { get; set; }
        [ForeignKey("Inventory")]
        public int InventoryId{ get; set; }
        [ForeignKey("ShoppingCart")]
        public int ShoppingCartId { get; set; }
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
