using System.ComponentModel.DataAnnotations;

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