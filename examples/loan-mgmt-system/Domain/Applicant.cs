using LoanManagementSystem.Domain;

namespace LoanManagement.Domain
{
    public partial class Applicant
    {
        public Applicant()
        {
            LoanContracts = new HashSet<LoanContract>();
        }

        public int ApplicantId { get; set; }
        public string NationalId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? BirthCityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public byte Gender { get; set; }
        public byte? EducationLevelId { get; set; }
        public byte? EmploymentStatusId { get; set; }
        public byte? MaritalStatusId { get; set; }
        public byte? MilitaryStatusId { get; set; }
        public DateTime CreationDate { get; set; }
        public string IdentityDocumentNumber { get; set; }
        public long? AddressId { get; set; }
        public string EmployerName { get; set; }
        public string ExternalReferenceCode { get; set; }
        public long? ProfileImageId { get; set; }
        public string PlaceOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? LastModificationDate { get; set; }

        public virtual ICollection<LoanContract> LoanContracts { get; set; }
    }
}