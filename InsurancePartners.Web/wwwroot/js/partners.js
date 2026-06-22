$(function () {
    const $partnerDetailsModal = $('#partnerDetailsModal');
    const $partnerDetailsModalContent = $('#partnerDetailsModalContent');

    const $deletePartnerModal = $('#deletePartnerModal');
    const $deletePartnerForm = $('#deletePartnerForm');
    const $deletePartnerName = $('#deletePartnerName');

    const $addPolicyModal = $('#addPolicyModal');
    const $addPolicyForm = $('#addPolicyForm');
    const $addPolicyModalLabel = $('#addPolicyModalLabel');
    const $addPolicySubmitButton = $('#addPolicySubmitButton');
    const $addPolicyId = $('#addPolicyId');
    const $addPolicyPartnerId = $('#addPolicyPartnerId');
    const $addPolicyRowVersion = $('#addPolicyRowVersion');
    const $addPolicyPartnerName = $('#addPolicyPartnerName');
    const $addPolicyNumber = $('#addPolicyNumber');
    const $addPolicyAmount = $('#addPolicyAmount');
    const $addPolicyErrors = $('#addPolicyErrors');

    function showPartnerDetails(partnerId) {
        if (!partnerId) {
            return;
        }

        $partnerDetailsModalContent.html(`
            <div class="modal-body text-center p-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="sr-only">Loading...</span>
                </div>
                <div class="mt-3 text-muted">Loading partner details...</div>
            </div>
        `);

        $partnerDetailsModal.modal('show');

        $.get(`/Partners/Details/${partnerId}`)
            .done(function (html) {
                $partnerDetailsModalContent.html(html);
            })
            .fail(function () {
                $partnerDetailsModalContent.html(`
                    <div class="modal-header">
                        <h5 class="modal-title">Partner Details</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>

                    <div class="modal-body">
                        <div class="alert alert-danger mb-0">
                            Partner details could not be loaded.
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            Close
                        </button>
                    </div>
                `);
            });
    }

    function clearPolicyForm() {
        $addPolicyErrors.empty();
        $addPolicyId.val('');
        $addPolicyPartnerId.val('');
        $addPolicyRowVersion.val('');
        $addPolicyPartnerName.text('');
        $addPolicyNumber.val('');
        $addPolicyAmount.val('');
    }

    function prepareAddPolicyModal(partnerId, partnerName) {
        clearPolicyForm();

        $addPolicyForm.attr('action', '/Policies/Create');
        $addPolicyModalLabel.text('Add Policy');
        $addPolicySubmitButton.text('Save Policy');

        $addPolicyPartnerId.val(partnerId);
        $addPolicyPartnerName.text(partnerName || 'Selected partner');
    }

    function prepareEditPolicyModal(policy) {
        clearPolicyForm();

        $addPolicyForm.attr('action', `/Policies/Edit/${policy.id}`);
        $addPolicyModalLabel.text('Edit Policy');
        $addPolicySubmitButton.text('Save Changes');

        $addPolicyId.val(policy.id);
        $addPolicyPartnerId.val(policy.partnerId);
        $addPolicyRowVersion.val(policy.rowVersion);
        $addPolicyPartnerName.text(`Policy ${policy.policyNumber}`);
        $addPolicyNumber.val(policy.policyNumber);
        $addPolicyAmount.val(policy.policyAmount);
    }

    function showPolicyModal() {
        if ($partnerDetailsModal.hasClass('show')) {
            $partnerDetailsModal.one('hidden.bs.modal', function () {
                $addPolicyModal.modal('show');
            });

            $partnerDetailsModal.modal('hide');

            return;
        }

        $addPolicyModal.modal('show');
    }

    function showAddPolicyErrors(errors) {
        if (!errors || errors.length === 0) {
            $addPolicyErrors.html(`
                <div class="alert alert-danger">
                    Policy could not be saved.
                </div>
            `);

            return;
        }

        const errorItems = errors
            .map(function (error) {
                return `<li>${escapeHtml(error.message || 'Validation error')}</li>`;
            })
            .join('');

        $addPolicyErrors.html(`
            <div class="alert alert-danger">
                <ul class="mb-0">
                    ${errorItems}
                </ul>
            </div>
        `);
    }

    function updatePartnerPolicySummary(partnerId, policyCount, totalPolicyAmount, isMarked) {
        const $row = $(`tr[data-partner-id="${partnerId}"]`);

        if ($row.length === 0) {
            return;
        }

        $row.find('.partner-policy-count').text(policyCount);

        const formattedAmount = Number(totalPolicyAmount).toLocaleString(undefined, {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });

        $row.find('.partner-total-policy-amount').text(formattedAmount);

        const $fullName = $row.find('.partner-full-name');
        const currentName = $fullName.text().trim();
        const nameWithoutMarker = currentName.startsWith('* ')
            ? currentName.substring(2)
            : currentName;

        $fullName.text(isMarked ? `* ${nameWithoutMarker}` : nameWithoutMarker);
    }

    function escapeHtml(value) {
        return $('<div>').text(value).html();
    }

    $(document).on('click', '.js-partner-details', function (event) {
        event.preventDefault();
        event.stopPropagation();

        const partnerId = $(this).data('partner-id');

        showPartnerDetails(partnerId);
    });

    $(document).on('click', '.js-partner-row', function (event) {
        const clickedActionElement = $(event.target).closest('a, button, input, select, textarea, .btn');

        if (clickedActionElement.length > 0) {
            return;
        }

        const partnerId = $(this).data('partner-id');

        showPartnerDetails(partnerId);
    });

    $(document).on('click', '.js-delete-partner', function (event) {
        event.preventDefault();
        event.stopPropagation();

        const partnerId = $(this).data('partner-id');
        const partnerName = $(this).data('partner-name');

        if (!partnerId) {
            return;
        }

        $deletePartnerName.text(partnerName || 'Selected partner');
        $deletePartnerForm.attr('action', `/Partners/Delete/${partnerId}`);

        $deletePartnerModal.modal('show');
    });

    $(document).on('click', '.js-add-policy', function (event) {
        event.preventDefault();
        event.stopPropagation();

        const partnerId = $(this).data('partner-id');
        const partnerName = $(this).data('partner-name');

        if (!partnerId) {
            return;
        }

        prepareAddPolicyModal(partnerId, partnerName);
        showPolicyModal();
    });

    $(document).on('click', '.js-edit-policy', function (event) {
        event.preventDefault();
        event.stopPropagation();

        const policyId = $(this).data('policy-id');

        if (!policyId) {
            return;
        }

        $.get(`/Policies/Edit/${policyId}`)
            .done(function (policy) {
                prepareEditPolicyModal(policy);
                showPolicyModal();
            })
            .fail(function () {
                alert('Policy could not be loaded.');
            });
    });

    $addPolicyForm.on('submit', function (event) {
        event.preventDefault();

        const $form = $(this);
        const $submitButton = $form.find('button[type="submit"]');

        $addPolicyErrors.empty();
        $submitButton.prop('disabled', true);

        $.ajax({
            type: 'POST',
            url: $form.attr('action'),
            data: $form.serialize()
        })
            .done(function (response) {
                updatePartnerPolicySummary(
                    response.partnerId,
                    response.policyCount,
                    response.totalPolicyAmount,
                    response.isMarked
                );

                $addPolicyModal.modal('hide');
                clearPolicyForm();
            })
            .fail(function (xhr) {
                const response = xhr.responseJSON;
                showAddPolicyErrors(response ? response.errors : []);
            })
            .always(function () {
                $submitButton.prop('disabled', false);
            });
    });
});