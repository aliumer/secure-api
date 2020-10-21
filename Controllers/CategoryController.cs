using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PtcApi.Model;

namespace PtcApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : BaseApiController
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            IActionResult actionResult = null;
            List<Category> list = new List<Category>();

            try
            {
                using (var db = new PtcDbContext())
                {
                    if (db.Categories.Count() > 0)
                    {
                        // NOTE: Declare 'list' outside the using to avoid 
                        // it being disposed before it is returned.
                        list = db.Categories.OrderBy(p => p.CategoryName).ToList();
                        actionResult = StatusCode(StatusCodes.Status200OK, list);
                    }
                    else
                    {
                        actionResult = StatusCode(StatusCodes.Status404NotFound,
                                       "Can't Find Categories");
                    }
                }
            }
            catch (Exception ex)
            {
                actionResult = HandleException(ex,
                     "Exception trying to get all Categories");
            }

            return actionResult;
        }
    }
}
