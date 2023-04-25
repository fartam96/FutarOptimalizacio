using Futarapp.Models;

namespace Futarapp.Helpers
{
    public class CsvReader
    {
        public static List<City> ParseCitiesFromFile(string filePath)
        {
            List<City> cities = new List<City>();
            string line = null;

            if (File.Exists(filePath))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(filePath);
                    while ((line = file.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        string[] vals = line.Split(' ');
                        cities.Add(new City(Int32.Parse(vals[0]), Double.Parse(vals[1]), Double.Parse(vals[2])));
                    }
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }

            return cities;
        }
    }
}
