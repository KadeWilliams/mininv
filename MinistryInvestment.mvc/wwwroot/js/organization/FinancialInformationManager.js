import { api } from '../services/api.js'
import { Validator } from '../services/validator.js'
import '../jquery.import.js';
export class FinancialManager {
    constructor(tableSelector, organizationId) {
        this.table = $(tableSelector);
        this.organizationId = organizationId;
        this.dataTable = null;
        this.financialDocumentsDataTable = null;
        this.setupEventHandlers();
    }

    initializePlugins() {
        let d = $("#OrganizationDetails_FinancialInformation_FiscalYear").val() == "0001-01-01T00:00" ? new Date() : new Date($("#OrganizationDetails_FinancialInformation_FiscalYear").val());
        flatpickr("#OrganizationDetails_FinancialInformation_FiscalYear", {
            defaultDate: d,
            allowInput: true,
            plugins: [
                monthSelectPlugin({
                    dateFormat: "m/Y",
                })
            ],
        });
    }

    setupEventHandlers() {
        $("#addEditFinancialInformationBtn").on('click', () => this.showModal());

        $("#tab-Financials-body").on('click', '#editFinancialInformationBtn', async (e) => {
            const btn = $(e.currentTarget);
            const financialInformationID = btn.data('financialInformationId');
            const financialDocuments = await api.getFinancialDocuments(financialInformationID);
            this.renderFinancialDocuments(financialDocuments);
            this.showModal({
                financialInformationID: financialInformationID,
                organizationID: btn.data('organizationId'),
                fiscalYear: btn.data('fiscalYear'),
                fiscalYearEnd: btn.data('fiscalYearEnd'),
                revenue: btn.data('revenue'),
                programs: btn.data('programs'),
                administration: btn.data('administration'),
                fundraising: btn.data('fundraising'),
                liabilities: btn.data('liabilities'),
                totalExpenses: btn.data('totalExpenses'),
                cash: btn.data('cash'),
                notes: btn.data('notes'),
                lastchangeuser: btn.data('lastChangeUser'),
                lastchangedttm: btn.data('lastChangeDttm'),
            });
        });

        $('#tab-Financials-body').on('click', '#deleteFinancialInformation', async (e) => {
            const financialId = $(e.currentTarget).data('financialInformationId');
            await this.delete(financialId);
        });

        $('#addEditFinancialInformationModal').on('hidden.bs.modal', () => this.clearForm());

        $('#saveFinancialInformationBtn').on('click', async (e) => {
            $('#OrganizationDetails_SaveFinancialInformation').val(true);
            this.save();
        })

        $("#OrganizationDetails_FinancialInformation_FiscalYear").on('change', (e) => {
            this.handleFiscalYearChange(e);
        })
    }

    async load() {
        const data = await api.getFinancials(this.organizationId);
        this.render(data);
    }

    render(data) {
        if (this.dataTable) {
            this.dataTable.destroy();
        }
        this.dataTable = new DataTable(this.table[0], {
            response: false,
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

    renderFinancialDocuments(dataArray) {
        if (this.financialDocumentsDataTable) {
            this.financialDocumentsDataTable.destroy();
            this.financialDocumentsDataTable = null;
        }

        $('#FinancialDocumentsTable').empty();

        if (!dataArray || dataArray.length === 0) {
            return;
        }

        $('#FinancialDocumentsTable').html(`
            <thead>
                <tr>
                    <th>Actions</th>
                    <th>Document Name</th>
                </tr>
            </thead>
            <tbody></tbody>
        `);

        this.financialDocumentsDataTable = new DataTable("#FinancialDocumentsTable", {
            destroy: true,
            response: false,
            autoWidth: false,
            data: dataArray,
            searching: false,
            paging: true,
            columns: [
                {
                    data: null,
                    render: (data) => this.renderCSButtons(data)
                },
                { data: "documentName" },
            ],
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
            {
                data: null,
                render: (data) => {
                    const fiscalStart = new Date(data.fiscalYear);
                    const fiscalEnd = new Date(data.fiscalYearEnd);
                    return `${fiscalStart.getMonth() + 1}/${fiscalStart.getFullYear()} to ${fiscalEnd.getMonth() + 1}/${fiscalEnd.getFullYear()}`
                }
            },
            {
                data: "revenue",
                render: (data, type, row) => {
                    return `${data.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}<br/><span style="color:${row.revenueGrowthPercent >= 0 ? 'green' : 'red'}">(${row.revenueGrowthPercent}% Revenue Growth)</span>`;
                }
            },
            {
                data: "programs",
                render: (data, type, row) => {
                    return `${data.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}<br/><span style="color:${row.programsPercent >= 0 ? 'green' : 'red'}">(${row.programsPercent}% of Revenue)</span>`;
                }
            },
            {
                data: "administration",
                render: (data, type, row) => {
                    return `${data.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}<br/><span style="color:${row.administrationPercent >= 0 ? 'green' : 'red'
                        }">(${row.administrationPercent}% of Revenue)</span>`;
                }
            },
            {
                data: "fundraising",
                render: (data, type, row) => {
                    return `${data.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}<br/><span style="color:${row.fundraisingPercent >= 0 ? 'green' : 'red'}">(${row.fundraisingPercent}% of Revenue)</span>`;
                }
            },
            {
                data: "liabilities",
                render: (data, type, row) => {
                    return `${data.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}<br/><span style="color:${row.liabilitiesPercent >= 0 ? 'green' : 'red'}">(${row.liabilitiesPercent}% of Revenue)</span>`;
                }
            },
            {
                data: "totalExpenses",
                render: (data, type, row) => {
                    return `${data.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}<br/><span style="color:${row.totalExpensesPercent >= 0 ? 'green' : 'red'}">(${row.totalExpensesPercent}% of Revenue)</span>`;
                }
            },
            {
                data: "cash",
                render: (data, type, row) => {
                    return `${data.toLocaleString('en-US', { style: 'currency', currency: 'USD' })}<br/><span style="color:${row.totalExpensesPercent >= 0 ? 'red' : 'green'}">(${row.cashPercent}% of Expenses)</span>`;
                }
            },
        ];
    }

    renderCSButtons(data) {
        return `
            <a class="btn btn-sm btn-info" style="margin-left: 8px;" target="_blank" href="${data.link}">
                <i class="fas fa-file-pdf fa-color-white"></i> Review
            </a>
            <a class="btn btn-sm btn-info" style="margin-left: 8px;" target="_blank">
                <i class="fas fa-download fa-color-white"></i>
            </a>
        `;
    }

    renderActionButtons(financialInformation) {
        const hasEditPermissions = $('#financials-tab').data('has-edit-permissions');

        if (!hasEditPermissions) {
            return '';
        }
        return `
            <button type='button'
                class='btn btn-primary'
                id='editFinancialInformationBtn'
                data-financial-information-id='${financialInformation.financialInformationID}'
                data-organization-id='${financialInformation.organizationID}'
                data-fiscal-year='${financialInformation.fiscalYear}'
                data-fiscal-year-end='${financialInformation.fiscalYearEnd}'
                data-revenue='${financialInformation.revenue}'
                data-programs='${financialInformation.programs}'
                data-administration='${financialInformation.administration}'
                data-fundraising='${financialInformation.fundraising}'
                data-liabilities='${financialInformation.liabilities}'
                data-total-expenses='${financialInformation.totalExpenses}'
                data-cash='${financialInformation.cash}'
                data-notes='${financialInformation.notes}'
                data-lastchangeuser='${financialInformation.lastChangeUser}'
                data-lastchangedttm='${financialInformation.lastChangeDttm}'
            ><i class='fa fa-edit'></i></button>
            <button type='button'
                class='btn btn-danger mx-1'
                id='deleteFinancialInformation'
                data-financial-information-id='${financialInformation.financialInformationID}'
            ><i class='fa fa-trash'></i></button>
        `;
    }

    showModal(financial) {
        if (financial) {
            const fiscalEnd = new Date(financial.fiscalYearEnd);

            $('#OrganizationDetails_FinancialInformation_FinancialInformationID').val(financial.financialInformationID);
            $('#OrganizationDetails_FinancialInformation_OrganizationID').val(this.organizationId);
            $('#OrganizationDetails_FinancialInformation_FiscalYear').val(financial.fiscalYear);
            $('#FiscalYearEnd').text(`${fiscalEnd.getMonth() + 1}/${fiscalEnd.getFullYear()}`);
            $('#OrganizationDetails_FinancialInformation_Revenue').val(financial.revenue);
            $('#OrganizationDetails_FinancialInformation_Programs').val(financial.programs);
            $('#OrganizationDetails_FinancialInformation_Administration').val(financial.administration);
            $('#OrganizationDetails_FinancialInformation_Fundraising').val(financial.fundraising);
            $('#OrganizationDetails_FinancialInformation_Liabilities').val(financial.liabilities);
            $('#OrganizationDetails_FinancialInformation_TotalExpenses').val(financial.totalExpenses);
            $('#OrganizationDetails_FinancialInformation_Cash').val(financial.cash);
            $('#OrganizationDetails_FinancialInformation_Notes').val(financial.notes);
            if (financial.lastChangeUser) {
                $('#lastChangedFinancialInformation').show();
                $('#lastChangedFinancialInformation #OrganizationDetails_FinancialInformation_LastChangeUser').text(financial.lastChangeUser);
                $('#lastChangedFinancialInformation #OrganizationDetails_FinancialInformation_LastChangeDttm').text(financial.lastChangeDttm);
            }
        } else {
            this.clearForm();
        }
        this.initializePlugins();
        $('#addEditFinancialInformationModal').modal('show');
    }
    clearForm() {
        $('#OrganizationDetails_FinancialInformation_FinancialInformationID').val(0)
        $('#OrganizationDetails_FinancialInformation_OrganizationID').val(this.organizationId)
        $('#OrganizationDetails_FinancialInformation_FiscalYear').val('');
        $("#FiscalYearEnd").text('');
        $('#OrganizationDetails_FinancialInformation_Revenue').val('');
        $('#OrganizationDetails_FinancialInformation_Programs').val('');
        $('#OrganizationDetails_FinancialInformation_Administration').val('');
        $('#OrganizationDetails_FinancialInformation_Fundraising').val('');
        $('#OrganizationDetails_FinancialInformation_Liabilities').val('');
        $('#OrganizationDetails_FinancialInformation_TotalExpenses').val('');
        $('#OrganizationDetails_FinancialInformation_Cash').val('');
        $('#OrganizationDetails_FinancialInformation_Notes').val('');
        $('#lastChangedFinancialInformation').hide();
    }
    validate() {
        const validator = new Validator();

        const fiscalYear = $('#OrganizationDetails_FinancialInformation_FiscalYear').val();

        const isValid = validator.validate({
            'OrganizationDetails.FinancialInformation.FiscalYear': [
                { validator: () => validator.required(fiscalYear, 'Fiscal Year', 'OrganizationDetails.FinancialInformation.FiscalYear') }
            ],
        })

        return isValid;
        // todo later 
    }

    async save(isDelete = false) {
        if (!isDelete) {
            if (!this.validate()) {
                return;
            }
        }
        const form = $('#newOrganizationDetailsForm');

        validationService.validateForm(form[0], async (e) => {
            try {
                const formData = new FormData(form[0]);
                const res = await api.saveOrganizations(formData);
                window.location.href = res.url;
                //await this.load();
                $('#addEditFinancialInformationModal').modal('hide');
            } catch (e) {
                console.error('Failed to save financial:', e)
                alert('Failed to save financial');
            }
        })
    }
    async delete(financialId) {
        if (!confirm('Are you sure you want to delete this financial information?')) {
            return;
        }

        try {
            $("#OrganizationDetails_FinancialInformation_FinancialInformationID").val(financialId);
            $("#OrganizationDetails_DeleteFinancialInformation").val(true);
            await this.save(true);
            //await this.load();
        } catch (e) {
            console.error('Failed to delete financial: ', e);
            alert('Failed to delete financial');
        }
    }

    handleFiscalYearChange(e) {
        const newDate = $(e.currentTarget).val().split('/');
        let newYear, newMonth = ''

        if (Number(newDate[0]) - 1 == 0) {
            newMonth = 12;
            newYear = Number(newDate[1]);
        } else {
            newMonth = Number(newDate[0]) - 1;
            newYear = Number(newDate[1]) + 1;
        }
        $("#FiscalYearEnd").text(`${newMonth}/${newYear}`);
    }
}
