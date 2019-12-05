using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace CodeGenerator
{
    /// <summary>
    /// CreateModel.xaml 的交互逻辑
    /// </summary>
    public partial class CreateModel : Window
    {
        private List<FieldInfo> _fields = new List<FieldInfo>();
        public List<string> fieldDateType = new List<string> { "string", "int", "float", "date", "bool" };
        private CodeGenerateConfig _mainWindow;
        public bool _shouldClose;
        public CreateModel(CodeGenerateConfig mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            dgrid_Fields.ItemsSource = _fields;
            cmb_DataType.ItemsSource = fieldDateType;
            this.Closing += OnClosing;
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            this.Hide();
            if (!_shouldClose)
            {
                e.Cancel = true;
            }
        }
        private void btn_CreateModel_Add_Click(object sender, RoutedEventArgs e)
        {
            _fields.Add(new FieldInfo
            {
                Name = txt_Name.Text,
                Description = txt_Description.Text,
                DataType = cmb_DataType.SelectedValue.ToString(),
                IsRequied = chk_IsRequied.IsChecked ?? false,
                MaxLength = int.Parse(txt_MaxLength.Text)
            });
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    dgrid_Fields.Items.Refresh();
                });
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
        private void btn_CreateModel_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgrid_Fields.SelectedItems.Count > 0)
            {
                foreach (FieldInfo selectedItem in dgrid_Fields.SelectedItems)
                {
                    _fields.Remove(selectedItem);
                }
                RefreshGrid();
            }
        }

        private void btn_CreateModel_Save_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.SetFields(_fields);
            this.Hide();
        }

        private void btn_CreateModel_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void txt_MaxLength_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");

            e.Handled = re.IsMatch(e.Text);
        }
    }
}
