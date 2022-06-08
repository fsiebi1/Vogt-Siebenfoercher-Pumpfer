
$(document).ready(() => {

    $("#CPassword").blur(() => {
        cpassword = $("#CPassword").val() == $("#Password").val();

        if (cpassword) {
            $("#cpasswordMessage").css("display", "none");
            $("#btnSubmit").prop('disabled', false);
            $("#btnSubmit").css('background-color', "#45a049");
        } else {
            $("#cpasswordMessage").css("display", "block");
            $("#btnSubmit").prop('disabled', true);
            $("#btnSubmit").css('background-color', "grey");
        }
    });

});