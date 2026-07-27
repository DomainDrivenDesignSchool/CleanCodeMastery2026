namespace LoanManagementSystem.Domain;

public partial class LoanStage
{
    public LoanStage()
    {
        ContractAmendmentStatusLogs = new HashSet<ContractAmendmentStatusLog>();
        ContractAmendmentRequests = new HashSet<ContractAmendmentRequest>();
    }

    /// <summary>
    /// Primary key
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// Stage category
    /// </summary>
    public long? StageCategoryId { get; set; }
    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Unique key
    /// </summary>
    public int? UniqueKey { get; set; }
    /// <summary>
    /// Active / Inactive
    /// </summary>
    public bool? IsActive { get; set; }

    public virtual ICollection<ContractAmendmentStatusLog> ContractAmendmentStatusLogs { get; set; }
    public virtual ICollection<ContractAmendmentRequest> ContractAmendmentRequests { get; set; }
}