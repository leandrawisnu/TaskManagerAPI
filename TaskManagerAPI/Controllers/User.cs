using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User(TaskManagerContext _context) : ControllerBase
    {
        TaskManagerContext _context = _context;

        [HttpGet]
        [Authorize(Policy = "AdminOrManager")]
        public ActionResult All([FromQuery] string? search)
        {
            var data = (from User in _context.Users
                        join Role in _context.Roles on User.RoleId equals Role.Id
                        select new
                        {
                            id = User.Id,
                            name = User.Name,
                            email = User.Email,
                            role = Role.Name
                        }).ToList();
            if (!search.IsNullOrEmpty())
            {
                data = data.Where(f => f.name.Contains(search!) || f.email.Contains(search!) || f.role.Contains(search!)).ToList();
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    data = data
                });
            }
            if (data != null)
            {
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    data = data
                });
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "No users found"
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrManager")]
        public ActionResult Detail(int id)
        {
            var data = (from User in _context.Users
                        join Role in _context.Roles on User.RoleId equals Role.Id
                        where User.Id == id
                        select new
                        {
                            id = User.Id,
                            name = User.Name,
                            email = User.Email,
                            role = Role.Name
                        }).FirstOrDefault(f => f.id == id);
            if (data != null)
            {
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    data = data
                });
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "User not found"
            });
        }
    }
}
