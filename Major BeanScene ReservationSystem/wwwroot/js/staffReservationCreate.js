$(() => {
    var date = $("#SelectedDate").val();
    let selectedDate = new Date(date).toISOString()
    let sittingIdFromRoute = new URL(window.location.href).searchParams.get("sittingId");
    fetchSittings(selectedDate, sittingIdFromRoute);
    //to update this guestList, you alway need to reselect the sitting
    updateSelectTagForNumberOfGuest(sittingIdFromRoute)
    $("#reservationForm").on("submit", e => {
        const numGuests = $("#numGuests").val();
        const guestsValidationError = document.getElementById("guestsValidationError");

        if (numGuests <= 0) {
            guestsValidationError.textContent = "There must be at least 1 guest to make a reservation."
        }
        else {
            guestsValidationError.textContent = ""
        }
    })


    $('#SelectedDate').on('change', function () {
        var date = $(this).val();
        let selectedDate = new Date(date).toISOString()

        fetchSittings(selectedDate)
    });

    function fetchSittings(selectedDate,sittingIdFromRoute) {
        fetch(`/api/sittings/${selectedDate}`)
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
                    $("#SittingId").val(sittingIdFromRoute)
                }

            })
            .catch(error => {
                console.error("Error:", error);
            });
    }

    function updateSelectTagForNumberOfGuest(id) {
        fetch(`/api/sittings/getCurrentCapacity/${id}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                $('#numGuests').empty();
                for (let i = 1; i <= data; i++) {
                    $("#numGuests").append($("<option/>").attr('value', i).text(i))
                }
                //$("#numGuests").foreach

                //    < select id = "numGuests" asp -for= "NumberOfGuests" asp - items= "@(new SelectList(Enumerable.Range(1, 10)))" class="form-control" >
                //        <option value="">--Select the number of guests--</option>
                //</select >
            })
            .catch(error => {
                console.error("Error:", error);
            });
    }

    $('#SittingId').on('change', function () {
        updateAvailableTimes();
        let id = $(this).val()
        updateSelectTagForNumberOfGuest(id);
    });

    function updateAvailableTimes() {

        var selectedSittingId = $('#SittingId').val();
        var selectedDate = $('#SelectedDate').val();

        if (selectedSittingId && selectedDate) {
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
    updateAvailableTimes();



});