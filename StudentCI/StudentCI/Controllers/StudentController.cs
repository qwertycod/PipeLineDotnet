using Microsoft.AspNetCore.Mvc;
using StudentCI.Models;

namespace StudentCI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : Controller
    {
        private readonly Data.SchoolContext _context;

        public StudentController(Data.SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            var ss = _context.Students;
            return ss;
        }

        [HttpGet]
        [Route("{id}")]     // http://localhost:5001/student/3
        public ActionResult<string>? GetNameById(int id)
        {
            try
            {
                var ss = _context.Students?.Where(d => d.ID == id).FirstOrDefault();

                if (ss != null)
                {
                    var res = ss.FirstMidName;
                    return res;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return ex.Message ?? "Error occoured";
            }
        }

        [HttpPost]
        [Route("Add/")]
        public IActionResult Add(Student s)
        {
            try
            {
                var ss = _context.Students?.Add(s);
                _context.SaveChanges();
                return Created("", new { id = s.ID });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [Route("test/{number}")]
        public string Get(string number)
        {
            return (int.Parse(number) * 3).ToString();
        }

    }
}
