var currentPickingOrderId = 0;
var pickedMedicines = [];

function openAddProductModal(orderId, orderCode, imageUrl) {
    currentPickingOrderId = orderId;
    pickedMedicines = [];

    $('#lblOrderCode').text(orderCode);
    $('#imgPrescription').attr('src', imageUrl);
    $('#totalPrescriptionAmount').text('0 đ');
    $('#tablePickedMedicines tbody').empty();

    $('#modalPrescriptionPicker').modal('show');

    // Khởi tạo autocomplete sau khi modal mở để đảm bảo phần tử input đã tồn tại
    initMedicineAutocomplete();
}

// Đảm bảo khởi tạo Autocomplete khi modal đã hiển thị
$(document).on('shown.bs.modal', '#modalPrescriptionPicker', function () {
    initMedicineAutocomplete();
});

function initMedicineAutocomplete() {
    // Kiểm tra xem input đã tồn tại chưa
    const $input = $("#searchMedicine");
    if ($input.length === 0) return;

    // Hủy autocomplete cũ nếu có để tránh trùng lặp
    if ($input.data("ui-autocomplete")) {
        $input.autocomplete("destroy");
    }

    $input.autocomplete({
        source: function (request, response) {
            $.ajax({
                url: rootPath + "Order/SearchProductAdmin", // Kiểm tra lại chính xác URL này
                type: "GET",
                dataType: "json",
                data: { term: request.term },
                success: function (data) {
                    console.log("Dữ liệu nhận được:", data); // Log ra để kiểm tra
                    // Map dữ liệu về định dạng label/value của jQuery UI
                    response($.map(data, function (item) {
                        return {
                            label: item.TenThuoc,
                            value: item.TenThuoc,
                            id: item.ThuocId,
                            price: item.GiaBan || item.GiaGoc,
                            stock: item.SoLuongTon
                        };
                    }));
                },
                error: function (xhr, status, error) {
                    console.error("Lỗi gọi API tìm kiếm:", error);
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            addToPickedList({
                ThuocId: ui.item.id,
                TenThuoc: ui.item.label,
                GiaBan: ui.item.price,
                HinhAnh: ui.item.image
            });
            $(this).val('');
            return false;
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        return $("<li>")
            .append(`<div class="p-2 border-bottom" style="cursor:pointer">
                        <div class="fw-bold text-primary">${item.label}</div>
                        <small class="text-muted">Kho: ${item.stock} | Giá: <span class="text-danger">${formatCurrency(item.price)}</span></small>
                     </div>`)
            .appendTo(ul);
    };
}

function addToPickedList(item) {
    let existing = pickedMedicines.find(x => x.ProductId === item.ThuocId);
    if (existing) {
        existing.Quantity++;
    } else {
        pickedMedicines.push({
            ProductId: item.ThuocId,
            ProductName: item.TenThuoc,
            Quantity: 1,
            UnitPrice: item.GiaBan || item.GiaGoc || 0,
            ProductImage: item.HinhAnh
        });
    }
    renderPickedTable();
    toastr.success("Đã thêm: " + item.TenThuoc);
}

function renderPickedTable() {
    let tbody = $('#tablePickedMedicines tbody');
    tbody.empty();
    let total = 0;

    if (pickedMedicines.length === 0) {
        tbody.append('<tr><td colspan="5" class="text-center text-muted">Chưa có thuốc nào được chọn</td></tr>');
    } else {
        pickedMedicines.forEach((m, index) => {
            let subtotal = m.Quantity * m.UnitPrice;
            total += subtotal;
            tbody.append(`
                <tr>
                    <td class="small">${m.ProductName}</td>
                    <td><input type="number" class="form-control form-control-sm w-100" value="${m.Quantity}" min="1" onchange="updateQty(${index}, this.value)"></td>
                    <td class="small">${formatCurrency(m.UnitPrice)}</td>
                    <td class="text-end fw-bold">${formatCurrency(subtotal)}</td>
                    <td class="text-center"><button class="btn btn-sm text-danger" onclick="removePicked(${index})"><i class="fas fa-trash"></i></button></td>
                </tr>`);
        });
    }
    $('#totalPrescriptionAmount').text(formatCurrency(total));
}

function updateQty(index, val) {
    let qty = parseInt(val);
    if (isNaN(qty) || qty < 1) qty = 1;
    pickedMedicines[index].Quantity = qty;
    renderPickedTable();
}

function removePicked(index) {
    pickedMedicines.splice(index, 1);
    renderPickedTable();
}

function savePrescriptionItems() {
    if (pickedMedicines.length === 0) {
        Swal.fire('Cảnh báo', "Vui lòng chọn ít nhất 1 loại thuốc!", 'warning');
        return;
    }

    Swal.fire({
        title: 'Xác nhận báo giá?',
        text: "Hệ thống sẽ cập nhật sản phẩm và gửi thông báo báo giá đến khách hàng.",
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Kiểm tra lại'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Order/UpdateOrderPrescription',
                type: 'POST',
                data: JSON.stringify({
                    orderId: currentPickingOrderId,
                    details: pickedMedicines
                }),
                contentType: 'application/json',
                success: function (res) {
                    if (res.code === 200) {
                        toastr.success(res.msg);
                        $('#modalPrescriptionPicker').modal('hide');
                        loadDonHang(1);
                    } else {
                        Swal.fire('Lỗi', res.msg, 'error');
                    }
                }
            });
        }
    });
}
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
            loadDonHang(page);
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
        url: rootPath + 'Order/GetDonHang',
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
                const statusOrder = {
                    "Chờ xác nhận": 1,
                    "Chờ xác nhận đơn thuốc": 1,
                    "Chờ dược sĩ soạn đơn": 1,
                    "Đã báo giá": 2,
                    "Đã xác nhận": 3,
                    "Người bán đang chuẩn bị đơn hàng": 4,
                    "Đã giao cho đơn vị vận chuyển": 5,
                    "Hoàn thành": 6,
                    "Hủy": 7
                };
                res.items.forEach(item => {
                    const currentLevel = statusOrder[item.Status] || 0;
                    let statusOptions = '';

                    if (item.OrderType === 1 || item.OrderType === 2) {
                        // LUỒNG ĐƠN THUỐC
                        const initialLabel = item.OrderType === 1 ? "Chờ xác nhận đơn thuốc" : "Chờ dược sĩ soạn đơn";
                        statusOptions = `
                            <option value="${initialLabel}" ${item.Status === initialLabel ? "selected" : ""} ${currentLevel > 1 ? "disabled" : ""}>${initialLabel}</option>
                            <option value="Đã báo giá" ${item.Status === "Đã báo giá" ? "selected" : ""} ${currentLevel > 2 ? "disabled" : ""}>Đã báo giá</option>
                            <option value="Đã xác nhận" ${item.Status === "Đã xác nhận" ? "selected" : ""} ${currentLevel > 3 ? "disabled" : ""}>Đã xác nhận (Khách đồng ý)</option>
                        `;
                    } else {
                        // LUỒNG ĐƠN THƯỜNG
                        statusOptions = `
                            <option value="Chờ xác nhận" ${item.Status === "Chờ xác nhận" ? "selected" : ""} ${currentLevel > 1 ? "disabled" : ""}>Chờ xác nhận</option>
                            <option value="Đã xác nhận" ${item.Status === "Đã xác nhận" ? "selected" : ""} ${currentLevel > 3 ? "disabled" : ""}>Đã xác nhận</option>
                        `;
                    }

                    // CÁC TRẠNG THÁI CHUNG PHÍA SAU
                    statusOptions += `
                        <option value="Người bán đang chuẩn bị đơn hàng" ${item.Status === "Người bán đang chuẩn bị đơn hàng" ? "selected" : ""} ${currentLevel > 4 ? "disabled" : ""}>Người bán đang chuẩn bị đơn hàng</option>
                        <option value="Đã giao cho đơn vị vận chuyển" ${item.Status === "Đã giao cho đơn vị vận chuyển" ? "selected" : ""} ${currentLevel > 5 ? "disabled" : ""}>Đã giao cho đơn vị vận chuyển</option>
                        <option value="Hoàn thành" ${item.Status === "Hoàn thành" ? "selected" : ""} ${currentLevel > 6 ? "disabled" : ""}>Hoàn thành</option>
                        <option value="Hủy" ${item.Status === "Hủy" ? "selected" : ""} ${(currentLevel >= 5 || currentLevel === 7) ? "disabled" : ""}>Hủy</option>
                        `;
                    const prescriptionBadge = item.HinhAnhDonThuoc
                        ? `<a href="${item.HinhAnhDonThuoc}" target="_blank" class="ms-1 text-danger" title="Xem đơn thuốc">
                        <i class="fas fa-file-prescription"></i>
                        </a>`
                        : "";
                    const orderTypeLabel = item.OrderType === 2
                        ? '<span class="badge bg-warning text-dark">Gửi nhanh</span>'
                        : (item.OrderType === 1 ? '<span class="badge bg-info">Đặt lẻ</span>' : '');
                    const row = `
                <tr>
            <td></td>
            <td>${i++}</td>
            <td>${item.OrderCode} ${prescriptionBadge}</td>
            <td>${item.CustomerName}</td>
            <td>${formatCurrency(item.TotalAmount)}</td>
            <td>
                <select class="form-select form-select-sm" onchange="updateStatus(${item.ID}, this)">
                    ${statusOptions}
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
                ${item.OrderType === 2 && item.TotalAmount === 0 ?
                `<button title="Soạn thuốc & Báo giá" class="btn btn-outline-warning btn-sm" onclick="openAddProductModal(${item.ID}, '${item.OrderCode}', '${item.HinhAnhDonThuoc}')"><i class="fas fa-pills"></i></button>` : ''}
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
        url: rootPath + 'Order/UpdateStatus',
        type: 'POST',
        data: { id: ID, status: newStatus, carrierName: carrierName },
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                loadDonHang(1);
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
            url: rootPath + 'Order/DeleteAccount',
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
    $.get(rootPath + 'Order/GetDropdownData', function (res) {
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
        url: rootPath + 'Order/ChiTietDonHang',
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
        url: rootPath + 'Shipping/GetDeliveryServices',
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

