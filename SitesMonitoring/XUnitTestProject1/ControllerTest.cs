using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SitesMonitoring.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace SitesMonitoring.Tests
{

    public class ControllerTest
    {/*
        [Fact]
        public void SitesController_Index()
        {
            var moq = CreateTwoSitesAndSettings(false);
            SitesController controller = new SitesController(moq.Object);

            var resultTask = controller.Index();
            resultTask.Wait();
            var result = resultTask.Result;

            
            
            Assert.NotNull(result);
            var model = (List<Site>)((ViewResult)result).Model;
            Assert.Equal(2, model.Count());
                       
        }

        [Fact]
        public void SitesController_GetWaitTime()
        {
            var moq = CreateTwoSitesAndSettings(false);
            SitesController controller = new SitesController(moq.Object);
            //должно быть по умолчанию)
            Assert.Equal(1000, controller.GetWaitTime());
            var moq1 = CreateTwoSitesAndSettings(true);
            SitesController controller1 = new SitesController(moq1.Object);
            Assert.Equal(10, controller1.GetWaitTime());
        }*/
        public interface IRepositaryIdentityDbContext
        {


        }

        
            public Mock<IRepositaryIdentityDbContext> CreateTwoSitesAndSettings(bool bCreateSettings)
        {
            var SitesTestData = new List<Site>() { new Site { ID = 1, URL = "yandex.ru" } };
            SitesTestData.Add(new Site { ID = 2, URL = "google.com" });
            var sites = MockDbSet(SitesTestData);
            var dbContext = new Mock<IRepositaryIdentityDbContext>();
            //dbContext.Setup(m => m.Site).Returns(sites.Object);
            if (bCreateSettings)
            {
                var SettingsTestData = new List<Settings>() { new Settings { ID = 1, WaitSecond = 10 } };
                var settings = MockDbSet(SettingsTestData);
                //dbContext.Setup(m => m.Settings).Returns(settings.Object);
            }
            return dbContext;
        }*/

        Mock<IDbSet<T>> MockDbSet<T>(IEnumerable<T> list) where T : class, new()
        {
            IQueryable<T> queryableList = list.AsQueryable();
            Mock<IDbSet<T>> dbSetMock = new Mock<IDbSet<T>>();
            dbSetMock.As<IQueryable<T>>()
               .Setup(m => m.Provider)
               .Returns(new AsyncQueryProvider<T>(queryableList.Provider));
            
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(() => queryableList.GetEnumerator());
            dbSetMock.As<IDbAsyncEnumerable<T>>()
              .Setup(m => m.GetAsyncEnumerator())
              .Returns(new AsyncEnumerator<T>(queryableList.GetEnumerator()));


            return dbSetMock;
        }

        class AsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal AsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression) => new AsyncEnumerable<TEntity>(expression);

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new AsyncEnumerable<TElement>(expression);

            public object Execute(Expression expression) => _inner.Execute(expression);

            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute(expression));

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));
        }

        class AsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
        {
            public AsyncEnumerable(Expression expression) : base(expression) { }

            public IDbAsyncEnumerator<T> GetAsyncEnumerator() => new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => GetAsyncEnumerator();

            IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
        }

        class AsyncEnumerator<T> : IDbAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public AsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public void Dispose() => _inner.Dispose();

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken) => Task.FromResult(_inner.MoveNext());

            public T Current => _inner.Current;

            object IDbAsyncEnumerator.Current => Current;
        }

        internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestDbAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestDbAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestDbAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute<TResult>(expression));
            }
        }

        internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
        {
            public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestDbAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            public IDbAsyncEnumerator<T> GetAsyncEnumerator()
            {
                return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
            {
                return GetAsyncEnumerator();
            }

            IQueryProvider IQueryable.Provider
            {
                get { return new TestDbAsyncQueryProvider<T>(this); }
            }
        }

        internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestDbAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public void Dispose()
            {
                _inner.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_inner.MoveNext());
            }

            public T Current
            {
                get { return _inner.Current; }
            }

            object IDbAsyncEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
