$(document).ready(function () {
    loadReview(1);
    renderPagination();

    $("#filterForm").submit(function (e) {
        e.preventDefault();
        loadReview(1);
    });

    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        loadReview(1);
    });
    $("#loadingOverlay").hide();
});

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
            loadReview(page);
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
function loadReview(page = 1) {
    var searchString = $("#searchString").val();
    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();
    var rating = $("#Rating").val();

    $.ajax({
        url: rootPath + 'ProductReview/GetReview',
        type: 'GET',
        data: {
            searchString: searchString,
            FromDate: fromDate,
            ToDate: toDate,
            Rating: rating,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#review-body');
            tbody.empty();
            
            let i = 1;

            res.items.forEach(item => {
                let stars = '';
                for (let j = 1; j <= 5; j++) {
                    if (j <= item.Rating) {
                        stars += `<i class="fas fa-star" style="color: gold;"></i>`;
                    } else {
                        stars += `<i class="far fa-star" style="color: gold;"></i>`;
                    }
                }
                const row = `
            <tr>
            <td>${i}</td>
            <td class="text-truncate" style="max-width: 250px;white-space: nowrap;" title="${item.ProductName}">${item.ProductName}</td>
            <td>${item.CustomerName}</td>
            <td>${stars}</td>
            <td class="text-truncate" style="max-width: 300px;white-space: nowrap;" title="${item.Comment ?? ''}">${item.Comment ?? ''}</td>
            <td>${formatDate(item.CreatedDate, true)}</td>
            <td>
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="handleDelete('${item.Id}'); event.stopPropagation();">
                    <i class="fas fa-trash-alt"></i>
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
        error: function () {
            alert("Có lỗi khi tải dữ liệu đánh giá");
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: rootPath + 'ProductReview/DeleteAccount',
            type: 'POST',
            data: { Id: id },
            success: function (res) {
                if (res.code === 200) {
                    toastr.success(res.msg || "Xoá thành công");
                    /*setTimeout(function () {
                        location.reload();
                    }, 1500);*/
                    loadReview();
                } else {
                    toastr.success(res.msg || "Xoá thất bại");
                }
            }
        });
    }
}