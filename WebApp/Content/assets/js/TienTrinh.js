document.addEventListener('DOMContentLoaded', function () {
    loadAllDropdowns();
});

function closeModal() {
    $('#modalTienTrinh').modal('hide');
}
function openModal() {
    $.ajax({
        url: '/Case/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalTienTrinh .modal-body').html(res);
            $('#modalTienTrinh').modal('show');
        }
    })
}
function handleFormUpdateBaiViet(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/Case/Edit',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalTienTrinh .modal-body').html(res);
            $('#modalTienTrinh').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}

function loadTienTrinh() {
    $.ajax({
        url: '/Case/Index',
        type: 'GET',
        success: function (data) {

            $('#ds-tientrinh').html(data);
        }
    });
}


function SaveTienTrinh() {
    debugger
    var form = $('#form-addTienTrinh')[0];
    var formData = new FormData(form);
    var id = $('#Id').val();

    var url = (id != null && parseInt(id) > 0) ? '/Case/Update' : '/Case/Add';
    console.log([...formData.entries()]);

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                alert(res.msg);
                $('#modalTienTrinh').modal('hide');
                location.reload();
                //loadDanhMuc(); 
            } else if (typeof res === 'string') {
                $('#modalTienTrinh .modal-body').html(res);
            } else {
                alert(res.msg);
            }
        }
    });
}
function renderDropdown(selectSelector, data, defaultOptionText = "") {
    const selectElement = document.querySelector(selectSelector);
    if (!selectElement) {
        console.error(`Không tìm thấy phần tử select với selector: ${selectSelector}`);
        return;
    }

    selectElement.innerHTML = '';

    const defaultOption = document.createElement('option');
    defaultOption.value = "";
    defaultOption.textContent = defaultOptionText;
    selectElement.appendChild(defaultOption);

    if (data && Array.isArray(data)) {
        data.forEach(item => {
            const option = document.createElement('option');
            option.value = item.id || item.Id;
            option.textContent = item.name || item.Name;
            selectElement.appendChild(option);
        });
    }
}

function loadAllDropdowns() {
    $.get('/Case/GetDropdownData', function (res) {
        if (!res.success) {
            alert("Không thể tải dropdown");
            return;
        }
        renderDropdown('#month', res.months);
        if (res.currentMonth) {
            document.querySelector('#month').value = res.currentMonth.toString();
        }
        renderDropdown('#year', res.year);
        if (res.currentYear) {
            document.querySelector('#year').value = res.currentYear.toString();
        }
        //renderDropdown('#selectToaNha', res.toaNha, 'Toà nhà');
    }).fail(function () {
        alert("Lỗi khi tải dropdown từ server");
    });
}
function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Case/DeleteAccount',
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
function toggleStatus(id) {
    debugger

    $.ajax({
        url: '/Case/ToggleStatus',
        type: 'POST',
        data: { Id: id },
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
        }
    });
}

