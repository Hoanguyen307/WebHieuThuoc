$(document).ready(function () {
    function handleLogin() {
        $("#message").html("");
        $(".btn-login").prop("disabled", true);

        var tendangnhap = $("#tendangnhap").val().trim();
        var matkhau = $("#matkhau").val().trim();

        if (!tendangnhap || !matkhau) {
            $("#message").html('<div class="alert alert-warning">Vui lòng nhập đầy đủ thông tin!</div>');
            $(".btn-login").prop("disabled", false);
            return;
        }

        $.ajax({
            url: "/DangNhap/DangNhap",
            type: "POST",
            data: { tendangnhap: tendangnhap, matkhau: matkhau },
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
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = xhr.responseText ? xhr.responseText : "Lỗi kết nối đến máy chủ!";
                $("#message").html('<div class="alert alert-danger">Lỗi: ' + errorMessage + '</div>');
                $(".btn-login").prop("disabled", false);
            }
        });
    }

    $("#loginForm").submit(function (event) {
        event.preventDefault();
        handleLogin();
    });
});
