$(document).ready(function () {
    loadVoucher(1);
});
function formatCurrency(value) {
    return value != null ? value.toLocaleString('vi-VN') + ' đ' : '';
}
function formatDate(dateStr, includeTime = false) {
    const match = /\/Date\((\d+)\)\//.exec(dateStr);
    const timestamp = match ? parseInt(match[1], 10) : null;

    const d = timestamp ? dayjs(timestamp) : dayjs(dateStr);
    if (!d.isValid()) return 'Invalid Date';

    return includeTime ? d.format('DD/MM/YYYY HH:mm:ss') : d.format('DD/MM/YYYY');
}

function closeModal() {
    $('#modalVoucher').modal('hide');
}

function LoadForm() {
    $.ajax({
        url: '/Voucher/Add',
        type: 'GET',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalVoucher .modal-body').html(res);
            $('#modalVoucher').modal('show');
        }
    });
}

function handleFormUpdateVoucher(id) {
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/Voucher/Edit',
        type: 'GET',
        data: { ID: id },
        success: function (res) {
            $('#modalVoucher .modal-body').html(res);
            $('#modalVoucher').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
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
            loadVoucher(page);
        }
    });
}

function loadVoucher(page = 1) {
    $("#loadingOverlay").show();

    $.ajax({
        url: '/Voucher/GetDanhSachVoucher',
        type: 'GET',
        data: {
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#voucher-body');
            tbody.empty();
            let i = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {

                let discountDisplay = "";

                if (item.Percentage === "Percentage") {
                    discountDisplay = `${item.DiscountValue}%`;
                } else if (item.Percentage === "FixedAmount") {
                    discountDisplay = `${Number(item.DiscountValue).toLocaleString()}₫`;
                } else {
                    discountDisplay = item.DiscountValue ?? '';
                }
                const row = `
                    <tr>
                        <td>${i}</td>
                        <td>${item.Code}</td>
                        <td>${item.Percentage ?? ''}</td>
                        <td>${discountDisplay}</td>
                        <td>${formatDate(item.StartDate, true)}</td>
                        <td>${formatDate(item.EndDate, true)}</td>
                        <td>${item.Quantity ?? ''}</td>
                        <td>
                            <label class="switch">
                                <input type="checkbox" class="toggle-status" data-id="${item.Id}" ${item.IsActive ? "checked" : ""}>
                                <span class="slider round"></span>
                            </label>
                        </td>
                        <td>
                            <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateVoucher('${item.Id}')">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}')">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                            <button type="button" class="btn btn-outline-success btn-sm" onclick="handleDistribute('${item.Id}')">
                                <i class="fas fa-paper-plane"></i>
                            </button>
                        </td>
                    </tr>
                `;
                i++;
                tbody.append(row);
            });
            $('.toggle-status').off('change').on('change', function () {
                const id = $(this).data('id');
                const trangThai = $(this).is(':checked');

                $.ajax({
                    url: '/Voucher/ToggleHienThi',
                    type: 'POST',
                    data: { Id: id, isActive: trangThai },
                    success: function (res) {
                        if (res.code === 200) {
                            toastr.success(res.msg || "Đã cập nhật trạng thái hiển thị.");
                        } else if (res.code === 500) {
                            toastr.error(res.msg || "Cập nhật thất bại.");
                        }
                    },
                    error: function () {
                        toastr.error("Lỗi khi cập nhật.");
                    }
                });
            });
            const pageSize = 10;
            const totalCount = res.totalCount ?? res.items.length;
            const totalPages = Math.ceil(totalCount / pageSize);
            renderPagination(totalPages, page);
        },
        error: function () {
            toastr.error("Không thể tải danh sách voucher.");
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}

function SaveVoucher() {
    const form = $('#form-addVoucher')[0];
    const formData = new FormData(form);
    const id = $('#Id').val();
    formData.delete("IsActive");
    formData.append("IsActive", $('#IsActive').is(':checked'));

    const url = (id && parseInt(id) > 0) ? '/Voucher/Update' : '/Voucher/Add';

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalVoucher').modal('hide');
                setTimeout(() => location.reload(), 1500);
            } else if (typeof res === 'string') {
                $('#modalVoucher .modal-body').html(res);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa voucher này không?')) {
        $.ajax({
            url: '/Voucher/Delete',
            type: 'POST',
            data: { ID: id },
            success: function (res) {
                if (res.code === 200) {
                    toastr.success(res.msg || "Xóa thành công");
                    loadVoucher(1);
                } else {
                    toastr.error(res.msg || "Xóa thất bại");
                }
            }
        });
    }
}
