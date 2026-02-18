export class Validator {
    constructor() {
        this.errors = {};
    }

    required(value, fieldName, fullFieldName) {
        if (!value || value.trim() === '') {
            this.errors[fullFieldName] = `${fieldName} is required`;
            return false;
        }
        return true;
    }

    min(value, minValue, fieldName, fullFieldName) {
        if (parseFloat(value) < minValue || !value) {
            this.errors[fullFieldName || fieldName] = `${fieldName} must have a value`;
            return false;
        }
        return true;
    }

    max(value, maxValue, fieldName, fullFieldName) {
        if (parseFloat(value) > maxValue) {
            this.errors[fullFieldName || fieldName] = `${fieldName} cannot exceed ${maxValue}`;
            return false;
        }
        return true;
    }

    email(value, fieldName, fullFieldName) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\@]+$/;
        if (value && !emailRegex.test(value)) {
            this.errors[fullFieldName || fieldName] = 'Invalid email format';
            return false;
        }
        return true;
    }
    validate(rules) {
        this.errors = {};
        let isValid = true;

        for (const [field, fieldRules] of Object.entries(rules)) {
            for (const rule of fieldRules) {
                if (!rule.validator()) {
                    isValid = false;
                    break;
                }
            }
        }
        return isValid;
    }

    showErrors(formElement) {
        $(formElement).find('.field-validation-error').empty().removeClass('field-validation-error');
        $(formElement).find('.input-validation-error').removeClass('input-validation-error');

        for (const [field, message] of Object.entries(this.errors)) {
            const input = $(formElement).find(`[name="${field}"]`);
            input.addClass('input-validation-error');
            input.siblings('.field-validation-valid')
                .removeClass('field-validation-valid')
                .addClass('field-validation-error')
                .text(message);
        }
    }
}