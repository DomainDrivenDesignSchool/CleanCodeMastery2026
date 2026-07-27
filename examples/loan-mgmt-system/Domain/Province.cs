using LoanManagement.Domain;

namespace LoanManagementSystem.Domain
{
    public partial class Province
    {
        public Province()
        {
            AmendmentRequestBranches = new HashSet<ContractAmendmentRequestBranch>();
            ContractAmendmentRequests = new HashSet<ContractAmendmentRequest>();
        }

        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string RegionCode { get; set; }
        public DateTime CreationDate { get; set; }
        public string ExternalReferenceCode { get; set; }

        public virtual ICollection<BankBranch> BankBranches { get; set; }
        public virtual ICollection<ContractAmendmentRequestBranch>  AmendmentRequestBranches { get; set; }
        public virtual ICollection<ContractAmendmentRequest> ContractAmendmentRequests { get; set; }
    }
}