using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using NuGet.Protocol;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Controllers
{
    public class CommonController : Controller
    {
        //If your context class is named differently, fix this
        //and the constructor parameter
        private readonly LMSContext db;

        public CommonController(LMSContext _db)
        {
            db = _db;
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            Console.WriteLine("CommonController.cs: GetDepartments() called!");
            // CHECK:
            var query = from d in db.Departments
                    select new
                    {
                        name = d.Name,
                        subject = d.SubjectAbb
                    };
                        
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            Console.WriteLine("CommonController.cs: GetCatalog() called!");
            var query = from d in db.Departments
                        join co in db.Courses 
                        on d.SubjectAbb equals co.SubjectAbb
                select new
                {
                    subject = d.SubjectAbb,
                    dname = d.Name,
                    courses = (from c in db.Courses where c.SubjectAbb == d.SubjectAbb
                        select new
                        {
                            number = c.CourseNumber,
                            cname = c.CourseName
                        }).ToArray()

                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            Console.WriteLine("CommonController.cs: GetClassOfferings() called!");

            // CHECK: change courseNumber to int?
            var query = from co in db.Courses
                join c in db.Classes on co.CatalogId equals c.CatalogId
                join p in db.Professors on c.ProfessorUId equals p.UId
                join u in db.Users on p.UId equals u.UId
                where subject == co.SubjectAbb
                where number.ToString() == co.CourseNumber

                select new
                {
                    season = c.Season,
                    year = c.Year,
                    location = c.Location,
                    start = c.StartTime.ToString(),
                    end = c.EndTime.ToString(),
                    fname = u.FirstName,
                    lname = u.LastName
                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            Console.WriteLine("CommonController.cs: GetAssignmentContents() called!");
            // CHECK: Remove submissions join
            var query = from s in db.Submissions
                join c in db.Classes on s.ClassId equals c.ClassId
                join cat in db.AssignmentCategories on s.ClassId equals cat.ClassId
                join co in db.Courses on c.CatalogId equals co.CatalogId
                join asg in db.Assignments on cat.CategoryId equals asg.CategoryId
                where subject == co.SubjectAbb
                where num.ToString() == co.CourseNumber
                where season == c.Season
                where year == c.Year
                where category == cat.Name
                where asgname == asg.Name
                select asg.Content;
            return Content(query.FirstOrDefault() ?? "");
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            Console.WriteLine("CommonController.cs: GetSubmissionText() called!");
            var query = from s in db.Submissions
                join c in db.Classes on s.ClassId equals c.ClassId
                join cat in db.AssignmentCategories on s.ClassId equals cat.ClassId
                join co in db.Courses on c.CatalogId equals co.CatalogId
                join asg in db.Assignments on cat.CategoryId equals asg.CategoryId
                where subject == co.SubjectAbb
                where num.ToString() == co.CourseNumber
                where season == c.Season
                where year == c.Year
                where category == cat.Name
                where asgname == asg.Name
                select s.SubmissionContents;
            return Content(query.LastOrDefault() ?? "");
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {
            Console.WriteLine("CommonController.cs: GetUser() called!");
            try
            {
                if (db.Students.Find(uid) != null)
                {
                    Console.WriteLine("CommonController.cs: GetUser() called! Found Student!");
                    var query = from u in db.Users
                        join s in db.Students on u.UId equals s.UId
                        join d in db.Departments on s.Major equals d.SubjectAbb
                        where s.UId == uid
                        select new
                        {
                            fname = u.FirstName,
                            lname = u.LastName,
                            uid = u.UId,
                            department = d.Name
                        };
                    return Json(query.First());
                }
            
                if (db.Professors.Find(uid) != null)
                {
                    Console.WriteLine("CommonController.cs: GetUser() called! Found Professor!");
                    var query = from u in db.Users
                        join p in db.Professors on u.UId equals p.UId
                        join d in db.Departments on p.SubjectAbb equals d.SubjectAbb
                        where p.UId == uid
                        select new
                        {
                            fname = u.FirstName,
                            lname = u.LastName,
                            uid = u.UId,
                            department = d.Name
                        };
                    return Json(query.First());
                }
            
                if (db.Administrators.Find(uid) != null)
                {
                    Console.WriteLine("CommonController.cs: GetUser() called! Found Admin!");
                    var query = from u in db.Users
                        join a in db.Administrators on u.UId equals a.UId
                        where a.UId == uid
                        select new
                        {
                            fname = u.FirstName,
                            lname = u.LastName,
                            uid = u.UId
                        };
                    return Json(query.First());
                }
            }
            catch (Exception e)
            {
                // Silence unnecessary exceptions that do not matter.
            }
            
            return Json(new { success = false });
        }


        /*******End code to modify********/
    }
}

