using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models.ViewModels
{
    public class ShoppingCartItemsDetails
    {
        public ShoppingCart ShoppingCart { get; set; }
        public Items Items { get; set; }
        public RoboDexer RoboDexer { get; set; }

    }
}
