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
    $('#Item_Kato_Code').val('');
    $('#Item_Kato_Address').val('');
    $('#Item_Kato_ComplexName').val('');
}

function SetValues(data) {
    $('#Item_Name').val(data.obj.name);
    $('#Item_Kato_Code').val(data.obj.katoCode);
    $('#Item_Kato_Address').val(data.obj.katoAddress);
    $('#Item_Kato_ComplexName').val(data.obj.katoComplex);
}

function SetError(data) {
    var htmlError = '<span id="' + errorGetEnterpriseId + '" class="text-danger">' + data.description + '</span>'
    $('form:first').prepend(htmlError);
}