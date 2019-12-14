using System;
using System.IO;
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
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static Token token;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!File.Exists("token.json"))
            {
                Application.Run(new Koppellen());
            }
            using (StreamReader r = new StreamReader("token.json"))
            {
                string json = r.ReadToEnd();
                token = JsonConvert.DeserializeObject<Token>(json);
                Console.WriteLine(token.access_token);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                RunAsync().GetAwaiter().GetResult();
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
            }

        }
        static async Task RunAsync()
        {
            Api api = new Api();
            api.token = token;
            UserResponse response = await api.GetUser(fields: "prefix,lastName,code,firstName");
            List<UData> users = response.response.data;

            SortedDictionary<string, SortedDictionary<string, List<string>>> klassen = await Sorting.GroupBySubject(users, api);
            
            //string json = JsonConvert.SerializeObject(klassen, Formatting.Indented);
            await Sorting.SortVakkenToFile(Sorting.CompareKlassen, klassen, $"{DateTime.Now.Year}/vakken");
            await Sorting.SortLeerlingenToFile(Sorting.CompareKlassen, klassen, users, $"{DateTime.Now.Year}/leerlingen");
            //Console.WriteLine(json);

        }
    }
}
