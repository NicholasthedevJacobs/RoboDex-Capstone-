using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models.ViewModels
{
    public class ItemTagsLocation
    {
        public Items Items { get; set; }
        public List<Tags> Tags { get; set; }
        public Tags TagsInitial { get; set; }
        public string TagName { get; set; }
        public int TagId { get; set; }
        public LocationPlace LocationPlace { get; set; }
        public Inventory Inventory { get; set; }
        public RoboDexer RoboDexer { get; set; }
    }
}
