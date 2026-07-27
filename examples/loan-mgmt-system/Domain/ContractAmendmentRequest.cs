using LoanManagement.Domain;

namespace LoanManagementSystem.Domain
{
    public partial class ContractAmendmentRequest
    {
        public ContractAmendmentRequest()
        {
            StatusChangeLogs = new HashSet<ContractAmendmentStatusLog>();
            RequestedBranches = new HashSet<ContractAmendmentRequestBranch>();
        }

        /// <summary>
        /// Primary key
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Request tracking code
        /// </summary>
        public long? TrackingCode { get; set; }
        /// <summary>
        /// Contract tracking code
        /// </summary>
        public long? ContractTrackingCode { get; set; }
        /// <summary>
        /// Contract ID
        /// </summary>
        public int? ContractId { get; set; }
        /// <summary>
        /// Amendment reason
        /// </summary>
        public long? AmendmentReasonId { get; set; }
        /// <summary>
        /// Effective from date
        /// </summary>
        public DateTime? EffectiveFromDate { get; set; }
        /// <summary>
        /// Effective to date
        /// </summary>
        public DateTime? EffectiveToDate { get; set; }
        /// <summary>
        /// National ID of applicant
        /// </summary>
        public long? ApplicantNationalId { get; set; }
        /// <summary>
        /// Request creation date
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// Final status ID
        /// </summary>
        public long? FinalStatusId { get; set; }
        /// <summary>
        /// Status change date
        /// </summary>
        public DateTime? StatusChangeDate { get; set; }
        /// <summary>
        /// Rejection reason
        /// </summary>
        public long? RejectionReasonId { get; set; }
        /// <summary>
        /// Decision date
        /// </summary>
        public DateTime? DecisionDate { get; set; }
        /// <summary>
        /// Officer comments
        /// </summary>
        public string OfficerComments { get; set; }
        /// <summary>
        /// Reviewing officer ID
        /// </summary>
        public int? ReviewingOfficerId { get; set; }
        /// <summary>
        /// Bank branch ID
        /// </summary>
        public int? BankBranchId { get; set; }
        /// <summary>
        /// Operational region ID
        /// </summary>
        public int? OperationalRegionId { get; set; }

        public virtual LoanStage FinalStatus { get; set; }
        public virtual LoanContract Contract { get; set; }
        public virtual LoanBranchInfo Branch { get; set; }
        public virtual OperationalRegion Region { get; set; }
        public virtual ICollection<ContractAmendmentStatusLog> StatusChangeLogs { get; set; }
        public virtual ICollection<ContractAmendmentRequestBranch> RequestedBranches { get; set; }
    }
}