using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{
    /// <summary>
    /// This Model is used to register user in LMS (WIKIPIAN)
    /// Created By: Kulesh
    /// Created On: 28-Jul-2017
    /// Updated By: Kulesh
    /// Updated On: 28-Jul-2017
    /// Reviewed By:
    /// Reviewed On:
    /// </summary>
    public class UserRegView
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [StringLength(128)]
        public string UserRole { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [StringLength(16)]
        public string MobileNumber { get; set; }

        [StringLength(16)]
        public string Password { get; set; }

        [StringLength(512)]
        public string Redirectionurl { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string CourseCode { get; set; }
    }

    /// <summary>
    /// To get data from Courese Masters Data From LMS (WIKIPIAN)
    /// Created By: Kulesh
    /// Created On: 28-Jul-2017
    /// Updated By: Kulesh
    /// Updated On: 28-Jul-2017
    /// Reviewed By:
    /// Reviewed On:    
    /// </summary>
    public class CourseMasterViewModel
    {
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(128)]
        public string CourseName { get; set; }

        [StringLength(32)]
        public string CourseDuration { get; set; }

        public Int64 CategoryId { get; set; }

        [StringLength(128)]
        public string CategoryName { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public double CourseFee { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [DefaultValue(false)]
        public bool DiscussionForum { get; set; }

        public Int16 ContentVisiblity { get; set; }

        [DefaultValue(0)]
        public int CountLikes { get; set; }

        [StringLength(1024)]
        public string CourseDescription { get; set; }

        [DefaultValue(false)]
        public bool RandomNavigation { get; set; }

        [DefaultValue(false)]
        public bool ShowNavigation { get; set; }

        public int TotalTopics { get; set; }

        public int TotalLecture { get; set; }

        public int CompletedLectures { get; set; }

        public string Name { get; set; }

        public string CompanyName { get; set; }

        public DateTime SubscribedOn { get; set; }

        public bool ActiveOrExpired { get; set; }

        public Int64 ContentFileId { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        public string AcutalFileName { get; set; }

        public Int64 UserCourseSubscriptionId { get; set; }

        public string CourseSubscriber { get; set; }

        public string TopicId { get; set; }

        public string LectureId { get; set; }

        public string Status { get; set; }

        public string CourseLiked { get; set; }

        public int Subscriptions { get; set; }

        public string Rated { get; set; }

        public double Rating { get; set; }

        public string RefUserId { get; set; }

        public string RefCourseCode { get; set; }

        public string LMSCourseCode { get; set; }

        public bool IsCourseIntegrated { get; set; }
    }


    /// <summary>
    /// To get assessment detail from prelore.com
    /// Created By: Rahul
    /// Created On: 31-Jul-2017
    /// Updated By: Rahul
    /// Updated On: 31-Jul-2017
    /// Reviewed By:
    /// Reviewed On:    
    /// </summary>
    public class PublicationView
    {
        [Key]
        [StringLength(128)]
        public string PublicationId { get; set; }

        [StringLength(256)]
        public string Title { get; set; }

        public string PublishType { get; set; }

        public Int16 OptionType { get; set; }

        [StringLength(128)]
        public string AssessmentId { get; set; }

        public Int16 AccessLevel { get; set; }

        public bool RequestPassword { get; set; }

        public bool PasswordType { get; set; }

        [StringLength(32)]
        public string PasswordValue { get; set; }

        public bool CustomMessage { get; set; }

        public string CustomMsgValueAssmtPage { get; set; }

        public string CustomMsgValueResultPage { get; set; }

        public Int32 TotalScore { get; set; }

        public Int32 PassingScore { get; set; }

        public bool ScoreType { get; set; }

        public Int32 MaxTime { get; set; }

        public bool ShowTimer { get; set; }

        public int NumberOfQue { get; set; }

        [StringLength(2048)]
        public string Description { get; set; }

        [StringLength(256)]
        public string Keywords { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastEditedOn { get; set; }

        public string CreatedBy { get; set; }

        [StringLength(128)]
        public string TestTaken { get; set; }

        [StringLength(128)]
        public string Responses { get; set; }

        [StringLength(16)]
        public string TotalEnroll { get; set; }

        [StringLength(128)]
        public string TotalQuestions { get; set; }

        [StringLength(128)]
        public string PublicationKey { get; set; }
    }

    /// <summary>
    /// To Register Candidate & Enroll on prelore.com
    /// Created By: Rahul
    /// Created On: 2-Jul-2017
    /// Updated By: Rahul
    /// Updated On: 2-Jul-2017
    /// Reviewed By:
    /// Reviewed On:    
    /// </summary>

    public class CandidateRegisterView
    {
        [Key]
        [StringLength(128)]
        public string CandidateId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(512)]
        public string Redirectionurl { get; set; }

        [StringLength(128)]
        public string PublicationId { get; set; }

        [StringLength(23)]
        public string Password { get; set; }

        [StringLength(128)]
        public string Category { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }
    }

    public class AssessmentTrainingView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 AssessmentId { get; set; }

        [StringLength(128)]
        public string Assessment { get; set; }

        public Int64 Weightage { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        [StringLength(128)]
        public string PublicationId { get; set; }

        [StringLength(256)]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

    }
}