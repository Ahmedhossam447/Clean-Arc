using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface IBackgroundJobService
    {
        void EnqueueJob<T>(Expression<Func<T,Task>> methodCall);
    }
}
