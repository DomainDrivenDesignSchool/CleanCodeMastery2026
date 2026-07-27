using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LoanManagementSystem.Domain;
using LoanManagement.Domain;

namespace LoanManagementSystem;

public class LoanManagementDbContext : DbContext
{
    private readonly string _connectionString;

    public LoanManagementDbContext()
    {
        _connectionString = GetConnectionString();
    }

    public LoanManagementDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    private static string GetConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return configuration.GetConnectionString("DefaultConnection");
    }

    public DbSet<ContractAmendmentRequest> ContractAmendmentRequests => Set<ContractAmendmentRequest>();
    public DbSet<ContractAmendmentStatusLog> ContractAmendmentStatusLogs => Set<ContractAmendmentStatusLog>();
    public DbSet<LoanContract> LoanContracts => Set<LoanContract>();
    public DbSet<VehicleAsset> VehicleAssets => Set<VehicleAsset>();
    public DbSet<ApplicantNationalId> ApplicantNationalIds => Set<ApplicantNationalId>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<BankBranch> BankBranches => Set<BankBranch>();
    public DbSet<OperationalRegion> OperationalRegions => Set<OperationalRegion>();
    public DbSet<LoanStage> LoanStages => Set<LoanStage>();
    public DbSet<LoanPolicy> LoanPolicies => Set<LoanPolicy>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(applicant => applicant.ApplicantId);
        });

        modelBuilder.Entity<ApplicantNationalId>(entity =>
        {
            entity.HasKey(record => record.NationalIdRecordId);
        });

        modelBuilder.Entity<VehicleAsset>(entity =>
        {
            entity.HasKey(asset => asset.AssetId);
            entity.HasOne(asset => asset.NationalCode)
                .WithMany(code => code.VehicleAssets)
                .HasForeignKey(asset => asset.NationalCodeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OperationalRegion>(entity =>
        {
            entity.HasKey(region => region.OperationalRegionId);
        });

        modelBuilder.Entity<BankBranch>(entity =>
        {
            entity.HasKey(branch => branch.BankBranchId);
            entity.HasOne(branch => branch.OperationalRegion)
                .WithMany(region => region.BankBranches)
                .HasForeignKey(branch => branch.OperationalRegionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LoanContract>(entity =>
        {
            entity.HasKey(contract => contract.ContractId);
            entity.HasOne(contract => contract.Applicant)
                .WithMany(applicant => applicant.LoanContracts)
                .HasForeignKey(contract => contract.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(contract => contract.Asset)
                .WithMany(asset => asset.LoanContracts)
                .HasForeignKey(contract => contract.AssetInfoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(contract => contract.Branch)
                .WithMany(branch => branch.LoanContracts)
                .HasForeignKey(contract => contract.BankBranchId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LoanStage>(entity =>
        {
            entity.HasKey(stage => stage.Id);
        });

        modelBuilder.Entity<ContractAmendmentRequest>(entity =>
        {
            entity.HasKey(request => request.Id);
            entity.HasOne(request => request.Contract)
                .WithMany(contract => contract.ContractAmendmentRequests)
                .HasForeignKey(request => request.ContractId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(request => request.Region)
                .WithMany(region => region.ContractAmendmentRequests)
                .HasForeignKey(request => request.OperationalRegionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(request => request.FinalStatus)
                .WithMany(stage => stage.ContractAmendmentRequests)
                .HasForeignKey(request => request.FinalStatusId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(request => request.StatusChangeLogs)
                .WithOne(log => log.AmendmentRequest)
                .HasForeignKey(log => log.AmendmentRequestId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ContractAmendmentStatusLog>(entity =>
        {
            entity.HasKey(log => log.Id);
            entity.HasOne(log => log.Stage)
                .WithMany(stage => stage.ContractAmendmentStatusLogs)
                .HasForeignKey(log => log.StageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LoanPolicy>(entity =>
        {
            entity.HasKey(policy => policy.Id);
        });
    }
}