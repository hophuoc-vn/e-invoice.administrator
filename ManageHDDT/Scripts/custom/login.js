$("#cancel-btn, #close-btn").on("click", function () {
    $("#user-email").val("");
    $('#validation-message').text("");
});

$("#send-btn").on("click", function () {
    userEmail = $("#user-email").val();
    //$.post("/Home/SendResetPwdEmail", { Email: userEmail }, function (returnData) {
    //    array_result = returnData.split(":");
    //    var resultType = array_result[0];
    //    var resultMsg = array_result[1];
    //    switch (resultType) {
    //        case "Error":
    //            $('#validation-message').addClass('ux-red font-weight-bold');
    //        case "Success":
    //            $('#validation-message').addClass('ux-green font-weight-bold');
    //    }
    //    $('#validation-message').text(resultMsg);
    //});
});