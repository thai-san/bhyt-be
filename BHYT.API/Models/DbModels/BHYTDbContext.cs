using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BHYT.API.Models.DbModels;

public partial class BHYTDbContext : DbContext
{
    public BHYTDbContext(DbContextOptions<BHYTDbContext> options): base(options)
    {
    }
    public virtual DbSet<Benefit> Benefits { get; set; }

    public virtual DbSet<Compensation> Compensations { get; set; }

    public virtual DbSet<CustomerPolicy> CustomerPolicies { get; set; }

    public virtual DbSet<HealthHistory> HealthHistories { get; set; }

    public virtual DbSet<Insurance> Insurances { get; set; }

    public virtual DbSet<InsurancePayment> InsurancePayments { get; set; }

    public virtual DbSet<InsuranceRequired> InsuranceRequireds { get; set; }

    public virtual DbSet<InsuranceType> InsuranceTypes { get; set; }

    public virtual DbSet<PolicyApproval> PolicyApprovals { get; set; }

    public virtual DbSet<ResetPasswordRequest> ResetPasswordRequests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<HealthIndicator> HealthIndicators { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.Entity<HealthIndicator>().HasData(
           new HealthIndicator
           {
               Id = 1,
               Guid = Guid.NewGuid(),
               CustomerId = 1,
               Height = 170,
               Weight = 65,
               Cholesterol = 5.2f,
               BMI = 23.4f,
               BPM = 80,
               RespiratoryRate = 18,
               Diseases = "None",
               LastestUpdate = DateTime.Now
           },
           new HealthIndicator
           {
               Id = 2,
               Guid = Guid.NewGuid(),
               CustomerId = 2,
               Height = 165,
               Weight = 60,
               Cholesterol = 4.8f,
               BMI = 22.1f,
               BPM = 75,
               RespiratoryRate = 16,
               Diseases = "None",
               LastestUpdate = DateTime.Now
           }
       );

        modelBuilder.Entity<Benefit>().HasData(
            new Benefit { Id = 1, Guid = Guid.NewGuid(), Name = "Health Insurance", Description = "Coverage for medical expenses" },
            new Benefit { Id = 2, Guid = Guid.NewGuid(), Name = "Life Insurance", Description = "Coverage in the event of death" }
        );

        modelBuilder.Entity<Compensation>().HasData(
            new Compensation
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                PolicyId = 1,
                EmployeeId = 1,
                Date = DateTime.Now,
                Amount = 1000.50,
                Note = "Bonus payment",
                Status = true,
                HoptitalName="Từ Dũ",
                HopitalCode="294903456",
                DateRequest= DateTime.Now,
                UsedServices="all",
                GetOption= 1
            },
            new Compensation
            {
                Id = 2,
                Guid = Guid.NewGuid(),
                PolicyId = 2,
                EmployeeId = 2,
                Date = DateTime.Now,
                Amount = 750.25,
                Note = "Incentive payment",
                Status = true,
                HoptitalName = "Từ Dũ",
                HopitalCode = "294903456",
                DateRequest = DateTime.Now,
                UsedServices = "all",
                GetOption = 1

            }
        );

        modelBuilder.Entity<CustomerPolicy>().HasData(
            new CustomerPolicy
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                CustomerId = 1,
                StartDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                PremiumAmount = 1000.50,
                PaymentOption = true,
                CoverageType = "Comprehensive",
                DeductibleAmount = 500.00,
                BenefitId = 1,
                InsuranceId = 1,
                LatestUpdate = DateTime.Now,
                Description = "Policy for car insurance",
                Status = true,
                Company = "ABC Insurance"
            },
            new CustomerPolicy
            {
                Id = 2,
                Guid = Guid.NewGuid(),
                CustomerId = 2,
                StartDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                PremiumAmount = 1500.75,
                PaymentOption = false,
                CoverageType = "Third Party",
                DeductibleAmount = 1000.00,
                BenefitId = 2,
                InsuranceId = 2,
                LatestUpdate = DateTime.Now,
                Description = "Policy for home insurance",
                Status = true,
                Company = "XYZ Insurance"
            }
        );
       
        modelBuilder.Entity<HealthHistory>().HasData(
            new HealthHistory
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                CustomerId = 1,
                InsuranceId = 1,
                CreatedDate = DateTime.Now,
                Detail = "Patient's health history detail",
                Note = "Additional notes about the health history",
                Diagnostic = "Diagnosis of the patient's condition",
                HospitalNumber = "123456789",
                Condition = "health condition"
            },
            new HealthHistory
            {
                Id = 2,
                Guid = Guid.NewGuid(),
                CustomerId = 2,
                InsuranceId = 2,
                CreatedDate = DateTime.Now,
                Detail = "Patient's health history detail",
                Note = "Additional notes about the health history",
                Diagnostic = "Diagnosis of the patient's condition",
                HospitalNumber = "987654321",
                Condition = "health condition"
            }
            );
        modelBuilder.Entity<Insurance>().HasData(
            new Insurance
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                Name = "Term Life Insurance",
                Description = "Provides coverage for a specific term or period of time",
                InsuranceTypeId = "1",
                StartAge = 18,
                EndAge = 65
            },
            new Insurance
            {
                Id = 2,
                Guid = Guid.NewGuid(),
                Name = "Family Health Insurance",
                Description = "Covers medical expenses for the entire family",
                InsuranceTypeId = "2",
                StartAge = 0,
                EndAge = 99
            }
        );

        modelBuilder.Entity<InsurancePayment>().HasData(
            new InsurancePayment
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                CustomerId = 1,
                PolicyId = 1,
                Date = DateTime.Now,
                Amount = 1000.50,
                Status = true,
                Type = "Payment",
                Note = "Payment for insurance policy"
            },
            new InsurancePayment
            {
                Id = 2,
                Guid = Guid.NewGuid(),
                CustomerId = 2,
                PolicyId = 2,
                Date = DateTime.Now,
                Amount = 1500.75,
                Status = true,
                Type = "Payment",
                Note = "Payment for insurance policy"
            }
        );

        modelBuilder.Entity<InsuranceRequired>().HasData(
        new InsuranceRequired
        {
            Id = 1,
            Guid = Guid.NewGuid(),
            PolicyId = 1,
            Status = 1,
            Date = DateTime.Now,
            Amount = 500.50,
            MedicalServiceName = "Medical Checkup",
            ServiceDescription = "Required annual medical checkup",
            Note = "Please schedule the appointment."
        },
        new InsuranceRequired
        {
            Id = 2,
            Guid = Guid.NewGuid(),
            PolicyId = 2,
            Status = 1,
            Date = DateTime.Now,
            Amount = 1000.75,
            MedicalServiceName = "Diagnostic Tests",
            ServiceDescription = "Required diagnostic tests for policy renewal",
            Note = "Please complete the tests by the specified date."
        }
        );

        modelBuilder.Entity<InsuranceType>().HasData(
            new InsuranceType
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                Name = "Life Insurance",
                Description = "Provides coverage for the risk of life"
            },
            new InsuranceType
            {
                Id = 2,
                Guid = Guid.NewGuid(),
                Name = "Health Insurance",
                Description = "Covers medical expenses and healthcare services"
            }
        );

        modelBuilder.Entity<PolicyApproval>().HasData(
            new PolicyApproval
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                PolicyId = 1,
                EmployeeId = 1,
                ApprovalDate = DateTime.Now,
                StatusId = 1
            },
            new PolicyApproval
            {
                Id = 2,
                Guid = Guid.NewGuid(),
                PolicyId = 2,
                EmployeeId = 2,
                ApprovalDate = DateTime.Now,
                StatusId = 2
            }
        );
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Guid = Guid.NewGuid(), Name = "employee" },
            new Role { Id = 2, Guid = Guid.NewGuid(), Name = "customer" }
        );
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Guid = Guid.NewGuid(), Name = true },
            new Status { Id = 2, Guid = Guid.NewGuid(), Name = false }
        );

        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
