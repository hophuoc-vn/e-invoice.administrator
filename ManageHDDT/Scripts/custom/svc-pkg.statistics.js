// #region Khai báo hằng số
// #endregion
// #region Khai báo biến số
var svcPkgChartData;
var svcPkgTableData;
// #endregion

$(document).ready(function () {    
    // #region Khởi tạo các LoadIndicator
    var svcPkgChartIndicator = $("#svc-pkg-chart-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    var svcPkgTableIndicator = $("#svc-pkg-table-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    // #endregion
    // #region Khởi tạo biểu đồ số công ty theo gói hđ
    svcPkgChartIndicator.option("visible", true);
    $.get(GETDataReqStr, { ActionName: mSvcPkgChartData }, function (returnData) {
        svcPkgChartData = returnData;
        $("#svc-pkg-chart").dxPieChart({
            palette: "Material",
            dataSource: svcPkgChartData,
            margin: {
                bottom: 20
            },
            legend: {
                visible: true
            },
            animation: {
                enabled: true
            },
            resolveLabelOverlapping: "shift",
            series: [{
                argumentField: "SvcPkgTitle",
                valueField: "CoQty",
                label: {
                    visible: true,
                }
            }],
            onDrawn: function (e) {
                e.element.find(".dxc-series, .dxc-legend, text, .dxc-labels, .dxl-marker").hover(function () { $(this).css('cursor', 'pointer'); }, function () { $(this).css('cursor', 'auto'); });
            },
            onPointClick: function (e) {
                var point = e.target;
                var svcPkgName = point.argument;

                $("#svc-pkg-table-title").text(svcPkgName);
                svcPkgTableIndicator.option("visible", true);
                $.get(GETDataReqStr, { ActionName: mSvcPkgTableData, ParmOne: svcPkgName }, function (returnData) {
                    if (returnData.Msg != undefined) {
                        dispMsg(returnData.Msg);
                    }
                    else {
                        svcPkgTableData = returnData;
                        if (svcPkgName != "Khác") {
                            $("#svc-pkg-table").dxDataGrid({
                                dataSource: svcPkgTableData,
                                showBorders: true,
                                paging: {
                                    pageSize: 10
                                },
                                pager: {
                                    showNavigationButtons: true
                                },
                                columns: [
                                    {
                                        dataField: "CoName",
                                        caption: "Tên công ty"
                                    },
                                    {
                                        dataField: "CoBoardRep",
                                        caption: "Người đại diện"
                                    },
                                    {
                                        dataField: "SvcCode",
                                        caption: "Mã dịch vụ",
                                        width: 100,
                                        alignment: "center"
                                    },
                                    {
                                        dataField: "SvcStt",
                                        caption: "Trạng thái",
                                        width: 90,
                                        alignment: "center",
                                        cellTemplate: function (container, options) {
                                            if (options.data.SvcStt === 1) {
                                                var e = $("<span style='color:limegreen'>Hoạt động</span>");
                                                e.appendTo(container);
                                            } else {
                                                var e = $("<span style='color:orangered'>Không rõ</span>");
                                                e.appendTo(container);
                                            }
                                        }
                                    }
                                ]
                            });
                            svcPkgTableIndicator.option("visible", false);
                        }
                        else {
                            $("#svc-pkg-table").dxDataGrid({
                                dataSource: svcPkgTableData,
                                showBorders: true,
                                paging: {
                                    pageSize: 10
                                },
                                pager: {
                                    showNavigationButtons: true
                                },
                                columns: [
                                    {
                                        dataField: "CoName",
                                        caption: "Tên công ty"
                                    },
                                    {
                                        dataField: "CoBoardRep",
                                        caption: "Người đại diện"
                                    },
                                    {
                                        dataField: "SvcName",
                                        caption: "Tên gói dv"
                                    },
                                    {
                                        dataField: "SvcCode",
                                        caption: "Mã dịch vụ",
                                        width: 100,
                                        alignment: "center"
                                    },
                                    {
                                        dataField: "SvcStt",
                                        caption: "Trạng thái",
                                        width: 90,
                                        alignment: "center",
                                        cellTemplate: function (container, options) {
                                            if (options.data.SvStt === 1) {
                                                var e = $("<span style='color:limegreen'>Hoạt động</span>");
                                                e.appendTo(container);
                                            } else {
                                                var e = $("<span style='color:orangered'>Không rõ</span>");
                                                e.appendTo(container);
                                            }
                                        }
                                    }
                                ]
                            });
                            svcPkgTableIndicator.option("visible", false);
                        }
                    }
                });
            },
            onLegendClick: function (e) {
                var arg = e.target;
                var svcPkgName = this.getAllSeries()[0].getPointsByArg(arg)[0].argument;

                $("#svc-pkg-table-title").text(svcPkgName);
                svcPkgTableIndicator.option("visible", true);
                $.get(GETDataReqStr, { ActionName: mSvcPkgTableData, ParmOne: svcPkgName }, function (returnData) {
                    if (returnData.Msg != undefined) {
                        dispMsg(returnData.Msg);
                    }
                    else {
                        svcPkgTableData = returnData;
                        if (svcPkgName != "Khác") {
                            $("#svc-pkg-table").dxDataGrid({
                                dataSource: svcPkgTableData,
                                showBorders: true,
                                paging: {
                                    pageSize: 10
                                },
                                pager: {
                                    showNavigationButtons: true
                                },
                                columns: [
                                    {
                                        dataField: "CoName",
                                        caption: "Tên công ty"
                                    },
                                    {
                                        dataField: "CoBoardRep",
                                        caption: "Người đại diện"
                                    },
                                    {
                                        dataField: "SvcCode",
                                        caption: "Mã dịch vụ",
                                        width: 100,
                                        alignment: "center"
                                    },
                                    {
                                        dataField: "SvcStt",
                                        caption: "Trạng thái",
                                        width: 90,
                                        alignment: "center",
                                        cellTemplate: function (container, options) {
                                            if (options.data.SvcStt === 1) {
                                                var e = $("<span style='color:limegreen'>Hoạt động</span>");
                                                e.appendTo(container);
                                            } else {
                                                var e = $("<span style='color:orangered'>Không rõ</span>");
                                                e.appendTo(container);
                                            }
                                        }
                                    }
                                ]
                            });
                            svcPkgTableIndicator.option("visible", false);
                        }
                        else {
                            $("#svc-pkg-table").dxDataGrid({
                                dataSource: svcPkgTableData,
                                showBorders: true,
                                paging: {
                                    pageSize: 10
                                },
                                pager: {
                                    showNavigationButtons: true
                                },
                                columns: [
                                    {
                                        dataField: "CoName",
                                        caption: "Tên công ty"
                                    },
                                    {
                                        dataField: "CoBoardRep",
                                        caption: "Người đại diện"
                                    },
                                    {
                                        dataField: "SvcName",
                                        caption: "Tên gói dv"
                                    },
                                    {
                                        dataField: "SvcCode",
                                        caption: "Mã dịch vụ",
                                        width: 100,
                                        alignment: "center"
                                    },
                                    {
                                        dataField: "SvcStt",
                                        caption: "Trạng thái",
                                        width: 90,
                                        alignment: "center",
                                        cellTemplate: function (container, options) {
                                            if (options.data.SvStt === 1) {
                                                var e = $("<span style='color:limegreen'>Hoạt động</span>");
                                                e.appendTo(container);
                                            } else {
                                                var e = $("<span style='color:orangered'>Không rõ</span>");
                                                e.appendTo(container);
                                            }
                                        }
                                    }
                                ]
                            });
                            svcPkgTableIndicator.option("visible", false);
                        }
                    }
                });
            }
        }).dxPieChart("instance");
        svcPkgChartIndicator.option("visible", false);
    });        
    // #endregion
    // #region Khởi tạo bảng kê các công ty theo gói hđ
    $("#svc-pkg-table").dxDataGrid({
        showBorders: true,
        columns: [
            {
                caption: "Tên công ty"
            },
            {
                caption: "Người đại diện"
            },
            {
                caption: "Mã dịch vụ",
                width: 100,
                alignment: "center"
            },
            {
                caption: "Trạng thái",
                width: 90,
                alignment: "center"
            }
        ]
    });
    // #endregion
});