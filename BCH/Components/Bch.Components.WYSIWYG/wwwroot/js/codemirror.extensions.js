/* code-mirror-autorefresh.js */

// CodeMirror, copyright (c) by Marijn Haverbeke and others
// Distributed under an MIT license: https://codemirror.net/5/LICENSE

(function (mod) {
    if (typeof exports == "object" && typeof module == "object") // CommonJS
        mod(require("../../lib/codemirror"))
    else if (typeof define == "function" && define.amd) // AMD
        define(["../../lib/codemirror"], mod)
    else // Plain browser env
        mod(CodeMirror)
})(function (CodeMirror) {
    "use strict"

    CodeMirror.defineOption("autoRefresh", false, function (cm, val) {
        if (cm.state.autoRefresh) {
            stopListening(cm, cm.state.autoRefresh)
            cm.state.autoRefresh = null
        }
        if (val && cm.display.wrapper.offsetHeight == 0)
            startListening(cm, cm.state.autoRefresh = { delay: val.delay || 250 })
    })

    function startListening(cm, state) {
        function check() {
            if (cm.display.wrapper.offsetHeight) {
                stopListening(cm, state)
                if (cm.display.lastWrapHeight != cm.display.wrapper.clientHeight)
                    cm.refresh()
            } else {
                state.timeout = setTimeout(check, state.delay)
            }
        }
        state.timeout = setTimeout(check, state.delay)
        state.hurry = function () {
            clearTimeout(state.timeout)
            state.timeout = setTimeout(check, 50)
        }
        CodeMirror.on(window, "mouseup", state.hurry)
        CodeMirror.on(window, "keyup", state.hurry)
    }

    function stopListening(_cm, state) {
        clearTimeout(state.timeout)
        CodeMirror.off(window, "mouseup", state.hurry)
        CodeMirror.off(window, "keyup", state.hurry)
    }
});

/* code-mirror-autorefresh.js */

/* code-mirror-formatting.js */
(function () {

    CodeMirror.extendMode("css", {
        commentStart: "/*",
        commentEnd: "*/",
        newlineAfterToken: function (type, content) {
            return /^[;{}]$/.test(content);
        }
    });

    CodeMirror.extendMode("javascript", {
        commentStart: "/*",
        commentEnd: "*/",
        // FIXME semicolons inside of for
        newlineAfterToken: function (type, content, textAfter, state) {
            if (this.jsonMode) {
                return /^[\[,{]$/.test(content) || /^}/.test(textAfter);
            } else {
                if (content == ";" && state.lexical && state.lexical.type == ")") return false;
                return /^[;{}]$/.test(content) && !/^;/.test(textAfter);
            }
        }
    });

    CodeMirror.extendMode("xml", {
        commentStart: "<!--",
        commentEnd: "-->",
        newlineAfterToken: function (type, content, textAfter) {
            return type == "tag" && />$/.test(content) || /^</.test(textAfter);
        }
    });

    // Comment/uncomment the specified range
    CodeMirror.defineExtension("commentRange", function (isComment, from, to) {
        var cm = this, curMode = CodeMirror.innerMode(cm.getMode(), cm.getTokenAt(from).state).mode;
        cm.operation(function () {
            if (isComment) { // Comment range
                cm.replaceRange(curMode.commentEnd, to);
                cm.replaceRange(curMode.commentStart, from);
                if (from.line == to.line && from.ch == to.ch) // An empty comment inserted - put cursor inside
                    cm.setCursor(from.line, from.ch + curMode.commentStart.length);
            } else { // Uncomment range
                var selText = cm.getRange(from, to);
                var startIndex = selText.indexOf(curMode.commentStart);
                var endIndex = selText.lastIndexOf(curMode.commentEnd);
                if (startIndex > -1 && endIndex > -1 && endIndex > startIndex) {
                    // Take string till comment start
                    selText = selText.substr(0, startIndex)
                        // From comment start till comment end
                        + selText.substring(startIndex + curMode.commentStart.length, endIndex)
                        // From comment end till string end
                        + selText.substr(endIndex + curMode.commentEnd.length);
                }
                cm.replaceRange(selText, from, to);
            }
        });
    });

    // Applies automatic mode-aware indentation to the specified range
    CodeMirror.defineExtension("autoIndentRange", function (from, to) {
        var cmInstance = this;
        this.operation(function () {
            for (var i = from.line; i <= to.line; i++) {
                cmInstance.indentLine(i, "smart");
            }
        });
    });

    // Applies automatic formatting to the specified range
    CodeMirror.defineExtension("autoFormatRange", function (from, to) {
        var cm = this;
        var outer = cm.getMode(), text = cm.getRange(from, to).split("\n");
        var state = CodeMirror.copyState(outer, cm.getTokenAt(from).state);
        var tabSize = cm.getOption("tabSize");

        var out = "", lines = 0, atSol = from.ch == 0;
        function newline() {
            out += "\n";
            atSol = true;
            ++lines;
        }

        for (var i = 0; i < text.length; ++i) {
            var stream = new CodeMirror.StringStream(text[i], tabSize);
            while (!stream.eol()) {
                var inner = CodeMirror.innerMode(outer, state);
                var style = outer.token(stream, state), cur = stream.current();
                stream.start = stream.pos;
                if (!atSol || /\S/.test(cur)) {
                    out += cur;
                    atSol = false;
                }
                if (!atSol && inner.mode.newlineAfterToken &&
                    inner.mode.newlineAfterToken(style, cur, stream.string.slice(stream.pos) || text[i + 1] || "", inner.state))
                    newline();
            }
            if (!stream.pos && outer.blankLine) outer.blankLine(state);
            if (!atSol) newline();
        }

        cm.operation(function () {
            cm.replaceRange(out, from, to);
            for (var cur = from.line + 1, end = from.line + lines; cur <= end; ++cur)
                cm.indentLine(cur, "smart");
            cm.setSelection(from, cm.getCursor(false));
        });
    });
})();

/* code-mirror-formatting.js */

/* code-mirror-min-lines.js */
// set minimum number of lines CodeMirror instance is allowed to have
(function (mod) {
    mod(CodeMirror);
})(function (CodeMirror) {
    var fill = function (cm, start, n) {
        while (start < n) {
            let count = cm.lineCount();
            cm.replaceRange("\n", { line: count - 1 }), start++;
            // remove new line change from history (otherwise user could ctrl+z to remove line)
            let history = cm.getHistory();
            history.done.pop(), history.done.pop();
            cm.setHistory(history);
            if (start == n) break;
        }
    };
    var pushLines = function (cm, selection, n) {
        // push lines to last change so that "undo" doesn't add lines back
        var line = cm.lineCount() - 1;
        var history = cm.getHistory();
        history.done[history.done.length - 2].changes.push({
            from: {
                line: line - n,
                ch: cm.getLine(line - n).length,
                sticky: null
            },
            text: [""],
            to: { line: line, ch: 0, sticky: null }
        });
        cm.setHistory(history);
        cm.setCursor({ line: selection.start.line, ch: selection.start.ch });
    };

    var keyMap = {
        Backspace: function (cm) {
            var cursor = cm.getCursor();
            var selection = {
                start: cm.getCursor(true),
                end: cm.getCursor(false)
            };

            // selection
            if (selection.start.line !== selection.end.line) {
                let func = function (e) {
                    var count = cm.lineCount(); // current number of lines
                    var n = cm.options.minLines - count; // lines needed
                    if (e.key == "Backspace" || e.code == "Backspace" || e.which == 8) {
                        fill(cm, 0, n);
                        if (count <= cm.options.minLines) pushLines(cm, selection, n);
                    }
                    cm.display.wrapper.removeEventListener("keydown", func);
                };
                cm.display.wrapper.addEventListener("keydown", func); // fires after CodeMirror.Pass

                return CodeMirror.Pass;
            } else if (selection.start.ch !== selection.end.ch) return CodeMirror.Pass;

            // cursor
            var line = cm.getLine(cursor.line);
            var prev = cm.getLine(cursor.line - 1);
            if (
                cm.lineCount() == cm.options.minLines &&
                prev !== undefined &&
                cursor.ch == 0
            ) {
                if (line.length) {
                    // add a line because this line will be attached to previous line per default behaviour
                    cm.replaceRange("\n", { line: cm.lineCount() - 1 });
                    return CodeMirror.Pass;
                } else cm.setCursor(cursor.line - 1, prev.length); // set cursor at end of previous line
            }
            if (cm.lineCount() > cm.options.minLines || cursor.ch > 0)
                return CodeMirror.Pass;
        },
        Delete: function (cm) {
            var cursor = cm.getCursor();
            var selection = {
                start: cm.getCursor(true),
                end: cm.getCursor(false)
            };

            // selection
            if (selection.start.line !== selection.end.line) {
                let func = function (e) {
                    var count = cm.lineCount(); // current number of lines
                    var n = cm.options.minLines - count; // lines needed
                    if (e.key == "Delete" || e.code == "Delete" || e.which == 46) {
                        fill(cm, 0, n);
                        if (count <= cm.options.minLines) pushLines(cm, selection, n);
                    }
                    cm.display.wrapper.removeEventListener("keydown", func);
                };
                cm.display.wrapper.addEventListener("keydown", func); // fires after CodeMirror.Pass

                return CodeMirror.Pass;
            } else if (selection.start.ch !== selection.end.ch) return CodeMirror.Pass;

            // cursor
            var line = cm.getLine(cursor.line);
            if (cm.lineCount() == cm.options.minLines) {
                if (
                    cursor.ch == 0 &&
                    (line.length !== 0 || cursor.line == cm.lineCount() - 1)
                )
                    return CodeMirror.Pass;
                if (cursor.ch == line.length && cursor.line + 1 < cm.lineCount()) {
                    // add a line because next line will be attached to this line per default behaviour
                    cm.replaceRange("\n", { line: cm.lineCount() - 1 });
                    return CodeMirror.Pass;
                } else if (cursor.ch > 0) return CodeMirror.Pass;
            } else return CodeMirror.Pass;
        }
    };

    var onCut = function (cm) {
        var selection = {
            start: cm.getCursor(true),
            end: cm.getCursor(false)
        };
        setTimeout(function () {
            // wait until after cut is complete
            var count = cm.lineCount(); // current number of lines
            var n = cm.options.minLines - count; // lines needed
            fill(fm, 0, n);
            if (count <= cm.options.minLines) pushLines(cm, selection, n);
        });
    };

    var start = function (cm) {
        // set minimum number of lines on init
        var count = cm.lineCount(); // current number of lines
        cm.setCursor(count); // set the cursor at the end of existing content
        fill(cm, 0, cm.options.minLines - count);

        cm.addKeyMap(keyMap);

        // bind events
        cm.display.wrapper.addEventListener("cut", onCut, true);
    };
    var end = function (cm) {
        cm.removeKeyMap(keyMap);

        // unbind events
        cm.display.wrapper.removeEventListener("cut", onCut, true);
    };

    CodeMirror.defineOption("minLines", undefined, function (cm, val, old) {
        if (val !== undefined && val > 0) start(cm);
        else end(cm);
    });
});
/* code-mirror-min-lines.js */