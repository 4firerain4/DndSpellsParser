using System.Text.Json;

namespace TTGClub
{
    internal static class Parser
    {
        public static async Task<List<string>> PostRequestAsync(params string[] url)
        {
            var requestBody = """{"page":0,"size":999999999,"search":{"value":"","exact":false},"order":[{"field":"level","direction":"asc"},{"field":"name","direction":"asc"}]}""";
            
            using var client = new HttpClient();
            using var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            List<string> result = new();

            int iterator = 0;

            foreach (string i in url)
            {
                using var response = await client.PostAsync(i, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                result.Add(responseBody);

                Console.WriteLine(iterator);
                iterator++;
            }

            return result;
        }

    }
}