namespace Futarapp.Models
{
    public class Courier
    {

        public int Id { get; set; }
        public double avgSpeed { get; set; }
        public List<City> route { get; set; }
        public double cost { get; set; }
        public double nextCityCost { get; set; }
        public int insertIndex { get; set; }

        public Courier(int id, double speed, List<City> routes, double dist, double nextCost, int insert)
        {
            Id = id;
            avgSpeed = speed;
            route = routes;
            cost = dist;
            nextCityCost = nextCost;
            insertIndex = insert;
        }
    }
}
