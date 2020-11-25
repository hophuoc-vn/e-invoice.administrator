using ManageHDDT.Helpers;
using ManageHDDT.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageHDDT.Controllers
{
    public class FeatureController : Controller
    {
        #region Khai báo hằng số
        hddt_data_HD_Entities _dbDataHD = new hddt_data_HD_Entities();
        hddt_qldv_HD_Entities _dbQldvHD = new hddt_qldv_HD_Entities();
        hddt_data_MIFI_Entities _dbDataMIFI = new hddt_data_MIFI_Entities();
        hddt_qldv_MIFI_Entities _dbQldvMIFI = new hddt_qldv_MIFI_Entities();
        hddt_data_tester_Entities _dbDataTester = new hddt_data_tester_Entities();
        hddt_qldv_tester_Entities _dbQldvTester = new hddt_qldv_tester_Entities();
        hddt_data_MB_Entities _dbDataMB = new hddt_data_MB_Entities();
        hddt_qldv_MB_Entities _dbQldvMB = new hddt_qldv_MB_Entities();
        string _failReqMsg = "2000:warning:350:" + ConfigurationManager.AppSettings["FailReqMsg"];
        string _resultError = "2000:error:300:" + ConfigurationManager.AppSettings["ServerErrorMsg"];
        string _resultWarning = "1000:warning:250:";
        string _resultSuccess = "500:success:250:Cập nhật thành công.";
        #endregion

        // GET: Feature
        [SessionCheck]
        public ActionResult ResourceUpdate()
        {
            bool req_auth_resp = new GeneralController().AuthReq("ResourceUpdate");
            if (req_auth_resp)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SupTkts", "Home");
            }
        }        

        [SessionCheck, HttpPost]
        public JsonResult ImportLead(HttpPostedFileBase UploadFile)
        {
            string resultTxt = _resultError;
            bool req_auth_resp = new GeneralController().AuthReq("ImportLead");

            if (req_auth_resp)
            {
                try
                {
                    if (UploadFile.ContentLength == 0)
                    {
                        resultTxt = _resultWarning + "Nội dung file rỗng.";
                        return Json(new { success = false, responseText = resultTxt }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var file_name = Path.GetFileName(UploadFile.FileName);
                        var root_path = Server.MapPath("~/TempFiles/");
                        var file_directory = root_path + file_name;

                        UploadFile.SaveAs(file_directory);
                        if (!System.IO.File.Exists(file_directory))
                        {
                            resultTxt = "1500:warning:300:Không tìm thấy đường dẫn file.";
                            return Json(new { success = false, responseText = resultTxt }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var excel_cnx_str = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file_directory + ";Extended Properties=\"Excel 12.0;HDR=No;IMEX=1\"";
                            LeadHelpers lead_helper = new LeadHelpers();
                            var file_content = lead_helper.GetDataFromExcel(excel_cnx_str);

                            if (Utils.CheckDataTable(file_content))
                            {
                                var odoo_api = new OdooApiService();
                                string odoo_team_code = "HDDT";
                                int odoo_company_id = 1;
                                string odoo_state_code = string.Empty;
                                string odoo_country_code = "VN";
                                //string odoo_source_code = "ImportExcel";
                                int planned_revenue = 100100;
                                DataRow row = null;

                                for (int i = 1; i < file_content.Rows.Count; i++)
                                {
                                    row = file_content.Rows[i];

                                    string street = string.Format("{0}", row[21]);
                                    string city = string.Format("{0}", row[18]);
                                    string zip = string.Empty;
                                    string company_name = string.Format("{0}", row[13]);
                                    string contact_name = string.Format("{0}", row[9]);
                                    string function = string.Format("{0}", row[11]);
                                    string phone = string.Format("{0}", row[1]);
                                    string email = string.Format("{0}", row[14]);
                                    string odoo_source_code = string.Format("{0}", row[2]);
                                    string description = string.Format("Last day contact: {0}."
                                        + "\nGroup status: {1}. Last status call: {2}."
                                        + "\nNoted: {3}.\nPhòng ban: {4}."
                                        + "\nSố hóa đơn cần/tháng (năm): {5}."
                                        + "\nNgành nghề: {6}."
                                        + "\nXuất cho bao nhiêu MST: {7}."
                                        + "\nĐã có Chữ ký số: {8}."
                                        + "\nĐang dùng bên nào: {9}."
                                        + "\nCòn bao nhiêu Hóa đơn giấy: {10}."
                                        + "\nKhi nào sẽ triển khai: {11}."
                                        + "\nLưu ý tính cách KH: Dễ/Khó/Bình thường: {12}",
                                        string.Format("{0}", row[3]),
                                        string.Format("{0}", row[4]),
                                        string.Format("{0}", row[5]),
                                        string.Format("{0}", row[7]),
                                        string.Format("{0}", row[10]),
                                        string.Format("{0}", row[12]),
                                        string.Format("{0}", row[15]),
                                        string.Format("{0}", row[16]),
                                        string.Format("{0}", row[17]),
                                        string.Format("{0}", row[19]),
                                        string.Format("{0}", row[20]),
                                        string.Format("{0}", row[22]),
                                        string.Format("{0}", row[23]));

                                    odoo_api.CreateLead(odoo_team_code, odoo_company_id, odoo_state_code, odoo_country_code, odoo_source_code, street, city, zip, company_name, contact_name, contact_name, function, phone, email, phone, description, planned_revenue);
                                }

                                resultTxt = _resultSuccess;
                                Json(new { success = true, responseText = resultTxt }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/ImportLead, dòng {0}: {1}", exc_line, exc.Message));
                    return Json(new { success = false, responseText = resultTxt }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                resultTxt = _failReqMsg;
            }
            return Json(new { success = false, responseText = resultTxt }, JsonRequestBehavior.AllowGet);
        }

        public string CheckTableStack(string FieldName, int RecStack, int DbId)
        {
            string result = string.Empty;
            int table_rec_count = 0;

            switch (DbId)
            {
                case 1:
                    try
                    {
                        switch (FieldName)
                        {
                            case "EmailTmpl":
                                table_rec_count = _dbDataHD.MailTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvTmpl":
                                table_rec_count = _dbQldvHD.InvoiceTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "TaxAuth":
                                table_rec_count = _dbQldvHD.TaxAuthorities.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Unit":
                                table_rec_count = _dbQldvHD.Units.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Notif":
                                table_rec_count = _dbQldvMIFI.Notifications.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvCat":
                                table_rec_count = _dbQldvHD.InvCategories.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                        }
                    }
                    catch (Exception exc)
                    {                        
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/CheckTableStack, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
                case 2:
                    try
                    {
                        switch (FieldName)
                        {
                            case "EmailTmpl":
                                table_rec_count = _dbDataMIFI.MailTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvTmpl":
                                table_rec_count = _dbQldvMIFI.InvoiceTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "TaxAuth":
                                table_rec_count = _dbQldvMIFI.TaxAuthorities.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Unit":
                                table_rec_count = _dbQldvMIFI.Units.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Notif":
                                table_rec_count = _dbQldvMIFI.Notifications.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvCat":
                                table_rec_count = _dbQldvMIFI.InvCategories.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/CheckTableStack, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
                case 3:
                    try
                    {
                        switch (FieldName)
                        {
                            case "EmailTmpl":
                                table_rec_count = _dbDataMB.MailTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvTmpl":
                                table_rec_count = _dbQldvMB.InvoiceTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "TaxAuth":
                                table_rec_count = _dbQldvMB.TaxAuthorities.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Unit":
                                table_rec_count = _dbQldvMB.Units.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Notif":
                                table_rec_count = _dbQldvMB.Notifications.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvCat":
                                table_rec_count = _dbQldvMB.InvCategories.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/CheckTableStack, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
                case 0:
                    try
                    {
                        switch (FieldName)
                        {
                            case "EmailTmpl":
                                table_rec_count = _dbDataTester.MailTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvTmpl":
                                table_rec_count = _dbQldvTester.InvoiceTemplates.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "TaxAuth":
                                table_rec_count = _dbQldvTester.TaxAuthorities.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Unit":
                                table_rec_count = _dbQldvTester.Units.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "Notif":
                                table_rec_count = _dbQldvMIFI.Notifications.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                            case "InvCat":
                                table_rec_count = _dbQldvTester.InvCategories.Count();
                                if (RecStack - table_rec_count > 0)
                                {
                                    result = bool.FalseString;
                                }
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/CheckTableStack, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
            }
            return result;
        }

        public JsonResult GetEmailTmplList(int RecStack, int RecQty, int DbId)
        {
            List<EmailTmplDataModel> result = new List<EmailTmplDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var email_tmpl_array = _dbDataHD.MailTemplates
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                }).ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var email_tmpl_array = _dbDataHD.MailTemplates
                                .OrderBy(mt => mt.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetEmailTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var email_tmpl_array = _dbDataMIFI.MailTemplates
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                }).ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var email_tmpl_array = _dbDataMIFI.MailTemplates
                                .OrderBy(mt => mt.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetEmailTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var email_tmpl_array = _dbDataMB.MailTemplates
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                }).ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var email_tmpl_array = _dbDataMB.MailTemplates
                                .OrderBy(mt => mt.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetEmailTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var email_tmpl_array = _dbDataTester.MailTemplates
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                }).ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var email_tmpl_array = _dbDataTester.MailTemplates
                                .OrderBy(mt => mt.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    CoId = s.ComID,
                                    SenderEmail = s.EmailFrom,
                                    Sender = s.EmailFromLabel,
                                    EmailSubject = s.Subject,
                                    EmailContent = s.TemContent,
                                    EmailType = s.Type
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in email_tmpl_array)
                            {
                                EmailTmplDataModel model_instance = new EmailTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.CoId = item.CoId.Value;
                                model_instance.SenderEmail = item.SenderEmail;
                                model_instance.Sender = item.Sender;
                                model_instance.EmailSubject = item.EmailSubject;
                                model_instance.EmailContent = item.EmailContent;
                                model_instance.EmailType = item.EmailType.Value;
                                result.Add(model_instance);
                            }
                        }                        
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetEmailTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsEmailTmpl(string UpdateData, int DbId)
        {
            string result = _resultError;
            MailTemplate model_instance = new MailTemplate();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "CoId":
                        model_instance.ComID = int.Parse(data_val.ToString());
                        break;
                    case "SenderEmail":
                        model_instance.EmailFrom = data_val.ToString();
                        break;
                    case "Sender":
                        model_instance.EmailFromLabel = data_val.ToString();
                        break;
                    case "EmailSubject":
                        model_instance.Subject = data_val.ToString();
                        break;
                    case "EmailContent":
                        model_instance.TemContent = data_val.ToString();
                        break;
                    case "EmailType":
                        model_instance.Type = int.Parse(data_val.ToString());
                        break;
                }
            }
            if (model_instance.Type == null) model_instance.Type = 0;
            if (model_instance.EmailFromLabel == null) model_instance.EmailFromLabel = null;
            switch (DbId)
            {
                case 1:
                    try
                    {
                        _dbDataHD.MailTemplates.Add(model_instance);
                        _dbDataHD.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 2:
                    try
                    {
                        _dbDataMIFI.MailTemplates.Add(model_instance);
                        _dbDataMIFI.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 3:
                    try
                    {
                        _dbDataMB.MailTemplates.Add(model_instance);
                        _dbDataMB.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 0:
                    try
                    {
                        _dbDataTester.MailTemplates.Add(model_instance);
                        _dbDataTester.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;

            }
            return result;
        }

        public string UpdateEmailTmpl(int TmplId, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var email_tmpl_to_update = _dbDataHD.MailTemplates.SingleOrDefault(mt => mt.id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoId":
                            email_tmpl_to_update.ComID = int.Parse(data_val.ToString());
                            break;
                        case "SenderEmail":
                            email_tmpl_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "Sender":
                            email_tmpl_to_update.EmailFromLabel = data_val.ToString();
                            break;
                        case "EmailSubject":
                            email_tmpl_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_tmpl_to_update.TemContent = data_val.ToString();
                            break;
                        case "EmailType":
                            email_tmpl_to_update.Type = int.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbDataHD.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var email_tmpl_to_update = _dbDataMIFI.MailTemplates.SingleOrDefault(mt => mt.id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoId":
                            email_tmpl_to_update.ComID = int.Parse(data_val.ToString());
                            break;
                        case "SenderEmail":
                            email_tmpl_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "Sender":
                            email_tmpl_to_update.EmailFromLabel = data_val.ToString();
                            break;
                        case "EmailSubject":
                            email_tmpl_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_tmpl_to_update.TemContent = data_val.ToString();
                            break;
                        case "EmailType":
                            email_tmpl_to_update.Type = int.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbDataMIFI.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var email_tmpl_to_update = _dbDataMB.MailTemplates.SingleOrDefault(mt => mt.id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoId":
                            email_tmpl_to_update.ComID = int.Parse(data_val.ToString());
                            break;
                        case "SenderEmail":
                            email_tmpl_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "Sender":
                            email_tmpl_to_update.EmailFromLabel = data_val.ToString();
                            break;
                        case "EmailSubject":
                            email_tmpl_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_tmpl_to_update.TemContent = data_val.ToString();
                            break;
                        case "EmailType":
                            email_tmpl_to_update.Type = int.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbDataMB.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var email_tmpl_to_update = _dbDataTester.MailTemplates.SingleOrDefault(mt => mt.id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoId":
                            email_tmpl_to_update.ComID = int.Parse(data_val.ToString());
                            break;
                        case "SenderEmail":
                            email_tmpl_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "Sender":
                            email_tmpl_to_update.EmailFromLabel = data_val.ToString();
                            break;
                        case "EmailSubject":
                            email_tmpl_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_tmpl_to_update.TemContent = data_val.ToString();
                            break;
                        case "EmailType":
                            email_tmpl_to_update.Type = int.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbDataTester.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateEmailTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetInvTmplList(int RecStack, int RecQty, int DbId)
        {
            List<InvTmplDataModel> result = new List<InvTmplDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_tmpl_array = _dbQldvHD.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                }).ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_tmpl_array = _dbQldvHD.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetInvTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_tmpl_array = _dbQldvMIFI.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                }).ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_tmpl_array = _dbQldvMIFI.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetInvTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_tmpl_array = _dbQldvMB.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                }).ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_tmpl_array = _dbQldvMB.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetInvTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_tmpl_array = _dbQldvTester.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                }).ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_tmpl_array = _dbQldvTester.InvoiceTemplates
                                .OrderBy(it => it.InvCateID)
                                .ThenBy(it => it.Id)
                                .Select(s => new
                                {
                                    s.Id,
                                    InvCatId = s.InvCateID,
                                    InvCatname = s.InvCateName,
                                    TmplName = s.TemplateName,
                                    TmplXml = s.XmlFile,
                                    TmplXslt = s.XsltFile,
                                    SvcType = s.ServiceType,
                                    InvType = s.InvoiceType,
                                    InvView = s.InvoiceView,
                                    iGenerator = s.IGenerator,
                                    iViewer = s.IViewer,
                                    TmplCss = s.CssData,
                                    TmplThumbnailDir = s.ImagePath,
                                    s.IsPub,
                                    IsCert = s.IsCertify
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_tmpl_array)
                            {
                                InvTmplDataModel model_instance = new InvTmplDataModel();

                                model_instance.Id = item.Id;
                                model_instance.InvCatId = item.InvCatId.Value;
                                model_instance.InvCatName = item.InvCatname;
                                model_instance.TmplName = item.TmplName;
                                model_instance.TmplXml = item.TmplXml;
                                model_instance.TmplXslt = item.TmplXslt;
                                model_instance.SvcType = item.SvcType;
                                model_instance.InvType = item.InvType;
                                model_instance.InvView = item.InvView;
                                model_instance.iGenerator = item.iGenerator;
                                model_instance.iViewer = item.iViewer;
                                model_instance.TmplCss = item.TmplCss;
                                model_instance.TmplThumbnailDir = item.TmplThumbnailDir;
                                model_instance.IsPub = item.IsPub == false ? "false" : "true";
                                model_instance.IsCert = item.IsCert == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }                        
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetInvTmplList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsInvTmpl(string UpdateData, int DbId)
        {
            string result = _resultError;
            InvoiceTemplate model_instance = new InvoiceTemplate();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "InvCatId":
                        model_instance.InvCateID = int.Parse(data_val.ToString());
                        break;
                    case "InvCatName":
                        model_instance.InvCateName = data_val.ToString();
                        break;
                    case "TmplName":
                        model_instance.TemplateName = data_val.ToString();
                        break;
                    case "TmplXml":
                        model_instance.XmlFile = data_val.ToString();
                        break;
                    case "TmplXslt":
                        model_instance.XsltFile = data_val.ToString();
                        break;
                    case "SvcType":
                        model_instance.ServiceType = data_val.ToString();
                        break;
                    case "InvType":
                        model_instance.InvoiceType = data_val.ToString();
                        break;
                    case "InvView":
                        model_instance.InvoiceView = data_val.ToString();
                        break;
                    case "iGenerator":
                        model_instance.IGenerator = data_val.ToString();
                        break;
                    case "iViewer":
                        model_instance.IViewer = data_val.ToString();
                        break;
                    case "TmplCss":
                        model_instance.CssData = data_val.ToString();
                        break;
                    case "TmplThumbnailDir":
                        model_instance.ImagePath = data_val.ToString();
                        break;
                    case "IsPub":
                        model_instance.IsPub = bool.Parse(data_val.ToString());
                        break;
                    case "IsCert":
                        model_instance.IsCertify = bool.Parse(data_val.ToString());
                        break;
                }
            }
            if (model_instance.InvCateName == null)
            {
                switch (model_instance.InvCateID)
                {
                    case 1:
                        model_instance.InvCateName = "Hóa đơn giá trị gia tăng";
                        break;
                    case 2:
                        model_instance.InvCateName = "Hóa đơn bán hàng";
                        break;
                    case 3:
                        model_instance.InvCateName = "Phiếu xuất kho";
                        break;
                }                
            }
            if (model_instance.ServiceType == null) model_instance.ServiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
            if (model_instance.InvoiceType == null)
            {
                if (model_instance.InvCateID == 1) model_instance.InvoiceType = "EInvoice.Mapping.Domain.ProductVATInvoice,EInvoice.Mapping";
                else model_instance.InvoiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
            }
            if (model_instance.InvoiceView == null) model_instance.InvoiceView = "PRODVAT";
            if (model_instance.IGenerator == null) model_instance.IGenerator = "EInvoice.Core.Launching.DsigGenerator";
            if (model_instance.IViewer == null) model_instance.IViewer = "EInvoice.Core.Viewer.DsigViewer";
            if (model_instance.IsCertify == null) model_instance.IsCertify = false;
            switch (DbId)
            {
                case 1:
                    try
                    {
                        _dbQldvHD.InvoiceTemplates.Add(model_instance);
                        _dbQldvHD.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 2:
                    try
                    {
                        _dbQldvMIFI.InvoiceTemplates.Add(model_instance);
                        _dbQldvMIFI.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 3:
                    try
                    {
                        _dbQldvMB.InvoiceTemplates.Add(model_instance);
                        _dbQldvMB.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 0:
                    try
                    {
                        _dbQldvTester.InvoiceTemplates.Add(model_instance);
                        _dbQldvTester.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
            }
            return result;
        }

        public string UpdateInvTmpl(int TmplId, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var inv_tmpl_to_update = _dbQldvHD.InvoiceTemplates.SingleOrDefault(it => it.Id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "InvCatId":
                            inv_tmpl_to_update.InvCateID = int.Parse(data_val.ToString());
                            break;
                        case "InvCatName":
                            inv_tmpl_to_update.InvCateName = data_val.ToString();
                            break;
                        case "TmplName":
                            inv_tmpl_to_update.TemplateName = data_val.ToString();
                            break;
                        case "TmplXml":
                            inv_tmpl_to_update.XmlFile = data_val.ToString();
                            break;
                        case "TmplXslt":
                            inv_tmpl_to_update.XsltFile = data_val.ToString();
                            break;
                        case "SvcType":
                            inv_tmpl_to_update.ServiceType = data_val.ToString();
                            break;
                        case "InvType":
                            inv_tmpl_to_update.InvoiceType = data_val.ToString();
                            break;
                        case "InvView":
                            inv_tmpl_to_update.InvoiceView = data_val.ToString();
                            break;
                        case "iGenerator":
                            inv_tmpl_to_update.IGenerator = data_val.ToString();
                            break;
                        case "iViewer":
                            inv_tmpl_to_update.IViewer = data_val.ToString();
                            break;
                        case "TmplCss":
                            inv_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "TmplThumbnailDir":
                            inv_tmpl_to_update.ImagePath = data_val.ToString();
                            break;
                        case "IsPub":
                            inv_tmpl_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                        case "IsCert":
                            inv_tmpl_to_update.IsCertify = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (inv_tmpl_to_update.InvCateName == null)
                {
                    switch (inv_tmpl_to_update.InvCateID)
                    {
                        case 1:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn giá trị gia tăng";
                            break;
                        case 2:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn bán hàng";
                            break;
                        case 3:
                            inv_tmpl_to_update.InvCateName = "Phiếu xuất kho";
                            break;
                    }
                }
                if (inv_tmpl_to_update.ServiceType == null) inv_tmpl_to_update.ServiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                if (inv_tmpl_to_update.InvoiceType == null)
                {
                    if (inv_tmpl_to_update.InvCateID == 1) inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.Domain.ProductVATInvoice,EInvoice.Mapping";
                    else inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                }
                if (inv_tmpl_to_update.InvoiceView == null) inv_tmpl_to_update.InvoiceView = "PRODVAT";
                if (inv_tmpl_to_update.IGenerator == null) inv_tmpl_to_update.IGenerator = "EInvoice.Core.Launching.DsigGenerator";
                if (inv_tmpl_to_update.IViewer == null) inv_tmpl_to_update.IViewer = "EInvoice.Core.Viewer.DsigViewer";
                if (inv_tmpl_to_update.IsCertify == null) inv_tmpl_to_update.IsCertify = false;
                try
                {
                    _dbQldvHD.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var inv_tmpl_to_update = _dbQldvMIFI.InvoiceTemplates.SingleOrDefault(it => it.Id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "InvCatId":
                            inv_tmpl_to_update.InvCateID = int.Parse(data_val.ToString());
                            break;
                        case "InvCatName":
                            inv_tmpl_to_update.InvCateName = data_val.ToString();
                            break;
                        case "TmplName":
                            inv_tmpl_to_update.TemplateName = data_val.ToString();
                            break;
                        case "TmplXml":
                            inv_tmpl_to_update.XmlFile = data_val.ToString();
                            break;
                        case "TmplXslt":
                            inv_tmpl_to_update.XsltFile = data_val.ToString();
                            break;
                        case "SvcType":
                            inv_tmpl_to_update.ServiceType = data_val.ToString();
                            break;
                        case "InvType":
                            inv_tmpl_to_update.InvoiceType = data_val.ToString();
                            break;
                        case "InvView":
                            inv_tmpl_to_update.InvoiceView = data_val.ToString();
                            break;
                        case "iGenerator":
                            inv_tmpl_to_update.IGenerator = data_val.ToString();
                            break;
                        case "iViewer":
                            inv_tmpl_to_update.IViewer = data_val.ToString();
                            break;
                        case "TmplCss":
                            inv_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "TmplThumbnailDir":
                            inv_tmpl_to_update.ImagePath = data_val.ToString();
                            break;
                        case "IsPub":
                            inv_tmpl_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                        case "IsCert":
                            inv_tmpl_to_update.IsCertify = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (inv_tmpl_to_update.InvCateName == null)
                {
                    switch (inv_tmpl_to_update.InvCateID)
                    {
                        case 1:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn giá trị gia tăng";
                            break;
                        case 2:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn bán hàng";
                            break;
                        case 3:
                            inv_tmpl_to_update.InvCateName = "Phiếu xuất kho";
                            break;
                    }
                }
                if (inv_tmpl_to_update.ServiceType == null) inv_tmpl_to_update.ServiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                if (inv_tmpl_to_update.InvoiceType == null)
                {
                    if (inv_tmpl_to_update.InvCateID == 1) inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.Domain.ProductVATInvoice,EInvoice.Mapping";
                    else inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                }
                if (inv_tmpl_to_update.InvoiceView == null) inv_tmpl_to_update.InvoiceView = "PRODVAT";
                if (inv_tmpl_to_update.IGenerator == null) inv_tmpl_to_update.IGenerator = "EInvoice.Core.Launching.DsigGenerator";
                if (inv_tmpl_to_update.IViewer == null) inv_tmpl_to_update.IViewer = "EInvoice.Core.Viewer.DsigViewer";
                if (inv_tmpl_to_update.IsCertify == null) inv_tmpl_to_update.IsCertify = false;
                try
                {
                    _dbQldvMIFI.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var inv_tmpl_to_update = _dbQldvMB.InvoiceTemplates.SingleOrDefault(it => it.Id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "InvCatId":
                            inv_tmpl_to_update.InvCateID = int.Parse(data_val.ToString());
                            break;
                        case "InvCatName":
                            inv_tmpl_to_update.InvCateName = data_val.ToString();
                            break;
                        case "TmplName":
                            inv_tmpl_to_update.TemplateName = data_val.ToString();
                            break;
                        case "TmplXml":
                            inv_tmpl_to_update.XmlFile = data_val.ToString();
                            break;
                        case "TmplXslt":
                            inv_tmpl_to_update.XsltFile = data_val.ToString();
                            break;
                        case "SvcType":
                            inv_tmpl_to_update.ServiceType = data_val.ToString();
                            break;
                        case "InvType":
                            inv_tmpl_to_update.InvoiceType = data_val.ToString();
                            break;
                        case "InvView":
                            inv_tmpl_to_update.InvoiceView = data_val.ToString();
                            break;
                        case "iGenerator":
                            inv_tmpl_to_update.IGenerator = data_val.ToString();
                            break;
                        case "iViewer":
                            inv_tmpl_to_update.IViewer = data_val.ToString();
                            break;
                        case "TmplCss":
                            inv_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "TmplThumbnailDir":
                            inv_tmpl_to_update.ImagePath = data_val.ToString();
                            break;
                        case "IsPub":
                            inv_tmpl_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                        case "IsCert":
                            inv_tmpl_to_update.IsCertify = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (inv_tmpl_to_update.InvCateName == null)
                {
                    switch (inv_tmpl_to_update.InvCateID)
                    {
                        case 1:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn giá trị gia tăng";
                            break;
                        case 2:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn bán hàng";
                            break;
                        case 3:
                            inv_tmpl_to_update.InvCateName = "Phiếu xuất kho";
                            break;
                    }
                }
                if (inv_tmpl_to_update.ServiceType == null) inv_tmpl_to_update.ServiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                if (inv_tmpl_to_update.InvoiceType == null)
                {
                    if (inv_tmpl_to_update.InvCateID == 1) inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.Domain.ProductVATInvoice,EInvoice.Mapping";
                    else inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                }
                if (inv_tmpl_to_update.InvoiceView == null) inv_tmpl_to_update.InvoiceView = "PRODVAT";
                if (inv_tmpl_to_update.IGenerator == null) inv_tmpl_to_update.IGenerator = "EInvoice.Core.Launching.DsigGenerator";
                if (inv_tmpl_to_update.IViewer == null) inv_tmpl_to_update.IViewer = "EInvoice.Core.Viewer.DsigViewer";
                if (inv_tmpl_to_update.IsCertify == null) inv_tmpl_to_update.IsCertify = false;
                try
                {
                    _dbQldvMB.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var inv_tmpl_to_update = _dbQldvTester.InvoiceTemplates.SingleOrDefault(it => it.Id == TmplId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "InvCatId":
                            inv_tmpl_to_update.InvCateID = int.Parse(data_val.ToString());
                            break;
                        case "InvCatName":
                            inv_tmpl_to_update.InvCateName = data_val.ToString();
                            break;
                        case "TmplName":
                            inv_tmpl_to_update.TemplateName = data_val.ToString();
                            break;
                        case "TmplXml":
                            inv_tmpl_to_update.XmlFile = data_val.ToString();
                            break;
                        case "TmplXslt":
                            inv_tmpl_to_update.XsltFile = data_val.ToString();
                            break;
                        case "SvcType":
                            inv_tmpl_to_update.ServiceType = data_val.ToString();
                            break;
                        case "InvType":
                            inv_tmpl_to_update.InvoiceType = data_val.ToString();
                            break;
                        case "InvView":
                            inv_tmpl_to_update.InvoiceView = data_val.ToString();
                            break;
                        case "iGenerator":
                            inv_tmpl_to_update.IGenerator = data_val.ToString();
                            break;
                        case "iViewer":
                            inv_tmpl_to_update.IViewer = data_val.ToString();
                            break;
                        case "TmplCss":
                            inv_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "TmplThumbnailDir":
                            inv_tmpl_to_update.ImagePath = data_val.ToString();
                            break;
                        case "IsPub":
                            inv_tmpl_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                        case "IsCert":
                            inv_tmpl_to_update.IsCertify = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (inv_tmpl_to_update.InvCateName == null)
                {
                    switch (inv_tmpl_to_update.InvCateID)
                    {
                        case 1:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn giá trị gia tăng";
                            break;
                        case 2:
                            inv_tmpl_to_update.InvCateName = "Hóa đơn bán hàng";
                            break;
                        case 3:
                            inv_tmpl_to_update.InvCateName = "Phiếu xuất kho";
                            break;
                    }
                }
                if (inv_tmpl_to_update.ServiceType == null) inv_tmpl_to_update.ServiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                if (inv_tmpl_to_update.InvoiceType == null)
                {
                    if (inv_tmpl_to_update.InvCateID == 1) inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.Domain.ProductVATInvoice,EInvoice.Mapping";
                    else inv_tmpl_to_update.InvoiceType = "EInvoice.Mapping.ServiceImp.ProductVATInvoiceService,EInvoice.Mapping";
                }
                if (inv_tmpl_to_update.InvoiceView == null) inv_tmpl_to_update.InvoiceView = "PRODVAT";
                if (inv_tmpl_to_update.IGenerator == null) inv_tmpl_to_update.IGenerator = "EInvoice.Core.Launching.DsigGenerator";
                if (inv_tmpl_to_update.IViewer == null) inv_tmpl_to_update.IViewer = "EInvoice.Core.Viewer.DsigViewer";
                if (inv_tmpl_to_update.IsCertify == null) inv_tmpl_to_update.IsCertify = false;
                try
                {
                    _dbQldvTester.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetTaxAuthList(int RecStack, int RecQty, int DbId)
        {
            List<TaxAuthDataModel> result = new List<TaxAuthDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var tax_auth_array = _dbQldvHD.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                }).ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var tax_auth_array = _dbQldvHD.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetTaxAuthList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var tax_auth_array = _dbQldvMIFI.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                }).ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var tax_auth_array = _dbQldvMIFI.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetTaxAuthList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var tax_auth_array = _dbQldvMB.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                }).ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var tax_auth_array = _dbQldvMB.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetTaxAuthList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var tax_auth_array = _dbQldvTester.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                }).ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var tax_auth_array = _dbQldvTester.TaxAuthorities
                                .OrderBy(ta => ta.Code)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name,
                                    s.Address,
                                    s.Phone,
                                    Visibility = s.Visible
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in tax_auth_array)
                            {
                                TaxAuthDataModel model_instance = new TaxAuthDataModel();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                model_instance.Address = item.Address;
                                model_instance.Phone = item.Phone;
                                model_instance.Visibility = item.Visibility == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetTaxAuthList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsTaxAuth(string UpdateData, int DbId)
        {
            string result = _resultError;
            TaxAuthority model_instance = new TaxAuthority();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Code":
                        model_instance.Code = data_val.ToString();
                        break;
                    case "Name":
                        model_instance.Name = data_val.ToString();
                        break;
                    case "Address":
                        model_instance.Address = data_val.ToString();
                        break;
                    case "Phone":
                        model_instance.Phone = data_val.ToString();
                        break;
                    case "Locality":
                        model_instance.City = data_val.ToString();
                        break;
                    case "Visibility":
                        model_instance.Visible = bool.Parse(data_val.ToString());
                        break;                    
                }
            }
            if (model_instance.Visible == null) model_instance.Visible = true;
            switch (DbId)
            {
                case 1:
                    if (_dbQldvHD.TaxAuthorities.Any(ta => ta.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã chi cục thuế đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvHD.TaxAuthorities.Add(model_instance);
                            _dbQldvHD.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 2:
                    if (_dbQldvMIFI.TaxAuthorities.Any(ta => ta.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã chi cục thuế đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvMIFI.TaxAuthorities.Add(model_instance);
                            _dbQldvMIFI.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 3:
                    if (_dbQldvMB.TaxAuthorities.Any(ta => ta.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã chi cục thuế đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvMB.TaxAuthorities.Add(model_instance);
                            _dbQldvMB.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 0:
                    if (_dbQldvTester.TaxAuthorities.Any(ta => ta.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã chi cục thuế đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvTester.TaxAuthorities.Add(model_instance);
                            _dbQldvTester.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
            }
            return result;
        }

        public string UpdateTaxAuth(string Code, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var tax_auth_to_update = _dbQldvHD.TaxAuthorities.SingleOrDefault(ta => ta.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            tax_auth_to_update.Name = data_val.ToString();
                            break;
                        case "Address":
                            tax_auth_to_update.Address = data_val.ToString();
                            break;
                        case "Phone":
                            tax_auth_to_update.Phone = data_val.ToString();
                            break;
                        case "Locality":
                            tax_auth_to_update.City = data_val.ToString();
                            break;
                        case "Visibility":
                            tax_auth_to_update.Visible = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (tax_auth_to_update.Visible == null) tax_auth_to_update.Visible = true;
                try
                {
                    _dbQldvHD.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var tax_auth_to_update = _dbQldvMIFI.TaxAuthorities.SingleOrDefault(ta => ta.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            tax_auth_to_update.Name = data_val.ToString();
                            break;
                        case "Address":
                            tax_auth_to_update.Address = data_val.ToString();
                            break;
                        case "Phone":
                            tax_auth_to_update.Phone = data_val.ToString();
                            break;
                        case "Locality":
                            tax_auth_to_update.City = data_val.ToString();
                            break;
                        case "Visibility":
                            tax_auth_to_update.Visible = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (tax_auth_to_update.Visible == null) tax_auth_to_update.Visible = true;
                try
                {
                    _dbQldvMIFI.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var tax_auth_to_update = _dbQldvMB.TaxAuthorities.SingleOrDefault(ta => ta.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            tax_auth_to_update.Name = data_val.ToString();
                            break;
                        case "Address":
                            tax_auth_to_update.Address = data_val.ToString();
                            break;
                        case "Phone":
                            tax_auth_to_update.Phone = data_val.ToString();
                            break;
                        case "Locality":
                            tax_auth_to_update.City = data_val.ToString();
                            break;
                        case "Visibility":
                            tax_auth_to_update.Visible = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (tax_auth_to_update.Visible == null) tax_auth_to_update.Visible = true;
                try
                {
                    _dbQldvMB.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var tax_auth_to_update = _dbQldvTester.TaxAuthorities.SingleOrDefault(ta => ta.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            tax_auth_to_update.Name = data_val.ToString();
                            break;
                        case "Address":
                            tax_auth_to_update.Address = data_val.ToString();
                            break;
                        case "Phone":
                            tax_auth_to_update.Phone = data_val.ToString();
                            break;
                        case "Locality":
                            tax_auth_to_update.City = data_val.ToString();
                            break;
                        case "Visibility":
                            tax_auth_to_update.Visible = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                if (tax_auth_to_update.Visible == null) tax_auth_to_update.Visible = true;
                try
                {
                    _dbQldvTester.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateTaxAuth, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetUnitList(int RecStack, int RecQty, int DbId)
        {
            List<Unit> result = new List<Unit>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var unit_array = _dbQldvHD.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                }).ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var unit_array = _dbQldvHD.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetUnitList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var unit_array = _dbQldvMIFI.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                }).ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var unit_array = _dbQldvMIFI.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetUnitList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var unit_array = _dbQldvMB.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                }).ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var unit_array = _dbQldvMB.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetUnitList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var unit_array = _dbQldvTester.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                }).ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var unit_array = _dbQldvTester.Units
                                .OrderBy(u => u.Name)
                                .Select(s => new
                                {
                                    s.Code,
                                    s.Name
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in unit_array)
                            {
                                Unit model_instance = new Unit();

                                model_instance.Code = item.Code;
                                model_instance.Name = item.Name;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetUnitList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsUnit(string UpdateData, int DbId)
        {
            string result = _resultError;
            Unit model_instance = new Unit();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Code":
                        model_instance.Code = data_val.ToString();
                        break;
                    case "Name":
                        model_instance.Name = data_val.ToString();
                        break;                    
                }
            }
            switch (DbId)
            {
                case 1:
                    if (_dbQldvHD.Units.Any(u => u.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã đơn vị tính đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvHD.Units.Add(model_instance);
                            _dbQldvHD.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsUnit, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 2:
                    if (_dbQldvMIFI.Units.Any(u => u.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã đơn vị tính đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvMIFI.Units.Add(model_instance);
                            _dbQldvMIFI.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsUnit, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 3:
                    if (_dbQldvMB.Units.Any(u => u.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã đơn vị tính đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvMB.Units.Add(model_instance);
                            _dbQldvMB.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsUnit, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 0:
                    if (_dbQldvTester.Units.Any(u => u.Code == model_instance.Code))
                    {
                        result = "2000:warning:250:Mã đơn vị tính đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvTester.Units.Add(model_instance);
                            _dbQldvTester.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsUnit, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
            }
            return result;
        }

        public string UpdateUnit(string Code, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var unit_to_update = _dbQldvHD.Units.SingleOrDefault(u => u.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            unit_to_update.Name = data_val.ToString();
                            break;
                    }
                }
                try
                {
                    _dbQldvHD.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateUnit, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var unit_to_update = _dbQldvMIFI.Units.SingleOrDefault(u => u.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            unit_to_update.Name = data_val.ToString();
                            break;
                    }
                }
                try
                {
                    _dbQldvMIFI.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateUnit, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var unit_to_update = _dbQldvMB.Units.SingleOrDefault(u => u.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            unit_to_update.Name = data_val.ToString();
                            break;
                    }
                }
                try
                {
                    _dbQldvMB.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateUnit, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var unit_to_update = _dbQldvTester.Units.SingleOrDefault(u => u.Code == Code);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            unit_to_update.Name = data_val.ToString();
                            break;
                    }
                }
                try
                {
                    _dbQldvTester.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateUnit, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetNotifList(int RecStack, int RecQty, int DbId)
        {
            List<NotifDataModel> result = new List<NotifDataModel>();

            switch (DbId)
            {
                case 2:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var notif_array = _dbQldvMIFI.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                }).ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var notif_array = _dbQldvMIFI.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetNotifList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var notif_array = _dbQldvMB.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                }).ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var notif_array = _dbQldvMB.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetNotifList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var notif_array = _dbQldvTester.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                }).ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var notif_array = _dbQldvTester.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetNotifList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                default:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var notif_array = _dbQldvMIFI.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                }).ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var notif_array = _dbQldvMIFI.Notifications
                                .OrderBy(n => n.FromDate)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Title,
                                    s.Content,
                                    LnchDate = s.FromDate,
                                    DismDate = s.ToDate,
                                    Stt = s.Status
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in notif_array)
                            {
                                NotifDataModel model_instance = new NotifDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Title = item.Title;
                                model_instance.Content = item.Content;
                                model_instance.LnchDate = item.LnchDate.Value.ToString("yyyy-MM-dd");
                                model_instance.DismDate = item.DismDate.Value.ToString("yyyy-MM-dd");
                                model_instance.Stt = item.Stt.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetNotifList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsNotif(string UpdateData, int DbId)
        {
            string result = _resultError;
            Notification model_instance = new Notification();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Title":
                        model_instance.Title = data_val.ToString();
                        break;
                    case "Content":
                        model_instance.Content = data_val.ToString();
                        break;
                    case "LnchDate":
                        model_instance.FromDate =  DateTime.Parse(data_val.ToString());
                        break;
                    case "DismDate":
                        model_instance.ToDate = DateTime.Parse(data_val.ToString());
                        break;
                }
            }
            if (model_instance.Status == null) model_instance.Status = 0;
            switch (DbId)
            {
                case 2:
                    try
                    {
                        _dbQldvMIFI.Notifications.Add(model_instance);
                        _dbQldvMIFI.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsNotif, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 3:
                    try
                    {
                        _dbQldvMB.Notifications.Add(model_instance);
                        _dbQldvMB.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsNotif, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 0:
                    try
                    {
                        _dbQldvTester.Notifications.Add(model_instance);
                        _dbQldvTester.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsNotif, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                default:
                    try
                    {
                        _dbQldvMIFI.Notifications.Add(model_instance);
                        _dbQldvMIFI.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsNotif, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
            }            
            return result;
        }

        public string UpdateNotif(int Id, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 2)
            {
                var notif_to_update = _dbQldvMIFI.Notifications.SingleOrDefault(n => n.id == Id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Title":
                            notif_to_update.Title = data_val.ToString();
                            break;
                        case "Content":
                            notif_to_update.Content = data_val.ToString();
                            break;
                        case "LnchDate":
                            notif_to_update.FromDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "DismDate":
                            notif_to_update.ToDate = DateTime.Parse(data_val.ToString());
                            break;
                    }
                }
                if (notif_to_update.Status == null) notif_to_update.Status = 0;
                try
                {
                    _dbQldvMIFI.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateNotif, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var notif_to_update = _dbQldvMB.Notifications.SingleOrDefault(n => n.id == Id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Title":
                            notif_to_update.Title = data_val.ToString();
                            break;
                        case "Content":
                            notif_to_update.Content = data_val.ToString();
                            break;
                        case "LnchDate":
                            notif_to_update.FromDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "DismDate":
                            notif_to_update.ToDate = DateTime.Parse(data_val.ToString());
                            break;
                    }
                }
                if (notif_to_update.Status == null) notif_to_update.Status = 0;
                try
                {
                    _dbQldvMB.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateNotif, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var notif_to_update = _dbQldvTester.Notifications.SingleOrDefault(n => n.id == Id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Title":
                            notif_to_update.Title = data_val.ToString();
                            break;
                        case "Content":
                            notif_to_update.Content = data_val.ToString();
                            break;
                        case "LnchDate":
                            notif_to_update.FromDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "DismDate":
                            notif_to_update.ToDate = DateTime.Parse(data_val.ToString());
                            break;
                    }
                }
                if (notif_to_update.Status == null) notif_to_update.Status = 0;
                try
                {
                    _dbQldvTester.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateNotif, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetInvCatList(int RecStack, int RecQty, int DbId)
        {
            List<InvCatDataModel> result = new List<InvCatDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_cat_array = _dbQldvHD.InvCategories
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                }).ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_cat_array = _dbQldvHD.InvCategories
                                .OrderBy(ic => ic.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetAllInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_cat_array = _dbQldvMIFI.InvCategories
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                }).ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_cat_array = _dbQldvMIFI.InvCategories
                                .OrderBy(ic => ic.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetAllInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_cat_array = _dbQldvMB.InvCategories
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                }).ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_cat_array = _dbQldvMB.InvCategories
                                .OrderBy(ic => ic.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetAllInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        if (RecQty == 8)
                        {
                            var inv_cat_array = _dbQldvTester.InvCategories
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                }).ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                        else
                        {
                            var inv_cat_array = _dbQldvTester.InvCategories
                                .OrderBy(ic => ic.id)
                                .Select(s => new
                                {
                                    Id = s.id,
                                    s.Name,
                                    PatPrefix = s.InvPattern,
                                    Desc = s.Description,
                                    UsageStt = s.IsPub
                                })
                                .Skip(RecStack)
                                .Take(RecQty)
                                .ToList();

                            foreach (var item in inv_cat_array)
                            {
                                InvCatDataModel model_instance = new InvCatDataModel();

                                model_instance.Id = item.Id;
                                model_instance.Name = item.Name;
                                model_instance.PatPrefix = item.PatPrefix;
                                model_instance.Desc = item.Desc;
                                model_instance.UsageStt = item.UsageStt == false ? "false" : "true";
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/GetAllInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsInvCat(string UpdateData, int DbId)
        {
            string result = _resultError;
            InvCategory model_instance = new InvCategory();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Name":
                        model_instance.Name = data_val.ToString();
                        break;
                    case "PatPrefix":
                        model_instance.InvPattern = data_val.ToString();
                        break;
                    case "Desc":
                        model_instance.Description = data_val.ToString();
                        break;
                    case "UsageStt":
                        model_instance.IsPub = bool.Parse(data_val.ToString());
                        break;
                }
            }
            switch (DbId)
            {
                case 1:
                    try
                    {
                        _dbQldvHD.InvCategories.Add(model_instance);
                        _dbQldvHD.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 2:
                    try
                    {
                        _dbQldvMIFI.InvCategories.Add(model_instance);
                        _dbQldvMIFI.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 3:
                    try
                    {
                        _dbQldvMB.InvCategories.Add(model_instance);
                        _dbQldvMB.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 0:
                    try
                    {
                        _dbQldvTester.InvCategories.Add(model_instance);
                        _dbQldvTester.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Feature/InsInvCat, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
            }
            return result;
        }

        public string UpdateInvCat(int Id, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var inv_cat_to_update = _dbQldvHD.InvCategories.SingleOrDefault(ic => ic.id == Id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            inv_cat_to_update.Name = data_val.ToString();
                            break;
                        case "PatPrefix":
                            inv_cat_to_update.InvPattern = data_val.ToString();
                            break;
                        case "Desc":
                            inv_cat_to_update.Description = data_val.ToString();
                            break;
                        case "UsageStt":
                            inv_cat_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbQldvHD.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvCat, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var inv_cat_to_update = _dbQldvMIFI.InvCategories.SingleOrDefault(ic => ic.id == Id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            inv_cat_to_update.Name = data_val.ToString();
                            break;
                        case "PatPrefix":
                            inv_cat_to_update.InvPattern = data_val.ToString();
                            break;
                        case "Desc":
                            inv_cat_to_update.Description = data_val.ToString();
                            break;
                        case "UsageStt":
                            inv_cat_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbQldvMIFI.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvCat, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var inv_cat_to_update = _dbQldvMB.InvCategories.SingleOrDefault(ic => ic.id == Id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            inv_cat_to_update.Name = data_val.ToString();
                            break;
                        case "PatPrefix":
                            inv_cat_to_update.InvPattern = data_val.ToString();
                            break;
                        case "Desc":
                            inv_cat_to_update.Description = data_val.ToString();
                            break;
                        case "UsageStt":
                            inv_cat_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbQldvMB.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvCat, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var inv_cat_to_update = _dbQldvTester.InvCategories.SingleOrDefault(ic => ic.id == Id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Name":
                            inv_cat_to_update.Name = data_val.ToString();
                            break;
                        case "PatPrefix":
                            inv_cat_to_update.InvPattern = data_val.ToString();
                            break;
                        case "Desc":
                            inv_cat_to_update.Description = data_val.ToString();
                            break;
                        case "UsageStt":
                            inv_cat_to_update.IsPub = bool.Parse(data_val.ToString());
                            break;
                    }
                }
                try
                {
                    _dbQldvTester.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Feature/UpdateInvCat, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }
    }
}