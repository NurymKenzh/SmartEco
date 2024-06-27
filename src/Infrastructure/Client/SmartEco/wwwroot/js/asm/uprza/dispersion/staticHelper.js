export class IsolineStyle {

    static maxPdkForIsolines;
    static minPdkForIsolines;
    static COUNT_STEPS;


    static Perc2colorForIsolines(pdk, opacity) {
        let color;
        if (this.maxPdkForIsolines === this.minPdkForIsolines) {
            color = pdk > this.maxPdkForIsolines ? 255 : 0;
        } else {
            color = (
                (pdk - this.minPdkForIsolines) /
                ((this.maxPdkForIsolines - this.minPdkForIsolines) / (this.COUNT_STEPS * 2))
            ) * (255 / this.COUNT_STEPS);
        }
        return 'rgba(' + (0 + color) + ',' + (255 - color) + ',' + (0) + ', ' + opacity + ')';
    }

    static CreateTextStyle(feature, resolution, dom) {
        var align = dom.align;
        var baseline = dom.baseline;
        var size = dom.size;
        var offsetX = parseInt(dom.offsetX, 10);
        var offsetY = parseInt(dom.offsetY, 10);
        var weight = dom.weight;
        var placement = dom.placement ? dom.placement : undefined;
        var maxAngle = dom.maxangle ? parseFloat(dom.maxangle) : undefined;
        var overflow = dom.overflow ? (dom.overflow.value === 'true') : undefined;
        var rotation = parseFloat(dom.rotation);
        var font = weight + ' ' + size + ' ' + dom.font;
        var fillColor = dom.color;
        var outlineColor = dom.outline;
        var outlineWidth = parseInt(dom.outlineWidth, 10);

        return new ol.style.Text({
            textAlign: align === '' ? undefined : align,
            textBaseline: baseline,
            font: font,
            text: this.GetText(feature, resolution, dom),
            fill: new ol.style.Fill({ color: fillColor }),
            stroke: new ol.style.Stroke({ color: outlineColor, width: outlineWidth }),
            offsetX: offsetX,
            offsetY: offsetY,
            placement: placement,
            maxAngle: true,
            overflow: overflow,
            rotation: rotation
        });
    }

    static GetText(feature, resolution, dom) {
        var text = dom.getPropertyFunction(feature);
        var isMpcChecked = $('#MpcCheckbox').is(':checked');
        if (isMpcChecked)
            return CommonStyle.StringDivider(text, 16, '\n');
        else
            return null;
    }
}

export class IsobandStyle {
    static Perc2colorForIsobands(pdkRange, opacity) {
        let color;
        let pdk = parseFloat(pdkRange.split('-')[0]);

        if (pdk < 0.2) { color = '0, 255, 0' }
        else if (pdk < 0.5) { color = '255, 255, 0' }
        else if (pdk < 1) { color = '255, 100, 0' }
        else { color = '100, 0, 255' }

        return 'rgba(' + color + ', ' + opacity + ')';
    }
}

export class BorderStyle {
    static dom = {
        polygons: {
            text: 'wrap',
            align: '',
            baseline: 'middle',
            rotation: '0',
            font: '\'Verdana\'',
            weight: 'normal',
            placement: 'line',
            maxangle: '0.1',
            overflow: 'false',
            size: '10px',
            height: '1',
            offsetX: '0',
            offsetY: '0',
            color: 'black',
            outline: '#ffffff',
            outlineWidth: '3',
            maxreso: '20',
        },
    };

    static GetIndSiteBorderStyle(borderText) {
        return new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'black',
                width: 2,
            }),
            fill: new ol.style.Fill({
                color: 'rgba(100, 100, 100, 0.1)',
            }),
            text: this.CreateTextStyle(borderText)
        })
    }

    static GetSanZoneBorderStyle(borderText) {
        return new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'red',
                lineDash: [10],
                width: 2,
            }),
            fill: new ol.style.Fill({
                color: 'rgba(0, 0, 0, 0)',
            }),
            text: this.CreateTextStyle(borderText)
        })
    }

    static CreateTextStyle(text) {
        var align = this.dom.polygons.align;
        var baseline = this.dom.polygons.baseline;
        var size = this.dom.polygons.size;
        var offsetX = parseInt(this.dom.polygons.offsetX, 10);
        var offsetY = parseInt(this.dom.polygons.offsetY, 10);
        var weight = this.dom.polygons.weight;
        var placement = this.dom.polygons.placement ? this.dom.polygons.placement : undefined;
        var maxAngle = this.dom.polygons.maxangle ? parseFloat(this.dom.polygons.maxangle) : undefined;
        var overflow = this.dom.polygons.overflow ? (this.dom.polygons.overflow.value === 'true') : undefined;
        var rotation = parseFloat(this.dom.polygons.rotation);
        var font = weight + ' ' + size + ' ' + this.dom.polygons.font;
        var fillColor = this.dom.polygons.color;
        var outlineColor = this.dom.polygons.outline;
        var outlineWidth = parseInt(this.dom.polygons.outlineWidth, 10);

        return new ol.style.Text({
            textAlign: align === '' ? undefined : align,
            textBaseline: baseline,
            font: font,
            text: this.GetText(text),
            fill: new ol.style.Fill({ color: fillColor }),
            stroke: new ol.style.Stroke({ color: outlineColor, width: outlineWidth }),
            offsetX: offsetX,
            offsetY: offsetY,
            placement: placement,
            maxAngle: true,
            overflow: overflow,
            rotation: rotation
        });
    }

    static GetText(text) {
        var isBordersChecked = $('#MarkerBordersCheckbox').is(':checked');
        if (isBordersChecked)
            return CommonStyle.StringDivider(text, 16, '\n');
        else
            return null;
    }
}

export class ApsStyle {
    static dom = {
        polygons: {
            text: 'wrap',
            align: '',
            baseline: 'middle',
            rotation: '0',
            font: '\'Verdana\'',
            weight: 'normal',
            placement: 'point',
            maxangle: '0.1',
            overflow: 'false',
            size: '10px',
            height: '1',
            offsetX: '0',
            offsetY: '15',
            color: 'black',
            outline: '#ffffff',
            outlineWidth: '3',
            maxreso: '20',
        },
    };

    static CreateTextStyle(feature) {
        var text = feature.get('name');
        var align = this.dom.polygons.align;
        var baseline = this.dom.polygons.baseline;
        var size = this.dom.polygons.size;
        var offsetX = parseInt(this.dom.polygons.offsetX, 10);
        var offsetY = parseInt(this.dom.polygons.offsetY, 10);
        var weight = this.dom.polygons.weight;
        var placement = this.dom.polygons.placement ? this.dom.polygons.placement : undefined;
        var maxAngle = this.dom.polygons.maxangle ? parseFloat(this.dom.polygons.maxangle) : undefined;
        var overflow = this.dom.polygons.overflow ? (this.dom.polygons.overflow.value === 'true') : undefined;
        var rotation = parseFloat(this.dom.polygons.rotation);
        var font = weight + ' ' + size + ' ' + this.dom.polygons.font;
        var fillColor = this.dom.polygons.color;
        var outlineColor = this.dom.polygons.outline;
        var outlineWidth = parseInt(this.dom.polygons.outlineWidth, 10);

        return new ol.style.Text({
            textAlign: align === '' ? undefined : align,
            textBaseline: baseline,
            font: font,
            text: this.GetText(text),
            fill: new ol.style.Fill({ color: fillColor }),
            stroke: new ol.style.Stroke({ color: outlineColor, width: outlineWidth }),
            offsetX: offsetX,
            offsetY: offsetY,
            placement: placement,
            maxAngle: true,
            overflow: overflow,
            rotation: rotation
        });
    }

    static GetText(text) {
        var isApsChecked = $('#ApsCheckbox').is(':checked');
        if (isApsChecked)
            return CommonStyle.StringDivider(text, 16, '\n');
        else
            return null;
    }
}

class CommonStyle {
    static StringDivider(str, width, spaceReplacer) {
        if (str.length > width) {
            let p = width;
            while (p > 0 && (str[p] !== ' ' && str[p] !== '-')) {
                p--;
            }
            if (p > 0) {
                let left;
                if (str.substring(p, p + 1) === '-') {
                    left = str.substring(0, p + 1);
                } else {
                    left = str.substring(0, p);
                }
                const right = str.substring(p + 1);
                return left + spaceReplacer + this.StringDivider(right, width, spaceReplacer);
            }
        }
        return str;
    }
}