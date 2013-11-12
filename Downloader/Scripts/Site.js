function downloadFailed() {
    $("#downloadResults").html("Sorry, there was a problem with the download.");
}

$("#download").submit(function (event) {
    event.preventDefault();

    var form = $(this);
    $.ajax({
        url: form.attr("action"),
        data: form.serialize(),
        beforeSend: function () {
            $("#ajax-loader").show();
            $(":submit").attr("disabled", "disabled");
        },
        complete: function () {
            $("#ajax-loader").hide();
            $(":submit").removeAttr("disabled");
        },
        error: downloadFailed,
        success: function (data) {
            $("#downloadResults").html(data);
        }
    });
});
