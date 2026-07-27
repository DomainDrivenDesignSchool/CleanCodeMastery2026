namespace LoanManagementSystem.Domain
{
    public partial class VehicleAsset
    {
        public VehicleAsset()
        {
            LoanContracts = new HashSet<LoanContract>();
        }

        public long AssetId { get; set; }
        public byte AssetCategoryId { get; set; }
        public bool IsImported { get; set; }
        public byte? PlateRegionCode { get; set; }
        public long? PlateNumberPart1 { get; set; }
        public byte? PlateNumberPart2 { get; set; }
        public long? PlateNumberPart3 { get; set; }
        public byte? ImportCountryCode { get; set; }
        public int? ImportPlateNumber { get; set; }
        public string ChassisNumber { get; set; }
        public int? VehicleTypeId { get; set; }
        public int? ModelId { get; set; }
        public string BodyStyle { get; set; }
        public string DriveType { get; set; }
        public string TransmissionType { get; set; }
        public string Color { get; set; }
        public byte CalendarType { get; set; }
        public int ManufacturingYear { get; set; }
        public byte FuelTypeId { get; set; }
        public string EngineCapacity { get; set; }
        public string EngineNumber { get; set; }
        public string FrameNumber { get; set; }
        public bool HasGpsTracking { get; set; }
        public string RegistrationCardNumber { get; set; }
        public bool HasSmartCard { get; set; }
        public string SmartCardNumber { get; set; }
        public byte VehicleGroupId { get; set; }
        public byte? UsageCategoryId { get; set; }
        public string InsurancePolicyNumber { get; set; }
        public DateTime InsuranceIssueDate { get; set; }
        public long InsuranceCertificateImageId { get; set; }
        public string TechnicalInspectionNumber { get; set; }
        public DateTime? TechnicalInspectionDate { get; set; }
        public long? TechnicalInspectionImageId { get; set; }
        public DateTime? CreationDate { get; set; }
        public long FrontCardImageId { get; set; }
        public long BackCardImageId { get; set; }
        public string ExternalReferenceCode { get; set; }
        public long? NationalCodeId { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public DateTime? TechnicalInspectionExpiryDate { get; set; }
        public int NationalRegistrationNumber { get; set; }
        /// <summary>
        /// Vehicle condition (1=new, 2=used)
        /// </summary>
        public byte VehicleCondition { get; set; }
        public byte? InsuranceCategoryCode { get; set; }
        public byte? InsurancePolicyType { get; set; }
        public byte? ImportCountryTwoDigitCode { get; set; }
        public double? EngineDisplacement { get; set; }
        public long? CapacityUnitId { get; set; }

        public virtual ApplicantNationalId NationalCode { get; set; }
        public virtual ICollection<LoanContract> LoanContracts { get; set; }
    }
}