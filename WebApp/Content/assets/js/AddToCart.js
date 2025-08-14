<script>
    $(document).ready(function () {
        $('.btn-add-to-cart').on('click', function () {
            var productId = '@Model.Product.Id';
            var quantity = $('.quantity-selector input').val();

            $.ajax({
                url: '/AddToCart',
                type: 'POST',
                data: {
                    productId: productId,
                    quantity: quantity
                },
                success: function (res) {
                    if (res.success) {
                        alert('Sản phẩm đã được thêm vào giỏ hàng');
                        // Có thể cập nhật số lượng hiển thị giỏ hàng ở header
                    } else {
                        alert(res.message || 'Thêm giỏ hàng thất bại');
                    }
                },
                error: function () {
                    alert('Có lỗi xảy ra');
                }
            });
        });
    });
</script>
