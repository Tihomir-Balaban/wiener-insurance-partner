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
});