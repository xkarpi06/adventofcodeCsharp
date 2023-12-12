using System.Reflection;

namespace AdventOfCode.Helpers
{

    /*
     * Created by xkarpi06 on 01.12.2023
     */
    internal class InputLoader
    {
        public AoCYear Year { get; private set; }
        public InputLoader(AoCYear year) { Year = year; }

        public List<int> LoadInts(string resource)
        {
            return LoadStrings(resource).Select(int.Parse).ToList();
        }
        public List<string> LoadStrings(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith($"{Year.GetResourcePath()}.{resource}"));

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource not found: {resource}");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd().Trim().Split(Environment.NewLine).ToList();
                }
            }
        }
    }
}
