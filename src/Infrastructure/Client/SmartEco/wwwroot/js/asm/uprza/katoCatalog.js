//Set kato catalogs to datalist
function SetKatoes(input) {
    var value = input.val();
    var katoCatologsDataId = `#KatoCatologsDataList`;
    if (value.length < 2) {
        $(katoCatologsDataId).empty();
        return;
    }
    $.ajax({
        data: {
            katoCodeName: value
        },
        url: $('#GetKatoCatalogsReq').data('url'),
        type: 'GET',
        success: function (data) {
            $(katoCatologsDataId).empty();
            $.each(data, function (i) {
                $(katoCatologsDataId).append($('<option>').val(data[i].codeName));
            });
        }
    });
}

//Set id of kato to input
function SetKatoValue(input) {
    var value = input.val();
    var katoCodeInput = $('#KatoCode');
    var katoNameInput = $('#KatoName');
    $.ajax({
        data: {
            katoCodeName: value
        },
        url: $('#GetKatoCatalogReq').data('url'),
        type: 'GET',
        success: function (data) {
            if (data != null) {
                katoCodeInput.attr('value', data.code);
                katoNameInput.attr('value', data.name);
            }
            else {
                katoCodeInput.attr('value', '');
                katoNameInput.attr('value', '');
            }
        }
    });
}