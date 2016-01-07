using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Localization.Extraction.Analysis
{
	public class CsAnalyzer : IAnalyzer
	{
		public string DefaultExtension
		{
			get
			{
				return ".cs";
			}
		}

		public IEnumerable<Keyword> DefaultKeywords
		{
			get
			{
				return new[]
				{
					Keyword.Parse("GetString"),
					Keyword.Parse("GetPluralString:1,2"),
					Keyword.Parse("GetParticularString:1c,2"),
					Keyword.Parse("GetParticularPluralString:1c,2,3")
				};
			}
		}

		public IEnumerable<Invocation> GetInvocations(TextReader reader, string fileName = null)
		{
			var code = reader.ReadToEnd();

			var result = new List<Invocation>();

			var tree = CSharpSyntaxTree.ParseText(code);

			var root = tree.GetCompilationUnitRoot();

			var invocationNodes = root.DescendantNodes()
				.OfType<InvocationExpressionSyntax>()
				.ToArray();

			foreach (var node in invocationNodes)
			{
				var name = "";
				if (node.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
					name = ((MemberAccessExpressionSyntax)node.Expression).Name.Identifier.Text;
				else if (node.Expression.Kind() == SyntaxKind.IdentifierName)
					name = ((IdentifierNameSyntax)node.Expression).Identifier.Text;
				else
					// unsupported
					continue;

				var arguments = node.ArgumentList.Arguments
					.Select(argument =>
					{
						var expression = argument.Expression;

						if (expression.Kind() == SyntaxKind.StringLiteralExpression)
							return new Argument(null, ((LiteralExpressionSyntax)expression).Token.ValueText);

						// unsupported argument
						return new Argument(null, null);
					})
					.ToArray();

				var location = node.Expression.GetLocation()
					.GetLineSpan();

				result.Add(new Invocation(
					name, arguments,
					source: new SourceInfo(fileName, location.StartLinePosition.Line, location.StartLinePosition.Character)
				));
			}

			var attributeNodes = root.DescendantNodes()
				.OfType<AttributeSyntax>()
				.ToArray();

			foreach (var node in attributeNodes)
			{
				var name = "";
				if (node.Name?.Kind() == SyntaxKind.IdentifierName)
					name = ((IdentifierNameSyntax)node.Name).Identifier.Text;
				else
					// unsupported
					continue;

				var arguments = default(IReadOnlyList<Argument>);
				if (node.ArgumentList != null)
				{
					arguments = node.ArgumentList.Arguments
						.Select(argument =>
						{
							var expression = argument.Expression;

							if (expression.Kind() == SyntaxKind.StringLiteralExpression)
								return new Argument(null, ((LiteralExpressionSyntax)expression).Token.ValueText);

							// unsupported argument
							return new Argument(null, null);
						})
						.ToArray();
				}
				else
					arguments = new Argument[0];

				var location = node.GetLocation()
					.GetLineSpan();

				result.Add(new Invocation(
					name, arguments,
					source: new SourceInfo(fileName, location.StartLinePosition.Line, location.StartLinePosition.Character)
				));
			}

			var indexerNodes = root.DescendantNodes()
				.OfType<ElementAccessExpressionSyntax>()
				.ToArray();

			foreach (var node in indexerNodes)
			{
				var name = "";
				if (node.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
					name = ((MemberAccessExpressionSyntax)node.Expression).Name.Identifier.Text;
				else if (node.Expression.Kind() == SyntaxKind.IdentifierName)
					name = ((IdentifierNameSyntax)node.Expression).Identifier.Text;
				else
					// unsupported
					continue;

				var arguments = default(IReadOnlyList<Argument>);
				if (node.ArgumentList != null)
				{
					arguments = node.ArgumentList.Arguments
						.Select(argument =>
						{
							var expression = argument.Expression;

							if (expression.Kind() == SyntaxKind.StringLiteralExpression)
								return new Argument(null, ((LiteralExpressionSyntax)expression).Token.ValueText);

							// unsupported argument
							return new Argument(null, null);
						})
						.ToArray();
				}
				else
					arguments = new Argument[0];

				var location = node.GetLocation()
					.GetLineSpan();

				result.Add(new Invocation(
					name, arguments,
					source: new SourceInfo(fileName, location.StartLinePosition.Line, location.StartLinePosition.Character)
				));
			}

			return result;
		}
	}
}
