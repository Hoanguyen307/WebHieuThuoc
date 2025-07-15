
let currentProcessId = 0;       
let currentStep = 1;            // Bước hiện tại 
let lastCompletedStep = 0;      // Bước đã được hoàn thành
const totalSteps = 7;           // Tổng số bước 


function showProcessMessage(message, type = 'info') {
    if (typeof toastr !== 'undefined') {
        if (type === 'success') toastr.success(message);
        else if (type === 'error') toastr.error(message);
        else if (type === 'warning') toastr.warning(message);
        else toastr.info(message);
    } else {
        alert(message); // Fallback nếu Toastr không khả dụng
    }
}

function clearProcessMessage() {
    // Với Toastr, không cần làm gì ở đây vì nó tự ẩn
}

function updateStepTotalDisplay() {
    // Cập nhật hiển thị "Bước X / Y" nếu có phần tử HTML này
    const displayElement = $('#stepProgressDisplay'); // Thay bằng selector của bạn
    if (displayElement.length) {
        displayElement.text(`Bước ${currentStep} / ${totalSteps}`);
    }
}

function updateProcessStepsUI() {
    document.querySelectorAll('.step').forEach(stepElement => {
        const idNumber = parseInt(stepElement.dataset.stepNumber);

        stepElement.classList.remove('active', 'disabled', 'pending', 'done');

        const nextArrow = stepElement.nextElementSibling;
        if (nextArrow && nextArrow.classList.contains('arrow')) {
            nextArrow.style.color = '#ccc'; 
        }

        if (!currentProcessId || currentProcessId === 0) {
            if (idNumber === 1) {
                stepElement.classList.add('pending'); 
            } else {
                stepElement.classList.add('disabled'); 
            }
        } else {
            if (idNumber === currentStep) {
                stepElement.classList.add('active'); 
                if (nextArrow) {
                    nextArrow.style.color = '#dc3545'; 
                }
            } else if (idNumber <= lastCompletedStep) {
                stepElement.classList.add('done'); 
                if (nextArrow) {
                    nextArrow.style.color = 'blue'; 
                }
            }  else {
                stepElement.classList.add('pending'); 
            }
        }

        const btnNext = stepElement.querySelector('.btn-next-step');
        const btnBack = stepElement.querySelector('.btn-back-step');
        const btnDetail = stepElement.querySelector('.btn-detail');
        const btnCreateNew = stepElement.querySelector('#btnCreateNewProcess'); 

        if (btnNext) { btnNext.style.pointerEvents = 'none'; btnNext.style.opacity = '0.5'; }
        if (btnBack) { btnBack.style.pointerEvents = 'none'; btnBack.style.opacity = '0.5'; }
        if (btnDetail) { btnDetail.style.pointerEvents = 'none'; btnDetail.style.opacity = '0.5'; }
        if (btnCreateNew) { btnCreateNew.style.pointerEvents = 'none'; btnCreateNew.style.opacity = '0.5'; } 


        if (!currentProcessId || currentProcessId === 0) {
            if (idNumber === 1 && btnCreateNew) {
                btnCreateNew.style.pointerEvents = 'auto'; 
                btnCreateNew.style.opacity = '1';
            }
        } else {
            if (btnCreateNew) { 
                btnCreateNew.style.pointerEvents = 'none';
                btnCreateNew.style.opacity = '0.5';
            }

            if (idNumber <= currentStep && btnDetail) {
                btnDetail.style.pointerEvents = 'auto';
                btnDetail.style.opacity = '1';
            }

            if (idNumber === currentStep) {
                if (btnNext && currentStep < totalSteps) {
                    btnNext.style.pointerEvents = 'auto';
                    btnNext.style.opacity = '1';
                }
                if (btnBack && currentStep > 1) {
                    btnBack.style.pointerEvents = 'auto';
                    btnBack.style.opacity = '1';
                }
            }
        }
    });
    updateStepTotalDisplay();
}

function nextStep() {
    clearProcessMessage();

    if (!currentProcessId || currentProcessId === 0) {
        showProcessMessage("Vui lòng tạo hoặc chọn một tiến trình để tiếp tục.", 'info');
        return;
    }
    if (currentStep >= totalSteps) { 
        showProcessMessage("Đã đến bước cuối cùng của tiến trình.", 'info');
        return;
    }

    const nextStepNumber = currentStep + 1; 
    const completedStepNumber = currentStep;

    const confirmMessage = `Bạn có chắc chắn muốn hoàn thành Bước ${completedStepNumber} và chuyển sang Bước ${nextStepNumber} không?`;

    if (confirm(confirmMessage)) {
        $.ajax({
            url: '/Home/UpdateProcessStep',
            type: 'POST',
            data: {
                ProcessId: currentProcessId,  
                CurrentStep: nextStepNumber,
                LastCompletedStep: completedStepNumber
            },
            success: function (response) {
                if (response.success) {
                    lastCompletedStep = completedStepNumber;
                    currentStep = nextStepNumber;

                    sessionStorage.setItem('currentProcessId', currentProcessId.toString());
                    sessionStorage.setItem('currentStep', currentStep.toString());
                    sessionStorage.setItem('lastCompletedStep', lastCompletedStep.toString());

                    updateProcessStepsUI();
                } else {
                    showProcessMessage(response.message || "Cập nhật bước tiến trình thất bại.", 'error');
                }
            },
            error: function (xhr, status, error) {
                showProcessMessage("Lỗi kết nối hoặc server khi cập nhật bước.", 'error');
                console.error('AJAX Error updating step:', error, xhr.responseText, xhr);
            }
        });
    }
}

function backStep() {
    clearProcessMessage();

    if (!currentProcessId || currentProcessId === 0) {
        showProcessMessage("Không có tiến trình nào được chọn.", 'info');
        return;
    }
    if (currentStep <= 1) {
        showProcessMessage("Đã ở bước đầu tiên của tiến trình.", 'info');
        return;
    }

    const prevStepNumber = currentStep - 1;
    const newLastCompletedStep = prevStepNumber - 1;

    //const lastCompletedStep = parseInt(sessionStorage.getItem('lastCompletedStep')) || 0;

    if (confirm(`Bạn có muốn quay lại Bước ${prevStepNumber} không?`)) {
        $.ajax({
            url: '/Home/UpdateProcessStep', 
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                ProcessId: currentProcessId,
                CurrentStep: prevStepNumber,
                LastCompletedStep: newLastCompletedStep 
            }),
            success: function (response) {
                if (response.success) {
                    currentStep = prevStepNumber;
                    lastCompletedStep = newLastCompletedStep; 
                    sessionStorage.setItem('currentStep', currentStep.toString());
                    sessionStorage.setItem('lastCompletedStep', lastCompletedStep.toString());
                    updateProcessStepsUI();
                } else {
                    showProcessMessage(response.message || "Lỗi khi quay lại bước.", 'error');
                }
            },
            error: function (xhr, status, error) {
                showProcessMessage("Lỗi kết nối hoặc server khi quay lại bước.", 'error');
                console.error('AJAX Error backing step:', error, xhr.responseText, xhr);
            }
        });
    }
}

function loadProcessStateFromServer(processIdFromUrlOrSession) {
    if (processIdFromUrlOrSession && processIdFromUrlOrSession > 0) {
        $.ajax({
            url: '/Home/GetProcessState',
            type: 'GET',
            data: { processId: processIdFromUrlOrSession },
            success: function (response) {
                console.log("Response GetProcessState:", response);

                if (response && response.success) {
                    currentProcessId = response.processId || 0;
                    currentStep = response.currentStep || 1;
                    lastCompletedStep = response.lastCompletedStep || 0;

                    sessionStorage.setItem("currentProcessId", response.processId);
                    sessionStorage.setItem("currentStep", response.lastCompletedStep);
                    sessionStorage.setItem("lastCompletedStep", response.lastCompletedStep);
                    window.location.href = '/Home/Index?processId=' + response.processId;

                    sessionStorage.setItem('currentProcessId', currentProcessId.toString());
                    sessionStorage.setItem('currentStep', currentStep.toString());
                    sessionStorage.setItem('lastCompletedStep', lastCompletedStep.toString());

                    updateProcessStepsUI();
                } else {
                    showProcessMessage('Không thể tải trạng thái tiến trình: ' + response?.message, 'error');
                    console.warn("Lỗi hoặc không có processId:", response);

                    currentProcessId = 0;
                    currentStep = 1;
                    lastCompletedStep = 0;
                    sessionStorage.clear();
                    updateProcessStepsUI();
                }
            },

            error: function (xhr, status, error) {
                console.error('AJAX Error loading process state:', error, xhr.responseText, xhr);
                showProcessMessage('Lỗi kết nối khi tải trạng thái tiến trình.', 'error');
                currentProcessId = 0;
                currentStep = 1;
                lastCompletedStep = 0;
                sessionStorage.clear();
                updateProcessStepsUI();
            }
        });
    } else {
        currentProcessId = 0;
        currentStep = 1;
        lastCompletedStep = 0;
        sessionStorage.clear();
        updateProcessStepsUI();
    }
}

function openModal() {
    $.ajax({
        url: '/Home/Add',
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

function closeModal() {
    $('#modalTienTrinh').modal('hide');
}

function renderDropdown(selectSelector, data, defaultOptionText = "", valueKey = "Id", textKey = "Name") {
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
            option.value = item[valueKey];
            option.textContent = item[textKey];
            selectElement.appendChild(option);
        });
    }
}

function loadAllDropdowns() {
    $.ajax({
        url: '/Home/GetDropdownData',
        type: 'GET',
        success: function (res) {
            if (res.success) {
                renderDropdown('#month', res.months);
                if (res.currentMonth) {
                    document.querySelector('#month').value = res.currentMonth.toString();
                }
                renderDropdown('#year', res.year);
                if (res.currentYear) {
                    document.querySelector('#year').value = res.currentYear.toString();
                }
                renderDropdown('#selectToaNha', res.toaNha, 'Chọn toà nhà', 'BuildingId','BuildingName' );
            } else {
                showProcessMessage("Lỗi khi tải dữ liệu dropdown: " + res.message, 'error');
                console.error("Failed to load dropdown data:", res.message);
            }
        },
        error: function (xhr, status, error) {
            showProcessMessage("Lỗi kết nối khi tải dữ liệu dropdown.", 'error');
            console.error("AJAX error loading dropdown data:", error, xhr.responseText);
        }
    });
}

function SaveTienTrinh() {
    var formData = $('#form-addTienTrinh').serialize();
    $.ajax({
        url: '/Home/Add', 
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.code === 200) {
                showProcessMessage(response.msg, 'success');
                $('#form-addTienTrinh').modal('hide');

                currentProcessId = response.processId;
                currentStep = response.currentStep || 1; // Đảm bảo có giá trị mặc định
                lastCompletedStep = response.lastCompletedStep || 0; // Đảm bảo có giá trị mặc định

                // LƯU VÀO SESSION STORAGE
                sessionStorage.setItem('currentProcessId', currentProcessId.toString());
                sessionStorage.setItem('currentStep', currentStep.toString());
                sessionStorage.setItem('lastCompletedStep', lastCompletedStep.toString());

                window.location.href = '/Home/Index?processId=' + currentProcessId;
                //updateProcessStepsUI(); // Cập nhật giao diện
            } else {
                showProcessMessage(response.message, 'error');
            }
        },
        error: function (xhr, status, error) {
            showProcessMessage('Lỗi khi thêm tiến trình.', 'error');
            console.error('AJAX Error adding process:', error, xhr.responseText);
        }
    });
}

$(document).ready(function () {
    loadAllDropdowns();
    const urlParams = new URLSearchParams(window.location.search);
    const idFromUrl = urlParams.get('processId');

    const storedProcessId = sessionStorage.getItem('currentProcessId');
    const storedCurrentStep = sessionStorage.getItem('currentStep');
    const storedLastCompletedStep = sessionStorage.getItem('lastCompletedStep');

    if (storedProcessId && parseInt(storedProcessId) > 0) {
        currentProcessId = parseInt(storedProcessId);
        currentStep = parseInt(storedCurrentStep) || 1;
        lastCompletedStep = parseInt(storedLastCompletedStep) || 0;
        updateProcessStepsUI();
    } else if (idFromUrl && parseInt(idFromUrl) > 0) {
        loadProcessStateFromServer(parseInt(idFromUrl));
    } else {
        $.ajax({
            url: '/Home/GetLatestProcessState',
            type: 'GET',
            success: function (res) {
                if (res.success) {
                    currentProcessId = res.processId;
                    currentStep = res.currentStep || 1;
                    lastCompletedStep = res.lastCompletedStep || 0;

                    sessionStorage.setItem('currentProcessId', currentProcessId);
                    sessionStorage.setItem('currentStep', currentStep);
                    sessionStorage.setItem('lastCompletedStep', lastCompletedStep);

                    window.location.href = '/Home/Index?processId=' + currentProcessId;

                    //updateProcessStepsUI();
                } else {
                    showProcessMessage(res.message || "Không có tiến trình để hiển thị.", 'warning');
                    updateProcessStepsUI();
                }
            },
            error: function () {
                showProcessMessage("Lỗi kết nối khi lấy tiến trình mới nhất.", 'error');
                updateProcessStepsUI();
            }
        });
    }


    document.querySelectorAll('.btn-next-step').forEach(button => {
        button.addEventListener('click', function () {
            const stepNumberOfThisButton = parseInt(this.closest('.step').dataset.stepNumber);
            if (stepNumberOfThisButton === currentStep) {
                nextStep();
            } else if (!currentProcessId || currentProcessId === 0) {
                showProcessMessage("Vui lòng tạo hoặc chọn một tiến trình.", 'info');
            } else {
                showProcessMessage("Bạn chỉ có thể tiến bước từ bước hiện tại.", 'warning');
            }
        });
    });

    document.querySelectorAll('.btn-back-step').forEach(button => {
        button.addEventListener('click', function () {
            const stepNumberOfThisButton = parseInt(this.closest('.step').dataset.stepNumber);
            if (stepNumberOfThisButton === currentStep) {
                backStep();
            } else if (!currentProcessId || currentProcessId === 0) {
                showProcessMessage("Vui lòng tạo hoặc chọn một tiến trình.", 'info');
            } else {
                showProcessMessage("Bạn chỉ có thể lùi từ bước hiện tại.", 'warning');
            }
        });
    });

    document.querySelectorAll('.btn-detail').forEach(button => {
        button.addEventListener('click', function () {
            if (this.style.pointerEvents === 'none' || this.style.opacity === '0.5') {
                showProcessMessage('Bạn chưa được phép xem chi tiết bước này.', 'warning');
                return;
            }
            const stepNumber = parseInt(this.closest('.step').dataset.stepNumber);
        });
    });

    document.querySelector('#step1').addEventListener('click', function () {
        const stepNumber = parseInt(this.dataset.stepNumber);

        if (currentProcessId && currentProcessId > 0) {
            if (currentStep === 1) {
                if (confirm("Bạn có muốn bắt đầu tiến trình và chuyển sang Bước 2 không?")) {
                    const nextStepNumber = 2;
                    const completedStepNumber = 1;

                    $.ajax({
                        url: '/Home/UpdateProcessStep',
                        type: 'POST',
                        data: {
                            ProcessId: currentProcessId,
                            CurrentStep: nextStepNumber,
                            LastCompletedStep: completedStepNumber
                        },
                        success: function (response) {
                            if (response.success) {
                                currentStep = nextStepNumber;
                                lastCompletedStep = completedStepNumber;

                                sessionStorage.setItem('currentProcessId', currentProcessId.toString());
                                sessionStorage.setItem('currentStep', currentStep.toString());
                                sessionStorage.setItem('lastCompletedStep', lastCompletedStep.toString());

                                showProcessMessage("Tiến trình đã được bắt đầu. Chuyển sang bước 2.", 'success');
                                updateProcessStepsUI();
                            } else {
                                showProcessMessage(response.message || "Không thể cập nhật bước.", 'error');
                            }
                        },
                        error: function (xhr, status, error) {
                            showProcessMessage("Lỗi kết nối khi bắt đầu tiến trình.", 'error');
                            console.error('Bắt đầu tiến trình lỗi:', error, xhr.responseText);
                        }
                    });
                }
            } else {
                showProcessMessage("Tiến trình đã bắt đầu từ trước.", 'info');
            }
        } else {
            openModal();
        }
    });

    document.querySelectorAll('.btn-complete-process').forEach(button => {
        button.addEventListener('click', function () {
            if (!currentProcessId || currentProcessId === 0) {
                showProcessMessage("Không có tiến trình nào để hoàn tất.", 'warning');
                return;
            }

            if (currentStep < totalSteps) {
                showProcessMessage("Bạn chưa đến bước cuối cùng để hoàn tất.", 'warning');
                return;
            }

            if (confirm("Bạn có chắc chắn muốn hoàn tất tiến trình này không?")) {
                $.ajax({
                    url: '/Home/UpdateProcessStep',
                    type: 'POST',
                    data: {
                        ProcessId: currentProcessId,
                        CurrentStep: currentStep,              
                        LastCompletedStep: currentStep - 1     
                    },
                    success: function (response) {
                        if (response.success) {
                            showProcessMessage("Tiến trình đã được hoàn tất thành công!", 'success');

                            sessionStorage.clear();
                            window.location.href = '/Home/Index';
                        } else {
                            showProcessMessage(response.message || "Không thể hoàn tất tiến trình.", 'error');
                        }
                    },
                    error: function (xhr, status, error) {
                        showProcessMessage("Lỗi khi hoàn tất tiến trình.", 'error');
                        console.error('Lỗi kết thúc:', error, xhr.responseText);
                    }
                });

            }
        });
    });
});