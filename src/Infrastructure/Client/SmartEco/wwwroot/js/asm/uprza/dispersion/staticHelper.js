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
        var size = 100;
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

        return this.StringDivider(text, 16, '\n');
    }

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