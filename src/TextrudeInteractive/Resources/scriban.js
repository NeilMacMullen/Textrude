/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
define('vs/basic-languages/scriban/scriban',["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.language = exports.conf = void 0;
    exports.conf = {
        wordPattern: /(-?\d*\.\d\w*)|([^\`\~\!\#\$\%\^\&\*\(\)\-\=\+\[\{\]\}\\\|\;\:\'\"\,\.\<\>\/\?\s]+)/g,
        "comments": {
            // symbols used for start and end a block comment. Remove this entry if your language does not support block comments
            "blockComment": [ "{%{", "}%}" ]
        },
        // symbols used as brackets
        "brackets": [
            ["{", "}"],
            ["[", "]"],
            ["(", ")"]
        ],
        // symbols that are auto closed when typing
        "autoClosingPairs": [
            ["{", "}"],
            ["[", "]"],
            ["(", ")"],
            ["\"", "\""],
            ["'", "'"],
            ["`", "`"]
        ],
        // symbols that that can be used to surround a selection
        "surroundingPairs": [
            ["{", "}"],
            ["[", "]"],
            ["(", ")"],
            ["\"", "\""],
            ["'", "'"],
            ["`", "`"]
        ],
        "indentationRules": {
            "increaseIndentPattern": "^\\s*(func|else\\s+if|if|else|for|while|capture|with|wrap|when|case)\\b.*$",
            "decreaseIndentPattern": "^\\s*(end|else|else\\s+if|case)\\b[^;]*$"
        },
        folding: {
            markers: {
                start: new RegExp('^\\{\\{'),
                end: new RegExp('^\\}\\}')
            }
        }
    };
    exports.language = {
        defaultToken: '',
        tokenPostfix: '.cs',
        brackets: [
            { open: '{', close: '}', token: 'delimiter.curly' },
            { open: '[', close: ']', token: 'delimiter.square' },
            { open: '(', close: ')', token: 'delimiter.parenthesis' },
            { open: '<', close: '>', token: 'delimiter.angle' }
        ],
        keywords: [
            'func',
            'end',
            'if',
            'else',
            'for',
            'break',
            'continue',
            'in',
            'while',
            'capture',
            'readonly',
            'import',
            'with',
            'wrap',
            'include',
            'ret',
            'case',
            'when',
            'this',
            'empty',
            'tablerow'
        ],
        namespaceFollows: ['namespace', 'using'],
        parenFollows: [],
        operators: [
            '=',
            '??',
            '||',
            '&&',
            '|',
            '^',
            '&',
            '==',
            '!=',
            '<=',
            '>=',
            '<<',
            '+',
            '-',
            '*',
            '/',
            '%',
            '!',
            '~',
            '++',
            '--',
            '+=',
            '-=',
            '*=',
            '/=',
            '%=',
            '&=',
            '|=',
            '^=',
            '<<=',
            '>>=',
            '>>',
            '=>'
        ],
        symbols: /[=><!~?:&|+\-*\/\^%]+/,
        // escape sequences
        escapes: /\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,
        // The main tokenizer for our languages
        tokenizer: {
            root: [
                // identifiers and keywords
                [
                    /\@?[a-zA-Z_]\w*/,
                    {
                        cases: {
                            '@namespaceFollows': {
                                token: 'keyword.$0',
                                next: '@namespace'
                            },
                            '@keywords': {
                                token: 'keyword.$0',
                                next: '@qualified'
                            },
                            '@default': { token: 'identifier', next: '@qualified' }
                        }
                    }
                ],
                // whitespace
                { include: '@whitespace' },
                // delimiters and operators
                [
                    /}/,
                    {
                        cases: {
                            '$S2==interpolatedstring': {
                                token: 'string.quote',
                                next: '@pop'
                            },
                            '$S2==litinterpstring': {
                                token: 'string.quote',
                                next: '@pop'
                            },
                            '@default': '@brackets'
                        }
                    }
                ],
                [/[{}()\[\]]/, '@brackets'],
                [/[<>](?!@symbols)/, '@brackets'],
                [
                    /@symbols/,
                    {
                        cases: {
                            '@operators': 'delimiter',
                            '@default': ''
                        }
                    }
                ],
                // numbers
                [/[0-9_]*\.[0-9_]+([eE][\-+]?\d+)?[fFdD]?/, 'number.float'],
                [/0[xX][0-9a-fA-F_]+/, 'number.hex'],
                [/0[bB][01_]+/, 'number.hex'],
                [/[0-9_]+/, 'number'],
                // delimiter: after number because of .\d floats
                [/[;,.]/, 'delimiter'],
                // strings
                [/"([^"\\]|\\.)*$/, 'string.invalid'],
                [/"/, { token: 'string.quote', next: '@string' }],
                [/\$\@"/, { token: 'string.quote', next: '@litinterpstring' }],
                [/\@"/, { token: 'string.quote', next: '@litstring' }],
                [/\$"/, { token: 'string.quote', next: '@interpolatedstring' }],
                // characters
                [/'[^\\']'/, 'string'],
                [/(')(@escapes)(')/, ['string', 'string.escape', 'string']],
                [/'/, 'string.invalid']
            ],
            qualified: [
                [
                    /[a-zA-Z_][\w]*/,
                    {
                        cases: {
                            '@keywords': { token: 'keyword.$0' },
                            '@default': 'identifier'
                        }
                    }
                ],
                [/\./, 'delimiter'],
                ['', '', '@pop']
            ],
            namespace: [
                { include: '@whitespace' },
                [/[A-Z]\w*/, 'namespace'],
                [/[\.=]/, 'delimiter'],
                ['', '', '@pop']
            ],
            comment: [
                [/[^\/*]+/, 'comment'],
                // [/\/\*/,    'comment', '@push' ],    // no nested comments :-(
                ['\\*/', 'comment', '@pop'],
                [/[\/*]/, 'comment']
            ],
            string: [
                [/[^\\"]+/, 'string'],
                [/@escapes/, 'string.escape'],
                [/\\./, 'string.escape.invalid'],
                [/"/, { token: 'string.quote', next: '@pop' }]
            ],
            litstring: [
                [/[^"]+/, 'string'],
                [/""/, 'string.escape'],
                [/"/, { token: 'string.quote', next: '@pop' }]
            ],
            litinterpstring: [
                [/[^"{]+/, 'string'],
                [/""/, 'string.escape'],
                [/{{/, 'string.escape'],
                [/}}/, 'string.escape'],
                [/{/, { token: 'string.quote', next: 'root.litinterpstring' }],
                [/"/, { token: 'string.quote', next: '@pop' }]
            ],
            interpolatedstring: [
                [/[^\\"{]+/, 'string'],
                [/@escapes/, 'string.escape'],
                [/\\./, 'string.escape.invalid'],
                [/{{/, 'string.escape'],
                [/}}/, 'string.escape'],
                [/{/, { token: 'string.quote', next: 'root.interpolatedstring' }],
                [/"/, { token: 'string.quote', next: '@pop' }]
            ],
            whitespace: [
                [/^[ \t\v\f]*#((r)|(load))(?=\s)/, 'directive.csx'],
                [/^[ \t\v\f]*#\w.*$/, 'namespace.cpp'],
                [/[ \t\v\f\r\n]+/, ''],
                [/\/\*/, 'comment', '@comment'],
                [/\/\/.*$/, 'comment']
            ]
        }
    };
});

