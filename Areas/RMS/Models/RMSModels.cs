using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Areas.RMS.Models
{
    public class QuestionMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 QuestionId { get; set; }

        [StringLength(512)]
        public string Question { get; set; }

        //[StringLength(16)]
        //public string Frequency { get; set; }

        [StringLength(16)]
        public string Category { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }
    }

    public class BranchDetails
    {
        [Key]
        [StringLength(32)]
        public string BranchCode { get; set; }

        [StringLength(64)]
        public string BranchName { get; set; }

        [StringLength(128)]
        public string BranchZone { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }
        
        [StringLength(128)]
        public string SubcriberId { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class TrainerAssign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TrainerAssignId { get; set; }

        [StringLength(32)]
        public string BranchCode { get; set; }

        [StringLength(128)]
        public string TrainerId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfJoining { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LeavingDate { get; set; }

        [ForeignKey("BranchCode")]
        public virtual BranchDetails BranchDetails { get; set; }

    }

    public class FeedBack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FeedbackId { get; set; }

        public Int64 QuestionId { get; set; }

        [StringLength(512)]
        public string Answer { get; set; }

        [StringLength(128)]
        public string TrainerId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FeedBackdate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }

        [ForeignKey("QuestionId")]
        public virtual QuestionMaster QuestionMaster { get; set; }

    }   

    public class Feedbackview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 QuestionId { get; set; }

        [StringLength(512)]
        public string Question { get; set; }

        [StringLength(16)]
        public string Frequency { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(64)]
        public string GapObserved { get; set; }

        [StringLength(128)]
        public string SuggestiveMeasures { get; set; }

        public Int64 FeedbackId { get; set; }

        public Int64 TrainerAssignId { get; set; }

        public string BranchCode { get; set; }

        public string TrainerId { get; set; }

        public string EmployeeName { get; set; }

        public string BranchZone { get; set; }

        public string BranchName { get; set; }

        public string CorporateId { get; set; }

        public string AlternateContact { get; set; }

        public string Name { get; set; }        

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FeedBackdate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedOn { get; set; }
    }

    public class BranchDetailsView
    {
        [Key]
        [StringLength(32)]
        public string BranchCode { get; set; }

        [StringLength(64)]
        public string BranchName { get; set; }

        [StringLength(128)]
        public string BranchZone { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public string EmployeeName { get; set; }

        public string AlternateContact { get; set; }
        
        [StringLength(128)]
        public string SubcriberId { get; set; }

        [ForeignKey("UserId")]
        public virtual EmployeeBasicDetails EmployeeBasicDetails { get; set; }

        [ForeignKey("CorporateId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class TrainerAssignView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TrainerAssignId { get; set; }

        [StringLength(32)]
        public string BranchCode { get; set; }

        [StringLength(128)]
        public string TrainerId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfJoining { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LeavingDate { get; set; }

        public string Name { get; set; }
        
        [StringLength(64)]
        public string BranchName { get; set; }

        [StringLength(128)]
        public string BranchZone { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }
    }

}