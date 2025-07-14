function closeModal() {
    $('#modalDanhMuc').modal('hide');
}
function LoadForm() {
    $.ajax({
        url: '/Category/Add',     
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalDanhMuc .modal-body').html(res);
            $('#modalDanhMuc').modal('show');
        }
    })
}
function handleFormUpdateDanhMuc(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/Category/Edit',
        type: 'GET',
        data: { ID: id },
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

function loadDanhMuc() {
    $.ajax({
        url: '/Category/GetDanhSachDanhMuc',
        type: 'GET',
        success: function (data) {

            $('#ds-danhmuc').html(data);
        }
    });
}

function SaveDanhMuc() {
    debugger
    var form = $('#form-addDanhmuc')[0];
    var formData = new FormData(form);
    var id = $('#ID').val();
    formData.delete("IsActive");
    formData.append("IsActive", $('#IsActive').is(':checked'));
    /*var name = ($('#Name').val() || '').trim();
    var slug = ($('#Slug').val() || '').trim();
    var description = ($('#Description').val() || '').trim();
    var image = ($('#Image').val() || '').trim();
    var displayOrder = parseInt($('#DisplayOrder').val()) || 0;
    var isActive = $('#IsActive').is(':checked');*/

    var url = (id != null && parseInt(id) > 0) ? '/Category/Update' : '/Category/Add';
    console.log([...formData.entries()]);
    /*var data = {
        ID: id,
        Name: name,
        Slug: slug,
        Description: description,
        Image: image,
        DisplayOrder: displayOrder,
        IsActive: isActive
    };*/

    $.ajax({
        url: url,
        type: 'POST',
        data: formData,
        processData: false,   
        contentType: false,
        success: function (res) {
            if (res.code === 200) {
                alert(res.msg);
                $('#modalDanhMuc').modal('hide');
                location.reload();
                //loadDanhMuc(); 
            } else if (typeof res === 'string') {
                $('#modalDanhMuc .modal-body').html(res);
            } else {
                alert(res.msg);
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Category/DeleteAccount',
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
