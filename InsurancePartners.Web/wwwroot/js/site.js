$(function () {
    const $dismissibleAlerts = $('.alert-dismissible');

    if ($dismissibleAlerts.length > 0) {
        window.setTimeout(function () {
            $dismissibleAlerts.alert('close');
        }, 5000);
    }

    const $firstInvalidInput = $('.input-validation-error:first');

    if ($firstInvalidInput.length > 0) {
        $firstInvalidInput.trigger('focus');
    }

    const $highlightedRow = $('tr.table-warning:first');

    if ($highlightedRow.length > 0) {
        $highlightedRow[0].scrollIntoView({
            behavior: 'smooth',
            block: 'center'
        });
    }
});