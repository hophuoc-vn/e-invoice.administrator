// #region Khai báo hằng số
const colors = ["#77d8d8", "#4ddac1"];
const curDate = new Date();
const curMonth = curDate.getMonth();
const curYear = curDate.getFullYear();
// #endregion
// #region Khai báo biến số
var yearArray = [2019];
var startDate, endDate;
var cusEntryChartData = [];
var coQtyChartData = [];
var cusEntryTableData;
var coQtyTableData;
// #endregion

if (curYear > yearArray.slice(-1).pop()) {
    var yearGap = curYear - yearArray.slice(-1).pop();
    for (var i = 1; i <= yearGap; i++) {
        yearArray.push(yearArray.slice(-1).pop() + 1);
    }
}

$(window).on("load", function () {    
    // #region Khởi tạo biểu đồ đầu vào khách hàng trong năm hiện tại
    var isFirstLevel = true,
        chartContainer = $("#cus-entry-chart"),
        chart = chartContainer.dxChart({
            dataSource: filterData(""),
            title: "Đầu vào khách hàng trong năm " + curYear,
            series: {
                type: "bar",
                label: {
                    visible: true,
                    backgroundColor: "#f6acc8"
                },
            },
            legend: {
                visible: false
            },
            valueAxis: {
                showZero: false
            },
            //commonSeriesSettings: {
            //    selectionStyle: {
            //        color: '#4ddac1'
            //    }
            //},
            onPointClick: function (e) {
                if (isFirstLevel) {
                    isFirstLevel = false;
                    removePointerCursor(chartContainer);
                    chart.option({
                        dataSource: filterData(e.target.originalArgument)
                    });
                    $("#backButton")
                        .dxButton("instance")
                        .option("visible", true);
                }
            },
            customizePoint: function () {
                var pointSettings = {
                    color: colors[Number(isFirstLevel)]
                };

                if (!isFirstLevel) {
                    pointSettings.hoverStyle = {
                        hatching: "none"
                    };
                }

                return pointSettings;
            }
        }).dxChart("instance");
    //var series = chartContainer.dxChart('instance').getAllSeries();
    //var initialSelectedPoint = series[0].getPointByPos(curMonth);
    //initialSelectedPoint.select();
    

    $("#backButton").dxButton({
        text: "Quay lại",
        icon: "chevronleft",
        visible: false,
        onClick: function () {
            if (!isFirstLevel) {                    
                isFirstLevel = true;
                addPointerCursor(chartContainer);
                chart.option("dataSource", filterData(""));
                //var series = chartContainer.dxChart('instance').getAllSeries();
                //var initialSelectedPoint = series[0].getPointByPos(curMonth);
                //initialSelectedPoint.select();
                this.option("visible", false);
            }
        }
    });    

    addPointerCursor(chartContainer);
    // #endregion
});

$(document).ready(function () {
    // #region Khởi tạo các LoadIndicator
    var cusEntryChartInd = $("#cus-entry-chart-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    var cusEntryTableIndicator = $("#cus-entry-table-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    var coQtyChartIndicator = $("#co-qty-chart-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    var coQtyTableIndicator = $("#co-qty-table-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");    
    // #endregion    
    // #region Khởi tạo bảng kê đầu vào khách hàng
    $("#cus-entry-table").dxDataGrid({
        showBorders: true,
        columns: [
            {
                caption: "Công ty"
            },
            {
                caption: "Ngày đăng ký dv",
                width: 120,
                alignment: "center"
            }
        ]
    });
    // #endregion
    // #region Khởi tạo biểu đồ số lượng công ty theo trạng thái dv
    coQtyChartIndicator.option('visible', true);
    $.get(GETDataReqStr, { ActionName: mCoQtyChartData }, function (returnData) {
        coQtyChartData = returnData;
        $("#co-qty-chart").dxPieChart({
            palette: "Material",
            dataSource: coQtyChartData,
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
                argumentField: "SvcSttTitle",
                valueField: "CoQty",
                label: {
                    visible: true,
                }
            }],
            onDrawn: function (e) {
                e.element.find(".dxc-series, .dxc-legend, text, .dxc-labels").hover(function () { $(this).css('cursor', 'pointer'); }, function () { $(this).css('cursor', 'auto'); });
            },
            onPointClick: function (e) {
                var point = e.target;
                var svcStt = point.argument;

                $("#co-qty-table-title").text(svcStt.substring(8));
                coQtyTableIndicator.option("visible", true);
                $.get(GETDataReqStr, { ActionName: mCoQtyTableData, ParmOne: svcStt }, function (returnData) {
                    if (returnData.Msg != undefined) {
                        dispMsg(returnData.Msg);
                    }
                    else {
                        coQtyTableData = returnData;
                        $("#co-qty-table").dxDataGrid({
                            dataSource: coQtyTableData,
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
                                    caption: "Công ty"
                                },
                                {
                                    dataField: "CoBoardRep",
                                    caption: "Đại diện lãnh đạo"
                                },
                                {
                                    dataField: "CoPhone",
                                    caption: "Số liên lạc",
                                    width: 120
                                },
                                {
                                    dataField: "CoAddress",
                                    caption: "Địa chỉ"
                                }
                            ]
                        });
                        coQtyTableIndicator.option("visible", false);
                    }
                });
            },
            onLegendClick: function (e) {
                var arg = e.target;
                var svcStt = this.getAllSeries()[0].getPointsByArg(arg)[0].argument;

                $("#co-qty-table-title").text(svcStt.substring(8));
                coQtyTableIndicator.option("visible", true);
                $.get(GETDataReqStr, { ActionName: mCoQtyTableData, ParmOne: svcStt }, function (returnData) {
                    if (returnData.Msg != undefined) {
                        dispMsg(returnData.Msg);
                    }
                    else {
                        coQtyTableData = returnData;
                        $("#co-qty-table").dxDataGrid({
                            dataSource: coQtyTableData,
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
                                    caption: "Công ty"
                                },
                                {
                                    dataField: "CoBoardRep",
                                    caption: "Đại diện lãnh đạo"
                                },
                                {
                                    dataField: "CoPhone",
                                    caption: "Số liên lạc",
                                    width: 120
                                },
                                {
                                    dataField: "CoAddress",
                                    caption: "Địa chỉ"
                                }
                            ]
                        });
                        coQtyTableIndicator.option("visible", false);
                    }
                });
            }
        }).dxPieChart("instance");
        coQtyChartIndicator.option("visible", false);
    });
    // #endregion
    // #region Khởi tạo bảng kê các công ty theo trạng thái dv
    $("#co-qty-table").dxDataGrid({
        showBorders: true,
        columns: [
            {
                caption: "Công ty"
            },
            {
                caption: "Đại diện lãnh đạo"
            },
            {
                caption: "Số liên lạc",
                width: 120
            },
            {
                caption: "Địa chỉ"
            }
        ]
    });
    // #endregion    
    // #region Cấu hình SelectBox tìm đầu vào khách hàng theo năm
    $("#year-selector").dxSelectBox({
        items: yearArray,
        onValueChanged: function (data) {
            cusEntryChartInd.option("visible", true);
            $.get(GETDataReqStr, { ActionName: mCusEntryChartData, ParmOne: data.value }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
                else {
                    cusEntryChartData = returnData;
                    var isFirstLevel = true,
                        chartContainer = $("#cus-entry-chart"),
                        chart = chartContainer.dxChart({
                            dataSource: filterData(""),
                            title: "Đầu vào khách hàng trong năm " + data.value,
                            series: {
                                type: "bar",
                                label: {
                                    visible: true,
                                    backgroundColor: "#f6acc8"
                                },
                            },
                            legend: {
                                visible: false
                            },
                            valueAxis: {
                                showZero: false
                            },
                            //commonSeriesSettings: {
                            //    selectionStyle: {
                            //        color: '#4ddac1'
                            //    }
                            //},
                            onPointClick: function (e) {
                                if (isFirstLevel) {
                                    isFirstLevel = false;
                                    removePointerCursor(chartContainer);
                                    chart.option({
                                        dataSource: filterData(e.target.originalArgument)
                                    });
                                    $("#backButton")
                                        .dxButton("instance")
                                        .option("visible", true);
                                }
                            },
                            customizePoint: function () {
                                var pointSettings = {
                                    color: colors[Number(isFirstLevel)]
                                };

                                if (!isFirstLevel) {
                                    pointSettings.hoverStyle = {
                                        hatching: "none"
                                    };
                                }

                                return pointSettings;
                            }
                        }).dxChart("instance");
                    //if (curYear == data.value) {
                    //    var series = chartContainer.dxChart('instance').getAllSeries();
                    //    var initialSelectedPoint = series[0].getPointByPos(curMonth);
                    //    initialSelectedPoint.select();
                    //}
                    cusEntryChartInd.option("visible", false);

                    $("#backButton").dxButton({
                        text: "Quay lại",
                        icon: "chevronleft",
                        visible: false,
                        onClick: function () {
                            if (!isFirstLevel) {
                                isFirstLevel = true;
                                addPointerCursor(chartContainer);
                                chart.option("dataSource", filterData(""));
                                //if (curYear == data.value) {
                                //    var series = chartContainer.dxChart('instance').getAllSeries();
                                //    var initialSelectedPoint = series[0].getPointByPos(curMonth);
                                //    initialSelectedPoint.select();
                                //}
                                cusEntryChartInd.option("visible", false);
                                this.option("visible", false);
                            }
                        }
                    });

                    addPointerCursor(chartContainer);
                }
            });
        }
    });
    // #endregion
    // #region Cấu hình các Input nhập ngày tìm đầu vào khách hàng
    $("#date-picker-start").dxDateBox({
        pickerType: "rollers",
        displayFormat: "dd.MM.yyyy",
        max: curDate,
        onValueChanged: function (e) {
            const previousValue = e.previousValue;
            const newValue = e.value;
            startDate = newValue;
            // Event handling commands go here
            if (endDate == undefined) {
                $("#date-picker-end").dxDateBox("instance").option('min', startDate);
            }
        }
    });

    $("#date-picker-end").dxDateBox({
        pickerType: "rollers",
        displayFormat: "dd.MM.yyyy",
        max: curDate,
        onValueChanged: function (e) {
            const previousValue = e.previousValue;
            const newValue = e.value;
            endDate = newValue;
            // Event handling commands go here
            if (startDate == undefined) {
                $("#date-picker-start").dxDateBox("instance").option('max', endDate);
            }
        }
    });
    // #endregion
    // #region Cấu hình Button tìm đầu vào khách hàng trong khoảng thời gian
    $("#search-btn").dxButton({
        icon: "search",
        onClick: function (e) {
            if (startDate == undefined && endDate == undefined) {
                DevExpress.ui.notify({ message: "Chưa nhập ngày tìm kiếm.", width: 300, position: "center" }, "warning", 1500);
            }
            else if (startDate == undefined) {
                DevExpress.ui.notify({ message: "Chưa nhập ngày tìm đầu.", width: 300, position: "center" }, "warning", 1500);
            }
            else if (endDate == undefined) {
                DevExpress.ui.notify({ message: "Chưa nhập ngày tìm cuối.", width: 300, position: "center" }, "warning", 1500);
            }
            else {
                cusEntryTableIndicator.option("visible", true);
                var formattedStartDate = addZero(startDate.getFullYear()) + "-"
                    + addZero((startDate.getMonth() + 1)) + "-"
                    + addZero(startDate.getDate());
                var formattedEndDate = addZero(endDate.getFullYear()) + "-"
                    + addZero((endDate.getMonth() + 1)) + "-"
                    + addZero(endDate.getDate());
                $.get(GETDataReqStr, { ActionName: mCusEntryTableData, ParmOne: formattedStartDate, ParmTwo: formattedEndDate }, function (returnData) {
                    if (returnData.Msg != undefined) {
                        dispMsg(returnData.Msg);
                    }
                    else {
                        cusEntryTableData = returnData;
                        $("#cus-entry-table").dxDataGrid({
                            dataSource: cusEntryTableData,
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
                                    caption: "Công ty"
                                },
                                {
                                    dataField: "RegDate",
                                    caption: "Ngày đăng ký dv",
                                    width: 120,
                                    alignment: "center"
                                }
                            ]
                        });
                        cusEntryTableIndicator.option("visible", false);
                    }                    
                });                
            }            
        }
    });
    // #endregion
});

function filterData(name) {
    return cusEntryChartData.filter(function (item) {
        return item.parentID === name;
    });
}

function addPointerCursor(container) {
    container.addClass("pointer-on-bars");
}

function removePointerCursor(container) {
    container.removeClass("pointer-on-bars");
}

function addZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}