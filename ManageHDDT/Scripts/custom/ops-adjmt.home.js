// #region Khai báo hằng số
const flagArray = [{
    "FlagVal": "false",
    "FlagDes": "Hoạt động"
}, {
    "FlagVal": "true",
    "FlagDes": "Vô hiệu"
}];
// #endregion
// #region Khai báo biến số
var formData;
var rankArray = [];
var userArray = [];
// #endregion

$.get(GETDataReqStr, { ActionName: mUserDetailsData }, function (returnData) {
    formData = returnData;
});

$(document).ready(function () {
    $(function () {
        // #region Khởi tạo các LoadIndicator
        var roleTableIndicator = $("#role-auth-table-indicator").dxLoadIndicator({
            height: 25,
            width: 25,
            visible: false
        }).dxLoadIndicator("instance");
        // #endregion
        // #region Tạo form điều chỉnh thông tin
        $("#form-container").dxForm({
            formData: formData,
            colCount: 2,
            readOnly: false,
            showColonAfterLabel: true,
            showValidationSummary: true,
            validationGroup: "userDetails",
            items: [{
                itemType: "group",
                items: [{
                    dataField: "LoginTkn",
                    label: {
                        text: "Mật khẩu",
                    },
                    editorOptions: {
                        mode: "password",
                        onContentReady: function (info) {
                            $(info.element).find("input").attr("autocomplete", "new-password");
                        },
                    },
                    validationRules: [{
                        type: "required",
                        message: "Mật khẩu là bắt buộc"
                    }]
                }]
            }, {
                itemType: "group",
                items: [{
                    dataField: "EmailAddr",
                    label: {
                        text: "Email"
                    },
                    validationRules: [{
                        type: "required",
                        message: "Email là bắt buộc"
                    }, {
                        type: "email",
                        message: "Email không hợp lệ"
                        //}, {
                        //    type: "async",
                        //    message: "Email is already registered",
                        //    validationCallback: function (params) {
                        //        return sendRequest(params.value);
                        //    }
                    }]
                }]
            }, {
                colSpan: 2,
                itemType: "button",
                horizontalAlignment: "center",
                buttonOptions: {
                    text: "Cập nhật",
                    type: "success",
                    onClick: function () {
                        $.post(POSTDataReqStr, { ActionName: mUserDetailsUpdate, ParmOne: JSON.stringify(formData) }, function (returnData) {
                            dispMsg(returnData);
                        });
                    }
                }
            }]
        });
        // #endregion
        // #region Bắt sự kiện mở accordion Chức vụ
        $('#collapseTwo').on('show.bs.collapse', function () {
            roleTableIndicator.option("visible", true);
            $.get(GETDataReqStr, { ActionName: mRankData }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
                else {
                    rankArray = returnData;
                    $.get(GETDataReqStr, { ActionName: mUserData }, function (returnData) {
                        if (returnData.Msg != undefined) {
                            dispMsg(returnData.Msg);
                        }
                        else {
                            userArray = returnData;
                            // #region Cấu hình custom store cho bảng kê chức vụ
                            var roleStore = new DevExpress.data.CustomStore({
                                key: "Id",
                                load: function () {
                                    return $.get(GETDataReqStr, { ActionName: mRoleData }, function (returnData) {
                                        if (returnData.Msg != undefined) {
                                            dispMsg(returnData.Msg);
                                        }
                                    });
                                },
                                insert: function (values) {
                                    return $.post(POSTDataReqStr, { ActionName: mRoleIns, ParmOne: JSON.stringify(values) }, function (returnData) {
                                        dispMsg(returnData);
                                    });
                                },
                                update: function (key, values) {
                                    return $.post(POSTDataReqStr, { ActionName: mRoleUpdate, ParmOne: key, ParmTwo: JSON.stringify(values) }, function (returnData) {
                                        dispMsg(returnData);
                                    });
                                }
                            });
                            // #endregion
                            // #region Cấu hình bảng kê chức vụ
                            $("#role-auth-table").dxDataGrid({
                                dataSource: roleStore,
                                keyExpr: "Id",
                                showBorders: true,
                                hoverStateEnabled: true,
                                headerFilter: {
                                    visible: true
                                },
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
                                        title: "Chi tiết chức vụ",
                                        showTitle: true,
                                        width: 400,
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
                                            dataField: "Name",
                                            caption: "Tên chức vụ"
                                        }, {
                                            dataField: "Rank",
                                            caption: "Cấp"
                                        }, {
                                            dataField: "User",
                                            caption: "Người dùng"
                                        }, {
                                            dataField: "Flag",
                                            caption: "Cờ"
                                        }]
                                    }
                                },
                                columns: [
                                    {
                                        dataField: "Name",
                                        caption: "Tên chức vụ",
                                        validationRules: [{ type: "required" }]
                                    },
                                    {
                                        dataField: "Rank",
                                        caption: "Cấp",
                                        lookup: {
                                            dataSource: rankArray,
                                            displayExpr: "Name",
                                            valueExpr: "Value"
                                        },
                                        validationRules: [{ type: "required" }]
                                    },
                                    {
                                        dataField: "User",
                                        caption: "Người dùng",
                                        allowHeaderFiltering: false,
                                        lookup: {
                                            dataSource: userArray,
                                            displayExpr: "Name",
                                            valueExpr: "Id"
                                        }
                                    },
                                    {
                                        dataField: "CreateDate",
                                        caption: "Ngày khởi tạo",
                                        width: 150,
                                        alignment: "center",
                                        allowHeaderFiltering: false
                                    },
                                    {
                                        dataField: "ModifyDate",
                                        caption: "Ngày điều chỉnh",
                                        width: 150,
                                        alignment: "center",
                                        allowHeaderFiltering: false
                                    },
                                    {
                                        dataField: "Flag",
                                        caption: "Cờ",
                                        width: 100,
                                        allowHeaderFiltering: false,
                                        lookup: {
                                            dataSource: flagArray,
                                            displayExpr: "FlagDes",
                                            valueExpr: "FlagVal"
                                        }
                                    }
                                ]
                            }).dxDataGrid("instance");
                            roleTableIndicator.option("visible", false);
                        // #endregion
                        }                        
                    });
                }                
            });            
        });
        // #endregion
        // #region Bắt sự kiện mở accordion Cấp bậc
        $('#collapseThree').on('show.bs.collapse', function () {
            // #region Cấu hình custom store cho bảng kê cấp bậc
            var rankStore = new DevExpress.data.CustomStore({
                key: "Id",
                load: function () {
                    return $.get(GETDataReqStr, { ActionName: mRankData }, function (returnData) {
                        if (returnData.Msg != undefined) {
                            dispMsg(returnData.Msg);
                        }
                    });
                },
                insert: function (values) {
                    return $.post(POSTDataReqStr, { ActionName: mRankIns, ParmOne: JSON.stringify(values) }, function (returnData) {
                        dispMsg(returnData);
                    });
                },
                update: function (key, values) {
                    return $.post(POSTDataReqStr, { ActionName: mRankUpdate, ParmOne: key, ParmTwo: JSON.stringify(values) }, function (returnData) {
                        dispMsg(returnData);
                    });
                }
            });
            // #endregion
            // #region Cấu hình bảng kê cấp bậc
            $("#rank-auth-table").dxDataGrid({
                dataSource: rankStore,
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
                        title: "Chi tiết cấp bậc",
                        showTitle: true,
                        width: 400,
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
                            dataField: "Value",
                            caption: "Cấp bậc"
                        }, {
                            dataField: "Name",
                            caption: "Diễn giải",
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 75
                            }
                        }, {
                            dataField: "IsMaster",
                            caption: "Cấp tối cao",
                            editorType: "dxCheckBox"
                        }, {
                            dataField: "Flag",
                            caption: "Cờ"
                        }]
                    }
                },
                columns: [
                    {
                        dataField: "Value",
                        caption: "Cấp bậc",
                        dataType: "number",
                        width: 80,
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "Name",
                        caption: "Diễn giải",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "IsMaster",
                        caption: "Cấp tối cao",
                        width: 120
                    },
                    {
                        dataField: "CreateDate",
                        caption: "Ngày khởi tạo",
                        width: 150,
                        alignment: "center"
                    },
                    {
                        dataField: "ModifyDate",
                        caption: "Ngày điều chỉnh",
                        width: 150,
                        alignment: "center"
                    },
                    {
                        dataField: "Flag",
                        caption: "Cờ",
                        width: 100,
                        lookup: {
                            dataSource: flagArray,
                            displayExpr: "FlagDes",
                            valueExpr: "FlagVal"
                        }
                    }
                ]
            });
            // #endregion                        
        });
        // #endregion
        // #region Bắt sự kiện mở accordion Chức năng
        $('#collapseFour').on('show.bs.collapse', function () {
            // #region Cấu hình custom store cho bảng kê chức năng
            var funcStore = new DevExpress.data.CustomStore({
                key: "Id",
                load: function () {
                    return $.get(GETDataReqStr, { ActionName: mFuncData }, function (returnData) {
                        if (returnData.Msg != undefined) {
                            dispMsg(returnData.Msg);
                        }
                    });
                },
                insert: function (values) {
                    return $.post(POSTDataReqStr, { ActionName: mFuncIns, ParmOne: JSON.stringify(values) }, function (returnData) {
                        dispMsg(returnData);
                    });
                },
                update: function (key, values) {
                    return $.post(POSTDataReqStr, { ActionName: mFuncUpdate, ParmOne: key, ParmTwo: JSON.stringify(values) }, function (returnData) {
                        dispMsg(returnData);
                    });
                }
            });
            // #endregion
            // #region Cấu hình bảng kê chức năng
            $("#func-auth-table").dxDataGrid({
                dataSource: funcStore,
                keyExpr: "Id",
                showBorders: true,
                hoverStateEnabled: true,
                paging: {
                    pageSize: 10
                },
                pager: {
                    showNavigationButtons: true
                },
                searchPanel: {
                    visible: true,
                    width: 250
                },
                editing: {
                    mode: "popup",
                    allowAdding: true,
                    allowUpdating: true,
                    selectTextOnEditStart: true,
                    startEditAction: "dblClick",
                    popup: {
                        title: "Chi tiết chức năng",
                        showTitle: true,
                        width: 500,
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
                            dataField: "Id",
                            caption: "Số đd"
                        }, {
                            dataField: "RootId",
                            caption: "Số đd nguồn"
                        }, {
                            dataField: "Name",
                            caption: "Tên chức năng"
                        }, {
                            dataField: "Des",
                            caption: "Diễn giải",
                            editorType: "dxTextArea",
                            editorOptions: {
                                height: 75
                            }
                        }, {
                            dataField: "IsAuthed",
                            caption: "Xét quyền truy vấn",
                            editorType: "dxCheckBox"
                        }, {
                            dataField: "Flag",
                            caption: "Cờ"
                        }]
                    }
                },
                columns: [
                    {
                        dataField: "Id",
                        caption: "Số đd",
                        width: 80,
                        dataType: "number",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "RootId",
                        caption: "Số đd nguồn",
                        width: 100,
                        dataType: "number"
                    },
                    {
                        dataField: "Name",
                        caption: "Tên chức năng",
                        width: 250
                    },
                    {
                        dataField: "Des",
                        caption: "Diễn giải",
                        validationRules: [{ type: "required" }]
                    },
                    {
                        dataField: "IsAuthed",
                        caption: "Xét quyền truy vấn",
                        width: 150
                    },
                    {
                        dataField: "CreateDate",
                        caption: "Ngày khởi tạo",
                        width: 150,
                        alignment: "center"
                    },
                    {
                        dataField: "ModifyDate",
                        caption: "Ngày điều chỉnh",
                        width: 150,
                        alignment: "center"
                    },
                    {
                        dataField: "Flag",
                        caption: "Cờ",
                        width: 100,
                        lookup: {
                            dataSource: flagArray,
                            displayExpr: "FlagDes",
                            valueExpr: "FlagVal"
                        }
                    }
                ]
            });
            // #endregion            
        });
        // #endregion
    });
});