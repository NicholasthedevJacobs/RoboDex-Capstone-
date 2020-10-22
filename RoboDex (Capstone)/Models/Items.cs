using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_.Models
{
    public class Items
    {
        [Key]
        public int ItemId { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        [ForeignKey("Tags")]
        public int TagId { get; set; }
        public string IMGUrl { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime TimeAdded { get; set; }

        public Items()
        {
            TimeAdded = DateTime.Now;
        }

    }
}
