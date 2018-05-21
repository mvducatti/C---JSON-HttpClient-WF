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
        Experiences exp = new Experiences();
        int myValue;

        public Form1()
        {
            InitializeComponent();
            btnUpdate.Enabled = !true;
            fillDataAsync();
        }

        private async void fillDataAsync()
        {
            //creating programatically the grid columns, properties and options
            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "id";
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Name = "companyName";
            dataGridView1.Columns[1].Visible = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;

            using (var client = createClient())
            {
                try
                {
                    response = await client.GetAsync("experiences");

                    if (response.IsSuccessStatusCode)
                    {
                        //Instantiating the class Experience as a List to be accessed  and read Asynchrononously
                        List<Experiences> experiences = await response.Content.ReadAsAsync<List<Experiences>>();
                        for (int i = 0; i < experiences.Count; i++)
                        {
                            //adding to the grid with 2 parameters: 1 - id and 2 - companyname on position [i]
                            string[] row = { "" + experiences[i].id + "", "" + experiences[i].companyname + "" };
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
            //gambiarra
            if (textBox1.Text == "Empresa")
            {
                textBox1.Text = "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //setting the focus to the label, so the hint can be shonw on the input text
            this.ActiveControl = label1;
        }

        private static HttpClient createClient()
        {
            //pre-loading HttpClient initial specs

            //generating the method createCliente(), to be added to all methods that needs to access the server
            //methods

            var client = new HttpClient();
            //Go get the data
            client.BaseAddress = new Uri("http://localhost:3000/");
            client.DefaultRequestHeaders.Accept.Clear();
            //adding json
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private async void btnAdd_ClickAsync(object sender, EventArgs e)
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
                    //Creating a new experience
                    Experiences newExperience = new Experiences();
                    newExperience.companyname = companyName;

                    try
                    {
                        //posting and clearing the grid and filling it again
                        response = await client.PostAsJsonAsync("experiences", newExperience);

                        if (response.IsSuccessStatusCode)
                        {
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

        private async void btnDelete_ClickAsync(object sender, EventArgs e)
        {
            using (var client = createClient())
            {
                try
                {
                    //deleting with global parameter from GridDoubleClick and clearing the grid and filling it again
                    response = await client.DeleteAsync("experiences/" + myValue);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show(" Experiência deletada com sucesso", "DELETE METHOD",
                                MessageBoxButtons.OK);

                        dataGridView1.Rows.Clear();
                        fillDataAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("JSON offline...");
                }
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow.Index != -1)
            {
                //get value from dataGrid double click row and add the value to the global var myValue
                exp.id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
                myValue = exp.id;
                btnDelete.Enabled = true;

                exp.companyname = Convert.ToString(dataGridView1.CurrentRow.Cells["companyName"].Value);
                btnUpdate.Enabled = true;

                if (dataGridView1.CurrentRow.Index != -1)
                {
                    {
                        txtExpName.Text = exp.companyname;
                    }
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
            txtExpName.ResetText();
        }

        private async void btnUpdate_ClickAsync(object sender, EventArgs e)
        {
            using (var client = createClient())
            {

                if (txtExpName.Text == "")
                {
                    MessageBox.Show("Por favor insira uma empresa antes de continuar.", "Campo Vazio",
                        MessageBoxButtons.OK);
                }
                else
                {
                    try
                    {
                        var companyUpdateName = txtExpName.Text;

                        Experiences newExperience = new Experiences();

                        newExperience.companyname = companyUpdateName;

                        response = await client.PutAsJsonAsync("experiences/" + myValue, newExperience);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Experiência atualizada com sucesso", "UPDATE METHOD",
                                    MessageBoxButtons.OK);

                            txtExpName.ResetText();

                            dataGridView1.Rows.Clear();
                            fillDataAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("JSON offline...");
                    }
                }
            }
        }
    }
}
