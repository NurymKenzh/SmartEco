var map;
var isOrganized;
var inputs = { coordinates: {}, coordinates3857: {}, abscissaCoordinate: {}, ordinateCoordinate: {}, abscissa3857: {}, ordinate3857: {}, sizeLength: {}, sizeWidth: {} };

$('.show-map-btn').click(function (e) {
    InitializeInputs();
    InitializeMap();
});

$('.show-map').on('shown.bs.modal', function () {
    map.updateSize();
})

$('.clear-map').click(function (e) {
    airSource.clear();
    vectorAirSource.clear();
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
    inputs.abscissaCoordinate = $('#RectangleNewAbscissaX');
    inputs.ordinateCoordinate = $('#RectangleNewOrdinateY');
    inputs.abscissa3857 = $('#RectangleNewAbscissa3857');
    inputs.ordinate3857 = $('#RectangleNewOrdinate3857');
    inputs.sizeLength = $('#RectangleNewLength');
    inputs.sizeWidth = $('#RectangleNewWidth');
}

//Air source rectangle layer
var vectorAirSource = new ol.source.Vector();
var vectorAirLayer = new ol.layer.Vector({
    source: vectorAirSource
});

//Air pollution source layer
var airSource = new ol.source.Vector();
var airLayer = new ol.layer.Vector({
    source: airSource
});

function InitializeMap() {
    var target = 'map';
    $('#' + target).empty();
    airSource.clear();
    vectorAirSource.clear();

    //Initialize map
    map = new ol.Map({
        target: target,
        controls: ol.control.defaults().extend([new ol.control.FullScreen()]),
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            }),
            //vectorLayer,
            airLayer,
            vectorAirLayer
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([68.291, 47.5172]),
            zoom: 4
        })
    });

    ChangeMapSources();

    //Set zoom to rectangle source
    var featureLength = vectorAirSource.getFeatures().length;
    if (featureLength > 0) {
        map.getView().fit(vectorAirSource.getExtent(), map.getSize());
    }
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

        ShowRectangle(e.feature);
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

        ShowRectangle(collection.item(0));
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

        ShowRectangle(featurePoint);
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

function ShowRectangle(feature) {
    vectorAirSource.clear();
    let centerCoordinate = feature.getGeometry().getCoordinates();
    let sizeRect = GetSizeRectangle();
    let widthRect = sizeRect.length;
    let heightRect = sizeRect.width;
    let scale = 100; // 1 cm on map = 100 cm = 1m in real

    // calculation
    let widthInMeters = widthRect * (scale / 100);
    let heightInMeters = widthInMeters * (heightRect / widthRect);

    let pointRes = ol.proj.getPointResolution(map.getView().getProjection(), 1, centerCoordinate);
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
    vectorAirSource.addFeature(featureAirSource);
}

function GetSizeRectangle() {
    var length, width;

    //Get or set length
    if (inputs.sizeLength.val()) {
        length = parseFloat(inputs.sizeLength.val().replace(',', '.'));
    }
    else {
        length = 10;
        inputs.sizeLength.val(length);
    }

    //Get or set width
    if (inputs.sizeWidth.val()) {
        width = parseFloat(inputs.sizeWidth.val().replace(',', '.'));
    }
    else {
        width = 10;
        inputs.sizeWidth.val(width);
    }

    return {
        length: length,
        width: width
    }
}