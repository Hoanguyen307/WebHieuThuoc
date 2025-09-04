function closeModal() {
    $('#modalNhanVien').modal('hide');
}
function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN') + " đ";
}
function formatDate(dateStr, includeTime = false) {
    if (!dateStr) return '';

    const d = dayjs(dateStr, 'YYYY/MM/DD', true);

    if (!d.isValid()) return 'Invalid Date';

    return includeTime ? d.format('DD/MM/YYYY HH:mm:ss') : d.format('DD/MM/YYYY');
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
function loadDonHang(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const keyWord = $('#searchString').val();
    const status = $('#status').val() ? parseInt($('#status').val()) : null;
    const start = Date.now();
    const minDelay = 500;

    $.ajax({
        url: '/Order/GetDonHang',
        type: 'GET',
        data: {
            Month: month,
            Year: year,
            searchString: keyWord,
            Status: status,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const elapsed = Date.now() - start;
            const remaining = Math.max(0, minDelay - elapsed);
            setTimeout(() => {
            const tbody = $('#donhang-body');
            tbody.empty();
            let index = (res.currentPage - 1) * res.pageSize + 1;
            let i = 1;

            res.items.forEach(item => {
                const row = `
                <tr id="trow_${item.Id}" onclick="loadChiTietDonHang(${item.Id})" style="cursor:pointer;">
            <td></td>
            <td>${index + 1}</td>
            <td>${item.orderCode}</td>
            <td>${item.customerName}</td>
            <td>${formatCurrency(item.totalAmount)}</td>
            <td>${item.status}</td>
            <td>${item.note ?? ''}</td>
            <td>${formatDate(item.createdDate)}</td>
            <td>
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateDonHang('${item.id}'); event.stopPropagation();"><i class="fas fa-edit"></i></button>
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.id}'); event.stopPropagation();"><i class="fas fa-trash-alt"></i></button>
            </td>
        </tr>`;
                i++;
                tbody.append(row);
            });
            const pageSize = 10;
            const totalCount = res.totalCount ?? res.items.length;
            const totalPages = Math.ceil(totalCount / pageSize);
            renderPagination(totalPages, page);
            }, remaining);
        },
        complete: function () {
            $("#loadingOverlay").hide();
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

$(document).ready(function () {

    loadDonHang(1);
    renderPagination();

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();

        loadDonHang();
    });

    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        loadDonHang(1);
    });
    $("#loadingOverlay").hide();
});

