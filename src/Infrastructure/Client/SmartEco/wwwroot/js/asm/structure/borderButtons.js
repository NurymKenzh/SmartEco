function InitializeBorderButtons(id) {
    if (id == undefined || id == null) {
        HideButtons();
    }
    else {
        var idArray = id.split('_');
        if (idArray.length == 2) {
            var idName = idArray[0];
            var idNumber = idArray[1];
            if (idName == 'indSiteEnterprise') {
                HideButtons();
                ShowButtons(idNumber);
            }
            else {
                HideButtons();
            }
        }
        else {
            HideButtons();
        }
    }
}

function HideButtons() {
    var elements = document.querySelectorAll("[id^='ContainerIndSiteEnterpriseBorders']");
    $.each(elements, function (index, element) {
        $('#' + element.id).css('display', 'none');
    });
}

function ShowButtons(idNumber) {
    $('#ContainerIndSiteEnterpriseBorders_' + idNumber).css('display', 'block');
}