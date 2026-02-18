class FormStateManager {
    constructor() {
        this.initialStates = new Map();
        this.formSelectors = {
            assessment: '#assessmentForm',
            request: '#requestDetailForm',
            gift: '#giftDetailForm',
            organization: '#newOrganizationDetailsForm'
        }
        this.customStateCapturers = new Map();
        this.setupBeforeUnloadWarning();
    }

    registerCustomCapturer(formType, capturerFn) {
        this.customStateCapturers.set(formType, capturerFn);
    }

    /// Captures the current state of a form as the "initial" state
    captureFormState(formType) {
        if (this.customStateCapturers.has(formType)) {
            const capturer = this.customStateCapturers.get(formType);
            const state = capturer();
            this.initialStates.set(formType, state);
            return;
        }

        const selector = this.formSelectors[formType];
        const form = $(selector)[0];

        if (!form) {
            this.initialStates.set(formType, null);
            return;
        }

        const formData = new FormData(form);
        const serialized = new URLSearchParams(formData).toString();
        this.initialStates.set(formType, serialized)
    }

    getCurrentFormState(formType) {
        const selector = this.formSelectors[formType];
        const form = $(selector)[0];

        if (!form) return null;

        const formData = new FormData(form);
        return new URLSearchParams(formData).toString();
    }

    captureAssessmentState() {
        const state = {
            requestDate: $("#requestDate").val() || '',
            assessmentNote: $("#assessmentNote").val() || '',
            scores: [],
        };

        $('.score-input').each(function () {
            const categoryId = $(this).data('categoryid');
            const checked = $(`input.score-input[data-categoryid=${categoryId}]:checked`).val();
            const notes = $(`textarea.notes-input[data-categoryid=${categoryId}]`).val() || '';

            state.scores.push({
                categoryId: categoryId,
                score: checked || '',
                notes: notes,
            });
        });

        return JSON.stringify(state);
    }

    /// Checks if a specific form has unsaved changes 
    isFormDirty(formType) {
        const initial = this.initialStates.get(formType);
        let current;
        if (formType == 'assessment') {
            current = this.captureAssessmentState();
        } else {
            current = this.getCurrentFormState(formType);
        }

        if (initial === null || current === null) {
            return false;
        }

        return initial !== current;
    }

    /// Gets an array of all forms with unsaved changes
    getDirtyForms(formType) {
        const dirtyForms = [];

        for (const [formType, _] of this.initialStates) {
            if (this.isFormDirty(formType)) {
                dirtyForms.push(formType);
            }
        }

        return dirtyForms;
    }

    /// Checks if any form has unsaved changes
    hasUnsavedChanges() {
        return this.getDirtyForms().length > 0;
    }

    /// Shows confirmation dialog if there are unsaved changes 
    confirmNavigation(excludeOrganization = false) {
        console.trace('confirming navigation')
        const dirtyForms = this.getDirtyForms().filter(form => {
            if (excludeOrganization && form === 'organization') {
                return false;
            }
            return true;
        })

        if (dirtyForms.length === 0) {
            return true;
        }

        const formNames = dirtyForms.map((form, i) => {
            if (i === 0) return form;
            if (i === dirtyForms.length - 1) return ` and ${form}`;
            return `, ${form}`;
        }).join('');

        const pluralForm = dirtyForms.length > 1 ? 'forms' : 'form';
        const message = `You have unsaved changes in the ${formNames} ${pluralForm}. Do you want to discard these changes?`;

        return confirm(message);
    }

    /// Resets a form's initial state (call after save)
    resetFormState(formType) {
        this.captureFormState(formType);
    }

    /// Clears all form states (call when returning to list view)
    clearAllStates() {
        this.initialStates.clear();
    }

    clearSpecificState(formType) {
        this.initialStates.delete(formType);
    }

    setupBeforeUnloadWarning() {
        window.addEventListener('beforeunload', (e) => {
            if (this.hasUnsavedChanges()) {
                e.preventDefault();
                e.returnValue;
                return '';
            }
        })
    }

    withUnsavedCheck(navigationFn, excludeOrganization = false) {
        return (...args) => {
            if (this.confirmNavigation(excludeOrganization)) {
                return navigationFn(...args);
            }
            return false;
        }
    }
}
export const formStateManager = new FormStateManager();