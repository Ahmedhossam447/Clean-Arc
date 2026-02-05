using CleanArc.Core.Interfaces;
using Hangfire;
using System.Linq.Expressions;

namespace CleanArc.Infrastructure.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        public void EnqueueJob<T>(Expression<Func<T,Task>> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
        }
    }
}
