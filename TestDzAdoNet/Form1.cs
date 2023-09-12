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

        private bool newRowAdding=false;
        public Form1()
        {
            InitializeComponent();
        }


        private void LoadData()
        {
            try
            {
                sqlDataAdapter=new SqlDataAdapter("select *, 'Delete' AS [Command] FROM Authors",sqlConnection );
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

        private void ReloadData()
        {
            try
            {
                dataSet.Tables["Authors"].Clear();

                sqlDataAdapter.Fill(dataSet, "Authors");

                dataGridView1.DataSource = dataSet.Tables["Authors"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[3, i] = linkCell;
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.ColumnIndex==3)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                    if(task=="Delete")
                    {
                        if(MessageBox.Show("Удалить эту строку?","Удаление",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            dataSet.Tables["Authors"].Rows[rowIndex].Delete();
                            sqlDataAdapter.Update(dataSet, "Authors");
                        }
                    }
                    else if(task=="Insert")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;
                        DataRow row = dataSet.Tables["Authors"].NewRow();

                        row["FirstName"] = dataGridView1.Rows[rowIndex].Cells["FirstName"].Value;
                        row["LastName"] = dataGridView1.Rows[rowIndex].Cells["LastName"].Value;

                        dataSet.Tables["Authors"].Rows.Add(row);
                        dataSet.Tables["Authors"].Rows.RemoveAt(dataSet.Tables["Authors"].Rows.Count-1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[3].Value = "Delete";
                        sqlDataAdapter.Update(dataSet,"Authors");
                        newRowAdding = false;
                    }
                    else if (task=="Update")
                    {
                        int r = e.RowIndex;
                        dataSet.Tables["Authors"].Rows[r]["FirstName"] = dataGridView1.Rows[r].Cells["FirstName"].Value;
                        dataSet.Tables["Authors"].Rows[r]["LastName"] = dataGridView1.Rows[r].Cells["LastName"].Value;

                        sqlDataAdapter.Update(dataSet, "Authors");
                        dataGridView1.Rows[e.RowIndex].Cells[3].Value = "Delete";
                    }
                    ReloadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if(newRowAdding==false)
                {
                    newRowAdding = true;

                    int lastRow=dataGridView1.RowCount-2;
                    DataGridViewRow row = dataGridView1.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[3, lastRow] = linkCell;
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(newRowAdding==false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow= dataGridView1.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[3, rowIndex] = linkCell;
                    editingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if(dataGridView1.CurrentCell.ColumnIndex==3)
            {
                TextBox textBox = e.Control as TextBox;

                if(textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
        }
        private void Column_KeyPress(object sender,KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
