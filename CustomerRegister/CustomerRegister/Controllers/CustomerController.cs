using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using CustomerRegister.Entities;
using CustomerRegister.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Targets;

namespace CustomerRegister.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {

        private readonly ILogger<CustomerController> logger;
        private readonly string pathToCsvFile;
        private DatabaseContext context;

        public CustomerController(DatabaseContext context, IHostingEnvironment env, ILogger<CustomerController> logger)
        {
            pathToCsvFile = Path.Combine(env.WebRootPath, "assets", "Customers.csv");
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {

            var customers = context.Customers
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
                });

            logger.LogInformation($"Getting all customers from database, count: {customers.Count()}");

            return Ok(customers);
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

            if (!ModelState.IsValid)
            {
                logger.LogInformation("Adding customer failed because of invalid state model");
                return BadRequest("Alla fält måste vara ifyllda.");
            }


            customer.CustomerCreated = DateTime.Now;
            context.Customers.Add(customer);
            context.SaveChanges();

            logger.LogInformation($"New customer added: {customer.FirstName} {customer.LastName}");

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

            logger.LogInformation($"Customer {customer.FirstName} {customer.LastName} edited");

            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteCustomer(int id)
        {
            context.Customers.Remove(context.Customers.Find(id));
            context.SaveChanges();

            logger.LogInformation($"Customer with id {id} deleted");

            return Ok();
        }

        [HttpGet]
        [Route("seed")]
        public IActionResult Seed()
        {
            context.Customers.RemoveRange(context.Customers);

            using (var sr = System.IO.File.OpenText(pathToCsvFile))
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

            logger.LogInformation($"Seed function used");

            context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("log/{shortdate}")]
        public IActionResult GetNLogFile(DateTime shortdate)
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("ownFile-web");
            var logEventInfo = new LogEventInfo { TimeStamp = shortdate };
            string fileName = fileTarget.FileName.Render(logEventInfo);

            if (!System.IO.File.Exists(fileName))
                return NotFound("No file exists");
            else
            {
                return Ok(System.IO.File.ReadAllLines(fileName));
            }
        }

    }
}