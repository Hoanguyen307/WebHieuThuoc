function formatDate1(dateStr) {
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

    return d.format('DD/MM/YYYY');
}
function closeModal() {
    $('#modalNhapKho').modal('hide');
    $('#productModal').modal('hide');
    $('#modalDetail').modal('hide');

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
                    <td>${formatDate(item.NgayNhap, true)}</td>
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

var allProducts = [];
var selectedProducts = [];
var pageSize = 10;

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
function LoadForm() {
    $.get('/NhapKho/Add', function (res) {
        $('#modalNhapKho .modal-body').html(res);

        selectedProducts = [];
        allProducts = window.tempProducts || [];

        selectedCurrentPage = 1;
        renderSelectedProducts();
        $('#modalNhapKho').modal('show');
    });
}

function handleFormUpdateNhapKho(id) {
    if (!id || id <= 0) return toastr.warning('ID không hợp lệ!');

    $.get('/NhapKho/Edit', { NhapKhoId: id }, function (res) {
        $('#modalNhapKho .modal-body').html(res);

        allProducts = window.tempProducts || [];
        selectedProducts = window.tempSelected || [];

        renderSelectedProducts();
        $('#modalNhapKho').modal('show');
    }).fail(function (xhr) {
        console.error('Lỗi tải form cập nhật', xhr);
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
            loadPhieuNhap(page);
        }
    });
}

function openProductModal() {
    $('#productModal').modal('show');
    renderProductTable(1);
}
function loadAllProductsFromServer() {
    return $.get('/NhapKho/GetAllProducts', function (res) {
        allProducts = res;
    });
}
function renderProductTable(page = 1) {
    currentPage = page;
    let keyword = ($('#searchProduct').val() || '').toLowerCase();
    let filtered = allProducts.filter(p => (p.TenThuoc || '').toLowerCase().includes(keyword));

    let tbody = $("#tableAllProducts tbody").empty();
    let paged = filtered.slice((page - 1) * pageSize, page * pageSize);

    paged.forEach(p => {
        let $tr = $('<tr>');

        let $tdName = $('<td style="max-width:200px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;">')
            .text(p.TenThuoc)
            .attr('title', p.TenThuoc);

        let giaGocHienThi = p.GiaGoc ? formatCurrency(p.GiaGoc) : "";

        $tr.append($tdName);
        $tr.append(`<td><input type="number" id="SoLuong_${p.ThuocId}" value="50" min="1" class="form-control" /></td>`);

        $tr.append(`<td><input type="text" id="DonGiaNhap_${p.ThuocId}" value="${giaGocHienThi}" class="form-control input-format-currency" /></td>`);

        $tr.append(`<td><input type="date" id="NgaySanXuat_${p.ThuocId}" class="form-control" /></td>`);
        $tr.append(`<td><input type="number" id="HanSuDung_${p.ThuocId}" value="24" class="form-control" /></td>`);

        let $btn = $('<button type="button" class="btn btn-success btn-sm"><i class="fas fa-plus"></i></button>')
            .click(function () { addProductToList(p.ThuocId); });

        $tr.append($('<td>').append($btn));
        tbody.append($tr);
    });

    renderProductPagination(filtered.length);
}
function renderProductPagination(totalItems) {
    var totalPages = Math.ceil(totalItems / pageSize);
    var container = $("#productPagination").empty();
    if (totalPages <= 1) return;

    let html = '<ul class="pagination pagination-sm justify-content-center">';

    html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="renderProductTable(${currentPage - 1}); return false;">‹</a>
             </li>`;

    const maxVisible = 5;
    let start = Math.max(1, currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(totalPages, start + maxVisible - 1);

    if (end - start + 1 < maxVisible) {
        start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
        html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="renderProductTable(${i}); return false;">${i}</a>
                 </li>`;
    }

    html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="renderProductTable(${currentPage + 1}); return false;">›</a>
             </li>`;

    html += '</ul>';
    container.append(html);
}

function renderSelectedPagination(totalItems) {
    var totalPages = Math.ceil(totalItems / selectedPageSize);
    var container = $("#SelectedPagination").empty();
    if (totalPages <= 1) return;

    var paginationHtml = `<ul class="pagination pagination-sm justify-content-end">`;
    for (var i = 1; i <= totalPages; i++) {
        paginationHtml += `
            <li class="page-item ${i === selectedCurrentPage ? 'active' : ''}">
                <a class="page-link" href="javascript:void(0)" onclick="renderSelectedProducts(${i})">${i}</a>
            </li>`;
    }
    paginationHtml += `</ul>`;
    container.append(paginationHtml);
}
function addProductToList(productId) {
    let productOriginal = allProducts.find(x => x.ThuocId === productId);
    if (!productOriginal) return;

    let soLuong = +$("#SoLuong_" + productId).val();
    let donGia = parseCurrency($("#DonGiaNhap_" + productId).val());
    let ngaySanXuat = $("#NgaySanXuat_" + productId).val();
    let hanSuDung = $("#HanSuDung_" + productId).val();

    if (soLuong <= 0 || donGia <= 0) {
        return toastr.warning("Nhập số lượng và đơn giá hợp lệ!");
    }

    let existing = selectedProducts.find(p => p.ProductId === productId);
    let item = {
        ProductId: productId,
        ProductName: productOriginal.TenThuoc,
        SoLuong: soLuong,
        DonGiaNhap: donGia,
        NgaySanXuat: ngaySanXuat ? ngaySanXuat : null, // Đảm bảo gửi null nếu trống
        HanSuDung: hanSuDung,
        GhiChu: "" // Thêm trường này cho đủ bộ Model
    };
    if (existing) {
        Object.assign(existing, item);
    } else {
        selectedProducts.push(item);
    }

    renderSelectedProducts();
    toastr.success("Đã thêm " + productOriginal.TenThuoc);
}

var selectedCurrentPage = 1;
var selectedPageSize = 10;

function renderSelectedProducts(page = 1) {
    selectedCurrentPage = page;
    var tbody = $("#selectedProductsTable tbody").empty();

    if (!selectedProducts || !selectedProducts.length) {
        $("#SelectedPagination").empty();
        return tbody.html(`<tr><td colspan="7" class="text-center">Chưa có sản phẩm nào.</td></tr>`);
    }

    var start = (selectedCurrentPage - 1) * selectedPageSize;
    var end = start + selectedPageSize;
    var pagedItems = selectedProducts.slice(start, end);

    pagedItems.forEach((item, index) => {
        var i_original = start + index;
        var thanhTien = (Number(item.SoLuong) || 0) * (Number(item.DonGiaNhap) || 0);

        tbody.append(`
            <tr>
                <td style="max-width:300px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap;" title="${item.ProductName}">
                    <input type="hidden" name="ChiTietNhapKho[${i_original}].ProductId" value="${item.ProductId}" />${item.ProductName}
                </td>
                <td><input type="number" class="form-control sp-soluong" data-index="${i_original}" value="${item.SoLuong}" min="1" /></td>
                <td><input type="text" class="form-control sp-dongia" data-index="${i_original}" value="${formatCurrency(item.DonGiaNhap)}" /></td>
                <td><input type="date" class="form-control sp-nsx" data-index="${i_original}" value="${item.NgaySanXuat ? formatDateForInput(item.NgaySanXuat) : ''}" /></td>
                <td><input type="number" class="form-control sp-hsd" data-index="${i_original}" value="${item.HanSuDung || ''}" min="0" /></td>
                <td class="text-end">${formatCurrency(thanhTien)}</td>
                <td><button type="button" class="btn btn-danger btn-sm" onclick="removeProduct(${i_original})"><i class="fas fa-trash-alt"></i></button></td>
            </tr>
        `);
    });

    renderSelectedPagination(selectedProducts.length);

    var total = selectedProducts.reduce((acc, it) => acc + ((Number(it.SoLuong) || 0) * (Number(it.DonGiaNhap) || 0)), 0);
    $("#previewTotalAmount").text(formatCurrency(total));
    $("#TotalAmount").val(total);

    attachSelectedTableEvents();
}
function attachSelectedTableEvents() {
    // Cập nhật Số lượng
    $(".sp-soluong").off('change input').on('change input', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].SoLuong = parseFloat($(this).val()) || 0;
        updateInstantTotal(this, idx);
    });

    // Cập nhật Đơn giá
    $(".sp-dongia").off('change input').on('change input', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].DonGiaNhap = parseCurrency($(this).val());
        updateInstantTotal(this, idx);
    });

    // Cập nhật Ngày sản xuất
    $(".sp-nsx").off('change').on('change', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].NgaySanXuat = $(this).val();
    });

    // Cập nhật Hạn sử dụng
    $(".sp-hsd").off('change input').on('change input', function () {
        var idx = $(this).data('index');
        selectedProducts[idx].HanSuDung = $(this).val();
    });
}

function updateInstantTotal(el, idx) {
    var thanhTien = (selectedProducts[idx].SoLuong || 0) * (selectedProducts[idx].DonGiaNhap || 0);
    $(el).closest('tr').find('.text-end').text(formatCurrency(thanhTien));

    var total = selectedProducts.reduce((acc, it) => acc + ((it.SoLuong || 0) * (it.DonGiaNhap || 0)), 0);
    $("#previewTotalAmount").text(formatCurrency(total));
    $("#TotalAmount").val(total);
}
function removeProduct(i) {
    selectedProducts.splice(i, 1);
    renderSelectedProducts();
}

$("#searchProduct").on("keyup", function () {
    renderProductTable(1);
});

function SavePhieuNhap() {
    var idValue = parseInt($("input[name='Id']").val()) || 0;

    if (!selectedProducts || selectedProducts.length === 0) {
        toastr.warning("Vui lòng thêm ít nhất một sản phẩm vào danh sách!");
        return;
    }

    var data = {
        Id: idValue,
        MaPhieu: $("#MaPhieu").val(),
        GhiChu: $("#GhiChu").val(),
        TotalAmount: parseCurrency($("#previewTotalAmount").text()) || 0,
        ChiTietNhapKho: selectedProducts.map(item => {
            return {
                ProductId: parseInt(item.ProductId),
                ProductName: item.ProductName,
                SoLuong: parseInt(item.SoLuong) || 0,
                DonGiaNhap: parseFloat(item.DonGiaNhap) || 0,
                NgaySanXuat: (item.NgaySanXuat && item.NgaySanXuat.trim() !== "") ? item.NgaySanXuat : null,
                HanSuDung: parseInt(item.HanSuDung) || 0
            };
        })
    };


    $.ajax({
        url: idValue > 0 ? '/NhapKho/Update' : '/NhapKho/Add',
        type: 'POST',
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $("button[onclick='SavePhieuNhap()']").prop('disabled', true).text('Đang lưu...');
        },
        success: function (res) {
            if (res && (res.code === 200 || res.success)) {
                toastr.success(res.msg || 'Lưu thành công');
                $('#modalNhapKho').modal('hide');
                setTimeout(() => { location.reload(); }, 1000);
            } else {
                toastr.error(res.msg || 'Lỗi từ phía máy chủ');
                $("button[onclick='SavePhieuNhap()']").prop('disabled', false).text('Lưu');
            }
        },
        error: function () {
            toastr.error('Không thể lưu phiếu nhập! Vui lòng kiểm tra lại kết nối.');
            $("button[onclick='SavePhieuNhap()']").prop('disabled', false).text('Lưu');
        }
    });
}
function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa phiếu nhập này không?')) {
        $.post('/NhapKho/DeleteAccount', { Id: id }, function (res) {
            if (res.code === 200) {
                toastr.success(res.msg);
                loadPhieuNhap();
            } else {
                toastr.error(res.msg);
            }
        });
    }
}

var detailItems = [];
var detailCurrentPage = 1;
var detailPageSize = 10;

function viewDetail(id) {
    if (!id || id <= 0) {
        return;
    }

    $.ajax({
        url: "/NhapKho/Detail",
        type: "GET",
        data: { NhapKhoId: id },
        success: function (res) {
            $('#modalDetail .modal-body').html(res);

            var jsonStr = $('#modalDetail #data-detail-json').text();

            detailItems = parseJsonSafe(jsonStr, []);

            if (detailItems.length > 0) {
                renderDetailTable(1);
            } else {
                $("#detailBody").html('<tr><td colspan="4" class="text-center">Không có dữ liệu chi tiết</td></tr>');
            }

            $('#modalDetail').modal('show');
        },
        error: function () {
            toastr.error("Không thể tải chi tiết phiếu nhập!");
        }
    });
}
function renderDetailTable(page = 1) {
    detailCurrentPage = page;
    var tbody = $("#detailBody").empty();

    if (!detailItems || detailItems.length === 0) {
        tbody.append('<tr><td colspan="4" class="text-center">Không có dữ liệu chi tiết</td></tr>');
        return;
    }

    var start = (detailCurrentPage - 1) * detailPageSize;
    var end = start + detailPageSize;
    var pagedItems = detailItems.slice(start, end);

    pagedItems.forEach(item => {
        tbody.append(`
            <tr>
                <td>${item.ProductName}</td>
                <td>${item.SoLuong}</td>
                <td>${formatCurrency(item.DonGiaNhap)}</td>
                <td>${item.HanSuDung}</td>
            </tr>
        `);
    });

    renderDetailPagination(detailItems.length);
}

function renderDetailPagination(totalItems) {
    var totalPages = Math.ceil(totalItems / detailPageSize);
    var container = $("#detailPagination").empty();

    if (totalPages <= 1) return;

    var html = `<ul class="pagination pagination-sm justify-content-center">`;
    for (var i = 1; i <= totalPages; i++) {
        html += `<li class="page-item ${i === detailCurrentPage ? 'active' : ''}">
                    <a class="page-link" href="javascript:void(0)" onclick="renderDetailTable(${i})">${i}</a>
                 </li>`;
    }
    html += `</ul>`;
    container.append(html);
}

$(document).ready(function () {
    $('#modalNhapKho').on('hidden.bs.modal', function (e) {
        if (e.target.id === 'modalNhapKho') {
            selectedProducts = [];
            selectedCurrentPage = 1;
            if ($("#form-addNhapKho").length > 0) {
                $("#form-addNhapKho")[0].reset();
                $("#form-addNhapKho input[name='Id']").val(0);
            }
            $("#Id").val(0);
        }
    });

    $(document).on('input', '#searchProduct', function () {
        renderProductTable(1);
    });

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();
        loadPhieuNhap(1);
    });
    loadPhieuNhap(1);
    $("#loadingOverlay").hide();
});


