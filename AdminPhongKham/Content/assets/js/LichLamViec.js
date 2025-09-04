function formatDate(dateStr, includeTime = false) {
    if (!dateStr) return '';

    const match = /\/Date\((\d+)\)\//.exec(dateStr);
    const timestamp = match ? parseInt(match[1], 10) : null;

    const d = timestamp ? dayjs(timestamp) : dayjs(dateStr);
    if (!d.isValid()) return 'Invalid Date';

    const weekdays = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'][d.day()];

    return includeTime
        ? ` ${d.format('DD/MM/YYYY HH:mm:ss')}`
        : `${d.format('DD/MM/YYYY')}`;
}

function lichLamViec(page = 1) {
    const fromDate = $('#FromDate').val();
    const toDate = $('#ToDate').val();
    const calam = $('#ShiftId').val() ? parseInt($('#ShiftId').val()) : null;
    const chucvu = $('#PositionId').val() ? parseInt($('#PositionId').val()) : null;
    $("#loadingOverlay").show();
    const start = Date.now();
    const minDelay = 500;

    $.ajax({
        url: '/NhanVien/LichLamViec',
        type: 'GET',
        data: {
            FromDate: fromDate,
            ToDate: toDate,
            Page: page,
            ShiftId: calam,
            PositionId: chucvu,
            PageSize: 20,
            Draw: 1,
        },
        success: function (res) {
            const elapsed = Date.now() - start;
            const remaining = Math.max(0, minDelay - elapsed);
            setTimeout(() => {
                if (res.result === "success") {
                    renderTable(res.data);
                } else {
                    toastr.error("Không tải được dữ liệu: " + res.message);
                }
                $("#loadingOverlay").hide();
            }, remaining);
        },
        error: function (err) {
            console.error(err);
            toastr.warning("Lỗi khi gọi API dữ liệu");
            $("#loadingOverlay").hide();
        }
    });
}
function generateWeekDays(formatDate) {
    const from = dayjs(formatDate, 'YYYY-MM-DD');
    if (!from.isValid()) return [];

    const days = [];
    for (let i = 0; i < 7; i++) {
        const d = from.add(i, 'day');
        days.push({
            date: d.format('YYYY-MM-DD'),
            label: `${['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'][d.day()]}`
        });
    }
    return days;
}

function renderTable(data) {
    console.log(data);
    const tbody = $('#lichlamviec-body');
    const thead = $('#lichlamviec-header');
    tbody.empty();
    thead.empty();

    const fromDate = $('#FromDate').val() || dayjs().startOf('week').add(1, 'day').format('YYYY-MM-DD');
    const toDate = $('#ToDate').val() || dayjs().endOf('week').add(1, 'day').format('YYYY-MM-DD');

    const weekDays = generateWeekDays(fromDate);
    if (weekDays.length === 0) {
        tbody.append('<tr><td colspan="7" class="text-center text-muted">Ngày bắt đầu không hợp lệ</td></tr>');
        return;
    }

    const headerRow = $('<tr></tr>');
    weekDays.forEach(day => {
        headerRow.append(`<th>${day.label}</th>`);
    });
    thead.append(headerRow);

    const row = $('<tr></tr>');
    weekDays.forEach(day => {
        const col = $(`<td data-date="${day.date}"><div class="lichlamviec-dropzone"></div></td>`);
        const itemsInDay = data.filter(x => {
            const match = /\/Date\((\d+)\)\//.exec(x.ThoiGian);
            const timestamp = match ? parseInt(match[1], 10) : null;
            const itemDate = dayjs(timestamp).format('YYYY-MM-DD');
            return itemDate === day.date;
        });

        const container = col.find('.lichlamviec-dropzone');
        if (itemsInDay.length > 0) {
            itemsInDay.forEach(item => {
                const status = parseInt(item.ShiftId);
                let bgColor = '#f3e5f5';
                switch (status) {
                    case 1: bgColor = '#fff3cd'; break; // vàng
                    case 2: bgColor = '#d1ecf1'; break; // xanh nhạt
                    case 3: bgColor = '#d4edda'; break; // xanh lá
                }
                const html = `
                    <div class="lichlamviec-item" draggable="true" data-id="${item.Id}" data-time="${item.ThoiGian}" style="background:${bgColor}; border-radius:8px; padding:6px; margin-bottom:5px;">
                        <b>${item.FullName}</b><br>
                        <span>${formatDate(item.ThoiGian)}</span><br>
                        <span class="ca-lam clickable" data-id="${item.Id}" data-shiftid="${item.ShiftId}">
                            <em>Ca làm:</em> <span class="ca-lam-label">${item.ShiftName || 'Chưa phân công'}</span>
                            <select class="ca-lam-select d-none" style=" margin-top:4px;">
                                ${caLamOptions}
                            </select>
                        </span><br>
                        <span class="nguoi-xu-ly clickable" data-id="${item.Id}" data-nhanvienid="${item.PositionId}">
                            <em>Chức vụ:</em> <span class="nguoi-xu-ly-label">${item.PositionName || 'Chưa phân công'}</span>
                            <select class="nguoi-xu-ly-select d-none" style=" margin-top:4px;"></select>
                        </span>
                    </div>
                `;
                container.append(html);
            });
        } else {
            col.append(`<span class="text-muted"></span>`);
        }
        row.append(col);
    });

    tbody.append(row);

    document.querySelectorAll('.lichlamviec-dropzone').forEach(zone => {
        Sortable.create(zone, {
            group: 'lichlamviec',
            animation: 150,
            onAdd: function (evt) {
                const itemEl = evt.item;
                const oldParent = evt.from;
                const oldIndex = evt.oldIndex;
                const newDate = evt.to.closest('td').dataset.date;
                const id = $(itemEl).data('id');

                updateThoiGian(id, newDate, function success() {
                }, function fail() {
                    oldParent.insertBefore(itemEl, oldParent.children[oldIndex]);
                });
            }
        });
    });
}

function updateThoiGian(id, newDate, onSuccess, onFailure) {
    $("#loadingOverlay").show();
    const draggedItem = $(`.lichlamviec-item[data-id="${id}"]`);
    const oldDateTime = draggedItem.data('time');
    const match = /\/Date\((\d+)\)\//.exec(oldDateTime);
    const oldTimestamp = match ? parseInt(match[1], 10) : null;
    const oldTime = dayjs(oldTimestamp);
    const timePart = oldTime.format('HH:mm:ss');

    const combinedDateTime = dayjs(`${newDate} ${timePart}`).format('YYYY-MM-DDTHH:mm:ss');

    $.ajax({
        url: '/NhanVien/UpdateTime',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Id: id,
            NewThoiGian: combinedDateTime,
            UpdatedBy: 'admin',
            UpdatedDate: dayjs().format()
        }),
        success: function (res) {

            switch (res.code) {
                case 200:
                    toastr.success(res.msg || "Cập nhật thành công");
                    if (onSuccess) onSuccess();
                    lichLamViec();
                    break;
                case 500:
                    toastr.error(res.msg || "Có lỗi xảy ra");
                    if (onFailure) onFailure();
                    break;
                default:
                    toastr.error("Lỗi không xác định");
                    if (onFailure) onFailure();
                    break;
            }
        },
        error: function (err) {
            toastr.error('Lỗi cập nhật thời gian');
            if (onFailure) onFailure();
        },
        complete: function () {
            $("#loadingOverlay").hide();
        }
    });
}

$(document).ready(function () {
    
    $('#filterForm').on('submit', function (e) {
        e.preventDefault();

        lichLamViec();
    });

    $('#searchBtn').on('click', function (e) {
        e.preventDefault();
        lichLamViec(1);
    });
});

$(document).on('click', '.ca-lam', function () {
    const span = $(this);
    const select = span.find('.ca-lam-select');
    const label = span.find('.ca-lam-label');

    label.hide();
    select.removeClass('d-none').focus();

});
$(document).on('change', '.ca-lam-select', function () {
    const select = $(this);
    const span = select.closest('.ca-lam');
    const label = span.find('.ca-lam-label');
    const id = span.data('id');
    const selectedId = parseInt(select.val()) || null;

    $.ajax({
        url: '/NhanVien/UpdateCaLam',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Id: id,
            ShiftId: selectedId,
            assignedBy: 'admin'
        }),
        success: function (res) {
            if (res.code === 200) {
                const selectedText = select.find('option:selected').text();
                label.text(selectedText);
                span.data('shiftid', selectedId);
                toastr.success("Cập nhật ca làm thành công");
                lichLamViec();
            } else {
                toastr.error(res.message || "Cập nhật thất bại");
            }
        },
        error: function () {
            toastr.error("Lỗi cập nhật ca làm");
        },
        complete: function () {
            label.show();
            select.addClass('d-none');
        }
    });
});
$(document).on('click', function (e) {
    const $target = $(e.target);
    if (!$target.closest('.ca-lam').length) {
        $('.ca-lam-select').each(function () {
            const select = $(this);
            const span = select.closest('.ca-lam');
            const label = span.find('.ca-lam-label');

            if (!select.hasClass('d-none')) {
                select.addClass('d-none');
                label.show();
            }
        });
    }
});

