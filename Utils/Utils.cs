namespace DronesDeliveryService.Utils
{
    public static class Utils
    {
        public static Dictionary<string, int> GetDronesAndWeights(string line)
        {
            Dictionary<string, int> dronesAndWeights = new();
            int i = 0;
            
            // We make groups of 2 elements, [Drone name, Drone max weight]
            var query = from s in line.Split(",")
                        let num = i++
                        group s by num / 2 into g
                        select g.ToArray();

            // Then we parse the information into a dictionary
            foreach (var item in query.ToList())
            {
                dronesAndWeights.Add(item[0].Trim(), int.Parse(item[1].Trim(' ', '[', ']')));
            }

            return dronesAndWeights;
        }

        public static (string key, int value) GetLocations(string line)
        {
            string locationName = "";
            int locationWeight = 0;

            var location = line.Split(',');
            if (location.Length > 0)
            {
                locationName = location[0].Trim();
                locationWeight = int.Parse(location[1].Trim(' ', '[', ']'));
            }

            return (key: locationName, value: locationWeight);
        }

        /// <summary>
        /// Gets the all the possible routes for a drone to make on each trip to deliver the packages
        /// based on its max weight capacity and sorted in ascending order by list length.
        /// </summary>
        /// <param name="target">Drone's max weight capacity</param>
        /// <param name="numbers">List of weights a drone can ship at each location</param>
        /// <returns>Routes sorted in ascending order by list length</returns>
        public static List<List<int>> GetBestRoutes(int target, int[] numbers)
        {
            bool[] wheel = new bool[numbers.Length];
            int? sum = 0;
            List<List<int>> routes = new();

            do
            {
                sum = IncrementWheel(0, sum, numbers, wheel);
                //Use subtraction comparison due to double type imprecision
                if (sum.HasValue && Math.Abs(sum.Value - target) < 0.000001F)
                {
                    //Found a subset. Save the result.
                    var bestRoute = numbers.Where((n, idx) => wheel[idx]).ToList();
                    bestRoute.Sort();

                    if (!routes.Any(r => r.SequenceEqual(bestRoute)))
                    {
                        routes.Add(bestRoute);
                    }
                }
            } while (sum != null);

            routes.Sort((a, b) => { return a.Count.CompareTo(b.Count); });
            return routes;
        }

        /// <summary>
        /// Gets the list of trips a drone can make based on the best route possible.
        /// </summary>
        /// <param name="locations">Locations</param>
        /// <param name="weights">List of weights of the best trip</param>
        /// <returns>The best trip possible based on the best route possible determined by its weights</returns>
        public static List<string> GetDronesRoutes(Dictionary<string, int> locations, List<int> weights)
        {
            List<string> locationRoutes = new();
            Dictionary<string, int> locationsCopy = new(locations);

            foreach (var weight in weights) 
            {
                var locationName = locationsCopy.Where(x => x.Value == weight).FirstOrDefault();
                if (locationName.Key != null && !locationRoutes.Contains(locationName.Key))
                {
                    locationRoutes.Add(locationName.Key);
                    locationsCopy.Remove(locationName.Key);
                }
            }

            return locationRoutes;
        }

        private static int? IncrementWheel(int position, int? sum, int[] numbers, bool[] wheel)
        {
            if (position == numbers.Length || !sum.HasValue)
            {
                return null;
            }

            wheel[position] = !wheel[position];

            if (!wheel[position])
            {
                sum -= numbers[position];
                sum = IncrementWheel(position + 1, sum, numbers, wheel);
            }
            else
            {
                sum += numbers[position];
            }
            return sum;
        }
    }
}
