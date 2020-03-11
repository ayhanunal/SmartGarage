using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Akıllı_Garaj
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        SqlConnection con;
        SqlCommand com;

        private bool AlgoritmikŞifre(string şifre)
        {
            bool kabul = false;
            if (şifre.Length == 9)
            {
                try
                {
                    if ((Char.GetNumericValue(şifre[0]) * Char.GetNumericValue(şifre[1])) % 2 == Char.GetNumericValue(şifre[2]) &&
                        (Char.GetNumericValue(şifre[3]) * Char.GetNumericValue(şifre[4])) % 2 == Char.GetNumericValue(şifre[5]) &&
                        (Char.GetNumericValue(şifre[6]) * Char.GetNumericValue(şifre[7])) % 2 == Char.GetNumericValue(şifre[8]) &&
                        !şifre.Substring(0,3).Equals(şifre.Substring(3,3)) && !şifre.Substring(3, 3).Equals(şifre.Substring(6, 3)) &&
                        !şifre.Substring(0, 3).Equals(şifre.Substring(6, 3)))
                    {
                        kabul = true;
                    }
                } catch (Exception e)
                {
                    kabul = false;
                }

            }
            return kabul;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Alanlardan birisi bile boş bırakılamaz!", "Alanlar Boş", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                if (textBox1.Text.Length > 40 || textBox2.Text.Length > 40)
                {
                    MessageBox.Show("Kullanıcı adı veya şifre 40 karakteri geçemez!", "Karakter Aşımı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else
                {
                    if (!AlgoritmikŞifre(textBox3.Text)) MessageBox.Show("Algoritmik şifre yanlış!", "Algoritmik Şifre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        con = new SqlConnection("Data Source=.\\SQLEXPRESS; Database=AkilliGaraj; Trusted_Connection=true;");
                        com = new SqlCommand();
                        con.Open();
                        com.Connection = con;
                        com.CommandText = "INSERT INTO Kullanicilar(kullaniciAdi,sifre) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "')";
                        com.ExecuteNonQuery();
                        con.Dispose();
                        con.Close();
                        MessageBox.Show("Kayıt başarılı!", "Başarılı Kayıt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 f1 = new Form1();
            foreach (Form _f in Application.OpenForms)
            {
                if (_f.Name == "Form1")
                {
                    f1 = (Form1)_f;
                }
            }
            f1.Enabled = true;
        }
    }
}
