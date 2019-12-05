using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeGenerator
{
    /// <summary>
    /// Interaction logic for CodeGenerateConfig.xaml
    /// </summary>
    public partial class CodeGenerateConfig : Window
    {
        private CodeGenerateInfo _codeGenerateInfo;
        private string _modelFolder;
        private string _controllerFolder;
        private string _viewFolder;
        private MainWindow _mainWindow;
        public bool _shouldClose;
        CreateModel _cmWindow;

        public CodeGenerateConfig(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            InitData();
            this.Closing += OnClosing;
            _cmWindow = new CreateModel(this);
        }

        private List<FieldInfo> _CustomFields;
        public List<FieldInfo> CustomFields
        {
            get
            {
                if (_CustomFields == null)
                {
                    _CustomFields = new List<FieldInfo>();
                }
                return _CustomFields;
            }
            set { _CustomFields = value; }
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            this.Hide();
            if (!_shouldClose)
            {
                e.Cancel = true;
            }
            else
            {
                _cmWindow._shouldClose = true;
                _cmWindow.Close();
            }
        }

        private void InitData()
        {
            _codeGenerateInfo = new CodeGenerateInfo();
            _modelFolder = System.Configuration.ConfigurationSettings.AppSettings["ModelPath"];
            _controllerFolder = System.Configuration.ConfigurationSettings.AppSettings["ControllerPath"];
            _viewFolder = System.Configuration.ConfigurationSettings.AppSettings["ViewPath"];

            _mainWindow.LoadCodeGenerateConfig(_codeGenerateInfo);
            txt_ResultPath.Text = _codeGenerateInfo.GenerateResultPath;// System.Configuration.ConfigurationSettings.AppSettings["ResultPath"];
            txt_Company.Text = _codeGenerateInfo.Company;//  System.Configuration.ConfigurationSettings.AppSettings["Company"];
            txt_Category.Text = _codeGenerateInfo.Category;//  System.Configuration.ConfigurationSettings.AppSettings["Category"];
        }

        internal void SetFields(List<FieldInfo> _fields)
        {
            if (_fields != null && _fields.Count > 0)
            {
                CustomFields.Clear();
                foreach (var fieldInfo in _fields)
                {
                    CustomFields.Add(fieldInfo.Clone() as FieldInfo);
                }
                lbl_CreateModel.Content = _fields.Count;
            }
        }

        private void SetGenerateInfo()
        {
            _codeGenerateInfo.GenerateResultPath = txt_ResultPath.Text.Trim();
            _codeGenerateInfo.Category = txt_Category.Text.Trim();
            _codeGenerateInfo.Company = txt_Company.Text.Trim();

            _codeGenerateInfo.ModelFilePath = System.IO.Path.Combine(_codeGenerateInfo.GenerateResultPath, _modelFolder, _codeGenerateInfo.Category);
            _codeGenerateInfo.ControllerFilePath = System.IO.Path.Combine(_codeGenerateInfo.GenerateResultPath, _controllerFolder, _codeGenerateInfo.Category);
            _codeGenerateInfo.ViewFilePath = System.IO.Path.Combine(_codeGenerateInfo.GenerateResultPath, _viewFolder, _codeGenerateInfo.Category);
        }

        private bool _hasCreatedFolder;

        private void CreateFolder()
        {
            if (!_hasCreatedFolder)
            {
                Utility.CreateFolder(_codeGenerateInfo.ModelFilePath);
                Utility.CreateFolder(_codeGenerateInfo.ControllerFilePath);
                Utility.CreateFolder(_codeGenerateInfo.ViewFilePath);
                _hasCreatedFolder = true;
            }
        }

        private void btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            SetGenerateInfo();
            CreateFolder();

            string cmdStrIns;
            string objName;
            string modelName = null;
            ModelInfo modelInfo = new ModelInfo();
            if (rd_ModelSourceTypeDb.IsChecked.Value && _mainWindow.GetCommand(out cmdStrIns, out objName))
            {
                DataTable dataSchemaTb = _mainWindow.DBPrivoder.GetDataSchema(cmdStrIns);
                modelName = checkBox.IsChecked.Value ? dataSchemaTb.TableName : txt_Model.Text.Trim();
                if (string.IsNullOrEmpty(modelName))
                {
                    modelName = objName.Substring(objName.IndexOf('.') + 1);
                }
                var tableInfo = _mainWindow.DBPrivoder.GetTableInfo(modelName);


                for (int j = 0; j < dataSchemaTb.Columns.Count; j++)
                {
                    FieldInfo field = new FieldInfo();
                    field.Name = dataSchemaTb.Columns[j].Caption;
                    field.DataType = dataSchemaTb.Columns[j].DataType.Name;
                    field.IsRequied = !dataSchemaTb.Columns[j].AllowDBNull;
                    field.MaxLength = dataSchemaTb.Columns[j].MaxLength;
                    field.Description = tableInfo.Find(f => f.ColumnName == field.Name)?.Description;

                    modelInfo.Fields.Add(field);
                }
            }
            else if (rd_ModelSourceTypeCustom.IsChecked.Value)
            {
                modelName = checkBox.IsChecked.Value ? "ModelName" : txt_Model.Text.Trim();
                modelInfo.Fields = CustomFields;
            }
            modelInfo.Name = modelName;
            CodeGeneratorProvider provider = new CodeGeneratorProvider(GeneratorTypeEnum.ExtJS6);

            provider.Start(modelInfo, _codeGenerateInfo);
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void btn_SelectResultFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "请选择代码生成的目录";
                dlg.SelectedPath = txt_ResultPath.Text;
                dlg.ShowNewFolderButton = true;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txt_ResultPath.Text = dlg.SelectedPath;
                }
            }
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as System.Windows.Controls.CheckBox;

            txt_Model.IsReadOnly = checkBox.IsChecked.Value;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            SetGenerateInfo();
            _mainWindow.SaveConfig(_codeGenerateInfo);
        }

        private void rd_ModelSourceType2_Checked(object sender, RoutedEventArgs e)
        {
            btn_CreateModel.IsEnabled = true;// rd_ModelSourceType2.IsChecked.Value;
            checkBox.IsChecked = false;
            txt_Model.IsReadOnly = false;
        }

        private void rd_ModelSourceType2_Unchecked(object sender, RoutedEventArgs e)
        {
            btn_CreateModel.IsEnabled = false;
            checkBox.IsChecked = true;
            txt_Model.IsReadOnly = true;
        }

        private void btn_CreateModel_Click(object sender, RoutedEventArgs e)
        {
            _cmWindow.ShowDialog();
        }
    }
}
