using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AJSolutions.Areas.Admin.Models
{
    public class UserAttendanceViewModel
    {

        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [StringLength(16)]
        public string Phone { get; set; }

        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutDate { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [StringLength(64)]
        public string AndroidId { get; set; }

        [StringLength(128)]
        public string AndroidDeviceName { get; set; }


        public float Latitude { get; set; }


        public float Longitude { get; set; }

        public string LoggedInIp { get; set; }

    }
}