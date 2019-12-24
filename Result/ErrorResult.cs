using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using AJSolutions.Models;
namespace AJSolutions.Result
{
    public class ErrorResult : IHttpActionResult
    {
        Error _error;
        HttpRequestMessage _request;

        public ErrorResult(Error error, HttpRequestMessage request)
        {
            _error = error;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new ObjectContent<Error>(_error, new JsonMediaTypeFormatter()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }

    }

    public class Error
    {
        public bool Status { get; set; }
        public string Message { get; set; }

    }

    public class LogInResult : IHttpActionResult
    {

        LogInData _login;
        HttpRequestMessage _request;

        public LogInResult(LogInData login, HttpRequestMessage request)
        {
            _login = login;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new ObjectContent<LogInData>(_login, new JsonMediaTypeFormatter()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }

    }

    public class LogInData
    {
        //for error messege
        public int Status { get; set; }
        public int CheckedStatus { get; set; }
        public string Message { get; set; }
        //for user details and checincheckout
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Phonenumber { get; set; }
        public Dictionary<string, string> BearerCode { get; set; }
        public string DeviceName { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime CurrentTime { get; set; }
        public int LocationStatus { get; set; }
        public TimeSpan ServerTime { get; set; }
    }

    //result for employee leave
    public class LeaveResult : IHttpActionResult
    {

        LeaveDetails _leave;
        HttpRequestMessage _request;

        public LeaveResult(LeaveDetails leave, HttpRequestMessage request)
        {
            _leave = leave;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new ObjectContent<LeaveDetails>(_leave, new JsonMediaTypeFormatter()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }

    }
    public class LeaveDetails
    {
        public List<EngagementTypeMaster> Leavetype { get; set; }
        public List<TrainerPlannerView> EmpLeaveDetails { get; set; }

    }
    //result for employee attendance
    public class AttendanceResult : IHttpActionResult
    {

        Attendance _attendance;
        HttpRequestMessage _request;

        public AttendanceResult(Attendance attendance, HttpRequestMessage request)
        {
            _attendance = attendance;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new ObjectContent<Attendance>(_attendance, new JsonMediaTypeFormatter()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }

    }
    public class Attendance
    {
        public List<EmpAttendanceAppView> AttendanceData { get; set; }

    }
}
