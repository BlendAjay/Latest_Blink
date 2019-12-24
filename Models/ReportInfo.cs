using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{
    public class ReportInfo
    {
        public List<ClientJobOrderAdmin> clientJobOrderAdmin { get; set; }
        public List<ClientProfileAdmin> clientProfileAdmin { get; set; }
        public ClientProfileAdminRequiredField clientProfileAdminRequiredField { get; set; }
        public ClientJobOrderAdminRequiredField clientJobOrderAdminRequiredField { get; set; }
        public List<CandidateCheckInCheckOut> candidateCheckInCheckOut { get; set; }
        public List<CandidateAttendenceDetailsDateWise> candidateAttendenceDetailsDateWise { get; set; }
        public CandidateDetails candidateDetails { get; set; }
    }
    public class ClientJobOrderAdmin
    {
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(1024)]
        public string ClientId { get; set; }
        [StringLength(512)]
        public string JobOrderStatus { get; set; }

        public int JobOrderTypeId { get; set; }

        public int Duration { get; set; }
        public string JobOrderNumber { get; set; }
        [StringLength(128)]
        public string JOAction { get; set; }
        [StringLength(128)]
        public string SubscriberID { get; set; }
        [StringLength(128)]
        public string Subject { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        public DateTime? JOPostedOn { get; set; }
        public int TotalCost { get; set; }
        [StringLength(512)]
        public string ClientName { get; set; }
        [StringLength(128)]
        public string SubscriberName { get; set; }
        [StringLength(128)]
        public string TotalInv { get; set; }
        [StringLength(128)]
        public string Status { get; set; }
        [StringLength(128)]
        public string InvNo { get; set; }
        [StringLength(128)]
        public string JobOrderType { get; set; }

    }
    public class ClientProfileAdmin
    {
        [StringLength(128)]
        public string UserId { get; set; }
        [StringLength(512)]
        public string SubscriberID { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(64)]
        public string BankName { get; set; }
        [StringLength(32)]
        public string AccountNumber { get; set; }
        [StringLength(128)]
        public string AccountOwner { get; set; }
        [StringLength(32)]
        public string IFSCCode { get; set; }
        [StringLength(16)]
        public string BranchCode { get; set; }
        [StringLength(128)]
        public string BranchAddress { get; set; }
        [StringLength(64)]
        public string AddressLine1 { get; set; }
        [StringLength(64)]
        public string AddressLine2 { get; set; }
        [StringLength(64)]
        public string City { get; set; }
        [StringLength(32)]
        public string State { get; set; }
        [StringLength(40)]
        public string Country { get; set; }
        [StringLength(16)]
        public string PostalCode { get; set; }
        [StringLength(2)]
        public string AddressType { get; set; }
        [StringLength(256)]
        public string OderType { get; set; }
        [StringLength(512)]
        public string CompnayAddress { get; set; }
        [StringLength(128)]
        public string CorrespondenceAddress { get; set; }
        [StringLength(512)]
        public string PresentAddress { get; set; }
        [StringLength(512)]
        public string PermanenetAddress { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime RegisteredOn { get; set; }
        [StringLength(64)]
        public string CompanyName { get; set; }
        [StringLength(32)]
        public string CompanySize { get; set; }
        [StringLength(128)]
        public string CompanyType { get; set; }
        [StringLength(64)]
        public string Website { get; set; }

    }
    public class ClientProfileAdminRequiredField
    {
        public bool Name { get; set; }
        public bool Bank { get; set; }
        public bool CompnayAddress { get; set; }
        public bool CorrespondenceAddress { get; set; }
        public bool PresentAddress { get; set; }
        public bool PermanenetAddress { get; set; }
        public bool RegisteredOn { get; set; }
        public bool LastLogin { get; set; }
        public bool CompanyName { get; set; }
        public bool CompanySize { get; set; }
        public bool CompanyType { get; set; }
        public bool Website { get; set; }
    }
    public class ClientJobOrderAdminRequiredField
    {
        public bool JobOrderID { get; set; }
        public bool JobOrderTitle { get; set; }
        public bool InvoiceAmount { get; set; }
        public bool InvoiceNumber { get; set; }
        public bool JobDescription { get; set; }
        public bool InvoiceStatus { get; set; }
    }
    public class CandidateCheckInCheckOut
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        [StringLength(1024)]
        public string UserName { get; set; }
        [StringLength(512)]
        public string Name { get; set; }
        public DateTime? CIn { get; set; }
        public DateTime? COut { get; set; }
        public int TotalDays { get; set; }
        public int Attendance { get; set; }
        public int TotalAttendence { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        [StringLength(512)]
        public string PhoneNumber { get; set; }
        [StringLength(64)]
        public string AlternateEmail { get; set; }
        [StringLength(16)]
        public string AlternateContact { get; set; }
        [StringLength(2)]
        public string Gender { get; set; }
        [StringLength(16)]
        public string RegistrationId { get; set; }
        [StringLength(128)]
        public string Designation { get; set; }
        [StringLength(128)]
        public string Region { get; set; }
        [StringLength(128)]
        public string Branch { get; set; }
        [StringLength(128)]
        public string BranchCategory { get; set; }
        [StringLength(128)]
        public string BranchState { get; set; }
        [StringLength(128)]
        public string BranchCode { get; set; }
    }

    public class CandidateAttendenceDetailsDateWise
    {
        public DateTime? AttendenceDate { get; set; }

        [StringLength(2)]
        public string Ispresent { get; set; }

        [StringLength(1024)]
        public string Comment { get; set; }
    }
    public class CandidateDetails
    {
        public bool CandidateEmail { get; set; }
        public bool Candidatemobile { get; set; }
        public bool CandidaternateEmail { get; set; }
        public bool Candidaternatemobile { get; set; }
        public bool CandidateGender { get; set; }
        public bool CandidateRegistrationID { get; set; }
        public bool CandidateDesignation { get; set; }
        public bool CandidateRegion { get; set; }
        public bool CandidateBranchName { get; set; }
        public bool CandidateBranchCategory { get; set; }
        public bool CandidateBranchState { get; set; }
        public bool CandidateBranchCode { get; set; }
    }
    //public class EmployeeTraining
    //{
    //    [StringLength(128)]
    //    public string CourseCode { get; set; }
    //    [StringLength(512)]
    //    public string CourseName { get; set; }
    //    [StringLength(128)]
    //    public string Name { get; set; }
    //    [StringLength(64)]
    //    public string BankName { get; set; }
    //    [StringLength(32)]
    //}
}