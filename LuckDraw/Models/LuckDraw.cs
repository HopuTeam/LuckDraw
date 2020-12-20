using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    public class LuckDraw
    {
        [Key]
        public int ID { get; set; }
        public int LuckID { get; set; }
        public int DrawID { get; set; }
        public int? Number { get; set; }
        public DateTime? EntryTime { get; set; }

        [ForeignKey("LuckID")]
        public Luck Luck { get; set; }

        [ForeignKey("DrawID")]
        public Draw Draw { get; set; }
    }
}
