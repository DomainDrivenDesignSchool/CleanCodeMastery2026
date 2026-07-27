namespace LoanManagementSystem.Domain
{
    public partial class ApplicantNationalId
    {
        public ApplicantNationalId()
        {
            VehicleAssets = new HashSet<VehicleAsset>();
        }

        public long NationalIdRecordId { get; set; }
        public string NationalId { get; set; }
        public string CreditScoreId { get; set; }
        public string LoanRiskClassId { get; set; }
        public short? CreditHistoryYears { get; set; }
        public DateTime? CreditReportDate { get; set; }
        public string CreditRating { get; set; }

        public virtual ICollection<VehicleAsset> VehicleAssets { get; set; }
    }
}