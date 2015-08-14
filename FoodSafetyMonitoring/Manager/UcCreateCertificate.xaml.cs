﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FoodSafetyMonitoring.dao;
using System.Data;
using FoodSafetyMonitoring.Common;
using Toolkit = Microsoft.Windows.Controls;
using FoodSafetyMonitoring.Manager.UserControls;
using System.Printing;


namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCreateCertificate.xaml 的交互逻辑
    /// </summary>
    public partial class UcCreateCertificate : UserControl
    {
        //DataTable ProvinceCityTable;
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();
        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        string username = (Application.Current.Resources["User"] as UserInfo).ShowName;
        private string company_id;
        private string batch_no;
        //private List<string> selectdetect = new List<string>();

        public UcCreateCertificate(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            //ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            //DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");
            //ComboboxTool.InitComboboxSource(_province, rows, "lr");
            //_province.SelectionChanged += new SelectionChangedEventHandler(_province_SelectionChanged);

            //ComboboxTool.InitComboboxSource(_source_company, string.Format(" call p_provice_dept_hb('{0}','yz') ", userId), "lr");
            ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company", "lr");
            //_source_company.SelectionChanged += new SelectionChangedEventHandler(_source_company_SelectionChanged);
            //_cdatetime.Text = string.Format("{0:g}", System.DateTime.Now);
            //_cperson.Text = (Application.Current.Resources["User"] as UserInfo).ShowName;
            //_cdept.Text = dbOperation.GetDbHelper().GetSingle("SELECT INFO_NAME  from  sys_client_sysdept WHERE INFO_CODE = " + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

        }

        //void _province_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_province.SelectedIndex > 0)
        //    {
        //        DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_province.SelectedItem as Label).Tag.ToString() + "'");
        //        ComboboxTool.InitComboboxSource(_city, rows, "lr");
        //        _city.SelectionChanged += new SelectionChangedEventHandler(_city_SelectionChanged);
        //    }
        //}


        //void _city_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_city.SelectedIndex > 0)
        //    {
        //        DataRow[] rows = ProvinceCityTable.Select("pid = '" + (_city.SelectedItem as Label).Tag.ToString() + "'");
        //        ComboboxTool.InitComboboxSource(_region, rows, "lr");
        //        _region.SelectionChanged += new SelectionChangedEventHandler(_region_SelectionChanged);
        //    }
        //}

        //void _region_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (_region.SelectedIndex > 0)
        //    {
        //        ComboboxTool.InitComboboxSource(_source_company, "SELECT COMPANYID,COMPANYNAME FROM t_company where AREAID =" + (_region.SelectedItem as Label).Tag.ToString(), "lr");

        //    }
        //}

        //void _source_company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //来源单位下拉选择的是有效内容，则将自动带出抽检信息
        //    if (_source_company.SelectedIndex >= 1)
        //    {
        //        //清空列表
        //        lvlist.DataContext = null;

        //        //根据条件查询出数据
        //        DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_certificate_main('{0}')", (_source_company.SelectedItem as Label).Tag.ToString())).Tables[0];
        //        current_table = table;
        //        lvlist.DataContext = table;
        //    }
        //}

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (_source_company.SelectedIndex == 0)
            {
                Toolkit.MessageBox.Show("被检单位不能为空", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //先判断该批次是否满足抽检率


            //根据条件查询出数据
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_create_certificate({0},'{1}')",
                                             userId, _source_company.SelectedIndex < 1 ? "" : (_source_company.SelectedItem as Label).Tag)).Tables[0];
            if (table.Rows.Count == 0)
            {
                Toolkit.MessageBox.Show("该被检单位还未做过出证检测！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                company_id = (_source_company.SelectedItem as Label).Tag.ToString();
                _company.Text = table.Rows[0][4].ToString();
                _detect_object.Text = table.Rows[0][3].ToString();
                _object_count.Text = table.Rows[0][1].ToString() + "头";
                batch_no = table.Rows[0][0].ToString();
                _user_name.Text = username;
                _nian.Text = DateTime.Now.Year.ToString();
                _yue.Text = DateTime.Now.Month.ToString();
                _day.Text = DateTime.Now.Day.ToString();

            }
            
            
        }

        private void _create_Click(object sender, RoutedEventArgs e)
        {
            if (_card_id.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入检疫证号！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if(_company.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入货主！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_phone.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入联系电话！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_detect_object.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入动物种类！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_object_count.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入数量及单位！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_for_use.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入用途！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_city_ks.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入启运地点:市（州）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_region_ks.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入启运地点:县（市、区）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_town_ks.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入启运地点:乡（镇）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //if (_village_ks.Text.Trim().Length == 0)
            //{
            //    Toolkit.MessageBox.Show("请输入启运地点:村（养殖场、交易市场）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
            if (_city_js.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入到达地点:市（州）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_region_js.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入到达地点:县（市、区）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_town_js.Text.Trim().Length == 0)
            {
                Toolkit.MessageBox.Show("请输入到达地点:乡（镇）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //if (_village_js.Text.Trim().Length == 0)
            //{
            //    Toolkit.MessageBox.Show("请输入到达地点:村（养殖场、交易市场）！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            string sql = string.Format("call p_insert_certificate('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')"
                            , _card_id.Text, company_id, _company.Text, batch_no, _detect_object.Text, _object_count.Text, _phone.Text,
                            _for_use.Text, _city_ks.Text, _region_ks.Text, _town_ks.Text, _village_ks.Text, _city_js.Text, _region_js.Text,
                            _town_js.Text, _village_js.Text, _object_lable.Text,
                            (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                            (Application.Current.Resources["User"] as UserInfo).ID,
                            System.DateTime.Now);

            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
            if (i >= 0)
            {
                
            }
            else
            {
                Toolkit.MessageBox.Show("电子出证单生成失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        public static PrintQueue GetPrinter(string printerName = null)
        {
            try
            {
                PrintQueue selectedPrinter = null;
                if (!string.IsNullOrEmpty(printerName))
                {
                    var printers = new LocalPrintServer().GetPrintQueues();
                    selectedPrinter = printers.FirstOrDefault(p => p.Name == printerName);
                }
                else
                {
                    selectedPrinter = LocalPrintServer.GetDefaultPrintQueue();
                }
                return selectedPrinter;
            }
            catch
            {
                return null;
            }
        }

        private void _print_Click(object sender, RoutedEventArgs e)
        {
            UcCertificateDetails cer = new UcCertificateDetails();
            //var printDialog = new PrintDialog();
            //printDialog.PrintQueue = GetPrinter();
            //printDialog.PrintVisual(cer, cer.Name);

             PrintDialog dialog = new PrintDialog();
             if (dialog.ShowDialog() == true)
             {
                 dialog.PrintVisual(cer, "Print Test");
             }
        }


        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    if (selectdetect.Count > 0)
        //    {
        //        string msg = "";
        //        if (_object_count.Text.Trim().Length == 0)
        //        {
        //            msg = "*批次头数不能为空";
        //        }
        //        else if (_object_label.Text.Trim().Length == 0)
        //        {
        //            msg = "*耳标号不能为空";
        //        }
        //        else
        //        {
        //            //生成检疫证号
        //            string card_id = dbOperation.GetDbHelper().GetSingle(string.Format("select f_create_cardid('{0}')", (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();
        //            _cardId.Text = card_id;

        //            string detect_id = "";
        //            foreach (var detectid in selectdetect)
        //            {
        //                detect_id = detect_id + "," + detectid;
        //            }

        //            string sql = string.Format("call p_insert_certificate('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')"
        //                          , card_id, (_source_company.SelectedItem as Label).Tag.ToString(),
        //                          _object_count.Text, _object_label.Text,
        //                          (Application.Current.Resources["User"] as UserInfo).DepartmentID,
        //                          (Application.Current.Resources["User"] as UserInfo).ID,
        //                          System.DateTime.Now,
        //                          detect_id);


        //            int i = dbOperation.GetDbHelper().ExecuteSql(sql);
        //            if (i == 1)
        //            {
        //                Toolkit.MessageBox.Show("电子出证单生成成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //                clear();
        //            }
        //            else
        //            {
        //                Toolkit.MessageBox.Show("电子出证单生成失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //                return;
        //            }
        //        }
        //        txtMsg.Text = msg;
        //    }  
        //    else
        //    {
        //        Toolkit.MessageBox.Show("请先选择检测数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
        //}

        //private void _btn_create_Click(object sender, RoutedEventArgs e)
        //{
        //    //生成检疫证号
        //    string card_id = dbOperation.GetDbHelper().GetSingle(string.Format("select f_create_cardid('{0}')", (Application.Current.Resources["User"] as UserInfo).DepartmentID)).ToString();

        //    string batch_no = (sender as Button).Tag.ToString();

        //    string sql = string.Format("call p_insert_certificate('{0}','{1}','{2}','{3}','{4}')"
        //                    , card_id, batch_no,
        //                    (Application.Current.Resources["User"] as UserInfo).DepartmentID,
        //                    (Application.Current.Resources["User"] as UserInfo).ID,
        //                    System.DateTime.Now);


        //    int i = dbOperation.GetDbHelper().ExecuteSql(sql);
        //    if (i == 0)
        //    {
        //        Toolkit.MessageBox.Show("电子出证单生成成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        getdata();

        //        CertificatePreview cer = new CertificatePreview();
        //        cer.ShowDialog();
        //    }
        //    else
        //    {
        //        Toolkit.MessageBox.Show("电子出证单生成失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }
        //}

        //private void _chk_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox cb = sender as CheckBox;
        //    string detectorder = cb.Tag.ToString(); //获取该行detectid   
        //    if (cb.IsChecked == true)
        //    {
        //        selectdetect.Add(detectorder);  //如果选中就保存detectid   
        //    }
        //    else
        //    {
        //        selectdetect.Remove(detectorder);   //如果选中取消就删除里面的detectid   
        //    }  
        //}
        
    }
}
