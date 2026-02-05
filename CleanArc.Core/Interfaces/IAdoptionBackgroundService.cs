using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface IAdoptionBackgroundService
    {
        Task ProcessRejectedRequestsAsync(int animalId, int AcceptedReqId);
    }
}
