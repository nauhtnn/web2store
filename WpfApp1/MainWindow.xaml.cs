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
       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult h = System.Windows.Forms.MessageBox.Show("Bạn có muốn thoát không! ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (h == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
                System.Windows.Forms.Application.Exit();
            }
            else
            {
               e.Handled = true;
            }
            
        }
       
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

           
           System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

            // set filter for file extension and default file extension 
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "spreadsheet (.xlsx;.csv)|*.xlsx;*.csv";
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            System.Windows.Forms.MessageBox.Show("Lấy dữ liệu thành công",
                "Thông báo",MessageBoxButtons.OK) ;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string s = mBoard.Load(dlg.FileName);
                if(s != "ok")
                    System.Windows.MessageBox.Show(s, "SQL cmd error");
                GridShowExaminee(grd2);
            }
            
        }

        SqlConnection GetDBConnection()
        {
            string cn = "Server=.\\SQLEXPRESS;Database=quanlychungchi;Integrated Security=True";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(cn);
                conn.Open();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "SQL connection error");
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
                System.Windows.MessageBox.Show( "Nhập dữ liệu thành công!","Thông báo",MessageBoxButton.OK);
            }
            catch(SqlException ex)
            {
                System.Windows.MessageBox.Show("Dữ liệu này đã được lấy!", "Thông báo", MessageBoxButton.OK);
                return;
            }
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            mBoard = new Board();
            btn_ngaythi.IsEnabled = false;
            btn_hoten.IsEnabled = false;
            btn_ho.IsEnabled = false;
            btn_ten.IsEnabled = false;

        }

        private void SearchName(string qry)
        {
            grd1.Children.Clear();
            mBoard.vExaminee.Clear();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = GetDBConnection();
            if (cmd.Connection == null)
                return;
            cmd.CommandText = qry;
            SqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "SQL cmd error");
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

        //approximate name
        private void btn_ApproxName(object sender, RoutedEventArgs e)
        {
            SearchName("SELECT * FROM w2s_examinee WHERE name LIKE N'%" + TextBox1.Text + "%'");//todo
        }

        //exact name-- tim theo ho ten chinh xac --- ok
        private void btn_ExactName(object sender, RoutedEventArgs e)
        {
            SearchName("SELECT * FROM w2s_examinee WHERE name = N'" + TextBox1.Text + "'");
        }

        //prefix name-- tìm theo tên chính xác-- OK
        private void btn_PrefixName(object sender, RoutedEventArgs e)
        {
            SearchName("SELECT * FROM w2s_examinee WHERE name LIKE N'%" + TextBox1.Text + "'");
        }

        //postfix name
        private void btn_SuffixName(object sender, RoutedEventArgs e)
        {
            SearchName("SELECT * FROM w2s_examinee WHERE name LIKE N'" + TextBox1.Text + "%'");
        }

        //test date-- tim theo ngày thi-- OK
        private void btn_TestDate(object sender, RoutedEventArgs e)
        {
            SearchName("SELECT * FROM w2s_examinee WHERE test_date='" + Combobox1.Text + "'");
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
                //examinee ID
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

                //birthdate
                t = new TextBlock();
                t.Text = nee.mBirthdate.ToString(DT.RF);
                grd.Children.Add(t);
                Grid.SetRow(t, r);
                Grid.SetColumn(t, 2);
                TextBox1.Text = nee.mName;

                //birthplace
                t = new TextBlock();
                t.Text = nee.mBirthplace;
                grd.Children.Add(t);
                Grid.SetRow(t, r);
                Grid.SetColumn(t, 3);

                //grade1
                t = new TextBlock();
                t.Text = nee.mGrade1.ToString();
                grd.Children.Add(t);
                Grid.SetRow(t, r);
                Grid.SetColumn(t, 4);

                //
                ++r;
            }
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult h = System.Windows.Forms.MessageBox.Show("Bạn có muốn thoát không! ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (h == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                e.Handled = true;
            }
        }


        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            btn_ngaythi.IsEnabled = false;
            btn_hoten.IsEnabled = true;
            btn_ho.IsEnabled = true;
            btn_ten.IsEnabled = true;
            
            
        }
        private void Combobox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_hoten.IsEnabled = false;
            btn_ho.IsEnabled = false;
            btn_ten.IsEnabled = false;
            btn_ngaythi.IsEnabled = true;
            

        }
    }
}
