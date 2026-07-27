using LoanManagementSystem.Domain;
using System;
using System.Collections.Generic;

namespace LoanManagement.Domain
{
    public partial class BankBranch
    {
        public BankBranch()
        {
            LoanContracts = new HashSet<LoanContract>();
            AmendmentRequestBranches = new HashSet<ContractAmendmentRequestBranch>();
            ContractAmendmentRequests = new HashSet<ContractAmendmentRequest>();
        }

        public int BankBranchId { get; set; }
        /// <summary>
        /// Branch name / title
        /// </summary>
        public string BranchName { get; set; }
        /// <summary>
        /// Branch code
        /// </summary>
        public string BranchCode { get; set; }
        /// <summary>
        /// Operational region ID
        /// </summary>
        public int OperationalRegionId { get; set; }
        /// <summary>
        /// Establishment date
        /// </summary>
        public DateTime EstablishmentDate { get; set; }
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// External reference code
        /// </summary>
        public string ExternalReferenceCode { get; set; }
        /// <summary>
        /// Active / Inactive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Branch address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Branch phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Branch manager name
        /// </summary>
        public string ManagerName { get; set; }

        public virtual OperationalRegion OperationalRegion { get; set; }
        public virtual ICollection<LoanContract> LoanContracts { get; set; }
        public virtual ICollection<ContractAmendmentRequestBranch> AmendmentRequestBranches { get; set; }
        public virtual ICollection<ContractAmendmentRequest> ContractAmendmentRequests { get; set; }
    }
}