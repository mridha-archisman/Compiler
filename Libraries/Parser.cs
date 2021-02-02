using System.Collections.Generic;
using Libraries.Tokeninfo;
using Libraries.Tokens;
using Compiler;
using System;

namespace Libraries.Parser
{
    class Parser
    {
        private readonly TokenDetails[] charachters;
        private int position;
        private List<string> errors = new List<string>( );

        public Parser ( string text )
        {
            var tokenList = new List<TokenDetails>( );

            var lexer = new Lexer.Lexer( text );
            TokenDetails tokenDetails;

            do
            {
                tokenDetails = lexer.ParseToken( );

                if ( tokenDetails.tokenType != Tokentypes.WhiteSpace && tokenDetails.tokenType != Tokentypes.Unknown )
                    tokenList.Add( tokenDetails );

            } while ( tokenDetails.tokenType != Tokentypes.EndOfFile );

            this.charachters = tokenList.ToArray( );
            this.position = 0;

            errors.AddRange( lexer.errors );
        }

        private TokenDetails Current
        {
            get
            {
                if ( position >= charachters.Length )
                    return charachters[ charachters.Length - 1 ];

                return charachters[ position ];
            }
        }

        public IEnumerable<string> Errors => errors;

        private TokenDetails nextToken ( )
        {
            var current = Current;
            position++;
            return current;
        }

        private TokenDetails matchToken ( Tokentypes tokenType )
        {
            if ( Current.tokenType == tokenType )
                return nextToken( );

            errors.Add($"ERROR: Unexpected token <{ Current.tokenType }>, expected <{ tokenType }>");

            return new TokenDetails( tokenType, Current.position, null, null );
        }

        public ExpressionSyntax Parse ( )
        {
            var left = ParsePrimaryExpression( );

            while ( Current.tokenType == Tokentypes.Plus || Current.tokenType == Tokentypes.Minus )
            {
                var operatorToken = nextToken( );
                var right = ParsePrimaryExpression( );
                left = new BinaryExpressionSyntax( left, operatorToken, right );
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression ( )
        {
            var numberToken = matchToken( Tokentypes.Number );
            return new NumberExpressionSyntax( numberToken );
        }
    }
}