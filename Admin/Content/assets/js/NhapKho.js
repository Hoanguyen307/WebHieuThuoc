function formatDate(dateStr) {
    if (!dateStr) return '';

    const match = /\/Date\((\d+)(?:[+-]\d+)?\)\//.exec(dateStr);
    let d;

    if (match) {
        const timestamp = parseInt(match[1]);
        d = dayjs(timestamp);
    } else {
        d = dayjs(dateStr);
    }

    if (!d.isValid()) return '';

    return d.format('YYYY-MM-DD');
}
function closeModal() {
    $('#modalNhapKho').modal('hide');
    $('#productModal').modal('hide');
    $('#modalDetail').modal('hide');

}
function LoadForm() {
    $.get('/NhapKho/Add', function (res) {
        $('#modalNhapKho .modal-body').html(res);
        $('#modalNhapKho').modal('show');

        allProducts = parseJsonSafe($('#ProductsData').val(), []);
        selectedProducts = parseJsonSafe($('#SelectedProductsJson').val(), []);
        console.log('LoadForm -> allProducts:', allProducts);
        renderSelectedProducts();
    }).fail(function (xhr) {
        console.error('LoadForm error', xhr);
    });
}
function handleFormUpdateNhapKho(id) {
    if (!id || id <= 0) {
        return toastr.warning('ID không hợp lệ!');
    }
    $.get('/NhapKho/Edit', { NhapKhoId: id }, function (res) {
        $('#modalNhapKho .modal-body').html(res);
        $('#modalNhapKho').modal('show');

        allProducts = parseJsonSafe($('#ProductsData').val(), []);
        selectedProducts = parseJsonSafe($('#SelectedProductsJson').val(), []);
        console.log('Edit -> allProducts:', allProducts);
        renderSelectedProducts();
    }).fail(function (xhr) {
        console.error('handleFormUpdateNhapKho error', xhr);
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
            loadPheuNhap(page);
        }
    });
}
function loadPhieuNhap(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const maPhieu = $('#searchString').val();

    $.ajax({
        url: '/NhapKho/GetNhapKho',
        type: 'GET',
        data: {
            searchString: maPhieu,
            Month: month,
            Year: year,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#nhapkho-body');
            tbody.empty();
            let stt = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {
                const row = `
                <tr id="trow_${item.Id}">
                    <td>${stt}</td>
                    <td>${item.MaPhieu ?? ''}</td>
                    <td>${formatDate(item.NgayNhap, true) }</td>
                    <td>${item.NguoiNhap ?? ''}</td>
                    <td>${item.NhaCungCap ?? ''}</td>
                    <td>${item.GhiChu ?? ''}</td>
                    <td>
                        <button type="button" class="btn btn-outline-info btn-sm" onclick="viewDetail(${item.Id}); event.stopPropagation();">
                            <i class="fas fa-eye"></i>  
                        </button>
                        <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateNhapKho(${item.Id}); event.stopPropagation();">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete(${item.Id}); event.stopPropagation();">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </td>
                </tr>`;
                stt++;
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
function viewDetail(id) {
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: "/NhapKho/Detail",
        type: "GET",
        data: { NhapKhoId: id },
        success: function (res) {
            $('#modalDetail .modal-body').html(res);
            $('#modalDetail').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}


let allProducts = [];
let selectedProducts = [];
let currentPage = 1;
const pageSize = 10;

function parseJsonSafe(str, fallback = []) {
    try {
        if (typeof str === 'undefined' || str === null) return fallback;
        if (typeof str === 'object') return str;
        return JSON.parse(str);
    } catch (e) {
        console.error('parseJsonSafe failed', e, str);
        return fallback;
    }
}

function formatDateForInput(dateStr) {
    if (!dateStr) return '';
    var d = dayjs(dateStr);
    if (!d.isValid()) {
        var m = /\/Date\((\d+)\)\//.exec(dateStr);
        if (m) d = dayjs(parseInt(m[1], 10));
    }
    return d.isValid() ? d.format('YYYY-MM-DD') : '';
}
function formatDate(dateStr) {
    if (!dateStr) return "";
    let d = new Date(dateStr);
    if (isNaN(d)) return "";
    let month = (d.getMonth() + 1).toString().padStart(2, '0');
    let day = d.getDate().toString().padStart(2, '0');
    return `${d.getFullYear()}-${month}-${day}`;
}

function formatCurrency(val) {
    if (val === null || val === undefined || val === '') return '';
    var n = Number(val);
    if (isNaN(n)) return '';
    return n.toLocaleString('vi-VN');
}
function parseCurrency(str) {
    if (str === null || typeof str === 'undefined' || str === '') return 0;
    var s = String(str).trim().replace(/\./g, '').replace(',', '.');
    var n = parseFloat(s);
    return isNaN(n) ? 0 : n;
}

function openProductModal() {
    $('#productModal').modal('show');
    renderProductTable(1);
}

function renderProductTable(page = 1) {
    currentPage = page;

    let keyword = ($("#searchProduct").val() || '').toLowerCase();
    let filtered = allProducts.filter(p => (p.TenThuoc || '').toLowerCase().includes(keyword));

    var tbody = $("#tableAllProducts tbody").empty();
    var paged = filtered.slice((page - 1) * pageSize, page * pageSize);

    if (!paged.length) {
        tbody.html(`<tr><td colspan="6" class="text-center">Không tìm thấy sản phẩm</td></tr>`);
        renderProductPagination(0);
        return;
    }
    paged.forEach(p => {
        let existing = selectedProducts.find(x => x.ProductId === p.ThuocId);
        var safeName = JSON.stringify(p.TenThuoc);

        tbody.append(`
        <tr>
            <td style="max-width:200px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;" title="${p.TenThuoc}">
                ${p.TenThuoc}
            </td>
            <td><input type="number" id="SoLuong_${p.ThuocId}" value="${existing ? existing.SoLuong : 1}" min="1" class="form-control" /></td>
            <td><input type="text" id="DonGiaNhap_${p.ThuocId}" value="${existing ? formatCurrency(existing.DonGiaNhap) : ''}" class="form-control" /></td>
            <td><input type="date" id="NgaySanXuat_${p.ThuocId}" value="${existing ? formatDateForInput(existing.NgaySanXuat) : ''}" class="form-control" /></td>
            <td><input type="number" id="HanSuDung_${p.ThuocId}" value="${existing ? existing.HanSuDung : ''}" min="0" class="form-control" /></td>
            <td><button type="button" class="btn btn-success btn-sm" onclick='addProductToList(${p.ThuocId}, ${safeName})'><i class="fas fa-plus"></i></button></td>
        </tr>
    `);
    });
    renderProductPagination(filtered.length);
}

function renderProductPagination(totalItems) {
    var totalPages = Math.ceil(totalItems / pageSize);
    var container = $("#productPagination").empty();
    if (totalPages <= 1) return;
    for (var i = 1; i <= totalPages; i++) {
        container.append(`<li class="page-item ${i === currentPage ? 'active' : ''}"><a class="page-link" href="#" onclick="renderProductTable(${i}); return false;">${i}</a></li>`);
    }
}

function addProductToList(productId, productName) {
    let soLuong = parseInt($("#SoLuong_" + productId).val()) || 0;
    var donGia = parseCurrency($("#DonGiaNhap_" + productId).val());
    var ngaySanXuat = $("#NgaySanXuat_" + productId).val();
    var hanSuDung = $("#HanSuDung_" + productId).val();

    if (soLuong <= 0 || donGia <= 0) {
        return toastr.warning("Nhập số lượng và đơn giá hợp lệ!");
    }

    var existing = selectedProducts.find(p => p.ProductId === productId);
    if (existing) {
        Object.assign(existing, { SoLuong: soLuong, DonGiaNhap: donGia, NgaySanXuat: ngaySanXuat, HanSuDung: hanSuDung });
    } else {
        selectedProducts.push({
            ProductId: productId,
            ProductName: productName,
            SoLuong: soLuong,
            DonGiaNhap: donGia,
            NgaySanXuat: ngaySanXuat,
            HanSuDung: hanSuDung,
            GhiChu: ""
        });
    }
    renderSelectedProducts();
    $('#productModal').modal('hide');
}
function renderSelectedProducts() {
    var tbody = $("#selectedProductsTable tbody").empty();
    if (!selectedProducts || !selectedProducts.length) {
        return tbody.html(`<tr><td colspan="7" class="text-center">Chưa có sản phẩm nào.</td></tr>`);
    }

    var total = 0;
    selectedProducts.forEach((item, i) => {
        var thanhTien = (Number(item.SoLuong) || 0) * (Number(item.DonGiaNhap) || 0);
        total += thanhTien;
        tbody.append(`
            <tr>
                <td><input type="hidden" name="ChiTietNhapKho[${i}].ProductId" value="${item.ProductId}" />${item.ProductName}</td>
                <td><input type="number" class="form-control sp-soluong" data-index="${i}" value="${item.SoLuong}" min="0" /></td>
                <td><input type="text" class="form-control sp-dongia" data-index="${i}" value="${formatCurrency(item.DonGiaNhap)}" /></td>
                <td><input type="date" class="form-control sp-nsx" data-index="${i}" value="${item.NgaySanXuat ? formatDateForInput(item.NgaySanXuat) : ''}" /></td>
                <td><input type="number" class="form-control sp-hsd" data-index="${i}" value="${item.HanSuDung || ''}" min="0" /></td>
                <td class="text-end">${formatCurrency(thanhTien)}</td>
                <td><button type="button" class="btn btn-danger btn-sm" onclick="removeProduct(${i})"><i class="fas fa-trash-alt"></i></button></td>
            </tr>
        `);
    });

    $("#previewTotalAmount").text(formatCurrency(total));
    $("#TotalAmount").val(total);

    $(".sp-soluong").off('input').on('input', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].SoLuong = +$(this).val();
        renderSelectedProducts();
    });

    $(".sp-dongia").off('input').on('input', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].DonGiaNhap = parseCurrency($(this).val());
        renderSelectedProducts();
    });

    $(".sp-nsx").off('change').on('change', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].NgaySanXuat = $(this).val();
    });

    $(".sp-hsd").off('input').on('input', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].HanSuDung = +$(this).val();
    });
}

function removeProduct(index) {
    if (confirm("Bạn có chắc muốn xóa sản phẩm này?")) {
        selectedProducts.splice(index, 1);
        renderSelectedProducts();
    }
}

function loadEditData() {
    const productsDataString = $('#ProductsData').val();
    try {
        allProducts = JSON.parse(productsDataString);
    } catch (e) {
        console.error("Lỗi parse ProductsData:", e);
        allProducts = [];
    }

    const selectedProductsString = $('#SelectedProductsJson').val();
    try {
        const detailedProducts = JSON.parse(selectedProductsString);

        selectedProducts = detailedProducts.map(p => ({
            productId: p.ProductId,
            productName: p.ProductName ?? (allProducts.find(x => x.Id === p.ProductId)?.Name || ""),
            soLuong: p.SoLuong,
            donGia: p.DonGiaNhap,
            hanSuDung: p.HanSuDung ? dayjs(p.HanSuDung).format("YYYY-MM-DD") : ""
        }));
        console.log("Chi tiết phiếu nhập:", detailedProducts);

    } catch (e) {
        console.error("Lỗi parse SelectedProductsJson:", e);
        selectedProducts = [];
    }

    renderProductTable();
    renderSelectedProducts();
    updatePreview();

    $("#MaPhieu, #NguoiNhap, #NhaCungCap, #GhiChu").on("input", updatePreview);
}


$("#searchProduct").on("keyup", function () {
    renderProductTable(1);
});

$(document).ready(function () {
    loadPhieuNhap(1);

    $(document).on('input', '#searchProduct', function () {
        renderProductTable(1);
    });

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();
        loadPhieuNhap(1);
    });
});

function SavePhieuNhap() {
    var total = selectedProducts.reduce((acc, it) => acc + ((Number(it.SoLuong) || 0) * (Number(it.DonGiaNhap) || 0)), 0);
    $("#TotalAmount").val(total);

    var data = {
        Id: $("#Id").val(),
        MaPhieu: $("#MaPhieu").val(),
        NguoiNhap: $("#NguoiNhap").val(),
        NhaCungCap: $("#NhaCungCap").val(),
        GhiChu: $("#GhiChu").val(),
        TotalAmount: total,
        ChiTietNhapKho: selectedProducts
    };

    $.ajax({
        url: $("#Id").val() > 0 ? '/NhapKho/Update' : '/NhapKho/Add',
        type: "POST",
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (res) {
            if (res && res.code === 200) {
                toastr.success(res.msg || 'Lưu thành công');
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                toastr.error((res && res.msg) || 'Lỗi khi lưu phiếu nhập');
            }
        },
        error: function () {
            toastr.error("Lỗi khi gọi API");
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa phiếu nhập này không?')) {
        $.post('/NhapKho/Delete', { Id: id }, function (res) {
            if (res.code === 200) {
                toastr.success(res.msg);
                location.reload();
            } else {
                toastr.error(res.msg);
            }
        });
    }
}

