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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private SqlConnection Con;

        private void ketnoi()
        {
            string cn = "Data Source=.\\sqlexpress;Initial Catalog=quanlychungchi;Integrated Security=True";
            try
            {
                Con = new SqlConnection(cn);
                Con.Open();
                MessageBox.Show("Ket noi thanh cong", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ket noi that bai", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           /* IDataAdapter = new SqlDataAdapter("select *form TTLA", Con);
            detable = new DataTable();*/
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ketnoi();
        }
    }
    
}
