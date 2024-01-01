//select class to selected category
$(".btn").on("click",function () {
    $(".btn").removeClass("selected");
    $(this).addClass("selected")
})
//filter Selected category
function filterSelection(category) {
    let filterItems = $(".filterDiv")
    if (category == "All") {
        return $(".filterDiv").addClass("show")
    }
    else {
        filterItems.removeClass("show");
        $(`.${category}`).addClass("show");
    }
}
//Initialise
filterSelection('All')