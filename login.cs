using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

public partial class Buyer_login : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection("Data Source=.;Initial Catalog=sentimentanalysis;Integrated Security=True");

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        con.Open();
        SqlCommand cmd = new SqlCommand("select * from Stock where id = '" + TextBox3.Text + "' and password= '" + TextBox4.Text + "'", con);
        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            StreamWriter sw = new StreamWriter(@"C:\stock.txt", false);
            sw.WriteLine(TextBox3.Text);
            sw.Close();
            System.Diagnostics.Process.Start(@"C:\Stock\sentiment analysis\bin\Debug\sentiment analysis.exe");
            con.Close();
            
        }
        else
        {
            MessageBox.Show("Enter correct username and password");
            con.Close();
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        con.Open();
        SqlCommand cmd = new SqlCommand("select * from Buyer where id = '" + TextBox1.Text + "' and password= '" + TextBox2.Text + "'", con);
        SqlDataReader dr;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {

            System.Diagnostics.Process.Start(@"C:\Buyer\sentiment analysis\bin\Debug\sentiment analysis.exe");
            con.Close();            
        }
        else
        {
            MessageBox.Show("Enter correct username and password");
            con.Close();
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        TextBox1.Text = TextBox2.Text = "";
    }
    protected void Button4_Click(object sender, EventArgs e)
    {
        TextBox3.Text = TextBox4.Text = "";
    }
}
