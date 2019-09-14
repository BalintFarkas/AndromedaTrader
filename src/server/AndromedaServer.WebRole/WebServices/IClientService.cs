using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Andromeda.ClientEntities;

namespace Andromeda.WebServices
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        List<MapObject> GetStars();

        [OperationContract]
        List<MapObject> GetShips();
    }
}