using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace Balon_zakazivanje
{
    public partial class Rezervacija : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Label1.Text = Korisnik.user_ime + " " + Korisnik.user_prezime;
                ObjekatPopulate();

                if (Korisnik.user_id == 0)
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.Date <= DateTime.Now)
            {
                e.Cell.BackColor = System.Drawing.Color.Gray;
                e.Day.IsSelectable = false;
            }
        }

        private void ObjekatPopulate()
        {
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Objekat", veza);
            DataTable dt_igraonice = new DataTable();
            adapter.Fill(dt_igraonice);
            ddl_objekat.DataSource = dt_igraonice;
            ddl_objekat.DataValueField = "id";
            ddl_objekat.DataTextField = "naziv";
            ddl_objekat.DataBind();
        }

        private void TerminiPopulate()
        {
            ddl_pocetak.Items.Clear();
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM dbo.SlobodniTermini('" + Calendar1.SelectedDate.ToString("yyyy-MM-dd") + "', " + ddl_objekat.SelectedValue + ")", veza);
            DataTable dt_termini = new DataTable();
            adapter.Fill(dt_termini);
            ddl_pocetak.DataSource = dt_termini;
            ddl_pocetak.DataValueField = "vreme";
            ddl_pocetak.DataTextField = "vreme";
            ddl_pocetak.DataBind();
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            TerminiPopulate();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Calendar1.SelectedDate == null || Calendar1.SelectedDate == Convert.ToDateTime("01/01/0001") || ddl_pocetak.SelectedIndex < 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Izaberite datum i vreme')", true);
            }
            else
            {
                SqlConnection veza = Konekcija.Connect();
                string kraj = (Convert.ToInt32(ddl_pocetak.SelectedValue) + 1).ToString();
                string naredba = "EXEC Dodaj_Rezervacija " + Korisnik.user_id + ", " + ddl_objekat.SelectedValue + ", '" + Calendar1.SelectedDate.ToString("yyyy-MM-dd") + "', " + ddl_pocetak.SelectedValue + ", " + kraj;
                SqlCommand komanda = new SqlCommand(naredba.ToString(), veza);
                
                try
                {
                    veza.Open();
                    komanda.ExecuteNonQuery();
                    veza.Close();
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Uspesno ste rezervisali termin')", true);
                    System.Threading.Thread.Sleep(1000);
                    Response.Redirect("Rezervacija.aspx");
                }
                catch (Exception greska)
                {
                    Console.WriteLine(greska.Message);
                }
            }
        }

        protected void ddl_objekat_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerminiPopulate();
        }
    }
}