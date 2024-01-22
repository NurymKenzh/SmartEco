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
    var calculationId = $('#CalculationId').val();
    var jobId = $('#JobIdStateCalc').val();
    $.ajax({
        data: {
            calculationId: calculationId,
            jobId: jobId
        },
        url: $('#StateCalcUpdateReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#StateCalculationTable').empty();
            $('#StateCalculationTable').html(result);
        }
    });
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