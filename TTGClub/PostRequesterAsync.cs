using System.Text.Json;

namespace TTGClub
{
    public class Parser
    {
        static public async Task<List<string>> PostRequestAsync(params string[] url)
        {
            var requestBody = """{"page":0,"size":999999999,"search":{"value":"","exact":false},"order":[{"field":"level","direction":"asc"},{"field":"name","direction":"asc"}]}""";
            
            using var client = new HttpClient();
            var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            List<string> result = new();

            int iterator = 0;

            foreach (string i in url)
            {
                var response = await client.PostAsync(i, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                result.Add(responseBody);

                Console.WriteLine(iterator);
                iterator++;
            }

            return result;
        }

    }
}