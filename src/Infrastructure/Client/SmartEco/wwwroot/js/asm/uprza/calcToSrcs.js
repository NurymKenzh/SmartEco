//Change value toggle checkbox by click button
//Because there is a library 'bug': when rebooting 'PartialView', an event 'onchange' is added every time
$('.button-checkbox-toggle').click(function () {
    var editRow = $(this).closest('dd').length > 0
        ? $(this).closest('dd')
        : $(this).closest('td');
    var checkboxToggle = editRow.find('.checkbox-toggle');
    var valueChanged = checkboxToggle.val() == 'true' ? false : true;
    if (valueChanged) {
        SelectSource(editRow, checkboxToggle, valueChanged);
    }
    else {
        DeleteSource(editRow, checkboxToggle, valueChanged)
    }
});

//Clicked on button for select/unselect all sources
function AllSourcesToggleChange() {
    var checkboxToggle = $('#IsInvolvedAllSourcesToggle');
    var valueChanged = checkboxToggle.val() == 'true' ? false : true;
    if (valueChanged) {
        SelectAllSources(checkboxToggle, valueChanged);
    }
    else {
        DeleteAllSources(checkboxToggle, valueChanged)
    }
}

//Select all sources to calculation
function SelectAllSources(checkboxToggle, valueChanged) {
    var dataFilter = CreateSourceFilter();
    dataFilter.PageNumber = $('li.page-item.active .page-link').val();
    $.ajax({
        data: dataFilter,
        url: $('#CalcToSrcSelectAllReq').data('url'),
        type: 'POST',
        traditional: true, //for send array
        success: function (result) {
            $('#AirPollutionSources').empty();
            $('#AirPollutionSources').html(result);
        }
    });
}

//Unselect all sources from calculation
function DeleteAllSources(checkboxToggle, valueChanged) {
    var dataFilter = CreateSourceFilter();
    dataFilter.PageNumber = $('li.page-item.active .page-link').val();
    $.ajax({
        data: dataFilter,
        url: $('#CalcToSrcDeleteAllReq').data('url'),
        type: 'POST',
        traditional: true, //for send array
        success: function (result) {
            $('#AirPollutionSources').empty();
            $('#AirPollutionSources').html(result);
        }
    });
}

//Select source to calculation
function SelectSource(editRow, checkboxToggle, valueChanged) {
    var calculationId = $('#CalculationId').val();
    var sourceId = editRow.closest('tr').find('[name="IdSource"]').val();
    var calcToSrc = new CalculationToSource(
        calculationId,
        sourceId
    );
    $.ajax({
        data: {
            calcToSrc: calcToSrc
        },
        url: $('#CalcToSrcCreateReq').data('url'),
        type: 'POST',
        success: function (result) {
            checkboxToggle.bootstrapToggle('toggle');
            checkboxToggle.val(valueChanged);
        }
    });
}

//Unselect source from calculation
function DeleteSource(editRow, checkboxToggle, valueChanged) {
    var calculationId = $('#CalculationId').val();
    var sourceId = editRow.closest('tr').find('[name="IdSource"]').val();
    var calcToSrc = new CalculationToSource(
        calculationId,
        sourceId
    );
    $.ajax({
        data: {
            calcToSrc: calcToSrc
        },
        url: $('#CalcToSrcDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            checkboxToggle.bootstrapToggle('toggle');
            checkboxToggle.val(valueChanged);
        }
    });
}

//Updating sources, when changed enterprises in tab
function UpdateSources() {
    var dataFilter = CreateSourceFilter();
    GetCalcToSources(dataFilter);
}

function CreateSourceFilter(pageNumber) {
    var dataFilter = new SourceFilter();
    dataFilter.CalculationId = $('#CalculationId').val();
    dataFilter.EnterpriseIds = GetEnterpriseIds();
    dataFilter.PageSize = $('[name="PageSize"]').val();
    dataFilter.PageNumber = pageNumber;
    return dataFilter;
}

function GetEnterpriseIds() {
    var enterpriseIds = [];
    var enterpriseIdsInp = $('#EnterprisesTable').find('[name="EnterpriseId"]');
    $(enterpriseIdsInp).each(function (index) {
        enterpriseIds.push($(this).val());
    });
    return enterpriseIds;
}

function GetCalcToSources(data) {
    $.ajax({
        data: data,
        url: $('#CalcToSrcGetReq').data('url'),
        type: 'GET',
        traditional: true, //for send array
        success: function (result) {
            $('#AirPollutionSources').empty();
            $('#AirPollutionSources').html(result);
        }
    });
}

//#region Coordinates initialize
$('.view-info-btn').click(function (e) {
    var btn = $(this);
    var editRow = GetViewSourceRow(btn);
    var isOrganized = editRow.find('[name="IsOrganizedSource"]').val();
    InitializeSourceInputs(editRow, isOrganized);
});

function GetViewSourceRow(btn) {
    return btn.closest('tr').length > 0 ? btn.closest('tr') : btn.closest('dl');
}

function InitializeSourceInputs(editRow, isOrganized) {
    var inputs = { coordinates: {}, longCoordinate: {}, latCoordinate: {}, sizeLength: {}, sizeWidth: {} };
    inputs.coordinates = editRow.find('[name="CoordinateInfo"]');
    inputs.longCoordinate = editRow.find('[name="CoordinateLongInfo"]');
    inputs.latCoordinate = editRow.find('[name="CoordinateLatInfo"]');
    inputs.sizeLength = editRow.find('[name="LengthInfo"]');
    inputs.sizeWidth = editRow.find('[name="WidthInfo"]');
    if (inputs.coordinates.val()) {
        var coordinatesSplit = inputs.coordinates.val().split(',');
        inputs.longCoordinate.val(coordinatesSplit[0]);
        inputs.latCoordinate.val(coordinatesSplit[1]);
    }

    //Changing inputs depending on source type
    if (isOrganized == 'true') {
        $('.is-organized-block').prop('hidden', false);
        $('.isnot-organized-block').prop('hidden', true);
    }
    else {
        $('.is-organized-block').prop('hidden', true);
        $('.isnot-organized-block').prop('hidden', false);
    }
}

//#endregion Coordinates initialize

//#region Filter events

//Apply filter
$("#FilterCalculationToSources").click(function (e) {
    var dataFilter = CreateSourceFilter();
    GetCalcToSources(dataFilter);
});

//#endregion Filter events

//#region Pagionation event

$('.page-link').click(function (e) {
    var pageNumber = $(this).val();
    var dataFilter = CreateSourceFilter(pageNumber);
    GetCalcToSources(dataFilter);
});

//#endregion Pagination event