$(document).ready(function () {
    loadDungTich(1);
});

function closeModal() {
    $('#modalDungTich').modal('hide');
}

function formatCurrency(value) {
    return value != null ? value.toLocaleString('vi-VN') + ' đ' : '';
}

function handleFormUpdateDungTich(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/DungTich/Edit',
        type: 'GET',
        data: { ID: id },
        success: function (res) {
            $('#modalDungTich .modal-body').html(res);
            $('#modalDungTich').modal('show');
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
            loadDungTich(page);
        }
    });
}
function loadDungTich(page = 1) {
    $("#loadingOverlay").show();

    $.ajax({
        url: '/DungTich/GetDanhSachDungTich',
        type: 'GET',
        data: {
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#dungtich-body');
            tbody.empty();
            let index = (res.currentPage - 1) * res.pageSize + 1;
            let i = 1;
            //let index = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {
                const row = `
                    <tr>
                        <td>${i}</td>
                        <td>${item.ProductName}</td>
                        <td>${item.DungTichValue ?? ''}</td>
                        <td>${item.SoLuong}</td>
                        <td>${formatCurrency(item.Gia)}</td>
                        <td>${formatCurrency(item.SalePrice)}</td>
                        <td>
                            <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateDungTich('${item.Id}')">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}')">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                        </td>
                    </tr>
                `;
                i++;
                tbody.append(row);
            });

            const pageSize = 10;
            const totalCount = res.totalCount ?? res.items.length;
            const totalPages = Math.ceil(totalCount / pageSize);
            renderPagination(totalPages, page);
        },
        error: function () {
            toastr.error("Không thể tải danh mục.");
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}
function SaveDungTich() {
    debugger
    var form = $('#form-addDungTich')[0];
    var formData = new FormData(form);
    var id = $('#Id').val();

    $.ajax({
        url: '/Products/Update',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalDungTich').modal('hide');
                setTimeout(function () {
                    location.reload();
                }, 1500);
                //loadDanhMuc();
            } else if (typeof res === 'string') {
                $('#modalDungTich .modal-body').html(res);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/DungTich/DeleteAccount',
            type: 'POST',
            data: { ID: id },
            success: function (res) {
                if (res.code === 200) {
                    alert(res.msg);
                    location.reload();
                    //loadDanhmuc();
                } else {
                    alert(res.msg);
                }
            }
        });
    }
}
