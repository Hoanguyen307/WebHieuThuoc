$(document).ready(function () {
    loadFlashSale(1);
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
    $('#modalFlashSale').modal('hide');
}

function LoadForm() {
    $.ajax({
        url: rootPath + 'FlashSale/Add',
        type: 'GET',
        success: function (res) {
            if ($('#form-addFlashSale').length > 0) {
                $('#form-addFlashSale')[0].reset();
            }
            $('#Id').val('');
            $('#modalFlashSale .modal-body').html(res);
            $('#modalFlashSale').modal('show');
        }
    });
}

function handleFormUpdateFlashSale(id) {
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: rootPath + 'FlashSale/Edit',
        type: 'GET',
        data: { ID: id },
        success: function (res) {
            $('#modalFlashSale .modal-body').html(res);
            $('#modalFlashSale').modal('show');
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
            loadFlashSale(page);
        }
    });
}

function loadFlashSale(page = 1) {
    $("#loadingOverlay").show();

    $.ajax({
        url: rootPath + 'FlashSale/GetFlashSale',
        type: 'GET',
        data: {
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#flashSale-body');
            tbody.empty();
            let i = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {
                const row = `
                    <tr>
                        <td>${i}</td>
                        <td>${item.Title ?? ''}</td>
                        <td>${formatDate(item.StartTime, true)}</td>
                        <td>${formatDate(item.EndTime, true)}</td>
                        <td>${item.DiscountPercent ?? 0}%</td>
                        <td>${formatCurrency(item.DiscountAmount ?? 0)}</td>
                        <td>${item.CreatedBy ?? ''}</td>
                        <td>${formatDate(item.CreatedDate, true)}</td>
                        <td>
                <label class="switch">
                    <input type="checkbox" class="toggle-status" data-id="${item.Id}" ${item.IsActive ? "checked" : ""}>
                    <span class="slider round"></span>
                </label>
            </td>
                        <td>
                            <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateFlashSale('${item.Id}')">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}')">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                            <button type="button" class="btn btn-outline-success btn-sm" onclick="openSelectProducts(${item.Id})">
                                <i class="fas fa-box"></i> Sản phẩm
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
                    url: rootPath + 'FlashSale/ToggleHienThi',
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

            const pageSize = res.pageSize || 10;
            const totalCount = res.totalCount ?? res.items.length;
            const totalPages = Math.ceil(totalCount / pageSize);
            renderPagination(totalPages, page);
        },
        error: function () {
            toastr.error("Không thể tải danh sách FlashSale.");
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}


function SaveFlashSale() {
    const form = $('#form-addFlashSale')[0];
    const formData = new FormData(form);
    const id = $('#Id').val();
    formData.delete("IsActive");
    formData.append("IsActive", $('#IsActive').is(':checked'));

    const url = (id && parseInt(id) > 0) ? rootPath + 'FlashSale/Update' : rootPath + 'FlashSale/Add';

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Lưu Flash Sale thành công.");
                closeModal();
                loadFlashSale();
            } else if (typeof res === 'string') {
                $('#modalFlashSale .modal-body').html(res);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa chương trình FlashSale này không?')) {
        $.ajax({
            url: rootPath + 'FlashSale/Delete',
            type: 'POST',
            data: { ID: id },
            success: function (res) {
                if (res.code === 200) {
                    toastr.success(res.msg || "Xóa thành công");
                    loadFlashSale();
                } else {
                    toastr.error(res.msg || "Xóa thất bại");
                }
            }
        });
    }
}

function renderProductPagination(totalPages, currentPage, keyword) {
    if (totalPages === 0) {
        $('#productPagination').html('');
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

    $('#productPagination').html(html);

    $('#productPagination').off('click').on('click', 'a.page-link', function (e) {
        e.preventDefault();
        const page = parseInt($(this).data("page"));
        if (!isNaN(page) && page !== currentPage) {
            loadProducts(keyword, page);
        }
    });
}

var currentFlashSaleId = 0;

function openSelectProducts(flashSaleId) {
    currentFlashSaleId = flashSaleId;
    $("#selectProductsModal").modal("show");
    loadProducts();
}

function loadProducts(keyword = "", page = 1) {
    $.get(rootPath + "FlashSaleProduct/GetProducts", {
            flashSaleId:
            currentFlashSaleId,
            keyword: keyword,
            page: page,
            pageSize: 10
    }, function (res) {
        let html = "";
        if (res.items && res.items.length > 0) {
            res.items.forEach(p => {

                let stockValue = p.FlashStock ? p.FlashStock : 10;
                let isChecked = p.IsSelected ? "checked" : "";
                html += `<tr>
                    <td><input type="checkbox" class="product-checkbox" value="${p.ThuocId}" ${p.IsSelected ? "checked" : ""}></td>
                    <td><img src="${p.HinhAnh}" alt="Ảnh" style="height:50px" /></td>
                    <td>${p.TenThuoc}</td>
                    <td>${formatCurrency(p.GiaGoc)}</td>
                    <td>
                        <input type="number" class="form-control form-control-sm flash-stock-input"
                               value="${stockValue}" min="1" 
                               style="width:80px" ${!p.IsSelected ? 'disabled' : ''}>
                    </td>
                </tr>`;
            });

            const pageSize = res.pageSize || 10;
            const totalCount = res.totalCount ?? res.items.length;
            const totalPages = Math.ceil(totalCount / pageSize);
            renderProductPagination(totalPages, page, keyword);

        } else {
            html = `<tr><td colspan="3" class="text-center">Không có sản phẩm</td></tr>`;
        }
        $("#productList").html(html);
        $('.product-checkbox').change(function () {
            $(this).closest('tr').find('.flash-stock-input').prop('disabled', !this.checked);
        });
    });
}

let productSearchTimeout = null;
$("#searchProduct").on("keyup", function () {
    const val = $(this).val();
    clearTimeout(productSearchTimeout);
    productSearchTimeout = setTimeout(() => loadProducts(val), 300);
});

function saveSelectedProducts() {
    const productData = [];

    $(".product-checkbox:checked").each(function () {
        const row = $(this).closest('tr');
        const productId = parseInt($(this).val());
        const flashStock = parseFloat(row.find('.flash-stock-input').val());

        if (!isNaN(productId)) {
            productData.push({
                ThuocId: productId,
                FlashStock: isNaN(flashStock) ? 0 : flashStock
            });
        }
    });
    $.ajax({
        url: rootPath + "FlashSaleProduct/Add",
        type: "POST",
        data: JSON.stringify({
            flashSaleId: currentFlashSaleId,
            products: productData
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        traditional: true,
        success: function (res) {
            if (res.success) {
                toastr.success("Đã lưu sản phẩm cho FlashSale");
                $("#selectProductsModal").modal("hide");
            } else toastr.error("Lưu thất bại");
        }
    });
}
