function closeModal() {
    $('#modalProduct').modal('hide');
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
function formatCurrency(value) {
    return value != null ? value.toLocaleString('vi-VN') + ' đ' : '';
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
function LoadForm() {
    $.ajax({
        url: '/Products/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalProduct .modal-body').html(res);
            $('#modalProduct').modal('show');
        }
    })
}
function handleFormUpdateProduct(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/Products/Edit',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalProduct .modal-body').html(res);
            $('#modalProduct').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}

function loadData(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const categoryId = $('#selectCategory').val();
    const name = $('#searchString').val();
    const minPrice = $('#MinPrice').val();
    const maxPrice = $('#MaxPrice').val();
    $.ajax({
        url: '/Products/GetProduct',
        type: 'GET',
        data: {
            Month: month,
            Year: year,
            CategoryId: categoryId,
            searchString: name,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#sanpham-body');
            tbody.empty();

            let index = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {
                const row = `
                    <tr id="trow_${item.Id}">
            <td></td>
            <td>${index + 1}</td>
            <td><img src="${item.Image}" alt="Ảnh" style="height:50px" /></td>
            <td>${item.Name}</td>
            <td>${item.CategoryName || ''}</td>
            <td>${item.Quantity}</td>
            <td>${item.Sold}</td>
            <td>${formatCurrency(item.Price)}</td>
            <td>${formatCurrency(item.SalePrice)}</td>
            <td>
                <span class="badge badge-${item.IsActive ? 'success' : 'secondary'}">
                    ${item.IsActive ? 'Hiện' : 'Ẩn'}
                </span>
            </td>
            <td>
                <span class="badge badge-${item.IsFeatured ? 'warning' : 'light'}">
                    ${item.IsFeatured ? 'Nổi bật' : '-'}
                </span>
            </td>
            <td>
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateProduct('${item.Id}')">
                    <i class="fas fa-edit"></i>
                </button>
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}')">
                    <i class="fas fa-trash-alt"></i>
                </button>
            </td>
        </tr>`;
                tbody.append(row);
            });
            const pageSize = 10;
            const totalCount = res.totalCount ?? res.items.length;
            const totalPages = Math.ceil(totalCount / pageSize);
            renderPagination(totalPages, page);

        },
        error: function () {
            alert("Lỗi khi tải danh sách tiến trình");
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}

function SaveProduct() {
    debugger
    var form = $('#form-addProduct')[0];
    var formData = new FormData(form);
    var id = $('#Id').val();
    formData.delete("IsActive");
    formData.delete("IsFeatured");
    formData.append("IsActive", $('#IsActive').is(':checked'));
    formData.append("IsFeatured", $('#IsFeatured').is(':checked'));

    var url = (id != null && parseInt(id) > 0) ? '/Products/Update' : '/Products/Add';
    console.log([...formData.entries()]);
    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalProduct').modal('hide');
                location.reload();
                //loadDanhMuc();
            } else if (typeof res === 'string') {
                $('#modalProduct .modal-body').html(res);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Products/DeleteAccount',
            type: 'POST',
            data: { ID: id },
            success: function (res) {
                if (res.code === 200) {
                    toastr.success(res.msg || "Xoá thành công");
                    location.reload();
                    //loadProduct();
                } else {
                    toastr.error(res.msg || "Xoá thất bại");
                }
            }
        });
    }
}

$(document).ready(function () {
    
    loadData(1);
    renderPagination();

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();

        loadData();
    });

    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        loadData(1);
    });
    $("#loadingOverlay").hide();
});
