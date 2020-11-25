// #region Khai báo hằng số
const pubSttArray = [{
    "SttId": 0,
    "SttDes": "Chưa được duyệt"
}, {
    "SttId": 2,
    "SttDes": "Được duyệt"
}, {
    "SttId": 3,
    "SttDes": "Bị hủy"
}];
const emailSttArray = [{
    "SttId": 0,
    "SttDes": "Chưa gửi"
}, {
    "SttId": 1,
    "SttDes": "Đã gửi"
}, {
    "SttId": 2,
    "SttDes": "Gửi lỗi"
}, {
    "SttId": 3,
    "SttDes": "Gửi lại"
}];
const themeArray = [
    "default",
    "abcdef",
    "blackboard",
    "cobalt",
    "darcula",
    "duotone-light",
    "eclipse",
    "elegant",
    "idea",
    "mbo",
    "mdn-like",
    "neat",
    "neo",
    "night",
    "pastel-on-dark",
    "rubyblue",
    "the-matrix",
    "ttcn",
    "vibrant-ink",
    "yonce"
];
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
// #endregion
// #region Khai báo biến số
var curDate = new Date();
var curDbId = 2;
var curCoId;
var curInvId = "";
var curCusRef;
var curScrollX;
var curScrollY;
var newCusRef;
var coFormData;
var coListByCusRefArray = [];
var coListArray = [];
var coPubArray = [];
var pubTmplArray = [];
var coInvListArray = [];
var coInvPubStEmailArray = [];
var coDigiCertArray = [];
var coConfigArray = [];
// #endregion

$(document).ready(function () {
    // #region Khởi tạo các LoadIndicator
    var coDataInd = $("#co-data-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    var coListDropDownInd = $("#co-list-dropdown-indicator").dxLoadIndicator({
        height: 35,
        width: 35,
        visible: false
    }).dxLoadIndicator("instance");
    var coListSelectInd = $("#co-list-select-indicator").dxLoadIndicator({
        height: 35,
        width: 35,
        visible: false
    }).dxLoadIndicator("instance");
    var cusSearchInd = $("#cus-search-indicator").dxLoadIndicator({
        height: 35,
        width: 35,
        visible: false
    }).dxLoadIndicator("instance");
    var pubTmplInd = $("#pub-tmpl-select-indicator").dxLoadIndicator({
        height: 35,
        width: 35,
        visible: false
    }).dxLoadIndicator("instance");
    var coInvListDropDownInd = $("#co-list-dropdown-indicator").dxLoadIndicator({
        height: 35,
        width: 35,
        visible: false
    }).dxLoadIndicator("instance");
    // #endregion
    // #region Khởi tạo các CustomStore thao tác dữ liệu
    var makeAsyncDataSourceCo = function () {
        return new DevExpress.data.CustomStore({
            loadMode: "raw",
            key: "CoId",
            load: function () {
                return coListArray;
            }
        });
    };
    var makeAsyncDataSourceInv = function () {
        return new DevExpress.data.CustomStore({
            loadMode: "raw",
            key: "InvId",
            load: function () {
                return coInvListArray;
            }
        });
    };
    var pubStore = new DevExpress.data.CustomStore({
        key: "PubTmplId",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mCoPubData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
                else {
                    coPubArray = returnData
                }
            });
        },
        update: function (key, values) {
            return $.post(POSTDataReqStr, { ActionName: mCoPubUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var emailStore = new DevExpress.data.CustomStore({
        key: "EmailId",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mCoInvPubStEmailData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
                else {
                    coInvPubStEmailArray = returnData
                }
            });
        },
        update: function (key, values) {
            return $.post(POSTAuthedDataReqStr, { ActionName: mCoInvPubStEmailUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var certStore = new DevExpress.data.CustomStore({
        key: "Id",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mCoDigiCertData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
                else {
                    coDigiCertArray = returnData
                }
            });
        },
        insert: function (values) {
            return $.post(POSTDataReqStr, { ActionName: mCoDigiCertIns, ParmOne: curCoId, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    var cfgStore = new DevExpress.data.CustomStore({
        key: "Id",
        load: function () {
            return $.get(GETDataReqStr, { ActionName: mCoConfigData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
                else {
                    coConfigArray = returnData
                }
            });
        },
        insert: function (values) {
            return $.post(POSTDataReqStr, { ActionName: mCoConfigIns, ParmOne: curCoId, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        },
        update: function (key, values) {
            return $.post(POSTDataReqStr, { ActionName: mCoConfigUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    });
    // #endregion
    // #region Khởi tạo SelectBox chọn csdl hđđt
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
            $("#co-list-dropdown").remove();
            $("#co-list-dropdown-indicator").before('<div id="co-list-dropdown"></div>');
            $("#inv-list-dropdown").remove();
            $("#inv-list-dropdown-indicator").before('<div id="inv-list-dropdown"></div>');
            clearCoData();
            clearPageData();
            $.get(GETDataReqStr, { ActionName: mCoListData, ParmOne: curDbId }, function (returnData) {
                coListArray = returnData;                
                // #region Cấu hình DropDownBox chọn công ty theo số định danh
                $("#co-list-dropdown").dxDropDownBox({
                    valueExpr: "CoId",
                    deferRendering: false,
                    placeholder: "Danh sách công ty...",
                    displayExpr: function (item) {
                        return item.CoName;
                    },
                    showClearButton: true,
                    dataSource: makeAsyncDataSourceCo(),
                    onValueChanged(e) {
                        if (e.value === null) {
                            curCoId = undefined;
                        }
                    },
                    contentTemplate: function (e) {
                        var value = e.component.option("value"),
                            $dataGrid = $("<div>").dxDataGrid({
                                dataSource: e.component.option("dataSource"),
                                hoverStateEnabled: true,
                                columns: [
                                    {
                                        dataField: "CoId",
                                        caption: "Số định danh",
                                        alignment: "center",
                                        width: 100
                                    },
                                    {
                                        dataField: "CoName",
                                        caption: "Tên công ty"
                                    },
                                    {
                                        dataField: "CoDomain",
                                        caption: "Tên miền"
                                    }
                                ],
                                hoverStateEnabled: true,
                                paging: { enabled: true, pageSize: 10 },
                                filterRow: { visible: true },
                                scrolling: { mode: "infinite" },
                                selection: { mode: "single" },
                                selectedRowKeys: [value],
                                height: "100%",
                                onSelectionChanged: function (selectedItems) {
                                    var keys = selectedItems.selectedRowKeys,
                                        hasSelection = keys.length;

                                    e.component.option("value", hasSelection ? keys[0] : null);
                                    e.component.close();
                                    if (hasSelection) {
                                        coListDropDownInd.option('visible', true);
                                        curCoId = keys[0];
                                        genPageData();
                                        coListDropDownInd.option('visible', false);
                                    }
                                }
                            });

                        dataGrid = $dataGrid.dxDataGrid("instance");

                        e.component.on("valueChanged", function (args) {
                            dataGrid.selectRows(args.value, false);
                        });

                        return $dataGrid;
                    }
                });
                // #endregion
                // #region Khởi tạo DropDownBox danh sách hóa đơn theo mẫu hđ
                $("#inv-list-dropdown").dxDropDownBox({
                    deferRendering: false,
                    placeholder: "Danh sách hóa đơn...",
                    onInitialized: function (e) {
                        setTimeout(function () {
                            e.element.find(".dx-texteditor-container").on("dxclick", function (event) {
                                if (!e.component.getDataSource().items().length) {
                                    event.stopPropagation();
                                }
                            })
                        }, 300);
                    }
                });
                // #endregion
            });            
        }
    });
    // #endregion
    // #region Khởi tạo DropDownBox chọn công ty theo số định danh
    $.get(GETDataReqStr, { ActionName: mCoListData, ParmOne: curDbId }, function (returnData) {
        coListArray = returnData;
        $("#co-list-dropdown").dxDropDownBox({
            valueExpr: "CoId",
            deferRendering: false,
            placeholder: "Danh sách công ty...",
            displayExpr: function (item) {
                return item.CoName;
            },
            showClearButton: true,
            dataSource: makeAsyncDataSourceCo(),
            onValueChanged(e) {
                if (e.value === null) {
                    curCoId = undefined;
                }
            },
            contentTemplate: function (e) {
                var value = e.component.option("value"),
                    $dataGrid = $("<div>").dxDataGrid({
                        dataSource: e.component.option("dataSource"),
                        hoverStateEnabled: true,
                        columns: [
                            {
                                dataField: "CoId",
                                caption: "Số định danh",
                                alignment: "center",
                                width: 100
                            },
                            {
                                dataField: "CoName",
                                caption: "Tên công ty"
                            },
                            {
                                dataField: "CoDomain",
                                caption: "Tên miền"
                            }
                        ],
                        hoverStateEnabled: true,
                        paging: { enabled: true, pageSize: 10 },
                        filterRow: { visible: true },
                        scrolling: { mode: "infinite" },
                        selection: { mode: "single" },
                        selectedRowKeys: [value],
                        height: "100%",
                        onSelectionChanged: function (selectedItems) {
                            var keys = selectedItems.selectedRowKeys,
                                hasSelection = keys.length;

                            e.component.option("value", hasSelection ? keys[0] : null);
                            e.component.close();
                            if (hasSelection) {
                                coDataInd.option('visible', true);
                                curCoId = keys[0];
                                genPageData();
                            }
                        }
                    });

                dataGrid = $dataGrid.dxDataGrid("instance");

                e.component.on("valueChanged", function (args) {
                    dataGrid.selectRows(args.value, false);
                });

                return $dataGrid;
            }
        });
    });
    // #endregion
    // #region Khởi tạo Input tìm công ty theo mã kh
    $("#cus-search-box").dxTextBox({
        placeholder: "Ví dụ: MB123456..",
        mode: "search",
        showClearButton: true,
        onValueChanged: function (e) {
            var previousValue = e.previousValue;
            var newValue = e.value;
            // Event handling commands go here
            curCusRef = previousValue;
            newCusRef = newValue;
        },
        onEnterKey: function (e) {
            curCusRef = newCusRef;
            // Event handling commands go here
            cusSearchInd.option('visible', true);
            $.get(GETDataReqStr, { ActionName: mCoListByCusRefData, ParmOne: curCusRef, ParmTwo: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    cusSearchInd.option('visible', false);
                    dispMsg(returnData.Msg);
                }
                else {                        
                    coListByCusRefArray = returnData;
                    $("#co-list-select").dxSelectBox({
                        dataSource: new DevExpress.data.ArrayStore({
                            data: coListByCusRefArray,
                            key: "CoId"
                        }),
                        displayExpr: "CoDomain",
                        valueExpr: "CoId",
                        onValueChanged: function (data) {
                            coListSelectInd.option('visible', true);
                            curCoId = data.value;
                            genPageData();
                            coListSelectInd.option('visible', false);
                        }
                    });
                    cusSearchInd.option('visible', false);
                }
            });
        }
    });
    // #endregion
    // #region Khởi tạo Button tìm công ty theo mã kh
    $("#cus-search-btn").dxButton({
        icon: "search",
        onClick: function (e) {
            curCusRef = newCusRef;
            cusSearchInd.option('visible', true);
            $.get(GETDataReqStr, { ActionName: mCoListByCusRefData, ParmOne: curCusRef, ParmTwo: curDbId }, function (returnData) {
                if (returnData.Msg != undefined) {
                    cusSearchInd.option('visible', false);
                    dispMsg(returnData.Msg);
                }
                else {
                    coListByCusRefArray = returnData;
                    $("#co-list-select").dxSelectBox({
                        dataSource: new DevExpress.data.ArrayStore({
                            data: coListByCusRefArray,
                            key: "CoId"
                        }),
                        displayExpr: "CoDomain",
                        valueExpr: "CoId",
                        onValueChanged: function (data) {
                            coListSelectInd.option('visible', true);
                            curCoId = data.value;
                            genPageData();
                            coListSelectInd.option('visible', false);
                        }
                    });
                    cusSearchInd.option('visible', false);
                }
            });
        }
    });
    // #endregion    
    // #region Khởi tạo SelectBox chọn công ty theo mã kh
    $("#co-list-select").dxSelectBox();
    // #endregion
    // #region Khởi tạo bảng kê thông báo phát hành
    var pubGrid = $("#pub-grid").dxDataGrid({
        showBorders: true,
        columns: [{
            caption: "Mẫu số"
        }, {
            caption: "Ký hiệu"
        }, {
            caption: "Ngày phát hành"
        }, {
            caption: "Trạng thái",
            alignment: "center"
        }, {
            alignment: "center"
        }]
    }).dxDataGrid("instance");
    // #endregion
    // #region Khởi tạo các selector cập nhật mẫu hđ
    var pubTmplSelector = $("#pub-tmpl-select").dxSelectBox().dxSelectBox("instance");

    $("#theme-select").dxSelectBox({
        items: themeArray,
        value: themeArray[0],
        onValueChanged: function (data) {
            tmplCssEditor.setOption("theme", data.value);
            logoCssEditor.setOption("theme", data.value);
            bgrCssEditor.setOption("theme", data.value);
            tmplHtmlEditor.setOption("theme", data.value);
        }
    });
    $("#inv-list-dropdown").dxDropDownBox({
        deferRendering: false,
        placeholder: "Danh sách hóa đơn...",
        onInitialized: function (e) {
            setTimeout(function () {
                e.element.find(".dx-texteditor-container").on("dxclick", function (event) {
                    if (!e.component.getDataSource().items().length) {
                        event.stopPropagation();
                    }
                })
            }, 300);
        }
    });
    // #endregion
    // #region Khởi tạo editor TmplCss sửa Css mẫu hđ
    const tmplCssEditor = CodeMirror.fromTextArea(document.getElementById('tmpl-css'), {
        mode: "css",
        autoCloseBrackets: true,
        autoRefresh: true,
        extraKeys: {
            "Ctrl-O": cm => CodeMirror.commands.foldAll(cm),
            "Ctrl-P": cm => CodeMirror.commands.unfoldAll(cm)
        },
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        lineNumbers: true,
        styleActiveLine: true,
        styleSelectedText: true
    });

    tmplCssEditor.setSize(null, 320);
    // #endregion
    // #region Khởi tạo các nút tính năng cho editor TmplCss
    $("#tmpl-css-undo-btn").dxButton({
        icon: 'undo',
        hint: "Bỏ tác vụ",
        onClick: function (e) {
            tmplCssEditor.undo();
        }
    });

    $("#tmpl-css-redo-btn").dxButton({
        icon: 'redo',
        hint: "Tái tác vụ",
        onClick: function (e) {
            tmplCssEditor.redo();
        }
    });

    $("#tmpl-css-fold-btn").dxButton({
        icon: 'collapse',
        hint: "Gập tất cả",
        onClick: function (e) {
            CodeMirror.commands.foldAll(tmplCssEditor);
        }
    });

    $("#tmpl-css-unfold-btn").dxButton({
        icon: 'expand',
        hint: "Bung tất cả",
        onClick: function (e) {
            CodeMirror.commands.unfoldAll(tmplCssEditor);
        }
    });

    $("#tmpl-css-beautify-btn").dxButton({
        icon: 'increaseindent',
        hint: "Xếp",
        onClick: function (e) {
            var tmplCssBefore = tmplCssEditor.getDoc().getValue();
            var tmplCssAfter = css_beautify(tmplCssBefore);
            tmplCssEditor.getDoc().setValue(tmplCssAfter);
        }
    });
    // #endregion
    // #region Khởi tạo editor LogoCss sửa Logo mẫu hđ
    const logoCssEditor = CodeMirror.fromTextArea(document.getElementById('logo-css'), {
        mode: "css",
        autoCloseBrackets: true,
        autoRefresh: true,
        extraKeys: {
            "Ctrl-O": cm => CodeMirror.commands.foldAll(cm),
            "Ctrl-P": cm => CodeMirror.commands.unfoldAll(cm)
        },
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        lineNumbers: true,
        styleActiveLine: true,
        styleSelectedText: true
    });

    logoCssEditor.setSize(null, 320);
    // #endregion
    // #region Khởi tạo các nút tính năng cho editor LogoCss
    $("#logo-css-undo-btn").dxButton({
        icon: 'undo',
        hint: "Bỏ tác vụ",
        onClick: function (e) {
            logoCssEditor.undo();
        }
    });

    $("#logo-css-redo-btn").dxButton({
        icon: 'redo',
        hint: "Tái tác vụ",
        onClick: function (e) {
            logoCssEditor.redo();
        }
    });

    $("#logo-css-fold-btn").dxButton({
        icon: 'collapse',
        hint: "Gập tất cả",
        onClick: function (e) {
            CodeMirror.commands.foldAll(logoCssEditor);
        }
    });

    $("#logo-css-unfold-btn").dxButton({
        icon: 'expand',
        hint: "Bung tất cả",
        onClick: function (e) {
            CodeMirror.commands.unfoldAll(logoCssEditor);
        }
    });

    $("#logo-css-beautify-btn").dxButton({
        icon: 'increaseindent',
        hint: "Xếp",
        onClick: function (e) {
            var logoCssBefore = logoCssEditor.getDoc().getValue();
            var logoCssAfter = css_beautify(logoCssBefore);
            logoCssEditor.getDoc().setValue(logoCssAfter);
        }
    });
    // #endregion
    // #region Khởi tạo editor BgrCss sửa Background mẫu hđ
    const bgrCssEditor = CodeMirror.fromTextArea(document.getElementById('bgr-css'), {
        mode: "css",
        autoCloseBrackets: true,
        autoRefresh: true,
        extraKeys: {
            "Ctrl-O": cm => CodeMirror.commands.foldAll(cm),
            "Ctrl-P": cm => CodeMirror.commands.unfoldAll(cm)
        },
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        lineNumbers: true,
        styleActiveLine: true,
        styleSelectedText: true
    });

    bgrCssEditor.setSize(null, 320);
    // #endregion
    // #region Khởi tạo các nút tính năng cho editor BgrCss
    $("#bgr-css-undo-btn").dxButton({
        icon: 'undo',
        hint: "Bỏ tác vụ",
        onClick: function (e) {
            bgrCssEditor.undo();
        }
    });

    $("#bgr-css-redo-btn").dxButton({
        icon: 'redo',
        hint: "Tái tác vụ",
        onClick: function (e) {
            bgrCssEditor.redo();
        }
    });

    $("#bgr-css-fold-btn").dxButton({
        icon: 'collapse',
        hint: "Gập tất cả",
        onClick: function (e) {
            CodeMirror.commands.foldAll(bgrCssEditor);
        }
    });

    $("#bgr-css-unfold-btn").dxButton({
        icon: 'expand',
        hint: "Bung tất cả",
        onClick: function (e) {
            CodeMirror.commands.unfoldAll(bgrCssEditor);
        }
    });

    $("#bgr-css-beautify-btn").dxButton({
        icon: 'increaseindent',
        hint: "Xếp",
        onClick: function (e) {
            var bgrCssBefore = bgrCssEditor.getDoc().getValue();
            var bgrCssAfter = css_beautify(bgrCssBefore);
            bgrCssEditor.getDoc().setValue(bgrCssAfter);
        }
    });
    // #endregion
    // #region Khởi tạo editor TmplHtml sửa Html mẫu hđ
    const tmplHtmlEditor = CodeMirror.fromTextArea(document.getElementById('tmpl-html'), {
        mode: "htmlmixed",
        autoCloseBrackets: true,
        autoCloseTags: true,
        autoRefresh: true,
        extraKeys: {
            "Ctrl-O": cm => CodeMirror.commands.foldAll(cm),
            "Ctrl-P": cm => CodeMirror.commands.unfoldAll(cm)
        },
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        lineNumbers: true,
        styleActiveLine: true,
        styleSelectedText: true
    });

    tmplHtmlEditor.setSize(null, 500);
    // #endregion                
    // #region Khởi tạo các nút tính năng cho editor TmplHtml
    $("#tmpl-html-undo-btn").dxButton({
        icon: 'undo',
        hint: "Bỏ tác vụ",
        onClick: function (e) {
            tmplHtmlEditor.undo();
        }
    });

    $("#tmpl-html-redo-btn").dxButton({
        icon: 'redo',
        hint: "Tái tác vụ",
        onClick: function (e) {
            tmplHtmlEditor.redo();
        }
    });

    $("#tmpl-html-fold-btn").dxButton({
        icon: 'collapse',
        hint: "Gập tất cả",
        onClick: function (e) {
            CodeMirror.commands.foldAll(tmplHtmlEditor);
        }
    });

    $("#tmpl-html-unfold-btn").dxButton({
        icon: 'expand',
        hint: "Bung tất cả",
        onClick: function (e) {
            CodeMirror.commands.unfoldAll(tmplHtmlEditor);
        }
    });

    $("#tmpl-html-beautify-btn").dxButton({
        icon: 'increaseindent',
        hint: "Xếp",
        onClick: function (e) {
            var tmplHtmlBefore = tmplHtmlEditor.getDoc().getValue();
            var tmplHtmlAfter = html_beautify(tmplHtmlBefore);
            tmplHtmlEditor.getDoc().setValue(tmplHtmlAfter);
        }
    });
    // #endregion
    // #region Khởi tạo các nút chức năng mẫu hđ
    var tmplUpdateBtn = $("#tmpl-update-btn").dxButton({
        icon: 'save',
        text: "Cập nhật mẫu hđ",
        disabled: true,
        onClick: function (e) {
            var pubTmplId = pubTmplSelector.option('value');
            var tmplUpdateData = new TmplDataModel(tmplCssEditor.getDoc().getValue(), logoCssEditor.getDoc().getValue(), bgrCssEditor.getDoc().getValue(), tmplHtmlEditor.getDoc().getValue());
            $.post(POSTAuthedDataReqStr, { ActionName: mPubTmplUpdate, ParmOne: pubTmplId, ParmTwo: JSON.stringify(tmplUpdateData), ParmThree: curDbId }, function (returnData) {
                dispMsg(returnData);
            });
        }
    }).dxButton("instance");

    var tmplViewBtn = $("#tmpl-view-btn").dxButton({
        icon: 'far fa-eye',
        text: "Xem mẫu hđ",
        disabled: true,
        onClick: function (e) {
            compInvTmplView();
        }
    }).dxButton("instance");
    // #endregion
    // #region Khởi tạo bảng kê email phát hành hđ
    var invPubStGrid = $("#inv-pub-st-table").dxDataGrid({
        showBorders: true,
        columns: [
            {
                caption: "Tiêu đề"
            },
            {
                caption: "Nội dung"
            },
            {
                caption: "Email gửi"
            },
            {
                caption: "Email nhận"
            },
            {
                caption: "Ngày tạo lập",
                width: 120,
                alignment: "center"
            },
            {
                caption: "Ngày gửi",
                width: 120,
                alignment: "center"
            },
            {
                caption: "Trạng thái",
                width: 120,
                alignment: "center"
            }
        ]
    }).dxDataGrid("instance");
    // #endregion
    // #region Khởi tạo bảng kê chứng thư số
    var digiCertGrid = $("#digi-cert-table").dxDataGrid({
        showBorders: true,
        columns: [
            {
                caption: "Thứ tự khe",
                width: 100
            },
            {
                caption: "Đường dẫn"
            },
            {
                caption: "Số sê-ri chứng thư"
            }
        ]
    }).dxDataGrid("instance");
    // #endregion
    // #region Khởi tạo bảng kê khóa tính năng
    var configGrid = $("#config-table").dxDataGrid({
        showBorders: true,
        columns: [
            {
                caption: "Tên khóa"
            },
            {
                caption: "Giá trị"
            }
        ]
    }).dxDataGrid("instance");
    // #endregion

    // #region Bắt sự kiện mở tab Thông tin công ty
    $(document).on('shown.bs.tab', 'a[href="#first"]', function () {
        if (curCoId != undefined && coFormData != undefined) {
            initCoInfoForm()
        }
    });
    // #endregion
    // #region Bắt sự kiện mở tab Thông tin phát hành
    $(document).on('shown.bs.tab', 'a[href="#second"]', function () {
        if (curCoId != undefined && coPubArray.length == 0) {
            initPubTable()
        }
    });
    // #endregion
    // #region Bắt sự kiện mở tab Cập nhật mẫu hđ
    $(document).on('shown.bs.tab', 'a[href="#third"]', function () {
        if (curCoId != undefined && pubTmplArray.length == 0) {
            // #region Cấu hình SelectBox chọn mẫu hđ
            pubTmplInd.option('visible', true);
            initTmplSelector();
            // #endregion
        }
    });
    // #endregion
    // #region Bắt sự kiện mở tab Thư điện tử phát hành hđ
    $(document).on('shown.bs.tab', 'a[href="#fourth"]', function () {
        if (curCoId != undefined && coInvPubStEmailArray.length == 0) {
            initInvPubStTable();
        }
    });
    // #endregion
    // #region Bắt sự kiện mở tab Chứng thư số
    $(document).on('shown.bs.tab', 'a[href="#fifth"]', function () {
        if (curCoId != undefined && coDigiCertArray.length == 0) {
            initDigiCertTable();
        }
    });
    // #endregion
    // #region Bắt sự kiện mở tab Khóa tính năng
    $(document).on('shown.bs.tab', 'a[href="#sixth"]', function () {
        if (curCoId != undefined && coConfigArray.length == 0) {
            initConfigTable();
        }
    });
    // #endregion
    // #region Bắt sự kiện đóng Modal xem mẫu hóa đơn
    $('#invTmplViewModal').on('hidden.bs.modal', function () {
        window.scrollTo(curScrollX, curScrollY)
    });
    // #endregion
    // #region Bắt sự kiện nhấp nút Toggle hiện hđ gốc
    $('#toggle-orig-inv').change(function () {
        $('#inv-tmpl-container').toggleClass('d-none');
    })
    // #endregion
    
    function initCoInfoForm() {
        // #region Cấu hình Form thông tin công ty
        $("#co-form-container").dxForm({
            formData: coFormData,
            colCount: 2,
            readOnly: false,
            showColonAfterLabel: true,
            showValidationSummary: true,
            validationGroup: "coInfo",
            items: [{
                colSpan: 2,
                dataField: "CoDomain",
                label: {
                    text: "Tên miền"
                },
                editorType: "dxTagBox",
                editorOptions: {
                    items: [],
                    acceptCustomValue: true,
                    deferRendering: false,
                    openOnFieldClick: false,
                    onCustomItemCreating: function (args) {
                        var newValue = args.text,
                            component = args.component,
                            currentItems = component.option("items");
                        currentItems.unshift(newValue);
                        component.option("items", currentItems);
                        args.customItem = newValue;
                    }
                }
            }, {
                itemType: "group",
                caption: "Thông tin định danh",
                items: [{
                    dataField: "CoCEO",
                    label: {
                        text: "Giám đốc"
                    }
                }, {
                    dataField: "CoTaxCode",
                    label: {
                        text: "Mã số thuế"
                    },
                    validationRules: [{
                        type: "required",
                        message: "Mã số thuế là bắt buộc"
                    }]
                }, {
                    dataField: "CoAddr",
                    label: {
                        text: "Địa chỉ"
                    },
                    validationRules: [{
                        type: "required",
                        message: "Địa chỉ là bắt buộc"
                    }]
                }, {
                    dataField: "CoBankAcctName",
                    label: {
                        text: "Tên tk ngân hàng"
                    }
                }, {
                    dataField: "CoBankName",
                    label: {
                        text: "Tên ngân hàng"
                    }
                }, {
                    dataField: "CoBankNo",
                    label: {
                        text: "Số tk"
                    }
                }]
            }, {
                itemType: "group",
                caption: "Thông tin liên hệ",
                items: [{
                    dataField: "CoEmailAddr",
                    label: {
                        text: "Email"
                    },
                    validationRules: [{
                        type: "email",
                        message: "Email không hợp lệ"
                    }]
                }, {
                    dataField: "CoFax",
                    label: {
                        text: "Fax"
                    }
                }, {
                    dataField: "CoPhone",
                    label: {
                        text: "Số đt"
                    }
                }]
            }, {
                colSpan: 2,
                itemType: "button",
                horizontalAlignment: "center",
                buttonOptions: {
                    text: "Cập nhật",
                    type: "success",
                    onClick: function () {
                        $.post(POSTDataReqStr, { ActionName: mCoInfoUpdate, ParmOne: curCoId, ParmTwo: JSON.stringify(coFormData), ParmThree: curDbId }, function (returnData) {
                            dispMsg(returnData);
                        });
                    }
                }
            }]
        });
        // #endregion
    }

    function initPubTable() {
        // #region Cấu hình bảng kê thông báo phát hành
        $("#pub-grid").dxDataGrid({
            dataSource: pubStore,
            hoverStateEnabled: true,
            headerFilter: {
                visible: true
            },
            editing: {
                mode: "batch",
                allowUpdating: true,
                selectTextOnEditStart: true,
                startEditAction: "dblClick"
            },
            showBorders: true,
            columns: [{
                dataField: "PubInvPattern",
                caption: "Mẫu số",
                allowEditing: false
            }, {
                dataField: "PubInvSerial",
                caption: "Ký hiệu",
                allowEditing: false
            }, {
                dataField: "PubStPubDate",
                caption: "Ngày phát hành",
                alignment: "center",
                dataType: "date",
                format: 'dd.MM.yyyy'
            }, {
                dataField: "PubStStt",
                caption: "Trạng thái",
                alignment: "center",
                lookup: {
                    dataSource: pubSttArray,
                    displayExpr: "SttDes",
                    valueExpr: "SttId"
                }
            }
            ],
            masterDetail: {
                enabled: true,
                template: function (container, options) {
                    var dataSource = [{
                        "PubTmplId": options.data.PubTmplId,
                        "DecNo": options.data.DecNo,
                        "PubInvPattern": options.data.PubInvPattern,
                    }];

                    $("<div>")
                        .addClass("master-detail-caption")
                        .text("Chi tiết quyết định")
                        .appendTo(container);
                    $("<div>")
                        .dxDataGrid({
                            columnAutoWidth: true,
                            showBorders: true,
                            columns: [{
                                dataField: "DecNo",
                                caption: "Số quyết định"
                            }, {
                                dataField: "PubInvPattern",
                                caption: "Mẫu số phát hành"
                            }],
                            dataSource: new DevExpress.data.DataSource({
                                store: new DevExpress.data.ArrayStore({
                                    key: "PubTmplId",
                                    data: dataSource
                                }),
                                filter: ["PubTmplId", "=", options.key]
                            })
                        }).appendTo(container);

                    $("<div class='m-t-10'>")
                        .addClass("master-detail-caption")
                        .text("Chi tiết thông báo")
                        .appendTo(container);
                    $("<div>")
                        .dxDataGrid({
                            columnAutoWidth: true,
                            showBorders: true,
                            editing: {
                                mode: "batch",
                                allowUpdating: true,
                                selectTextOnEditStart: true,
                                startEditAction: "dblClick"
                            },
                            columns: [{
                                dataField: "PubStCode",
                                caption: "Mã thông báo",
                                allowEditing: false
                            }, {
                                dataField: "TaxAuthCode",
                                caption: "Mã chi cục thuế",
                                validationRules: [{ type: "required" }]
                            }, {
                                dataField: "TaxAuthName",
                                caption: "Tên chi cục thuế",
                                validationRules: [{ type: "required" }]
                            }, {
                                dataField: "TaxAuthLocality",
                                caption: "Địa phương chi cục thuế",
                                validationRules: [{ type: "required" }]
                            }],
                            dataSource: new DevExpress.data.DataSource({
                                store: new DevExpress.data.CustomStore({
                                    key: "PubTmplId",
                                    load: function () {
                                        return $.get(GETDataReqStr, { ActionName: mCoPubData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
                                            if (returnData.Msg != undefined) {
                                                dispMsg(returnData.Msg);
                                            }
                                        });
                                    },
                                    update: function (key, values) {
                                        return $.post(POSTDataReqStr, { ActionName: mCoPubUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                                            dispMsg(returnData);
                                        });
                                    }
                                }),
                                filter: ["PubTmplId", "=", options.key]
                            })
                        }).appendTo(container);

                    $("<div class='m-t-10'>")
                        .addClass("master-detail-caption")
                        .text("Chi tiết gói hđ thông báo")
                        .appendTo(container);
                    $("<div>")
                        .dxDataGrid({
                            columnAutoWidth: true,
                            showBorders: true,
                            editing: {
                                mode: "batch",
                                allowUpdating: true,
                                selectTextOnEditStart: true,
                                startEditAction: "dblClick"
                            },
                            columns: [{
                                dataField: "PubPkgPurDate",
                                caption: "Ngày kích hoạt gói",
                                alignment: "center",
                                dataType: "date",
                                format: 'dd.MM.yyyy',
                                validationRules: [{ type: "required" }]
                            }, {
                                dataField: "PubPkgExpDate",
                                caption: "Ngày gói hết hạn",
                                alignment: "center",
                                dataType: "date",
                                format: 'dd.MM.yyyy',
                                validationRules: [{ type: "required" }]
                            }, {
                                dataField: "PubPkgBeginNo",
                                caption: "Số bắt đầu gói",
                                dataType: "number",
                                validationRules: [{ type: "required" }]
                            }, {
                                dataField: "PubPkgEndNo",
                                caption: "Số kết thúc gói",
                                dataType: "number",
                                validationRules: [{ type: "required" }]
                            }],
                            dataSource: new DevExpress.data.DataSource({
                                store: new DevExpress.data.CustomStore({
                                    key: "PubTmplId",
                                    load: function () {
                                        return $.get(GETDataReqStr, { ActionName: mCoPubData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
                                            if (returnData.Msg != undefined) {
                                                dispMsg(returnData.Msg);
                                            }
                                        });
                                    },
                                    update: function (key, values) {
                                        return $.post(POSTDataReqStr, { ActionName: mCoPubUpdate, ParmOne: key, ParmTwo: JSON.stringify(values), ParmThree: curDbId }, function (returnData) {
                                            dispMsg(returnData);
                                        });
                                    }
                                }),
                                filter: ["PubTmplId", "=", options.key]
                            })
                        }).appendTo(container);
                }
            }
        });
        // #endregion
    }

    function initTmplSelector() {
        $.get(GETDataReqStr, { ActionName: mCoPubData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
            if (returnData.Msg != undefined) {
                dispMsg(returnData.Msg);
            }
            else {
                pubTmplArray = returnData;
                $("#pub-tmpl-select").dxSelectBox({
                    items: pubTmplArray,
                    displayExpr: "PubTmplInd",
                    valueExpr: "PubTmplId",
                    onValueChanged: function (data) {
                        if (data.value) {
                            pubTmplInd.option('visible', true);
                            $.get(GETDataReqStr, { ActionName: mPubTmplData, ParmOne: data.value, ParmTwo: curDbId }, function (returnData) {
                                if (returnData.Msg != undefined) {
                                    dispMsg(returnData.Msg);
                                }
                                else {
                                    tmplCssEditor.getDoc().setValue(returnData.TmplCss);
                                    tmplCssEditor.clearHistory();
                                    logoCssEditor.getDoc().setValue(returnData.LogoCss);
                                    logoCssEditor.clearHistory();
                                    bgrCssEditor.getDoc().setValue(returnData.BgrCss);
                                    bgrCssEditor.clearHistory();
                                    tmplHtmlEditor.getDoc().setValue(returnData.TmplHtml);
                                    tmplHtmlEditor.clearHistory();
                                    tmplUpdateBtn.option('disabled', false);
                                    tmplViewBtn.option('disabled', false);
                                    pubTmplInd.option('visible', false);
                                }
                            });
                            $("#inv-list-dropdown").remove();
                            $("#inv-list-dropdown-indicator").before('<div id="inv-list-dropdown"></div>');
                            // #region Cấu hình DropDownBox danh sách hóa đơn theo mẫu hđ
                            $.get(GETDataReqStr, { ActionName: mCoInvData, ParmOne: curCoId, ParmTwo: data.value, ParmThree: curDbId }, function (returnData) {
                                coInvListArray = returnData;
                                $("#inv-list-dropdown").dxDropDownBox({
                                    valueExpr: "InvId",
                                    deferRendering: false,
                                    placeholder: "Danh sách hóa đơn...",
                                    displayExpr: function (item) {
                                        if (item.InvNo == 0) {
                                            return item.InvCus == null ?
                                                "Hóa đơn thuộc mẫu số " + item.InvPattern +
                                                ", ký hiệu " + item.InvSerial +                                                
                                                ", trị giá " + new Intl.NumberFormat('vi-VI', { maximumSignificantDigits: 3, style: 'currency', currency: 'VND' }).format(item.InvAmount)
                                                :
                                                "Hóa đơn thuộc mẫu số " + item.InvPattern +
                                                ", ký hiệu " + item.InvSerial +
                                                " của đơn vị " + item.InvCus +
                                                ", trị giá " + new Intl.NumberFormat('vi-VI', { maximumSignificantDigits: 3, style: 'currency', currency: 'VND' }).format(item.InvAmount);
                                        }
                                        else {
                                            return "Hóa đơn số " + item.InvNo +
                                                " thuộc mẫu số " + item.InvPattern +
                                                ", ký hiệu " + item.InvSerial;
                                        }                                        
                                    },
                                    showClearButton: true,
                                    dataSource: makeAsyncDataSourceInv(),
                                    onValueChanged(e) {
                                        if (e.value === null) {
                                            curInvId = "";
                                        }
                                    },
                                    contentTemplate: function (e) {
                                        var value = e.component.option("value"),
                                            $dataGrid = $("<div>").dxDataGrid({
                                                dataSource: e.component.option("dataSource"),
                                                hoverStateEnabled: true,
                                                columns: [
                                                    {
                                                        dataField: "InvNo",
                                                        caption: "Số hđ",
                                                        alignment: "center",
                                                        width: 80
                                                    },
                                                    {
                                                        dataField: "InvPattern",
                                                        caption: "Mẫu số hđ"
                                                    },
                                                    {
                                                        dataField: "InvSerial",
                                                        caption: "Ký hiệu hđ"
                                                    },
                                                    {
                                                        dataField: "InvCus",
                                                        caption: "Đơn vị đối tác"
                                                    },                                                    
                                                    {
                                                        dataField: "InvAmount",
                                                        caption: "Trị giá hđ",
                                                        dataType: "number",
                                                        format: function (value) {                                                            
                                                            return new Intl.NumberFormat('vi-VI', { maximumSignificantDigits: 3, style: 'currency', currency: 'VND' }).format(value);
                                                        }
                                                    }
                                                ],
                                                hoverStateEnabled: true,
                                                paging: { enabled: true, pageSize: 10 },
                                                filterRow: { visible: true },
                                                scrolling: { mode: "infinite" },
                                                selection: { mode: "single" },
                                                selectedRowKeys: [value],
                                                height: "100%",
                                                onSelectionChanged: function (selectedItems) {
                                                    var keys = selectedItems.selectedRowKeys,
                                                        hasSelection = keys.length;

                                                    e.component.option("value", hasSelection ? keys[0] : null);
                                                    e.component.close();
                                                    if (hasSelection) {
                                                        coInvListDropDownInd.option('visible', true);
                                                        curInvId = keys[0].toString();
                                                        coInvListDropDownInd.option('visible', false);
                                                    }
                                                }
                                            });

                                        dataGrid = $dataGrid.dxDataGrid("instance");

                                        e.component.on("valueChanged", function (args) {
                                            dataGrid.selectRows(args.value, false);
                                        });

                                        return $dataGrid;
                                    }
                                });
                            });
                            // #endregion
                        }
                    }
                });
                pubTmplInd.option('visible', false);
            }
        });
    }

    function initInvPubStTable() {
        // #region Cấu hình bảng kê email phát hành hđ
        $("#inv-pub-st-table").dxDataGrid({
            dataSource: emailStore,
            keyExpr: "EmailId",
            showBorders: true,
            hoverStateEnabled: true,
            headerFilter: {
                visible: true
            },
            editing: {
                mode: "popup",
                allowUpdating: true,
                popup: {
                    title: "Chi tiết email",
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
                        dataField: "EmailSubject",
                        caption: "Tiêu đề",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 75
                        }
                    }, {
                        dataField: "EmailContent",
                        caption: "Nội dung",
                        editorType: "dxTextArea",
                        editorOptions: {
                            height: 200
                        }
                    }, {
                        dataField: "SenderEmail",
                        caption: "Email gửi"
                    }, {
                        dataField: "ReceiverEmail",
                        caption: "Email nhận"
                    }, {
                        dataField: "SttId",
                        caption: "Trạng thái"
                    }]
                }
            },
            columns: [
                {
                    dataField: "EmailSubject",
                    caption: "Tiêu đề",
                    validationRules: [{ type: "required" }],
                    allowHeaderFiltering: false
                },
                {
                    dataField: "EmailContent",
                    caption: "Nội dung",
                    validationRules: [{ type: "required" }],
                    allowHeaderFiltering: false
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
                    dataField: "ReceiverEmail",
                    caption: "Email nhận",
                    allowHeaderFiltering: false,
                    editCellTemplate: tagBoxEditorTemplate,
                    lookup: {
                        dataSource: []
                    },
                    validationRules: [{ type: "required" }],
                    cellTemplate: function (container, options) {
                        var noBreakSpace = "\u00A0",
                            text = (options.value || []).map(function (element) {
                                return options.column.lookup.calculateCellValue(element);
                            }).join(", ");
                        container.text(text || noBreakSpace).attr("title", text);
                    },
                    calculateFilterExpression: function (filterValue, selectedFilterOperation, target) {
                        if (target === "search" && typeof (filterValue) === "string") {
                            return [this.dataField, "contains", filterValue]
                        }
                        return function (data) {
                            return (data.ReceiverEmail || []).indexOf(filterValue) !== -1
                        }
                    }
                },
                {
                    dataField: "CreateDate",
                    caption: "Ngày tạo lập",
                    width: 150,
                    alignment: "center"
                },
                {
                    dataField: "SendDate",
                    caption: "Ngày gửi",
                    width: 120,
                    alignment: "center"
                },
                {
                    dataField: "SttId",
                    caption: "Trạng thái",
                    width: 120,
                    alignment: "center",
                    lookup: {
                        dataSource: emailSttArray,
                        displayExpr: "SttDes",
                        valueExpr: "SttId"
                    }
                }
            ]
        });
        // #endregion
    }

    function initDigiCertTable() {
        // #region Cấu hình bảng kê chứng thư số
        $("#digi-cert-table").dxDataGrid({
            dataSource: certStore,
            keyExpr: "Id",
            showBorders: true,
            hoverStateEnabled: true,
            paging: {
                enabled: false
            },
            editing: {
                mode: "popup",
                allowAdding: true
            },
            columns: [
                {
                    dataField: "SlotIndx",
                    caption: "Thứ tự khe",
                    width: 100,
                    dataType: "number"
                },
                {
                    dataField: "Path",
                    caption: "Đường dẫn",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "CertSerial",
                    caption: "Số sê-ri chứng thư",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "Pwd",
                    caption: "Mật khẩu",
                    editorOptions: {
                        mode: "password"
                    },
                    validationRules: [{ type: "required" }],
                    visible: false
                },
                {
                    dataField: "Type",
                    caption: "Loại",
                    dataType: "number",
                    visible: false
                },
                {
                    dataField: "CertNo",
                    caption: "Số chứng thư",
                    dataType: "number",
                    visible: false
                }
            ]
        });
        // #endregion
    }

    function initConfigTable() {
        // #region Cấu hình bảng kê khóa chức năng
        $("#config-table").dxDataGrid({
            dataSource: cfgStore,
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
                startEditAction: "dblClick"
            },
            columns: [
                {
                    dataField: "Code",
                    caption: "Tên khóa",
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: "Value",
                    caption: "Giá trị",
                    validationRules: [{ type: "required" }]
                }
            ]
        });
        // #endregion
    }

    function compInvTmplView() {
        var tmplData = new TmplDataModel(tmplCssEditor.getDoc().getValue(), logoCssEditor.getDoc().getValue(), bgrCssEditor.getDoc().getValue(), tmplHtmlEditor.getDoc().getValue());
        $.post(POSTAuthedDataReqStr, { ActionName: mInvTmplViewData, ParmOne: $("#pub-tmpl-select").dxSelectBox("instance").option('value'), ParmTwo: JSON.stringify(tmplData), ParmThree: curInvId, ParmFour: curDbId }, function (returnData) {
            if (returnData.indexOf(":warning:") >= 0 || returnData.indexOf(":error:") >= 0) {
                dispMsg(returnData);
            }
            else {
                curScrollX = window.pageXOffset;
                curScrollY = window.pageYOffset;
                $('#inv-tmpl-container').html(returnData);
                $('#pages-container').remove();
                $('.pagination-container').remove();
                $('#inv-tmpl-container').after("<ul id='pages-container' class='p-0'></ul>");
                $("table[style*='page-break-inside: avoid']").wrap("<li inv-page class='lst-sty-none'><div id='printView'><div class='VATTEMP'><div class='content'></div></div></div></li>");
                $("[inv-page]").each(function () {
                    $("#pages-container").append($(this));
                });
                $('#pages-container').paginathing({
                    limitPagination: false,
                    perPage: 1,
                    prevNext: true,
                    firstLast: true,
                    prevText: '&lt;',
                    nextText: '&gt;',
                    firstText: '&laquo;',
                    lastText: '&raquo;',
                    containerClass: 'pagination-container fl-r',
                    ulClass: 'pagination m-b-0',
                    liClass: 'page-item',
                    activeClass: 'active',
                    disabledClass: 'disabled'
                });
                $('#inv-tmpl-container').html(returnData);
                if (curInvId == undefined) {
                    $('.VATTEMP').prepend('<img src="/img/pub-tmpl-stamp.png" id="pub-tmpl-stamp" />');
                }                
                $('#invTmplViewModal').modal('show');
                curInvTmplHtml = returnData;
            }            
        });
    }

    function clearCoData() {
        $('#co-name-label').empty();
        $('#co-name-sub-label').empty();
        $('#co-inv-mgmt-site-label').empty();
        $("#co-form-container").empty();
        curInvId = null;
        pubGrid.option('dataSource', []);
        pubTmplSelector.option('items', []);
        pubTmplSelector.option('value', null);
        invPubStGrid.option('dataSource', []);
        digiCertGrid.option('dataSource', []);
        configGrid.option('dataSource', []);
        if (coFormData != undefined) {
            for (const prop of Object.getOwnPropertyNames(coFormData)) {
                delete coFormData[prop];
            }
        }
        coPubArray.splice(0, coPubArray.length);
        pubTmplArray.splice(0, pubTmplArray.length);
        coInvListArray.splice(0, coInvListArray.length);
        tmplCssEditor.getDoc().setValue("");
        tmplCssEditor.clearHistory();
        logoCssEditor.getDoc().setValue("");
        logoCssEditor.clearHistory();
        bgrCssEditor.getDoc().setValue("");
        bgrCssEditor.clearHistory();
        tmplHtmlEditor.getDoc().setValue("");
        tmplHtmlEditor.clearHistory();
        tmplUpdateBtn.option('disabled', true);
        coInvPubStEmailArray.splice(0, coInvPubStEmailArray.length);
        coDigiCertArray.splice(0, coDigiCertArray.length);
        coConfigArray.splice(0, coConfigArray.length);
    }

    function genPageData() {
        clearCoData();
        $("#inv-list-dropdown").remove();
        $("#inv-list-dropdown-indicator").before('<div id="inv-list-dropdown"></div>');
        $.get(GETDataReqStr, { ActionName: mCoInfoData, ParmOne: curCoId, ParmTwo: curDbId }, function (returnData) {
            if (returnData.Msg != undefined) {
                dispMsg(returnData.Msg);
            }
            else {
                coFormData = returnData;
                $('#co-name-label').text(coFormData.CoName);
                $('#co-name-sub-label').text(coFormData.XferStt);
                $('#co-inv-mgmt-site-label').html("<span title='Đến trang quản lý hđđt của công ty này.' class='cursor-pointer' onclick='redirectToCoInvMgmtSite()'><i class='fas fa-external-link-alt i-fa-w-16'></i>Quản lý hđđt</span>");
                if ($('#first-tab').hasClass('active')) {
                    initCoInfoForm();
                }
                if ($('#second-tab').hasClass('active')) {
                    initPubTable();
                }
                if ($('#third-tab').hasClass('active')) {
                    pubTmplInd.option('visible', true);
                    initTmplSelector();
                }
                if ($('#fourth-tab').hasClass('active')) {
                    initInvPubStTable();
                }
                if ($('#fifth-tab').hasClass('active')) {
                    initDigiCertTable();
                }
                if ($('#sixth-tab').hasClass('active')) {
                    initConfigTable();
                }
                coDataInd.option('visible', false);
            }
        });
        // #region Khởi tạo DropDownBox danh sách hóa đơn theo mẫu hđ
        $("#inv-list-dropdown").dxDropDownBox({
            deferRendering: false,
            placeholder: "Danh sách hóa đơn...",
            onInitialized: function (e) {
                setTimeout(function () {
                    e.element.find(".dx-texteditor-container").on("dxclick", function (event) {
                        if (!e.component.getDataSource().items().length) {
                            event.stopPropagation();
                        }
                    })
                }, 300);
            }
        });
        // #endregion
    }
});

function tagBoxEditorTemplate(cellElement, cellInfo) {
    return $("<div>").dxTagBox({
        dataSource: [],
        value: cellInfo.value,
        maxDisplayedTags: 4,
        acceptCustomValue: true,
        deferRendering: false,
        openOnFieldClick: false,
        onValueChanged: function (e) {
            cellInfo.setValue(e.value)
        },
        onSelectionChanged: function (e) {
            cellInfo.component.updateDimensions();
        }
    });
}

function clearPageData() {
    curCoId = null;    
    curCusRef = null;     
}

function redirectToCoInvMgmtSite() {
    $.get(CoInvMgmtSiteUrlStr, { CoId: curCoId, DbId: curDbId }, function (returnData) {
        if (returnData.indexOf("warning") >= 0 || returnData.indexOf("error") >= 0) {
            dispMsg(returnData);
        }
        else {
            window.open(returnData);
        }
    });
}

function printInvTmpl() {
    $("#inv-tmpl-container").printArea({
        mode: "iframe"
    });
    return (false)
}

function expInvTmpl() {
    window.open(mInvTmplPdfData + '?CoId=' + curCoId + '&PubTmplId=' + $("#pub-tmpl-select").dxSelectBox("instance").option('value') + '&InvId=' + curInvId + '&DbId=' + curDbId);
}

class TmplDataModel {
    constructor(tmplCss, logoCss, bgrCss, tmplHtml) {
        this.TmplCss = tmplCss;
        this.LogoCss = logoCss;
        this.BgrCss = bgrCss;
        this.TmplHtml = tmplHtml;
    }
}