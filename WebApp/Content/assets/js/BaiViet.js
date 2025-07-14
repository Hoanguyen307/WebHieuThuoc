function closeModal() {
    $('#modalBaiViet').modal('hide');
}
function LoadForm() {
    $.ajax({
        url: '/BaiViet/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalBaiViet .modal-body').html(res);
            $('#modalBaiViet').modal('show');
        }
    })
}
function handleFormUpdateBaiViet(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/BaiViet/Edit',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalBaiViet .modal-body').html(res);
            $('#modalBaiViet').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}

function loadBaiViet() {
    $.ajax({
        url: '/BaiViet/Index',
        type: 'GET',
        success: function (data) {

            $('#ds-baiviet').html(data);
        }
    });
}


function SaveBaiViet() {
    debugger
    var form = $('#form-addBaiViet')[0];
    var formData = new FormData(form);
    var id = $('#Id').val();

    var url = (id != null && parseInt(id) > 0) ? '/BaiViet/Update' : '/BaiViet/Add';
    console.log([...formData.entries()]);

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                alert(res.msg);
                $('#modalBaiViet').modal('hide');
                location.reload();
                //loadDanhMuc(); 
            } else if (typeof res === 'string') {
                $('#modalBaiViet .modal-body').html(res);
            } else {
                alert(res.msg);
            }
        }
    });
}
function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/BaiViet/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    alert(res.msg);
                    location.reload();
                    //loadKhachHang();
                } else {
                    alert(res.msg);
                }
            }
        });
    }
}
function toggleStatus(id) {
    debugger

    $.ajax({
        url: '/BaiViet/ToggleStatus',
        type: 'POST',
        data: { Id: id },
        success: function (res) {
            if (res.code === 200) {
                alert(res.msg);
                location.reload();
            } else {
                alert(res.msg);
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi gọi API.");
        }
    });
}

