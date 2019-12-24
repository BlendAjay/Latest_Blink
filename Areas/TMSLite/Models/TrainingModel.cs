using AJSolutions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Areas.TMSLite.Models
{
    public class Course
    {

        [Key]
        [StringLength(16)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(128)]
        public string CourseName { get; set; }

        public Int64 CategoryId { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public Int16 CourseDuration { get; set; }

        public double CourseFee { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [DefaultValue(false)]
        public bool DiscussionForum { get; set; }

        [DefaultValue(0)]
        public int CountLikes { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }

    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 SubjectId { get; set; }

        [StringLength(32)]
        public string SubjectName { get; set; }

        [StringLength(8)]
        public string ShortName { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class CourseSubject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CSId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        public Int64 SubjectId { get; set; }
    }

    public class Batches
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 BatchId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(128)]
        public string BatchName { get; set; }

        public DateTime BatchStartDate { get; set; }

        public DateTime BatchEndDate { get; set; }

        public DateTime BatchStartTime { get; set; }

        public DateTime BatchEndTime { get; set; }

        public Boolean IsAttendanceRequired { get; set; }

    }


}