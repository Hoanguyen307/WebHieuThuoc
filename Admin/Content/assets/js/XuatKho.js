function closeModal() {
    $('#modalXuatKho').modal('hide');
    $('#modalDetail').modal('hide');
}
function LoadForm() {
    $.get('/NhapKho/Add', function (res) {
        $('#modalNhapKho .modal-body').html(res);
        $('#modalNhapKho').modal('show');

        const productsDataString = $('#ProductsData').val();
        try {
            allProducts = JSON.parse(productsDataString);
        } catch (e) {
            console.error("Lỗi khi phân tích dữ liệu sản phẩm:", e);
            allProducts = [];
        }
        // Lấy selectedProducts từ partial view
        const json = $('#ChiTietNhapKhoJson').val();
        selectedProducts = json ? JSON.parse(json) : [];

        renderProductTable();
        // Gắn sự kiện input live update tab preview
        $("#MaPhieu, #NguoiNhap, #NhaCungCap, #GhiChu").on("input", updatePreview);

        // Hiển thị preview ngay
        updatePreview();
    });
}
function handleFormUpdateNhapKho(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/NhapKho/Edit',
        type: 'GET',
        data: { NhapKhoId: id },
        success: function (res) {
            $('#modalNhapKho .modal-body').html(res);
            $('#modalNhapKho').modal('show');
            loadEditData();
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
            loadPheuXuat(page);
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
function loadPhieuXuat(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const maPhieu = $('#searchString').val();

    $.ajax({
        url: '/XuatKho/GetXuatKho',
        type: 'GET',
        data: {
            searchString: maPhieu,
            Month: month,
            Year: year,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#xuatkho-body');
            tbody.empty();
            let stt = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {
                const row = `
                <tr id="trow_${item.Id}">
                    <td>${stt}</td>
                    <td>${item.MaPhieu ?? ''}</td>
                    <td>${formatDate(item.NgayXuat, true)}</td>
                    <td>${item.NguoiXuat ?? ''}</td>
                    <td>${item.GhiChu ?? ''}</td>
                    <td>
                        <button type="button" class="btn btn-outline-info btn-sm" onclick="viewDetail(${item.Id}); event.stopPropagation();">
                            <i class="fas fa-eye"></i>  
                        </button>
                        <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateXuatKho(${item.Id}); event.stopPropagation();">
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
        url: "/XuatKho/Detail",
        type: "GET",
        data: { XuatKhoId: id },
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
function updatePreview() {
    // Tab 1: Thông tin phiếu
    const MaPhieu = $("#MaPhieu").val();
    const NguoiNhap = $("#NguoiNhap").val();
    const NhaCungCap = $("#NhaCungCap").val();
    const GhiChu = $("#GhiChu").val();

    let totalAmount = 0;
    selectedProducts.forEach(p => {
        totalAmount += p.soLuong * p.donGia;
    });
    const previewPhieu = $("#previewPhieu");
    previewPhieu.empty();
    previewPhieu.append(`<li><strong>Mã phiếu:</strong> ${MaPhieu}</li>`);
    previewPhieu.append(`<li><strong>Người nhập:</strong> ${NguoiNhap}</li>`);
    previewPhieu.append(`<li><strong>Nhà cung cấp:</strong> ${NhaCungCap}</li>`);
    previewPhieu.append(`<li><strong>Ghi chú:</strong> ${GhiChu}</li>`);
    previewPhieu.append(`<li><strong>Tổng tiền:</strong> <span class="text-danger fw-bold">${totalAmount.toLocaleString('vi-VN')}</span></li>`);

    // Tab 2: Chi tiết sản phẩm
    const tbody = $("#previewChiTiet tbody");
    tbody.empty();
    selectedProducts.forEach(p => {
        const row = `
            <tr>
                <td>${p.productName}</td>
                <td>${p.soLuong}</td>
                <td>${p.donGia.toLocaleString()}</td>
                <td>${p.hanSuDung}</td>
            </tr>
        `;
        tbody.append(row);
    });
}
function renderProductTable(page = 1) {
    currentPage = page;

    let keyword = ($("#searchProduct").val() || '').toLowerCase();

    // Lọc trên toàn bộ danh sách đã có sẵn
    let filtered = allProducts.filter(p => (p.Name || '').toLowerCase().includes(keyword));

    let start = (currentPage - 1) * pageSize;
    let paged = filtered.slice(start, start + pageSize);

    let tbody = $("#tableAllProducts tbody");
    tbody.empty();

    if (paged.length === 0) {
        tbody.append(`<tr><td colspan="5" class="text-center">Không tìm thấy sản phẩm</td></tr>`);
    } else {
        paged.forEach(p => {
            // tìm sản phẩm trong selectedProducts
            let existing = selectedProducts.find(x => x.productId === p.Id);

            let soLuong = existing ? existing.soLuong : 1;
            let donGia = existing ? existing.donGia : "";
            let hanSuDung = existing && existing.hanSuDung ? existing.hanSuDung : "";

            let row = `
                <tr>
                    <td>${p.Name}</td>
                    <td><input type="number" id="SoLuong_${p.Id}" class="form-control" min="1" value="${soLuong}" /></td>
                    <td><input type="number" id="DonGiaNhap_${p.Id}" class="form-control" min="0" step="0.01" value="${donGia}" /></td>
                    <td><input type="date" id="HanSuDung_${p.Id}" class="form-control" value="${hanSuDung}" /></td>
                    <td><button type="button" class="btn btn-success btn-sm" onclick="addProductToList(${p.Id}, '${p.Name}')">Chọn</button></td>
                </tr>`;
            tbody.append(row);
        });
    }

    renderProductPagination(filtered.length);
}

function renderProductPagination(totalItems) {
    let totalPages = Math.ceil(totalItems / pageSize);
    let container = $("#productPagination");
    container.empty();

    if (totalPages <= 1) return;

    for (let i = 1; i <= totalPages; i++) {
        let btn = `<button class="btn btn-sm ${i === currentPage ? 'btn-primary' : 'btn-light'}"
                        onclick="renderProductTable(${i})">${i}</button>`;
        container.append(btn + " ");
    }
}

function addProductToList(productId, productName) {
    let soLuong = parseInt($("#SoLuong_" + productId).val()) || 0;
    let donGia = parseFloat($("#DonGiaNhap_" + productId).val()) || 0;
    let hanSuDung = $("#HanSuDung_" + productId).val();

    if (soLuong <= 0 || donGia <= 0 || !hanSuDung) {
        toastr.warning("Vui lòng nhập đủ Số lượng, Đơn giá nhập và Hạn sử dụng!");
        return;
    }

    let existing = selectedProducts.find(p => p.productId === productId);
    if (existing) {
        existing.soLuong = soLuong;
        existing.donGia = donGia;
        existing.hanSuDung = hanSuDung;
        toastr.info("Sản phẩm đã được cập nhật!");
    } else {
        selectedProducts.push({
            productId: parseInt(productId),
            productName,
            soLuong,
            donGia,
            hanSuDung
        });
        toastr.success("Sản phẩm đã được thêm vào danh sách!");
    }
    renderSelectedProducts();
    updatePreview();
}
function renderSelectedProducts() {
    const tbody = $("#productTableBody");
    tbody.empty();

    if (selectedProducts.length === 0) {
        tbody.html(`<tr><td colspan="6" class="text-center">Chưa có sản phẩm nào được chọn.</td></tr>`);
        return;
    }

    selectedProducts.forEach((item, index) => {
        const hanSuDungFormatted = item.HanSuDung ? moment(item.HanSuDung).format('DD/MM/YYYY') : '';
        const row = `
            <tr>
                <td><input type="hidden" name="ChiTietNhapKho[${index}].ProductId" value="${item.productId}" />${item.ProductName}</td>
                <td><input type="number" name="ChiTietNhapKho[${index}].SoLuong" value="${item.soLuong}" class="form-control" min="1" required /></td>
                <td><input type="number" name="ChiTietNhapKho[${index}].DonGiaNhap" value="${item.donGiaNhap}" class="form-control" step="0.01" min="0" required /></td>
                <td><input type="date" name="ChiTietNhapKho[${index}].HanSuDung" value="${item.hanSuDung}" class="form-control" /></td>
                <td><input type="text" name="ChiTietNhapKho[${index}].GhiChu" value="${item.ghiChu ?? ''}" class="form-control" /></td>
            </tr>`;
        tbody.append(row);
    });
}

function removeProduct(index) {
    if (confirm("Bạn có chắc muốn xóa sản phẩm này?")) {
        selectedProducts.splice(index, 1);
        renderSelectedProducts();
        updatePreview();
    }
}

function loadEditData() {
    // 1. Parse tất cả sản phẩm từ ViewBag
    const productsDataString = $('#ProductsData').val();
    try {
        allProducts = JSON.parse(productsDataString);
    } catch (e) {
        console.error("Lỗi parse ProductsData:", e);
        allProducts = [];
    }

    // 2. Parse chi tiết phiếu nhập (nếu có)
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

    // 3. Render lại UI
    renderProductTable();
    renderSelectedProducts();
    updatePreview();

    // 4. Gắn sự kiện input live-update preview
    $("#MaPhieu, #NguoiNhap, #NhaCungCap, #GhiChu").on("input", updatePreview);
}


$("#searchProduct").on("keyup", function () {
    renderProductTable(1);
});

$(document).ready(function () {
    loadPhieuXuat(1);

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();
        loadPhieuXuat(1);
    });
});

function SavePhieuXuat() {
    // Thu thập dữ liệu từ form
    const formData = new FormData($('#form-addNhapKho')[0]);

    // Thêm các chi tiết sản phẩm đã chọn vào formData
    selectedProducts.forEach((p, index) => {
        formData.append(`ChiTietNhapKho[${index}].ProductId`, p.productId);
        formData.append(`ChiTietNhapKho[${index}].SoLuong`, p.soLuong);
        formData.append(`ChiTietNhapKho[${index}].DonGiaNhap`, p.donGia);
        formData.append(`ChiTietNhapKho[${index}].HanSuDung`, p.hanSuDung);
        // Có thể thêm GhiChu nếu cần
    });
    console.log(formData);
    const isEdit = $('#Id').length > 0 && $('#Id').val() > 0;
    const url = isEdit ? "/NhapKho/Update" : "/NhapKho/Add";

    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalNhapKho').modal('hide');
                setTimeout(function () {
                    loadPhieuNhap();
                }, 1500);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        },
        error: function () {
            toastr.error("Lỗi khi gọi API");
        }
    });
}
$(document).on('submit', '#form-addXuatKho', function (e) {
    e.preventDefault();
    SavePhieuXuat();
});

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/XuatKho/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    alert(res.msg);
                    location.reload();
                    //loadKhachHang();
                } else {
                    alert(res.msg);
                }
            }
        });
    }
}

