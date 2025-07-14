function closeModal() {
    $('#modalnguoidung').modal('hide');
}

function LoadAddForm() {
    $.ajax({
        url: '/Account/Add',
        type: 'GET',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalnguoidung .modal-body').html(res);
            $('#modalnguoidung').modal('show');
        }
    });
}

function handleFormUpdateNguoiDung(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/Account/Edit',
        type: 'GET',
        data: { id: id }, 
        success: function (res) {
            $('#modalnguoidung .modal-body').html(res);
            $('#modalnguoidung').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}

function loadNguoiDung() {
    $.ajax({
        url: '/Account/LoadNguoiDung',
        type: 'GET',
        success: function (data) {

            $('#nguoidung-body').html(data);
        }
    });
}


function SaveNguoiDung() {
    var id = $('#Id').val();
    //var name = ($('#UserName').val() || '').trim();

    var tendangnhap = ($('#UserName').val() || '').trim();;
    var hovaten = $('#FullName').val().trim();
    var matkhau = $('#Password').val().trim();
    var email = $('#Email').val().trim();
    var phone = $('#Phone').val().trim();
    var roleId = parseInt($('#Role').val()) || null;
    
    debugger
    var url = (id != null && parseInt(id) > 0) ? '/Account/Update' : '/Account/Add';
    var data = {
        Id: id,
        UserName: tendangnhap,
        FullName: hovaten,
        Password: matkhau,
        Email: email,
        Phone: phone,
        RoleId: roleId
    };

    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: function (res) {
            console.log("Kết quả từ server:", res);
            if (res.code === 200) {
                alert(res.msg);
                $('#modalnguoidung').modal('hide');
                location.reload();
                //loadNguoiDung();
            } else if (typeof res === 'string') {
                $('#modalnguoidung .modal-body').html(res);
            } else {
                alert(res.msg);
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Account/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    alert(res.msg);
                    location.reload();
                    //loadNguoiDung();
                } else {
                    alert(res.msg);
                }
            }
        });
    }
}
