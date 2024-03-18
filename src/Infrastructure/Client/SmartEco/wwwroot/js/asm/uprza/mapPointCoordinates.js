var map;
var isOrganized;
var inputs = { coordinates: {}, coordinates3857: {}, abscissaCoordinate: {}, ordinateCoordinate: {}, abscissa3857: {}, ordinate3857: {} };

$('.show-map-btn').click(function (e) {
    InitializeInputs();
    InitializeMap();
});

$('.show-map').on('shown.bs.modal', function () {
    map.updateSize();
})

$('.clear-map').click(function (e) {
    airSource.clear();
    $.each(inputs, function (index, value) {
        $(this).val('');
    });
    ChangeMapSources();
});

$('.coordinate-input').change(function () {
    ChangeMapSources();
});

$('.size-input').change(function () {
    ChangeMapSources();
});

function InitializeInputs() {
    inputs.abscissaCoordinate = $('#PointNewAbscissaX');
    inputs.ordinateCoordinate = $('#PointNewOrdinateY');
    inputs.abscissa3857 = $('#PointNewAbscissa3857');
    inputs.ordinate3857 = $('#PointNewOrdinate3857');
}

//Air pollution source layer
var airSource = new ol.source.Vector();
var airLayer = new ol.layer.Vector({
    source: airSource
});

function InitializeMap() {
    var target = 'map';
    $('#' + target).empty();
    airSource.clear();

    //Initialize map
    map = new ol.Map({
        target: target,
        controls: ol.control.defaults().extend([new ol.control.FullScreen()]),
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            }),
            airLayer
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([68.291, 47.5172]),
            zoom: 4
        })
    });

    ChangeMapSources();
}

function AddDrawInteraction() {
    var draw = new ol.interaction.Draw({
        source: airSource,
        type: "Point",
    });
    map.addInteraction(draw);
    draw.on('drawend', function (e) {
        feature = e.feature;
        map.removeInteraction(draw); // remove draw interaction
        var featureClone = feature.clone(); // cloning feature
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326'); // transform cloned feature to EPSG:4326
        
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3857 = feature.getGeometry().getCoordinates();
        SetCoordinates(coords, coords3857);
        AddModifyIteraction();
    });
}

function AddModifyIteraction() {
    var modifyInteraction = new ol.interaction.Modify({
        source: airSource
    });
    map.addInteraction(modifyInteraction);
    modifyInteraction.on('modifyend', function (evt) {
        var collection = evt.features; // get features
        var featureClone = collection.item(0).clone(); // There's only one feature, so get the first and only one
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326'); //transform cloned feature to EPSG:4326
        
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3857 = collection.item(0).getGeometry().getCoordinates();
        SetCoordinates(coords, coords3857);
    });
}

function ChangeMapSources() {
    if (inputs.abscissaCoordinate.val() && inputs.ordinateCoordinate.val()) {
        var coordinates = [parseFloat(inputs.abscissaCoordinate.val()), parseFloat(inputs.ordinateCoordinate.val())];

        //Change coords on map
        coordinates = ol.proj.transform(coordinates, 'EPSG:4326', 'EPSG:3857');
        inputs.abscissa3857.val(coordinates[0]);
        inputs.ordinate3857.val(coordinates[1]);

        var point = new ol.geom.Point(
            coordinates
        );
        var featurePoint = new ol.Feature({
            geometry: point
        });
        airSource.clear();
        airSource.addFeature(featurePoint);
        
        AddModifyIteraction();
    }
    else {
        AddDrawInteraction();
    }
};

function SetCoordinates(coords, coords3857) {
    inputs.abscissaCoordinate.val(coords[0]);
    inputs.ordinateCoordinate.val(coords[1]);
    inputs.abscissa3857.val(coords3857[0]);
    inputs.ordinate3857.val(coords3857[1]);
}