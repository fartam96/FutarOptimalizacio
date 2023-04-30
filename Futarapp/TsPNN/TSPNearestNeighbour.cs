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

        public void TSPNN()
        {

            totalDistanceRoute = 0;
            totalDistanceRoute2 = 0;

            citiesResult = new List<City>();

            City firstCity = cities.Where(c => c.id == 1).FirstOrDefault();

            City currentCity = firstCity;

            AddCityToRoutesList(cities, citiesResult, currentCity);

            while (cities.Count > 0)
            {

                currentCity = FindClosestCity(cities, currentCity);

                totalDistanceRoute += currentCity.lastDistanceMeasured;


                AddCityToRoutesList(cities, citiesResult, currentCity);
            }

        }

        private void AddCityToRoutesList(List<City> cities, List<City> citiesResult, City currentCity)
        {
            citiesResult.Add(currentCity);
            cities.Remove(currentCity);
        }

        private City FindClosestCity(List<City> cities, City currentCity)
        {

            City nearestCity;

            if (cities.Count == 1)
            {
                nearestCity = cities[0];
                nearestCity.MeasureDistance(currentCity);

            }
            else
            {
                nearestCity =
                    cities.Aggregate((c1, c2) => c1.MeasureDistance(currentCity) < c2.MeasureDistance(currentCity) ? c1 : c2);

                totalDistanceRoute2 += currentCity.MeasureDistance(nearestCity);

            }

            return nearestCity;
        }

    }
}
