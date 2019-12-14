using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace LeerlingLijst
{
    public partial class Koppellen : Form
    {
        static Api api = new Api();
        public Koppellen()
        {
            InitializeComponent();
        }

        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(SchoolEntry.Text);
            System.Diagnostics.Debug.WriteLine(KoppelEntry.Text);
            Token token = await api.KoppelAuthAsync(SchoolEntry.Text, KoppelEntry.Text);
            File.WriteAllText("token.json", JsonConvert.SerializeObject(token));
            this.Close();
        }
    }
}
