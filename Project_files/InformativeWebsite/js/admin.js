//JS file for the admin Page
//Currently controls the graphs on the page

//Force chartjs to use the css linked in the header (instead of using its own)
//Copied from the wiki but not tested
Chart.platform.disableCSSInjection = true;

//~~~~~~~~~~~~~~~~~Global vars~~~~~~~~~~~~~~~~~~
//Note: the following vars should be injected using PHP before calling this script
//all_page_visits - raw data for page visits and time
//submission_age_ranges - raw data for the age ranges in the submitted expression of interest forms
//submission_countries - raw data for the age ranges in the submitted expression of interest forms

//For converting Date index to month name
const monthNames = ["January", "February", "March", "April", "May", "June",
	"July", "August", "September", "October", "November", "December"];
const chartColors = ["rgb(54, 162, 235)", "rgb(75, 192, 192)", "rgb(255, 159, 64)", "rgb(153, 102, 255)", "rgb(255, 99, 132)", "rgb(255, 205, 86)", "rgb(201, 203, 207)"];

//The "bins" for the #clicks graph (e.g. Jan 2018, Feb 2018 ....)
var clicksGraph_cols = generate_chart_colums(all_page_visits);
var col_type = document.getElementById('visitsGraph_select').value;

//~~~~~~~~~~~~~~~~Functions~~~~~~~~~~~~~~~~~~
//Generate the "bins" based on the current value of col_type (Hours, Day, Month, Year)
//Assumes raw_data is an array of {page_name, visit_time} objects
//Where page_name is the page visited and visit_time is a string of MYSQL timestamp
//Does add elements to raw_data but will not modify page_name or visit_time
function generate_chart_colums(raw_data) {
	if (col_type !== 'year' && col_type !== 'month' && col_type !== 'day' && col_type !== 'hour') {
		col_type = 'day';
	}

	var count_month = false;
	if (col_type === 'month' || col_type === 'day' || col_type === 'hour') {
		count_month = true;
	}
	var count_day = false;
	if (col_type === 'day' || col_type === 'hour') {
		count_day = true;
	}
	var count_hour = false;
	if (col_type === 'hour') {
		count_hour = true;
	}

	//Convert MYSQL timestamp to JS data object
	for (var i = 0; i < raw_data.length; i++) {
		if (raw_data[i]) {
			var t = raw_data[i].visit_time.split(/[- :]/);
			raw_data[i].time = new Date(t[0], t[1] - 1, t[2], t[3], t[4], t[5]);
		}
	}

	//Find the maximum date in the raw data
	var maxDate = new Date(raw_data[0].time);
	for (var i = 0; i < raw_data.length; i++) {
		if (raw_data[i].time > maxDate) {
			maxDate = new Date(raw_data[i].time);
		}
	}

	//Find the minimum date in the raw data
	var minDate = new Date(raw_data[0].time);
	for (var i = 0; i < raw_data.length; i++) {
		if (raw_data[i].time < minDate) {
			minDate = new Date(raw_data[i].time);
		}
	}

	//Set the starting date of the colums to be the minimum of that in the data
	//Year, Month, Date, Hours, Minutes, Seconds, Milliseconds
	var currentCol = new Date(minDate.getFullYear(), count_month ? minDate.getMonth() : 0, count_day ? minDate.getDate() : 1, count_hour ? minDate.getHours() : 0, 0, 0);

	var cols = [];

	var i = 0;
	while (currentCol < maxDate) {
		//Generate the label for the current bin
		var label = currentCol.getFullYear().toString();
		if (count_month) {
			label = monthNames[currentCol.getMonth()] + " " + label;
		}
		if (count_day) {
			var d = currentCol.getDate();
			label = d + (d > 0 ? ['th', 'st', 'nd', 'rd'][(d > 3 && d < 21) || d % 10 > 3 ? 0 : d % 10] : '') + " " + label;
		}
		if (count_hour) {
			var hours = currentCol.getHours();
			var ampm = hours >= 12 ? "pm " : "am ";
			hours = hours % 12;
			hours = hours ? hours : 12; // the hour '0' should be '12'
			label = hours + ampm + label;
		}

		cols[i] = {
			label: label,
			date: new Date(currentCol)
		};

		if (col_type === 'year') {
			currentCol.setYear(currentCol.getFullYear() + 1);
		} else if (col_type === 'month') {
			currentCol.setMonth(currentCol.getMonth() + 1);
		} else if (col_type === 'day') {
			currentCol.setDate(currentCol.getDate() + 1);
		} else if (col_type === 'hour') {
			currentCol.setHours(currentCol.getHours() + 1);
		}

		i++;
	}
	return cols;
}

//Based on the raw data, clicksGraph_cols and col_type generate the Chartsjs data object for the #visits bar graph
//Assumes raw_data is an array of {page_name, visit_time} objects
//Where page_name is the page visited and visit_time is the MYSQL timestamp of the visit
//Does add to raw_data but will not modify page_name or visit_time
function generate_chart_data(raw_data, min_index, max_index) {
	var count_month = false;
	if (col_type === 'month' || col_type === 'day' || col_type === 'hour') {
		count_month = true;
	}
	var count_day = false;
	if (col_type === 'day' || col_type === 'hour') {
		count_day = true;
	}
	var count_hour = false;
	if (col_type === 'hour') {
		count_hour = true;
	}

	c = {};
	c.labels = [];
	c.datasets = [];

	//Reset counted var
	for (var i = 0; i < raw_data.length; i++) {
		if (raw_data[i]) {
			raw_data[i].counted = false;
		}
	}

	if (min_index == null) {
		min_index = 0;
	}
	if (max_index == null) {
		max_index = clicksGraph_cols.length - 1;
	}

	var relative_pos = 0;
	for (var i = min_index; i <= max_index; i++) {

		c.labels[relative_pos] = clicksGraph_cols[i].label;

		for (var j = 0; j < raw_data.length; j++) {
			var yearMatch = clicksGraph_cols[i].date.getFullYear() === raw_data[j].time.getFullYear();
			var monthMatch = clicksGraph_cols[i].date.getMonth() === raw_data[j].time.getMonth();
			var dayMatch = clicksGraph_cols[i].date.getDate() === raw_data[j].time.getDate();
			var hoursMatch = clicksGraph_cols[i].date.getHours() === raw_data[j].time.getHours();

			var match = yearMatch && (monthMatch || !count_month) && (dayMatch || !count_day) && (hoursMatch || !count_hour);

			if (match) {
				for (var k = 0; k < c.datasets.length; k++) {
					if (c.datasets[k].label === raw_data[j].page_name) {
						if (c.datasets[k].data[relative_pos]) {
							c.datasets[k].data[relative_pos]++;
						} else {
							c.datasets[k].data[relative_pos] = 1;
						}
						raw_data[j].counted = true;
					}
				}

				if (!raw_data[j].counted) {
					c.datasets[c.datasets.length] = {}
					c.datasets[c.datasets.length - 1].label = raw_data[j].page_name;
					c.datasets[c.datasets.length - 1].data = []
					c.datasets[c.datasets.length - 1].data[relative_pos] = 1;
					raw_data[j].counted = true;
				}
			}
		}

		relative_pos++;
	}

	for (var i = 0; i < c.datasets.length; i++) {
		var chosen = chartColors[i % chartColors.length];
		c.datasets[i].backgroundColor = Chart.helpers.color(chosen).alpha(0.3).rgbString();
		c.datasets[i].borderColor = chosen;
		c.datasets[i].borderWidth = 1;
	}

	return c;
}

function setup_page_visits_graph() {
	var ctx = document.getElementById('pageVisitsGraph').getContext('2d');
	window.myChart = new Chart(ctx, {
		type: 'bar',
		data: generate_chart_data(all_page_visits),
		options: {
			responsive: true,
			scales: {
				yAxes: [{
					type: 'linear', // only linear but allow scale type registration. This allows extensions to exist solely for log scale for instance
					position: 'left',
					ticks: {
						beginAtZero: true,
                        fontColor : '#FFFFFF'
					},
					display: true,
                    gridLines: {color: '#FFFFFF', zeroLineColor: '#FFFFFF'},
				}],
                xAxes: [{
					ticks: {
                        fontColor : '#FFFFFF'
					},
					display: true,
                    gridLines: {color: '#FFFFFF', zeroLineColor: '#FFFFFF'},
				}],
			},
			title: {
				display: false,
				text: 'Page visits per ' + col_type
			},
            legend: {
                labels: {
                    fontColor: '#FFFFFF'
                }
            }
		}
	});
}

function setup_age_range_chart() {
    var ageData = {
        datasets: [{data : [],
            backgroundColor: [],
            label: 'Age Ranges'}],
        labels: []};

    for(var i = 0; i < submission_age_ranges.length; i++) {
        ageData.datasets[0].data[i] = submission_age_ranges[i].count;
        ageData.datasets[0].backgroundColor[i] = chartColors[i % submission_age_ranges.length];
        ageData.labels[i] = submission_age_ranges[i].age_range;
    }

	var ctx = document.getElementById('ageRangesChart').getContext('2d');
	window.myPie = new Chart(ctx, {
        type: 'pie',
		data: ageData,
		options: {
            title: {
                display: false,
                text: 'Ages of user submissions'
            }
		}});
}

function setup_countries_chart() {
    var countriesData = {
        datasets: [{data : [],
            backgroundColor: [],
            label: 'Age Ranges'}],
        labels: []};

    for(var i = 0; i < submission_countries.length; i++) {
        countriesData.datasets[0].data[i] = submission_countries[i].count;
        countriesData.datasets[0].backgroundColor[i] = chartColors[i % submission_countries.length];
        countriesData.labels[i] = submission_countries[i].country_name;
    }

	var ctx = document.getElementById('countriesChart').getContext('2d');
	window.myPie = new Chart(ctx, {
        type: 'pie',
		data: countriesData,
		options: {
            title: {
                display: false,
                text: 'Countries of user submissions'
            },
            legend: {
                labels: {
                    fontColor: '#FFFFFF'
                }
            }
		}});
}

$("#visitsGraph_slider").slider({
	range: true,
	min: 0,
	max: clicksGraph_cols.length - 1,
	values: [0, clicksGraph_cols.length - 1],
	slide: function(event, ui) {
		$("#visitsGraph_slider_text").text("Graph range: " + clicksGraph_cols[ui.values[0]].label + " - " + clicksGraph_cols[ui.values[1]].label);

		window.myChart.config.data = generate_chart_data(all_page_visits, ui.values[0], ui.values[1]);
		window.myChart.update();
	}
});

//Reset the slider range
function update_slider() {
	$("#visitsGraph_slider").slider("values", 0, 0);
	$("#visitsGraph_slider").slider("option", "max", clicksGraph_cols.length - 1);
	$("#visitsGraph_slider").slider("values", 1, clicksGraph_cols.length - 1);
	$("#visitsGraph_slider_text").text("Graph range: " + clicksGraph_cols[$("#visitsGraph_slider").slider("values", 0)].label + " - " + clicksGraph_cols[$("#visitsGraph_slider").slider("values", 1)].label);
}

//Get started
update_slider();
setup_page_visits_graph();
setup_age_range_chart();
setup_countries_chart();

//Event listener for the grouping type dropdown (visits graph)
document.getElementById('visitsGraph_select').addEventListener('change', function() {
	col_type = this.value;
	clicksGraph_cols = generate_chart_colums(all_page_visits);
	window.myChart.config.data = generate_chart_data(all_page_visits);
	window.myChart.config.options.title.text = 'Page visits per ' + col_type;
	window.myChart.update();
	update_slider();
});
