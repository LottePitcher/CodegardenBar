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
        public DateTime LastDrinkDelivered { get; set; }
        public int PolitenessLevel { get; private set; } = 4;
        public bool IsPolite => PolitenessLevel >= 5;

        // Fallback is just for non breaking deploy
        private string _lastOrderedDrink;
        public string LastOrderedDrink
        {
            get => _lastOrderedDrink ?? Drink;
            set => _lastOrderedDrink = value;
        }

        public void IncreasePolitenessLevel()
        {
            if (PolitenessLevel >= 10)
            {
                return;
            }

            PolitenessLevel++;
        }

        public void DecreasePolitenessLevel()
        {
            if (PolitenessLevel <= 0)
            {
                return;
            }

            PolitenessLevel--;
        }
    }
}
