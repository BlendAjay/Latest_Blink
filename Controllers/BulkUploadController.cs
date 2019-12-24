using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AJSolutions.DAL;
using System.Reflection;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.UI;
using Microsoft.SqlServer;
using System.Data.OleDb;
using AJSolutions.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using System.Diagnostics;
//using Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
//using Microsoft.AspNet.SignalR;
//using RealTimeProgressBar;  

namespace AJSolutions.Controllers
{
    public class BulkUploadController : Controller
    {
        // GET: BulkUpload
        //Install-Package ClosedXML
        AdminManager admin = new AdminManager();
        Generic generic = new Generic();
        CMSManager cmsMgr = new CMSManager();
        EMSManager emsMgr = new EMSManager();
        UserDBContext db = new UserDBContext();
        ReportInfo rpt = new Models.ReportInfo();
        ReportManager rm = new ReportManager();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetBatch(string CourseCode)
        {
            List<SelectListItem> BatchId = new List<SelectListItem>();
            string SubscriberId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(CourseCode))
            {
                List<CourseBatch> Batches = (from b in db.CourseBatch
                                             join c in db.CourseMaster
                                             on b.CourseCode equals c.CourseCode
                                             where b.CourseCode == CourseCode && c.SubscriberId == SubscriberId
                                             select b).ToList();
                Batches.ForEach(x =>
                {
                    BatchId.Add(new SelectListItem { Text = x.BatchName, Value = x.BatchId.ToString() });
                });
            }
            return Json(BatchId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CandidateAttendenceBulkUpload(string TrainingId, string BatchId, string CourseCode)
        {
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            PopulateCourse(UserId, CourseCode);
            PopulateBatchByCourse(userDetails.SubscriberId, CourseCode, BatchId);
            return View();
        }

        private void PopulateBatchByCourse(string SubscriberId = null, string CourseCode = null, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            var query = tms.GetBatches(SubscriberId, CourseCode);
            SelectList BatchId = new SelectList(query, "BatchId", "BatchName", selectedValue);
            ViewBag.BatchId = BatchId;
        }

        private void PopulateCourse(string SubscriberId, object selectedValue = null)
        {
            TMSManager tms = new TMSManager();
            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            var query = tms.GetCourseDetails(userDetails.SubscriberId);
            SelectList CourseCode = new SelectList(query, "CourseCode", "CourseName", selectedValue);
            ViewBag.CourseCode = CourseCode;
        }

        public ActionResult DownloadFormat(string batchid, DateTime currentDate)
        {
            System.Data.DataTable dt = new System.Data.DataTable("CanidateAttendence");
            GridView gv = new GridView();
            gv.DataSource = rm.GetStudentAttendence(batchid, currentDate);
            gv.DataBind();
            if (gv.Rows.Count > 0)
            {
                foreach (TableCell cell in gv.HeaderRow.Cells)
                {
                    dt.Columns.Add(cell.Text);
                }
                foreach (GridViewRow row in gv.Rows)
                {
                    dt.Rows.Add();
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        dt.Rows[dt.Rows.Count - 1][i] = row.Cells[i].Text;
                    }
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=GridView.xlsx");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return View();
            //return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report123.csv");
        }

        public ActionResult UploadFile(string filpath)
        {
            Upload();
            return View();
        }

        public JsonResult Upload()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                //file.SaveAs(Server.MapPath("~/Content") + fileName); //File will be saved in application root\               
                fileName = Server.MapPath(Path.Combine("~/Content/", fileName));
                //file.s
                file.SaveAs(Path.ChangeExtension(fileName, ".xlsx"));
                importdatafromexcel(Path.Combine(fileName));
                string fullPath = fileName;
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
                TempData["Success"] = true;
            }

            return Json("Uploaded " + Request.Files.Count + " files");
        }

        public void importdatafromexcel(string excelfilepath)
        {
            GridView GridView1 = new GridView();
            int r = Request.Files.Count;
            HttpPostedFileBase file = Request.Files[0]; //Uploaded file
            //Use the following properties to get file's name, size and MIMEType
            int fileSize = file.ContentLength;
            string fileName = file.FileName;
            string[] values = fileName.Split('.');
            string SheetName = values[0];
            string filetype = file.GetType().ToString();


            string ExistedNumber = string.Empty;

            //fileName = Server.MapPath(Path.Combine("~/Content/",Path.ChangeExtension(fileName, ".xlsx")));
            //fileName = Server.MapPath(Path.Combine("~/Content/MyExcelFile_files/", Path.ChangeExtension(fileName, ".xlsx")));
            //file.SaveAs(fileName);
            fileName = Server.MapPath(Path.Combine("~/Content/", fileName));


            file.SaveAs(Path.ChangeExtension(fileName, ".xlsx"));
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                //Read the first Sheet from Excel file.
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

                //Get the Worksheet instance.
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

                //Fetch all the rows present in the Worksheet.
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                //Create a new DataTable.
                DataTable dt = new DataTable();

                //Loop through the Worksheet rows.
                foreach (Row row in rows)
                {
                    //Use the first row to add columns to DataTable.
                    if (row.RowIndex.Value == 1)
                    {
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dt.Columns.Add(GetValue(doc, cell));
                        }
                    }
                    else
                    {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = GetValue(doc, cell);
                            i++;
                        }
                    }
                }
                GridView1.DataSource = dt;
                GridView1.DataBind();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //DateTime today = System.DateTime.Now;
                    DateTime today = Convert.ToDateTime(dt.Rows[i][4]);
                    bool IsPresent = false;
                    if (dt.Rows[i][5].ToString() == "P" || dt.Rows[i][5].ToString() == "true")
                    {
                        IsPresent = true;
                    }
                    else
                    {
                        IsPresent = false;
                    }
                    if (!rm.IsStudentAttendenceExists(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), today))
                        rm.InsertStudentAttendence(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), today, IsPresent, dt.Rows[i][6].ToString());
                }
                doc.Close();
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                if (System.IO.File.Exists(fileName))
                {
                    if (System.IO.File.Exists(fileName))
                        System.IO.File.Delete(fileName);
                }
            }
            //string path = Path.ChangeExtension(excelfilepath, ".xlsx");
            ////string excelConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;\"";
            //string excelConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;\"";
            ////string path = @"E:\Blink\Blink\Content\MyExcelFile.xls";
            ////string path = Server.MapPath(excelfilepath); 
            //string ssqltable = "CandidateAttendances";
            //HttpPostedFileBase file = Request.Files[0]; //Uploaded file
            ////Use the following properties to get file's name, size and MIMEType
            //int fileSize = file.ContentLength;
            //string fileName = file.FileName;
            //string[] values = fileName.Split('.');
            //string SheetName = values[0];
            //string filetype = file.GetType().ToString();
            //OleDbConnection objConn = null;
            //System.Data.DataTable dt1 = null;
            //objConn = new OleDbConnection(excelConnection);
            //// Open connection with the database.
            //objConn.Open();
            //// Get the data table containg the schema guid.
            //dt1 = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);


            //String[] excelSheets = new String[dt1.Rows.Count];
            //int i1 = 0;

            //// Add the sheet name to the string array.
            //foreach (DataRow row in dt1.Rows)
            //{
            //    excelSheets[i1] = row["TABLE_NAME"].ToString();
            //    i1++;
            //}
            //// make sure your sheet name is correct, here sheet name is sheet1, so you can change your sheet name if have     different
            //string myexceldataquery = "select * from [" + excelSheets[0] + "]";
            //try
            //{
            //    //create our connection strings                
            //    string ssqlconnectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //    //execute a query to erase any previous data from our destination table
            //    string sclearsql = "select * from " + ssqltable;
            //    SqlConnection sqlconn = new SqlConnection(ssqlconnectionstring);
            //    SqlCommand sqlcmd = new SqlCommand(sclearsql, sqlconn);
            //    sqlconn.Open();
            //    sqlcmd.ExecuteNonQuery();
            //    sqlconn.Close();


            //    using (OleDbDataAdapter adaptor = new OleDbDataAdapter(myexceldataquery, excelConnection))
            //    {
            //        DataSet ds = new DataSet();
            //        adaptor.Fill(ds);
            //        System.Data.DataTable dt = ds.Tables[0];
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            //DateTime today = System.DateTime.Now;
            //            DateTime today = Convert.ToDateTime(dt.Rows[i][4]);
            //            bool IsPresent = false;
            //            if (dt.Rows[i][5].ToString() == "P" || dt.Rows[i][5].ToString() == "true")
            //            {
            //                IsPresent = true;
            //            }
            //            else
            //            {
            //                IsPresent = false;
            //            }
            //            if (!rm.IsStudentAttendenceExists(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), today))
            //                rm.InsertStudentAttendence(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), today, IsPresent, dt.Rows[i][6].ToString());
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ex.ToString();
            //    //handle exception
            //}

        }

        [HttpGet]
        [System.Web.Mvc.Authorize(Roles = "Admin")]
        public ActionResult UserBulkUpload(string SubscriberId, string ExistedNumber)
        {
            ViewBag.ExistedSaved = ExistedNumber;

            string UserId = User.Identity.GetUserId();
            UserViewModel userDetails = generic.GetUserDetail(UserId);
            ViewData["UserProfile"] = userDetails;
            //if (TempData["ExistedPhoneNumberSaved"] != null)
            //    ViewBag.ExistedPhoneNumberSaved = TempData["ExistedPhoneNumberSaved"].ToString();
            if (ViewBag.ExistedEmailSaved != null)
                ViewBag.ExistedEmailSaved1 = ViewBag.ExistedEmailSaved;
            if (ViewBag.ExistedPhoneNumber != null)
                ViewBag.ExistedPhoneNumber1 = ViewBag.ExistedPhoneNumber;
            if (ViewBag.ExistedEmail != null)
                ViewBag.ExistedEmail1 = ViewBag.ExistedEmail;
            return View();
        }

        private List<string> PopulateRole()
        {
            string ssqlconnectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string strgetrole = "select distinct RoleId from DepartmentMasters";
            SqlConnection sqlconn = new SqlConnection(ssqlconnectionstring);
            SqlCommand sqlcmd = new SqlCommand(strgetrole, sqlconn);
            sqlconn.Open();
            List<string> l = new List<string>();
            SqlDataReader rd = sqlcmd.ExecuteReader();
            int i = 0;
            while (rd.Read())
            {
                l[i] = rd[0].ToString();
            }
            sqlconn.Close();
            return l;

            //Microsoft.Office.Interop.Excel.Application xlapp = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook xlwb;
            //Microsoft.Office.Interop.Excel.Worksheet xlws;
            //Microsoft.Office.Interop.Excel.Range xlrng;

            //xlwb = xlapp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

            //xlws = xlwb.Worksheets.Add();
            //xlws.Name = "New Sheet";

            //xlws = xlwb.Worksheets.get_Item(1);

            //xlrng = xlws.get_Range("A1").get_Resize(10);
            //xlrng.Name = "DropDownList";
            //xlrng.Value = xlapp.Application.WorksheetFunction.Transpose(Role);

            //xlrng = xlrng.get_Offset(0, 1);

            //xlrng.Validation.Delete();

            //xlrng.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList, Microsoft.Office.Interop.Excel.XlDVAlertStyle.xlValidAlertInformation, Type.Missing, "=DropDownList");

            //xlapp.Visible = true;
            //xlapp.UserControl = true;
        }

        public ActionResult DownloadUserFormat()
        {
            string path = Path.Combine(Server.MapPath("~/Content/UserBulkUpload.xlsx"));
            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserBulkUpload.xlsx");
            //System.Data.DataTable dt = new System.Data.DataTable();
            //dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Name", typeof(string)),    
            //        new DataColumn("ContactNo", typeof(string)),    
            //        new DataColumn("emailAddress",typeof(string)),
            //new DataColumn("Role",typeof(Array)),
            //new DataColumn("department",typeof(Array))});
            //List<string> Role = PopulateRole();

            //for (int i = 0; i < 10000; i++)
            //{
            //    dt.Rows.Add("", "", "", Role, "");
            //}
            //GridView gv = new GridView();
            //gv.DataSource = dt;
            ////BoundField bfield = new BoundField();
            ////bfield.HeaderText = "Name";
            ////bfield.DataField = "Name";
            ////gv.Columns.Add(bfield);
            ////BoundField contactNo = new BoundField();
            ////bfield.HeaderText = "Contact No";
            ////bfield.DataField = "ContactNo";
            ////gv.Columns.Add(contactNo);
            ////BoundField emailAddress = new BoundField();
            ////bfield.HeaderText = "Email Address";
            ////bfield.DataField = "emailAddress";
            ////gv.Columns.Add(emailAddress);
            ////BoundField role = new BoundField();
            ////bfield.HeaderText = "Role";
            ////bfield.DataField = "Role";
            ////gv.Columns.Add(role);
            ////BoundField department = new BoundField();
            ////bfield.HeaderText = "Department";
            ////bfield.DataField = "department";
            ////gv.Columns.Add(department);


            //gv.DataBind();
            //////foreach (TableCell cell in gv.HeaderRow.Cells)
            //////{
            //////    dt.Columns.Add(cell.Text);
            //////}
            //////foreach (GridViewRow row in gv.Rows)
            //////{
            //////    dt.Rows.Add();
            //////    for (int i = 0; i < row.Cells.Count; i++)
            //////    {
            //////        dt.Rows[dt.Rows.Count - 1][i] = row.Cells[i].Text;
            //////    }
            //////}
            ////    string ssqlconnectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            ////    string strgetrole = "select distinct RoleId from DepartmentMasters";
            ////    SqlConnection sqlconn = new SqlConnection(ssqlconnectionstring);
            ////    SqlCommand sqlcmd = new SqlCommand(strgetrole, sqlconn);
            ////    sqlconn.Open();
            ////    List<string> l = new List<string>();
            ////    SqlDataReader rd = sqlcmd.ExecuteReader();
            ////    int i = 0;
            ////    while (rd.Read())
            ////    {
            ////        l[i] = rd[0].ToString();
            ////    }
            ////    Microsoft.Office.Interop.Excel.Application xlapp = new Microsoft.Office.Interop.Excel.Application();
            ////    Microsoft.Office.Interop.Excel.Workbook xlwb;
            ////    Microsoft.Office.Interop.Excel.Worksheet xlws;
            ////    Microsoft.Office.Interop.Excel.Range xlrng;

            ////    xlwb = xlapp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

            ////    xlws = xlwb.Worksheets.Add();
            ////    xlws.Name = "New Sheet";

            ////    xlws = xlwb.Worksheets.get_Item(1);

            ////    xlrng = xlws.get_Range("A1").get_Resize(30);
            ////    xlrng.Name = "DropDownList";
            ////    xlrng.Value = xlapp.Application.WorksheetFunction.Transpose(l);

            ////    xlrng = xlrng.get_Offset(0, 1);

            ////    xlrng.Validation.Delete();

            ////    xlrng.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList, Microsoft.Office.Interop.Excel.XlDVAlertStyle.xlValidAlertInformation, Type.Missing, "=DropDownList");

            ////    xlapp.Visible = true;
            ////    xlapp.UserControl = true;
            //using (XLWorkbook wb = new XLWorkbook())
            //{
            //    wb.Worksheets.Add(dt);
            //    Response.Clear();
            //    Response.Buffer = true;
            //    Response.Charset = "";
            //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    Response.AddHeader("content-disposition", "attachment;filename=GridView.xlsx");
            //    using (MemoryStream MyMemoryStream = new MemoryStream())
            //    {
            //        wb.SaveAs(MyMemoryStream);
            //        MyMemoryStream.WriteTo(Response.OutputStream);
            //        Response.Flush();
            //        Response.End();
            //    }
            //}
            //return View();
        }

        private string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }

        [HttpPost]
        public ActionResult UserBulkUpload()
        {
            //ProgressHub.SendMessage("initializing and preparing", 2);
            GridView GridView1 = new GridView();
            int r = Request.Files.Count;
            string ExistedNumber = string.Empty;
            if (r > 0)
            {
                HttpPostedFileBase file = Request.Files[0]; //Uploaded file
                //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string[] values = fileName.Split('.');
                string SheetName = values[0];
                string filetype = file.GetType().ToString();
                //OleDbConnection objConn = null;
                //System.Data.DataTable dt1 = null;



                //fileName = Server.MapPath(Path.Combine("~/Content/",Path.ChangeExtension(fileName, ".xlsx")));
                //fileName = Server.MapPath(Path.Combine("~/Content/MyExcelFile_files/", Path.ChangeExtension(fileName, ".xlsx")));
                //file.SaveAs(fileName);
                fileName = Server.MapPath(Path.Combine("~/Content/MyExcelFile_files/", fileName));


                file.SaveAs(Path.ChangeExtension(fileName, ".xlsx"));
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
                {
                    //Read the first Sheet from Excel file.
                    Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

                    //Get the Worksheet instance.
                    Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

                    //Fetch all the rows present in the Worksheet.
                    IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Loop through the Worksheet rows.
                    foreach (Row row in rows)
                    {
                        //Use the first row to add columns to DataTable.
                        if (row.RowIndex.Value == 1)
                        {
                            foreach (Cell cell in row.Descendants<Cell>())
                            {
                                dt.Columns.Add(GetValue(doc, cell));
                            }
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dt.Rows.Add();
                            int i = 0;
                            foreach (Cell cell in row.Descendants<Cell>())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = GetValue(doc, cell);
                                i++;
                            }
                        }
                    }
                    //GridView1.DataSource = dt;
                    //GridView1.DataBind();
                    int AlreadyExist = 0; string PhoneNumber = "", EmailAddress = "", PhoneNumberSaved = "", EmailAddressSaved = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][1].ToString() != string.Empty)
                        {
                            var user = new ApplicationUser { UserName = admin.GenerateUserName(), Email = dt.Rows[i][2].ToString(), PhoneNumber = dt.Rows[i][1].ToString() };
                            int count = admin.GetUserDetails(generic.GetSubscriberId(User.Identity.GetUserId())).Where(c => c.PhoneNumber == dt.Rows[i][1].ToString() || c.Email == dt.Rows[i][2].ToString()).Count();
                            if (count == 0)
                            {
                                rm.InsertBulkUser(user.Id, generic.GetSubscriberId(User.Identity.GetUserId()), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][0].ToString(), dt.Rows[i][4].ToString());
                                PhoneNumberSaved = PhoneNumberSaved + dt.Rows[i][1].ToString() + ",";
                                EmailAddressSaved = EmailAddressSaved + dt.Rows[i][2].ToString() + ",";
                            }
                            else
                            {
                                AlreadyExist = AlreadyExist + 1;
                                PhoneNumber = PhoneNumber + dt.Rows[i][1].ToString() + ",";
                                EmailAddress = EmailAddress + dt.Rows[i][2].ToString() + ",";
                            }
                        }
                    }
                    doc.Close();
                    if (PhoneNumberSaved != "" && EmailAddressSaved != "")
                    {
                        ExistedNumber = PhoneNumberSaved.TrimEnd(',');
                        ViewBag.BulkUserUpdated = true;
                        ViewBag.ExistedPhoneNumberSaved = PhoneNumberSaved.TrimEnd(',');
                        ViewBag.ExistedEmailSaved = PhoneNumberSaved.TrimEnd(',');
                    }
                    else if (PhoneNumber != "" && EmailAddress != "")
                    {
                        ExistedNumber = PhoneNumber.TrimEnd(',');
                        ViewBag.AlreadyExist = true;
                        ViewBag.ExistedPhoneNumber = PhoneNumber.TrimEnd(',');
                        ViewBag.ExistedEmail = EmailAddress.TrimEnd(',');
                    }
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    if (System.IO.File.Exists(fileName))
                    {
                        if (System.IO.File.Exists(fileName))
                            System.IO.File.Delete(fileName);
                    }
                    //return View();
                }
            }
            return RedirectToAction("UserBulkUpload", "BulkUpload", new { ExistedNumber = ExistedNumber });
            //ViewBag.ExistedSaved = ExistedNumber;
            //return View();
        }

        public void UploadUser()
        {
            GridView GridView1 = new GridView();
            int r = Request.Files.Count;
            HttpPostedFileBase file = Request.Files[0]; //Uploaded file
            //Use the following properties to get file's name, size and MIMEType
            int fileSize = file.ContentLength;
            string fileName = file.FileName;
            string[] values = fileName.Split('.');
            string SheetName = values[0];
            string filetype = file.GetType().ToString();
            //OleDbConnection objConn = null;
            //System.Data.DataTable dt1 = null;

            //fileName = Server.MapPath(Path.Combine("~/Content/",Path.ChangeExtension(fileName, ".xlsx")));
            //fileName = Server.MapPath(Path.Combine("~/Content/MyExcelFile_files/", Path.ChangeExtension(fileName, ".xlsx")));
            //file.SaveAs(fileName);
            fileName = Server.MapPath(Path.Combine("~/Content/", fileName));


            file.SaveAs(Path.ChangeExtension(fileName, ".xlsx"));
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                //Read the first Sheet from Excel file.
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

                //Get the Worksheet instance.
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

                //Fetch all the rows present in the Worksheet.
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                //Create a new DataTable.
                DataTable dt = new DataTable();

                //Loop through the Worksheet rows.
                foreach (Row row in rows)
                {
                    //Use the first row to add columns to DataTable.
                    if (row.RowIndex.Value == 1)
                    {
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dt.Columns.Add(GetValue(doc, cell));
                        }
                    }
                    else
                    {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = GetValue(doc, cell);
                            i++;
                        }
                    }
                }
                //GridView1.DataSource = dt;
                //GridView1.DataBind();
                int AlreadyExist = 0; string PhoneNumber = "", EmailAddress = "", PhoneNumberSaved = "", EmailAddressSaved = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][1].ToString() != string.Empty)
                    {
                        var user = new ApplicationUser { UserName = admin.GenerateUserName(), Email = dt.Rows[i][2].ToString(), PhoneNumber = dt.Rows[i][1].ToString() };
                        int count = admin.GetUserDetails(generic.GetSubscriberId(User.Identity.GetUserId())).Where(c => c.PhoneNumber == dt.Rows[i][1].ToString() || c.Email == dt.Rows[i][2].ToString()).Count();
                        if (count == 0)
                        {
                            rm.InsertBulkUser(user.Id, generic.GetSubscriberId(User.Identity.GetUserId()), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][0].ToString(), dt.Rows[i][4].ToString());
                            PhoneNumberSaved = PhoneNumberSaved + dt.Rows[i][1].ToString() + ",";
                            EmailAddressSaved = EmailAddressSaved + dt.Rows[i][2].ToString() + ",";
                        }
                        else
                        {
                            AlreadyExist = AlreadyExist + 1;
                            PhoneNumber = PhoneNumber + dt.Rows[i][1].ToString() + ",";
                            EmailAddress = EmailAddress + dt.Rows[i][2].ToString() + ",";
                        }
                    }
                }

                doc.Close();
                if (PhoneNumberSaved != "" && EmailAddressSaved != "")
                {
                    ViewBag.BulkUserUpdated = true;
                    ViewBag.ExistedPhoneNumberSaved = PhoneNumberSaved.TrimEnd(',');
                    ViewBag.ExistedEmailSaved = PhoneNumberSaved.TrimEnd(',');
                }
                else if (PhoneNumber != "" && EmailAddress != "")
                {
                    ViewBag.AlreadyExist = true;
                    ViewBag.ExistedPhoneNumber = PhoneNumber.TrimEnd(',');
                    ViewBag.ExistedEmail = EmailAddress.TrimEnd(',');
                }
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                if (System.IO.File.Exists(fileName))
                {
                    if (System.IO.File.Exists(fileName))
                        System.IO.File.Delete(fileName);
                }
            }

            TempData["BulkUserUpdated"] = true;
        }


    }
}