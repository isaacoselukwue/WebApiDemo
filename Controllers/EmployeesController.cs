using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EmployeeDataAccess;

namespace WebApiDemo.Controllers
{
    public class EmployeesController : ApiController
    {
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            using(EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                return entities.Employees.ToList();
            }
        }
        [Route(Name ="api/employee/bygender")]
        public HttpResponseMessage GetByGender(string gender ="All")
        {
            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                switch (gender.ToLower())
                {
                    case "all":
                        return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.ToList());
                    case "male":
                        return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.Where(e=>e.Gender.ToLower()=="male").ToList());
                    case "female":
                        return Request.CreateResponse(HttpStatusCode.OK, entities.Employees.Where(e => e.Gender.ToLower() == "female").ToList());
                    default:
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"gender with string: {gender} not found");
                }

            }
        }
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            using (EmployeeDBEntities entities = new EmployeeDBEntities())
            {
                var entity = entities.Employees.FirstOrDefault(e=>e.EmployeeId==id);
                if (entity!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Employee with id: {id} not found.");
                }
            }
        }
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    entities.Employees.Add(employee);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                    message.Headers.Location = new Uri(Request.RequestUri + employee.EmployeeId.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);

            }

        }
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var deleteEmployee = entities.Employees.FirstOrDefault(e => e.EmployeeId == id);
                    if (deleteEmployee == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, $"Employee with id: {id} not found");
                    }
                    entities.Employees.Remove(deleteEmployee);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, $"employee with id: {id} deleted successfully");
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromUri] Employee employee)
        {
            try
            {
                using (EmployeeDBEntities entities = new EmployeeDBEntities())
                {
                    var entity = entities.Employees.FirstOrDefault(e => e.EmployeeId == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Sorry employee details with id: {id} was not found");
                    }
                    entity.Name = employee.Name;
                    entity.Gender = employee.Gender;
                    entity.Salary = employee.Salary;
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
