import { ApsStyle, BorderStyle, IsolineStyle } from './staticHelper.js';

//Initializing map
$(function () {
    InitializeMap();
    InitializeAirSources();
    InitializeIndSites();
});

export let objects = {
    map: new ol.Map({}),
    format: new ol.format.GeoJSON(),
    pointsFeatures: []
}

export let sources = {
    isolinesSource: SetIsolinesSource(),
    pointsSource: {}
}

export let layers = {
    isolinesLayer: SetIsolinesLayer(),
    pointsLayer: {},
    airSourcesLayer: {}
}

//Isolines
var linesProperties = {
    text: 'wrap',
    align: '',
    baseline: 'middle',
    rotation: '0',
    font: '\'Courier New\'',
    weight: 'normal',
    placement: 'point',
    maxangle: '0.1',
    overflow: 'false',
    size: '8px',
    offsetX: '0',
    offsetY: '0',
    color: 'green',
    outline: '#ffffff',
    outlineWidth: '3',
    maxreso: '20',
    getPropertyFunction: function (feature) {
        return (Math.round(+feature.get('c_pdk') * 100) / 100).toString();
    }
};

function InitializeMap() {
    var target = 'Map';
    //Initialize map
    objects.map = new ol.Map({
        target: target,
        controls: ol.control.defaults().extend([new ol.control.FullScreen()]),
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            }),
            layers.isolinesLayer
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([68.291, 47.5172]),
            zoom: 5
        })
    });
}

function SetIsolinesSource() {
    return new ol.source.Vector({
        format: objects.format
    });
}

function SetIsolinesLayer() {
    return new ol.layer.Vector({
        source: sources.isolinesSource,
        name: 'Isolines',
        style: (feature, resolution) => {
            const color = IsolineStyle.Perc2colorForIsolines(parseFloat(feature.get('c_pdk')), 1);
            return new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: color,
                    width: 2
                }),
                text: IsolineStyle.CreateTextStyle(feature, resolution, linesProperties)
            });
        }
    });
}

function InitializeAirSources() {
    var sourcesFeatures = [];
    $('input[id^="AirSourceCheckbox_"]').each(function () {  // Matches those that begin with 'AirSourceCheckbox_'
        var coordinates = $(this).data('coordinates');
        var name = $(this).data('name');
        if (coordinates) {
            var coordinates4326 =
                [
                    parseFloat(coordinates.split(',')[0]),
                    parseFloat(coordinates.split(',')[1])
                ];
            var coordinates3857 = ol.proj.transform(coordinates4326, 'EPSG:4326', 'EPSG:3857');
            sourcesFeatures.push(new ol.Feature({
                'name': name,
                'geometry': new ol.geom.Point(coordinates3857)
            }));
        }
    });

    var airSourcesSource = new ol.source.Vector({
        features: sourcesFeatures
    });

    layers.airSourcesLayer = new ol.layer.Vector({
        source: airSourcesSource,
        name: 'Points',
        style: (feature) => {
            return new ol.style.Style({
                image: new ol.style.Icon(({
                    src: '/images/ASM/Icons/redCross.png'
                })),
                text: ApsStyle.CreateTextStyle(feature)
            })
        }
    });

    objects.map.addLayer(layers.airSourcesLayer);
}

function InitializeIndSites() {
    $('input[id^="IndSiteBorderCheckbox_"]').each(function () {  // Matches those that begin with 'IndSiteBorderCheckbox_'
        var borderCoordinates = $(this).data('coordinates');
        if (borderCoordinates) {
            //Industrial site border layer
            var inputId = $(this).attr('id');
            var borderText = $('label[for="' + inputId + '"]').text().trim();
            SetBorderLayer(borderCoordinates, BorderStyle.GetIndSiteBorderStyle(borderText), inputId);
        }
    });
    $('input[id^="SanZoneBorderCheckbox_"]').each(function () {  // Matches those that begin with 'SanZoneBorderCheckbox_'
        var borderCoordinates = $(this).data('coordinates');
        if (borderCoordinates) {
            //Industrial site border layer
            var inputId = $(this).attr('id');
            var borderText = $('label[for="' + inputId + '"]').text().trim();
            SetBorderLayer(borderCoordinates, BorderStyle.GetSanZoneBorderStyle(borderText), inputId);
        }
    });
}

function SetBorderLayer(borderCoordinates, borderStyle, name) {
    var vectorSource = new ol.source.Vector();
    var vectorLayer = new ol.layer.Vector({
        source: vectorSource,
        style: borderStyle,
        name: name
    });

    var polyCoords = ParcePolyCoordinates(borderCoordinates);
    var feature = new ol.Feature({
        geometry: new ol.geom.Polygon([polyCoords])
    })
    vectorSource.addFeature(feature);
    objects.map.addLayer(vectorLayer);
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