namespace Futarapp.Models
{
    public class CityandCour
    {
        public Courier courier;
        public List<City> cities;

        public CityandCour(Courier courier, List<City> cities)
        {
            this.courier = courier;
            this.cities = cities;
        }
    }
}
