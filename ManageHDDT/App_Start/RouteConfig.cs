using System.Web.Mvc;
using System.Web.Routing;

namespace ManageHDDT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ResourceUpdate",
                url: "tinh_nang/nhap_lieu/{id}",
                defaults: new { controller = "Feature", action = "ResourceUpdate", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UserCtrl",
                url: "dieu_hanh/nguoi_dung",
                defaults: new { controller = "Operating", action = "UserCtrl", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CoInvMgmt",
                url: "dieu_hanh/quan_ly_hddt/{id}",
                defaults: new { controller = "Operating", action = "CoInvMgmt", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SvPkg",
                url: "thong_ke/goi_dich_vu/{id}",
                defaults: new { controller = "Statistics", action = "SvcPkg", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "InvUsage",
                url: "thong_ke/su_dung_hoa_don/{id}",
                defaults: new { controller = "Statistics", action = "InvUsage", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CusQty",
                url: "thong_ke/luong_khach_hang/{id}",
                defaults: new { controller = "Statistics", action = "CusQty", id = UrlParameter.Optional }
            );            

            routes.MapRoute(
                name: "OpsAdjmt",
                url: "quan_tri",
                defaults: new { controller = "Home", action = "OpsAdjmt", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SupTkts",
                url: "trang_chu",
                defaults: new { controller = "Home", action = "SupTkts", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "dang_nhap",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "SupTkts", id = UrlParameter.Optional }
            );
        }
    }
}
