using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // I'm using fiddler as web debugger, you can set UseProxy to false to bypass it
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.UseProxy = true;
                httpClientHandler.Proxy = new WebProxy("http://localhost:8888", false);

                HttpClient _client = new HttpClient(httpClientHandler);
                _client.DefaultRequestHeaders.Clear();

                // host is suffixed with .fiddler to make fiddler catch the request, remove .fiddler if you don't use it
                await RefreshAsync(_client, "localhost.fiddler");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }

        private static async Task RefreshAsync(HttpClient _client, string host)
        {
            var client_id = "100";
            var client_secret = "0000";
            var username = "ganagus";
            var password = "12345";

            // Get tokens
            var asUrl = $"http://{host}:5001/api/token/auth?grant_type=password&client_id={client_id}&client_secret={client_secret}&username={username}&password={password}";
            Console.WriteLine("Begin authenticating:");
            HttpResponseMessage asMsg = await _client.GetAsync(asUrl);
            string result = await asMsg.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<ResponseData>(result);
            if (responseData.Code != "999")
            {
                Console.WriteLine("Authentication fail");
                return;
            }

            var token = JsonConvert.DeserializeObject<Token>(responseData.Data);
            Console.WriteLine("Authentication success");
            Console.WriteLine($"Authentication result: {result}");

            // Use token to access resource server
            Console.WriteLine("Begin to request the resouce server..");
            var rsUrl = $"http://{host}:5002/api/values/1";
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
            HttpResponseMessage rsMsg = await _client.GetAsync(rsUrl);
            Console.WriteLine("Result of requesting the resouce server:");
            Console.WriteLine(rsMsg.StatusCode);
            Console.WriteLine(await rsMsg.Content.ReadAsStringAsync());

            // Make token expired
            Console.WriteLine("Sleep 2 minutes to make the token expire");
            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(2));

            // Request resource server again with expired token
            Console.WriteLine("Begin to request the resouce server after token expire..");
            rsMsg = await _client.GetAsync(rsUrl);
            Console.WriteLine("Result of requesting the resouce server:");
            Console.WriteLine(rsMsg.StatusCode);
            Console.WriteLine(await rsMsg.Content.ReadAsStringAsync());

            if (rsMsg.StatusCode != HttpStatusCode.Unauthorized)
                throw new Exception("The request should fail with status 401");

            // refresh the token
            Console.WriteLine("Begin to refresh token");

            _client.DefaultRequestHeaders.Clear();
            var refresh_token = token.refresh_token;
            asUrl = $"http://{host}:5001/api/token/auth?grant_type=refresh_token&client_id={client_id}&client_secret={client_secret}&refresh_token={refresh_token}";
            HttpResponseMessage asMsgNew = await _client.GetAsync(asUrl);
            string resultNew = await asMsgNew.Content.ReadAsStringAsync();

            var responseDataNew = JsonConvert.DeserializeObject<ResponseData>(resultNew);
            if (responseDataNew.Code != "999")
            {
                Console.WriteLine("Refresh token fail");
                return;
            }

            Token tokenNew = JsonConvert.DeserializeObject<Token>(responseDataNew.Data);
            Console.WriteLine("Refresh token success");
            Console.WriteLine(asMsg.StatusCode);
            Console.WriteLine($"Response of refresh token: {resultNew}");

            // Use new tokens to access resource server
            Console.WriteLine("Request resource server again..");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenNew.access_token);
            HttpResponseMessage rsMsgNew = await _client.GetAsync($"http://{host}:5002/api/values/1");

            Console.WriteLine("Resource server response:");
            Console.WriteLine(rsMsgNew.StatusCode);
            Console.WriteLine(await rsMsgNew.Content.ReadAsStringAsync());
        }

    }

    public class Token
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public long expires_in { get; set; }
    }

    public class ResponseData
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

}
