import { objects, layers, sources } from './initializeLayers.js'
import { BorderStyle } from './staticHelper.js';

$('#AirSourcesCheckbox').on("change", function () {
    layers.airSourcesLayer.setVisible(this.checked);
});

$('#IsolinesCheckbox').on("change", function () {
    layers.isolinesLayer.setVisible(this.checked);
});

$('#PointsCheckbox').on("change", function () {
    layers.pointsLayer.setVisible(this.checked);
});

$('#MpcCheckbox').on("change", function () {
    if (sources.isolinesSource.state_)
        sources.isolinesSource.refresh();
    if (sources.pointsSource.state_)
        sources.pointsSource.refresh();
});

$('#ApsCheckbox').on("change", function () {
    layers.airSourcesLayer.getSource().refresh();
});

$('#MarkerBordersCheckbox').on("change", function () {
    $('input[id^="IndSiteBorderCheckbox_"]').each(function () {  // Matches those that begin with 'IndSiteBorderCheckbox_'
        ChangeBorderStyle($(this), true);
    });
    $('input[id^="SanZoneBorderCheckbox_"]').each(function () {  // Matches those that begin with 'SanZoneBorderCheckbox_'
        ChangeBorderStyle($(this), false);
    });
});


$(function () {
    InitializeIndSiteCheckboxes();
});

function InitializeIndSiteCheckboxes() {
    $('input[id^="IndSiteBorderCheckbox_"]').each(function () {  // Matches those that begin with 'IndSiteBorderCheckbox_'
        SetVisibleBorderListener($(this))
    });
    $('input[id^="SanZoneBorderCheckbox_"]').each(function () {  // Matches those that begin with 'SanZoneBorderCheckbox_'
        SetVisibleBorderListener($(this))
    });
}

function SetVisibleBorderListener(borderCheckbox) {
    var id = borderCheckbox.attr('id');
    objects.map.getLayers().forEach(function (layer) {
        if (layer.get('name') == id) {
            borderCheckbox.on("change", function () {
                layer.setVisible(this.checked);
            });
        }
    })
}

function ChangeBorderStyle(borderCheckbox, isIndSite) {
    var id = borderCheckbox.attr('id');
    objects.map.getLayers().forEach(function (layer) {
        if (layer.get('name') == id) {
            var borderText = $('label[for="' + id + '"]').text().trim();
            if (isIndSite)
                layer.setStyle(BorderStyle.GetIndSiteBorderStyle(borderText));
            else
                layer.setStyle(BorderStyle.GetSanZoneBorderStyle(borderText));
        }
    })
}