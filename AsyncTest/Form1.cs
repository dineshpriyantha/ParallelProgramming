using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncTest
{
    public partial class Form1 : Form
    {
        public int CalculateValue()
        {
            Thread.Sleep(5000);
            return 789;
        }

        public Task<int> CalculateAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                return 123;
            });
        }

        public Task<string> GetCompanyNameAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                return "ABC Tech";
            });
        }

        public async Task<int> CalculateAsync2()
        {
            await Task.Delay(5000);
            return 456;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            //int n = CalculateValue();
            //lblResult.Text = n.ToString();

            //var calculate = CalculateAsync();

            //calculate.ContinueWith(t =>
            //{
            //    lblResult.Text = t.Result.ToString();
            //}, TaskScheduler.FromCurrentSynchronizationContext());

            // var getNAME = GetCompanyNameAsync();
            // getNAME.ContinueWith(t =>
            //{
            //    string comName = t.Result.ToString();
            //    MessageBox.Show(comName);
            //}, TaskScheduler.FromCurrentSynchronizationContext());

            int value = await CalculateAsync();
            lblResult.Text = value.ToString();

            await Task.Delay(5000);

            using (var wc = new WebClient())
            {
                string data = await wc.DownloadStringTaskAsync("http://google.com/robots.txt");
                lblResult.Text = data.Split('\n')[0].Trim();
            }
        }
    }
}
