$(document).ready(function() {
    var path = window.location.pathname.replace("/", "");
    if (path == "")
        path = "home";
    if(path.search("planner") != -1)
        path = "planners"
    $("#"+path).addClass("active");
    
    moment.locale('en', { week: { dow: 1 }});
    $("#weeklyDatePicker").datetimepicker({
        format: "DD.MM.YYYY",
        date: new Date()
    });

    $('#weeklyDatePicker').on('dp.change', function (e) {
        value = $("#weeklyDatePicker").val();
        firstDate = moment(value, "DD.MM.YYYY").day(1).format("DD.MM.YYYY");
        lastDate =  moment(value, "DD.MM.YYYY").day(7).format("DD.MM.YYYY");
        $("#weeklyDatePicker").val(firstDate + " - " + lastDate);
    });
    
    $('#weeklyDatePicker').on('dp.show', function (e) {
        value = $("#weeklyDatePicker").val();
        firstDate = moment(value, "DD.MM.YYYY").day(1).format("DD.MM.YYYY");
        lastDate =  moment(value, "DD.MM.YYYY").day(7).format("DD.MM.YYYY");
        $("#weeklyDatePicker").val(firstDate + " - " + lastDate);
    });
    
    $('#signin-btn').on('click', function(e) {
        $(e.currentTarget).closest('ul').hide();
        $('form#signin').fadeIn('fast');
    });
    
    $('#signin').submit(function(e) {
        var $form = $(this);
        $.ajax({
            type: 'POST',
            url: '/login',
            data: $form.serialize()
        }).done(function() {
            location.reload();
        }).fail(function(xhr) {
            alert(xhr.status)
        });
        e.preventDefault(); 
    });
    
    $('#signup-btn').on('click', function(e) {
        $(e.currentTarget).closest('ul').hide();
        $('#subheader').hide();
        $('#regheader').fadeIn('fast');
        $('form#signup').fadeIn('fast');
        $('#intro-header .wrap-headline').css('padding-top', '5%')
    });
    
    $('#signup').submit(function(e) {
        var $form = $(this);
        // if ($("input[name=login]").val())
        $.ajax({
            type: 'POST',
            url: '/registration',
            data: $form.serialize()
        }).done(function() {
            location.reload();
        }).fail(function(xhr) {
            alert(xhr.status)
        });
        e.preventDefault(); 
    });
    
    $('#save-btn').click(function(e) {
        var s_data = {};
        $.each($('#profile-form').serializeArray(), function(){
            s_data[this.name]=this.value;
         });
        $.ajax({
            type: 'POST',
            dataType : 'json',
            contentType: 'application/json; charset=utf-8',
            url: '/profile',
            cache: false,
            data: JSON.stringify(s_data)
        }).done(function(msg) {alert(msg)} )
        .fail(function(xhr) {alert(xhr.status)} )
        e.preventDefault(); 
    });
    
    $('#add-planner-btn').click(function(e) {
        $(e.currentTarget).hide();
        $('#save-planner-btn').fadeIn('fast');
        let dateObj = new Date();
        let c_date = String(dateObj.getMonth() + 1).padStart(2, '0') + '-' + String(dateObj.getDate()).padStart(2, '0') + '-' + dateObj.getFullYear();
        let c_time = String(dateObj.getHours()).padStart(2, '0') + ':' + String(dateObj.getMinutes()).padStart(2, '0');
        let n = parseInt($("tr").last().find('td:first').text());
        var row = `<tr style="font-size: 2rem;">
                    <td>${isNaN(n) ? 1 : n+1}</td>
                    <td>
                        <input id="planner-name" type="text" class="form-control" placeholder="Input planner name">
                    </td>
                    <td>
                        <input id="planner-descr" type="text" class="form-control" placeholder="Input planner description">
                    </td>
                    <td><div id="planner-cr-time">${c_date}  ${c_time}</div></td>
                   </tr>`
        $('#planners-table').append(row)
    });
    
    $('#save-planner-btn').click(function(e) {
        $.ajax({
            type: 'POST',
            dataType : 'json',
            contentType: 'application/json; charset=utf-8',
            url: '/planners',
            cache: false,
            data: JSON.stringify({
                    "name":$("#planner-name").val(), 
                    "description": $("#planner-descr").val(), 
                    "creationTime": $("#planner-cr-time").html()
                })
        }).done(function(msg) { location.reload(); } )
        .fail(function(xhr) {alert(xhr.status)} )
    });
    
    $("#planners-table tr").click(function(e){
        window.location = "/planners/"+this.id;
    });
    
    $('#add-task').on('show.bs.modal', function(e) {
        let t = e.relatedTarget;
        $('#add-task-hour').text(t.parentNode.parentNode.id);
        $('#add-task-day').text(t.parentNode.id);
    });
    
    $('#add-task-save-btn').click(function(e) {
        monthes = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']
        offset = monthes.indexOf($("#add-task-day").text())
        var selected_date = moment($("#weeklyDatePicker").val(), "DD.MM.YYYY");
        selected_date.date(selected_date.date()+offset)
        $.ajax({
            type: 'POST',
            dataType : 'json',
            contentType: 'application/json; charset=utf-8',
            url: window.location.pathname,
            cache: false,
            data: JSON.stringify({
                    "time": $("#add-task-hour").text(), 
                    "date": selected_date.format('YYYY-MM-DD'), 
                    "name": $("#add-task-form input").val(),
                    "description": $("#add-task-form textarea").val()
                })
        }).done(function(msg) { alert(msg) } )
        .fail(function(xhr) {alert(xhr.status)} )
    });
    
});
function getTasks() {
    console.log('OK');
    $.ajax({
        type: 'GET',
        dataType : 'json',
        contentType: 'application/json; charset=utf-8',
        url: window.location.pathname + '/tasks',
        cache: false
    }).done(function(msg) { alert(msg) } )
    .fail(function(xhr) {alert(xhr.status)} )
}