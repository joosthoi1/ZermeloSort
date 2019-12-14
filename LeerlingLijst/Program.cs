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
        static public Token token;
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
            }
            Console.WriteLine(token.access_token);
            Application.Run(new Start());
        }
    }
}
