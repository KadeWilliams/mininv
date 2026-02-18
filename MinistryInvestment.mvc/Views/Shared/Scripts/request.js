let minDate = new Date()
minDate = minDate.setDate(0);

let giftForm;
let requestForm;
let organizationForm;
function setOrgForm() {
    [giftForm, requestForm, organizationForm] = getForms();
}
function setReqForm() {
    [giftForm, requestForm, _] = getForms();
}
function setGiftForm() {
    [giftForm, requestForm, _] = getForms();
}

function getForms() {
    return [new URLSearchParams(new FormData($("#giftDetailForm")[0])).toString(), new URLSearchParams(new FormData($("#requestDetailForm")[0])).toString(), new URLSearchParams(new FormData($("#newOrganizationDetailsForm")[0])).toString()];
}

function checkFormState(url, element, requestID = 0) {
    let [newGiftForm, newRequestForm, newOrganizationForm] = getForms();
    if (!newOrganizationForm) {
        location.href = url
        return;
    }

    const editedForms = [];

    newGiftForm != giftForm ? editedForms.push("gift") : null;
    newRequestForm != requestForm ? editedForms.push("request") : null;
    if (element === 'requestList' || !element) {
        newOrganizationForm != organizationForm ? editedForms.push("organization") : null;
    }

    let confirmation = true;

    if (editedForms.length > 0) {
        confirmation = confirm(`You've changed the ${editedForms.map((item, i) => i == 0 ? item : " and " + item).join('')} ${editedForms.length > 1 ? "forms" : "form"}, would you like to ignore these changes?`);
    }

    if (confirmation) {
        if (element == "requestList") {
            location.href = url;
        } else if (element == "request") {
            request.loadRequestPartial(url);
        } else if (requestID != 0 || element == "gift") {
            loadGiftPartial(url, requestID);
        } else if (!element) {
            location.href = url
        }
    }
}

function loadGiftPartial(url, requestID) {
    $("#GiftDetail_RequestID").val(requestID);
    $.when(
        $.get(url, function (data) {
            $("#requests").html(data)
        })
    )
        .done(createStartDateFlat)
        .done(triggerBuildProgressBar)
        .done(setGiftForm)
        .done(makeGiftScheduleDataTable)
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            console.log(errorThrown);
            console.error("Error occurred: " + textStatus);
        });
}
function makeDateFlatpickr() {
    let d = $("#RequestDetail_Request_VoteDate").val() == "0001-01-01T00:00" ? new Date() : new Date($("#RequestDetail_Request_VoteDate").val());
    $("#RequestDetail_Request_VoteDate").flatpickr({
        defaultDate: d,
        allowInput: true,
        plugins: [
            monthSelectPlugin({
                dateFormat: "m/Y",
            })
        ],
    });
}
function loadRequestPartial(url) {
    $.when(
        $.get(url, function (data) {
            $("#requests").html(data)
        })
    )
        .done(makeDateFlatpickr)
        .done(setReqForm)
        .done(makeGiftDataTable)
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            console.log(errorThrown);
            console.error("Error occurred: " + textStatus);
        });

}
function deleteElement(url, id, formName, elementName) {
    $(`#${formName}_Delete${elementName}`).val(true);
    $(`#${formName}_${elementName}_${elementName}ID`).val(id);
    if (confirm(`Are you sure you want to delete this ${elementName}?`)) {
        if (elementName == "Address" || elementName == "Contact" || elementName == "FinancialInformation" || elementName == "Assessment")
            saveOrganization(url);
        else if (elementName == "Gift")
            saveRequest(url);
        else if (elementName == "Condition" || elementName == "GiftSchedule")
            saveGift(url);
        else {
            $(`#${formName}_Delete${elementName}`).val(true);
            $(`#${formName}_${elementName}_${elementName}ID`).val(id);
        }
    }
}
async function saveRequest(url) {
    $("#RequestDetail_SaveRequest").val(true);
    const requestID = $("#RequestDetail_Request_RequestID").val();
    $("#OrganizationDetails_RequestID").val(requestID);
    validationService.validateForm($("#requestDetailForm")[0], async function (e) {
        if (e == false) {
            return false;
        }

        const form = new FormData($("#requestDetailForm")[0]);
        let json = ""
        try {
            const response = await fetch(url, {
                method: "POST",
                body: form
            });
            json = await response.json();
            $("#OrganizationDetails_RequestID").val(json.requestID);
        } catch (e) {
            console.error(e);
        }
        await saveOrganization(`organization/saveorganizationdetails`);
    });
}
async function saveOrganization(url, reload = false) {
    validationService.validateForm($("#newOrganizationDetailsForm")[0], async function (e) {
        if (e == false)
            return false;

        $("#OrganizationDetails_Address_Active").val($("#OrganizationDetails_Address_Active").prop("checked"));
        const form = new FormData($("#newOrganizationDetailsForm")[0]);
        try {
            const response = await fetch(url, {
                method: "POST",
                body: form
            });
            const json = await response.json();

            if (json.organizationId < 1) {
                $("#error-message-p").html("Unable to save as it would overwrite other changes. Please reload the page and try again.")
                $(".error-message").modal("show");
                return;
            } else {
                window.location = json.url;
            }
        } catch (e) {
            console.error(e);
        }
    });
}

async function saveNewGift(url) {
    $("#RequestDetail_SaveGift").val(true);
    saveRequest(url);
    $("#addEditGiftModal").modal("hide");
}
async function saveGift(url, requestID, child = null, rebalance = false) {
    let stopSubmission = false;
    if (child == "giftSchedule") {
        if ($("#GiftDetail_GiftSchedule_Amount").val() < 0 || $("#GiftDetail_GiftSchedule_Amount").val().length == 0) {
            $("#GiftDetail_GiftSchedule_Amount").addClass('input-validation-error');
            $("#GiftDetail_GiftSchedule_Amount").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html("Must be greater than 0");
            stopSubmission = true;
        }
        if ($("#GiftDetail_GiftSchedule_DisbursementDate").val() == 0 || $("#GiftDetail_GiftSchedule_DisbursementDate").val().length == 0
            || !$("#GiftDetail_GiftSchedule_DisbursementDate").val()) {
            $("#GiftDetail_GiftSchedule_DisbursementDate").addClass('input-validation-error');
            $("#GiftDetail_GiftSchedule_DisbursementDate").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html("Disbursement Date is required");
            stopSubmission = true;
        }
        //const remainingCheck = Number($("#GiftDetail_GiftSchedule_Amount").val() - $("#GiftDetail_Gift_RemainingGiftAmount").val());
        //const remainingCheck = Number($("#GiftDetail_Gift_RemainingGiftAmount").val() - $("#GiftDetail_GiftSchedule_Amount").val()) < $("#GiftDetail_Gift_GiftAmount").val();
        //const remainingCheck = Number($("#GiftDetail_Gift_GiftAmount").val() $("#GiftDetail_GiftSchedule_Amount").val()$("#GiftDetail_Gift_RemainingGiftAmount").val() > );

        const newGiftScheduleCheck = Number($("#GiftDetail_GiftSchedule_Amount").val()) > Number($("#GiftDetail_Gift_RemainingGiftAmount").val());
        if ($("#GiftDetail_GiftSchedule_GiftScheduleID").val() == 0 && newGiftScheduleCheck) { // if this is a new giftSchedule
            $("#GiftDetail_GiftSchedule_Amount").addClass('input-validation-error');
            $("#GiftDetail_GiftSchedule_Amount").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html(`Unable to save, Gift Schedule Amount exceeds the remaining Gift Amount of $${$("#GiftDetail_Gift_RemainingGiftAmount").val()}`);
            stopSubmission = true;
        }

        if (!rebalance) {
            const existingGiftScheduleCheck = Number($("#GiftDetail_GiftSchedule_Amount").val()) - Number($("#GiftDetail_GiftSchedule_PreviousAmount").val()) <= Number($("#GiftDetail_Gift_RemainingGiftAmount").val());
            if (!existingGiftScheduleCheck) {
                $("#GiftDetail_GiftSchedule_Amount").addClass('input-validation-error');
                $("#GiftDetail_GiftSchedule_Amount").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html(`Unable to save, Gift Schedule Amount exceeds the remaining Gift Amount of $${$("#GiftDetail_Gift_RemainingGiftAmount").val()}`);
                stopSubmission = true;
            }
        } else {
            if (Number($("#GiftDetail_GiftSchedule_Amount").val()) > Number($("#GiftDetail_Gift_RemainingGiftAmount").val()) && Number($("#GiftDetail_GiftSchedule_Amount").val()) > Number($("#GiftDetail_Gift_GiftAmount").val())) {
                $("#GiftDetail_GiftSchedule_Amount").addClass('input-validation-error');
                $("#GiftDetail_GiftSchedule_Amount").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html(`Unable to save, Gift Schedule Amount exceeds the remaining Gift Amount of $${$("#GiftDetail_Gift_RemainingGiftAmount").val()}`);
                stopSubmission = true;
            }
        }
    }
    if (child == 'generate') {
        if ($("#GiftDetail_Gift_FrequencyType").val() == 0 && $("#GiftDetail_Gift_SelectedFrequency").val() > 24) { // months
            $("#GiftDetail_Gift_SelectedFrequency").addClass('input-validation-error');
            $("#GiftDetail_Gift_SelectedFrequency").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html("Selected Frequency cannot be more than 24 months");
            stopSubmission = true;
        } else if ($("#GiftDetail_Gift_FrequencyType").val() == 1 && $("#GiftDetail_Gift_SelectedFrequency").val() > 8) { // quarters
            $("#GiftDetail_Gift_SelectedFrequency").addClass('input-validation-error');
            $("#GiftDetail_Gift_SelectedFrequency").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html("Selected Frequency cannot be more than 8 quarters");
            stopSubmission = true;
        }
    }
    if ($("#GiftDetail_Gift_GiftAmount").val() == 0 || $("#GiftDetail_Gift_GiftAmount").val().length == 0) {
        $("#GiftDetail_Gift_GiftAmount").addClass('input-validation-error');
        $("#GiftDetail_Gift_GiftAmount").siblings(".field-validation-valid").removeClass("field-validation-valid").addClass("field-validation-error").html("Must be greater than 0");
        stopSubmission = true;
    }
    if (stopSubmission)
        return;
    $("#GiftDetail_SaveGiftSchedule").val(false);
    $("#GiftDetail_GenerateSchedules").val(false);

    $("#GiftDetail_Gift_RequestID").val(requestID);
    $("#OrganizationDetails_RequestID").val(requestID);

    const giftID = $("#GiftDetail_Gift_GiftID").val();
    $("#OrganizationDetails_GiftID").val(giftID);

    $("#GiftDetail_Gift_Conditional").val($("#GiftDetail_Gift_Conditional").prop("checked"));
    $("#GiftDetail_Gift_Recurring").val($("#GiftDetail_Gift_Recurring").prop("checked"));
    $("#GiftDetail_Gift_ConditionCompleted").val($("#GiftDetail_Gift_ConditionCompleted").prop("checked"));
    switch (child) {
        case "giftSchedule":
            $("#GiftDetail_SaveGiftSchedule").val(true);
            $("#GiftDetail_GiftSchedule_IncludePaymentInfo").val($("#GiftDetail_GiftSchedule_IncludePaymentInfo").prop("checked"));
            $("#addEditGiftScheduleModal").modal("hide");
            if (rebalance)
                $("#GiftDetail_Gift_Rebalance").val(true);

            break;
        case "generate":
            $("#GiftDetail_GenerateSchedules").val(true);
            $("#GenerateModal").modal("hide");
            break;
    }
    const form = new FormData($("#giftDetailForm")[0]);

    try {
        const response = await fetch(url, {
            method: "POST",
            body: form
        });
        const json = await response.json();
        $("#OrganizationDetails_GiftID").val(json.giftID);
        $("#OrganizationDetails_RequestID").val(json.requestID);
        await saveOrganization(`organization/saveorganizationdetails`);
    } catch (e) {
        console.error(e);
    }
}
function showGiftScheduleModal(data) {
    if (typeof data === 'object') {
        //populate modal with data
        $("#GiftDetail_GiftSchedule_GiftScheduleID").val(data.giftScheduleID);

        let a = data.disbursementDate.split(" ")[0].split("/");
        let [m, d, y] = a;
        let date = `${m}-0${d}-${y}`;
        $("#GiftDetail_GiftSchedule_DisbursementDate").flatpickr({
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
        let minDate = data.split(" ")[0].split("/");
        let [mm, dd, yy] = minDate;
        minDate = `${mm}-0${dd}-${yy}`;
        $("#GiftDetail_GiftSchedule_DisbursementDate").flatpickr({
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
function resetGiftSchedule() {
    $("#GiftDetail_GiftSchedule_GiftScheduleID").val(0);
    $("#GiftDetail_GiftSchedule_Amount").val(0);
    $("#GiftDetail_GiftSchedule_DisbursementDate").val(null)
    $("#GiftDetail_GiftSchedule_IncludePaymentInfo").val(false);
    $("#addEditGiftScheduleModal").modal("hide");
}
function showConditionModal(data) {
    if (data) {
        //populate modal with data
        $("#GiftDetail_Condition_ConditionID").val(data.conditionID);
        let a = data.deadline.split(" ")[0].split("/");
        let [m, d, y] = a;
        let date = `${m}-0${d}-${y}`;
        $("#GiftDetail_Condition_Deadline").flatpickr({
            defaultDate: new Date(date),
            allowInput: true,
            plugins: [
                monthSelectPlugin({
                    dateFormat: "m/Y",
                })
            ],
        });

        $("#GiftDetail_Condition_Description").val(data.description);
        let bool = data.completed == "False" ? false : true;
        $("#GiftDetail_Condition_Completed").prop("checked", bool);
        $("#GiftDetail_Condition_Completed").val(bool);
    } else {
        $("#GiftDetail_Condition_Deadline").flatpickr({
            defaultDate: new Date(),
            allowInput: true,
            minDate: new Date(minDate),
            plugins: [
                monthSelectPlugin({
                    dateFormat: "m/Y",
                })
            ],
        });
        $("#GiftDetail_Condition_ConditionID").val(0);
        $("#addEditConditionModal input").val("");
    }
    $("#addEditConditionModal").modal("show");
}
function toggleFrequency() {
    let recurring = $("#GiftDetail_Gift_Recurring").prop("checked");
    if (!recurring) {
        $("#GenerateGiftScheduleBtn").addClass("d-none");
    } else {
        $("#GenerateGiftScheduleBtn").removeClass("d-none");
    }
}
function toggleCondition() {
    if ($("#conditionInformation").hasClass("d-none")) {
        $("#conditionInformation").removeClass("d-none");
    } else {
        $("#conditionInformation").addClass("d-none");
    }
}
function toggleProjectTypeOther() {
    let other = $('#RequestDetail_Request_ProjectTypeID').find("option:selected").text();
    if (other == "Other") {
        $("#ProjectTypeOther").removeClass("d-none");
        return;
    }
    $("#RequestDetail_Request_ProjectTypeOther").val("");
    $("#ProjectTypeOther").addClass("d-none");
}
function showGenerateModal() {
    $("#GenerateModal").modal("show");
}
function createStartDateFlat() {
    $("#GiftDetail_Gift_GiftStartDate").flatpickr({
        defaultDate: new Date(),
        allowInput: true,
        minDate: new Date(minDate),
        plugins: [
            monthSelectPlugin({
                dateFormat: "m/Y",
            })
        ],
    });
    if ($("#GiftDetail_Gift_Conditional").val() == 'true') {
        let x = $("#GiftDetail_Gift_ConditionDeadline").val() == "0001-01-01T00:00" ? new Date() : new Date($("#GiftDetail_Gift_ConditionDeadline").val());
        $("#GiftDetail_Gift_ConditionDeadline").flatpickr({
            defaultDate: new Date(x),
            allowInput: true,
            minDate: new Date(minDate),
            plugins: [
                monthSelectPlugin({
                    dateFormat: "m/Y",
                })
            ],
        });
    }
}
function buildProgressBar(giftTotalAmount, remainingGiftAmount) {
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
function triggerBuildProgressBar() {
    const evt = new Event('build');
    window.dispatchEvent(evt);
    $("#progress-bar-container").trigger('load')
}

function makeGiftDataTable() {
    new DataTable('#GiftsTable', {
        dom: "tir",
        paging: false,
        columnDefs: [
            {
                targets: 0, searchable: false, orderable: false,
            },
        ],
        order: [1],
    });
}

function makeGiftScheduleDataTable() {
    $('#GiftSchedulesTable').DataTable({
        dom: "tir",
        paging: false,
        columnDefs: [
            {
                targets: 0, searchable: false, orderable: false,
            },
            {
                targets: 4,
                data: 'disbursementDate',
                render: function (data) {
                    return new Date(data).getTime();
                },
                visible: false,
            },
            {
                'orderData': [4], 'targets': [1]
            },
            {
                'targets': [4],
                'visible': false,
            },

        ],
        order: [1],
    });

}

export { loadGiftPartial, loadRequestPartial, saveRequest, saveOrganization, saveNewGift, saveGift, showGiftScheduleModal, showConditionModal, toggleFrequency, showGenerateModal, deleteElement, toggleProjectTypeOther, toggleCondition, buildProgressBar, /*checkFormState,*/ setOrgForm, resetGiftSchedule }