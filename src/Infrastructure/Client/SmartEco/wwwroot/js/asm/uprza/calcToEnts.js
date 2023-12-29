//#region Calculation To Enterprises Tab
//Get list enterprises for page configure calculation
function GetEnterprisesByKato(input) {
    var value = input.val();
    var enterprisesDataId = `#EnterprisesDataList`;
    $.ajax({
        data: {
            enterpriseBinName: value,
            calcKatoCode: $('#CalculationKatoCode').val()
        },
        url: $('#GetEnterprisesByKatoReq').data('url'),
        type: 'GET',
        success: function (data) {
            $(enterprisesDataId).empty();
            $.each(data, function (i) {
                $(enterprisesDataId).append($('<option>')
                    .data('enterprise-id', data[i].id)
                    .val(`${data[i].name}. БИН: ${data[i].bin}`));
            });
        }
    });
}

//Set selected enterprise
function SetEnterprise(input) {
    var value = input.val();
    var enterpriseId = $('#EnterprisesDataList option').filter(function () {
        return this.value == value;
    }).data('enterprise-id');

    SetTitleValueEnterprise(enterpriseId);
}

//Select enterprise to calculation
function SelectEnterprise() {
    var calculationId = $('#CalculationId').val();
    var enterpriseId = $('.enterprise-name').data('enterprise-id');
    var calcToEnt = new CalculationToEnterprise(
        calculationId,
        enterpriseId
    );
    $.ajax({
        data: {
            calcToEnt: calcToEnt
        },
        url: $('#CalcToEntCreateReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#EnterprisesTable').empty();
            $('#EnterprisesTable').html(result);
        }
    });
    $('.enterprise-name').val('');
    SetTitleValueEnterprise();
}

function DeleteEnterprise(enterpriseId) {
    var calculationId = $('#CalculationId').val();
    var calcToEnt = new CalculationToEnterprise(
        calculationId,
        enterpriseId
    );
    $.ajax({
        data: {
            calcToEnt: calcToEnt
        },
        url: $('#CalcToEntDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#EnterprisesTable').empty();
            $('#EnterprisesTable').html(result);
        }
    });
}

function SetTitleValueEnterprise(enterpriseId) {
    var inputEnterprise = $('.enterprise-name');
    var value = inputEnterprise.val();
    inputEnterprise.prop('title', value);
    inputEnterprise.data('enterprise-id', enterpriseId);
    if (enterpriseId) {
        $('#SelectEnterpriseBtn').prop('disabled', false);
    }
    else {

        $('#SelectEnterpriseBtn').prop('disabled', true);
    }
}
//#endregion Calculation To Enterprises Tab