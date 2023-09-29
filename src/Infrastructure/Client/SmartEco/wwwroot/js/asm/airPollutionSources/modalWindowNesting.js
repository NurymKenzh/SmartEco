//handle modal windows nested up to 4 depths

var modal_lv = 0;
$('body').on('show.bs.modal', function (e) {
    if (modal_lv > 0)
        $(e.target).css('zIndex', 1051 + modal_lv);
    modal_lv++;
}).on('hidden.bs.modal', function () {
    if (modal_lv > 0)
        modal_lv--;
});