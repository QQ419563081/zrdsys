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
using DBUtility;
using FoodSafetyMonitoring.Manager.UserControls;

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcFeedDetectManager.xaml 的交互逻辑
    /// </summary>
    public partial class UcFeedDetectManager : UserControl
    {
        DataTable ProvinceCityTable;
        DbHelperMySQL dbOperation;

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;
        public UcFeedDetectManager()
        {
            InitializeComponent();

            dbOperation = DBUtility.DbHelperMySQL.CreateDbHelper();
            ProvinceCityTable = Application.Current.Resources["省市表"] as DataTable;
            DataRow[] rows = ProvinceCityTable.Select("pid = '0001'");

            //画面初始化-新增检测单画面
            ComboboxTool.InitComboboxSource(_feed_name, string.Format("call p_user_feed ('{0}')", userId), "lr");
            _feed_name.SelectionChanged += new SelectionChangedEventHandler(_feed_name_SelectionChanged);
    
            ComboboxTool.InitComboboxSource(_detect_trade, "select tradeId,tradeName from t_trade where openFlag = '1'", "lr");           
            _detect_trade.SelectionChanged += new SelectionChangedEventHandler(_detect_trade_SelectionChanged);
            _detect_trade.SelectedIndex = 1;
            //ComboboxTool.InitComboboxSource(_detect_item, "SELECT itemid,ItemNAME FROM v_user_item WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_object, " SELECT objectId,objectName FROM v_user_object WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_sample, "  SELECT sampleId,sampleName FROM v_user_sample WHERE userid = " + userId);
            //ComboboxTool.InitComboboxSource(_detect_sensitivity, "SELECT sensitivityId,sensitivityName FROM t_det_sensitivity where openFlag = '1'", "lr");
            ComboboxTool.InitComboboxSource(_detect_result, "SELECT resultId,resultName FROM t_det_result where openFlag = '1' ORDER BY id", "lr");
            _entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
            _detect_person.Text = (Application.Current.Resources["User"] as UserInfo).ShowName;
            _detect_site.Text = dbOperation.GetSingle("SELECT INFO_NAME  from  sys_client_sysdept WHERE INFO_CODE = " + (Application.Current.Resources["User"] as UserInfo).DepartmentID).ToString();

        }

        private void clear()
        {
            this._feed_name.SelectedIndex = 0;
            this._feed_brand.Text = "";
            this._produce_company.Text = "";
            this._produce_batchno.Text = "";
            this._batch_num.Text = "";
            this._buy_date.Text = "";
            this._detect_trade.SelectedIndex = 1;
            this._detect_item.SelectedIndex = 0;
            this._detect_method1.IsChecked = false;
            this._detect_method2.IsChecked = false;
            this._detect_method3.IsChecked = false;
            this._detect_object.SelectedIndex = 0;
            this._detect_sample.SelectedIndex = 0;
            this._detect_sensitivity.SelectedIndex = 0;
            this._detect_result.SelectedIndex = 0;
            this._entering_datetime.Text = string.Format("{0:g}", System.DateTime.Now);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            if (_feed_name.SelectedIndex < 1)
            {
                msg = "*请选择饲料名称";
            }
            else if (_detect_item.SelectedIndex < 1)
            {
                msg = "*请选择检查项目";
            }
            else if (_detect_method1.IsChecked != true && _detect_method2.IsChecked != true && _detect_method3.IsChecked != true)
            {
                msg = "*请选择检测方法";
            }
            else if (_detect_object.SelectedIndex < 1)
            {
                msg = "*请选择检测对象";
            }
            else if (_detect_sample.SelectedIndex < 1)
            {
                msg = "*请选择检测样本";
            }
            else if (_detect_sensitivity.SelectedIndex < 1)
            {
                msg = "*请选择检测灵敏度";
            }
            else if (_detect_result.SelectedIndex < 1)
            {
                msg = "*请选择检测结果";
            }
            else if (_detect_person.Text.Trim().Length == 0)
            {
                msg = "*请输入检测师";
            }
            else
            {
                string sql = string.Format("call p_insert_feed_detect('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')"
                              , (_feed_name.SelectedItem as Label).Tag.ToString(),
                              (_detect_item.SelectedItem as Label).Tag.ToString(),
                              (_detect_method1.IsChecked == true ? 1 : 0) + (_detect_method2.IsChecked == true ? 2 : 0) + (_detect_method3.IsChecked == true ? 3 : 0),
                              (_detect_object.SelectedItem as Label).Tag.ToString(),
                              (_detect_sample.SelectedItem as Label).Tag.ToString(),
                              (_detect_sensitivity.SelectedItem as Label).Tag.ToString(),
                              (_detect_result.SelectedItem as Label).Tag.ToString(),
                              (Application.Current.Resources["User"] as UserInfo).DepartmentID,
                              (Application.Current.Resources["User"] as UserInfo).ID,
                              System.DateTime.Now);

                int i = dbOperation.ExecuteSql(sql);
                if (i == 1)
                {
                    Toolkit.MessageBox.Show("添加成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                }
                else
                {
                    Toolkit.MessageBox.Show("添加失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            txtMsg.Text = msg;

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void _detect_method1_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Name == "_detect_method1")
            {
                _detect_method2.IsChecked = false;
                _detect_method3.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_detect_method2")
            {
                _detect_method1.IsChecked = false;
                _detect_method3.IsChecked = false;
            }
            else if ((sender as CheckBox).Name == "_detect_method3")
            {
                _detect_method1.IsChecked = false;
                _detect_method2.IsChecked = false;
            }
        }


        void _feed_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //饲料名称下拉选择的是有效内容
            if (_feed_name.SelectedIndex > 0)
            {
                DataTable table = dbOperation.GetDataSet(string.Format("select feedbrand,producecompany,producebatchno,batchnum,buydate from t_feed_info where id = '{0}'", (_feed_name.SelectedItem as Label).Tag.ToString())).Tables[0];

                this._feed_brand.Text = table.Rows[0][0].ToString();
                this._produce_company.Text = table.Rows[0][1].ToString();
                this._produce_batchno.Text = table.Rows[0][2].ToString();
                this._batch_num.Text = table.Rows[0][3].ToString();
                this._buy_date.Text = table.Rows[0][4].ToString();
            }
        }


        void _detect_trade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_trade.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_detect_item, string.Format("SELECT itemid,ItemNAME FROM v_user_item WHERE userid = '{0}' and (tradeId ='{1}'or ifnull(tradeId,'') = '')", userId, (_detect_trade.SelectedItem as Label).Tag), "lr");
                ComboboxTool.InitComboboxSource(_detect_item, string.Format("SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='{0}'or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", (_detect_trade.SelectedItem as Label).Tag), "lr");
                _detect_item.SelectionChanged += new SelectionChangedEventHandler(_detect_item_SelectionChanged);
            }
            //else
            //{
            //    _detect_item.Items.Clear();
            //    _detect_object.Items.Clear();
            //    _detect_sample.Items.Clear();
            //    _detect_sensitivity.Items.Clear();
            //}
        }

        void _detect_item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_item.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_detect_object, string.Format("call p_detect_object( '{0}','{1}')", userId, (_detect_item.SelectedItem as Label).Tag), "lr");
                ComboboxTool.InitComboboxSource(_detect_object, string.Format("SELECT objectId,objectName FROM v_item_object WHERE itemId = '{0}' and tradeId = '{1}'", (_detect_item.SelectedItem as Label).Tag, (_detect_trade.SelectedItem as Label).Tag), "lr");
                _detect_object.SelectionChanged += new SelectionChangedEventHandler(_detect_object_SelectionChanged);
            }
        }

        void _detect_object_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_object.SelectedIndex > 0)
            {
                //ComboboxTool.InitComboboxSource(_detect_sample, string.Format("call p_detect_sample( '{0}','{1}')", userId, (_detect_object.SelectedItem as Label).Tag), "lr");
                ComboboxTool.InitComboboxSource(_detect_sample, string.Format("SELECT sampleId,sampleName FROM v_object_sample WHERE objectId = '{0}'", (_detect_object.SelectedItem as Label).Tag), "lr");
                _detect_sample.SelectionChanged += new SelectionChangedEventHandler(_detect_sensitivity_SelectionChanged);
            }
        }

        void _detect_sensitivity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_detect_sample.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_sensitivity, string.Format("call p_detect_sensitivity( '{0}','{1}')", (_detect_item.SelectedItem as Label).Tag, (_detect_object.SelectedItem as Label).Tag), "lr");
            }
        }
    }
}
