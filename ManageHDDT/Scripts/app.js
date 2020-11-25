// #region Khai báo hằng số
const GETDataReqStr = '/General/SendGETDataReq';
const POSTDataReqStr = '/General/SendPOSTDataReq';
const POSTAuthedDataReqStr = '/General/SendAuthedPOSTDataReq';
const mCoListByCusRefData = 'GetCoListByCusRef';
const CoInvMgmtSiteUrlStr = '/Operating/GetCoInvMgmtSiteUrl';
const mCoListData = 'GetAllCo';
const mCoInfoData = 'GetCoInfo';
const mCoInfoUpdate = 'UpdateCoInfo';
const mCoPubData = 'GetCoPub';
const mCoPubUpdate = 'UpdateCoPub';
const mPubTmplData = 'GetPubTmpl';
const mCoInvData = 'GetCoInvList';
const mInvTmplViewData = 'CompInvTmplView';
const mInvTmplPdfData = '/Operating/CompInvTmplPdf';
const mPubTmplUpdate = 'UpdatePubTmpl';
const mCoInvPubStEmailData = 'GetCoInvPubStEmail';
const mCoInvPubStEmailUpdate = 'UpdateCoInvPubStEmail';
const mCoDigiCertData = 'GetCoDigiCert';
const mCoDigiCertIns = 'InsCoDigiCert';
const mCoConfigData = 'GetCoConfig';
const mCoConfigIns = 'InsCoConfig';
const mCoConfigUpdate = 'UpdateCoConfig';
const mCusEntryChartData = 'GetCusEntryQty';
const mCusEntryTableData = 'GetEntryListByDateRange';
const mCoQtyChartData = 'GetCoQtyBySvcStt';
const mCoQtyTableData = 'GetCoListBySvcStt';
const mInvUsageTableData = 'GetExpiringCoList';
const mInvUsageChartData = 'GetInvUsageByRecentDays';
const mUserDetailsData = 'GetLoggedUserDetails';
const mUserDetailsUpdate = 'UpdateLoggedUserDetails';
const mUserData = 'GetAllUser';
const mRoleData = 'GetAllRole';
const mRoleIns = 'InsRole';
const mRoleUpdate = 'UpdateRole';
const mRankData = 'GetAllRank';
const mRankIns = 'InsRank';
const mRankUpdate = 'UpdateRank';
const mFuncData = 'GetAllFunc';
const mFuncIns = 'InsFunc';
const mFuncUpdate = 'UpdateFunc';
const leadImportStr = '/Feature/ImportLead';
const mCheckTableStack = '/Feature/CheckTableStack';
const mEmailTmplData = 'GetEmailTmplList';
const mEmailTmplIns = 'InsEmailTmpl';
const mEmailTmplUpdate = 'UpdateEmailTmpl';
const mInvTmplData = 'GetInvTmplList';
const mInvTmplIns = 'InsInvTmpl';
const mInvTmplUpdate = 'UpdateInvTmpl';
const mTaxAuthData = 'GetTaxAuthList';
const mTaxAuthIns = 'InsTaxAuth';
const mTaxAuthUpdate = 'UpdateTaxAuth';
const mUnitData = 'GetUnitList';
const mUnitIns = 'InsUnit';
const mUnitUpdate = 'UpdateUnit';
const mNotifData = 'GetNotifList';
const mNotifIns = 'InsNotif';
const mNotifUpdate = 'UpdateNotif';
const mInvCatData = 'GetInvCatList';
const mInvCatIns = 'InsInvCat';
const mInvCatUpdate = 'UpdateInvCat';
const mSvcPkgChartData = 'GetCoQtyBySvcPkg';
const mSvcPkgTableData = 'GetCoListBySvcPkg';
const mLoggedUserData = 'GetAuthedUser';
const mLoggedFuncData = 'GetAuthedFunc';
const mRoleListData = 'GetRoleList';
const mUserIns = 'InsUser';
const mUserUpdate = 'UpdateUser';
// #endregion

$(window).on("load", function () {
    /*
     Template Name: Dashor - Responsive Bootstrap 4 Admin Dashboard
     Author: Themesdesign
     Website: www.themesdesign.in
     File: Main js
     */


    !function ($) {
        "use strict";

        var MainApp = function () { };

        MainApp.prototype.initNavbar = function () {

            $('.navbar-toggle').on('click', function (event) {
                $(this).toggleClass('open');
                $('#navigation').slideToggle(400);
            });

            $('.navigation-menu>li').slice(-1).addClass('last-elements');

            $('.navigation-menu li.has-submenu a[href="#"]').on('click', function (e) {
                if ($(window).width() < 992) {
                    e.preventDefault();
                    $(this).parent('li').toggleClass('open').find('.submenu:first').toggleClass('open');
                }
            });
        },
            MainApp.prototype.initScrollbar = function () {
                $('.slimscroll-noti').slimScroll({
                    height: '230px',
                    position: 'right',
                    size: "5px",
                    color: '#98a6ad',
                    wheelStep: 10
                });
            }
        // === following js will activate the menu in left side bar based on url ====
        MainApp.prototype.initMenuItem = function () {
            $(".navigation-menu a").each(function () {
                if (this.href == window.location.href) {
                    $(this).parent().addClass("active"); // add active to li of the current link
                    $(this).parent().parent().parent().addClass("active"); // add active class to an anchor
                    $(this).parent().parent().parent().parent().parent().addClass("active"); // add active class to an anchor
                }
            });
        },
            MainApp.prototype.initComponents = function () {
                $('[data-toggle="tooltip"]').tooltip();
                $('[data-toggle="popover"]').popover();
            },
            MainApp.prototype.initToggleSearch = function () {
                $('.toggle-search').on('click', function () {
                    var targetId = $(this).data('target');
                    var $searchBar;
                    if (targetId) {
                        $searchBar = $(targetId);
                        $searchBar.toggleClass('open');
                    }
                });
            },

            MainApp.prototype.init = function () {
                this.initNavbar();
                this.initScrollbar();
                this.initMenuItem();
                this.initComponents();
                this.initToggleSearch();
            },

            //init
            $.MainApp = new MainApp, $.MainApp.Constructor = MainApp
    }(window.jQuery),

    //initializing
    function ($) {
        "use strict";
        $.MainApp.init();
        }(window.jQuery);

    $('.panel-title').on('click', toggleIcon);    
});

function toggleIcon(e) {
    var target = $(e.target).find(".more-less");
    if (!target.prop("style")["transform"] || target.prop("style")["transform"] == 'none') {
        target.css({
            "transform": 'rotate(180deg)'
        });
    }
    else if (target.prop("style")["transform"] == 'rotate(180deg)') {
        target.css({
            "transform": 'none'
        });
    }
}

function dispMsg(msgCpnts) {
    msgArray = msgCpnts.split(':');
    DevExpress.ui.notify({ message: msgArray[3], width: parseInt(msgArray[2]), position: "center" }, msgArray[1], parseInt(msgArray[0]));
}