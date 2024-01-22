var map, inputCoordinates, inputLongCoordinate, inputLatCoordinate, inputWidth, inputLength;
var isOrganized;
var inputs = { coordinates: {}, coordinates3857: {}, longCoordinate: {}, latCoordinate: {}, sizeLength: {}, sizeWidth: {} };

$('.edit-info-btn').click(function (e) {
    var btn = $(this);
    var editRow = GetEditRow(btn);
    var sourceId = editRow.find('[name="IdSource"]').val();
    var indSiteId = editRow.find('[name="RelationSource"]').find(':selected').data('indsite');
    var coordinates = editRow.find('[name="RelationSource"]').find(`option[value=indSiteEnterprise_${indSiteId}]`).data('coordinates');
    isOrganized = editRow.find('[name="IsOrganizedSource"]').val();
    InitializeInputs(editRow);
    InitializeMap(sourceId, coordinates);
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

function InitializeInputs(editRow) {
    inputs.coordinates = editRow.find('[name="CoordinateInfo"]');
    inputs.coordinates3857 = editRow.find('[name="Coordinate3857Info"]');
    inputs.longCoordinate = editRow.find('[name="CoordinateLongInfo"]');
    inputs.latCoordinate = editRow.find('[name="CoordinateLatInfo"]');
    inputs.sizeLength = editRow.find('[name="LengthInfo"]');
    inputs.sizeWidth = editRow.find('[name="WidthInfo"]');
    if (inputs.coordinates.val()) {
        var coordinatesSplit = inputs.coordinates.val().split(',');
        inputs.longCoordinate.val(coordinatesSplit[0]);
        inputs.latCoordinate.val(coordinatesSplit[1]);
    }

    //Changing inputs depending on source type
    if (isOrganized == 'true') {
        $('.is-organized-block').prop('hidden', false);
        $('.isnot-organized-block').prop('hidden', true);
    }
    else {
        $('.is-organized-block').prop('hidden', true);
        $('.isnot-organized-block').prop('hidden', false);
    }
}

//Industrial site layer
var vectorSource = new ol.source.Vector();
var vectorLayer = new ol.layer.Vector({
    source: vectorSource
});

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

function InitializeMap(sourceId, coordinates) {
    var target = 'map_' + sourceId;
    $('#' + target).empty();
    airSource.clear();
    vectorAirSource.clear();

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
            airLayer,
            vectorAirLayer
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([68.291, 47.5172]),
            zoom: 4
        })
    });

    ChangeMapSources();

    //Set zoom to industrial site
    var featureLength = vectorSource.getFeatures().length;
    if (featureLength > 0) {
        map.getView().fit(vectorSource.getExtent(), map.getSize());
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

        CheckShowClearRectangle(e.feature);
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3875 = feature.getGeometry().getCoordinates();
        SetCoordinates(coords, coords3875);
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

        CheckShowClearRectangle(collection.item(0));
        var coords = featureClone.getGeometry().getCoordinates();
        var coords3875 = collection.item(0).getGeometry().getCoordinates();
        SetCoordinates(coords, coords3875);
    });
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

function ChangeMapSources() {
    if (inputs.longCoordinate.val() && inputs.latCoordinate.val()) {
        var coordinates = [parseFloat(inputs.longCoordinate.val()), parseFloat(inputs.latCoordinate.val())];

        //Set coordinates to inputs
        inputs.coordinates.val(coordinates);

        //Change coords on map
        coordinates = ol.proj.transform(coordinates, 'EPSG:4326', 'EPSG:3857');
        inputs.coordinates3857.val(coordinates);

        var point = new ol.geom.Point(
            coordinates
        );
        var featurePoint = new ol.Feature({
            geometry: point
        });
        airSource.clear();
        airSource.addFeature(featurePoint);

        CheckShowClearRectangle(featurePoint);
        AddModifyIteraction();
    }
    else {
        AddDrawInteraction();
    }
};

function CheckShowClearRectangle(feature) {
    if (isOrganized == 'false') {
        ShowRectangle(feature);
    }
    else {
        ClearSizeRectangle();
    }
}

function SetCoordinates(coords, coords3857) {
    inputs.coordinates.val(coords);
    inputs.coordinates3857.val(coords3857);
    inputs.longCoordinate.val(coords[0]);
    inputs.latCoordinate.val(coords[1]);
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

function ClearSizeRectangle() {
    inputs.sizeLength.val('');
    inputs.sizeWidth.val('');
}