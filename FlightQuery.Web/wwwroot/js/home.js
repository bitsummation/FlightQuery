
var home = {
    init: function () {
        this.initSubmit();
        this.initExamples();
        this.initKeyStrokes();
    },

    exampleQueries: {
        strokes: "Query().Filter('Stroke').PerPlayer().Count().Print()",
        gir: "Query().Filter('Green.GIR').PerPlayer().Count().Descending().Print()",
        girBirdies: "Query().Filter('Green.GIR.Holed').PerPlayer().Count().Descending().Print()",
        saves: "Query().Filter('Green.Holed').Not('.GIR').PreviousShot().Not('Green').Score('Par').PerPlayer().Count().Descending().Print()",
        missedSave: "Query().Filter('Green').Not('.GIR').PreviousShot().Not('Green').Score('Bogey').PerPlayer().Count().Descending().Print()",
        averagePutsForBirdie: "Query().Filter('Green.GIR').PerPlayer().Values('ToPin:Feet').Average().Print()",
        birdiePutsInsideTenFeet: "var range = {min:0, max:10};\r\n\r\nQuery().Filter('Green.GIR').PerPlayer().Values('ToPin:Feet').Between(range).Count().Descending().Print()",
        birdiePutsMadeInsideTenFeet: "var range = {min:0, max:10};\r\n\r\nQuery().Filter('Green.GIR.Holed').PerPlayer().Values('ToPin:Feet').Between(range).Count().Descending().Print()",
        philMadeBirdie: "Query().Filter('Green.GIR.Holed').PerPlayer('Phil').Values('ToPin:Feet').Descending('ToPin').Print()",
        kevinNaMissedGreenSavesFrom: "Query().Filter('Green.Holed').Not('.GIR').PreviousShot().Not('Green').Score('Par').PerPlayer('Kevin Na').Values('ToPin:Yards, Location').Descending('ToPin').Print();",
        missedPuttsFieldSevenToEight: "var range = {min:7, max:8};\r\n\r\nQuery().Filter('Green.Miss').PerTournment().Values('ToPin:Feet').Between(range).Count().Print();",
        bunkerSavesFieldDistanceFrom: "Query().Filter('Green.Holed').Not('.GIR').PreviousShot().Filter('Bunker').Score('Par').PerPlayer().Values('ToPin:Yards, Location').Descending('ToPin').Print();",
        offGreenHoleOuts: "Query().Filter('.Holed').Not('Green').PerPlayer().Count().Descending().Print()"
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

    renderQuery: function () {
        this.preLoad();
        var data = this.getQuery();
        var that = this;

        $.ajax({
            type: "Post",
            url: "query",
            data: data,
            contentType: 'text/plain',
            success: function (data) {
                that.loadResults(data);
            },
            error: function () {
                that.loadResults("<div class='error'>Unexpected error occured</div>");
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