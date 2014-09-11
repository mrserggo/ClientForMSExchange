$(document).ready(function () {

    $(function () {
        $('#deleteBtn').bind("click", DeleteEmail);
    });

    //$(function () {
    //    $('#loadBtn').bind("click", LoadEmails);
    //});

    //get mails for current catalog
    LoadEmails();

});

//get body for choosed email subject by InternetMailId
function GetSelectedEmail(id) {

    $.get('GetBodyEmailById?Id=' + id + '&' + 'emailStringCatalog='+$('.active').children().first().text(), null, function (data) {

        var divWithBody = $("<div/>").append(data);

        $('#newsContent').html(divWithBody);
    });
}

function LoadEmails() {

    var activeCatalog = $('.active').children().first().text();

    $.get('GetEmails?emailStringCatalog=' + activeCatalog,
        null,
        function (data) {

            var subjectsList = JSON.parse(data);

            $('#count').html(subjectsList.length);

            if (subjectsList.length != 0)
                $('#deleteBtn').removeAttr("disabled");

            $('#listTitles').html("");
            $('#newsContent').html("");

            for (var item in subjectsList) {

                var subElSubject = $("<div/>").addClass("div_subject").html(subjectsList[item].Subject);
                var subElTime = $("<div/>").addClass("div_time").html(subjectsList[item].DateCreation);

                var elem = $("<div/>").addClass("title_div").attr("id", subjectsList[item].Id).append(subElSubject);
                elem.append(subElTime);

                $('#listTitles').append(elem);
            }

            var firstEmailSubject = $('.title_div').first();
            firstEmailSubject.addClass("selected");
            var idSelected = firstEmailSubject.attr("id");
            GetSelectedEmail(idSelected);

            $('.title_div').on('click', function () {
                $('.title_div').removeClass("selected");
                $(this).addClass("selected");
                GetSelectedEmail($(this).attr("id"));
            });
        });
}

function DeleteEmail() {

    if (confirm("Are you approve deleting?")) {
        $.get('DeleteEmailById?Id='+ $('.selected').attr("Id")+'&' + 'emailStringCatalog='+$('.active').children().first().text() );
        window.location.href = "DeletedItems";
    } else {
        return false;
    }
    return true;
}