
var email = true;
var username = true;

$(document).ready(() => {

    $("#User_Email").blur(() => {

        if ($("#User_Email").val() != $("#User_EmailOld").val()) {

            $.ajax({
                url: "/user/IsUniqueEmail",
                method: "GET",
                async: false,
                data: { email: $("#User_Email").val() }
            })
                .done((dataFromServer) => {
                    if (dataFromServer === false) {
                        email = false;
                    } else {
                        email = true;
                    }
                })

                .fail(() => {
                    alert("server url nicht erreichbar");
                });

        } else {
            email = true;
        }

        setBtnSubmit();
    });

    $("#User_Username").blur(() => {

        if ($("#User_Username").val() != $("#User_UsernameOld").val()) {

            $.ajax({
                url: "/user/IsUniqueUsername",
                method: "GET",
                async: false,
                data: { username: $("#User_Username").val() }
            })
                .done((dataFromServer) => {
                    if (dataFromServer === false) {
                        username = false;
                    } else {
                        username = true;
                    }
                })

                .fail(() => {
                    alert("server url nicht erreichbar");
                });
        } else {
            username = true;
        }

        setBtnSubmit();
    });

    $("#btnSubmit").prop("value", "Update");
    $(".cpassword").css("display", "none");
});


function setBtnSubmit(data) {

    if (email && username) {
        $("#btnSubmit").prop('disabled', false);
        $("#btnSubmit").css('background-color', "#45a049");
    } else {
        $("#btnSubmit").prop('disabled', true);
        $("#btnSubmit").css('background-color', "grey");
    }

    if (!email) {
        $("#mailMessage").css("display", "block");
        $("#User_Email").addClass("redBorder");

    } else {
        $("#mailMessage").css("display", "none");
        $("#User_Email").removeClass("redBorder");
    }

    if (!username) {
        $("#usernameMessage").css("display", "block");
        $("#User_Username").addClass("redBorder");

    } else {
        $("#usernameMessage").css("display", "none");
        $("#User_Username").removeClass("redBorder");
    }
}