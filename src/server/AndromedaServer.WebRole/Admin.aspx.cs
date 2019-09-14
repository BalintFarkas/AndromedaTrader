using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Andromeda;
using System.IO;
using Troschuetz.Random;
using Andromeda.Data;
using Andromeda.ServerEntities;
using Andromeda.Common;

namespace Andromeda.WebPages
{
    public partial class Admin : System.Web.UI.Page
    {
        private AndromedaDataContext db = new AndromedaDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("Administrator"))
            {
                Response.Write("Access denied.");
                Response.End();
                return;
            }
        }

        #region Starfield Generation
        protected void GenerateStarfieldButton_Click(object sender, EventArgs e)
        {
            //Purge the data tables
            db.ExecuteCommand("DELETE FROM CommodityAtStars");
            db.ExecuteCommand("DELETE FROM CommodityInHolds");
            db.ExecuteCommand("DELETE FROM Commodities");
            db.ExecuteCommand("DELETE FROM Stars");

            //Generate new stars
            db.Stars.InsertAllOnSubmit(
                    GenerateStarfield(
                        int.Parse(StarfieldWidthTextBox.Text),
                        int.Parse(StarfieldHeightTextBox.Text),
                        int.Parse(StarClusterCountTextBox.Text),
                        int.Parse(StarClusterRadiusTextBox.Text),
                        int.Parse(ClusteredStarCountTextBox.Text),
                        int.Parse(VoidStarCountTextBox.Text),
                        int.Parse(MinimumDistanceTextBox.Text)
                    )
                );
            db.SubmitChanges();

            //Feedback
            FeedbackLabel.Text = "Starfield regenerated.";
            FeedbackLabel.Visible = true;
        }

        /// <summary>
        /// Generates random planet positions.
        /// </summary>
        private List<Star> GenerateStarfield(int width, int height, int clusterCount, int clusterRadius, int clusteredStarCount, int voidStarCount, int minDistance)
        {
            //Create variables
            Random rnd = new Random((int)DateTime.Now.Ticks);
            List<Cluster> clusters = new List<Cluster>();
            List<Star> stars = new List<Star>();

            //Create clusters
            for (int i = 0; i < clusterCount; i++)
            {
                while (true)
                {
                    //Generate candidates
                    double candidateX = rnd.NextDouble() * width;
                    double candidateY = rnd.NextDouble() * height;

                    //Validate candidate coordinates - if they fail validation, we'll stay in the loop and iterate once more
                    bool failedValidation = false;

                    //Rule 1 - Clusters may not overlap with the map edge
                    if (candidateX - clusterRadius < 0 || candidateX + clusterRadius > width ||
                        candidateY - clusterRadius < 0 || candidateY + clusterRadius > height)
                    {
                        failedValidation = true;
                    }
                    if (failedValidation) continue;

                    //Rule 2 - Clusters may not overlap with each other
                    foreach (Cluster cluster in clusters)
                    {
                        if (DistanceCalculator.GetDistance(candidateX, candidateY, cluster.X, cluster.Y) < 2 * clusterRadius)
                        {
                            failedValidation = true;
                        }
                    }
                    if (failedValidation) continue;

                    //The cluster passed
                    clusters.Add(new Cluster() { X = candidateX, Y = candidateY });
                    break;
                }
            }

            //Create stars within clusters
            for (int i = 0; i < clusteredStarCount; i++)
            {
                while (true)
                {
                    //Select a cluster
                    Cluster selectedCluster = clusters[rnd.Next(0, clusters.Count)];

                    //Generate a position within the radius of the cluster
                    //Get polar coordinates (to express being in the cluster's radius
                    //these coordinates are interpreted as being relative to the cluster)
                    double distance = rnd.NextDouble() * clusterRadius;
                    double angle = rnd.NextDouble() * Math.PI * 2;
                    //Convert polar coordinates to cartesian coordinates
                    double candidateX = distance * Math.Cos(angle);
                    double candidateY = distance * Math.Sin(angle);
                    //Add cluster's coordinates to make these absolute coordinates
                    candidateX += selectedCluster.X;
                    candidateY += selectedCluster.Y;

                    //Validate candidate coordinates - if they fail validation, we'll stay in the loop and iterate once more
                    bool failedValidation = false;

                    //Rule 1 - Stars may not be closer to each other than the specified minimum distance.
                    foreach (Star star in stars)
                    {
                        if (DistanceCalculator.GetDistance(candidateX, candidateY, star.X ?? 0, star.Y ?? 0) < minDistance)
                        {
                            failedValidation = true;
                        }
                    }
                    if (failedValidation) continue;

                    //The star passed
                    stars.Add(new Star()
                    {
                        Name = StarNameGenerator.GetStarName(),
                        Guid = Guid.NewGuid(),
                        X = candidateX,
                        Y = candidateY
                    });
                    break;
                }
            }

            //Create stars in the void between clusters
            for (int i = 0; i < voidStarCount; i++)
            {
                while (true)
                {
                    //Get candidate coordinates
                    double candidateX = rnd.NextDouble() * width;
                    double candidateY = rnd.NextDouble() * height;

                    //Validate candidate coordinates - if they fail validation, we'll stay in the loop and iterate once more
                    bool failedValidation = false;

                    //Rule 1 - Stars may not be closer to each other than the specified minimum distance.
                    foreach (Star star in stars)
                    {
                        if (DistanceCalculator.GetDistance(candidateX, candidateY, star.X ?? 0, star.Y ?? 0) < minDistance)
                        {
                            failedValidation = true;
                        }
                    }
                    if (failedValidation) continue;

                    //The star passed
                    stars.Add(new Star()
                    {
                        Name = StarNameGenerator.GetStarName(),
                        Guid = Guid.NewGuid(),
                        X = candidateX,
                        Y = candidateY
                    });
                    break;
                }
            }

            //Done
            return stars;
        }

        private class Cluster
        {
            public double X { get; set; }
            public double Y { get; set; }
        }
        #endregion

        #region Commodity Generation
        protected void GenerateCommoditiesButton_Click(object sender, EventArgs e)
        {
            //Purge the data tables
            db.ExecuteCommand("DELETE FROM CommodityAtStars");
            db.ExecuteCommand("DELETE FROM CommodityInHolds");
            db.ExecuteCommand("DELETE FROM Commodities");

            //Generate new commodities and per-star commodity information
            var result = GenerateCommodities(CommoditiesInputTextBox.Text);
            db.Commodities.InsertAllOnSubmit(result.Item1);
            db.CommodityAtStars.InsertAllOnSubmit(result.Item2);
            db.SubmitChanges();

            //Feedback
            FeedbackLabel.Text = "Commodities regenerated.";
            FeedbackLabel.Visible = true;
        }

        /// <summary>
        /// Generates commodities and commodity infos based on the input by parsing the input into temporary holding
        /// variables, acting on the parameters, then throwing the temporary variables away.
        /// </summary>
        private Tuple<List<Commodity>, List<CommodityAtStar>> GenerateCommodities(string input)
        {
            List<Commodity> commodities = new List<Commodity>();
            List<CommodityAtStar> commodityAtStars = new List<CommodityAtStar>();

            //Read all lines from user input and create the commodities and commodity generation infos one-by-one
            //These CommodityGenerationInfo local classes hold the generation info inputted by the user
            List<CommodityGenerationInfo> commodityGenerationInfos = new List<CommodityGenerationInfo>();
            StringReader reader = new StringReader(input);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] split = line.Split('|');

                Commodity commodity = new Commodity() { Name = split[0] };
                commodities.Add(commodity);

                commodityGenerationInfos.Add(new CommodityGenerationInfo()
                {
                    ReferencedCommodity = commodity,
                    PriceMean = int.Parse(split[1]),
                    PriceDeviation = int.Parse(split[2]),
                    ChanceOfManufacture = int.Parse(split[3]),
                    ProductionRateMean = int.Parse(split[4]),
                    ProductionRateDeviation = int.Parse(split[5]),
                    MaxCapacityMean = int.Parse(split[6]),
                    MaxCapacityDeviation = int.Parse(split[7])
                });
            }

            //Perform generation of commodity infos for each star
            List<Star> stars = db.Stars.ToList();
            //The generator object we'll use both in "plain" mode and wrapped in a normal distribution.
            //This is not a built-in generator, but from an external library that also provides the normal distribution.
            Generator generator = new StandardGenerator((int)DateTime.Now.Ticks);

            foreach (var star in stars)
            {
                foreach (var commodityGenerationInfo in commodityGenerationInfos)
                {
                    //Create object
                    CommodityAtStar commodityAtStar = new CommodityAtStar();
                    commodityAtStar.Commodity = commodityGenerationInfo.ReferencedCommodity;
                    commodityAtStar.Star = star;

                    //Set up generator with normal distribution
                    NormalDistribution normalDistribution = new NormalDistribution(generator);

                    //Create prices
                    normalDistribution.Mu = (double)commodityGenerationInfo.PriceMean;
                    normalDistribution.Sigma = (double)commodityGenerationInfo.PriceDeviation;
                    commodityAtStar.BuyPrice = MakeZeroIfNegative((int)Math.Round(normalDistribution.NextDouble()));
                    commodityAtStar.SellPrice = commodityAtStar.BuyPrice;

                    //Roll whether the planet produces the commodity at all
                    if (generator.NextDouble() <= (double)commodityGenerationInfo.ChanceOfManufacture / 100)
                    {
                        //Create production rate
                        normalDistribution.Mu = (double)commodityGenerationInfo.ProductionRateMean;
                        normalDistribution.Sigma = (double)commodityGenerationInfo.ProductionRateDeviation;
                        commodityAtStar.ProductionRate = MakeZeroIfNegative((int)Math.Round(normalDistribution.NextDouble()));

                        //Create max capacity
                        normalDistribution.Mu = (double)commodityGenerationInfo.MaxCapacityMean;
                        normalDistribution.Sigma = (double)commodityGenerationInfo.MaxCapacityDeviation;
                        commodityAtStar.MaxCapacity = MakeZeroIfNegative((int)Math.Round(normalDistribution.NextDouble()));
                    }
                    else
                    {
                        commodityAtStar.ProductionRate = 0;
                        commodityAtStar.MaxCapacity = 0;
                    }

                    //Finish the object instance
                    commodityAtStar.Stock = commodityAtStar.MaxCapacity;
                    commodityAtStar.IsSellable = true;

                    //Save the instance
                    commodityAtStars.Add(commodityAtStar);
                }
            }

            //Done
            return new Tuple<List<Commodity>, List<CommodityAtStar>>(commodities, commodityAtStars);
        }

        private int MakeZeroIfNegative(int input)
        {
            return input < 0 ? 0 : input;
        }

        /// <summary>
        /// Holds generation parameters for a commodity for the duration of the generation run.
        /// </summary>
        private class CommodityGenerationInfo
        {
            public Commodity ReferencedCommodity { get; set; }
            public int PriceMean { get; set; }
            public int PriceDeviation { get; set; }
            public int ChanceOfManufacture { get; set; }
            public int ProductionRateMean { get; set; }
            public int ProductionRateDeviation { get; set; }
            public int MaxCapacityMean { get; set; }
            public int MaxCapacityDeviation { get; set; }
        }
        #endregion
    }
}