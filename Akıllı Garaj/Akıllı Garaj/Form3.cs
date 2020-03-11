using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;

namespace Akıllı_Garaj
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        int ledDurum = 0;
        int motorDurum = 0;
        SerialPort sp;
        string gelenID;
        string kaydedilenID = "";

        private void GetIDs()
        {
            listBox1.Items.Clear();
            SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS; Database=AkilliGaraj; Trusted_Connection=true;");
            SqlCommand com = new SqlCommand();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT * FROM IzinliAraclar";
            com.ExecuteNonQuery();
            SqlDataReader dar = com.ExecuteReader();
            int sayaç = 0;
            while (dar.Read())
            {
                listBox1.Items.Add(dar["aracId"].ToString());
            }
            con.Dispose();
            con.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            tabControl1.Enabled = false;
            label3.ForeColor = Color.Red;
            label3.Text = "Kurulmadı";
            Form1 f1 = new Form1();
            foreach (Form _f in Application.OpenForms)
            {
                if (_f.Name == "Form1")
                {
                    f1 = (Form1)_f;
                }
            }
            label5.Text = f1.textBox1.Text;
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < System.IO.Ports.SerialPort.GetPortNames().Length; i++)
            {
                comboBox1.Items.Add(System.IO.Ports.SerialPort.GetPortNames()[i]);
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {   
            if (sp != null)
            if (sp.IsOpen)
            {
                sp.WriteLine("k");
            }
            Form1 f1 = new Form1();
            foreach (Form _f in Application.OpenForms)
            {
                if (_f.Name == "Form1")
                {
                    f1 = (Form1)_f;
                }
            }
            f1.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Arduino portunu seçmeniz gerekiyor!", "Port Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                sp = new SerialPort(comboBox1.Text, 9600);
                bool hata = false;
                try
                {
                    sp.Open();
                } catch
                {
                    hata = true;
                }
                if (sp.IsOpen && !hata)
                {
                    sp.WriteLine("a");
                    label3.ForeColor = Color.Green;
                    label3.Text = "Kuruldu (" + comboBox1.Text + ")";
                    button1.Enabled = false;
                    comboBox1.Enabled = false;
                    tabControl1.Enabled = true;
                    this.Text = "Akıllı Garaj - Yönetim";
                    tabControl1.SelectedIndex = 0;
                } else
                {
                    MessageBox.Show("Bağlantı kesilmiş olabilir!", "Bağlantı Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                this.Text = "Akıllı Garaj - Yönetim";
                timer1.Stop();
            }
            if (tabControl1.SelectedIndex == 1)
            {
                GetIDs();
                this.Text = "Akıllı Garaj - ID";
                timer1.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ledDurum == 0)
            {
                ledDurum = 1;
                button2.BackColor = Color.FromArgb(128, 255, 128);
                sp.WriteLine("01");
            }
            else
            {
                ledDurum = 0;
                button2.BackColor = Color.FromArgb(90, 90, 90);
                byte[] veri = BitConverter.GetBytes(1);
                sp.WriteLine("00");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (motorDurum == 0)
            {
                motorDurum = 1;
                button3.BackColor = Color.FromArgb(128, 255, 128);
                sp.WriteLine("11");
            }
            else
            {
                motorDurum = 0;
                button3.BackColor = Color.FromArgb(90, 90, 90);
                sp.WriteLine("10");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            gelenID = sp.ReadExisting();
            if (gelenID == "")
            {
                label9.Text = kaydedilenID;
            } else
            {
                kaydedilenID = gelenID;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (kaydedilenID.Length == 0) MessageBox.Show("Henüz bir kart okutulmamış!", "ID Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                bool hata = false;
                string düzenlenmiş = kaydedilenID.Substring(0, kaydedilenID.Length - 2);
                SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS; Database=AkilliGaraj; Trusted_Connection=true;");
                SqlCommand com = new SqlCommand();
                con.Open();
                com.Connection = con;
                com.CommandText = "INSERT INTO IzinliAraclar(aracId) VALUES ('" + düzenlenmiş + "')";
                try
                {
                    com.ExecuteNonQuery();
                } catch
                {
                    hata = true;
                    MessageBox.Show("Bu ID önceden eklenmiş", "UNIQUE ID Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }               
                con.Dispose();
                con.Close();
                if (!hata)
                {
                    listBox1.Items.Add(düzenlenmiş);
                    int sayı = listBox1.Items.Count;
                    sp.WriteLine("e " + sayı.ToString() + " " + düzenlenmiş + " ");
                }               
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GetIDs();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int seçilen = listBox1.SelectedIndex;
            if (seçilen == -1) MessageBox.Show("Önce bir ID seçmeniz gerek!", "Seçilmeyen ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string veri = listBox1.Items[seçilen].ToString();
                if (MessageBox.Show(veri + " ID'sini silmek istediğinize emin misiniz?", "Silmek İstiyor Musunuz?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    listBox1.Items.RemoveAt(seçilen);
                    SqlConnection con = new SqlConnection("Data Source=.\\SQLEXPRESS; Database=AkilliGaraj; Trusted_Connection=true;");
                    SqlCommand com = new SqlCommand();
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "DELETE FROM IzinliAraclar WHERE aracId = '" + veri + "'";
                    com.ExecuteNonQuery();
                    con.Dispose();
                    con.Close();
                    sp.Write("h");
                    for (int i=0; i<listBox1.Items.Count; i++) sp.WriteLine("e " + (i + 1).ToString() + " " + listBox1.Items[i] + " ");
                }
            }
        }
    }
}
