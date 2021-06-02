using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("administracao")]
    public class AdministracaoController : Controller
    {
        [Authorize]
        public IActionResult Dashboard()
        {
            return Ok();
        }

        [Authorize(Roles = "admin")]
        [Route("restricted")]
        public IActionResult Restricted()
        {
            return Ok();
        }
        
    }
}