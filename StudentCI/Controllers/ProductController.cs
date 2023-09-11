using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StudentCI.Models;

namespace StudentCI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly Data.SchoolContext _context;
        public ProductController(Data.SchoolContext context)
        {
            _context = context;
        }

        //static List<Product> products = new List<Product>
        //{
        //    new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
        //    new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
        //    new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        //};

        //[HttpGet(Name = "myproducts")]
        //public IEnumerable<Product> Get()
        //{
        //    return products;
        //}

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                if (_context.products != null)
                {
                    var ss = _context.products;
                    return Ok(ss);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + " " + e.InnerException?.Message);
            }
            return BadRequest();
        }


        [HttpGet]
        [Route("{id}")]     // http://localhost:5001/product/3
        public IActionResult GetNameById(int id)
        {
            try
            {
                var ss = _context.products?.Where(d => d.id == id).FirstOrDefault();

                if (ss != null)
                {
                    return Ok(ss.name);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + " " + e.InnerException?.Message);
            }
        }

        [Route("test/{number}")]
        public string Get(string number)
        {
            return (int.Parse(number) * 4).ToString();
        }

        [HttpPost]
        [Route("Add/")]
        public IActionResult Add(Product p)
        {
            try
            {
                var ss = _context.products?.Add(p);
                _context.SaveChanges();
                return Created("", new { id = p.id });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + " " + e.InnerException?.Message);
            }
        }
    }
}

