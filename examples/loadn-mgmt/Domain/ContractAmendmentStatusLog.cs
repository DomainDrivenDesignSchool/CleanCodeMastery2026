namespace LoanManagementSystem.Domain;

public partial class ContractAmendmentStatusLog
{
    /// <summary>
    /// Primary key
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// Amendment request ID
    /// </summary>
    public long? AmendmentRequestId { get; set; }
    /// <summary>
    /// Stage ID
    /// </summary>
    public long? StageId { get; set; }
    /// <summary>
    /// Change date
    /// </summary>
    public DateTime? ChangeDate { get; set; }
    /// <summary>
    /// Comments
    /// </summary>
    public string Comments { get; set; }
    /// <summary>
    /// Officer who made the change
    /// </summary>
    public int? ProcessingOfficerId { get; set; }

    public virtual ContractAmendmentRequest AmendmentRequest { get; set; }
    public virtual LoanStage Stage { get; set; }
}