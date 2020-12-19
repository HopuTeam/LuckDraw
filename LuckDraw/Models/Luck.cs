using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    //选项表
    public class Luck
    {
        public int ID {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public int Weigh {get;set;}
        public int SighID {get;set;}
        public int ParentID { get; set; }

        public List<LuckDraw> LuckDraws { get; set; }
    }
}
