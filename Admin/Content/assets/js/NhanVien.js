function closeModal() {
    $('#modalNhanVien').modal('hide');
    $('#modalXepLich').modal('hide');
}
function renderPagination(totalPages, currentPage) {
    if (totalPages === 0) {
        $('#pagination').html('');
        return;
    }

    let html = '<ul class="pagination pagination-sm justify-content-end">';

    const pageItem = (label, page, disabled = false, active = false) => `
        <li class="page-item ${disabled ? 'disabled' : ''} ${active ? 'active' : ''}">
            <a class="page-link" href="#" data-page="${page}">${label}</a>
        </li>
    `;

    html += pageItem("&laquo;", 1, currentPage === 1);
    html += pageItem("&lsaquo;", currentPage - 1, currentPage === 1);

    const range = 2;
    const start = Math.max(1, currentPage - range);
    const end = Math.min(totalPages, currentPage + range);

    for (let i = start; i <= end; i++) {
        html += pageItem(i, i, false, currentPage === i);
    }

    html += pageItem("&rsaquo;", currentPage + 1, currentPage === totalPages);
    html += pageItem("&raquo;", totalPages, currentPage === totalPages);
    html += "</ul>";

    $('#pagination').html(html);

    $('#pagination').off('click').on('click', 'a.page-link', function (e) {
        e.preventDefault();
        const page = parseInt($(this).data("page"));
        if (!isNaN(page) && page !== currentPage) {
            loadData(page);
        }
    });
}
function formatDate(dateStr, includeTime = false) {
    console.log(dateStr);
    const match = /\/Date\((\d+)\)\//.exec(dateStr);
    const timestamp = match ? parseInt(match[1], 10) : null;

    const d = timestamp ? dayjs(timestamp) : dayjs(dateStr);

    if (!d.isValid()) return 'Invalid Date';

    return includeTime ? d.format('DD/MM/YYYY HH:mm:ss') : d.format('DD/MM/YYYY');
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

function loadNhanVien(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const name = $('#searchString').val();
    const calam = $('#calam').val();
    const chucvu = $('#chucvu').val();

    $.ajax({
        url: '/NhanVien/GetNhanVien',
        type: 'GET',
        data: {
            Month: month,
            Year: year,
            searchString: name,
            ShiftId: calam,
            PositionId: chucvu,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#nhanvien-body');
            tbody.empty();

            let index = (res.currentPage - 1) * res.pageSize + 1;
            let i = 1;

            res.items.forEach(item => {
                const row = `
            <tr id="trow_${item.Id}" onclick="loadLichSuChucVu(${item.Id})" style="cursor:pointer;">
            <td></td>
            <td>${i}</td>
            <td>${item.FullName}</td>
            <td>${item.Gender ? 'Nam' : 'Nữ'}</td>
            <td>${item.Phone}</td>
            <td>${item.PositionName}</td>
            <td>${item.ShiftName}</td>
            <td>${formatDate(item.StartDate, true)}</td>
            <td>
                <button type="button" class="btn btn-outline-success btn-sm" onclick="handleSchedule('${item.Id}'); event.stopPropagation();">
                    <i class="fas fa-calendar-alt"></i>
                </button>
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateNhanVien('${item.Id}'); event.stopPropagation();">
                    <i class="fas fa-edit"></i>
                </button>
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}'); event.stopPropagation();">
                    <i class="fas fa-trash-alt"></i>
                </button>
            </td>
        </tr>`;
                i++;
                tbody.append(row);
            });
            const pageSize = 10;
            const totalCount = res.totalCount ?? res.items.length;
            const totalPages = Math.ceil(totalCount / pageSize);
            renderPagination(totalPages, page);

        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}

function SaveNhanVien() {
    var id = $('#form-addNhanVien #Id').val();

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
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalNhanVien').modal('hide');
                setTimeout(function () {
                    location.reload();
                }, 1500);
                //loadNhanVien();
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        },
        error: function (xhr) {
            alert('Lỗi: ' + xhr.responseText);
        }
    });
}
function handleSchedule(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/NhanVien/XepLich',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalXepLich .modal-body').html(res);
            $('#modalXepLich').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}

function SaveLichLamViec() {
    const id = $('#form-XepLich #Id').val();
    const lichList = [];

    $('input[name="lichTrongTuan"]:checked').each(function () {
        const value = $(this).val(); 
        const parts = value.split('|');

        if (parts.length === 2) {
            lichList.push({
                NgayLam: parts[0],
                ShiftId: parseInt(parts[1])
            });
        }
    });

    if (lichList.length === 0) {
        toastr.warning("Vui lòng chọn ít nhất một ca làm!");
        return;
    }

    $.ajax({
        url: '/NhanVien/XepLich',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            nhanVienId: id,
            lichTrongTuan: lichList
        }),
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg);
                $('#modalXepLich').modal('hide');
                location.reload();
            } else {
                toastr.error(res.msg || "Xếp lịch thất bại");
            }
        },
        error: function () {
            toastr.error("Lỗi khi lưu lịch");
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
                    toastr.success(res.msg || "Xoá thành công");
                    location.reload();
                    //loadNhanVien();
                } else {
                    toastr.success(res.msg || "Xoá thất bại");
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
                        <td>${formatDate(item.TuNgay, false)}</td>
                        <td>${item.DenNgay != null ? formatDate(item.DenNgay, false) : 'Hiện tại'}</td>
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

$(document).ready(function () {

    loadNhanVien(1);
    renderPagination();

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();

        loadNhanVien();
    });

    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        loadNhanVien(1);
    });
    $("#loadingOverlay").hide();
});

