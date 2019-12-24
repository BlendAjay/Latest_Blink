using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AJSolutions.Models
{
    public class CourseSummaryViewModel
    {
        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(128)]
        public string CourseName { get; set; }

        public int TotalBatches { get; set; }

        public int TotalCnadidates { get; set; }

        public int TotalTraining { get; set; }

        public int TotalAssignedTraining { get; set; }

        public int TotalInProgressTraining { get; set; }

        public int TotalCompletedTraining { get; set; }
    }

    public class TrainingDetailViewModel
    {
        [StringLength(16)]
        public string CourseCode { get; set; }

        [StringLength(128)]
        public string CourseName { get; set; }

        [StringLength(128)]
        public string CorporateId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string CompanyName { get; set; }

        public Int64 BatchId { get; set; }

        [StringLength(128)]
        public string BatchName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public int CountryId { get; set; }

        [StringLength(32)]
        public string Country { get; set; }

        public int StateId { get; set; }

        [StringLength(32)]
        public string State { get; set; }

        public int CityId { get; set; }


        [StringLength(32)]
        public string City { get; set; }

        [StringLength(16)]
        public string TrainingId { get; set; }

        [StringLength(128)]
        public string TrainerId { get; set; }

        [StringLength(128)]
        public string TrainerName { get; set; }

        [StringLength(128)]
        public string Venue { get; set; }

        [StringLength(16)]
        public string Status { get; set; }
    }
}