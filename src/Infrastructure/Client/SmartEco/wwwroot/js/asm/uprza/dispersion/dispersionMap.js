import { IsolineStyle } from './staticHelper.js';
import { objects, layers, sources } from './initializeLayers.js'

//Initializing dispersion
$(function () {
    ChangeAirPollutant();
});

//Setting dispersion by air pollutant code
function ChangeAirPollutant() {
    var pollutantCode = $('#AirPollutantsSelect').val();
    GetEmissionByCode(pollutantCode);
}

$('#AirPollutantsSelect').on("change", function () {
    ChangeAirPollutant();
});

function GetEmissionByCode(pollutantCode) {
    var rectanglesFeatures, pointsFeatures;
    var calculationId = $('#CalculationId').val();
    $.ajax({
        data: {
            calculationId: calculationId,
            pollutantCode: pollutantCode
        },
        url: $('#EmissionByCodeGetReq').data('url'),
        type: 'GET',
        success: function (result) {
            if (result) {
                rectanglesFeatures = result.rectanglesFeatures;
                pointsFeatures = result.pointsFeatures;
            }
        },
        complete: function (data) {
            $('#IsolinesCheckboxBlock').attr('hidden', true);
            $('#PointsCheckboxBlock').attr('hidden', true);

            if (rectanglesFeatures) {
                $('#IsolinesCheckboxBlock').attr('hidden', false);
                DrawDispersionOnMap(rectanglesFeatures);
            }

            if (pointsFeatures) {
                $('#PointsCheckboxBlock').attr('hidden', false);
                DrawPointsOnMap(pointsFeatures);
            }
        }
    });
}

function DrawDispersionOnMap(features) {
    var turfFeatureCollection = JSON.parse(features);
    IsolineStyle.COUNT_STEPS = Math.round(turfFeatureCollection.features.length / 100);
    const minAndMax = turfFeatureCollection.features.reduce(
        (values, feature) => {
            const c = feature.properties['c_pdk'];

            if (values.max < c) {
                values.max = c;
            }

            if (values.min > c) {
                values.min = c;
            }

            return values;
        },
        { min: Number.MAX_SAFE_INTEGER, max: 0 }
    );
    const step = (minAndMax.max - minAndMax.min) / IsolineStyle.COUNT_STEPS;
    IsolineStyle.maxPdkForIsolines = minAndMax.max;
    IsolineStyle.minPdkForIsolines = minAndMax.min;
    const breaks = [];
    breaks.push(0);
    if (step > 0) {
        for (let pdk = minAndMax.min; pdk <= minAndMax.max; pdk += step) {
            breaks.push(pdk);
        }

        breaks.push(1);
    }
    breaks.push(minAndMax.max);
    const lines = turf.isolines(turfFeatureCollection, breaks, { zProperty: 'c_pdk' });
    var smoothedLines = SmoothingIsolines(lines);
    var marker = objects.format.readFeatures(smoothedLines);
    layers.isolinesLayer.getSource().clear(true);
    layers.isolinesLayer.getSource().addFeatures(marker);

    const bands = turf.isobands(turfFeatureCollection, breaks, { zProperty: 'c_pdk' });
    bands.features.shift();
    var smoothedBands = SmoothingIsobandes(bands);
    var markerBands = objects.format.readFeatures(smoothedBands);
    layers.isobandsLayer.getSource().clear(true);
    layers.isobandsLayer.getSource().addFeatures(markerBands);

    SetMapZoomToExtent();
}

function SmoothingIsolines(lines) {
    try {
        var geojsonObject = {
            type: 'FeatureCollection',
            features: []
        };

        $.each(lines.features, function (index, line) {
            if (line.geometry.coordinates.length === 0)
                return;

            var polygon = turf.lineToPolygon(line);
            var smoothed = turf.polygonSmooth(polygon, { iterations: 5 })
            var line = turf.polygonToLine(smoothed.features.find(e => typeof e !== 'undefined'));
            if (line) {
                geojsonObject['features'].push(line);
            }
        });
        return geojsonObject;
    }
    catch (err) {
        return lines;
    }
}

function SmoothingIsobandes(bands) {
    try {
        var geojsonObject = {
            type: 'FeatureCollection',
            features: []
        };

        $.each(bands.features, function (index, band) {
            if (band.geometry.coordinates.length === 0)
                return;

            var bandCoords = [];
            $.each(band.geometry.coordinates, function (index, coord) {
                bandCoords.push({
                    type: "Feature",
                    geometry: {
                        type: "MultiPolygon",
                        coordinates: [coord]
                    },
                    properties: {
                        c_pdk: band.properties.c_pdk
                    }
                })
            });

            $.each(bandCoords, function (index, bandCoord) {
                var smoothed = turf.polygonSmooth(bandCoord, { iterations: 5 })
                var bandSmoothed = smoothed.features.find(e => typeof e !== 'undefined');
                if (bandSmoothed) {
                    geojsonObject['features'].push(bandSmoothed);
                }
            });
        });
        return geojsonObject;
    }
    catch (err) {
        return bands;
    }
}


function SetMapZoomToExtent() {
    var featureLength = sources.isolinesSource.getFeatures().length;
    if (featureLength > 0) {
        objects.map.getView().fit(sources.isolinesSource.getExtent(), objects.map.getSize());
    }
}

var textProperties = {
    text: 'wrap',
    align: '',
    baseline: 'middle',
    rotation: '0',
    font: '\'Courier New\'',
    weight: 'normal',
    placement: 'point',
    maxangle: '0.1',
    overflow: 'false',
    size: '12px',
    offsetX: '0',
    offsetY: '15',
    color: 'black',
    outline: '#ffffff',
    outlineWidth: '3',
    maxreso: '20',
    getPropertyFunction: function (feature) {
        return (Math.round(+feature.get('c_pdk') * 100) / 100).toString();
    }
};

function DrawPointsOnMap(features) {
    var pointsFeatures = JSON.parse(features);
    $.each(pointsFeatures.features, function (index, feature) {
        objects.pointsFeatures.push(new ol.Feature({
            'geometry': new ol.geom.Point([
                feature.geometry.coordinates[0],
                feature.geometry.coordinates[1],
            ]),
            'c_pdk': feature.properties['c_pdk']
        }));
    });

    sources.pointsSource = new ol.source.Vector({
        features: objects.pointsFeatures
    });

    layers.pointsLayer = new ol.layer.Vector({
        source: sources.pointsSource,
        name: 'Points',
        style: (feature, resolution) => {
            return new ol.style.Style({
                image: new ol.style.Icon(({
                    src: '/images/ASM/Icons/blackTriangle.png'
                })),
                text: IsolineStyle.CreateTextStyle(feature, resolution, textProperties)
            });
        }
    });

    objects.map.addLayer(layers.pointsLayer);
}