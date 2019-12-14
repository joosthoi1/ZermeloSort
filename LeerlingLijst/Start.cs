using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LeerlingLijst
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            long start = ((DateTimeOffset)dateTimeStart.Value.Date).ToUnixTimeSeconds();
            long end = ((DateTimeOffset)dateTimeEnd.Value.Date.AddDays(1)).ToUnixTimeSeconds();
            Console.WriteLine(start);
            Console.WriteLine(end);
            Api api = new Api();
            api.token = Program.token;
            UserResponse response = await api.GetUser(fields: "prefix,lastName,code,firstName");
            List<UData> users = response.response.data;

            SortedDictionary<string, SortedDictionary<string, List<string>>> klassen = await Sorting.GroupBySubject(users, api, start, end, progressBar1, progressLabel);
            await Sorting.SortVakkenToFile(Sorting.CompareKlassen, klassen, $"{textBox1.Text}/{DateTime.Now.Year}/vakken");
            await Sorting.SortLeerlingenToFile(Sorting.CompareKlassen, klassen, users, $"{textBox1.Text}/{DateTime.Now.Year}/leerlingen", progressBar1, progressLabel);
            MessageBox.Show("Done!");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {

                if (openFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog.SelectedPath))
                {
                    //Get the path of specified file
                    filePath = openFileDialog.SelectedPath;
                }
            }
            textBox1.Text = filePath;
        }

        private void Start_Load(object sender, EventArgs e)
        {
            dateTimeStart.Value = DateTime.Today;
            dateTimeEnd.Value = DateTime.Today;
        }
    }
}
