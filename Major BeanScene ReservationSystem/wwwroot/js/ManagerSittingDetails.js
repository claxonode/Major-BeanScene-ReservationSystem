$(document).ready(function () {
    $("tr").click(function () {

        let currentTds = $(this).closest("tr").find("td"); // find all td of selected row
        let data = $(currentTds).parent().attr("data-bs-data")

        //let item = $(currentTds).parent().attr("data-itemdata")
        let splitData = data.split(';');
        let notes = $(currentTds).parent().attr("data-bs-notes");
        let guestInfo = $(currentTds).parent().attr("data-bs-guest")
        let splitGuestInfo = guestInfo.split(';')


        let firstName = $(currentTds).eq(0).text(); // eq= cell , text = inner text
        let lastName = $(currentTds).eq(1).text();
        let email = $(currentTds).eq(2).text();
        let phone = $(currentTds).eq(3).text();
        let guest = $(currentTds).eq(4).text();

        let [reservationId,personId,reservationStart,sittingId,duration,origin,status] = splitData
        let pathRoute = window.location.pathname.split('/')[4]

        $("#Id").val(reservationId)
        $("#PersonId").val(personId)
        $("#Route").val(pathRoute)
        $('#FirstName').text(firstName)
        $('#LastName').text(lastName)
        $('#Email').text(email)
        $('#PhoneNumber').text(phone)

        let isoTimeLetterT= reservationStart.indexOf("T")
        let startDate = reservationStart.substring(0, isoTimeLetterT);
        let startTime = reservationStart.substring(isoTimeLetterT + 1, reservationStart.length)
        $('#SelectedDate').val(startDate)
        fetchSittings(startDate, sittingId) //the sittingId is chosen
        initialiseTime(Number(sittingId), startTime); //the startTime is chosen
        updateSelectTagForNumberOfGuest(sittingId,guest)
 
        $("#SelectedTime").val(startTime)
        
        $('#Duration').val(duration)
        $('#Guests').val(guest)
        $('#OriginId').val(origin)
        $('#StatusId').val(status)
        $('#AdditionalNotes').val(notes)

        let guestFirstNameForm = `<div class="form-group">
            <label for="GuestFullName" class="control-label">Guest First Name</label>
            <span name="GuestFullName" id="GuestFullName" class="form-control">${splitGuestInfo[0]}</span>
        </div>`;
        let guestLastNameForm = `<div class="form-group">
            <label for="GuestFullName" class="control-label">Guest First Name</label>
            <span name="GuestFullName" id="GuestFullName" class="form-control">${splitGuestInfo[1]}</span>
        </div>`;
        let guestEmailForm = `<div class="form-group">
                                <label class="control-label" for="GuestEmail">Guest Email</label>
                            <span name="GuestEmail" id="GuestEmail" class="form-control">${splitGuestInfo[2]}</span>
                            </div>`
        let guestPhoneForm = `<div class="form-group">
                            <label class="control-label" for="GuestPhoneNumber">Guest Phone</label>
                            <span name="GuestPhoneNumber" id="GuestPhoneNumber" class="form-control">${splitGuestInfo[3]}</span>
                            </div>`
        if (guestInfo !== "") {
            $("#GuestFirstName").html(guestFirstNameForm)
            $("#GuestLastName").html(guestLastNameForm)
            $("#GuestEmail").html(guestEmailForm)
            $("#GuestPhone").html(guestPhoneForm)
        }
        else {
            $("#GuestFullName").html()
            $("#GuestEmail").html()
            $("#GuestPhone").html()
        }


    });

    $('#SelectedDate').on('change', function () {
        var date = $(this).val();
        let selectedDate = new Date(date).toISOString()

        fetchSittings(selectedDate)
    });

    $('#SittingId').on('change', function () {
        updateAvailableTimes();
    });
    $("#delete").click(function () {
        if (window.confirm("Are you sure you want to delete this reservation?")) {
            const id = $("#Id").val()
            fetch(`/api/reservations/${id}`, {
                method: "DELETE",
            }).then(response => {
                if (response.ok) {
                    $("#editModal").modal("hide")
                } else {
                    throw new Error('Request failed')
                }
            }).catch(error => {
                console.log(error)
            })

        }
    })
    function fetchSittings(selectedDate,selectedOption) {
        fetch(`/api/sittings/dateTime/${selectedDate}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                $('#SittingId').empty();
                if (data.length === 0) {
                    $('#SittingId').append($("<option/>").attr('value', '').text("No available sittings"));
                } else {
                    $('#SittingId').append($("<option/>").attr('value', '').text(`${data.length} available sittings`));
                    $.each(data, function (index, item) {
                        $('#SittingId').append($("<option />").attr('value', item.id).text(item.name));
                    });
                }
                $("#SittingId").val(selectedOption)
            })
            .catch(error => {
                console.error("Error:", error);
            });

    }

    function updateAvailableTimes() {
        var selectedSittingId = $('#SittingId').val();
        var selectedDate = $('#SelectedDate').val();

        if ((selectedSittingId && selectedDate)) {
            fetch(`/api/sittings/GetAvailableTimes/${selectedSittingId}?selectedDate=${selectedDate}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {

                    var timeDropdown = $('#SelectedTime');
                    timeDropdown.empty();
                    timeDropdown.append($('<option>', {
                        value: '',
                        text: '-- Select Time --'
                    }));
                    $.each(data, function (index, time) {
                        const [hours, minutes, seconds] = time.split(':');

                        const date = new Date();
                        date.setHours(parseInt(hours, 10));
                        date.setMinutes(parseInt(minutes, 10));
                        date.setSeconds(parseInt(seconds, 10));

                        const options = { hour: '2-digit', minute: '2-digit', hour12: true };
                        const amPmTime = date.toLocaleTimeString('en-US', options);

                        timeDropdown.append($('<option>', {
                            value: time,
                            text: amPmTime
                        }));
                    });
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    }

    function initialiseTime(selectedSittingId, selectedTime) {
        var selectedDate = $('#SelectedDate').val();
        if ((selectedSittingId && selectedDate)) {
            fetch(`/api/sittings/GetAvailableTimes/${selectedSittingId}?selectedDate=${selectedDate}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {

                    var timeDropdown = $('#SelectedTime');
                    timeDropdown.empty();
                    timeDropdown.append($('<option>', {
                        value: '',
                        text: '-- Select Time --'
                    }));
                    $.each(data, function (index, time) {
                        const [hours, minutes, seconds] = time.split(':');

                        const date = new Date();
                        date.setHours(parseInt(hours, 10));
                        date.setMinutes(parseInt(minutes, 10));
                        date.setSeconds(parseInt(seconds, 10));

                        const options = { hour: '2-digit', minute: '2-digit', hour12: true };
                        const amPmTime = date.toLocaleTimeString('en-US', options);
                        timeDropdown.append($('<option>', {
                            value: time,
                            text: amPmTime
                        }));
                    });
                    $('#SelectedTime').val(selectedTime)
                    
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    }


    function updateSelectTagForNumberOfGuest(id, guestCount) {
        fetch(`/api/sittings/getCurrentCapacity/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                $("#Guests").empty()
                let capacity = data + Number(guestCount)
                for (let i = 1; i <= capacity; i++) {
                    if (i === Number(guestCount)) {
                        $("#Guests").append($("<option/>").attr('value', i).text(i).attr("selected", "selected"))
                    }
                    else {
                        $("#Guests").append($("<option/>").attr('value', i).text(i))
                    }
                }
            })
            .catch(error => {
                console.error("Error:", error);
            });
    }



})
   //In asp mvc, when submitting a form from a modal's update button example.
   //Recap of first answer, with buttons edit, delete, close on a modal.
