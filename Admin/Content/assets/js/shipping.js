
const STATUS_TEXT = {
    0: "Chờ xử lý",
    1: "Đã bàn giao cho đơn vị vận chuyển",
    2: "Đang lấy hàng",
    3: "Đang giao hàng",
    4: "Giao hàng thành công",
    5: "Giao thất bại"
};


let _list = [];
let _page = 1, _pageSize = 10;
let _autoTimer = null;
let _warehouses = [];
let _map, _routeLayer;

function showLoading(show) {
    $("#loadingOverlay").css("display", show ? "flex" : "none");
}
function badge(status) {
    const colors = { 0: "secondary", 1: "info", 2: "warning", 3: "primary", 4: "success", 5: "danger" };
    return `<span class="badge bg-${colors[status] || "secondary"} status-badge">${status} - ${STATUS_TEXT[status] || ""}</span>`;
}
function fmtDate(dt) {
    if (!dt) return "";
    try {
        if (typeof dt === "string" && dt.indexOf("/Date(") === 0) {
            const m = /\/Date\((\d+)\)\//.exec(dt);
            if (m) return dayjs(+m[1]).format("DD/MM/YYYY HH:mm:ss");
        }
        return dayjs(dt).format("DD/MM/YYYY HH:mm:ss");
    } catch { return ""; }
}
function paginate(total, current) {
    if (total <= 1) { $("#pagination").html(""); return; }
    let html = `<ul class="pagination pagination-sm justify-content-end">`;
    const li = (label, page, disabled = false, active = false) =>
        `<li class="page-item ${disabled ? "disabled" : ""} ${active ? "active" : ""}">
       <a class="page-link" href="#" data-page="${page}">${label}</a>
     </li>`;
    html += li("&laquo;", 1, current === 1);
    html += li("&lsaquo;", current - 1, current === 1);
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    for (let i = start; i <= end; i++) html += li(i, i, false, i === current);
    html += li("&rsaquo;", current + 1, current === total);
    html += li("&raquo;", total, current === total);
    html += `</ul>`;
    $("#pagination").html(html).off("click").on("click", "a.page-link", e => {
        e.preventDefault();
        const p = parseInt($(e.currentTarget).data("page"));
        if (!isNaN(p) && p !== _page) loadList(p);
    });
}

function loadList(page = 1) {
    _page = page;
    showLoading(true);

    $.getJSON("/Shipping/GetShippingOrders", {
        keyword: $("#searchKeyword").val() || null,
        status: $("#filter-status").val() || null,
        deliveryServiceId: $("#filter-ds").val() || null,
        driverId: $("#filter-driver").val() || null,
        warehouseId: $("#filter-warehouse").val() || null,
        fromDate: $("#filter-from").val() || null,
        toDate: $("#filter-to").val() || null
    }, res => {
        showLoading(false);
        if (res.code !== 200) {
            toastr.error(res.msg || "Tải danh sách thất bại");
            return;
        }

        let items = res.data || [];
        _list = items;

        const totalPages = Math.max(1, Math.ceil(items.length / _pageSize));
        const slice = items.slice((_page - 1) * _pageSize, _page * _pageSize);

        renderTable(slice);
        paginate(totalPages, _page);

    }).fail(() => {
        showLoading(false);
        toastr.error("Không thể tải dữ liệu");
    });
}


function renderTable(rows) {
    const tb = $("#shipping-body");
    tb.empty();
    let i = (_page - 1) * _pageSize + 1;
    rows.forEach(r => {
        const tr = `
      <tr
          data-id="${r.Id}" 
          data-orderid="${r.OrderId}" 
          data-ds="${r.DeliveryServiceId || ''}" 
          data-driver="${r.DriverId || ''}" 
          data-wh="${r.WarehouseId || ''}">
        <td>${i++}</td>
        <td class="fw-semibold">${r.OrderCode || ""}</td>
        <td class="text-muted">${r.TrackingCode || ""}</td>
        <td>${r.DeliveryServiceName || "<span class='text-secondary'>-</span>"}</td>
        <td>${r.DriverName || "<span class='text-secondary'>-</span>"}</td>
        <td>${r.WarehouseName || "<span class='text-secondary'>-</span>"}</td>
        <td>${badge(r.CurrentStatus || 0)}</td>
        <td>${fmtDate(r.CreatedDate)}</td>
        <td >
            <div class="btn-group btn-group-sm">
                <button class="btn btn-outline-primary btn-assign">Phân công</button>
                <button class="btn btn-outline-secondary btn-history">Hành trình</button>
                <button class="btn btn-outline-success btn-map">Bản đồ</button>
            </div>
        </td>
      </tr>
    `;
        tb.append(tr);
    });

    // row actions
    tb.off("click", ".btn-assign").on("click", ".btn-assign", onOpenAssign);
    tb.off("click", ".btn-history").on("click", ".btn-history", onOpenTimeline);
    tb.off("click", ".btn-map").on("click", ".btn-map", onOpenMap);
}

function fillAssignDS(selectedId) {
    $("#assign-ds").empty().append(`<option value="">-- Chọn DVVC --</option>`);

    // Load DS
    $.getJSON("/Shipping/GetDeliveryServices", res => {
        if (res.code === 200) {
            res.data.forEach(x => {
                $("#assign-ds").append(`<option value="${x.Id}">${x.Name}</option>`);
            });
            if (selectedId) $("#assign-ds").val(selectedId);
        }
    });
}

function onOpenAssign(e) {
    const $tr = $(e.currentTarget).closest("tr");

    const shippingId = +$tr.data("id");
    const dsId = $tr.data("ds");
    const driverId = $tr.data("driver");
    const whId = $tr.data("wh");

    console.log("--- Bắt đầu onOpenAssign ---");
    console.log("Thông tin dòng chọn:", { shippingId, dsId, driverId, whId });

    $("#assign-shippingId").val(shippingId);

    fillAssignDS(dsId);

    if (dsId) {
        console.log("Đang gọi lấy tài xế cho dsId:", dsId);
        $.getJSON("/Shipping/GetDrivers", { deliveryServiceId: dsId }, res => {
            console.log("Kết quả GetDrivers từ Server:", res); 

            if (res.code === 200) {
                $("#assign-driver").html(`<option value="">-- Chọn tài xế --</option>`);

                if (res.data && res.data.length > 0) {
                    res.data.forEach(d => {
                        console.log("Dữ liệu tài xế chi tiết:", d); 
                        $("#assign-driver").append(`<option value="${d.Id}">${d.DriverName} - ${d.Phone}</option>`);
                    });

                    if (driverId) {
                        console.log("Thực hiện gán driverId hiện tại vào dropdown:", driverId);
                        $("#assign-driver").val(driverId);
                    }
                } else {
                    console.warn("Mảng res.data rỗng - Không có tài xế cho dịch vụ này.");
                }
            } else {
                console.error("Lỗi từ Server khi lấy tài xế:", res.msg);
            }
        }).fail((jqXHR, textStatus, errorThrown) => {
            console.error("Lỗi kết nối API GetDrivers:", textStatus, errorThrown);
        });
    }

    if (whId) $("#assign-warehouse").val(whId);

    const oc = new bootstrap.Offcanvas("#offAssign");
    oc.show();
}

$("#assign-ds").on("change", function () {
    const dsId = $(this).val();
    console.log("--- Thay đổi DVVC trên Offcanvas ---");
    console.log("dsId mới chọn:", dsId);

    $("#assign-driver").html(`<option value="">-- Chọn tài xế --</option>`);
    if (!dsId) return;

    $.getJSON("/Shipping/GetDrivers", { deliveryServiceId: dsId }, res => {
        console.log("Kết quả nạp lại tài xế:", res);
        if (res.code === 200) {
            (res.data || []).forEach(d => {
                $("#assign-driver").append(`<option value="${d.Id}">${d.DriverName} - ${d.Phone}</option>`);
            });
        }
    });
});

$("#btnAssignSave").on("click", function () {
    const shippingOrderId = +$("#assign-shippingId").val();
    const dsId = $("#assign-ds").val() || null;
    const driverId = $("#assign-driver").val() || null;
    const whId = $("#assign-warehouse").val() || null;

    if (!dsId) { toastr.warning("Vui lòng chọn Đơn vị vận chuyển"); return; }
    if (!driverId) { toastr.warning("Vui lòng chọn Tài xế"); return; }
    if (!whId) { toastr.warning("Vui lòng chọn Kho xuất"); return; }

    showLoading(true);
    $.post("/Shipping/Assign", { shippingOrderId, deliveryServiceId: dsId, driverId, warehouseId: whId }, res => {
        showLoading(false);
        if (res.code === 200) {
            toastr.success("Đã bàn giao cho DVVC (Assigned)");
            loadList(_page);
            bootstrap.Offcanvas.getInstance(document.getElementById("offAssign")).hide();
        } else {
            toastr.error(res.msg || "Không thể cập nhật");
        }
    }).fail(() => { showLoading(false); toastr.error("Lỗi mạng"); });
});

// ====== Timeline ======
function onOpenTimeline(e) {
    const $tr = $(e.currentTarget).closest("tr");
    const shippingId = +$tr.data("id");
    const tracking = $tr.find("td:nth-child(3)").text().trim();

    $("#tl-tracking").text(tracking || "");
    $("#tl-body").html(`<div class="text-muted">Đang tải...</div>`);
    const modal = new bootstrap.Modal("#modalTimeline"); modal.show();

    $.getJSON("/Shipping/GetHistory", { shippingOrderId: shippingId }, res => {
        if (res.code !== 200) { $("#tl-body").html(`<div class="text-danger">Không tải được lịch sử</div>`); return; }
        const items = res.data || [];
        if (items.length === 0) { $("#tl-body").html(`<div class="text-muted">Chưa có lịch sử</div>`); return; }

        let html = "";
        items.forEach(h => {
            html += `
<div class="timeline-item">
    <div class="dot"></div>

    <div class="timeline-title">${STATUS_TEXT[h.Status] || h.Status}</div>

    <div class="timeline-time">
        ${fmtDate(h.CreatedDate)}
        ${h.Location ? " • " + h.Location : ""}
    </div>

    ${h.Note ? `<div class="timeline-note">${h.Note}</div>` : ""}
</div>`;

        });
        $("#tl-body").html(html);
        $("#btnAutoDemo").data("sid", shippingId);
    });
}

// Auto demo: 2 -> 3 -> 4
$("#btnAutoDemo").on("click", function () {
    const sid = $(this).data("sid");
    if (!sid) return;
    if (_autoTimer) { clearInterval(_autoTimer); _autoTimer = null; $(this).text("Auto Demo (2→3→4)"); return; }

    const seq = [2, 3, 4, 5]; let idx = 0;
    $(this).text("Đang chạy... (nhấn để dừng)");
    _autoTimer = setInterval(() => {
        const st = seq[idx++];
        $.post("/Shipping/UpdateStatus", { shippingOrderId: sid, status: st, location: null, note: `Auto set to ${STATUS_TEXT[st]}` }, () => {
            // reload timeline nhanh
            $.getJSON("/Shipping/GetHistory", { shippingOrderId: sid }, res => {
                if (res.code === 200) {
                    const items = res.data || [];
                    let html = "";
                    items.forEach(h => {
                        html += `<div class="timeline-item">
              <div class="fw-semibold">${STATUS_TEXT[h.Status] || h.Status}</div>
              <div class="small text-muted">${fmtDate(h.CreatedDate)} ${h.Location ? ("• " + h.Location) : ""}</div>
              ${h.Note ? (`<div>${h.Note}</div>`) : ""}
            </div>`;
                    });
                    $("#tl-body").html(html);
                }
            });
            loadList(_page);
        });
        if (idx >= seq.length) { clearInterval(_autoTimer); _autoTimer = null; $("#btnAutoDemo").text("Auto Demo (2→3→4)"); }
    }, 4000); // mỗi 4s
});

// ====== Map ======
function ensureMap() {
    if (_map) return;
    _map = L.map('map').setView([21.0285, 105.8048], 5);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(_map);
}
function drawRoute(warehouses) {
    if (!warehouses || warehouses.length === 0) return;
    if (_routeLayer) _map.removeLayer(_routeLayer);

    const points = warehouses
        .filter(w => w.Latitude && w.Longitude)
        .map(w => [w.Latitude, w.Longitude]);

    points.forEach((p, idx) => {
        const w = warehouses[idx];
        L.marker(p).addTo(_map).bindPopup(`<b>${w.Name}</b><br/>${w.Address || ""}`);
    });
    _routeLayer = L.polyline(points, { weight: 4 }).addTo(_map);
    _map.fitBounds(_routeLayer.getBounds(), { padding: [20, 20] });
}

function onOpenMap(e) {
    const modal = new bootstrap.Modal("#modalMap"); modal.show();
    setTimeout(() => {
        ensureMap();
        // Lấy danh sách kho từ server nếu chưa có
        if (_warehouses.length === 0) {
            $.getJSON("/Shipping/GetWarehouses", res => {
                if (res.code === 200) { _warehouses = res.data || []; drawRoute(_warehouses); }
            });
        } else {
            drawRoute(_warehouses);
        }
    }, 300);
}

// ====== Events: filters / reload ======
$("#btnReload").on("click", () => loadList(_page));
$("#filter-ds, #filter-status").on("change", () => loadList(1));
$("#filter-ds").on("change", function () {
    let id = $(this).val() || 0;

    $("#filter-driver").html(`<option value="">-- Tất cả --</option>`);

    if (id !== "") {
        $.get("/Shipping/GetDrivers", { deliveryServiceId: id }, function (res) {
            if (res.code === 200) {
                res.data.forEach(d => {
                    $("#filter-driver").append(
                        `<option value="${d.Id}">${d.DriverName}</option>`
                    );
                });
            }
        });
    }

    loadList();
});

// ====== Init ======
$(document).ready(function () {
    loadList(1);
});
