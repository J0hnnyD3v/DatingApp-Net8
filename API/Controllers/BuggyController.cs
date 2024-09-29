using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController(DataContext dataContext) : ApiBaseController
    {
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetAuth()
        {
            return Unauthorized();
        }

        [HttpGet("not-Found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = dataContext.Users.Find(-1);

            if (thing == null) return NotFound();

            return Ok(thing);
        }

        [HttpGet("server-error")]
        public ActionResult<AppUser> GetServerError()
        {
                var thing = dataContext.Users.Find(-1) ?? throw new Exception("A bad thing has happened");
                return Ok(thing);
            // try
            // {
            // }
            // catch (Exception ex) 
            // {
            //     return StatusCode(500, $"Computer says no! {ex.Message}");
            // }
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
}
