var mapRectangle;
var rectInputs = { coordinates: {}, coordinates3857: {}, abscissaCoordinate: {}, ordinateCoordinate: {}, abscissa3857: {}, ordinate3857: {}, sizeLength: {}, sizeWidth: {} };

$('.show-create-rectangle-btn').click(function (e) {
    InitializeInputs();
    InitializeMap();
});

$('.show-map-rectangle').on('shown.bs.modal', function () {
    mapRectangle.updateSize();
})

$('.clear-map-rectangle').click(function (e) {
    centerSource.clear();
    rectanglerSource.clear();
    $.each(rectInputs, function (index, value) {
        $(this).val('');
    });
    ChangeRectangleMapSources();
});

$('.coordinate-rectangle-input').change(function () {
    ChangeRectangleMapSources();
});

$('.size-rectangle-input').change(function () {
    ChangeRectangleMapSources();
});

function InitializeInputs() {
    rectInputs.abscissaCoordinate = $('#RectangleNewAbscissaX');
    rectInputs.ordinateCoordinate = $('#RectangleNewOrdinateY');
    rectInputs.abscissa3857 = $('#RectangleNewAbscissa3857');
    rectInputs.ordinate3857 = $('#RectangleNewOrdinate3857');
    rectInputs.sizeLength = $('#RectangleNewLength');
    rectInputs.sizeWidth = $('#RectangleNewWidth');
}

//Rectangle layer
var rectanglerSource = new ol.source.Vector();
var rectanglerLayer = new ol.layer.Vector({
    source: rectanglerSource
});

//Center layer
var centerSource = new ol.source.Vector();
var centerLayer = new ol.layer.Vector({
    source: centerSource
});

function InitializeMap() {
    var target = 'MapRectangle';
    $('#' + target).empty();
    centerSource.clear();
    rectanglerSource.clear();

    //Initialize map
    mapRectangle = new ol.Map({
        target: target,
        controls: ol.control.defaults().extend([new ol.control.FullScreen()]),
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            }),
            centerLayer,
            rectanglerLayer
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([68.291, 47.5172]),
            zoom: 4
        })
    });

    ChangeRectangleMapSources();

    //Set zoom to rectangle source
    var featureLength = rectanglerSource.getFeatures().length;
    if (featureLength > 0) {
        mapRectangle.getView().fit(rectanglerSource.getExtent(), mapRectangle.getSize());
    }
}

function AddRectangleDrawInteraction() {
    var draw = new ol.interaction.Draw({
        source: centerSource,
        type: "Point",
    });
    mapRectangle.addInteraction(draw);
    draw.on('drawend', function (e) {
        feature = e.feature;
        mapRectangle.removeInteraction(draw); // remove draw interaction
        var featureClone = feature.clone(); // cloning feature
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326'); // transform cloned feature to EPSG:4326

        ShowRectangle(e.feature);
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3857 = feature.getGeometry().getCoordinates();
        SetRectangleCoordinates(coords, coords3857);
        AddRectangleModifyIteraction();
    });
}

function AddRectangleModifyIteraction() {
    var modifyInteraction = new ol.interaction.Modify({
        source: centerSource
    });
    mapRectangle.addInteraction(modifyInteraction);
    modifyInteraction.on('modifyend', function (evt) {
        var collection = evt.features; // get features
        var featureClone = collection.item(0).clone(); // There's only one feature, so get the first and only one
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326'); //transform cloned feature to EPSG:4326

        ShowRectangle(collection.item(0));
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3857 = collection.item(0).getGeometry().getCoordinates();
        SetRectangleCoordinates(coords, coords3857);
    });
}

function ChangeRectangleMapSources() {
    if (rectInputs.abscissaCoordinate.val() && rectInputs.ordinateCoordinate.val()) {
        var coordinates =
            [
                parseFloat(rectInputs.abscissaCoordinate.val().replaceCommaToDot()),
                parseFloat(rectInputs.ordinateCoordinate.val().replaceCommaToDot())
            ];

        //Change coords on map
        coordinates = ol.proj.transform(coordinates, 'EPSG:4326', 'EPSG:3857');
        rectInputs.abscissa3857.val(coordinates[0]);
        rectInputs.ordinate3857.val(coordinates[1]);

        var point = new ol.geom.Point(
            coordinates
        );
        var featurePoint = new ol.Feature({
            geometry: point
        });
        centerSource.clear();
        centerSource.addFeature(featurePoint);

        ShowRectangle(featurePoint);
        AddRectangleModifyIteraction();

        //Set zoom to rectangle source
        var featureLength = rectanglerSource.getFeatures().length;
        if (featureLength > 0) {
            mapRectangle.getView().fit(rectanglerSource.getExtent(), mapRectangle.getSize());
        }
    }
    else {
        AddRectangleDrawInteraction();
    }
};

function SetRectangleCoordinates(coords, coords3857) {
    rectInputs.abscissaCoordinate.val(coords[0]);
    rectInputs.ordinateCoordinate.val(coords[1]);
    rectInputs.abscissa3857.val(coords3857[0]);
    rectInputs.ordinate3857.val(coords3857[1]);
}

function ShowRectangle(feature) {
    rectanglerSource.clear();
    let centerCoordinate = feature.getGeometry().getCoordinates();
    let sizeRect = GetSizeRectangle();
    let widthRect = sizeRect.length;
    let heightRect = sizeRect.width;
    let scale = 100; // 1 cm on map = 100 cm = 1m in real

    // calculation
    let widthInMeters = widthRect * (scale / 100);
    let heightInMeters = widthInMeters * (heightRect / widthRect);

    let pointRes = ol.proj.getPointResolution(mapRectangle.getView().getProjection(), 1, centerCoordinate);
    let widthInUnits = widthInMeters / pointRes;
    let heightInUnits = heightInMeters / pointRes;

    // coordinates of covered region
    let mapExtent = [
        centerCoordinate[0] - widthInUnits / 2,
        centerCoordinate[1] - heightInUnits / 2,
        centerCoordinate[0] + widthInUnits / 2,
        centerCoordinate[1] + heightInUnits / 2
    ];

    // and... the geometry
    var region = ol.geom.Polygon.fromExtent(mapExtent);
    var featureAirSource = new ol.Feature(region);
    rectanglerSource.addFeature(featureAirSource);
}

function GetSizeRectangle() {
    var length, width;

    //Get or set length
    if (rectInputs.sizeLength.val()) {
        length = parseFloat(rectInputs.sizeLength.val().replace(',', '.'));
    }
    else {
        length = 10;
        rectInputs.sizeLength.val(length);
    }

    //Get or set width
    if (rectInputs.sizeWidth.val()) {
        width = parseFloat(rectInputs.sizeWidth.val().replace(',', '.'));
    }
    else {
        width = 10;
        rectInputs.sizeWidth.val(width);
    }

    return {
        length: length,
        width: width
    }
}