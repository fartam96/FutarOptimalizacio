using System;

namespace Futarapp.Models
{
	[Serializable]
	public class City
	{
		public int id { get; set; } 

		public double x { get; set; } 

		public double y { get; set; } 

		public double lastDistanceMeasured { get; set; } = 0;

		public City(int id, double x, double y)
		{
			this.id = id;
			this.x = x;
			this.y = y;
		}

        public double MeasureDistance(City city)
        {
            double dist = (double)Math.Sqrt(
                Math.Pow(city.x - this.x, 2) +
                Math.Pow(city.y - this.y, 2));

            this.lastDistanceMeasured = dist;

            return dist;
        }









    }
}