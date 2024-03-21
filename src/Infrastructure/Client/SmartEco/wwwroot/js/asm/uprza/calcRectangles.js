//Create new calculation rectangle
function CreateRectangle() {
    var calcRectangle = CreateCalculationRectangleObj();
    $.ajax({
        data: {
            calcRectangle: calcRectangle
        },
        url: $('#CalcRectangleCreateReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#ShowCreateCalculationRectangleModal').remove();
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();

            $('#CalculationRectanglesTable').empty();
            $('#CalculationRectanglesTable').html(result);
        }
    });
}

function CreateCalculationRectangleObj() {
    var calculationId = $('#CalculationId').val();
    var number = $('#RectangleNewNumber').val();
    var calcRectangle = new CalculationRectangle(
        calculationId,
        number
    );
    calcRectangle.AbscissaX = $('#RectangleNewAbscissaX').val().replaceDotToComma();
    calcRectangle.OrdinateY = $('#RectangleNewOrdinateY').val().replaceDotToComma();
    calcRectangle.Width = $('#RectangleNewWidth').val();
    calcRectangle.Length = $('#RectangleNewLength').val();
    calcRectangle.Height = $('#RectangleNewHeight').val();
    calcRectangle.StepByWidth = $('#RectangleNewStepByWidth').val();
    calcRectangle.StepByLength = $('#RectangleNewStepByLength').val();
    calcRectangle.Abscissa3857 = $('#RectangleNewAbscissa3857').val().replaceDotToComma();
    calcRectangle.Ordinate3857 = $('#RectangleNewOrdinate3857').val().replaceDotToComma();
    return calcRectangle;
}

//Delete calculation rectangle
function DeleteRectangle(number) {
    var calculationId = $('#CalculationId').val();
    var calcRectangle = new CalculationRectangle(
        calculationId,
        number
    );
    $.ajax({
        data: {
            calcRectangle: calcRectangle
        },
        url: $('#CalcRectangleDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#CalculationRectanglesTable').empty();
            $('#CalculationRectanglesTable').html(result);
        }
    });
}

function ReplaceRectangleMarks(value) {
    return value.replace('.', ',');
}