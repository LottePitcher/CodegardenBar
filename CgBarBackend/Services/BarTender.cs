using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CgBarBackend.Models;
using CgBarBackend.Repositories;
using Microsoft.Extensions.Configuration;

namespace CgBarBackend.Services
{
    public class BarTender : IBarTender
    {
        private readonly IBarTenderRepository _barTenderRepository;
        private ConcurrentDictionary<string, Patron> _patrons = new();

        private object _bannedPatronsLock = new();
        private List<string> _bannedPatrons = new();

        private Timer _cleanupTimer = new Timer();
        private int _drinkExpireTimeInMinutes = 30;
        private int _patronExpireTimeInMinutes = 60;
        private int _expireTimeIntervalInMilliseconds = 60000;

        public event EventHandler<Patron> PatronAdded;
        public event EventHandler<string> PatronExpired;
        public event EventHandler<Patron> DrinkOrdered;
        public event EventHandler<string> DrinkExpired;

        public BarTender(IConfiguration configuration, IBarTenderRepository barTenderRepository)
        {
            _barTenderRepository = barTenderRepository;
            int.TryParse(configuration["BarTender:DrinkExpireTimeInMinutes"], out _drinkExpireTimeInMinutes);
            int.TryParse(configuration["BarTender:PatronExpireTimeInMinutes"], out _patronExpireTimeInMinutes);
            int.TryParse(configuration["BarTender:ExpireTimeIntervalInMilliseconds"], out _expireTimeIntervalInMilliseconds);
            _cleanupTimer.Interval = _expireTimeIntervalInMilliseconds;
            _cleanupTimer.Elapsed += (sender, args) => Cleanup();
            _cleanupTimer.Start();
        }

        public void AddPatron(string screenName, string name, string profileImage, string byScreenName = null)
        {
            if (_bannedPatrons.Contains(screenName) || _bannedPatrons.Contains(byScreenName))
            {
                return;
            }
            if (_patrons.ContainsKey(screenName))
            {
                return;
            }

            var patron = new Patron
                {ScreenName = screenName, Name = name, ProfileImage = profileImage, LastDrinkOrdered = DateTime.Now};
            _patrons.TryAdd(screenName, patron);
            PatronAdded?.Invoke(this,patron);
            _barTenderRepository.SavePatrons(Patrons); // this is call synchronously because we don't want to wait for this to complete
        }

        public bool PatronExists(string screenName) => _patrons.ContainsKey(screenName);

        public void OrderDrink(string screenName, string drink, string byScreenName = null)
        {
            if (_bannedPatrons.Contains(screenName) || _bannedPatrons.Contains(byScreenName))
            {
                return;
            }
            if (_patrons.ContainsKey(screenName) == false)
            {
                return;
            }

            _patrons[screenName].Drink = drink;
            _patrons[screenName].LastDrinkOrdered = DateTime.Now;
            DrinkOrdered?.Invoke(this, _patrons[screenName]);
        }

        public bool BanPatron(string screenName)
        {
            if (_bannedPatrons.Contains(screenName))
            {
                return false;
            }

            if (_patrons.ContainsKey(screenName) && _patrons.Remove(screenName, out _))
            {
                PatronExpired?.Invoke(this, screenName);
            }

            lock (_bannedPatronsLock)
            {
                _bannedPatrons.Add(screenName);
                _barTenderRepository.SaveBannedPatrons(_bannedPatrons);
                return true;
            }
        }

        public bool UnBanPatron(string screenName)
        {
            if (_bannedPatrons.Contains(screenName) == false)
            {
                return false;
            }
            lock (_bannedPatronsLock)
            {
                _bannedPatrons.Remove(screenName);
                _barTenderRepository.SaveBannedPatrons(_bannedPatrons);
                return true;
            }
        }

        public IEnumerable<Patron> Patrons => _patrons.Values.AsEnumerable();

        public async Task Load()
        {
            foreach (var patron in await _barTenderRepository.LoadPatrons().ConfigureAwait(false))
            {
                _patrons.Clear();
                _patrons.TryAdd(patron.ScreenName, patron);
            }

            foreach (var bannedPatron in await _barTenderRepository.LoadBannedPatrons().ConfigureAwait(false))
            {
                _bannedPatrons.Clear();
                _bannedPatrons.Add(bannedPatron);
            }
        }

        private void Cleanup()
        {
            var drinkTimeCheck = DateTime.Now.AddMinutes(-1 * _drinkExpireTimeInMinutes);
            var activeTimeCheck = DateTime.Now.AddMinutes(-1 * _patronExpireTimeInMinutes);
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

            _barTenderRepository.SavePatrons(Patrons);
        }
    }
}
