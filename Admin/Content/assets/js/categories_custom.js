/* JS Document */

/******************************

[Table of Contents]

1. Vars and Inits
2. Set Header
3. Init Menu
4. Init Favorite
5. Init Fix Product Border
6. Init Isotope Filtering
7. Init Price Slider
8. Init Checkboxes



******************************/

jQuery(document).ready(function ($) {
	"use strict";

	/* 

	1. Vars and Inits

	*/

	var header = $('.header');
	var topNav = $('.top_nav')
	var mainSlider = $('.main_slider');
	var hamburger = $('.hamburger_container');
	var menu = $('.hamburger_menu');
	var menuActive = false;
	var hamburgerClose = $('.hamburger_close');
	var fsOverlay = $('.fs_menu_overlay');

	setHeader();

	$(window).on('resize', function () {
		initFixProductBorder();
		setHeader();
	});

	$(document).on('scroll', function () {
		setHeader();
	});

	initMenu();
	initFavorite();
	initFixProductBorder();
	initIsotopeFiltering();
	initPriceSlider();
	initCheckboxes();

	/* 

	2. Set Header

	*/

	function setHeader() {
		if (window.innerWidth < 992) {
			if ($(window).scrollTop() > 100) {
				header.css({ 'top': "0" });
			}
			else {
				header.css({ 'top': "0" });
			}
		}
		else {
			if ($(window).scrollTop() > 100) {
				header.css({ 'top': "-50px" });
			}
			else {
				header.css({ 'top': "0" });
			}
		}
		if (window.innerWidth > 991 && menuActive) {
			closeMenu();
		}
	}

	/* 

	3. Init Menu

	*/

	function initMenu() {
		if (hamburger.length) {
			hamburger.on('click', function () {
				if (!menuActive) {
					openMenu();
				}
			});
		}

		if (fsOverlay.length) {
			fsOverlay.on('click', function () {
				if (menuActive) {
					closeMenu();
				}
			});
		}

		if (hamburgerClose.length) {
			hamburgerClose.on('click', function () {
				if (menuActive) {
					closeMenu();
				}
			});
		}

		if ($('.menu_item').length) {
			var items = document.getElementsByClassName('menu_item');
			var i;

			for (i = 0; i < items.length; i++) {
				if (items[i].classList.contains("has-children")) {
					items[i].onclick = function () {
						this.classList.toggle("active");
						var panel = this.children[1];
						if (panel.style.maxHeight) {
							panel.style.maxHeight = null;
						}
						else {
							panel.style.maxHeight = panel.scrollHeight + "px";
						}
					}
				}
			}
		}
	}

	function openMenu() {
		menu.addClass('active');
		// menu.css('right', "0");
		fsOverlay.css('pointer-events', "auto");
		menuActive = true;
	}

	function closeMenu() {
		menu.removeClass('active');
		fsOverlay.css('pointer-events', "none");
		menuActive = false;
	}

	/* 

	4. Init Favorite

	*/

	function initFavorite() {
		if ($('.favorite').length) {
			var favs = $('.favorite');

			favs.each(function () {
				var fav = $(this);
				var active = false;
				if (fav.hasClass('active')) {
					active = true;
				}

				fav.on('click', function () {
					var id = $(this).data('id');
					if (active) {
						fav.removeClass('active');
						active = false;
						//DeleteFavoutiesList(id);
					}
					else {
						fav.addClass('active');
						active = true;
						//AddFavoutiesList(id);
					}
				});
			});
		}
	}
	function DeleteFavoutiesList(id) {
		$.ajax({
			url: '/Wishlist/Delete',
			type: 'POST',
			data: { ProductId: id },
			success: function (res) {
				if (res.Success == false) {
					alert(res.Message);
				}
			}
		});
	}
	function AddFavoutiesList(id) {
		$.ajax({
			url: '/Wishlist/PostWishlist',
			type: 'POST',
			data: { ProductId: id },
			success: function (res) {
				if (res.Success == false) {
					alert(res.Message);
				}
			}
		});
	}
	/* 

	5. Init Fix Product Border

	*/

	function initFixProductBorder() {
		if ($('.product_filter').length) {
			var products = $('.product_filter:visible');
			var wdth = window.innerWidth;

			// reset border
			products.each(function () {
				$(this).css('border-right', 'solid 1px #e9e9e9');
			});

			// if window width is 991px or less

			if (wdth < 480) {
				for (var i = 0; i < products.length; i++) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			else if (wdth < 576) {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 1; i < products.length; i += 2) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			else if (wdth < 768) {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 2; i < products.length; i += 3) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			else if (wdth < 992) {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 2; i < products.length; i += 3) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			//if window width is larger than 991px
			else {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 3; i < products.length; i += 4) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}
		}
	}

	/* 

	6. Init Isotope Filtering

	*/

	function initIsotopeFiltering() {
		var sortTypes = $('.type_sorting_btn');
		var sortNums = $('.num_sorting_btn');
		var sortTypesSelected = $('.sorting_type .item_sorting_btn is-checked span');
		var filterButton = $('.filter_button');

		if ($('.product-grid').length) {
			$('.product-grid').isotope({
				itemSelector: '.product-item',
				getSortData: {
					price: function (itemElement) {
						var priceEle = $(itemElement).find('.in_product_price').text();
						return parseFloat(priceEle);
					},
					name: '.product_name'
				},
				animationOptions: {
					duration: 750,
					easing: 'linear',
					queue: false
				}
			});

			// Short based on the value from the sorting_type dropdown
			sortTypes.each(function () {
				$(this).on('click', function () {
					$('.type_sorting_text').text($(this).text());
					var option = $(this).attr('data-isotope-option');
					option = JSON.parse(option);
					$('.product-grid').isotope(option);
				});
			});

			// Show only a selected number of items
			sortNums.each(function () {
				$(this).on('click', function () {
					var numSortingText = $(this).text();
					var numFilter = ':nth-child(-n+' + numSortingText + ')';
					$('.num_sorting_text').text($(this).text());
					$('.product-grid').isotope({ filter: numFilter });
				});
			});

			// Filter based on the price range slider
			filterButton.on('click', function () {
				$('.product-grid').isotope({
					filter: function () {
						var priceRange = $('#amount').val();
						var priceMin = parseFloat(priceRange.split('-')[0].replace('Đ', ''));
						var priceMax = parseFloat(priceRange.split('-')[1].replace('Đ', ''));
						var itemPrice = $(this).find('.in_product_price').clone().children().remove().end().text();

						return (itemPrice > priceMin) && (itemPrice < priceMax);
					},
					animationOptions: {
						duration: 750,
						easing: 'linear',
						queue: false
					}
				});
			});
		}
	}

	/* 

	7. Init Price Slider

	*/

	function initPriceSlider() {
		$("#slider-range").slider(
			{
				range: true,
				min: 0,
				max: 1000000,
				values: [0, 2000000],
				slide: function (event, ui) {
					$("#amount").val(ui.values[0] + "Đ - " + ui.values[1] + "Đ");
					$("#FromAmount").val(ui.values[0]);
					$("#ToAmount").val(ui.values[1]);

				}
			});

		$("#amount").val($("#slider-range").slider("values", 0) + "Đ - " + $("#slider-range").slider("values", 1) + " Đ");
	}

	/* 

	8. Init Checkboxes

	*/
	// Gọi phương thức CheckOut khi ấn nút đặt hàng
	$("#yourSubmitButtonId").on("click", function () {
		// Lấy dữ liệu từ form hoặc các trường khác tùy theo cách bạn thiết kế
		var formData = $("#yourFormId").serialize();

		// Gọi Ajax để gửi dữ liệu đặt hàng
		$.ajax({
			url: '/ShoppingCart/CheckOut',
			type: 'POST',
			data: formData,
			success: function (result) {
				if (result.Success) {
					// Đặt hàng thành công
					alert('Đặt hàng thành công!');
					// Thực hiện các bước tiếp theo nếu cần
				} else {
					// Đặt hàng không thành công
					alert('Đặt hàng không thành công. ' + result.Message);

					// Hiển thị thông báo trong div có id là "notification"
					$("#notification").text(result.Message).show();

					// Nếu mã lỗi là -1 (số lượng tồn kho không đủ), có thể thực hiện các xử lý khác tùy thuộc vào yêu cầu của bạn
					if (result.Code === -1) {
						// Xử lý khi số lượng tồn kho không đủ
					}

					// Các bước khác tùy thuộc vào yêu cầu của bạn
				}
			},
			error: function () {
				// Xử lý lỗi nếu có
				alert('Đã xảy ra lỗi khi gửi yêu cầu đặt hàng.');
			}
		});
	});


	function initCheckboxes() {
		if ($('.checkboxes li').length) {
			var boxes = $('.checkboxes li');

			boxes.each(function () {
				var box = $(this);

				box.on('click', function () {
					if (box.hasClass('active')) {
						box.find('i').removeClass('fa-square');
						box.find('i').addClass('fa-square-o');
						box.toggleClass('active');
					}
					else {
						box.find('i').removeClass('fa-square-o');
						box.find('i').addClass('fa-square');
						box.toggleClass('active');
					}
					// box.toggleClass('active');
				});
			});

			if ($('.show_more').length) {
				var checkboxes = $('.checkboxes');

				$('.show_more').on('click', function () {
					checkboxes.toggleClass('active');
				});
			}
		};
	}

	$(document).ready(function (id) {
		$(document).on('click', '.btnUpdate', function () {
			var productId = $(this).data('id');
			var quantity = $("#Quantity_" + productId).val();

			$.ajax({
				url: '/ShoppingCart/Update',
				type: 'POST',
				data: { id: productId, quantity: quantity },
				success: function (response) {
					if (response.Success) {
						alert(response.msg);
						var updatedQuantity = response.updatedQuantity;
						var totalPrice = response.totalPrice;

						$("#Quantity_" + productId).val(updatedQuantity);

						$("#TotalPrice_" + productId).text(totalPrice);
					} else {
						alert(response.msg);
					}
				},

				error: function (error) {
					console.log(error);
				}
			});
		});
	});


	/*$(document).ready(function (id) {
		$(document).on('click', '.btnUpdate', function () {
			var productId = $(this).data('id');
			console.log(productId);
			var quantity = $("#Quantity_" + productId).val();

			$.ajax({
				url: '/ShoppingCart/Update',
				type: 'POST',
				data: { id: productId, quantity: quantity },
				success: function (response) {
					console.log(response);
					if (response.Success) {
						alert(response.msg);

						var updatedQuantity = response.updatedQuantity;
						$("#Quantity_" + productId).val(updatedQuantity);
					} else {
						alert(response.msg);
					}
				},
				error: function (error) {
					console.log(error);
				}
			});
			$.ajax({
				url: "/Order/OutOfStock",
				type: "GET",
				success: function (response) {
				},
				error: function (xhr) {
				}
			});

		});
	});*/

});


