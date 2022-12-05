$(document).ready(function () {
    $("a").each(function (i, $elem) {
        var $elemIcon = $($elem).find('i.fa').fadeIn();
        if ($elemIcon.length == 1) {
            var $elem = $(this);
            var currentSort = $elem.data("currentSort");
            var sort = $elem.data("sort");
            var $elemIcon = $(this).find('i.fa').fadeIn();

            var sortDefault = 'fa-sort';
            var sortAsc = 'fa-sort-asc';
            var sortDesc = 'fa-sort-desc';

            if (~currentSort.indexOf(sort) || ~sort.indexOf(currentSort)) {
                $elemIcon
                    .toggleClass(sortDefault, currentSort.trim() && !(/desc/i.test(sort)))
                    .toggleClass(sortAsc, !!currentSort.trim() && (/desc/i.test(sort)))
                    .toggleClass(sortDesc, !!currentSort.trim() && !(/desc/i.test(sort)));
            }
            else {
                $elemIcon
                    .toggleClass(sortDefault)
            }
        }
    });
});