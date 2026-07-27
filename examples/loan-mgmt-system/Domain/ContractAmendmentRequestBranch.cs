using LoanManagementSystem.Domain;
using System;
using System.Collections.Generic;

namespace LoanManagement.Domain
{
    public partial class ContractAmendmentRequestBranch
    {
        public ContractAmendmentRequestBranch()
        {
        }

        /// <summary>
        /// Primary key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Contract amendment request ID (foreign key to ContractAmendmentRequest)
        /// </summary>
        public long? AmendmentRequestId { get; set; }

        /// <summary>
        /// Operational region ID (foreign key to OperationalRegion)
        /// </summary>
        public int? OperationalRegionId { get; set; }

        /// <summary>
        /// Bank branch ID (foreign key to BankBranch)
        /// The selected branch for processing the loan
        /// </summary>
        public int? BankBranchId { get; set; }

        /// <summary>
        /// Creation date of this record
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Indicates if this is the primary branch for this request
        /// </summary>
        public bool IsPrimaryBranch { get; set; }

        // Navigation properties
        public virtual BankBranch BankBranch { get; set; }
        public virtual OperationalRegion OperationalRegion { get; set; }
        public virtual ContractAmendmentRequest AmendmentRequest { get; set; }
    }
}