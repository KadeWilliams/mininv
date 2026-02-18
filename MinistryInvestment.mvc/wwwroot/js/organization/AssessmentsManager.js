import { api } from '../services/api.js'
import '../jquery.import.js';
import showToast from '../services/toast.js';
import { formStateManager } from '../services/FormStateManager.js';
export class AssessmentManager {
    constructor(containerSelector, organizationId) {
        this.container = $(containerSelector);
        this.organizationId = organizationId;
        this.dataTable = null;
        this.currentAssessmentId = null;
        formStateManager.registerCustomCapturer('assessment', () => {
            return formStateManager.captureAssessmentState();
        })
        this.setupEventHandlers();
    }

    setupEventHandlers() {

        $(document).on('click', '#addAssessmentBtn', () => {
            if (!formStateManager.confirmNavigation(true)) {
                return;
            }
            this.loadAssessmentForm(0); // 0 = new assessment
        });

        this.container.on('click', '.edit-assessment', (e) => {
            const assessmentId = $(e.currentTarget).data('assessmentid');
            this.loadAssessmentForm(assessmentId);
        });

        this.container.on('click', '.delete-assessment', async (e) => {
            const assessmentId = $(e.currentTarget).data('assessmentid');
            this.deleteAssessment(assessmentId);
        });

        $(document).on('click', '#saveAssessmentBtn', async () => {
            $("#blockApplicationDialog").modal('show');
            await this.saveAssessment();
        });

        $(document).on('click', '#backToAssessmentList', async () => {
            if (!formStateManager.confirmNavigation(true)) {
                return;
            }
            await this.load();
        });

        $(document).on('input', '.score-input', () => {
            this.updateScoreSummary();
        });
    }

    async load() {
        try {
            const assessments = await api.getAssessments(this.organizationId);
            this.renderTable(assessments);
            formStateManager.clearSpecificState('assessment');
        } catch (error) {
            console.error('Failed to load assessments: ', error);
            alert('Failed to load assessments');
        }
    }

    renderTable(assessments) {
        this.showTableView();

        if (this.dataTable) {
            this.dataTable.destroy();
        }

        this.dataTable = new DataTable('#AssessmentsTable', {
            destroy: true,
            responsive: false,
            autoWidth: false,
            data: assessments,
            paging: true,
            columns: this.getColumns(),
            columnDefs: [
                { targets: 0, searchable: false, orderable: false, },
            ],
            order: [[1, 'desc']],
        });
    }

    getColumns() {
        return [
            {
                data: null,
                render: (data) => this.renderActionButtons(data)
            },
            {
                data: 'voteDate',
                render: (data) => {
                    if (!data) return '';
                    const date = new Date(data);
                    return `${date.getMonth() + 1}/${date.getFullYear()}`;
                }
            },
            {
                data: 'score',
            },
            { data: 'lastChangeUser' }
        ]
    }

    renderActionButtons(assessment) {
        return `
            <button class="btn btn-primary edit-assessment" data-assessmentid="${assessment.assessmentID}" data-votedate="${assessment.voteDate}">
            <i class="fa fa-pencil"></i>
            </button>
            <button class="btn btn-danger delete-assessment" data-assessmentid="${assessment.assessmentID}">
                <i class="fa fa-trash"></i>
            </button>
        `
    }

    async loadAssessmentForm(assessmentId) {
        try {
            let assessmentDetail;

            if (assessmentId === 0) {
                assessmentDetail = await api.newAssessmentForm(this.organizationId);
            } else {
                assessmentDetail = await api.getAssessmentDetail(assessmentId, this.organizationId);
            }

            this.currentAssessmentId = assessmentId;
            this.renderAssessmentForm(assessmentDetail);
            setTimeout(() => {
                formStateManager.captureFormState('assessment');
            }, 100)
        } catch (error) {
            console.error('Failed to load assessment form: ', error);
            alert('Failed to load assessment form');
        }
    }

    renderAssessmentForm(detail) {
        this.showFormView();

        const groupsHtml = detail.headerGroups.map(group => `
            <div class="card my-2 p-0">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">${group.header}</h5>
                    <span class="badge bg-warning text-light" id="headerScore-${group.header}">
                        ${group.headerScore ? group.headerScore : 'No scores'}
                    </span>
                </div>
                <div class="card-body">
                    <table class="table table-hover">
                        <thead>
                            <tr>
                                <th>Question</th>
                                <th style="width: 60px;">Score</th>
                                <th>Notes</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${group.questions.map((q, index) =>
            `
                                <tr>
                                    <td>
                                        <strong>${q.category}</strong>
                                        <p class="text-muted mb-0 small">${q.description}</p>
                                    </td>
                                    <td style="min-width: 100px;">
                                    <div class="radio-group">
                                      <label>
                                        <input type="radio" class="score-input" name="score_${q.assessmentCategoryID}" data-categoryid="${q.assessmentCategoryID}" data-weight="${q.weight * 0}" data-header="${group.header}" value="${q.weight * 0}" ${q.score === q.weight * 0 ? 'checked' : ''}> Poor
                                      </label>
                                      <label>
                                        <input type="radio" class="score-input" name="score_${q.assessmentCategoryID}" data-categoryid="${q.assessmentCategoryID}" data-weight="${q.weight / 2}" data-header="${group.header}" value="${q.weight / 2}" ${q.score === q.weight / 2 ? 'checked' : ''}> Good 
                                      </label>
                                      <label>
                                        <input type="radio" class="score-input" name="score_${q.assessmentCategoryID}" data-categoryid="${q.assessmentCategoryID}" data-weight="${q.weight}" data-header="${group.header}" value="${q.weight}" ${q.score === q.weight ? 'checked' : ''}> Excellent 
                                      </label>
                                    </div>

                                    ${/*

                                        <select class="form-select score-input"
                                            data-categoryid="${q.assessmentCategoryID}"
                                            data-weight="${q.weight}"
                                            data-header="${group.header}">
                                            <option value="${q.weight * 0}" ${q.score === q.weight * 0 ? 'selected' : ''}>0</option>
                                            <option value="${q.weight / 2}" ${q.score === q.weight / 2 ? 'selected' : ''}>${q.weight / 2}</option>
                                            <option value="${q.weight}" ${q.score === q.weight ? 'selected' : ''}>${q.weight}</option>
                                        </select>
                                    <label for="amount-${q.category}-${q.weight * 0}">Poor</label>
                                    <input type="radio" id="amount-${q.category}-${q.weight * 0}" name="amount-${q.category}-${q.weight}" value="${q.weight * 0}"/>

                                    <label for="amount-${q.category}-${q.weight / 2}">Good</label>
                                    <input type="radio" id="amount-${q.category}-${q.weight / 2}" name="amount-${q.category}-${q.weight / 2}" value="${q.weight / 2}" checked/>

                                    <label for="amount-${q.category}-${q.weight}">Excellent</label>
                                    <input type="radio" id="amount-${q.category}-${q.weight}" name="amount-${q.category}-${q.weight}" value="${q.weight}" />
                                        */
            ''
            }
                                    </td>
                                    <td>
                                        <textarea
                                            class="form-control notes-input"
                                            data-categoryid="${q.assessmentCategoryID}"
                                            placeholder="Optional notes..."
                                            rows="3"
                                        >${q.notes || ''}</textarea>
                                    </td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                </div>
            </div>
        `).join('');

        const requestsHtml = detail.requests.map(r => {
            const date = new Date(r.voteDate)
            return `<option value="${r.requestID}">${date.getMonth() + 1}/${date.getFullYear()}</option>`
        });


        const isEditMode = this.currentAssessmentId !== 0;
        const selectedRequestId = detail.selectedRequestID || (detail.requests.length > 0 ? detail.requests[0].requestID : '');

        const formHtml = `
            <div class="d-flex justify-content-between align-items-center mb-3">
                <div>
                    <button class="btn btn-link p-0" id="backToAssessmentList">
                        <i class="fa fa-arrow-left"></i> Back to Assessments
                    </button>
                </div>
                <div class="d-flex align-items-center gap-3">
                    <div id="scoreSummary" class="text-end">
                        <button class="btn btn-primary" id="saveAssessmentBtn">
                            <i class="fa fa-save"></i> Save Assessment
                        </button>
                    </div>
                </div>
            </div>
            <div class="row m-3">
                <div class="col-md-4 p-0">
                    <label class="form-label">Request Vote Date</label>
                    <select class="form-select date-input" id="requestDate" >
                        ${isEditMode ? `<option value="${selectedRequestId}" selected>${this.getVoteDateDisplay(detail.requests, selectedRequestId)}</option>` : requestsHtml}
                    </select>
                    ${isEditMode ? `<input type="hidden" id="requestDateHidden" value="${selectedRequestId}">` : ''}
                </div>
            </div>
            <div class="row m-3 p-0">
                <div class="col-md-5 p-0">
                    <label class="form-label">Assessment Note</label>
                    <textarea id="assessmentNote" class="form-control" rows="4">${detail.notes || ''}</textarea
                </div>
            </div>
            ${groupsHtml}
            <div class="d-flex align-items-center gap-3 justify-content-end">
                <button class="btn btn-primary" id="saveAssessmentBtn">
                    <i class="fa fa-save"></i> Save Assessment
                </button>
            </div>
        `;

        $('#assessment-form-container').html(formHtml);

        this.updateScoreSummary();
    }

    getVoteDateDisplay(requests, requestId) {
        const request = requests.find(r => r.requestID === requestId);
        if (request) {
            const date = new Date(request.voteDate);
            return `${date.getMonth() + 1}/${date.getFullYear()}`;
        }
        return '';
    }

    updateScoreSummary() {
        const allInputs = $('.score-input:checked');
        let answered = 0;
        let totalWeight = 0;
        let earnedPoints = 0;

        const headerScores = {};

        allInputs.each(function () {
            const score = parseFloat($(this).val()) || 0;
            const weight = parseInt($(this).data('weight') || 0);
            const header = $(this).data('header');

            if (score >= 0) {
                answered++;
                earnedPoints += score;
                totalWeight += weight;

                if (!headerScores[header]) {
                    headerScores[header] = { earned: 0, weight: 0 };
                }

                headerScores[header].earned += score;
                headerScores[header].weight += weight;
            }
        });

        const overallScore = earnedPoints;

        $('#answeredCount').text(answered);
        $('#overallScore').text(overallScore);

        for (const [header, scores] of Object.entries(headerScores)) {
            const headerScore = scores.weight > 0
                ? (scores.earned)
                : 'No scores';
            $(`#headerScore-${header}`).text(headerScore);
        }
    }

    async saveAssessment() {
        const scores = [];
        $('.score-input').each(function () {
            const categoryId = $(this).data('categoryid');
            const score = parseFloat($(`input.score-input[data-categoryid=${categoryId}]:checked`).val()) || 0;
            const notes = $(`textarea.notes-input[data-categoryid=${categoryId}]`).val() || '';

            scores.push({
                AssessmentCategoryID: Number(categoryId),
                Score: Number(score),
                Notes: notes
            });
        });

        const requestID = $('#requestDate').val();
        if (!requestID) {
            alert('Please select an request vote date');
            return;
        }
        const notes = $('#assessmentNote').val();

        $('#assessmentID').val(this.currentAssessmentId);
        $('#assessmentOrganizationID').val(this.organizationId);
        $('#assessmentRequestID').val(requestID);
        $('#assessmentNotes').val(notes);

        const scoresContainer = $('#scoresContainer');
        scoresContainer.empty();

        scores.forEach((score, index) => {
            scoresContainer.append(`
                <input type="hidden" name="Scores[${index}].AssessmentCategoryID" value="${score.AssessmentCategoryID}"/>
                <input type="hidden" name="Scores[${index}].Score" value="${score.Score}"/>
                <input type="hidden" name="Scores[${index}].Notes" value="${score.Notes}"/>
            `)

        })

        try {
            const form = $('#assessmentForm')[0];
            const formData = new FormData(form);
            const result = await api.saveAssessment(formData);

            if (result.assessmentID) {
                this.currentAssessmentId = result.assessmentID;
            }
            if (!result.success) {
                throw new Error(result.error);
            }

            formStateManager.resetFormState('assessment');

            //alert('Assessment saved successfully');
            showToast('Your changes have been saved!')
        } catch (error) {
            console.error('Failed to save assessment: ', error);
            showToast(error, 'error');
        }
        finally {
            $("#blockApplicationDialog").modal('hide');
        }
    }

    async deleteAssessment(assessmentId) {
        if (!confirm('Are you sure you want to delete this assessment?')) {
            return;
        }

        try {
            await api.deleteAssessment(assessmentId);
            await this.load();
        } catch (error) {
            console.error('Failed to delete assessment: ', error);
            alert('Failed to delete assessment');
        }
    }

    showTableView() {
        $('#assessment-table-view').show();
        $('#assessment-form-view').hide();
    }

    showFormView() {
        $('#assessment-table-view').hide();
        $('#assessment-form-view').show();
    }
}
