using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board mBoard;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

            // set filter for file extension and default file extension 
            dlg.DefaultExt = ".docx";
            dlg.Filter = "spreadsheet (.xlsx;.csv)|*.xlsx;*.csv";
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                mBoard.LoadExaminee(dlg.FileName);
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            string cn = "Data Source=.\\testinstance;Initial Catalog=web2store;Integrated Security=True";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(cn);
                conn.Open();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "SQL connection error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO w2s_board VALUES (" + tbxDate.Text +
                "," + cbxTestType.SelectedIndex + ")";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "SQL cmd error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //StringBuilder qry = new StringBuilder();
            //qry.Append("INSERT INTO w2s_examinee VALUES (");
            //foreach (Examinee nee in mBoard.vExaminee)
            //    qry.Append(tbxDate.Text + "," + cbxTestType.SelectedIndex +
            //        "," + nee.min);
            //cmd.CommandText = qry.ToString();
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            mBoard = new Board();
        }
    }
}
