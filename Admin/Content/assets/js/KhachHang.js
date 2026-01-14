function closeModal() {
    $('#modalKhachHang').modal('hide');
    $('#modalPointHistory').modal('hide');
}
function LoadForm() {
    $("#loadingOverlay").show();
    $.ajax({
        url: rootPath + 'KhachHang/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalKhachHang .modal-body').html(res);
            $('#modalKhachHang').modal('show');
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    })
}
function handleFormUpdateKhachHang(id) {
    $("#loadingOverlay").show();
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: rootPath + 'KhachHang/Edit',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalKhachHang .modal-body').html(res);
            $('#modalKhachHang').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}
function formatDate(dateStr, includeTime = false) {
    if (!dateStr) return '';

    const match = /\/Date\((\d+)(?:[+-]\d+)?\)\//.exec(dateStr);
    if (!match) return 'Invalid Date';

    const timestamp = parseInt(match[1]);
    const d = dayjs(timestamp);

    if (!d.isValid()) return 'Invalid Date';

    return includeTime ? d.format('DD/MM/YYYY HH:mm:ss') : d.format('DD/MM/YYYY');
}
function loadKhachHang(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const name = $('#searchString').val();

    $.ajax({
        url: rootPath + 'KhachHang/GetKhachHang',
        type: 'GET',
        data: {
            Month: month,
            Year: year,
            searchString: name,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#khachhang-body');
            tbody.empty();

            let index = (res.currentPage - 1) * res.pageSize + 1;
            let i = 1;

            res.items.forEach(item => {
                const row = `
                    <tr id="trow_${item.Id}" onclick="" style="cursor:pointer;">
            <td></td>
            <td>${i}</td>
            <td>${item.FullName}</td>
            <td>${item.Gender ? 'Nam' : 'Nữ'}</td>
            <td>${formatDate(item.BirthDate, false)}</td>
            <td>${item.Address}</td>
            <td>${item.Phone}</td>
            <td>${item.Email}</td>
            <td>${item.LoyaltyPoint }</td>
            <td id="status_${item.Id}" style="color:${item.IsActive ? 'green' : 'red'}">
                ${item.IsActive ? 'Đang hoạt động' : 'Đã khóa'}
            </td>
            <td>
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="xemLichSuDiem('${item.Id}')"><i class="fas fa-history"></i></button>
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="handleFormUpdateKhachHang('${item.Id}')"><i class="fas fa-edit"></i></button>
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}')"><i class="fas fa-trash-alt"></i></button>
                <button type="button" class="btn btn-outline-warning btn-sm" onclick="toggleStatus('${item.Id}')">
                    ${item.IsActive ? 'Khóa' : 'Mở khóa'}
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

function SaveKhachHang() {
    debugger
    var btn = $('#btnLuu');
    btn.attr('disabled', true);
    btn.find('.text').addClass('d-none');
    btn.find('.spinner-border').removeClass('d-none');

    var id = $('#Id').val();
    var fullName = $('#FullName').val().trim();
    if (fullName === '') {
        alert('Vui lòng nhập họ tên!');
        return;
    }
    var url = (id != null && parseInt(id) > 0) ? rootPath + 'KhachHang/Update' : rootPath + 'KhachHang/Add';

    $.ajax({
        url: url,
        type: 'POST',
        data: $('#form-addKhachHang').serialize(),
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                $('#modalKhachHang').modal('hide');
                loadKhachHang(1);
            } else {
                toastr.error(res.msg || "Cập nhật thất bại");
            }
        },
        error: function (xhr) {
            console.error(xhr.responseText);
            alert('Lỗi: ' + xhr.responseText);
        },
        complete: function () {
            btn.removeAttr('disabled');
            btn.find('.text').removeClass('d-none');
            btn.find('.spinner-border').addClass('d-none');
        }
    });
}

function handleDelete(id) {
    $("#loadingOverlay").show();
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: rootPath + 'KhachHang/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    toastr.success(res.msg || "Xoá thành công");
                    loadKhachHang(1);
                } else {
                    toastr.error(res.msg || "Xoá thất bại");
                }
            },
            complete: function () {
                $("#loadingOverlay").hide();
            }
        });
    }
}
function toggleStatus(id) {
    $("#loadingOverlay").show();
    var lyDo = prompt("Nhập lý do khóa/mở tài khoản:");
    if (lyDo == null || lyDo.trim() === "") {
        alert("Bạn phải nhập lý do.");
        return;
    }

    $.ajax({
        url: rootPath + 'KhachHang/ToggleStatus',
        type: 'POST',
        data: { Id: id, lyDo: lyDo },  
        success: function (res) {
            if (res.code === 200) {
                toastr.success(res.msg || "Cập nhật thành công");
                loadKhachHang(1);
            } else {
                toastr.error(res.msg || "Cập nhật thành công");
            }
        },
        error: function () {
            toastr.warning(res.msg || "Lỗi gọi tới API");
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}
function xemLichSuDiem(customerId, page = 1) {
    $.ajax({
        url: rootPath + 'KhachHang/GetPointHistory',
        type: 'GET',
        data: {
            customerId: customerId,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            if (res.code === 200) {
                var rows = '';
                $.each(res.items, function (i, item) {
                    rows += `
                        <tr>
                            <td>${i + 1}</td>
                            <td>${dayjs(item.CreatedDate).format('DD/MM/YYYY HH:mm')}</td>
                            <td>${item.Points}</td>
                            <td>${item.Type}</td>
                            <td>${item.Description ?? ''}</td>
                        </tr>`;
                });
                const pageSize = 10;
                const totalCount = res.totalCount ?? res.items.length;
                const totalPages = Math.ceil(totalCount / pageSize);
                renderPointPagination(totalPages, page, customerId);

                $('#pointHistoryTable').html(rows);
                $('#modalPointHistory').modal('show');
            } else {
                $('#pointHistoryTable').html(`
                    <tr>
                        <td colspan="5" class="text-center text-muted">
                            ${res.msg}
                        </td>
                    </tr>
                `);
                $('#modalPointHistory').modal('show');
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi tải lịch sử điểm.");
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

            loadKhachHang(page);
            xemLichSuDiem(customerId, page);
        }
    });
}
function renderPointPagination(totalPages, currentPage, customerId) {
    if (totalPages === 0) {
        $('#pointPagination').html('');
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

    $('#pointPagination').html(html);

    $('#pointPagination').off('click').on('click', 'a.page-link', function (e) {
        e.preventDefault();
        const page = parseInt($(this).data("page"));
        if (!isNaN(page) && page !== currentPage) {
            xemLichSuDiem(customerId, page);
        }
    });
}


$(document).ready(function () {

    loadKhachHang(1);
    renderPagination();

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();

        loadKhachHang();
    });

    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        loadKhachHang(1);
    });
    $("#loadingOverlay").hide();
});