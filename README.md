# LMS Project Phase 3: LINQ Queries

In Phase 3, you will implement the back-end controllers that drive the web server. Some starting code has been provided, including views.

## Setup

The skeleton code is in this repo.  It's probably easiest to clone the repo to work with it locally, and then create add update the remote to a shared repo and you and your partner can work in (and you add me/the TAs to)

Both team members should be present, but only perform the below setup steps once.

### Login Database Setup

Since we are letting .NET handle user logins and permissions, we will need to point the project to a new database that it can fill in with its own user tables. These tables will store login information and hashed passwords. I have created another database for each team on atr.eng.utah.edu called `TeamXLMSUsers`, where X is your team number.

In the project directory of the skeleton code (the project directory, not the solution directory!), set up your database connection string using the user-secrets API:

```
dotnet user-secrets init
dotnet user-secrets set "LMS:IdentityConnectionString" "database=YOURUSERSDB;uid=YOURUID;password=YOURPASSWORD;server=atr.eng.utah.edu"
```

In Program.cs, we reference this user-secret to get the connection string for the Identity database

Next, run the following comands to set the DB up:

```
~/.dotnet/tools/dotnet-ef database update #may not be necessary/do anything...
~/.dotnet/tools/dotnet-ef migrations add CreateIdentitySchema # produces a set of SQL commands to build the appropriate tables in your database
~/.dotnet/tools/dotnet-ef database update # should actually run the commands to setup the DB
```


After it completes, you should see a bunch of auto-generated tables in your `TeamXLMSUsers` database. Verify this.

You should be able to run the tool at this point, register and it should say you're logged in as asdf.



## Scaffolding

*Before Scaffolding* - if you used the SQL 'YEAR' type in any of your columns, change it to INT UNSIGNED. The scaffolding tool does not know how to interpret YEAR types. 

In this project you will have to scaffold your LMS tables to get the C# models, similar to the scaffolding you did in Lab 8. 

We'll again, use the dotnet user-secrets API to pass in our connection string:

```
dotnet user-secrets set "LMS:LMSConnectionString" "database=TeamXLibrary;uid=YOURUID;password=YOURPASSWORD;server=atr.eng.utah.edu"
```

Note the secret name, and the database fields are different from the prevoius example!

Now run the scaffolder:

```
~/.dotnet/tools/dotnet-ef dbcontext Scaffold Name="LMS:LMSConnectionString" --context LMSContext Pomelo.EntityFrameworkCore.MySql -o Models/LMSModels
```

Note: The output directory is Models/LMSModels. This is to keep the code structure more organized, since there are other types of models in this project used by the front-end. 

After running the scaffolder, you should now have a class in Models/LMSModels/LMSContext.  To get the Dependency Injection system to automatically create one of these objects when needed, connected to the appropriate database add:

```var lmsConnectionString = builder.Configuration["LMS:LMSConnectionString"];
        builder.Services.AddDbContext<lmsContext>(options =>
            options.UseMySql(lmsConnectionString, ServerVersion.AutoDetect(lmsConnectionString)));            
```

After the similar lines in `Program.cs` that set up the Identity database.

You should be able to run the web server now. Try it.  If you get build errors related to the name of your context class, you can update the name of the class in the controller classes and program.cs.

If you modify your database schema (maybe you forgot a field an want to run an `ALTER TABLE ADD COLUMN` command, for example), you can rerun the scaffolder and add the `-f` at the end of the command and it will overwrite the classes in LMSModels with updated versions.  You shouldn't need to edit those files yourself, so you shouldn't use any work by having the scaffolder recreate them, but I'd recommend commiting and pushing your work to github before rerunning the scaffolder.  You can see what changed after rerunning it with `git diff`.  The changes should make sense (adding a new property to a class, etc).

## Data Types

While scaffolding, you might have seen some warnings that it skipped certain columns; most likely date-based columns. The pomelo scaffolding tool does not recognize these types. The best way to fix this is to change your SQL types as suggested above for YEAR types above. 

## Controllers

There are a number of controller methods left for you to fill in. These methods are defined in:

* CommonController.cs
* ProfessorController.cs
* StudentController.cs
* AdministratorController.cs

There's also one method you'll need to fill in in `Areas/Identity/Pages/Account/Register.cshtml.cs`.  The `CreateNewUser` method is probably the first method you'll want to write because you'll need it to register users in the Administrator/Professor/Student roles in order to actually run/test anything else.  

The methods for you to write in these files are below a comment that looks like this:

`/*******Begin code to modify********/`

The intended functionality of these methods is documented in comments above the method. Fill in the implementation according to those comments.

Your first step should be to fill in the CreateNewUser method in Register.cshtml.cs.  

Your next step should be to fill in AdminController.CreateDepartment, and CommonController.GetDepartments. This will allow the  admin to create departments, and the registration page to see the departments for creating a new professor/student.

## Users in Two Places

Since we are using ASP.NET user identities (setup in an earlier step), we will have two places where users are "stored". You should never directly do anything with the TeamXLMSUsers tables; all of that is automatically handled. You do, however, need to create your own Student, Professor, or Administrator that matches the uID when a new user is created. ASP.NET identities will handle authentication and login in its own tables (TeamXLMSUsers), and your own tables will handle things like who is enrolled in what class, and who works for which department, etc.

While debugging, things may get out of sync in your tables between the dotnet Identity stuff and your own user tables.  You may want to delete the entries from the relevant tables in **both** database while debugging, especially since your logic for picking a new UID probably won't check what's in the dotnet Identity table...

## What Next?

There are a lot of controllers for you to fill in, and the order that you do so is up to you. We recommend starting with Administrator.

To figure out when/why each of the controllers get invoked, we recommend putting a breakpoint on every controller you are supposed to implement, and see which parts of the web page invoke which controller(s). As you start to understand each controller, remove its breakpoint.

If the appropriate controllers are not yet implemented, various parts of the web page will either not display at all, or not display correctly, or report an error.

## "Auto-Grading"

In this LMS, letter grades for a student's enrollment will be automatically calculated based on assignment scores and category weights.

Grades will be automatically updated and saved in the database whenever either of the two actions occur:

* A professor scores a student's submission. The student's grade for the class should be updated. Modify your GradeSubmission method to have this side-effect.
* A professor creates a new assignment. The grades of all students in the class should be updated. Modify your CreateAssignment method to have this side-effect.

Letter grades should be calculated as follows:

* If a student does not have a submission for an assignment, the score for that assignment is treated as 0.
* If an AssignmentCategory does not have any assignments, it is not included in the calculation.
* For each non-empty category in the class:
* Calculate the percentage of (total points earned / total max points) of all assignments in the category. This should be a number between 0.0 - 1.0. For example, if there are 875 possible points spread among various assignments in the category, and the student earned a total of 800 among all assignments in that category, this value should be ~0.914
* Multiply the percentage by the category weight. For example, if the category weight is 15, then the scaled total for the category is ~13.71 (using the previous example).
* Compute the total of all scaled category totals from the previous step. WARNING - there is no rule that assignment category weights must sum to 100. Therefore, we have to re-scale in the next step.
* Compute the scaling factor to make all category weights add up to 100%. This scaling factor is 100 / (sum of all category weights).
* Multiply the total score you computed in step 4 by the scaling factor you computed in step 5. This is the total percentage the student earned in the class.
* Convert the class percentage to a letter grade using the following scale:  grade >= 92 = A, >= 90 = A-, >= 87 = B+, >= 82 = B, >= 80 = B-, etc.  Any grade lower than a 60 is an E.
* After you have calculated a grade, remember to update the grade saved in the database!

Obviously, you should make a helper method for calculating a student's grade. You can add a private method to the appropriate controller class for this.  

## Test Data

For testing you can either fill in the controllers and use the web functionality to add data, or you can directly manipulate the databse with SQL commands.

To test the controllers, you'll probably want:

* At least two departments
* One or two Courses for each department
* At least one Class of each Course (the professor for the class should be created through the UI, not manually)
* Enroll one or two students in some classes (the students should be created through the UI, not manually)
* At least two categories and 1-2 assignments per category
* Student submissions for the assignments

## Partners

Both partners should be able to log in to the database by setting the appropriate dotnet user-secret values.



## Handin

Add the TAs to the repo with your solution in it.

In Phase 4, you will deploy your web server to a Linux machine online. 

## Notes

This section will contain hints and clarifications, and will be expanded as needed.

Feel free to add private methods (and private static methods) to controllers to implement helper functions.

If you need to do a left join, linq requires the 'on' ... 'equals' syntax, and you can't use where, and you can't use && or || to make a complex filter. Since you can only use one 'equals', to make a complex filter, you just need to compare two complex objects. As an example, consider the GetAssignmentsInClass controller. Suppose I have a variable called query1 which holds all of the assignments for a given class. I need to join that with the Submissions table where the assignment ID matches the submission ID, AND the student ID of the submission matches the student ID provided to the controller. If the submission doesn't exist for the student, I need to keep the assignment row, but pass null for the score (this is why we need a left join). To do this complex filter, I create two dynamic objects that hold the assignment IDs and student IDs that need to match, as so: 
var query2 =
from q in query1  // query1 holds the assignments for the class
join s in db.Submissions
on new { A = q.AssignmentId, B = uid } equals new { A = s.Assignment, B = s.Student }
into joined
from j in joined.DefaultIfEmpty()
...
Within an assignment category, assignment names must be unique. This was not specified in the Phase2 requirements, so you will have to enforce it in software. Return success=false from the CreateAssignment controller if the user tries to create a duplicate.