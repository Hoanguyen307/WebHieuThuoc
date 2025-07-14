function closeModal() {
    $('#modalProduct').modal('hide');
}
function LoadForm() {
    $.ajax({
        url: '/Products/Add',
        type: 'Get',
        success: function (res) {
            if ($('#formAdd').length > 0) {
                $('#formAdd')[0].reset();
            }
            $('#Id').val('');
            $('#modalProduct .modal-body').html(res);
            $('#modalProduct').modal('show');
        }
    })
}
function handleFormUpdateProduct(id) {
    debugger
    if (!id || id <= 0) {
        alert('ID không hợp lệ!');
        return;
    }

    $.ajax({
        url: '/Products/Edit',
        type: 'GET',
        data: { Id: id },
        success: function (res) {
            $('#modalProduct .modal-body').html(res);
            $('#modalProduct').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi load form:', error);
            alert('Có lỗi xảy ra khi tải form. Vui lòng thử lại!');
        }
    });
}

function loadProduct() {
    $.ajax({
        url: '/Products/GetDanhSachDanhMuc',
        type: 'GET',
        success: function (data) {

            $('#ds-danhmuc').html(data);
        }
    });
}

function SaveProduct() {
    debugger
    var form = $('#form-addProduct')[0];
    var formData = new FormData(form);
    var id = $('#Id').val();
    formData.delete("IsActive");
    formData.delete("IsFeatured");
    formData.append("IsActive", $('#IsActive').is(':checked'));
    formData.append("IsFeatured", $('#IsFeatured').is(':checked'));
    /*var name = ($('#Name').val() || '').trim();
    var slug = ($('#Slug').val() || '').trim();
    var description = ($('#Description').val() || '').trim();
    var image = ($('#Image').val() || '').trim();
    var displayOrder = parseInt($('#DisplayOrder').val()) || 0;
    var isActive = $('#IsActive').is(':checked');*/

    var url = (id != null && parseInt(id) > 0) ? '/Products/Update' : '/Products/Add';
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
                $('#modalProduct').modal('hide');
                location.reload();
                //loadDanhMuc();
            } else if (typeof res === 'string') {
                $('#modalProduct .modal-body').html(res);
            } else {
                alert(res.msg);
            }
        }
    });
}

function handleDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa tài khoản này không?')) {
        $.ajax({
            url: '/Products/DeleteAccount',
            type: 'POST',
            data: { ID: id },
            success: function (res) {
                if (res.code === 200) {
                    alert(res.msg);
                    location.reload();
                    //loadProduct();
                } else {
                    alert(res.msg);
                }
            }
        });
    }
}
