function closeModal() {
    $('#modalKhachHang').modal('hide');
}
function LoadForm() {
    $("#loadingOverlay").show();
    $.ajax({
        url: '/KhachHang/Add',
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
        url: '/KhachHang/Edit',
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

function loadKhachHang() {
    /*$("#loadingOverlay").show();*/
    $.ajax({
        url: '/KhachHang/Index',
        type: 'GET',
        success: function (data) {

            $('#ds-khachhang').html(data);
            const pageSize = 10;
            const totalCount = res.result.totalCount || 0;
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
    var url = (id != null && parseInt(id) > 0) ? '/KhachHang/Update' : '/KhachHang/Add';

    $.ajax({
        url: url,
        type: 'POST',
        data: $('#form-addKhachHang').serialize(),
        success: function (res) {
            if (res.code === 200) {
                alert(res.msg);
                $('#modalKhachHang').modal('hide');
                location.reload();
            } else {
                alert(res.msg);
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
            url: '/KhachHang/DeleteAccount',
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
            },
            complete: function () {
                $("#loadingOverlay").hide();
            }
        });
    }
}
function toggleStatus(id) {
    $("#loadingOverlay").show();
    debugger
    var lyDo = prompt("Nhập lý do khóa/mở tài khoản:");
    if (lyDo == null || lyDo.trim() === "") {
        alert("Bạn phải nhập lý do.");
        return;
    }

    $.ajax({
        url: '/KhachHang/ToggleStatus',
        type: 'POST',
        data: { Id: id, lyDo: lyDo },  
        success: function (res) {
            if (res.code === 200) {
                alert(res.msg);
                location.reload(); 
            } else {
                alert(res.msg);
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi gọi API.");
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}
function renderPagination(totalPages, currentPage) {
    if (totalPages <= 1) {
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

