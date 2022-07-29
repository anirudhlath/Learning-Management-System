using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        //If your context is named something else, fix this and the
        //constructor param
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
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


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// 
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// 
        /// Each object in the array should have the following fields:
        /// 
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// 
        /// </summary>
        /// 
        /// <param name="uid">The uid of the student</param>
        /// 
        /// <returns>The JSON array</returns>
        /// 
        public IActionResult GetMyClasses(string uid)
        {
            var query = from e in db.Enrollments
                        join c in db.Classes on e.ClassId equals c.ClassId
                        into leftSide
                        
                        from l in leftSide
                        join r in db.Courses on l.CatalogId equals  r.CatalogId
                        where e.UId == uid
                        
                        select new
                        {
                            subject = r.SubjectAbb,
                            number = r.CourseNumber,
                            name = r.CourseName,
                            season = l.Season,
                            year = l.Year,
                            grade = e.Grade
                        };


            return Json(query.ToArray());
        }

        /// <summary>
        /// 
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        ///
        /// Each object in the array should have the following fields:
        /// 
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        ///
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// 
        /// <returns>The JSON array</returns>
        /// 
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var query = from c in db.Classes
                        join asc in db.AssignmentCategories on c.ClassId equals asc.ClassId
                        join ass in db.Assignments on asc.CategoryId equals ass.CategoryId
                        join co in db.Courses on c.CatalogId equals co.CatalogId
                        join e in db.Enrollments on c.ClassId equals e.ClassId
                        where co.CourseNumber == num.ToString()
                        where string.Equals(c.Season, season, StringComparison.CurrentCultureIgnoreCase)
                        where c.Year == year
                        where string.Equals(e.UId.ToLower(), uid.ToLower(), StringComparison.CurrentCultureIgnoreCase)
                        where string.Equals(co.SubjectAbb.ToLower(), subject.ToLower(), StringComparison.CurrentCultureIgnoreCase)
                        select new
                        {
                            aname = ass.Name,
                            cname = asc.Name,
                            due = ass.DueDateTime,
                            score = from sub in db.Submissions
                                    where string.Equals(sub.UId.ToLower(), uid.ToLower(), StringComparison.CurrentCultureIgnoreCase)
                                    where c.ClassId == sub.ClassId
                                    select sub.Score
                        };

            var result = new List<object>();
            foreach (var assignment in query)
            {
                result.Add(new { assignment.aname, assignment.cname, assignment.due, score = (assignment.score.Any() ? assignment.score : null) });
            }

            return Json(result);
        }



        /// <summary>
        /// 
        /// Adds a submission to the given assignment for the given student
        /// 
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// 
        /// The score of the submission should start as 0 until a Professor grades it
        /// 
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// 
        /// </summary>
        /// 
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// 
        /// <returns>A JSON object containing {success = true/false}</returns>
        /// 
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            bool submitted = false;

            var query_assignment = (from c in db.Classes
                                    join co in db.Courses
                                    on c.CatalogId equals co.CatalogId
                                    join asscat in db.AssignmentCategories
                                    on c.ClassId equals asscat.ClassId
                                    join ass in db.Assignments
                                    //CHECK: is this selecting right?
                                    on asscat.CategoryId equals ass.CategoryId

                                    where co.SubjectAbb == subject
                                    where co.CourseNumber == num.ToString()
                                    where c.Season == season
                                    where c.Year == year
                                    where asscat.Name == category
                                    where ass.Name == asgname

                                    select ass).FirstOrDefault();

            var query_submission = (from sub in db.Submissions
                                    //CHECK: check if submission id is the equivalent of assignment id
                                    where sub.SubmissionId == query_assignment.AssignmentId
                                    select sub).FirstOrDefault();

            //if there has been no submission yet, set the submission
            if (query_submission == null)
            {
                Submission subm = new Submission();

                subm.SubmissionTime = DateTime.Now;
                //set score to 0
                subm.Score = 0;
                subm.SubmissionContents = contents;
                subm.UId = uid;
                //CHECK: may be a good idea to set submission id = to assignment id so they're connected that way
                subm.SubmissionId = query_assignment.AssignmentId;

                //add to database
                db.Submissions.Add(subm);

                submitted = true;
            }

            //if the assignment has already been submitted once
            else
            {
                query_submission.SubmissionContents = contents;
                query_submission.SubmissionTime = DateTime.Now;

                submitted = true;
            }

            db.SaveChanges();

            return Json(new { success = submitted });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// 
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// 
        /// <returns>A JSON object containing {success = {true/false}.
        /// 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        /// 
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            bool studentEnrolled = false;

            var check_enrollment = (from e in db.Enrollments

                                    join cl in db.Classes
                                    on e.ClassId equals cl.ClassId

                                    join co in db.Courses
                                    on cl.CatalogId equals co.CatalogId

                                    where e.UId == uid
                                    where co.SubjectAbb == subject
                                    where co.CourseNumber == num.ToString()
                                    where cl.Season == season
                                    where cl.Year == year

                                    select e).FirstOrDefault();

            if (check_enrollment == null)
            {
                Enrollment enrollStudent = new Enrollment();

                enrollStudent.UId = uid;
                //set grade to be equal to null that will look good in the table ( -- )
                enrollStudent.Grade = "--";
                enrollStudent.ClassId = check_enrollment.ClassId;

                studentEnrolled = true;

                db.Enrollments.Add(enrollStudent);
                db.SaveChanges(); 
            }

            //else student is already enrolled
            else
            {
                studentEnrolled = false; 
            }

            return Json(new { success = studentEnrolled});
        }



        /// <summary>
        /// Calculates a student's GPA
        /// 
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        ///
        /// Assume all classes are 4 credit hours.
        /// 
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// 
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// 
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// 
        /// </summary>
        /// 
        /// <param name="uid">The uid of the student</param>
        /// 
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        /// 
        public IActionResult GetGPA(string uid)
        {
            double studentGrades = 0.0;
            int num_of_grades = 0;

            var get_grades = from e in db.Enrollments
                             where e.UId == uid
                             select e.Grade;

            foreach (var grade in get_grades)
            {
                if (grade == "A")
                {
                    studentGrades += 4.0;
                    num_of_grades++;
                    break;
                }

                else if (grade == "A-")
                {
                    studentGrades += 3.7;
                    num_of_grades++;
                    break;
                }

                else if (grade == "B+")
                {
                    studentGrades += 3.3;
                    num_of_grades++;
                    break;
                }

                else if (grade == "B")
                {
                    studentGrades += 3.0;
                    num_of_grades++;
                    break;
                }

                else if (grade == "B-")
                {
                    studentGrades += 2.7;
                    num_of_grades++;
                    break;
                }

                else if (grade == "C+")
                {
                    studentGrades += 2.3;
                    num_of_grades++;
                    break;
                }

                else if (grade == "C")
                {
                    studentGrades += 2.0;
                    num_of_grades++;
                    break;
                }

                else if (grade == "C-")
                {
                    studentGrades += 1.7;
                    num_of_grades++;
                    break;
                }

                else if (grade == "D+")
                {
                    studentGrades += 1.3;
                    num_of_grades++;
                    break;
                }

                else if (grade == "D")
                {
                    studentGrades += 1.0;
                    num_of_grades++;
                    break;
                }

                else if (grade == "D-")
                {
                    studentGrades += 0.7;
                    num_of_grades++;
                    break;
                }

                else if (grade == "E")
                {
                    studentGrades += 0.0;
                    num_of_grades++;
                    break;
                }
            }

            double cumulativevGPA = (studentGrades / num_of_grades); 

            return Json(cumulativevGPA);
        }
                
        /*******End code to modify********/

    }
}

