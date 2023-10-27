//#region Row events

//Add new emission
function AddEmissionClick(btn) {
    var operationModeId = btn.closest('tr').prev().find('[name="IdMode"]').val();
    $.ajax({
        data: {
            OperationModeId: operationModeId
        },
        url: $('#EmissionCreateReq').data('url'),
        type: 'POST',
        success: function (result) {
            var airEmissionsTableId = `#AirEmissions_${operationModeId}`;
            $(airEmissionsTableId).empty();
            $(airEmissionsTableId).html(result);
        }
    });
};


//Set pollutions to datalist
function SetPollutions(input) {
    var value = input.val();
    var editRow = GetEditRow(input);
    var emissionId = editRow.find('[name="IdEmission"]').val();
    $.ajax({
        data: {
            pollutantCodeName: value
        },
        url: $('#EmissionGetPollutantsReq').data('url'),
        type: 'GET',
        success: function (data) {
            var airPollutantsDataId = `#AirPollutantsDataList_${emissionId}`;
            $(airPollutantsDataId).empty();
            $.each(data, function (i) {
                $(airPollutantsDataId).append($('<option>').val(data[i].codeName));
            });
        }
    });
}

//Set id of pollution to input
function SetPollutionValue(input) {
    var value = input.val();
    var editRow = GetEditRow(input);
    var pollutantEmissionInput = editRow.find('[name="PollutantEmission"]');
    $.ajax({
        data: {
            pollutantCodeName: value
        },
        url: $('#EmissionGetPollutantIdReq').data('url'),
        type: 'GET',
        success: function (data) {
            if (data != null) {
                pollutantEmissionInput.val(data.id);
            }
            else {
                pollutantEmissionInput.val('');
            }
        }
    });
}

//Edit emission
function EditEmissionClick(btn) {
    var editRow = GetEditRow(btn);
    var tdInputs = editRow.find('input');

    editRow.find('.select-text').removeClass('d-inline-block').addClass('d-none');
    $(tdInputs).each(function (index) {
        var currentValue = $(this).val();
        SetCurrentValueToData(this, currentValue);
    });

    EditableEmissionButtonsShow(btn, editRow, true);
};

//Cancel changes
function CancelEmissionClick(btn) {
    var editRow = GetEditRow(btn);
    var tdInputs = editRow.find('input');

    editRow.find('.select-text').removeClass('d-none').addClass('d-inline-block');
    $(tdInputs).each(function (index) {
        var currentValue = $(this).data('current-value');
        SetCurrentValue(this, currentValue);
    });

    editRow.find('.invalid-feedback').removeClass('d-inline-block').text('');
    EditableEmissionButtonsShow(btn, editRow);
};

//Save emission
function SaveEmissionClick(btn) {
    var editRow = GetEditRow(btn);
    var dataEmission = CreateEmission(btn);
    editRow.find('.invalid-feedback').removeClass('d-inline-block').text('');
    SetTitlePollutant(editRow);

    $.ajax({
        data: dataEmission,
        url: $('#EmissionEditReq').data('url'),
        type: 'POST',
        success: function (result) {
            SetDisabledFields(editRow);
            EditableEmissionButtonsShow(btn, editRow);

            editRow.find('.select-text').removeClass('d-none').addClass('d-inline-block');
            editRow.find('select').removeClass('d-inline-block').addClass('d-none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var status = textStatus;
            var error = $.parseJSON(jqXHR.responseText);
            ValidEmission(editRow, error)
        }
    });
};

//Delete emission
function DeleteEmissionClick(btn) {
    var operationModeId = btn.data('modeid');
    var emissionId = btn.val();

    $(`#DeleteAirEmission_${emissionId} .close-btn`).click()
    $.ajax({
        data: {
            Id: emissionId,
            OperationModeId: operationModeId,
        },
        url: $('#EmissionDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            var airEmissionsTableId = `#AirEmissions_${operationModeId}`;
            $(airEmissionsTableId).empty();
            $(airEmissionsTableId).html(result);
        }
    });
};

//#endregion Row events

function EditableEmissionButtonsShow(btn, editRow, isEditBtnClick) {
    var editBtn = editRow.find('.edit-emission-btn');
    var saveBtn = editRow.find('.save-emission-btn');
    var cancelBtn = editRow.find('.cancel-emission-btn');

    ShowHideButtons(editRow, editBtn, saveBtn, cancelBtn, isEditBtnClick);
}

function SetTitlePollutant(editRow) {
    var inputPollutant = editRow.find('.pollutant-name');
    var value = inputPollutant.val();
    inputPollutant.prop('title', value);
}

function CreateEmission(btn) {
    var editRow = GetEditRow(btn);
    var dataEmission = new Emission();

    dataEmission.Id = editRow.find('[name="IdEmission"]').val();
    dataEmission.PollutantId = editRow.find('[name="PollutantEmission"]').val();
    dataEmission.MaxGramSec = editRow.find('[name="MaxGramSecEmission"]').val();
    dataEmission.MaxMilligramMeter = editRow.find('[name="MaxMilligramMeterEmission"]').val();
    dataEmission.GrossTonYear = editRow.find('[name="GrossTonYearEmission"]').val();
    dataEmission.SettlingCoef = editRow.find('[name="SettlingCoefEmission"]').val();
    dataEmission.EnteredDate = editRow.find('[name="EnteredDateEmission"]').val();
    dataEmission.OperationModeId = editRow.closest('.emissions-row').prev().find('[name="IdMode"]').val();

    return dataEmission;
}

function ValidEmission(editRow, error) {
    if (error.PollutantId) {
        editRow.find('.pollutant-invalid').addClass('d-inline-block').text(error.PollutantId[0]);
    }
    if (error.MaxGramSec) {
        editRow.find('.maxgramsec-invalid').addClass('d-inline-block').text(error.MaxGramSec[0]);
    }
    if (error.MaxMilligramMeter) {
        editRow.find('.maxmilligrammeter-invalid').addClass('d-inline-block').text(error.MaxMilligramMeter[0]);
    }
    if (error.GrossTonYear) {
        editRow.find('.grosstonyear-invalid').addClass('d-inline-block').text(error.GrossTonYear[0]);
    }
    if (error.SettlingCoef) {
        editRow.find('.settlingcoef-invalid').addClass('d-inline-block').text(error.SettlingCoef[0]);
    }
}