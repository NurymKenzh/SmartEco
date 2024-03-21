var mapPoint;
var pointInputs = { coordinates: {}, coordinates3857: {}, abscissaCoordinate: {}, ordinateCoordinate: {}, abscissa3857: {}, ordinate3857: {} };

$('.show-create-point-btn').click(function (e) {
    InitializePointInputs();
    InitializePointMap();
});

$('.show-map-point').on('shown.bs.modal', function () {
    mapPoint.updateSize();
})

$('.clear-map-point').click(function (e) {
    pointSource.clear();
    $.each(pointInputs, function (index, value) {
        $(this).val('');
    });
    ChangePointMapSources();
});

$('.coordinate-point-input').change(function () {
    ChangePointMapSources();
});

function InitializePointInputs() {
    pointInputs.abscissaCoordinate = $('#PointNewAbscissaX');
    pointInputs.ordinateCoordinate = $('#PointNewOrdinateY');
    pointInputs.abscissa3857 = $('#PointNewAbscissa3857');
    pointInputs.ordinate3857 = $('#PointNewOrdinate3857');
}

//Point layer
var pointSource = new ol.source.Vector();
var pointLayer = new ol.layer.Vector({
    source: pointSource
});

function InitializePointMap() {
    var target = 'MapPoint';
    $('#' + target).empty();
    pointSource.clear();

    //Initialize map
    mapPoint = new ol.Map({
        target: target,
        controls: ol.control.defaults().extend([new ol.control.FullScreen()]),
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            }),
            pointLayer
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([68.291, 47.5172]),
            zoom: 4
        })
    });

    ChangePointMapSources();
}

function AddPointDrawInteraction() {
    var draw = new ol.interaction.Draw({
        source: pointSource,
        type: "Point",
    });
    mapPoint.addInteraction(draw);
    draw.on('drawend', function (e) {
        feature = e.feature;
        mapPoint.removeInteraction(draw); // remove draw interaction
        var featureClone = feature.clone(); // cloning feature
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326'); // transform cloned feature to EPSG:4326
        
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3857 = feature.getGeometry().getCoordinates();
        SetPointCoordinates(coords, coords3857);
        AddPointModifyIteraction();
    });
}

function AddPointModifyIteraction() {
    var modifyInteraction = new ol.interaction.Modify({
        source: pointSource
    });
    mapPoint.addInteraction(modifyInteraction);
    modifyInteraction.on('modifyend', function (evt) {
        var collection = evt.features; // get features
        var featureClone = collection.item(0).clone(); // There's only one feature, so get the first and only one
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326'); //transform cloned feature to EPSG:4326
        
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3857 = collection.item(0).getGeometry().getCoordinates();
        SetPointCoordinates(coords, coords3857);
    });
}

function ChangePointMapSources() {
    if (pointInputs.abscissaCoordinate.val() && pointInputs.ordinateCoordinate.val()) {
        var coordinates = 
            [
                parseFloat(pointInputs.abscissaCoordinate.val().replaceCommaToDot()),
                parseFloat(pointInputs.ordinateCoordinate.val().replaceCommaToDot())
            ];

        //Change coords on map
        coordinates = ol.proj.transform(coordinates, 'EPSG:4326', 'EPSG:3857');
        pointInputs.abscissa3857.val(coordinates[0]);
        pointInputs.ordinate3857.val(coordinates[1]);

        var point = new ol.geom.Point(
            coordinates
        );
        var featurePoint = new ol.Feature({
            geometry: point
        });
        pointSource.clear();
        pointSource.addFeature(featurePoint);
        
        AddPointModifyIteraction();
    }
    else {
        AddPointDrawInteraction();
    }
};

function SetPointCoordinates(coords, coords3857) {
    pointInputs.abscissaCoordinate.val(coords[0]);
    pointInputs.ordinateCoordinate.val(coords[1]);
    pointInputs.abscissa3857.val(coords3857[0]);
    pointInputs.ordinate3857.val(coords3857[1]);
}