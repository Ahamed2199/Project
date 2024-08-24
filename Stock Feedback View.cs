using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace sentiment_analysis
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection("Data Source=.;Initial Catalog=sentimentanalysis;Integrated Security=True");


        private void Form6_Load(object sender, EventArgs e)
        {
            StreamReader sw = new StreamReader(@"C:\stock.txt");
            label1.Text = sw.ReadLine();
            sw.Close();              
            con.Close();
            con.Open();
            string l = label1.Text;
            SqlCommand cmd = new SqlCommand("select [Other Suggestions] as Suggestions  from feedback where id= '" + l + "'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            dataGridView1.DataSource = dt;

            SqlCommand cmd1 = new SqlCommand("select Message  from general feedback where [Stock id]= '" + l + "'", con);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            con.Close();
            dataGridView2.DataSource = dt1;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int good = 0, avg = 0, bad = 0;

            con.Close();
            con.Open();
            SqlCommand cmdd = new SqlCommand("select @p = COUNT([Over All Status]) from feedback where id = '"+label1.Text+"' and  [Over All Status] = 'Bad'", con);
            cmdd.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd.ExecuteNonQuery();
            bad = int.Parse(cmdd.Parameters["@p"].Value.ToString());
            con.Close();


            con.Open();
            SqlCommand cmdd1 = new SqlCommand("select @p = COUNT([Over All Status]) from feedback where id = '" + label1.Text + "' and  [Over All Status] = 'Good'", con);
            cmdd1.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd1.ExecuteNonQuery();
            good = int.Parse(cmdd1.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd2 = new SqlCommand("select @p = COUNT([Over All Status]) from feedback where id = '" + label1.Text + "' and  [Over All Status] = 'Average'", con);
            cmdd2.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd2.ExecuteNonQuery();
            avg = int.Parse(cmdd2.Parameters["@p"].Value.ToString());
            con.Close();



            con.Open();
            SqlCommand cmdd3 = new SqlCommand("select @p = COUNT([Status]) from general feedback where [Stock id] = '" + label1.Text + "' and  [Status] = 'Good'", con);
            cmdd3.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd3.ExecuteNonQuery();
            good += int.Parse(cmdd3.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd4 = new SqlCommand("select @p = COUNT([Status]) from general feedback where [Stock id]= '" + label1.Text + "' and  [Status] = 'Average'", con);
            cmdd4.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd4.ExecuteNonQuery();
            avg += int.Parse(cmdd4.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd5 = new SqlCommand("select @p = COUNT([Status]) from general feedback where [Stock id] = '" + label1.Text + "' and  [Status] = 'Bad'", con);
            cmdd5.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd5.ExecuteNonQuery();
            bad += int.Parse(cmdd5.Parameters["@p"].Value.ToString());
            con.Close();
            

            // Data arrays.
            string[] seriesArray = { "Good", "Average","Bad" };
            int[] pointsArray = { good,avg, bad};

            // Set palette.
            this.chart1.Palette = ChartColorPalette.BrightPastel;

            // Set title.
            this.chart1.Titles.Add("Over All Status");

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart1.Series.Add(seriesArray[i]);

                // Add point.
                series.Points.Add(pointsArray[i]);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int b = 0, t = 0, c = 0, s = 0;


            con.Close();
            con.Open();
            SqlCommand cmmd = new SqlCommand("select @p = COUNT([Over All Status]) from feedback where id = '" + label1.Text + "' and  [Over All Status] = 'Bad'", con);
            cmmd.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmmd.ExecuteNonQuery();
            b = int.Parse(cmmd.Parameters["@p"].Value.ToString());
            con.Close();

            // Data arrays.
            string[] seriesArray = {  "Stock rate","Teaching Style","Communication","Subject Knowledge" };
            int[] pointsArray = { b, c, t,s };

            // Set palette.
            this.chart2.Palette = ChartColorPalette.Fire;

            // Set title.
            this.chart2.Titles.Add("Individual Status");

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart2.Series.Add(seriesArray[i]);

                // Add point.
                series.Points.Add(pointsArray[i]);
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            int good = 0, bad = 0, avg = 0;
            
            con.Close();
            con.Open();
            SqlCommand cmdd = new SqlCommand("select @p = COUNT([Stock rate]) from feedback where id = '" + label1.Text + "' and  [Stock rate] = 'Bad'", con);
            cmdd.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd.ExecuteNonQuery();
            bad = int.Parse(cmdd.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd1 = new SqlCommand("select @p = COUNT([Stock rate]) from feedback where id = '" + label1.Text + "' and  [Stock rate] = 'Good'", con);
            cmdd1.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd1.ExecuteNonQuery();
            good = int.Parse(cmdd1.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd2 = new SqlCommand("select @p = COUNT([Stock rate]) from feedback where id = '" + label1.Text + "' and  [Stock rate] = 'Average'", con);
            cmdd2.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd2.ExecuteNonQuery();
            avg = int.Parse(cmdd2.Parameters["@p"].Value.ToString());
            con.Close();


            // Data arrays.
            string[] seriesArray = { "Good", "Average", "Bad" };
            int[] pointsArray = { good, avg, bad };

            // Set palette.
            this.chart2.Palette = ChartColorPalette.BrightPastel;

            // Set title.
            this.chart2.Titles.Add("Over All Status");

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart2.Series.Add(seriesArray[i]);

                // Add point.
                series.Points.Add(pointsArray[i]);
            }

            //teching style

            con.Close();
            con.Open();
            SqlCommand cmdd3 = new SqlCommand("select @p = COUNT([Teaching Style]) from feedback where id = '" + label1.Text + "' and  [Teaching Style] = 'Bad'", con);
            cmdd3.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd3.ExecuteNonQuery();
            bad = int.Parse(cmdd3.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd4 = new SqlCommand("select @p = COUNT([Teaching Style]) from feedback where id = '" + label1.Text + "' and  [Teaching Style] = 'Good'", con);
            cmdd4.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd4.ExecuteNonQuery();
            good = int.Parse(cmdd4.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd5 = new SqlCommand("select @p = COUNT([Teaching Style]) from feedback where id = '" + label1.Text + "' and  [Teaching Style] = 'Average'", con);
            cmdd5.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd5.ExecuteNonQuery();
            avg = int.Parse(cmdd5.Parameters["@p"].Value.ToString());
            con.Close();
            for(int j=0;j<3;j++)
            {
                if (j == 0)
                {
                    pointsArray[j] = good;
                }

                if (j == 1)
                {
                    pointsArray[j] = avg;
                }
                if (j == 2)
                {
                    pointsArray[j] = bad;
                }



            }
             

            // Set palette.
            this.chart3.Palette = ChartColorPalette.BrightPastel;

            // Set title.
            this.chart3.Titles.Add("Teaching Style");

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart3.Series.Add(seriesArray[i]);

                // Add point.
                series.Points.Add(pointsArray[i]);
            }


            // communication

            con.Close();
            con.Open();
            SqlCommand cmdd6 = new SqlCommand("select @p = COUNT([Communication]) from feedback where id = '" + label1.Text + "' and  [Communication] = 'Bad'", con);
            cmdd6.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd6.ExecuteNonQuery();
            bad = int.Parse(cmdd6.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd7 = new SqlCommand("select @p = COUNT([Communication]) from feedback where id = '" + label1.Text + "' and  [Communication] = 'Good'", con);
            cmdd7.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd7.ExecuteNonQuery();
            good = int.Parse(cmdd7.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd8 = new SqlCommand("select @p = COUNT([Communication]) from feedback where id = '" + label1.Text + "' and  [Communication] = 'Average'", con);
            cmdd8.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd8.ExecuteNonQuery();
            avg = int.Parse(cmdd8.Parameters["@p"].Value.ToString());
            con.Close();
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    pointsArray[j] = good;
                }

                if (j == 1)
                {
                    pointsArray[j] = avg;
                }
                if (j == 2)
                {
                    pointsArray[j] = bad;
                }



            }


            // Set palette.
            this.chart4.Palette = ChartColorPalette.BrightPastel;

            // Set title.
            this.chart4.Titles.Add("Teaching Style");

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart4.Series.Add(seriesArray[i]);

                // Add point.
                series.Points.Add(pointsArray[i]);
            }

            //Subject knowledge


            con.Close();
            con.Open();
            SqlCommand cmdd9 = new SqlCommand("select @p = COUNT([Subject Knowledge]) from feedback where id = '" + label1.Text + "' and  [Subject Knowledge] = 'Bad'", con);
            cmdd9.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd9.ExecuteNonQuery();
            bad = int.Parse(cmdd9.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd10 = new SqlCommand("select @p = COUNT([Subject Knowledge]) from feedback where id = '" + label1.Text + "' and  [Subject Knowledge] = 'Good'", con);
            cmdd10.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd10.ExecuteNonQuery();
            good = int.Parse(cmdd10.Parameters["@p"].Value.ToString());
            con.Close();

            con.Open();
            SqlCommand cmdd11 = new SqlCommand("select @p = COUNT([Subject Knowledge]) from feedback where id = '" + label1.Text + "' and  [Subject Knowledge] = 'Average'", con);
            cmdd11.Parameters.Add("@p", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdd11.ExecuteNonQuery();
            avg = int.Parse(cmdd11.Parameters["@p"].Value.ToString());
            con.Close();
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    pointsArray[j] = good;
                }

                if (j == 1)
                {
                    pointsArray[j] = avg;
                }
                if (j == 2)
                {
                    pointsArray[j] = bad;
                }



            }


            // Set palette.
            this.chart5.Palette = ChartColorPalette.BrightPastel;

            // Set title.
            this.chart5.Titles.Add("Teaching Style");

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart5.Series.Add(seriesArray[i]);

                // Add point.
                series.Points.Add(pointsArray[i]);
            }






        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form7 f7 = new Form7();
            f7.Show();
            this.Close();
        }
    }
}
