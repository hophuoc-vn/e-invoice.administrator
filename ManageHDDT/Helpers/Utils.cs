using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace ManageHDDT.Helpers
{
    public class Utils
    {
        public static bool CheckDataSet(DataSet ds, int idx)
        {
            if (ds == null || ds.Tables == null || ds.Tables.Count == 0 ||
                ds.Tables[idx].Rows == null || ds.Tables[idx].Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool CheckDataTable(DataTable dt)
        {
            if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static int ParseToInt32(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
            {
                return 0;
            }
            try
            {
                return Convert.ToInt32(obj.ToString());
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return 0;
            }            
        }

        public static double ConvertToDouble(object data)
        {
            try
            {
                var s = string.Format("{0}", data);
                var r = Double.Parse(s);

                return r;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return 0;
            }            
        }

        public static string JoinStrArrayIntoStr(string[] array)
        {
            // Use string Join to concatenate the string elements.
            string result = string.Join(",", array);
            return result;
        }

        public static string EncryptStr(string str)
        {
            SHA512 d_EncryptionSHA512 = new SHA512Managed();
            byte[] ByteHasEncryption = d_EncryptionSHA512.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder StrBuilderStringEncryption = new StringBuilder();
            for (int i = 0; i < ByteHasEncryption.Length; i++)
            {
                StrBuilderStringEncryption.Append(ByteHasEncryption[i].ToString("x2"));
            }
            return StrBuilderStringEncryption.ToString();
        }
    }

    public class Logs
    {
        public static void WriteToLogFile(string logMessage)
        {
            if (Utils.ParseToInt32(ConfigurationManager.AppSettings["AllowLog"]) == 0)
            {
                return;
            }

            string strLogMessage = string.Empty;
            DateTime dtCurr = DateTime.Now;
            string sPath = ConfigurationManager.AppSettings["LogFilePath"];            
            string strLogFile = @sPath + @"\\DhqlHddtMBLog_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            StreamWriter swLog;

            strLogMessage = string.Format("{0}: {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), logMessage);
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            if (!File.Exists(strLogFile))
            {
                swLog = new StreamWriter(strLogFile);
            }
            else
            {
                swLog = File.AppendText(strLogFile);
            }
            swLog.WriteLine(strLogMessage);

            swLog.Close();
        }
    }

    public class UserDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddr { get; set; }
        public string LoginTkn { get; set; }
        public string Role { get; set; }
        public string[] Funcs { get; set; }
        public string CreateDate { get; set; }
        public string ModifyDate { get; set; }
        public string Flag { get; set; }
    }

    public class FuncDataModel
    {
        public int Card { get; set; }
        public int Id { get; set; }
        public int RootId { get; set; }
        public string Name { get; set; }
        public string Des { get; set; }
        public bool IsAuthed { get; set; }
        public string CreateDate { get; set; }
        public string ModifyDate { get; set; }
        public string Flag { get; set; }
    }

    public class RankDataModel
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public bool IsMaster { get; set; }
        public string CreateDate { get; set; }
        public string ModifyDate { get; set; }
        public string Flag { get; set; }
    }

    public class RoleDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int Rank { get; set; }
        public string CreateDate { get; set; }
        public string ModifyDate { get; set; }
        public string Flag { get; set; }
    }

    public class InvCatDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PatPrefix { get; set; }
        public string Desc { get; set; }
        public string UsageStt { get; set; }
    }

    public class NotifDataModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LnchDate { get; set; }
        public string DismDate { get; set; }
        public int Stt { get; set; }
    }

    public class TaxAuthDataModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }
        public string Phone { get; set; }
        public string Visibility { get; set; }
    }

    public class InvTmplDataModel
    {
        public int Id { get; set; }
        public int InvCatId { get; set; }
        public string InvCatName { get; set; }
        public string TmplName { get; set; }
        public string TmplXml { get; set; }
        public string TmplXslt { get; set; }
        public string SvcType { get; set; }
        public string InvType { get; set; }
        public string InvView { get; set; }
        public string iGenerator { get; set; }
        public string iViewer { get; set; }
        public string TmplCss { get; set; }
        public string TmplThumbnailDir { get; set; }
        public string IsPub { get; set; }
        public string IsCert { get; set; }
    }

    public class EmailTmplDataModel
    {
        public int Id { get; set; }
        public int CoId { get; set; }
        public string SenderEmail { get; set; }
        public string Sender { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
        public int EmailType { get; set; }

    }

    public class CoConfigDataModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class CoDigiCertDataModel
    {
        public int Id { get; set; }
        public int CoId { get; set; }
        public int Type { get; set; }
        public string Path { get; set; }
        public string Pwd { get; set; }
        public int SlotIndx { get; set; }
        public string CertSerial { get; set; }
        public int CertNo { get; set; }
    }

    public class CoInvPubStDataModel
    {
        public Guid EmailId { get; set; }
        public string SenderEmail { get; set; }
        public string[] ReceiverEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
        public int SttId { get; set; }
        public string CreateDate { get; set; }
        public string SendDate { get; set; }
    }

    public class Product
    {
        [XmlElement("Code")]
        public string ProdCode { get; set; }

        [XmlElement("ProdName")]
        public string ProdName { get; set; }

        [XmlElement("ProdUnit")]
        public string ProdUnit { get; set; }

        [XmlElement("ProdQuantity")]
        public double ProdQty { get; set; }

        [XmlElement("ProdPrice")]
        public decimal ProdPrice { get; set; }

        [XmlElement("Total")]
        public decimal ProdSubTotal { get; set; }

        [XmlElement("VATRate")]
        public double ProdVATRate { get; set; }

        [XmlElement("VATAmount")]
        public decimal ProdVATDue { get; set; }

        [XmlElement("ProdType")]
        public int ProdType { get; set; }

        [XmlElement("Amount")]
        public decimal ProdTotal { get; set; }

        [XmlElement("Discount")]
        public decimal ProdDisc { get; set; }        

        [XmlElement("DiscountAmount")]
        public decimal ProdDiscDue { get; set; }

        [XmlElement("Extra")]
        public string ProdExtra { get; set; }

        [XmlElement("Stt")]
        public int ProdStt { get; set; }

        [XmlElement("ConNo")]
        public string ConsgtNo { get; set; }

        [XmlElement("ExpDate")]
        public string ProdExpDate { get; set; }
    }

    public class InvDetailsDataModel
    {
        [XmlElement("InvoiceName")]
        public string InvCatName { get; set; }

        [XmlElement("InvoicePattern")]
        public string InvPat { get; set; }

        [XmlElement("SerialNo")]
        public string InvSerNo { get; set; }

        [XmlElement("InvoiceNo")]
        public string InvNo { get; set; }

        [XmlElement("PaymentMethod")]
        public string PymtMeth { get; set; }

        [XmlElement("ComName")]
        public string CoName { get; set; }

        [XmlElement("ComTaxCode")]
        public string CoTaxCode { get; set; }

        [XmlElement("ComAddress")]
        public string CoAddr { get; set; }

        [XmlElement("ComPhone")]
        public string CoPhone { get; set; }

        [XmlElement("ComFax")]
        public string CoFax { get; set; }

        [XmlElement("ComEmail")]
        public string CoEmailAddr { get; set; }

        [XmlElement("ComBankNo")]
        public string CoBankNo { get; set; }

        [XmlElement("ComBankName")]
        public string CoBankName { get; set; }

        [XmlElement("Buyer")]
        public string CusBuyer { get; set; }

        [XmlElement("CusCode")]
        public string CusCode { get; set; }

        [XmlElement("CusName")]
        public string CusName { get; set; }

        [XmlElement("CusTaxCode")]
        public string CusTaxCode { get; set; }

        [XmlElement("CusPhone")]
        public string CusPhone { get; set; }

        [XmlElement("CusAddress")]
        public string CusAddr { get; set; }

        [XmlElement("CusBankName")]
        public string CusBankName { get; set; }

        [XmlElement("CusBankNo")]
        public string CusBankNo { get; set; }

        [XmlElement("Total")]
        public decimal InvSubTotal { get; set; }

        [XmlElement("Amount")]
        public decimal InvTotal { get; set; }

        [XmlElement("AmountInWords")]
        public string InvTotalInWords { get; set; }

        [XmlElement("KindOfService")]
        public string InvSvcCat { get; set; }

        [XmlElement("VATAmount")]
        public decimal InvVATDue { get; set; }

        [XmlElement("Discount")]
        public decimal InvDiscDue { get; set; }

        [XmlElement("Note")]
        public string InvNote { get; set; }

        [XmlElement("Fkey")]
        public string InvRefCode { get; set; }

        [XmlElement("ArisingDate")]
        public string InvSignDate { get; set; }

        [XmlElement("Extra")]
        public string InvExtra { get; set; }

        [XmlElement("SO")]
        public string InvSO { get; set; }

        [XmlElement("GrossValue")]
        public decimal NonVATProdsTotal { get; set; }

        [XmlElement("GrossValue0")]
        public decimal NilVATProdsTotal { get; set; }

        [XmlElement("GrossValue5")]
        public decimal FivePcVATProdsTotal { get; set; }

        [XmlElement("GrossValue10")]
        public decimal TenPcVATProdsTotal { get; set; }

        [XmlElement("VatAmount5")]
        public decimal FivePcVATDue { get; set; }

        [XmlElement("VatAmount10")]
        public decimal TenPcVATDue { get; set; }

        public List<Product> Products { get; set; }
    }

    [XmlRoot("Invoice")]
    public class InvDataModel
    {
        [XmlElement("Content")]
        public InvDetailsDataModel Details { get; set; }

        [XmlElement("image")]
        public string SignStt { get; set; }
    }

    public class CoInvDataModel
    {
        public int InvId { get; set; }
        public string InvPattern { get; set; }
        public string InvSerial { get; set; }
        public decimal InvNo { get; set; }
        public string InvCus { get; set; }
        public int InvProdCount { get; set; }
        public decimal InvAmount { get; set; }
    }

    public class PubTmplDataModel
    {
        public string TmplCss { get; set; }
        public string LogoCss { get; set; }
        public string BgrCss { get; set; }
        public string TmplHtml { get; set; }
    }

    public class CoPubDataModel
    {
        public string DecNo { get; set; }
        public string PubStCode { get; set; }
        public string PubStPubDate { get; set; }
        public string TaxAuthCode { get; set; }
        public string TaxAuthName { get; set; }
        public string TaxAuthLocality { get; set; }
        public int PubStStt { get; set; }
        public string PubInvSerial { get; set; }
        public string PubInvPattern { get; set; }
        public string PubTmplInd { get; set; }
        public int PubTmplId { get; set; }
        public string PubPkgPurDate { get; set; }
        public string PubPkgExpDate { get; set; }
        public int PubPkgBeginNo { get; set; }
        public int PubPkgEndNo { get; set; }
    }

    public class CoInfoDataModel
    {
        public string CoName { get; set; }
        public string CoAddr { get; set; }
        public string CoBankAcctName { get; set; }
        public string CoBankName { get; set; }
        public string CoBankNo { get; set; }
        public string CoCEO { get; set; }
        public string CoEmailAddr { get; set; }
        public string CoFax { get; set; }
        public string CoPhone { get; set; }        
        public string CoTaxCode { get; set; }
        public string[] CoDomain { get; set; }
        public string XferStt { get; set; }
    }

    public class CoIdDataModel
    {
        public int CoId { get; set; }
        public string CoName { get; set; }
        public string CoDomain { get; set; }
    }

    public class CoListBySvcPkgModel
    {
        public string CoName { get; set; }
        public string CoBoardRep { get; set; }
        public string SvcName { get; set; }
        public string SvCode { get; set; }
        public int SvStt { get; set; }
    }

    public class CoQtyBySvcPkgModel
    {
        public string SvcPkgTitle { get; set; }
        public int CoQty { get; set; }
    }

    public class ExpiringCoModel
    {
        public string CoName { get; set; }
        public string RepPerson { get; set; }
        public string CoPhone { get; set; }
        public string CoAddress { get; set; }
        public int RemainInv { get; set; }
    }

    public class InvUsageByDayModel
    {
        public string Date { get; set; }
        public int InvUsageQty { get; set; }
    }

    public class CoListBySvcSttModel
    {
        public string CoName { get; set; }
        public string CoBoardRep { get; set; }
        public string CoPhone { get; set; }
        public string CoAddress { get; set; }
    }

    public class CoQtyBySvcSttModel
    {
        public string SvcSttTitle { get; set; }
        public int CoQty { get; set; }
    }

    public class EntryListByDateRangeModel
    {
        public string CoName { get; set; }
        public string RegDate { get; set; }
    }

    public class CusEntryQtyModel
    {
        public string arg { get; set; }
        public int val { get; set; }
        public string parentID { get; set; }
    }

    public class OdooCusDataModel
    {
        public string customer_ref { get; set; }
        public string customer_name { get; set; }
        public string product_category_name { get; set; }
        public string product_name { get; set; }
    }

    public class OdooCusSvcDataModel
    {
        public int id { get; set; }
        public string reference { get; set; }
        public string parent_reference { get; set; }
        public string name { get; set; }
        public string ip_hosting { get; set; }
        public int product_category_id { get; set; }
        public string product_category_code { get; set; }
        public string product_category_name { get; set; }
        public string parent_product_category_code { get; set; }
        public string product_id { get; set; }
        public string parent_product_id { get; set; }
        public string status { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string reseller_code { get; set; }
        public string service_type { get; set; }
        public bool can_be_renew { get; set; }
        public bool is_auto_renew { get; set; }
        public bool is_active { get; set; }
        public bool is_addons { get; set; }
        public bool is_stop_manual { get; set; }
        public bool is_stop { get; set; }
        public string temp_un_stop_date { get; set; }
        public string service_name
        {
            get
            {
                return name.Replace(reference + " - ", "");
            }
        }
        public string domain_name
        {
            get
            {
                return name.Replace(reference + " - ", "");
            }
        }
        public string _start_date
        {
            get
            {
                return start_date.ToString("dd/MM/yyyy");
            }
        }
        public string _end_date
        {
            get
            {
                return end_date.ToString("dd/MM/yyyy");
            }
        }
        public string _status
        {
            get
            {
                string result = "Đã xóa";
                if (status == "waiting" || status == "draft")
                    result = "Chưa kích hoạt";
                else if (status == "refused" || status == "closed")
                    result = "Hết hạn";
                else if (status == "active")
                {
                    if (DateTime.Compare(end_date.Date, DateTime.Now.Date) < 0)
                        result = "Hết hạn";
                    else if (DateTime.Compare(end_date.AddDays(-30).Date, DateTime.Now.Date) < 0)
                        result = "Sắp hết hạn";
                    else
                        result = "Đang sử dụng";
                }
                return result;
            }
        }
        public string _status_code
        {
            get
            {
                string result = "expire";
                if (status == "waiting" || status == "draft")
                    result = "inactive";
                else if (status == "refused" || status == "closed")
                    result = "expire";
                else if (status == "active")
                {
                    if (DateTime.Compare(end_date.Date, DateTime.Now.Date) < 0)
                        result = "expire";
                    else if (DateTime.Compare(end_date.AddDays(-30).Date, DateTime.Now.Date) < 0)
                        result = "near_expire";
                    else
                        result = "active";
                }
                return result;
            }
        }
    }

    public class OdooRespDataModel
    {
        public IList<OdooCusDataModel> data { get; set; }
        public int code { get; set; }
        public string messages { get; set; }
    }    

    public class FailReqModel
    {
        public string Msg { get; set; }
    }
}