using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        //If your context is named something else, fix this
        //and the constructor param
        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// 
        /// Each object in the array should have the following fields:
        /// 
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// 
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        ///
        /// <returns>The JSON array</returns>
        /// 
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            //var query_class = from co in db.Courses
            //                  join cl in db.Classes
            //                  on co.CatalogId equals cl.CatalogId

            //                  where co.SubjectAbb == subject
            //                  where co.CourseNumber == num.ToString()
            //                  where cl.Season == season
            //                  where cl.Year == year

            var query_class = (from cl in db.Classes
                              where cl.Season == season
                              where cl.Year == year

                              select new
                              {
                                  catalogID = cl.CatalogId,
                                  classId = cl.ClassId
                              }).FirstOrDefault();

            var query_course = (from co in db.Courses
                               where co.SubjectAbb == subject
                               where co.CourseNumber == num.ToString()
                               where co.CatalogId == query_class.catalogID

                               select new
                               {
                                   courseID = co.CatalogId
                               }).FirstOrDefault();

            var query_student = from e in db.Enrollments
                                join s in db.Students
                                on e.UId equals s.UId
                                join u in db.Users
                                on s.UId equals u.UId
                                where e.ClassId == query_class.classId

                                select new
                                {
                                    fname = u.FirstName,
                                    lname = u.LastName,
                                    uid = u.UId,
                                    dob = u.Dob,
                                    grade = e.Grade
                                };

                              //var query_students = from 

            return Json(query_student.ToArray());
        }



        /// <summary>
        /// 
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// 
        /// If the "category" parameter is null, return all assignments in the class.
        /// 
        /// Each object in the array should have the following fields:
        /// 
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// 
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// 
        /// <returns>The JSON array</returns>
        /// 
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var queryAssignCat = from cl in db.Classes

                              join co in db.Courses
                              on cl.CatalogId equals co.CatalogId

                              join asscat in db.AssignmentCategories
                              on cl.ClassId equals asscat.ClassId

                              join ass in db.Assignments
                              on asscat.CategoryId equals ass.CategoryId

                              join sub in db.Submissions
                              on cl.ClassId equals sub.ClassId

                              where co.SubjectAbb == subject
                              where co.CourseNumber == num.ToString()
                              where cl.Season == season
                              where cl.Year == year
                              where asscat.Name == category

                              select new
                              {
                                  aname = ass.Name,
                                  cname = asscat.Name,
                                  due = ass.DueDateTime,
                                  //CHECK: submissions count
                                  //submissions = 
                              };

            if (category == null)
            {
                var query = from cl in db.Classes

                            join co in db.Courses
                            on cl.CatalogId equals co.CatalogId

                            join asscat in db.AssignmentCategories
                            on cl.ClassId equals asscat.ClassId

                            join ass in db.Assignments
                            on asscat.CategoryId equals ass.CategoryId

                            join sub in db.Submissions
                            on cl.ClassId equals sub.ClassId

                            where co.SubjectAbb == subject
                            where co.CourseNumber == num.ToString()
                            where cl.Season == season
                            where cl.Year == year

                            select new
                            {
                                aname = ass.Name,
                                cname = asscat.Name,
                                due = ass.DueDateTime,
                                //CHECK: submissions count
                                //submissions = 
                            };

                return Json(query.ToArray());
            }

            return Json(queryAssignCat.ToArray());
        }


        /// <summary>
        /// 
        /// Returns a JSON array of the assignment categories for a certain class.
        /// 
        /// Each object in the array should have the folling fields:
        /// 
        /// "name" - The category name
        /// "weight" - The category weight
        /// 
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// 
        /// <returns>The JSON array</returns>
        /// 
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year, string category)
        {
            var query = from cl in db.Classes

                        join asscat in db.AssignmentCategories
                        on cl.ClassId equals asscat.ClassId

                        join co in db.Courses
                        on cl.CatalogId equals co.CatalogId

                        where co.SubjectAbb == subject
                        where co.CourseNumber == num.ToString()
                        where cl.Season == season
                        where cl.Year == year
                        where asscat.Name == category

                        select new
                        {
                            name = asscat.Name,
                            weight = asscat.GradingWeight
                        };

            return Json(query.ToArray());
        }

        /// <summary>
        /// 
        /// Creates a new assignment category for the specified class.
        /// 
        /// If a category of the given class with the given name already exists, return success = false.
        ///
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// 
        /// <returns>A JSON object containing {success = true/false} </returns>
        /// 
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year,
            string category, int catweight)
        {

            bool createdCategory = false;

            var query = from cl in db.Classes

                        join asscat in db.AssignmentCategories
                        on cl.ClassId equals asscat.ClassId

                        join co in db.Courses
                        on cl.CatalogId equals co.CatalogId

                        where co.SubjectAbb == subject
                        where co.CourseNumber == num.ToString()
                        where cl.Season == season
                        where cl.Year == year
                        where asscat.Name == category
                        where asscat.GradingWeight == catweight

                        select new
                        {
                            weight = asscat.GradingWeight,
                            name = asscat.Name
                        };

            //if category already exists
            if (query.Count() > 0)
            {
                // do nothing
                createdCategory = false;

            }

            else
            {
                var class_query = from cl in db.Classes
                                  join co in db.Courses
                                  on cl.CatalogId equals co.CatalogId
                                  where co.SubjectAbb == subject
                                  where co.CourseNumber == num.ToString()
                                  where cl.Season == season
                                  where cl.Year == year
                                  select new
                                  {
                                      cl.ClassId
                                  };

                AssignmentCategory newAssignCat = new AssignmentCategory();

                newAssignCat.GradingWeight = (uint)catweight;
                newAssignCat.Name = category;
                newAssignCat.ClassId = class_query.First().ClassId;

                db.AssignmentCategories.Add(newAssignCat);
                db.SaveChanges();

                createdCategory = true;
            }
            return Json(new { success = createdCategory });
        }

        /// <summary>
        /// 
        /// Creates a new assignment for the given class and category.
        /// 
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// 
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// 
        /// <returns>A JSON object containing success = true/false</returns>
        /// 
        public IActionResult CreateAssignment(string subject, int num, string season, int year,
            string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            bool createdAssignment = false;

            var query = from cl in db.Classes

                        join asscat in db.AssignmentCategories
                        on cl.ClassId equals asscat.ClassId

                        join co in db.Courses
                        on cl.CatalogId equals co.CatalogId

                        join ass in db.Assignments
                        on asscat.CategoryId equals ass.CategoryId

                        where co.SubjectAbb == subject
                        where co.CourseNumber == num.ToString()
                        where cl.Season == season
                        where cl.Year == year
                        where asscat.Name == category

                        select new
                        {
                            asgname = asscat.Name,
                            asgpoints = ass.MaxPoints,
                            asgdue = ass.DueDateTime,
                            asgcontents = ass.Content
                        };

            //if assignment already exists
            if (query.Count() > 0)
            {
                // do nothing
                createdAssignment = false;
            }

            else
            {
                Assignment newAssign = new Assignment();

                newAssign.Name = asgname;
                newAssign.MaxPoints = (uint)asgpoints;
                newAssign.DueDateTime = asgdue;
                newAssign.Content = asgcontents;

                //update grade
                var update_grade = from en in db.Enrollments
                                   select en.Grade;
                //CHECK : put autograder back?

                db.Assignments.Add(newAssign);
                db.SaveChanges();

                createdAssignment = true;
            }

            return Json(new { success = createdAssignment });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            return Json(null);
        }


        /// <summary>
        /// 
        /// Set the score of an assignment submission
        /// 
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// 
        /// <param name="score">The new score for the submission</param>
        /// 
        /// <returns>A JSON object containing success = true/false</returns>
        /// 
        public IActionResult GradeSubmission(string subject, int num, string season,
            int year, string category, string asgname, string uid, int score)
        {
            bool gradeSubmitted = false;

            var query = from ass in db.Assignments
                        join asscat in db.AssignmentCategories
                        on ass.CategoryId equals asscat.CategoryId

                        join cl in db.Classes
                        on asscat.ClassId equals cl.ClassId

                        join co in db.Courses
                        on cl.CatalogId equals co.CatalogId

                        join sub in db.Submissions
                        on cl.ClassId equals sub.ClassId

                        where co.SubjectAbb == subject
                        where co.CourseNumber == num.ToString()
                        where cl.Season == season
                        where cl.Year == year
                        where asscat.Name == category
                        where ass.Name == asgname
                        where sub.UId == uid

                        select sub; 

            foreach (var q in query)
            {
                q.Score = (uint)score;
            }

            Autograder(uid, subject, num, season, year, category, asgname);

            gradeSubmitted = true;
            db.SaveChanges();

            return Json(new { success = gradeSubmitted });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {            
            return Json(null);
        }


        /// Autograder helper function
        /// 
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "num" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        ///
        /// "asgpoints" - The max point value for the new assignment
        /// 
        private void Autograder(string uid, string subject, int num, string season, int year,
            string category, string asgname)
        {

            var query = from asscat in db.AssignmentCategories

                        join cl in db.Classes
                        on asscat.ClassId equals cl.ClassId

                        join co in db.Courses
                        on cl.CatalogId equals co.CatalogId

                        where co.SubjectAbb == subject
                        where cl.Season == season
                        where cl.Year == year
                        where asscat.Name == category

                        select new
                        {
                            category_weight = asscat.GradingWeight,
                            assignments =   from ass in db.Assignments
                                            join cat in db.AssignmentCategories on ass.CategoryId equals cat.CategoryId
                                            join c in db.Classes on cat.ClassId equals c.ClassId
                                            join sub in db.Submissions on c.ClassId equals sub.ClassId

                                            where ass.CategoryId == asscat.CategoryId
                                            where sub.UId == uid

                                            select new
                                            {
                                                max_score = ass.MaxPoints,
                                                score = sub.Score
                                            }
                        };

            int weight_total = 0;
            double percent = 0.0;


            foreach (var assignment_category in query)
            {
                int points_total = 0;
                int max_points = 0;

                foreach (var assignment in assignment_category.assignments)
                {
                    points_total += (int)assignment.score;
                    max_points += (int)assignment.max_score;
                }

                if (points_total == 0)
                {
                    return;
                }

                percent += ((double)points_total / max_points) * assignment_category.category_weight;
                weight_total += (int)assignment_category.category_weight;
            }

            double scaling_factor = 100 / (double)weight_total;
            double percentGrade = percent * scaling_factor;

            string totalGrade = "";

            if (percentGrade >= 92)
            {
                totalGrade = "A";
            }

            else if (percentGrade >= 90)
            {
                totalGrade = "A-";
            }

            else if (percentGrade >= 87)
            {
                totalGrade = "B+";
            }

            else if (percentGrade >= 82)
            {
                totalGrade = "B";
            }

            else if (percentGrade >= 80)
            {
                totalGrade = "B-";
            }

            else if (percentGrade >= 77)
            {
                totalGrade = "C+";
            }

            else if (percentGrade >= 72)
            {
                totalGrade = "C";
            }

            else if (percentGrade >= 70)
            {
                totalGrade = "C-";
            }

            else if (percentGrade >= 67)
            {
                totalGrade = "D+";
            }

            else if (percentGrade >= 62)
            {
                totalGrade = "D";
            }

            else if (percentGrade >= 60)
            {
                totalGrade = "D-";
            }

            else if (percentGrade < 60)
            {
                totalGrade = "E";
            }


            var querry_enrolled = from en in db.Enrollments
                                  join cl in db.Classes
                                  on en.ClassId equals cl.ClassId

                                  join co in db.Courses
                                  on cl.CatalogId equals co.CatalogId

                                  where co.SubjectAbb == subject
                                  where cl.Season == season
                                  where cl.Year == year
                                  where co.CourseNumber == num.ToString()
                                  where en.UId == uid

                                  select en;

            foreach (var enrollment in querry_enrolled)
            {
                enrollment.Grade = totalGrade;
            }

            db.SaveChanges();
        }

            /*******End code to modify********/
        }
    }

