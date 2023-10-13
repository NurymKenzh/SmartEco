//#region Row events

//Add new mode
$('.emissions-btn').click(function () {
    $(this).toggleClass('down');
    $(this).parents('tr').next('.emissions-row').toggle('slow');
});

function AddModeClick(btn) {
    var sourceId = btn.closest('tr').prev().find('[name="IdSource"]').val();
    $.ajax({
        data: {
            SourceId: sourceId
        },
        url: $('#ModeCreateReq').data('url'),
        type: 'POST',
        success: function (result) {
            var operationModeTableId = `#OperationModes_${sourceId}`;
            $(operationModeTableId).empty();
            $(operationModeTableId).html(result);
        }
    });
};

//Edit mode
function EditModeClick(btn) {
    var editRow = GetEditRow(btn);
    var tdInputs = editRow.find('input');

    editRow.find('.select-text').removeClass('d-inline-block').addClass('d-none');
    $(tdInputs).each(function (index) {
        var currentValue = $(this).val();
        SetCurrentValueToData(this, currentValue);
    });

    EditableModeButtonsShow(btn, editRow, true);
};

//Cancel changes
function CancelModeClick(btn) {
    var editRow = GetEditRow(btn);
    var tdInputs = editRow.find('input');

    editRow.find('.select-text').removeClass('d-none').addClass('d-inline-block');
    $(tdInputs).each(function (index) {
        var currentValue = $(this).data('current-value');
        SetCurrentValue(this, currentValue);
    });

    editRow.find('.invalid-feedback').removeClass('d-inline-block').text('');
    EditableModeButtonsShow(btn, editRow);
};

//Save mode
function SaveModeClick(btn) {
    var editRow = GetEditRow(btn);
    var dataMode = CreateMode(btn);
    editRow.find('.invalid-feedback').removeClass('d-inline-block').text('');

    $.ajax({
        data: dataMode,
        url: $('#ModeEditReq').data('url'),
        type: 'POST',
        success: function (result) {
            SetDisabledFields(editRow);
            EditableModeButtonsShow(btn, editRow);

            editRow.find('.select-text').removeClass('d-none').addClass('d-inline-block');
            editRow.find('select').removeClass('d-inline-block').addClass('d-none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var status = textStatus;
            var error = $.parseJSON(jqXHR.responseText);
            ValidModeMixture(editRow, error)
        }
    });
};

//Delete mode
function DeleteModeClick(btn) {
    var sourceId = btn.data('sourceid');
    var operationModeId = btn.val();

    $(`#DeleteOperationMode_${operationModeId} .close`).click();
    $.ajax({
        data: {
            Id: operationModeId,
            SourceId: sourceId,
        },
        url: $('#ModeDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            var operationModeTableId = `#OperationModes_${sourceId}`;
            $(operationModeTableId).empty();
            $(operationModeTableId).html(result);
        }
    });
};

//#endregion Row events

function EditableModeButtonsShow(btn, editRow, isEditBtnClick) {
    var editBtn = editRow.find('.edit-mode-btn');
    var saveBtn = editRow.find('.save-mode-btn');
    var cancelBtn = editRow.find('.cancel-mode-btn');

    ShowHideButtons(editRow, editBtn, saveBtn, cancelBtn, isEditBtnClick);
}

function CreateMode(btn) {
    var editRow = GetEditRow(btn);
    var dataMode = new OperationMode();

    dataMode.Id = editRow.find('[name="IdMode"]').val();
    dataMode.Name = editRow.find('[name="NameMode"]').val();
    dataMode.WorkedTime = editRow.find('[name="WorkedTimeMode"]').val();
    dataMode.SourceId = editRow.closest('.operation-modes-row').prev().find('[name="IdSource"]').val();

    var editRowMixture = $('#ShowGasAirMixture_' + dataMode.Id).find('dl');
    dataMode.GasAirMixture = CreateMixture(editRowMixture);

    return dataMode;
}

function CreateMixture(editRow) {
    var dataMixture = new GasAirMixture();
    dataMixture.OperationModeId = editRow.find('[name="OperationModeIdMixture"]').val();
    dataMixture.Temperature = editRow.find('[name="TemperatureMixture"]').val();
    dataMixture.Pressure = editRow.find('[name="PressureMixture"]').val();
    dataMixture.Speed = editRow.find('[name="SpeedMixture"]').val();
    dataMixture.Volume = editRow.find('[name="VolumeMixture"]').val();
    dataMixture.Humidity = editRow.find('[name="HumidityMixture"]').val();
    dataMixture.Density = editRow.find('[name="DensityMixture"]').val();
    dataMixture.ThermalPower = editRow.find('[name="ThermalPowerMixture"]').val();
    dataMixture.PartRadiation = editRow.find('[name="PartRadiationMixture"]').val();
    return dataMixture;
}

function ValidModeMixture(editRow, error) {
    if (error.Name) {
        editRow.find('.name-invalid').addClass('d-inline-block').text(error.Name[0]);
    }
    if (error.WorkedTime) {
        editRow.find('.workedtime-invalid').addClass('d-inline-block').text(error.WorkedTime[0]);
    }

    //Check valid mixture
    if (error['GasAirMixture.Temperature']) {
        SetInvalidParameter(editRow, "TemperatureInvalid", error['GasAirMixture.Temperature'][0]);
    }
    if (error['GasAirMixture.Pressure']) {
        SetInvalidParameter(editRow, "PressureInvalid", error['GasAirMixture.Pressure'][0]);
    }
    if (error['GasAirMixture.Speed']) {
        SetInvalidParameter(editRow, "SpeedInvalid", error['GasAirMixture.Speed'][0]);
    }
    if (error['GasAirMixture.Volume']) {
        SetInvalidParameter(editRow, "VolumeInvalid", error['GasAirMixture.Volume'][0]);
    }
    if (error['GasAirMixture.Humidity']) {
        SetInvalidParameter(editRow, "HumidityInvalid", error['GasAirMixture.Humidity'][0]);
    }
    if (error['GasAirMixture.Density']) {
        SetInvalidParameter(editRow, "DensityInvalid", error['GasAirMixture.Density'][0]);
    }
    if (error['GasAirMixture.ThermalPower']) {
        SetInvalidParameter(editRow, "ThermalPowerInvalid", error['GasAirMixture.ThermalPower'][0]);
    }
    if (error['GasAirMixture.PartRadiation']) {
        SetInvalidParameter(editRow, "PartRadiationInvalid", error['GasAirMixture.PartRadiation'][0]);
    }
}