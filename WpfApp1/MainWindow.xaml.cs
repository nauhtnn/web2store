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


        //Tim theo họ
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            grd1.Children.Clear();
            mBoard.vExaminee.Clear();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = GetDBConnection();
            if (cmd.Connection == null)
                return;
            cmd.CommandText = "SELECT * FROM w2s_examinee WHERE name LIKE '%" + TextBox1.Text + "'";
            SqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "SQL cmd error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    Examinee nee = new Examinee();
                    int i = 1;
                    nee.mIndex = reader.GetInt16(++i);
                    nee.mName = reader.GetString(++i);
                    ++i;
                    //nee.mBirthdate = reader.GetString(++i);
                    nee.mBirthplace = reader.GetString(++i);
                    //nee.mGrade1 = reader.GetFloat(++i);
                    mBoard.vExaminee.Add(nee);
                }
                GridShowExaminee(grd1);
            }
        }
       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Bạn có chắc muốn thoát không?",
                "THÔNG BÁO", MessageBoxButtons.YesNoCancel);
            System.Windows.Application.Current.Shutdown();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            
           System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

            // set filter for file extension and default file extension 
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "spreadsheet (.xlsx;.csv)|*.xlsx;*.csv";
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                mBoard.Load(dlg.FileName);
                GridShowExaminee(grd2);
            }
        }

        SqlConnection GetDBConnection()
        {
            string cn = "Data Source=.\\TESTINSTANCE;Initial Catalog=web2store;Integrated Security=True";
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
                conn = null;
            }
            return conn;
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = GetDBConnection();
            if (cmd.Connection == null)
                return;
            cmd.CommandText = mBoard.InsertQry();
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
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            mBoard = new Board();
        }

        //tim theo ten gan dung
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            grd1.Children.Clear();
            mBoard.vExaminee.Clear();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = GetDBConnection();
            if (cmd.Connection == null)
                return;
            cmd.CommandText = "SELECT * FROM w2s_examinee WHERE name LIKE N'%" + TextBox1.Text + "'";
            SqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "SQL cmd error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    Examinee nee = new Examinee();
                    int i = 1;
                    nee.mIndex = reader.GetInt16(++i);
                    nee.mName = reader.GetString(++i);
                    nee.mBirthdate = reader.GetDateTime(++i);
                    nee.mBirthplace = reader.GetString(++i);
                    nee.mGrade1 = (float)reader.GetDouble(++i);
                    mBoard.vExaminee.Add(nee);
                }
                GridShowExaminee(grd1);
            }
        }

        void GridShowExaminee(Grid grd)
        {
            grd.RowDefinitions.Clear();
            grd.Children.Clear();
            int r = 0;
            foreach(Examinee nee in mBoard.vExaminee)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(20);
                grd.RowDefinitions.Add(rd);
                //SBD
                TextBlock t = new TextBlock();
                t.Text = nee.mIndex.ToString();
                grd.Children.Add(t);
                Grid.SetRow(t, r);
                Grid.SetColumn(t, 0);

                //Name
                t = new TextBlock();
                t.Text = nee.mName;
                grd.Children.Add(t);
                Grid.SetRow(t, r);
                Grid.SetColumn(t, 1);
                TextBox1.Text = nee.mName;

                //birthplace
                t = new TextBlock();
                t.Text = nee.mBirthplace;
                grd.Children.Add(t);
                Grid.SetRow(t, r);
                Grid.SetColumn(t, 3);

                //
                ++r;
            }
        }
        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
           


        }

        //tim theo ten chinh xac
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            grd1.Children.Clear();
            mBoard.vExaminee.Clear();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = GetDBConnection();
            if (cmd.Connection == null)
                return;
            cmd.CommandText = "SELECT * FROM w2s_examinee WHERE name LIKE '%" + TextBox1.Text + "'";
            SqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (SqlException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "SQL cmd error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    Examinee nee = new Examinee();
                    int i = 1;
                    nee.mIndex = reader.GetInt16(++i);
                    nee.mName = reader.GetString(++i);
                    ++i;
                    //nee.mBirthdate = reader.GetString(++i);
                    nee.mBirthplace = reader.GetString(++i);
                    //nee.mGrade1 = reader.GetFloat(++i);
                    mBoard.vExaminee.Add(nee);
                }
                GridShowExaminee(grd1);
            }
        }
    }
    
}
