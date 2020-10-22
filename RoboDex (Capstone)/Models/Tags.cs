using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class Tags
    {
        [Key]
        public int TagId { get; set; }
        public string Name { get; set; }
        public ICollection<ItemTags> ItemTags { get; set; }
    }
}
