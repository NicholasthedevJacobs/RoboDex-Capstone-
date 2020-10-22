using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class ItemTags
    {
        [Key, Column(Order = 1)]
        public int ItemId { get; set; }
        public Items Item { get; set; }

        [Key, Column(Order = 2)]
        public int TagsId { get; set; }
        public Tags Tag { get; set; }

        
    }
}
