namespace Futarapp.Models
{
    public class Params
    {
        public List<City> city { get; set; }
        public List<int> order { get; set; }

        public Params(List<City> city, List<int> order)
        {
            this.city = city;
            this.order = order;
        }
    }
}
