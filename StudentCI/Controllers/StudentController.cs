﻿using Microsoft.AspNetCore.Mvc;
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
            var ss = _context.students;
            return ss;
        }

        [HttpGet]
        [Route("{id}")]     // http://localhost:5001/student/3
        public ActionResult<string>? GetNameById(int id)
        {
            try
            {
                var ss = _context.students?.Where(d => d.id == id).FirstOrDefault();

                if (ss != null)
                {
                    var res = ss.firstmidname;
                    return res;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return e.Message + " " +  e.InnerException?.Message ?? "Error occoured";
            }
        }

        [HttpPost]
        [Route("Add/")]
        public ActionResult<string> Add(Student s)
        {
            try
            {
                var ss = _context.students?.Add(s);
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
