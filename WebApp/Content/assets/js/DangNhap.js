$(document).ready(function () {
    function handleLogin() {
        $("#message").html("");
        $(".btn-login").prop("disabled", true);
        $("#loadingIcon").show();

        var tendangnhap = $("#tendangnhap").val().trim();
        var matkhau = $("#matkhau").val().trim();

        if (!tendangnhap || !matkhau) {
            $("#message").html('<div class="alert alert-warning">Vui lòng nhập đầy đủ thông tin!</div>');
            $(".btn-login").prop("disabled", false);
            $("#loadingIcon").hide();
            return;
        }

        $.ajax({
            url: "/Account/Login",
            type: "POST",
            data: {
                UserName: tendangnhap,
                Password: matkhau,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            dataType: "json",
            success: function (data) {
                if (data.code === 200) {
                    $("#message").html('<div class="alert alert-success">' + data.msg + '</div>');
                    setTimeout(function () {
                        window.location.href = data.redirectUrl || "/QuanLy/Index";
                    }, 1500);
                } else {
                    $("#message").html('<div class="alert alert-danger">' + data.msg + '</div>');
                    $(".btn-login").prop("disabled", false);
                    $("#loadingIcon").hide();
                }
            },
            error: function (xhr) {
                var errorMessage = xhr.responseText ? xhr.responseText : "Lỗi kết nối đến máy chủ!";
                $("#message").html('<div class="alert alert-danger">Lỗi: ' + errorMessage + '</div>');
                $(".btn-login").prop("disabled", false);
                $("#loadingIcon").hide();
            }
        });
    }

    $("#loginForm").submit(function (event) {
        event.preventDefault();
        handleLogin();
    });

    $("#form-register").submit(function (e) {
        debugger
        e.preventDefault();

        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = {
            email: $("#Email").val(),
            userName: $("#UserName").val(),  
            password: $("#Password").val(),
            confirmPassword: $("#ConfirmPassword").val(),
            __RequestVerificationToken: token
        };

        $.ajax({
            url: '/Account/Register',
            type: 'POST',
            data: formData,
            dataType: "json",
            success: function (res) {
                console.log(res);
                if (res.code === 200) {
                    toastr.success(res.msg || "Đăng ký thành công!");
                    setTimeout(function () {
                        window.location.href = "/Account/ConfirmOTP?email=" + encodeURIComponent(formData.Email);
                    }, 1000);
                } else if (res.code === 400){
                    toastr.error(res.msg || "Đăng ký thất bại!");
                }
            },
            error: function (xhr) {
                toastr.error("Có lỗi xảy ra, vui lòng thử lại!");
            }
        });
    });
});
