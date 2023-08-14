function InitializeTreeButtons(id, urls) {
    if (id == undefined || id == null) {
        SetFieldsDefault();
        ShowButtonsDefault();
    }
    else {
        var idArray = id.split('_');
        if (idArray.length == 2) {
            var idName = idArray[0];
            var idNumber = idArray[1];
            if (idName == 'indSiteEnterprise') {
                SetFieldsIndSiteEnterprise(idNumber, urls.urlDetailIdnSiteEnterprise);
                ShowButtonsIndSiteEnterprise();
            }
            else if (idName == 'workshop') {
                SetFieldsWorkshop(idNumber, urls.urlDetailWorkhsop);
                ShowButtonsWorkshop();
            }
            else if (idName == 'area') {
                SetFieldsArea(idNumber, urls.urlDetailArea);
                ShowButtonsArea();
            }
            else {
                SetFieldsDefault();
                ShowButtonsDefault();
            }
        }
        else {
            SetFieldsDefault();
            ShowButtonsDefault();
        }
    }
}

//#region ShowButtons

function ShowButtonsDefault() {
    $('#ContainerButtonsDefault').css('display', 'inline-block');
    $('#ContainerButtonsIndSiteEnterprise').css('display', 'none');
    $('#ContainerButtonsWorkshop').css('display', 'none');
    $('#ContainerButtonsArea').css('display', 'none');
}

function ShowButtonsIndSiteEnterprise() {
    $('#ContainerButtonsDefault').css('display', 'none');
    $('#ContainerButtonsIndSiteEnterprise').css('display', 'inline-block');
    $('#ContainerButtonsWorkshop').css('display', 'none');
    $('#ContainerButtonsArea').css('display', 'none');
}

function ShowButtonsWorkshop() {
    $('#ContainerButtonsDefault').css('display', 'none');
    $('#ContainerButtonsIndSiteEnterprise').css('display', 'none');
    $('#ContainerButtonsWorkshop').css('display', 'inline-block');
    $('#ContainerButtonsArea').css('display', 'none');
}

function ShowButtonsArea() {
    $('#ContainerButtonsDefault').css('display', 'none');
    $('#ContainerButtonsIndSiteEnterprise').css('display', 'none');
    $('#ContainerButtonsWorkshop').css('display', 'none');
    $('#ContainerButtonsArea').css('display', 'inline-block');
}

//#endregion A

//#region SetFields

function SetFieldsDefault() {
    $('.indSiteEnterpriseId').val('');
    $('.workshopId').val('');
    $('.areaId').val('');
    $('#NameIndSiteEnterprise').val('');
    $('#MinSizeSanitaryZoneValue').val('');
    $('#NameWorkshop').val('');
    $('#NameArea').val('');
}

function SetFieldsIndSiteEnterprise(indSiteEnterpriseId, url) {
    GetIndSiteEnterprise(indSiteEnterpriseId, url);
    $('.workshopId').val('');
    $('.areaId').val('');
    $('#NameWorkshop').val('');
    $('#NameArea').val('');
}

function SetFieldsWorkshop(worshopId, url) {
    GetWorkshop(worshopId, url);
    $('.areaId').val('');
    $('#NameIndSiteEnterprise').val('');
    $('#MinSizeSanitaryZoneValue').val('');
    $('#NameWorkshop').val('');
}

function SetFieldsArea(areaId, url) {
    GetArea(areaId, url);
    $('.indSiteEnterpriseId').val('');
    $('#NameIndSiteEnterprise').val('');
    $('#MinSizeSanitaryZoneValue').val('');
    $('#NameWorkshop').val('');
}

//#endregion SetFields

//#region GetDetailsAjax

function GetIndSiteEnterprise(indSiteEnterpriseId, url) {
    $.ajax({
        url: url,
        data: {
            id: indSiteEnterpriseId,
        },
        type: 'GET',
        success: function (indSiteEnterprise) {
            $('.indSiteEnterpriseId').val(indSiteEnterprise.id);
            $('#NameIndSiteEnterprise').val(indSiteEnterprise.name);
            $('#MinSizeSanitaryZoneValue').val(indSiteEnterprise.minSizeSanitaryZone);
        },
        error: function () {
            $('.indSiteEnterpriseId').val('');
            $('#NameIndSiteEnterprise').val('');
            $('#MinSizeSanitaryZoneValue').val('');
        }
    })
}

function GetWorkshop(workshopId, url) {
    $.ajax({
        url: url,
        data: {
            id: workshopId,
        },
        type: 'GET',
        success: function (workshop) {
            $('.indSiteEnterpriseId').val(workshop.indSiteEnterprise.id);
            $('.workshopId').val(workshop.id);
            $('#NameWorkshop').val(workshop.name);
        },
        error: function () {
            $('.indSiteEnterpriseId').val('');
            $('.workshopId').val('');
            $('#NameWorkshop').val('');
        }
    })
}

function GetArea(areaId, url) {
    $.ajax({
        url: url,
        data: {
            id: areaId,
        },
        type: 'GET',
        success: function (area) {
            $('.workshopId').val(area.workshop.id);
            $('.areaId').val(area.id);
            $('#NameArea').val(area.name);
        },
        error: function () {
            $('.workshopId').val('');
            $('.areaId').val('');
            $('#NameArea').val('');
        }
    })
}

//#endregion GetDetailsAjax