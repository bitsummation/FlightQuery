var intellisense = {

    globalTriggers : ["from", "join"],
    scopeTriggers: ["select", "on", "where", "and", ",", "when", "then", "else"],

    fetch: function (args, callback) {
       
        $.ajax({
            type: "Post",
            url: "scope?" + $.param(args.pos),
            data: args.code,
            contentType: 'text/plain',
            success: function (data) {
                callback(data);
            }
        });

    },

    getPrompts: function (args, callback) {
        var that = this;
       
        this.fetch(args, function (scope) {
            if (!scope)
                return;

            if (that.globalTriggers.includes(args.token)) { //global tables
                callback(scope.global.keys)
            }
            else if (that.scopeTriggers.includes(args.token) || args.token == ',') { //variables
                if (scope.queryScope.keys.length > 0) {
                    var prompts = []
                    prompts = prompts.concat(scope.queryScope.keys);
                    scope.queryScope.keys.forEach(function (e) { prompts = prompts.concat(scope.queryScope.items[e]); });
                    callback(prompts);
                }
            }
            else { //member variables
                if (args.token == '.' && scope.queryScope.keys.length > 0 && scope.queryScope.keys.includes(args.previousToken)) { 
                    var prompts = scope.queryScope.items[args.previousToken]
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
        var Tokenizer = ace.require("ace/token_iterator").TokenIterator;

        langTools.setCompleters([])

        editor.getSession().setMode("ace/mode/mysql");
        editor.setValue(""); //clear global ones

        var getLastTokens = function (row, column) {
            var iterator = new Tokenizer(editor.getSession(), row, column);
            var token = iterator.getCurrentToken();
            var args = { token: '', previousToken: ''};
            while (token !== null) {
                if (token === undefined) {
                    token = iterator.stepBackward();
                    continue;
                }
                if (/^\s+$/.test(token.value)) { //skip whitespace
                    token = iterator.stepBackward();
                }
                else {
                    if (args.token == '') {
                        args.token = token.value.trim();
                        token = iterator.stepBackward();
                    }
                    else if (args.previousToken == '') {
                        args.previousToken = token.value.trim();
                        break;
                    }
                }
            }
            return args;
        }

        var staticWordCompleter = {
            getCompletions: function (editor, session, pos, prefix, callback) {
                var args = getLastTokens(pos.row, pos.column);

                var code = editor.getValue();
                args.code = code;
                args.pos = { row: pos.row + 1, column: pos.column };
               
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

        Handlebars.registerHelper('isNull', function (value) {
            return value == null;
        });
    },

    exampleQueries: {
        airportInfo: "/*\n\tAirport Information for Austin\n*/\n\nselect *\nfrom airportinfo\nwhere airportCode = 'kaus'",
        austinDepartures: "/*\n\tCurrent upcoming Austin departures\n*/\n\nselect *\nfrom airlineflightschedules\nwhere departuretime > '{current_date}' and origin = 'kaus'",
        austinDeparturesFriendly: "/*\n\tCurrent upcoming Austin departures\n\tjoins against airportinfo to display friendly name of origin and destination\n*/\n\nselect\n\tf.ident,\n\tf.departuretime,\n\tf.arrivaltime,\n\to.name as origin,\n\td.name as destination\nfrom airlineflightschedules f\njoin airportinfo d on d.airportCode = f.destination\njoin airportinfo o on o.airportCode = f.origin\nwhere f.departuretime > '{current_date}' and f.origin = 'kaus'",
        austinDeparturesNoStatus: "/*\n\tCurrent upcoming Austin departures\n\tjoins to get flightid and uses the flightinfoex to get the status of the flight\n\tIt uses a case statement to determine status\n*/\n\nselect s.ident,\n\te.origin,\n\ts.originCity,\n\te.destination,\n\ts.destinationCity,\n\ts.filed_departuretime,\n\ts.estimatedarrivaltime,\n\tcase\n\twhen e.actualarrivaltime = -1 and e.actualdeparturetime = -1 and e.estimatedarrivaltime = -1\n\t\tthen 'cancelled'\n\twhen e.actualdeparturetime != 0 and e.actualarrivaltime = 0\n\t\tthen 'enroute'\n\twhen e.actualdeparturetime != 0 and e.actualarrivaltime != 0 and e.actualdeparturetime != e.actualarrivaltime\n\t\tthen 'arrived'\n\telse 'not departed'\n\tend as status\nfrom scheduled s\njoin getflightid f on f.departureTime = s.filed_departuretime and f.ident = s.ident\njoin flightinfoex e on e.faFlightID = f.faFlightID\nwhere airport = \"kaus\" and filter = \"airline\""
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
        var token = '';
        if (userName != '' && pass != ''){
            token = userName + ":" + pass;
        }

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