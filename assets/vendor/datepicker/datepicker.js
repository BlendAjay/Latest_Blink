jQuery(document).ready(function ($) {
    'use strict';

    if ($("#datetimepicker1").length) {
        $('#datetimepicker1').datetimepicker();

    }

    /* Calender jQuery **/

    if ($("datetimepicker2").length) {

        $('#datetimepicker2').datetimepicker({
            locale: 'ru',
            icons: {
                time: "far fa-clock",
                date: "fa fa-calendar-alt",
                up: "fa fa-arrow-up",
                down: "fa fa-arrow-down"
            }
        });
    }


    if ($("#datetimepicker3").length) {

        $('#datetimepicker3').datetimepicker({
            format: 'LT'
        });
    }

    if ($("#datetimepicker4").length) {
        $('#datetimepicker4').datetimepicker({
            format: 'L'
        });

    }
    if ($("#datetimepicker5").length) {
        $('#datetimepicker5').datetimepicker();

    }

    if ($("#datetimepicker6").length) {
        $('#datetimepicker6').datetimepicker({
            defaultDate: "11/1/2013",
            disabledDates: [
                moment("12/25/2013"),
                new Date(2013, 11 - 1, 21),
                "11/22/2013 00:53"
            ],
            icons: {
                time: "far fa-clock",
                date: "fa fa-calendar-alt",
                up: "fa fa-arrow-up",
                down: "fa fa-arrow-down"
            }
        });
    }

    if ($("#datetimepicker7").length) {
        $(function () {
            $('#datetimepicker7').datetimepicker({
                icons: {
                    time: "far fa-clock",
                    date: "fa fa-calendar-alt",
                    up: "fa fa-arrow-up",
                    down: "fa fa-arrow-down"
                }
            });
            $('#datetimepicker8').datetimepicker({
                useCurrent: false,
                icons: {
                    time: "far fa-clock",
                    date: "fa fa-calendar-alt",
                    up: "fa fa-arrow-up",
                    down: "fa fa-arrow-down"
                }
            });
            $("#datetimepicker7").on("change.datetimepicker", function (e) {
                $('#datetimepicker8').datetimepicker('minDate', e.date);
            });
            $("#datetimepicker8").on("change.datetimepicker", function (e) {
                $('#datetimepicker7').datetimepicker('maxDate', e.date);
            });
        });
    }

    if ($("#datetimepicker10").length) {
        $('#datetimepicker10').datetimepicker({
            viewMode: 'years',
            icons: {
                time: "far fa-clock",
                date: "fa fa-calendar-alt",
                up: "fa fa-arrow-up",
                down: "fa fa-arrow-down"
            }
        });
    }

    if ($("#datetimepicker11").length) {
        $('#datetimepicker11').datetimepicker({
            viewMode: 'years',
            format: 'MM/YYYY'
        });
    }

    if ($("#datetimepicker13").length) {
        $('#datetimepicker13').datetimepicker({
            inline: true,
            sideBySide: true
        });

    }

    if ($("#datetimepicker21").length) {
        $('#datetimepicker21').datetimepicker();

    }

    /* Calender jQuery **/

    if ($("datetimepicker22").length) {

        $('#datetimepicker22').datetimepicker({
            locale: 'ru',
            icons: {
                time: "far fa-clock",
                date: "fa fa-calendar-alt",
                up: "fa fa-arrow-up",
                down: "fa fa-arrow-down"
            }
        });
    }


    if ($("#datetimepicker23").length) {

        $('#datetimepicker23').datetimepicker({
            format: 'LT'
        });
    }

    if ($("#datetimepicker24").length) {
        $('#datetimepicker24').datetimepicker({
            format: 'L'
        });

    }
    if ($("#datetimepicker25").length) {
        $('#datetimepicker25').datetimepicker();

    }

    if ($("#datetimepicker26").length) {
        $('#datetimepicker26').datetimepicker({
            defaultDate: "11/1/2013",
            disabledDates: [
                moment("12/25/2013"),
                new Date(2013, 11 - 1, 21),
                "11/22/2013 00:53"
            ],
            icons: {
                time: "far fa-clock",
                date: "fa fa-calendar-alt",
                up: "fa fa-arrow-up",
                down: "fa fa-arrow-down"
            }
        });
    }

    if ($("#datetimepicker27").length) {
        $(function () {
            $('#datetimepicker27').datetimepicker({
                icons: {
                    time: "far fa-clock",
                    date: "fa fa-calendar-alt",
                    up: "fa fa-arrow-up",
                    down: "fa fa-arrow-down"
                }
            });
            $('#datetimepicker28').datetimepicker({
                useCurrent: false,
                icons: {
                    time: "far fa-clock",
                    date: "fa fa-calendar-alt",
                    up: "fa fa-arrow-up",
                    down: "fa fa-arrow-down"
                }
            });
            $("#datetimepicker27").on("change.datetimepicker", function (e) {
                $('#datetimepicker28').datetimepicker('minDate', e.date);
            });
            $("#datetimepicker28").on("change.datetimepicker", function (e) {
                $('#datetimepicker27').datetimepicker('maxDate', e.date);
            });
        });
    }

    if ($("#datetimepicker30").length) {
        $('#datetimepicker30').datetimepicker({
            viewMode: 'years',
            icons: {
                time: "far fa-clock",
                date: "fa fa-calendar-alt",
                up: "fa fa-arrow-up",
                down: "fa fa-arrow-down"
            }
        });
    }

    if ($("#datetimepicker31").length) {
        $('#datetimepicker31').datetimepicker({
            viewMode: 'years',
            format: 'MM/YYYY'
        });
    }

    if ($("#datetimepicker33").length) {
        $('#datetimepicker33').datetimepicker({
            inline: true,
            sideBySide: true
        });

    }
});