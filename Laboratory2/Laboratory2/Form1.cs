using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations.Schema;


                                                    //Data Source=ALINA-PC\SQLEXPRESS;Database=Store;Integrated Security=True
namespace Laboratory2
{
    public partial class Form1 : Form
    {
        SqlConnection cs;
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();

        SqlDataAdapter da2 = new SqlDataAdapter();
        DataSet ds2 = new DataSet();

        


        public Form1()
        {
            InitializeComponent();
            string con = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
            cs = new SqlConnection(con);

            List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));

            int count = 0;

            foreach (string column in ColumnNamesList)
            {
                count++;

                TextBox txt = new TextBox();
                txt.Name = column;
                txt.Size = new System.Drawing.Size(150, 20);
                txt.Location = new System.Drawing.Point(95, 25 * count);

                Label label = new Label();
                label.Name = column + "Label";
                label.Text = column;
                label.Location = new System.Drawing.Point(0, 25 * count);

                panel1.Controls.Add(txt);
                panel1.Controls.Add(label);
            }


        }


        private void Connect_Click(object sender, EventArgs e)
        {
            string parentTableName = ConfigurationManager.AppSettings["ParentTableName"];
            da.SelectCommand = new SqlCommand("SELECT * FROM " + parentTableName, cs);
            ds.Clear();
            da.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];


        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
                List<string> ColumnTypesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnTypes"].Split(','));
                string childTableName = ConfigurationManager.AppSettings["ChildTableName"];

                string columns = "";
                string param = "";

                foreach (string column in ColumnNamesList)
                {
                    columns += column + ",";
                    param += "@" + column + ",";
                }
                columns = columns.Remove(columns.Length - 1);
                param = param.Remove(param.Length - 1);

                string insertString = " INSERT INTO " + childTableName + "(" + columns + ") values (" + param + ")" ;
                da2.InsertCommand = new SqlCommand(insertString, cs);

                

                for (int i = 0; i < ColumnNamesList.Count; i++)
                {
                    string columnName = ColumnNamesList[i];
                    if (ColumnTypesList[i] == "int" && i !=0)
                        da2.InsertCommand.Parameters.Add("@" + columnName, SqlDbType.Int).Value = Int32.Parse(panel1.Controls[columnName].Text);
                    else
                        da2.InsertCommand.Parameters.Add("@" + columnName, SqlDbType.VarChar).Value = panel1.Controls[columnName].Text;
                }

                cs.Open();
                da2.InsertCommand.ExecuteNonQuery();
                MessageBox.Show("Added successfully!");

                cs.Close();
                ds2.Clear();
                da2.Fill(ds2);
                dataGridView1.DataSource = ds2.Tables[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                cs.Close();
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
                List<string> ColumnTypesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnTypes"].Split(','));
                string childPrimaryKey = ConfigurationManager.AppSettings["ChildPrimaryKey"];
                string childTableName = ConfigurationManager.AppSettings["ChildTableName"];

                string param = "";

                foreach (string column in ColumnNamesList)
                {
                    param += column + " = " + "@" + column + ",";
                }
                param = param.Remove(param.Length - 1);

                string updateString = "UPDATE " + childTableName + " SET " + param + " where " + childPrimaryKey + " = @" + childPrimaryKey;

                da2.UpdateCommand = new SqlCommand(updateString, cs);

                for (int i = 0; i < ColumnNamesList.Count; i++)
                {
                    string columnName = ColumnNamesList[i];
                    if (ColumnTypesList[i] == "int")
                        da2.UpdateCommand.Parameters.Add("@" + columnName, SqlDbType.Int).Value = Int32.Parse(panel1.Controls[columnName].Text);
                    else
                        da2.UpdateCommand.Parameters.Add("@" + columnName, SqlDbType.VarChar).Value = panel1.Controls[columnName].Text;
                }


                cs.Open();
                da2.UpdateCommand.ExecuteNonQuery();
                MessageBox.Show("Updated successfully!");
                cs.Close();
                ds2.Clear();
                da2.Fill(ds2);
                dataGridView1.DataSource = ds2.Tables[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                cs.Close();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            try
            {
                string childTableName = ConfigurationManager.AppSettings["ChildTableName"];
                string childPrimaryKey = ConfigurationManager.AppSettings["ChildPrimaryKey"];

                string deleteString = "DELETE FROM " + childTableName + " where " + childPrimaryKey + " = @" + childPrimaryKey;

                da2.DeleteCommand = new SqlCommand(deleteString, cs);
                da2.DeleteCommand.Parameters.Add("@" + childPrimaryKey, SqlDbType.Int).Value = Int32.Parse(panel1.Controls[childPrimaryKey].Text);
                cs.Open();
                da2.DeleteCommand.ExecuteNonQuery();
                MessageBox.Show("Deleted succesfully!");
                cs.Close();
                ds2.Clear();
                da2.Fill(ds2);
                dataGridView1.DataSource = ds2.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                cs.Close();
            }

        }

        private void Show_Click(object sender, EventArgs e)
        {
            try
            {
                string childTableName = ConfigurationManager.AppSettings["ChildTableName"];
                string childForeignKey = ConfigurationManager.AppSettings["ChildForeignKey"];

                string selectString = "SELECT * FROM " + childTableName + " where " + childTableName + "." + childForeignKey + " = @" + childForeignKey;

                da2.SelectCommand = new SqlCommand(selectString, cs);
                da2.SelectCommand.Parameters.Add("@" + childForeignKey, SqlDbType.Int).Value = Int32.Parse(panel1.Controls[childForeignKey].Text);
                ds2.Clear();
                da2.Fill(ds2);
                dataGridView1.DataSource = ds2.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }


}
