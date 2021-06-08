using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Authorization;
using CgBarBackend.Models;
using CgBarBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CgBarBackend.Controllers
{
    [Route("Bar/[action]")]
    public class BarController : ControllerBase
    {
        private readonly IBarTender _barTender;

        public BarController(IBarTender barTender)
        {
            _barTender = barTender;
        }

        public bool Ping()
        {
            return true;
        }

        public IEnumerable<PatronDto> Patrons() => _barTender.Patrons.Select(patron => new PatronDto(patron));
    }
}
