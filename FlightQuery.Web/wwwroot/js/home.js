
var home = {
    init: function () {
        this.initSubmit();
        this.initExamples();
        this.initKeyStrokes();
    },

    exampleQueries: {
        strokes: "select location, name\nfrom AirportInfo\nwhere airportCode = 'kaus'",
        gir: "Query().Filter('Green.GIR').PerPlayer().Count().Descending().Print()",
        girBirdies: "Query().Filter('Green.GIR.Holed').PerPlayer().Count().Descending().Print()",
        saves: "Query().Filter('Green.Holed').Not('.GIR').PreviousShot().Not('Green').Score('Par').PerPlayer().Count().Descending().Print()",
        missedSave: "Query().Filter('Green').Not('.GIR').PreviousShot().Not('Green').Score('Bogey').PerPlayer().Count().Descending().Print()"
    },

    animateSubmit: function () {
        $("form :submit").delay(200).fadeOut('slow').delay(50).fadeIn('slow', this.animateSubmit);
    },

    preLoad: function () {
        $("#results").empty();
        $("#loading").show();
    },

    postLoad: function () {
        $("#loading").hide();
        $("table tbody tr:odd").addClass("odd");
    },

    loadExamples: function (attr) {
        var query = this.exampleQueries[attr];
        $("#code").val(query);
        this.renderQuery();
    },

    initKeyStrokes: function () {
        var that = this;
        $("#code").keyup(function () {
            $("#results").empty();
        });
    },

    initExamples: function () {
        $("#code").val(this.exampleQueries.strokes);

        var that = this;
        $("#examples a").click(function () {
            var attr = $(this).attr("query");
            that.loadExamples(attr);
            return false;
        });
    },

    initSubmit: function () {
        var that = this;
        $("form").submit(function () {
            that.renderQuery();
            return false;
        });
    },

    getQuery: function () {
        return $("#code").val();
    },

    getBasicHeader: function () {
        var userName = $("#username").val();
        var pass = $("#pass").val();
        var token = userName + ":" + pass;
        var hash = btoa(token);
        return "Basic " + hash;
    },

    renderQuery: function () {
        this.preLoad();
        var data = this.getQuery();
        var that = this;

        $.ajax({
            type: "Post",
            beforeSend: function (request) {
                request.setRequestHeader("Authorization", that.getBasicHeader());
            },
            url: "query",
            data: data,
            contentType: 'text/plain',
            success: function (data) {
                that.loadResults(data);
            },
            error: function () {
                that.loadResults({ errors: [{ message: 'Bad error. Interpreter crashy' }] });
            }
        });
    },

    loadResults: function (json) {
        // Grab the template script
        var theTemplateScript = $("#results-template").html();

        // Compile the template
        var theTemplate = Handlebars.compile(theTemplateScript);

        var html = theTemplate(json);
        $("#results").html(html);
        this.postLoad();
    }

};


$().ready(function () {
    home.init();
});