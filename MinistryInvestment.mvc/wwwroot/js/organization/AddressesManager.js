import { api } from '../services/api.js';
import { Validator } from '../services/validator.js';
import "../jquery.import.js";
import { formStateManager } from '../services/FormStateManager.js';

export class AddressesManager {
    constructor(tableSelector, organizationId) {
        this.table = $(tableSelector);
        this.organizationId = organizationId;
        this.dataTable = null;
        this.setupEventHandlers();
    }

    setupEventHandlers() {
        $('#addEditAddressBtn').on('click', () => this.showModal());

        $('#tab-Addresses-body').on('click', '#editAddressBtn', (e) => {
            const btn = $(e.currentTarget);
            this.showModal({
                addressID: btn.data('addressid'),
                organizationID: btn.data('organizationid'),
                attnLine: btn.data('attnline'),
                address1: btn.data('address1'),
                address2: btn.data('address2'),
                city: btn.data('city'),
                state: btn.data('state'),
                zipCode: btn.data('zipcode'),
                countryID: btn.data('countryid'),
                active: btn.data('active'),
                lastChangeUser: btn.data('lastchangeuser'),
                lastChangeDttm: btn.data('lastchangedttm'),
            });
        });

        $('#tab-Addresses-body').on('click', '.delete-address-btn', async (e) => {
            $("#OrganizationDetails_Address_AddressID").val($(e.currentTarget).data('addressid'));
            $("#OrganizationDetails_DeleteAddress").val(true);
            await this.delete();
        })

        $('#addEditAddressModal').on('hidden.bs.modal', () => this.clearForm());

        $('#saveAddressBtn').on('click', async (e) => {
            $('#OrganizationDetails_SaveAddress').val(true);
            this.save();
        })

        $('#OrganizationDetails_Address_Active').on('change', function () {
            $('#OrganizationDetails_Address_Active').val(this.checked);
        });

    }

    async load() {
        try {
            const data = await api.getAddresses(this.organizationId);
            this.render(data);
        } catch (e) {
            console.error('Failed to load addresses:', e);
            alert('Failed to load addresses');
        }
    }

    render(data) {
        if (this.dataTable) {
            this.dataTable.destroy();
        }

        this.dataTable = new DataTable(this.table[0], {
            destroy: true,
            responsive: false,
            autoWidth: false,
            data: data,
            paging: true,
            columns: this.getColumns(),
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
                render: (data) => this.renderActionButtons(data),
            },
            { data: 'attnLine' },
            { data: 'address1' },
            { data: 'address2' },
            { data: 'city' },
            { data: 'state' },
            { data: 'zipCode' },
            { data: 'countryName' },
            {
                data: null,
                render: (data) => data.active ? 'Active' : 'Inactive'
            },
        ];
    }

    renderActionButtons(address) {
        const hasEditPermissions = $("#addresses-tab").data('has-edit-permissions');

        if (!hasEditPermissions) {
            return '';
        }

        return `
            <button type="button"
                class="btn btn-primary"
                id="editAddressBtn"
                data-addressid="${address.addressID}"
                data-organizationid="${address.organizationID}"
                data-attnline="${address.attnLine || ''}"
                data-address1="${address.address1 || ''}"
                data-address2="${address.address2 || ''}"
                data-city="${address.city || ''}"
                data-state="${address.state || ''}"
                data-zipcode="${address.zipCode || ''}"
                data-countryid="${address.countryID || ''}"
                data-active="${address.active}"
                data-lastchangeuser="${address.lastChangeUser}"
                data-lastchangedttm="${address.lastChangeDttm}"
            ><i class="fa fa-edit"></i></button>
            <button type="button"
                class="btn btn-danger mx-1 delete-address-btn"
                data-addressid="${address.addressID}"
            ><i class="fa fa-trash"></i></button>
        `;
    }

    showModal(address = null) {
        if (address) {
            $('#OrganizationDetails_Address_AddressID').val(address.addressID);
            $('#OrganizationDetails_Address_OrganizationID').val(address.organizationID);
            $('#OrganizationDetails_Address_AttnLine').val(address.attnLine);
            $('#OrganizationDetails_Address_Address1').val(address.address1);
            $('#OrganizationDetails_Address_Address2').val(address.address2);
            $('#OrganizationDetails_Address_City').val(address.city);
            $('#OrganizationDetails_Address_State').val(address.state);
            $('#OrganizationDetails_Address_ZipCode').val(address.zipCode);
            $('#OrganizationDetails_Address_CountryID').val(address.countryID);
            $('#OrganizationDetails_Address_Active').prop('checked', address.active);
            $('#OrganizationDetails_Address_Active').val(address.active);
            $('#OrganizationDetails_Address_LastChangeUser').val(address.lastChangeUser);

            if (address.lastChangeUser) {
                $('#lastChangedAddress').show();
                $('#lastChangedAddress #OrganizationDetails_Address_LastChangeUser').text(address.lastChangeUser);
                $('#lastChangedAddress #OrganizationDetails_Address_LastChangeDttm').text(address.lastChangeDttm);
            }
        } else {
            this.clearForm();
        }
        $("#addEditAddressModal").modal('show');
    }

    clearForm() {
        $('#OrganizationDetails_Address_AddressID').val(0);
        $('#OrganizationDetails_Address_AttnLine').val('');
        $('#OrganizationDetails_Address_Address1').val('');
        $('#OrganizationDetails_Address_Address2').val('');
        $('#OrganizationDetails_Address_City').val('');
        $('#OrganizationDetails_Address_State').val('');
        $('#OrganizationDetails_Address_ZipCode').val('');
        $('#OrganizationDetails_Address_CountryID').val(0);
        $('#OrganizationDetails_Address_Active').prop('checked', true);
        $('#lastChangedAddress').hide();

        $(".field-validation-error").empty().removeClass('field-validation-error');
        $(".input-validation-error").removeClass('input-validation-error');
    }

    validate() {
        const validator = new Validator();

        const address1 = $('#OrganizationDetails_Address_Address1').val();
        const countryId = $('#OrganizationDetails_Address_CountryID').val();

        const isValid = validator.validate({
            'OrganizationDetails.Address.Address1': [
                { validator: () => validator.required(address1, 'Address 1', 'OrganizationDetails.Address.Address1') }
            ],
            'OrganizationDetails.Address.CountryID': [
                { validator: () => validator.min(countryId, 1, 'Country', 'OrganizationDetails.Address.CountryID') }
            ],
        })

        if (!isValid) {
            validator.showErrors('#addEditAddressModal');
        }
        return isValid;
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
                $('#addEditAddressModal').modal('hide');
            });
        } catch (e) {
            console.error('Failed to save address:', e);
            alert('Failed to save address');
        }
    }

    async delete() {
        if (!confirm('Are you sure you want to delete this address?')) {
            return;
        }

        try {
            await this.save(true);
            // this.load();
        } catch (e) {
            console.error('Failed to delete address', e);
            alert('Failed to delete address');
        }
    }
}