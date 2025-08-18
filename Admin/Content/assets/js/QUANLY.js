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

    const hamburger = document.getElementById("topnav-hamburger-icon");
    const body = document.body;

    hamburger.addEventListener("click", function () {
        // Toggle class "menu-collapsed" để đóng/mở sidebar
        body.classList.toggle("vertical-collapsed");

        // Nếu có localStorage thì lưu trạng thái
        const isCollapsed = body.classList.contains("vertical-collapsed");
        localStorage.setItem("sidebarCollapsed", isCollapsed);
    });

    // Khôi phục trạng thái từ localStorage (nếu muốn)
    const savedState = localStorage.getItem("sidebarCollapsed");
    if (savedState === "true") {
        document.body.classList.add("vertical-collapsed");
    }
});
});
document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("toggleUserMenu");
    const target = document.getElementById("menuUser");

    if (toggle && target) {
        const collapse = new bootstrap.Collapse(target, {
            toggle: false // không tự động bật khi khởi tạo
        });

        toggle.addEventListener("click", function () {
            if (target.classList.contains("show")) {
                collapse.hide();
                toggle.classList.add("collapsed");
                toggle.setAttribute("aria-expanded", "false");
            } else {
                collapse.show();
                toggle.classList.remove("collapsed");
                toggle.setAttribute("aria-expanded", "true");
            }
        });
    }
});



