$(document).ready(function () {
    loadDanhMuc(1);
});

function closeModal() {
    $('#modalDanhMuc').modal('hide');
}
function LoadForm() {
    $.ajax({
        url: '/ProductCategory/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#loadDanhMuc').val('');
            $('#modalDanhMuc .modal-body').html(res);
            $('#modalDanhMuc').modal('show');
        }
    })
}
function handleFormUpdateDanhMuc(id) {
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/ProductCategory/Edit',
        type: 'GET',
        data: { DanhMucId: id },
        success: function (res) {
            $('#modalDanhMuc .modal-body').html(res);
            $('#modalDanhMuc').modal('show');
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
            loadDanhMuc(page);
        }
    });
}
function loadDanhMuc(page = 1) {
    $("#loadingOverlay").show();

    $.ajax({
        url: '/ProductCategory/GetDanhSachDanhMuc',
        type: 'GET',
        data: {
            page: page,
            pageSize: 13
        },
        success: function (res) {
            const tbody = $('#danhmuc-body');
            tbody.empty();
            let index = (res.currentPage - 1) * res.pageSize + 1;
            let i = 1;
            //let index = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {
                const row = `
                    <tr>
                        <td>${i}</td>
                        <td>${item.TenDanhMuc}</td>
                        <td>${item.MoTa ?? ''}</td>
                        <td>${item.CategoryName ?? ''}</td>
                        <td>
                            <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateDanhMuc('${item.DanhMucId}')">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.DanhMucId}')">
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
function SaveDanhMuc() {
    var form = $('#form-addDanhmuc')[0];
    var formData = new FormData(form);
    var id = $('#DanhMucId').val();
    formData.delete("KichHoat");
    formData.append("KichHoat", $('#KichHoat').is(':checked'));

    var url = (id != null && parseInt(id) > 0) ? '/ProductCategory/Update' : '/ProductCategory/Add';

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalDanhMuc').modal('hide');
                
                loadDanhMuc(1); 
            } else if (typeof res === 'string') {
                $('#modalDanhMuc .modal-body').html(res);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/ProductCategory/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    toastr.success(res.msg || "Xoá thành công");
                    loadDanhMuc(1);
                } else {
                    toastr.error(res.msg || "Xoá thất bại");
                }
            }
        });
    }
}
