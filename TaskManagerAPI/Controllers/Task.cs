using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TaskManagerAPI.DTO;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Task(TaskManagerContext _context) : ControllerBase
    {
        TaskManagerContext _context = _context;

        // Rumus Start
        private int startForm(int x)
        {
            int start = (x - 1) * 10;
            return start;
        }

        private int GetUserID()
        {
            string header = Request.Headers["Authorization"]!;
            string token = header.Substring("Bearer ".Length).Trim();

            var handler = new JsonWebTokenHandler();
            var id = int.Parse(handler.ReadJsonWebToken(token).Claims.FirstOrDefault(f => f.Type == "UserID")!.Value);

            return id;
        }

        [HttpGet]
        [Authorize]
        public ActionResult AllMine([FromQuery] string? search)
        {
            int userId = GetUserID();

            var data = _context.Tasks.Where(f => f.UserId == userId).OrderBy(f => f.Status).ToList();
            if (!search.IsNullOrEmpty())
            {
                if (data.Any())
                {
                    data = data.Where(f => f.Title.Contains(search!) || f.Description.Contains(search!)).ToList();
                    return Ok(data);
                }
            }
            if (data.Any())
            {
                return Ok(data);
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "No tasks found"
            });
        }

        [HttpGet("All")]
        //[Authorize(Policy = "Admin")]
        public ActionResult All([FromQuery] int page = 1)
        {
            int pages = page * 10;

            if (pages > 10)
            {
                var count = _context.Tasks.Take(pages).OrderDescending().ToList();
                var data = _context.Tasks.Where(f => f.Id > count.First().Id).Take(10).ToList();
                if (data.Any())
                {
                    return Ok(data);
                }
            }
            else
            {
                var data = _context.Tasks.Take(10).ToList();
                if (data.Any())
                {
                    return Ok(data);
                }
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "No tasks found"
            });
        }

        [HttpGet("Available")]
        [Authorize(Policy = "ManagerOrUser")]
        public ActionResult Available()
        {
            var data = _context.Tasks.Where(f => f.Status == "Pending").Select(f => new
            {
                id = f.Id,
                title = f.Title,
                description = f.Description,
                createAt = f.CreateAt.ToString("dd-MMM-yyyy hh:mm")
            }).ToList();
            if (data.Any())
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
                message = "No tasks Available"
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult GetTask(int id)
        {
            int userId = GetUserID();

            var data = _context.Tasks.Where(f => f.UserId == userId && f.Id == id).ToList();
            var task = data.FirstOrDefault(f => f.Id == id);

            if (task != null)
            {
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    title = task.Title,
                    description = task.Description,
                    status = task.Status,
                    createAt = task.CreateAt.ToString("dd-MMM-yyyy hh:mm"),
                    doneAt = task.DoneAt?.ToString("dd-MMM-yyyy hh:mm")
                });
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "Task not found / You are not authorized to view this Task"
            });
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public ActionResult AddTask(TaskDTO taskDTO)
        {
            Models.Task task = new Models.Task()
            {
                UserId = taskDTO.UserID,
                Title = taskDTO.Title,
                Description = taskDTO.Description,
                Status = "Pending",
                CreateAt = DateTime.Now
            };

            //_context.Tasks.Add(task);
            //_context.SaveChanges();

            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "Task Created Successfully",
                task = task
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult UpdateTask(int id, TaskEdit taskEdit)
        {
            var data = _context.Tasks.FirstOrDefault(f => f.Id == id);
            if (data != null)
            {
                Models.Task taskBaru = new Models.Task()
                {
                    Id = data.Id,
                    Title = taskEdit.Title,
                    Description = taskEdit.Description,
                };

                //_context.Update(taskBaru);
                //_context.SaveChanges();

                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Task Updated Successfully",
                    task = taskBaru
                });
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "Task not found"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult DeleteTask(int id)
        {
            var data = _context.Tasks.FirstOrDefault(f => f.Id == id);
            if (data != null)
            {
                if (data.Status == "Progress")
                {
                    return Conflict(new
                    {
                        statusCode = StatusCodes.Status409Conflict,
                        message = "Task is in Progress"
                    });
                }
                else if (data.Status == "Completed")
                {
                    return Conflict(new
                    {
                        statusCode = StatusCodes.Status409Conflict,
                        message = "Task is Completed"
                    });
                }
                else
                {
                    //_context.Tasks.Remove(data);
                    //_context.SaveChanges();
                    return Ok(new
                    {
                        statusCode = StatusCodes.Status200OK,
                        message = "Task Deleted Successfully"
                    });
                }
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "Task not found"
            });
        }

        [HttpPost("{id}/Apply")]
        [Authorize(Policy = "ManagerOrUser")]
        public ActionResult Apply(int id)
        {
            int userId = GetUserID();
            var tasks = _context.Tasks.Where(f => f.Status == "Pending").ToList();

            if (tasks.Any())
            {
                var data = tasks.FirstOrDefault(f => f.Id == id);
                if (data != null)
                {
                    if (data!.UserId != null)
                    {
                        return Conflict(new
                        {
                            statusCode = StatusCodes.Status409Conflict,
                            message = "Task is already taken"
                        });
                    }
                    else
                    {
                        data.UserId = userId;
                        data.Status = "Progress";
                        //_context.Update(data);
                        //_context.SaveChanges();
                        return Ok(new
                        {
                            statusCode = StatusCodes.Status200OK,
                            message = "Task Applied Successfully"
                        });
                    } 
                }
                return NotFound(new
                {
                    statusCode = StatusCodes.Status404NotFound,
                    message = "Task not found / Task is not Available"
                });
            } return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "No tasks Available"
            });
        }

        [HttpPost("{id}/Complete")]
        [Authorize(Policy = "Manager")]
        public ActionResult Complete(int id)
        {
            int userId = GetUserID();
            var tasks = _context.Tasks.Where(f => f.UserId == userId);

            var data = tasks.FirstOrDefault(f => f.Id == id);
            if (data != null)
            {
                if (data.Status == "Completed")
                {
                    return Conflict(new
                    {
                        statusCode = StatusCodes.Status409Conflict,
                        message = "Task is Completed"
                    });
                }
                else if (data.Status == "Pending")
                {
                    return Conflict(new
                    {
                        statusCode = StatusCodes.Status409Conflict,
                        message = "Task is Pending"
                    }); 
                }
                else
                {
                    data.Status = "Completed";
                    data.DoneAt = DateTime.Now;
                    //_context.Update(data);
                    //_context.SaveChanges();
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Task not found / You are not authorized to Complete this Task"
                    });
                    return Ok(new
                    {
                        statusCode = StatusCodes.Status200OK,
                        message = "Task Completed Successfully"
                    });
                }
            }
            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "Task not found / You are not authorized to Complete this Task"
            });
        }
    }
}
