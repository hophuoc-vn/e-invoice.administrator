using ManageHDDT.Helpers;
using ManageHDDT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NReco.PdfGenerator;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using XmlRpc;

namespace ManageHDDT.Controllers
{
    public class OperatingController : Controller
    {
        #region Khai báo hằng số
        hddt_dhql_Entities _dbDhql = new hddt_dhql_Entities();
        hddt_data_HD_Entities _dbDataHD = new hddt_data_HD_Entities();
        hddt_qldv_HD_Entities _dbQldvHD = new hddt_qldv_HD_Entities();
        hddt_data_MIFI_Entities _dbDataMIFI = new hddt_data_MIFI_Entities();
        hddt_qldv_MIFI_Entities _dbQldvMIFI = new hddt_qldv_MIFI_Entities();
        hddt_data_tester_Entities _dbDataTester = new hddt_data_tester_Entities();
        hddt_qldv_tester_Entities _dbQldvTester = new hddt_qldv_tester_Entities();
        hddt_data_MB_Entities _dbDataMB = new hddt_data_MB_Entities();
        hddt_qldv_MB_Entities _dbQldvMB = new hddt_qldv_MB_Entities();
        readonly string _odooServiceURL = ConfigurationManager.AppSettings["OdooServiceURL"];
        readonly string _odooDb = ConfigurationManager.AppSettings["OdooDatabase"];
        readonly string _odooUser = ConfigurationManager.AppSettings["OdooUsername"];
        readonly string _odooPwd = ConfigurationManager.AppSettings["OdooPassword"];
        readonly string _odooTo = ConfigurationManager.AppSettings["OdooTimeout"];
        string _initPwd = ConfigurationManager.AppSettings["InitPwd"];
        string _failReqMsg = "2000:warning:350:" + ConfigurationManager.AppSettings["FailReqMsg"];        
        string _resultError = "2000:error:300:" + ConfigurationManager.AppSettings["ServerErrorMsg"];
        string _resultWarning = "1500:warning:250:";
        string _resultSuccess = "500:success:250:Cập nhật thành công.";
        private Cache _objCache = new Cache();
        XmlRpcResponse _loginResponse = null;
        #endregion

        // GET: Operating
        [SessionCheck]
        public ActionResult CoInvMgmt()
        {
            bool req_auth_resp = new GeneralController().AuthReq("CoInvMgmt");
            if (req_auth_resp)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SupTkts", "Home");
            }
        }

        [SessionCheck]
        public ActionResult UserCtrl()
        {
            bool req_auth_resp = new GeneralController().AuthReq("UserCtrl");

            if (req_auth_resp)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SupTkts", "Home");
            }
        }

        [HttpGet]
        public string GetCoInvMgmtSiteUrl(int CoId, int DbId)
        {            
            string result = _resultError;
            bool req_auth_resp = new GeneralController().AuthReq("GetCoInvMgmtSiteUrl");

            if (req_auth_resp)
            {
                switch (DbId)
                {
                    case 1:
                        try
                        {
                            var co_array = _dbQldvHD.Companies
                                .Where(c => c.id == CoId)
                                .Select(s => new
                                {
                                    CoDomain = s.Domain
                                }).ToList();
                            var co_adm_token_array = _dbDataHD.userdatas
                                .Where(u => u.GroupName == CoId.ToString() && u.username == "admin")
                                .Select(s => new
                                {
                                    AdmTk = s.token
                                }).ToList();

                            var co_domain_array = co_array[0].CoDomain.Split(')');
                            string co_domain = co_domain_array[0].Substring(1);
                            string co_adm_tk = co_adm_token_array[0].AdmTk.Replace(" ", "+");

                            result = "http://" + co_domain + "/logintoken.aspx?Token=" + co_adm_tk;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvMgmtSiteUrl, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                    case 2:
                        try
                        {
                            var co_array = _dbQldvMIFI.Companies
                                .Where(c => c.id == CoId)
                                .Select(s => new
                                {
                                    CoDomain = s.Domain
                                }).ToList();
                            var co_adm_token_array = _dbDataMIFI.userdatas
                                .Where(u => u.GroupName == CoId.ToString() && u.username == "admin")
                                .Select(s => new
                                {
                                    AdmTk = s.token
                                }).ToList();

                            var co_domain_array = co_array[0].CoDomain.Split(')');
                            string co_domain = co_domain_array[0].Substring(1);
                            string co_adm_tk = co_adm_token_array[0].AdmTk.Replace(" ", "+");

                            result = "http://" + co_domain + "/logintoken.aspx?Token=" + co_adm_tk;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvMgmtSiteUrl, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                    case 3:
                        try
                        {
                            var co_array = _dbQldvMB.Companies
                                .Where(c => c.id == CoId)
                                .Select(s => new
                                {
                                    CoDomain = s.Domain
                                }).ToList();
                            var co_adm_token_array = _dbDataMIFI.userdatas
                                .Where(u => u.GroupName == CoId.ToString() && u.username == "admin")
                                .Select(s => new
                                {
                                    AdmTk = s.token
                                }).ToList();

                            var co_domain_array = co_array[0].CoDomain.Split(')');
                            string co_domain = co_domain_array[0].Substring(1);
                            string co_adm_tk = co_adm_token_array[0].AdmTk.Replace(" ", "+");

                            result = "http://" + co_domain + "/logintoken.aspx?Token=" + co_adm_tk;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvMgmtSiteUrl, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                    case 0:
                        try
                        {
                            var co_array = _dbQldvTester.Companies
                                .Where(c => c.id == CoId)
                                .Select(s => new
                                {
                                    CoDomain = s.Domain
                                }).ToList();
                            var co_adm_token_array = _dbDataTester.userdatas
                                .Where(u => u.GroupName == CoId.ToString() && u.username == "admin")
                                .Select(s => new
                                {
                                    AdmTk = s.token
                                }).ToList();

                            var co_domain_array = co_array[0].CoDomain.Split(')');
                            string co_domain = co_domain_array[0].Substring(1);
                            string co_adm_tk = co_adm_token_array[0].AdmTk.Replace(" ", "+");

                            result = "http://" + co_domain + "/logintoken.aspx?Token=" + co_adm_tk;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvMgmtSiteUrl, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                }
            }
            else
            {
                result = _failReqMsg;
            }            
            return result;
        }

        public JsonResult GetCoListByCusRef(string CusRef, int DbId)
        {
            JsonResult result = new JsonResult();
            List<OdooCusSvcDataModel> cus_svc = GetOdooCusSvc(CusRef);
            List<CoIdDataModel> raw_data_array = new List<CoIdDataModel>();

            if (cus_svc.Count == 0)
            {
                FailReqModel model_instance = new FailReqModel();

                model_instance.Msg = "2000:warning:350:Không tìm thấy khách hàng với mã này.";
                result = Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            else
            {
                switch (DbId)
                {
                    case 1:
                        try
                        {                            
                            foreach (var item in cus_svc)
                            {
                                var co_by_domain = _dbQldvHD.Companies.SingleOrDefault(c => c.Domain.Contains(item.domain_name));

                                if (co_by_domain != null)
                                {
                                    CoIdDataModel model_instance = new CoIdDataModel();

                                    model_instance.CoId = co_by_domain.id;
                                    model_instance.CoName = co_by_domain.Name;
                                    model_instance.CoDomain = co_by_domain.Domain.Split(')')[0].Substring(1);
                                    raw_data_array.Add(model_instance);
                                }
                            }

                            var data_array = raw_data_array.Distinct().ToList();

                            result = Json(data_array, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception exc)
                        {
                            FailReqModel model_instance = new FailReqModel();
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            model_instance.Msg = _resultError;
                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoListByCusRef, dòng {0}: {1}", exc_line, exc.Message));
                            return Json(model_instance, JsonRequestBehavior.AllowGet);
                        }
                        break;
                    case 2:
                        try
                        {
                            foreach (var item in cus_svc)
                            {
                                var co_by_domain = _dbQldvMIFI.Companies.SingleOrDefault(c => c.Domain.Contains(item.domain_name));

                                if (co_by_domain != null)
                                {
                                    CoIdDataModel model_instance = new CoIdDataModel();

                                    model_instance.CoId = co_by_domain.id;
                                    model_instance.CoName = co_by_domain.Name;
                                    model_instance.CoDomain = co_by_domain.Domain.Split(')')[0].Substring(1);
                                    raw_data_array.Add(model_instance);
                                }
                            }

                            var data_array = raw_data_array.Distinct().ToList();

                            result = Json(data_array, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception exc)
                        {
                            FailReqModel model_instance = new FailReqModel();
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            model_instance.Msg = _resultError;
                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoListByCusRef, dòng {0}: {1}", exc_line, exc.Message));
                            return Json(model_instance, JsonRequestBehavior.AllowGet);
                        }
                        break;
                    case 3:
                        try
                        {
                            foreach (var item in cus_svc)
                            {
                                var co_by_domain = _dbQldvMB.Companies.SingleOrDefault(c => c.Domain.Contains(item.domain_name));

                                if (co_by_domain != null)
                                {
                                    CoIdDataModel model_instance = new CoIdDataModel();

                                    model_instance.CoId = co_by_domain.id;
                                    model_instance.CoName = co_by_domain.Name;
                                    model_instance.CoDomain = co_by_domain.Domain.Split(')')[0].Substring(1);
                                    raw_data_array.Add(model_instance);
                                }
                            }

                            var data_array = raw_data_array.Distinct().ToList();

                            result = Json(data_array, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception exc)
                        {
                            FailReqModel model_instance = new FailReqModel();
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            model_instance.Msg = _resultError;
                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoListByCusRef, dòng {0}: {1}", exc_line, exc.Message));
                            return Json(model_instance, JsonRequestBehavior.AllowGet);
                        }
                        break;
                    case 0:
                        try
                        {
                            foreach (var item in cus_svc)
                            {
                                var co_by_domain = _dbQldvTester.Companies.SingleOrDefault(c => c.Domain.Contains(item.domain_name));

                                if (co_by_domain != null)
                                {
                                    CoIdDataModel model_instance = new CoIdDataModel();

                                    model_instance.CoId = co_by_domain.id;
                                    model_instance.CoName = co_by_domain.Name;
                                    model_instance.CoDomain = co_by_domain.Domain.Split(')')[0].Substring(1);
                                    raw_data_array.Add(model_instance);
                                }
                            }

                            var data_array = raw_data_array.Distinct().ToList();

                            result = Json(data_array, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception exc)
                        {
                            FailReqModel model_instance = new FailReqModel();
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            model_instance.Msg = _resultError;
                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoListByCusRef, dòng {0}: {1}", exc_line, exc.Message));
                            return Json(model_instance, JsonRequestBehavior.AllowGet);
                        }
                        break;
                }
            }
            return result;
        }

        public JsonResult GetCoInfo(int CoId, int DbId)
        {
            CoInfoDataModel result = new CoInfoDataModel();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var co_info = _dbQldvHD.Companies
                            .Where(c => c.id == CoId)
                            .Select(s => new
                            {
                                CoName = s.Name,
                                CoAddr = s.Address,
                                CoBankAcctName = s.BankAccountName,
                                CoBankName = s.BankName,
                                CoBankNo = s.BankNumber,
                                CoCEO = s.ContactPerson,
                                CoEmailAddr = s.Email,
                                CoFax = s.Fax,
                                CoPhone = s.Phone,
                                CoTaxCode = s.TaxCode,
                                CoDomain = s.Domain
                            }).ToList();

                        result.CoName = co_info[0].CoName;
                        result.CoAddr = co_info[0].CoAddr;
                        result.CoBankAcctName = co_info[0].CoBankAcctName;
                        result.CoBankName = co_info[0].CoBankName;
                        result.CoBankNo = co_info[0].CoBankNo;
                        result.CoCEO = co_info[0].CoCEO;
                        result.CoEmailAddr = co_info[0].CoEmailAddr;
                        result.CoFax = co_info[0].CoFax;
                        result.CoPhone = co_info[0].CoPhone;
                        result.CoTaxCode = co_info[0].CoTaxCode;

                        List<string> co_domain = new List<string>();
                        var co_domain_array = co_info[0].CoDomain.Split(')');

                        for (int i = 0; i < co_domain_array.Length; i++)
                        {
                            if (co_domain_array[i] != "" && co_domain_array[i].IndexOf(":") < 0)
                            {
                                co_domain.Add(co_domain_array[i].Substring(1));
                            }
                        }
                        result.CoDomain = co_domain.ToArray();

                        string co_name = co_info[0].CoName;

                        if (_dbQldvMIFI.Companies.Any(c => c.Name == co_name))
                        {
                            result.XferStt = "(Đã chuyển qua hđđt Mifi)";
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var co_info = _dbQldvMIFI.Companies
                            .Where(c => c.id == CoId)
                            .Select(s => new
                            {
                                CoName = s.Name,
                                CoAddr = s.Address,
                                CoBankAcctName = s.BankAccountName,
                                CoBankName = s.BankName,
                                CoBankNo = s.BankNumber,
                                CoCEO = s.ContactPerson,
                                CoEmailAddr = s.Email,
                                CoFax = s.Fax,
                                CoPhone = s.Phone,
                                CoTaxCode = s.TaxCode,
                                CoDomain = s.Domain
                            }).ToList();

                        result.CoName = co_info[0].CoName;
                        result.CoAddr = co_info[0].CoAddr;
                        result.CoBankAcctName = co_info[0].CoBankAcctName;
                        result.CoBankName = co_info[0].CoBankName;
                        result.CoBankNo = co_info[0].CoBankNo;
                        result.CoCEO = co_info[0].CoCEO;
                        result.CoEmailAddr = co_info[0].CoEmailAddr;
                        result.CoFax = co_info[0].CoFax;
                        result.CoPhone = co_info[0].CoPhone;
                        result.CoTaxCode = co_info[0].CoTaxCode;

                        List<string> co_domain = new List<string>();
                        var co_domain_array = co_info[0].CoDomain.Split(')');

                        for (int i = 0; i < co_domain_array.Length; i++)
                        {
                            if (co_domain_array[i] != "" && co_domain_array[i].IndexOf(":") < 0)
                            {
                                co_domain.Add(co_domain_array[i].Substring(1));
                            }
                        }
                        result.CoDomain = co_domain.ToArray();
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        var co_info = _dbQldvMB.Companies
                            .Where(c => c.id == CoId)
                            .Select(s => new
                            {
                                CoName = s.Name,
                                CoAddr = s.Address,
                                CoBankAcctName = s.BankAccountName,
                                CoBankName = s.BankName,
                                CoBankNo = s.BankNumber,
                                CoCEO = s.ContactPerson,
                                CoEmailAddr = s.Email,
                                CoFax = s.Fax,
                                CoPhone = s.Phone,
                                CoTaxCode = s.TaxCode,
                                CoDomain = s.Domain
                            }).ToList();

                        result.CoName = co_info[0].CoName;
                        result.CoAddr = co_info[0].CoAddr;
                        result.CoBankAcctName = co_info[0].CoBankAcctName;
                        result.CoBankName = co_info[0].CoBankName;
                        result.CoBankNo = co_info[0].CoBankNo;
                        result.CoCEO = co_info[0].CoCEO;
                        result.CoEmailAddr = co_info[0].CoEmailAddr;
                        result.CoFax = co_info[0].CoFax;
                        result.CoPhone = co_info[0].CoPhone;
                        result.CoTaxCode = co_info[0].CoTaxCode;

                        List<string> co_domain = new List<string>();
                        var co_domain_array = co_info[0].CoDomain.Split(')');

                        for (int i = 0; i < co_domain_array.Length; i++)
                        {
                            if (co_domain_array[i] != "" && co_domain_array[i].IndexOf(":") < 0)
                            {
                                co_domain.Add(co_domain_array[i].Substring(1));
                            }
                        }
                        result.CoDomain = co_domain.ToArray();
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        var co_info = _dbQldvTester.Companies
                            .Where(c => c.id == CoId)
                            .Select(s => new
                            {
                                CoName = s.Name,
                                CoAddr = s.Address,
                                CoBankAcctName = s.BankAccountName,
                                CoBankName = s.BankName,
                                CoBankNo = s.BankNumber,
                                CoCEO = s.ContactPerson,
                                CoEmailAddr = s.Email,
                                CoFax = s.Fax,
                                CoPhone = s.Phone,
                                CoTaxCode = s.TaxCode,
                                CoDomain = s.Domain
                            }).ToList();

                        result.CoName = co_info[0].CoName;
                        result.CoAddr = co_info[0].CoAddr;
                        result.CoBankAcctName = co_info[0].CoBankAcctName;
                        result.CoBankName = co_info[0].CoBankName;
                        result.CoBankNo = co_info[0].CoBankNo;
                        result.CoCEO = co_info[0].CoCEO;
                        result.CoEmailAddr = co_info[0].CoEmailAddr;
                        result.CoFax = co_info[0].CoFax;
                        result.CoPhone = co_info[0].CoPhone;
                        result.CoTaxCode = co_info[0].CoTaxCode;

                        List<string> co_domain = new List<string>();
                        var co_domain_array = co_info[0].CoDomain.Split(')');

                        for (int i = 0; i < co_domain_array.Length; i++)
                        {
                            if (co_domain_array[i] != "" && co_domain_array[i].IndexOf(":") < 0)
                            {
                                co_domain.Add(co_domain_array[i].Substring(1));
                            }
                        }
                        result.CoDomain = co_domain.ToArray();
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string UpdateCoInfo(int CoId, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var co_to_update = _dbQldvHD.Companies.SingleOrDefault(c => c.id == CoId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoAddr":
                            co_to_update.Address = data_val.ToString();
                            break;
                        case "CoBankAcctName":
                            co_to_update.BankAccountName = data_val.ToString();
                            break;
                        case "CoBankName":
                            co_to_update.BankName = data_val.ToString();
                            break;
                        case "CoBankNo":
                            co_to_update.BankNumber = data_val.ToString();
                            break;
                        case "CoCEO":
                            co_to_update.ContactPerson = data_val.ToString();
                            break;
                        case "CoEmailAddr":
                            co_to_update.Email = data_val.ToString();
                            break;
                        case "CoFax":
                            co_to_update.Fax = data_val.ToString();
                            break;
                        case "CoPhone":
                            co_to_update.Phone = data_val.ToString();
                            break;
                        case "CoTaxCode":
                            co_to_update.TaxCode = data_val.ToString();
                            break;
                        case "CoDomain":
                            var co_domain_array = data_val.ToObject<string[]>();
                            var co_domain = string.Empty;

                            for (int i = 0; i < co_domain_array.Length; i++)
                            {
                                co_domain += "(" + co_domain_array[i] + ")(" + co_domain_array[i] + ":443)";
                            }
                            co_to_update.Domain = co_domain;
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var co_to_update = _dbQldvMIFI.Companies.SingleOrDefault(c => c.id == CoId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoAddr":
                            co_to_update.Address = data_val.ToString();
                            break;
                        case "CoBankAcctName":
                            co_to_update.BankAccountName = data_val.ToString();
                            break;
                        case "CoBankName":
                            co_to_update.BankName = data_val.ToString();
                            break;
                        case "CoBankNo":
                            co_to_update.BankNumber = data_val.ToString();
                            break;
                        case "CoCEO":
                            co_to_update.ContactPerson = data_val.ToString();
                            break;
                        case "CoEmailAddr":
                            co_to_update.Email = data_val.ToString();
                            break;
                        case "CoFax":
                            co_to_update.Fax = data_val.ToString();
                            break;
                        case "CoPhone":
                            co_to_update.Phone = data_val.ToString();
                            break;
                        case "CoTaxCode":
                            co_to_update.TaxCode = data_val.ToString();
                            break;
                        case "CoDomain":
                            var co_domain_array = data_val.ToObject<string[]>();
                            var co_domain = string.Empty;

                            for (int i = 0; i < co_domain_array.Length; i++)
                            {
                                co_domain += "(" + co_domain_array[i] + ")(" + co_domain_array[i] + ":443)";
                            }
                            co_to_update.Domain = co_domain;
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var co_to_update = _dbQldvMB.Companies.SingleOrDefault(c => c.id == CoId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoAddr":
                            co_to_update.Address = data_val.ToString();
                            break;
                        case "CoBankAcctName":
                            co_to_update.BankAccountName = data_val.ToString();
                            break;
                        case "CoBankName":
                            co_to_update.BankName = data_val.ToString();
                            break;
                        case "CoBankNo":
                            co_to_update.BankNumber = data_val.ToString();
                            break;
                        case "CoCEO":
                            co_to_update.ContactPerson = data_val.ToString();
                            break;
                        case "CoEmailAddr":
                            co_to_update.Email = data_val.ToString();
                            break;
                        case "CoFax":
                            co_to_update.Fax = data_val.ToString();
                            break;
                        case "CoPhone":
                            co_to_update.Phone = data_val.ToString();
                            break;
                        case "CoTaxCode":
                            co_to_update.TaxCode = data_val.ToString();
                            break;
                        case "CoDomain":
                            var co_domain_array = data_val.ToObject<string[]>();
                            var co_domain = string.Empty;

                            for (int i = 0; i < co_domain_array.Length; i++)
                            {
                                co_domain += "(" + co_domain_array[i] + ")(" + co_domain_array[i] + ":443)";
                            }
                            co_to_update.Domain = co_domain;
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var co_to_update = _dbQldvTester.Companies.SingleOrDefault(c => c.id == CoId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "CoAddr":
                            co_to_update.Address = data_val.ToString();
                            break;
                        case "CoBankAcctName":
                            co_to_update.BankAccountName = data_val.ToString();
                            break;
                        case "CoBankName":
                            co_to_update.BankName = data_val.ToString();
                            break;
                        case "CoBankNo":
                            co_to_update.BankNumber = data_val.ToString();
                            break;
                        case "CoCEO":
                            co_to_update.ContactPerson = data_val.ToString();
                            break;
                        case "CoEmailAddr":
                            co_to_update.Email = data_val.ToString();
                            break;
                        case "CoFax":
                            co_to_update.Fax = data_val.ToString();
                            break;
                        case "CoPhone":
                            co_to_update.Phone = data_val.ToString();
                            break;
                        case "CoTaxCode":
                            co_to_update.TaxCode = data_val.ToString();
                            break;
                        case "CoDomain":
                            var co_domain_array = data_val.ToObject<string[]>();
                            var co_domain = string.Empty;

                            for (int i = 0; i < co_domain_array.Length; i++)
                            {
                                co_domain += "(" + co_domain_array[i] + ")(" + co_domain_array[i] + ":443)";
                            }
                            co_to_update.Domain = co_domain;
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInfo, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetCoPub(int CoId, int DbId)
        {
            List<CoPubDataModel> result = new List<CoPubDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var co_pub_st_array = _dbDataHD.Publishes
                            .Where(p => p.ComID == CoId)
                            .Select(s => new
                            {
                                CoName = s.ComName,
                                PubStId = s.id,
                                PubStCode = s.Code,
                                PubStPubDate = s.PublishDate,
                                TaxAuthCode = s.TaxAuthorityCode,
                                TaxAuthName = s.TaxAuthorityName,
                                TaxAuthLocality = s.City,
                                PubStStt = s.Status
                            }).ToList();
                        var co_pub_pkg_array = _dbDataHD.PublishInvoices
                            .Where(pi => pi.ComId == CoId)
                            .Select(s => new
                            {
                                PubId = s.PublishID,
                                PubPkgBeginNo = s.FromNo,
                                PubPkgEndNo = s.ToNo,
                                PubPkgStt = s.Status,
                                PubPkgInvSerial = s.InvSerial,
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgTmplId = s.RegisterID,
                                PubPkgPurDate = s.StartDate,
                                PubPkgExpDate = s.EndDate
                            }).ToList();
                        var pub_array = co_pub_pkg_array
                            .AsEnumerable()
                            .Join(co_pub_st_array, co_pub_pkg => co_pub_pkg.PubId, pub_st => pub_st.PubStId, (co_pub_pkg, pub_st) => new
                            {
                                pub_st.CoName,
                                pub_st.PubStCode,
                                pub_st.PubStPubDate,
                                pub_st.PubStStt,
                                pub_st.TaxAuthCode,
                                pub_st.TaxAuthName,
                                pub_st.TaxAuthLocality,
                                co_pub_pkg.PubPkgInvPattern,
                                co_pub_pkg.PubPkgInvSerial,
                                co_pub_pkg.PubPkgBeginNo,
                                co_pub_pkg.PubPkgEndNo,
                                co_pub_pkg.PubPkgPurDate,
                                co_pub_pkg.PubPkgExpDate,
                                co_pub_pkg.PubPkgTmplId,
                                co_pub_pkg.PubPkgStt
                            }).ToList();

                        foreach (var item in pub_array)
                        {
                            CoPubDataModel model_instance = new CoPubDataModel();

                            if (_dbDataHD.Decisions.Any(d => d.ListInvPattern == item.PubPkgInvPattern))
                            {
                                var exist_dec = _dbDataHD.Decisions.FirstOrDefault(d => d.ListInvPattern == item.PubPkgInvPattern);

                                model_instance.DecNo = exist_dec.DecisionNo;
                            }
                            else
                            {
                                model_instance.DecNo = string.Empty;
                            }
                            model_instance.PubStCode = item.PubStCode;
                            model_instance.PubStPubDate = item.PubStPubDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubStStt = item.PubStStt.Value;
                            model_instance.TaxAuthCode = item.TaxAuthCode;
                            model_instance.TaxAuthName = item.TaxAuthName;
                            model_instance.TaxAuthLocality = item.TaxAuthLocality;
                            model_instance.PubInvPattern = item.PubPkgInvPattern;
                            model_instance.PubInvSerial = item.PubPkgInvSerial;
                            model_instance.PubTmplInd = item.PubPkgInvPattern + " – " + item.PubPkgInvSerial;
                            model_instance.PubTmplId = item.PubPkgTmplId.Value;
                            model_instance.PubPkgPurDate = item.PubPkgPurDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgExpDate = item.PubPkgExpDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgBeginNo = int.Parse(item.PubPkgBeginNo.ToString());
                            model_instance.PubPkgEndNo = int.Parse(item.PubPkgEndNo.ToString());
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoPub, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var co_pub_st_array = _dbDataMIFI.Publishes
                            .Where(p => p.ComID == CoId)
                            .Select(s => new
                            {
                                CoName = s.ComName,
                                PubStId = s.id,
                                PubStCode = s.Code,
                                PubStPubDate = s.PublishDate,
                                TaxAuthCode = s.TaxAuthorityCode,
                                TaxAuthName = s.TaxAuthorityName,
                                TaxAuthLocality = s.City,
                                PubStStt = s.Status
                            }).ToList();
                        var co_pub_pkg_array = _dbDataMIFI.PublishInvoices
                            .Where(pi => pi.ComId == CoId)
                            .Select(s => new
                            {
                                PubId = s.PublishID,
                                PubPkgBeginNo = s.FromNo,
                                PubPkgEndNo = s.ToNo,
                                PubPkgStt = s.Status,
                                PubPkgInvSerial = s.InvSerial,
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgTmplId = s.RegisterID,
                                PubPkgPurDate = s.StartDate,
                                PubPkgExpDate = s.EndDate
                            }).ToList();
                        var pub_array = co_pub_pkg_array
                            .AsEnumerable()
                            .Join(co_pub_st_array, co_pub_pkg => co_pub_pkg.PubId, pub_st => pub_st.PubStId, (co_pub_pkg, pub_st) => new
                            {
                                pub_st.CoName,
                                pub_st.PubStCode,
                                pub_st.PubStPubDate,
                                pub_st.PubStStt,
                                pub_st.TaxAuthCode,
                                pub_st.TaxAuthName,
                                pub_st.TaxAuthLocality,
                                co_pub_pkg.PubPkgInvPattern,
                                co_pub_pkg.PubPkgInvSerial,
                                co_pub_pkg.PubPkgBeginNo,
                                co_pub_pkg.PubPkgEndNo,
                                co_pub_pkg.PubPkgPurDate,
                                co_pub_pkg.PubPkgExpDate,
                                co_pub_pkg.PubPkgTmplId,
                                co_pub_pkg.PubPkgStt
                            }).ToList();

                        foreach (var item in pub_array)
                        {
                            CoPubDataModel model_instance = new CoPubDataModel();

                            if (_dbDataMIFI.Decisions.Any(d => d.ListInvPattern == item.PubPkgInvPattern))
                            {
                                var exist_dec = _dbDataMIFI.Decisions.FirstOrDefault(d => d.ListInvPattern == item.PubPkgInvPattern);

                                model_instance.DecNo = exist_dec.DecisionNo;
                            }
                            else
                            {
                                model_instance.DecNo = string.Empty;
                            }
                            model_instance.PubStCode = item.PubStCode;
                            model_instance.PubStPubDate = item.PubStPubDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubStStt = item.PubStStt.Value;
                            model_instance.TaxAuthCode = item.TaxAuthCode;
                            model_instance.TaxAuthName = item.TaxAuthName;
                            model_instance.TaxAuthLocality = item.TaxAuthLocality;
                            model_instance.PubInvPattern = item.PubPkgInvPattern;
                            model_instance.PubInvSerial = item.PubPkgInvSerial;
                            model_instance.PubTmplInd = item.PubPkgInvPattern + " – " + item.PubPkgInvSerial;
                            model_instance.PubTmplId = item.PubPkgTmplId.Value;
                            model_instance.PubPkgPurDate = item.PubPkgPurDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgExpDate = item.PubPkgExpDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgBeginNo = int.Parse(item.PubPkgBeginNo.ToString());
                            model_instance.PubPkgEndNo = int.Parse(item.PubPkgEndNo.ToString());
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoPub, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        var co_pub_st_array = _dbDataMB.Publishes
                            .Where(p => p.ComID == CoId)
                            .Select(s => new
                            {
                                CoName = s.ComName,
                                PubStId = s.id,
                                PubStCode = s.Code,
                                PubStPubDate = s.PublishDate,
                                TaxAuthCode = s.TaxAuthorityCode,
                                TaxAuthName = s.TaxAuthorityName,
                                TaxAuthLocality = s.City,
                                PubStStt = s.Status
                            }).ToList();
                        var co_pub_pkg_array = _dbDataMB.PublishInvoices
                            .Where(pi => pi.ComId == CoId)
                            .Select(s => new
                            {
                                PubId = s.PublishID,
                                PubPkgBeginNo = s.FromNo,
                                PubPkgEndNo = s.ToNo,
                                PubPkgStt = s.Status,
                                PubPkgInvSerial = s.InvSerial,
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgTmplId = s.RegisterID,
                                PubPkgPurDate = s.StartDate,
                                PubPkgExpDate = s.EndDate
                            }).ToList();
                        var pub_array = co_pub_pkg_array
                            .AsEnumerable()
                            .Join(co_pub_st_array, co_pub_pkg => co_pub_pkg.PubId, pub_st => pub_st.PubStId, (co_pub_pkg, pub_st) => new
                            {
                                pub_st.CoName,
                                pub_st.PubStCode,
                                pub_st.PubStPubDate,
                                pub_st.PubStStt,
                                pub_st.TaxAuthCode,
                                pub_st.TaxAuthName,
                                pub_st.TaxAuthLocality,
                                co_pub_pkg.PubPkgInvPattern,
                                co_pub_pkg.PubPkgInvSerial,
                                co_pub_pkg.PubPkgBeginNo,
                                co_pub_pkg.PubPkgEndNo,
                                co_pub_pkg.PubPkgPurDate,
                                co_pub_pkg.PubPkgExpDate,
                                co_pub_pkg.PubPkgTmplId,
                                co_pub_pkg.PubPkgStt
                            }).ToList();

                        foreach (var item in pub_array)
                        {
                            CoPubDataModel model_instance = new CoPubDataModel();

                            if (_dbDataMIFI.Decisions.Any(d => d.ListInvPattern == item.PubPkgInvPattern))
                            {
                                var exist_dec = _dbDataMIFI.Decisions.FirstOrDefault(d => d.ListInvPattern == item.PubPkgInvPattern);

                                model_instance.DecNo = exist_dec.DecisionNo;
                            }
                            else
                            {
                                model_instance.DecNo = string.Empty;
                            }
                            model_instance.PubStCode = item.PubStCode;
                            model_instance.PubStPubDate = item.PubStPubDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubStStt = item.PubStStt.Value;
                            model_instance.TaxAuthCode = item.TaxAuthCode;
                            model_instance.TaxAuthName = item.TaxAuthName;
                            model_instance.TaxAuthLocality = item.TaxAuthLocality;
                            model_instance.PubInvPattern = item.PubPkgInvPattern;
                            model_instance.PubInvSerial = item.PubPkgInvSerial;
                            model_instance.PubTmplInd = item.PubPkgInvPattern + " – " + item.PubPkgInvSerial;
                            model_instance.PubTmplId = item.PubPkgTmplId.Value;
                            model_instance.PubPkgPurDate = item.PubPkgPurDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgExpDate = item.PubPkgExpDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgBeginNo = int.Parse(item.PubPkgBeginNo.ToString());
                            model_instance.PubPkgEndNo = int.Parse(item.PubPkgEndNo.ToString());
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoPub, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        var co_pub_st_array = _dbDataTester.Publishes
                            .Where(p => p.ComID == CoId)
                            .Select(s => new
                            {
                                CoName = s.ComName,
                                PubStId = s.id,
                                PubStCode = s.Code,
                                PubStPubDate = s.PublishDate,
                                TaxAuthCode = s.TaxAuthorityCode,
                                TaxAuthName = s.TaxAuthorityName,
                                TaxAuthLocality = s.City,
                                PubStStt = s.Status
                            }).ToList();
                        var co_pub_pkg_array = _dbDataTester.PublishInvoices
                            .Where(pi => pi.ComId == CoId)
                            .Select(s => new
                            {
                                PubId = s.PublishID,
                                PubPkgBeginNo = s.FromNo,
                                PubPkgEndNo = s.ToNo,
                                PubPkgStt = s.Status,
                                PubPkgInvSerial = s.InvSerial,
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgTmplId = s.RegisterID,
                                PubPkgPurDate = s.StartDate,
                                PubPkgExpDate = s.EndDate
                            }).ToList();
                        var pub_array = co_pub_pkg_array
                            .AsEnumerable()
                            .Join(co_pub_st_array, co_pub_pkg => co_pub_pkg.PubId, pub_st => pub_st.PubStId, (co_pub_pkg, pub_st) => new
                            {
                                pub_st.CoName,
                                pub_st.PubStCode,
                                pub_st.PubStPubDate,
                                pub_st.PubStStt,
                                pub_st.TaxAuthCode,
                                pub_st.TaxAuthName,
                                pub_st.TaxAuthLocality,
                                co_pub_pkg.PubPkgInvPattern,
                                co_pub_pkg.PubPkgInvSerial,
                                co_pub_pkg.PubPkgBeginNo,
                                co_pub_pkg.PubPkgEndNo,
                                co_pub_pkg.PubPkgPurDate,
                                co_pub_pkg.PubPkgExpDate,
                                co_pub_pkg.PubPkgTmplId,
                                co_pub_pkg.PubPkgStt
                            }).ToList();

                        foreach (var item in pub_array)
                        {
                            CoPubDataModel model_instance = new CoPubDataModel();

                            if (_dbDataTester.Decisions.Any(d => d.ListInvPattern == item.PubPkgInvPattern))
                            {
                                var exist_dec = _dbDataTester.Decisions.FirstOrDefault(d => d.ListInvPattern == item.PubPkgInvPattern);

                                model_instance.DecNo = exist_dec.DecisionNo;
                            }
                            else
                            {
                                model_instance.DecNo = string.Empty;
                            }
                            model_instance.PubStCode = item.PubStCode;
                            model_instance.PubStPubDate = item.PubStPubDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubStStt = item.PubStStt.Value;
                            model_instance.TaxAuthCode = item.TaxAuthCode;
                            model_instance.TaxAuthName = item.TaxAuthName;
                            model_instance.TaxAuthLocality = item.TaxAuthLocality;
                            model_instance.PubInvPattern = item.PubPkgInvPattern;
                            model_instance.PubInvSerial = item.PubPkgInvSerial;
                            model_instance.PubTmplInd = item.PubPkgInvPattern + " – " + item.PubPkgInvSerial;
                            model_instance.PubTmplId = item.PubPkgTmplId.Value;
                            model_instance.PubPkgPurDate = item.PubPkgPurDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgExpDate = item.PubPkgExpDate.Value.ToString("yyyy-MM-dd");
                            model_instance.PubPkgBeginNo = int.Parse(item.PubPkgBeginNo.ToString());
                            model_instance.PubPkgEndNo = int.Parse(item.PubPkgEndNo.ToString());
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoPub, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            //return Json(result, JsonRequestBehavior.AllowGet);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public string UpdateCoPub(int PubTmplId, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var pub_st = _dbDataHD.PublishInvoices
                    .Where(pi => pi.RegisterID == PubTmplId)
                    .Select(s => new
                    {
                        PubStId = s.PublishID.Value
                    }).ToList();
                int pub_st_id = pub_st[0].PubStId;
                var pub_pkg_to_update = _dbDataHD.PublishInvoices.SingleOrDefault(pi => pi.RegisterID == PubTmplId);
                var pub_st_to_update = _dbDataHD.Publishes.SingleOrDefault(p => p.id == pub_st_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "PubStPubDate":
                            pub_st_to_update.PublishDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubStStt":
                            pub_st_to_update.Status = int.Parse(data_val.ToString());
                            pub_pkg_to_update.Status = int.Parse(data_val.ToString());
                            break;
                        case "TaxAuthCode":
                            pub_st_to_update.TaxAuthorityCode = data_val.ToString();
                            break;
                        case "TaxAuthName":
                            pub_st_to_update.TaxAuthorityName = data_val.ToString();
                            break;
                        case "TaxAuthLocality":
                            pub_st_to_update.City = data_val.ToString();
                            break;
                        case "PubPkgPurDate":
                            pub_pkg_to_update.StartDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgExpDate":
                            pub_pkg_to_update.EndDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgBeginNo":
                            pub_pkg_to_update.FromNo = int.Parse(data_val.ToString());
                            break;
                        case "PubPkgEndNo":
                            pub_pkg_to_update.ToNo = int.Parse(data_val.ToString());
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoPub, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var pub_st = _dbDataMIFI.PublishInvoices
                    .Where(pi => pi.RegisterID == PubTmplId)
                    .Select(s => new
                    {
                        PubStId = s.PublishID.Value
                    }).ToList();

                int pub_st_id = pub_st[0].PubStId;
                var pub_pkg_to_update = _dbDataMIFI.PublishInvoices.SingleOrDefault(pi => pi.RegisterID == PubTmplId);
                var pub_st_to_update = _dbDataMIFI.Publishes.SingleOrDefault(p => p.id == pub_st_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "PubStPubDate":
                            pub_st_to_update.PublishDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubStStt":
                            pub_st_to_update.Status = int.Parse(data_val.ToString());
                            pub_pkg_to_update.Status = int.Parse(data_val.ToString());
                            break;
                        case "TaxAuthCode":
                            pub_st_to_update.TaxAuthorityCode = data_val.ToString();
                            break;
                        case "TaxAuthName":
                            pub_st_to_update.TaxAuthorityName = data_val.ToString();
                            break;
                        case "TaxAuthLocality":
                            pub_st_to_update.City = data_val.ToString();
                            break;
                        case "PubPkgPurDate":
                            pub_pkg_to_update.StartDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgExpDate":
                            pub_pkg_to_update.EndDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgBeginNo":
                            pub_pkg_to_update.FromNo = int.Parse(data_val.ToString());
                            break;
                        case "PubPkgEndNo":
                            pub_pkg_to_update.ToNo = int.Parse(data_val.ToString());
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoPub, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var pub_st = _dbDataMB.PublishInvoices
                    .Where(pi => pi.RegisterID == PubTmplId)
                    .Select(s => new
                    {
                        PubStId = s.PublishID.Value
                    }).ToList();

                int pub_st_id = pub_st[0].PubStId;
                var pub_pkg_to_update = _dbDataMB.PublishInvoices.SingleOrDefault(pi => pi.RegisterID == PubTmplId);
                var pub_st_to_update = _dbDataMB.Publishes.SingleOrDefault(p => p.id == pub_st_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "PubStPubDate":
                            pub_st_to_update.PublishDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubStStt":
                            pub_st_to_update.Status = int.Parse(data_val.ToString());
                            pub_pkg_to_update.Status = int.Parse(data_val.ToString());
                            break;
                        case "TaxAuthCode":
                            pub_st_to_update.TaxAuthorityCode = data_val.ToString();
                            break;
                        case "TaxAuthName":
                            pub_st_to_update.TaxAuthorityName = data_val.ToString();
                            break;
                        case "TaxAuthLocality":
                            pub_st_to_update.City = data_val.ToString();
                            break;
                        case "PubPkgPurDate":
                            pub_pkg_to_update.StartDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgExpDate":
                            pub_pkg_to_update.EndDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgBeginNo":
                            pub_pkg_to_update.FromNo = int.Parse(data_val.ToString());
                            break;
                        case "PubPkgEndNo":
                            pub_pkg_to_update.ToNo = int.Parse(data_val.ToString());
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoPub, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var pub_st = _dbDataTester.PublishInvoices
                    .Where(pi => pi.RegisterID == PubTmplId)
                    .Select(s => new
                    {
                        PubStId = s.PublishID.Value
                    }).ToList();

                int pub_st_id = pub_st[0].PubStId;
                var pub_pkg_to_update = _dbDataTester.PublishInvoices.SingleOrDefault(pi => pi.RegisterID == PubTmplId);
                var pub_st_to_update = _dbDataTester.Publishes.SingleOrDefault(p => p.id == pub_st_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "PubStPubDate":
                            pub_st_to_update.PublishDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubStStt":
                            pub_st_to_update.Status = int.Parse(data_val.ToString());
                            pub_pkg_to_update.Status = int.Parse(data_val.ToString());
                            break;
                        case "TaxAuthCode":
                            pub_st_to_update.TaxAuthorityCode = data_val.ToString();
                            break;
                        case "TaxAuthName":
                            pub_st_to_update.TaxAuthorityName = data_val.ToString();
                            break;
                        case "TaxAuthLocality":
                            pub_st_to_update.City = data_val.ToString();
                            break;
                        case "PubPkgPurDate":
                            pub_pkg_to_update.StartDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgExpDate":
                            pub_pkg_to_update.EndDate = DateTime.Parse(data_val.ToString());
                            break;
                        case "PubPkgBeginNo":
                            pub_pkg_to_update.FromNo = int.Parse(data_val.ToString());
                            break;
                        case "PubPkgEndNo":
                            pub_pkg_to_update.ToNo = int.Parse(data_val.ToString());
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoPub, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetPubTmpl(int PubTmplId, int DbId)
        {
            PubTmplDataModel result = new PubTmplDataModel();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var reg_tmpl = _dbQldvHD.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvHD.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        result.TmplCss = reg_tmpl[0].TmplCss;
                        result.LogoCss = reg_tmpl[0].LogoCss;
                        result.BgrCss = reg_tmpl[0].BgrCss;
                        result.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetPubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var reg_tmpl = _dbQldvMIFI.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvMIFI.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        result.TmplCss = reg_tmpl[0].TmplCss;
                        result.LogoCss = reg_tmpl[0].LogoCss;
                        result.BgrCss = reg_tmpl[0].BgrCss;
                        result.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetPubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        var reg_tmpl = _dbQldvMB.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvMB.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        result.TmplCss = reg_tmpl[0].TmplCss;
                        result.LogoCss = reg_tmpl[0].LogoCss;
                        result.BgrCss = reg_tmpl[0].BgrCss;
                        result.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetPubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        var reg_tmpl = _dbQldvTester.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvTester.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        result.TmplCss = reg_tmpl[0].TmplCss;
                        result.LogoCss = reg_tmpl[0].LogoCss;
                        result.BgrCss = reg_tmpl[0].BgrCss;
                        result.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetPubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            //return Json(result, JsonRequestBehavior.AllowGet);

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetCoInvList(int CoId, int PubTmplId, int DbId)
        {
            List<CoInvDataModel> result = new List<CoInvDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var pub_pkg_array = _dbDataHD.PublishInvoices
                            .Where(pi => pi.RegisterID == PubTmplId)
                            .Select(s => new
                            {
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgInvSerial = s.InvSerial
                            }).ToList();

                        foreach (var pub_pkg in pub_pkg_array)
                        {
                            string pub_pattern = pub_pkg.PubPkgInvPattern;
                            string pub_serial = pub_pkg.PubPkgInvSerial;
                            var co_inv = _dbDataHD.VATInvoices
                                .OrderBy(vi => vi.No)
                                .Where(vi => vi.ComID == CoId && vi.Pattern == pub_pattern && vi.Serial == pub_serial)
                                .Select(s => new
                                {
                                    InvId = s.id,
                                    InvPattern = s.Pattern,
                                    InvSerial = s.Serial,
                                    InvNo = s.No,
                                    InvCus = s.CusName,
                                    InvAmount = s.Amount
                                }).ToList();

                            foreach (var item in co_inv)
                            {
                                CoInvDataModel model_instance = new CoInvDataModel();

                                model_instance.InvId = item.InvId;
                                model_instance.InvPattern = item.InvPattern;
                                model_instance.InvSerial = item.InvSerial;
                                model_instance.InvNo = item.InvNo.Value;
                                model_instance.InvCus = item.InvCus;
                                model_instance.InvAmount = item.InvAmount.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var pub_pkg_array = _dbDataMIFI.PublishInvoices
                            .Where(pi => pi.RegisterID == PubTmplId)
                            .Select(s => new
                            {
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgInvSerial = s.InvSerial
                            }).ToList();

                        foreach (var pub_pkg in pub_pkg_array)
                        {
                            string pub_pattern = pub_pkg.PubPkgInvPattern;
                            string pub_serial = pub_pkg.PubPkgInvSerial;
                            var co_inv = _dbDataMIFI.VATInvoices
                                .OrderBy(vi => vi.No)
                                .Where(vi => vi.ComID == CoId && vi.Pattern == pub_pattern && vi.Serial == pub_serial)
                                .Select(s => new
                                {
                                    InvId = s.id,
                                    InvPattern = s.Pattern,
                                    InvSerial = s.Serial,
                                    InvNo = s.No,
                                    InvCus = s.CusName,
                                    InvAmount = s.Amount
                                }).ToList();

                            foreach (var item in co_inv)
                            {
                                CoInvDataModel model_instance = new CoInvDataModel();

                                model_instance.InvId = item.InvId;
                                model_instance.InvPattern = item.InvPattern;
                                model_instance.InvSerial = item.InvSerial;
                                model_instance.InvNo = item.InvNo.Value;
                                model_instance.InvCus = item.InvCus;
                                model_instance.InvAmount = item.InvAmount.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        var pub_pkg_array = _dbDataMB.PublishInvoices
                            .Where(pi => pi.RegisterID == PubTmplId)
                            .Select(s => new
                            {
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgInvSerial = s.InvSerial
                            }).ToList();

                        foreach (var pub_pkg in pub_pkg_array)
                        {
                            string pub_pattern = pub_pkg.PubPkgInvPattern;
                            string pub_serial = pub_pkg.PubPkgInvSerial;
                            var co_inv = _dbDataMB.VATInvoices
                                .OrderBy(vi => vi.No)
                                .Where(vi => vi.ComID == CoId && vi.Pattern == pub_pattern && vi.Serial == pub_serial)
                                .Select(s => new
                                {
                                    InvId = s.id,
                                    InvPattern = s.Pattern,
                                    InvSerial = s.Serial,
                                    InvNo = s.No,
                                    InvCus = s.CusName,
                                    InvAmount = s.Amount
                                }).ToList();

                            foreach (var item in co_inv)
                            {
                                CoInvDataModel model_instance = new CoInvDataModel();

                                model_instance.InvId = item.InvId;
                                model_instance.InvPattern = item.InvPattern;
                                model_instance.InvSerial = item.InvSerial;
                                model_instance.InvNo = item.InvNo.Value;
                                model_instance.InvCus = item.InvCus;
                                model_instance.InvAmount = item.InvAmount.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        var pub_pkg_array = _dbDataTester.PublishInvoices
                            .Where(pi => pi.RegisterID == PubTmplId)
                            .Select(s => new
                            {
                                PubPkgInvPattern = s.InvPattern,
                                PubPkgInvSerial = s.InvSerial
                            }).ToList();

                        foreach (var pub_pkg in pub_pkg_array)
                        {
                            string pub_pattern = pub_pkg.PubPkgInvPattern;
                            string pub_serial = pub_pkg.PubPkgInvSerial;
                            var co_inv = _dbDataTester.VATInvoices
                                .OrderBy(vi => vi.No)
                                .Where(vi => vi.ComID == CoId && vi.Pattern == pub_pattern && vi.Serial == pub_serial)
                                .Select(s => new
                                {
                                    InvId = s.id,
                                    InvPattern = s.Pattern,
                                    InvSerial = s.Serial,
                                    InvNo = s.No,
                                    InvCus = s.CusName,
                                    InvAmount = s.Amount
                                }).ToList();

                            foreach (var item in co_inv)
                            {
                                CoInvDataModel model_instance = new CoInvDataModel();                                

                                model_instance.InvId = item.InvId;
                                model_instance.InvPattern = item.InvPattern;
                                model_instance.InvSerial = item.InvSerial;
                                model_instance.InvNo = item.InvNo.Value;
                                model_instance.InvCus = item.InvCus;
                                model_instance.InvAmount = item.InvAmount.Value;
                                result.Add(model_instance);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvList, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string CompInvTmplView(int PubTmplId, string InvTmpl, string InvId, int DbId)
        {
            string result = string.Empty;
            string tmpl_css = string.Empty;
            string tmpl_html = string.Empty;
            string xml_str = string.Empty;
            JObject inv_tmpl = JObject.Parse(InvTmpl);

            foreach (KeyValuePair<string, JToken> item in inv_tmpl)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "TmplCss":
                        tmpl_css += data_val.ToString() + Environment.NewLine + Environment.NewLine;
                        break;
                    case "LogoCss":
                        tmpl_css += data_val.ToString() + Environment.NewLine + Environment.NewLine;
                        break;
                    case "BgrCss":
                        tmpl_css += data_val.ToString() + Environment.NewLine;
                        break;
                    case "TmplHtml":
                        tmpl_html = data_val.ToString();
                        break;
                }
            }

            string xsl_str = tmpl_html;

            if (tmpl_html.Contains("<style type=\"text/css\">") && tmpl_html.Contains("</style>"))
            {
                int css_start_indx = tmpl_html.IndexOf("<style type=\"text/css\">", 0) + "<style type=\"text/css\">".Length;
                int css_end_indx = tmpl_html.IndexOf("</style>", css_start_indx);
                string str_to_replace = tmpl_html.Substring(css_start_indx, css_end_indx - css_start_indx);

                xsl_str = tmpl_html.Replace(str_to_replace, tmpl_css);
            }
            switch (DbId)
            {
                case 1:
                    try
                    {
                        if (InvId.Length == 0 || InvId == "null")
                        {
                            var reg_tmpl = _dbQldvHD.RegisterTemps
                                .Where(c => c.Id == PubTmplId)
                                .Select(s => new
                                {
                                    InvCatId = s.InvCateID
                                }).ToList();
                            var inv_cat_id = reg_tmpl[0].InvCatId;
                            var inv_cat = _dbQldvHD.InvCategories
                                .Where(ic => ic.id == inv_cat_id)
                                .Select(s => new
                                {
                                    s.Name
                                }).ToList();
                            var co_pub_pkg = _dbDataHD.PublishInvoices
                                .Where(pi => pi.RegisterID == PubTmplId)
                                .Select(s => new
                                {
                                    CoId = s.ComId,
                                    PubPkgInvPattern = s.InvPattern,
                                    PubPkgInvSerial = s.InvSerial
                                }).ToList();
                            var co_id = co_pub_pkg[0].CoId;
                            var co_info = _dbQldvHD.Companies
                                .Where(c => c.id == co_id)
                                .Select(s => new
                                {
                                    s.Name,
                                    s.TaxCode,
                                    s.Address,
                                    s.Phone,
                                    s.Fax,
                                    s.Email,
                                    BankNo = s.BankNumber,
                                    s.BankName
                                }).ToList();
                            InvDataModel inv_instance = new InvDataModel();
                            InvDetailsDataModel inv_details = new InvDetailsDataModel();

                            inv_details.InvCatName = inv_cat[0].Name;
                            inv_details.InvPat = co_pub_pkg[0].PubPkgInvPattern;
                            inv_details.InvSerNo = co_pub_pkg[0].PubPkgInvSerial;
                            inv_details.InvNo = "0000000";
                            inv_details.CoName = co_info[0].Name;
                            inv_details.CoTaxCode = co_info[0].TaxCode;
                            inv_details.CoAddr = co_info[0].Address;
                            inv_details.CoPhone = co_info[0].Phone;
                            inv_details.CoFax = co_info[0].Fax;
                            inv_details.CoEmailAddr = co_info[0].Email;
                            inv_details.CoBankNo = co_info[0].BankNo;
                            inv_details.CoBankName = co_info[0].BankName;
                            inv_instance.Details = inv_details;
                            inv_instance.SignStt = "temp";

                            XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                            XmlWriterSettings writer_settings = new XmlWriterSettings();
                            XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                            writer_settings.Encoding = new UnicodeEncoding(false, false);
                            writer_settings.Indent = true;
                            writer_settings.OmitXmlDeclaration = true;
                            serializer_class.Add(string.Empty, string.Empty);
                            using (StringWriter str_writer = new StringWriter())
                            {
                                using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                {
                                    xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                }
                                xml_str = str_writer.ToString();
                            }
                        }
                        else
                        {
                            var inv_id = int.Parse(InvId);
                            var co_inv = _dbDataHD.VATInvoices
                                .Where(vi => vi.id == inv_id)
                                .Select(s => new
                                {
                                    InvData = s.Data,
                                    InvCatName = s.Name,
                                    InvPat = s.Pattern,
                                    InvSerNo = s.Serial,
                                    InvNo = s.No,
                                    CoName = s.ComName,
                                    CoTaxCode = s.ComTaxCode,
                                    CoAddr = s.ComAddress,
                                    CoPhone = s.ComPhone,
                                    CoFax = s.ComFax,
                                    CoBankNo = s.ComBankNo,
                                    CoBankName = s.ComBankName,
                                    PymtMeth = s.PaymentMethod,
                                    CusBuyer = s.Buyer,
                                    s.CusCode,
                                    s.CusName,
                                    s.CusTaxCode,
                                    s.CusPhone,
                                    CusAddr = s.CusAddress,
                                    s.CusBankNo,
                                    s.CusBankName,
                                    InvSubTotal = s.Total,
                                    InvVATDue = s.VATAmount,
                                    InvDiscDue = s.Discount,
                                    InvTotal = s.Amount,
                                    InvTotalInWords = s.AmountInWords,
                                    InvNote = s.Note,
                                    InvRefCode = s.Fkey,
                                    InvSignDate = s.ArisingDate,
                                    InvExtra = s.Extra,
                                    InvSO = s.SO,
                                    InvNonVATProdsTotal = s.GrossValue,
                                    InvNilVATProdsTotal = s.GrossValue0,
                                    InvFivePcVATProdsTotal = s.GrossValue5,
                                    InvTenPcVATProdsTotal = s.GrossValue10,
                                    InvFivePcVATDue = s.VatAmount5,
                                    InvTenPcVATDue = s.VatAmount10
                                }).ToList();

                            if (co_inv[0].InvNo == 0)
                            {
                                var inv_prod = _dbDataHD.ProductInvs
                                    .Where(pi => pi.InvID == inv_id)
                                    .Select(s => new
                                    {
                                        ProdCode = s.Code,
                                        ProdName = s.Name,
                                        ProdPrice = s.Price,
                                        ProdQty = s.Quantity,
                                        ProdUnit = s.Unit,
                                        ProdSubTotal = s.Total,
                                        ProdVATRate = s.VATRate,
                                        ProdDisc = s.Discount,
                                        ProdVATDue = s.VATAmount,
                                        s.ProdType,
                                        ProdDiscDue = s.DiscountAmount,
                                        ProdTotal = s.Amount,
                                        ProdExtra = s.Extra,
                                        ProdStt = s.Stt,
                                        ConsgtNo = s.ConNo,
                                        ProdExpDate = s.ExpDate
                                    }).ToList();
                                InvDataModel inv_instance = new InvDataModel();
                                InvDetailsDataModel inv_details = new InvDetailsDataModel();
                                List<Product> inv_prods = new List<Product>();

                                foreach (var item in inv_prod)
                                {
                                    Product model_instance = new Product();

                                    model_instance.ProdCode = item.ProdCode;
                                    model_instance.ProdName = item.ProdName;
                                    model_instance.ProdPrice = item.ProdPrice == null ? 0 : item.ProdPrice.Value;
                                    model_instance.ProdQty = item.ProdQty == null ? 0 : item.ProdQty.Value;
                                    model_instance.ProdUnit = item.ProdUnit;
                                    model_instance.ProdSubTotal = item.ProdSubTotal == null ? 0 : item.ProdSubTotal.Value;
                                    model_instance.ProdVATRate = item.ProdVATRate == null ? 0 : item.ProdVATRate.Value;
                                    model_instance.ProdDisc = item.ProdDisc == null ? 0 : item.ProdDisc.Value;
                                    model_instance.ProdVATDue = item.ProdVATDue == null ? 0 : item.ProdVATDue.Value;
                                    model_instance.ProdType = item.ProdType == null ? 0 : item.ProdType.Value;
                                    model_instance.ProdDiscDue = item.ProdDiscDue == null ? 0 : item.ProdDiscDue.Value;
                                    model_instance.ProdTotal = item.ProdTotal == null ? 0 : item.ProdTotal.Value;
                                    model_instance.ProdExtra = item.ProdExtra;
                                    model_instance.ProdStt = item.ProdStt == null ? 0 : item.ProdStt.Value;
                                    model_instance.ConsgtNo = item.ConsgtNo;
                                    model_instance.ProdExpDate = item.ProdExpDate;
                                    inv_prods.Add(model_instance);
                                }
                                inv_details.InvCatName = co_inv[0].InvCatName;
                                inv_details.InvPat = co_inv[0].InvPat;
                                inv_details.InvSerNo = co_inv[0].InvSerNo;
                                inv_details.InvNo = "0000000";
                                inv_details.CoName = co_inv[0].CoName;
                                inv_details.CoTaxCode = co_inv[0].CoTaxCode;
                                inv_details.CoAddr = co_inv[0].CoAddr;
                                inv_details.CoPhone = co_inv[0].CoPhone;
                                inv_details.CoFax = co_inv[0].CoFax;
                                inv_details.CoBankNo = co_inv[0].CoBankNo;
                                inv_details.CoBankName = co_inv[0].CoBankName;
                                inv_details.PymtMeth = co_inv[0].PymtMeth;
                                inv_details.CusBuyer = co_inv[0].CusBuyer;
                                inv_details.CusCode = co_inv[0].CusCode;
                                inv_details.CusName = co_inv[0].CusName;
                                inv_details.CusTaxCode = co_inv[0].CusTaxCode;
                                inv_details.CusPhone = co_inv[0].CusPhone;
                                inv_details.CusAddr = co_inv[0].CusAddr;
                                inv_details.CusBankNo = co_inv[0].CusBankNo;
                                inv_details.CusBankName = co_inv[0].CusBankName;
                                inv_details.InvSubTotal = co_inv[0].InvSubTotal == null ? 0 : co_inv[0].InvSubTotal.Value;
                                inv_details.InvVATDue = co_inv[0].InvVATDue == null ? 0 : co_inv[0].InvVATDue.Value;
                                inv_details.InvDiscDue = co_inv[0].InvDiscDue == null ? 0 : co_inv[0].InvDiscDue.Value;
                                inv_details.InvTotal = co_inv[0].InvTotal == null ? 0 : co_inv[0].InvTotal.Value;
                                inv_details.InvTotalInWords = co_inv[0].InvTotalInWords;
                                inv_details.InvNote = co_inv[0].InvNote;
                                inv_details.InvRefCode = co_inv[0].InvRefCode;
                                inv_details.InvSignDate = co_inv[0].InvSignDate.Value.ToString("dd.MM.yyyy");
                                inv_details.InvExtra = co_inv[0].InvExtra;
                                inv_details.NonVATProdsTotal = co_inv[0].InvNonVATProdsTotal == null ? 0 : co_inv[0].InvNonVATProdsTotal.Value;
                                inv_details.NilVATProdsTotal = co_inv[0].InvNilVATProdsTotal == null ? 0 : co_inv[0].InvNilVATProdsTotal.Value;
                                inv_details.FivePcVATProdsTotal = co_inv[0].InvFivePcVATProdsTotal == null ? 0 : co_inv[0].InvFivePcVATProdsTotal.Value;
                                inv_details.TenPcVATProdsTotal = co_inv[0].InvTenPcVATProdsTotal == null ? 0 : co_inv[0].InvTenPcVATProdsTotal.Value;
                                inv_details.FivePcVATDue = co_inv[0].InvFivePcVATDue == null ? 0 : co_inv[0].InvFivePcVATDue.Value;
                                inv_details.TenPcVATDue = co_inv[0].InvTenPcVATDue == null ? 0 : co_inv[0].InvTenPcVATDue.Value;
                                inv_details.Products = inv_prods;
                                inv_instance.Details = inv_details;

                                XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                                XmlWriterSettings writer_settings = new XmlWriterSettings();
                                XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                                writer_settings.Encoding = new UnicodeEncoding(false, false);
                                writer_settings.Indent = true;
                                writer_settings.OmitXmlDeclaration = true;
                                serializer_class.Add(string.Empty, string.Empty);
                                using (StringWriter str_writer = new StringWriter())
                                {
                                    using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                    {
                                        xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                    }
                                    xml_str = str_writer.ToString();
                                }
                            }
                            else
                            {
                                xml_str = co_inv[0].InvData;
                                xml_str = xml_str.Replace("</Invoice>", "<image>sign</image></Invoice>");
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
                case 2:
                    try
                    {
                        if (InvId.Length == 0 || InvId == "null")
                        {
                            var reg_tmpl = _dbQldvMIFI.RegisterTemps
                                .Where(c => c.Id == PubTmplId)
                                .Select(s => new
                                {
                                    InvCatId = s.InvCateID
                                }).ToList();
                            var inv_cat_id = reg_tmpl[0].InvCatId;
                            var inv_cat = _dbQldvMIFI.InvCategories
                                .Where(ic => ic.id == inv_cat_id)
                                .Select(s => new
                                {
                                    s.Name
                                }).ToList();
                            var co_pub_pkg = _dbDataMIFI.PublishInvoices
                                .Where(pi => pi.RegisterID == PubTmplId)
                                .Select(s => new
                                {
                                    CoId = s.ComId,
                                    PubPkgInvPattern = s.InvPattern,
                                    PubPkgInvSerial = s.InvSerial
                                }).ToList();
                            var co_id = co_pub_pkg[0].CoId;
                            var co_info = _dbQldvMIFI.Companies
                                .Where(c => c.id == co_id)
                                .Select(s => new
                                {
                                    s.Name,
                                    s.TaxCode,
                                    s.Address,
                                    s.Phone,
                                    s.Fax,
                                    s.Email,
                                    BankNo = s.BankNumber,
                                    s.BankName
                                }).ToList();
                            InvDataModel inv_instance = new InvDataModel();
                            InvDetailsDataModel inv_details = new InvDetailsDataModel();

                            inv_details.InvCatName = inv_cat[0].Name;
                            inv_details.InvPat = co_pub_pkg[0].PubPkgInvPattern;
                            inv_details.InvSerNo = co_pub_pkg[0].PubPkgInvSerial;
                            inv_details.InvNo = "0000000";
                            inv_details.CoName = co_info[0].Name;
                            inv_details.CoTaxCode = co_info[0].TaxCode;
                            inv_details.CoAddr = co_info[0].Address;
                            inv_details.CoPhone = co_info[0].Phone;
                            inv_details.CoFax = co_info[0].Fax;
                            inv_details.CoEmailAddr = co_info[0].Email;
                            inv_details.CoBankNo = co_info[0].BankNo;
                            inv_details.CoBankName = co_info[0].BankName;
                            inv_instance.Details = inv_details;
                            inv_instance.SignStt = "temp";

                            XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                            XmlWriterSettings writer_settings = new XmlWriterSettings();
                            XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                            writer_settings.Encoding = new UnicodeEncoding(false, false);
                            writer_settings.Indent = true;
                            writer_settings.OmitXmlDeclaration = true;
                            serializer_class.Add(string.Empty, string.Empty);
                            using (StringWriter str_writer = new StringWriter())
                            {
                                using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                {
                                    xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                }
                                xml_str = str_writer.ToString();
                            }
                        }
                        else
                        {
                            var inv_id = int.Parse(InvId);
                            var co_inv = _dbDataMIFI.VATInvoices
                                .Where(vi => vi.id == inv_id)
                                .Select(s => new
                                {
                                    InvData = s.Data,
                                    InvCatName = s.Name,
                                    InvPat = s.Pattern,
                                    InvSerNo = s.Serial,
                                    InvNo = s.No,
                                    CoName = s.ComName,
                                    CoTaxCode = s.ComTaxCode,
                                    CoAddr = s.ComAddress,
                                    CoPhone = s.ComPhone,
                                    CoFax = s.ComFax,
                                    CoBankNo = s.ComBankNo,
                                    CoBankName = s.ComBankName,
                                    PymtMeth = s.PaymentMethod,
                                    CusBuyer = s.Buyer,
                                    s.CusCode,
                                    s.CusName,
                                    s.CusTaxCode,
                                    s.CusPhone,
                                    CusAddr = s.CusAddress,
                                    s.CusBankNo,
                                    s.CusBankName,
                                    InvSubTotal = s.Total,
                                    InvVATDue = s.VATAmount,
                                    InvDiscDue = s.Discount,
                                    InvTotal = s.Amount,
                                    InvTotalInWords = s.AmountInWords,
                                    InvNote = s.Note,
                                    InvRefCode = s.Fkey,
                                    InvSignDate = s.ArisingDate,
                                    InvExtra = s.Extra,
                                    InvSO = s.SO,
                                    InvNonVATProdsTotal = s.GrossValue,
                                    InvNilVATProdsTotal = s.GrossValue0,
                                    InvFivePcVATProdsTotal = s.GrossValue5,
                                    InvTenPcVATProdsTotal = s.GrossValue10,
                                    InvFivePcVATDue = s.VatAmount5,
                                    InvTenPcVATDue = s.VatAmount10
                                }).ToList();

                            if (co_inv[0].InvNo == 0)
                            {
                                var inv_prod = _dbDataMIFI.ProductInvs
                                    .Where(pi => pi.InvID == inv_id)
                                    .Select(s => new
                                    {
                                        ProdCode = s.Code,
                                        ProdName = s.Name,
                                        ProdPrice = s.Price,
                                        ProdQty = s.Quantity,
                                        ProdUnit = s.Unit,
                                        ProdSubTotal = s.Total,
                                        ProdVATRate = s.VATRate,
                                        ProdDisc = s.Discount,
                                        ProdVATDue = s.VATAmount,
                                        s.ProdType,
                                        ProdDiscDue = s.DiscountAmount,
                                        ProdTotal = s.Amount,
                                        ProdExtra = s.Extra,
                                        ProdStt = s.Stt,
                                        ConsgtNo = s.ConNo,
                                        ProdExpDate = s.ExpDate
                                    }).ToList();
                                InvDataModel inv_instance = new InvDataModel();
                                InvDetailsDataModel inv_details = new InvDetailsDataModel();
                                List<Product> inv_prods = new List<Product>();

                                foreach (var item in inv_prod)
                                {
                                    Product model_instance = new Product();

                                    model_instance.ProdCode = item.ProdCode;
                                    model_instance.ProdName = item.ProdName;
                                    model_instance.ProdPrice = item.ProdPrice == null ? 0 : item.ProdPrice.Value;
                                    model_instance.ProdQty = item.ProdQty == null ? 0 : item.ProdQty.Value;
                                    model_instance.ProdUnit = item.ProdUnit;
                                    model_instance.ProdSubTotal = item.ProdSubTotal == null ? 0 : item.ProdSubTotal.Value;
                                    model_instance.ProdVATRate = item.ProdVATRate == null ? 0 : item.ProdVATRate.Value;
                                    model_instance.ProdDisc = item.ProdDisc == null ? 0 : item.ProdDisc.Value;
                                    model_instance.ProdVATDue = item.ProdVATDue == null ? 0 : item.ProdVATDue.Value;
                                    model_instance.ProdType = item.ProdType == null ? 0 : item.ProdType.Value;
                                    model_instance.ProdDiscDue = item.ProdDiscDue == null ? 0 : item.ProdDiscDue.Value;
                                    model_instance.ProdTotal = item.ProdTotal == null ? 0 : item.ProdTotal.Value;
                                    model_instance.ProdExtra = item.ProdExtra;
                                    model_instance.ProdStt = item.ProdStt == null ? 0 : item.ProdStt.Value;
                                    model_instance.ConsgtNo = item.ConsgtNo;
                                    model_instance.ProdExpDate = item.ProdExpDate;
                                    inv_prods.Add(model_instance);
                                }
                                inv_details.InvCatName = co_inv[0].InvCatName;
                                inv_details.InvPat = co_inv[0].InvPat;
                                inv_details.InvSerNo = co_inv[0].InvSerNo;
                                inv_details.InvNo = "0000000";
                                inv_details.CoName = co_inv[0].CoName;
                                inv_details.CoTaxCode = co_inv[0].CoTaxCode;
                                inv_details.CoAddr = co_inv[0].CoAddr;
                                inv_details.CoPhone = co_inv[0].CoPhone;
                                inv_details.CoFax = co_inv[0].CoFax;
                                inv_details.CoBankNo = co_inv[0].CoBankNo;
                                inv_details.CoBankName = co_inv[0].CoBankName;
                                inv_details.PymtMeth = co_inv[0].PymtMeth;
                                inv_details.CusBuyer = co_inv[0].CusBuyer;
                                inv_details.CusCode = co_inv[0].CusCode;
                                inv_details.CusName = co_inv[0].CusName;
                                inv_details.CusTaxCode = co_inv[0].CusTaxCode;
                                inv_details.CusPhone = co_inv[0].CusPhone;
                                inv_details.CusAddr = co_inv[0].CusAddr;
                                inv_details.CusBankNo = co_inv[0].CusBankNo;
                                inv_details.CusBankName = co_inv[0].CusBankName;
                                inv_details.InvSubTotal = co_inv[0].InvSubTotal == null ? 0 : co_inv[0].InvSubTotal.Value;
                                inv_details.InvVATDue = co_inv[0].InvVATDue == null ? 0 : co_inv[0].InvVATDue.Value;
                                inv_details.InvDiscDue = co_inv[0].InvDiscDue == null ? 0 : co_inv[0].InvDiscDue.Value;
                                inv_details.InvTotal = co_inv[0].InvTotal == null ? 0 : co_inv[0].InvTotal.Value;
                                inv_details.InvTotalInWords = co_inv[0].InvTotalInWords;
                                inv_details.InvNote = co_inv[0].InvNote;
                                inv_details.InvRefCode = co_inv[0].InvRefCode;
                                inv_details.InvSignDate = co_inv[0].InvSignDate.Value.ToString("dd.MM.yyyy");
                                inv_details.InvExtra = co_inv[0].InvExtra;
                                inv_details.NonVATProdsTotal = co_inv[0].InvNonVATProdsTotal == null ? 0 : co_inv[0].InvNonVATProdsTotal.Value;
                                inv_details.NilVATProdsTotal = co_inv[0].InvNilVATProdsTotal == null ? 0 : co_inv[0].InvNilVATProdsTotal.Value;
                                inv_details.FivePcVATProdsTotal = co_inv[0].InvFivePcVATProdsTotal == null ? 0 : co_inv[0].InvFivePcVATProdsTotal.Value;
                                inv_details.TenPcVATProdsTotal = co_inv[0].InvTenPcVATProdsTotal == null ? 0 : co_inv[0].InvTenPcVATProdsTotal.Value;
                                inv_details.FivePcVATDue = co_inv[0].InvFivePcVATDue == null ? 0 : co_inv[0].InvFivePcVATDue.Value;
                                inv_details.TenPcVATDue = co_inv[0].InvTenPcVATDue == null ? 0 : co_inv[0].InvTenPcVATDue.Value;
                                inv_details.Products = inv_prods;
                                inv_instance.Details = inv_details;

                                XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                                XmlWriterSettings writer_settings = new XmlWriterSettings();
                                XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                                writer_settings.Encoding = new UnicodeEncoding(false, false);
                                writer_settings.Indent = true;
                                writer_settings.OmitXmlDeclaration = true;
                                serializer_class.Add(string.Empty, string.Empty);
                                using (StringWriter str_writer = new StringWriter())
                                {
                                    using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                    {
                                        xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                    }
                                    xml_str = str_writer.ToString();
                                }
                            }
                            else
                            {
                                xml_str = co_inv[0].InvData;
                                xml_str = xml_str.Replace("</Invoice>", "<image>sign</image></Invoice>");
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
                case 3:
                    try
                    {
                        if (InvId.Length == 0 || InvId == "null")
                        {
                            var reg_tmpl = _dbQldvMB.RegisterTemps
                                .Where(c => c.Id == PubTmplId)
                                .Select(s => new
                                {
                                    InvCatId = s.InvCateID
                                }).ToList();
                            var inv_cat_id = reg_tmpl[0].InvCatId;
                            var inv_cat = _dbQldvMB.InvCategories
                                .Where(ic => ic.id == inv_cat_id)
                                .Select(s => new
                                {
                                    s.Name
                                }).ToList();
                            var co_pub_pkg = _dbDataMB.PublishInvoices
                                .Where(pi => pi.RegisterID == PubTmplId)
                                .Select(s => new
                                {
                                    CoId = s.ComId,
                                    PubPkgInvPattern = s.InvPattern,
                                    PubPkgInvSerial = s.InvSerial
                                }).ToList();
                            var co_id = co_pub_pkg[0].CoId;
                            var co_info = _dbQldvMB.Companies
                                .Where(c => c.id == co_id)
                                .Select(s => new
                                {
                                    s.Name,
                                    s.TaxCode,
                                    s.Address,
                                    s.Phone,
                                    s.Fax,
                                    s.Email,
                                    BankNo = s.BankNumber,
                                    s.BankName
                                }).ToList();
                            InvDataModel inv_instance = new InvDataModel();
                            InvDetailsDataModel inv_details = new InvDetailsDataModel();

                            inv_details.InvCatName = inv_cat[0].Name;
                            inv_details.InvPat = co_pub_pkg[0].PubPkgInvPattern;
                            inv_details.InvSerNo = co_pub_pkg[0].PubPkgInvSerial;
                            inv_details.InvNo = "0000000";
                            inv_details.CoName = co_info[0].Name;
                            inv_details.CoTaxCode = co_info[0].TaxCode;
                            inv_details.CoAddr = co_info[0].Address;
                            inv_details.CoPhone = co_info[0].Phone;
                            inv_details.CoFax = co_info[0].Fax;
                            inv_details.CoEmailAddr = co_info[0].Email;
                            inv_details.CoBankNo = co_info[0].BankNo;
                            inv_details.CoBankName = co_info[0].BankName;
                            inv_instance.Details = inv_details;
                            inv_instance.SignStt = "temp";

                            XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                            XmlWriterSettings writer_settings = new XmlWriterSettings();
                            XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                            writer_settings.Encoding = new UnicodeEncoding(false, false);
                            writer_settings.Indent = true;
                            writer_settings.OmitXmlDeclaration = true;
                            serializer_class.Add(string.Empty, string.Empty);
                            using (StringWriter str_writer = new StringWriter())
                            {
                                using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                {
                                    xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                }
                                xml_str = str_writer.ToString();
                            }
                        }
                        else
                        {
                            var inv_id = int.Parse(InvId);
                            var co_inv = _dbDataMB.VATInvoices
                                .Where(vi => vi.id == inv_id)
                                .Select(s => new
                                {
                                    InvData = s.Data,
                                    InvCatName = s.Name,
                                    InvPat = s.Pattern,
                                    InvSerNo = s.Serial,
                                    InvNo = s.No,
                                    CoName = s.ComName,
                                    CoTaxCode = s.ComTaxCode,
                                    CoAddr = s.ComAddress,
                                    CoPhone = s.ComPhone,
                                    CoFax = s.ComFax,
                                    CoBankNo = s.ComBankNo,
                                    CoBankName = s.ComBankName,
                                    PymtMeth = s.PaymentMethod,
                                    CusBuyer = s.Buyer,
                                    s.CusCode,
                                    s.CusName,
                                    s.CusTaxCode,
                                    s.CusPhone,
                                    CusAddr = s.CusAddress,
                                    s.CusBankNo,
                                    s.CusBankName,
                                    InvSubTotal = s.Total,
                                    InvVATDue = s.VATAmount,
                                    InvDiscDue = s.Discount,
                                    InvTotal = s.Amount,
                                    InvTotalInWords = s.AmountInWords,
                                    InvNote = s.Note,
                                    InvRefCode = s.Fkey,
                                    InvSignDate = s.ArisingDate,
                                    InvExtra = s.Extra,
                                    InvSO = s.SO,
                                    InvNonVATProdsTotal = s.GrossValue,
                                    InvNilVATProdsTotal = s.GrossValue0,
                                    InvFivePcVATProdsTotal = s.GrossValue5,
                                    InvTenPcVATProdsTotal = s.GrossValue10,
                                    InvFivePcVATDue = s.VatAmount5,
                                    InvTenPcVATDue = s.VatAmount10
                                }).ToList();

                            if (co_inv[0].InvNo == 0)
                            {
                                var inv_prod = _dbDataMB.ProductInvs
                                    .Where(pi => pi.InvID == inv_id)
                                    .Select(s => new
                                    {
                                        ProdCode = s.Code,
                                        ProdName = s.Name,
                                        ProdPrice = s.Price,
                                        ProdQty = s.Quantity,
                                        ProdUnit = s.Unit,
                                        ProdSubTotal = s.Total,
                                        ProdVATRate = s.VATRate,
                                        ProdDisc = s.Discount,
                                        ProdVATDue = s.VATAmount,
                                        s.ProdType,
                                        ProdDiscDue = s.DiscountAmount,
                                        ProdTotal = s.Amount,
                                        ProdExtra = s.Extra,
                                        ProdStt = s.Stt,
                                        ConsgtNo = s.ConNo,
                                        ProdExpDate = s.ExpDate
                                    }).ToList();
                                InvDataModel inv_instance = new InvDataModel();
                                InvDetailsDataModel inv_details = new InvDetailsDataModel();
                                List<Product> inv_prods = new List<Product>();

                                foreach (var item in inv_prod)
                                {
                                    Product model_instance = new Product();

                                    model_instance.ProdCode = item.ProdCode;
                                    model_instance.ProdName = item.ProdName;
                                    model_instance.ProdPrice = item.ProdPrice == null ? 0 : item.ProdPrice.Value;
                                    model_instance.ProdQty = item.ProdQty == null ? 0 : item.ProdQty.Value;
                                    model_instance.ProdUnit = item.ProdUnit;
                                    model_instance.ProdSubTotal = item.ProdSubTotal == null ? 0 : item.ProdSubTotal.Value;
                                    model_instance.ProdVATRate = item.ProdVATRate == null ? 0 : item.ProdVATRate.Value;
                                    model_instance.ProdDisc = item.ProdDisc == null ? 0 : item.ProdDisc.Value;
                                    model_instance.ProdVATDue = item.ProdVATDue == null ? 0 : item.ProdVATDue.Value;
                                    model_instance.ProdType = item.ProdType == null ? 0 : item.ProdType.Value;
                                    model_instance.ProdDiscDue = item.ProdDiscDue == null ? 0 : item.ProdDiscDue.Value;
                                    model_instance.ProdTotal = item.ProdTotal == null ? 0 : item.ProdTotal.Value;
                                    model_instance.ProdExtra = item.ProdExtra;
                                    model_instance.ProdStt = item.ProdStt == null ? 0 : item.ProdStt.Value;
                                    model_instance.ConsgtNo = item.ConsgtNo;
                                    model_instance.ProdExpDate = item.ProdExpDate;
                                    inv_prods.Add(model_instance);
                                }
                                inv_details.InvCatName = co_inv[0].InvCatName;
                                inv_details.InvPat = co_inv[0].InvPat;
                                inv_details.InvSerNo = co_inv[0].InvSerNo;
                                inv_details.InvNo = "0000000";
                                inv_details.CoName = co_inv[0].CoName;
                                inv_details.CoTaxCode = co_inv[0].CoTaxCode;
                                inv_details.CoAddr = co_inv[0].CoAddr;
                                inv_details.CoPhone = co_inv[0].CoPhone;
                                inv_details.CoFax = co_inv[0].CoFax;
                                inv_details.CoBankNo = co_inv[0].CoBankNo;
                                inv_details.CoBankName = co_inv[0].CoBankName;
                                inv_details.PymtMeth = co_inv[0].PymtMeth;
                                inv_details.CusBuyer = co_inv[0].CusBuyer;
                                inv_details.CusCode = co_inv[0].CusCode;
                                inv_details.CusName = co_inv[0].CusName;
                                inv_details.CusTaxCode = co_inv[0].CusTaxCode;
                                inv_details.CusPhone = co_inv[0].CusPhone;
                                inv_details.CusAddr = co_inv[0].CusAddr;
                                inv_details.CusBankNo = co_inv[0].CusBankNo;
                                inv_details.CusBankName = co_inv[0].CusBankName;
                                inv_details.InvSubTotal = co_inv[0].InvSubTotal == null ? 0 : co_inv[0].InvSubTotal.Value;
                                inv_details.InvVATDue = co_inv[0].InvVATDue == null ? 0 : co_inv[0].InvVATDue.Value;
                                inv_details.InvDiscDue = co_inv[0].InvDiscDue == null ? 0 : co_inv[0].InvDiscDue.Value;
                                inv_details.InvTotal = co_inv[0].InvTotal == null ? 0 : co_inv[0].InvTotal.Value;
                                inv_details.InvTotalInWords = co_inv[0].InvTotalInWords;
                                inv_details.InvNote = co_inv[0].InvNote;
                                inv_details.InvRefCode = co_inv[0].InvRefCode;
                                inv_details.InvSignDate = co_inv[0].InvSignDate.Value.ToString("dd.MM.yyyy");
                                inv_details.InvExtra = co_inv[0].InvExtra;
                                inv_details.NonVATProdsTotal = co_inv[0].InvNonVATProdsTotal == null ? 0 : co_inv[0].InvNonVATProdsTotal.Value;
                                inv_details.NilVATProdsTotal = co_inv[0].InvNilVATProdsTotal == null ? 0 : co_inv[0].InvNilVATProdsTotal.Value;
                                inv_details.FivePcVATProdsTotal = co_inv[0].InvFivePcVATProdsTotal == null ? 0 : co_inv[0].InvFivePcVATProdsTotal.Value;
                                inv_details.TenPcVATProdsTotal = co_inv[0].InvTenPcVATProdsTotal == null ? 0 : co_inv[0].InvTenPcVATProdsTotal.Value;
                                inv_details.FivePcVATDue = co_inv[0].InvFivePcVATDue == null ? 0 : co_inv[0].InvFivePcVATDue.Value;
                                inv_details.TenPcVATDue = co_inv[0].InvTenPcVATDue == null ? 0 : co_inv[0].InvTenPcVATDue.Value;
                                inv_details.Products = inv_prods;
                                inv_instance.Details = inv_details;

                                XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                                XmlWriterSettings writer_settings = new XmlWriterSettings();
                                XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                                writer_settings.Encoding = new UnicodeEncoding(false, false);
                                writer_settings.Indent = true;
                                writer_settings.OmitXmlDeclaration = true;
                                serializer_class.Add(string.Empty, string.Empty);
                                using (StringWriter str_writer = new StringWriter())
                                {
                                    using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                    {
                                        xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                    }
                                    xml_str = str_writer.ToString();
                                }
                            }
                            else
                            {
                                xml_str = co_inv[0].InvData;
                                xml_str = xml_str.Replace("</Invoice>", "<image>sign</image></Invoice>");
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
                case 0:
                    try
                    {
                        if (InvId.Length == 0 || InvId == "null")
                        {
                            var reg_tmpl = _dbQldvTester.RegisterTemps
                                .Where(c => c.Id == PubTmplId)
                                .Select(s => new
                                {
                                    InvCatId = s.InvCateID
                                }).ToList();
                            var inv_cat_id = reg_tmpl[0].InvCatId;
                            var inv_cat = _dbQldvTester.InvCategories
                                .Where(ic => ic.id == inv_cat_id)
                                .Select(s => new
                                {
                                    s.Name
                                }).ToList();
                            var co_pub_pkg = _dbDataTester.PublishInvoices
                                .Where(pi => pi.RegisterID == PubTmplId)
                                .Select(s => new
                                {
                                    CoId = s.ComId,
                                    PubPkgInvPattern = s.InvPattern,
                                    PubPkgInvSerial = s.InvSerial
                                }).ToList();
                            var co_id = co_pub_pkg[0].CoId;
                            var co_info = _dbQldvTester.Companies
                                .Where(c => c.id == co_id)
                                .Select(s => new
                                {
                                    s.Name,
                                    s.TaxCode,
                                    s.Address,
                                    s.Phone,
                                    s.Fax,
                                    s.Email,
                                    BankNo = s.BankNumber,
                                    s.BankName
                                }).ToList();
                            InvDataModel inv_instance = new InvDataModel();
                            InvDetailsDataModel inv_details = new InvDetailsDataModel();

                            inv_details.InvCatName = inv_cat[0].Name;
                            inv_details.InvPat = co_pub_pkg[0].PubPkgInvPattern;
                            inv_details.InvSerNo = co_pub_pkg[0].PubPkgInvSerial;
                            inv_details.InvNo = "0000000";
                            inv_details.CoName = co_info[0].Name;
                            inv_details.CoTaxCode = co_info[0].TaxCode;
                            inv_details.CoAddr = co_info[0].Address;
                            inv_details.CoPhone = co_info[0].Phone;
                            inv_details.CoFax = co_info[0].Fax;
                            inv_details.CoEmailAddr = co_info[0].Email;
                            inv_details.CoBankNo = co_info[0].BankNo;
                            inv_details.CoBankName = co_info[0].BankName;
                            inv_instance.Details = inv_details;
                            inv_instance.SignStt = "temp";

                            XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                            XmlWriterSettings writer_settings = new XmlWriterSettings();
                            XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                            writer_settings.Encoding = new UnicodeEncoding(false, false);
                            writer_settings.Indent = true;
                            writer_settings.OmitXmlDeclaration = true;
                            serializer_class.Add(string.Empty, string.Empty);
                            using (StringWriter str_writer = new StringWriter())
                            {
                                using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                {
                                    xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                }
                                xml_str = str_writer.ToString();
                            }
                        }
                        else
                        {
                            var inv_id = int.Parse(InvId);
                            var co_inv = _dbDataTester.VATInvoices
                                .Where(vi => vi.id == inv_id)
                                .Select(s => new
                                {
                                    InvData = s.Data,
                                    InvCatName = s.Name,
                                    InvPat = s.Pattern,
                                    InvSerNo = s.Serial,
                                    InvNo = s.No,
                                    CoName = s.ComName,
                                    CoTaxCode = s.ComTaxCode,
                                    CoAddr = s.ComAddress,
                                    CoPhone = s.ComPhone,
                                    CoFax = s.ComFax,
                                    CoBankNo = s.ComBankNo,
                                    CoBankName = s.ComBankName,
                                    PymtMeth = s.PaymentMethod,
                                    CusBuyer = s.Buyer,
                                    s.CusCode,
                                    s.CusName,
                                    s.CusTaxCode,
                                    s.CusPhone,
                                    CusAddr = s.CusAddress,
                                    s.CusBankNo,
                                    s.CusBankName,
                                    InvSubTotal = s.Total,
                                    InvVATDue = s.VATAmount,
                                    InvDiscDue = s.Discount,
                                    InvTotal = s.Amount,
                                    InvTotalInWords = s.AmountInWords,
                                    InvNote = s.Note,
                                    InvRefCode = s.Fkey,
                                    InvSignDate = s.ArisingDate,
                                    InvExtra = s.Extra,
                                    InvSO = s.SO,
                                    InvNonVATProdsTotal = s.GrossValue,
                                    InvNilVATProdsTotal = s.GrossValue0,
                                    InvFivePcVATProdsTotal = s.GrossValue5,
                                    InvTenPcVATProdsTotal = s.GrossValue10,
                                    InvFivePcVATDue = s.VatAmount5,
                                    InvTenPcVATDue = s.VatAmount10
                                }).ToList();

                            if (co_inv[0].InvNo == 0)
                            {
                                var inv_prod = _dbDataTester.ProductInvs
                                    .Where(pi => pi.InvID == inv_id)
                                    .Select(s => new
                                    {
                                        ProdCode = s.Code,
                                        ProdName = s.Name,
                                        ProdPrice = s.Price,
                                        ProdQty = s.Quantity,
                                        ProdUnit = s.Unit,
                                        ProdSubTotal = s.Total,
                                        ProdVATRate = s.VATRate,
                                        ProdDisc = s.Discount,
                                        ProdVATDue = s.VATAmount,
                                        s.ProdType,
                                        ProdDiscDue = s.DiscountAmount,
                                        ProdTotal = s.Amount,
                                        ProdExtra = s.Extra,
                                        ProdStt = s.Stt,
                                        ConsgtNo = s.ConNo,
                                        ProdExpDate = s.ExpDate
                                    }).ToList();
                                InvDataModel inv_instance = new InvDataModel();
                                InvDetailsDataModel inv_details = new InvDetailsDataModel();
                                List<Product> inv_prods = new List<Product>();

                                foreach (var item in inv_prod)
                                {
                                    Product model_instance = new Product();

                                    model_instance.ProdCode = item.ProdCode;
                                    model_instance.ProdName = item.ProdName;
                                    model_instance.ProdPrice = item.ProdPrice == null ? 0 : item.ProdPrice.Value;
                                    model_instance.ProdQty = item.ProdQty == null ? 0 : item.ProdQty.Value;
                                    model_instance.ProdUnit = item.ProdUnit;
                                    model_instance.ProdSubTotal = item.ProdSubTotal == null ? 0 : item.ProdSubTotal.Value;
                                    model_instance.ProdVATRate = item.ProdVATRate == null ? 0 : item.ProdVATRate.Value;
                                    model_instance.ProdDisc = item.ProdDisc == null ? 0 : item.ProdDisc.Value;
                                    model_instance.ProdVATDue = item.ProdVATDue == null ? 0 : item.ProdVATDue.Value;
                                    model_instance.ProdType = item.ProdType == null ? 0 : item.ProdType.Value;
                                    model_instance.ProdDiscDue = item.ProdDiscDue == null ? 0 : item.ProdDiscDue.Value;
                                    model_instance.ProdTotal = item.ProdTotal == null ? 0 : item.ProdTotal.Value;
                                    model_instance.ProdExtra = item.ProdExtra;
                                    model_instance.ProdStt = item.ProdStt == null ? 0 : item.ProdStt.Value;
                                    model_instance.ConsgtNo = item.ConsgtNo;
                                    model_instance.ProdExpDate = item.ProdExpDate;
                                    inv_prods.Add(model_instance);
                                }
                                inv_details.InvCatName = co_inv[0].InvCatName;
                                inv_details.InvPat = co_inv[0].InvPat;
                                inv_details.InvSerNo = co_inv[0].InvSerNo;
                                inv_details.InvNo = "0000000";
                                inv_details.CoName = co_inv[0].CoName;
                                inv_details.CoTaxCode = co_inv[0].CoTaxCode;
                                inv_details.CoAddr = co_inv[0].CoAddr;
                                inv_details.CoPhone = co_inv[0].CoPhone;
                                inv_details.CoFax = co_inv[0].CoFax;
                                inv_details.CoBankNo = co_inv[0].CoBankNo;
                                inv_details.CoBankName = co_inv[0].CoBankName;
                                inv_details.PymtMeth = co_inv[0].PymtMeth;
                                inv_details.CusBuyer = co_inv[0].CusBuyer;
                                inv_details.CusCode = co_inv[0].CusCode;
                                inv_details.CusName = co_inv[0].CusName;
                                inv_details.CusTaxCode = co_inv[0].CusTaxCode;
                                inv_details.CusPhone = co_inv[0].CusPhone;
                                inv_details.CusAddr = co_inv[0].CusAddr;
                                inv_details.CusBankNo = co_inv[0].CusBankNo;
                                inv_details.CusBankName = co_inv[0].CusBankName;
                                inv_details.InvSubTotal = co_inv[0].InvSubTotal == null ? 0 : co_inv[0].InvSubTotal.Value;
                                inv_details.InvVATDue = co_inv[0].InvVATDue == null ? 0 : co_inv[0].InvVATDue.Value;
                                inv_details.InvDiscDue = co_inv[0].InvDiscDue == null ? 0 : co_inv[0].InvDiscDue.Value;
                                inv_details.InvTotal = co_inv[0].InvTotal == null ? 0 : co_inv[0].InvTotal.Value;
                                inv_details.InvTotalInWords = co_inv[0].InvTotalInWords;
                                inv_details.InvNote = co_inv[0].InvNote;
                                inv_details.InvRefCode = co_inv[0].InvRefCode;
                                inv_details.InvSignDate = co_inv[0].InvSignDate.Value.ToString("dd.MM.yyyy");
                                inv_details.InvExtra = co_inv[0].InvExtra;
                                inv_details.NonVATProdsTotal = co_inv[0].InvNonVATProdsTotal == null ? 0 : co_inv[0].InvNonVATProdsTotal.Value;
                                inv_details.NilVATProdsTotal = co_inv[0].InvNilVATProdsTotal == null ? 0 : co_inv[0].InvNilVATProdsTotal.Value;
                                inv_details.FivePcVATProdsTotal = co_inv[0].InvFivePcVATProdsTotal == null ? 0 : co_inv[0].InvFivePcVATProdsTotal.Value;
                                inv_details.TenPcVATProdsTotal = co_inv[0].InvTenPcVATProdsTotal == null ? 0 : co_inv[0].InvTenPcVATProdsTotal.Value;
                                inv_details.FivePcVATDue = co_inv[0].InvFivePcVATDue == null ? 0 : co_inv[0].InvFivePcVATDue.Value;
                                inv_details.TenPcVATDue = co_inv[0].InvTenPcVATDue == null ? 0 : co_inv[0].InvTenPcVATDue.Value;
                                inv_details.Products = inv_prods;
                                inv_instance.Details = inv_details;

                                XmlSerializer xml_serializer = new XmlSerializer(inv_instance.GetType());
                                XmlWriterSettings writer_settings = new XmlWriterSettings();
                                XmlSerializerNamespaces serializer_class = new XmlSerializerNamespaces();

                                writer_settings.Encoding = new UnicodeEncoding(false, false);
                                writer_settings.Indent = true;
                                writer_settings.OmitXmlDeclaration = true;
                                serializer_class.Add(string.Empty, string.Empty);
                                using (StringWriter str_writer = new StringWriter())
                                {
                                    using (XmlWriter xml_writer = XmlWriter.Create(str_writer, writer_settings))
                                    {
                                        xml_serializer.Serialize(xml_writer, inv_instance, serializer_class);
                                    }
                                    xml_str = str_writer.ToString();
                                }
                            }
                            else
                            {
                                xml_str = co_inv[0].InvData;
                                xml_str = xml_str.Replace("</Invoice>", "<image>sign</image></Invoice>");
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        return _resultError;
                    }
                    break;
            }
            try
            {
                XslCompiledTransform xsl_compiler = new XslCompiledTransform();
                using (XmlReader xml_reader = XmlReader.Create(new StringReader(xsl_str)))
                {
                    xsl_compiler.Load(xml_reader);
                }

                StringWriter str_writer = new StringWriter();

                using (XmlReader xml_reader = XmlReader.Create(new StringReader(xml_str)))
                {
                    xsl_compiler.Transform(xml_reader, null, str_writer);
                }
                result = str_writer.ToString();
            }
            catch
            {
                result = "2500:warning:300:Văn bản mẫu hóa đơn không hợp lệ.";
                return result;
            }
            
            return result;
        }

        [HttpGet]
        public ActionResult CompInvTmplPdf(int CoId, int PubTmplId, string InvId, int DbId)
        {
            PubTmplDataModel tmpl_data = new PubTmplDataModel();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var reg_tmpl = _dbQldvHD.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvHD.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        tmpl_data.TmplCss = reg_tmpl[0].TmplCss;
                        tmpl_data.LogoCss = reg_tmpl[0].LogoCss;
                        tmpl_data.BgrCss = reg_tmpl[0].BgrCss;
                        tmpl_data.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        throw;
                    }
                    break;
                case 2:
                    try
                    {
                        var reg_tmpl = _dbQldvMIFI.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvMIFI.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        tmpl_data.TmplCss = reg_tmpl[0].TmplCss;
                        tmpl_data.LogoCss = reg_tmpl[0].LogoCss;
                        tmpl_data.BgrCss = reg_tmpl[0].BgrCss;
                        tmpl_data.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        throw;
                    }
                    break;
                case 3:
                    try
                    {
                        var reg_tmpl = _dbQldvMB.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvMB.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        tmpl_data.TmplCss = reg_tmpl[0].TmplCss;
                        tmpl_data.LogoCss = reg_tmpl[0].LogoCss;
                        tmpl_data.BgrCss = reg_tmpl[0].BgrCss;
                        tmpl_data.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        throw;
                    }
                    break;
                case 0:
                    try
                    {
                        var reg_tmpl = _dbQldvTester.RegisterTemps
                            .Where(rt => rt.Id == PubTmplId)
                            .Select(s => new
                            {
                                TmplCss = s.CssData,
                                LogoCss = s.CssLogo,
                                BgrCss = s.CssBackgr,
                                IOTmplId = s.TempID
                            }).ToList();
                        var io_tmpl_id = reg_tmpl[0].IOTmplId;
                        var tmpl_html = _dbQldvTester.InvTemplates
                            .Where(it => it.Id == io_tmpl_id)
                            .Select(s => new
                            {
                                TmplHtml = s.XsltFile
                            }).ToList();

                        tmpl_data.TmplCss = reg_tmpl[0].TmplCss;
                        tmpl_data.LogoCss = reg_tmpl[0].LogoCss;
                        tmpl_data.BgrCss = reg_tmpl[0].BgrCss;
                        tmpl_data.TmplHtml = tmpl_html[0].TmplHtml;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/CompInvTmplView, dòng {0}: {1}", exc_line, exc.Message));
                        throw;
                    }
                    break;
            }

            string tmpl_str = JsonConvert.SerializeObject(tmpl_data);
            var inv_tmpl_html = CompInvTmplView(PubTmplId, tmpl_str, InvId == null ? "" : InvId, DbId);
            var pdf_cnvter = new HtmlToPdfConverter();
            var co_inv_orientation = new HomeController().GetCoConfigVal(CoId, "PageOrientation", DbId);

            pdf_cnvter.Size = PageSize.A4;
            pdf_cnvter.Zoom = 1.2F;
            pdf_cnvter.Margins = new PageMargins { Top = 0, Bottom = 0, Left = 2, Right = 0 };
            pdf_cnvter.CustomWkHtmlArgs = "--encoding UTF-8";
            if ("Landscape".Equals(co_inv_orientation)) pdf_cnvter.Orientation = PageOrientation.Landscape;

            byte[] pdf_file = pdf_cnvter.GeneratePdf(inv_tmpl_html);
            return File(pdf_file, "application/pdf");
        }

        public string UpdatePubTmpl(int PubTmplId, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var reg_tmpl = _dbQldvHD.RegisterTemps
                    .Where(rt => rt.Id == PubTmplId)
                    .Select(s => new
                    {
                        IOInvTmplId = s.TempID
                    }).ToList();
                var io_tmpl_id = reg_tmpl[0].IOInvTmplId;
                var reg_tmpl_to_update = _dbQldvHD.RegisterTemps.SingleOrDefault(rt => rt.Id == PubTmplId);
                var tmpl_to_update = _dbQldvHD.InvTemplates.SingleOrDefault(it => it.Id == io_tmpl_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "TmplCss":
                            reg_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "LogoCss":
                            reg_tmpl_to_update.CssLogo = data_val.ToString();
                            break;
                        case "BgrCss":
                            reg_tmpl_to_update.CssBackgr = data_val.ToString();
                            break;
                        case "TmplHtml":
                            tmpl_to_update.XsltFile = data_val.ToString();
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdatePubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var reg_tmpl = _dbQldvMIFI.RegisterTemps
                    .Where(rt => rt.Id == PubTmplId)
                    .Select(s => new
                    {
                        IOInvTmplId = s.TempID
                    }).ToList();
                var io_tmpl_id = reg_tmpl[0].IOInvTmplId;
                var reg_tmpl_to_update = _dbQldvMIFI.RegisterTemps.SingleOrDefault(rt => rt.Id == PubTmplId);
                var tmpl_to_update = _dbQldvMIFI.InvTemplates.SingleOrDefault(it => it.Id == io_tmpl_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "TmplCss":
                            reg_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "LogoCss":
                            reg_tmpl_to_update.CssLogo = data_val.ToString();
                            break;
                        case "BgrCss":
                            reg_tmpl_to_update.CssBackgr = data_val.ToString();
                            break;
                        case "TmplHtml":
                            tmpl_to_update.XsltFile = data_val.ToString();
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdatePubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var reg_tmpl = _dbQldvMB.RegisterTemps
                    .Where(rt => rt.Id == PubTmplId)
                    .Select(s => new
                    {
                        IOInvTmplId = s.TempID
                    }).ToList();
                var io_tmpl_id = reg_tmpl[0].IOInvTmplId;
                var reg_tmpl_to_update = _dbQldvMB.RegisterTemps.SingleOrDefault(rt => rt.Id == PubTmplId);
                var tmpl_to_update = _dbQldvMB.InvTemplates.SingleOrDefault(it => it.Id == io_tmpl_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "TmplCss":
                            reg_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "LogoCss":
                            reg_tmpl_to_update.CssLogo = data_val.ToString();
                            break;
                        case "BgrCss":
                            reg_tmpl_to_update.CssBackgr = data_val.ToString();
                            break;
                        case "TmplHtml":
                            tmpl_to_update.XsltFile = data_val.ToString();
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdatePubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var reg_tmpl = _dbQldvTester.RegisterTemps
                    .Where(rt => rt.Id == PubTmplId)
                    .Select(s => new
                    {
                        IOInvTmplId = s.TempID
                    }).ToList();
                var io_tmpl_id = reg_tmpl[0].IOInvTmplId;
                var reg_tmpl_to_update = _dbQldvTester.RegisterTemps.SingleOrDefault(rt => rt.Id == PubTmplId);
                var tmpl_to_update = _dbQldvTester.InvTemplates.SingleOrDefault(it => it.Id == io_tmpl_id);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "TmplCss":
                            reg_tmpl_to_update.CssData = data_val.ToString();
                            break;
                        case "LogoCss":
                            reg_tmpl_to_update.CssLogo = data_val.ToString();
                            break;
                        case "BgrCss":
                            reg_tmpl_to_update.CssBackgr = data_val.ToString();
                            break;
                        case "TmplHtml":
                            tmpl_to_update.XsltFile = data_val.ToString();
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdatePubInvTmpl, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetCoInvPubStEmail(int CoId, int DbId)
        {
            List<CoInvPubStDataModel> result = new List<CoInvPubStDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var co_inv_pub_st = _dbDataHD.SendEmails
                            .Where(se => se.GroupName == CoId.ToString())
                            .Select(s => new
                            {
                                EmailId = s.id,
                                SenderEmail = s.EmailFrom,
                                ReceiverEmail = s.Email,
                                EmailSubject = s.Subject,
                                EmailContent = s.Body,
                                SttId = s.Status.Value,
                                s.CreateDate,
                                SendDate = s.SendedDate
                            }).ToList();

                        foreach (var item in co_inv_pub_st)
                        {
                            CoInvPubStDataModel model_instance = new CoInvPubStDataModel();

                            model_instance.EmailId = item.EmailId;
                            model_instance.SenderEmail = item.SenderEmail;
                            model_instance.ReceiverEmail = item.ReceiverEmail == null ? Array.Empty<string>() : item.ReceiverEmail.Split(',');
                            model_instance.EmailSubject = item.EmailSubject;
                            model_instance.EmailContent = item.EmailContent;
                            model_instance.SttId = item.SttId;
                            model_instance.CreateDate = item.CreateDate.Value.ToString("dd.MM.yyyy");
                            model_instance.SendDate = item.SendDate == null ? item.CreateDate.Value.ToString("dd.MM.yyyy") : item.SendDate.Value.ToString("dd.MM.yyyy");
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var co_inv_pub_st = _dbDataMIFI.SendEmails
                            .Where(se => se.GroupName == CoId.ToString())
                            .Select(s => new
                            {
                                EmailId = s.id,
                                SenderEmail = s.EmailFrom,
                                ReceiverEmail = s.Email,
                                EmailSubject = s.Subject,
                                EmailContent = s.Body,
                                SttId = s.Status.Value,
                                s.CreateDate,
                                SendDate = s.SendedDate
                            }).ToList();

                        foreach (var item in co_inv_pub_st)
                        {
                            CoInvPubStDataModel model_instance = new CoInvPubStDataModel();

                            model_instance.EmailId = item.EmailId;
                            model_instance.SenderEmail = item.SenderEmail;
                            model_instance.ReceiverEmail = item.ReceiverEmail == null ? Array.Empty<string>() : item.ReceiverEmail.Split(',');
                            model_instance.EmailSubject = item.EmailSubject;
                            model_instance.EmailContent = item.EmailContent;
                            model_instance.SttId = item.SttId;
                            model_instance.CreateDate = item.CreateDate.Value.ToString("dd.MM.yyyy");
                            model_instance.SendDate = item.SendDate == null ? item.CreateDate.Value.ToString("dd.MM.yyyy") : item.SendDate.Value.ToString("dd.MM.yyyy");
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        var co_inv_pub_st = _dbDataMB.SendEmails
                            .Where(se => se.GroupName == CoId.ToString())
                            .Select(s => new
                            {
                                EmailId = s.id,
                                SenderEmail = s.EmailFrom,
                                ReceiverEmail = s.Email,
                                EmailSubject = s.Subject,
                                EmailContent = s.Body,
                                SttId = s.Status.Value,
                                s.CreateDate,
                                SendDate = s.SendedDate
                            }).ToList();

                        foreach (var item in co_inv_pub_st)
                        {
                            CoInvPubStDataModel model_instance = new CoInvPubStDataModel();

                            model_instance.EmailId = item.EmailId;
                            model_instance.SenderEmail = item.SenderEmail;
                            model_instance.ReceiverEmail = item.ReceiverEmail == null ? Array.Empty<string>() : item.ReceiverEmail.Split(',');
                            model_instance.EmailSubject = item.EmailSubject;
                            model_instance.EmailContent = item.EmailContent;
                            model_instance.SttId = item.SttId;
                            model_instance.CreateDate = item.CreateDate.Value.ToString("dd.MM.yyyy");
                            model_instance.SendDate = item.SendDate == null ? item.CreateDate.Value.ToString("dd.MM.yyyy") : item.SendDate.Value.ToString("dd.MM.yyyy");
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        var co_inv_pub_st = _dbDataTester.SendEmails
                            .Where(se => se.GroupName == CoId.ToString())
                            .Select(s => new
                            {
                                EmailId = s.id,
                                SenderEmail = s.EmailFrom,
                                ReceiverEmail = s.Email,
                                EmailSubject = s.Subject,
                                EmailContent = s.Body,
                                SttId = s.Status.Value,
                                s.CreateDate,
                                SendDate = s.SendedDate
                            }).ToList();

                        foreach (var item in co_inv_pub_st)
                        {
                            CoInvPubStDataModel model_instance = new CoInvPubStDataModel();

                            model_instance.EmailId = item.EmailId;
                            model_instance.SenderEmail = item.SenderEmail;
                            model_instance.ReceiverEmail = item.ReceiverEmail == null ? Array.Empty<string>() : item.ReceiverEmail.Split(',');
                            model_instance.EmailSubject = item.EmailSubject;
                            model_instance.EmailContent = item.EmailContent;
                            model_instance.SttId = item.SttId;
                            model_instance.CreateDate = item.CreateDate.Value.ToString("dd.MM.yyyy");
                            model_instance.SendDate = item.SendDate == null ? item.CreateDate.Value.ToString("dd.MM.yyyy") : item.SendDate.Value.ToString("dd.MM.yyyy");
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string UpdateCoInvPubStEmail(string EmailId, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var email_to_update = _dbDataHD.SendEmails.SingleOrDefault(se => se.id.ToString() == EmailId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "EmailSubject":
                            email_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_to_update.Body = data_val.ToString();
                            break;
                        case "SenderEmail":
                            email_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "ReceiverEmail":
                            email_to_update.Email = Utils.JoinStrArrayIntoStr(data_val.ToObject<string[]>());
                            break;
                        case "SttId":
                            email_to_update.Status = int.Parse(data_val.ToString());
                            break;
                    }
                }
                email_to_update.SendedDate = DateTime.Now;
                try
                {
                    _dbDataHD.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 2)
            {
                var email_to_update = _dbDataMIFI.SendEmails.SingleOrDefault(se => se.id.ToString() == EmailId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "EmailSubject":
                            email_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_to_update.Body = data_val.ToString();
                            break;
                        case "SenderEmail":
                            email_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "ReceiverEmail":
                            email_to_update.Email = Utils.JoinStrArrayIntoStr(data_val.ToObject<string[]>());
                            break;
                        case "SttId":
                            email_to_update.Status = int.Parse(data_val.ToString());
                            break;
                    }
                }
                email_to_update.SendedDate = DateTime.Now;
                try
                {
                    _dbDataMIFI.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else if (DbId == 3)
            {
                var email_to_update = _dbDataMB.SendEmails.SingleOrDefault(se => se.id.ToString() == EmailId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "EmailSubject":
                            email_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_to_update.Body = data_val.ToString();
                            break;
                        case "SenderEmail":
                            email_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "ReceiverEmail":
                            email_to_update.Email = Utils.JoinStrArrayIntoStr(data_val.ToObject<string[]>());
                            break;
                        case "SttId":
                            email_to_update.Status = int.Parse(data_val.ToString());
                            break;
                    }
                }
                email_to_update.SendedDate = DateTime.Now;
                try
                {
                    _dbDataMB.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            else
            {
                var email_to_update = _dbDataTester.SendEmails.SingleOrDefault(se => se.id.ToString() == EmailId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "EmailSubject":
                            email_to_update.Subject = data_val.ToString();
                            break;
                        case "EmailContent":
                            email_to_update.Body = data_val.ToString();
                            break;
                        case "SenderEmail":
                            email_to_update.EmailFrom = data_val.ToString();
                            break;
                        case "ReceiverEmail":
                            email_to_update.Email = Utils.JoinStrArrayIntoStr(data_val.ToObject<string[]>());
                            break;
                        case "SttId":
                            email_to_update.Status = int.Parse(data_val.ToString());
                            break;
                    }
                }
                email_to_update.SendedDate = DateTime.Now;
                try
                {
                    _dbDataTester.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoInvPubStEmail, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetCoDigiCert(int CoId, int DbId)
        {
            List<CoDigiCertDataModel> result = new List<CoDigiCertDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var co_digi_cert = _dbQldvHD.KeyStores
                            .Where(ks => ks.ComID == CoId)
                            .Select(s => new
                            {
                                s.Type,
                                s.Path,
                                Pwd = s.Password,
                                SlotIndx = s.SlotIndex,
                                CertSerial = s.SerialCert,
                                CertNo = s.KeyStoresOf
                            }).ToList();

                        foreach (var item in co_digi_cert)
                        {
                            CoDigiCertDataModel model_instance = new CoDigiCertDataModel();

                            model_instance.Type = int.Parse(item.Type.ToString());
                            model_instance.Path = item.Path;
                            model_instance.Pwd = item.Pwd;
                            model_instance.SlotIndx = item.SlotIndx.Value;
                            model_instance.CertSerial = item.CertSerial;
                            model_instance.CertNo = item.CertNo;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var co_digi_cert = _dbQldvMIFI.KeyStores
                            .Where(ks => ks.ComID == CoId)
                            .Select(s => new
                            {
                                s.Type,
                                s.Path,
                                Pwd = s.Password,
                                SlotIndx = s.SlotIndex,
                                CertSerial = s.SerialCert,
                                CertNo = s.KeyStoresOf
                            }).ToList();

                        foreach (var item in co_digi_cert)
                        {
                            CoDigiCertDataModel model_instance = new CoDigiCertDataModel();

                            model_instance.Type = int.Parse(item.Type.ToString());
                            model_instance.Path = item.Path;
                            model_instance.Pwd = item.Pwd;
                            model_instance.SlotIndx = item.SlotIndx.Value;
                            model_instance.CertSerial = item.CertSerial;
                            model_instance.CertNo = item.CertNo;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet); ;
                    }
                    break;
                case 3:
                    try
                    {
                        var co_digi_cert = _dbQldvMB.KeyStores
                            .Where(ks => ks.ComID == CoId)
                            .Select(s => new
                            {
                                s.Type,
                                s.Path,
                                Pwd = s.Password,
                                SlotIndx = s.SlotIndex,
                                CertSerial = s.SerialCert,
                                CertNo = s.KeyStoresOf
                            }).ToList();

                        foreach (var item in co_digi_cert)
                        {
                            CoDigiCertDataModel model_instance = new CoDigiCertDataModel();

                            model_instance.Type = int.Parse(item.Type.ToString());
                            model_instance.Path = item.Path;
                            model_instance.Pwd = item.Pwd;
                            model_instance.SlotIndx = item.SlotIndx.Value;
                            model_instance.CertSerial = item.CertSerial;
                            model_instance.CertNo = item.CertNo;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet); ;
                    }
                    break;
                case 0:
                    try
                    {
                        var co_digi_cert = _dbQldvTester.KeyStores
                            .Where(ks => ks.ComID == CoId)
                            .Select(s => new
                            {
                                s.Type,
                                s.Path,
                                Pwd = s.Password,
                                SlotIndx = s.SlotIndex,
                                CertSerial = s.SerialCert,
                                CertNo = s.KeyStoresOf
                            }).ToList();

                        foreach (var item in co_digi_cert)
                        {
                            CoDigiCertDataModel model_instance = new CoDigiCertDataModel();

                            model_instance.Type = int.Parse(item.Type.ToString());
                            model_instance.Path = item.Path;
                            model_instance.Pwd = item.Pwd;
                            model_instance.SlotIndx = item.SlotIndx.Value;
                            model_instance.CertSerial = item.CertSerial;
                            model_instance.CertNo = item.CertNo;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet); ;
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsCoDigiCert(int CoId, string UpdateData, int DbId)
        {
            string result = _resultError;
            KeyStore model_instance = new KeyStore();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Type":
                        model_instance.Type = int.Parse(data_val.ToString());
                        break;
                    case "Path":
                        model_instance.Path = data_val.ToString();
                        break;
                    case "Pwd":
                        model_instance.Password = data_val.ToString();
                        break;
                    case "SlotIndx":
                        model_instance.SlotIndex = int.Parse(data_val.ToString());
                        break;
                    case "CertSerial":
                        model_instance.SerialCert = data_val.ToString();
                        break;
                    case "CertNo":
                        model_instance.KeyStoresOf = int.Parse(data_val.ToString());
                        break;
                }
            }
            model_instance.ComID = CoId;
            if (model_instance.Type == null) model_instance.Type = 0;
            if (model_instance.SlotIndex == null) model_instance.SlotIndex = 0;
            switch (DbId)
            {
                case 1:
                    try
                    {
                        _dbQldvHD.KeyStores.Add(model_instance);
                        _dbQldvHD.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 2:
                    try
                    {
                        _dbQldvMIFI.KeyStores.Add(model_instance);
                        _dbQldvMIFI.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 3:
                    try
                    {
                        _dbQldvMB.KeyStores.Add(model_instance);
                        _dbQldvMB.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
                case 0:
                    try
                    {
                        _dbQldvTester.KeyStores.Add(model_instance);
                        _dbQldvTester.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoDigiCert, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                    break;
            }
            return result;
        }

        public JsonResult GetCoConfig(int CoId, int DbId)
        {
            List<CoConfigDataModel> result = new List<CoConfigDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var co_config = _dbQldvHD.Configs
                            .Where(c => c.ComID == CoId)
                            .Select(s => new
                            {
                                Id = s.ID,
                                s.Code,
                                s.Value
                            }).ToList();

                        foreach (var item in co_config)
                        {
                            CoConfigDataModel model_instance = new CoConfigDataModel();

                            model_instance.Id = item.Id;
                            model_instance.Code = item.Code;
                            model_instance.Value = item.Value;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var co_config = _dbQldvMIFI.Configs
                            .Where(c => c.ComID == CoId)
                            .Select(s => new
                            {
                                Id = s.ID,
                                s.Code,
                                s.Value
                            }).ToList();
                        foreach (var item in co_config)
                        {
                            CoConfigDataModel model_instance = new CoConfigDataModel();

                            model_instance.Id = item.Id;
                            model_instance.Code = item.Code;
                            model_instance.Value = item.Value;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet); ;
                    }
                    break;
                case 3:
                    try
                    {
                        var co_config = _dbQldvMB.Configs
                            .Where(c => c.ComID == CoId)
                            .Select(s => new
                            {
                                Id = s.ID,
                                s.Code,
                                s.Value
                            }).ToList();
                        foreach (var item in co_config)
                        {
                            CoConfigDataModel model_instance = new CoConfigDataModel();

                            model_instance.Id = item.Id;
                            model_instance.Code = item.Code;
                            model_instance.Value = item.Value;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet); ;
                    }
                    break;
                case 0:
                    try
                    {
                        var co_config = _dbQldvTester.Configs
                            .Where(c => c.ComID == CoId)
                            .Select(s => new
                            {
                                Id = s.ID,
                                s.Code,
                                s.Value
                            }).ToList();
                        foreach (var item in co_config)
                        {
                            CoConfigDataModel model_instance = new CoConfigDataModel();

                            model_instance.Id = item.Id;
                            model_instance.Code = item.Code;
                            model_instance.Value = item.Value;
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        FailReqModel model_instance = new FailReqModel();
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        model_instance.Msg = _resultError;
                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/GetCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(model_instance, JsonRequestBehavior.AllowGet); ;
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsCoConfig(int CoId, string UpdateData, int DbId)
        {
            string result = _resultError;
            Config model_instance = new Config();
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
                    case "Value":
                        model_instance.Value = data_val.ToString();
                        break;                    
                }
            }
            model_instance.ComID = CoId;
            switch (DbId)
            {
                case 1:
                    if (_dbQldvHD.Configs.Any(c => c.Code == model_instance.Code && c.ComID == CoId))
                    {
                        result = _resultWarning + "Khóa tính năng đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvHD.Configs.Add(model_instance);
                            _dbQldvHD.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 2:
                    if (_dbQldvMIFI.Configs.Any(c => c.Code == model_instance.Code && c.ComID == CoId))
                    {
                        result = _resultWarning + "Khóa tính năng đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvMIFI.Configs.Add(model_instance);
                            _dbQldvMIFI.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 3:
                    if (_dbQldvMB.Configs.Any(c => c.Code == model_instance.Code && c.ComID == CoId))
                    {
                        result = _resultWarning + "Khóa tính năng đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvMB.Configs.Add(model_instance);
                            _dbQldvMB.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
                case 0:
                    if (_dbQldvTester.Configs.Any(c => c.Code == model_instance.Code && c.ComID == CoId))
                    {
                        result = _resultWarning + "Khóa tính năng đã tồn tại.";
                        return result;
                    }
                    else
                    {
                        try
                        {
                            _dbQldvTester.Configs.Add(model_instance);
                            _dbQldvTester.SaveChanges();
                            result = _resultSuccess;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Operating/InsCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                    }
                    break;
            }
            return result;
        }

        public string UpdateCoConfig(int ConfigId, string UpdateData, int DbId)
        {
            string result = _resultError;

            if (DbId == 1)
            {
                var config_to_update = _dbQldvHD.Configs.SingleOrDefault(c => c.ID == ConfigId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Code":
                            config_to_update.Code = data_val.ToString();
                            break;
                        case "Value":
                            config_to_update.Value = data_val.ToString();
                            break;
                    }
                }
                var dupe_config = _dbQldvHD.Configs.Where(c => c.Code == config_to_update.Code).ToList();

                if (dupe_config.Count == 2)
                {
                    result = _resultWarning + "Khóa tính năng đã tồn tại.";
                    return result;
                }
                else
                {
                    try
                    {
                        _dbQldvHD.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                }
            }
            else if (DbId == 2)
            {
                var config_to_update = _dbQldvMIFI.Configs.SingleOrDefault(c => c.ID == ConfigId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Code":
                            config_to_update.Code = data_val.ToString();
                            break;
                        case "Value":
                            config_to_update.Value = data_val.ToString();
                            break;
                    }
                }
                var dupe_config = _dbQldvMIFI.Configs.Where(c => c.Code == config_to_update.Code).ToList();

                if (dupe_config.Count == 2)
                {
                    result = _resultWarning + "Khóa tính năng đã tồn tại.";
                    return result;
                }
                else
                {
                    try
                    {
                        _dbQldvMIFI.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                }
            }
            else if (DbId == 3)
            {
                var config_to_update = _dbQldvMB.Configs.SingleOrDefault(c => c.ID == ConfigId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Code":
                            config_to_update.Code = data_val.ToString();
                            break;
                        case "Value":
                            config_to_update.Value = data_val.ToString();
                            break;
                    }
                }
                var dupe_config = _dbQldvMB.Configs.Where(c => c.Code == config_to_update.Code).ToList();

                if (dupe_config.Count == 2)
                {
                    result = _resultWarning + "Khóa tính năng đã tồn tại.";
                    return result;
                }
                else
                {
                    try
                    {
                        _dbQldvMB.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                }
            }
            else
            {
                var config_to_update = _dbQldvTester.Configs.SingleOrDefault(c => c.ID == ConfigId);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "Code":
                            config_to_update.Code = data_val.ToString();
                            break;
                        case "Value":
                            config_to_update.Value = data_val.ToString();
                            break;
                    }
                }
                var dupe_config = _dbQldvTester.Configs.Where(c => c.Code == config_to_update.Code).ToList();

                if (dupe_config.Count == 2)
                {
                    result = _resultWarning + "Khóa tính năng đã tồn tại.";
                    return result;
                }
                else
                {
                    try
                    {
                        _dbQldvTester.SaveChanges();
                        result = _resultSuccess;
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Operating/UpdateCoConfig, dòng {0}: {1}", exc_line, exc.Message));
                        return result;
                    }
                }
            }
            return result;
        }

        public JsonResult GetAuthedUser()
        {
            string logged_user_name = System.Web.HttpContext.Current.Session["User"].ToString();
            List<UserDataModel> result = new List<UserDataModel>();

            try
            {
                if (logged_user_name == "beta")
                {
                    var user_array = _dbDhql.dhql_user
                        .Select(s => new
                        {
                            Id = s.id,
                            Name = s.name,
                            Role = s.role,
                            Funcs = s.functions,
                            CreateDate = s.create_date,
                            ModifyDate = s.modify_date,
                            Flag = s.flag
                        }).ToList();

                    foreach (var item in user_array)
                    {
                        UserDataModel model_instance = new UserDataModel();

                        model_instance.Id = item.Id;
                        model_instance.Name = item.Name;
                        model_instance.Role = item.Role;
                        model_instance.Funcs = item.Funcs.Split(',');
                        model_instance.CreateDate = item.CreateDate.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.ModifyDate = item.ModifyDate.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.Flag = item.Flag == false ? "false" : "true";
                        result.Add(model_instance);
                    }
                }
                else
                {
                    var logged_user_id = _dbDhql.dhql_user
                        .Where(u => u.name == logged_user_name)
                        .Select(s => new
                        {
                            Id = s.id
                        }).ToList();
                    var logged_user_rank = _dbDhql.dhql_role
                        .Where(r => r.user_id == logged_user_id[0].Id)
                        .Select(s => new
                        {
                            Rank = s.rank
                        }).ToList();
                    var role_array = _dbDhql.dhql_role
                        .Where(r => r.rank <= logged_user_rank[0].Rank)
                        .Select(s => new
                        {
                            UserId = s.user_id
                        }).ToList();
                    var user_array = _dbDhql.dhql_user
                        .Where(u => role_array.Any(r => r.UserId == u.id))
                        .Select(s => new
                        {
                            Id = s.id,
                            Name = s.name,
                            Role = s.role,
                            Funcs = s.functions,
                            CreateDate = s.create_date,
                            ModifyDate = s.modify_date,
                            Flag = s.flag
                        }).ToList();

                    foreach (var item in user_array)
                    {
                        UserDataModel model_instance = new UserDataModel();

                        model_instance.Id = item.Id;
                        model_instance.Name = item.Name;
                        model_instance.Role = item.Role;
                        model_instance.Funcs = item.Funcs.Split(',');
                        model_instance.CreateDate = item.CreateDate.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.ModifyDate = item.ModifyDate.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.Flag = item.Flag == false ? "false" : "true";
                        result.Add(model_instance);
                    }
                }
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAuthedUser, dòng {0}: {1}", exc_line, exc.Message));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleList()
        {
            List<RoleDataModel> result = new List<RoleDataModel>();
            bool req_auth_resp = new GeneralController().AuthReq("GetRoleList");

            if (req_auth_resp)
            {
                try
                {
                    var role_array = _dbDhql.dhql_role
                        .Select(s => new
                        {
                            Name = s.name
                        }).ToList();

                    foreach (var item in role_array)
                    {
                        RoleDataModel model_instance = new RoleDataModel();

                        model_instance.Name = item.Name;
                        result.Add(model_instance);
                    }
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Home/GetRoleList, dòng {0}: {1}", exc_line, exc.Message));
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsUser(string UpdateData)
        {
            string result = _resultError;
            dhql_user model_instance = new dhql_user();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Name":
                        model_instance.name = data_val.ToString();
                        break;
                    case "Role":
                        model_instance.role = data_val.ToString();
                        break;
                    case "Funcs":
                        model_instance.functions = Utils.JoinStrArrayIntoStr(data_val.ToObject<string[]>());
                        break;
                    case "Flag":
                        model_instance.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            model_instance.login_token = Utils.EncryptStr(model_instance.name + _initPwd);
            model_instance.create_date = DateTime.Now;
            model_instance.modify_date = DateTime.Now;
            if (model_instance.flag == null) model_instance.flag = false;
            if (_dbDhql.dhql_user.Any(u => u.name == model_instance.name))
            {
                result = _resultWarning + "Người dùng đã tồn tại.";
                return result;
            }
            else
            {
                try
                {
                    _dbDhql.dhql_user.Add(model_instance);
                    _dbDhql.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Home/InsUser, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }            
            return result;
        }

        public string UpdateUser(int Id, string UpdateData)
        {
            string result = _resultError;
            var user_to_update = _dbDhql.dhql_user.SingleOrDefault(u => u.id == Id);
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Role":
                        user_to_update.role = data_val.ToString();
                        break;
                    case "Funcs":
                        user_to_update.functions = Utils.JoinStrArrayIntoStr(data_val.ToObject<string[]>());
                        break;
                    case "Flag":
                        user_to_update.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            user_to_update.modify_date = DateTime.Now;
            try
            {
                _dbDhql.SaveChanges();
                result = _resultSuccess;
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] Home/UpdateUser, dòng {0}: {1}", exc_line, exc.Message));
                return result;
            }
            return result;
        }

        #region Odoo API
        private bool OdooApiLogin()
        {
            try
            {
                XmlRpcClient xml_rpc = new XmlRpcClient();

                xml_rpc.Url = _odooServiceURL;
                xml_rpc.Path = "common";
                xml_rpc.Timeout = 100000;

                XmlRpcRequest login_req = new XmlRpcRequest("authenticate");

                login_req.AddParams(_odooDb, _odooUser, _odooPwd, XmlRpcParameter.EmptyStruct());
                _loginResponse = xml_rpc.Execute(login_req);
                if (_loginResponse.IsFault())
                {
                    Logs.WriteToLogFile(string.Format("[LỖI] OdooLoginApi: {0}", _loginResponse.GetFaultString()));
                }
                else
                {
                    Logs.WriteToLogFile(string.Format("OdooLoginApi: {0}", _loginResponse.GetString()));
                    if (_loginResponse.GetInt() > 0)
                    return true;
                }
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] OdooLoginApi, dòng {0}: {1}", exc_line, exc.Message));
            }
            _loginResponse = null;

            return false;
        }

        private bool CheckLogin()
        {
            if (_loginResponse == null || _loginResponse.IsFault()) return false;
            return true;
        }

        private int GetLoginValue(XmlRpcResponse Resp)
        {
            if (Resp == null) return -1;
            return Resp.GetInt();
        }        

        private IList<OdooCusDataModel> GetOdooCus()
        {
            var key = "GetOdooCus";

            if (_objCache[key] != null)
            {
                return (IList<OdooCusDataModel>)_objCache[key];
            }
            else
            {
                string product_cat_code = "HDDT";
                string active_stt = "active";
                var odoo_resp = new OdooRespDataModel();
                string resp_result = string.Empty;

                try
                {
                    if (GetLoginValue(_loginResponse) <= 0)
                    {
                        var r = OdooApiLogin();

                        if (!r) return null;
                    }

                    XmlRpcClient xml_rpc = new XmlRpcClient();

                    xml_rpc.Url = _odooServiceURL;
                    xml_rpc.Path = "common";
                    xml_rpc.Timeout = 100000;
                    xml_rpc.Path = "object";

                    XmlRpcRequest req = new XmlRpcRequest("execute_kw");

                    req.AddParams(_odooDb, _loginResponse.GetInt(), _odooPwd, "sale.service", "get_service_datas", XmlRpcParameter.AsArray(product_cat_code, active_stt));

                    XmlRpcResponse req_resp = xml_rpc.Execute(req);

                    if (req_resp.IsFault())
                    {
                        resp_result = req_resp.GetFaultString();
                    }
                    else
                    {
                        resp_result = req_resp.GetString();
                        odoo_resp = JsonConvert.DeserializeObject<OdooRespDataModel>(resp_result);
                        if (odoo_resp == null || odoo_resp.code != 200 || odoo_resp.data.Count == 0) return null;
                        _objCache.Add(key, odoo_resp.data, null, DateTime.Now.AddMinutes(10), new TimeSpan(0, 0, 0), CacheItemPriority.Normal, null);
                        return odoo_resp.data;
                    }
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] GetOdooCustomer, dòng {0}: {1}", exc_line, exc.Message));
                    throw;
                }
                return null;
            }
        }

        private List<OdooCusSvcDataModel> GetOdooCusSvc(string Id, string Type = "HO", string ResellerCode = "", string ServiceCode = "HDDT")
        {
            List<OdooCusSvcDataModel> result = new List<OdooCusSvcDataModel>();

            try
            {
                if (GetLoginValue(_loginResponse) <= 0)
                {
                    var r = OdooApiLogin();

                    if (!r) return null;
                }

                XmlRpcClient xml_rpc = new XmlRpcClient();

                xml_rpc.Url = _odooServiceURL;
                xml_rpc.Path = "common";
                xml_rpc.Timeout = 100000;

                string req_result = string.Empty;

                if (!_loginResponse.IsFault())
                {
                    xml_rpc.Path = "object";

                    List<object> array_instance = XmlRpcParameter.AsArray();

                    if (ServiceCode.Trim() != "")
                    {
                        string[] svc_code_array = ServiceCode.Split(',');

                        for (int i = 0; i < svc_code_array.Length; i++)
                        {
                            array_instance.Add(svc_code_array[i]);
                        }
                    }

                    XmlRpcRequest req = new XmlRpcRequest("execute_kw");

                    req.AddParams(_odooDb, _loginResponse.GetInt(), _odooPwd, "external.create.service", "get_service_new", XmlRpcParameter.AsArray(Id, Type, Convert.ToInt32(_odooTo), ResellerCode, array_instance));

                    XmlRpcResponse req_resp = xml_rpc.Execute(req);

                    if (!req_resp.IsFault())
                    {
                        req_result = req_resp.GetString();

                        dynamic req_result_obj = JsonConvert.DeserializeObject(req_result);

                        if (req_result_obj != null && req_result_obj.code == 200)
                        {
                            result = JsonConvert.DeserializeObject<List<OdooCusSvcDataModel>>(req_result_obj.data.ToString());
                        }
                        else
                        {
                            Logs.WriteToLogFile(string.Format("[LỖI] GetOdooCusSvc: {0}", req_result));
                        }
                    }
                    else
                    {
                        req_result = req_resp.GetFaultString();                        
                        Logs.WriteToLogFile(string.Format("[LỖI] GetOdooCusSvc: {0}", req_result));
                    }
                }
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] GetOdooCusSvc, dòng {0}: {1}", exc_line, exc.Message));
            }
            return result;
        }
        #endregion
    }
}