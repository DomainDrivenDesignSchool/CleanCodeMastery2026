using LoanManagement.Domain;

namespace LoanManagementSystem.Domain
{
    public partial class LoanContract
    {
        public LoanContract()
        {
            ContractAmendmentRequests = new HashSet<ContractAmendmentRequest>();
        }

        public int ContractId { get; set; }
        public int? ApplicantId { get; set; }
        public int? CorporateClientId { get; set; }
        public byte OwnershipTypeId { get; set; }
        public DateTime? CreationDate { get; set; }
        public decimal ContractTrackingCode { get; set; }
        public byte ContractStatus { get; set; }
        public long AssetInfoId { get; set; }
        public byte ActivationStatus { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveExpiryDate { get; set; }
        public int BankBranchId { get; set; }
        public long? ContractDocumentImageId { get; set; }
        public string ContractNumber { get; set; }
        public string ContractSerialNumber { get; set; }
        public long? VehicleEvaluationImageId { get; set; }
        public long? IncomeProofImageId { get; set; }
        public bool IsOwnerOccupant { get; set; }
        public long? TreasuryReferenceNumber { get; set; }
        public long? GuaranteeDocumentImageId { get; set; }
        public string PreviousStatus { get; set; }
        public string ExternalReferenceCode { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? AmendedContractId { get; set; }
        public int? LastApprovingOfficerId { get; set; }
        public int? LastVerificationOfficerId { get; set; }
        public long MobilePhoneNumber { get; set; }
        public long LandlinePhoneNumber { get; set; }
        public byte LoanTypeId { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public string BankAccountIBAN { get; set; }
        public DateTime? ContractIssuanceDate { get; set; }
        public long? InsuranceDocumentId { get; set; }
        /// <summary>
        /// Receipt image for loan processing fee
        /// </summary>
        public long? FeeReceiptImageId { get; set; }
        /// <summary>
        /// Payment date
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// Payment amount
        /// </summary>
        public long? PaymentAmount { get; set; }
        /// <summary>
        /// Payment tracking code
        /// </summary>
        public string PaymentTrackingCode { get; set; }
        public DateTime? LastStatusChangeDate { get; set; }

        public virtual BankBranch Branch { get; set; }
        public virtual Applicant Applicant { get; set; }
        public virtual VehicleAsset Asset { get; set; }
        public virtual ICollection<ContractAmendmentRequest> ContractAmendmentRequests { get; set; }
    }
}