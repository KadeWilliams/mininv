import { api } from '../services/api.js';
import { Validator } from '../services/validator.js';
import "../jquery.import.js";
import { formStateManager } from '../services/FormStateManager.js';

export class ContactsManager {
    constructor(tableSelector, organizationId) {
        this.table = $(tableSelector);
        this.organizationId = organizationId;
        this.dataTable = null;
        this.setupEventHandlers();
    }

    render(data) {
        if (this.dataTable) {
            this.dataTable.destroy();
        }

        this.dataTable = new DataTable(this.table[0], {
            data: data,
            columns: this.getColumns(),
            responsive: true,
            autoWidth: false,
            paging: true,
            columnDefs: [
                {
                    targets: 0,
                    searchable: false,
                    orderable: false,
                },
            ],
            order: [[1, 'asc']],
        });
    }

    getColumns() {
        return [
            {
                data: null,
                render: (data) => this.renderActionButtons(data)
            },
            { data: 'contactName' },
            { data: 'email' },
            { data: 'cell' },
            { data: 'contactTypeName' },
        ];
    }

    setupEventHandlers() {
        $('#addEditContactBtn').on('click', () => this.showModal());

        $('#tab-Contacts-body').on('click', '#editContactBtn', (e) => {
            const btn = $(e.currentTarget);
            this.showModal({
                contactId: btn.data('contactId'),
                organizationId: btn.data('organizationId'),
                contactName: btn.data('contactName'),
                cell: btn.data('cell'),
                office: btn.data('office'),
                email: btn.data('email'),
                contactTypeId: btn.data('contactTypeId'),
                contactTypeName: btn.data('contactTypeName'),
                contactNotes: btn.data('contactNotes'),
                lastChangeUser: btn.data('lastChangeUser'),
                lastChangeDttm: btn.data('lastChangeDttm'),
            });
        });

        $('#tab-Contacts-body').on('click', '.delete-contact-btn', async (e) => {
            const contactId = $(e.currentTarget).data('contactId');
            $("#OrganizationDetails_DeleteContact").val(true);
            $("#OrganizationDetails_Contact_ContactID").val(contactId);
            await this.delete();
        })

        $('#saveContactBtn').on('click', async (e) => {
            $('#OrganizationDetails_SaveContact').val(true);
            this.save();
        })

        $('#addEditContactModal').on('hidden.bs.modal', () => this.clearForm());
    }

    renderActionButtons(contact) {
        return `
            <button class="btn btn-primary" id="editContactBtn"
                data-contact-id="${contact.contactID}"
                data-organization-id="${contact.organizationID}"
                data-contact-name="${contact.contactName || ''}"
                data-email="${contact.email || ''}"
                data-cell="${contact.cell || ''}"
                data-office="${contact.office || ''}"
                data-contact-notes="${contact.contactNotes || ''}"
                data-contact-type-id="${contact.contactTypeID}"
                data-contact-type-name="${contact.contactTypeName}"
                data-last-change-user="${contact.lastChangeUser}"
                data-last-change-dttm="${contact.lastChangeDttm}">
                <i class="fa fa-edit"></i>
            </button>
            <button class="btn btn-danger delete-contact-btn" data-contact-id="${contact.contactID}">
                <i class="fa fa-trash"></i>
            </button>
        `
    }

    validate() {
        const validator = new Validator();

        const contactName = $('#OrganizationDetails_Contact_ContactName').val();
        const contactTypeID = $('#OrganizationDetails_Contact_ContactTypeID').val();

        const isValid = validator.validate({
            'OrganizationDetails.Contact.ContactName': [
                { validator: () => validator.required(contactName, 'Contact Name', 'OrganizationDetails.Contact.ContactName') }
            ],
            'OrganizationDetails.Contact.ContactTypeID': [
                { validator: () => validator.required(contactTypeID, 'Contact Type', 'OrganizationDetails.Contact.ContactTypeID') }
            ],
        })

        if (!isValid) {
            validator.showErrors('#addEditContactModal');
        }
        return isValid;
    }

    async delete(contactId) {
        if (!confirm('Are you sure you want to delete this contact?')) {
            return;
        }

        try {
            this.save(true);
        } catch (e) {
            console.error('Failed to delete contact', e);
            alert('Failed to delete contact');
        }
    }

    async load() {
        const data = await api.getContacts(this.organizationId);
        this.render(data)
    }

    async save(isDelete) {
        if (!isDelete) {
            if (!this.validate()) {
                return;
            }
        }

        try {
            const form = $('#newOrganizationDetailsForm');
            validationService.validateForm(form[0], async () => {
                const formData = new FormData(form[0]);
                const res = await api.saveOrganizations(formData);
                window.location.href = res.url;
                //await this.load();
                $('#addEditContactModal').modal('hide');
            });
            //await this.load();
        } catch (e) {
            console.error('Failed to save contact:', e);
            alert('Failed to save contact');
        }
    }

    showModal(contact = null) {
        if (contact) {
            $('#OrganizationDetails_Contact_ContactID').val(contact.contactId);
            $('#OrganizationDetails_Contact_OrganizationID').val(contact.organizationId);
            $('#OrganizationDetails_Contact_ContactName').val(contact.contactName);
            $('#OrganizationDetails_Contact_Cell').val(contact.cell);
            $('#OrganizationDetails_Contact_Email').val(contact.email);
            $('#OrganizationDetails_Contact_Office').val(contact.office);
            $('#OrganizationDetails_Contact_ContactTypeID').val(contact.contactTypeId);
            $('#OrganizationDetails_Contact_ContactNotes').val(contact.contactNotes);
            if (contact.lastChangeUser) {
                $('#lastChangedContact').show();
                $('#lastChangedContact #OrganizationDetails_Contact_LastChangeUser').text(contact.lastChangeUser);
                $('#lastChangedContact #OrganizationDetails_Contact_LastChangeDttm').text(contact.lastChangeDttm);
            }
        } else {
            this.clearForm();
        }
        $('#addEditContactModal').modal('show');
        //$('#OrganizationDetails_Contact_Contact');
    }

    clearForm() {
        $('#OrganizationDetails_Contact_ContactID').val(0);
        $('#OrganizationDetails_Contact_OrganizationID').val(this.organizationId);
        $('#OrganizationDetails_Contact_ContactName').val('');
        $('#OrganizationDetails_Contact_Cell').val('');
        $('#OrganizationDetails_Contact_Email').val('');
        $('#OrganizationDetails_Contact_Office').val('');
        $('#OrganizationDetails_Contact_ContactTypeID').val(0);
        $('#OrganizationDetails_Contact_ContactNotes').val('');
        $('#lastChangedContact').hide();
    }
}