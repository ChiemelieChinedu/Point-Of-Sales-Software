using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.ServiceProcess;

namespace PointOfSale
{
    public partial class Home : Form
    {
        public static string adama;
        public string roles;
        int timeLeft;
        public Home()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random rand = new Random();
            int one = rand.Next(0, 255);
            int tone = rand.Next(0, 255);
            int fone = rand.Next(0, 255);
            int sone = rand.Next(0, 255);
            label1.ForeColor = Color.FromArgb(one, tone, fone, sone);
        }
        public void GetPharmCategory()
        {
            comboBox1.Items.Clear();
            string app = "SELECT Fullname FROM Users";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string category = (string)dr["Fullname"];
                comboBox1.Items.Add(category);

            }
            dr.Close();
            con.Close();
            dr.Close();
            cmd.Dispose();

        }
        private void Home_Load(object sender, EventArgs e)
        {
            bool app = IsServerConnected();
            if (app == true)
            {
                timer1.Start();
                timer1.Enabled = true;
                GetPharmCategory();
                comboBox1.SelectedIndex = 0;
                textBox2.UseSystemPasswordChar = true;
            }
            else
            {
                string strService = "SQL Server (SQLEXPRESS)";
                ServiceController serv = new ServiceController(strService);
                if (serv != null)
                {
                    if (serv.Status == ServiceControllerStatus.Stopped)
                    {
                        
                        try
                        {
                            serv.Start();
                            serv.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 10));
                            GetPharmCategory();
                            comboBox1.SelectedIndex = 0;
                            textBox2.UseSystemPasswordChar = true;
                        }
                        catch
                        {
                            MessageBox.Show("RIGHT CLICK ON WINDOWS DESKTOP ICON AND SELECT RUN...IN THE RUN OPEN TEXTBOX ENTER 'services.msc' AND CLICK OK....... IN THE LIST OF SERVICES LOOK FOR 'SQL Server (SQLEXPRESS)' AND START SERVICE FROM MENU", "LOGIN ERROR MESSAGE", MessageBoxButtons.OK);
                        }
                    }
                }

            }
        }
        public bool IsServerConnected()
        {
            using (var con = new SqlConnection(conState.ConnectionString))
            {
                try
                {
                    con.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }
        int attempt = 3;
        private void button1_Click(object sender, EventArgs e)
        {
            
            if ((comboBox1.Text.Trim() != "") && (textBox2.Text.Trim() != "")) // to validate if user and pass have data

            {
                string que = "SELECT Status FROM Users WHERE Fullname = @Fullname AND Password = @Password";
                var con = new SqlConnection(conState.ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(que, con);
                cmd.Parameters.AddWithValue("@Fullname", comboBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", textBox2.Text.Trim());
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    roles = reader["Status"].ToString();
                    attempt = 0;
                    if (roles == "Admin")
                    {
                        adama = comboBox1.Text;
                        Admin cu = new Admin();
                        cu.Show();
                        this.Hide();
                    }
                    else if (roles == "Staff")
                    {
                        adama = comboBox1.Text;
                        SalesForm fm = new SalesForm();
                        fm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Prohibited", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if ((attempt == 3) && (attempt > 0))
                {
                    label4.Visible = true;
                    label4.Text = ("You Have Only " + Convert.ToString(attempt) + " Attempt Left To Try");
                    --attempt;
                }
                else if ((attempt == 2) && (attempt > 0))
                {
                    label4.Text = ("You Have Only " + Convert.ToString(attempt) + " Attempt Left To Try");
                    --attempt;
                }
                else if ((attempt == 1) && (attempt > 0))
                {
                    label4.Text = ("You Have Only " + Convert.ToString(attempt) + " Attempt Left To Try");
                    --attempt;
                }
                else
                {
                    label4.Text = ("ACCESS DENIED!!! Attempt AFTER 3 Mins");
                    button1.Enabled = false;
                    label5.Visible = true;
                    timeLabel.Visible = true;
                    timeLeft = 120;
                    timeLabel.Text = "3mins";
                    timer2.Start();

                }
            }
            else
            {
                MessageBox.Show("Enter username and password", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft = timeLeft - 1;
                timeLabel.Text = timeLeft + "Secs";
            }
            else
            {
                timer2.Stop();
                timeLabel.Text = "Ready!!!";
                attempt = 3;
                button1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
               textBox2.UseSystemPasswordChar = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(Control.IsKeyLocked(Keys.CapsLock))
            {
                label4.Text = "The Caps Lock Key is ON.";
            }
        }
    }
}
