//Change value toggle checkbox by click button
//Because there is a library 'bug': when rebooting 'PartialView', an event 'onchange' is added every time
//Add 'setting' prefix to separate from pollution sources

$('.button-checkbox-toggle-setting').on("click", function () {
    var editDiv = $(this).closest('div');
    var checkboxToggle = editDiv.find('.checkbox-toggle');
    var valueChanged = checkboxToggle.val() == 'true' ? false : true;
    if (valueChanged) {
        checkboxToggle.bootstrapToggle('toggle');
        checkboxToggle.val(valueChanged);
    }
    else {
        checkboxToggle.bootstrapToggle('toggle');
        checkboxToggle.val(valueChanged);
    }
});


//Initializing blocks by modes
$(function () {
    IsUsePollutantsListChange(document.getElementById("IsUsePollutantsList"));
    InitializeMultipleSelectList();
    ChangeWindSpeedBlocks(document.getElementById("WindSpeedMode")); 
    ChangeWindDirectionBlocks(document.getElementById("WindDirectionMode"));
});

function InitializeMultipleSelectList() {
    $('#AirPollutantsSelectList').multiselect({
        nonSelectedText: 'Выберете вещества...',
        includeSelectAllOption: true,
        selectAllText: 'Выбрать все',
        allSelectedText: 'Выбраны все вещества',
        nSelectedText: 'выбрано',
        numberDisplayed: 1,
        maxHeight: 200,
        //buttonWidth: '55%',
        buttonTextAlignment: 'center'
    });
}

function ChangeWindSpeedBlocks(selectModeObject) {
    var value = selectModeObject.value;
    var windSpeedBlocks = {
        fixed: $('#WindSpeedModeFixedBlock'),
        iteratingSetNumbers: $('#WindSpeedModeIteratingSetNumbersBlock'),
        iteratingByStep: $('#WindSpeedModeIteratingByStepBlock')
    };
    $.each(windSpeedBlocks, function (index, value) {
        $(this).prop('hidden', true);
    });

    if (CalcWindModes.Fixed == value) {
        windSpeedBlocks.fixed.prop('hidden', false);
    }
    if (CalcWindModes.IteratingSetNumbers == value) {
        windSpeedBlocks.iteratingSetNumbers.prop('hidden', false);
    }
    if (CalcWindModes.IteratingByStep == value) {
        windSpeedBlocks.iteratingByStep.prop('hidden', false);
    }
}

function ChangeWindDirectionBlocks(selectModeObject) {
    var value = selectModeObject.value;
    var windDirectionBlocks = {
        fixed: $('#WindDirectionModeFixedBlock'),
        iteratingSetNumbers: $('#WindDirectionModeIteratingSetNumbersBlock'),
        iteratingByStep: $('#WindDirectionModeIteratingByStepBlock')
    };
    $.each(windDirectionBlocks, function (index, value) {
        $(this).prop('hidden', true);
    });

    if (CalcWindModes.Fixed == value) {
        windDirectionBlocks.fixed.prop('hidden', false);
    }
    if (CalcWindModes.IteratingSetNumbers == value) {
        windDirectionBlocks.iteratingSetNumbers.prop('hidden', false);
    }
    if (CalcWindModes.IteratingByStep == value) {
        windDirectionBlocks.iteratingByStep.prop('hidden', false);
    }
}

//List pollutants
function IsUsePollutantsListChange(object) {
    var isUsePollutantsVal = $(object).prop('checked');
    if (isUsePollutantsVal) {
        $('#AirPollutantsSelectListBlock').prop('hidden', false);
    }
    else {
        $('#AirPollutantsSelectListBlock').prop('hidden', true);
    }
}

//Updating pollutants, when changed selected source in tab
function UpdatePollutants() {
    var dataFilter = CreateSourceFilter();
    GetAirPollutants(dataFilter);
}

function GetAirPollutants(dataFilter) {
    $.ajax({
        data: {
            calculationId: dataFilter.CalculationId,
            enterpriseIds: dataFilter.EnterpriseIds,
            airPollutionsSelected: $('#AirPollutantsSelectList').val()
        },
        url: $('#GetAirPollutants').data('url'),
        type: 'GET',
        traditional: true, //for send array
        success: function (result) {
            $('#AirPollutantsSelectListBlock').empty();
            $('#AirPollutantsSelectListBlock').html(result);
            InitializeMultipleSelectList();
        }
    });
}

//Create or update settings
function SetCalculationSetting() {
    var calcSetting = CreateCalculationSettingObj();
    $.ajax({
        data: {
            calcSetting: calcSetting,
            calcSetting2: calcSetting
        },
        url: $('#SetCalculationSettingReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#ErrorSettingValid')
                .text('')
                .prop('hidden', true);

            RunCalculation();
        },
        error: function (xhr, status, error) {
            $('#ErrorSettingValid')
                .text(`Ошибка: ${xhr.responseText}`)
                .prop('hidden', false);
        }
    });
}

//Run calculation
function RunCalculation() {
    var calculationId = $('#CalculationId').val();
    $.ajax({
        data: {
            calculationId: calculationId
        },
        url: $('#RunCalculationReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#ErrorSettingValid')
                .text('')
                .prop('hidden', true);

            GetStateCalc();
        },
        error: function (xhr, status, error) {
            $('#ErrorSettingValid')
                .text(`Ошибка: ${xhr.responseText}`)
                .prop('hidden', false);
        }
    });
}

function CreateCalculationSettingObj() {
    var calculationId = $('#CalculationId').val();
    var calcSetting = new CalculationSetting(calculationId);
    calcSetting.WindSpeedSetting = CreateWindSpeedSettingObj();
    calcSetting.WindDirectionSetting = CreateWindDirectionSettingObj();

    calcSetting.ContributorCount = $('#ContributorCount').val();
    calcSetting.ThresholdPdk = $('#ThresholdPdk').val();
    calcSetting.Season = $('#Season').val();

    calcSetting.IsUseSummationGroups = $('#IsUseSummationGroups').prop('checked');
    calcSetting.IsUseBackground = $('#IsUseBackground').prop('checked');
    calcSetting.IsUseBuilding = $('#IsUseBuilding').prop('checked');

    calcSetting.UnitBorderStep = $('#UnitBorderStep').val();
    calcSetting.SanitaryAreaBorderStep = $('#SanitaryAreaBorderStep').val();
    calcSetting.LivingAreaBorderStep = $('#LivingAreaBorderStep').val();

    calcSetting.IsUsePollutantsList = $('#IsUsePollutantsList').prop('checked');
    calcSetting.AirPollutantIds = $('#AirPollutantsSelectList').val();
    return calcSetting;
}

function CreateWindSpeedSettingObj() {
    var windSpeedSetting = new CalcWindSpeedSetting();
    windSpeedSetting.Mode = $('#WindSpeedMode').val();
    windSpeedSetting.Speed = $('#WindSpeed').val();
    windSpeedSetting.StartSpeed = $('#WindStartSpeed').val();
    windSpeedSetting.EndSpeed = $('#WindEndSpeed').val();
    windSpeedSetting.StepSpeed = $('#WindStepSpeed').val();
    windSpeedSetting.Speeds = ParceWindArray($('#WindSpeeds').val());
    return windSpeedSetting;
}

function CreateWindDirectionSettingObj() {
    var windDirectionSetting = new CalcWindDirectionSetting();
    windDirectionSetting.Mode = $('#WindDirectionMode').val();
    windDirectionSetting.Direction = $('#WindDirection').val();
    windDirectionSetting.StartDirection = $('#WindStartDirection').val();
    windDirectionSetting.EndDirection = $('#WindEndDirection').val();
    windDirectionSetting.StepDirection = $('#WindStepDirection').val();
    windDirectionSetting.Directions = ParceWindArray($('#WindDirections').val());
    return windDirectionSetting;
}

function ParceWindArray(enumeration) {
    var values = [];
    enumeration.split(";").forEach(function (item) {
        values.push(item.trim());
    });
    return values;
}