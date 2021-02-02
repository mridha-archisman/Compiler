using System;
using Libraries.Lexer;
using Libraries.Tokens;
using Libraries.Tokeninfo;
using System.Collections.Generic;
using Libraries.Parser;
using System.Linq;

namespace Compiler
{
    class Program
    {
        static void Main ( )
        {
            while( true )
            {
                Console.Write("> ");

                var line = Console.ReadLine( );
                
                if ( string.IsNullOrWhiteSpace(line) )
                    return;

                var parser = new Parser( line );
                var expression = parser.Parse( );

                prettyPrint( expression );

                if ( parser.Errors.Any( ) )
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach ( var error in parser.Errors )
                        Console.WriteLine( error );

                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        static void prettyPrint ( SyntaxNode node, string indent = "", bool isLastChild = true, bool firstChild = true )
        {

            var marker = isLastChild ? "└──" : "├──";
            var lastChild = node.GetChildren( ).LastOrDefault( );

            Console.Write( indent + (firstChild ? "" : marker) + node.tokenType );

            if ( node is TokenDetails t && t.value != null )
            {
                Console.Write(" ");
                Console.Write( t.value );
            }

            Console.WriteLine( );

            indent += isLastChild ? "    ": "│   ";

            foreach( var child in node.GetChildren( ) )
                prettyPrint( child, indent, child == lastChild, false );
        }
    }

    abstract class SyntaxNode
    {
        abstract public Tokentypes tokenType { get; }
        abstract public IEnumerable<SyntaxNode> GetChildren( );
    }

    abstract class ExpressionSyntax: SyntaxNode
    {}

    sealed class NumberExpressionSyntax: ExpressionSyntax
    {
        public NumberExpressionSyntax ( TokenDetails numberToken )
        {
            this.numberToken = numberToken;
        }

        public override Tokentypes tokenType
        {
            get
            {
                return Tokentypes.NumberExpression;
            }
        }

        public TokenDetails numberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren( )
        {
            yield return numberToken;
        }
    }

    sealed class BinaryExpressionSyntax: ExpressionSyntax
    {
        public BinaryExpressionSyntax ( ExpressionSyntax left, TokenDetails operatorToken, ExpressionSyntax right )
        {
            this.right = right;
            this.operatorToken = operatorToken;
            this.left = left;
        }

        public override Tokentypes tokenType
        {
            get
            {
                return Tokentypes.BinaryExpression;
            }
        }

        public ExpressionSyntax left { get; }
        public TokenDetails operatorToken { get; }
        public ExpressionSyntax right { get; }

        public override IEnumerable<SyntaxNode> GetChildren( )
        {
            yield return left;
            yield return operatorToken;
            yield return right;
        }
    }
}