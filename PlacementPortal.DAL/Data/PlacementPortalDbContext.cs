using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PlacementPortal.DAL.Models;

namespace PlacementPortal.DAL.Data;

public partial class PlacementPortalDbContext : DbContext
{
    public PlacementPortalDbContext(DbContextOptions<PlacementPortalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aggregatedcounter> Aggregatedcounters { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<CompanyJobListing> CompanyJobListings { get; set; }

    public virtual DbSet<CompanyProfile> CompanyProfiles { get; set; }

    public virtual DbSet<CompanyVisit> CompanyVisits { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Hash> Hashes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<Jobparameter> Jobparameters { get; set; }

    public virtual DbSet<Jobqueue> Jobqueues { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Lock> Locks { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationUserMapping> NotificationUserMappings { get; set; }

    public virtual DbSet<PlacedStudent> PlacedStudents { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schema> Schemas { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<StudentDetail> StudentDetails { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aggregatedcounter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aggregatedcounter_pkey");

            entity.ToTable("aggregatedcounter", "hangfire");

            entity.HasIndex(e => e.Key, "aggregatedcounter_key_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.ActionByEmail)
                .HasMaxLength(255)
                .HasColumnName("action_by_email");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.EntityId)
                .HasMaxLength(150)
                .HasColumnName("entity_id");
            entity.Property(e => e.EntityName)
                .HasMaxLength(150)
                .HasColumnName("entity_name");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<CompanyJobListing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_job_listings_pkey");

            entity.ToTable("company_job_listings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CompanyVisitId).HasColumnName("company_visit_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.GeneratedKey)
                .HasMaxLength(100)
                .HasColumnName("generated_key");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.JobDomain)
                .HasMaxLength(100)
                .HasColumnName("job_domain");
            entity.Property(e => e.JobProfile)
                .HasMaxLength(100)
                .HasColumnName("job_profile");
            entity.Property(e => e.MaximumSalary)
                .HasPrecision(10, 2)
                .HasColumnName("maximum_salary");
            entity.Property(e => e.MinimumSalary)
                .HasPrecision(10, 2)
                .HasColumnName("minimum_salary");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.SalaryUnit)
                .HasMaxLength(50)
                .HasColumnName("salary_unit");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyJobListings)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("company_job_listings_company_id_fkey");

            entity.HasOne(d => d.CompanyVisit).WithMany(p => p.CompanyJobListings)
                .HasForeignKey(d => d.CompanyVisitId)
                .HasConstraintName("company_visit_job_listing_fkey");
        });

        modelBuilder.Entity<CompanyProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_profiles_pkey");

            entity.ToTable("company_profiles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyType)
                .HasMaxLength(100)
                .HasColumnName("company_type");
            entity.Property(e => e.CompanyWebsite)
                .HasMaxLength(255)
                .HasColumnName("company_website");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CompanyVisit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_visits_pkey");

            entity.ToTable("company_visits");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationEndDate).HasColumnName("application_end_date");
            entity.Property(e => e.ApplicationStartDate).HasColumnName("application_start_date");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.MaxBacklogsAllowed).HasColumnName("max_backlogs_allowed");
            entity.Property(e => e.MinCgpaRequired)
                .HasPrecision(4, 2)
                .HasColumnName("min_cgpa_required");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyVisits)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("company_visits_company_id_fkey");
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("counter_pkey");

            entity.ToTable("counter", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_counter_expireat");

            entity.HasIndex(e => e.Key, "ix_hangfire_counter_key");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.HasIndex(e => e.Name, "departments_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Hash>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("hash_pkey");

            entity.ToTable("hash", "hangfire");

            entity.HasIndex(e => new { e.Key, e.Field }, "hash_key_field_key").IsUnique();

            entity.HasIndex(e => e.Expireat, "ix_hangfire_hash_expireat");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Field).HasColumnName("field");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("job_pkey");

            entity.ToTable("job", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_job_expireat");

            entity.HasIndex(e => e.Statename, "ix_hangfire_job_statename");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Arguments)
                .HasColumnType("jsonb")
                .HasColumnName("arguments");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Invocationdata)
                .HasColumnType("jsonb")
                .HasColumnName("invocationdata");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Statename).HasColumnName("statename");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("job_applications_pkey");

            entity.ToTable("job_applications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppliedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("applied_date");
            entity.Property(e => e.ApplyStatus).HasColumnName("apply_status");
            entity.Property(e => e.CompanyListingId).HasColumnName("company_listing_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.CompanyListing).WithMany(p => p.JobApplications)
                .HasForeignKey(d => d.CompanyListingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("job_applications_company_listing_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.JobApplications)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("job_applications_student_id_fkey");
        });

        modelBuilder.Entity<Jobparameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobparameter_pkey");

            entity.ToTable("jobparameter", "hangfire");

            entity.HasIndex(e => new { e.Jobid, e.Name }, "ix_hangfire_jobparameter_jobidandname");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Jobid).HasColumnName("jobid");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Job).WithMany(p => p.Jobparameters)
                .HasForeignKey(d => d.Jobid)
                .HasConstraintName("jobparameter_jobid_fkey");
        });

        modelBuilder.Entity<Jobqueue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobqueue_pkey");

            entity.ToTable("jobqueue", "hangfire");

            entity.HasIndex(e => new { e.Jobid, e.Queue }, "ix_hangfire_jobqueue_jobidandqueue");

            entity.HasIndex(e => new { e.Queue, e.Fetchedat }, "ix_hangfire_jobqueue_queueandfetchedat");

            entity.HasIndex(e => new { e.Queue, e.Fetchedat, e.Jobid }, "jobqueue_queue_fetchat_jobid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fetchedat).HasColumnName("fetchedat");
            entity.Property(e => e.Jobid).HasColumnName("jobid");
            entity.Property(e => e.Queue).HasColumnName("queue");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("list_pkey");

            entity.ToTable("list", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_list_expireat");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Lock>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("lock", "hangfire");

            entity.HasIndex(e => e.Resource, "lock_resource_key").IsUnique();

            entity.Property(e => e.Acquired).HasColumnName("acquired");
            entity.Property(e => e.Resource).HasColumnName("resource");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.RedirectUrl).HasColumnName("redirect_url");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<NotificationUserMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_user_mapping_pkey");

            entity.ToTable("notification_user_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Notification).WithMany(p => p.NotificationUserMappings)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_user_mapping_notification_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUserMappings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_user_mapping_user_id_fkey");
        });

        modelBuilder.Entity<PlacedStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("placed_students_pkey");

            entity.ToTable("placed_students");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.JoiningDate).HasColumnName("joining_date");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.PlacedDate).HasColumnName("placed_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Company).WithMany(p => p.PlacedStudents)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("placed_students_company_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.PlacedStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("placed_students_student_id_fkey");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("resumes_pkey");

            entity.ToTable("resumes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ResumeFileUrl)
                .HasMaxLength(500)
                .HasColumnName("resume_file_url");
            entity.Property(e => e.ResumeName)
                .HasMaxLength(100)
                .HasColumnName("resume_name");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Resumes)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resumes_student_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("schema_pkey");

            entity.ToTable("schema", "hangfire");

            entity.Property(e => e.Version)
                .ValueGeneratedNever()
                .HasColumnName("version");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("server_pkey");

            entity.ToTable("server", "hangfire");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Lastheartbeat).HasColumnName("lastheartbeat");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("set_pkey");

            entity.ToTable("set", "hangfire");

            entity.HasIndex(e => e.Expireat, "ix_hangfire_set_expireat");

            entity.HasIndex(e => new { e.Key, e.Score }, "ix_hangfire_set_key_score");

            entity.HasIndex(e => new { e.Key, e.Value }, "set_key_value_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expireat).HasColumnName("expireat");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("state_pkey");

            entity.ToTable("state", "hangfire");

            entity.HasIndex(e => e.Jobid, "ix_hangfire_state_jobid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Jobid).HasColumnName("jobid");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Updatecount).HasColumnName("updatecount");

            entity.HasOne(d => d.Job).WithMany(p => p.States)
                .HasForeignKey(d => d.Jobid)
                .HasConstraintName("state_jobid_fkey");
        });

        modelBuilder.Entity<StudentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_details_pkey");

            entity.ToTable("student_details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Branch).HasColumnName("branch");
            entity.Property(e => e.Cgpa)
                .HasPrecision(4, 2)
                .HasColumnName("cgpa");
            entity.Property(e => e.Cpi)
                .HasPrecision(4, 2)
                .HasColumnName("cpi");
            entity.Property(e => e.CpiDiploma)
                .HasPrecision(4, 2)
                .HasColumnName("cpi_diploma");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EnrollmentNumber).HasColumnName("enrollment_number");
            entity.Property(e => e.EnrollmentYear).HasColumnName("enrollment_year");
            entity.Property(e => e.HscBoard)
                .HasMaxLength(100)
                .HasColumnName("hsc_board");
            entity.Property(e => e.HscPassingYear).HasColumnName("hsc_passing_year");
            entity.Property(e => e.HscPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("hsc_percentage");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.PendingBacklog).HasColumnName("pending_backlog");
            entity.Property(e => e.PlacedAlready).HasColumnName("placed_already");
            entity.Property(e => e.SscBoard)
                .HasMaxLength(100)
                .HasColumnName("ssc_board");
            entity.Property(e => e.SscPassingYear).HasColumnName("ssc_passing_year");
            entity.Property(e => e.SscPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("ssc_percentage");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.StudentDetails)
                .HasForeignKey(d => d.Branch)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_details_branch_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentDetails)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_details_student_id_fkey");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_accounts_pkey");

            entity.ToTable("user_accounts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("is_active");
            entity.Property(e => e.IsInitialLogin)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("is_initial_login");
            entity.Property(e => e.IsTwoFactorsEnabled).HasColumnName("is_two_factors_enabled");
            entity.Property(e => e.MfaSecretKey)
                .HasMaxLength(255)
                .HasColumnName("mfa_secret_key");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_accounts_role_id_fkey");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_details_pkey");

            entity.ToTable("user_details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_details_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
