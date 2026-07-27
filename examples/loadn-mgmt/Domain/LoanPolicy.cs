namespace LoanManagementSystem.Domain;

public partial class LoanPolicy
{
    public long Id { get; set; }
    public byte? AssetCategoryId { get; set; }
    public string PolicyIdentifier { get; set; }
    public DateTime? CreationDate { get; set; }
    public int? CreatedByOfficerId { get; set; }
    public DateTime? DeactivationDate { get; set; }
    public int? DeactivatedByOfficerId { get; set; }
    public int? ProcessingMode { get; set; }
}