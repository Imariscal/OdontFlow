using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace OdontFlow.Infrastructure.Repositories.Utilities;

public class ExpressionStringBuilderVisitor : ExpressionVisitor
{
    private readonly StringBuilder _sb = new();
    public override string ToString() => _sb.ToString();

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _sb.Append('('); Visit(node.Left);

        _sb.Append(GetOperatorString(node.NodeType));

        VisitConstant(Expression.Constant(Expression.Lambda(node.Right).Compile().DynamicInvoke()));
        _sb.Append(')');
        return node;
    }

    protected override Expression VisitMember(MemberExpression node) 
    { _sb.Append(node.Member.Name); return node; }
    protected override Expression VisitConstant(ConstantExpression node)
    { _sb.Append(JsonSerializer.Serialize(node.Value)); return node; }

    protected override Expression VisitParameter(ParameterExpression node) 
    { _sb.Append(node.Name); return node; }

    private string GetOperatorString(ExpressionType type)
    {
        return type switch
        {
            ExpressionType.Equal => "==",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "&&",
            ExpressionType.OrElse => "||",
            _ => type.ToString(),
        };
    }
}
