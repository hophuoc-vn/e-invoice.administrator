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
var roleArray = [];
var funcArray = [];
var treeView;
var syncTreeViewSelection = function (treeView, value) {
    if (!value) {
        return;
    }

    value.forEach(function (key) {
        treeView.selectItem(key);
    });
};
var makeAsyncDataSource = function () {
    return new DevExpress.data.CustomStore({
        loadMode: "raw",
        key: "Id",
        load: function () {
            return funcArray;
        }
    });
};
var getSelectedItemsKeys = function (items) {
    var rawSelArray = [];
    var filteredSelArray = [];
    var result;
    items.forEach(function (item) {
        if (item.selected) {
            rawSelArray.push(item.key);
            if (item.parent != null || item.parent != undefined) {
                rawSelArray.push(item.parent.key);
            }
        }
        if (item.items.length) {
            rawSelArray = rawSelArray.concat(getSelectedItemsKeys(item.items));
        }
    });
    $.each(rawSelArray, function (i, item) {
        if ($.inArray(item, filteredSelArray) === -1) filteredSelArray.push(item);
    });
    result = filteredSelArray;
    return result;
};
// #endregion

$(document).ready(function () {
    $(function () {
        // #region Khởi tạo các LoadIndicator
        var userTableIndicator = $("#user-auth-table-indicator").dxLoadIndicator({
            height: 25,
            width: 25,
            visible: false
        }).dxLoadIndicator("instance");
        // #endregion
        // #region Khởi tạo bảng kê người dùng
        userTableIndicator.option("visible", true);
        $.get(GETDataReqStr, { ActionName: mRoleListData }, function (returnData) {
            roleArray = returnData;
            $.get(GETDataReqStr, { ActionName: mLoggedFuncData }, function (returnData) {
                funcArray = returnData;                            
                // #region Cấu hình custom store cho bảng kê người dùng
                var userStore = new DevExpress.data.CustomStore({
                    key: "Id",
                    load: function () {
                        return $.get(GETDataReqStr, { ActionName: mLoggedUserData }, function (returnData) {
                            if (returnData.Msg != undefined) {
                                dispMsg(returnData.Msg);
                            }
                        });
                    },
                    insert: function (values) {
                        return $.post(POSTDataReqStr, { ActionName: mUserIns, ParmOne: JSON.stringify(values) }, function (returnData) {
                            dispMsg(returnData);
                        });
                    },
                    update: function (key, values) {
                        return $.post(POSTDataReqStr, { ActionName: mUserUpdate, ParmOne: key, ParmTwo: JSON.stringify(values) }, function (returnData) {
                            dispMsg(returnData);
                        });
                    }
                });
                // #endregion
                // #region Cấu hình bảng kê người dùng
                $("#user-auth-table").dxDataGrid({
                    dataSource: userStore,
                    keyExpr: "Id",
                    showBorders: true,
                    hoverStateEnabled: true,
                    headerFilter: {
                        visible: true
                    },
                    paging: {
                        enabled: false
                    },
                    editing: {
                        mode: "popup",
                        allowAdding: true,
                        allowUpdating: true,
                        selectTextOnEditStart: true,
                        startEditAction: "dblClick",
                        popup: {
                            title: "Chi tiết người dùng",
                            showTitle: true,
                            width: 750,
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
                                caption: "Tên người dùng"
                            }, {
                                dataField: "RoleId",
                                caption: "Chức vụ"
                            }, {
                                dataField: "Funcs",
                                caption: "Chức năng"
                            }, {
                                dataField: "Flag",
                                caption: "Cờ"
                            }]
                        }
                    },
                    columns: [
                        {
                            dataField: "Name",
                            caption: "Tên người dùng",
                            allowHeaderFiltering: false,
                            validationRules: [{ type: "required" }]
                        },
                        {
                            dataField: "RoleId",
                            caption: "Chức vụ",
                            lookup: {
                                dataSource: roleArray,
                                displayExpr: "Name",
                                valueExpr: "Id"
                            },
                            validationRules: [{ type: "required" }]
                        },
                        {
                            dataField: "Funcs",
                            caption: "Chức năng",
                            allowHeaderFiltering: false,
                            editCellTemplate: dropDownBoxEditorTemplate,
                            lookup: {
                                dataSource: funcArray,
                                valueExpr: "Id",
                                displayExpr: "Id"
                            },
                            validationRules: [{ type: "required" }]
                        },
                        {
                            dataField: "CreateDate",
                            caption: "Ngày khởi tạo",
                            width: 200,
                            alignment: "center",
                            allowHeaderFiltering: false
                        },
                        {
                            dataField: "ModifyDate",
                            caption: "Ngày điều chỉnh",
                            width: 200,
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
                userTableIndicator.option("visible", false);
                // #endregion
            });
        });
        // #endregion
    });
});

function dropDownBoxEditorTemplate(cellElement, cellInfo) {
    return $("<div>").dxDropDownBox({
        value: cellInfo.value,
        valueExpr: "Id",
        displayExpr: "Id",
        placeholder: "Chọn một giá trị...",
        showClearButton: true,
        dataSource: makeAsyncDataSource(),
        contentTemplate: function (e) {
            var value = e.component.option("value"),
                $treeView = $("<div>").dxTreeView({
                    dataSource: e.component.option("dataSource"),
                    dataStructure: "plain",
                    keyExpr: "Id",
                    parentIdExpr: "RootId",
                    selectionMode: "multiple",
                    displayExpr: "Des",
                    onContentReady: function (args) {
                        syncTreeViewSelection(args.component, value);
                    },
                    selectNodesRecursive: false,
                    showCheckBoxesMode: "normal",
                    onItemSelectionChanged: function (args) {
                        var nodes = args.component.getNodes(),
                            value = getSelectedItemsKeys(nodes);

                        e.component.option("value", value);
                        cellInfo.setValue(value);
                    }
                });

            treeView = $treeView.dxTreeView("instance");

            e.component.on("valueChanged", function (args) {
                var value = args.value;
                syncTreeViewSelection(treeView, value);
            });

            return $treeView;
        }
    });
}