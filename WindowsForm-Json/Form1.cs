using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using WindowsForm_Json.Models;

namespace WindowsForm_Json
{
    public partial class Form1 : Form
    {
        private static HttpResponseMessage response;

        public Form1()
        {
            InitializeComponent();
            fillDataAsync();
        }

        private async void fillDataAsync()
        {
            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].Name = "companyName";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            using (var client = createClient())
            {
                try
                {
                    response = await client.GetAsync("experiences");

                    if (response.IsSuccessStatusCode)
                    {
                        List<Experiences> experiences = await response.Content.ReadAsAsync<List<Experiences>>();
                        for (int i = 0; i < experiences.Count; i++)
                        {
                            string[] row = { "" + experiences[i].companyname + "" };
                            dataGridView1.Rows.Add(row);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Verifique se o JSON está ligado", "Erro de conexão",
                        MessageBoxButtons.OK);
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Empresa")
            {
                textBox1.Text = "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = label1;
        }


        //pre-loading HttpClient initial specs
        private static HttpClient createClient()
        {
            var client = new HttpClient();
            //Go get the data
            client.BaseAddress = new Uri("http://localhost:3000/");
            client.DefaultRequestHeaders.Accept.Clear();
            //adding json
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            using (var client = createClient())
            {
                //get value from textBox
                var companyName = textBox1.Text;

                if (textBox1.Text == "")
                {
                    MessageBox.Show("Por favor insira uma empresa antes de continuar.", "Campo Vazio",
                        MessageBoxButtons.OK);
                }
                else
                {
                    Experiences newExperience = new Experiences();
                    newExperience.companyname = companyName;

                    try
                    {
                        response = await client.PostAsJsonAsync("experiences", newExperience);

                        if (response.IsSuccessStatusCode)
                        {
                            Uri experienceUrl = response.Headers.Location;
                            MessageBox.Show(companyName + " adicionada(O) com sucesso", "POST METHOD",
                                MessageBoxButtons.OK);
                            textBox1.Text = "Empresa";
                            dataGridView1.Rows.Clear();
                            fillDataAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Verifique se o JSON está ligado", "Erro de conexão",
                            MessageBoxButtons.OK);
                    }
                }
            }
        }

        private async void dataGridView1_CellContentClickAsync(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
