import { OrganizationForm } from './OrganizationForm.js';
import { ContactsManager } from './ContactsManager.js';
import { AddressesManager } from './AddressesManager.js';
import { RequestManager } from './RequestManager.js';
import { FinancialManager } from './FinancialInformationManager.js';
import { AssessmentManager } from './AssessmentsManager.js';
import "../jquery.import.js";
//import { checkFormState } from '../../../Views/Shared/Scripts/request.js';

window.organizationApp = {};

document.addEventListener('DOMContentLoaded', () => {
    const orgIdElement = $('#OrganizationDetails_Organization_OrganizationID');

    if (!orgIdElement) {
        console.warn("Organization form not found on this page");
        return;
    }
    const organizationId = orgIdElement.val();

    const orgForm = new OrganizationForm('#newOrganizationDetailsForm');
    window.organizationApp.orgForm = orgForm;

    if (organizationId && parseInt(organizationId) >= 0) {
        const contactsManager = new ContactsManager('#ContactsTable', orgForm.getOrganizationId());
        const addressesManager = new AddressesManager('#AddressesTable', orgForm.getOrganizationId());
        const financialManager = new FinancialManager('#FinancialsTable', orgForm.getOrganizationId());
        const assessmentManager = new AssessmentManager('#AssessmentsTable', orgForm.getOrganizationId());
        const requestManager = new RequestManager('#requests', orgForm.getOrganizationId());

        window.organizationApp.contacts = contactsManager;
        window.organizationApp.addresses = addressesManager;
        window.organizationApp.requests = requestManager;
        window.organizationApp.financials = financialManager;
        window.organizationApp.assessments = assessmentManager;

        $('#contacts-tab').on('click', () => {
            //    orgForm.checkForChanges(() => {
            contactsManager.load();
            //    });
        });

        $('#addresses-tab').on('click', () => {
            //    orgForm.checkForChanges(() => {
            addressesManager.load();
            //    });
        });

        $('#financials-tab').on('click', () => {
            //    orgForm.checkForChanges(() => {
            financialManager.load();
            //    });
        });

        $('#assessments-tab').on('click', () => {
            //    orgForm.checkForChanges(() => {
            assessmentManager.load();
            //    });
        });

        const queryString = window.location.search;
        const urlParams = new URLSearchParams(queryString);
        const [request, gift, contact, financial, assessment] = [urlParams.get('RequestID'), urlParams.get('GiftID'), urlParams.get('ContactID'), urlParams.get('FinancialInformationID'), urlParams.get('AssessmentID')];

        switch (true) {
            case (request != 0):
                // todo open request? 
                break
            case (gift != 0):
                // todo open gift
                break
            case (contact != 0):
                $('#contacts-tab').trigger('click');
                break
            case (financial != 0):
                $('#financials-tab').trigger('click');
                break
            case (assessment != 0):
                $('#assessments-tab').trigger('click');
                break
        }

        $(document).on('click', '#btnReloadRequestList', () => {
            orgForm.checkForChanges(() => {
                location.href = $('#btnReloadRequestList').data('orgNavigation');
            });
        })

        initializeRequestsTable();
    } else {
        showError('Unable to save as it would overwrite other changes. Please reload the page and try again.')
    }

    handlePageLoadParameters();
});

function showError(message) {
    $('#error-message-p').text(message);
    $('.error-message').modal('show');
}

function initializeRequestsTable() {
    new DataTable('#RequestsTable', {
        destroy: true,
        paging: true,
        columnDefs: [
            {
                targets: 0,
                searchable: false,
                orderable: false,
            },
            {
                type: 'month/year',
                targets: 1
            }
        ],
        order: [[1, 'asc']],
    });
}

function handlePageLoadParameters() {
    document.getElementById("pageStart").style.opacity = "1";
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('AddressID') && parseInt(urlParams.get('AddressID')) > 0) {
        $('#addresses-tab').click();
    }

    if (urlParams.has('ContactID') && parseInt(urlParams.get('ContactID')) > 0) {
        $('#contacts-tab').click();
    }

    if (urlParams.has('RequestID') && parseInt(urlParams.get('RequestID')) > 0) {
        console.log('Load request: ', urlParams.get('RequestID'));
    }
}

