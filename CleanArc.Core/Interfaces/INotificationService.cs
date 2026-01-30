using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface INotificationService
    {
       public Task SendNotificationAsync(string userId, string type, object data);
    }
}
