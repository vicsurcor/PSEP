using System.Net.Http;

using System.Threading.Tasks;

namespace HttpClientAndUri

{

    class Program

    {

           // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.

            static readonly HttpClient client = new HttpClient();

            static async Task Main()

            {

                // Call asynchronous network methods in a try/catch block to handle exceptions.

                try

                {

                    HttpResponseMessage response = await client.GetAsync("http://www.c.com/");

                    //response.EnsureSuccessStatusCode();
                    
                    if(response.IsSuccessStatusCode) {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Above three lines can be replaced with new helper method below

                        // string responseBody = await client.GetStringAsync(uri);

                        Console.WriteLine("\n" + response.StatusCode);
                        
                    }
                }

                catch (HttpRequestException e)

                {

                    Console.WriteLine("\nException Caught!");

                    Console.WriteLine("Message: {0} Status Code: {1} HResult: {2} RequestError: {3} Data: {4}", e.Message, e.StatusCode, e.HResult, e.HttpRequestError, e.Data);

                }

            }

        

    }

}