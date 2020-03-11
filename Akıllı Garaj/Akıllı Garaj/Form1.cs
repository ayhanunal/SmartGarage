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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection con;
        SqlCommand com;
        SqlDataReader dar;
        bool giriş;

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Alanlar boş bırakılamaz!", "Eksik Alan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                giriş = false;
                con = new SqlConnection("Data Source=.\\SQLEXPRESS; Database=AkilliGaraj; Trusted_Connection=true;");
                com = new SqlCommand();
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM Kullanicilar";
                com.ExecuteNonQuery();
                dar = com.ExecuteReader();
                while (dar.Read())
                {
                    if (dar["kullaniciAdi"].ToString() == textBox1.Text)
                    {
                        if (dar["sifre"].ToString() == textBox2.Text)
                        {                            
                            giriş = true;
                            Form3 f3 = new Form3();
                            f3.Show();
                            this.Hide();
                        }                       
                    }
                }
                if (!giriş) MessageBox.Show("Yanlış kullanıcı adı veya şifre!", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Dispose();
                con.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
            this.Enabled = false;
        }
    }
}
