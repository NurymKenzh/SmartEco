var page = document.currentScript.dataset.page;
var selectedTabKey = 'selected' + page + 'Tab';

$('a[data-toggle="tab"]').click(function (e) {
    e.preventDefault();
    $(this).tab('show');
});

$('a[data-toggle="tab"]').on("shown.bs.tab", function (e) {
    var id = $(e.target).attr("href");
    localStorage.setItem(selectedTabKey, id)
});

var selectedTab = localStorage.getItem(selectedTabKey);
if (selectedTab != null) {
    $('a[data-toggle="tab"][href="' + selectedTab + '"]').tab('show');
}
else {
    var firstTab = $('a[data-toggle="tab"]:first');
    var href = firstTab.prop("hash");
    $('a[data-toggle="tab"][href="' + href + '"]').tab('show');
}