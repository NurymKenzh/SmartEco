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
    var features;
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
                features = result.rectanglesFeatures;
            }
        },
        complete: function (data) {
            if (features) {
                DrawDispersionOnMap(features);
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
    console.log(turfFeatureCollection);
    var marker = objects.format.readFeatures(lines);
    layers.isolinesLayer.getSource().clear(true);
    layers.isolinesLayer.getSource().addFeatures(marker);

    SetMapZoomToExtent();
}

function SetMapZoomToExtent() {
    var featureLength = sources.isolinesSource.getFeatures().length;
    if (featureLength > 0) {
        objects.map.getView().fit(sources.isolinesSource.getExtent(), objects.map.getSize());
    }
}