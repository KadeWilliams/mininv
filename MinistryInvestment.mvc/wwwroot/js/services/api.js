class ApiService {
    constructor(baseUrl = '') {
        this.baseUrl = baseUrl;
    }

    async saveOrganizations(form = new FormData($('#newOrganizationDetailsForm')[0])) {
        const response = await fetch('/Organization/SaveOrganizationDetails', {
            method: 'POST',
            body: form,
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        return await response.json();
    }

    async request(url, options = {}) {
        try {
            const response = await fetch(this.baseUrl + url, {
                ...options,
                headers: {
                    'Content-Type': 'application/json',
                    ...options.headers,
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            return await response.json();
        } catch (e) {
            console.error('API Error:', e);
            throw error;
        }
    }

    async getContacts(organizationId) {
        return this.request(`/Organization/GetContacts?organizationId=${organizationId}`);
    }

    async getAddresses(organizationId) {
        return this.request(`/Organization/GetAddresses?organizationId=${organizationId}`);
    }

    async getFinancials(organizationId) {
        return this.request(`/Organization/GetFinancials?organizationId=${organizationId}`);
    }

    async getFinancialDocuments(financialInformationID) {
        return this.request(`/Organization/GetFinancialDocuments?financialInformationId=${financialInformationID}`);
    }

    async getAssessments(organizationId) {
        return this.request(`/Organization/GetAssessments?organizationID=${organizationId}`);
    }
    async getAssessmentDetail(assessmentId, organizationId) {
        return this.request(`/Organization/GetAssessmentDetails?assessmentID=${assessmentId}&organizationID=${organizationId}`);
    }
    async newAssessmentForm(organizationId) {
        return this.request(`/Organization/NewAssessmentForm?organizationID=${organizationId}`);
    }
    async saveAssessment(formData) {
        const response = await fetch('/Organization/SaveAssessment', {
            method: 'POST',
            body: formData
        })

        if (!response.ok) {
            const error = await response.text();
            console.error('response status: ', response.status)
            console.error('response error: ', response.error)
            throw new Error(error);
        }

        return response.json();
    }

    async deleteAssessment(assessmentId) {
        const response = await fetch(`/Organization/DeleteAssessment?assessmentID=${assessmentId}`, {
            method: 'GET'
        });

        if (!response.ok) {
            throw new Error('Failed to delete assessment');
        }
    }

    async deleteGiftSchedule(giftId, giftScheduleId) {
        $("#GiftDetail_Gift_GiftID").val(giftId)
        $("#GiftDetail_GiftSchedule_GiftScheduleID").val(giftScheduleId)
        const form = $('#giftDetailForm');
        if (!form.length) {
            console.error('Gift form not found');
            return;
        }

        validationService.validateForm(form[0], async (e) => {
            try {
                const formData = new FormData(form[0]);
                const response = await fetch('/Organization/SaveGiftForm', {
                    method: 'POST',
                    body: formData,
                });

                const result = await response.json();

                // Reload the request to show updated data
                //await this.loadRequest(result.requestID || this.currentRequestId);
                api.saveOrganizations();
            } catch (error) {
                console.error('Failed to save gift:', error);
            }
            finally {
                $("#blockApplicationDialog").modal('hide');
            }
        });
    }
}

export const api = new ApiService();

