using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PtcApi.Model;

namespace PtcApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "CanAccessAdmin")]
    public class AdminController : BaseApiController
    {
        [HttpGet("getbyname/{userName}")]
        public IActionResult GetByName(string userName)
        {
            IActionResult ret = null;
            AppUser entity = null;

            try
            {
                using (var db = new PtcDbContext())
                {
                    entity = db.Users.Include(item => item.Claims).Where(u => u.UserName == userName).FirstOrDefault<AppUser>();
                    if (entity != null)
                    {
                        ret = StatusCode(StatusCodes.Status200OK, entity);
                    }
                    else
                    {
                        ret = StatusCode(StatusCodes.Status404NotFound,
                                 "Can't Find Product: " + userName);
                    }
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex, ex.Message);
            }

            return ret;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            IActionResult ret = null;
            AppUser entity = null;

            try
            {
                using (var db = new PtcDbContext())
                {
                    entity = db.Users.Find(id);
                    if (entity != null)
                    {
                        ret = StatusCode(StatusCodes.Status200OK, entity);
                    }
                    else
                    {
                        ret = StatusCode(StatusCodes.Status404NotFound,
                                 "Can't Find Product: " + id.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ret = HandleException(ex,
                  "Exception trying to retrieve a single product.");
            }

            return ret;
        }

        [HttpPost()]
        public IActionResult Post([FromBody]AppUser entity)
        {
            IActionResult actionResult = null;

            try
            {
                using (var db = new PtcDbContext())
                {
                    if (entity != null)
                    {
                        db.Users.Add(entity);
                        db.SaveChanges();
                        actionResult = StatusCode(StatusCodes.Status201Created,
                            entity);
                    }
                    else
                    {
                        actionResult = StatusCode(StatusCodes.Status400BadRequest, "Invalid object passed to POST method");
                    }
                }
            }
            catch (Exception ex)
            {
                actionResult = HandleException(ex, "Exception trying to insert a new user");
            }

            return actionResult;
        }

        [HttpPut()]
        public IActionResult Put([FromBody]AppUser entity)
        {
            IActionResult actionResult = null;

            try
            {
                using (var db = new PtcDbContext())
                {
                    if (entity != null)
                    {
                        db.Update(entity);
                        db.SaveChanges();
                        actionResult = StatusCode(StatusCodes.Status200OK, entity);
                    }
                    else
                    {
                        actionResult = StatusCode(StatusCodes.Status400BadRequest, "Invalid object passed to PUT method");
                    }
                }
            }
            catch (Exception ex)
            {
                actionResult = HandleException(ex, "Exception trying to update product: " + entity.UserId.ToString());
            }

            return actionResult;
        }

    }
}
