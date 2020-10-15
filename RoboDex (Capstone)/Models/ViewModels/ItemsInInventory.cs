using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models.ViewModels
{
    public class ItemsInInventory
    {
        public List<Items> AllItems { get; set; }
        public List<Inventory> Inventory { get; set; }
    }
}
