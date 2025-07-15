
function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Case/Delete',
            type: 'POST',
            data: { ID: id },
            success: function (res) {
                if (res.code === 200) {
                    alert(res.msg);
                    location.reload();
                } else {
                    alert(res.msg);
                }
            }
        });
    }
}
function handleFormUpdateTienTrinh(processId, lastCompletedStep) {
    if (processId && lastCompletedStep >= 0) {
        sessionStorage.setItem("currentProcessId", processId.toString());
        sessionStorage.setItem("currentStep", (lastCompletedStep + 1).toString()); 
        sessionStorage.setItem("lastCompletedStep", lastCompletedStep.toString());

        window.location.href = "/Home/Index?processId=" + processId;
    }
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

function formatDate(dateStr) {
    if (!dateStr) return '';

    const match = /\/Date\((\d+)(?:[+-]\d+)?\)\//.exec(dateStr);
    if (!match) return 'Invalid Date';

    const timestamp = parseInt(match[1]);
    const d = dayjs(timestamp); 

    if (!d.isValid()) return 'Invalid Date';

    return d.format('DD/MM/YYYY HH:mm:ss');
}

function loadData(page = 1) {
    $("#loadingOverlay").show();
    const month = $('#month').val();
    const year = $('#year').val();
    const buildingId = $('#selectToaNha').val();

    $.ajax({
        url: '/Case/GetProcesses',
        type: 'GET',
        data: {
            Month: month,
            Year: year,
            BuildingId: buildingId,
            page: page,
            pageSize: 10
        },
        success: function (res) {
            const tbody = $('#tientrinh-body');
            tbody.empty();

            let index = (res.currentPage - 1) * res.pageSize + 1;

            res.items.forEach(item => {
                const row = `
                    <tr id="trow_${item.ProcessId}" onclick="loadTienTrinh(${item.ProcessId})" style="cursor:pointer;">
                        <td><input type="checkbox" /></td>
                        <td>${index++}</td>
                        <td>${item.ProcessName || ''}</td>
                        <td>${item.LastCompletedStep || ''}</td>
                        <td>${formatDate(item.CreatedDate)}</td>
                        <td>${formatDate(item.LastUpdatedDate)}</td>
                        <td>${item.BuildingName || ''}</td>
                        <td>${getStatusText(item.Status)}</td>
                        <td>
                            <button type="button" class="btn btn-outline-primary btn-sm"
                                    onclick="handleFormUpdateTienTrinh(${item.ProcessId}, ${item.LastCompletedStep || 0}); event.stopPropagation();">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button type="button" class="btn btn-outline-danger btn-sm"
                                    onclick="handleDelete(${item.ProcessId}); event.stopPropagation();">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                        </td>
                    </tr>
                `;
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

function getStatusText(status) {
    switch (status) {
        case 0: return "Chờ xử lý";
        case 1: return "Đang xử lý";
        case 2: return "Đã hoàn thành";
        default: return "Không xác định";
    }
}

$(document).ready(function () {
    loadData(1);
    renderPagination();
    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        loadData(1);
    });
    $("#loadingOverlay").hide();
});