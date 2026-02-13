using System.Linq.Expressions;

namespace CleanArc.Core.Interfaces
{
    public interface IBackgroundJobService
    {
        void EnqueueJob<T>(Expression<Func<T,Task>> methodCall);
    }
}
