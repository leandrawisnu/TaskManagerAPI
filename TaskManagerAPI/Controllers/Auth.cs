using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManagerAPI.DTO;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth(TaskManagerContext _context, TokenService tokenService) : ControllerBase
    {
        TaskManagerContext _context = _context;

        private int GetUserID()
        {
            string header = Request.Headers["Authorization"]!;
            string token = header.Substring("Bearer ".Length).Trim();

            var handler = new JsonWebTokenHandler();
            var id = int.Parse(handler.ReadJsonWebToken(token).Claims.FirstOrDefault(f => f.Type == "UserID")!.Value);

            return id;
        }

        private string Hash(string input)
        {
            string output = string.Join("", MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input)).Select(f => f.ToString("x2"))).ToUpper();
            return output;
        }

        [HttpPost("Login")]
        public ActionResult Login(LoginDTO loginDTO)
        {
            var data = _context.Users.FirstOrDefault(f => f.Email == loginDTO.Email);
            if (data == null)
            {
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Email not found"
                });
            }
            if (data.Password != Hash(loginDTO.Password))
            {
                return Unauthorized(new
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "Invalid Password"
                });
            }
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                token = tokenService.CreateJWT(data),
                message = "Login Successful"
            });
        }

        [HttpPost("Register")]
        public ActionResult Register(RegisterDTO registerDTO)
        {
            var data = _context.Users.FirstOrDefault(f => f.Email == registerDTO.Email);
            if (data != null)
            {
                return Conflict(new
                {
                    statusCode = StatusCodes.Status409Conflict,
                    message = "Email is already Registered"
                });
            }
            Models.User user = new Models.User()
            {
                Name = registerDTO.Name,
                Email = registerDTO.Email,
                Password = Hash(registerDTO.Password),
                RoleId = 3
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "Registration Successful",
            });
        }

        [HttpGet("Me")]
        [Authorize]
        public ActionResult Me()
        {
            int id = GetUserID();

            var me = _context.Users.Where(f => f.Id == id).ToList();
            var data = (from user in me
                        join roles in _context.Roles on user.RoleId equals roles.Id
                        select new
                        {
                            id = user.Id,
                            name = user.Name,
                            email = user.Email,
                            role = roles.Name
                        }).First();
            var task = _context.Tasks.Where(f => f.UserId == id).Select(f => new
            {
                id = f.Id,
                title = f.Title,
                description = f.Description,
                status = f.Status
            }).ToList();
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                data = data,
                tasks = task
            });
        }
    }
}
