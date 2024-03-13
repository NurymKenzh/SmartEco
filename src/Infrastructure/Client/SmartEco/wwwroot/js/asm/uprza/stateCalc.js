const intervalTimeSec = 30;
let updateInterval = setInterval(CheckStateCalc, intervalTimeSec * 1000);

//Show or hide row diagnostic info

function DiagnoscticInfoShowHide(btn) {
    btn.toggleClass('down');
    btn.parents('tr').next('.diagnostic-info-row').toggle('slow');
};

//Get state calculation
function GetStateCalc() {
    var calculationId = $('#CalculationId').val();
    $.ajax({
        data: {
            calculationId: calculationId
        },
        url: $('#StateCalcGetReq').data('url'),
        type: 'GET',
        success: function (result) {
            $('#StateCalculationTable').empty();
            $('#StateCalculationTable').html(result);

            clearInterval(updateInterval);
            updateInterval = setInterval(CheckStateCalc, intervalTimeSec * 1000);
        }
    });
}

//Update state calculation
function UpdateStateCalc() {
    var updateBtn = $('#UpdateBtn');
    var calculationId = $('#CalculationId').val();
    var jobId = $('#JobIdStateCalc').val();
    var isRectangleArea = IsAreaType('#CalculationRectanglesTable', 'RectangleNumber');
    var isPointArea = IsAreaType('#CalculationPointsTable', 'PointNumber');
    updateBtn.prop('disabled', true);
    $.ajax({
        data: {
            calculationId: calculationId,
            jobId: jobId,
            isRectangleArea: isRectangleArea,
            isPointArea: isPointArea
        },
        url: $('#StateCalcUpdateReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#StateCalculationTable').empty();
            $('#StateCalculationTable').html(result);
        }
    });
}

function IsAreaType(tableId, areaTypeNameNumber) {
    var areaTypeArray = $(tableId).find(`[name="${areaTypeNameNumber}"]`);
    return (typeof areaTypeArray !== 'undefined' && areaTypeArray.length > 0);
}

//Delete state calculation
function DeleteStateCalc() {
    var calculationId = $('#CalculationId').val();
    $.ajax({
        data: {
            calculationId: calculationId
        },
        url: $('#StateCalcDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#StateCalculationTable').empty();
            $('#StateCalculationTable').html(result);
        }
    });
}

//interval update
function CheckStateCalc() {
    var statusId = $('#StatusIdCalc').val();
    if (CalcStatuses.Initiated == statusId) {
        UpdateStateCalc();
    } else {
        clearInterval(updateInterval);
    }
}