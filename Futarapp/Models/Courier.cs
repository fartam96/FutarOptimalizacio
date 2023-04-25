namespace Futarapp.Models
{
    public class Courier
    {

        public int Id { get; set; }
        public double avgSpeed { get; set; }
        public List<City> route { get; set; }
        public double cost { get; set; }
        public double nextCityCost { get; set; }
    }
}
