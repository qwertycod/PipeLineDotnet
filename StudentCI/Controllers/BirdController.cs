using Microsoft.AspNetCore.Mvc;
using StudentCI.Models;

namespace StudentCI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BirdController : Controller
    {
        private readonly Data.SchoolContext _context;

        public BirdController(Data.SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Bird> Get()
        {
            var ss = _context.birds;
            return ss;
        }

        [HttpGet]
        [Route("{id}")]     // http://localhost:5001/bird/3
        public ActionResult<string>? GetNameById(int id)
        {
            try
            {
                var ss = _context.birds?.Where(d => d.id == id).FirstOrDefault();

                if (ss != null)
                {
                    var res = ss.name;
                    return res;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return e.Message + " " + e.InnerException?.Message ?? "Error occoured";
            }
        }

        [HttpPost]
        [Route("Add/")]
        public ActionResult<string> Add(Bird s)
        {
            try
            {
                var ss = _context.birds?.Add(s);
                _context.SaveChanges();
                return Created("", new { id = s.id });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + " " + e.InnerException?.Message);
            }
        }


        [Route("test/{number}")]
        public string Get(string number)
        {
            return (int.Parse(number) * 3).ToString();
        }
    }
}
