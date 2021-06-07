using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CgBarBackend.Models;

namespace CgBarBackend.Services
{
    public class BarTender : IBarTender
    {
        private ConcurrentDictionary<string, Patron> _patrons = new();
        private Timer _cleanupTimer = new Timer();

        public event EventHandler<Patron> PatronAdded;
        public event EventHandler<string> PatronExpired;
        public event EventHandler<Patron> DrinkOrdered;
        public event EventHandler<string> DrinkExpired;

        public BarTender()
        {
            _cleanupTimer.Elapsed += (sender, args) => Cleanup();
            _cleanupTimer.Start();
        }

        public void AddPatron(string screenName, string name, string profileImage)
        {
            if (_patrons.ContainsKey(screenName))
            {
                return;
            }

            var patron = new Patron
                {ScreenName = screenName, Name = name, ProfileImage = profileImage, LastDrinkOrdered = DateTime.Now};
            _patrons.TryAdd(screenName, patron);
            PatronAdded?.Invoke(this,patron);
        }

        public void OrderDrink(string screenName, string drink)
        {
            if (_patrons.ContainsKey(screenName) == false)
            {
                return;
            }

            _patrons[screenName].Drink = drink;
            _patrons[screenName].LastDrinkOrdered = DateTime.Now;
            DrinkOrdered?.Invoke(this, _patrons[screenName]);
        }

        public IEnumerable<Patron> Patrons => _patrons.Values.AsEnumerable();

        private void Cleanup()
        {
            var drinkTimeCheck = DateTime.Now.AddMinutes(-30);
            var activeTimeCheck = DateTime.Now.AddMinutes(-60);
            foreach (var patron in _patrons)
            {
                if (patron.Value.LastDrinkOrdered < activeTimeCheck && _patrons.Remove(patron.Key, out _))
                {
                    PatronExpired?.Invoke(this,patron.Key);
                    continue;
                }

                if (patron.Value.LastDrinkOrdered < drinkTimeCheck)
                {
                    patron.Value.Drink = null;
                    DrinkExpired?.Invoke(this, patron.Key);
                }
            }
        }
    }
}
