using System;
using System.ComponentModel.DataAnnotations;

namespace LuckDraw.Models
{
    public class LuckDraw
    {
        [Key]
        public int ID { get; set; }
        public int LuckID { get; set; }
        public int DrawID { get; set; }
        public int Number { get; set; }
        public DateTime? EntryTime { get; set; }
    }
}