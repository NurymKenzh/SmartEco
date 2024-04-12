import { objects, layers, sources } from './initializeLayers.js'

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


$(function () {
    InitializeIndSiteCheckboxes();
});

function InitializeIndSiteCheckboxes() {
    $('input[id^="IndSiteBorderCheckbox_"]').each(function () {  // Matches those that begin with 'IndSiteBorderCheckbox_'
        var indSiteBorderCheckbox = $(this);
        var id = indSiteBorderCheckbox.attr('id');
        objects.map.getLayers().forEach(function (layer) {
            if (layer.get('name') == id) {
                indSiteBorderCheckbox.on("change", function () {
                    layer.setVisible(this.checked);
                });
            }
        })
    });
    $('input[id^="SanZoneBorderCheckbox_"]').each(function () {  // Matches those that begin with 'SanZoneBorderCheckbox_'
        var sanZoneBorderCheckbox = $(this);
        var id = sanZoneBorderCheckbox.attr('id');
        objects.map.getLayers().forEach(function (layer) {
            if (layer.get('name') == id) {
                sanZoneBorderCheckbox.on("change", function () {
                    layer.setVisible(this.checked);
                });
            }
        })
    });
}