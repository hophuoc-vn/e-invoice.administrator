using ManageHDDT.Helpers;
using ManageHDDT.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace ManageHDDT.Controllers
{
    public class HomeController : Controller
    {
        #region Khai báo hằng số
        hddt_dhql_Entities _dbDhql = new hddt_dhql_Entities();
        hddt_data_HD_Entities _dbDataHD = new hddt_data_HD_Entities();
        hddt_qldv_HD_Entities _dbQldvHD = new hddt_qldv_HD_Entities();
        hddt_data_MIFI_Entities _dbDataMIFI = new hddt_data_MIFI_Entities();
        hddt_qldv_MIFI_Entities _dbQldvMIFI = new hddt_qldv_MIFI_Entities();
        hddt_data_MB_Entities _dbDataMB = new hddt_data_MB_Entities();
        hddt_qldv_MB_Entities _dbQldvMB = new hddt_qldv_MB_Entities();
        hddt_data_tester_Entities _dbDataTester = new hddt_data_tester_Entities();
        hddt_qldv_tester_Entities _dbQldvTester = new hddt_qldv_tester_Entities();
        string _initUser = ConfigurationManager.AppSettings["InitUser"];
        string _initPwd = ConfigurationManager.AppSettings["InitPwd"];
        string _resultError = "2000:error:300:" + ConfigurationManager.AppSettings["ServerErrorMsg"];
        string _resultWarning = "1000:warning:250:";
        string _resultSuccess = "500:success:250:Cập nhật thành công.";
        #endregion

        // GET: Home
        public ViewResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Login");
        }

        [SessionCheck]
        public ViewResult SupTkts()
        {
            return View();
        }

        [SessionCheck]
        public ViewResult OpsAdjmt()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection LoginForm)
        {
            string username = LoginForm["username"];
            string pwd = LoginForm["pwd"];
            string user_token = Utils.EncryptStr(username + pwd);
            var user_array = _dbDhql.dhql_user.ToList();

            if (user_array.Count == 0)
            {
                if (username == _initUser && pwd == _initPwd)
                {
                    Session["User"] = "beta";
                    Session["Mater"] = "0";
                    return RedirectToAction("SupTkts", "Home");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                var logging_user = _dbDhql.dhql_user.SingleOrDefault(u => u.name == username);
                bool user_flag = logging_user.flag.Value;

                if (logging_user == null || user_flag)
                {
                    ViewBag.Message = "Tài khoản chưa tồn tại.";
                    return View();
                }
                else
                {
                    if (logging_user.login_token != user_token)
                    {
                        ViewBag.Message = "Mật khẩu không chính xác.";
                        return View();
                    }
                    else
                    {
                        Session["User"] = username;

                        var logging_user_role = _dbDhql.dhql_role.SingleOrDefault(r => r.user_id == logging_user.id);
                        var logging_user_rank = _dbDhql.dhql_rank.SingleOrDefault(r => r.value == logging_user_role.rank);

                        if (logging_user_rank.is_master == true)
                        {
                            Session["Mater"] = "1";
                        }
                        else
                        {
                            Session["Mater"] = "0";
                        }
                        return RedirectToAction("SupTkts", "Home");
                    }
                }
            }            
        }

        public JsonResult GetAllCo(int DbId)
        {
            List<CoIdDataModel> result = new List<CoIdDataModel>();

            switch (DbId)
            {
                case 1:
                    try
                    {
                        var co_array = _dbQldvHD.Companies
                            .Select(s => new
                            {
                                CoId = s.id,
                                CoName = s.Name,
                                CoDomain = s.Domain
                            }).ToList();

                        foreach (var item in co_array)
                        {
                            CoIdDataModel model_instance = new CoIdDataModel();

                            model_instance.CoId = item.CoId;
                            model_instance.CoName = item.CoName;
                            model_instance.CoDomain = item.CoDomain.Split(')')[0].Substring(1);
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAllCo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 2:
                    try
                    {
                        var co_array = _dbQldvMIFI.Companies
                            .Select(s => new
                            {
                                CoId = s.id,
                                CoName = s.Name,
                                CoDomain = s.Domain
                            }).ToList();
                        foreach (var item in co_array)
                        {
                            CoIdDataModel model_instance = new CoIdDataModel();
                            model_instance.CoId = item.CoId;
                            model_instance.CoName = item.CoName;
                            model_instance.CoDomain = item.CoDomain.Split(')')[0].Substring(1);
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAllCo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 3:
                    try
                    {
                        var co_array = _dbQldvMB.Companies
                            .Select(s => new
                            {
                                CoId = s.id,
                                CoName = s.Name,
                                CoDomain = s.Domain
                            }).ToList();
                        foreach (var item in co_array)
                        {
                            CoIdDataModel model_instance = new CoIdDataModel();
                            model_instance.CoId = item.CoId;
                            model_instance.CoName = item.CoName;
                            model_instance.CoDomain = item.CoDomain.Split(')')[0].Substring(1);
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAllCo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case 0:
                    try
                    {
                        var co_array = _dbQldvTester.Companies
                            .Select(s => new
                            {
                                CoId = s.id,
                                CoName = s.Name,
                                CoDomain = s.Domain
                            }).ToList();
                        foreach (var item in co_array)
                        {
                            CoIdDataModel model_instance = new CoIdDataModel();
                            model_instance.CoId = item.CoId;
                            model_instance.CoName = item.CoName;
                            model_instance.CoDomain = item.CoDomain.Split(')')[0].Substring(1);
                            result.Add(model_instance);
                        }
                    }
                    catch (Exception exc)
                    {
                        int exc_line = new GeneralController().GetExcLineNo(exc);

                        Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAllCo, dòng {0}: {1}", exc_line, exc.Message));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    break;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLoggedUserDetails()
        {
            string logged_user_name = System.Web.HttpContext.Current.Session["User"].ToString();
            UserDataModel result = new UserDataModel();

            if (logged_user_name != "beta")
            {
                try
                {
                    var user_details = _dbDhql.dhql_user.SingleOrDefault(u => u.name == logged_user_name);

                    result.LoginTkn = user_details.login_token;
                    result.EmailAddr = user_details.email_address;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Home/GetLoggedUserDetails, dòng {0}: {1}", exc_line, exc.Message));
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string UpdateLoggedUserDetails(string UpdateData)
        {
            string result = _resultError;
            string logged_user_name = System.Web.HttpContext.Current.Session["User"].ToString();

            if (logged_user_name != "beta")
            {
                var user_to_update = _dbDhql.dhql_user.SingleOrDefault(u => u.name == logged_user_name);
                JObject update_data = JObject.Parse(UpdateData);

                foreach (KeyValuePair<string, JToken> item in update_data)
                {
                    string data_key = item.Key;
                    JToken data_val = item.Value;

                    switch (data_key)
                    {
                        case "LoginTkn":
                            user_to_update.login_token = Utils.EncryptStr(logged_user_name + data_val.ToString());
                            break;
                        case "EmailAddr":
                            user_to_update.email_address = data_val.ToString();
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

                    Logs.WriteToLogFile(string.Format("[LỖI] Home/UpdateLoggedUserDetails, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public JsonResult GetAllUser()
        {
            List<UserDataModel> result = new List<UserDataModel>();

            try
            {
                var user_array = _dbDhql.dhql_user.ToList();

                foreach (var item in user_array)
                {
                    UserDataModel model_instance = new UserDataModel();

                    model_instance.Id = item.id;
                    model_instance.Name = item.name;
                    result.Add(model_instance);
                }
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] Home/GetUserList, dòng {0}: {1}", exc_line, exc.Message));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAuthedFunc()
        {
            string logged_user_name = System.Web.HttpContext.Current.Session["User"].ToString();
            List<FuncDataModel> result = new List<FuncDataModel>();

            try
            {
                if (logged_user_name == "beta")
                {
                    var func_array = _dbDhql.dhql_functional_authority
                        .Select(s => new
                        {
                            Card = s.card,
                            Id = s.id,
                            RootId = s.root_id,
                            Name = s.name,
                            Des = s.description,
                            CreateDate = s.create_date,
                            ModifyDate = s.modify_date,
                            Flag = s.flag
                        }).ToList();

                    foreach (var item in func_array)
                    {
                        FuncDataModel model_instance = new FuncDataModel();

                        model_instance.Card = item.Card;
                        model_instance.Id = item.Id.Value;
                        model_instance.RootId = item.RootId.Value;
                        model_instance.Name = item.Name;
                        model_instance.Des = item.Des;
                        model_instance.CreateDate = item.CreateDate.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.ModifyDate = item.ModifyDate.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.Flag = item.Flag == false ? "false" : "true";
                        result.Add(model_instance);
                    }
                }
                else
                {
                    var user_funcs = _dbDhql.dhql_user
                        .Where(u => u.name == logged_user_name)
                        .Select(s => new
                        {
                            UserFuncs = s.functions
                        }).ToList();
                    var user_funcs_array = user_funcs[0].UserFuncs.Split(',');
                    var func_array = _dbDhql.dhql_functional_authority
                        .Where(fa => user_funcs_array.Contains(fa.id.ToString()))
                        .Select(s => new
                        {
                            Card = s.card,
                            Id = s.id,
                            RootId = s.root_id,
                            Name = s.name,
                            Des = s.description,
                            CreateDate = s.create_date,
                            ModifyDate = s.modify_date,
                            Flag = s.flag
                        }).ToList();

                    foreach (var item in func_array)
                    {
                        FuncDataModel model_instance = new FuncDataModel();

                        model_instance.Card = item.Card;
                        model_instance.Id = item.Id.Value;
                        model_instance.RootId = item.RootId.Value;
                        model_instance.Name = item.Name;
                        model_instance.Des = item.Des;
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

                Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAuthedFunc, dòng {0}: {1}", exc_line, exc.Message));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllRole()
        {
            List<RoleDataModel> result = new List<RoleDataModel>();
            bool req_auth_resp = new GeneralController().AuthReq("GetAllCo");

            if (req_auth_resp)
            {
                try
                {
                    var role_array = _dbDhql.dhql_role.ToList();

                    foreach (var item in role_array)
                    {
                        RoleDataModel model_instance = new RoleDataModel();

                        model_instance.Id = item.id;
                        model_instance.Name = item.name;
                        model_instance.Rank = item.rank.Value;
                        model_instance.CreateDate = item.create_date.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.ModifyDate = item.modify_date.Value.ToString("dd.MM.yyyy HH:mm:ss");
                        model_instance.Flag = item.flag == false ? "false" : "true";
                        result.Add(model_instance);
                    }
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAllRole, dòng {0}: {1}", exc_line, exc.Message));
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsRole(string UpdateData)
        {
            string result = _resultError;
            dhql_role model_instance = new dhql_role();
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
                    case "Rank":
                        model_instance.rank = short.Parse(data_val.ToString());
                        break;
                    case "Flag":
                        model_instance.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            if (model_instance.flag == null) model_instance.flag = false;
            model_instance.create_date = DateTime.Now;
            model_instance.modify_date = DateTime.Now;
            try
            {
                _dbDhql.dhql_role.Add(model_instance);
                _dbDhql.SaveChanges();
                result = _resultSuccess;
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] Home/InsRole, dòng {0}: {1}", exc_line, exc.Message));
                return result;
            }
            return result;
        }

        public string UpdateRole(int Id, string UpdateData)
        {
            string result = _resultError;
            var role_to_update = _dbDhql.dhql_role.SingleOrDefault(r => r.id == Id);
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Name":
                        role_to_update.name = data_val.ToString();
                        break;
                    case "Rank":
                        role_to_update.rank = short.Parse(data_val.ToString());
                        break;
                    case "Flag":
                        role_to_update.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            role_to_update.modify_date = DateTime.Now;
            try
            {
                _dbDhql.SaveChanges();
                result = _resultSuccess;
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] Home/UpdateRole, dòng {0}: {1}", exc_line, exc.Message));
                return result;
            }
            return result;
        }

        public JsonResult GetAllRank()
        {
            List<RankDataModel> result = new List<RankDataModel>();

            try
            {
                var rank_array = _dbDhql.dhql_rank.ToList();

                foreach (var item in rank_array)
                {
                    RankDataModel model_instance = new RankDataModel();

                    model_instance.Id = item.id;
                    model_instance.Value = item.value.Value;
                    model_instance.Name = item.name;
                    model_instance.IsMaster = item.is_master.Value;
                    model_instance.CreateDate = item.create_date.Value.ToString("dd.MM.yyyy HH:mm:ss");
                    model_instance.ModifyDate = item.modify_date.Value.ToString("dd.MM.yyyy HH:mm:ss");
                    model_instance.Flag = item.flag == false ? "false" : "true";
                    result.Add(model_instance);
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAllRank, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsRank(string UpdateData)
        {
            string result = _resultError;
            dhql_rank model_instance = new dhql_rank();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Value":
                        model_instance.value = int.Parse(data_val.ToString());
                        break;
                    case "Name":
                        model_instance.name = data_val.ToString();
                        break;
                    case "IsMaster":
                        model_instance.is_master = bool.Parse(data_val.ToString());
                        break;
                    case "Flag":
                        model_instance.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            if (model_instance.is_master == null) model_instance.is_master = false;
            if (model_instance.flag == null) model_instance.flag = false;
            model_instance.create_date = DateTime.Now;
            model_instance.modify_date = DateTime.Now;
            try
            {
                _dbDhql.dhql_rank.Add(model_instance);
                _dbDhql.SaveChanges();
                result = _resultSuccess;
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] Home/InsRank, dòng {0}: {1}", exc_line, exc.Message));
                return result;
            }
            return result;
        }

        public string UpdateRank(int Id, string UpdateData)
        {
            string result = _resultError;
            var rank_to_update = _dbDhql.dhql_rank.SingleOrDefault(r => r.id == Id);
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Value":
                        rank_to_update.value = int.Parse(data_val.ToString());
                        break;
                    case "Name":
                        rank_to_update.name = data_val.ToString();
                        break;
                    case "IsMaster":
                        rank_to_update.is_master = bool.Parse(data_val.ToString());
                        break;
                    case "Flag":
                        rank_to_update.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            rank_to_update.modify_date = DateTime.Now;
            try
            {
                _dbDhql.SaveChanges();
                result = _resultSuccess;
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] Home/UpdateRank, dòng {0}: {1}", exc_line, exc.Message));
                return result;
            }
            return result;
        }

        public JsonResult GetAllFunc()
        {
            List<FuncDataModel> result = new List<FuncDataModel>();

            try
            {
                var func_array = _dbDhql.dhql_functional_authority.OrderBy(fa => fa.root_id).ThenBy(fa => fa.id).ToList();

                foreach (var item in func_array)
                {
                    FuncDataModel model_instance = new FuncDataModel();

                    model_instance.Card = item.card;
                    model_instance.Id = item.id.Value;
                    model_instance.RootId = item.root_id.Value;
                    model_instance.Name = item.name;
                    model_instance.Des = item.description;
                    model_instance.IsAuthed = item.is_authorized.Value;
                    model_instance.CreateDate = item.create_date.Value.ToString("dd.MM.yyyy HH:mm:ss");
                    model_instance.ModifyDate = item.modify_date.Value.ToString("dd.MM.yyyy HH:mm:ss");
                    model_instance.Flag = item.flag == false ? "false" : "true";
                    result.Add(model_instance);
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Home/GetAllFunc, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string InsFunc(string UpdateData)
        {
            string result = _resultError;
            dhql_functional_authority model_instance = new dhql_functional_authority();
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Id":
                        model_instance.id = int.Parse(data_val.ToString());
                        break;
                    case "RootId":
                        model_instance.root_id = int.Parse(data_val.ToString());
                        break;
                    case "Name":
                        model_instance.name = data_val.ToString();
                        break;
                    case "Des":
                        model_instance.description = data_val.ToString();
                        break;
                    case "IsAuthed":
                        model_instance.is_authorized = bool.Parse(data_val.ToString());
                        break;
                    case "Flag":
                        model_instance.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            if (model_instance.root_id == null) model_instance.root_id = 0;
            if (model_instance.is_authorized == null) model_instance.is_authorized = false;
            if (model_instance.flag == null) model_instance.flag = false;
            model_instance.create_date = DateTime.Now;
            model_instance.modify_date = DateTime.Now;
            if (_dbDhql.dhql_functional_authority.Any(fa => fa.id == model_instance.id))
            {
                result = _resultWarning + "Chức năng đã tồn tại.";
                return result;
            }
            else
            {
                try
                {
                    _dbDhql.dhql_functional_authority.Add(model_instance);
                    _dbDhql.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Home/InsFunc, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }            
            return result;
        }

        public string UpdateFunc(int Id, string UpdateData)
        {
            string result = _resultError;
            var func_to_update = _dbDhql.dhql_functional_authority.SingleOrDefault(fa => fa.id == Id);
            JObject update_data = JObject.Parse(UpdateData);

            foreach (KeyValuePair<string, JToken> item in update_data)
            {
                string data_key = item.Key;
                JToken data_val = item.Value;

                switch (data_key)
                {
                    case "Id":
                        func_to_update.id = int.Parse(data_val.ToString());                    
                        break;
                    case "RootId":
                        func_to_update.root_id = int.Parse(data_val.ToString() == "" ? "0" : data_val.ToString());
                        break;
                    case "Name":
                        func_to_update.name = data_val.ToString();
                        break;
                    case "Des":
                        func_to_update.description = data_val.ToString();
                        break;
                    case "IsAuthed":
                        func_to_update.is_authorized = bool.Parse(data_val.ToString());
                        break;
                    case "Flag":
                        func_to_update.flag = bool.Parse(data_val.ToString());
                        break;
                }
            }
            func_to_update.modify_date = DateTime.Now;

            var dupe_func = _dbDhql.dhql_functional_authority.Where(fa => fa.id == func_to_update.id).ToList();

            if (dupe_func.Count == 2)
            {
                result = _resultWarning + "Chức năng bị trùng lặp.";
                return result;
            }
            else
            {
                try
                {
                    _dbDhql.SaveChanges();
                    result = _resultSuccess;
                }
                catch (Exception exc)
                {
                    int exc_line = new GeneralController().GetExcLineNo(exc);

                    Logs.WriteToLogFile(string.Format("[LỖI] Home/UpdateFunc, dòng {0}: {1}", exc_line, exc.Message));
                    return result;
                }
            }
            return result;
        }

        public string GetCoConfigVal(int CoId, string ConfigKey, int DbId)
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
                            var co_config = _dbQldvHD.Configs.Where(c => c.ComID == CoId && c.Code == ConfigKey).ToList();

                            result = co_config.Count == 0 ? "" : co_config[0].Value;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Home/GetCoConfigVal, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                    case 2:
                        try
                        {
                            var co_config = _dbQldvMIFI.Configs.Where(c => c.ComID == CoId && c.Code == ConfigKey).ToList();

                            result = co_config.Count == 0 ? "" : co_config[0].Value;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Home/GetCoConfigVal, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                    case 3:
                        try
                        {
                            var co_config = _dbQldvMB.Configs.Where(c => c.ComID == CoId && c.Code == ConfigKey).ToList();

                            result = co_config.Count == 0 ? "" : co_config[0].Value;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Home/GetCoConfigVal, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                    case 0:
                        try
                        {
                            var co_config = _dbQldvTester.Configs.Where(c => c.ComID == CoId && c.Code == ConfigKey).ToList();

                            result = co_config.Count == 0 ? "" : co_config[0].Value;
                        }
                        catch (Exception exc)
                        {
                            int exc_line = new GeneralController().GetExcLineNo(exc);

                            Logs.WriteToLogFile(string.Format("[LỖI] Home/GetCoConfigVal, dòng {0}: {1}", exc_line, exc.Message));
                            return result;
                        }
                        break;
                }
            }
            return result;
        }
    }
}