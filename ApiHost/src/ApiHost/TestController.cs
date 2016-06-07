using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiHost
{
    public class TestController : ControllerBase
    {
        [Route("test")]
        [Authorize]
        public IActionResult Get()
        {
            var claims = User.Claims.Select(x => new { x.Type, x.Value });
            return new JsonResult(claims.ToArray());
        }
    }
}
