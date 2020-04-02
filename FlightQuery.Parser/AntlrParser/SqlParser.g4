parser grammar SqlParser;
@parser::header {#pragma warning disable 3021}

options { tokenVocab = SqlLexer; }

program
	: queryStatement* EOF								
	;

queryStatement
	: s=selectStatement f=fromStatement w=whereStatement?	# QueryStatementExp
	;

selectStatement
	: SELECT (selectArg COMMA)* selectArg					# SelectStatementExp
	;

selectArg
	: selectVariable (AS ID)?								# SelectArgsExp
	;

selectVariable
	: ID													# SelectVariableIdExp
	| t=ID DOT m=ID											# SelectVariableMemberExp
	| literal												# LiteralVariableMemberExp
	;

fromStatement
	: FROM t=ID a=ID? j=innerJoinStatement?					# fromStatementExp
	;

innerJoinStatement
	: JOIN t=ID a=ID? ON b=boolExpression j=innerJoinStatement?	# innerJoinStatementExp
	;

whereStatement
	: WHERE b=boolExpression								# whereStatementExp
	;

boolExpression
	: l=andExpression (OR andExpression)*					# BoolStatementExp
	;

andExpression
	:  l=boolTerm (AND boolTerm)*							# AndStatementExp
	;

boolTerm
	: l=selectVariable (o=boolOperator r=selectVariable)?	# BoolOperatorStatementExp
	| OPENPAREN b=boolExpression CLOSEPAREN					# BoolTermParenStatementExpr
	;

boolOperator
	: EQUALS												# EqualsStatmentExp
	| LESSTHAN												# LessThanStatementExp
	| LESSTHANEQUAL											# LessThanEqualStatementExp
	| GREATERTHAN											# GreaterThanStatementExp
	| GREATERTHANEQUAL										# GreaterThanEqualStatementExp
	| NOTEQUAL												# NotEqualStatementExp
	;

literal
	: INT													# IntegerExp
	| STRING_LITERAL										# StringLiteralExp
	;