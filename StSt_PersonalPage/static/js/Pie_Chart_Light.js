/**
 * Created by gabri on 6/7/16.
 */

google.charts.load('current', {'packages':['corechart']});
google.charts.setOnLoadCallback(drawChart);
      function drawChart() {

        var user = window.location.href.substr(31);
        var excellent_position = 0;
        var good_position = 0;
        var poor_position = 0;
        var verypoor_position = 0;
        var excellent_light = 0;
        var good_light = 0;
        var poor_light = 0;
        var verypoor_light = 0;


        $.ajax('/sessions/json/'+user,{
            method: "GET",
            dataType: "json",
            success: function(data, status){
                var sessions = data['sessions'];
                for(i=0; i<sessions.length; i++) {
                    if (parseInt(sessions[i]['position']) > 80) excellent_position++;
                    if (parseInt(sessions[i]['position']) <= 80 && sessions[i]['position'] > 60) good_position++;
                    if (parseInt(sessions[i]['position']) <= 60 && sessions[i]['position'] > 40) poor_position++;
                    if (parseInt(sessions[i]['position']) <= 40) verypoor_position++;
                    if (parseInt(sessions[i]['light']) > 80) excellent_light++;
                    if (parseInt(sessions[i]['light']) <= 80 && sessions[i]['position'] > 60) good_light++;
                    if (parseInt(sessions[i]['light']) <= 60 && sessions[i]['position'] > 40) poor_light++;
                    if (parseInt(sessions[i]['light']) <= 40) verypoor_light++;
                }
                var data_position = google.visualization.arrayToDataTable([
                  ['Quality', 'Number of Session'],
                  ['Excellent', excellent_position],
                  ['Very Poor', good_position],
                  ['Poor',  poor_position],
                  ['Good',    verypoor_position]
                ]);

                var data_light = google.visualization.arrayToDataTable([
                  ['Quality', 'Number of Session'],
                  ['Excellent', excellent_light],
                  ['Very Poor', good_light],
                  ['Poor',  poor_light],
                  ['Good',    verypoor_light]
                ]);

                var options = {
                  title: 'Sessions Light',
                  is3D: true
                };

                var chart = new google.visualization.PieChart(document.getElementById('piechart_light'));

                var chart2 = new google.visualization.PieChart(document.getElementById('piechart_position'));

                chart2.draw(data_position, options);

                chart.draw(data_light, options);
            }
        })


  }