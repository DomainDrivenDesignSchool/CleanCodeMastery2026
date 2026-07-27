using LoanManagementSystem.Domain;
using System;
using System.Collections.Generic;

namespace LoanManagement.Domain
{
    public partial class OperationalRegion
    {
        public OperationalRegion()
        {
            BankBranches = new HashSet<BankBranch>();
            AmendmentRequestBranches = new HashSet<ContractAmendmentRequestBranch>();
            ContractAmendmentRequests = new HashSet<ContractAmendmentRequest>();
        }

        public int OperationalRegionId { get; set; }
        /// <summary>
        /// Region name
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// Region code
        /// </summary>
        public string RegionCode { get; set; }
        /// <summary>
        /// Region description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// External reference code
        /// </summary>
        public string ExternalReferenceCode { get; set; }
        /// <summary>
        /// Is region active
        /// </summary>
        public bool IsActive { get; set; }

        public virtual ICollection<BankBranch> BankBranches { get; set; }
        public virtual ICollection<ContractAmendmentRequestBranch> AmendmentRequestBranches { get; set; }
        public virtual ICollection<ContractAmendmentRequest> ContractAmendmentRequests { get; set; }
    }
}