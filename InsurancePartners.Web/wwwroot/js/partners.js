$(function () {
    const $partnerDetailsModal = $('#partnerDetailsModal');
    const $partnerDetailsModalContent = $('#partnerDetailsModalContent');

    const $deletePartnerModal = $('#deletePartnerModal');
    const $deletePartnerForm = $('#deletePartnerForm');
    const $deletePartnerName = $('#deletePartnerName');

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
});