//#region Initialize events

$('#ClearPolygonBtn').click(function () {
    ClearMap();
});

$("#AddRowBtn").click(function () {
    AddNewRow();
});

$("#ApplyCoordsBtn").click(function () {
    ApplyCoords();
});


$('.collapse').on('shown.bs.collapse', function () {
    AdjustTable();
})

//#endregion Initialize buttons

var vectorSource = new ol.source.Vector();
var vectorLayer = new ol.layer.Vector({
    source: vectorSource
});
var drawInteraction = new ol.interaction.Draw({ source: vectorSource, type: "Polygon" });
var selectInteraction = new ol.interaction.Select();

var map = new ol.Map({
    target: 'map',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        }),
        vectorLayer
    ],
    view: new ol.View({
        center: ol.proj.fromLonLat([68.291, 47.5172]),
        zoom: 4
    })
});

var formatGeoJSON = new ol.format.GeoJSON();
var feature = new ol.Feature();

var polyCoords = [];
SetPolyCoords();

if (polyCoords.length > 0) {
    ModifyPolygon();
    var coords = GetCoords();
    if (coords.length > 0)
        SetTableValues(coords);
}
else {
    DrawPolygon();
}

function GetCoords() {
    var coords = [];
    $("input[name='Coordinates']").each(function () {
        var coord = this.value.split(',');
        coords.push(coord);
    });
    return coords;
}

function SetPolyCoords() {
    polyCoords = [];
    $("input[name='Coordinates']").each(function () {
        var coord = this.value.split(',');
        polyCoords.push(ol.proj.transform([parseFloat(coord[0]), parseFloat(coord[1])], 'EPSG:4326', 'EPSG:3857'));
    });
}

function ModifyPolygon() {
    feature = new ol.Feature({
        geometry: new ol.geom.Polygon([polyCoords])
    })
    vectorSource.addFeature(feature);

    SetMapZoomToExtent();

    selectInteraction = new ol.interaction.Select();
    map.addInteraction(selectInteraction);
    // select feature:
    selectInteraction.getFeatures().push(feature);
    modifyInteraction = new ol.interaction.Modify({ features: selectInteraction.getFeatures() });
    map.addInteraction(modifyInteraction);
    modifyInteraction.on('modifyend', function (evt) {
        // get features:
        var collection = evt.features;
        // There's only one feature, so get the first and only one:
        var featureClone = collection.item(0).clone();
        // transform cloned feature to WGS84:
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326');
        // get GeoJSON of feature:
        var geojson = formatGeoJSON.writeFeature(featureClone);
        // save or do whatever...
        var coords = featureClone.getGeometry().getCoordinates();
        SetCoordinates(coords[0]);
        SetTableValues(coords[0]);
    });
}

function DrawPolygon() {
    map.addInteraction(drawInteraction);

    drawInteraction.on('drawend', function (e) {
        feature = e.feature;
        // remove draw interaction:
        map.removeInteraction(drawInteraction);
        // Create a select interaction and add it to the map:
        selectInteraction = new ol.interaction.Select();
        map.addInteraction(selectInteraction);
        // select feature:
        selectInteraction.getFeatures().push(feature);
        // clone feature:
        var featureClone = feature.clone();
        // transform cloned feature to WGS84:
        featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326');
        // get GeoJSON of feature:
        var geojson = formatGeoJSON.writeFeature(featureClone);
        // save or do whatever...
        var coords = featureClone.getGeometry().getCoordinates();
        SetCoordinates(coords[0]);
        SetTableValues(coords[0]);

        // Create a modify interaction and add to the map:
        modifyInteraction = new ol.interaction.Modify({ features: selectInteraction.getFeatures() });
        map.addInteraction(modifyInteraction);
        // set listener on "modifyend":
        modifyInteraction.on('modifyend', function (evt) {
            // get features:
            var collection = evt.features;
            // There's only one feature, so get the first and only one:
            var featureClone = collection.item(0).clone();
            // transform cloned feature to WGS84:
            featureClone.getGeometry().transform('EPSG:3857', 'EPSG:4326');
            // get GeoJSON of feature:
            var geojson = formatGeoJSON.writeFeature(featureClone);
            // save or do whatever...
            var coords = featureClone.getGeometry().getCoordinates();
            SetCoordinates(coords[0]);
            SetTableValues(coords[0]);
        });
    });
}

function SetMapZoomToExtent() {
    var featureLength = vectorSource.getFeatures().length;
    if (featureLength > 0) {
        map.getView().fit(vectorSource.getExtent(), map.getSize());
    }
}

function SetCoordinates(coords) {
    $('#ContainerCoordinates').empty();
    $.each(coords, function (index, coord) {
        $('#ContainerCoordinates').append('<input type="hidden" name="Coordinates" value="' + coord + '" />');
    });
}

function ClearMap () {
    vectorLayer.getSource().clear();
    if (selectInteraction) {
        selectInteraction.getFeatures().clear();
    }
    map.removeInteraction(selectInteraction);

    vectorLayer = new ol.layer.Vector({
        source: vectorSource
    });
    map.addInteraction(drawInteraction);

    SetCoordinates();
    SetTableValues();
};

function AddNewRow(longitude, latitude) {
    var rowCount = GetRowCount();
    var rowNumber = ++rowCount;
    longitude = longitude ?? '';
    latitude = latitude ?? '';
    var rowHtml = '<tr><td>' + rowNumber +'</td>'
        + '<td><input type="text" value="' + longitude +'" /></td>'
        + '<td><input type="text" value="' + latitude +'" /></td>'
        + '<td><button id="DeleteRowBtn" type="button" class="btn text-danger" title="Удалить" onclick="DeleteRow(this)"><i class="fa-solid fa-trash fa-lg"></i></button></td></tr>';
    $("#CoordinatesTableBody").append(rowHtml);
    AdjustTable();
}

function DeleteRow(element) {
    var rowCount = GetRowCount();
    if (rowCount <= 0) {
        return;
    }
    if (element) {
        //delete specific row
        var rowNumber = $(element).parents("tr").first().text();
        var nextRows = $(element).parents("tr").nextAll();
        //change numeration next rows
        nextRows.each(function (i, obj) {
            $(this).find('td').first().text(rowNumber++);
        });
        $(element).parents("tr").remove();
    }
    else {
        //delete last row
        tableBody.deleteRow(rowCount - 1);
    }
    AdjustTable();
}

function AdjustTable() {
    let maxHeight = screen.height / 2.2;
    if ($('#CoordinatesTableBody').height() >= maxHeight)
        $('table').addClass('table-scroll');
    else
        $('table').removeClass('table-scroll');
}

function GetRowCount() {
    var tableBody = $('#CoordinatesTableBody')[0];
    return tableBody.rows.length;
}

function ApplyCoords() {
    var rows = $('#CoordinatesTableBody').find('tr');
    var coords = [];
    rows.each(function (i, obj) {
        var longitude = $(this).find('input').first().val();
        var latitude = $(this).find('input').last().val();
        if (longitude != '' && latitude != '') {
            var coord = []
            coord.push(longitude);
            coord.push(latitude);
            coords.push(coord);
        }
    });
    ClearMap();
    SetCoordinates(coords);
    SetTableValues(coords);
    SetPolyCoords();
    if (polyCoords.length > 0) {
        ModifyPolygon();
    }
}

function SetTableValues(values) {
    $('#CoordinatesTableBody').empty();
    $.each(values, function (index, value) {
        var longitude = value[0];
        var latitude = value[1];
        AddNewRow(longitude, latitude);
    });
}