using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class Inbox
    {
        [Key]
        public int InboxId { get; set; }
        public string Message { get; set; }

    }
}
