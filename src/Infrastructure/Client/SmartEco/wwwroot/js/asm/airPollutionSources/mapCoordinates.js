var map, inputCoordinates, inputLongCoordinate, inputLatCoordinate;

$('.edit-info-btn').click(function (e) {
    var btn = $(this);
    var editRow = GetEditRow(btn);
    var sourceId = editRow.find('[name="IdSource"]').val();
    var indSiteId = editRow.find('[name="RelationSource"]').find(':selected').data('indsite');
    var coordinates = editRow.find('[name="RelationSource"]').find(`option[value=indSiteEnterprise_${indSiteId}]`).data('coordinates');
    InitializeInputs(editRow);
    InitializeMap(sourceId, coordinates);
    ChangeCoordinates();
});

function InitializeInputs(editRow) {
    inputCoordinates = editRow.find('[name="CoordinateInfo"]');
    inputLongCoordinate = editRow.find('[name="CoordinateLongInfo"]');
    inputLatCoordinate = editRow.find('[name="CoordinateLatInfo"]');
    if (inputCoordinates.val()) {
        var coordinatesSplit = inputCoordinates.val().split(',');
        inputLongCoordinate.val(coordinatesSplit[0]);
        inputLatCoordinate.val(coordinatesSplit[1]);
    }
}

//Industrial site layer
var vectorSource = new ol.source.Vector();
var vectorLayer = new ol.layer.Vector({
    source: vectorSource
});

//Air pollution source layer
var airSource = new ol.source.Vector();
var airLayer = new ol.layer.Vector({
    source: airSource
});

function InitializeMap(sourceId, coordinates) {
    var target = 'map_' + sourceId;
    $('#' + target).empty();
    airSource.clear();

    if (coordinates) {
        var polyCoords = ParcePolyCoordinates(coordinates);
        feature = new ol.Feature({
            geometry: new ol.geom.Polygon([polyCoords])
        })
        vectorSource.addFeature(feature);
    }

    //Initialize map
    map = new ol.Map({
        target: target,
        controls: ol.control.defaults().extend([new ol.control.FullScreen()]),
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            }),
            vectorLayer,
            airLayer
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([68.291, 47.5172]),
            zoom: 4
        })
    });

    //Set event click for map
    map.on('click', function (event) {
        var coordinates = event.coordinate;
        var point = new ol.geom.Point(
            coordinates
        );
        var featurePoint = new ol.Feature({
            geometry: point
        });
        airSource.clear();
        airSource.addFeature(featurePoint);

        //Set coordinates to inputs
        var featureClone = featurePoint.clone();
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326');
        var coords = featureClone.getGeometry().getCoordinates();

        inputCoordinates.val(coords);
        inputLongCoordinate.val(coords[0]);
        inputLatCoordinate.val(coords[1]);
    });

    //Set zoom to industrial site
    var featureLength = vectorSource.getFeatures().length;
    if (featureLength > 0) {
        map.getView().fit(vectorSource.getExtent(), map.getSize());
    }
}

function ParcePolyCoordinates(coordinates) {
    var coordsArr = coordinates.split(';');
    var polyCoords = [];

    $.each(coordsArr, function (index, coords) {
        var coord = coords.split(',');
        polyCoords.push(ol.proj.transform([parseFloat(coord[0]), parseFloat(coord[1])], 'EPSG:4326', 'EPSG:3857'));
    });
    return polyCoords;
}

$('.show-map').on('shown.bs.modal', function () {
    map.updateSize();
})

$('.clear-map').click(function (e) {
    airSource.clear();
    inputCoordinates.val('');
    inputLongCoordinate.val('');
    inputLatCoordinate.val('');
});

$('.coordinate-input').change(function () {
    ChangeCoordinates();
});

function ChangeCoordinates() {
    if (inputLongCoordinate.val() && inputLatCoordinate.val()) {
        var coordinates = [parseFloat(inputLongCoordinate.val()), parseFloat(inputLatCoordinate.val())];

        coordinates = ol.proj.transform(coordinates, 'EPSG:4326', 'EPSG:3857');
        var point = new ol.geom.Point(
            coordinates
        );
        var featurePoint = new ol.Feature({
            geometry: point
        });
        airSource.clear();
        airSource.addFeature(featurePoint);
    }
};