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
using System.Threading;

namespace Investing_parser
{
    public partial class Form1 : Form
    {
        public static Settings settings = new Settings();
        public static string programStatus = "...";
        Task Parser;
        static CancellationTokenSource src = new CancellationTokenSource();
        CancellationToken token = src.Token;

        Dictionary<string, string> Countries = new Dictionary<string, string>();
        Dictionary<string, string> Categories = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();

            countriesBox.Items.Clear();
            var f = File.ReadAllLines("countries.txt");
            foreach(var s in f)
            {
                var v = s.Split(':');
                countriesBox.Items.Add(v[0]);
                Countries.Add(v[0], v[1]);
            }

            categoriesBox.Items.Clear();
            f = File.ReadAllLines("categories.txt");
            foreach (var s in f)
            {
                var v = s.Split(':');
                categoriesBox.Items.Add(v[0]);
                Categories.Add(v[0], v[1]);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.startDay = int.Parse(textBox1.Text);
                textBox1.BackColor = Color.White;
            }
            catch
            {
                textBox1.BackColor = Color.Pink;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            try
            {
                settings.startMonth = int.Parse(textBox2.Text);
                textBox2.BackColor = Color.White;
            }
            catch
            {
                textBox2.BackColor = Color.Pink;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.startYear = int.Parse(textBox3.Text);
                textBox3.BackColor = Color.White;
            }
            catch
            {
                textBox3.BackColor = Color.Pink;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.finishDay = int.Parse(textBox6.Text);
                textBox6.BackColor = Color.White;
            }
            catch
            {
                textBox6.BackColor = Color.Pink;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.finishMonth = int.Parse(textBox5.Text);
                textBox5.BackColor = Color.White;
            }
            catch
            {
                textBox5.BackColor = Color.Pink;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.finishYear = int.Parse(textBox4.Text);
                textBox4.BackColor = Color.White;
            }
            catch
            {
                textBox4.BackColor = Color.Pink;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                settings.importance = textBox7.Text.Split(',').Select(c=>int.Parse(c)).ToList();
                textBox7.BackColor = Color.White;
            }
            catch
            {
                textBox7.BackColor = Color.Pink;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.countries.Clear();
            foreach (var i in countriesBox.CheckedItems)
            {
                settings.countries.Add(Countries[i.ToString()]);
            }
        }

        private void categoriesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.categories.Clear();
            foreach (var i in categoriesBox.CheckedItems)
            {
                settings.categories.Add(Categories[i.ToString()]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Статус выполнения: " + programStatus;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Старт")
            {
                src = new CancellationTokenSource();
                token = src.Token;
                Parser = new Task(()=> Invest.getData(settings, token));
                Parser.Start();
                button1.Text = "Стоп";
            }
            else
            {
                programStatus = "stopped";
                src.Cancel();
                button1.Text = "Старт";
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            settings.countries.Clear();
            for (int i = 0; i < countriesBox.Items.Count; i++)
            {
                countriesBox.SetItemChecked(i, true);
                settings.countries.Add(Countries[countriesBox.Items[i].ToString()]);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            settings.countries.Clear();
            for (int i = 0; i < countriesBox.Items.Count; i++)
                countriesBox.SetItemChecked(i, false);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            settings.categories.Clear();
            for (int i = 0; i < categoriesBox.Items.Count; i++)
            {
                categoriesBox.SetItemChecked(i, false);
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            settings.categories.Clear();
            for (int i = 0; i < categoriesBox.Items.Count; i++)
            {
                categoriesBox.SetItemChecked(i, true);
                settings.categories.Add(Categories[categoriesBox.Items[i].ToString()]);
            }
        }
    }
}
