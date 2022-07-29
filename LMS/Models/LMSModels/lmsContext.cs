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
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignment_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId, "AssignmentCategories_Classes_classID_fk");

                entity.HasIndex(e => new { e.Name, e.ClassId }, "AssignmentCategories_key")
                    .IsUnique();

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever()
                    .HasColumnName("categoryID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.GradingWeight)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("gradingWeight");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_Classes_classID_fk");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => e.ProfessorUId, "Classes_Professors_uID_fk");

                entity.HasIndex(e => new { e.CatalogId, e.Season }, "catalogID")
                    .IsUnique();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever()
                    .HasColumnName("classID");

                entity.Property(e => e.CatalogId)
                    .HasMaxLength(5)
                    .HasColumnName("catalogID")
                    .IsFixedLength();

                entity.Property(e => e.EndTime)
                    .HasColumnType("time")
                    .HasColumnName("endTime");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.ProfessorUId)
                    .HasMaxLength(8)
                    .HasColumnName("professor_uID")
                    .IsFixedLength();

                entity.Property(e => e.Season)
                    .HasMaxLength(6)
                    .HasColumnName("season");

                entity.Property(e => e.StartTime)
                    .HasColumnType("time")
                    .HasColumnName("startTime");

                entity.Property(e => e.Year)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("year");

                entity.HasOne(d => d.Catalog)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_Courses_catalogID_fk");

                entity.HasOne(d => d.ProfessorU)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ProfessorUId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_Professors_uID_fk");
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
                entity.HasKey(e => new { e.UId, e.ClassId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("Enrollment");

                entity.HasIndex(e => e.ClassId, "Enrollment_Classes_classID_fk");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Grade)
                    .HasMaxLength(2)
                    .HasColumnName("grade");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_Classes_classID_fk");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_Students_uID_fk");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.SubjectAbb, "Professors_Departments_subjectAbb_fk");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.SubjectAbb)
                    .HasMaxLength(4)
                    .HasColumnName("subjectAbb")
                    .IsFixedLength();

                entity.HasOne(d => d.SubjectAbbNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.SubjectAbb)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_Departments_subjectAbb_fk");

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Professor)
                    .HasForeignKey<Professor>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major, "Students_Departments_subjectAbb_fk");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Major)
                    .HasMaxLength(4)
                    .HasColumnName("major")
                    .IsFixedLength();

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_Departments_subjectAbb_fk");

                entity.HasOne(d => d.UIdNavigation)
                    .WithOne(p => p.Student)
                    .HasForeignKey<Student>(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_Users_uID_fk");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasIndex(e => e.ClassId, "Submissions_ibfk_2");

                entity.HasIndex(e => e.UId, "uID");

                entity.Property(e => e.SubmissionId)
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever()
                    .HasColumnName("submissionID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Score)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("score");

                entity.Property(e => e.SubmissionContents)
                    .HasMaxLength(8192)
                    .HasColumnName("submissionContents");

                entity.Property(e => e.SubmissionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("submissionTime");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("DOB");

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
