using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
namespace DataTableToCpp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DBHelperTableAdapters.TableDescribeTableAdapter describeAdapter=new DBHelperTableAdapters.TableDescribeTableAdapter();
        private DBHelperTableAdapters.TableNameTableAdapter nameAdapter = new DBHelperTableAdapters.TableNameTableAdapter();
        private global::System.Data.Odbc.OdbcConnection conn = null;
        private string database = null;

        private List<TableBox> tableBox=new List<TableBox>();
        private int usedCount = 0;
        
        public MainWindow()
        {
            InitializeComponent();

            savePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        }

        private void findTableButton_Click(object sender, RoutedEventArgs e)
        {
            global::System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(connectionString.Text);
            nameAdapter.Connection = describeAdapter .Connection= conn;
            Regex r = new Regex("database *=[a-zA-Z0-9_]*[;|\\s]",RegexOptions.IgnoreCase);

            var m=r.Match(connectionString.Text);

            if (m.Success == false)
            {
                MessageBox.Show("连接字符串错误 无法找到参数 database");
                return;
            }

            database = (m.Value.Split('=')[1] as string).Replace(";", "").Trim();

            DBHelper.TableNameDataTable table = null;
            try
            {
                table = nameAdapter.GetTableNameByDBName(database);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("无法找到表结构");
                return;
            }

            releaseAllTableBox();

            for (int i = 0; i < table.Count; i++)
            {
                createTableBox(table[i]);
            }
        }

        private void createTableBox(DataTableToCpp.DBHelper.TableNameRow tableInfo)
        {
            TableBox b;
            if (tableBox.Count > usedCount)
            {
                b = tableBox[usedCount++];
            }
            else
            {
                b = new TableBox();
                tableBox.Add(b);
                usedCount++;
                b.EXPORT += b_EXPORT;
            }

            b.title = tableInfo.TABLE_NAME;
            b.comment = tableInfo.TABLE_COMMENT;

            var tableDescribe = describeAdapter.GetDataByTableName(database, tableInfo.TABLE_NAME);

            b.tableData = tableDescribe;


            tableGroup.Children.Add(b);
        }

        private void releaseAllTableBox()
        {
            tableGroup.Children.RemoveRange(0, usedCount);
            
            usedCount = 0;
        }



        private void antiSel_Click(object sender, RoutedEventArgs e)
        {
            //反选
            for (int i = 0; i < usedCount; i++)
            {
                tableBox[i].selected = !tableBox[i].selected;
            }
        }

        private void selectAll_Click(object sender, RoutedEventArgs e)
        {
            //全选
            for (int i = 0; i < usedCount; i++)
            {
                tableBox[i].selected = true;
            }
        }

        private Encoding getCodeType()
        {
            if ((codeType.SelectedItem as ComboBoxItem).Content.ToString() == "系统默认")
            {
                return System.Text.Encoding.Default;
            }
            else if ((codeType.SelectedItem as ComboBoxItem).Content.ToString() == "UTF-8")
            {
                return System.Text.Encoding.UTF8;
            }
            else if ((codeType.SelectedItem as ComboBoxItem).Content.ToString() == "ANSI")
            {
                return System.Text.Encoding.ASCII;
            }
            else if ((codeType.SelectedItem as ComboBoxItem).Content.ToString() == "Unicode")
            {
                return System.Text.Encoding.Unicode;
            }
            return System.Text.Encoding.Default;
        }

        private string getSavePath()
        {
            if(savePath.Text[savePath.Text.Length-1]!='\\')
                return savePath.Text+'\\';

            return savePath.Text;
        }




        void b_EXPORT(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "头文件(*.h)|*.h;";
            if (dialog.ShowDialog(this) == false)
                return;
            

            //导出单张表
            var box = sender as TableBox;

            List<List<TableDescribeVO>> table = new List<List<TableDescribeVO>>();
            table.Add(box.tableDescribe);

            string code = Helper.TableDescribeToFile(dialog.SafeFileName, table);

            Directory.CreateDirectory(dialog.FileName);
            StreamWriter s = new StreamWriter(new FileStream(dialog.FileName, FileMode.Create), getCodeType());
            s.Write(code);
            s.Close();

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if ((exportType.SelectedItem as ComboBoxItem).Content.ToString() == "多个.h文件")
            {
                for (int i = 0; i < usedCount; i++)
                {
                    if (tableBox[i].selected)
                    {
                        var describe = tableBox[i].tableDescribe;

                        List<List<TableDescribeVO>> table = new List<List<TableDescribeVO>>();
                        table.Add(describe);

                        string code = Helper.TableDescribeToFile(describe[0].TABLE_NAME+".h", table);

                        Directory.CreateDirectory(getSavePath() + expName.Text + "\\");
                        StreamWriter s = new StreamWriter(new FileStream(getSavePath() + expName.Text + "\\" + describe[0].TABLE_NAME + ".h", FileMode.Create), getCodeType());
                        s.Write(code);
                        s.Close();

                    }
                }
            }
            else if ((exportType.SelectedItem as ComboBoxItem).Content.ToString() == "单个.h文件")
            {
                List<List<TableDescribeVO>> table = new List<List<TableDescribeVO>>();

                for (int i = 0; i < usedCount; i++)
                {
                    if (tableBox[i].selected)
                    {
                        var describe = tableBox[i].tableDescribe;

                        table.Add(describe);
                    }
                }

                string code = Helper.TableDescribeToFile(expName.Text + ".h", table);

                Directory.CreateDirectory(getSavePath());
                StreamWriter s = new StreamWriter(new FileStream(getSavePath() + expName.Text + ".h", FileMode.Create), getCodeType());
                s.Write(code);
                s.Close();
            }

        }

        
    }
}
