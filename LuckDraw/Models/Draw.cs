using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    //抽奖表
    public class Draw
    {
        public int ID{get;set;}
        public string Name{get;set;}
        public int SignID{get;set;}
        public int OptionID { get; set; }

        public List<LuckDraw> LuckDraws { get; set; }
    }
}
