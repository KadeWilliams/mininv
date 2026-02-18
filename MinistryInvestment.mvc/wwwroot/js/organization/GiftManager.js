import { api } from '../services/api.js';
import flatpickr from "flatpickr";
import monthSelectPlugin from "flatpickr/dist/plugins/monthSelect";
import showToast from '../services/toast.js';
import { formStateManager } from '../services/FormStateManager.js';

export class GiftManager {
    constructor(containerSelector, requestId, organizationId, requestManager) {
        this.container = $(containerSelector);
        this.requestId = requestId;
        this.organizationId = organizationId;
        this.requestManager = requestManager;
        this.currentGiftId = null;

        this.setupEventHandlers();
        this.initialize();
    }

    setupEventHandlers() {
        this.container.on('click', '#cancelGiftScheduleBtn', () => {
            $("#addEditGiftScheduleModal").modal("hide");
        });

        this.container.on('click', '.back-to-request', (e) => {
            if (!formStateManager.confirmNavigation(true)) {
                return;
            }
            e.preventDefault();
            this.backToRequest();
        });

        this.container.on('click', '#SaveGiftBtn', (e) => {
            e.preventDefault();
            $('#GiftDetail_SaveGift').val(true);
            this.saveGift();
        });

        this.container.on('click', '#addGiftSchedule', (e) => {
            e.preventDefault();
            this.showGiftScheduleModal($(e.currentTarget).data());
        })

        this.container.on('click', '.edit-schedule', (e) => {
            e.preventDefault();
            this.showGiftScheduleModal($(e.currentTarget).data());
        })

        this.container.on('click', '.delete-schedule', (e) => {
            e.preventDefault();
            $("#GiftDetail_DeleteGiftSchedule").val(true);
            this.deleteSchedule($(e.currentTarget).data('scheduleid'));
        })

        this.container.on('click', '#saveGiftScheduleRebalanceBtn', (e) => { // normal save
            this.saveSchedule(true);
        });

        this.container.on('click', '#saveGiftScheduleBtn', (e) => { // normal save
            this.saveSchedule(false);
        });

        this.container.on('click', '#GenerateGiftScheduleBtn', (e) => { // normal save
            $("#GenerateModal").modal('show');
        });

        this.container.on('click', '#generateSchedulesBtn', (e) => { // normal save
            this.generateSchedules();
        });

        this.container.on('change', "#GiftDetail_Gift_Conditional", () => {
            this.toggleConditional();
        })

        this.container.on('change', "#GiftDetail_Gift_Recurring", () => {
            this.toggleRecurring();
        })
    }

    initialize() {
        this.currentGiftId = $('#GiftDetail_Gift_GiftID').val();
        this.initializeGiftForm();
        setTimeout(() => {
            formStateManager.captureFormState('gift');
        }, 100)
    }

    initializeGiftForm() {
        // Initialize date pickers for gift
        if ($('#GiftDetail_Gift_GiftStartDate').length) {
            flatpickr("#GiftDetail_Gift_GiftStartDate", {
                defaultDate: new Date(),
                allowInput: true,
                plugins: [
                    monthSelectPlugin({
                        dateFormat: "m/Y",
                    })
                ],
            });
        }

        if ($('#GiftDetail_Gift_ConditionDeadline').length) {
            flatpickr('#GiftDetail_Gift_ConditionDeadline', {
                allowInput: true,
                plugins: [
                    monthSelectPlugin({ dateFormat: 'm/Y' })
                ],
            });
        }

        // Initialize gift schedules table
        if ($('#GiftSchedulesTable').length) {
            new DataTable("#GiftSchedulesTable", {
                dom: "tir",
                paging: false,
                columnDefs: [
                    { targets: 0, searchable: false, orderable: false },
                    { targets: 4, visible: false },
                ],
                order: [[1, 'asc']],
            });
        }

        this.updateProgressBar();
    }


    showGiftScheduleModal(data) {
        if (!data['voteDate']) {
            //populate modal with data
            $("#GiftDetail_GiftSchedule_GiftScheduleID").val(data.giftScheduleId);

            let a = data.disbursementDate.split(" ")[0].split("/");
            let [m, d, y] = a;
            let date = `${m}-0${d}-${y}`;
            flatpickr("#GiftDetail_GiftSchedule_DisbursementDate", {
                defaultDate: new Date(date),
                allowInput: true,
                plugins: [
                    monthSelectPlugin({
                        dateFormat: "m/Y",
                    })
                ],
            });
            $("#GiftDetail_GiftSchedule_Amount").val(data.amount);
            $("#GiftDetail_GiftSchedule_PreviousAmount").val(data.amount);

            let bool = data.includePaymentInfo == "False" ? false : true;
            $("#GiftDetail_GiftSchedule_IncludePaymentInfo").prop("checked", bool);
            $("#GiftDetail_GiftSchedule_IncludePaymentInfo").val(bool);
        } else {
            let minDate = data.voteDate.split(" ")[0].split("/");
            let [mm, dd, yy] = minDate;
            minDate = `${mm}-0${dd}-${yy}`;
            flatpickr("#GiftDetail_GiftSchedule_DisbursementDate", {
                defaultDate: new Date(),
                allowInput: true,
                minDate: new Date(minDate),
                plugins: [
                    monthSelectPlugin({
                        dateFormat: "m/Y",
                    })
                ],
            });
            $("#GiftDetail_GiftSchedule_GiftScheduleID").val(0);
            $("#addEditGiftScheduleModal input").val("");
            if (!$("#GiftDetail_Gift_Recurring").prop("checked"))
                $("#GiftDetail_GiftSchedule_Amount").val($("#GiftDetail_Gift_RemainingGiftAmount").val());
        }
        $("#addEditGiftScheduleModal").modal("show");
    }

    toggleConditional() {
        const isConditional = $('#GiftDetail_Gift_Conditional').prop('checked')
        if (isConditional) {
            $('#conditionInformation').removeClass('d-none');
        } else {
            $('#conditionInformation').addClass('d-none');
        }
    }

    toggleRecurring() {
        const isRecurring = $('#GiftDetail_Gift_Recurring').prop('checked');
        if (isRecurring && $(".giftScheduleRecord").length < 1) {
            $('#GenerateGiftScheduleBtn').removeClass('d-none');
        } else {
            $('#GenerateGiftScheduleBtn').addClass('d-none');
        }
    }

    getGiftFormData() {
        return {
            giftId: $('#GiftDetail_Gift_GiftID').val(),
            requestId: this.requestId,
            organization: this.organizationId,
            giftAmount: $('#GiftDetail_Gift_GiftAmount').val(),
            conditional: $('#GiftDetail_Gift_Conditional').prop('checked'),
            recurring: $('#GiftDetail_Gift_Recurring').prop('checked'),
            conditionDescription: $('#GiftDetail_Gift_ConditionDescription').val(),
            conditionDeadline: $('#GiftDetail_Gift_ConditionDeadline').val(),
            conditionCompleted: $('#GiftDetail_Gift_ConditionCompleted').prop('checked'),
            paymentMemo: $('#GiftDetail_Gift_PaymentMemo').val(),
            giftNote: $('#GiftDetail_Gift_GiftNote').val(),
        }
    }

    validate() {
        const giftAmount = $('#GiftDetail_Gift_GiftAmount').val();
        if (!giftAmount || parseFloat(giftAmount) <= 0) {
            alert('Gift amount must be greater than 0');
            return false;
        }
        return true;
    }

    updateProgressBar() {
        const progressContainer = $("#progress-bar-container");
        const giftTotalAmount = progressContainer.data('giftAmount');
        const remainingGiftAmount = progressContainer.data('remainingGiftAmount');

        const remainingPercentage = 1 - Number(remainingGiftAmount / giftTotalAmount) < .01 ? 0 : 1 - Number(remainingGiftAmount / giftTotalAmount);
        let progressBar = new ProgressBar.Line("#progress-bar-container", {
            strokeWidth: 6,
            easing: 'bounce',
            duration: 1400,
            color: '#FFEA82',
            trailColor: '#eee',
            trailWidth: 1,
            svgStyle: { width: '100%', height: '100%' },
            text: {
                style: {
                    color: '#000',
                    position: 'absolute',
                    right: '16px',
                    top: '16px',
                    padding: 0,
                    margin: 0,
                    transform: null
                },
                autoStyleContainer: false
            },
            from: { color: '#1d8621' },
            to: { color: '#ff763a' },
            step: (state, bar) => {
                //bar.setText("Testing where this goes...");
                bar.setText(Math.round(remainingPercentage * 100) + '% of funds allocated');
                bar.path.setAttribute('stroke', state.color);
            }
        })

        progressBar.animate(remainingPercentage);

    }

    backToRequest() {
        //formStateManager.clearAllStates();
        formStateManager.clearSpecificState('gift');
        this.requestManager.loadRequest(this.requestId);
    }

    async reload() {
        const url = `/Organization/LoadGiftPartial?giftID=${this.currentGiftId}&requestID=${this.requestId}?organizationID=${this.organizationId}`;
        const html = await fetch(url).then(r => r.text());
        this.container.html(html);
        this.initialize();
    }

    destroy() {
        this.container.off();
    }

    async saveGift() {
        if (!this.validateGift()) {
            return;
        }
        this.submitGiftForm('gift');
        formStateManager.resetFormState('gift');
    }

    async saveSchedule(isRebalance = false) {
        if (!this.validateSchedule(isRebalance)) {
            return;
        }
        await this.submitGiftForm('giftSchedule', isRebalance);
        $("#addEditGiftScheduleModal").modal("hide");
    }

    async generateSchedules() {
        if (!this.validateGenerate()) {
            return;
        }
        await this.submitGiftForm('generate');
        $('#GenerateModal').modal('hide');
    }

    async deleteSchedule(scheduleId) {
        if (!confirm('Delete this schedule?')) {
            return;
        }
        $('#GiftDetail_DeleteGiftSchedule').val(true);

        try {
            await api.deleteGiftSchedule(this.currentGiftId, scheduleId);
            await this.reload();
        } catch (e) {
            console.error('Failed to delete schedule: ', e);
            alert('Failed to delete schedule');
        }
    }

    validateGift() {
        const giftAmount = this.getGiftAmount();

        if (!giftAmount || giftAmount <= 0) {
            this.showFieldError(
                '#GiftDetail_Gift_GiftAmount',
                'Must be greater than 0'
            );
            return false;
        }
        return true;
    }

    validateSchedule(isRebalance = false) {
        this.clearValidationErrors();
        let isValid = true;

        const scheduleAmount = this.getScheduleAmount();
        if (!scheduleAmount || scheduleAmount <= 0) {
            this.showFieldError(
                '#GiftDetail_GiftSchedule_Amount',
                'Must be greater than 0'
            );
            isValid = false;
        }

        const disbursementDate = $('#GiftDetail_GiftSchedule_DisbursementDate').val();
        if (!disbursementDate || disbursementDate.length === 0) {
            this.showFieldError(
                '#GiftDetail_GiftSchedule_DisbursementDate',
                'Disbursement Date is required'
            );
            isValid = false;
        }

        if (scheduleAmount > 0 && !this.isScheduleAmountValid(scheduleAmount, isRebalance)) {
            isValid = false;
        }
        return isValid;
    }

    isScheduleAmountValid(scheduleAmount, isRebalance) {
        const remainingGiftAmount = this.getRemainingGiftAmount();
        const totalGiftAmount = this.getGiftAmount();
        const scheduleId = this.getCurrentScheduleId();
        const isNewSchedule = scheduleId == 0;

        if (isNewSchedule) {
            if (scheduleAmount > remainingGiftAmount) {
                this.showFieldError(
                    '#GiftDetail_GiftSchedule_Amount',
                    `Unable to save. Gift Schedule Amount exceeds the remaining Gift Amount of $${remainingGiftAmount.toFixed(2)}`
                );
                return false;
            }
            return true;
        }

        if (isRebalance) {
            if (scheduleAmount > remainingGiftAmount && scheduleAmount > totalGiftAmount) {
                this.showFieldError(
                    '#GiftDetail_GiftSchedule_Amount',
                    `Unable to save. Gift Schedule Amount exceeds the remaining Gift Amount of $${remainingGiftAmount.toFixed(2)}`
                );
                return false;
            }
            return true;
        } else {
            const previousAmount = this.getPreviousScheduleAmount();
            const amountChange = scheduleAmount - previousAmount;

            if (amountChange > remainingGiftAmount) {
                this.showFieldError(
                    '#GiftDetail_GiftSchedule_Amount',
                    `Unable to save. Gift Schedule Amount exceeds the remaining Gift Amount of $${remainingGiftAmount.toFixed(2)}`
                );
                return false;
            }
            return true;
        }

    }

    validateGenerate() {
        this.clearValidationErrors();

        const frequencyType = $('#GiftDetail_Gift_FrequencyType').val();
        const selectedFrequery = parseInt($('#GiftDetail_Gift_SelectedFrequency').val());

        if (frequencyType == 0 && selectedFrequery > 24) {
            this.showFieldError(
                '#GiftDetail_Gift_SelectedFrequency',
                'Selected Frequency cannot be more than 24 months'
            );
            return false;
        }

        if (frequencyType == 1 && selectedFrequery > 8) {
            this.showFieldError(
                '#GiftDetail_Gift_SelectedFrequency',
                'Selected Frequency cannot be more than 8 quarters'
            );
            return false;
        }

        return true;
    }


    async submitGiftForm(saveType, isRebalance = false) {
        try {
            $('#GiftDetail_SaveGiftSchedule').val(false);
            $('#GiftDetail_GenerateSchedules').val(false);
            $('#GiftDetail_Gift_Rebalance').val(false);

            $('#OrganizationDetails_RequestID').val(this.requestId);
            $('#OrganizationDetails_GiftID').val(this.currentGiftId);
            $('#GiftDetail_Gift_RequestID').val(this.requestId);

            this.setCheckboxValues();

            switch (saveType) {
                case 'giftSchedule':
                    $('#GiftDetail_SaveGiftSchedule').val(true);
                    $('#GiftDetail_GiftSchedule_IncludePaymentInfo').val(
                        $('#GiftDetail_GiftSchedule_IncludePaymentInfo').prop('checked')
                    );
                    if (isRebalance) {
                        $('#GiftDetail_Gift_Rebalance').val(true);
                    }
                    break;
                case 'generate':
                    $('#GiftDetail_GenerateSchedules').val(true);
                    break;
                case 'gift':
                    break;
            }

            $('#GiftDetail_SaveGift').val(true);

            const form = $("#giftDetailForm");
            if (!form.length) {
                console.error('Request form not found');
                return;
            }


            validationService.validateForm(form[0], async () => {
                // Submit
                try {
                    const formData = new FormData(form[0]);
                    const response = await fetch('/Organization/SaveGiftForm', {
                        method: 'POST',
                        body: formData,
                    });

                    const result = await response.json();
                    this.currentGiftId = result.giftID;

                    // Reload the request to show updated data
                    showToast('Your changes have been saved!')
                    await this.reload();
                    //alert('Gift saved successfully');
                    //api.save(true);
                } catch (error) {
                    console.error('Failed to save gift:', error);
                    //alert('Failed to save gift');
                    showToast('Failed to save gift', 'error')
                }
            });
        } catch (e) {
            console.error('Failed to save gift:', e);
            alert('Failed to save gift');
        }
    }

    setCheckboxValues() {
        $('#GiftDetail_Gift_Conditional').val(
            $('#GiftDetail_Gift_Conditional').prop('checked')
        );
        $('#GiftDetail_Gift_ConditionalCompleted').val(
            $('#GiftDetail_Gift_ConditionalCompleted').prop('checked')
        );
        $('#GiftDetail_Gift_Recurring').val(
            $('#GiftDetail_Gift_Recurring').prop('checked')
        );
    }

    getGiftAmount() {
        return parseFloat($('#GiftDetail_Gift_GiftAmount').val()) || 0
    }

    getRemainingGiftAmount() {
        return parseFloat($('#GiftDetail_Gift_RemainingGiftAmount').val()) || 0
    }

    getScheduleAmount() {
        return parseFloat($('#GiftDetail_GiftSchedule_Amount').val()) || 0
    }

    getPreviousScheduleAmount() {
        return parseFloat($('#GiftDetail_GiftSchedule_PreviousAmount').val()) || 0
    }

    getCurrentScheduleId() {
        return parseFloat($('#GiftDetail_GiftSchedule_GiftScheduleID').val()) || 0
    }

    showFieldError(fieldSelector, message) {
        const field = $(fieldSelector);

        field.addClass('input-validation-error');

        field.siblings('.field-validate-valid')
            .removeClass('field-validation-valid')
            .addClass('field-validation-error')
            .html(message);
    }

    clearValidationErrors() {
        $('.input-validation-error').removeClass('input-validation-error');
        $('.field-validation-error')
            .removeClass('field-validation-error')
            .addClass('field-validation-valid')
            .empty();
    }

}
