function closeModal() {
    $('#modalNhanVien').modal('hide');
}

function loadDonHang() {
    $.ajax({
        url: '/Order/Index',
        type: 'GET',
        success: function (data) {

            $('#ds-donhang').html(data);
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Order/DeleteAccount',
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
function renderDropdown(selectSelector, data, defaultOptionText = "") {
    const selectElement = document.querySelector(selectSelector);
    if (!selectElement) {
        console.error(`Không tìm thấy phần tử select với selector: ${selectSelector}`);
        return;
    }

    selectElement.innerHTML = '';

    const defaultOption = document.createElement('option');
    defaultOption.value = "";
    defaultOption.textContent = defaultOptionText;
    selectElement.appendChild(defaultOption);

    if (data && Array.isArray(data)) {
        data.forEach(item => {
            const option = document.createElement('option');
            option.value = item.id || item.Id;
            option.textContent = item.name || item.Name;
            selectElement.appendChild(option);
        });
    }
}

function loadAllDropdowns() {
    $.get('/Order/GetDropdownData', function (res) {
        if (!res.success) {
            alert("Không thể tải dropdown");
            return;
        }
        renderDropdown('#month', res.months, 'Tháng');
        if (res.currentMonth) {
            document.querySelector('#month').value = res.currentMonth.toString();
        }
        renderDropdown('#year', res.year, 'Năm');
        if (res.currentYear) {
            document.querySelector('#year').value = res.currentYear.toString();
        }
    }).fail(function () {
        alert("Lỗi khi tải dropdown từ server");
    });
}
function loadLichSuDonHang(orderId) {
    debugger
    $.ajax({
        url: '/Order/LichSuDonHang',
        type: 'GET',
        data: { id: orderId },
        success: function (res) {
            var html = '';
            if (res.data.length > 0) {
                $.each(res.data, function (i, item) {
                    html += `<tr>
                        <td>${i + 1}</td>
                        <td>${item.Productld}</td>
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

function formatDate(dateStr) {
    if (!dateStr) return '';
    var date = new Date(dateStr);
    return ('0' + date.getDate()).slice(-2) + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear();
}

