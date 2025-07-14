function closeModal() {
    $('#modalNhanVien').modal('hide');
}
function LoadForm() {
    $.ajax({
        url: '/NhanVien/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalNhanVien .modal-body').html(res);
            $('#modalNhanVien').modal('show');
        }
    })
}
function handleFormUpdateNhanVien(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/NhanVien/Edit',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalNhanVien .modal-body').html(res);
            $('#modalNhanVien').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}

function loadNhanVien() {
    $.ajax({
        url: '/NhanVien/Index',
        type: 'GET',
        success: function (data) {

            $('#ds-nhanvien').html(data);
        }
    });
}

function SaveNhanVien() {
    debugger
    var id = $('#Id').val();
    var fullName = $('#FullName').val().trim();
    if (fullName === '') {
        alert('Vui lòng nhập họ tên!');
        return;
    }
    var url = (id != null && parseInt(id) > 0) ? '/NhanVien/Update' : '/NhanVien/Add';

    $.ajax({
        url: url,
        type: 'POST',
        data: $('#form-addNhanVien').serialize(),
        success: function (res) {
            if (res.code === 200) {
                alert(res.msg);
                $('#modalNhanVien').modal('hide');
                location.reload();
                //loadNhanVien();
            } else {
                alert(res.msg);
            }
        },
        error: function (xhr) {
            console.error(xhr.responseText);
            alert('Lỗi: ' + xhr.responseText);
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/NhanVien/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    alert(res.msg);
                    location.reload();
                    //loadNhanVien();
                } else {
                    alert(res.msg);
                }
            }
        });
    }
}
function loadLichSuChucVu(nhanVienId) {
    debugger
    $.ajax({
        url: '/NhanVien/LichSuChucVu',
        type: 'GET',
        data: { id: nhanVienId },
        success: function (res) {
            var html = '';
            if (res.data.length > 0) {
                $.each(res.data, function (i, item) {
                    html += `<tr>
                        <td>${i + 1}</td>
                        <td>${item.PositionName}</td>
                        <td>${formatDate(item.TuNgay)}</td>
                        <td>${item.DenNgay != null ? formatDate(item.DenNgay) : 'Hiện tại'}</td>
                    </tr>`;
                });
            } else {
                html = '<tr><td colspan="4" class="text-center">Không có lịch sử chức vụ</td></tr>';
            }
            $('#lichSuBody').html(html);
        }

    });
    window.selectedNhanVienId = nhanVienId;

}
function showChiTiet() {
    if (!window.selectedNhanVienId) {
        $('#ThongTinBody').html('<tr><td colspan="10" class="text-center">Vui lòng chọn nhân viên</td></tr>');
        return;
    }
    loadThongTinChiTiet(window.selectedNhanVienId);
}
function loadThongTinChiTiet(nhanVienId) {
    debugger
    window.selectedNhanVienId = nhanVienId;

    $('#chiTietBody').html('<tr><td colspan="10" class="text-center">Vui lòng chọn nhân viên</td></tr>');
    $('#chiTietContainer').hide();
    $.ajax({
        url: '/NhanVien/ChiTietNhanVien',
        type: 'GET',
        data: { id: nhanVienId },
        success: function (res) {
            if (res.success) {
                // Hiển thị thông tin chi tiết lên tab "Thông tin khác"
                var html = `
                    <p><strong>Họ và tên:</strong> ${res.data.FullName}</p>
                    <p><strong>Họ và tên:</strong> ${res.data.FullName}</p>
                    <p><strong>Giới tính:</strong> ${res.data.Gender ? 'Nam' : 'Nữ'}</p>
                    <p><strong>Ngày sinh:</strong> ${formatDate(res.data.BirthDate)}</p>
                    <p><strong>Email:</strong> ${res.data.Email}</p>
                    <p><strong>Số điện thoại:</strong> ${res.data.Phone}</p>
                    <p><strong>Chức vụ hiện tại:</strong> ${res.data.PositionName}</p>
                    <p><strong>Ngày bắt đầu:</strong> ${formatDate(res.data.StartDate)}</p>
                    <p><strong>Lương:</strong> ${res.data.Salary.toLocaleString()} đ</p>
                    <p><strong>Ca làm:</strong> ${res.data.ShiftName}</p>
                `;
                $('#khac').html(html);
            }
        }
    });
}
/*function loadThongTinChiTiet(nhanVienId) {
    debugger
    window.selectedNhanVienId = nhanVienId;

    $('#chiTietBody').html('<tr><td colspan="10" class="text-center">Vui lòng chọn nhân viên</td></tr>');
    $('#chiTietContainer').hide();
    $.ajax({
        url: '/Admin/NhanVien/ChiTietNhanVien',
        type: 'GET',
        data: { id: nhanVienId },
        success: function (res) {
            var html = '';
            if (res.data.length > 0) {
                $.each(res.data, function (i, item) {
                    html += `<tr>
                        <td>${i + 1}</td>
                        <td>${item.FullName}</td>
                        <td>${item.Gender ? 'Nam' : 'Nữ'}</td>
                        <td>${formatDate(item.BirthDate)}</td>
                        <td>${item.Email}</td>
                        <td>${item.Phone}</td>
                        <td>${item.PositionName}</td>
                        <td>${formatDate(item.StartDate)}</td>
                        <td>${item.ShiftName}</td>
                        <td>${item.Salary.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                    </tr>`;
                });
            } else {
                html = '<tr><td colspan="9" class="text-center">Không có thông tin chi tiết</td></tr>';
            }
            $('#ThongTinBody').html(html);

        }
    });
}*/

function formatDate(dateStr) {
    if (!dateStr) return '';
    var date = new Date(dateStr);
    return ('0' + date.getDate()).slice(-2) + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear();
}

