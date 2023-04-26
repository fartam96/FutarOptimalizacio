using Futarapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Futarapp.Controllers
{
    public class VRPalg : Controller
    {

        public List<Courier> courierList = new List<Courier>();
        public SortedList<double,City> cityList = new SortedList<double, City>();
        public List<City> cityToRand = new List<City>();    
        public TSPalg tsp;


        public VRPalg()
        {
            
        }

        [HttpPost("VRPNN")]
        public IActionResult VrpNearest(int cityNumber, int courierNumber)
        {

            CreateCitiesAndCourier(cityNumber, courierNumber);


            foreach (var city in cityList)
            {
                int courierIndex = -1;
                int routeIndex = -1;

                double nextCityDist = Int32.MaxValue;

                

                for (int j = 0; j < courierList.Count; j++)
                {
                    var cour = nextCityCost(city.Value, courierList[j]);

                    if(cour.nextCityCost< nextCityDist)
                    {
                        nextCityDist = cour.nextCityCost;
                        routeIndex = cour.insertIndex;
                        courierIndex = j;
                    }
                }
                if (routeIndex >= courierList[courierIndex].route.Count)
                {
                    courierList[courierIndex].route.Add(city.Value);
                }

                else
                {
                    courierList[courierIndex].route.Insert(routeIndex, city.Value);
                }

                courierList[courierIndex].cost = nextCityDist;
                foreach(var cour in courierList) 
                {
                    cour.nextCityCost = 0;
                    cour.insertIndex = 0;

                }

            }

            return Ok(new
            {
                cities = cityList,
                couriers = courierList
            });

        }

        public void GenerateRandomLoop(List<City> listToShuffle)
        {
            var _rand = new Random();

            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = _rand.Next(i + 1);
                var value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[i];
                listToShuffle[i] = value;
            }
           
        }

        [HttpPost("VRPNNRandom")]
        public IActionResult VRPNNRandom(int cityNumber, int courierNumber)
        {

            CreateCitiesAndCourier(cityNumber, courierNumber);

            int counter = 0;


            while (cityToRand.Any()) {
                counter++;

                GenerateRandomLoop(cityToRand);

                var firstCity = cityToRand.First();


                int courierIndex = -1;
                int routeIndex = -1;

                double nextCityDist = Int32.MaxValue;



                for (int j = 0; j < courierList.Count; j++)
                {
                    var cour = nextCityCost(firstCity, courierList[j]);

                    if (cour.nextCityCost < nextCityDist)
                    {
                        nextCityDist = cour.nextCityCost;
                        routeIndex = cour.insertIndex;
                        courierIndex = j;
                    }
                }
                if (routeIndex >= courierList[courierIndex].route.Count)
                {
                    courierList[courierIndex].route.Add(firstCity);
                }

                else
                {
                    courierList[courierIndex].route.Insert(routeIndex, firstCity);
                }

                courierList[courierIndex].cost = nextCityDist;
                foreach (var cour in courierList)
                {
                    cour.nextCityCost = 0;
                    cour.insertIndex = 0;

                }

                cityToRand.RemoveAt(0);

            }

            return Ok(new
            {
                cityList = cityList,
                counter = counter,
                cities = cityList,
                couriers = courierList
            });

        }

        private double MeasureDistance(double x1, double x2, double y1, double y2)
        {
            var distance = Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));

            return distance;
        }

        private void CreateCitiesAndCourier(int citynumber, int courierNumber)
        {
            Random rnd = new Random();
            for (int i = 0; i < citynumber; i++)
            {
                City city = new City(i, rnd.Next(0, 500), rnd.Next(0, 500));
                var dist = MeasureDistance(0, city.x, 0, city.y);
                cityList.Add(dist, city);
                cityToRand.Add(city);

            }

            for (int i = 0; i < courierNumber; i++)
            {
                List<City> route = new List<City>();
                City depo = new City((citynumber + i), 0, 0);
                route.Add(depo);
                Courier cour = new Courier(i, rnd.Next(15, 30), route,0,0,-1);
                courierList.Add(cour);

            }

        }

        private Courier nextCityCost(City city, Courier courier)
        {
            int index = 0;
            double mindistance = Int32.MaxValue;

            for (int i = 0;i < courier.route.Count;i++)
            {
                var dist = MeasureDistance(city.x, courier.route[i].x, city.y, courier.route[i].y);
                if (dist < mindistance)
                {
                    mindistance = dist;
                    index = i;
                }

            }

            List<City> insertBefor = new List<City>(courier.route);
            insertBefor.Insert(index, city);
            List<City> insertAfter = new List<City>(courier.route);
            insertAfter.Insert(index+1,city);
            

            double beforDist = Distance(insertBefor);
            double afterDist = Distance(insertAfter);

            double minDist = Math.Min(afterDist, beforDist);

            Courier courier1 = courier;
            courier1.nextCityCost = (minDist - courier1.cost);

            if(afterDist <= beforDist)
            {
                courier1.insertIndex = (index+1);
            }
            else
            {
                courier1.insertIndex =index;
            }

            return courier1;

        }

        private double Distance(List<City> cities)
        {
            if (cities.Count < 2)
            {
                return 0;
            }
            double sum = 0;
            for (var i = 0; i < cities.Count - 1; i++)
            {
                var cityA = cities[i];
                var cityB = cities[i+1];
                var d = MeasureDistance(cityA.x, cityB.x, cityA.y, cityB.y);
                sum += d;
            }
            return sum;
        }

    }

}


