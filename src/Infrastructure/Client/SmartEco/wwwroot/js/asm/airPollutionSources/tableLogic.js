//Initializing text dropdownlists
$(function () {
    $('#AirPollutionSourcesTable').find('tbody > tr').each(function () {
        RelationTextChange($(this));
        TypeTextChange($(this));
    });
});

//#region Row events

$('tr').click(function () {
    console.log($(this));
    RowColorChanging($(this));
});

$('.operation-modes-btn').click(function () {
    $(this).toggleClass('down');
    $(this).parents('tr').next('.operation-modes-row').toggle('slow');
    AddSourceDisabling();
    RowColorChanging($(this));
});

//Add new source
$('#AddAirPollutionSource').click(function (e) {
    var pageSize = $('[name="PageSize"]').val();
    $.ajax({
        data: {
            EnterpriseId: $('#FilterEnterpriseId').data('value'),
            PageSize: pageSize
        },
        url: $('#ApsCreateReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#AirPollutionSourcesTab').empty();
            $('#AirPollutionSourcesTab').html(result);
        }
    });
});

//Edit source
$(".edit-btn").click(function (e) {
    var btn = $(this);
    var editRow = GetEditRow(btn);
    var tdInputs = editRow.find('input');
    var tdSelects = editRow.find('select');

    editRow.find('.select-text').removeClass('d-inline-block').addClass('d-none');
    $(tdInputs).each(function (index) {
        var currentValue = $(this).val();
        SetCurrentValueToData(this, currentValue);

        if ($(this).is(':checkbox')) {
            $(this).parent().data('toggleOff', $(this).parent().hasClass('off'));
        }
    });
    $(tdSelects).each(function (index) {
        var currentValue = $(this).find(":selected").val();
        SetCurrentValueToData(this, currentValue);
        $(this).removeClass('d-none').addClass('d-inline-block');
    });

    EditableButtonsShow(btn, editRow, true);
});

//Copy source
$('.copy-btn').click(function (e) {
    var btn = $(this);
    var editRow = GetEditRow(btn);
    var dataSource = CreateSource(btn);
    var pageSize = $('[name="PageSize"]').val();
    var isOrganized = editRow.find('[name="IsOrganizedSource"]').val();
    $.ajax({
        data: {
            EnterpriseId: $('#FilterEnterpriseId').data('value'),
            PageSize: pageSize,
            IsOrganized: isOrganized,
            AirPollutionSource: dataSource
        },
        url: $('#ApsCopyReq').data('url'),
        type: 'POST',
        success: function (result) {
            $('#AirPollutionSourcesTab').empty();
            $('#AirPollutionSourcesTab').html(result);
        }
    });
});

//Delete source
$('[name="DeleteSourceBtn"]').click(function (e) {
    var sourceId = $(this).val();
    var dataFilter = CreateFilter();
    dataFilter.PageNumber = $('li.page-item.active .page-link').val();

    $.ajax({
        data: {
            Id: sourceId,
            Filter: dataFilter
        },
        url: $('#ApsDeleteReq').data('url'),
        type: 'POST',
        success: function (result) {
            $(`DeleteAirPollutionSource_${sourceId}`).remove();
            $('.modal-backdrop').remove();

            $('#AirPollutionSourcesTab').empty();
            $('#AirPollutionSourcesTab').html(result);
        }
    });
});

//Cancel changes
$(".cancel-btn").click(function (e) {
    var btn = $(this);
    var editRow = GetEditRow(btn);
    var tdInputs = editRow.find('input');
    var tdSelects = editRow.find('select');

    editRow.find('.select-text').removeClass('d-none').addClass('d-inline-block');
    $(tdInputs).each(function (index) {
        var currentValue = $(this).data('current-value');
        SetCurrentValue(this, currentValue);

        if ($(this).is(':checkbox')) {
            if ($(this).parent().data('toggleOff') == true) {
                $(this).closest('div').removeClass('off').addClass('off');
            }
            else {
                $(this).closest('div').removeClass('off');
            }
        }
    });
    $(tdSelects).each(function (index) {
        var currentValue = $(this).data('current-value');
        if (currentValue != "") {
            SetCurrentValue(this, currentValue);
        }
        $(this).removeClass('d-inline-block').addClass('d-none');
    });

    editRow.find('.invalid-feedback').removeClass('d-inline-block').text('');
    EditableButtonsShow(btn, editRow);
});

//Save source
$('[name="SaveSourceBtn"]').click(function (e) {
    var btn = $(this);
    var editRow = GetEditRow(btn);
    var dataSource = CreateSource(btn);
    editRow.find('.invalid-feedback').removeClass('d-inline-block').text('');

    $.ajax({
        data: dataSource,
        url: $('#ApsEditReq').data('url'),
        type: 'POST',
        success: function (result) {
            SetDisabledFields(editRow);
            RelationTextChange(editRow);
            TypeTextChange(editRow);
            EditableButtonsShow(btn, editRow);

            editRow.find('.select-text').removeClass('d-none').addClass('d-inline-block');
            editRow.find('select').removeClass('d-inline-block').addClass('d-none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var status = textStatus;;
            var error = $.parseJSON(jqXHR.responseText);
            ValidSourceInfo(editRow, error);
        }
    });
});

//Valid info source
$('[name="SaveInfoBtn"]').click(function (e) {
    var btn = $(this);
    var editRow = GetEditRow(btn);
    editRow.find('.invalid-feedback').removeClass('d-inline-block').text('');
    var dataInfo = CreateInfo(editRow);

    $.ajax({
        data: dataInfo,
        url: $('#ApsValidInfoReq').data('url'),
        type: 'POST',
        success: function (result) {
            SetDisabledFields(editRow);
            EditableButtonsShow(btn, editRow);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var status = textStatus;;
            var error = $.parseJSON(jqXHR.responseText);
            if (error.TerrainCoefficient) {
                editRow.find('[name="TerrainCoefficientInvalid"]').addClass('d-inline-block').text(error.TerrainCoefficient[0]);
            }
            if (error.AngleDeflection) {
                editRow.find('[name="AngleDeflectionInvalid"]').addClass('d-inline-block').text(error.AngleDeflection[0]);
            }
            if (error.AngleRotation) {
                editRow.find('[name="AngleRotationInvalid"]').addClass('d-inline-block').text(error.AngleRotation[0]);
            }
            if (error.Hight) {
                editRow.find('[name="HightInvalid"]').addClass('d-inline-block').text(error.Hight[0]);
            }
            if (error.Diameter) {
                editRow.find('[name="DiameterInvalid"]').addClass('d-inline-block').text(error.Diameter[0]);
            }
            if (error.RelationBackgroundId) {
                editRow.find('[name="RelationBackgroundInvalid"]').addClass('d-inline-block').text(error.RelationBackgroundId[0]);
            }
        }
    });
});

//Change value toggle checkbox
//$(':checkbox').change(function () {
//    var currentValue = $(this).val();
//    var value = currentValue == 'true' ? false : true;
//    $(this).val(value);
//});

//Change value toggle checkbox by click button
//Because there is a library 'bug': when rebooting 'PartialView', an event 'onchange' is added every time
$('.button-checkbox-toggle').click(function () {
    var editRow = $(this).closest('dd').length > 0
        ? $(this).closest('dd')
        : $(this).closest('td')
    var checkboxToggle = editRow.find('.checkbox-toggle');
    checkboxToggle.bootstrapToggle('toggle');
    var valueChanged = checkboxToggle.val() ==  'True' ? false : true;
    checkboxToggle.val(valueChanged);
});

//Change list source type (depends on 'IsOrganized' checkbox)
$('[name="IsOrganizedSource"]').change(function () {
    var isOrganizedChecked = this.checked;
    var editRow = GetEditRow($(this));
    var showSourceTypes = [];
    var sourceTypes = editRow.find('[name="TypeIdSource"]').find('option');

    $(sourceTypes).each(function (index) {
        var isOrganizedType = $(this).data('organized');
        if (isOrganizedChecked == isOrganizedType) {
            $(this).prop('hidden', false);
        }
        else {
            $(this).prop('hidden', true);
        }
        $(this).removeAttr('selected');
    });
    editRow.find('[name="TypeIdSource"]').find(`[data-organized='${isOrganizedChecked}']`).first().prop('selected', true);
});

//#endregion Events

//#region Filter events

$(".sort-btn").click(function (e) {
    var sortOrder = $(this).data('sort');
    $('[name="SortOrder"]').val(sortOrder);
    var dataFilter = CreateFilter();
    GetSources(dataFilter);
});

//Apply filter
$("#FilterAirPollutionSources").click(function (e) {
    var dataFilter = CreateFilter();
    GetSources(dataFilter);
});

$("#ResetFilterAirPollutionSources").click(function (e) {
    var dataFilter = new Filter();
    dataFilter.EnterpriseId = $('#FilterEnterpriseId').data('value');
    GetSources(dataFilter);
});

//#endregion Filter events

function AddSourceDisabling() {
    $('#AddAirPollutionSource').prop('disabled', false);
    $('.operation-modes-btn').each(function (index) {
        if ($(this).hasClass('down')) {
            $('#AddAirPollutionSource').prop('disabled', true);
            return false;
        }
    });
}

function RowColorChanging(btn) {
    var tr = btn.parents('tr');
    if (tr.hasClass('bg-info-opacity-15')) {
        tr.removeClass('bg-info-opacity-15');
    }
    else {
        tr.addClass('bg-info-opacity-15');
    }
}

function CreateFilter() {
    var dataFilter = new Filter();
    dataFilter.EnterpriseId = $('#FilterEnterpriseId').data('value');
    dataFilter.NumberFilter = $('[name="NumberFilter"]').val();
    dataFilter.NameFilter = $('[name="NameFilter"]').val();
    dataFilter.RelationFilter = $('[name="RelationFilter"]').val();
    dataFilter.SortOrder = $('[name="SortOrder"]').val();
    dataFilter.PageSize = $('[name="PageSize"]').val();
    return dataFilter;
}

function GetSources(data) {
    $.ajax({
        data: data,
        url: $('#ApsGetSourcesReq').data('url'),
        type: 'GET',
        success: function (result) {
            $('#AirPollutionSourcesTab').empty();
            $('#AirPollutionSourcesTab').html(result);
        }
    });
}

function GetEditRow(btn) {
    return btn.closest('tr').length > 0 ? btn.closest('tr') : btn.closest('dl');
}

function SetCurrentValueToData(elem, val) {
    $(elem).data('current-value', val);
    $(elem).prop('disabled', false);
}

function SetCurrentValue(elem, val) {
    $(elem).val(val);
    $(elem).data('current-value', '');
    $(elem).prop('disabled', true);
}

function SetDisabledFields(editRow) {
    var tdInputs = editRow.find('input');
    var tdSelects = editRow.find('select');

    $(tdInputs).each(function (index) {
        $(this).prop('disabled', true);
    });
    $(tdSelects).each(function (index) {
        $(this).prop('disabled', true);
    });
}

function EditableButtonsShow(btn, editRow, isEditBtnClick) {
    var editBtn = editRow.find('.edit-btn');
    var saveBtn = editRow.find('.save-btn');
    var cancelBtn = editRow.find('.cancel-btn');

    ShowHideButtons(editRow, editBtn, saveBtn, cancelBtn, isEditBtnClick);
}

function ShowHideButtons(editRow, editBtn, saveBtn, cancelBtn, isEditBtnClick) {
    if (isEditBtnClick) {
        editBtn.removeClass('d-inline-block').addClass('d-none');
        saveBtn.removeClass('d-none').addClass('d-inline-block');
        cancelBtn.removeClass('d-none').addClass('d-inline-block');
        editRow.addClass('bg-warning-opacity-25');
        editRow.find('.form-control-plaintext').removeClass('form-control-plaintext').addClass('form-control');
        editRow.find('.btn-disabled').prop('disabled', false);
    }
    else {
        editBtn.removeClass('d-none').addClass('d-inline-block');
        saveBtn.removeClass('d-inline-block').addClass('d-none');
        cancelBtn.removeClass('d-inline-block').addClass('d-none');
        editRow.removeClass('bg-warning-opacity-25');
        editRow.find('.form-control').removeClass('form-control').addClass('form-control-plaintext');
        editRow.find('.btn-disabled').prop('disabled', true);
    }
}

function CreateSource(btn) {
    var editRow = GetEditRow(btn);
    var dataSource = new AirPollutionSource();

    dataSource.Id = editRow.find('[name="IdSource"]').val()
    dataSource.Number = editRow.find('[name="NumberSource"]').val();
    dataSource.Name = editRow.find('[name="NameSource"]').val();
    dataSource.IsActive = editRow.find('[name="IsActiveSource"]').val();
    dataSource.TypeId = editRow.find('[name="TypeIdSource"]').val();

    var editRowInfo = $('#ShowAirPollutionSourceInfo_' + dataSource.Id).find('dl');
    dataSource.SourceInfo = CreateInfo(editRowInfo);

    var parcedRelation = GetSourceRelation(editRow);
    if (parcedRelation.name == 'indSiteEnterprise') {
        dataSource.SourceIndSite = new SourceIndSite(dataSource.Id, parcedRelation.number);
    }
    else if (parcedRelation.name == 'workshop') {
        dataSource.SourceWorkshop = new SourceWorkshop(dataSource.Id, parcedRelation.number);
    }
    else {
        dataSource.SourceArea = new SourceArea(dataSource.Id, parcedRelation.number);
    }
    return dataSource;
}

function CreateInfo(editRow) {
    var dataInfo = new AirPollutionSourceInfo();
    dataInfo.SourceId = editRow.find('[name="SourceIdInfo"]').val();
    dataInfo.Coordinate = editRow.find('[name="CoordinateInfo"]').val();
    dataInfo.TerrainCoefficient = editRow.find('[name="TerrainCoefficientInfo"]').val();
    dataInfo.IsCalculateByGas = editRow.find('[name="IsCalculateByGasInfo"]').val();
    dataInfo.IsVerticalDeviation = editRow.find('[name="IsVerticalDeviationInfo"]').val();
    dataInfo.AngleDeflection = editRow.find('[name="AngleDeflectionInfo"]').val();
    dataInfo.AngleRotation = editRow.find('[name="AngleRotationInfo"]').val();
    dataInfo.IsCovered = editRow.find('[name="IsCoveredInfo"]').val();
    dataInfo.IsSignFlare = editRow.find('[name="IsSignFlareInfo"]').val();
    dataInfo.Hight = editRow.find('[name="HightInfo"]').val();
    dataInfo.Diameter = editRow.find('[name="DiameterInfo"]').val();
    dataInfo.RelationBackgroundId = editRow.find('[name="RelationBackgroundInfo"]').val();
    return dataInfo;
}

function RelationTextChange(row) {
    var relationText = row.find('[name="RelationSource"]').find(':selected').data('relation');
    row.find('.relation-select-text').text(relationText);
}

function TypeTextChange(row) {
    var typeText = row.find('[name="TypeIdSource"]').find(':selected').text();
    row.find('.type-select-text').text(typeText);
}

function GetSourceRelation(editRow) {
    var relationValue = editRow.find('[name="RelationSource"]').find(':selected').val();
    var parcedValue = ParseRelationValue(relationValue);
    return parcedValue;
}

function ParseRelationValue(value) {
    var valueArray = value.split('_');
    var valueName = valueArray[0];
    var valueNumber = valueArray[1];
    return {
        name: valueName,
        number: valueNumber
    };
}

function ValidSourceInfo(editRow, error) {
    if (error.Number) {
        editRow.find('.number-invalid').addClass('d-inline-block').text(error.Number[0]);
    }
    if (error.Name) {
        editRow.find('.name-invalid').addClass('d-inline-block').text(error.Name[0]);
    }

    //Check valid info
    if (error['SourceInfo.TerrainCoefficient']) {
        SetInvalidParameter(editRow, "TerrainCoefficientInvalid", error['SourceInfo.TerrainCoefficient'][0]);
    }
    if (error['SourceInfo.AngleDeflection']) {
        SetInvalidParameter(editRow, "AngleDeflectionInvalid", error['SourceInfo.AngleDeflection'][0]);
    }
    if (error['SourceInfo.AngleRotation']) {
        SetInvalidParameter(editRow, "AngleRotationInvalid", error['SourceInfo.AngleRotation'][0]);
    }
    if (error['SourceInfo.Hight']) {
        SetInvalidParameter(editRow, "HightInvalid", error['SourceInfo.Hight'][0]);
    }
    if (error['SourceInfo.Diameter']) {
        SetInvalidParameter(editRow, "DiameterInvalid", error['SourceInfo.Diameter'][0]);
    }
}

function SetInvalidParameter(editRow, nameParamInvalid, errorText) {
    editRow.find(`[name="${nameParamInvalid}"]`).addClass('d-inline-block').text(errorText);
    editRow.find('.parameters-invalid').addClass('d-inline-block').text('Проверьте параметры');
}

//#region Pagionation event

$('.page-link').click(function (e) {
    ;
    var dataFilter = CreateFilter();
    dataFilter.PageNumber = $(this).val();
    GetSources(dataFilter);
});

//#endregion Pagination event