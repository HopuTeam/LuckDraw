using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    public class Sign
    {
        [Key]
        public int ID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public int Identity { get; set; }
    }
}
