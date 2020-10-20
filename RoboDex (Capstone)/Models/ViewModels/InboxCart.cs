using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models.ViewModels
{
    public class InboxCart
    {
        public Inbox Inbox { get; set; }
        public int cartId { get; set; }
        public int InboxId { get; set; }
        public int ItemId { get; set; }
    }
}
