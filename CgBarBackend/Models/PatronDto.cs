using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CgBarBackend.Models
{
    public class PatronDto
    {
        public PatronDto()
        {
            
        }

        public PatronDto(Patron patron)
        {
            ScreenName = patron.ScreenName;
            Name = patron.Name;
            ProfileImage = patron.ProfileImage;
            Drink = patron.Drink;
            IsPolite = patron.IsPolite;
        }

        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string Drink { get; set; }
        public bool IsPolite { get; set; }
    }
}
