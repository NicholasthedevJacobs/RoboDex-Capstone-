using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class Inbox
    {
        [Key]
        public int Id { get; set; }   
        public int InboxId { get; set; }
        public string Message { get; set; }
        public bool isRead { get; set; }
        [ForeignKey("Items")]
        public int ItemId { get; set; }

    }
}
