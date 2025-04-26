using System.Reflection;
using System.Transactions;
using System.Collections.Generic;
using System.ComponentModel;
using static CustomAttribute_22._04.Form1;

namespace CustomAttribute_22._04
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        public class Ogrenci
        {
            [ZorunluAlanAttributeCustom("Lütfen bir isim giriniz!!")]
            public string Isim;
            [ZorunluAlanAttributeCustom("Lütfen bir Soyisim giriniz!!")]
            public string Soyisim;
            [ZorunluAlanAttributeCustom("Lütfen bir Bölüm giriniz!!")]
            public string Bolum;
        }

        private void btnBilgiDogrula_Click(object sender, EventArgs e)
        {
            Ogrenci ogr = new Ogrenci()
            {
                Isim = txtbxAdiniz.Text,
                Soyisim = txtbxSoyadiniz.Text,
                Bolum = txtbxBolum.Text
            };

            lblGirilenBilgi.Text = $"Ad: {ogr.Isim}\nSoyad: {ogr.Soyisim}\nBölüm: {ogr.Bolum}";


            List<string> hatalar = Bilgileridogrula(ogr);
            if(hatalar.Count> 0)
            {
                MessageBox.Show(("Birşeyler eksik veya yanlış yeniden gözden geçiriniz!!"), "hatalar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Bilgiler başarıyla doğrulandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

      public static List<string> Bilgileridogrula(object DogrulanacakObje)
        {
            List<string> hatalar = new List<string>();
            Type DogrulanacakTur = DogrulanacakObje.GetType();
            FieldInfo[] DogrulanacakTurAlanlari = DogrulanacakTur.GetFields(BindingFlags.Public | BindingFlags.Instance);


            foreach (FieldInfo DogrulanacakturAlani in DogrulanacakTurAlanlari)
            {
                object[] ZorunluAlanOzellikleri = DogrulanacakturAlani.GetCustomAttributes(typeof(ZorunluAlanAttributeCustom), true);
                       if (ZorunluAlanOzellikleri.Length != 0)
                {
                    string alandegeri = DogrulanacakturAlani.GetValue(DogrulanacakObje) as string;

                    if (string.IsNullOrEmpty(alandegeri))
                    {
                        foreach(ZorunluAlanAttributeCustom attribute in ZorunluAlanOzellikleri)
                        {
                            hatalar.Add(attribute.HataMesaji);
                        }
                    }

                }
            }
            return hatalar;
        }
    } }
    


 
