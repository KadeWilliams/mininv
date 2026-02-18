import { api } from '../services/api.js';
import { Validator } from '../services/validator.js';
import { formStateManager } from '../services/FormStateManager.js';
import TomSelect from "tom-select";

export class OrganizationForm {
    constructor(formSelector) {
        this.form = $(formSelector);
        this.forms = this.serializeForms();
        this.organizationId = $("#OrganizationDetails_Organization_OrganizationID").val();
        this.initialState = this.serialize();
        this.initializePlugins();
        this.setupEventHandlers();
        //formStateManager.captureFormState('organization');
    }

    getOrganizationId() {
        return this.organizationId;
    }

    serializeForms() {
        return [new URLSearchParams(new FormData($("#giftDetailForm")[0])).toString(), new URLSearchParams(new FormData($("#requestDetailForm")[0])).toString(), new URLSearchParams(new FormData($("#newOrganizationDetailsForm")[0])).toString()];
    }

    serialize() {
        return new URLSearchParams(new FormData(this.form[0])).toString();
    }

    hasChanges() {
        return this.serialize() !== this.initialState;
    }

    resetState() {
        this.initialState = this.serialize();
    }

    setupEventHandlers() {
        $("#saveOrganizationBtn").on('click', () => this.save());

        $("#ContentServer").on('click', () => this.handleContentServer());

        $("#OrganizationDetails_Organization_CategoryID").on('change', () => {
            this.toggleCategoryOther();
        })

        this.setupLogoUpload();
    }

    initializePlugins() {
        new TomSelect(".Organization_CategoryID", {
            hideSelected: true
        });

        new TomSelect(".PartnerTypeID", {
            hideSelected: true
        })

        new TomSelect('.select_regions', {
            create: false,
            plugins: {
                remove_button: {
                    title: 'Remove this item',
                },
                'input_autogrow': {},
            },
            sortField: {
                field: 'text',
                direction: 'asc'
            }
        });

        if ($('#RequestsTable').length) {
            new DataTable('#RequestsTable', {
                paging: true,
                response: false,
                autoWidth: false,
                columnDefs: [
                    {
                        targets: 0, searchable: false, orderable: false,
                    },
                    { type: 'month/year', targets: 1 }
                ],
                order: [1],
            });
        }

        this.toggleCategoryOther();
    }

    toggleCategoryOther() {
        const categoryOtherId = parseInt($("#OrganizationDetails_Organization_CategoryID").data("other-id"));
        const selectedCategoryId = parseInt($("#OrganizationDetails_Organization_CategoryID").val());

        if (selectedCategoryId === categoryOtherId) {
            $("#CategoryOtherOrganizationRow").show();
        } else {
            $("#CategoryOtherOrganizationRow").hide();
            $("#OrganizationDetails_Organization_CategoryOther").val('');
        }
    }

    setupLogoUpload() {
        window.addEventListener('paste', (e) => {
            const items = e.clipboardData.files;
            if (items.length === 0 || !items[0].type.includes('image')) return;

            const canvas = document.getElementById('my_canvas');
            if (canvas) {
                canvas.style.display = 'block';
                const fileInput = document.querySelector('#OrganizationDetails_Organization_OrganizationLogo');
                fileInput.files = items;

                this.previewImage(items[0], canvas);
            }
        })
    }

    previewImage(file, canvas) {
        const reader = new FileReader();
        reader.onload = (e) => {
            const img = new Image();
            img.onload = () => {
                const ctx = canvas.getContext('2d');
                canvas.width = img.width;
                canvas.height = img.height;
                ctx.drawImage(img, 0, 0);
            };
            img.src = e.target.result;
        }
        reader.readAsDataURL(file);
    }

    async handleContentServer() {
        const folderId = $("#OrganizationDetails_Organization_CSFolderId").val();
        const folderUrlTemplate = $("#ContentServer").data('folder-url');

        if (parseInt(folderId) > 0) {
            const url = folderUrlTemplate.replace('{0}', folderId).replaceAll("amp", '');
            window.open(decodeURI(url));
        } else {
            $('#OrganizationDetails_ContentServer').val(true);
            await this.save();;
        }
    }
    validate() {
        //TODO later 
        return true;
        const validator = new Validator();

        const organizationName = $('#OrganizationDetails_Organization_OrganizationName').val();
        const categoryId = $('#OrganizationDetails_Organization_CategoryID').val();
        const partnerTypeId = $('#OrganizationDetails_Organization_PartnerTypeID').val();
        const categoryOtherId = $('#OrganizationDetails_Organization_CategoryID').data('other-id');
        const categoryOther = $('#OrganizationDetails_Organization_CategoryOther').val();

        const rules = {
            'OrganizationDetails.Organization.OrganizationName': [
                { validator: () => validator.required(organizationName, 'Organization Name') }
            ],
            'OrganizationDetails.Organization.CategoryId': [
                { validator: () => validator.min(categoryId, 1, 'Category') }
            ],
            'OrganizationDetails.Organization.PartnerTypeId': [
                { validator: () => validator.min(partnerTypeId, 1, 'Partner Type') }
            ],
        };

        if (parseInt(categoryId) === parseInt(categoryOtherId)) {
            rules['OrganizationDetails.Organization.CategoryOther'] = [
                { validator: () => validator.required(categoryOther, 'Category Other') }
            ];
        }

        const isValid = validator.validate(rules);

        if (!isValid) {
            validator.showErrors();
        }

        return isValid;
    }

    async save(reload = false) {
        if (!this.validate()) {
            return false;
        }

        try {
            $("#OrganizationDetails_Address_Active").val(
                $("#OrganizationDetails_Address_Active").prop('checked')
            );

            const form = new FormData($('#newOrganizationDetailsForm')[0]);
            const response = await api.saveOrganizations(form);
            const organizationId = new URLSearchParams(response.url.replace('/organization?', '')).get('OrganizationID');

            if (organizationId && organizationId > 0) {
                window.location.href = response.url;
            } else {
                this.showError('Unable to save as it would overwrite other changes. Please reload the page and try again.')
                return false;
            }
        } catch (e) {
            console.error('Failed to save organization:', e);
            this.showError('Failed to save organization. Please try again.');
            return false;
        }
    }

    showSuccess(message) {
        $('#success-message-p').text(message);
        $('.success-message').modal('show');
    }

    showError(message) {
        $('#error-message-p').text(message);
        $('.error-message').modal('show');
    }

    checkForChanges(callback) {
        if (this.hasChanges()) {
            const confirmed = confirm("You've changed the organization form. Would you like to ignore these changes?");
            if (confirmed) {
                callback();
            }
        } else {
            callback();
        }
    }
    async loadRequest(requestId) {
        try {
            const url = `/Organization/RequestFormPartial?requestID=${requestId}&organizationID=${this.organizationId}`;
            const html = await fetch(url).then(r => r.text());

            this.container.html(html);
            this.currentRequestId = requestId;

            // Initialize plugins for the new content
            this.initializeRequestForm();
        } catch (error) {
            console.error('Failed to load request:', error);
            alert('Failed to load request');
        }
    }


}