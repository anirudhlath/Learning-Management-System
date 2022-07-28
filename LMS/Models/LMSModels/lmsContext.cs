using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Enrollment> Enrollments { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentMajorsIn> StudentMajorsIns { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.1.48-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Administrator)
                    .HasForeignKey<Administrator>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Administrators_ibfk_1");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("Assignment");

                entity.HasIndex(e => new { e.Name, e.CategoryId }, "Assignment_uk")
                    .IsUnique();

                entity.HasIndex(e => e.CategoryId, "categoryID");

                entity.Property(e => e.AssignmentId)
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever()
                    .HasColumnName("assignmentID");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("categoryID");

                entity.Property(e => e.Content)
                    .HasMaxLength(8192)
                    .HasColumnName("content");

                entity.Property(e => e.DueDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dueDateTime");

                entity.Property(e => e.MaxPoints)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("maxPoints");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Assignments)
                    .HasPrincipalKey(p => p.CategoryId)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignment_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => new { e.CatalogId, e.Semester, e.Section, e.Name })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

                entity.HasIndex(e => e.CategoryId, "AssignmentCategories_categoryID_uindex")
                    .IsUnique();

                entity.Property(e => e.CatalogId)
                    .HasMaxLength(5)
                    .HasColumnName("catalogID")
                    .IsFixedLength();

                entity.Property(e => e.Semester)
                    .HasMaxLength(11)
                    .HasColumnName("semester");

                entity.Property(e => e.Section)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("section");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("categoryID");

                entity.Property(e => e.GradingWeight)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("gradingWeight");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => new { d.CatalogId, d.Semester, d.Section })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_Classes_catalogID_semester_section_fk");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(e => new { e.CatalogId, e.Semester, e.Section })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.HasIndex(e => e.ClassId, "Classes_pk")
                    .IsUnique();

                entity.HasIndex(e => new { e.Location, e.StartTime }, "time_location")
                    .IsUnique();

                entity.HasIndex(e => new { e.Location, e.EndTime }, "time_location_2")
                    .IsUnique();

                entity.Property(e => e.CatalogId)
                    .HasMaxLength(5)
                    .HasColumnName("catalogID")
                    .IsFixedLength();

                entity.Property(e => e.Semester)
                    .HasMaxLength(11)
                    .HasColumnName("semester");

                entity.Property(e => e.Section)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("section");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time")
                    .HasColumnName("endTime");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.StartTime)
                    .HasColumnType("time")
                    .HasColumnName("startTime");

                entity.HasOne(d => d.Catalog)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_Courses_catalogID_fk");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CatalogId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.SubjectAbb, "Courses_Departments_subjectAbb_fk");

                entity.HasIndex(e => new { e.CourseNumber, e.SubjectAbb }, "Courses_uk")
                    .IsUnique();

                entity.Property(e => e.CatalogId)
                    .HasMaxLength(5)
                    .HasColumnName("catalogID")
                    .IsFixedLength();

                entity.Property(e => e.CourseName)
                    .HasMaxLength(100)
                    .HasColumnName("courseName")
                    .IsFixedLength();

                entity.Property(e => e.CourseNumber)
                    .HasMaxLength(4)
                    .HasColumnName("courseNumber")
                    .IsFixedLength();

                entity.Property(e => e.SubjectAbb)
                    .HasMaxLength(4)
                    .HasColumnName("subjectAbb")
                    .IsFixedLength();

                entity.HasOne(d => d.SubjectAbbNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.SubjectAbb)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_Departments_subjectAbb_fk");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.SubjectAbb)
                    .HasName("PRIMARY");

                entity.Property(e => e.SubjectAbb)
                    .HasMaxLength(4)
                    .HasColumnName("subjectAbb")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.CatalogId, e.Semester })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.ToTable("Enrollment");

                entity.HasIndex(e => new { e.CatalogId, e.Semester }, "Students_ibfk_2");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.CatalogId)
                    .HasMaxLength(5)
                    .HasColumnName("catalogID")
                    .IsFixedLength();

                entity.Property(e => e.Semester)
                    .HasMaxLength(11)
                    .HasColumnName("semester");

                entity.Property(e => e.Grade)
                    .HasMaxLength(2)
                    .HasColumnName("grade");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Professor)
                    .HasForeignKey<Professor>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");

                entity.HasMany(d => d.Classes)
                    .WithMany(p => p.UIds)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProfessorTeachesIn",
                        l => l.HasOne<Class>().WithMany().HasPrincipalKey("ClassId").HasForeignKey("ClassId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("ProfessorTeachesIn_ibfk_2"),
                        r => r.HasOne<Professor>().WithMany().HasForeignKey("UId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("ProfessorTeachesIn_ibfk_1"),
                        j =>
                        {
                            j.HasKey("UId", "ClassId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("ProfessorTeachesIn");

                            j.HasIndex(new[] { "ClassId" }, "classID");

                            j.IndexerProperty<string>("UId").HasMaxLength(8).HasColumnName("uID").IsFixedLength();

                            j.IndexerProperty<uint>("ClassId").HasColumnType("int(10) unsigned").HasColumnName("classID");
                        });

                entity.HasMany(d => d.SubjectAbbs)
                    .WithMany(p => p.UIds)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProfessorWorksIn",
                        l => l.HasOne<Department>().WithMany().HasForeignKey("SubjectAbb").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("ProfessorWorksIn_ibfk_2"),
                        r => r.HasOne<Professor>().WithMany().HasForeignKey("UId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("ProfessorWorksIn_ibfk_1"),
                        j =>
                        {
                            j.HasKey("UId", "SubjectAbb").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("ProfessorWorksIn");

                            j.HasIndex(new[] { "SubjectAbb" }, "subjectAbb");

                            j.IndexerProperty<string>("UId").HasMaxLength(8).HasColumnName("uID").IsFixedLength();

                            j.IndexerProperty<string>("SubjectAbb").HasMaxLength(4).HasColumnName("subjectAbb").IsFixedLength();
                        });
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Student)
                    .HasForeignKey<Student>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_Users_uID_fk");
            });

            modelBuilder.Entity<StudentMajorsIn>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.ToTable("StudentMajorsIn");

                entity.HasIndex(e => e.SubjectAbb, "StudentMajorsIn_1_Departments_subjectAbb_fk");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.SubjectAbb)
                    .HasMaxLength(4)
                    .HasColumnName("subjectAbb")
                    .IsFixedLength();

                entity.HasOne(d => d.SubjectAbbNavigation)
                    .WithMany(p => p.StudentMajorsIns)
                    .HasForeignKey(d => d.SubjectAbb)
                    .HasConstraintName("StudentMajorsIn_1_Departments_subjectAbb_fk");

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.StudentMajorsIn)
                    .HasForeignKey<StudentMajorsIn>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("StudentMajorsIn_1_Students_uID_fk");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.SubmissionId, e.UId, e.CatalogId, e.Semester })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

                entity.HasIndex(e => new { e.CatalogId, e.Semester, e.Section }, "catalogID");

                entity.HasIndex(e => e.UId, "uID");

                entity.Property(e => e.SubmissionId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("submissionID");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.CatalogId)
                    .HasMaxLength(5)
                    .HasColumnName("catalogID")
                    .IsFixedLength();

                entity.Property(e => e.Semester)
                    .HasMaxLength(11)
                    .HasColumnName("semester");

                entity.Property(e => e.Score)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("score");

                entity.Property(e => e.Section)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("section");

                entity.Property(e => e.SubmissionContents)
                    .HasMaxLength(8192)
                    .HasColumnName("submissionContents");

                entity.Property(e => e.SubmissionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("submissionTime");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => new { d.CatalogId, d.Semester, d.Section })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("firstName");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("lastName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
