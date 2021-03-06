parser grammar SqlParser;
@parser::header {#pragma warning disable 3021}

options { tokenVocab = SqlLexer; }

program
	: queryStatement* EOF								
	;

queryStatement
	: s=selectStatement f=fromStatement w=whereStatement? l=limitStatement?	# QueryStatementExp
	;

selectStatement
	: SELECT STAR											# SelectStarStatementExp
	| SELECT (selectArg COMMA)* selectArg					# SelectStatementExp
	;

selectArg
	: selectVariable (AS a=ID)?								# SelectArgsExp
	| caseStatement	(AS a=ID)?								# SelectArgsExp
	| mathExpression (AS a=ID)?								# SelectArgsExp
	;

caseStatement
	: CASE whenExpression+ (ELSE s=selectVariable)? END 	# CaseStatementExp
	;

whenExpression
	: WHEN b=boolExpression THEN s=selectVariable			# WhenExpressionExp
	;

selectVariable
	: ID													# SelectVariableIdExp
	| t=ID DOT m=ID											# SelectVariableMemberExp
	| literal												# LiteralVariableMemberExp
	;

fromStatement
	: FROM OPENPAREN q=queryStatement CLOSEPAREN a=ID j=innerJoinStatement?		# fromNestedQueryExp 
	| FROM t=ID a=ID? j=innerJoinStatement?										# fromStatementExp
	;

innerJoinStatement
	: JOIN t=ID a=ID? ON b=boolExpression j=innerJoinStatement?	# innerJoinStatementExp
	;

whereStatement
	: WHERE b=boolExpression 								# whereStatementExp
	;

limitStatement
	: LIMIT (o=INT COMMA)? c=INT							# limitStatementExp 	
	;

mathExpression
    :  l=mathExpressionGroup (( ADD | SUBTRACT ) mathExpressionGroup)*	# addSubtractStatementExp
	;

mathExpressionGroup
	: l=atom (( STAR | DIVIDE ) atom)*						# multDivStatementExp
  	;

atom
    : selectVariable
    | OPENPAREN mathExpression CLOSEPAREN
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
	| CURRENT_TIMESTAMP (OPENPAREN CLOSEPAREN)?				# CurrentTimestampExp
	;