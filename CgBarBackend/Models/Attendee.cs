using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CgBarBackend.Models
{
    public class Patron
    {
        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string Drink { get; set; }
        public DateTime LastDrinkOrdered { get; set; }
    }
}
