using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using kayitformuDBli.Models;
using Microsoft.Data.SqlClient;
//using System.Data.SqlClient;

namespace kayitformuDBli
{
    public partial class Form1 : Form
    {

        string connectionString = "Data Source=DESKTOP-DEG1LQD\\SQLEXPRESS01;Initial Catalog=AcunMedya_SL19;Integrated Security=True;Trust Server Certificate=True";

        int MarkaID = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MarkaGetir();
            
        }
        public void ModelGetir(int markaId)
        {
            List<modeller> modellistesi = null;
            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                try
                {
                    modellistesi = new List<modeller>();
                    connection.Open();
                    // MessageBox.Show("veri tabanına bağlandı.");
                    
                    string sorgu = "select * from Model where MarkaID="+markaId;
                    SqlCommand command = new SqlCommand(sorgu, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        modellistesi.Add(new modeller
                        {
                            id = Convert.ToInt32(reader["ID"]),
                            modeli = reader["ModelAdi"].ToString()
                        });
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("veri tabanına bağlanamadı." + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                comboBoxmodel.DataSource = modellistesi;
                comboBoxmodel.DisplayMember = "modeli";
                comboBoxmodel.ValueMember = "id";
            }
        }
        public void MarkaGetir()
        {
            List<Marka> markalistesi = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               try {
                    connection.Open();
                    // MessageBox.Show("veri tabanına bağlandı.");

                    string sorgu = "select * from Marka";
                    SqlCommand command = new SqlCommand(sorgu, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    markalistesi = new List<Marka>();
                    markalistesi.Add(new Marka { id=-1, markasi="Marka Seçiniz"});
                    while (reader.Read())
                    {
                        markalistesi.Add(new Marka {
                            id = Convert.ToInt32(reader["ID"]),
                            markasi = (string)reader["MarkaAdi"] });
                    }

                }
                
               catch (Exception ex)
            {
                    MessageBox.Show("veri tabanına bağlanmadı." +ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                // combobox veri kaynağını dtabse yapan kısım
                comboBoxMarka.DataSource= markalistesi;
                comboBoxMarka.DisplayMember = "markasi";
                comboBoxMarka.ValueMember = "id"; // id ve markasi kısmı marka classındaki tanımmları ile alakalı
            }
        }

        private void comboBoxMarka_SelectedIndexChanged(object sender, EventArgs e)
        {
            Marka marka = comboBoxMarka.SelectedItem as Marka;
            if (marka != null)
            {
                MarkaID = marka.id;
                if(MarkaID > 0)
                {
                    ModelGetir(MarkaID);
                }
                
            }
        }
    }
}
