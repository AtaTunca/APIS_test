using ConsoleApp2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {

            Token token = new Token();
            
            try
            {
                token = GetElibilityToken().Result;
                SendFlightData(token.AccessToken);

            }
            catch (Exception ex)
            {

                throw;
            }

            Console.Read();

        }

        private static string CreateJsonBody()
        {
            try
            {
                CreatingFlightJson jsonBody = new CreatingFlightJson();

                jsonBody.idExternal = "PGT-123-456-202218071304";

                jsonBody.flight.Designator = new FlightDesignator() {
                    CompanyCode = new IcaoOrIataCarrierCode() { Icao = "PGT",
                    Norm =  ProvidedNormNorm.ICAO },
                    NumberExternal = "1244" };

                jsonBody.flight.Departure = new FlightIdentifierScheduledOrEstimated()
                {
                    Station = new Station() { Code = new IcaoOrIataStationCode() { Icao = "ELLX", Norm = ProvidedNormNorm.ICAO  } },
                    Scheduled = new Scheduled() { Local = new FunctionalDateTime() { Date = new int[3] { 2022, 8, 2 }, Time = new int[2] { 13, 20 } } }
                };

                jsonBody.flight.Arrival = new FlightIdentifierScheduledOrEstimated()
                {
                    Station = new Station() { Code = new IcaoOrIataStationCode() { Icao = "VOAT", Norm = ProvidedNormNorm.ICAO  } },
                    Scheduled = new Scheduled() { Local = new FunctionalDateTime() { Date = new int[3] { 2022, 8, 3 }, Time = new int[2] { 13, 50 } } }
                };


                var jsonbody = JsonConvert.SerializeObject(jsonBody);


                return jsonbody;
            }
            catch (Exception ex)
            {
                return "";

            }

        }

        #region Flight Create

        public static async Task<string> SendFlightData(string token)
        {
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    
                    return await PostFlight(client, CreateJsonBody());
                }
            }
        }

        private static async Task<string> PostFlight(HttpClient client, string jsonBody)
        {
            string baseAddress = @"https://certif.paxgov.govlink.streamlane.eu/padelebaa/api/provider-flight-spaces/upload";



            var content = new StringContent(jsonBody.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage JsonMessage = await client.PostAsync(baseAddress, content);
            var jsonContent = await JsonMessage.Content.ReadAsStringAsync();
            return jsonContent;
        }


        #endregion

        #region Token Methods

        public static async Task<Token> GetElibilityToken()
        {
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    return await GetElibilityToken(client);
                }
            }
        }
        private static async Task<Token> GetElibilityToken(HttpClient client)
        {
            string baseAddress = @"https://certif.paxgov.auth.streamlane.eu/auth/realms/ebaa/protocol/openid-connect/token";

            string grant_type = "client_credentials";
            string client_id = "test";
            string client_secret = "test";

            var form = new Dictionary<string, string>
                {
                    {"grant_type", grant_type},
                    {"client_id", client_id},
                    {"client_secret", client_secret},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(baseAddress, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);
            return tok;
        }

        #endregion


        internal class Token
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }
    }
}
