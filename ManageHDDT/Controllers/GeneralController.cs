using ManageHDDT.Helpers;
using ManageHDDT.Models;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageHDDT.Controllers
{    
    public class GeneralController : Controller
    {
        #region Khai báo hằng số
        hddt_dhql_Entities _dbDhql = new hddt_dhql_Entities();
        string _failReqMsg = "2000:warning:350:" + ConfigurationManager.AppSettings["FailReqMsg"];
        string _sessTimeoutMsg = "2500:warning:375:" + ConfigurationManager.AppSettings["SessTimeoutMsg"];
        #endregion

        // GET: General
        public bool AuthReq(string ActionName)
        {
            bool result = false;

            try
            {
                var req_action = _dbDhql.dhql_functional_authority.SingleOrDefault(fa => fa.name == ActionName);

                if (req_action == null || req_action.is_authorized == false) result = true;
                else
                {
                    string logged_user_name = System.Web.HttpContext.Current.Session["User"].ToString();

                    if (logged_user_name == "beta") result = true;
                    else
                    {
                        var logged_user = _dbDhql.dhql_user
                            .Where(u => u.name == logged_user_name)
                            .Select(s => new
                            {
                                UserFuncs = s.functions
                            }).ToList();
                        var logged_user_funcs_array = logged_user[0].UserFuncs.Split(',');

                        if (logged_user_funcs_array.Contains(req_action.id.ToString())) result = true;
                    }
                }
            }
            catch (Exception exc)
            {
                int exc_line = new GeneralController().GetExcLineNo(exc);

                Logs.WriteToLogFile(string.Format("[LỖI] General/AuthReq, dòng {0}: {1}", exc_line, exc.Message));
                return result;
            }
            return result;
        }

        public bool IsTimeout()
        {
            bool result = false;

            if (Session["User"] == null) result = true;
            return result;
        }

        [HttpGet]
        public ActionResult SendGETDataReq(string ActionName, string ParmOne = "", string ParmTwo = "", string ParmThree = "")
        {
            JsonResult data_result = new JsonResult();

            if (IsTimeout())
            {
                FailReqModel model_instance = new FailReqModel();

                model_instance.Msg = _sessTimeoutMsg;
                data_result = Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (AuthReq(ActionName))
                {
                    switch (ActionName)
                    {
                        case "GetCusEntryQty":
                            data_result = new StatisticsController().GetCusEntryQty(int.Parse(ParmOne));
                            break;
                        case "GetEntryListByDateRange":
                            data_result = new StatisticsController().GetEntryListByDateRange(ParmOne, ParmTwo);
                            break;
                        case "GetCoQtyBySvcStt":
                            data_result = new StatisticsController().GetCoQtyBySvcStt();
                            break;
                        case "GetCoListBySvcStt":
                            data_result = new StatisticsController().GetCoListBySvcStt(ParmOne);
                            break;
                        case "GetInvUsageByRecentDays":
                            data_result = new StatisticsController().GetInvUsageByRecentDays(int.Parse(ParmOne));
                            break;
                        case "GetExpiringCoList":
                            data_result = new StatisticsController().GetExpiringCoList();
                            break;
                        case "GetCoQtyBySvcPkg":
                            data_result = new StatisticsController().GetCoQtyBySvcPkg();
                            break;
                        case "GetCoListBySvcPkg":
                            data_result = new StatisticsController().GetCoListBySvcPkg(ParmOne);
                            break;
                        case "GetCoListByCusRef":
                            data_result = new OperatingController().GetCoListByCusRef(ParmOne, int.Parse(ParmTwo));
                            break;
                        case "GetCoInfo":
                            data_result = new OperatingController().GetCoInfo(int.Parse(ParmOne), int.Parse(ParmTwo));
                            break;
                        case "GetCoPub":
                            data_result = new OperatingController().GetCoPub(int.Parse(ParmOne), int.Parse(ParmTwo));
                            break;
                        case "GetPubTmpl":
                            data_result = new OperatingController().GetPubTmpl(int.Parse(ParmOne), int.Parse(ParmTwo));
                            break;
                        case "GetCoInvList":
                            data_result = new OperatingController().GetCoInvList(int.Parse(ParmOne), int.Parse(ParmTwo), int.Parse(ParmThree));
                            break;
                        case "GetCoInvPubStEmail":
                            data_result = new OperatingController().GetCoInvPubStEmail(int.Parse(ParmOne), int.Parse(ParmTwo));
                            break;
                        case "GetCoDigiCert":
                            data_result = new OperatingController().GetCoDigiCert(int.Parse(ParmOne), int.Parse(ParmTwo));
                            break;
                        case "GetCoConfig":
                            data_result = new OperatingController().GetCoConfig(int.Parse(ParmOne), int.Parse(ParmTwo));
                            break;
                        case "GetAuthedUser":
                            data_result = new OperatingController().GetAuthedUser();
                            break;
                        case "GetRoleList":
                            data_result = new OperatingController().GetRoleList();
                            break;
                        case "GetAllCo":
                            data_result = new HomeController().GetAllCo(int.Parse(ParmOne));
                            break;
                        case "GetLoggedUserDetails":
                            data_result = new HomeController().GetLoggedUserDetails();
                            break;
                        case "GetAllUser":
                            data_result = new HomeController().GetAllUser();
                            break;
                        case "GetAuthedFunc":
                            data_result = new HomeController().GetAuthedFunc();
                            break;
                        case "GetAllRole":
                            data_result = new HomeController().GetAllRole();
                            break;
                        case "GetAllRank":
                            data_result = new HomeController().GetAllRank();
                            break;
                        case "GetAllFunc":
                            data_result = new HomeController().GetAllFunc();
                            break;
                        case "GetEmailTmplList":
                            data_result = new FeatureController().GetEmailTmplList(int.Parse(ParmOne), int.Parse(ParmTwo), int.Parse(ParmThree));
                            break;
                        case "GetInvTmplList":
                            data_result = new FeatureController().GetInvTmplList(int.Parse(ParmOne), int.Parse(ParmTwo), int.Parse(ParmThree));
                            break;
                        case "GetTaxAuthList":
                            data_result = new FeatureController().GetTaxAuthList(int.Parse(ParmOne), int.Parse(ParmTwo), int.Parse(ParmThree));
                            break;
                        case "GetUnitList":
                            data_result = new FeatureController().GetUnitList(int.Parse(ParmOne), int.Parse(ParmTwo), int.Parse(ParmThree));
                            break;
                        case "GetNotifList":
                            data_result = new FeatureController().GetNotifList(int.Parse(ParmOne), int.Parse(ParmTwo), int.Parse(ParmThree));
                            break;
                        case "GetInvCatList":
                            data_result = new FeatureController().GetInvCatList(int.Parse(ParmOne), int.Parse(ParmTwo), int.Parse(ParmThree));
                            break;
                    }
                }
                else
                {
                    FailReqModel model_instance = new FailReqModel();

                    model_instance.Msg = _failReqMsg;
                    data_result = Json(model_instance, JsonRequestBehavior.AllowGet);
                }
            }
            var result = Json(data_result.Data, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }

        [HttpPost]
        public string SendPOSTDataReq(string ActionName, string ParmOne = "", string ParmTwo = "", string ParmThree = "")
        {
            string result = string.Empty;

            if (IsTimeout()) result = _sessTimeoutMsg;
            else
            {
                if (AuthReq(ActionName))
                {
                    switch (ActionName)
                    {
                        case "UpdateCoInfo":
                            result = new OperatingController().UpdateCoInfo(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "UpdateCoPub":
                            result = new OperatingController().UpdateCoPub(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsCoDigiCert":
                            result = new OperatingController().InsCoDigiCert(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsCoConfig":
                            result = new OperatingController().InsCoConfig(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "UpdateCoConfig":
                            result = new OperatingController().UpdateCoConfig(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsUser":
                            result = new OperatingController().InsUser(ParmOne);
                            break;
                        case "UpdateUser":
                            result = new OperatingController().UpdateUser(int.Parse(ParmOne), ParmTwo);
                            break;
                        case "InsRole":
                            result = new HomeController().InsRole(ParmOne);
                            break;
                        case "UpdateRole":
                            result = new HomeController().UpdateRole(int.Parse(ParmOne), ParmTwo);
                            break;
                        case "InsRank":
                            result = new HomeController().InsRank(ParmOne);
                            break;
                        case "UpdateRank":
                            result = new HomeController().UpdateRank(int.Parse(ParmOne), ParmTwo);
                            break;
                        case "InsFunc":
                            result = new HomeController().InsFunc(ParmOne);
                            break;
                        case "UpdateFunc":
                            result = new HomeController().UpdateFunc(int.Parse(ParmOne), ParmTwo);
                            break;
                        case "InsTaxAuth":
                            result = new FeatureController().InsTaxAuth(ParmOne, int.Parse(ParmTwo));
                            break;
                        case "UpdateTaxAuth":
                            result = new FeatureController().UpdateTaxAuth(ParmOne, ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsUnit":
                            result = new FeatureController().InsUnit(ParmOne, int.Parse(ParmTwo));
                            break;
                        case "UpdateUnit":
                            result = new FeatureController().UpdateUnit(ParmOne, ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsNotif":
                            result = new FeatureController().InsNotif(ParmOne, int.Parse(ParmTwo));
                            break;
                        case "UpdateNotif":
                            result = new FeatureController().UpdateNotif(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsInvCat":
                            result = new FeatureController().InsInvCat(ParmOne, int.Parse(ParmTwo));
                            break;
                        case "UpdateInvCat":
                            result = new FeatureController().UpdateInvCat(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                    }
                }
                else result = _failReqMsg;
            }
            return result;
        }

        [HttpPost, ValidateInput(false)]
        public string SendAuthedPOSTDataReq(string ActionName, string ParmOne = "", string ParmTwo = "", string ParmThree = "", string ParmFour = "")
        {
            string result = string.Empty;

            if (IsTimeout()) result = _sessTimeoutMsg;
            else
            {
                if (AuthReq(ActionName))
                {
                    switch (ActionName)
                    {
                        case "UpdatePubTmpl":
                            result = new OperatingController().UpdatePubTmpl(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "CompInvTmplView":
                            result = new OperatingController().CompInvTmplView(int.Parse(ParmOne), ParmTwo, ParmThree, int.Parse(ParmFour));
                            break;
                        case "UpdateCoInvPubStEmail":
                            result = new OperatingController().UpdateCoInvPubStEmail(ParmOne, ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsEmailTmpl":
                            result = new FeatureController().InsEmailTmpl(ParmOne, int.Parse(ParmTwo));
                            break;
                        case "UpdateEmailTmpl":
                            result = new FeatureController().UpdateEmailTmpl(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                        case "InsInvTmpl":
                            result = new FeatureController().InsInvTmpl(ParmOne, int.Parse(ParmTwo));
                            break;
                        case "UpdateInvTmpl":
                            result = new FeatureController().UpdateInvTmpl(int.Parse(ParmOne), ParmTwo, int.Parse(ParmThree));
                            break;
                    }
                }
                else result = _failReqMsg;
            }
            return result;
        }

        public int GetExcLineNo(Exception exc)
        {
            var stack_trace = new StackTrace(exc, true);
            var frame = stack_trace.GetFrame(stack_trace.FrameCount - 1);
            var line_no = frame.GetFileLineNumber();
            return line_no;
        }
    }

    public class SessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;

            if (session != null && session["User"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "Controller", "Home" },
                        { "Action", "Login" }
                    });
            }
        }
    }    
}