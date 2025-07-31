$(document).ready(function () {
    const ctx = document.getElementById('doanhThuChart').getContext('2d');
    let chart = null;

    function loadChart(fromDate, toDate) {
        $.ajax({
            url: '/DoanhThu/GetBaoCaoDoanhThu',
            type: 'GET',
            data: { fromDate, toDate },
            success: function (res) {
                if (res.success && res.data.length > 0) {
                    const data = res.data;
                    const labels = data.map(x => x.NgayStr);
                    const values = data.map(x => x.DoanhThu);

                    if (chart) chart.destroy(); // Xoá biểu đồ cũ nếu có

                    chart = new Chart(ctx, {
                        type: 'bar',
                        data: {
                            labels: labels,
                            datasets: [{
                                label: 'Doanh thu',
                                data: values,
                                backgroundColor: 'rgba(75, 192, 192, 0.6)'
                            }]
                        },
                        options: {
                            responsive: true,
                            scales: {
                                y: {
                                    beginAtZero: true
                                }
                            }
                        }
                    });
                } else {
                    if (chart) chart.destroy(); // Nếu không có dữ liệu, xoá biểu đồ
                    chart = null;
                    alert("Không có dữ liệu trong khoảng thời gian này.");
                }
            }
        });
    }

    // ❌ Không gọi loadChart() ban đầu —> chỉ gọi khi submit:
    $('#filterForm').on('submit', function (e) {
        e.preventDefault();
        const fromDate = $('#FromDate').val();
        const toDate = $('#ToDate').val();
        loadChart(fromDate, toDate);
    });
});
