
var email = true;
var username = true;
var cpassword = true;

function setBtnSubmit(data) {
    if (email && username && cpassword) {
        $("#btnSubmit").prop('disabled', false);
        $("#btnSubmit").css('background-color', "#45a049");
    } else {
        $("#btnSubmit").prop('disabled', true);
        $("#btnSubmit").css('background-color', "grey");
    }
}

$(document).ready(() => {

    $("#User_Email").blur(() => {

        $.ajax({
            url: "/user/IsUniqueEmail",
            method: "GET",
            data: { email: $("#User_Email").val() }
        })
            .done((dataFromServer) => {
                if (dataFromServer === false) {
                    email = false;
                } else {
                    email = true;
                }
                setBtnSubmit();

                if (!email) {
                    $("#mailMessage").css("display", "block");
                    $("#User_Email").addClass("redBorder");

                } else {
                    $("#mailMessage").css("display", "none");
                    $("#User_Email").removeClass("redBorder");
                }
            })

            .fail(() => {
                alert("server url nicht erreichbar");
            });
    });

    $("#User_Username").blur(() => {

        $.ajax({
            url: "/user/IsUniqueUsername",
            method: "GET",
            data: { username: $("#User_Username").val() }
        })
            .done((dataFromServer) => {
                if (dataFromServer === false) {
                    username = false;
                } else {
                    username = true;
                }
                setBtnSubmit();

                if (!username) {
                    $("#usernameMessage").css("display", "block");
                    $("#User_Username").addClass("redBorder");

                } else {
                    $("#usernameMessage").css("display", "none");
                    $("#User_Username").removeClass("redBorder");
                }
            })

            .fail(() => {
                alert("server url nicht erreichbar");
            });
    });

    $("#User_CPassword").blur(() => {
        cpassword = $("#User_CPassword").val() == $("#User_Password").val();
        setBtnSubmit();

        if (cpassword) {
            $("#cpasswordMessage").css("display", "none");
        } else {
            $("#cpasswordMessage").css("display", "block");
        }
    });
});