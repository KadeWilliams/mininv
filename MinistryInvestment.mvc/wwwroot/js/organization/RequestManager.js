import { api } from '../services/api.js';
import { Validator } from '../services/validator.js';
import '../jquery.import.js';
import { GiftManager } from './GiftManager.js';
import showToast from '../services/toast.js';
import { formStateManager } from '../services/FormStateManager.js';
export class RequestManager {
    constructor(containerSelector, organizationId) {
        this.container = $(containerSelector);
        this.organizationId = organizationId;
        this.currentRequestId = null;
        this.currentGiftId = null;
        this.giftManager = null;

        // Attach handlers ONCE using delegation
        this.setupEventHandlers();
    }

    setupEventHandlers() {
        // Event delegation - handlers attached to container, work on dynamic content
        // Back to org list
        this.container.on('click', '#back-to-org-list', (e) => {
            if (!formStateManager.confirmNavigation(true)) {
                return;
            }
            e.preventDefault();
            this.handleBackToOrgList();
        });

        // Load gift
        this.container.on('click', '.load-gift', (e) => {
            if (!formStateManager.confirmNavigation(true)) {
                return;
            }
            e.preventDefault();
            const giftId = $(e.currentTarget).data('gift-id');
            const requestId = $(e.currentTarget).data('request-id');
            this.loadGift(giftId, requestId);
        });

        this.container.on('click', '.delete-gift', (e) => {
            e.preventDefault();
            const giftId = $(e.currentTarget).data('giftid');
            $("#RequestDetail_Gift_GiftID").val(giftId);
            $("#RequestDetail_DeleteGift").val(true);
            this.saveRequest();
        });

        this.container.on('click', '#addGiftBtn', (e) => {
            if (!formStateManager.confirmNavigation(true)) {
                return;
            }
            e.preventDefault();
            const requestId = $(e.currentTarget).data('request-id');
            this.loadGift(0, requestId);
        });

        this.container.on('click', '#addGiftBtn', (e) => {
            if (!formStateManager.confirmNavigation(true)) {
                return;
            }
            e.preventDefault();
            const requestId = $(e.currentTarget).data('request-id');
            this.loadGift(0, requestId);
        });

        $(document).on('click', '.loadRequestPartial', (e) => {
            const requestId = $(e.currentTarget).data('requestid');
            this.loadRequest(requestId);
        });

        this.container.on('click', '#addEditRequestBtn', (e) => {
            this.loadRequest(0);
        });

        this.container.on('click', '#SaveRequestBtn', (e) => {
            this.saveRequest();
        });

        this.container.on('click', '.show-gift-schedule-modal', (e) => {
            this.showGiftScheduleModal($(e.currentTarget).data());
        })

        this.container.on('click', '.delete-request-btn', (e) => {
            $('#OrganizationDetails_ForDeletion').val(true);
            const requestId = $(e.currentTarget).data('requestid');
            $('#OrganizationDetails_Request_RequestID').val(requestId)
            this.deleteRequest();
        });
    }

    handleBackToOrgList() {
        formStateManager.clearSpecificState('request');
        window.location.href = `/Organization/Index?OrganizationID=${this.organizationId}`;
    }

    async loadRequest(requestId) {
        try {
            //if (this.giftManager) {
            //    this.giftManager.destroy();
            //    this.giftManager = null;
            //}
            const url = `/Organization/RequestFormPartial?requestID=${requestId}&organizationID=${this.organizationId}`;
            const html = await fetch(url).then(r => r.text());

            this.container.html(html);
            this.currentRequestId = requestId;

            // Initialize plugins for the new content
            this.initializeRequestForm();
            setTimeout(() => {
                formStateManager.captureFormState('request');
            }, 100)
            //[window.organizationApp.giftForm, window.organizationApp.requestForm] = getForms();
        } catch (error) {
            console.error('Failed to load request:', error);
            alert('Failed to load request');
        }
    }

    async loadGift(giftId, requestId) {
        try {
            formStateManager.clearSpecificState('request');
            const url = `/Organization/LoadGiftPartial?giftID=${giftId}&requestID=${requestId}&organizationID=${this.organizationId}`;
            const html = await fetch(url).then(r => r.text());

            this.container.html(html);

            this.giftManager = new GiftManager(
                this.container,
                requestId,
                this.organizationId,
                this
            );
            formStateManager.captureFormState('gift');

            //this.currentGiftId = giftId;
            //this.currentRequestId = requestId;

            // Initialize plugins for the new content
            //this.initializeGiftForm();
            //[window.organizationApp.giftForm, window.organizationApp.requestForm] = getForms();

        } catch (error) {
            console.error('Failed to load gift:', error);
            alert('Failed to load gift');
        }
    }

    initializeRequestForm() {
        // Initialize date picker
        let d = $("#RequestDetail_Request_VoteDate").val() == "0001-01-01T00:00" ? new Date() : new Date($("#RequestDetail_Request_VoteDate").val());
        flatpickr("#RequestDetail_Request_VoteDate", {
            defaultDate: d,
            allowInput: true,
            plugins: [
                monthSelectPlugin({
                    dateFormat: "m/Y",
                })
            ],
        });

        // Initialize dropdowns
        if ($('.Request_ProjectTypeID').length) {
            new TomSelect('.Request_ProjectTypeID', { hideSelected: true });
        }

        // Initialize gifts table if present
        if ($('#GiftsTable').length) {
            new DataTable('#GiftsTable', {
                dom: "tir",
                paging: false,
                columnDefs: [
                    { targets: 0, searchable: false, orderable: false }
                ],
                order: [[1, 'asc']],
            });
        }
    }

    hasUnsavedChanges() {
        // Check if form has been modified
        const form = this.container.find('form');
        if (!form.length) return false;

        // You can implement proper change tracking here
        // For now, just check if any input has been changed
        return false; // Placeholder
    }

    async deleteRequest() {
        $('#OrganizationDetails_SaveRequest').val(true);
        const form = $('#newOrganizationDetailsForm');
        if (!form.length) {
            console.error('Organization form not found');
            return;
        }

        validationService.validateForm(form[0], async (e) => {
            try {
                const formData = new FormData(form[0]);
                const res = await api.saveOrganizations(formData);
                window.location.href = res.url;
            } catch (error) {
                console.error('Failed to delete request:', error);
                alert('Failed to save request');
            }
        });
    }

    async saveRequest() {
        // Get form data
        $('#OrganizationDetails_SaveRequest').val(true);
        const form = $('#requestDetailForm');
        if (!form.length) {
            console.error('Request form not found');
            return;
        }

        if (!this.validate()) {
            return;
        }
        $("#blockApplicationDialog").modal('show');


        validationService.validateForm(form[0], async (e) => {
            try {
                const formData = new FormData(form[0]);
                const response = await fetch('/Organization/SaveRequestDetail', {
                    method: 'POST',
                    body: formData,
                });

                const result = await response.json();
                // Reload the request to show updated data
                //await this.loadRequest(result.requestID || this.currentRequestId);
                showToast('Your changes have been saved!')
                api.saveOrganizations();
                this.loadRequest(result.requestID || this.currentRequestId);
            } catch (error) {
                console.error('Failed to save request:', error);
                showToast('Failed to save request');
            }
            finally {
                $("#blockApplicationDialog").modal('hide');
            }
        });
        formStateManager.resetFormState('request');
    }

    destroy() {
        if (this.giftManager) {
            this.giftManager.destroy();
        }
        //this.container.off();
    }

    validate() {
        const validator = new Validator();

        const requestedAmount = $('#RequestDetail_Request_RequestedAmount').val();
        const description = $('#RequestDetail_Request_Description').val();

        const isValid = validator.validate({
            'RequestDetail.Request.RequestedAmount': [
                { validator: () => validator.required(requestedAmount, 'Requested Amount', 'RequestDetail.Request.RequestedAmount') }
            ],
            'RequestDetail.Request.Description': [
                { validator: () => validator.required(description, 'Description', 'RequestDetail.Request.Description') }
            ],
        })

        if (!isValid) {
            validator.showErrors('#RequestContainer');
        }
        return isValid;
    }
}
