//Create new calculation point
function CreatePoint() {
    var calcPoint = CreateCalculationPointObj();
    $.ajax({
        data: {
            calcPoint: calcPoint
        },
        url: $('#CalcPointCreateReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#ShowCreateCalculationPointModal').remove();
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();

            $('#CalculationPointsTable').empty();
            $('#CalculationPointsTable').html(result);
        }
    });
}

function CreateCalculationPointObj() {
    var calculationId = $('#CalculationId').val();
    var number = $('#PointNewNumber').val();
    var calcPoint = new CalculationPoint(
        calculationId,
        number
    );
    calcPoint.AbscissaX = $('#PointNewAbscissaX').val();
    calcPoint.OrdinateY = $('#PointNewOrdinateY').val();
    calcPoint.ApplicateZ = $('#PointNewApplicateZ').val();
    return calcPoint;
}

//Delete calculation point
function DeletePoint(number) {
    var calculationId = $('#CalculationId').val();
    var calcPoint = new CalculationPoint(
        calculationId,
        number
    );
    $.ajax({
        data: {
            calcPoint: calcPoint
        },
        url: $('#CalcPointDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#CalculationPointsTable').empty();
            $('#CalculationPointsTable').html(result);
        }
    });
}