lexer grammar SqlLexer;
@lexer::header {#pragma warning disable 3021}

OPENPAREN : '(';
CLOSEPAREN : ')';

SELECT : S E L E C T;
AS : A S;
FROM : F R O M;
INNER : I N N E R;
JOIN: J O I N;
ON: O N;
WHERE : W H E R E;
COMMA : ',';
DOT : '.';
STAR : '*';

CASE : C A S E;
WHEN : W H E N;
THEN: T H E N;
END: E N D;
ELSE: E L S E;

AND : A N D;
OR : O R;

INT : MINUS? DIGIT+;

EQUALS : '=';
LESSTHAN : '<';
LESSTHANEQUAL: '<=';
GREATERTHAN: '>';
GREATERTHANEQUAL: '>=';
NOTEQUAL: '!=';

ID : (LETTER | '_')+;
STRING_LITERAL: '\'' ~('\'')* '\'';

fragment A:('a'|'A');
fragment B:('b'|'B');
fragment C:('c'|'C');
fragment D:('d'|'D');
fragment E:('e'|'E');
fragment F:('f'|'F');
fragment G:('g'|'G');
fragment H:('h'|'H');
fragment I:('i'|'I');
fragment J:('j'|'J');
fragment K:('k'|'K');
fragment L:('l'|'L');
fragment M:('m'|'M');
fragment N:('n'|'N');
fragment O:('o'|'O');
fragment P:('p'|'P');
fragment Q:('q'|'Q');
fragment R:('r'|'R');
fragment S:('s'|'S');
fragment T:('t'|'T');
fragment U:('u'|'U');
fragment V:('v'|'V');
fragment W:('w'|'W');
fragment X:('x'|'X');
fragment Y:('y'|'Y');
fragment Z:('z'|'Z');

fragment MINUS:'-';
fragment DIGIT: '0'..'9';
fragment NEWLINE : ('\n'|'\r');
fragment LETTER :('a'..'z' | 'A'..'Z'); 
WS :  (' '|'\t'|NEWLINE)+ -> skip ;