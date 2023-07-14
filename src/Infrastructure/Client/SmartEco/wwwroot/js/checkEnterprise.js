var errorGetEnterpriseId = "errorGetEnterprise";

$('#Item_Bin').bind('input', function (e) {
    BeforeGetEnterprise();
    var bin = $('#Item_Bin').val();
    if (bin.length === 12) {
        $.ajax({
            url: $(this).data('url'),
            data: {
                Bin: bin,
            },
            type: 'POST',
            success: function (data) {
                if (data.success) {
                    SetValues(data);
                }
                else {
                    SetError(data);
                }
            },
            error: function (data) {
                SetError(data);
            }
        })
    }
});

function BeforeGetEnterprise() {
    $('#' + errorGetEnterpriseId).remove();
    $('#Item_Name').val('');
    $('#Item_KatoId').val('');
    $('#Item_Kato').val('');
}

function SetValues(data) {
    $('#Item_Name').val(data.obj.name);
    $('#Item_KatoId').val(data.obj.katoId);
    $('#Item_Kato').val(data.obj.katoComplex);
}

function SetError(data) {
    var htmlError = '<span id="' + errorGetEnterpriseId + '" class="text-danger">' + data.description + '</span>'
    $('form:first').prepend(htmlError);
}