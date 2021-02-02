using Compiler;
using Libraries.Tokens;
using System.Collections.Generic;
using System.Linq;

sealed class SyntaxTree
{
    public SyntaxTree ( IEnumerable<string> errors, ExpressionSyntax root, Tokentypes endOfFileToken )
    {
        this.root = root;
        this.errors = errors.ToArray( );
        this.endOfFileToken = endOfFileToken;
    }

    public ExpressionSyntax root { get; }
    public Tokentypes endOfFileToken { get; }
    public IReadOnlyList<string> errors { get; }
}