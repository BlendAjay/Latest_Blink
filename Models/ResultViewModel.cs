using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace AJSolutions.Models
{
    public class CandidatePublicationView
    {
        [Key]
        [StringLength(128)]
        public string EnrollId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EnrollDate { get; set; }

        [StringLength(128)]
        public string CandidateId { get; set; }

        public Int64 FolderId { get; set; }

        [StringLength(128)]
        public string PublicationId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? EndTime { get; set; }

        [StringLength(128)]
        public string TimeZone { get; set; }

        [StringLength(256)]
        public string TestLink { get; set; }

        [Required]
        [StringLength(16)]
        public string Status { get; set; }

        public bool SendEmailNotification { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? ExamTakenDate { get; set; }

        [StringLength(16)]
        public string AccessCode { get; set; }

        [StringLength(32)]
        public string Password { get; set; }

        public float CandidateScore { get; set; }

        public Int32 TotalAttemptedQue { get; set; }

        public bool BrowsingTolerance { get; set; }

        public Int16 ToleranceValue { get; set; }

        public bool ShowRemainingCounts { get; set; }

        public bool IPAccessRestriction { get; set; }

        public Int16 IPRestrictionType { get; set; }

        [StringLength(16)]
        public string IPStartValue { get; set; }

        [StringLength(16)]
        public string IPEndValue { get; set; }

        public bool ShareGrade { get; set; }

        public Int64? PlanId { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

    }

    public class CandidateResultView
    {
        public Int64 QuestionId { get; set; }
        public string Question { get; set; }
        public Int64 AnswerId { get; set; }
        public string Answer { get; set; }
        public bool IsRight { get; set; }
        public double Score { get; set; }
        public Int32 CountEssay { get; set; }
        public string QuestionTypeName { get; set; }
        public Int64? CandidateResponseId { get; set; }

    }

    public class CandidateAttemptCountsView
    {
        public string QuestionFlg { get; set; }
        public int Value { get; set; }
        public int TotalQuestions { get; set; }
    }


    public class ExamDetailView
    {
        public string EnrollId { get; set; }
        public DateTime? EnrollDate { get; set; }
        public DateTime? ExamTakenDate { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public string IPAddress { get; set; }
    }

    public class CandidateReportView
    {
        public double Score { get; set; }
        public Int64 FolderId { get; set; }
        public string FolderName { get; set; }
    }

    /* Reference to Prelore view models*/
    public class ResultsView
    {
        [StringLength(128)]
        public string EnrollId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public DateTime ExamDate { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        public int TotalAttemptedQues { get; set; }

        public int TotalQuestions { get; set; }

        [StringLength(128)]
        public string AssessmentId { get; set; }

        [StringLength(128)]
        public string PublicationId { get; set; }

        [StringLength(128)]
        public string CandidateId { get; set; }

        public double? Percentage { get; set; }

        public int? Totalmark { get; set; }

        public double? Obtained { get; set; }

        public string TAG { get; set; }

        public string UserId { get; set; }

        public string NoOfCorrectAns { get; set; }

        public string Title { get; set; }
    }

    public class CandidateAssessmentReportViewModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string RegistrationNo { get; set; }

        [StringLength(128)]
        public string AssessmentId { get; set; }

        [StringLength(128)]
        public string PublicationId { get; set; }

        [StringLength(128)]
        public string Title { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalAttemptedQues { get; set; }

        public string NoOfCorrectAns { get; set; }

        public int? Totalmark { get; set; }

        public double? Percentage { get; set; }

        public double? Obtained { get; set; }

        public DateTime ExamDate { get; set; }

    }

    public class CandidateResultSummaryView
    {

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string AssessmentId { get; set; }

        [StringLength(128)]
        public string PublicationId { get; set; }

        public string Title { get; set; }

        [StringLength(16)]
        public string Status { get; set; }

        public int TotalQuestions { get; set; }

        public int TotalAttemptedQues { get; set; }

        public string NoOfCorrectAns { get; set; }

        public int? Totalmark { get; set; }

        public double? Percentage { get; set; }

        public double? Obtained { get; set; }

        public DateTime ExamDate { get; set; }

    }

    public class ResponceCountByStatus
    {
        public string Status { get; set; }

        public int TCount { get; set; }
    }

}