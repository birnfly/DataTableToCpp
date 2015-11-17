using System;
using System.Collections.Generic;
using System.Data;
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

namespace DataTableToCpp
{
    /// <summary>
    /// TableBox.xaml 的交互逻辑
    /// </summary>
    public partial class TableBox : UserControl
    {
        public event EventHandler EXPORT;

        public string title
        {
            get {return titleLable.Content as string;}
            set { titleLable.Content = value;}
        }

        public string comment
        {
            get { return titleLable.ToolTip as string; }
            set { titleLable.ToolTip = value; }
        }

        public bool selected
        {
            get { return selectedBox.IsChecked??false; }
            set { selectedBox.IsChecked = value; }

        }

        public object tableData
        {
            get { return dataGrid.DataContext; }
            set { dataGrid.DataContext = value; }
        }

        public List<TableDescribeVO> tableDescribe
        {
            get
            {
                return Helper.DateTableToList<TableDescribeVO>(dataGrid.DataContext as DataTable);
            }
        }

        public TableBox()
        {
            InitializeComponent();
        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            if (EXPORT != null)
                EXPORT(this, e);
        }
    }
}
