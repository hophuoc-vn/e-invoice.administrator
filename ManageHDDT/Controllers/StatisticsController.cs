using ManageHDDT.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Configuration;
using System;
using System.Data.Entity;
using System.Globalization;
using ManageHDDT.Helpers;

namespace ManageHDDT.Controllers
{
    public class StatisticsController : Controller
    {
        #region Khai báo hằng số
        hddt_data_HD_Entities _dbDataHD = new hddt_data_HD_Entities();
        hddt_qldv_HD_Entities _dbQldvHD = new hddt_qldv_HD_Entities();
        hddt_data_MIFI_Entities _dbDataMIFI = new hddt_data_MIFI_Entities();
        hddt_qldv_MIFI_Entities _dbQldvMIFI = new hddt_qldv_MIFI_Entities();
        string _activeSvcPkg = ConfigurationManager.AppSettings["ActiveSvcPkg"].ToString();
        string _resultError = "2000:error:300:" + ConfigurationManager.AppSettings["ServerErrorMsg"];
        #endregion

        // GET: Statistics
        [SessionCheck]
        public ActionResult CusQty()
        {
            bool req_auth_resp = new GeneralController().AuthReq("CusQty");
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
        public ActionResult InvUsage()
        {
            bool req_auth_resp = new GeneralController().AuthReq("InvUsage");
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
        public ActionResult SvcPkg()
        {
            bool req_auth_resp = new GeneralController().AuthReq("SvcPkg");
            if (req_auth_resp)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SupTkts", "Home");
            }
        }

        public JsonResult GetCusEntryQty(int Year)
        {
            List<CusEntryQtyModel> result = new List<CusEntryQtyModel>();

            try
            {
                var MB_cus_entry_by_month = _dbDataHD.userdatas
                    .Where(u => u.CreateDate.Year == Year && u.username == "admin")
                    .GroupBy(u => u.CreateDate.Month)
                    .Select(s => new
                    {
                        Month = s.Key,
                        CusEntryCount = s.Select(u => u.userid).Count()
                    }).ToList();
                var MB_cus_entry_by_month_week = _dbDataHD.userdatas
                    .Where(u => u.CreateDate.Year == Year && u.username == "admin")
                    .GroupBy(u => new
                    {
                        u.CreateDate.Month,
                        Week = u.CreateDate.Day / 7 + 1
                    })
                    .Select(s => new
                    {
                        s.Key.Month,
                        s.Key.Week,
                        CusEntryCount = s.Select(u => u.userid).Count()
                    }).ToList();
                var MIFI_co_array = _dbQldvMIFI.Companies
                    .Select(s => new
                    {
                        CoId = s.id.ToString(),
                        CoName = s.Name
                    }).ToList();
                var new_co_list = MIFI_co_array
                    .Where(mf_co => !_dbQldvHD.Companies.Any(mb_co => mb_co.Name == mf_co.CoName))
                    .Select(s => new
                    {
                        s.CoId
                    }).ToList();
                var MIFI_cus_list = _dbDataMIFI.userdatas
                    .Where(u => u.CreateDate.Year == Year && u.username == "admin")
                    .Select(s => new
                    {
                        CoId = s.GroupName,
                        s.CreateDate
                    }).ToList();
                var MIFI_new_cus_list = MIFI_cus_list
                    .AsEnumerable()
                    .Join(new_co_list, MIFI_cus => MIFI_cus.CoId, new_co => new_co.CoId, (MIFI_cus, new_co) => new
                    {
                        MIFI_cus.CoId,
                        MIFI_cus.CreateDate
                    }).ToList();
                var MIFI_cus_entry_by_month = MIFI_new_cus_list
                    .Where(new_cus => new_cus.CreateDate.Year == Year)
                    .GroupBy(new_cus => new_cus.CreateDate.Month)
                    .Select(s => new
                    {
                        Month = s.Key,
                        CusEntryCount = s.Select(new_cus => new_cus.CoId).Count()
                    }).ToList();
                var MIFI_cus_entry_by_month_week = MIFI_new_cus_list
                    .Where(new_cus => new_cus.CreateDate.Year == Year)
                    .GroupBy(new_cus => new
                    {
                        new_cus.CreateDate.Month,
                        Week = new_cus.CreateDate.Day / 7 + 1
                    })
                    .Select(s => new
                    {
                        s.Key.Month,
                        s.Key.Week,
                        CusEntryCount = s.Select(new_cus => new_cus.CoId).Count()
                    }).ToList();

                CusEntryQtyModel model_instance;

                for (int i = 1; i < 13; i++)
                {
                    model_instance = new CusEntryQtyModel();
                    model_instance.arg = "Tháng " + i;
                    for (int i1 = 0; i1 < MB_cus_entry_by_month.Count; i1++)
                    {
                        if (MB_cus_entry_by_month[i1].Month == i)
                        {
                            model_instance.val = MB_cus_entry_by_month[i1].CusEntryCount;
                        }
                    }
                    for (int i1 = 0; i1 < MIFI_cus_entry_by_month.Count; i1++)
                    {
                        if (MIFI_cus_entry_by_month[i1].Month == i)
                        {
                            model_instance.val += MIFI_cus_entry_by_month[i1].CusEntryCount;
                        }
                    }
                    model_instance.parentID = "";
                    result.Add(model_instance);
                }
                for (int i = 1; i < 13; i++)
                {
                    for (int i1 = 1; i1 < 6; i1++)
                    {
                        model_instance = new CusEntryQtyModel();
                        model_instance.arg = "Tuần " + i1;
                        for (int i2 = 0; i2 < MB_cus_entry_by_month_week.Count; i2++)
                        {
                            if (MB_cus_entry_by_month_week[i2].Month == i && MB_cus_entry_by_month_week[i2].Week == i1)
                            {
                                model_instance.val = MB_cus_entry_by_month_week[i2].CusEntryCount;
                            }
                        }
                        for (int i2 = 0; i2 < MIFI_cus_entry_by_month_week.Count; i2++)
                        {
                            if (MIFI_cus_entry_by_month_week[i2].Month == i && MIFI_cus_entry_by_month_week[i2].Week == i1)
                            {
                                model_instance.val += MIFI_cus_entry_by_month_week[i2].CusEntryCount;
                            }
                        }
                        model_instance.parentID = "Tháng " + i;
                        result.Add(model_instance);
                    }
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetCusEntryQty, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEntryListByDateRange(string StartDate, string EndDate)
        {
            List<EntryListByDateRangeModel> result = new List<EntryListByDateRangeModel>();
            DateTime parsed_start_date = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime parsed_end_date = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                var MB_cus_list_id_array = _dbDataHD.userdatas
                    .Where(u => u.CreateDate >= parsed_start_date && u.CreateDate <= parsed_end_date && u.username == "admin")
                    .OrderBy(u => u.CreateDate)
                    .Select(u => new
                    {
                        CoId = u.GroupName,
                        RegDate = u.CreateDate
                    }).ToList();
                var MB_cus_array = _dbQldvHD.Companies
                    .Select(s => new
                    {
                        CoId = s.id.ToString(),
                        CoName = s.Name
                    }).ToList();
                var MB_cus_list_array = MB_cus_list_id_array
                    .AsEnumerable()
                    .Join(MB_cus_array, MB_cus_list_id => MB_cus_list_id.CoId, MB_cus => MB_cus.CoId, (MB_cus_list_id, MB_cus) => new
                    {
                        MB_cus.CoName,
                        MB_cus_list_id.RegDate
                    }).ToList();
                var MIFI_cus_list_id_array = _dbDataMIFI.userdatas
                    .Where(u => u.CreateDate >= parsed_start_date && u.CreateDate <= parsed_end_date && u.username == "admin")
                    .OrderBy(u => u.CreateDate)
                    .Select(u => new
                    {
                        CoId = u.GroupName,
                        RegDate = u.CreateDate
                    }).ToList();
                var MIFI_cus_array = _dbQldvMIFI.Companies
                    .Select(s => new
                    {
                        CoId = s.id.ToString(),
                        CoName = s.Name
                    }).ToList();
                var MIFI_cus_list_array = MIFI_cus_list_id_array
                    .AsEnumerable()
                    .Join(MIFI_cus_array, MIFI_cus_list_id => MIFI_cus_list_id.CoId, MIFI_cus => MIFI_cus.CoId, (MIFI_cus_list_id, MIFI_cus) => new
                    {
                        MIFI_cus.CoName,
                        MIFI_cus_list_id.RegDate
                    }).ToList();
                var MIFI_new_cus_list_array = MIFI_cus_list_array
                    .Where(mf_co => !_dbQldvHD.Companies.Any(mb_co => mb_co.Name == mf_co.CoName))
                    .Select(s => new
                    {
                        s.CoName,
                        s.RegDate
                    }).ToList();

                foreach (var item in MB_cus_list_array)
                {
                    EntryListByDateRangeModel model_instance = new EntryListByDateRangeModel();

                    model_instance.CoName = item.CoName;
                    model_instance.RegDate = item.RegDate.ToString("dd.MM.yyyy");
                    result.Add(model_instance);
                }
                foreach (var item in MIFI_new_cus_list_array)
                {
                    EntryListByDateRangeModel model_instance = new EntryListByDateRangeModel();

                    model_instance.CoName = item.CoName;
                    model_instance.RegDate = item.RegDate.ToString("dd.MM.yyyy");
                    result.Add(model_instance);
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetEntryListByDateRange, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoQtyBySvcStt()
        {
            List<CoQtyBySvcSttModel> result = new List<CoQtyBySvcSttModel>();

            try
            {
                var MB_pub_st_array = _dbDataHD.Publishes
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MB_co_array = _dbQldvHD.Companies
                    .Select(s => new
                    {
                        CoId = s.id
                    }).ToList();
                var MB_activated_array = MB_co_array
                    .Where(MB_co => !MB_pub_st_array.Any(p => p.CoId == MB_co.CoId))
                    .Select(s => new
                    {
                        s.CoId
                    }).ToList();
                var MB_pending_pub_st_array = _dbDataHD.Publishes
                    .Where(p => p.Status != 2)
                    .Select(s => new
                    {
                        CoId = s.ComID
                    }).ToList();
                var MB_approved_pub_st_array = _dbDataHD.Publishes
                    .Where(p => p.Status == 2)
                    .Select(s => new
                    {
                        CoId = s.ComID
                    }).ToList();
                var MIFI_pub_st_array = _dbDataMIFI.Publishes
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MIFI_co_array = _dbQldvMIFI.Companies
                    .Select(s => new
                    {
                        CoId = s.id.ToString(),
                        CoName = s.Name
                    }).ToList();
                var MIFI_activated_array = MIFI_co_array
                    .Where(co => !MIFI_pub_st_array.Any(p => p.CoId.ToString() == co.CoId))
                    .Select(s => new
                    {
                        s.CoId
                    }).ToList();
                var MIFI_pending_pub_st_array = _dbDataMIFI.Publishes
                    .Where(p => p.Status != 2)
                    .Select(s => new
                    {
                        CoId = s.ComID.Value.ToString()
                    }).ToList();
                var MIFI_approved_pub_st_array = _dbDataMIFI.Publishes
                    .Where(p => p.Status == 2)
                    .Select(s => new
                    {
                        CoId = s.ComID.Value.ToString()
                    }).ToList();

                for (int i = 0; i < 3; i++)
                {
                    CoQtyBySvcSttModel model_instance = new CoQtyBySvcSttModel();

                    switch (i)
                    {
                        case 0:
                            model_instance.SvcSttTitle = "Công ty đã đăng ký";
                            model_instance.CoQty = MB_activated_array.Count + MIFI_activated_array.Count;
                            result.Add(model_instance);
                            break;
                        case 1:
                            model_instance.SvcSttTitle = "Công ty đang chờ duyệt";
                            model_instance.CoQty = MB_pending_pub_st_array.Count + MIFI_pending_pub_st_array.Count;
                            result.Add(model_instance);
                            break;
                        case 2:
                            model_instance.SvcSttTitle = "Công ty đã được duyệt";
                            model_instance.CoQty = MB_approved_pub_st_array.Count + MIFI_approved_pub_st_array.Count;
                            result.Add(model_instance);
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetCoQtyBySvcStt, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoListBySvcStt(string SvcStt)
        {
            List<CoListBySvcSttModel> result = new List<CoListBySvcSttModel>();

            try
            {
                var MB_co_array = _dbQldvHD.Companies
                    .Select(s => new
                    {
                        CoId = s.id,
                        CoName = s.Name,
                        CoBoardRep = s.ContactPerson,
                        CoPhone = s.Phone,
                        CoAddress = s.Address
                    }).ToList();
                var MB_pub_st_array = _dbDataHD.Publishes
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MB_activated_co_array = MB_co_array
                    .Where(MB_co => !MB_pub_st_array.Any(p => p.CoId == MB_co.CoId))
                    .Select(s => new
                    {
                        s.CoName,
                        s.CoBoardRep,
                        s.CoPhone,
                        s.CoAddress
                    }).ToList();
                var MB_pending_pub_st_array = _dbDataHD.Publishes
                    .Where(p => p.Status != 2)
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MB_pending_co_array = MB_co_array
                    .Where(MB_co => MB_pending_pub_st_array.Any(p => p.CoId == MB_co.CoId))
                    .Select(s => new
                    {
                        s.CoName,
                        s.CoBoardRep,
                        s.CoPhone,
                        s.CoAddress
                    }).ToList();
                var MB_approved_pub_st_array = _dbDataHD.Publishes
                    .Where(p => p.Status == 2)
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MB_approved_co_array = MB_co_array
                    .Where(MB_co => MB_approved_pub_st_array.Any(p => p.CoId == MB_co.CoId))
                    .Select(s => new
                    {
                        s.CoName,
                        s.CoBoardRep,
                        s.CoPhone,
                        s.CoAddress
                    }).ToList();
                var MIFI_co_array = _dbQldvMIFI.Companies
                    .Select(s => new
                    {
                        CoId = s.id,
                        CoName = s.Name,
                        CoBoardRep = s.ContactPerson,
                        CoPhone = s.Phone,
                        CoAddress = s.Address
                    }).ToList();
                var MIFI_pub_st_array = _dbDataMIFI.Publishes
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MIFI_activated_co_array = MIFI_co_array
                    .Where(MIFI_co => !MB_pub_st_array.Any(p => p.CoId == MIFI_co.CoId))
                    .Select(s => new
                    {
                        s.CoName,
                        s.CoBoardRep,
                        s.CoPhone,
                        s.CoAddress
                    }).ToList();
                var MIFI_pending_pub_st_array = _dbDataMIFI.Publishes
                    .Where(p => p.Status != 2)
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MIFI_pending_co_array = MIFI_co_array
                    .Where(MIFI_co => MB_pending_pub_st_array.Any(p => p.CoId == MIFI_co.CoId))
                    .Select(s => new
                    {
                        s.CoName,
                        s.CoBoardRep,
                        s.CoPhone,
                        s.CoAddress
                    }).ToList();
                var MIFI_approved_pub_st_array = _dbDataMIFI.Publishes
                    .Where(p => p.Status == 2)
                    .GroupBy(p => p.ComID)
                    .Select(s => new
                    {
                        CoId = s.Key
                    }).ToList();
                var MIFI_approved_co_array = MIFI_co_array
                    .Where(MIFI_co => MB_approved_pub_st_array.Any(p => p.CoId == MIFI_co.CoId))
                    .Select(s => new
                    {
                        s.CoName,
                        s.CoBoardRep,
                        s.CoPhone,
                        s.CoAddress
                    }).ToList();

                switch (SvcStt)
                {
                    case "Công ty đã đăng ký":
                        foreach (var item in MB_activated_co_array)
                        {
                            CoListBySvcSttModel model_instance = new CoListBySvcSttModel();

                            model_instance.CoName = item.CoName;
                            model_instance.CoBoardRep = item.CoBoardRep;
                            model_instance.CoPhone = item.CoPhone;
                            model_instance.CoAddress = item.CoAddress;
                            result.Add(model_instance);
                        }
                        foreach (var item in MIFI_activated_co_array)
                        {
                            CoListBySvcSttModel model_instance = new CoListBySvcSttModel();

                            model_instance.CoName = item.CoName;
                            model_instance.CoBoardRep = item.CoBoardRep;
                            model_instance.CoPhone = item.CoPhone;
                            model_instance.CoAddress = item.CoAddress;
                            result.Add(model_instance);
                        }
                        break;
                    case "Công ty đang chờ duyệt":
                        foreach (var item in MB_pending_co_array)
                        {
                            CoListBySvcSttModel model_instance = new CoListBySvcSttModel();

                            model_instance.CoName = item.CoName;
                            model_instance.CoBoardRep = item.CoBoardRep;
                            model_instance.CoPhone = item.CoPhone;
                            model_instance.CoAddress = item.CoAddress;
                            result.Add(model_instance);
                        }
                        foreach (var item in MIFI_pending_co_array)
                        {
                            CoListBySvcSttModel model_instance = new CoListBySvcSttModel();

                            model_instance.CoName = item.CoName;
                            model_instance.CoBoardRep = item.CoBoardRep;
                            model_instance.CoPhone = item.CoPhone;
                            model_instance.CoAddress = item.CoAddress;
                            result.Add(model_instance);
                        }
                        break;
                    case "Công ty đã được duyệt":
                        foreach (var item in MB_approved_co_array)
                        {
                            CoListBySvcSttModel model_instance = new CoListBySvcSttModel();

                            model_instance.CoName = item.CoName;
                            model_instance.CoBoardRep = item.CoBoardRep;
                            model_instance.CoPhone = item.CoPhone;
                            model_instance.CoAddress = item.CoAddress;
                            result.Add(model_instance);
                        }
                        foreach (var item in MIFI_approved_co_array)
                        {
                            CoListBySvcSttModel model_instance = new CoListBySvcSttModel();

                            model_instance.CoName = item.CoName;
                            model_instance.CoBoardRep = item.CoBoardRep;
                            model_instance.CoPhone = item.CoPhone;
                            model_instance.CoAddress = item.CoAddress;
                            result.Add(model_instance);
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetCoListBySvcStt, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInvUsageByRecentDays(int RecentDayQty)
        {
            List<InvUsageByDayModel> result = new List<InvUsageByDayModel>();

            try
            {
                int interval_days = RecentDayQty - 1;
                var today = DateTime.Today;
                var end_date = today.AddDays(-interval_days);
                var MB_inv_usage_array = _dbDataHD.VATInvoices
                    .Where(inv => inv.ArisingDate.Value >= end_date && inv.ArisingDate.Value <= today)
                    .GroupBy(inv => DbFunctions.TruncateTime(inv.ArisingDate))
                    .Select(s => new
                    {
                        Date = (DateTime)s.Key,
                        InvUsageCount = s.Select(inv => inv.id).Count()
                    }).ToList();
                var MIFI_inv_usage_array = _dbDataMIFI.VATInvoices
                    .Where(inv => inv.ArisingDate.Value >= end_date && inv.ArisingDate.Value <= today)
                    .GroupBy(inv => DbFunctions.TruncateTime(inv.ArisingDate))
                    .Select(s => new
                    {
                        Date = (DateTime)s.Key,
                        InvUsageCount = s.Select(inv => inv.id).Count()
                    }).ToList();

                for (var i = end_date; i <= today; i = i.AddDays(1))
                {
                    InvUsageByDayModel model_instance = new InvUsageByDayModel();

                    model_instance.Date = i.ToString("dd.MM.yyyy");
                    for (int i1 = 0; i1 < MB_inv_usage_array.Count; i1++)
                    {
                        if (MB_inv_usage_array[i1].Date.ToString("dd.MM.yyyy") == i.ToString("dd.MM.yyyy"))
                        {
                            model_instance.InvUsageQty = MB_inv_usage_array[i1].InvUsageCount;
                            for (int i2 = 0; i2 < MIFI_inv_usage_array.Count; i2++)
                            {
                                if (MIFI_inv_usage_array[i2].Date.ToString("dd.MM.yyyy") == i.ToString("dd.MM.yyyy"))
                                {
                                    model_instance.InvUsageQty += MIFI_inv_usage_array[i2].InvUsageCount;
                                    result.Add(model_instance);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    result.Add(model_instance);
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetInvUsageByTimeline, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExpiringCoList()
        {
            List<ExpiringCoModel> result = new List<ExpiringCoModel>();

            try
            {
                var MB_co_array = _dbDataHD.Publishes
                    .Select(p => new
                    {
                        CoId = p.ComID,
                        CoName = p.ComName,
                        RepPerson = p.RepresentPerson,
                        CoPhone = p.ComPhone,
                        CoAddress = p.ComAddress
                    }).ToList();
                var MB_co_sv_pkg_array = _dbDataHD.PublishInvoices
                    .GroupBy(pi => pi.ComId)
                    .Select(s => new
                    {
                        CoId = s.Key,
                        RemainInv = (int)s.Sum(pi => pi.ToNo - pi.CurrentNo),
                        RemainInvPct = (double)(s.Sum(pi => pi.ToNo - pi.CurrentNo) / s.Sum(pi => pi.Quantity))
                    }).ToList();
                var MB_expiring_co_sv_pkg_array = MB_co_sv_pkg_array
                    .Where(s => s.RemainInvPct <= 0.1)
                    .Select(s => new
                    {
                        s.CoId,
                        s.RemainInv
                    }).ToList();
                var MB_expiring_co_array = MB_expiring_co_sv_pkg_array
                    .AsEnumerable()
                    .Join(MB_co_array, MB_expiring_co_sv_pkg => MB_expiring_co_sv_pkg.CoId, MB_co => MB_co.CoId, (MB_expiring_co_sv_pkg, MB_co) => new
                    {
                        MB_co.CoName,
                        MB_co.RepPerson,
                        MB_co.CoPhone,
                        MB_co.CoAddress,
                        MB_expiring_co_sv_pkg.RemainInv
                    }).ToList();
                var MIFI_co_array = _dbDataMIFI.Publishes
                    .Select(p => new
                    {
                        CoId = p.ComID,
                        CoName = p.ComName,
                        RepPerson = p.RepresentPerson,
                        CoPhone = p.ComPhone,
                        CoAddress = p.ComAddress
                    }).ToList();
                var new_co_list = MIFI_co_array
                    .Where(mf_co => !_dbQldvHD.Companies.Any(mb_co => mb_co.Name == mf_co.CoName))
                    .Select(s => new
                    {
                        s.CoId,
                        s.CoName,
                        s.RepPerson,
                        s.CoPhone,
                        s.CoAddress
                    }).ToList();
                var MIFI_co_sv_pkg_array = _dbDataMIFI.PublishInvoices
                    .GroupBy(pi => pi.ComId)
                    .Select(s => new
                    {
                        CoId = s.Key,
                        RemainInv = (int)s.Sum(pi => pi.ToNo - pi.CurrentNo),
                        RemainInvPct = (double)(s.Sum(pi => pi.ToNo - pi.CurrentNo) / s.Sum(pi => pi.Quantity))
                    }).ToList();
                var MIFI_expiring_co_sv_pkg_array = MIFI_co_sv_pkg_array
                    .Where(s => s.RemainInvPct <= 0.1)
                    .Select(s => new
                    {
                        s.CoId,
                        s.RemainInv
                    }).ToList();
                var MIFI_expiring_co_array = MIFI_expiring_co_sv_pkg_array
                    .AsEnumerable()
                    .Join(new_co_list, MIFI_expiring_co_sv_pkg => MIFI_expiring_co_sv_pkg.CoId, new_co => new_co.CoId, (MIFI_expiring_co_sv_pkg, new_co) => new
                    {
                        new_co.CoName,
                        new_co.RepPerson,
                        new_co.CoPhone,
                        new_co.CoAddress,
                        MIFI_expiring_co_sv_pkg.RemainInv
                    }).ToList();

                foreach (var item in MB_expiring_co_array)
                {
                    ExpiringCoModel model_instance = new ExpiringCoModel();

                    model_instance.CoName = item.CoName;
                    model_instance.RepPerson = item.RepPerson;
                    model_instance.CoPhone = item.CoPhone;
                    model_instance.CoAddress = item.CoAddress;
                    model_instance.RemainInv = item.RemainInv;
                    result.Add(model_instance);
                }
                foreach (var item in MIFI_expiring_co_array)
                {
                    ExpiringCoModel model_instance = new ExpiringCoModel();

                    model_instance.CoName = item.CoName;
                    model_instance.RepPerson = item.RepPerson;
                    model_instance.CoPhone = item.CoPhone;
                    model_instance.CoAddress = item.CoAddress;
                    model_instance.RemainInv = item.RemainInv;
                    result.Add(model_instance);
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetExpiringCoList, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoQtyBySvcPkg()
        {
            List<CoQtyBySvcPkgModel> result = new List<CoQtyBySvcPkgModel>();
            string[] active_svc_pkg_array = _activeSvcPkg.Split(';');

            try
            {
                var MB_svc_pkg_array = _dbQldvHD.ServicePackages
                    .Select(sp => new
                    {
                        ComId = sp.ComID,
                        SvcName = sp.ServiceName
                    }).ToList();
                var MB_co_array = _dbQldvHD.Companies
                    .Select(co => new
                    {
                        CoId = co.id,
                        CoName = co.Name
                    }).ToList();
                var MB_co_svc_pkg_array = MB_svc_pkg_array
                    .AsEnumerable()
                    .Join(MB_co_array, MB_svc_pkg => MB_svc_pkg.ComId, MB_co => MB_co.CoId, (MB_svc_pkg, MB_co) => new
                    {
                        MB_co.CoName,
                        MB_svc_pkg.SvcName
                    }).ToList();
                var MIFI_svc_pkg_array = _dbQldvMIFI.ServicePackages
                    .Select(sp => new
                    {
                        ComId = sp.ComID,
                        SvcName = sp.ServiceName
                    }).ToList();
                var MIFI_co_array = _dbQldvMIFI.Companies
                    .Select(co => new
                    {
                        CoId = co.id,
                        CoName = co.Name
                    }).ToList();
                var MIFI_co_svc_pkg_array = MIFI_svc_pkg_array
                    .AsEnumerable()
                    .Join(MIFI_co_array, MIFI_svc_pkg => MIFI_svc_pkg.ComId, MIFI_co => MIFI_co.CoId, (MIFI_svc_pkg, MIFI_co) => new
                    {
                        MIFI_co.CoName,
                        MIFI_svc_pkg.SvcName
                    }).ToList();
                CoQtyBySvcPkgModel model_instance;

                for (int i = 0; i < active_svc_pkg_array.Length; i++)
                {
                    model_instance = new CoQtyBySvcPkgModel();

                    model_instance.SvcPkgTitle = active_svc_pkg_array[i];

                    var MB_active_co_svc_pkg = MB_co_svc_pkg_array
                        .Where(MB_co_svc_pkg => MB_co_svc_pkg.SvcName == active_svc_pkg_array[i])
                        .Select(s => new
                        {
                            s.CoName
                        }).ToList();
                    var MIFI_active_co_svc_pkg = MIFI_co_svc_pkg_array
                        .Where(MIFI_co_svc_pkg => MIFI_co_svc_pkg.SvcName == active_svc_pkg_array[i])
                        .Select(s => new
                        {
                            s.CoName
                        }).ToList();
                    var MB_active_co_svc_pkg_qty = MB_active_co_svc_pkg.Count == 0 ? 0 : MB_active_co_svc_pkg.Count;
                    var MIFI_active_co_svc_pkg_qty = MIFI_active_co_svc_pkg.Count == 0 ? 0 : MIFI_active_co_svc_pkg.Count;

                    model_instance.CoQty = MB_active_co_svc_pkg_qty + MIFI_active_co_svc_pkg_qty;
                    result.Add(model_instance);
                }
                model_instance = new CoQtyBySvcPkgModel();
                model_instance.SvcPkgTitle = "Khác";

                var MB_inactive_co_svc_pkg = MB_co_svc_pkg_array
                    .Where(MB_co_svc_pkg => !active_svc_pkg_array.Contains(MB_co_svc_pkg.SvcName))
                    .ToList();
                var MIFI_inactive_co_svc_pkg = MIFI_co_svc_pkg_array
                    .Where(MIFI_co_svc_pkg => !active_svc_pkg_array.Contains(MIFI_co_svc_pkg.SvcName))
                    .ToList();
                var MB_inactive_co_svc_pkg_qty = MB_inactive_co_svc_pkg.Count == 0 ? 0 : MB_inactive_co_svc_pkg.Count;
                var MIFI_inactive_co_svc_pkg_qty = MIFI_inactive_co_svc_pkg.Count == 0 ? 0 : MIFI_inactive_co_svc_pkg.Count;

                model_instance.CoQty = MB_inactive_co_svc_pkg_qty + MIFI_inactive_co_svc_pkg_qty;
                result.Add(model_instance);
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetCoQtyBySvcPkg, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoListBySvcPkg(string SvcPkgName)
        {
            List<CoListBySvcPkgModel> result = new List<CoListBySvcPkgModel>();
            string[] active_svc_pkg_array = _activeSvcPkg.Split(';');

            try
            {
                if (SvcPkgName == "Khác")
                {
                    var MB_svc_pkg_list_array = _dbQldvHD.ServicePackages
                        .Where(sp => !active_svc_pkg_array.Contains(sp.ServiceName))
                        .Select(s => new
                        {
                            CoId = s.ComID,
                            SvcCode = s.ServiceCode,
                            SvcName = s.ServiceName,
                            SvcStt = s.IsActive
                        }).ToList();
                    var MB_co_list_array = _dbQldvHD.Companies
                        .Select(s => new
                        {
                            CoId = s.id,
                            CoName = s.Name,
                            CoBoardRep = s.ContactPerson
                        }).ToList();
                    var MB_co_svc_pkg_list = MB_svc_pkg_list_array
                        .AsEnumerable()
                        .Join(MB_co_list_array, MB_svc_pkg_list => MB_svc_pkg_list.CoId, MB_co_list => MB_co_list.CoId, (MB_svc_pkg_list, MB_co_list) => new
                        {
                            MB_co_list.CoName,
                            MB_co_list.CoBoardRep,
                            MB_svc_pkg_list.SvcName,
                            MB_svc_pkg_list.SvcCode,
                            MB_svc_pkg_list.SvcStt
                        }).ToList();
                    var MIFI_svc_pkg_list_array = _dbQldvMIFI.ServicePackages
                        .Where(sp => !active_svc_pkg_array.Contains(sp.ServiceName))
                        .Select(s => new
                        {
                            CoId = s.ComID,
                            SvcCode = s.ServiceCode,
                            SvcName = s.ServiceName,
                            SvcStt = s.IsActive
                        }).ToList();
                    var MIFI_co_list_array = _dbQldvMIFI.Companies
                        .Select(s => new
                        {
                            CoId = s.id,
                            CoName = s.Name,
                            CoBoardRep = s.ContactPerson
                        }).ToList();
                    var MIFI_co_svc_pkg_list = MIFI_svc_pkg_list_array
                        .AsEnumerable()
                        .Join(MIFI_co_list_array, MIFI_svc_pkg_list => MIFI_svc_pkg_list.CoId, MIFI_co_list => MIFI_co_list.CoId, (MIFI_svc_pkg_list, MIFI_co_list) => new
                        {
                            MIFI_co_list.CoName,
                            MIFI_co_list.CoBoardRep,
                            MIFI_svc_pkg_list.SvcName,
                            MIFI_svc_pkg_list.SvcCode,
                            MIFI_svc_pkg_list.SvcStt
                        }).ToList();

                    foreach (var item in MB_co_svc_pkg_list)
                    {
                        CoListBySvcPkgModel model_instance = new CoListBySvcPkgModel();

                        model_instance.CoName = item.CoName;
                        model_instance.CoBoardRep = item.CoBoardRep == null ? string.Empty : item.CoBoardRep;
                        model_instance.SvcName = item.SvcName == null ? string.Empty : item.SvcName;
                        model_instance.SvCode = item.SvcCode == null ? string.Empty : item.SvcCode;
                        model_instance.SvStt = item.SvcStt == null ? 1 : item.SvcStt.Value;
                        result.Add(model_instance);
                    }
                    foreach (var item in MIFI_co_svc_pkg_list)
                    {
                        CoListBySvcPkgModel model_instance = new CoListBySvcPkgModel();

                        model_instance.CoName = item.CoName;
                        model_instance.CoBoardRep = item.CoBoardRep == null ? string.Empty : item.CoBoardRep;
                        model_instance.SvcName = item.SvcName == null ? string.Empty : item.SvcName;
                        model_instance.SvCode = item.SvcCode == null ? string.Empty : item.SvcCode;
                        model_instance.SvStt = item.SvcStt == null ? 1 : item.SvcStt.Value;
                        result.Add(model_instance);
                    }
                }
                else
                {
                    var MB_svc_pkg_list_array = _dbQldvHD.ServicePackages
                        .Where(sp => sp.ServiceName == SvcPkgName)
                        .Select(s => new
                        {
                            CoId = s.ComID,
                            SvcCode = s.ServiceCode,
                            SvcStt = s.IsActive
                        }).ToList();
                    var MB_co_list_array = _dbQldvHD.Companies
                        .Select(s => new
                        {
                            CoId = s.id,
                            CoName = s.Name,
                            CoBoardRep = s.ContactPerson
                        }).ToList();
                    var MB_co_svc_pkg_list = MB_svc_pkg_list_array
                        .AsEnumerable()
                        .Join(MB_co_list_array, MB_svc_pkg_list => MB_svc_pkg_list.CoId, MB_co_list => MB_co_list.CoId, (MB_svc_pkg_list, MB_co_list) => new
                        {
                            MB_co_list.CoName,
                            MB_co_list.CoBoardRep,
                            MB_svc_pkg_list.SvcCode,
                            MB_svc_pkg_list.SvcStt
                        }).ToList();
                    var MIFI_svc_pkg_list_array = _dbQldvMIFI.ServicePackages
                        .Where(sp => sp.ServiceName == SvcPkgName)
                        .Select(s => new
                        {
                            CoId = s.ComID,
                            SvcCode = s.ServiceCode,
                            SvcStt = s.IsActive
                        }).ToList();
                    var MIFI_co_list_array = _dbQldvMIFI.Companies
                        .Select(s => new
                        {
                            CoId = s.id,
                            CoName = s.Name,
                            CoBoardRep = s.ContactPerson
                        }).ToList();
                    var MIFI_co_svc_pkg_list = MIFI_svc_pkg_list_array
                        .AsEnumerable()
                        .Join(MIFI_co_list_array, MIFI_svc_pkg_list => MIFI_svc_pkg_list.CoId, MIFI_co_list => MIFI_co_list.CoId, (MIFI_svc_pkg_list, MIFI_co_list) => new
                        {
                            MIFI_co_list.CoName,
                            MIFI_co_list.CoBoardRep,
                            MIFI_svc_pkg_list.SvcCode,
                            MIFI_svc_pkg_list.SvcStt
                        }).ToList();

                    foreach (var item in MB_co_svc_pkg_list)
                    {
                        CoListBySvcPkgModel model_instance = new CoListBySvcPkgModel();

                        model_instance.CoName = item.CoName;
                        model_instance.CoBoardRep = item.CoBoardRep == null ? string.Empty : item.CoBoardRep;
                        model_instance.SvcName = SvcPkgName;
                        model_instance.SvCode = item.SvcCode == null ? string.Empty : item.SvcCode;
                        model_instance.SvStt = item.SvcStt == null ? 1 : item.SvcStt.Value;
                        result.Add(model_instance);
                    }
                    foreach (var item in MIFI_co_svc_pkg_list)
                    {
                        CoListBySvcPkgModel model_instance = new CoListBySvcPkgModel();

                        model_instance.CoName = item.CoName;
                        model_instance.CoBoardRep = item.CoBoardRep == null ? string.Empty : item.CoBoardRep;
                        model_instance.SvcName = SvcPkgName;
                        model_instance.SvCode = item.SvcCode == null ? string.Empty : item.SvcCode;
                        model_instance.SvStt = item.SvcStt == null ? 1 : item.SvcStt.Value;
                        result.Add(model_instance);
                    }
                }
            }
            catch (Exception exc)
            {
                FailReqModel model_instance = new FailReqModel();
                int exc_line = new GeneralController().GetExcLineNo(exc);

                model_instance.Msg = _resultError;
                Logs.WriteToLogFile(string.Format("[LỖI] Statistics/GetCoListBySvcPkg, dòng {0}: {1}", exc_line, exc.Message));
                return Json(model_instance, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}