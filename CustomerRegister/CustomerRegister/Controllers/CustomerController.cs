using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CustomerRegister.Entities;
using CustomerRegister.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerRegister.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {

        private DatabaseContext context;
        private readonly string pathToCsvFile;

        public CustomerController(DatabaseContext context, IHostingEnvironment env)
        {
            pathToCsvFile = env.ContentRootPath
                + Path.DirectorySeparatorChar.ToString()
                + "Assets"
                + Path.DirectorySeparatorChar.ToString()
                + "Customers.csv";

            this.context = context;
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(context.Customers
                .Select(o => new CustomerVM
                {
                    Id = o.Id,
                    Age = o.Age,
                    CustomerCreated = o.CustomerCreated.ToString("dd/MM/yyyy HH:mm:ss"),
                    CustomerEdited = o.CustomerEdited.ToString("dd/MM/yyyy HH:mm:ss"),
                    Email = o.Email,
                    FirstName = o.FirstName,
                    Gender = o.Gender,
                    LastName = o.LastName,
                }));
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetCustomer(int id)
        {
            return Ok(context.Customers.Find(id));
        }

        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            customer.CustomerCreated = DateTime.Now;
            context.Customers.Add(customer);
            context.SaveChanges();
            return Ok();
        }

        [HttpPatch]
        public IActionResult EditCustomer(Customer customer)
        {
            var entity = context.Customers.Find(customer.Id);
            entity.CustomerEdited = DateTime.Now;
            entity.Age = customer.Age;
            entity.Email = customer.Email;
            entity.FirstName = customer.FirstName;
            entity.Gender = customer.Gender;
            entity.LastName = customer.LastName;
            //context.Entry(customer).State = EntityState.Modified;
            //context.Customers.Update(customer);
            context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteCustomer(int id)
        {
            context.Customers.Remove(context.Customers.Find(id));
            context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("seed")]
        public IActionResult Seed()
        {
            context.Customers.RemoveRange(context.Customers);

            using (StreamReader sr = new StreamReader(pathToCsvFile))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Trim().Split(",");
                    context.Customers.Add(new Customer
                    {
                        FirstName = line[0],
                        LastName = line[1],
                        Email = line[2],
                        Gender = line[3],
                        Age = int.Parse(line[4]),
                        CustomerCreated = DateTime.Now,
                    });
                }
            }

            context.SaveChanges();
            return Ok();
        }

    }
}