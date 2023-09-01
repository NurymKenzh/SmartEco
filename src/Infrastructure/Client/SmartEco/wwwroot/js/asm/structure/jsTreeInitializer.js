var classes = {
    indSiteEnterpriseId: '.indSiteEnterpriseId',
    workshopId: '.workshopId',
    areaId: '.areaId'
}

var ids = {
    NameIndSiteEnterprise: '#NameIndSiteEnterprise',
    NameWorkshop: '#NameWorkshop',
    NameArea: '#NameArea',
    MinSizeSanitaryZoneValue: '#MinSizeSanitaryZoneValue'
}

var btnIds = {
    ContainerButtonsDefault: '#ContainerButtonsDefault',
    ContainerButtonsIndSiteEnterprise: '#ContainerButtonsIndSiteEnterprise',
    ContainerButtonsWorkshop: '#ContainerButtonsWorkshop',
    ContainerButtonsArea: '#ContainerButtonsArea'
}

function InitializeTreeButtons(data, urls) {
    if (data == undefined || data == null) {
        ShowButtonsDefault();
    }
    else {
        var parcedId = ParseNodeId(data.node.id);
        if (parcedId.length == 2) {
            if (parcedId.name == 'indSiteEnterprise') {
                SetFieldsIndSiteEnterprise(data, parcedId.number);
                ShowButtonsIndSiteEnterprise();
            }
            else if (parcedId.name == 'workshop') {
                SetFieldsWorkshop(data, parcedId.number);
                ShowButtonsWorkshop();
            }
            else if (parcedId.name == 'area') {
                SetFieldsArea(data, parcedId.number);
                ShowButtonsArea();
            }
            else {
                ShowButtonsDefault();
            }
        }
        else {
            ShowButtonsDefault();
        }
    }
}

function ParseNodeId(id) {
    var idArray = id.split('_');
    var idName = idArray[0];
    var idNumber = idArray[1];
    return {
        length: idArray.length,
        name: idName,
        number: idNumber
    };
}

//#region ShowButtons

function ShowButtonsDefault() {
    $(btnIds.ContainerButtonsDefault).css('display', 'inline-block');
    $(btnIds.ContainerButtonsIndSiteEnterprise).css('display', 'none');
    $(btnIds.ContainerButtonsWorkshop).css('display', 'none');
    $(btnIds.ContainerButtonsArea).css('display', 'none');
}

function ShowButtonsIndSiteEnterprise() {
    $(btnIds.ContainerButtonsDefault).css('display', 'none');
    $(btnIds.ContainerButtonsIndSiteEnterprise).css('display', 'inline-block');
    $(btnIds.ContainerButtonsWorkshop).css('display', 'none');
    $(btnIds.ContainerButtonsArea).css('display', 'none');
}

function ShowButtonsWorkshop() {
    $(btnIds.ContainerButtonsDefault).css('display', 'none');
    $(btnIds.ContainerButtonsIndSiteEnterprise).css('display', 'none');
    $(btnIds.ContainerButtonsWorkshop).css('display', 'inline-block');
    $(btnIds.ContainerButtonsArea).css('display', 'none');
}

function ShowButtonsArea() {
    $(btnIds.ContainerButtonsDefault).css('display', 'none');
    $(btnIds.ContainerButtonsIndSiteEnterprise).css('display', 'none');
    $(btnIds.ContainerButtonsWorkshop).css('display', 'none');
    $(btnIds.ContainerButtonsArea).css('display', 'inline-block');
}

//#endregion A

//#region SetFields

function SetFieldsIndSiteEnterprise(data, number) {
    $(classes.indSiteEnterpriseId).val(number);
    $(ids.NameIndSiteEnterprise).val(data.node.text);
    $(ids.MinSizeSanitaryZoneValue).val(data.node.a_attr.minSizeSanitaryZone);
}

function SetFieldsWorkshop(data, number) {
    var parsedParentId = ParseNodeId(data.node.parent);

    $(classes.indSiteEnterpriseId).val(parsedParentId.number);
    $(classes.workshopId).val(number);
    $(ids.NameWorkshop).val(data.node.text);
}

function SetFieldsArea(data, number) {
    var parsedParentId = ParseNodeId(data.node.parent);

    $(classes.workshopId).val(parsedParentId.number);
    $(classes.areaId).val(number);
    $(ids.NameArea).val(data.node.text);
}

//#endregion SetFields