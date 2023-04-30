using Futarapp.Context;
using Futarapp.Helpers;
using Futarapp.Models;
using Futarapp.TsPNN;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Futarapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TSPalg : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration _configuration;
        private List<City> cities = new List<City>();
        private List<int> order = new List<int>();
        private double recordDistance = Int32.MaxValue;
        private List<int> bestEver = new List<int>();
        private List<List<int>> allOrder = new List<List<int>>();
        private readonly IWebHostEnvironment _env;
        private static string TSPtest = "";




        public TSPalg(AppDbContext appDbContext, IConfiguration configuration, IWebHostEnvironment env)
        {
            this.appDbContext = appDbContext;
            _configuration = configuration;
            _env = env;

            string contentRootPath = _env.ContentRootPath;

            TSPtest = Path.Combine(contentRootPath, "TspData", "test2tsp.txt");

        }

        private void CreateCitiesAndOrder(int citynumber, int mapSizeX, int mapSizeY)
        {
            Random rnd = new Random();
            for (int i = 0; i < citynumber; i++)
            {
                City city = new City(i, rnd.Next(0, mapSizeX), rnd.Next(0, mapSizeY));
                cities.Add(city);
                order.Add(i);

            }

            var d = Distance();
            recordDistance = d;
            bestEver = new List<int>(order);

        }

        private List<int> swap(List<int> order, int swapIndex1, int swapIndex2)
        {
            if (swapIndex1 < order.Count && swapIndex2 < order.Count && swapIndex1 >= 0 && swapIndex2 >= 0)
            {
                int tempIndex = order[swapIndex1];
                order[swapIndex1] = order[swapIndex2];
                order[swapIndex2] = tempIndex;

            }

            return order;
        }

        private double MeasureDistance(double x1, double x2, double y1, double y2)
        {
            var distance = Math.Sqrt((Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));

            return distance;
        }

        private double Distance()
        {
            double sum = 0;
            for (var i = 0; i < order.Count - 1; i++)
            {
                var cityAIndex = order[i];
                var cityA = cities[cityAIndex];
                var cityBIndex = order[i + 1];
                var cityB = cities[cityBIndex];
                var d = MeasureDistance(cityA.x, cityA.y, cityB.x, cityB.y);
                sum += d;
            }
            return sum;
        }

        private int Factorial(int f)
        {
            if (f == 0)
                return 1;
            else
                return f * Factorial(f - 1);
        }

        [HttpGet("nearestNeighbor")]
        public IActionResult FindRouteWithNN()
        {
            List<City> cities = CsvReader.ParseCitiesFromFile(TSPtest);

            //CreateCitiesAndOrder(8, 400, 400);

            var timer = new Stopwatch();
            timer.Start();

            TSPNearestNeighbour nn = new TSPNearestNeighbour(cities);
            nn.TSPNN();

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;


            return Ok(new
            {
                distance = nn.GetTotalDistanceRoute(),
                cityOrder = nn.GetCitiesResultToString(),
                time = timeTaken
            });
        }


        [HttpPost("TSPBruteForce")]
        public async Task<IActionResult> BruteForce([FromBody] int cityNumber)
        {

            int diffrentOrders = Factorial(cityNumber);

            CreateCitiesAndOrder(cityNumber, 400, 400);

            List<int> copy = new List<int>(order);

            allOrder.Add(copy);

            var timer = new Stopwatch();
            timer.Start();

            var d = Distance();
            if (d < recordDistance)
            {
                recordDistance = d;
                bestEver = new List<int>(order);
            }

            for (int i = 0; i < ((diffrentOrders / 2) - 1); i++)
            {
                nextOrder();
                if (order == null)
                {
                    break;
                }
                var k = Distance();
                if (k < recordDistance)
                {
                    recordDistance = d;
                    bestEver = new List<int>(order);
                }
            }

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;

            return Ok(new
            {
                status = 200,
                bestEver = bestEver,
                Recorddist = recordDistance,
                Cities = cities,
                time = timeTaken
            });
        }

        private void nextOrder()
        {

            // STEP 1 of the algorithm
            // https://www.quora.com/How-would-you-explain-an-algorithm-that-generates-permutations-using-lexicographic-ordering
            var largestI = -1;
            for (var i = 0; i < order.Count - 1; i++)
            {
                if (order[i] < order[i + 1])
                {
                    largestI = i;
                }
            }
            if (largestI == -1)
            {
                return;
            }

            // STEP 2
            var largestJ = -1;
            for (var j = 0; j < order.Count; j++)
            {
                if (order[largestI] < order[j])
                {
                    largestJ = j;
                }
            }

            // STEP 3
            swap(order, largestI, largestJ);

            // STEP 4: reverse from largestI + 1 to the end
            order.Reverse(largestI + 1, order.Count - (largestI + 1));

            List<int> copy1 = new List<int>(order);
            allOrder.Add(copy1);
        }






    }
}
