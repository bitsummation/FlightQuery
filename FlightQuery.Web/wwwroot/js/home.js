var intellisense = {

    globalTriggers : ["from", "join"],
    scopeTriggers: ["select", "on", "where", "and"],

    fetch: function (code, callback) {
       
        $.ajax({
            type: "Post",
            url: "scope",
            data: code,
            contentType: 'text/plain',
            success: function (data) {
                callback(data);
            }
        });

    },

    getPrompts: function (args, callback) {
        var that = this;
       
        this.fetch(args.code, function (scope) {
            console.log(scope);
            if (that.globalTriggers.includes(args.token)) {
                callback(scope.global.keys)
            }
            else if (that.scopeTriggers.includes(args.token)) {
                if (scope.queryScope.keys.length > 0) {
                    var prompts = []
                    prompts = prompts.concat(scope.queryScope.keys);
                    scope.queryScope.keys.forEach(function (e) { prompts = prompts.concat(scope.queryScope.items[e]); });
                    callback(prompts);
                }
            }
            else { 
                if (args.promptToken == '.' && scope.queryScope.keys.length > 0 && scope.queryScope.keys.includes(args.token)) { //do we match alias with a dot
                    var prompts = scope.queryScope.items[args.token]
                    callback(prompts);
                }
                else
                    callback([]);
            }
        });

    },

    init: function () {
        
        var that = this;
        var editor = ace.edit("code");
        var langTools = ace.require('ace/ext/language_tools');

        langTools.setCompleters([])

        editor.getSession().setMode("ace/mode/mysql");
        editor.setValue(""); //clear global ones

        var staticWordCompleter = {
            getCompletions: function (editor, session, pos, prefix, callback) {
                console.log("fetch completions");
                console.log({ "prefix": prefix });

                var args = {};
                var firstToken = editor.session.getTokenAt(pos.row, pos.column);
                if (firstToken != null) {
                    console.log({ "firsttoken": firstToken.value });
                    args.promptToken = firstToken.value;
                }
                
                var token = editor.session.getTokenAt(pos.row, pos.column - 1);
                if (token != null) {
                    token = token.value.toLowerCase();
                    args.token = token;
                    console.log({ "keyword": token });
                }

                var code = editor.getValue();
                args.code = code;
                console.log({"source": code });
                that.getPrompts(args, function (wordList) {

                    callback(null, wordList.map(function (word) {
                        return {
                            caption: word,
                            value: word,
                            score: 1000
                        };
                    }));

                });

               
            }
        };

        langTools.addCompleter(staticWordCompleter);

        var doLiveAutocomplete = function (e) {
            var editor = e.editor;
            if (e.command.name === "insertstring") {
                // Only autocomplete if there's a prefix that can be matched
                console.log({ "insert": e.args })
                if (/[\.|\s]/.test(e.args)) {
                    editor.execCommand("startAutocomplete")
                }
            }
        };

        editor.commands.on('afterExec', doLiveAutocomplete);
        editor.setOptions({
            enableBasicAutocompletion: true,
            fontSize: "12px"
        });

    }

};

var home = {
    init: function () {
        this.initSubmit();
        this.initExamples();
    },

    exampleQueries: {
        airportInfo: "select *\nfrom AirportInfo\nwhere airportCode = 'kaus'",
        gir: "Query().Filter('Green.GIR').PerPlayer().Count().Descending().Print()",
        girBirdies: "Query().Filter('Green.GIR.Holed').PerPlayer().Count().Descending().Print()",
        saves: "Query().Filter('Green.Holed').Not('.GIR').PreviousShot().Not('Green').Score('Par').PerPlayer().Count().Descending().Print()",
        missedSave: "Query().Filter('Green').Not('.GIR').PreviousShot().Not('Green').Score('Bogey').PerPlayer().Count().Descending().Print()"
    },

    preLoad: function () {
        $("#results").empty();
        $("#loading").show();
        $(".green-form input").removeClass("error");
    },

    postLoad: function () {
        $("#loading").hide();
        $("table tbody tr:odd").addClass("odd");
    },

    loadExamples: function (attr) {
        var query = this.exampleQueries[attr];
        var editor = ace.edit("code");
        editor.setValue(query);
    },

    initExamples: function () {
        var that = this;
        $("#examples a").click(function () {
            var attr = $(this).attr("query");
            that.loadExamples(attr);
            return false;
        });
    },

    initSubmit: function () {
        var that = this;
        $("#run").click(function () {
            that.renderQuery();
        });
    },

    getQuery: function () {
        var editor = ace.edit("code");
        return editor.getValue();
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
            statusCode: {
                401: function () {
                    that.loadResults({ errors: [{ message: 'Authentication error' }] });
                    $(".green-form input").addClass("error");
                }
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
    intellisense.init();
});