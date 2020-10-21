using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace PtcApi.Controllers
{
  public class BaseApiController : Controller
  {
    protected IActionResult HandleException(Exception ex, string msg)
    {
      IActionResult actionResult;

      // Create new exception with generic message        
      actionResult = StatusCode(StatusCodes.Status500InternalServerError, new Exception(msg, ex));

      return actionResult;
    }
  }
}
