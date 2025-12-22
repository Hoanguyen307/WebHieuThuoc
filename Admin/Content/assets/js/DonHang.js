function closeModal() {
    $('#modalNhanVien').modal('hide');
}
function formatCurrency(totalAmount) {
    if (totalAmount == null || isNaN(totalAmount)) return "0 đ";
    return Number(totalAmount).toLocaleString('vi-VN') + " đ";
}

function formatDate(dateStr) {
    if (!dateStr) return '';

    const match = /\/Date\((\d+)(?:[+-]\d+)?\)\//.exec(dateStr);
    if (!match) return 'Invalid Date';

    const timestamp = parseInt(match[1]);
    const d = dayjs(timestamp);

    if (!d.isValid()) return 'Invalid Date';

    return d.format('DD/MM/YYYY HH:mm:ss');
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
    const month = $('#month').val() || null;
    const year = $('#year').val() || null;
    const keyWord = $('#searchString').val() || null;
    const status = $('#status').val() || null;

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
                <tr>
            <td></td>
            <td>${i++}</td>
            <td>${item.OrderCode}</td>
            <td>${item.CustomerName}</td>
            <td>${formatCurrency(item.TotalAmount)}</td>
            <td>
                <select class="form-select form-select-sm"
                    onchange="updateStatus(${item.ID}, this)">
                    <option value="Chờ xác nhận" ${item.Status === "Chờ xác nhận" ? "selected" : ""}>Chờ xác nhận</option>
                    <option value="Đã xác nhận" ${item.Status === "Đã xác nhận" ? "selected" : ""}>Đã xác nhận</option>
                    <option value="Người bán đang chuẩn bị đơn hàng" ${item.Status === "Người bán đang chuẩn bị đơn hàng" ? "selected" : ""}>Người bán đang chuẩn bị đơn hàng</option>
                    <option value="Đã giao cho đơn vị vận chuyển" ${item.Status === "Đã giao cho đơn vị vận chuyển" ? "selected" : ""}>Đã giao cho đơn vị vận chuyển</option>
                    <option value="Hoàn thành" ${item.Status === "Hoàn thành" ? "selected" : ""}>Hoàn thành</option>
                    <option value="Hủy" ${item.Status === "Hủy" ? "selected" : ""}>Hủy</option>
                </select>
                <select class="form-select form-select-sm carrier-select mt-1"
                    data-order-id="${item.ID}"
                    data-selected-carrier="${item.CarrierName || ''}" 
                    style="display: ${item.Status === "Đã giao cho đơn vị vận chuyển" ? "block" : "none"}">
                    <option value="">Đang tải...</option>
                </select>

            </td>
            <td>${item.Note ?? ''}</td>
            <td>${formatDate(item.CreatedDate, true)}</td>
            <td>
                <button type="button" class="btn btn-outline-info btn-sm" onclick="loadChiTietDonHang('${item.ID}'); event.stopPropagation();"><i class="fas fa-eye"></i></button>
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.ID}'); event.stopPropagation();"><i class="fas fa-trash-alt"></i></button>
            </td>
        </tr>`;
                    tbody.append(row);
                    if (item.Status === "Đã giao cho đơn vị vận chuyển") {
                        loadDeliveryServicesForRow(item.ID, item.CarrierName);
                    }
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
function updateStatus(ID, statusElm, carrierElm = null) {
    const newStatus = $(statusElm).val();
    const carrierDropdown = $(statusElm).siblings(".carrier-select");

    if (newStatus === "Đã giao cho đơn vị vận chuyển") {
        carrierDropdown.show();

        if (carrierDropdown.find('option').length <= 1) {
            loadDeliveryServicesForRow(ID, carrierDropdown.data("selected-carrier"));
        }

        if (!carrierElm) {
            return;
        }
    } else {
        carrierDropdown.hide();
    }

    const carrierName = carrierElm ? $(carrierElm).val() : "";

    $.ajax({
        url: '/Order/UpdateStatus',
        type: 'POST',
        data: { id: ID, status: newStatus, carrierName: carrierName },
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                carrierDropdown.data("selected-carrier", carrierName);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        }
    });
}


function handleDelete(ID) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Order/DeleteAccount',
            type: 'POST',
            data: { Id: ID },
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
function loadChiTietDonHang(ID) {
    $.ajax({
        url: '/Order/ChiTietDonHang',
        type: 'GET',
        data: { id: ID },
        success: function (html) {
            $("#orderDetailModal .modal-content").html(html);
            $("#orderDetailModal").modal("show");
        },
        error: function () {
            $("#orderDetailModal .modal-content").html('<div class="text-center text-danger p-3">Lỗi khi tải dữ liệu</div>');
            $("#orderDetailModal").modal("show");
        }
    });
}

function loadDeliveryServicesForRow(orderId, selectedCarrier) {
    const ddl = $(`.carrier-select[data-order-id="${orderId}"]`);

    $.ajax({
        url: '/Shipping/GetDeliveryServices',
        type: 'GET',
        success: function (res) {
            if (res.code !== 200) return;

            let options = '<option value="">-- Chọn đơn vị vận chuyển --</option>';
            res.data.forEach(ds => {
                // Kiểm tra nếu tên trùng với giá trị đã chọn thì thêm thuộc tính selected
                const isSelected = ds.Name === selectedCarrier ? "selected" : "";
                options += `<option value="${ds.Name}" ${isSelected}>${ds.Name}</option>`;
            });
            ddl.html(options);
        }
    });
}
$(document).on("change", ".carrier-select", function () {
    const orderId = $(this).data("order-id");
    const status = $(this).prev().val();
    const carrierName = $(this).val();

    updateStatus(orderId, this.previousElementSibling, this);
});



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

