using System;
using System.Web.Configuration;
using XmlRpc;
using LazyCache;

namespace ManageHDDT.Helpers
{
    public class OdooApiService
    {
        private string ODOO_SERVICE_URL = WebConfigurationManager.AppSettings["OdooServiceURL"];
        private string ODOO_DATABASE = WebConfigurationManager.AppSettings["OdooDatabase"];
        private string ODOO_USERNAME = WebConfigurationManager.AppSettings["OdooUsername"];
        private string ODOO_PASSWORD = WebConfigurationManager.AppSettings["OdooPassword"];
                
        private string ALLOW_CACHE_API_ODOO = WebConfigurationManager.AppSettings["AllowCacheOdooLoginAPI"];
        private XmlRpcResponse _responseLogin = null;
        private IAppCache _cachingService = new CachingService { DefaultCacheDuration = 120 };
        private static string _cacheLoginApiOdooKey = "OdooAPIServiceLOGIN";
        private static int _cacheLoginApiOdooTime = 5;
        
        public OdooApiService()
        {
            if (!"1".Equals(ALLOW_CACHE_API_ODOO))
            {
                LoginApi();
            }
            else
            {
                if (!CheckLogin())
                {
                    var result = LoginApi();
                    if (result)
                    {
                        _cachingService.Add(_cacheLoginApiOdooKey, _responseLogin, DateTime.Now.AddMinutes(_cacheLoginApiOdooTime));
                    }
                }
            }
        }

        #region Check Login API Odoo
        private bool LoginApi()
        {
            try
            {
                XmlRpcClient client = new XmlRpcClient();
                client.Url = ODOO_SERVICE_URL;
                client.Path = "common";
                client.Timeout = 100000;

                // LOGIN

                XmlRpcRequest requestLogin = new XmlRpcRequest("authenticate");
                requestLogin.AddParams(ODOO_DATABASE, ODOO_USERNAME, ODOO_PASSWORD, XmlRpcParameter.EmptyStruct());

                _responseLogin = client.Execute(requestLogin);

                if (!_responseLogin.IsFault())
                {
                    if (_responseLogin.GetInt() > 0)
                        return true;
                }
            }
            catch
            { }

            _responseLogin = null;

            return false;
        }

        private bool CheckLogin()
        {
            try
            {
                if (_cachingService.Get<XmlRpcResponse>(_cacheLoginApiOdooKey) != null)
                {
                    _responseLogin = _cachingService.Get<XmlRpcResponse>(_cacheLoginApiOdooKey);
                }

                if (_responseLogin == null || _responseLogin.IsFault()) return false;

                return true;
            }
            catch
            { }

            return false;
        }

        private int GetLoginValue(XmlRpcResponse res)
        {
            try
            {
                if (res == null) return -1;

                return res.GetInt();
            }
            catch
            { }

            return -1;
        }

        private void ResetCacheLogin()
        {
            _cachingService.Remove(_cacheLoginApiOdooKey);
        }
        #endregion

        public string CreateLead(string odoo_team_code, int odoo_company_id, string odoo_state_code, string odoo_country_code, string odoo_source_code
            , string street, string city, string zip, string company_name, string contact_name, string name, string function
            , string phone, string email, string mobile, string description, int planned_revenue)
        {
            var returnMsg = string.Empty;

            try
            {
                XmlRpcClient client = new XmlRpcClient();
                client.Url = ODOO_SERVICE_URL;
                client.Path = "common";
                client.Timeout = 100000;

                //LOGIN
                if (_responseLogin.IsFault())
                {
                    return "Login Fault";
                }
                else
                {
                    //CREATE
                    client.Path = "object";

                    XmlRpcRequest requestCreate = new XmlRpcRequest("execute_kw");

                    var paramInput = XmlRpcParameter.AsStruct(
                                XmlRpcParameter.AsMember("partner_id", "")
                                , XmlRpcParameter.AsMember("user_id", "")
                                , XmlRpcParameter.AsMember("team_id", string.Format("{0}", odoo_team_code))
                                , XmlRpcParameter.AsMember("company_id", odoo_company_id)
                                , XmlRpcParameter.AsMember("state_id", string.Format("{0}", odoo_state_code))
                                , XmlRpcParameter.AsMember("country_id", string.Format("{0}", odoo_country_code))
                                , XmlRpcParameter.AsMember("title", "")
                                , XmlRpcParameter.AsMember("source_id", string.Format("{0}", odoo_source_code))
                                , XmlRpcParameter.AsMember("street", string.Format("{0}", street))
                                , XmlRpcParameter.AsMember("street2", "")
                                , XmlRpcParameter.AsMember("city", string.Format("{0}", city))
                                , XmlRpcParameter.AsMember("zip", string.Format("{0}", zip))
                                , XmlRpcParameter.AsMember("partner_name", string.Format("{0}", company_name))
                                , XmlRpcParameter.AsMember("contact_name", string.Format("{0}", contact_name))
                                , XmlRpcParameter.AsMember("name", string.Format("{0}", name))
                                , XmlRpcParameter.AsMember("function", string.Format("{0}", function))
                                , XmlRpcParameter.AsMember("phone", string.Format("{0}", phone))
                                , XmlRpcParameter.AsMember("email_from", string.Format("{0}", email))
                                , XmlRpcParameter.AsMember("mobile", string.Format("{0}", mobile))
                                , XmlRpcParameter.AsMember("fax", "")
                                , XmlRpcParameter.AsMember("description", string.Format("{0}", description))
                                , XmlRpcParameter.AsMember("referred", "")
                                , XmlRpcParameter.AsMember("planned_revenue", planned_revenue)
                            );

                    requestCreate.AddParams(ODOO_DATABASE, _responseLogin.GetInt(), ODOO_PASSWORD, "crm.lead", "create_lead",
                        XmlRpcParameter.AsArray(paramInput)
                    );

                    XmlRpcResponse responseCreate = client.Execute(requestCreate);

                    if (!responseCreate.IsFault())
                    {
                        return "Done";
                    }
                    else
                    {
                        Logs.WriteToLogFile(string.Format("[ERROR] CreateLead: {0}", responseCreate.GetString()));
                        return responseCreate.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}