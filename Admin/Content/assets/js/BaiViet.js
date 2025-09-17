function closeModal() {
    $('#modalBaiViet').modal('hide');
}
function LoadForm() {
    $.ajax({
        url: '/BaiViet/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalBaiViet .modal-body').html(res);
            $('#modalBaiViet').modal('show');
        }
    })
}
function handleFormUpdateBaiViet(id) {
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/BaiViet/Edit',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalBaiViet .modal-body').html(res);
            $('#modalBaiViet').modal('show');
        },
        error: function (xhr, status, error) {
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
            loadData(page);
        }
    });
}
function formatDate(dateStr, includeTime = false) {
    if (!dateStr) return '';

    // Parse chuỗi có định dạng yyyy/MM/dd
    const d = dayjs(dateStr, 'YYYY/MM/DD', true); // strict mode

    if (!d.isValid()) return 'Invalid Date';

    return includeTime ? d.format('DD/MM/YYYY HH:mm:ss') : d.format('DD/MM/YYYY');
}
function loadBaiViet(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const tieude = $('#searchString').val();
    $.ajax({
        url: '/BaiViet/GetBaiViet',
        type: 'GET',
        data: {
            Month: month,
            Year: year,
            searchString: tieude,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#baiviet-body');
            tbody.empty(); 
            let index = (res.currentPage - 1) * res.pageSize + 1;
            let i = 1;

            res.items.forEach(item => {
                const row = `
                <tr>
            <td>${i}</td>
            <td>${item.TieuDe}</td>
            <td>${item.NoiDung}</td>
            <td id="status_${item.Id}" style="color: ${item.TrangThai ? 'green' : 'red'};">${item.TrangThai ? 'Hiển thị' : 'Ẩn'}</td>
            <td>
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateBaiViet('${item.Id}'); event.stopPropagation();">
                    <i class="fas fa-edit"></i>
                </button>
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}'); event.stopPropagation();">
                    <i class="fas fa-trash-alt"></i>
                </button>
                <button type="button" class="btn btn-outline-warning btn-sm" onclick="toggleStatus('${item.Id}')">${item.TrangThai ? 'Ẩn' : 'Hiển thị'}
                </button>
            </td>
        </tr>`;
                i++;
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
$(document).ready(function () {

    loadBaiViet(1);
    renderPagination();

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();

        loadBaiViet();
    });

    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        loadBaiViet(1);
    });
    $("#loadingOverlay").hide();
});

function SaveBaiViet() {
    var form = $('#form-addBaiViet')[0];
    var formData = new FormData(form);
    var id = $('#form-addBaiViet #Id').val();

    var url = (id != null && parseInt(id) > 0) ? '/BaiViet/Update' : '/BaiViet/Add';

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalBaiViet').modal('hide');
                //setTimeout(function () {
                //    location.reload(); 
                //}, 1500);
                loadBaiViet(1);
            } else if (typeof res === 'string') {
                $('#modalBaiViet .modal-body').html(res);
            } else {
                alert(res.msg);
            }
        }
    });
}
function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa bài viết này không?')) {
        $.ajax({
            url: '/BaiViet/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    toastr.success(res.msg || "Xoá thành công");
                    //location.reload();
                    loadBaiViet(1);
                } else {
                    toastr.error(res.msg || "Xoá thất bại");
                }
            }
        });
    }
}
function toggleStatus(id) {

    $.ajax({
        url: '/BaiViet/ToggleStatus',
        type: 'POST',
        data: { Id: id },
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật trạng thái thành công");
               // location.reload();
                loadBaiViet(1);
            } else {
                toastr.error(res.msg || "Cập nhật trạng thái thất bại");
            }
        },
        error: function () {
            toastr.warring(res.msg || "Có lỗi xảy ra");
        }
    });
}

