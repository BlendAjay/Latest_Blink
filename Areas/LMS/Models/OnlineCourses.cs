using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using AJSolutions.Models;

namespace AJSolutions.Areas.LMS.Models
{
    public class LectureMaster
    {
        [Key]
        [StringLength(16)]
        public string LectureId { get; set; }

        [StringLength(256)]
        public string LectureName { get; set; }

        [StringLength(1024)]
        public string LectureDescription { get; set; }

        [StringLength(256)]
        public string Keywords { get; set; }

        [DefaultValue(true)]
        public bool Permission { get; set; }

        public Int16 LectureStatus { get; set; }

        public Int16 Weightage { get; set; }

        [DefaultValue(true)]
        public bool IsDelete { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual CorporateProfile CorporateProfile { get; set; }
    }

    public class TopicMaster
    {
        [Key]
        [StringLength(16)]
        public string TopicId { get; set; }

        [StringLength(128)]
        public string TopicName { get; set; }

        [StringLength(1024)]
        public string TopicDescription { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class TopicLectures
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 TopicLectureId { get; set; }

        [StringLength(1024)]
        public string LectureId { get; set; }

        [StringLength(16)]
        public string TopicId { get; set; }

        public Int16 SortOrder { get; set; }

        [DefaultValue(true)]
        public bool LectureType { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [ForeignKey("LectureId")]
        public virtual LectureMaster LectureMaster { get; set; }

        [ForeignKey("TopicId")]
        public virtual TopicMaster TopicMaster { get; set; }

        [ForeignKey("SubscriberId")]
        public virtual CorporateProfile CorporateProfile { get; set; }

    }

    public class TopicLecturesView
    {
        public Int64 TopicLectureId { get; set; }

        [StringLength(1024)]
        public string LectureId { get; set; }

        [StringLength(256)]
        public string LectureName { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(16)]
        public string TopicId { get; set; }

        [StringLength(128)]
        public string TopicName { get; set; }

        public Int16 SortOrder { get; set; }

        [DefaultValue(true)]
        public bool LectureType { get; set; }

        [StringLength(128)]
        public string SubscriberId { get; set; }

        [StringLength(128)]
        public string Status { get; set; }

        public Int64 UserCourseSubscriptionId { get; set; }

        public Int64 FileId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [StringLength(1024)]
        public string ContentUrl { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public Int64 SNo { get; set; }
    }

    public class COURSETOPICS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CourseTopicId { get; set; }

        [StringLength(16)]
        public string TopicId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        public Int16 TopicSortOrder { get; set; }

        [DefaultValue(true)]
        public bool TopicType { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [ForeignKey("TopicId")]
        public virtual TopicMaster TopicMaster { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }
    }

    public class COURSETOPICSVIEW
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CourseTopicId { get; set; }

        [StringLength(16)]
        public string TopicId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        public Int16 TopicSortOrder { get; set; }

        [DefaultValue(true)]
        public bool TopicType { get; set; }

        [StringLength(128)]
        public string CourseName { get; set; }

        [StringLength(128)]
        public string TopicName { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(1024)]
        public string TopicDescription { get; set; }

        [ForeignKey("TopicId")]
        public virtual TopicMaster TopicMaster { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }
    }

    public class DiscussionForum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 CommentId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(512)]
        public string Comments { get; set; }

        public DateTime CommentedOn { get; set; }

        [ForeignKey("CourseCode")]
        public virtual CourseMaster CourseMaster { get; set; }
    }

    public class ReviewReply
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ReplyId { get; set; }

        public Int64 CommentId { get; set; }

        [StringLength(512)]
        public string Reply { get; set; }

        public DateTime RepliedOn { get; set; }

        public string UserId { get; set; }

        [ForeignKey("CommentId")]
        public virtual DiscussionForum DiscussionForum { get; set; }

    }

    public class DiscussionForumView
    {
        public Int64 CommentId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(512)]
        public string Comments { get; set; }

        public DateTime CommentedOn { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }
    }

    public class ReviewReplyView
    {
        public Int64 ReplyId { get; set; }

        public Int64 CommentId { get; set; }

        [StringLength(512)]
        public string Reply { get; set; }

        public DateTime RepliedOn { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

    }

    public class GetCountComments
    {
        public int TOTALCOMMENTS { get; set; }
    }

    public partial class LectureContentUpload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 FileId { get; set; }

        [StringLength(16)]
        public string LectureId { get; set; }

        [StringLength(128)]
        public string FileName { get; set; }

        [StringLength(128)]
        public string ContentType { get; set; }

        [ForeignKey("LectureId")]
        public virtual LectureMaster LectureMaster { get; set; }
    }

    public class CandidateResponseView
    {
        public Int64 ResponseId { get; set; }

        public Int64 UserCourseSubscriptionId { get; set; }

        public Int64 TopicLectureId { get; set; }

        [StringLength(128)]
        public string LectureId { get; set; }

        public string UserId { get; set; }

        [StringLength(1024)]
        public string Feedback { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime ResponseOn { get; set; }

        public float Rating { get; set; }

        [StringLength(128)]
        public string RatingFeedback { get; set; }

        [DefaultValue(false)]
        public bool Likes { get; set; }

        public Int64 TotalRatings { get; set; }

        public Int64 TotalLikes { get; set; }
    }
}