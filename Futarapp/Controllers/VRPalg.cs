using Futarapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Futarapp.Controllers
{
    public class VRPalg : Controller
    {

        public List<Courier> courierList;
        public List<City> cityList;
        public TSPalg tsp;

        public VRPalg(List<Courier> courierlist, List<City> citylist, TSPalg tspmethods)
        {
            courierList = courierlist;
            cityList = citylist;
            tsp = tspmethods;
        }

        [HttpPost("VRPNN")]
        public Task<IActionResult> VrpNearest([FromBody] int cityNumber)
        {

            int diffrentOrders = Factorial(cityNumber);

            tsp.CreateCitiesAndOrder(cityNumber, 400, 400);

            return;

        }));

        private void CreateCitiesAndCourier(int citynumber)
        {
            Random rnd = new Random();
            for (int i = 0; i < citynumber; i++)
            {
                City city = new City(i, rnd.Next(0, 500), rnd.Next(0, 500));
                cityList.Add(city);
                order.Add(i);

            }

        }
    }

}


