using Futarapp.Models;

namespace Futarapp.TsPNN
{
    public class TSPNearestNeighbour
    {

        private List<City> citiesResult = null;
        private double totalDistanceRoute = 0;
        private double totalDistanceRoute2 = 0;
        private List<City> cities = null;

        public TSPNearestNeighbour(List<City> cities)
        {
            this.cities = cities;
        }

        public double GetTotalDistanceRoute()
        {
            return totalDistanceRoute;
        }

        public List<City> GetCitiesResult()
        {
            return citiesResult;
        }

        public string GetCitiesResultToString()
        {
            string sequence = "";

            foreach (City s in citiesResult)
            {
                sequence += s.id + ", ";
            }

            return sequence;
        }



        /**
         * Main execution method.
         */
        public void TSPNN()
        {

            totalDistanceRoute = 0;
            totalDistanceRoute2 = 0;

            // Create an array which will hold he cities route
            citiesResult = new List<City>();

            // Get city with id 1 where to start from.
            City firstCity = cities.Where(c => c.id == 1).FirstOrDefault();

            City currentCity = firstCity;

            // Add the city to route list
            AddCityToRoutesList(cities, citiesResult, currentCity);

            while (cities.Count > 0)
            {

                // Get closes city
                currentCity = FindClosestCity(cities, currentCity);

                // Add to the total distance travelled.
                totalDistanceRoute += currentCity.lastDistanceMeasured;


                // Add the city to route list
                AddCityToRoutesList(cities, citiesResult, currentCity);
            }

        }

        private void AddCityToRoutesList(List<City> cities, List<City> citiesResult, City currentCity)
        {
            // Add viry to route
            citiesResult.Add(currentCity);
            // Remove from remainign cities to visit list.
            cities.Remove(currentCity);
        }

        private City FindClosestCity(List<City> cities, City currentCity)
        {

            City nearestCity;

            // If only 1 remains, measure to the current city and return
            if (cities.Count == 1)
            {
                nearestCity = cities[0];
                nearestCity.MeasureDistance(currentCity);

            }
            else
            {
                // Find the closest city by comparing them all with 
                // their respective position of the current city.
                nearestCity =
                    cities.Aggregate((c1, c2) => c1.MeasureDistance(currentCity) < c2.MeasureDistance(currentCity) ? c1 : c2);

                totalDistanceRoute2 += currentCity.MeasureDistance(nearestCity);

            }

            return nearestCity;
        }

    }
}
