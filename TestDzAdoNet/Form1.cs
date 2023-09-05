using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TestDzAdoNet
{
    public partial class Form1 : Form
    {

        private SqlConnection sqlConnection=null;
        private SqlCommandBuilder sqlBuilder=null;
        private SqlDataAdapter sqlDataAdapter=null;
        private DataSet dataSet=null;
        public Form1()
        {
            InitializeComponent();
        }


        private void LoadData()
        {
            try
            {
                sqlDataAdapter=new SqlDataAdapter("select *, 'Delete' AS [Delete] FROM Authors",sqlConnection );
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();

                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "Authors");

                dataGridView1.DataSource= dataSet.Tables["Authors"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[3,i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection=new SqlConnection(@"Data Source=DESKTOP-UND2H5H\SQLEXPRESS;Initial Catalog=LibraryDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
            sqlConnection.Open();

            LoadData();
        }
    }
}
