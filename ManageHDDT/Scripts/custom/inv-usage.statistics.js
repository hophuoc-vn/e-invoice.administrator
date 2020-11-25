// #region Khai báo hằng số
const intervalDaysArray = [ 7, 14, 30 ];
// #endregion
// #region Khai báo biến số
var curDate = new Date();
var requestTimeLog;
var invUsageChartData = [];
var invUsageTableData = [];
// #endregion

$(document).ready(function () {
    // #region Khởi tạo các LoadIndicator
    var invUsageChartInd = $("#inv-usage-chart-indicator").dxLoadIndicator({
        height: 25,
        width: 25,
        visible: false
    }).dxLoadIndicator("instance");
    // #endregion
    // #region Khởi tạo biểu đồ lượng sử dụng hđ theo thời gian gần nhất
    $("#inv-usage-chart").dxChart({
        palette: "Dark Moon",
        dataSource: invUsageChartData,
        commonSeriesSettings: {
            argumentField: "Date"
        },
        margin: {
            bottom: 20
        },
        argumentAxis: {
            valueMarginsEnabled: false,
            grid: {
                visible: true
            }
        },
        series: {
            valueField: "InvUsageQty",
            name: "Hđ đã phát hành",
            label: {
                visible: true,
                backgroundColor: "#f6acc8"
            },
        },
        legend: {
            verticalAlignment: "bottom",
            horizontalAlignment: "center",
            itemTextPosition: "bottom"
        },
        title: {
            text: "Lượng sử dụng hóa đơn"            
        }
    }).dxChart("instance");
    // #endregion
    // #region Khởi tạo SelectBox tìm lượng sử dụng hđ theo thời gian gần nhất
    $("#days-selector").dxSelectBox({
        items: intervalDaysArray,
        onValueChanged: function (data) {
            invUsageChartInd.option("visible", true);
            $.get(GETDataReqStr, { ActionName: mInvUsageChartData, ParmOne: data.value }, function (returnData) {
                if (returnData.Msg != undefined) {
                    dispMsg(returnData.Msg);
                }
                else {
                    invUsageChartData = returnData;
                    requestTimeLog = addZero(curDate.getHours()) + ":"
                        + addZero(curDate.getMinutes()) + " ngày "
                        + addZero(curDate.getDate()) + "."
                        + addZero((curDate.getMonth() + 1)) + "."
                        + addZero(curDate.getFullYear());
                    // #region Cấu hình biểu đồ lượng sử dụng hđ theo thời gian gần nhất    
                    $("#inv-usage-chart").dxChart({
                        palette: "Dark Moon",
                        dataSource: invUsageChartData,
                        commonSeriesSettings: {
                            argumentField: "Date"
                        },
                        margin: {
                            bottom: 20
                        },
                        argumentAxis: {
                            valueMarginsEnabled: false,
                            grid: {
                                visible: true
                            }
                        },
                        series: {
                            valueField: "InvUsageQty",
                            name: "Hđ đã phát hành",
                            label: {
                                visible: true,
                                backgroundColor: "#f6acc8"
                            },
                        },
                        legend: {
                            verticalAlignment: "bottom",
                            horizontalAlignment: "center",
                            itemTextPosition: "bottom"
                        },
                        title: {
                            text: "Lượng sử dụng hóa đơn " + data.value + " ngày gần nhất",
                            subtitle: {
                                text: "(Ghi nhận lúc " + requestTimeLog + ")"
                            }
                        },
                        export: {
                            enabled: true,
                            printingEnabled: true,
                            fileName: "MatBao_eInvSys"
                        }
                    }).dxChart("instance");
                    // #endregion
                    invUsageChartInd.option("visible", false);
                }
            });
        }
    });
    // #endregion
    // #region Khởi tạo bảng kê các công ty gần hết hđ
    $("#inv-usage-table").dxDataGrid({
        dataSource: invUsageTableData,
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
                dataField: "RepPerson",
                caption: "Người đại diện"
            },
            {
                dataField: "CoPhone",
                caption: "Số liên lạc",
                width: 100,
            },
            {
                dataField: "CoAddress",
                caption: "Địa chỉ"
            },
            {
                dataField: "RemainInv",
                caption: "Số hđ còn lại",
                width: 100,
                alignment: "center"
            }
        ]
    });
    // #endregion
});

function addZero(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}