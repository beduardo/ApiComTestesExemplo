using System;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        public IActionResult Get()
        {
            return Ok("Versão: 1.0.1");
        }
    }
}