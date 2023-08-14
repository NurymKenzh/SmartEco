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
                ShowButtons();
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
    $('#ContainerIndSiteEnterpriseBorders').css('display', 'none');
}

function ShowButtons() {
    $('#ContainerIndSiteEnterpriseBorders').css('display', 'block');
}