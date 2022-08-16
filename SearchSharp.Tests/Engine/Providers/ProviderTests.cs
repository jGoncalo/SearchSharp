using Moq;
using AutoFixture;
using System.Linq.Expressions;
using SearchSharp.Engine.Repositories;
using SearchSharp.Engine.Providers;
using SearchSharp.Engine.Parser.Components;

namespace SearchSharp.Tests.Engine.Providers;

public class ProviderTests {
    public class Data : QueryData {
        public Guid Id { get; set; }
        public int Sum { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is not Data data) return false;
            return data?.Id != Id;
        }
        public override int GetHashCode() => HashCode.Combine(Id, Sum);
    }

    private static IFixture _fixture {
        get {
            var fixture = new Fixture();
            fixture.Customize<Data>(composer => composer.With(d => d.Id, Guid.NewGuid()));
            return fixture;
        }
    }

    private static IRepository<Data, IQueryable<Data>> MockRepo(Expression<Func<IRepository<Data, IQueryable<Data>>, bool>> predicate) => 
        Mock.Of<IRepository<Data, IQueryable<Data>>>(predicate);
    private static IRepositoryFactory<Data, IRepository<Data, IQueryable<Data>>, IQueryable<Data>> MockRepoFactory(Expression<Func<IRepositoryFactory<Data, IRepository<Data, IQueryable<Data>>, IQueryable<Data>>, bool>> predicate) => 
        Mock.Of<IRepositoryFactory<Data, IRepository<Data, IQueryable<Data>>, IQueryable<Data>>>(predicate);

    [Fact]
    public async Task GetAsync_NoParametersCall_ListRepo(){
        #region Assemble
        var dataSource = _fixture.CreateMany<Data>(10).AsQueryable();
        var mockRepo = MockRepo(mockRepo => mockRepo.CountAsync(It.IsAny<CancellationToken>()) == Task.FromResult(dataSource.Count())
            && mockRepo.ApplyAsync(It.IsAny<Expression<Func<Data, bool>>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask
            && mockRepo.ModifyAsync(It.IsAny<Func<IQueryable<Data>, IQueryable<Data>>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask
            && mockRepo.FetchAsync(It.IsAny<CancellationToken>()) == Task.FromResult(dataSource.ToArray()));
        var mockRepoFactory = MockRepoFactory(repoFactory => repoFactory.Instance() == mockRepo);

        var provider = new Provider<Data, IRepository<Data, IQueryable<Data>>, IQueryable<Data>>("provider", mockRepoFactory);
        #endregion

        #region Act
        var results = await provider.GetAsync(Array.Empty<Command>(), null, CancellationToken.None);
        #endregion

        #region Assert
        Mock.Get(mockRepoFactory).Verify(it => it.Instance(), Times.Once);
        Mock.Get(mockRepo).Let(mock => {
            mock.Verify(it => it.ModifyAsync(It.IsAny<Func<IQueryable<Data>, IQueryable<Data>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            mock.Verify(it => it.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(it => it.ApplyAsync(It.IsAny<Expression<Func<Data, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
            mock.Verify(it => it.FetchAsync(It.IsAny<CancellationToken>()), Times.Once);
        });

        Assert.NotNull(results);
        Assert.Equal(dataSource.Count(), results.Count);
        Assert.NotEmpty(results.Content);

        Assert.Equal(dataSource.ToArray(), results.Content);
        #endregion
    }

    [Fact]
    public async Task GetAsync_UnknownCommandsNoCaluse_ListRepo(){
        #region Assemble
        var dataSource = _fixture.CreateMany<Data>(10).AsQueryable();
        var mockRepo = MockRepo(mockRepo => mockRepo.CountAsync(It.IsAny<CancellationToken>()) == Task.FromResult(dataSource.Count())
            && mockRepo.ApplyAsync(It.IsAny<Expression<Func<Data, bool>>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask
            && mockRepo.ModifyAsync(It.IsAny<Func<IQueryable<Data>, IQueryable<Data>>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask
            && mockRepo.FetchAsync(It.IsAny<CancellationToken>()) == Task.FromResult(dataSource.ToArray()));
        var mockRepoFactory = MockRepoFactory(repoFactory => repoFactory.Instance() == mockRepo);

        var provider = new Provider<Data, IRepository<Data, IQueryable<Data>>, IQueryable<Data>>("provider", mockRepoFactory);
        #endregion

        #region Act
        var results = await provider.GetAsync(new [] {
            new Command("unknown1", new Arguments(10.AsLiteral())),
            new Command("unknown2", new Arguments(13.AsLiteral())),
        }, null, CancellationToken.None);
        #endregion

        #region Assert
        Mock.Get(mockRepoFactory).Verify(it => it.Instance(), Times.Once);
        Mock.Get(mockRepo).Let(mock => {
            mock.Verify(it => it.ModifyAsync(It.IsAny<Func<IQueryable<Data>, IQueryable<Data>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            mock.Verify(it => it.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(it => it.ApplyAsync(It.IsAny<Expression<Func<Data, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
            mock.Verify(it => it.FetchAsync(It.IsAny<CancellationToken>()), Times.Once);
        });

        Assert.NotNull(results);
        Assert.Equal(dataSource.Count(), results.Count);
        Assert.NotEmpty(results.Content);

        Assert.Equal(dataSource.ToArray(), results.Content);
        #endregion
    }

    [Fact]
    public async Task GetAsync_NoCommandsWithCaluse_ListRepo(){
        #region Assemble
        var dataSource = _fixture.CreateMany<Data>(10).AsQueryable();
        Expression<Func<Data, bool>> whereClause = (d) => d.Sum % 2 == 0;
        var mockRepo = MockRepo(mockRepo => mockRepo.CountAsync(It.IsAny<CancellationToken>()) == Task.FromResult(dataSource.Count())
            && mockRepo.ApplyAsync(It.IsAny<Expression<Func<Data, bool>>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask
            && mockRepo.ModifyAsync(It.IsAny<Func<IQueryable<Data>, IQueryable<Data>>>(), It.IsAny<CancellationToken>()) == Task.CompletedTask
            && mockRepo.FetchAsync(It.IsAny<CancellationToken>()) == Task.FromResult(dataSource.Where(whereClause).ToArray()));
        var mockRepoFactory = MockRepoFactory(repoFactory => repoFactory.Instance() == mockRepo);

        var provider = new Provider<Data, IRepository<Data, IQueryable<Data>>, IQueryable<Data>>("provider", mockRepoFactory);
        #endregion

        #region Act
        var results = await provider.GetAsync(new [] {
            new Command("unknown1", new Arguments(10.AsLiteral())),
            new Command("unknown2", new Arguments(13.AsLiteral())),
        }, whereClause, CancellationToken.None);
        #endregion

        #region Assert
        Mock.Get(mockRepoFactory).Verify(it => it.Instance(), Times.Once);
        Mock.Get(mockRepo).Let(mock => {
            mock.Verify(it => it.ModifyAsync(It.IsAny<Func<IQueryable<Data>, IQueryable<Data>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            mock.Verify(it => it.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(it => it.ApplyAsync(It.IsAny<Expression<Func<Data, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            mock.Verify(it => it.FetchAsync(It.IsAny<CancellationToken>()), Times.Once);
        });

        Assert.NotNull(results);
        Assert.Equal(dataSource.Count(), results.Count);
        Assert.NotEmpty(results.Content);

        Assert.Equal(dataSource.Where(whereClause).ToArray(), results.Content);
        #endregion
    }
}