﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17379
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AndromedaScaffold.WorkerRole.AndromedaServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Spaceship", Namespace="http://schemas.datacontract.org/2004/07/Andromeda.ClientEntities")]
    [System.SerializableAttribute()]
    public partial class Spaceship : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CannonCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.CommodityInHold> CargoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int DriveCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int FreeCapacityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsInTransitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime LastRaidedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ModificationsRemainingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long MoneyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int SensorCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double SensorRangeInLightYearsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int ShieldCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double SpeedInLightYearsPerMinuteField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TotalCapacityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TotalShipsOwnedByPlayerField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CannonCount {
            get {
                return this.CannonCountField;
            }
            set {
                if ((this.CannonCountField.Equals(value) != true)) {
                    this.CannonCountField = value;
                    this.RaisePropertyChanged("CannonCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.CommodityInHold> Cargo {
            get {
                return this.CargoField;
            }
            set {
                if ((object.ReferenceEquals(this.CargoField, value) != true)) {
                    this.CargoField = value;
                    this.RaisePropertyChanged("Cargo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DriveCount {
            get {
                return this.DriveCountField;
            }
            set {
                if ((this.DriveCountField.Equals(value) != true)) {
                    this.DriveCountField = value;
                    this.RaisePropertyChanged("DriveCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FreeCapacity {
            get {
                return this.FreeCapacityField;
            }
            set {
                if ((this.FreeCapacityField.Equals(value) != true)) {
                    this.FreeCapacityField = value;
                    this.RaisePropertyChanged("FreeCapacity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsInTransit {
            get {
                return this.IsInTransitField;
            }
            set {
                if ((this.IsInTransitField.Equals(value) != true)) {
                    this.IsInTransitField = value;
                    this.RaisePropertyChanged("IsInTransit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastRaided {
            get {
                return this.LastRaidedField;
            }
            set {
                if ((this.LastRaidedField.Equals(value) != true)) {
                    this.LastRaidedField = value;
                    this.RaisePropertyChanged("LastRaided");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ModificationsRemaining {
            get {
                return this.ModificationsRemainingField;
            }
            set {
                if ((this.ModificationsRemainingField.Equals(value) != true)) {
                    this.ModificationsRemainingField = value;
                    this.RaisePropertyChanged("ModificationsRemaining");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Money {
            get {
                return this.MoneyField;
            }
            set {
                if ((this.MoneyField.Equals(value) != true)) {
                    this.MoneyField = value;
                    this.RaisePropertyChanged("Money");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SensorCount {
            get {
                return this.SensorCountField;
            }
            set {
                if ((this.SensorCountField.Equals(value) != true)) {
                    this.SensorCountField = value;
                    this.RaisePropertyChanged("SensorCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double SensorRangeInLightYears {
            get {
                return this.SensorRangeInLightYearsField;
            }
            set {
                if ((this.SensorRangeInLightYearsField.Equals(value) != true)) {
                    this.SensorRangeInLightYearsField = value;
                    this.RaisePropertyChanged("SensorRangeInLightYears");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ShieldCount {
            get {
                return this.ShieldCountField;
            }
            set {
                if ((this.ShieldCountField.Equals(value) != true)) {
                    this.ShieldCountField = value;
                    this.RaisePropertyChanged("ShieldCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double SpeedInLightYearsPerMinute {
            get {
                return this.SpeedInLightYearsPerMinuteField;
            }
            set {
                if ((this.SpeedInLightYearsPerMinuteField.Equals(value) != true)) {
                    this.SpeedInLightYearsPerMinuteField = value;
                    this.RaisePropertyChanged("SpeedInLightYearsPerMinute");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TotalCapacity {
            get {
                return this.TotalCapacityField;
            }
            set {
                if ((this.TotalCapacityField.Equals(value) != true)) {
                    this.TotalCapacityField = value;
                    this.RaisePropertyChanged("TotalCapacity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TotalShipsOwnedByPlayer {
            get {
                return this.TotalShipsOwnedByPlayerField;
            }
            set {
                if ((this.TotalShipsOwnedByPlayerField.Equals(value) != true)) {
                    this.TotalShipsOwnedByPlayerField = value;
                    this.RaisePropertyChanged("TotalShipsOwnedByPlayer");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CommodityInHold", Namespace="http://schemas.datacontract.org/2004/07/Andromeda.ClientEntities")]
    [System.SerializableAttribute()]
    public partial class CommodityInHold : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int StockField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Stock {
            get {
                return this.StockField;
            }
            set {
                if ((this.StockField.Equals(value) != true)) {
                    this.StockField = value;
                    this.RaisePropertyChanged("Stock");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Star", Namespace="http://schemas.datacontract.org/2004/07/Andromeda.ClientEntities")]
    [System.SerializableAttribute()]
    public partial class Star : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.CommodityAtStar> CommoditiesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DistanceInLightYearsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid StarGuidField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.CommodityAtStar> Commodities {
            get {
                return this.CommoditiesField;
            }
            set {
                if ((object.ReferenceEquals(this.CommoditiesField, value) != true)) {
                    this.CommoditiesField = value;
                    this.RaisePropertyChanged("Commodities");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double DistanceInLightYears {
            get {
                return this.DistanceInLightYearsField;
            }
            set {
                if ((this.DistanceInLightYearsField.Equals(value) != true)) {
                    this.DistanceInLightYearsField = value;
                    this.RaisePropertyChanged("DistanceInLightYears");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid StarGuid {
            get {
                return this.StarGuidField;
            }
            set {
                if ((this.StarGuidField.Equals(value) != true)) {
                    this.StarGuidField = value;
                    this.RaisePropertyChanged("StarGuid");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CommodityAtStar", Namespace="http://schemas.datacontract.org/2004/07/Andromeda.ClientEntities")]
    [System.SerializableAttribute()]
    public partial class CommodityAtStar : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int PriceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int StockField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Price {
            get {
                return this.PriceField;
            }
            set {
                if ((this.PriceField.Equals(value) != true)) {
                    this.PriceField = value;
                    this.RaisePropertyChanged("Price");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Stock {
            get {
                return this.StockField;
            }
            set {
                if ((this.StockField.Equals(value) != true)) {
                    this.StockField = value;
                    this.RaisePropertyChanged("Stock");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MerchantShip", Namespace="http://schemas.datacontract.org/2004/07/Andromeda.ClientEntities")]
    [System.SerializableAttribute()]
    public partial class MerchantShip : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DistanceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid TransponderCodeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Distance {
            get {
                return this.DistanceField;
            }
            set {
                if ((this.DistanceField.Equals(value) != true)) {
                    this.DistanceField = value;
                    this.RaisePropertyChanged("Distance");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid TransponderCode {
            get {
                return this.TransponderCodeField;
            }
            set {
                if ((this.TransponderCodeField.Equals(value) != true)) {
                    this.TransponderCodeField = value;
                    this.RaisePropertyChanged("TransponderCode");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AndromedaServiceReference.IAndromedaService")]
    public interface IAndromedaService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/GetSpaceshipStatus", ReplyAction="http://tempuri.org/IAndromedaService/GetSpaceshipStatusResponse")]
        AndromedaScaffold.WorkerRole.AndromedaServiceReference.Spaceship GetSpaceshipStatus(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/GetCurrentStar", ReplyAction="http://tempuri.org/IAndromedaService/GetCurrentStarResponse")]
        AndromedaScaffold.WorkerRole.AndromedaServiceReference.Star GetCurrentStar(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/GetVisibleStars", ReplyAction="http://tempuri.org/IAndromedaService/GetVisibleStarsResponse")]
        System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.Star> GetVisibleStars(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/GetRaidableShips", ReplyAction="http://tempuri.org/IAndromedaService/GetRaidableShipsResponse")]
        System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.MerchantShip> GetRaidableShips(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/GetOwnedShips", ReplyAction="http://tempuri.org/IAndromedaService/GetOwnedShipsResponse")]
        System.Collections.Generic.List<System.Guid> GetOwnedShips(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/RegisterRunLocation", ReplyAction="http://tempuri.org/IAndromedaService/RegisterRunLocationResponse")]
        void RegisterRunLocation(System.Guid playerGuid, bool isEmulator);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/LaunchSpaceship", ReplyAction="http://tempuri.org/IAndromedaService/LaunchSpaceshipResponse")]
        void LaunchSpaceship(System.Guid playerGuid, System.Guid starGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/Buy", ReplyAction="http://tempuri.org/IAndromedaService/BuyResponse")]
        void Buy(System.Guid playerGuid, string commodityName, int quantity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/BuyMaximum", ReplyAction="http://tempuri.org/IAndromedaService/BuyMaximumResponse")]
        void BuyMaximum(System.Guid playerGuid, string commodityName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/Sell", ReplyAction="http://tempuri.org/IAndromedaService/SellResponse")]
        void Sell(System.Guid playerGuid, string commodityName, int quantity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/AddSensor", ReplyAction="http://tempuri.org/IAndromedaService/AddSensorResponse")]
        void AddSensor(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/RemoveSensor", ReplyAction="http://tempuri.org/IAndromedaService/RemoveSensorResponse")]
        void RemoveSensor(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/AddDrive", ReplyAction="http://tempuri.org/IAndromedaService/AddDriveResponse")]
        void AddDrive(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/RemoveDrive", ReplyAction="http://tempuri.org/IAndromedaService/RemoveDriveResponse")]
        void RemoveDrive(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/UpgradeShip", ReplyAction="http://tempuri.org/IAndromedaService/UpgradeShipResponse")]
        void UpgradeShip(System.Guid playerGuid, int modelType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/AddCannon", ReplyAction="http://tempuri.org/IAndromedaService/AddCannonResponse")]
        void AddCannon(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/RemoveCannon", ReplyAction="http://tempuri.org/IAndromedaService/RemoveCannonResponse")]
        void RemoveCannon(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/AddShield", ReplyAction="http://tempuri.org/IAndromedaService/AddShieldResponse")]
        void AddShield(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/RemoveShield", ReplyAction="http://tempuri.org/IAndromedaService/RemoveShieldResponse")]
        void RemoveShield(System.Guid playerGuid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/Raid", ReplyAction="http://tempuri.org/IAndromedaService/RaidResponse")]
        void Raid(System.Guid playerGuid, System.Guid transponderCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAndromedaService/BuyNewShip", ReplyAction="http://tempuri.org/IAndromedaService/BuyNewShipResponse")]
        void BuyNewShip(System.Guid playerGuid);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAndromedaServiceChannel : AndromedaScaffold.WorkerRole.AndromedaServiceReference.IAndromedaService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AndromedaServiceClient : System.ServiceModel.ClientBase<AndromedaScaffold.WorkerRole.AndromedaServiceReference.IAndromedaService>, AndromedaScaffold.WorkerRole.AndromedaServiceReference.IAndromedaService {
        
        public AndromedaServiceClient() {
        }
        
        public AndromedaServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AndromedaServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AndromedaServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AndromedaServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public AndromedaScaffold.WorkerRole.AndromedaServiceReference.Spaceship GetSpaceshipStatus(System.Guid playerGuid) {
            return base.Channel.GetSpaceshipStatus(playerGuid);
        }
        
        public AndromedaScaffold.WorkerRole.AndromedaServiceReference.Star GetCurrentStar(System.Guid playerGuid) {
            return base.Channel.GetCurrentStar(playerGuid);
        }
        
        public System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.Star> GetVisibleStars(System.Guid playerGuid) {
            return base.Channel.GetVisibleStars(playerGuid);
        }
        
        public System.Collections.Generic.List<AndromedaScaffold.WorkerRole.AndromedaServiceReference.MerchantShip> GetRaidableShips(System.Guid playerGuid) {
            return base.Channel.GetRaidableShips(playerGuid);
        }
        
        public System.Collections.Generic.List<System.Guid> GetOwnedShips(System.Guid playerGuid) {
            return base.Channel.GetOwnedShips(playerGuid);
        }
        
        public void RegisterRunLocation(System.Guid playerGuid, bool isEmulator) {
            base.Channel.RegisterRunLocation(playerGuid, isEmulator);
        }
        
        public void LaunchSpaceship(System.Guid playerGuid, System.Guid starGuid) {
            base.Channel.LaunchSpaceship(playerGuid, starGuid);
        }
        
        public void Buy(System.Guid playerGuid, string commodityName, int quantity) {
            base.Channel.Buy(playerGuid, commodityName, quantity);
        }
        
        public void BuyMaximum(System.Guid playerGuid, string commodityName) {
            base.Channel.BuyMaximum(playerGuid, commodityName);
        }
        
        public void Sell(System.Guid playerGuid, string commodityName, int quantity) {
            base.Channel.Sell(playerGuid, commodityName, quantity);
        }
        
        public void AddSensor(System.Guid playerGuid) {
            base.Channel.AddSensor(playerGuid);
        }
        
        public void RemoveSensor(System.Guid playerGuid) {
            base.Channel.RemoveSensor(playerGuid);
        }
        
        public void AddDrive(System.Guid playerGuid) {
            base.Channel.AddDrive(playerGuid);
        }
        
        public void RemoveDrive(System.Guid playerGuid) {
            base.Channel.RemoveDrive(playerGuid);
        }
        
        public void UpgradeShip(System.Guid playerGuid, int modelType) {
            base.Channel.UpgradeShip(playerGuid, modelType);
        }
        
        public void AddCannon(System.Guid playerGuid) {
            base.Channel.AddCannon(playerGuid);
        }
        
        public void RemoveCannon(System.Guid playerGuid) {
            base.Channel.RemoveCannon(playerGuid);
        }
        
        public void AddShield(System.Guid playerGuid) {
            base.Channel.AddShield(playerGuid);
        }
        
        public void RemoveShield(System.Guid playerGuid) {
            base.Channel.RemoveShield(playerGuid);
        }
        
        public void Raid(System.Guid playerGuid, System.Guid transponderCode) {
            base.Channel.Raid(playerGuid, transponderCode);
        }
        
        public void BuyNewShip(System.Guid playerGuid) {
            base.Channel.BuyNewShip(playerGuid);
        }
    }
}