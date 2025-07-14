document.addEventListener("DOMContentLoaded", function () {
    var searchInput = document.getElementById("search-options"); // Ô tìm kiếm
    var menuItems = document.querySelectorAll("#navbar-nav .nav-item, #navbar-nav .menu-title, #navbar-nav .nav-item.dropdown"); // Các mục trong menu

    searchInput.addEventListener("input", function () {
        var keyword = searchInput.value.trim().toLowerCase(); // Lấy từ khóa nhập vào và chuyển thành chữ thường

        menuItems.forEach(function (item) {
            var text = item.textContent.trim().toLowerCase(); // Lấy nội dung của từng mục
            if (text.includes(keyword)) {
                item.style.display = ""; // Hiển thị nếu có chứa từ khóa
            } else {
                item.style.display = "none"; // Ẩn nếu không chứa từ khóa
            }
        });

        // Hiển thị lại tiêu đề menu nếu có ít nhất một mục bên dưới nó hiển thị
        document.querySelectorAll("#navbar-nav .menu-title").forEach(function (title) {
            var nextElement = title.nextElementSibling;
            var hasVisibleItem = false;

            while (nextElement && !nextElement.classList.contains("menu-title")) {
                if (nextElement.style.display !== "none") {
                    hasVisibleItem = true;
                    break;
                }
                nextElement = nextElement.nextElementSibling;
            }

            title.style.display = hasVisibleItem ? "" : "none";
        });
    });
});
