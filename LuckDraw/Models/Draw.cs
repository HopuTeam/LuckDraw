using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    //抽奖表
    public class Draw
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int SignID { get; set; }
        public int OptionID { get; set; }
    }
}