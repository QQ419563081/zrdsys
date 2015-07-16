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

namespace FoodSafetyMonitoring.Manager
{
    /// <summary>
    /// UcCultureDetectInquiry.xaml 的交互逻辑
    /// </summary>
    public partial class UcCultureDetectInquire : UserControl
    {
        public IDBOperation dbOperation = null;
        private Dictionary<string, MyColumn> MyColumns = new Dictionary<string, MyColumn>();

        string userId = (Application.Current.Resources["User"] as UserInfo).ID;

        public UcCultureDetectInquire(IDBOperation dbOperation)
        {
            InitializeComponent();
            this.dbOperation = dbOperation;

            //画面初始化-检测单列表画面
            dtpStartDate.Value = DateTime.Now.AddDays(-1);
            dtpEndDate.Value = DateTime.Now;
            ComboboxTool.InitComboboxSource(_culture_company, string.Format("call p_user_dept('{0}')", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_detect_item1, "SELECT ItemID,ItemNAME FROM t_det_item WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1' order by orderId", "cxtj");
            ComboboxTool.InitComboboxSource(_detect_object1, "SELECT objectId,objectName FROM t_det_object WHERE  (tradeId ='1'or tradeId ='2' or tradeId ='3' or ifnull(tradeId,'') = '') and OPENFLAG = '1'", "cxtj");
            ComboboxTool.InitComboboxSource(_detect_result1, "SELECT resultId,resultName FROM t_det_result where openFlag = '1' ORDER BY id", "cxtj");
            ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");
            ComboboxTool.InitComboboxSource(_detect_method, "select reagentId,reagentName from t_det_reagent where openFlag = '1'", "cxtj");
            ComboboxTool.InitComboboxSource(_detect_type, "SELECT sourceId,sourceName FROM t_det_source where openFlag = '1'", "cxtj");

            _culture_company.SelectionChanged += new SelectionChangedEventHandler(_culture_company_SelectionChanged);

            SetColumns();
        }

        private void SetColumns()
        {
            MyColumns.Add("orderid", new MyColumn("orderid", "检测单编号") { BShow = true, Width = 10 });
            MyColumns.Add("detecttype", new MyColumn("detecttype", "数据来源id") { BShow = false });
            MyColumns.Add("detecttypename", new MyColumn("detecttypename", "数据来源") { BShow = true, Width = 10 });
            MyColumns.Add("detectdate", new MyColumn("detectdate", "检测时间") { BShow = true, Width = 18 });
            MyColumns.Add("deptid", new MyColumn("deptid", "养殖企业id") { BShow = false });
            MyColumns.Add("deptname", new MyColumn("deptname", "养殖企业名称") { BShow = true, Width = 16 });
            MyColumns.Add("itemid", new MyColumn("itemid", "检测项目id") { BShow = false });
            MyColumns.Add("itemname", new MyColumn("itemname", "检测项目") { BShow = true, Width = 12 });
            MyColumns.Add("objectid", new MyColumn("objectid", "检测对象id") { BShow = false });
            MyColumns.Add("objectname", new MyColumn("objectname", "检测对象") { BShow = true, Width = 10 });
            MyColumns.Add("sampleid", new MyColumn("sampleid", "检测样本id") { BShow = false });
            MyColumns.Add("samplename", new MyColumn("samplename", "检测样本") { BShow = true, Width = 10 });
            MyColumns.Add("sensitivityid", new MyColumn("sensitivityid", "检测灵敏度id") { BShow = false });
            MyColumns.Add("sensitivityname", new MyColumn("sensitivityname", "检测灵敏度") { BShow = true, Width = 10 });
            MyColumns.Add("reagentid", new MyColumn("companyid", "检测方法id") { BShow = false });
            MyColumns.Add("reagentname", new MyColumn("reagentname", "检测方法") { BShow = false });
            MyColumns.Add("resultid", new MyColumn("resultid", "检测结果id") { BShow = false });
            MyColumns.Add("resultname", new MyColumn("resultname", "检测结果") { BShow = true, Width = 10 });
            MyColumns.Add("detectuserid", new MyColumn("detectuserid", "检测师id") { BShow = false });
            MyColumns.Add("culturefileno", new MyColumn("culturefileno", "档案编号") { BShow = true, Width = 10 });
            //MyColumns.Add("areaname", new MyColumn("areaname", "来源产地") { BShow = false });
            //MyColumns.Add("companyid", new MyColumn("companyid", "来源单位id") { BShow = false });
            //MyColumns.Add("companyname", new MyColumn("companyname", "来源单位") { BShow = true, Width = 18 });
            MyColumns.Add("sum_num", new MyColumn("sum_num", "总行数") { BShow = false });

            _tableview.MyColumns = MyColumns;
            _tableview.BShowModify = false;
            _tableview.BShowDetails = true;

            if ((Application.Current.Resources["User"] as UserInfo).FlagTier == "0")
            {
                _tableview.BShowDelete = true;
            }
            else
            {
                _tableview.BShowDelete = false;
            }

            _tableview.DetailsRowEnvent += new UcTableOperableView.DetailsRowEventHandler(_tableview_DetailsRowEnvent);
            _tableview.DeleteRowEnvent += new UcTableOperableView.DeleteRowEventHandler(_tableview_DeleteRowEnvent);
        }

       
        void _culture_company_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_culture_company.SelectedIndex > 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("SELECT RECO_PKID,INFO_USER,NUMB_USER FROM sys_client_user where fk_dept = '{0}'", (_culture_company.SelectedItem as Label).Tag.ToString()), "cxtj");
            }
            else if (_culture_company.SelectedIndex == 0)
            {
                ComboboxTool.InitComboboxSource(_detect_person1, string.Format("call p_user_detuser('{0}')", userId), "cxtj");
            }
        }

        private void _query_Click(object sender, RoutedEventArgs e)
        {
            if (dtpStartDate.Value.Value.Date > dtpEndDate.Value.Value.Date)
            {
                Toolkit.MessageBox.Show("开始时间大于结束时间，请重新选择！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _tableview.GetDataByPageNumberEvent += new UcTableOperableView.GetDataByPageNumberEventHandler(_tableview_GetDataByPageNumberEvent);
            GetData();
            _tableview.Title = string.Format("数据统计时间:{0}年{1}月{2}日到{3}年{4}月{5}日 合计{6}条数据", dtpStartDate.Value.Value.Year, dtpStartDate.Value.Value.Month, dtpStartDate.Value.Value.Day,
                          dtpEndDate.Value.Value.Year, dtpEndDate.Value.Value.Month, dtpEndDate.Value.Value.Day, _tableview.RowTotal);
            _tableview.PageIndex = 1;
        }

        private void GetData()
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_culture_detect({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},{11})",
                   (Application.Current.Resources["User"] as UserInfo).ID,
                //dtpStartDate.Value.ToString() == dtpEndDate.Value.ToString() ? "" : dtpStartDate.Value.ToString(),
                //dtpStartDate.Value.ToString() == dtpEndDate.Value.ToString() ? "" : dtpEndDate.Value.ToString(),
                   ((DateTime)dtpStartDate.Value).ToShortDateString(),
                   ((DateTime)dtpEndDate.Value).ToShortDateString(),
                   _culture_company.SelectedIndex < 1 ? "" : (_culture_company.SelectedItem as Label).Tag,
                   _detect_item1.SelectedIndex < 1 ? "" : (_detect_item1.SelectedItem as Label).Tag,
                   _detect_object1.SelectedIndex < 1 ? "" : (_detect_object1.SelectedItem as Label).Tag,
                   _detect_result1.SelectedIndex < 1 ? "" : (_detect_result1.SelectedItem as Label).Tag,
                   _detect_method.SelectedIndex < 1 ? "" : (_detect_method.SelectedItem as Label).Tag,
                   _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as Label).Tag,
                   _detect_type.SelectedIndex < 1 ? "" : (_detect_type.SelectedItem as Label).Tag,
                   (_tableview.PageIndex - 1) * _tableview.RowMax,
                   _tableview.RowMax)).Tables[0];

            _tableview.Table = table;
        }

        void _tableview_GetDataByPageNumberEvent()
        {
            GetData();
        }

        void _tableview_DetailsRowEnvent(string id)
        {
            int orderid = int.Parse(id);
            culture_detectdetails det = new culture_detectdetails(dbOperation, orderid);
            det.ShowDialog();
        }

        void _tableview_DeleteRowEnvent(string id)
        {
            if (Toolkit.MessageBox.Show("确定要删除该条检测单吗？", "系统询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    bool reviewflag = dbOperation.GetDbHelper().Exists(string.Format("select count(id) from t_detect_review where DetectId = '{0}'", id));
                    if (reviewflag)
                    {
                        int result1 = dbOperation.GetDbHelper().ExecuteSql(string.Format("delete from t_detect_review where DetectId ='{0}'", id));
                        if (result1 <= 0)
                        {
                            Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    int result = dbOperation.GetDbHelper().ExecuteSql(string.Format("delete from t_detect_report where ORDERID ='{0}'", id));
                    if (result > 0)
                    {
                        Toolkit.MessageBox.Show("删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        GetData();
                    }
                    else
                    {
                        Toolkit.MessageBox.Show("删除失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                catch
                {
                    Toolkit.MessageBox.Show("删除失败2！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
        }

        private void _export_Click(object sender, RoutedEventArgs e)
        {
            DataTable table = dbOperation.GetDbHelper().GetDataSet(string.Format("call p_query_culture_detect({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10},{11})",
                   (Application.Current.Resources["User"] as UserInfo).ID,
                //dtpStartDate.Value.ToString() == dtpEndDate.Value.ToString() ? "" : dtpStartDate.Value.ToString(),
                //dtpStartDate.Value.ToString() == dtpEndDate.Value.ToString() ? "" : dtpEndDate.Value.ToString(),
                   ((DateTime)dtpStartDate.Value).ToShortDateString(),
                   ((DateTime)dtpEndDate.Value).ToShortDateString(),
                   _culture_company.SelectedIndex < 1 ? "" : (_culture_company.SelectedItem as Label).Tag,
                   _detect_item1.SelectedIndex < 1 ? "" : (_detect_item1.SelectedItem as Label).Tag,
                   _detect_object1.SelectedIndex < 1 ? "" : (_detect_object1.SelectedItem as Label).Tag,
                   _detect_result1.SelectedIndex < 1 ? "" : (_detect_result1.SelectedItem as Label).Tag,
                   _detect_method.SelectedIndex < 1 ? "" : (_detect_method.SelectedItem as Label).Tag,
                   _detect_person1.SelectedIndex < 1 ? "" : (_detect_person1.SelectedItem as Label).Tag,
                   _detect_type.SelectedIndex < 1 ? "" : (_detect_type.SelectedItem as Label).Tag,
                  0,
                  _tableview.RowTotal)).Tables[0];

            _tableview.ExportExcel(table);
        }
    }
}
