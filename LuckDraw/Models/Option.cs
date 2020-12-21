using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    //方式表
    public class Option
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}