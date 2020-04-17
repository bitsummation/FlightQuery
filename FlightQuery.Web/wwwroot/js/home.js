var intellisense = {

    globalTriggers : ["from", "join"],
    scopeTriggers: ["select", "on", "where", "and", ","],

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
           
            if (that.globalTriggers.includes(args.token)) {
                callback(scope.global.keys)
            }
            else if (that.scopeTriggers.includes(args.token) || args.promptToken == ',') {
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
               
                var args = {};
                var firstToken = editor.session.getTokenAt(pos.row, pos.column);
                if (firstToken != null) {
                    args.promptToken = firstToken.value;
                }
                
                var token = editor.session.getTokenAt(pos.row, pos.column - 1);
                if (token != null) {
                    token = token.value.toLowerCase();
                    args.token = token.trim();
                }

                var code = editor.getValue();
                args.code = code;
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
                if (/[\.|\s|,]/.test(e.args)) {
                    editor.execCommand("startAutocomplete")
                }
            }
        };

        editor.commands.on('afterExec', doLiveAutocomplete);
        editor.setShowPrintMargin(false);
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
        airportInfo: "select *\nfrom airportinfo\nwhere airportCode = 'kaus'",
        austinDepartures: "select *\nfrom airlineflightschedules\nwhere departuretime > '{current_date}' and origin = 'kaus'",
        austinDeparturesFriendly: "select f.ident, f.departuretime, f.arrivaltime, o.name as origin, d.name as destination\nfrom airlineflightschedules f\njoin airportinfo d on d.airportCode = f.destination\njoin airportinfo o on o.airportCode = f.origin\nwhere f.departuretime > '{current_date}' and f.origin = 'kaus'",
        austinDeparturesNoCancelled: "select *\nfrom airlineflightschedules a\njoin getflightid f on f.departureTime = a.departuretime and f.ident = a.ident\njoin flightinfoex e on e.faFlightID = f.faFlightID and e.actualarrivaltime != -1 and e.actualdeparturetime != -1 and estimatedarrivaltime != -1\nwhere a.departuretime > '{current_date}' and a.origin = 'kaus'"
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
        var d = new Date();
        var dateString = d.getUTCFullYear() + "-" + (d.getUTCMonth() + 1) + "-" + d.getUTCDate() + " " + d.getUTCHours() + ":" + ((d.getMinutes() < 10 ? '0' : '') + d.getMinutes())
       
        query = query.replace("{current_date}", dateString)
        var editor = ace.edit("code");
        editor.setValue(query);
        this.renderQuery();
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
        var source = editor.getSelectedText();
        if (source == "") {
            source = editor.getValue();
        }

        return source;
    },

    getUserName: function () {
        return $("#username").val();
    },

    getPassword: function () {
        return $("#pass").val();
    },

    getBasicHeader: function () {
        var userName = this.getUserName();
        var pass = this.getPassword();
        var token = userName + ":" + pass;
        var hash = btoa(token);
        return "Basic " + hash;
    },

    renderQuery: function () {
        if (this.getUserName() == '' || this.getPassword() == '') {
            $(".green-form input").addClass("error");
            this.loadResults({ errors: [{ message: 'Authentication error' }] });
            return;
        }

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