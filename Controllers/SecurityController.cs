using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PtcApi.Security;
using PtcApi.Model;

namespace PtcApi.Controllers
{
    [Route("api/[controller]")]
    public class SecurityController : BaseApiController {

        private JwtSettings _settings;
        public SecurityController(JwtSettings settings)
        {
            _settings = settings;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]AppUser user) {
            IActionResult actionResult = null;
            AppUserAuth auth = new AppUserAuth();
            SecurityManager mgr = new SecurityManager(_settings);
            auth = mgr.ValidateUser(user);
            if (auth.IsAuthenticated) {
                actionResult = StatusCode(StatusCodes.Status200OK, auth);
            } else {
                actionResult = StatusCode(StatusCodes.Status404NotFound, "Invalid User Name/Password");
            }
            return actionResult;
        }
    }
}