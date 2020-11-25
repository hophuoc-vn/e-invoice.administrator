// #region Khai báo hằng số
const pubSttArray = [{
    "pubSttId": "false",
    "pubSttDes": "Không phát hành"
}, {
    "pubSttId": "true",
    "pubSttDes": "Phát hành"
    }];
const certSttArray = [{
    "certSttId": "false",
    "certSttDes": "Chưa duyệt"
}, {
    "certSttId": "true",
    "certSttDes": "Đã duyệt"
    }];
const taxAuthSttArray = [{
    "taxAuthSttId": "false",
    "taxAuthSttDes": "Không sử dụng"
}, {
    "taxAuthSttId": "true",
    "taxAuthSttDes": "Được sử dụng"
    }];
const notifSttArray = [{
    "notifSttId": 0,
    "notifSttDes": "Không sử dụng"
}, {
    "notifSttId": 1,
    "notifSttDes": "Được sử dụng"
}];
const dbArray = [{
    "DbId": 1,
    "DbDes": "HoaDon"
}, {
    "DbId": 2,
    "DbDes": "MIFI"
}, {
    "DbId": 3,
    "DbDes": "MatBao"
}, {
    "DbId": 0,
    "DbDes": "Tester"
    }];
const fieldArray = [{
    "FieldId": "EmailTmpl",
    "FieldDes": "Mẫu thư điện tử"
}, {
    "FieldId": "InvTmpl",
    "FieldDes": "Mẫu hđ"
}, {
    "FieldId": "TaxAuth",
    "FieldDes": "Chi cục thuế"
}, {
    "FieldId": "Unit",
    "FieldDes": "Đơn vị tính"
}, {
    "FieldId": "Notif",
    "FieldDes": "Thông báo hệ thống"
}, {
    "FieldId": "InvCat",
    "FieldDes": "Loại hđ"
    }];
const recQtyArray = [{
    "RecQty": 10,
    "QtyDes": "10 kết quả"
}, {
    "RecQty": 20,
    "QtyDes": "20 kết quả"
}, {
    "RecQty": 8,
    "QtyDes": "Tất cả"
}];
//#endregion
// #region Khai báo biến số
var coListArray = [];
var invCatListArray = [];
var curDbId = 2;
var curFieldId;
var leftEndStack = 0;
var rightEndStack = 0;
var curRecStack = 0;
var curRecQty = 8;
// #endregion

$(document).ready(function () {
    // #region Khởi tạo LoadIndicator
    var dataTableIndicator = $("#data-table-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    // #endregion
    // #region Cấu hình các CustomStore
    var emailTmplStore = new DevExpress.data.CustomStore({
        key: "Id",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mEmailTmplData, ParmOne: curRecStack, ParmTwo: curRecQty, ParmThree: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
            });
        },
        insert: function (values) {
            return $.post(POSTAuthedDataReqStr, { ActionName: mEmailTmplIns, ParmOne: JSON.stringify(values), ParmTwo: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        },
        update: function (key, values) {
            return $.post(POSTAuthedDataReqStr, { ActionName: mEmailTmplUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var invTmplStore = new DevExpress.data.CustomStore({
        key: "Id",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mInvTmplData, ParmOne: curRecStack, ParmTwo: curRecQty, ParmThree: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
            });
        },
        insert: function (values) {
            return $.post(POSTAuthedDataReqStr, { ActionName: mInvTmplIns, ParmOne: JSON.stringify(values), ParmTwo: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        },
        update: function (key, values) {
            return $.post(POSTAuthedDataReqStr, { ActionName: mInvTmplUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var taxAuthStore = new DevExpress.data.CustomStore({
        key: "Code",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mTaxAuthData, ParmOne: curRecStack, ParmTwo: curRecQty, ParmThree: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
            });
        },
        insert: function (values) {
            return $.post(POSTDataReqStr, { ActionName: mTaxAuthIns, ParmOne: JSON.stringify(values), ParmTwo: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        },
        update: function (key, values) {
            return $.post(POSTDataReqStr, { ActionName: mTaxAuthUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var unitStore = new DevExpress.data.CustomStore({
        key: "Code",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mUnitData, ParmOne: curRecStack, ParmTwo: curRecQty, ParmThree: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
            });
        },
        insert: function (values) {
            return $.post(POSTDataReqStr, { ActionName: mUnitIns, ParmOne: JSON.stringify(values), ParmTwo: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        },
        update: function (key, values) {
            return $.post(POSTDataReqStr, { ActionName: mUnitUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var notifStore = new DevExpress.data.CustomStore({
        key: "Id",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mNotifData, ParmOne: curRecStack, ParmTwo: curRecQty, ParmThree: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
            });
        },
        insert: function (values) {
            return $.post(POSTDataReqStr, { ActionName: mNotifIns, ParmOne: JSON.stringify(values) }, function (returnData) {
                dispMsg(returnData);
            });
        },
        update: function (key, values) {
            return $.post(POSTDataReqStr, { ActionName: mNotifUpdate, ParmOne: key, ParmTwo: JSON.stringify(values) }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var invCatStore = new DevExpress.data.CustomStore({
        key: "Id",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mInvCatData, ParmOne: curRecStack, ParmTwo: curRecQty, ParmThree: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
            });
        },
        insert: function (values) {
            return $.post(POSTDataReqStr, { ActionName: mInvCatIns, ParmOne: JSON.stringify(values), ParmTwo: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        },
        update: function (key, values) {
            return $.post(POSTDataReqStr, { ActionName: mInvCatUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    // #endregion
    // #region Khởi tạo các nguyên tố lấy dữ liệu
    $("#db-sel").dxSelectBox({
        dataSource: new DevExpress.data.ArrayStore({
            data: dbArray,
            key: "DbId"
        }),
        displayExpr: "DbDes",
        valueExpr: "DbId",
        value: curDbId,
        onValueChanged: function (data) {
            curDbId = data.value;
            clearPageData();
        }
    });
    $("#field-sel").dxSelectBox({
        dataSource: new DevExpress.data.ArrayStore({
            data: fieldArray,
            key: "FieldId"
        }),
        displayExpr: "FieldDes",
        valueExpr: "FieldId",
        onValueChanged: function (data) {
            curFieldId = data.value;
            clearPageData();
        }
    });
    var rwdBtn = $("#prev-rec-qty").dxButton({
        icon: "fas fa-step-backward",
        onClick: function (e) {
            if (curFieldId == undefined) {
                DevExpress.ui.notify({ message: "Chưa chọn trường dữ liệu", width: 250, position: "center" }, "warning", 1500);
            }
            else {
                leftEndStack = leftEndStack - curRecQty < 0 ? 0 : leftEndStack - curRecQty;
                rightEndStack = leftEndStack + curRecQty;
                curRecStack = leftEndStack;
                switch (curFieldId) {
                    case "EmailTmpl":
                        initEmailTmplTable();
                        break;
                    case "InvTmpl":
                        initInvTmplTable();
                        break;
                    case "TaxAuth":
                        initTaxAuthTable();
                        break;
                    case "Unit":
                        initUnitTable();
                        break;
                    case "Notif":
                        initNotifTable();
                        break;
                    case "InvCat":
                        initInvCatTable();
                        break;
                }
                if (curRecQty == 8) {
                    clearPageData();
                }
                if (leftEndStack == 0) {
                    rwdBtn.option("disabled", true);
                }
            }
        }
    }).dxButton("instance");
    $("#rec-qty-sel").dxSelectBox({
        dataSource: new DevExpress.data.ArrayStore({
            data: recQtyArray,
            key: "RecQty"
        }),
        displayExpr: "QtyDes",
        valueExpr: "RecQty",
        value: curRecQty,
        onValueChanged: function (data) {
            curRecQty = data.value;
        }
    });
    $("#next-rec-qty").dxButton({
        icon: "fas fa-step-forward",
        onClick: function (e) {
            if (curFieldId == undefined) {
                DevExpress.ui.notify({ message: "Chưa chọn trường dữ liệu", width: 250, position: "center" }, "warning", 1500);
            }
            else {
                curRecStack = rightEndStack;
                $.get(mCheckTableStack, { FieldName: curFieldId, RecStack: curRecStack, DbId: curDbId }, function (returnData) {
                    if (returnData.indexOf(":") >= 0) {
                        dispMsg(returnData);
                    }
                    if (returnData == "False") {
                        DevExpress.ui.notify({ message: "Không còn dữ liệu để lấy", width: 250, position: "center" }, "warning", 1500);
                    }
                    else {                        
                        switch (curFieldId) {
                            case "EmailTmpl":
                                initEmailTmplTable();
                                break;
                            case "InvTmpl":
                                initInvTmplTable();
                                break;
                            case "TaxAuth":
                                initTaxAuthTable();
                                break;
                            case "Unit":
                                initUnitTable();
                                break;
                            case "Notif":
                                initNotifTable();
                                break;
                            case "InvCat":
                                initInvCatTable();
                                break;
                        }                        
                        if (curRecQty == 8) {
                            clearPageData();
                            rwdBtn.option("disabled", true);
                        }
                        else {                            
                            rightEndStack += curRecQty;
                            leftEndStack = rightEndStack - curRecQty;
                            if (leftEndStack == 0) {
                                rwdBtn.option("disabled", true);
                            }
                            else {
                                rwdBtn.option("disabled", false);
                            }
                        }
                    }
                });                                                            
            }            
        }
    });
    // #endregion
    // #region Bắt sự kiện mở accordion Lead
    $('#collapseOne').on('show.bs.collapse', function () {
        // #region Cấu hình FileUploader nhập lead
        $("#file-uploader").dxFileUploader({
            multiple: false,
            uploadMode: "useButtons",
            uploadUrl: leadImportStr,
            onUploaded: function (e) {
                dispMsg(e.request.responseText);
            },
            onUploadError: function (e) {
                dispMsg(e.request.responseText);
            },
            allowedFileExtensions: [".xls", ".xlsx"],
            maxFileSize: 400000000
        });
        // #endregion
    });
    // #endregion

    function initEmailTmplTable() {
        dataTableIndicator.option("visible", true);
        $.get(GETDataReqStr, { ActionName: mCoListData, ParmOne: curDbId }, function (returnData) {
            coListArray = returnData;
            // #region Cấu hình bảng kê mẫu email
            $("#data-table").dxDataGrid({
                dataSource: emailTmplStore,
                keyExpr: "Id",
                showBorders: true,
                hoverStateEnabled: true,
                paging: {
                    pageSize: 10
                },
                pager: {
                    showNavigationButtons: true
                },
                editing: {
                    mode: "popup",
                    allowAdding: true,
                    allowUpdating: true,
                    popup: {
                        title: "Chi tiết mẫu email",
                        showTitle: true,
                        width: 700,
                        height: "auto",
                        position: {
                            my: "center",
                            at: "center",
                            of: window
                        },
                        dragEnabled: false,
                        closeOnOutsideClick: true
                    },
                    form: {
                        colCount: 1,
                        items: [{
                            dataField: "CoId",
                            caption: "Tên công ty"
                        }, {
                            dataField: "SenderEmail",
                            caption: "Email gửi"
                        }, {
                            dataField: "Sender",
                            caption: "Người gửi"
                        }, {
                            dataField: "EmailSubject",
                            caption: "Tiêu đề email",
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 75
                            }
                        }, {
                            dataField: "EmailContent",
                            caption: "Nội dung email",
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 200
                            }
                        }, {
                            dataField: "EmailType",
                            caption: "Loại email"
                        }]
                    }
                },
                columns: [
                    {
                        dataField: "CoId",
                        caption: "Tên công ty",
                        validationRules: [{ type: "required" }],
                        lookup: {
                            dataSource: coListArray,
                            displayExpr: "CoName",
                            valueExpr: "CoId"
                        }
                    },
                    {
                        dataField: "SenderEmail",
                        caption: "Email gửi",
                        validationRules: [{
                            type: "required"
                        }, {
                            type: "email"
                        }]
                    },
                    {
                        dataField: "Sender",
                        caption: "Người gửi"
                    },
                    {
                        dataField: "EmailSubject",
                        caption: "Tiêu đề email"
                    },
                    {
                        dataField: "EmailContent",
                        caption: "Nội dung email"
                    },
                    {
                        width: 80,
                        alignment: "center",
                        cellTemplate: function (container, options) {
                            $("<span id='email-tmpl-" + options.data.Id + "'><i class='fa fa-eye ux-blue cursor-pointer'></i></span>").appendTo(container);
                            $("#email-tmpl-" + options.data.Id).click(function () {
                                $("#email-tmpl-container").html(options.data.EmailContent);
                                $('#emailTmplViewModal').modal('show');
                            });
                        }
                    },
                    {
                        dataField: "EmailType",
                        caption: "Loại email",
                        dataType: "number",
                        visible: false
                    }
                ]
            }).dxDataGrid("instance");            
            // #endregion
            dataTableIndicator.option("visible", false);
        });
    }

    function initInvTmplTable() {
        dataTableIndicator.option("visible", true);
        $.get(GETDataReqStr, { ActionName: mInvCatData, ParmOne: curRecStack, ParmTwo: curRecQty, ParmThree: curDbId }, function (returnData) {
            invCatListArray = returnData;
            // #region Cấu hình bảng kê mẫu hđ
            $("#data-table").dxDataGrid({
                dataSource: invTmplStore,
                keyExpr: "Id",
                showBorders: true,
                hoverStateEnabled: true,
                paging: {
                    pageSize: 10
                },
                pager: {
                    showNavigationButtons: true
                },
                editing: {
                    mode: "popup",
                    allowAdding: true,
                    allowUpdating: true,
                    selectTextOnEditStart: true,
                    startEditAction: "dblClick",
                    popup: {
                        title: "Chi tiết mẫu hđ",
                        showTitle: true,
                        width: 1000,
                        height: "auto",
                        position: {
                            my: "top",
                            at: "top",
                            of: window
                        },
                        dragEnabled: false,
                        closeOnOutsideClick: true
                    },
                    form: {
                        colCount: 2,
                        items: [{
                            dataField: "InvCatId",
                            caption: "Loại hóa đơn"
                        }, {
                            dataField: "TmplName",
                            caption: "Tên mẫu"
                        }, {
                            dataField: "TmplCss",
                            caption: "Phần Css",
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 200
                            }
                        }, {
                            dataField: "TmplXml",
                            caption: "Phần Xml",
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 200
                            }
                        }, {
                            dataField: "TmplXslt",
                            caption: "Phần Xslt",
                            colSpan: 2,
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 200
                            }
                        }, {
                            dataField: "IsPub",
                            caption: "Trạng thái phát hành"
                        }, {
                            dataField: "IsCert",
                            caption: "Trạng thái duyệt"
                        }, {
                            dataField: "InvCatName",
                            caption: "Tên loại hóa đơn"
                        }, {
                            dataField: "TmplThumbnailDir",
                            caption: "Đường dẫn hình mẫu"
                        }, {
                            dataField: "SvcType",
                            caption: "Khóa kiểu dv",
                            colSpan: 2
                        }, {
                            dataField: "InvType",
                            caption: "Khóa kiểu hđ"
                        }, {
                            dataField: "InvView",
                            caption: "Khóa hiển thị hđ"
                        }, {
                            dataField: "iGenerator",
                            caption: "Khóa khởi tạo"
                        }, {
                            dataField: "iViewer",
                            caption: "Khóa hiển thị"
                        }]
                    }
                },
                columns: [
                    {
                        dataField: "InvCatId",
                        caption: "Loại hóa đơn",
                        width: 150,
                        validationRules: [{ type: "required" }],
                        lookup: {
                            dataSource: invCatListArray,
                            displayExpr: "Name",
                            valueExpr: "Id"
                        }
                    },
                    {
                        dataField: "TmplName",
                        caption: "Tên mẫu",
                        width: 100,
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "TmplCss",
                        caption: "Phần Css",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "TmplXml",
                        caption: "Phần Xml",
                        validationRules: [{ type: "required" }],
                        visible: false
                    },
                    {
                        dataField: "TmplXslt",
                        caption: "Phần Xslt",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "IsPub",
                        caption: "Trạng thái phát hành",
                        width: 150,
                        validationRules: [{ type: "required" }],
                        lookup: {
                            dataSource: pubSttArray,
                            displayExpr: "pubSttDes",
                            valueExpr: "pubSttId"
                        }
                    },
                    {
                        dataField: "InvCatName",
                        caption: "Tên loại hóa đơn",
                        visible: false
                    },
                    {
                        dataField: "SvcType",
                        caption: "Khóa kiểu dv",
                        visible: false
                    },
                    {
                        dataField: "InvType",
                        caption: "Khóa kiểu hđ",
                        visible: false
                    },
                    {
                        dataField: "InvView",
                        caption: "Khóa hiển thị hđ",
                        visible: false
                    },
                    {
                        dataField: "iGenerator",
                        caption: "Khóa khởi tạo",
                        visible: false
                    },
                    {
                        dataField: "iViewer",
                        caption: "Khóa hiển thị",
                        visible: false
                    },
                    {
                        dataField: "TmplThumbnailDir",
                        caption: "Đường dẫn hình mẫu",
                        visible: false
                    },
                    {
                        dataField: "IsCert",
                        caption: "Trạng thái duyệt",
                        lookup: {
                            dataSource: certSttArray,
                            displayExpr: "certSttDes",
                            valueExpr: "certSttId"
                        },
                        visible: false
                    }
                ]
            }).dxDataGrid("instance");            
            // #endregion
            dataTableIndicator.option("visible", false);
        });
    }

    function initTaxAuthTable() {
        if (curDbId == 1) {
            // #region Cấu hình bảng kê chi cục thuế
            $("#data-table").dxDataGrid({
                dataSource: taxAuthStore,
                keyExpr: "Code",
                showBorders: true,
                hoverStateEnabled: true,
                paging: {
                    pageSize: 10
                },
                pager: {
                    showNavigationButtons: true
                },
                editing: {
                    mode: "batch",
                    allowAdding: true,
                    allowUpdating: true,
                    selectTextOnEditStart: true,
                    startEditAction: "dblClick",
                },
                columns: [
                    {
                        dataField: "Code",
                        caption: "Mã",
                        width: 100,
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "Name",
                        caption: "Tên",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "Address",
                        caption: "Địa chỉ"
                    },
                    {
                        dataField: "Visibility",
                        caption: "Trạng thái sử dụng",
                        width: 150,
                        lookup: {
                            dataSource: taxAuthSttArray,
                            displayExpr: "taxAuthSttDes",
                            valueExpr: "taxAuthSttId"
                        }
                    },
                    {
                        dataField: "Phone",
                        caption: "Số điện thoại",
                        dataType: "number",
                        visible: false
                    }
                ]
            });
            // #endregion
        }
        else {
            // #region Cấu hình bảng kê chi cục thuế
            $("#data-table").dxDataGrid({
                dataSource: taxAuthStore,
                keyExpr: "Code",
                showBorders: true,
                hoverStateEnabled: true,
                paging: {
                    pageSize: 10
                },
                pager: {
                    showNavigationButtons: true
                },
                editing: {
                    mode: "batch",
                    allowAdding: true,
                    allowUpdating: true,
                    selectTextOnEditStart: true,
                    startEditAction: "dblClick",
                },
                columns: [
                    {
                        dataField: "Code",
                        caption: "Mã",
                        width: 100,
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "Name",
                        caption: "Tên",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "Address",
                        caption: "Địa chỉ"
                    },
                    {
                        dataField: "Locality",
                        caption: "Địa phương",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "Visibility",
                        caption: "Trạng thái sử dụng",
                        width: 150,
                        lookup: {
                            dataSource: taxAuthSttArray,
                            displayExpr: "taxAuthSttDes",
                            valueExpr: "taxAuthSttId"
                        }
                    },
                    {
                        dataField: "Phone",
                        caption: "Số điện thoại",
                        dataType: "number",
                        visible: false
                    }
                ]
            });
            // #endregion
        }
    }

    function initUnitTable() {
        // #region Cấu hình bảng kê đơn vị tính
        $("#data-table").dxDataGrid({
            dataSource: unitStore,
            keyExpr: "Id",
            showBorders: true,
            hoverStateEnabled: true,
            paging: {
                pageSize: 10
            },
            pager: {
                showNavigationButtons: true
            },
            editing: {
                mode: "batch",
                allowAdding: true,
                allowUpdating: true,
                selectTextOnEditStart: true,
                startEditAction: "dblClick",
            },
            columns: [
                {
                    dataField: "Code",
                    caption: "Mã",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "Name",
                    caption: "Tên",
                    validationRules: [{ type: "required" }]
                }
            ]
        });
        // #endregion
    }

    function initNotifTable() {
        // #region Cấu hình bảng kê thông báo hệ thống
        $("#data-table").dxDataGrid({
            dataSource: notifStore,
            keyExpr: "Id",
            showBorders: true,
            hoverStateEnabled: true,
            paging: {
                pageSize: 10
            },
            pager: {
                showNavigationButtons: true
            },
            editing: {
                mode: "popup",
                allowAdding: true,
                allowUpdating: true,
                popup: {
                    title: "Chi tiết thông báo hệ thống",
                    showTitle: true,
                    width: 700,
                    height: "auto",
                    position: {
                        my: "center",
                        at: "center",
                        of: window
                    },
                    dragEnabled: false,
                    closeOnOutsideClick: true
                },
                form: {
                    colCount: 1,
                    items: [{
                        dataField: "Title",
                        caption: "Tiêu đề",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 75
                        }
                    }, {
                        dataField: "Content",
                        caption: "Nội dung",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 200
                        }
                    }, {
                        dataField: "LnchDate",
                        caption: "Ngày bắt đầu"
                    }, {
                        dataField: "DismDate",
                        caption: "Ngày kết thúc"
                    }, {
                        dataField: "Stt",
                        caption: "Trạng thái"
                    }]
                }
            },
            columns: [
                {
                    dataField: "Title",
                    caption: "Tiêu đề",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "Content",
                    caption: "Nội dung",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "LnchDate",
                    caption: "Ngày bắt đầu",
                    width: 120,
                    alignment: "center",
                    dataType: "date",
                    format: 'dd.MM.yyyy',
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "DismDate",
                    caption: "Ngày kết thúc",
                    width: 120,
                    alignment: "center",
                    dataType: "date",
                    format: 'dd.MM.yyyy',
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "Stt",
                    caption: "Trạng thái sử dụng",
                    width: 150,
                    lookup: {
                        dataSource: notifSttArray,
                        displayExpr: "notifSttDes",
                        valueExpr: "notifSttId"
                    }
                }
            ]                
        });
        // #endregion
    }

    function initInvCatTable() {
        // #region Cấu hình bảng kê đơn vị tính
        $("#data-table").dxDataGrid({
            dataSource: invCatStore,
            keyExpr: "Id",
            showBorders: true,
            hoverStateEnabled: true,
            paging: {
                pageSize: 10
            },
            pager: {
                showNavigationButtons: true
            },
            editing: {
                mode: "batch",
                allowAdding: true,
                allowUpdating: true,
                selectTextOnEditStart: true,
                startEditAction: "dblClick",
            },
            columns: [
                {
                    dataField: "Name",
                    caption: "Tên",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "PatPrefix",
                    caption: "Phần đầu mẫu số",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "Desc",
                    caption: "Mô tả"
                },
                {
                    dataField: "Name",
                    caption: "Trạng thái"
                }
            ]
        });
        // #endregion
    }
});

function clearPageData() {
    leftEndStack = 0;
    rightEndStack = 0;
    curRecStack = 0;
}