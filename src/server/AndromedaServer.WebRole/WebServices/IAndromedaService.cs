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
    public interface IAndromedaService
    {
        #region Queries
        [OperationContract]
        Spaceship GetSpaceshipStatus(Guid playerGuid);

        [OperationContract]
        Star GetCurrentStar(Guid playerGuid);

        [OperationContract]
        List<Star> GetVisibleStars(Guid playerGuid);

        [OperationContract]
        List<MerchantShip> GetRaidableShips(Guid playerGuid);

        [OperationContract]
        List<Guid> GetOwnedShips(Guid playerGuid);
        #endregion

        #region Commands
        [OperationContract]
        void RegisterRunLocation(Guid playerGuid, bool isEmulator);

        [OperationContract]
        void LaunchSpaceship(Guid playerGuid, Guid starGuid);

        [OperationContract]
        void Buy(Guid playerGuid, string commodityName, int quantity);

        [OperationContract]
        void BuyMaximum(Guid playerGuid, string commodityName);

        [OperationContract]
        void Sell(Guid playerGuid, string commodityName, int quantity);

        [OperationContract]
        void AddSensor(Guid playerGuid);

        [OperationContract]
        void RemoveSensor(Guid playerGuid);

        [OperationContract]
        void AddDrive(Guid playerGuid);

        [OperationContract]
        void RemoveDrive(Guid playerGuid);

        [OperationContract]
        void UpgradeShip(Guid playerGuid, int modelType);

        [OperationContract]
        void AddCannon(Guid playerGuid);

        [OperationContract]
        void RemoveCannon(Guid playerGuid);

        [OperationContract]
        void AddShield(Guid playerGuid);

        [OperationContract]
        void RemoveShield(Guid playerGuid);

        [OperationContract]
        void Raid(Guid playerGuid, Guid transponderCode);

        [OperationContract]
        void BuyNewShip(Guid playerGuid);
        #endregion
    }
}