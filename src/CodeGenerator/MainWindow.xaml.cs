using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace CodeGenerator
{
    // 2013年04月10日 9时 唐友辉

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _conStr = "Data Source={0};Initial Catalog={1};uid={2};pwd={3};";
        private readonly string _path = System.AppDomain.CurrentDomain.BaseDirectory + @"config.xml";
        private string _queryStr = "SELECT * FROM {0}";
        private CodeGenerateConfig _codeGenerateConfigWindow;
        private NotifyIcon _notifyIcon;
        public string ConnectionString
        {
            get
            {
                return string.Format(_conStr, txt_DbName.Text, txt_server.Text, txt_UId.Text,
                                                 txt_Pwd.Text);
            }
            private set { throw new NotImplementedException(); }
        }

        public readonly SQLHelper DBPrivoder = new SQLHelper();
        private XDocument _doc;
        public XDocument Doc
        {
            get
            {
                if (_doc == null)
                    _doc = XDocument.Load(_path);
                return _doc;
            }
        }

        private XElement ConfigNode
        {
            get
            {
                return Doc.Root;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Init();
            InitNotifyIcon();
        }

        void Init()
        {

            if (ConfigNode != null)
                GetConParam();

            Closing += OnClosing;
            _codeGenerateConfigWindow = new CodeGenerateConfig(this);
        }

        private void InitNotifyIcon()
        {
            this._notifyIcon = new NotifyIcon();
            this._notifyIcon.BalloonTipText = "物链-代码生成器 v1.1";
            this._notifyIcon.ShowBalloonTip(2000);
            this._notifyIcon.Text = "物链-代码生成器 v1.1";
            this._notifyIcon.Icon = new System.Drawing.Icon(@"logo.ico");
            //this._notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this._notifyIcon.Visible = true;
            //打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("显示窗体");
            open.Click += new EventHandler(Show);
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(Close);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            this._notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left) this.Show(o, e);
            });
        }

        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Close(object sender, EventArgs e)
        {
            _codeGenerateConfigWindow._shouldClose = true;
            _codeGenerateConfigWindow.Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void GetConParam()
        {
            txt_DbName.Text = ConfigNode.Element("datasource").Value;
            txt_server.Text = ConfigNode.Element("initialcatalog").Value;
            txt_UId.Text = ConfigNode.Element("uid").Value;
            txt_Pwd.Text = ConfigNode.Element("pwd").Value;
        }

        private void SaveConfig()
        {
            ConfigNode.Element("datasource").Value = txt_DbName.Text;
            ConfigNode.Element("initialcatalog").Value = txt_server.Text;
            ConfigNode.Element("uid").Value = txt_UId.Text;
            ConfigNode.Element("pwd").Value = txt_Pwd.Text;
            Doc.Save(_path);
        }

        public void SaveConfig(CodeGenerateInfo cgInfo)
        {
            ConfigNode.Element("company").Value = cgInfo.Company;
            ConfigNode.Element("category").Value = cgInfo.Category;
            ConfigNode.Element("generatePath").Value = cgInfo.GenerateResultPath;
            Doc.Save(_path);
        }

        public void LoadCodeGenerateConfig(CodeGenerateInfo cgInfo)
        {
            cgInfo.Company = ConfigNode.Element("company").Value;
            cgInfo.Category = ConfigNode.Element("category").Value;
            cgInfo.GenerateResultPath = ConfigNode.Element("generatePath").Value;
        }

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            if (!DBPrivoder.TestCon(ConnectionString))
            {
                MessageBox.Show("连接失败");
                return;
            }

            DBPrivoder.SetCon(ConnectionString);
            DataTable schemeTb = DBPrivoder.GetDBSchema();
            string name;
            cbx_TableList.Items.Clear();
            cbx_ViewList.Items.Clear();
            for (int i = 0; i < schemeTb.Rows.Count; i++)
            {
                if (string.Equals(schemeTb.Rows[i]["TABLE_OWNER"], "dbo"))
                {
                    name = schemeTb.Rows[i]["TABLE_OWNER"] + "." + schemeTb.Rows[i]["TABLE_NAME"];
                    if (string.Equals(schemeTb.Rows[i]["TABLE_TYPE"], "TABLE"))
                        cbx_TableList.Items.Add(name);
                    else if (string.Equals(schemeTb.Rows[i]["TABLE_TYPE"], "VIEW"))
                        cbx_ViewList.Items.Add(name);
                }
            }
            cbx_TableList.SelectedIndex = 0;
            cbx_ViewList.SelectedIndex = 0;
        }

        private void btn_testCon_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(DBPrivoder.TestCon(ConnectionString) ? "连接成功" : "连接失败");
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }

        private void btn_run_Click(object sender, RoutedEventArgs e)
        {
            string cmdStrIns;
            string objName;
            if (GetCommand(out cmdStrIns, out objName))
            {
                try
                {
                    WriteResult(cmdStrIns);
                }
                catch (Exception ex)
                {
                    this.txt_result.Text = ex.ToString();
                }
            }
        }

        public bool GetCommand(out string cmdStrIns, out string objName)
        {
            bool valid = true;
            cmdStrIns = null;
            objName = null;
            if (rd_table.IsChecked.Value && cbx_TableList.SelectedIndex >= 0)
            {
                string tablename = cbx_TableList.Items[cbx_TableList.SelectedIndex].ToString();
                cmdStrIns = string.Format(_queryStr, tablename);
                objName = tablename;
            }
            else if (rd_view.IsChecked.Value && cbx_ViewList.SelectedIndex >= 0)
            {
                string viewname = cbx_ViewList.Items[cbx_ViewList.SelectedIndex].ToString();
                cmdStrIns = string.Format(_queryStr, viewname);
                objName = viewname;
            }
            else if (!string.IsNullOrWhiteSpace(txt_execute.Text))
            {
                cmdStrIns = txt_execute.Text;
            }
            else
            {
                MessageBox.Show("无可执行命令");
                valid = false;
            }
            return valid;
        }

        private void WriteResult(string cmdStrIns)
        {
            DataTable dataTb = DBPrivoder.GetDataList(cmdStrIns);
            DataTable dataSchemaTb = DBPrivoder.GetDataSchema(cmdStrIns);
            StringBuilder resultBuilder = new StringBuilder();
            string valuesStr = "( ";

            string template = string.Empty;
            for (int i = 0; i < dataTb.Rows.Count; i++)
            {
                if (i == 0)
                {
                    string columsName = "INSERT INTO [" + dataSchemaTb.TableName + "] ( ";
                    for (int j = 0; j < dataTb.Columns.Count; j++)
                    {
                        if (j != 0)
                        {
                            columsName += ", ";
                            valuesStr += ", ";
                        }
                        columsName += "[" + dataSchemaTb.Columns[j].Caption + "]";
                        switch (dataSchemaTb.Columns[j].DataType.Name)
                        {
                            case "String":
                                valuesStr += "'{" + j + "}'";
                                break;
                            case "DateTime":
                                valuesStr += "'{" + j + "}'";
                                break;
                            case "Guid":
                                valuesStr += "'{" + j + "}'";
                                break;
                            default:
                                valuesStr += "{" + j + "}";
                                break;
                        }
                    }
                    columsName += " )";
                    valuesStr += " )";
                    template = $"{columsName}\tVALUES\t{valuesStr};\r\n";
                }
                var tempStr = template;
                for (int j = 0; j < dataTb.Columns.Count; j++)
                {
                    string valueStr;
                    if (string.Equals(dataSchemaTb.Columns[j].DataType.Name, "String") && !(dataTb.Rows[i][j] is System.DBNull))
                        valueStr = dataTb.Rows[i][j].ToString().Replace("'", "''");
                    else
                        valueStr = (dataTb.Rows[i][j] is System.DBNull)
                                        ? "NULL"
                                        : string.Equals(dataSchemaTb.Columns[j].DataType.Name, "Boolean")
                                              ? ((bool)dataTb.Rows[i][j] ? 1 : 0).ToString()
                                              : dataTb.Rows[i][j].ToString();

                    tempStr = tempStr.Replace("{" + j + "}", valueStr);
                }

                tempStr = tempStr.Replace("'NULL'", "NULL");
                resultBuilder.Append(tempStr);
            }
            txt_result.Text = resultBuilder.ToString();
        }

        private void btn_copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(txt_result.Text);
        }

        private void btn_CodeGenerator_Click(object sender, RoutedEventArgs e)
        {
            _codeGenerateConfigWindow.ShowDialog();
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            _codeGenerateConfigWindow.Hide();
            e.Cancel = true;
            this.Hide();
        }
    }
}
