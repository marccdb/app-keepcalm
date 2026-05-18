using KeepCalm.Data;
using KeepCalm.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace KeepCalm.Tests.Helpers
{
    public static class TestHelpers
    {
        public static (Mock<MongoDbContext> Mock, List<TaskItem> Tasks, List<MicroStep> MicroSteps) CreateFullContext(
            List<TaskItem>? tasks = null,
            List<MicroStep>? microSteps = null)
        {
            var taskList = tasks ?? new List<TaskItem>();
            var microStepList = microSteps ?? new List<MicroStep>();

            var mockContext = new Mock<MongoDbContext>(new DbContextOptions<MongoDbContext>()) { CallBase = true };
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            var mockTasks = CreateAsyncDbSet(taskList);
            var mockMicroSteps = CreateAsyncDbSet(microStepList);

            mockContext.Setup(x => x.Tasks).Returns(mockTasks.Object);
            mockContext.Setup(x => x.MicroSteps).Returns(mockMicroSteps.Object);
            mockContext.Setup(x => x.FocusSessions).Returns(CreateAsyncDbSet(new List<FocusSession>()).Object);

            return (mockContext, taskList, microStepList);
        }

        private static Mock<DbSet<T>> CreateAsyncDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var asyncEnumerable = new TestAsyncEnumerable<T>(data);

            var mockSet = new Mock<DbSet<T>>() { CallBase = true };

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(asyncEnumerable));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator);

            mockSet.Setup(s => s.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((object[] ids, CancellationToken _) =>
                {
                    if (ids == null || ids.Length == 0) return null;
                    var idProp = typeof(T).GetProperty("Id");
                    return data.FirstOrDefault(x => idProp?.GetValue(x)?.Equals(ids[0]) == true);
                });

            mockSet.Setup(s => s.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync<T, CancellationToken>((entity, _) =>
                {
                    data.Add(entity);
                    return entity;
                });

            mockSet.Setup(s => s.AddAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync<CancellationToken>(async _ =>
                {
                    var entity = Activator.CreateInstance<T>();
                    data.Add(entity);
                    return entity;
                });

            mockSet.Setup(s => s.Add(It.IsAny<T>())).Returns<T>(entity =>
            {
                data.Add(entity);
                return entity;
            });

            mockSet.Setup(s => s.Remove(It.IsAny<T>())).Callback<T>(entity => data.Remove(entity));
            mockSet.Setup(s => s.Attach(It.IsAny<T>())).Returns<T>(entity =>
            {
                if (!data.Contains(entity)) data.Add(entity);
                return entity;
            });

            mockSet.Setup(s => s.AsAsyncEnumerable()).Returns(asyncEnumerable);

            return mockSet;
        }
    }

    public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly TestAsyncEnumerable<T> _source;

        public TestAsyncQueryProvider(TestAsyncEnumerable<T> source)
        {
            _source = source;
        }

        public bool HasLocalData => true;

        public IAsyncEnumerable<TResult> CreateAsyncQuery<TResult>(Expression expression)
            => new TestAsyncEnumerable<TResult>(expression, this);

        public TResult Execute<TResult>(Expression expression)
        {
            if (typeof(TResult).IsAssignableFrom(typeof(IQueryable<T>)))
            {
                return (TResult)(object)_source;
            }
            return default(TResult);
        }

        public TResult Execute<TResult>(Expression expression, CancellationToken token = default)
        {
            if (expression is MethodCallExpression methodCall)
            {
                var sourceArg = methodCall.Arguments[0] as IQueryable<T>;
                if (sourceArg == null) return default(TResult);

                var sourceList = _source.Data;

                if (methodCall.Method.Name == "ToListAsync")
                {
                    return (TResult)(object)sourceList;
                }

                if (methodCall.Method.Name == "FirstOrDefaultAsync")
                {
                    return (TResult)(object)(sourceList.Count > 0 ? sourceList[0] : null);
                }

                if (methodCall.Method.Name == "CountAsync")
                {
                    return (TResult)(object)sourceList.Count;
                }

                if (methodCall.Method.Name == "AnyAsync")
                {
                    return (TResult)(object)(sourceList.Count > 0);
                }

                if (methodCall.Method.Name == "MaxAsync")
                {
                    return (TResult)(object)(sourceList.Count > 0 ? 0 : -1);
                }

                if (methodCall.Method.Name == "AllAsync")
                {
                    return (TResult)(object)(sourceList.Count == 0 || true);
                }

                if (methodCall.Method.Name == "AsNoTracking")
                {
                    return (TResult)(object)_source;
                }

                if (methodCall.Method.Name == "Where")
                {
                    var predicate = (Expression<Func<T, bool>>)methodCall.Arguments[1];
                    var filtered = sourceList.Where(predicate.Compile()).ToList();
                    return (TResult)(object)new TestAsyncEnumerable<T>(filtered);
                }

                if (methodCall.Method.Name == "OrderBy")
                {
                    return (TResult)(object)_source;
                }

                if (methodCall.Method.Name == "Select")
                {
                    return (TResult)(object)_source;
                }

                if (methodCall.Method.Name == "Contains")
                {
                    var value = methodCall.Arguments[0];
                    return (TResult)(object)(sourceList.Count > 0);
                }

                if (methodCall.Method.Name == "DefaultIfEmpty")
                {
                    return (TResult)(object)_source;
                }
            }

            return default(TResult);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) where TElement : class
            => new TestAsyncEnumerable<TElement>(expression, this);

        public object Execute(Expression expression) => null!;
    }

    public class TestAsyncEnumerable<T> : IAsyncEnumerable<T>, IQueryable<T>
    {
        public List<T> Data { get; private set; }

        public TestAsyncEnumerable(List<T> data)
        {
            Data = data;
            Provider = new TestAsyncQueryProvider<T>(this);
            Expression = Expression.Constant(this);
            ElementType = typeof(T);
        }

        public TestAsyncEnumerable(Expression expression, IAsyncQueryProvider provider)
        {
            Data = new List<T>();
            Provider = provider;
            Expression = expression;
            ElementType = typeof(T);
        }

        public Expression Expression { get; }
        public Type ElementType { get; }
        public IAsyncQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator() => Data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IAsyncEnumerable<TResult> AsAsyncEnumerable<TResult>()
            => new TestAsyncEnumerable<TResult>(Data.Cast<TResult>().ToList());
    }
}
