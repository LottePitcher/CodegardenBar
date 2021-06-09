using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CgBarBackend.Models
{
    public class Order
    {
        public string ScreenName { get; set; }
        public string Drink { get; set; }

        public Order(string screenName, string drink)
        {
            ScreenName = screenName;
            Drink = drink;
        }
    }
}
