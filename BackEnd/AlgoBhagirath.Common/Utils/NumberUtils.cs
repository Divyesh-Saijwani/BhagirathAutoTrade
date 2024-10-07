namespace AlgoBhagirath.Common.Utils
{
    public static class NumberUtils
    {
        /// <summary>
        /// Generates a sequence of values around the next rounded base value.
        /// </summary>
        /// <param name="initialValue">The initial value to be rounded up and used as the base for generating the sequence.</param>
        /// <param name="step">The step size between consecutive values (default is 100).</param>
        /// <param name="count">The number of values to generate (default is 10).</param>
        /// <returns>A list of generated values around the next rounded base value.</returns>
        public static List<int> GenerateValuesAroundBase(int initialValue, int step = 100, int count = 10)
        {
            var result = new List<int>();

            // Round up to the next multiple of step
            int baseValue = (int)Math.Ceiling((double)initialValue / step) * step;

            // Find the starting point
            int startValue = baseValue - (count / 2) * step;

            // Adjust the startValue to ensure it is a multiple of step
            startValue = startValue - (startValue % step);

            // Generate values around the baseValue
            for (int i = 0; i < count; i++)
            {
                int value = startValue + i * step;
                result.Add(value);
            }

            return result;
        }
    }
}
