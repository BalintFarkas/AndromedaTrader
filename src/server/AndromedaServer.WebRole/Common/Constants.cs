using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Constants
/// </summary>
public static class Constants
{
    public const int BaseSpeed = 50;
    public const int BaseRange = 50;
    public const int DriveCostInCapacity = 20;
    public const int SensorCostInCapacity = 20;
    public const int DriveBonusInSpeed = 20;
    public const int SensorBonusInRange = 20;
    public const int CannonCostInCapacity = 20;
    public const int ShieldCostInCapacity = 20;
    public const int ModificationLimit = 1;
    public static TimeSpan RaidGracePeriod = TimeSpan.FromMinutes(10);
    public static TimeSpan CreationGracePeriod = TimeSpan.FromDays(1);
    public const int MaxShipCount = 5;

    public static List<ModelType> ModelTypes = new List<ModelType>()
    {
        new ModelType()
        {
            Name="Cobra",
            Capacity=100,
            Cost=0
        },
        new ModelType()
        {
            Name="Nebula",
            Capacity=200,
            Cost=1000000
        },
        new ModelType()
        {
            Name="Aquila",
            Capacity=300,
            Cost=10000000
        }
    };

    public static List<int> NewShipCosts = new List<int>()
    {
        10*1000000,
        50*1000000,
        150*1000000,
        400*1000000
    };

    public class ModelType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Cost { get; set; }
    }
}