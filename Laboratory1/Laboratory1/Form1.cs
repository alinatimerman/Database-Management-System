using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratory1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        //connect
        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection cs = new SqlConnection("Data Source=ALINA-PC\\SQLEXPRESS; Initial Catalog = Store; Integrated Security = True");
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();

            da.SelectCommand = new SqlCommand("SELECT * FROM Stores", cs);
            ds.Clear();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string connectionString = "Data Source=ALINA-PC\\SQLEXPRESS; Initial Catalog = Store; Integrated Security = True";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            DataGridViewRow dataGridViewRow = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];
            string idSelectat = dataGridViewRow.Cells[0].Value.ToString();

            using (connection)
            {
                    connection.Open();

                    string queryString = "SELECT * FROM Staff WHERE store_id=@store_id";
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@store_id", idSelectat);

                    SqlDataReader reader = command.ExecuteReader();
                    DataTable staff = new DataTable();
                    staff.Load(reader);
                    dataGridView2.DataSource = staff;

                    reader.Close();
                    connection.Close();

                    adapter = new SqlDataAdapter(queryString, connection);
         
               
               // Console.ReadLine();
            }
        }

        //add
        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection cs = new SqlConnection("Data Source=ALINA-PC\\SQLEXPRESS; Initial Catalog = Store; Integrated Security = True");
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();

            try
            {
                da.InsertCommand = new SqlCommand("INSERT INTO Staff (first_name, last_name, store_id) VALUES(@f, @l, @si)", cs);
               // da.InsertCommand.Parameters.Add("@s", SqlDbType.Int).Value = Int32.Parse(textBox1.Text);
                da.InsertCommand.Parameters.Add("@f", SqlDbType.VarChar,30).Value =textBox2.Text;
                da.InsertCommand.Parameters.Add("@l", SqlDbType.VarChar, 30).Value = textBox3.Text;
                da.InsertCommand.Parameters.Add("@si", SqlDbType.Int).Value = Int32.Parse(textBox4.Text);
                cs.Open();
                da.InsertCommand.ExecuteNonQuery();
               
                cs.Close();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                cs.Close();
            }
        }

        //update
        private void button3_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=ALINA-PC\\SQLEXPRESS; Initial Catalog = Store; Integrated Security = True";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            string queryString = "update Staff set first_name=@f, last_name=@l, store_id=@si where staff_id=@s";

            using (connection)
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@s", textBox1.Text);
                command.Parameters.AddWithValue("@f", textBox2.Text);
                command.Parameters.AddWithValue("@l", textBox3.Text);
                command.Parameters.AddWithValue("@si", textBox4.Text);
                

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                   

                    connection.Close();

                    adapter = new SqlDataAdapter(queryString, connection);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    connection.Close();
                }

            }
        }

        //delete
        private void button4_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=ALINA-PC\\SQLEXPRESS; Initial Catalog = Store; Integrated Security = True";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            string queryString = "delete from Staff where staff_id = @s";
            

            using (connection)
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@s", textBox1.Text);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();

                    adapter = new SqlDataAdapter(queryString, connection);
                    
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    connection.Close();
                }

            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string connectionString = "Data Source=ALINA-PC\\SQLEXPRESS; Initial Catalog = Store; Integrated Security = True";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            DataGridViewRow dataGridViewRow = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];
            string idSelectat = dataGridViewRow.Cells[0].Value.ToString();

            using (connection)
            {
        
                    connection.Open();

                    string queryString = "SELECT * FROM Staff WHERE store_id=@si";
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@si", idSelectat);

                    SqlDataReader reader = command.ExecuteReader();

                    DataTable staff = new DataTable();
                    staff.Load(reader);
                    dataGridView2.DataSource = staff;

                    int index = e.RowIndex;
                    DataGridViewRow selectedRow = dataGridView2.Rows[index];

                    textBox1.Text = selectedRow.Cells[0].Value.ToString();
                    textBox2.Text = selectedRow.Cells[1].Value.ToString();
                    textBox3.Text = selectedRow.Cells[2].Value.ToString();
                    textBox4.Text = selectedRow.Cells[3].Value.ToString();
                   

                    reader.Close();
                    connection.Close();

                    adapter = new SqlDataAdapter(queryString, connection);
            }
        }
    }
}
