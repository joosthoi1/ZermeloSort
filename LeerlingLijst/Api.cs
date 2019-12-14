using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace LeerlingLijst
{
    


    class Api
    {
        static HttpClient client = new HttpClient();

        public Token token { get; set; }
        public async Task<Token> KoppelAuthAsync(string School, string Koppel)
        {
            Koppel = Koppel.Replace(" ", "");
            using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), $"https://{School}.zportal.nl/api/v2/oauth/token"))
            {
                request.Content = new StringContent($"grant_type=authorization_code&code={Koppel}");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.SendAsync(request);
                Console.WriteLine(response);
                Token token = await response.Content.ReadAsAsync<Token>();
                token.school = School;

                this.token = token;
                return token;
            }
        }
        public async Task<UserResponse> GetUser(String user = "", String fields = "prefix,lastName,code,schoolInSchoolYears,roles,firstName", String flags = "&isStudent=true&schoolInSchoolYear=751")
        {
            String url;
            if (String.IsNullOrEmpty(user))
            {
                url = $"https://{this.token.school}.zportal.nl/api/v3/users?" +
                             $"access_token={this.token.access_token}" +
                             $"{flags}&" +
                             $"fields={fields}";
            }
            else
            {
                url = $"https://{this.token.school}.zportal.nl/api/v3/users?" +
                                $"access_token={this.token.access_token}" +
                                $"{flags}&" +
                                $"fields={fields}&" +
                                $"code={user}";
            }
            Console.WriteLine(url);
            HttpResponseMessage response = await client.GetAsync(url);
            

            return await response.Content.ReadAsAsync<UserResponse>();
        }
        public async Task<ScheduleResponse> GetWeekSchedule(int week_start, int week_end, string user = "~me", string[] fields = null)
        {
            fields = fields ?? new string[2] {"subjects", "groups"};

            DateTime today = DateTime.Today;
            long unix0 = ((DateTimeOffset)today).ToUnixTimeSeconds();
            long unix1 = ((DateTimeOffset)today).ToUnixTimeSeconds();
            unix0 += (-((int)today.DayOfWeek - 1) + (week_start * 7)) * 86400;
            unix1 += (-((int)today.DayOfWeek - 1) + (5 + week_end * 7)) * 86400;
            return await GetSchedule(unix0, unix1, user);
        }
        public async Task<ScheduleResponse> GetSchedule(long start, long end, string user = "~me", string[] fields = null)
        {
            fields = fields ?? new string[2] { "subjects", "groups" };

            String url = $"https://{this.token.school}.zportal.nl/api/v3/appointments?" +
                         $"access_token={this.token.access_token}&" +
                         $"start={start}&" +
                         $"end={end}&" +
                         $"fields={string.Join(",", fields)}&" +
                         $"user={user}";
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Content.ReadAsAsync<ScheduleResponse>();
        }
    }
}