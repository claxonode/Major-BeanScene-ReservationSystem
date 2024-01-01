function allowDrop(ev) {
    ev.preventDefault();
}
function drag(ev) {

    ev.dataTransfer.setData("text", ev.target.id);
}
function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    ev.target.appendChild(document.getElementById(data));
    debugger;
}
function dropAtTable(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    let reservationId = $("#" + data).attr("data-resId")
    let tableId = ev.target.getAttribute("data-tableId")

    fetch(`/api/Reservations/AssignTable?TableId=${tableId}&ReservationId=${reservationId}`, {
        method: "POST",
        redirect: "Staff/Home/AssignTablesToReservations"
    }).then(response => {
        return response.text();
    }).then(data => {
        console.log(data)
    }).catch(error => {
        console.error('Error:', error);
    })


}
