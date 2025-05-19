using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using hastaneRandevuVkayit.Hastane;
using Microsoft.Data.SqlClient;

namespace hastaneRandevuVkayit
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=DESKTOP-DEG1LQD\\SQLEXPRESS01;Initial Catalog=AcunMedya_SL19;Integrated Security=True;Trust Server Certificate=True";
        int BransID = 0;
        public Form1()
        {
            InitializeComponent();
        }
        public void BransGetir()
        {
            List<Brans> BransListesi = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // MessageBox.Show("veri tabanına bağlandı.");

                    string sorgu = "select * from Brans";
                    SqlCommand command = new SqlCommand(sorgu, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    BransListesi = new List<Brans>();
                    BransListesi.Add(new Brans { ID = -1, Bransi = "Poliklinik seçiniz" });

                    while (reader.Read())
                    {
                        BransListesi.Add(new Brans
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Bransi = (string)reader["Brans"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("veri tabanına bağlanamadı!!!");
                }
                finally
                {
                    connection.Close();
                }
                cmbBrans.DataSource = BransListesi;
                cmbBrans.DisplayMember = "Bransi";
                cmbBrans.ValueMember = "ID";
            }
        }

        public void DoktorGetir(int BransId)
        {
            List<Doktor> DoktorListesi = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    DoktorListesi = new List<Doktor>();
                    connection.Open();
                    // MessageBox.Show("veri tabanına bağlandı.");

                    string sorgu = "select * from Doktor where BransID=" + BransId;
                    SqlCommand command = new SqlCommand(sorgu, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    DoktorListesi = new List<Doktor>();
                    DoktorListesi.Add(new Doktor { ID = -1, Doktoradi = " Doktor seçiniz." });

                    while (reader.Read())
                    {
                        DoktorListesi.Add(new Doktor
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Doktoradi = reader["DoktorAdi"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("veri tabanına bağlanamadı!!!");
                }
                finally
                {
                    connection.Close();
                }
                cmbDoktor.DataSource = DoktorListesi;
                cmbDoktor.DisplayMember = "Doktoradi";
                cmbDoktor.ValueMember = "ID";
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            BransGetir();
        }

        private void cmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            Brans Brans = cmbBrans.SelectedItem as Brans;
            if (Brans != null)
            {
                BransID = Brans.ID;
                if (BransID > 0)
                {
                    DoktorGetir(BransID);
                }
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string Ad = txtAd.Text;
            string Soyad = txtSoyad.Text;
            string TcNoText = txtTC.Text; // tc no önce metin olarak alınır

            Brans SecilenBrans = cmbBrans.SelectedItem as Brans;
            Doktor SecilenDoktor = cmbDoktor.SelectedItem as Doktor;
            DateTime tarih = dateTimePicker1.Value;
            int tcNo = 0; // int tipinde bir değişken tanımlandı

            if(string.IsNullOrEmpty(Ad)|| string.IsNullOrEmpty(Soyad)|| string.IsNullOrEmpty(TcNoText) || SecilenBrans== null|| 
                SecilenBrans.ID<= 0|| SecilenDoktor== null || SecilenDoktor.ID <= 0)
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz ve Poliklinik ile doktor seçiniz.", "uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(TcNoText, out tcNo))
            {
                MessageBox.Show("Lütfen geçerli bir TC Kimlik Numarası giriniz (sayısal değer).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sorgu = "INSERT INTO HastaBilgi (HastaAdi, HastaSoyadi, HastaTC, tarih, BransId, DoktorID) " +
                                   "VALUES (@Ad, @Soyad, @TCNo, @tarih, @BransId, @DoktorID)";

                    SqlCommand command = new SqlCommand(sorgu, connection);
                    command.Parameters.AddWithValue("@Ad", Ad);
                    command.Parameters.AddWithValue("@Soyad", Soyad);
                    command.Parameters.AddWithValue("@TCNo", tcNo); // int tipindeki tcNo değişkenini ekliyoruz
                    command.Parameters.AddWithValue("@tarih", tarih);
                    command.Parameters.AddWithValue("@BransId", SecilenBrans.ID);
                    command.Parameters.AddWithValue("@DoktorID", SecilenDoktor.ID);
                    

                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Randevu kaydı başarıyla oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtAd.Clear();
                        txtSoyad.Clear();
                        txtTC.Clear();
                        cmbBrans.SelectedIndex = 0;
                        cmbDoktor.DataSource = null;
                        VerileriGetir();
                    }
                    else
                    {
                        MessageBox.Show("Randevu kaydı sırasında bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException ex) when (ex.Number == 2627) // Unique key violation (PK or unique index)
                {
                    MessageBox.Show("Bu TC Kimlik Numarası ile daha önce bir kayıt bulunmaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }
            }

        }
        public void VerileriGetir()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    string baglanti = "SELECT * FROM HastaBilgi"; // tüm kayıtları seç
                    SqlDataAdapter adapter = new SqlDataAdapter(baglanti, connection);
                    DataTable datatable = new DataTable();
                    adapter.Fill(datatable);
                    dataGridView1.DataSource = datatable;
                }
            }
            catch (Exception ex){

                MessageBox.Show("verileri getirirken bir hata oluştu!!"+ ex.Message,"Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
        } 
    }
}
