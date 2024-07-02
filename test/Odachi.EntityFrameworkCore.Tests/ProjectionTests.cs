using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VerifyXunit;
using Xunit;

namespace Odachi.EntityFrameworkCore.Tests;

public class TestUser
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }

    public required ICollection<TestArticle> AuthoredArticles { get; set; }
}

public class TestArticle
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public required TestUser Author { get; set; }
}

public class TestUserOut
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public static readonly Expression<Func<TestUser, TestUserOut>> Build =
        user => new TestUserOut()
        {
            Id = user.Id,
            Name = user.Name,
        };
}

public class TestArticleOut
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required TestUserOut Author { get; set; }

    public static readonly Expression<Func<TestArticle, TestArticleOut>> Build =
        article => new TestArticleOut()
        {
            Id = article.Id,
            Title = article.Title,
            Author = EF.Functions.Project(article.Author, TestUserOut.Build),
        };
}

class TestContext : DbContext
{
    public DbSet<TestUser> Users { get; set; }
    public DbSet<TestArticle> Articles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("DataSource=:memory:");
        optionsBuilder.UseProjectTranslation();

        base.OnConfiguring(optionsBuilder);
    }
}

public class ProjectionTests
{
    [Fact]
    public Task Can_project_inline()
    {
        using var context = new TestContext();

        var query = context.Articles
            .Where(e => e.Id == 1)
            .Select(x => new TestArticleOut()
            {
                Id = x.Id,
                Title = x.Title,
                Author = EF.Functions.Project(x.Author, x => new TestUserOut()
                {
                    Id = x.Id,
                    Name = x.Name,
                })
            })
            .ToQueryString();

        return Verifier.Verify(query);
    }

    [Fact]
    public Task Can_project_inline_anonymous()
    {
        using var context = new TestContext();

        var query = context.Articles
            .Where(e => e.Id == 1)
            .Select(x => new
            {
                x.Id,
                x.Title,
                Author = EF.Functions.Project(x.Author, x => new
                {
                    x.Id,
                    x.Name,
                })
            })
            .ToQueryString();

        return Verifier.Verify(query);
    }

    [Fact]
    public Task Can_project_external_expression()
    {
        using var context = new TestContext();

        var query = context.Articles
            .Where(e => e.Id == 1)
            .Select(TestArticleOut.Build)
            .ToQueryString();

        return Verifier.Verify(query);
    }
}
