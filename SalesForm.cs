using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PointOfSale
{
    public partial class SalesForm : Form
    {
        public static DataTable da = new DataTable();
        DataRow dy;
        private static List<Stream> m_streams;
        private static int m_currentPageIndex = 0;
        public SalesForm()
        {
            InitializeComponent();
        }

        private void SalesForm_Load(object sender, EventArgs e)
        {
            LoadFromSale();
            GetPharmCategory();
            GetPharmCategory2();
            GetPharmCategory3();
            label11.Text = Home.adama;
            GetPharmiCategory();
            bindApp();
            cmbExpTrans.SelectedIndex = 0;
            cmbTransBy.SelectedIndex = 0;

        }
        private void LoadFromSale()
        {
            DataTable dt = getProductName();
            AutoCompleteStringCollection aut = new AutoCompleteStringCollection();
            foreach(DataRow dr in dt.Rows)
            {
                aut.Add(dr[0].ToString());
            }
            textBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox1.AutoCompleteCustomSource = aut;
        }
        public DataTable getProductName()
        {

            string applet = "SELECT ProductName, SalesPrice FROM Products ORDER BY ProductName";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = applet;
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            con.Close();
            cmd.Dispose();
            return dt;
        }
        public void GetPharmCategory()
        {
            comboBox1.Items.Clear();
            string app = "SELECT category FROM Category";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string category = (string)dr["category"];
                comboBox1.Items.Add(category);
            }
            dr.Close();
            con.Close();
            cmd.Dispose();
        }
        public void GetPharmCategory3()
        {
            comboBox2.Items.Clear();
            string app = "SELECT DISTINCT Category FROM Products";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string category = (string)dr["category"];
                comboBox2.Items.Add(category);
            }
            dr.Close();
            con.Close();
            cmd.Dispose();
        }
        public void GetPharmCategory2()
        {
            listBox1.Items.Clear();
            string app = "SELECT DISTINCT ProductName FROM Products";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string category = (string)dr["ProductName"];
                listBox1.Items.Add(category);
            }
            dr.Close();
            con.Close();
            cmd.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Application.StartupPath + "\\Report2.rdlc";
            localReport.DataSources.Add(new ReportDataSource("DataSet2", da));
            PrintToPrinter(localReport);
              foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                
        int app = insertSales(row.Cells[0].Value.ToString(), int.Parse(row.Cells[1].Value.ToString()), int.Parse(row.Cells[2].Value.ToString()), int.Parse(row.Cells[3].Value.ToString()), int.Parse(row.Cells[4].Value.ToString()), row.Cells[5].Value.ToString(), int.Parse(row.Cells[6].Value.ToString()), int.Parse(row.Cells[7].Value.ToString()));
                    int ant = insertSalesUpdate(row.Cells[0].Value.ToString(), int.Parse(row.Cells[3].Value.ToString()), int.Parse(row.Cells[1].Value.ToString()), int.Parse(row.Cells[2].Value.ToString()), int.Parse(row.Cells[6].Value.ToString()), int.Parse(row.Cells[7].Value.ToString()));
                    UpdateProducts(row.Cells[0].Value.ToString(), int.Parse(row.Cells[3].Value.ToString()));
                }
                label10.Visible = true;
                label10.Text ="Sales Recorded";
                da.Rows.Clear();
                dataGridView2.DataSource = "";
        }
        public int insertSales(string ProductName, int PurchasePrice, int SalesPrice, int Qty, int BargainPrice, string TransBy, int NetSales, int Discount)
        {
            string app = "INSERT INTO SoldItems(ProductName, PurchasePrice, SalesPrice, Qty, SalesDate, BargainPrice, TransactionBy, NetSales, Discount)VALUES(@ProductName, @PurchasePrice,@SalesPrice, @Qty, GETDATE(), @BargainPrice, @TransactionBy, @NetSales, @Discount)";
            SqlConnection con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            cmd.Parameters.AddWithValue("@ProductName", ProductName);
            cmd.Parameters.AddWithValue("@PurchasePrice", PurchasePrice);
            cmd.Parameters.AddWithValue("@SalesPrice", SalesPrice);
            cmd.Parameters.AddWithValue("@Qty", Qty);
            cmd.Parameters.AddWithValue("@BargainPrice", BargainPrice);
            cmd.Parameters.AddWithValue("@TransactionBy", TransBy);
            cmd.Parameters.AddWithValue("@NetSales", NetSales);
            cmd.Parameters.AddWithValue("@Discount", Discount);
            int row = cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
            return row;

        }
        public void UpdateProducts(string ProductName, int Qty)
        {
            string app = "UPDATE [Products] SET Qty = Qty - @Qty WHERE ProductName = @ProductName";
            SqlConnection con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            cmd.Parameters.AddWithValue("@ProductName", ProductName);
            cmd.Parameters.AddWithValue("@Qty", Qty);
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
        }
       
        public int insertSalesUpdate(string ProductName, int QtySold, int PurchasePrice, int SalesPrice, int NetSales, int Discount)
        {
            string app = "BEGIN IF EXISTS(SELECT QtySold FROM SalesUpdate WHERE ProductName = @ProductName) BEGIN UPDATE[SalesUpdate] SET QtySold = QtySold + @QtySold, LastSoldDate = GETDATE(), PurchasePrice = PurchasePrice + @PurchasePrice, SalesPrice = SalesPrice + @SalesPrice, NetSales = NetSales + @NetSales, Discount = Discount + @Discount WHERE ProductName = @ProductName END ELSE BEGIN INSERT INTO SalesUpdate(ProductName, QtySold, LastSoldDate,PurchasePrice,SalesPrice, NetSales, Discount)VALUES(@ProductName, @QtySold, GETDATE(), @PurchasePrice,@SalesPrice, @NetSales, @Discount) END END";
            SqlConnection con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            cmd.Parameters.AddWithValue("@ProductName", ProductName);
            cmd.Parameters.AddWithValue("@QtySold", QtySold);
            cmd.Parameters.AddWithValue("@PurchasePrice", PurchasePrice);
            cmd.Parameters.AddWithValue("@SalesPrice", SalesPrice);
            cmd.Parameters.AddWithValue("@NetSales", NetSales);
            cmd.Parameters.AddWithValue("@Discount", Discount);
            int row = cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
            return row;

        }

        public DataTable getProductDetails(string ProductName)
        {

            string applet = "SELECT ProductName,Category, SalesPrice, Qty As QtyInStock, ISNULL(PurchasePrice, 0) AS CP FROM Products WHERE ProductName = @ProductName";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = applet;
            cmd.Parameters.AddWithValue("@ProductName", ProductName);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("rex");
            dt.Load(dr);
            dr.Close();
            con.Close();
            cmd.Dispose();
            return dt;
        }
        DataTable dt;


        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text != string.Empty)
            {
                int act = int.Parse(textBox3.Text);
                int apt = int.Parse(textBox4.Text);
                int CPrice = int.Parse(textBox6.Text);
                int ann = int.Parse(textBox8.Text);
                int actual = act * apt;
                int uni = act * ann;
                int actualCP = act * CPrice;
                textBox5.Text = actual.ToString();
                textBox6.Text = actualCP.ToString();
                textBox9.Text = uni.ToString();
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.RemoveAt(row.Index);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                dt = getProductDetails(listBox1.Text);
                
                foreach (DataRow row in dt.Rows)
                {
                    int app = int.Parse(row[3].ToString());
                    if (app > 0)
                    {
                        textBox2.Text = row[0].ToString();
                        textBox4.Text = row[2].ToString();
                        textBox8.Text = row[2].ToString();
                        textBox6.Text = row[4].ToString();
                        int act = int.Parse(textBox3.Text);
                        int apt = int.Parse(textBox4.Text);
                        int actual = act * apt;
                        textBox5.Text = actual.ToString();
                        textBox10.Text = "0";
                    }
                    else
                    {
                        MessageBox.Show("Items has finished in store", "Empy store Alert");
                    }
                }
                dataGridView2.DataSource = dt;
                dataGridView2.Columns[0].Width = 200;
                dataGridView2.Columns[2].Width = 90;
                dataGridView2.Columns[1].Width = 150;
                dataGridView2.Columns[3].Width = 90;
                dataGridView2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {   
             if (da.Columns.Count > 0)

                {
                    dy = da.NewRow();
                    dy["ProductName"] = this.Product;
                    dy["UnitPrice"] = this.Unit;
                    dy["SalesPrice"] = this.Price;
                    dy["Qty"] = this.QtySold;
                    dy["BargainPrice"] = this.BargainPrice;
                dy["TransactionBy"] = this.TransactionBy;
                dy["NetSales"] = this.NetSales;
                dy["Discount"] = this.Discount;
                dy["Attendant"] = this.Attendant;
                da.Rows.Add(dy);
                    dataGridView1.DataSource = da;
            }
            else
            {
                da.Columns.Add("ProductName", typeof(string));
                da.Columns.Add("UnitPrice", typeof(int));
                da.Columns.Add("SalesPrice", typeof(int));
                da.Columns.Add("Qty", typeof(int));
                da.Columns.Add("BargainPrice", typeof(int));
                da.Columns.Add("TransactionBy", typeof(string));
                da.Columns.Add("NetSales", typeof(int));
                da.Columns.Add("Discount", typeof(int));
                da.Columns.Add("Attendant", typeof(string));
                dy = da.NewRow();
                dy["ProductName"] = this.Product;
                dy["UnitPrice"] = this.Unit;
                dy["SalesPrice"] = this.Price;
                dy["Qty"] = this.QtySold;
                dy["BargainPrice"] = this.BargainPrice;
                dy["TransactionBy"] = this.TransactionBy;
                dy["NetSales"] = this.NetSales;
                dy["Discount"] = this.Discount;
                dy["Attendant"] = this.Attendant;
                da.Rows.Add(dy);
                dataGridView1.DataSource = da;
                }
            var sum = da.Compute("SUM(SalesPrice)", string.Empty);
            textBox7.Text = sum.ToString();
            //textBox3.Text = "1";
            
        }

        public string Product
        {
            get { return this.textBox2.Text; }
            set { this.textBox2.Text = value; }
        }
        public string Price
        {
            get { return this.textBox5.Text; }
            set { this.textBox5.Text = value; }
        }
        public string Discount
        {
            get { return this.textBox10.Text; }
            set { this.textBox10.Text = value; }
        }
        public string BargainPrice
        {
            get { return this.textBox8.Text; }
            set { this.textBox8.Text = value; }
        }
        public string TransactionBy
        {
            get { return this.cmbTransBy.Text; }
            set { this.cmbTransBy.Text = value; }
        }
        public string NetSales
        {
            get { return this.textBox9.Text; }
            set { this.textBox9.Text = value; }
        }
        public string QtySold
        {
            get { return this.textBox3.Text; }
            set { this.textBox3.Text = value; }
        }
        public string Unit
        {
            get { return this.textBox4.Text; }
            set { this.textBox4.Text = value; }
        }
        public string Attendant
        {
            get { return this.label11.Text; }
            set { this.label11.Text = value; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (da.Columns.Count == 0)
            {
                MessageBox.Show("Nothing to Clear");
            }
            else
            {
                da.Rows.Clear();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                dt = getProductDetails(textBox1.Text);
                foreach (DataRow row in dt.Rows)
                {
                    int app = int.Parse(row[3].ToString());
                    if (app > 0)
                    {
                        textBox2.Text = row[0].ToString();
                        textBox4.Text = row[2].ToString();
                        textBox8.Text = row[2].ToString();
                        textBox6.Text = row[4].ToString();
                        int act = int.Parse(textBox3.Text);
                        int apt = int.Parse(textBox4.Text);
                        int actual = act * apt;
                        textBox5.Text = actual.ToString();
                        textBox10.Text = "0";
                    }
                    else
                    {
                        MessageBox.Show("Items has finished in store", "Empy store Alert");
                    }
                }
                dataGridView2.DataSource = dt;
                dataGridView2.Columns[0].Width = 200;
                
                dataGridView2.Columns[2].Width = 90;
                dataGridView2.Columns[1].Width = 150;
                dataGridView2.Columns[3].Width = 90;
                dataGridView2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
            }
        }
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            dt = getCategoryDetails(comboBox1.Text);
            dataGridView2.DataSource = dt;
            dataGridView2.Columns[0].Width = 250;
            dataGridView2.Columns[1].Width = 150;
            dataGridView2.Columns[2].Width = 90;
            dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGreen;
            dataGridView2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
        }
        public DataTable getCategoryDetails(string Category)
        {

            string applet = "SELECT ProductName,Category, SalesPrice, Qty FROM Products WHERE Category = @Category";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = applet;
            cmd.Parameters.AddWithValue("@Category", Category);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Products");
            dt.Load(dr);
            dr.Close();
            con.Close();
            cmd.Dispose();
            return dt;
        }
        public DataTable getAll()
        {

            string applet = "SELECT [ProductName],[Category],[SalesPrice],[Qty] FROM Products";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = applet;
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Products");
            dt.Load(dr);
            dr.Close();
            con.Close();
            cmd.Dispose();
            return dt;
        }

        public DataTable getTypeDetails(string Type)
        {

            string applet = "SELECT ProductName, Price, Qty FROM Products WHERE Type = @Type";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = applet;
            cmd.Parameters.AddWithValue("@Type", Type);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Products");
            dt.Load(dr);
            dr.Close();
            con.Close();
            cmd.Dispose();
            return dt;
        }
       /* private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt = getTypeDetails(comboBox2.Text);
            dataGridView2.DataSource = dt;
            dataGridView2.Columns[0].Width = 350;
            dataGridView2.Columns[1].Width = 90;
            dataGridView2.Columns[2].Width = 50;
            dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGreen;
            dataGridView2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
        }*/

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dt = getAll();
            dataGridView2.DataSource = dt;
            dataGridView2.Columns[0].Width = 200;
            dataGridView2.Columns[1].Width = 150;
            dataGridView2.Columns[2].Width = 90;
            dataGridView2.Columns[3].Width = 50;
            dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGreen;
            dataGridView2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
        }
        
        public static void PrintToPrinter(LocalReport report)
        {
            Export(report);

        }

        public static void Export(LocalReport report, bool print = true)
        {
            string deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>3in</PageWidth>
                <PageHeight>8.3in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0.1in</MarginLeft>
                <MarginRight>0.1in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;

            if (print)
            {
                Print();
            }
        }


        public static void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }

        public static Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

          public static void DisposePrint()
           {
               if (m_streams != null)
               {
                   foreach (Stream stream in m_streams)
                       stream.Close();
                   m_streams = null;
               }
           }

        private void ExpSubmit_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(txtExpAmt.Text, @"^[0-9]+$"))
            {

                int app = insertExpenses(Convert.ToDateTime(ExpTimePicker1.Text), txtExpType.Text, int.Parse(txtExpAmt.Text), cmbExpTrans.Text);
                if (app > 0)
                {
                    bindApp();
                }
                else
                {
                    MessageBox.Show("DATA UPLOAD ERROR");
                }
            }
            else
            {
                txtExpAmt.Text = "";
                MessageBox.Show("MUST ENTER NUMERIC VALUE");
            }
        }
        public void bindApp()
        {
            DataTable dt = SelectExpenses();
            ExpdataGridView.DataSource = dt;
            ExpdataGridView.Font = new Font("Georgia", 10);
            ExpdataGridView.Columns[0].Width = 200;
            ExpdataGridView.Columns[1].Width = 150;
            ExpdataGridView.Columns[2].Width = 150;
            ExpdataGridView.Columns[3].Width = 170;
            ExpdataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.LightBlue;
            ExpdataGridView.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
            ExpdataGridView.Columns[4].Visible = false;
        }
        public DataTable SelectExpenses()
        {

            string applet = "SELECT * FROM Expenses";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = applet;
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Expenses");
            dt.Load(dr);
            dr.Close();
            con.Close();
            cmd.Dispose();
            return dt;
        }
        public int insertExpenses(DateTime ExpensesDate, string ExpensesType, int Amount, string TransactionBy)
        {

            int id;
            string app = "INSERT INTO Expenses(ExpensesDate,ExpensesType,Amount,TransactionBy)VALUES(@ExpensesDate,@ExpensesType,@Amount,@TransactionBy) SELECT SCOPE_IDENTITY()";
            var con = new SqlConnection(conState.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            cmd.Parameters.AddWithValue("@ExpensesDate", ExpensesDate);
            cmd.Parameters.AddWithValue("@ExpensesType", ExpensesType);
            cmd.Parameters.AddWithValue("@Amount", Amount);
            cmd.Parameters.AddWithValue("@TransactionBy", TransactionBy);
            id = cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
            return id;
        }

        private void ExpUpdate_Click(object sender, EventArgs e)
        {
            if (txtExpType.Text == string.Empty || txtExpAmt.Text == string.Empty)
            {
                MessageBox.Show("Invalid type and amount", "Expenditure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int app = updExp(int.Parse(label5.Text), txtExpType.Text, int.Parse(txtExpAmt.Text), cmbExpTrans.Text);
                string message = string.Empty;
                switch (app)
                {
                    case 1:
                        bindApp();
                        txtExpType.Text = "";
                        txtExpAmt.Text = "";
                        cmbExpTrans.Text = "";
                        break;
                    default:
                        message = "RECORD NOT UPDATED SUCCESFULLY\\nPlease try again";
                        MessageBox.Show(message, "Expenditure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

            }
        }
        public int updExp(int Id, string ExpensesType, int Amount, string TransactionBy)
        {
            int id;
            string app = "UPDATE[Expenses] SET[ExpensesType] = @ExpensesType,[Amount] = @Amount, [TransactionBy] = @TransactionBy WHERE Id = @Id";
            var con = new SqlConnection(conState.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = app;
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Parameters.AddWithValue("@ExpensesType", ExpensesType);
            cmd.Parameters.AddWithValue("@Amount", Amount);
            cmd.Parameters.AddWithValue("@TransactionBy", TransactionBy);
            id = cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
            return id;
        }

        private void ExpClear_Click(object sender, EventArgs e)
        {
            txtExpType.Text = "";
            txtExpAmt.Text = "";
            cmbExpTrans.Text = "";
        }

        private void ExpDelete_Click(object sender, EventArgs e)
        {
            if (txtExpType.Text == string.Empty || txtExpAmt.Text == string.Empty)
            {
                MessageBox.Show("Nothing to delete", "Contact", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int app = deleteLogin(int.Parse(label5.Text));
                if (app > 0)
                {
                    MessageBox.Show("RECORD REMOVED!!!", "Contact", MessageBoxButtons.OK);
                    txtExpType.Text = "";
                    txtExpAmt.Text = "";
                    cmbExpTrans.Text = "";
                    bindApp();
                }
                else
                {
                    MessageBox.Show("Record Not Deleted", "Contact", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bindApp();
                }
            }
        }
        public int deleteLogin(int Id)
        {
            string appleting = "DELETE FROM [Expenses] WHERE Id = @id";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = appleting;
            cmd.Parameters.AddWithValue("@Id", Id);
            int row = cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
            return row;
        }
        public void GetPharmiCategory()
        {
            cmbExpTrans.Items.Clear();
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
                cmbExpTrans.Items.Add(category);

            }
            dr.Close();
            con.Close();
            dr.Close();
            cmd.Dispose();

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataTable dt = getExpenses(Convert.ToDateTime(ExpTimePicker1.Text));
            ExpdataGridView.DataSource = dt;
        }
        public DataTable getExpenses(DateTime ExpensesDate)
        {

            string applet = "SELECT * FROM Expenses WHERE ExpensesDate = @ExpensesDate";
            var con = new SqlConnection(conState.ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = applet;
            cmd.Parameters.AddWithValue("@ExpensesDate", ExpensesDate);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable("Expenses");
            dt.Load(dr);
            con.Close();
            cmd.Dispose();
            return dt;
        }

        private void ExpdataGridView2_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ExpTimePicker1.Text = ExpdataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtExpType.Text = ExpdataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtExpAmt.Text = ExpdataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            cmbExpTrans.Text = ExpdataGridView.Rows[e.RowIndex].Cells[3].Value.ToString();
            label5.Text = ExpdataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (textBox8.Text != string.Empty)
            {
                int Bprice = int.Parse(textBox8.Text);
                int qty = int.Parse(textBox3.Text);
                int SPrice = int.Parse(textBox5.Text);
                int SoldPrice = Bprice * qty;
                textBox9.Text = SoldPrice.ToString();
                int discount = SPrice - SoldPrice;
                textBox10.Text = discount.ToString();
            }
        }

        private void dataGridView2_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            textBox2.Text = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
            textBox4.Text = dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString();
            textBox8.Text = dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString();
            textBox6.Text = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
           int act = int.Parse(textBox3.Text);
           int apt = int.Parse(textBox4.Text);
           int actual = act * apt;
            textBox5.Text = actual.ToString();
            textBox10.Text = "0";
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            Home ns = new Home();
            ns.Show();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt = getCategoryDetails(comboBox2.Text);
            dataGridView2.DataSource = dt;
            dataGridView2.Columns[0].Width = 250;
            dataGridView2.Columns[1].Width = 150;
            dataGridView2.Columns[2].Width = 90;
            dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGreen;
            dataGridView2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
        }
    }
}