using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Input;
using System.Data.OleDb;
using NLog;
using System.Xml;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using Action = System.Action;
using Application = System.Windows.Forms.Application;
using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;
using Label = System.Windows.Forms.Label;
using DataTable = System.Data.DataTable;
using Font = System.Drawing.Font;

using SmartXLS;




namespace Amkor_Material_Manager
{
    public partial class Form_ITS : Form
    {
        int nnTabIndex = 0;

        private static readonly NLog.Logger Synclog = NLog.LogManager.GetLogger("SyncLog");

        //Excel
        public static bool[] bExcelUse = new bool[5] { true, true, true, true, true }; //Excel 변환 작업 수앻 List

        //public static bool[] bGroupUse = new bool[6] { true, true, true, true, true, true }; 
        public static bool[] bGroupUse = new bool[9] { true, true, true, true, true, true, true, true, true }; //210824_Sangik.chpi_타워그룹추가  //220823_ilyoung_타워그룹추가

        public static bool[] bTowerUse = new bool[4] { true, true, true, true };
        public static bool bExcel_Start = false;
        public string strExcelfilePath = "";
        public static int nExcelIndex = 0;

        //timeset
        public static string strTimeset_date_st = "", strTimeset_date_ed = "";
        public static string strTimeset_hour_st = "", strTimeset_hour_ed = "";
        public static string strTimeset_Min_st = "", strTimeset_Min_ed = "";
        public static bool bSearch_sid = false;
        ///////

        public static bool IsDateGathering = false;
        public static bool bUpdate_Timer = false;

        public int nSum = 0;

        ///ASM DB
        public MsSqlManager MSSql = null;
        bool bASMconnect = false;
        string strASM_TowerLocation1 = "", strASM_TowerLocation2 = "", strASM_TowerLocation3 = "";
        int nDbUpdate = -1;

        Dictionary<int, string> Dec2Alpa = new Dictionary<int, string>();

        SharedAPI sharedAPI = new SharedAPI();

        public Form_ITS()
        {
            InitializeComponent();

            Fnc_Init();
            timer1.Start();
        }

        public void Fnc_Init()
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Excel");

            if (!di.Exists) { di.Create(); }
            strExcelfilePath = di.ToString();

            comboBox_searchtype.SelectedIndex = 0;
            comboBox_type.SelectedIndex = 0;
            comboBox_group.SelectedIndex = AMM_Main.nDefaultGroup - 1;

            comboBox_type2.SelectedIndex = 0;

            comboBox_group2.SelectedIndex = AMM_Main.nDefaultGroup - 1;

            tabControl_ITS.SelectedIndex = 0;

            strASM_TowerLocation1 = "Amkor.B-Line.S10-2_Kitting.20_Material_Tower";
            strASM_TowerLocation2 = "Amkor.B-Line.S10-2_Kitting.20_Material_Tower2";
            strASM_TowerLocation3 = "Amkor.B-Line.S10-2_Kitting.20_Material_Tower3";

            init_Dic();

            if (AMM_Main.strMatchTab == "TRUE")
            {
                Fnc_InitMSSql();
            }
        }

        private void init_Dic()
        {
            Dec2Alpa.Clear();
            Dec2Alpa.Add(1, "A");
            Dec2Alpa.Add(2, "B");
            Dec2Alpa.Add(3, "C");
            Dec2Alpa.Add(4, "D");
            Dec2Alpa.Add(5, "E");
            Dec2Alpa.Add(6, "F");
            Dec2Alpa.Add(7, "G");
            Dec2Alpa.Add(8, "H");
            Dec2Alpa.Add(9, "I");
            Dec2Alpa.Add(10, "J");
            Dec2Alpa.Add(11, "K");
            Dec2Alpa.Add(12, "L");
            Dec2Alpa.Add(13, "M");
        }

        private void tabControl_ITS_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabNo = tabControl_ITS.SelectedIndex;

            nnTabIndex = tabNo;


            if (tabNo == 0)
            {
                comboBox_type.SelectedIndex = 0;
                comboBox_group.SelectedIndex = AMM_Main.nDefaultGroup - 1;

                Fnc_Process_CalMaterialInfo();

                bUpdate_Timer = true;

            }
            //[210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님
            else if (tabNo == 1)
            {
                bUpdate_Timer = true;


                Fnc_Init_datagrid_capa();


            }
            //]210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님

            else if (tabNo == 2)
            {
                //[210813_Sangik.choi_장기보관관리기능추가 by이종명수석님

                //if (listk_count != 0)
                //{
                //    AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
                //}
                //]210813_Sangik.choi_장기보관관리기능추가 by이종명수석님


                comboBox_type2.SelectedIndex = 0;
                comboBox_group2.SelectedIndex = AMM_Main.nDefaultGroup - 1;

                button_search.Visible = false;
                textBox_sid.Visible = false;
                label_sid.Visible = false;
                textBox_sid.Text = "";

                Application.DoEvents();

                Fnc_Update_timeset();



                bUpdate_Timer = false;
            }
            else if (tabNo == 3)
            {


                //[210813_Sangik.choi_장기보관관리기능추가 by이종명수석님
                //if (listk_count != 0)
                //{
                //    AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
                //}
                textBox_badge.Text = "";

                //]210813_Sangik.choi_장기보관관리기능추가 by이종명수석님
                bUpdate_Timer = false;

                if (bASMconnect == false)
                {
                    MessageBox.Show("해당 Tab은 사용 하실 수 없습니다.");
                    tabControl_ITS.SelectedIndex = 0;
                }
                comboBox_sel.SelectedIndex = AMM_Main.nDefaultGroup - 1;
            }

            //[210806_Sangik.choi_장기보관관리기능추가 by이종명수석님

            else if (tabNo == 4)
            {
                textBox_badge.Text = "";

                Fnc_Process_LongtermInfo();

                bUpdate_Timer = false;
            }
            else if (tabNo == 5)
            {
                SDTSort.Value = DateTime.Now.Date.AddDays(-1);
                EDTSort.Value = DateTime.Now.Date.AddDays(-1);

                SDTTower.Value = DateTime.Now.Date.AddDays(-1);
                EDTTower.Value = DateTime.Now.Date;
            }
        }

        //[210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님

        private void Fnc_Init_datagrid_capa()
        {
            List<DataGridView> list = new List<DataGridView>();

            list.Add(dataGridView_group1);
            list.Add(dataGridView_group2);
            list.Add(dataGridView_group3);
            list.Add(dataGridView_group4);
            list.Add(dataGridView_group5);
            list.Add(dataGridView_group6);
            list.Add(dataGridView_group7);
            list.Add(dataGridView_group8);//220823_ilyoung_타워그룹추가 DB 추가 해야 됨
            list.Add(dataGridView_group9);//220823_ilyoung_타워그룹추가
            list.Add(dgvCapaAll);

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Columns.Clear();
                list[i].Rows.Clear();
                list[i].Refresh();

                list[i].Columns.Add("Capa", "Capa");
                list[i].Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                list[i].Columns.Add("현재 수량", "현재 수량");
                list[i].Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                list[i].Columns.Add("입고 가능 수량", "입고 가능 수량");
                list[i].Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                list[i].Columns.Add("적재율(%)", "적재율(%)");
                list[i].Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                list[i].AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            }

            var MtlList = AMM_Main.AMM.Get_Capa_inch();

            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_update_capa.Text = "최근 업데이트: " + strToday + " " + strHead;

            int nMtlCount = MtlList.Rows.Count;




            if (MtlList.Rows.Count == 0)
            {
                MessageBox.Show("DB 연결 실패");
                return;
            }

            List<Inchdata> inch_list = new List<Inchdata>();
            int Tot7InchCnt = 0;
            int Tot13InchCnt = 0;
            int Tot7InchCapa = 0;
            int Tot13InchCapa = 0;


            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                Inchdata data = new Inchdata();

                data.Equipid = MtlList.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.Inch_7_cnt = MtlList.Rows[i]["INCH_7_CNT"].ToString(); data.Inch_7_cnt = data.Inch_7_cnt.Trim();
                data.Inch_13_cnt = MtlList.Rows[i]["INCH_13_CNT"].ToString(); data.Inch_13_cnt = data.Inch_13_cnt.Trim();
                data.Inch_7_capa = MtlList.Rows[i]["INCH_7_CAPA"].ToString(); data.Inch_7_capa = data.Inch_7_capa.Trim();
                data.Inch_13_capa = MtlList.Rows[i]["INCH_13_CAPA"].ToString(); data.Inch_13_capa = data.Inch_13_capa.Trim();
                data.Inch_7_rate = MtlList.Rows[i]["INCH_7_LOAD_RATE"].ToString(); data.Inch_7_rate = data.Inch_7_rate.Trim();
                data.Inch_13_rate = MtlList.Rows[i]["INCH_13_LOAD_RATE"].ToString(); data.Inch_13_rate = data.Inch_13_rate.Trim();

                Tot7InchCnt += int.Parse(data.Inch_7_cnt == "" ? "0" : data.Inch_7_cnt);  //220823_ilyoung_타워그룹추가
                Tot13InchCnt += int.Parse(data.Inch_13_cnt == "" ? "0" : data.Inch_13_cnt);//220823_ilyoung_타워그룹추가
                Tot7InchCapa += int.Parse(data.Inch_7_capa == "" ? "0" : data.Inch_7_capa);//220823_ilyoung_타워그룹추가
                Tot13InchCapa += int.Parse(data.Inch_13_capa == "" ? "0" : data.Inch_13_capa);//220823_ilyoung_타워그룹추가

                string inch_7_cal = (Int32.Parse(data.Inch_7_capa == "" ? "0" : data.Inch_7_capa) - Int32.Parse(data.Inch_7_cnt == "" ? "0" : data.Inch_7_cnt)).ToString();//220823_ilyoung_타워그룹추가
                string inch_13_cal = (Int32.Parse(data.Inch_13_capa == "" ? "0" : data.Inch_13_capa) - Int32.Parse(data.Inch_13_cnt == "" ? "0" : data.Inch_13_cnt)).ToString();//220823_ilyoung_타워그룹추가


                if (MtlList.Rows[i]["EQUIP_ID"].ToString().Contains("TWR") == true)
                {
                    list[int.Parse(MtlList.Rows[i]["EQUIP_ID"].ToString().Replace("TWR", "")) - 1].Rows.Add(new object[4] { data.Inch_7_capa, data.Inch_7_cnt, inch_7_cal, data.Inch_7_rate });
                    list[int.Parse(MtlList.Rows[i]["EQUIP_ID"].ToString().Replace("TWR", "")) - 1].Rows.Add(new object[4] { data.Inch_13_capa, data.Inch_13_cnt, inch_13_cal, data.Inch_13_rate });

                    list[int.Parse(MtlList.Rows[i]["EQUIP_ID"].ToString().Replace("TWR", "")) - 1].Rows[0].HeaderCell.Value = "7\"";
                    list[int.Parse(MtlList.Rows[i]["EQUIP_ID"].ToString().Replace("TWR", "")) - 1].Rows[1].HeaderCell.Value = "13\"";

                    list[int.Parse(MtlList.Rows[i]["EQUIP_ID"].ToString().Replace("TWR", "")) - 1].Rows[0].Cells[2].Style.ForeColor = Color.Red;
                    list[int.Parse(MtlList.Rows[i]["EQUIP_ID"].ToString().Replace("TWR", "")) - 1].Rows[1].Cells[2].Style.ForeColor = Color.Red;
                }
            }

            string TotInch7Cal = (Tot7InchCapa - Tot7InchCnt).ToString();
            string TotInch13Cal = (Tot13InchCapa - Tot13InchCnt).ToString();

            list[list.Count - 1].Rows.Add(new object[4] { Tot7InchCapa, Tot7InchCnt, TotInch7Cal, Math.Round(((double)Tot7InchCnt / (double)Tot7InchCapa) * 100, 2).ToString() });
            list[list.Count - 1].Rows.Add(new object[4] { Tot13InchCapa, Tot13InchCnt, TotInch13Cal, Math.Round(((double)Tot13InchCnt / (double)Tot13InchCapa) * 100, 2).ToString() });
            list[list.Count - 1].Rows[0].HeaderCell.Value = "7\"";
            list[list.Count - 1].Rows[1].HeaderCell.Value = "13\"";
            list[list.Count - 1].Rows[0].DefaultCellStyle.BackColor = Color.Yellow;
            list[list.Count - 1].Rows[1].DefaultCellStyle.BackColor = Color.Yellow;
            list[list.Count - 1].Rows[0].Cells[2].Style.ForeColor = Color.Red;
            list[list.Count - 1].Rows[1].Cells[2].Style.ForeColor = Color.Red;
            list[list.Count - 1].DefaultCellStyle.SelectionBackColor = Color.Yellow;
            list[list.Count - 1].ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow;
            list[list.Count - 1].RowHeadersDefaultCellStyle.BackColor = Color.Yellow;
        }

        //]210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님


        private void Fnc_Init_datagrid(int nNum)
        {
            if (nNum == 0)
            {
                dataGridView_info.Columns.Clear();
                dataGridView_info.Rows.Clear();
                dataGridView_info.Refresh();

                dataGridView_info.Columns.Add("NO", "NO");
                dataGridView_info.Columns.Add("SID", "SID");
                dataGridView_info.Columns.Add("릴 수량", "릴 수량");
                dataGridView_info.Columns.Add("Qty", "Qty");
                dataGridView_info.Columns.Add("인치", "인치");
                dataGridView_info.Columns.Add("위치", "위치");
            }
            else if (nNum == 1)
            {
                dataGridView_info.Columns.Clear();
                dataGridView_info.Rows.Clear();
                dataGridView_info.Refresh();

                dataGridView_info.Columns.Add("NO", "NO");
                dataGridView_info.Columns.Add("SID", "SID");
                dataGridView_info.Columns.Add("AMKOR_BATCH", "Batch#");
                dataGridView_info.Columns.Add("UID", "UID");
                dataGridView_info.Columns.Add("QTY", "Qty");
                dataGridView_info.Columns.Add("INPUT_TYPE", "투입형태");
                dataGridView_info.Columns.Add("TOWER_NO", "위치");
                dataGridView_info.Columns.Add("PRODUCTION_DATE", "제조일");
                dataGridView_info.Columns.Add("DATETIME", "투입일");
                dataGridView_info.Columns.Add("MANUFACTURER", "제조사");
                dataGridView_info.Columns.Add("INCH_INFO", "인치");
            }
            else
            {
                dataGridView_sum.Columns.Clear();
                dataGridView_sum.Rows.Clear();
                dataGridView_sum.Refresh();

                dataGridView_sum.Columns.Add("TWR", "TWR");
                dataGridView_sum.Columns.Add("GROUP #1", "GROUP #1");
                dataGridView_sum.Columns.Add("GROUP #2", "GROUP #2");
                dataGridView_sum.Columns.Add("GROUP #3", "GROUP #3");
                dataGridView_sum.Columns.Add("GROUP #4", "GROUP #4");
                dataGridView_sum.Columns.Add("GROUP #5", "GROUP #5");
                dataGridView_sum.Columns.Add("GROUP #6", "GROUP #6");
                dataGridView_sum.Columns.Add("GROUP #7", "GROUP #7");//210831_Sangik.choi_타워그룹추가
                dataGridView_sum.Columns.Add("GROUP #8", "GROUP #8");//220823_ilyoung_타워그룹추가
                dataGridView_sum.Columns.Add("GROUP #9", "GROUP #9");//220823_ilyoung_타워그룹추가

            }
        }

        private void Fnc_Init_datagrid2(int nNum)
        {
            label_incount.Text = "-";
            label_returncount.Text = "-";
            label_outcount.Text = "-";

            if (nNum == 0)
            {
                dataGridView_input.Columns.Clear();
                dataGridView_input.Rows.Clear();
                dataGridView_input.Refresh();

                dataGridView_input.Columns.Add("NO", "NO");
                dataGridView_input.Columns.Add("SID", "SID");
                dataGridView_input.Columns.Add("릴 수량", "릴 수량");
                dataGridView_input.Columns.Add("Qty", "Qty");
                dataGridView_input.Columns.Add("인치", "인치");

                dataGridView_return.Columns.Clear();
                dataGridView_return.Rows.Clear();
                dataGridView_return.Refresh();

                dataGridView_return.Columns.Add("NO", "NO");
                dataGridView_return.Columns.Add("SID", "SID");
                dataGridView_return.Columns.Add("릴 수량", "릴 수량");
                dataGridView_return.Columns.Add("Qty", "Qty");
                dataGridView_return.Columns.Add("인치", "인치");

                dataGridView_output.Columns.Clear();
                dataGridView_output.Rows.Clear();
                dataGridView_output.Refresh();

                dataGridView_output.Columns.Add("NO", "NO");
                dataGridView_output.Columns.Add("SID", "SID");
                dataGridView_output.Columns.Add("릴 수량", "릴 수량");
                dataGridView_output.Columns.Add("Qty", "Qty");
                dataGridView_output.Columns.Add("인치", "인치");
            }
            else if (nNum == 1)
            {
                dataGridView_input.Columns.Clear();
                dataGridView_input.Rows.Clear();
                dataGridView_input.Refresh();

                dataGridView_input.Columns.Add("NO", "NO");
                dataGridView_input.Columns.Add("일자", "일자");
                dataGridView_input.Columns.Add("시간", "시간");
                dataGridView_input.Columns.Add("SID", "SID");
                dataGridView_input.Columns.Add("Batch#", "Batch#");
                dataGridView_input.Columns.Add("UID", "UID");
                dataGridView_input.Columns.Add("Qty", "Qty");
                dataGridView_input.Columns.Add("투입형태", "투입형태");
                dataGridView_input.Columns.Add("위치", "위치");
                dataGridView_input.Columns.Add("제조일", "제조일");
                dataGridView_input.Columns.Add("제조사", "제조사");
                dataGridView_input.Columns.Add("인치", "인치");

                dataGridView_return.Columns.Clear();
                dataGridView_return.Rows.Clear();
                dataGridView_return.Refresh();

                dataGridView_return.Columns.Add("NO", "NO");
                dataGridView_return.Columns.Add("일자", "일자");
                dataGridView_return.Columns.Add("시간", "시간");
                dataGridView_return.Columns.Add("SID", "SID");
                dataGridView_return.Columns.Add("Lot#", "Lot#");
                dataGridView_return.Columns.Add("UID", "UID");
                dataGridView_return.Columns.Add("Qty", "Qty");
                dataGridView_return.Columns.Add("투입형태", "투입형태");
                dataGridView_return.Columns.Add("위치", "위치");
                dataGridView_return.Columns.Add("제조일", "제조일");
                dataGridView_return.Columns.Add("제조사", "제조사");
                dataGridView_return.Columns.Add("인치", "인치");

                dataGridView_output.Columns.Clear();
                dataGridView_output.Rows.Clear();
                dataGridView_output.Refresh();

                dataGridView_output.Columns.Add("NO", "NO");
                dataGridView_output.Columns.Add("일자", "일자");
                dataGridView_output.Columns.Add("시간", "시간");
                dataGridView_output.Columns.Add("SID", "SID");
                dataGridView_output.Columns.Add("Batch#", "Batch#");
                dataGridView_output.Columns.Add("UID", "UID");
                dataGridView_output.Columns.Add("수량", "수량");
                dataGridView_output.Columns.Add("인치", "인치");
                dataGridView_output.Columns.Add("배출ID", "배출ID");
                dataGridView_output.Columns.Add("요청자", "요청자");
                dataGridView_output.Columns.Add("위치", "위치");
                dataGridView_output.Columns.Add("Type", "Type");
            }
        }



        //[210806_Sangik.choi_장기보관관리기능추가 by이종명수석님
        private void Fnc_Init_datagrid_longterm()
        {
            dataGridView_longterm.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView_longterm.Columns.Clear();
            dataGridView_longterm.Rows.Clear();
            dataGridView_longterm.Refresh();

            dataGridView_longterm.Columns.Add("SID", "SID");
            dataGridView_longterm.Columns.Add("Batch#", "Batch#");
            dataGridView_longterm.Columns.Add("UID", "UID");
            dataGridView_longterm.Columns.Add("Qty", "Qty");
            dataGridView_longterm.Columns.Add("투입형태", "투입형태");
            dataGridView_longterm.Columns.Add("위치", "위치");
            dataGridView_longterm.Columns.Add("제조일", "제조일");
            dataGridView_longterm.Columns.Add("투입일", "투입일");
            dataGridView_longterm.Columns.Add("제조사", "제조사");
            dataGridView_longterm.Columns.Add("인치", "인치");


        }
        //]210806_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[210806_Sangik.choi_장기보관관리기능추가 by이종명수석님

        public void Fnc_Process_LongtermInfo()
        {
            // IsDateGathering = true;
            Fnc_Init_datagrid_longterm();

            Application.DoEvents();

            comboBox_month.SelectedIndex = 0;
            comboBox_L_group.SelectedIndex = 0;


            int nMonth = comboBox_month.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_L_group.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();


            /*           if (nGroup != 7)
                           Fnc_Process_GetMaterialinfo_longterm(1, strEquipid);
                       else
                       {
                           Fnc_Process_GetMaterialinfo_All(1);
                       }*/

            //IsDateGathering = false;
        }

        //]210806_Sangik.choi_장기보관관리기능추가 by이종명수석님


        public void Fnc_Process_CalMaterialInfo()
        {
            IsDateGathering = true;

            Fnc_Init_datagrid(2); //Init

            Application.DoEvents();

            int[] nCount = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//210831_Sangik.choi_타워그룹추가  //220823_ilyoung_타워그룹추가

            DataTable MtlList = null;

            try
            {
                string strTowerNo = "", strEquip = "";
                for (int n = 1; n < 5; n++)
                {
                    strEquip = "TWR1"; strTowerNo = string.Format("T010{0}", n.ToString());
                    //GetMTLInfo()-query = string.Format(@"SELECT * FROM TB_MTL_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}' and TOWER_NO='{2}');
                    nCount[0] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    strEquip = "TWR2"; strTowerNo = string.Format("T020{0}", n.ToString());
                    nCount[1] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    strEquip = "TWR3"; strTowerNo = string.Format("T030{0}", n.ToString());
                    nCount[2] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    strEquip = "TWR4"; strTowerNo = string.Format("T040{0}", n.ToString());
                    nCount[3] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    strEquip = "TWR5"; strTowerNo = string.Format("T050{0}", n.ToString());
                    nCount[4] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    strEquip = "TWR6"; strTowerNo = string.Format("T060{0}", n.ToString());
                    nCount[5] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;


                    //[210831_Sangik.choi_타워그룹추가
                    strEquip = "TWR7"; strTowerNo = string.Format("T070{0}", n.ToString());
                    nCount[6] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    //dataGridView_sum.Rows.Add(new object[8] { n.ToString(), nCount[0].ToString(), nCount[1].ToString(), nCount[2].ToString(), nCount[3].ToString(), nCount[4].ToString(), nCount[5].ToString(), nCount[6].ToString() });
                    //]210831_Sangik.choi_타워그룹추가

                    //220823_ilyoung_타워그룹추가
                    strEquip = "TWR8"; strTowerNo = string.Format("T080{0}", n.ToString());
                    nCount[7] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    //dataGridView_sum.Rows.Add(new object[8] { n.ToString(), nCount[0].ToString(), nCount[1].ToString(), nCount[2].ToString(), nCount[3].ToString(), nCount[4].ToString(), nCount[5].ToString(), nCount[6].ToString() });

                    strEquip = "TWR9"; strTowerNo = string.Format("T090{0}", n.ToString());
                    nCount[8] = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo).Rows.Count;

                    dataGridView_sum.Rows.Add(new object[10] { n.ToString(), nCount[0].ToString(), nCount[1].ToString(), nCount[2].ToString(), nCount[3].ToString(), nCount[4].ToString(), nCount[5].ToString(), nCount[6].ToString(), nCount[7].ToString(), nCount[8].ToString() });
                    //220823_ilyoung_타워그룹추가
                }

                int[] nSum = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//210831_Sangik.choi_타워그룹추가 //220823_ilyoung_타워그룹추가
                string[] strSum = new string[9] { "", "", "", "", "", "", "", "", "" };//210831_Sangik.choi_타워그룹추가 //220823_ilyoung_타워그룹추가
                int nTotal = 0;

                for (int j = 0; j < nSum.Length; j++)//210831_Sangik.choi_타워그룹추가 //220823_ilyoung_타워그룹추가
                {
                    for (int i = 0; i < dataGridView_sum.Rows.Count; i++)   //220823_ilyoung_타워그룹추가
                    {
                        try
                        {
                            int nCal = Int32.Parse(dataGridView_sum.Rows[i].Cells[j + 1].Value.ToString().Replace(",", ""));
                            nSum[j] = nSum[j] + nCal;
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }

                    }

                    strSum[j] = string.Format("{0:0,0}", nSum[j]);
                    nTotal = nTotal + nSum[j];
                }

                dataGridView_sum.Rows.Add(new object[10] { "SUM", strSum[0].ToString(), strSum[1].ToString(), strSum[2].ToString(), strSum[3].ToString(), strSum[4].ToString(), strSum[5].ToString(), strSum[6].ToString(), strSum[7].ToString(), strSum[8].ToString() });//210831_Sangik.choi_타워그룹추가  //220823_ilyoung_타워그룹추가
                dataGridView_sum.Rows[4].DefaultCellStyle.ForeColor = Color.White;
                dataGridView_sum.Rows[4].DefaultCellStyle.BackColor = Color.OrangeRed;
                dataGridView_sum.Rows[4].DefaultCellStyle.Font = new Font("Calibri", 13.00F, FontStyle.Bold);
                dataGridView_sum.Rows[0].Selected = false;
                dataGridView_sum.Rows[4].Selected = false;

                string strnQty = string.Format("{0:0,0}", nTotal);
                label_total.Text = strnQty + " REEL";

                string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                label_updatedate.Text = "최근 업데이트: " + strToday + " " + strHead;

                //////Infomation
                int nType = comboBox_type.SelectedIndex; //0: SID, 1:Detail info
                int nGroup = comboBox_group.SelectedIndex + 1;

                string strEquipid = "TWR" + nGroup.ToString();

                Fnc_Init_datagrid(nType);

                if (nGroup != 10)//210831_Sangik.choi_타워그룹추가  //220823_ilyoung_타워그룹추가
                    Fnc_Process_GetMaterialinfo(nType, strEquipid);
                else
                {
                    Fnc_Process_GetMaterialinfo_All(nType);
                }

                IsDateGathering = false;
            }
            catch (Exception ex)
            {


            }


        }

        private int Fnc_Process_GetMaterialinfo_longterm_All(int month)
        {
            /**
            *dataGridView_longterm.Columns.Add("SID", "SID");
            *dataGridView_longterm.Columns.Add("Batch#", "Batch#");
            *dataGridView_longterm.Columns.Add("UID", "UID");
            *dataGridView_longterm.Columns.Add("Qty", "Qty");
            *dataGridView_longterm.Columns.Add("투입형태", "투입형태");
            *dataGridView_longterm.Columns.Add("위치", "위치");
            *dataGridView_longterm.Columns.Add("제조일", "제조일");
            *dataGridView_longterm.Columns.Add("투입일", "투입일");
            *dataGridView_longterm.Columns.Add("제조사", "제조사");
            *dataGridView_longterm.Columns.Add("인치", "인치");
             */
            dataGridView_longterm.Rows.Clear();

            DataTable dt = AMM_Main.AMM.MSSql.GetData($"Select [DATETIME],[LINE_CODE],[EQUIP_ID],[TOWER_NO],[UID],[SID],[LOTID],[QTY],[MANUFACTURER],[PRODUCTION_DATE]" +
                $",[INCH_INFO],[INPUT_TYPE],[AMKOR_BATCH] from TB_MTL_INFO with(nolock) " +
                $"where  [DATETIME] < REPLACE(REPLACE(REPLACE(CONVERT(varchar, dateadd(MONTH, -{month}, getdate()), 20), '-', ''), ' ', ''), ':', '') " +
                $"and (EQUIP_ID = 'TWR1' OR EQUIP_ID = 'TWR2' OR EQUIP_ID = 'TWR3') order by TOWER_NO");

            foreach (DataRow row in dt.Rows)
            {
                dataGridView_longterm.Rows.Add(new object[] {row["SID"].ToString(), row["AMKOR_BATCH"].ToString(), row["UID"].ToString(), row["QTY"].ToString(),
                    row["INPUT_TYPE"].ToString(), row["TOWER_NO"].ToString(), row["PRODUCTION_DATE"].ToString(), row["DATETIME"].ToString(), row["MANUFACTURER"].ToString(), row["INCH_INFO"].ToString() });
            }


            GetTowerSerial();

            DataSet ds = new DataSet();

            for (int i = 4; i <= 9; i++)
            {
                ds = GetMycronicData(i, $"Select [Carrier],[ArticleName],[DepotDate],[Depot],[Custom1],[Manufactur],[CreateDate],[Diameter],[Stock] from TCarrier with(nolock) where [DepotDate] <= DATEADD(MONTH, -{month},GETDATE()) order by Depot");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    dataGridView_longterm.Rows.Add(new object[]{row["ArticleName"].ToString(), "", row["Carrier"].ToString(), row["Stock"],
                    "", Tower_serial[row["Depot"].ToString().Split('.')[0]], row["CreateDate"].ToString(), row["DepotDate"].ToString(), row["Manufactur"].ToString(), row["Diameter"].ToString()});
                }
            }

            return 0;
            //DataSet ds = new DataSet();

            //for(int  i = 4; i <= 9; i++)
            //{
            //    ds = GetMycronicData(i, $"Select [Carrier],[ArticleName],[DepotDate],[Depot],[Custom1],[Manufactur],[CreateDate],[Diameter] form TCarrier with(nolock) where [DeportDate] <= DATEADD(MONTH,-{comboBox_month.SelectedIndex + 1})");

            //    for(int j = 0; ds.Tables[0].)
            //}

            //var MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquipid);

            //var today = DateTime.Now;

            //int month = comboBox_month.SelectedIndex + 1;

            //string format = "yyyyMMddHHmmss";

            //strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            //int nMtlCount = MtlList.Rows.Count;

            //if (MtlList.Rows.Count == 0)
            //{
            //    return nMtlCount;
            //}

            //List<StorageData> list = new List<StorageData>();

            //for (int i = 0; i < MtlList.Rows.Count; i++)
            //{
            //    StorageData data = new StorageData();

            //    data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
            //    data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
            //    data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
            //    data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
            //    data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
            //    data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
            //    data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
            //    data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
            //    data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
            //    data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

            //    //[2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님

            //    DateTime dt = DateTime.ParseExact(data.Input_date, format, null);
            //    DateTime dt_temp = today.AddMonths(-month);

            //    int result = DateTime.Compare(dt, dt_temp);


            //    if (result < 0)
            //    {
            //        list.Add(data);

            //    }
            //    //]2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님


            //}

            //list.Sort(sortlist_date);

            //foreach (var item in list)
            //{
            //    //string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));  //210818_Sangik_choi_입출고 조회중 DB 오류로 삭제
            //    string strdate = item.Input_date;
            //    strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
            //        + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

            //    dataGridView_longterm.Rows.Add(new object[10] { item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
            //}

            //return nMtlCount;


        }


        private void Fnc_Process_GetMycronicinfo_longterm(int month, int strEquipid)
        {
            GetTowerSerial();

            DataSet ds = new DataSet();
            
            ds = GetMycronicData(strEquipid, $"Select [Carrier],[ArticleName],[DepotDate],[Depot],[Custom1],[Manufactur],[CreateDate],[Diameter],[Stock] from TCarrier with(nolock) where [DepotDate] <= DATEADD(MONTH, -{month},GETDATE()) order by Depot");

            foreach(DataRow row in ds.Tables[0].Rows)
            {
                dataGridView_longterm.Rows.Add(new object[]{row["Custom1"].ToString(), "", row["Carrier"].ToString(), row["Stock"],
                row["Diameter"].ToString(), Tower_serial[row["Depot"].ToString().Split('.')[0]], row["CreateDate"].ToString(), row["DepotDate"].ToString(), row["Manufactur"].ToString(), row["Diameter"].ToString()});
            }
            
        }

        //[2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님


        private int Fnc_Process_GetMaterialinfo_longterm(int nType, string strEquipid)
        {
            var MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquipid);

            var today = DateTime.Now;

            int month = nType;

            string format = "yyyyMMddHHmmss";

            strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData> list = new List<StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                //[2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님

                DateTime dt = DateTime.ParseExact(data.Input_date, format, null);
                DateTime dt_temp = today.AddMonths(-month);

                int result = DateTime.Compare(dt, dt_temp);


                if (result < 0)
                {
                    list.Add(data);

                }
                //]2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님


            }

            list.Sort(sortlist_date);

            foreach (var item in list)
            {
                //string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));  //210818_Sangik_choi_입출고 조회중 DB 오류로 삭제
                string strdate = item.Input_date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                dataGridView_longterm.Rows.Add(new object[10] { item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
            }

            return nMtlCount;


        }

        //]2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님

        int sortlist_date(StorageData obj1, StorageData obj2)
        {
            return obj1.Input_date.CompareTo(obj2.Input_date);
        }

        //]2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님

        private void Fnc_Get_PickID(string strGroupinfo)
        {
            // GetPickIDNo - query = string.Format(@"SELECT * FROM TB_IDNUNMER_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);

            ///Pick id load
            string equipid = strGroupinfo;
            var tableList = AMM_Main.AMM.GetPickIDNo(AMM_Main.strDefault_linecode, equipid);

            if (tableList.Rows.Count == 0)
            {
                if (strGroupinfo == "TWR1")
                    label_pickid_LT.Text = "PD0000001";
                else if (strGroupinfo == "TWR2")
                    label_pickid_LT.Text = "PE0000001";
                else if (strGroupinfo == "TWR3")
                    label_pickid_LT.Text = "PF0000001";
                else if (strGroupinfo == "TWR4")
                    label_pickid_LT.Text = "PG0000001";
                else if (strGroupinfo == "TWR5")
                    label_pickid_LT.Text = "PH0000001";
                else if (strGroupinfo == "TWR6")
                    label_pickid_LT.Text = "PJ0000001";
                //220823_ilyoung_타워그룹추가
                else if (strGroupinfo == "TWR7")
                    label_pickid_LT.Text = "PK0000001";
                else if (strGroupinfo == "TWR8")
                    label_pickid_LT.Text = "PL0000001";
                else if (strGroupinfo == "TWR9")
                    label_pickid_LT.Text = "PM0000001";
                //220823_ilyoung_타워그룹추가

                //[20210805_Sangik.choi_타워그룹추가
                /*                else if (strGroupinfo == "7")
                                    label_pickid_LT.Text = "PK0000001";*/
            //]20210805_Sangik.choi_타워그룹추가

        }
            else
            {
                string strprefix = tableList.Rows[0]["PICK_PREFIX"].ToString();
                strprefix = strprefix.Trim();
                string strNo = tableList.Rows[0]["PICK_NUM"].ToString();
                strNo = strNo.Trim();

                label_pickid_LT.Text = strprefix + strNo;
            }


            string strPickingID = label_pickid_LT.Text;
            string strDefaultPickingID = "";


            if (AMM_Main.strDefault_Group == strGroupinfo)
                strDefaultPickingID = strPickingID;


            Fnc_Update_PickID(AMM_Main.strDefault_linecode, equipid, strPickingID);


        }

        //]2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[210813_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void Fnc_Update_PickID(string strlinecode, string streqid, string strCurPickID)
        {
            string strGetNo = strCurPickID.Substring(strCurPickID.Length - 7);
            string strGetPrefix = strCurPickID.Substring(0, 2);

            int nGetNo = Int32.Parse(strGetNo);

            if (nGetNo == 9999999)
                nGetNo = 0;

            nGetNo = nGetNo + 1;
            strGetNo = nGetNo.ToString();
            int nLength = strGetNo.Length;
            char[] chSetNo = new char[7];

            for (int n = 0; n < 7 - nLength; n++)
            {
                chSetNo[n] = '0';
            }

            for (int m = 0; m < nLength; m++)
            {
                chSetNo[7 - nLength + m] = strGetNo.Substring(m, 1)[0];
            }

            string text = new string(chSetNo);
            AMM_Main.AMM.Delete_PickIDNo(strlinecode, streqid);
            AMM_Main.AMM.SetPickIDNo(strlinecode, streqid, strGetPrefix, text);
        }

        //]210813_Sangik.choi_장기보관관리기능추가(이종명수석님)


        private int Fnc_Process_GetMaterialinfo(int nType, string strEquipid)
        {
            //GetMTLInfo()-query = string.Format(@"SELECT * FROM TB_MTL_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);

            var MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquipid);

            strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData> list = new List<StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                list.Add(data);
            }

            list.Sort(CompareStorageData);

            int nIndex = 1;

            if (nType == 0) //SID
            {
                string strSetSID = "", strinch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < nMtlCount; i++)
                {
                    if (strSetSID != list[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            //string strinch = list[i].Inch;
                            dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strEquipid });

                            strSetSID = list[i].SID;
                            strinch = list[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list[i].Quantity == "" ? "0" : list[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list[i].SID;
                            strinch = list[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list[i].Quantity == "" ? "0" : list[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list[i].Quantity == "" ? "0" : list[i].Quantity);
                    }

                    if (i == nMtlCount - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        //string strinch = list[i].Inch;
                        dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strEquipid });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                foreach (var item in list)
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity == "" ? "0" : item.Quantity));
                    string strdate = item.Input_date;
                    strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                        + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                    dataGridView_info.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
                }

            }
            else
            {
                return nMtlCount;
            }

            return nMtlCount;
        }

        DateTime FindTime = new DateTime();

        private int Fnc_Process_GetMaterialinfo_All(int nType) //nType 0 : SID, 1: 상세 정보
        {
            FindTime = isClick == true ? DateTime.Now : new DateTime();

            DataTable MtlList = null;

            List<StorageData> list = new List<StorageData>();

            MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Equipid = MtlList.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                list.Add(data);
            }

            list.Sort(CompareStorageData);

            

            int nIndex = 1;

            if (nType == 0) //SID
            {
                


                string strSetSID = "", strLocation = "", strLocation_before = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < nMtlCount; i++)
                {
                    if (strSetSID != list[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            string strinch = list[i].Inch;
                            dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strLocation });

                            strSetSID = list[i].SID;
                            strLocation = list[i].Equipid;
                            strLocation_before = list[i].Equipid;
                            //strInch = list[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list[i].Quantity == "" ? "0" : list[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            if (strLocation_before != list[i].Equipid)
                            {
                                if (strLocation == "")
                                    strLocation = list[i].Equipid;
                                else
                                {
                                    if (!strLocation.Contains(list[i].Equipid))
                                        strLocation = strLocation + "," + list[i].Equipid;
                                }
                            }

                            strSetSID = list[i].SID;
                            strLocation_before = list[i].Equipid;
                            //strInch = list[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list[i].Quantity == "" ? "0" : list[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list[i].Quantity == "" ? "0" : list[i].Quantity);

                        if (strLocation_before != list[i].Equipid)
                        {
                            if (strLocation == "")
                                strLocation = list[i].Equipid;
                            else
                            {
                                if (!strLocation.Contains(list[i].Equipid))
                                    strLocation = strLocation + "," + list[i].Equipid;
                            }
                        }

                        strLocation_before = list[i].Equipid;
                    }

                    if (i == nMtlCount - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        string strinch = list[i].Inch;
                        dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strLocation });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                dataGridView_info.Columns.Clear();
                dataGridView_info.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;                
                dataGridView_info.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                dataGridView_info.RowHeadersVisible = false;
                dataGridView_info.SuspendLayout();

                dataGridView_info.Columns.Add("DATETIME", "투입일");
                dataGridView_info.Columns.Add("LINE_CODE", "LINE CODE");
                dataGridView_info.Columns.Add("EQUIP_ID", "그룹");
                dataGridView_info.Columns.Add("TOWER_NO", "위치");
                dataGridView_info.Columns.Add("UID", "UID");
                dataGridView_info.Columns.Add("SID", "SID");
                dataGridView_info.Columns.Add("LOTID", "LOT");
                dataGridView_info.Columns.Add("QTY", "Qty");
                dataGridView_info.Columns.Add("MANUFACTURER", "제조사");
                dataGridView_info.Columns.Add("PRODUCTION_DATE", "제조일");
                dataGridView_info.Columns.Add("INCH_INFO", "인치");
                dataGridView_info.Columns.Add("INPUT_TYPE", "투입형태");
                dataGridView_info.Columns.Add("AMKOR_BATCH", "Batch#");

                //dataGridView_info.DataSource = MtlList;

                //foreach(DataColumn col in MtlList.Columns)
                //{
                //    var c = new DataGridViewTextBoxColumn() { HeaderText = col.ColumnName };
                //    dataGridView_info.Columns.Add(c);
                //}


                //dataGridView_info.DataSource = MtlList;

                foreach (DataRow row in MtlList.Rows)
                {                    
                    dataGridView_info.Rows.Add(row.ItemArray);
                }

                /*
                foreach (var item in list)
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    string strdate = item.Input_date;
                    strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                        + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                    dataGridView_info.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
                }
                */

                //dataGridView_info.RowHeadersVisible = true;
                dataGridView_info.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                dataGridView_info.ResumeLayout();
                
                MtlList = null;
            }
            else
            {                
                MtlList = null;
                return nMtlCount;
            }



            MtlList = null;
            return nMtlCount;
        }

        private void Fnc_Process_GetMaterialinfo_DetailAll()//상세 정보
        {
            DataTable MtlList = null;

            List<StorageData> list = new List<StorageData>();

            MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return;
            }

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Equipid = MtlList.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                string str = data.Tower_no.Substring(2, 1);
                int nTwr = Int32.Parse(str) - 1;
                if (bGroupUse[nTwr])
                {
                    list.Add(data);
                }
            }
            MtlList = null;

            list.Sort(CompareStorageData);

            int nIndex = 1;

            foreach (var item in list)
            {
                string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity == "" ? "0" : item.Quantity));
                string strdate = item.Input_date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                dataGridView_info.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
                Application.DoEvents();
            }
        }




        int CompareStorageData(StorageData obj1, StorageData obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
        }

        int CompareStorageData2(StorageData2 obj1, StorageData2 obj2)
        {
            return obj1.Creation_date.CompareTo(obj2.Creation_date);
        }

        int CompareStorageData3(StorageData2 obj1, StorageData2 obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
        }

        public void Fnc_ProcessFind(int nType, string strMtl)
        {
            List<StorageData> list = new List<StorageData>();

            DataTable MtlList = null;

            string strEquipid = "TWR";
            bool bSearch = false;
            string strnQty = "";

            if (strMtl.Length == 4 || nType == 1)
                comboBox_sid.Items.Clear();


            for (int j = 1; j < 10; j++)//220823_ilyoung_타워그룹추가
            {
                MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquipid + j.ToString());

                for (int i = 0; i < MtlList.Rows.Count; i++)
                {
                    StorageData data = new StorageData();

                    data.Equipid = strEquipid + j.ToString();
                    data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                    data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                    data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                    data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                    data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                    data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                    data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                    data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                    data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                    data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                    if (nType == 0)
                    {
                        if (strMtl.Length == 4)
                        {
                            string strCheck = data.SID.Substring(data.SID.Length - 4);
                            if (strMtl == strCheck)
                            {
                                list.Add(data);

                                int nCombocount = comboBox_sid.Items.Count;
                                bool bjudge = false;
                                for (int k = 0; k < nCombocount; k++)
                                {
                                    string str = comboBox_sid.Items[k].ToString();

                                    if (data.SID == str)
                                    {
                                        bjudge = true;
                                    }
                                }

                                if (!bjudge)
                                {
                                    comboBox_sid.Items.Add(data.SID);
                                }
                            }
                        }
                        else
                        {
                            if (data.SID == strMtl)
                            {
                                list.Add(data);
                            }
                        }
                    }
                    else
                    {
                        if (data.UID == strMtl)
                        {
                            strnQty = string.Format("{0:0,0}", Int32.Parse(data.Quantity == "" ? "0" : data.Quantity));

                            label_info1.Text = data.SID;
                            label_info2.Text = strEquipid + j.ToString();
                            label_info2.Text = label_info2.Text.Replace("TWR", "G");
                            label_info3.Text = "1";
                            label_info4.Text = strnQty;

                            bSearch = true;
                        }
                    }
                }
            }
            list.Sort(CompareStorageData);

            if (bSearch)
                return;

            if (list.Count == 0 || (nType == 1 && bSearch == false))
            {
                label_info1.Text = "-";
                label_info2.Text = "자재 없음!";
                label_info3.Text = "-";
                label_info4.Text = "-";

                return;
            }

            string strLocation = "";
            double nQty = 0;

            for (int i = 0; i < list.Count; i++)
            {
                nQty = nQty + Int32.Parse(list[i].Quantity == "" ? "0" : list[i].Quantity);
                if (strLocation != list[i].Equipid)
                {
                    if (strLocation == "")
                    {
                        strLocation = list[i].Equipid;
                    }
                    else
                    {
                        if (!strLocation.Contains(list[i].Equipid))
                            strLocation = strLocation + "," + list[i].Equipid;
                    }

                }
            }

            strnQty = string.Format("{0:0,0}", nQty);

            label_info1.Text = list[0].SID;
            label_info2.Text = strLocation;
            label_info2.Text = label_info2.Text.Replace("TWR", "G");
            label_info3.Text = list.Count.ToString();
            label_info4.Text = strnQty;
        }

        public int Fnc_Process_GetINOUT_mtlinfo(int nType, string strEquipid, double strTime_st, double strTime_ed)
        {
            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_updatedate2.Text = "최근 업데이트: " + strToday + " " + strHead;

            var MtlList = new DataTable();

            if (strEquipid != "ALL")
                MtlList = AMM_Main.AMM.GetInouthistroy(AMM_Main.strDefault_linecode, strEquipid, strTime_st, strTime_ed);
            else
            {
                string group_name = "";
                var TableData = new DataTable();
                for (int i = 0; i < comboBox_group2.Items.Count; i++)
                {
                    group_name = comboBox_group2.Items[i].ToString();

                    if (group_name != "ALL" && group_name != "")
                    {
                        TableData = AMM_Main.AMM.GetInouthistroy(AMM_Main.strDefault_linecode, "TWR" + (i + 1).ToString(), strTime_st, strTime_ed);

                        if (i == 0)
                        {
                            MtlList = TableData;
                        }
                        else
                        {
                            for (int j = 0; j < TableData.Rows.Count; j++)
                            {

                                MtlList.Rows.Add(TableData.Rows[j].ItemArray);
                            }
                        }
                    }
                }
            }

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }


            List<StorageData2> list_input = new List<StorageData2>();
            List<StorageData2> list_return = new List<StorageData2>();
            List<StorageData2> list_out = new List<StorageData2>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData2 data = new StorageData2();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Creation_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Creation_date = data.Creation_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();
                data.pickid = MtlList.Rows[i]["PICKID"].ToString(); data.pickid = data.pickid.Trim();
                data.Status = MtlList.Rows[i]["STATUS"].ToString(); data.Status = data.Status.Trim();
                data.Requestor = MtlList.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();

                if (data.Status == "IN" && data.Input_type == "CART")
                    list_input.Add(data);
                else if (data.Status == "IN" && data.Input_type == "RETURN")
                    list_return.Add(data);
                else if (data.Status == "OUT" || data.Status == "OUT-MANUAL")
                    list_out.Add(data);
            }

            int nIndex = 1;

            if (nType == 0) //SID
            {
                list_input.Sort(CompareStorageData3);

                string strSetSID = "", strInch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < list_input.Count; i++)
                {
                    if (strSetSID != list_input[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity == "" ? "" : list_input[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity == "" ? "" : list_input[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_input[i].Quantity == "" ? "" : list_input[i].Quantity);
                    }

                    if (i == list_input.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_return.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_return.Count; i++)
                {
                    if (strSetSID != list_return[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity == "" ? "0" : list_return[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity == "" ? "0" : list_return[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_return[i].Quantity == "" ? "0" : list_return[i].Quantity);
                    }

                    if (i == list_return.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_out.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_out.Count; i++)
                {
                    if (strSetSID != list_out[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));

                    }

                    if (i == list_out.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                list_input.Sort(CompareStorageData2);
                nIndex = 1;
                foreach (var item in list_input)
                {
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_input.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_return.Sort(CompareStorageData2);
                foreach (var item in list_return)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_return.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_out.Sort(CompareStorageData2);
                foreach (var item in list_out)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);

                    string strType = "자동";
                    if (item.pickid == "-" && item.Requestor == "-")
                        strType = "강제배출";

                    dataGridView_output.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Inch, item.pickid, item.Requestor, item.Tower_no, strType });
                }

            }
            else
            {
                return nMtlCount;
            }

            label_incount.Text = string.Format("{0:0,0}", list_input.Count.ToString());
            label_returncount.Text = string.Format("{0:0,0}", list_return.Count.ToString());
            label_outcount.Text = string.Format("{0:0,0}", list_out.Count.ToString());

            return nMtlCount;
        }
        public int Fnc_Process_GetINOUT_mtlinfo_Sid(int nType, string strSearch_sid, double strTime_st, double strTime_ed)
        {
            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_updatedate2.Text = "최근 업데이트: " + strToday + " " + strHead;

            var MtlList = AMM_Main.AMM.GetInouthistroy_Sid(AMM_Main.strDefault_linecode, strSearch_sid, strTime_st, strTime_ed);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData2> list_input = new List<StorageData2>();
            List<StorageData2> list_return = new List<StorageData2>();
            List<StorageData2> list_out = new List<StorageData2>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData2 data = new StorageData2();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Creation_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Creation_date = data.Creation_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();
                data.pickid = MtlList.Rows[i]["PICKID"].ToString(); data.pickid = data.pickid.Trim();
                data.Status = MtlList.Rows[i]["STATUS"].ToString(); data.Status = data.Status.Trim();
                data.Requestor = MtlList.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();

                if (data.Status == "IN" && data.Input_type == "CART")
                    list_input.Add(data);
                else if (data.Status == "IN" && data.Input_type == "RETURN")
                    list_return.Add(data);
                else if (data.Status == "OUT" || data.Status == "OUT-MANUAL")
                    list_out.Add(data);
            }

            int nIndex = 1;

            if (nType == 0) //SID
            {
                list_input.Sort(CompareStorageData3);

                string strSetSID = "", strInch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < list_input.Count; i++)
                {
                    if (strSetSID != list_input[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity == "" ? "0" : list_input[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity == "" ? "0" : list_input[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_input[i].Quantity == "" ? "0" : list_input[i].Quantity);
                    }

                    if (i == list_input.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_return.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_return.Count; i++)
                {
                    if (strSetSID != list_return[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity == "0" ? "" : list_return[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity == "0" ? "" : list_return[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_return[i].Quantity == "0" ? "" : list_return[i].Quantity);
                    }

                    if (i == list_return.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_out.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_out.Count; i++)
                {
                    if (strSetSID != list_out[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                    }

                    if (i == list_out.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                list_input.Sort(CompareStorageData2);
                nIndex = 1;
                foreach (var item in list_input)
                {
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_input.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_return.Sort(CompareStorageData2);
                foreach (var item in list_return)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_return.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_out.Sort(CompareStorageData2);
                foreach (var item in list_out)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);

                    string strType = "자동";
                    if (item.pickid == "-" && item.Requestor == "-")
                        strType = "강제배출";

                    dataGridView_output.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Inch, item.pickid, item.Requestor, item.Tower_no, strType });
                }

            }
            else
            {
                return nMtlCount;
            }

            label_incount.Text = string.Format("{0:0,0}", list_input.Count.ToString());
            label_returncount.Text = string.Format("{0:0,0}", list_return.Count.ToString());
            label_outcount.Text = string.Format("{0:0,0}", list_out.Count.ToString());

            return nMtlCount;
        }
        public int Fnc_Process_GetINOUT_mtlinfo_Sid2(int nType, string strEquip, string strSearch_sid, double strTime_st, double strTime_ed)
        {
            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_updatedate2.Text = "최근 업데이트: " + strToday + " " + strHead;

            var MtlList = AMM_Main.AMM.GetInouthistroy_Sid2(AMM_Main.strDefault_linecode, strEquip, strSearch_sid, strTime_st, strTime_ed);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData2> list_input = new List<StorageData2>();
            List<StorageData2> list_return = new List<StorageData2>();
            List<StorageData2> list_out = new List<StorageData2>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData2 data = new StorageData2();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Creation_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Creation_date = data.Creation_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();
                data.pickid = MtlList.Rows[i]["PICKID"].ToString(); data.pickid = data.pickid.Trim();
                data.Status = MtlList.Rows[i]["STATUS"].ToString(); data.Status = data.Status.Trim();
                data.Requestor = MtlList.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();

                if (data.Status == "IN" && data.Input_type == "CART")
                    list_input.Add(data);
                else if (data.Status == "IN" && data.Input_type == "RETURN")
                    list_return.Add(data);
                else if (data.Status == "OUT" || data.Status == "OUT-MANUAL")
                    list_out.Add(data);
            }

            int nIndex = 1;

            if (nType == 0) //SID
            {
                list_input.Sort(CompareStorageData3);

                string strSetSID = "", strInch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < list_input.Count; i++)
                {
                    if (strSetSID != list_input[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity == "" ? "0" : list_input[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity == "" ? "0" : list_input[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_input[i].Quantity == "" ? "0" : list_input[i].Quantity);
                    }

                    if (i == list_input.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_return.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_return.Count; i++)
                {
                    if (strSetSID != list_return[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity == ""  ? "0" : list_return[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity == "" ? "0" : list_return[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_return[i].Quantity == "" ? "0" : list_return[i].Quantity);
                    }

                    if (i == list_return.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_out.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_out.Count; i++)
                {
                    if (strSetSID != list_out[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_out[i].Quantity == "" ? "0" : list_out[i].Quantity.Replace(",", ""));
                    }

                    if (i == list_out.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                list_input.Sort(CompareStorageData2);
                nIndex = 1;
                foreach (var item in list_input)
                {
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_input.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_return.Sort(CompareStorageData2);
                foreach (var item in list_return)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_return.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_out.Sort(CompareStorageData2);
                foreach (var item in list_out)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", item.Quantity);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);

                    string strType = "자동";
                    if (item.pickid == "-" && item.Requestor == "-")
                        strType = "강제배출";

                    dataGridView_output.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Inch, item.pickid, item.Requestor, item.Tower_no, strType });
                }

            }
            else
            {
                return nMtlCount;
            }

            label_incount.Text = string.Format("{0:0,0}", list_input.Count.ToString());
            label_returncount.Text = string.Format("{0:0,0}", list_return.Count.ToString());
            label_outcount.Text = string.Format("{0:0,0}", list_out.Count.ToString());

            return nMtlCount;
        }
        private void button_update_Click(object sender, EventArgs e)
        {
            Fnc_Process_CalMaterialInfo();
        }

        private void button_excel_Click(object sender, EventArgs e)
        {
            //Fnc_Process_CalMaterialInfo();

            bExcel_Start = false;


            nExcelIndex = 0;

            Form_Excel Excel_Form = new Form_Excel();
            Excel_Form.ShowDialog();

            if (!bExcel_Start)
            {
                return;
            }

            FindTime = DateTime.Now;

            IsDateGathering = true;
            ////bExcelUse[0] = ASM all file, 1: ASM SID sorting , 2: In/out/return All data, 3: In/out/return SID sortinf 

            string strPath = strExcelfilePath + "\\";
            string strPath2 = strExcelfilePath + "\\";
            string stSaveTime_st = "", stSaveTime_ed = "", stSaveDate_st = "", stSaveDate_ed = "";
            //stSaveTime_st = label_Value_stTime.Text.Replace(":", "_");
            //stSaveTime_ed = label_Value_edTime.Text.Replace(":", "_");
            //stSaveDate_st = label_Value_date_st.Text.Replace("-", string.Empty);
            //stSaveDate_ed = label_Value_date_ed.Text.Replace("-", string.Empty);

            string strDate = stSaveDate_st + "_" + stSaveTime_st + "~" + stSaveDate_ed + "_" + stSaveTime_ed;
            string strDate2 = string.Format("{0}{1:00}{2:00}_{3}_{4}_{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            strPath = strPath + "ITS_" + strDate;
            strPath2 = strPath2 + "ITS_" + strDate2;

            string strPathName = "";

            if (bExcelUse[0])//Tower Inventory SID
            {
                strPathName = strPath2 + "_타워재고SID.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[타워 재고 SID]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                Fnc_ExcelCreate_InventoryInfo(strPathName, 0); //0: SID , 1: 상세 정보
            }

            if (bExcelUse[1])//Tower Inventory 상세 정보
            {
                strPathName = strPath2 + "_타워재고상세정보.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[타워 재고 상세 정보]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                //Fnc_ExcelCreate_InventoryInfo_Detail(strPathName, 0); //0: SID , 1: 상세 정보
                Fnc_ExcelCreate_InventoryInfo_Detail_All(strPathName, 0);
            }

            IsDateGathering = false;

            Fnc_Process_CalMaterialInfo();
        }

        public static DataTable GetDataGridViewAsDataTable(DataGridView _DataGridView)
        {
            try
            {
                if (_DataGridView.ColumnCount == 0)
                    return null;
                DataTable dtSource = new DataTable();
                //////create columns
                foreach (DataGridViewColumn col in _DataGridView.Columns)
                {
                    if (col.ValueType == null)
                        dtSource.Columns.Add(col.Name, typeof(string));
                    else
                        dtSource.Columns.Add(col.Name, col.ValueType);
                    dtSource.Columns[col.Name].Caption = col.HeaderText;
                }
                ///////insert row data
                foreach (DataGridViewRow row in _DataGridView.Rows)
                {
                    DataRow drNewRow = dtSource.NewRow();
                    foreach (DataColumn col in dtSource.Columns)
                    {
                        drNewRow[col.ColumnName] = row.Cells[col.ColumnName].Value;
                    }
                    dtSource.Rows.Add(drNewRow);
                }
                return dtSource;
            }
            catch
            {
                return null;
            }
        }

        public void Fnc_ExcelCreate_InventoryInfo(string strPath, int nType)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.Visible = false;
            xlApp.UserControl = false;

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;

            List<Excel.Worksheet> xlworksheets  = new List<Excel.Worksheet>();// = new Excel.Worksheet()[0];


            /*
            Excel.Worksheet xlWorkSheet;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            Excel.Worksheet xlWorkSheet4;
            Excel.Worksheet xlWorkSheet5;
            Excel.Worksheet xlWorkSheet6;
            Excel.Worksheet xlWorkSheet7;
            Excel.Worksheet xlWorkSheet8;//211018_Sangik.choi_재고관리 7번그룹 오류 수정
            Excel.Worksheet xlWorkSheet9;   //220823_ilyoung_타워그룹추가
            Excel.Worksheet xlWorkSheet10;   //220823_ilyoung_타워그룹추가
            */

            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            for (int i = 0; i < comboBox_group.Items.Count -1; i++)//220823_ilyoung_타워그룹추가
            {
                if(bGroupUse[i] == true)
                    xlworksheets.Add(xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue));
            }

            xlworksheets.Add(xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue)); // ALL Sheet

            /////Input save////////
            int nCellcount = 0;

            xlworksheets[0] = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlworksheets[0].Name = "ALL";

            int nBeforExcelRowcount = 0;
            int xlSheetCnt = 0;

            for (int i = 1; i < comboBox_group.Items.Count; i++)
            {
                if (bGroupUse[i -1] == true)
                {
                    dataGridView_info.Rows.Clear();
                    Fnc_Process_GetMaterialinfo(0, "TWR" + i.ToString());


                    DataTable MtlList = GetDataGridViewAsDataTable(dataGridView_info);

                    int iRow = 0;
                    string[] headers = new string[MtlList.Columns.Count];
                    string[] columns = new string[MtlList.Columns.Count];
                    string[,] item = new string[MtlList.Rows.Count, 6];

                    xlworksheets[xlSheetCnt + 1] = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(xlSheetCnt + 2);
                    xlworksheets[xlSheetCnt + 1].Name = "TWR" + i.ToString();

                    //Fnc_Init_datagrid(0);
                    //Fnc_Process_GetMaterialinfo_All(0);

                    int nGcount = MtlList.Rows.Count;
                    nCellcount = 0;

                    if (MtlList.Rows.Count > 0)
                    {

                        for (int c = 0; c < MtlList.Columns.Count; c++)
                        {
                            //DataTable 첫 Row에있는 컬럼명을 담기
                            headers[c] = MtlList.Columns[c].ColumnName;
                            //컬럼 위치값을 가져오기
                            columns[c] = ExcelColumnIndexToName(c);
                        }

                        for (int rowNo = 0; rowNo < MtlList.Rows.Count; rowNo++)
                        {

                            for (int colNo = 0; colNo < MtlList.Columns.Count; colNo++)
                            {
                                item[rowNo, colNo] = MtlList.Rows[rowNo][colNo].ToString();
                            }

                            iRow++;
                        }
                    }

                    xlworksheets[xlSheetCnt + 1].get_Range("A1", columns[MtlList.Columns.Count - 1] + "1").Value2 = headers;
                    xlworksheets[xlSheetCnt + 1].get_Range("A2", columns[MtlList.Columns.Count - 1] + (MtlList.Rows.Count + 1).ToString()).Value = item;
                    xlworksheets[xlSheetCnt + 1].Cells.NumberFormat = @"@";
                    xlworksheets[xlSheetCnt + 1].Columns.AutoFit();

                    xlworksheets[0].get_Range("A1", columns[MtlList.Columns.Count - 1] + "1").Value2 = headers;
                    xlworksheets[0].get_Range("A" + (nBeforExcelRowcount + 2).ToString(), columns[MtlList.Columns.Count - 1] + (MtlList.Rows.Count + nBeforExcelRowcount + 1).ToString()).Value = item;
                    nBeforExcelRowcount += iRow;
                    iRow = 0;
                    xlSheetCnt += 1;
                }
            }

            xlworksheets[0].Columns.AutoFit();

            for (int i = 0; i < nBeforExcelRowcount ; i++)
            {
            
                xlworksheets[0].Cells[i + 2, 1] = (i + 1).ToString();
            }

            //SIDxlApp.Visible = true;

            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            //IsDateGathering = false;

            if (nType == 0)
            {
                //string strMsg = "파일(타워재고 SID 기준)  저장 완료! 경로:" + strPath;
                //MessageBox.Show(strMsg);

                System.Diagnostics.Process.Start(strPath);
            }
        }

        public void Fnc_ExcelCreate_INOUTInfo_SID(string strPath, string strStart, string strEnd)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;

            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet3 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);

            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet1.Name = "입고";

            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "리턴";

            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheet3.Name = "출고";

            xlWorkSheet1.Cells[1, 2] = "No";
            xlWorkSheet1.Cells[1, 3] = "SID";
            xlWorkSheet1.Cells[1, 4] = "릴수";
            xlWorkSheet1.Cells[1, 5] = "TTL";
            xlWorkSheet1.Cells[1, 6] = "인치";
            xlWorkSheet1.Cells[1, 7] = "위치";

            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "SID";
            xlWorkSheet2.Cells[1, 4] = "릴수";
            xlWorkSheet2.Cells[1, 5] = "TTL";
            xlWorkSheet2.Cells[1, 6] = "인치";
            xlWorkSheet2.Cells[1, 7] = "위치";

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "SID";
            xlWorkSheet3.Cells[1, 4] = "릴수";
            xlWorkSheet3.Cells[1, 5] = "TTL";
            xlWorkSheet3.Cells[1, 6] = "인치";
            xlWorkSheet3.Cells[1, 7] = "위치";

            int nGcount_input = 0, nGcount_return = 0, nGcount_output = 0;
            int nCellcount_input = 0, nCellcount_return = 0, nCellcount_output = 0;

            for (int n = 0; n < bGroupUse.Length; n++) //220823_ilyoung_타워그룹추가
            {
                string strEqinfo = string.Format("TWR{0}", n + 1);

                if (bGroupUse[n])
                {
                    Fnc_Init_datagrid2(0);
                    Fnc_Process_GetINOUT_mtlinfo(0, strEqinfo, Double.Parse(strStart), Double.Parse(strEnd));

                    nGcount_input = dataGridView_input.RowCount;
                    nGcount_return = dataGridView_return.RowCount;
                    nGcount_output = dataGridView_output.RowCount;

                    for (int i = 0; i < nGcount_input; i++)
                    {
                        xlWorkSheet1.Cells[2 + nCellcount_input, 2] = nCellcount_input + 1;
                        xlWorkSheet1.Cells[2 + nCellcount_input, 3] = dataGridView_input.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 4] = dataGridView_input.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 5] = dataGridView_input.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 6] = dataGridView_input.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 7] = strEqinfo;

                        nCellcount_input++;
                    }

                    for (int i = 0; i < nGcount_return; i++)
                    {
                        xlWorkSheet2.Cells[2 + nCellcount_return, 2] = nCellcount_return + 1;
                        xlWorkSheet2.Cells[2 + nCellcount_return, 3] = dataGridView_return.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 4] = dataGridView_return.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 5] = dataGridView_return.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 6] = dataGridView_return.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 7] = strEqinfo;

                        nCellcount_return++;
                    }

                    for (int i = 0; i < nGcount_output; i++)
                    {
                        xlWorkSheet3.Cells[2 + nCellcount_output, 2] = nCellcount_output + 1;
                        xlWorkSheet3.Cells[2 + nCellcount_output, 3] = dataGridView_output.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 4] = dataGridView_output.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 5] = dataGridView_output.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 6] = dataGridView_output.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 7] = strEqinfo;

                        nCellcount_output++;
                    }
                }
            }

            xlWorkSheet1.Columns.AutoFit();
            xlWorkSheet2.Columns.AutoFit();
            xlWorkSheet3.Columns.AutoFit();
            ///////////////////////////////////////////////////
            /////////////////////////////////////////
            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet1);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkSheet3);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process.Start(strPath);
        }

        public void Fnc_ExcelCreate_INOUTInfo_Detail(string strPath, string strStart, string strEnd)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;

            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet3 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);

            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet1.Name = "입고";

            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "리턴";

            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheet3.Name = "출고";

            xlWorkSheet1.Cells[1, 2] = "No";
            xlWorkSheet1.Cells[1, 3] = "일자";
            xlWorkSheet1.Cells[1, 4] = "시간";
            xlWorkSheet1.Cells[1, 5] = "SID";
            xlWorkSheet1.Cells[1, 6] = "Lot#";
            xlWorkSheet1.Cells[1, 7] = "UID";
            xlWorkSheet1.Cells[1, 8] = "TTL";
            xlWorkSheet1.Cells[1, 9] = "투입형태";
            xlWorkSheet1.Cells[1, 10] = "위치";
            xlWorkSheet1.Cells[1, 11] = "제조일";
            xlWorkSheet1.Cells[1, 12] = "제조사";
            xlWorkSheet1.Cells[1, 13] = "인치";

            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "일자";
            xlWorkSheet2.Cells[1, 4] = "시간";
            xlWorkSheet2.Cells[1, 5] = "SID";
            xlWorkSheet2.Cells[1, 6] = "Lot#";
            xlWorkSheet2.Cells[1, 7] = "UID";
            xlWorkSheet2.Cells[1, 8] = "TTL";
            xlWorkSheet2.Cells[1, 9] = "투입형태";
            xlWorkSheet2.Cells[1, 10] = "위치";
            xlWorkSheet2.Cells[1, 11] = "제조일";
            xlWorkSheet2.Cells[1, 12] = "제조사";
            xlWorkSheet2.Cells[1, 13] = "인치";

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "일자";
            xlWorkSheet3.Cells[1, 4] = "시간";
            xlWorkSheet3.Cells[1, 5] = "SID";
            xlWorkSheet3.Cells[1, 6] = "Lot#";
            xlWorkSheet3.Cells[1, 7] = "UID";
            xlWorkSheet3.Cells[1, 8] = "TTL";
            xlWorkSheet3.Cells[1, 9] = "인치";
            xlWorkSheet3.Cells[1, 10] = "배출ID";
            xlWorkSheet3.Cells[1, 11] = "요청자";
            xlWorkSheet3.Cells[1, 12] = "위치";

            int nGcount_input = 0, nGcount_return = 0, nGcount_output = 0;
            int nCellcount_input = 0, nCellcount_return = 0, nCellcount_output = 0;

            for (int n = 0; n < bGroupUse.Length; n++)  //220823_ilyoung_타워그룹추가
            {
                string strEqinfo = string.Format("TWR{0}", n + 1);

                if (bGroupUse[n])
                {
                    Fnc_Init_datagrid2(1);
                    Fnc_Process_GetINOUT_mtlinfo(1, strEqinfo, Double.Parse(strStart), Double.Parse(strEnd));

                    nGcount_input = dataGridView_input.RowCount;
                    nGcount_return = dataGridView_return.RowCount;
                    nGcount_output = dataGridView_output.RowCount;

                    for (int i = 0; i < nGcount_input; i++)
                    {
                        xlWorkSheet1.Cells[2 + nCellcount_input, 2] = nCellcount_input + 1;
                        xlWorkSheet1.Cells[2 + nCellcount_input, 3] = dataGridView_input.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 4] = dataGridView_input.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 5] = dataGridView_input.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 6] = dataGridView_input.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 7] = dataGridView_input.Rows[i].Cells[5].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 8] = dataGridView_input.Rows[i].Cells[6].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 9] = dataGridView_input.Rows[i].Cells[7].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 10] = dataGridView_input.Rows[i].Cells[8].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 11] = dataGridView_input.Rows[i].Cells[9].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 12] = dataGridView_input.Rows[i].Cells[10].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 13] = dataGridView_input.Rows[i].Cells[11].Value.ToString();
                        //xlWorkSheet1.Cells[2 + nCellcount_input, 14] = strEqinfo;

                        nCellcount_input++;
                    }

                    for (int i = 0; i < nGcount_return; i++)
                    {
                        xlWorkSheet2.Cells[2 + nCellcount_return, 2] = nCellcount_return + 1;
                        xlWorkSheet2.Cells[2 + nCellcount_return, 3] = dataGridView_return.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 4] = dataGridView_return.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 5] = dataGridView_return.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 6] = dataGridView_return.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 7] = dataGridView_return.Rows[i].Cells[5].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 8] = dataGridView_return.Rows[i].Cells[6].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 9] = dataGridView_return.Rows[i].Cells[7].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 10] = dataGridView_return.Rows[i].Cells[8].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 11] = dataGridView_return.Rows[i].Cells[9].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 12] = dataGridView_return.Rows[i].Cells[10].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 13] = dataGridView_return.Rows[i].Cells[11].Value.ToString();
                        //xlWorkSheet2.Cells[2 + nCellcount_return, 14] = strEqinfo;

                        nCellcount_return++;
                    }

                    for (int i = 0; i < nGcount_output; i++)
                    {
                        xlWorkSheet3.Cells[2 + nCellcount_output, 2] = nCellcount_output + 1;
                        xlWorkSheet3.Cells[2 + nCellcount_output, 3] = dataGridView_output.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 4] = dataGridView_output.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 5] = dataGridView_output.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 6] = dataGridView_output.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 7] = dataGridView_output.Rows[i].Cells[5].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 8] = dataGridView_output.Rows[i].Cells[6].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 9] = dataGridView_output.Rows[i].Cells[7].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 10] = dataGridView_output.Rows[i].Cells[8].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 11] = dataGridView_output.Rows[i].Cells[9].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 12] = dataGridView_output.Rows[i].Cells[10].Value.ToString();
                        //xlWorkSheet3.Cells[2 + nCellcount_output, 12] = strEqinfo;

                        nCellcount_output++;
                    }
                }
            }

            xlWorkSheet1.Columns.AutoFit();
            xlWorkSheet2.Columns.AutoFit();
            xlWorkSheet3.Columns.AutoFit();
            ///////////////////////////////////////////////////
            /////////////////////////////////////////
            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet1);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkSheet3);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process.Start(strPath);
        }

        public void Fnc_ExcelCreate_InventoryInfo_Detail(string strPath, int nType)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            Excel.Worksheet xlWorkSheet4;
            Excel.Worksheet xlWorkSheet5;
            Excel.Worksheet xlWorkSheet6;
            Excel.Worksheet xlWorkSheet7;

            Excel.Worksheet xlWorkSheet8;   //220823_ilyoung_타워그룹추가
            Excel.Worksheet xlWorkSheet9;   //220823_ilyoung_타워그룹추가


            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet3 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet4 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet5 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet6 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet7 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet8 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet9 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);

            /////save////////
            int nCellcount = 0;

            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet1.Name = "Group 1";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR1");

            int nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet1.Cells[1, 2] = "No";
            xlWorkSheet1.Cells[1, 3] = "SID";
            xlWorkSheet1.Cells[1, 4] = "Batch#";
            xlWorkSheet1.Cells[1, 5] = "UID";
            xlWorkSheet1.Cells[1, 6] = "Qty";
            xlWorkSheet1.Cells[1, 7] = "투입형태";
            xlWorkSheet1.Cells[1, 8] = "위치";
            xlWorkSheet1.Cells[1, 9] = "제조일";
            xlWorkSheet1.Cells[1, 10] = "투입일";
            xlWorkSheet1.Cells[1, 11] = "제조사";
            xlWorkSheet1.Cells[1, 12] = "인치";

            if (bGroupUse[0])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet1.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet1.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet1.Columns.AutoFit();

            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "Group 2";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR2");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "SID";
            xlWorkSheet2.Cells[1, 4] = "Batch#";
            xlWorkSheet2.Cells[1, 5] = "UID";
            xlWorkSheet2.Cells[1, 6] = "Qty";
            xlWorkSheet2.Cells[1, 7] = "투입형태";
            xlWorkSheet2.Cells[1, 8] = "위치";
            xlWorkSheet2.Cells[1, 9] = "제조일";
            xlWorkSheet2.Cells[1, 10] = "투입일";
            xlWorkSheet2.Cells[1, 11] = "제조사";
            xlWorkSheet2.Cells[1, 12] = "인치";

            if (bGroupUse[1])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet2.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet2.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet2.Columns.AutoFit();
            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheet3.Name = "Group 3";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR3");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "SID";
            xlWorkSheet3.Cells[1, 4] = "Batch#";
            xlWorkSheet3.Cells[1, 5] = "UID";
            xlWorkSheet3.Cells[1, 6] = "Qty";
            xlWorkSheet3.Cells[1, 7] = "투입형태";
            xlWorkSheet3.Cells[1, 8] = "위치";
            xlWorkSheet3.Cells[1, 9] = "제조일";
            xlWorkSheet3.Cells[1, 10] = "투입일";
            xlWorkSheet3.Cells[1, 11] = "제조사";
            xlWorkSheet3.Cells[1, 12] = "인치";

            if (bGroupUse[2])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet3.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet3.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet3.Columns.AutoFit();
            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet4 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(4);
            xlWorkSheet4.Name = "Group 4";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR4");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet4.Cells[1, 2] = "No";
            xlWorkSheet4.Cells[1, 3] = "SID";
            xlWorkSheet4.Cells[1, 4] = "Batch#";
            xlWorkSheet4.Cells[1, 5] = "UID";
            xlWorkSheet4.Cells[1, 6] = "Qty";
            xlWorkSheet4.Cells[1, 7] = "투입형태";
            xlWorkSheet4.Cells[1, 8] = "위치";
            xlWorkSheet4.Cells[1, 9] = "제조일";
            xlWorkSheet4.Cells[1, 10] = "투입일";
            xlWorkSheet4.Cells[1, 11] = "제조사";
            xlWorkSheet4.Cells[1, 12] = "인치";

            if (bGroupUse[3])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet4.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet4.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet4.Columns.AutoFit();
            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet5 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(5);
            xlWorkSheet5.Name = "Group 5";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR5");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet5.Cells[1, 2] = "No";
            xlWorkSheet5.Cells[1, 3] = "SID";
            xlWorkSheet5.Cells[1, 4] = "Batch#";
            xlWorkSheet5.Cells[1, 5] = "UID";
            xlWorkSheet5.Cells[1, 6] = "Qty";
            xlWorkSheet5.Cells[1, 7] = "투입형태";
            xlWorkSheet5.Cells[1, 8] = "위치";
            xlWorkSheet5.Cells[1, 9] = "제조일";
            xlWorkSheet5.Cells[1, 10] = "투입일";
            xlWorkSheet5.Cells[1, 11] = "제조사";
            xlWorkSheet5.Cells[1, 12] = "인치";

            if (bGroupUse[4])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet5.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet5.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet5.Columns.AutoFit();
            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet6 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(6);
            xlWorkSheet6.Name = "Group 6";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR6");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet6.Cells[1, 2] = "No";
            xlWorkSheet6.Cells[1, 3] = "SID";
            xlWorkSheet6.Cells[1, 4] = "Batch#";
            xlWorkSheet6.Cells[1, 5] = "UID";
            xlWorkSheet6.Cells[1, 6] = "Qty";
            xlWorkSheet6.Cells[1, 7] = "투입형태";
            xlWorkSheet6.Cells[1, 8] = "위치";
            xlWorkSheet6.Cells[1, 9] = "제조일";
            xlWorkSheet6.Cells[1, 10] = "투입일";
            xlWorkSheet6.Cells[1, 11] = "제조사";
            xlWorkSheet6.Cells[1, 12] = "인치";

            if (bGroupUse[5])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet6.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet6.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet6.Columns.AutoFit();
            /////////////////////////////////////////
            ///
            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet7 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(7);
            xlWorkSheet7.Name = "Group 7";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR7");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet7.Cells[1, 2] = "No";
            xlWorkSheet7.Cells[1, 3] = "SID";
            xlWorkSheet7.Cells[1, 4] = "Batch#";
            xlWorkSheet7.Cells[1, 5] = "UID";
            xlWorkSheet7.Cells[1, 6] = "Qty";
            xlWorkSheet7.Cells[1, 7] = "투입형태";
            xlWorkSheet7.Cells[1, 8] = "위치";
            xlWorkSheet7.Cells[1, 9] = "제조일";
            xlWorkSheet7.Cells[1, 10] = "투입일";
            xlWorkSheet7.Cells[1, 11] = "제조사";
            xlWorkSheet7.Cells[1, 12] = "인치";

            if (bGroupUse[5])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet7.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet7.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet7.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet7.Columns.AutoFit();
            /////////////////////////////////////////
            ///
            /////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////////////////////////220823_ilyoung_타워그룹추가
            xlWorkSheet8 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(8);
            xlWorkSheet8.Name = "Group 8";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR8");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet8.Cells[1, 2] = "No";
            xlWorkSheet8.Cells[1, 3] = "SID";
            xlWorkSheet8.Cells[1, 4] = "Batch#";
            xlWorkSheet8.Cells[1, 5] = "UID";
            xlWorkSheet8.Cells[1, 6] = "Qty";
            xlWorkSheet8.Cells[1, 7] = "투입형태";
            xlWorkSheet8.Cells[1, 8] = "위치";
            xlWorkSheet8.Cells[1, 9] = "제조일";
            xlWorkSheet8.Cells[1, 10] = "투입일";
            xlWorkSheet8.Cells[1, 11] = "제조사";
            xlWorkSheet8.Cells[1, 12] = "인치";

            if (bGroupUse[5])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet8.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet8.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet8.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet8.Columns.AutoFit();
            ///////////////////////////////////////////220823_ilyoung_타워그룹추가
            ///
            /////////////////////////////////////////
            ///
            ///////////////////////////////////////////////////////////////////////////////////////220823_ilyoung_타워그룹추가
            xlWorkSheet9 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(9);
            xlWorkSheet9.Name = "Group 9";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR9");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet9.Cells[1, 2] = "No";
            xlWorkSheet9.Cells[1, 3] = "SID";
            xlWorkSheet9.Cells[1, 4] = "Batch#";
            xlWorkSheet9.Cells[1, 5] = "UID";
            xlWorkSheet9.Cells[1, 6] = "Qty";
            xlWorkSheet9.Cells[1, 7] = "투입형태";
            xlWorkSheet9.Cells[1, 8] = "위치";
            xlWorkSheet9.Cells[1, 9] = "제조일";
            xlWorkSheet9.Cells[1, 10] = "투입일";
            xlWorkSheet9.Cells[1, 11] = "제조사";
            xlWorkSheet9.Cells[1, 12] = "인치";

            if (bGroupUse[5])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet9.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet9.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet9.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet9.Columns.AutoFit();
            ///////////////////////////////////////////220823_ilyoung_타워그룹추가
            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet1);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkSheet3);
            Marshal.ReleaseComObject(xlWorkSheet4);
            Marshal.ReleaseComObject(xlWorkSheet5);
            Marshal.ReleaseComObject(xlWorkSheet6);
            Marshal.ReleaseComObject(xlWorkSheet7);

            Marshal.ReleaseComObject(xlWorkSheet8); //220823_ilyoung_타워그룹추가
            Marshal.ReleaseComObject(xlWorkSheet9); //220823_ilyoung_타워그룹추가

            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            //IsDateGathering = false;

            if (nType == 0)
            {
                //string strMsg = "파일(타워재고 상세 정보)  저장 완료! 경로:" + strPath;
                //MessageBox.Show(strMsg);

                System.Diagnostics.Process.Start(strPath);
            }
        }

        public void Fnc_ExcelCreate_InventoryInfo_Detail_All(string strPath, int nType)
        {
            try
            {            
                Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlApp.Visible = false;
                xlApp.UserControl = false;
                object TypMissing = Type.Missing;

                if (xlApp == null)
                {
                    MessageBox.Show("Excel is not properly installed!!");
                    return;
                }

                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet1;
                object misValue = System.Reflection.Missing.Value;

                xlWorkBook = xlApp.Workbooks.Add(misValue);

                /////save////////
                int nCellcount = 0;

                xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet1.Name = "상세정보";

                Fnc_Init_datagrid(1); //상세 정보
                //Fnc_Process_GetMaterialinfo_DetailAll();

                DataTable MtlList = null;                

                MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode);

                int iRow = 0;
                string[] headers = new string[MtlList.Columns.Count];
                string[] columns = new string[MtlList.Columns.Count];
                string[,] item = new string[MtlList.Rows.Count, MtlList.Columns.Count];
                
                if (MtlList.Rows.Count > 0)
                {
                    for (int c = 0; c < MtlList.Columns.Count; c++)
                    {
                        //DataTable 첫 Row에있는 컬럼명을 담기
                        headers[c] = MtlList.Columns[c].ColumnName;
                        //컬럼 위치값을 가져오기
                        columns[c] = ExcelColumnIndexToName(c);
                    }


                    for (int rowNo = 0; rowNo < MtlList.Rows.Count; rowNo++)
                    {
                        for (int colNo = 0; colNo < MtlList.Columns.Count; colNo++)
                        {

                            item[rowNo, colNo] = MtlList.Rows[rowNo][colNo].ToString();
                        }

                        iRow++;
                    }
                }

                //해당위치에 컬럼명을 담기
                xlWorkSheet1.get_Range("A1", columns[MtlList.Columns.Count - 1] + "1").Value2 = headers;
                //해당위치부터 데이터정보를 담기
                xlWorkSheet1.get_Range("A2", columns[MtlList.Columns.Count - 1] + (MtlList.Rows.Count + 1).ToString()).Value = item;
                xlWorkSheet1.Cells.NumberFormat = @"@";
                xlWorkSheet1.Columns.AutoFit();
                xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
                //xlWorkSheet1.SaveAs(strPath, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false,
                //Excel.XlSaveAsAccessMode.xlShared, false, false, null, null, null);

                xlApp.Visible = true;

                releaseObject(xlApp);
                releaseObject(xlWorkSheet1);
                releaseObject(xlWorkBook);

                xlWorkSheet1.Columns.AutoFit();

                /////////////////////////////////////////
                ///

               
                //IsDateGathering = false;

                if (nType == 0)
                {
                    //string strMsg = "파일(타워재고 상세 정보)  저장 완료! 경로:" + strPath;
                    //MessageBox.Show(strMsg);

                    System.Diagnostics.Process.Start(strPath);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private string ExcelColumnIndexToName(int Index)
        {
            string range = "";
            if (Index < 0) return range;
            for (int i = 1; Index + i > 0; i = 0)
            {
                range = ((char)(65 + Index % 26)).ToString() + range;
                Index /= 26;
            }
            if (range.Length > 1) range = ((char)((int)range[0] - 1)).ToString() + range.Substring(1);
            return range;
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception e)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        private void comboBox_group_SelectedIndexChanged(object sender, EventArgs e)
        {
            FindTime = isClick == true ? DateTime.Now : new DateTime();
            int nType = comboBox_type.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            IsDateGathering = true;

            Fnc_Init_datagrid(nType);

            //if (nGroup != 7)
            if (nGroup != comboBox_group.Items.Count) //210824_Sangik.choi_타워그룹추가 //220823_ilyoung_타워그룹추가
                Fnc_Process_GetMaterialinfo(nType, strEquipid);
            else
            {
                Fnc_Process_GetMaterialinfo_All(nType);
            }

            IsDateGathering = false;
        }

        private void comboBox_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nType = comboBox_type.SelectedIndex;
            FindTime = isClick == true ? DateTime.Now : new DateTime();
            Fnc_Init_datagrid(nType);
            

            if (AMM_Main.nSelectedWin == 2)
            {
                int nGroup = comboBox_group.SelectedIndex + 1;

                string strEquipid = "TWR" + nGroup.ToString();

                IsDateGathering = true;

                Fnc_Init_datagrid(nType);

                //if (nGroup != 7)
                if (nGroup != comboBox_group.Items.Count) //210824_Sangik.choi_타워그룹추가 //220823_ilyoung_타워그룹추가
                    Fnc_Process_GetMaterialinfo(nType, strEquipid);
                else
                {
                    Fnc_Process_GetMaterialinfo_All(nType);
                }

                IsDateGathering = false;
            }

        }

        private void comboBox_searchtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_mtlinput.Text = "";
            textBox_mtlinput.Focus();

            label_info1.Text = "-";
            label_info2.Text = "자재 없음";
            label_info3.Text = "-";
            label_info4.Text = "-";

            int n = comboBox_searchtype.SelectedIndex;

            if (n == 1)
            {
                comboBox_sid.Items.Clear();
                comboBox_sid.Items.Add("Reel ID");
                comboBox_sid.SelectedIndex = 0;
            }
        }

        private void comboBox_type2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nType = comboBox_type2.SelectedIndex;

            Fnc_Init_datagrid2(nType);

            if (AMM_Main.nSelectedWin == 2)
            {
                IsDateGathering = true;

                string strDate_st = "", strDate_ed = "";
                strDate_st = strTimeset_date_st.Replace("-", string.Empty);
                strDate_st = strDate_st.Trim();
                strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

                strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
                strDate_ed = strDate_ed.Trim();
                strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

                int nGroup = -1;
                string strEquipid = "";

                if (bSearch_sid)
                {
                    //int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
                    comboBox_group2.Text = "전체 조회";

                    Fnc_Init_datagrid2(nType);

                    if (strDate_st == "" || strDate_st == "")
                    {
                        IsDateGathering = false;
                        return;
                    }

                    Fnc_Process_GetINOUT_mtlinfo_Sid(nType, textBox_sid.Text, Double.Parse(strDate_st), Double.Parse(strDate_ed));
                }
                else
                {
                    //int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
                    nGroup = comboBox_group2.SelectedIndex + 1;

                    strEquipid = "TWR" + nGroup.ToString();

                    Fnc_Init_datagrid2(nType);

                    if (strDate_st == "" || strDate_st == "")
                    {
                        IsDateGathering = false;
                        return;
                    }

                    if (nGroup != 8) //210909_Sangik.choi_입출고정보 7번그룹 추가
                        Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));

                }

                IsDateGathering = false;
            }
        }

        private void comboBox_group2_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDateGathering = true;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            Fnc_Init_datagrid2(nType);

            if (strDate_st == "" || strDate_st == "")
            {
                IsDateGathering = false;
                return;
            }

            if (bSearch_sid)
            {
                Fnc_Process_GetINOUT_mtlinfo_Sid2(nType, strEquipid, textBox_sid.Text, Double.Parse(strDate_st), Double.Parse(strDate_ed));
            }
            else
            {
                if (nGroup != comboBox_group2.Items.Count)//210909_Sangik.choi_입출고정보 7번그룹 추가 //220823_ilyoung_타워그룹추가
                    Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));
                else if (nGroup == 10)
                {
                    Fnc_Process_GetINOUT_mtlinfo(nType, "ALL", Double.Parse(strDate_st), Double.Parse(strDate_ed));
                }
            }

            IsDateGathering = false;
        }

        private void button_excel2_Click(object sender, EventArgs e)
        {
            bExcel_Start = false;

            nExcelIndex = 1;

            Form_Excel Excel_Form = new Form_Excel();
            Excel_Form.ShowDialog();

            if (!bExcel_Start)
            {
                return;
            }

            IsDateGathering = true;

            string strPath = strExcelfilePath + "\\";
            string stSaveTime_st = "", stSaveTime_ed = "", stSaveDate_st = "", stSaveDate_ed = "";

            //string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            stSaveTime_st = label_Value_time_st.Text.Replace(":", "_");
            stSaveTime_ed = label_Value_time_ed.Text.Replace(":", "_");
            //stSaveTime_ed = strHead.Replace(":", "_");
            //stSaveTime_ed = stSaveTime_ed.Substring(0, 5);
            stSaveDate_st = label_Value_date_st.Text.Replace("-", string.Empty);
            stSaveDate_ed = label_Value_date_ed.Text.Replace("-", string.Empty);
            //stSaveDate_ed = strToday.Replace("-", string.Empty);

            string strDate = stSaveDate_st + "_" + stSaveTime_st + "~" + stSaveDate_ed + "_" + stSaveTime_ed;
            strPath = strPath + "ITS_" + strDate;
            string strPathName = "";

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            //strDate_ed = strToday.Replace("-", string.Empty);
            //strHead = strHead.Replace(":", string.Empty);
            //strDate_ed = strDate_ed.Trim();
            //strHead = strHead.Trim();
            //strDate_ed = strDate_ed + strHead;

            if (bExcelUse[2])//입출고 SID
            {
                strPathName = strPath + "_입출고SID.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[입출고 SID]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                Fnc_ExcelCreate_INOUTInfo_SID(strPathName, strDate_st, strDate_ed);
            }

            if (bExcelUse[3])//입출고 상세 정보
            {
                strPathName = strPath + "_입출고상세정보.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[일출고 상세 정보]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                Fnc_ExcelCreate_INOUTInfo_Detail(strPathName, strDate_st, strDate_ed);
            }

            Fnc_Update_timeset();

            IsDateGathering = false;
        }

        private void button_update_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_update, "정보 업데이트");
        }

        private void button_excel_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_excel, "액셀 저장");
        }

        private void button_timeset_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_timeset, "조회 시간 설정");
        }

        private void button_excel2_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_excel2, "액셀 저장");
        }

        private void comboBox_sid_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBox_searchtype.SelectedIndex;

            if (n == 0)
            {
                //n = comboBox_sid.SelectedIndex;
                string str = comboBox_sid.SelectedItem.ToString();

                textBox_mtlinput.Text = str;
                Fnc_ProcessFind(0, str);

                textBox_mtlinput.Text = "";
            }
        }

        private void textBox_find_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                //Find
                Fnc_Find(textBox_find.Text);
            }
        }

        public void Fnc_Find(string strFind)
        {
            dataGridView_info.ClearSelection();

            int nCount = dataGridView_info.RowCount;
            int nCount2 = dataGridView_info.ColumnCount;

            bool bfind = false;

            for (int m = 1; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_info.Rows[n].Cells[m].Value.ToString();

                    if (str == strFind)
                    {
                        dataGridView_info.Rows[n].Cells[m].Selected = true;
                        dataGridView_info.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount; m = nCount2;
                    }
                }
            }

            if (bfind)
                return;

            for (int m = 1; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_info.Rows[n].Cells[m].Value.ToString();

                    if (str.Contains(strFind))
                    {
                        dataGridView_info.Rows[n].Cells[m].Selected = true;
                        dataGridView_info.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount; m = nCount2;
                    }
                }
            }
        }

        int TimerCnt = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TimerCnt >= 180)
            {
                TimerCnt = 0;
                Fnc_Update();

                if ((DateTime.Now - FindTime).TotalMinutes >= Properties.Settings.Default.DefaultSaftyTime)
                {
                    l_WaitTime.Text = "00:00";
                    Fnc_Init_datagrid_capa();
                }
            }
            else
            {
                int TotSec = int.Parse(((Properties.Settings.Default.DefaultSaftyTime * 60) - (int)(DateTime.Now - FindTime).TotalSeconds).ToString());

                if (TotSec >= 0)
                {
                    l_WaitTime.Text = string.Format("{0}:{1}", (TotSec / 60).ToString("D2"), (TotSec % 60).ToString("D2"));
                }
                else
                {

                }

                TimerCnt++;
            }

                
        }



        public void Fnc_Update()
        {
            if (bUpdate_Timer)
                Fnc_Process_CalMaterialInfo();           
        }



        private void dataGridView_info_MouseUp(object sender, MouseEventArgs e)
        {
            nSum = 0;
            foreach (DataGridViewCell cell in dataGridView_info.SelectedCells)
            {
                if (cell.ColumnIndex == 2)
                {
                    var Value = dataGridView_info.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value.ToString();
                    nSum = nSum + Int32.Parse(Value == "" ? "0" : Value);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (comboBox_type.SelectedIndex == 0 && nSum != 0)
            {
                string str = string.Format("합계: {0}", nSum);
                MessageBox.Show(str);
            }
        }

        private void loadXmlData()
        { 
            if (Directory.Exists(@"C:\Program Files (x86)\SIPLACE\SIPLACE Material Tower\Data\Storage") == false)
            {
                MessageBox.Show("Tower 1,2,3 그룹은 원격에서 동기화를 진행 할 수 없습니다.", "Sync Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            XmlDocument xml = new XmlDocument();

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("TowerID", "TowerID");


            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(@"C:\Program Files (x86)\SIPLACE\SIPLACE Material Tower\Data\Storage");

            System.IO.FileInfo[] fileInfos = directoryInfo.GetFiles("*.xml");

            XmlNodeList element;

            string[] row = new string[4];
            int tower = 0;

            int cnt = 1;

            foreach (System.IO.FileInfo fi in fileInfos)
            {
                xml.Load(fi.FullName);
                var temp = xml["Storage"]["ListOfStacks"];

                tower++;

                foreach (XmlElement el in temp)
                {
                    foreach (XmlElement ch in el["ListOfShelfs"])
                    {
                        if (ch.ChildNodes.Count >= 2)
                        {
                            string[] a = ch.ChildNodes[1].OuterXml.Split(' ');
                            string[] b = ch.OuterXml.Split(' ');

                            row[0] = cnt.ToString();
                            row[1] = a[1].Split('=')[1].Replace("\"", "");
                            row[2] = a[2].Split('=')[1].Replace("\"", "");
                            row[3] = $"T0{comboBox_sel.SelectedIndex + 1}0{tower}";

                            dataGridView_asm.Rows.Add(row);
                            //Console.WriteLine("ID : " + a[1].Split('=')[1].Replace("\"", ""));
                            ++cnt;
                        }

                        if (ch.ChildNodes.Count == 3)
                        {
                            string[] a = ch.ChildNodes[2].OuterXml.Split(' ');
                            string[] b = ch.OuterXml.Split(' ');

                            row[0] = cnt.ToString();
                            row[1] = a[1].Split('=')[1].Replace("\"", "");
                            row[2] = a[2].Split('=')[1].Replace("\"", "");

                            dataGridView_asm.Rows.Add(row);
                            //Console.WriteLine("ID : " + a[1].Split('=')[1].Replace("\"", ""));
                            ++cnt;
                        }
                    }
                }
            }
        }

        private void button_dbload_Click(object sender, EventArgs e)
        {
            int n = comboBox_sel.SelectedIndex;

            try
            {
                Synclog.Info(string.Format("DB Load Click Selected Group : {0}", comboBox_sel.Text));

                if (n == 0)
                {
                    loadXmlData();
                    //Fnc_Process_GetMaterials_Tower1();
                    Fnc_Process_GetAMMinfo("TWR1");
                }
                else if (n == 1)
                {
                    loadXmlData();
                    //Fnc_Process_GetMaterials_Tower2();
                    Fnc_Process_GetAMMinfo("TWR2");
                }
                else if (n == 2)
                {
                    loadXmlData();
                    //Fnc_Process_GetMaterials_Tower3();
                    Fnc_Process_GetAMMinfo("TWR3");
                }
                else
                {
                    MDBConn = null;
                    GetMycronicTowerLocal(n + 1);
                    GetMycronicTowerBackup();
                    Fnc_Process_GetAMMinfo("TWR" + (n + 1).ToString());
                }

                dataGridView_missmatch.Columns.Clear();
                dataGridView_missmatch.Rows.Clear();

                nDbUpdate = 1;
            }
            catch (Exception ex)
            {
                Synclog.Error(ex.StackTrace);
                Synclog.Error(ex.Message);
            }

            dataGridView_missmatch.Columns.Clear();
            dataGridView_missmatch.Rows.Clear();

            nDbUpdate = 1;
        }


        private void GetMycronicTowerBackup()
        {
            try
            {
                dgv_backup.Rows.Clear();
                dgv_backup.Columns.Clear(); 

                dgv_backup.Columns.Add("NO", "NO");
                dgv_backup.Columns.Add("SID", "SID");
                dgv_backup.Columns.Add("LOTID", "LOTID");
                dgv_backup.Columns.Add("UID", "UID");
                dgv_backup.Columns.Add("Qty", "Qty");
                dgv_backup.Columns.Add("투입일", "투입일");
                dgv_backup.Columns.Add("제조일", "제조일");
                dgv_backup.Columns.Add("제조사", "제조사");
                dgv_backup.Columns.Add("위치", "위치");
                dgv_backup.Columns.Add("인치", "인치");

                DataTable dt = new DataTable();
                using (SqlConnection c = new SqlConnection("server=10.135.200.35;database=ATK4-AMM-DBv1;user id=amm;password=amm@123"))
                {
                    c.Open();

                    using (SqlCommand cmd = new SqlCommand($"SELECT [NAME],[VALUE],[TYPE] from [PUBLIC_SETTINGS] with(nolock) where NAME='TOWER{comboBox_sel.SelectedIndex + 1}_PATH'", c))
                    {
                        using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                }

                string path = dt.Rows[0][1].ToString();



                DataConn conn1 = new DataConn();
                //              0 UID     1 제조일       2 위치     3 입고날짜   4 Qty    5 인치       6 제조사       7 LOTID
                DataSet ds = conn1.GetDataset("select [Carrier], [CreateDate], [Depot], [DepotDate], [Stock], [Diameter], [Manufactur], [Custom1] from Carrier ", @"C:\SMDTowerSQL\BackupDB.MDB");// path);

                int rowcnt = 1;

                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    dgv_backup.Rows.Add(
                        rowcnt++,
                        "",
                        row[7].ToString(),
                        row[0].ToString(),
                        row[4].ToString(),
                        row[3].ToString(),
                        row[1].ToString(),
                        row[6].ToString(),
                        row[2].ToString().Contains(",") == false ? row[2].ToString() : row[2].ToString().Split(',')[0].Split(' ')[1],
                        row[5].ToString()
                    );
                }

            }
            catch (Exception ex)
            {
                Synclog.Error(ex.StackTrace);
                Synclog.Error(ex.Message);

                if (ex.Message == "디스크 또는 네트워크 오류입니다.")
                {
                    MessageBox.Show("Database에 접속 할 수 없습니다.\n네트워크를 점검해 주세요");
                }

            }
            
        }

        private DataSet GetMycronicData(int Group, string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection c = new SqlConnection("server=10.135.200.35;database=ATK4-AMM-DBv1;user id=amm;password=amm@123"))
            {
                c.Open();

                using (SqlCommand cmd = new SqlCommand($"SELECT [NAME],[VALUE],[TYPE] from [PUBLIC_SETTINGS] with(nolock) where NAME='TOWER{Group}_IP'", c))
                {
                    using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                    {
                        adt.Fill(dt);
                    }
                }
            }

            string IP = dt.Rows[0][1].ToString();
            DataSet ds = new DataSet();

            using (SqlConnection c = new SqlConnection($"server={IP};database=stsys; user id=amm;password=amm@123"))
            {
                c.Open();

                using (SqlCommand cmd = new SqlCommand(query, c))// "SELECT [Carrier],[CreateDate],[ArticleName],[Depot],[DepotDate],[Stock],[Diameter],[Manufactur],[Custom1],[Custom2],[Diameter] FROM [stsys].[dbo].[TCarrier] with(nolock) order by [Carrier]", c))
                {
                    using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                    {
                        adt.Fill(ds);
                    }
                }
            }

            return ds;
        }

        private void GetMycronicTowerLocal(int Group)
        {
            try 
            { 
                //string sql = @"SELECT ROW_NUMBER() OVER(order by(select 1)) as [NO], ROW_NUMBER() OVER(order by(select 1)) as [SID], [Batch],  [Carrier], [Stock], [DepotDate], [CreateDate], [Manufactur], SUBSTRING(Depot,7,5) from [Carrier]";

                
                SortMDB(GetMycronicData(Group, "SELECT [Carrier],[CreateDate],[ArticleName],[Depot],[DepotDate],[Stock],[Diameter],[Manufactur],[Custom1],[Custom2],[Diameter] FROM [stsys].[dbo].[TCarrier] with(nolock) order by [Carrier]"));
                //dataGridView_asm.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                Synclog.Error(ex.StackTrace);
                Synclog.Error(ex.Message);

                if(ex.Message == "디스크 또는 네트워크 오류입니다.")
                {
                    MessageBox.Show("Database에 접속 할 수 없습니다.\n네트워크를 점검해 주세요");
                }
                
            }
            
        }

        string MycronicTowerIP = "";

        private void DeleteMycronicTower(int Group, string UID)
        {
            DataTable dt = new DataTable();

            try
            {
                if (MycronicTowerIP == "")
                {
                    using (SqlConnection c = new SqlConnection("server=10.135.200.35;database=ATK4-AMM-DBv1;user id=amm;password=amm@123"))
                    {
                        c.Open();

                        using (SqlCommand cmd = new SqlCommand($"SELECT [NAME],[VALUE],[TYPE] from [PUBLIC_SETTINGS] with(nolock) where NAME='TOWER{Group}_IP'", c))
                        {
                            using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                            {
                                adt.Fill(dt);
                            }
                        }
                    }

                    MycronicTowerIP= dt.Rows[0][1].ToString();
                }

                DataSet ds = new DataSet();

                using (SqlConnection c = new SqlConnection($"server={MycronicTowerIP};database=stsys; user id=amm;password=amm@123"))
                {
                    c.Open();
                    
                    using (SqlCommand cmd = new SqlCommand($"select  * FROM [stsys].[dbo].[TCarrier] where Carrier='{UID}'", c))
                    {
                        using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                        {
                            adt.Fill(ds);
                        }
                    }
                }

                int res = ds.Tables[0].Rows.Count;
                

                if(res == 1)
                {
                    using (SqlConnection c = new SqlConnection($"server={MycronicTowerIP};database=stsys; user id=amm;password=amm@123"))
                    {
                        c.Open();

                        using (SqlCommand cmd = new SqlCommand($"delete FROM [stsys].[dbo].[TCarrier] where Carrier='{UID}'", c))
                        {
                            using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                            {
                                adt.Fill(ds);
                            }
                        }
                    }

                    Synclog.Info("Delete Success UID : {0}", UID);
                }
                else
                {
                    Synclog.Info($"Delete Fail UID : {UID}, res : {res}");
                    
                }
                
            }
            catch (Exception ex)
            {
                Synclog.Error(ex.StackTrace);
                Synclog.Error(ex.Message);

                if (ex.Message == "디스크 또는 네트워크 오류입니다.")
                {
                    MessageBox.Show("Database에 접속 할 수 없습니다.\n네트워크를 점검해 주세요");
                }
            }

        }



        Dictionary<string, string> Tower_serial = new Dictionary<string, string>();

        private void GetTowerSerial()
        {
            try
            {
                if (Tower_serial.Count != 0)
                    return;

                DataSet dt = new DataSet();
                

                using (SqlConnection c = new SqlConnection("server=10.135.200.35;database=ATK4-AMM-DBv1;user id=amm;password=amm@123"))
                {
                    c.Open();

                    using (SqlCommand cmd = new SqlCommand($"SELECT [NAME],[VALUE],[TYPE] from [PUBLIC_SETTINGS] with(nolock) where TYPE='TOWER_SERIAL'", c))
                    {
                        using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                }


                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    Tower_serial.Add(dt.Tables[0].Rows[i]["VALUE"].ToString(), dt.Tables[0].Rows[i]["NAME"].ToString());
                }
            }
            catch (Exception ex )
            {

                throw;
            }
            
        }

        private void SortMDB(DataSet ds)
        {

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("LOTID", "LOTID");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");
            dataGridView_asm.Columns.Add("인치", "인치");

            try
            {
                if(Tower_serial.Count == 0)
                    GetTowerSerial();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dataGridView_asm.Rows.Add(
                        (i + 1).ToString(),
                        ds.Tables[0].Rows[i]["ArticleName"].ToString().Trim(),
                        ds.Tables[0].Rows[i]["Custom1"].ToString().Trim(),
                        ds.Tables[0].Rows[i]["Carrier"].ToString().Trim(),
                        ds.Tables[0].Rows[i]["Stock"].ToString().Trim(),
                        ds.Tables[0].Rows[i]["DepotDate"].ToString().Trim(),
                        ds.Tables[0].Rows[i]["CreateDate"].ToString().Trim(),
                        ds.Tables[0].Rows[i]["Manufactur"].ToString().Trim(),
                        ds.Tables[0].Rows[i]["Depot"].ToString().Contains(".") == true ? Tower_serial[ds.Tables[0].Rows[i]["Depot"].ToString().Split('.')[0]] : ds.Tables[0].Rows[i]["Depot"].ToString(),
                        ds.Tables[0].Rows[i]["Diameter"].ToString().Trim()
                    );

                }
            }
            catch (Exception ex)
            {

            }

            //int idx = 1;
            //foreach (var item in simmList1)
            //{
            //    dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
            //    idx++;
            //}
            //tid = "T0302";
            //var simmList2 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            //foreach (var item in simmList2)
            //{
            //    dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
            //    idx++;
            //}

            //tid = "T0303";
            //var simmList3 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            //foreach (var item in simmList3)
            //{
            //    dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
            //    idx++;
            //}

            //tid = "T0304";
            //var simmList4 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            //foreach (var item in simmList4)
            //{
            //    dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
            //    idx++;
            //}
        }

        private void button_missmatch_Click(object sender, EventArgs e)
        {
            Synclog.Info("Missmatch Button Click");

            if (nDbUpdate != 1)
            {
                Synclog.Info("DB 조회가 되지 않았습니다. DB 조회를 먼저 진행 하십시오.");
                MessageBox.Show("DB 조회가 되지 않았습니다. DB 조회를 먼저 진행 하십시오.");
                return;
            }

            dataGridView_missmatch.Columns.Clear();
            dataGridView_missmatch.Rows.Clear();

            //dataGridView_missmatch.Columns.Add("NO", "NO");
            //dataGridView_missmatch.Columns.Add("UID", "UID");
            //dataGridView_missmatch.Columns.Add("위치", "위치");
            //dataGridView_missmatch.Columns.Add("MISS", "MISS");

            dataGridView_missmatch.Columns.Add("NO", "NO");
            dataGridView_missmatch.Columns.Add("SID", "SID");
            dataGridView_missmatch.Columns.Add("LOTID", "LOTID#");
            dataGridView_missmatch.Columns.Add("UID", "UID");
            dataGridView_missmatch.Columns.Add("Qty", "Qty");
            dataGridView_missmatch.Columns.Add("투입형태", "투입형태");
            dataGridView_missmatch.Columns.Add("위치", "위치");
            dataGridView_missmatch.Columns.Add("제조일", "제조일");
            dataGridView_missmatch.Columns.Add("투입일", "투입일");
            dataGridView_missmatch.Columns.Add("제조사", "제조사");
            dataGridView_missmatch.Columns.Add("인치", "인치");
            dataGridView_missmatch.Columns.Add("MISS", "MISS");

            int nStart = 1;
            
            if(comboBox_sel.SelectedIndex < 3)
            {
                runMissmatch();
            }
            else
            {
                nStart = Fnc_Missmatch_ASMcompare(nStart);
                Fnc_Missmatch_AMMcompare(nStart);
                MissmatchDB2Backup();
            }
            

            nDbUpdate = 2;
        }

        private void runMissmatch()
        {
            int cnt = 0;

            /*
             dataGridView_missmatch.Columns.Add("NO", "NO");
            dataGridView_missmatch.Columns.Add("SID", "SID");
            dataGridView_missmatch.Columns.Add("LOTID", "LOTID#");
            dataGridView_missmatch.Columns.Add("UID", "UID");
            dataGridView_missmatch.Columns.Add("Qty", "Qty");
            dataGridView_missmatch.Columns.Add("투입형태", "투입형태");
            dataGridView_missmatch.Columns.Add("위치", "위치");
            dataGridView_missmatch.Columns.Add("제조일", "제조일");
            dataGridView_missmatch.Columns.Add("투입일", "투입일");
            dataGridView_missmatch.Columns.Add("제조사", "제조사");
            dataGridView_missmatch.Columns.Add("인치", "인치");
            dataGridView_missmatch.Columns.Add("MISS", "MISS");
             
             */


            foreach (DataGridViewRow row in dataGridView_asm.Rows)
            {
                if(dataGridView_amm.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["UID"].Value.ToString() == row.Cells["UID"].Value.ToString() && r.Cells["SID"].Value.ToString() == row.Cells["SID"].Value.ToString()).ToList().Count == 0)
                {
                    if(row.Cells["SID"].Value.ToString() != "SID_TEST")
                        dataGridView_missmatch.Rows.Add(new object[] { ++cnt, row.Cells["SID"].Value.ToString(), "", row.Cells["UID"].Value.ToString(), "", "", row.Cells["TowerID"].Value.ToString(), "", "", "", "", "TOWER" });
                }
            }


            /*
            dataGridView_amm.Columns.Add("NO", "NO");
            dataGridView_amm.Columns.Add("SID", "SID");
            dataGridView_amm.Columns.Add("Batch#", "Batch#");
            dataGridView_amm.Columns.Add("UID", "UID");
            dataGridView_amm.Columns.Add("Qty", "Qty");
            dataGridView_amm.Columns.Add("투입형태", "투입형태");
            dataGridView_amm.Columns.Add("위치", "위치");
            dataGridView_amm.Columns.Add("제조일", "제조일");
            dataGridView_amm.Columns.Add("투입일", "투입일");
            dataGridView_amm.Columns.Add("제조사", "제조사");
            dataGridView_amm.Columns.Add("인치", "인치");
             * */


            foreach (DataGridViewRow row in dataGridView_amm.Rows)
            {
                if (dataGridView_asm.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["UID"].Value.ToString() == row.Cells["UID"].Value.ToString() && r.Cells["SID"].Value.ToString() == row.Cells["SID"].Value.ToString()).ToList().Count == 0)
                {
                    dataGridView_missmatch.Rows.Add(new object[] { ++cnt, row.Cells["SID"].Value.ToString(), "", row.Cells["UID"].Value.ToString(), row.Cells["QTY"].Value.ToString(), row.Cells["투입형태"].Value.ToString(), row.Cells["위치"].Value.ToString(), row.Cells["제조일"].Value.ToString(), row.Cells["투입일"].Value.ToString(), row.Cells["제조사"].Value.ToString(), row.Cells["인치"].Value.ToString(), "AMM" });
                }
            }
        }

        private void button_sync_Click(object sender, EventArgs e)
        {
            

            if (nDbUpdate != 2)
            {
                MessageBox.Show("Missmatch 확인을 먼저 하십시오");
                return;
            }

            if (IsDateGathering == true)
                return;

            //경고 메세지

            DialogResult ret = MessageBox.Show("동기화 하시겠습까?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ret != DialogResult.Yes)
                return;

            IsDateGathering = true;

            //AMM delete
            int n = comboBox_sel.SelectedIndex;

            string strEqid = "";
            if (n == 0)
                strEqid = "TWR1";
            else if (n == 1)
                strEqid = "TWR2";
            else if (n == 2)
                strEqid = "TWR3";

            int nCount = dataGridView_missmatch.Rows.Count;

            List<StorageData_Compare> uploadList = new List<StorageData_Compare>();

            for (int i = 0; i < nCount; i++)
            {
                StorageData_Compare data = new StorageData_Compare();

                data.SID = dataGridView_missmatch.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_missmatch.Rows[i].Cells[2].Value.ToString(); //LOTOD
                data.UID = dataGridView_missmatch.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_missmatch.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_type = dataGridView_missmatch.Rows[i].Cells[5].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_missmatch.Rows[i].Cells[6].Value.ToString(); //위치
                data.Production_date = dataGridView_missmatch.Rows[i].Cells[7].Value.ToString(); //제조일
                data.Input_date = dataGridView_missmatch.Rows[i].Cells[8].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_missmatch.Rows[i].Cells[9].Value.ToString(); //제조사
                data.Inch = dataGridView_missmatch.Rows[i].Cells[10].Value.ToString(); //인치
                data.Miss = dataGridView_missmatch.Rows[i].Cells[11].Value.ToString(); //Miss Type

                uploadList.Add(data);
            }
            //Tower번호;UID;SID;LOTID;QTY;제조사;제조일;INCH;투입TYPE

            int nNGcount = 0;
            foreach (var item in uploadList)
            {
                if (item.Miss == "AMM")
                {
                    string strFormat = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", item.Tower_no, item.UID, item.SID, item.LOTID, item.Quantity,
                        item.Manufacturer, item.Production_date, "NO INFO", "CART");

                    string strJudge = AMM_Main.AMM.SetLoadComplete("AJ54100", strEqid, strFormat, false);

                    if (strJudge == "NG")
                    {
                        nNGcount++;
                    }
                    else if (strJudge == "DUPLICATE")
                    {
                        nNGcount++;
                    }
                }
                else if (item.Miss == "ASM")
                {
                    string strJudge = AMM_Main.AMM.Delete_MTL_Info(item.UID);

                    if (strJudge == "NG")
                    {
                        nNGcount++;
                    }
                }

                Application.DoEvents();
                Thread.Sleep(100);
            }

            if (nNGcount > 0)
            {
                string str = string.Format("실패 {0}개", nNGcount);
                MessageBox.Show(str);
            }

            dataGridView_missmatch.Columns.Clear();
            dataGridView_missmatch.Rows.Clear();

            IsDateGathering = false;

            MessageBox.Show("완료 되었습니다.");

            int nIndex = comboBox_sel.SelectedIndex;

            if (nIndex == 0)
            {
                Fnc_Process_GetMaterials_Tower1();
                Fnc_Process_GetAMMinfo("TWR1");
            }
            else if (nIndex == 1)
            {
                Fnc_Process_GetMaterials_Tower2();
                Fnc_Process_GetAMMinfo("TWR2");
            }
            else if (nIndex == 2)
            {
                Fnc_Process_GetMaterials_Tower3();
                Fnc_Process_GetAMMinfo("TWR3");
            }



            nDbUpdate = 0;
        }

        private void textBox_mtlinput_KeyPress(object sender, KeyPressEventArgs e)
        {
            int n = comboBox_searchtype.SelectedIndex;

            if (n == 0) //SID
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
            }

            if (e.KeyChar == (char)13)
            {
                string strid = "";
                int nLength = 0;

                strid = textBox_mtlinput.Text;
                nLength = strid.Length;

                if (nLength < 3)
                    return;

                Fnc_ProcessFind(n, strid);

                if (nLength == 4)
                {
                    int nCombocount = comboBox_sid.Items.Count;

                    if (nCombocount > 0 && n == 0)
                    {
                        comboBox_sid.SelectedIndex = 0;
                    }
                    else
                    {
                        textBox_mtlinput.Text = "";
                        textBox_mtlinput.Focus();
                    }
                }
                else
                {
                    comboBox_sid.Items.Clear();
                    comboBox_sid.Text = "";
                }

                label_scount.Text = comboBox_sid.Items.Count.ToString();
            }
        }

        private void button_timeset_Click(object sender, EventArgs e)
        {
            Form_Timeset Timeset_Form = new Form_Timeset();
            Timeset_Form.ShowDialog();

            IsDateGathering = true;

            label_Value_date_st.Text = strTimeset_date_st;
            label_Value_date_ed.Text = strTimeset_date_ed;
            label_Value_time_st.Text = strTimeset_hour_st + ":" + strTimeset_Min_st;
            label_Value_time_ed.Text = strTimeset_hour_ed + ":" + strTimeset_Min_ed;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            if (bSearch_sid)
            {
                button_search.Visible = true;
                textBox_sid.Visible = true;
                label_sid.Visible = true;

                comboBox_type2.SelectedIndex = 0;
                nType = 0;
                Fnc_Init_datagrid2(nType);

                textBox_sid.Focus();
            }
            else
            {
                button_search.Visible = false;
                textBox_sid.Visible = false;
                label_sid.Visible = false;

                Fnc_Init_datagrid2(nType);

                if (nGroup != 7)
                    Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));
            }

            IsDateGathering = false;
        }

        public void Fnc_Update_timeset()
        {
            IsDateGathering = true;

            DateTime dToday = DateTime.Now;

            strTimeset_date_st = string.Format("{0}-{1:00}-{2:00}", dToday.Year, dToday.Month, dToday.Day);
            strTimeset_date_ed = string.Format("{0}-{1:00}-{2:00}", dToday.Year, dToday.Month, dToday.Day);

            strTimeset_hour_st = "08";
            strTimeset_hour_ed = "17";
            strTimeset_Min_st = "30";
            strTimeset_Min_ed = "30";

            label_Value_date_st.Text = strTimeset_date_st;
            label_Value_date_ed.Text = strTimeset_date_ed;
            label_Value_time_st.Text = strTimeset_hour_st + ":" + strTimeset_Min_st;
            label_Value_time_ed.Text = strTimeset_hour_ed + ":" + strTimeset_Min_ed;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            Fnc_Init_datagrid2(nType);

            if (nGroup != 7)
                Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));

            IsDateGathering = false;
        }

        public void Fnc_InitMSSql()
        {
            if (MSSql != null)
                return;

            string connectionStr = string.Format("server=10.133.146.151;database=SiplaceMaterialManager;user id=sa;password=Siplace.1");
            MSSql = new MsSqlManager(connectionStr);

            if (MSSql.OpenTest() == false)
            {
                MessageBox.Show("ASM DB연결 실패");
                bASMconnect = false;
            }
            else
            {
                bASMconnect = true;
            }
        }
        private int Fnc_Process_GetAMMinfo(string strEquipid)
        {
            dataGridView_amm.Columns.Clear();
            dataGridView_amm.Rows.Clear();

            dataGridView_amm.Columns.Add("NO", "NO");
            dataGridView_amm.Columns.Add("SID", "SID");
            dataGridView_amm.Columns.Add("Batch#", "Batch#");
            dataGridView_amm.Columns.Add("UID", "UID");
            dataGridView_amm.Columns.Add("Qty", "Qty");
            dataGridView_amm.Columns.Add("투입형태", "투입형태");
            dataGridView_amm.Columns.Add("위치", "위치");
            dataGridView_amm.Columns.Add("제조일", "제조일");
            dataGridView_amm.Columns.Add("투입일", "투입일");
            dataGridView_amm.Columns.Add("제조사", "제조사");
            dataGridView_amm.Columns.Add("인치", "인치");

            var MtlList = AMM_Main.AMM.GetMTLInfo("AJ54100", strEquipid);

            strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<AMM_StorageData> list = new List<AMM_StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                AMM_StorageData data = new AMM_StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                list.Add(data);
            }

            list.Sort(CompareStorageData_AMM);

            int nIndex = 1;

            foreach (var item in list)
            {
                try
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity == "" ? "0" : item.Quantity));
                    string strdate = item.Input_date;
                    strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                        + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                    dataGridView_amm.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
                }
                catch (Exception ex)
                {

                }
                
            }

            return nMtlCount;
        }
        public void Fnc_Process_GetMaterials_Tower1()
        {
            string tid = "";

            tid = "T0101";
            var simmList1 = GetSIMMMaterialList(strASM_TowerLocation1, tid);

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("LOTID", "LOTID");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");

            int idx = 1;
            foreach (var item in simmList1)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0102";
            var simmList2 = GetSIMMMaterialList(strASM_TowerLocation1, tid);

            foreach (var item in simmList2)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0103";
            var simmList3 = GetSIMMMaterialList(strASM_TowerLocation1, tid);

            foreach (var item in simmList3)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0104";
            var simmList4 = GetSIMMMaterialList(strASM_TowerLocation1, tid);

            foreach (var item in simmList4)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }
        }

        public void Fnc_Process_GetMaterials_Tower2()
        {
            string tid = "";

            tid = "T0201";
            var simmList1 = GetSIMMMaterialList(strASM_TowerLocation2, tid);

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("LOTID", "LOTID");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");

            int idx = 1;
            foreach (var item in simmList1)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0202";
            var simmList2 = GetSIMMMaterialList(strASM_TowerLocation2, tid);

            foreach (var item in simmList2)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0203";
            var simmList3 = GetSIMMMaterialList(strASM_TowerLocation2, tid);

            foreach (var item in simmList3)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0204";
            var simmList4 = GetSIMMMaterialList(strASM_TowerLocation2, tid);

            foreach (var item in simmList4)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }
        }

        public void Fnc_Process_GetMaterials_Tower3()
        {
            string tid = "";

            tid = "T0301";
            var simmList1 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("LOTID", "LOTID");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");

            int idx = 1;
            foreach (var item in simmList1)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }
            tid = "T0302";
            var simmList2 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            foreach (var item in simmList2)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0303";
            var simmList3 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            foreach (var item in simmList3)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0304";
            var simmList4 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            foreach (var item in simmList4)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }
        }

        public void MissmatchDB2Backup()
        {
            /*
            0 dataGridView_missmatch.Columns.Add("NO", "NO");
            1 dataGridView_missmatch.Columns.Add("SID", "SID");
            2 dataGridView_missmatch.Columns.Add("LOTID", "LOTID#");
            3 dataGridView_missmatch.Columns.Add("UID", "UID");
            4 dataGridView_missmatch.Columns.Add("Qty", "Qty");
            5 dataGridView_missmatch.Columns.Add("투입형태", "투입형태");
            6 dataGridView_missmatch.Columns.Add("위치", "위치");
            7 dataGridView_missmatch.Columns.Add("제조일", "제조일");
            8 dataGridView_missmatch.Columns.Add("투입일", "투입일");
            9 dataGridView_missmatch.Columns.Add("제조사", "제조사");
            10 dataGridView_missmatch.Columns.Add("인치", "인치");
            11 dataGridView_missmatch.Columns.Add("MISS", "MISS");           
            */


            List<DataGridViewRow> rowList = new List<DataGridViewRow>();

            foreach(DataGridViewRow row in dataGridView_asm.Rows)
            {
                rowList = dgv_backup.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["UID"].Value.ToString() == row.Cells["UID"].Value.ToString()).ToList();

                if(rowList.Count == 0 && (row.Cells["위치"].Value.ToString().Contains("T0") == true || cb_visible.Checked == true )) 
                {
                    dataGridView_missmatch.Rows.Add(
                        dataGridView_missmatch.RowCount + 1,
                        row.Cells["SID"].Value.ToString(),
                        row.Cells["LOTID"].Value.ToString(),
                        row.Cells["UID"].Value.ToString(),
                        row.Cells["Qty"].Value.ToString(),
                        "",
                        row.Cells["위치"].Value.ToString(),
                        row.Cells["제조일"].Value.ToString(),
                        row.Cells["투입일"].Value.ToString(),                        
                        row.Cells["제조사"].Value.ToString(),
                        row.Cells["인치"].Value.ToString(),
                        "Backup"                        
                        );

                    dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].DefaultCellStyle.BackColor = Color.LimeGreen;
                }
                else if(rowList.Count == 1 && row.Cells["위치"].Value.ToString().Contains("T0") == true )
                {
                    bool isIt = false;
                    int a = 0x00;

                    if (row.Cells["SID"].Value.ToString() != rowList[0].Cells["SID"].Value.ToString())
                    {
                        //rowList[0].Cells["SID"].Style.BackColor = Color.LimeGreen;
                        //isIt = true;
                    }

                    if (row.Cells["LOTID"].Value.ToString() != rowList[0].Cells["LOTID"].Value.ToString())
                    {
                        rowList[0].Cells["LOTID"].Style.BackColor = Color.LimeGreen;
                        isIt = true;
                        a |= 0x01;
                    }

                    if (row.Cells["UID"].Value.ToString() != rowList[0].Cells["UID"].Value.ToString())
                    {
                        rowList[0].Cells["UID"].Style.BackColor = Color.LimeGreen;
                        isIt = true;
                        a |= 0x02;
                    }

                    if (row.Cells["Qty"].Value.ToString() != rowList[0].Cells["Qty"].Value.ToString())
                    {
                        rowList[0].Cells["Qty"].Style.BackColor = Color.LimeGreen;
                        isIt = true;
                        a |= 0x04;
                    }

                    if (row.Cells["위치"].Value.ToString() != rowList[0].Cells["위치"].Value.ToString())
                    {
                        rowList[0].Cells["위치"].Style.BackColor = Color.LimeGreen;

                        if(row.Cells["위치"].Value.ToString().Contains("T0") == true || cb_visible.Checked == true)
                            isIt = true;
                        a |= 0x10;
                    }


                    if (row.Cells["제조일"].Value.ToString() != rowList[0].Cells["제조일"].Value.ToString())
                    {
                        rowList[0].Cells["제조일"].Style.BackColor = Color.LimeGreen;
                        //isIt = true;
                        a |= 0x20;
                    }


                    if (row.Cells["투입일"].Value.ToString() != rowList[0].Cells["투입일"].Value.ToString())
                    {
                        rowList[0].Cells["투입일"].Style.BackColor = Color.LimeGreen;
                        //isIt = true;
                        a |= 0x30;
                    }

                    
                    if (row.Cells["제조사"].Value.ToString() != rowList[0].Cells["제조사"].Value.ToString())
                    {
                        rowList[0].Cells["제조사"].Style.BackColor = Color.LimeGreen;
                        //isIt = true;
                        a |= 0x40;
                    }

                    if (row.Cells["인치"].Value.ToString() != rowList[0].Cells["인치"].Value.ToString())
                    {
                        rowList[0].Cells["인치"].Style.BackColor = Color.LimeGreen;
                        //isIt = true;
                        a |= 0x80;
                    }


                    if (isIt == true)
                    {
                        dataGridView_missmatch.Rows.Add(
                        dataGridView_missmatch.RowCount + 1,
                        row.Cells["SID"].Value.ToString(),
                        row.Cells["LOTID"].Value.ToString(),
                        row.Cells["UID"].Value.ToString(),
                        row.Cells["Qty"].Value.ToString(),
                        "",
                        row.Cells["위치"].Value.ToString(),
                        row.Cells["제조일"].Value.ToString(),
                        row.Cells["투입일"].Value.ToString(),
                        row.Cells["제조사"].Value.ToString(),
                        row.Cells["인치"].Value.ToString(),
                        "Backup"
                        );

                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["LOTID"].Style.BackColor = ((a) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["UID"].Style.BackColor = ((a >> 1) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["Qty"].Style.BackColor = ((a >> 2) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["위치"].Style.BackColor = ((a >> 3) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["제조일"].Style.BackColor = ((a >> 4) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["투입일"].Style.BackColor = ((a >> 5) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["제조사"].Style.BackColor = ((a >> 6) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["인치"].Style.BackColor = ((a >> 7) & 0x01) == 0x01 ? Color.LimeGreen : Color.FromArgb(255, 255, 192);
                    }
                }
                else
                {

                }

            }

            foreach(DataGridViewRow row in dgv_backup.Rows)
            {
                /*
                 dataGridView_asm.Columns.Add("NO", "NO");
                dataGridView_asm.Columns.Add("SID", "SID");
                dataGridView_asm.Columns.Add("LOTID", "LOTID");
                dataGridView_asm.Columns.Add("UID", "UID");
                dataGridView_asm.Columns.Add("Qty", "Qty");
                dataGridView_asm.Columns.Add("투입일", "투입일");
                dataGridView_asm.Columns.Add("제조일", "제조일");
                dataGridView_asm.Columns.Add("제조사", "제조사");
                dataGridView_asm.Columns.Add("위치", "위치");
                 */

                rowList = dataGridView_asm.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["UID"].Value.ToString() == row.Cells["UID"].Value.ToString()).ToList();

                if(rowList.Count == 0 && (row.Cells["위치"].Value.ToString().Contains("T0") == true || cb_visible.Checked == true))
                {
                    dataGridView_missmatch.Rows.Add(
                        dataGridView_missmatch.RowCount + 1,
                        row.Cells["SID"].Value.ToString(),
                        row.Cells["LOTID"].Value.ToString(),
                        row.Cells["UID"].Value.ToString(),
                        row.Cells["Qty"].Value.ToString(),
                        "",
                        row.Cells["위치"].Value.ToString(),
                        row.Cells["제조일"].Value.ToString(),
                        row.Cells["투입일"].Value.ToString(),
                        row.Cells["제조사"].Value.ToString(),
                        row.Cells["인치"].Value.ToString(),
                        "Local_DB");

                    dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].DefaultCellStyle.BackColor = Color.Lime;
                }
                else if(rowList.Count == 1 && row.Cells["위치"].Value.ToString().Contains("T0") == true)
                {
                    bool isIt = false;
                    int a = 0x00;

                    if (row.Cells["SID"].Value.ToString() != rowList[0].Cells["SID"].Value.ToString())
                    {
                        //rowList[0].Cells["SID"].Style.BackColor = Color.LimeGreen;
                        //isIt = true;
                    }

                    if (row.Cells["LOTID"].Value.ToString() != rowList[0].Cells["LOTID"].Value.ToString())
                    {
                        rowList[0].Cells["LOTID"].Style.BackColor = Color.Lime;
                        isIt = true;
                        a |= 0x01;
                    }

                    if (row.Cells["UID"].Value.ToString() != rowList[0].Cells["UID"].Value.ToString())
                    {
                        rowList[0].Cells["UID"].Style.BackColor = Color.Lime;
                        isIt = true;
                        a |= 0x02;
                    }

                    if (row.Cells["Qty"].Value.ToString() != rowList[0].Cells["Qty"].Value.ToString())
                    {
                        rowList[0].Cells["Qty"].Style.BackColor = Color.Lime;
                        isIt = true;
                        a |= 0x04;
                    }

                    if (row.Cells["위치"].Value.ToString() != rowList[0].Cells["위치"].Value.ToString())
                    {
                        rowList[0].Cells["위치"].Style.BackColor = Color.Lime;
                        isIt = true;
                        a |= 0x10;
                    }


                    if (row.Cells["제조일"].Value.ToString() != rowList[0].Cells["제조일"].Value.ToString())
                    {
                        rowList[0].Cells["제조일"].Style.BackColor = Color.Lime;
                        //isIt = true;
                        a |= 0x20;
                    }


                    if (row.Cells["투입일"].Value.ToString() != rowList[0].Cells["투입일"].Value.ToString())
                    {
                        rowList[0].Cells["투입일"].Style.BackColor = Color.Lime;
                        //isIt = true;
                        a |= 0x30;
                    }


                    if (row.Cells["제조사"].Value.ToString() != rowList[0].Cells["제조사"].Value.ToString())
                    {
                        rowList[0].Cells["제조사"].Style.BackColor = Color.Lime;
                        //isIt = true;
                        a |= 0x40;
                    }

                    if (row.Cells["인치"].Value.ToString() != rowList[0].Cells["인치"].Value.ToString())
                    {
                        rowList[0].Cells["인치"].Style.BackColor = Color.Lime;
                        //isIt = true;
                        a |= 0x80;
                    }


                    if (isIt == true)
                    {
                        dataGridView_missmatch.Rows.Add(
                        dataGridView_missmatch.RowCount + 1,
                        rowList[0].Cells["SID"].Value.ToString(),
                        rowList[0].Cells["LOTID"].Value.ToString(),
                        rowList[0].Cells["UID"].Value.ToString(),
                        rowList[0].Cells["Qty"].Value.ToString(),
                        "",
                        rowList[0].Cells["위치"].Value.ToString(),
                        rowList[0].Cells["제조일"].Value.ToString(),
                        rowList[0].Cells["투입일"].Value.ToString(),
                        rowList[0].Cells["제조사"].Value.ToString(),
                        rowList[0].Cells["인치"].Value.ToString(),
                        "Local_DB"
                        );

                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["LOTID"].Style.BackColor = ((a) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["UID"].Style.BackColor = ((a >> 1) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["Qty"].Style.BackColor = ((a >> 2) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["위치"].Style.BackColor = ((a >> 3) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["제조일"].Style.BackColor = ((a >> 4) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["투입일"].Style.BackColor = ((a >> 5) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["제조사"].Style.BackColor = ((a >> 6) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                        dataGridView_missmatch.Rows[dataGridView_missmatch.RowCount - 1].Cells["인치"].Style.BackColor = ((a >> 7) & 0x01) == 0x01 ? Color.Lime : Color.FromArgb(255, 255, 192);
                    }
                }
                else
                {

                }
            }
        }

        public int Fnc_Missmatch_ASMcompare(int idx)
        {
            /*
             dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("LOTID", "LOTID");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");
             */

            List<StorageData_Compare> asmList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_asm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_asm.Rows[i].Cells["SID"].Value.ToString(); //SID                
                data.UID = dataGridView_asm.Rows[i].Cells["UID"].Value.ToString(); //UID
                data.Tower_no = dataGridView_asm.Rows[i].Cells["위치"].Value.ToString();
                data.Quantity = dataGridView_asm.Rows[i].Cells["Qty"].Value.ToString();
                data.LOTID = dataGridView_asm.Rows[i].Cells["LOTID"].Value.ToString();
                data.Manufacturer = dataGridView_asm.Rows[i].Cells["제조사"].Value.ToString();
                data.Input_date = dataGridView_asm.Rows[i].Cells["투입일"].Value.ToString();
                data.Production_date = dataGridView_asm.Rows[i].Cells["제조일"].Value.ToString();
                data.Inch = dataGridView_asm.Rows[i].Cells["인치"].Value.ToString();


                if (data.UID != "")
                    asmList.Add(data);
            }

            asmList.Sort(CompareStorageData);

            List<StorageData_Compare> ammList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_amm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_amm.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_amm.Rows[i].Cells[2].Value.ToString(); //LOTOD
                data.UID = dataGridView_amm.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_amm.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_type = dataGridView_amm.Rows[i].Cells[5].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_amm.Rows[i].Cells[6].Value.ToString(); //위치
                data.Production_date = dataGridView_amm.Rows[i].Cells[7].Value.ToString(); //제조일
                data.Input_date = dataGridView_amm.Rows[i].Cells[8].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_amm.Rows[i].Cells[9].Value.ToString(); //제조사
                data.Inch = dataGridView_amm.Rows[i].Cells[10].Value.ToString(); //인치

                if (data.UID != "")
                {
                    ammList.Add(data);                    
                }
            }

            ammList.Sort(CompareStorageData);

            var missmatchList = GetMissMatchList(asmList, ammList);


            if(comboBox_sel.SelectedIndex > 3)
            { 
                //DataTable dt = new DataTable();

                //using (SqlConnection c = new SqlConnection("server=10.135.200.35;database=ATK4-AMM-DBv1;user id=amm;password=amm@123"))
                //{
                //    c.Open();

                //    using (SqlCommand cmd = new SqlCommand(string.Format("select VALUE from PUBLIC_SETTINGS with(nolock) where [NAME] = 'TOWER{0}_PATH' and [TYPE] ='AMM_SYNC'", comboBox_sel.SelectedIndex + 1), c))
                //    {
                //        using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                //        {
                //            adt.Fill(dt);
                //        }
                //    }
                //}

                    //string DB_path = dt.Rows[0][0].ToString();

                foreach (var item in missmatchList)
                {
                    try
                    {
                        //DataConn conn1 = new DataConn();
                        //DataSet ds;

                        //string sql = string.Format("SELECT [Article] from [Carrier] where [Carrier]='{0}'", item.UID);


                        ////string sql = @"SELECT ROW_NUMBER() OVER(order by(select 1)) as [NO], ROW_NUMBER() OVER(order by(select 1)) as [SID], [Batch],  [Carrier], [Stock], [DepotDate], [CreateDate], [Manufactur], SUBSTRING(Depot,7,5) from [Carrier]";

                        //ds = conn1.GetDataset(sql, DB_path);

                        //ds = conn1.GetDataset(string.Format("SELECT [Article] from [Article] where [ID]={0}", ds.Tables[0].Rows[0]["Article"]), DB_path);

                        //if (ds.Tables[0].Rows.Count != 0)
                        {
                            dataGridView_missmatch.Rows.Add(new object[12] { idx++, item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no,
                    item.Production_date, item.Input_date, item.Manufacturer,item.Inch, "AMM" });

                            dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.BackColor = Color.White;
                            dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.ForeColor = Color.Blue;

                            Synclog.Info(string.Format("Tower missmatch data added : {0}", item.UID));
                        }
                    //    else
                    //    {
                    //        dataGridView_missmatch.Rows.Add(new object[12] { idx++, "", item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no,
                    //item.Production_date, item.Input_date, item.Manufacturer,item.Inch, "AMM" });

                    //        dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.BackColor = Color.White;
                    //        dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.ForeColor = Color.Blue;

                    //        Synclog.Info(string.Format("Tower missmatch data added : {0}", item.UID));
                    //    }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                
            }

            return idx;
        }

        public int Fnc_Missmatch_AMMcompare(int idx)
        {
            List<StorageData_Compare> asmList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_asm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_asm.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_asm.Rows[i].Cells[2].Value.ToString(); //LOTID
                data.UID = dataGridView_asm.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_asm.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_date = dataGridView_asm.Rows[i].Cells[5].Value.ToString(); //투입일
                data.Production_date = dataGridView_asm.Rows[i].Cells[6].Value.ToString(); //제조일
                data.Manufacturer = dataGridView_asm.Rows[i].Cells[7].Value.ToString(); //제조사
                data.Tower_no = dataGridView_asm.Rows[i].Cells[8].Value.ToString(); //위치

                if (data.UID != "")
                    asmList.Add(data);
            }

            asmList.Sort(CompareStorageData);

            List<StorageData_Compare> ammList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_amm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_amm.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_amm.Rows[i].Cells[2].Value.ToString(); //LOTOD
                data.UID = dataGridView_amm.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_amm.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_type = dataGridView_amm.Rows[i].Cells[5].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_amm.Rows[i].Cells[6].Value.ToString(); //위치
                data.Production_date = dataGridView_amm.Rows[i].Cells[7].Value.ToString(); //제조일
                data.Input_date = dataGridView_amm.Rows[i].Cells[8].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_amm.Rows[i].Cells[9].Value.ToString(); //제조사
                data.Inch = dataGridView_amm.Rows[i].Cells[10].Value.ToString(); //인치

                if (data.UID != "")
                {
                    ammList.Add(data);                    
                }
            }

            ammList.Sort(CompareStorageData);

            var missmatchList = GetMissMatchList(ammList, asmList);

            foreach (var item in missmatchList)
            {
                try
                {
                    if (item.Tower_no.Contains("T0") == true || cb_visible.Checked == true)
                    {
                        dataGridView_missmatch.Rows.Add(new object[12] { idx++, item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no,
                    item.Production_date, item.Input_date, item.Manufacturer,item.Inch, "TOWER" });
                        dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.BackColor = Color.White;
                        dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.ForeColor = Color.Orange;

                        Synclog.Info(string.Format("AMM missmatch data added : {0}", item.UID));
                    }
                }
                catch (Exception ex)
                {

                    
                }
                
            }

            return idx;
        }

        public List<StorageData_Compare> GetMissMatchList(List<StorageData_Compare> source, List<StorageData_Compare> compare)
        {
            List<StorageData_Compare> retList = new List<StorageData_Compare>();
            List<string> compareList = new List<string>();
            bool isCompare = false;

            foreach (var item in compare)
                compareList.Add(item.UID);

            for (int i = 0; i < source.Count; i++)
            {
                isCompare = false;

                for (int j = 0; j < compare.Count; j++)
                {
                    if ((source[i].UID == compare[j].UID) && source[i].Tower_no == compare[j].Tower_no)
                    {
                        isCompare = true;
                        break;
                    }
                }

                if (isCompare == false)
                {                    
                    if(source[i].Tower_no.Contains("T0") == true || cb_visible.Checked == true)
                        retList.Add(source[i]);
                }
            }


            return retList;
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            if (textBox_sid.Text == "")
            {
                MessageBox.Show("SID 를 입력 하세요!");
                textBox_sid.Focus();
                return;
            }

            IsDateGathering = true;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            comboBox_type2.SelectedIndex = 0;
            comboBox_group2.Text = "전체 조회";

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            //int nGroup = comboBox_group2.SelectedIndex + 1;

            //string strEquipid = "TWR" + nGroup.ToString();

            Fnc_Init_datagrid2(nType);

            if (strDate_st == "" || strDate_st == "")
            {
                IsDateGathering = false;
                return;
            }

            Fnc_Process_GetINOUT_mtlinfo_Sid(nType, textBox_sid.Text, Double.Parse(strDate_st), Double.Parse(strDate_ed));

            IsDateGathering = false;
        }

        private void dataGridView_info_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_find_TextChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click_1(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_month_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void dataGridView_longterm_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }







        //[210813_Sangik.choi_장기보관관리기능추가(이종명수석님)

        
        //[210819_Sangik.choi_로그함수추가

        public void Fnc_SaveLog(string strLog, int nType) ///설비별 개별 로그 저장
        {
            string strPath = "";
            if (nType == 0)
                strPath = AMM_Main.strLogfilePath + "\\AMM_system_";
            else if (nType == 1)
                strPath = AMM_Main.strLogfilePath + "\\AMM_order_";
            else if (nType == 2)
                strPath = AMM_Main.strLogfilePath + "\\AMM_setting_";

            string strToday = string.Format("{0}{1:00}{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format(",{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            strPath = strPath + strToday + ".txt";
            strHead = strToday + strHead;

            string strSave;
            strSave = strHead + ',' + strLog;
            Fnc_WriteFile(strPath, strSave);
        }

        private void Fnc_WriteFile(string strFileName, string strLine)
        {
            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(strFileName, true))
            {
                file.WriteLine(strLine);
            }
        }
        //]210819_Sangik.choi_로그함수추가


        private void Fnc_Picklist_Send2(string strlincode, string strequip, string strPickID)
        {
            if (strPickID == "")
            {
                MessageBox.Show("배출 ID 정보가 없습니다.");
                return;
            }
            ///Picklist 생성
            DataTable dt = AMM_Main.AMM.GetPickingReadyinfo_ID(strPickID);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                MessageBox.Show("리스트 생성 목록이 없습니다.");
                return;
            }

            StorageData data = new StorageData();

            string strJudge = "";

            for (int i = 0; i < nCount; i++)
            {
                data.Linecode = dt.Rows[i]["LINE_CODE"].ToString(); data.Linecode = data.Linecode.Trim();
                data.Equipid = dt.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.UID = dt.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.Requestor = dt.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();
                data.Tower_no = dt.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.SID = dt.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = dt.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = dt.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = dt.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = dt.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = dt.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = dt.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                strJudge = AMM_Main.AMM.SetPicking_Listinfo(strlincode, strequip, strPickID, data.UID, textBox_badge.Text, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM");

                if (strJudge == "NG")
                {
                    MessageBox.Show("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                    AMM_Main.strAMM_Connect = "NG";
                    return;
                }
                else if (strJudge == "DUPLICATE")
                {
                    string str = string.Format("자재 리스트가 중복 되었습니다.\n SID = '{0}', UID = '{1}'", data.SID, data.UID);
                    MessageBox.Show(str);
                    return;
                }
            }

            strJudge = AMM_Main.AMM.Delete_PickReadyinfo(strlincode, strPickID);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";

                return;
            }
            ///Pick ID Info
            ///
            strJudge = AMM_Main.AMM.SetPickingID(strlincode, strequip, strPickID, label_count.Text, AMM_Main.strRequestor_id);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";
                return;
            }

            string strLog = string.Format("PICK LIST 생성 완료 - 사번:{0}, PICKID:{1}, 수량:{2}", textBox_badge.Text, strPickID, nCount.ToString());


        }


        //[210817_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void Fnc_Picklist_Send(string strlincode, string strequip, string strPickID)
        {
            if (strPickID == "")
            {
                MessageBox.Show("배출 ID 정보가 없습니다.");
                return;
            }
            ///Picklist 생성
            DataTable dt = AMM_Main.AMM.GetPickingReadyinfo_ID(strPickID);
            
            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                MessageBox.Show("리스트 생성 목록이 없습니다.");
                return;
            }

            StorageData data = new StorageData();

            string strJudge = "";

            for (int i = 0; i < nCount; i++)
            {
                data.Linecode = dt.Rows[i]["LINE_CODE"].ToString(); data.Linecode = data.Linecode.Trim();
                data.Equipid = dt.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.UID = dt.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.Requestor = dt.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();
                data.Tower_no = dt.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.SID = dt.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = dt.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = dt.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = dt.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = dt.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = dt.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = dt.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                strJudge = AMM_Main.AMM.SetPicking_Listinfo(strlincode, strequip, strPickID, data.UID, textBox_badge.Text, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM");
                 
                if (strJudge == "NG")
                {
                    MessageBox.Show("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                    AMM_Main.strAMM_Connect = "NG";
                    return;
                }
                else if (strJudge == "DUPLICATE")
                {
                    string str = string.Format("자재 리스트가 중복 되었습니다.\n SID = '{0}', UID = '{1}'", data.SID, data.UID);
                    MessageBox.Show(str);
                    return;
                }
            }

            strJudge = AMM_Main.AMM.Delete_PickReadyinfo(strlincode, strPickID);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";

                return;
            }
            ///Pick ID Info
            ///
            strJudge = AMM_Main.AMM.SetPickingID(strlincode, strequip, strPickID, nCount.ToString(), textBox_badge.Text);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";
                return;
            }
            

            string strLog = string.Format("PICK LIST 생성 완료 - 사번:{0}, PICKID:{1}, 수량:{2}", textBox_badge.Text, strPickID, nCount.ToString());
        }

        //]210817_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void Fnc_PicklistSync_Send(string strlincode, string strequip, string strPickID)
        {
            if (strPickID == "")
            {
                MessageBox.Show("배출 ID 정보가 없습니다.");
                return;
            }
            ///Picklist 생성
            DataTable dt = AMM_Main.AMM.GetPickingReadyinfo_ID(strPickID);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                MessageBox.Show("리스트 생성 목록이 없습니다.");
                return;
            }

            StorageData data = new StorageData();

            string strJudge = "";

            for (int i = 0; i < nCount; i++)
            {
                data.Linecode = dt.Rows[i]["LINE_CODE"].ToString(); data.Linecode = data.Linecode.Trim();
                data.Equipid = dt.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.UID = dt.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.Requestor = dt.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();
                data.Tower_no = dt.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.SID = dt.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = dt.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = dt.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = dt.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = dt.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = dt.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = dt.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                strJudge = AMM_Main.AMM.SetPicking_Listinfo(strlincode, strequip, strPickID, data.UID, textBox_badge.Text, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "SYNC");

                if (strJudge == "NG")
                {
                    MessageBox.Show("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                    AMM_Main.strAMM_Connect = "NG";
                    return;
                }
                else if (strJudge == "DUPLICATE")
                {
                    string str = string.Format("자재 리스트가 중복 되었습니다.\n SID = '{0}', UID = '{1}'", data.SID, data.UID);
                    MessageBox.Show(str);
                    return;
                }
            }

            strJudge = AMM_Main.AMM.Delete_PickReadyinfo(strlincode, strPickID);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";

                return;
            }
            ///Pick ID Info
            ///
            strJudge = AMM_Main.AMM.SetPickingID(strlincode, strequip, strPickID, nCount.ToString(), textBox_badge.Text);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";
                return;
            }

            string strLog = string.Format("PICK LIST 생성 완료 - 사번:{0}, PICKID:{1}, 수량:{2}", textBox_badge.Text, strPickID, nCount.ToString());
        }


        //[210812_Sangik.choi_장기보관관리기능추가(이종명수석님)

        public void Fnc_Picklist_Comfirm()
        {
            string strPrefix = label_pickid_LT.Text.Substring(0, 2);

            int nCount = dataGridView_LTlist.Rows.Count;

            if (nCount < 1)
                return;

            for (int n = 0; n < nCount; n++)
            {
                string strPosition = dataGridView_LTlist.Rows[n].Cells[5].Value.ToString().Substring(2, 1);

                if (strPrefix == "PA" || strPrefix == "PD")
                {
                    if (strPosition != "1")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PB" || strPrefix == "PE")
                {
                    if (strPosition != "2")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PC" || strPrefix == "PF")
                {
                    if (strPosition != "3")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PG")
                {
                    if (strPosition != "4")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PH")
                {
                    if (strPosition != "5")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PJ")
                {
                    if (strPosition != "6")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
            }
        }

        //]210812_Sangik.choi_장기보관관리기능추가(이종명수석님)



        //[210812_Sangik.choi_장기보관관리기능추가(이종명수석님)

        public void Fnc_DeleteReady(int nindex)
        {
            string strDeleteUID;
            strDeleteUID = dataGridView_LTlist.Rows[nindex].Cells[2].Value.ToString();
            string strPickingID = label_pickid_LT.Text;

            //Delete_PickReadyinfo_ReelID()-query = string.Format("DELETE FROM TB_PICK_READY_INFO WHERE LINE_CODE='{0}' and UID='{1}'", strLinecode, strReelid);
            string strJudge = AMM_Main.AMM.Delete_PickReadyinfo_ReelID(AMM_Main.strDefault_linecode, strDeleteUID);

            if (strJudge == "NG")
            {
                AMM_Main.strAMM_Connect = "NG";
                return;
            }

        }
        //]210812_Sangik.choi_장기보관관리기능추가(이종명수석님)



        //[210810_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void button_display_Click(object sender, EventArgs e)
        {

            string pid = label_pickid_LT.Text;
            if (pid != "")
            {
                string result = AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, pid); //210817_Sangik.choi_ui 삭제 후 db 에서 삭제
                label_pickid_LT.Text = "";
                /*                if (result == "NG")
                                {

                                    MessageBox.Show("Ready info DB 확인 필요.");
                                    return;
                                }*/
            }

            Fnc_Init_datagrid_longterm();

            int idx = comboBox_month.SelectedIndex + 1;
            int nGroup = comboBox_L_group.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            if (idx <= 12 && idx >= 1)
            {
                if (comboBox_L_group.Text != "ALL")
                {
                    if (comboBox_L_group.SelectedIndex < 3)
                        Fnc_Process_GetMaterialinfo_longterm(idx, strEquipid);
                    else
                    {
                        Fnc_Process_GetMycronicinfo_longterm(idx, nGroup);
                    }
                }
                else
                {
                    Fnc_Process_GetMaterialinfo_longterm_All(comboBox_month.SelectedIndex + 1);
                }
            }
            else
            {
                Fnc_Process_GetMaterialinfo_All(1);
            }


            //Fnc_Get_PickID(strEquipid);
        }
        //]210810_Sangik.choi_장기보관관리기능추가(이종명수석님)
        

      

        public string GetTowerPath(int TowerNum)
        {
            DataTable dt = new DataTable();
            using (SqlConnection c = new SqlConnection("server=10.135.200.35;database=ATK4-AMM-DBv1;user id=amm;password=amm@123"))
            {
                c.Open();

                using (SqlCommand cmd = new SqlCommand($"SELECT [NAME],[VALUE],[TYPE] from [PUBLIC_SETTINGS] with(nolock) where NAME='TOWER{TowerNum}_PATH'", c))
                {
                    using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                    {
                        adt.Fill(dt);
                    }
                }
            }

            return dt.Rows[0][1].ToString();
        }


        //[210810_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void textBox_search_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                //Find
                Fnc_Find_longterm(textBox_search.Text);
            }
        }

        public void Fnc_Find_longterm(string strFind)
        {
            dataGridView_longterm.ClearSelection();

            int nCount = dataGridView_longterm.RowCount;
            int nCount2 = dataGridView_longterm.ColumnCount;



            bool bfind = false;

            for (int m = 0; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_longterm.Rows[n].Cells[m].Value.ToString();

                    if (str == strFind)
                    {
                        dataGridView_longterm.Rows[n].Cells[m].Selected = true;
                        dataGridView_longterm.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount; m = nCount2;
                    }
                }
            }

            if (bfind)
                return;

            for (int m = 0; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_longterm.Rows[n].Cells[m].Value.ToString();

                    if (str.Contains(strFind))
                    {
                        dataGridView_longterm.Rows[n].Cells[m].Selected = true;
                        dataGridView_longterm.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount; m = nCount2;
                    }
                }
            }
        }


        //[210810_Sangik.choi_장기보관관리기능추가(이종명수석님)


        private void button_delete_LT_Click(object sender, EventArgs e)
        {


            if (dataGridView_LTlist.CurrentCell == null)
            {
                MessageBox.Show("삭제할 Reel 이 없습니다.");

            }
            else
            {
                int current_index = dataGridView_LTlist.CurrentCell.RowIndex;

                //[210812_Sangik.choi_장기보관관리기능추가(이종명수석님)]
                StorageData_Compare data = new StorageData_Compare();

                data.SID = dataGridView_LTlist.Rows[current_index].Cells["SID"].Value.ToString(); //SID
                data.LOTID = dataGridView_LTlist.Rows[current_index].Cells["Batch#"].Value.ToString(); //LOTOD
                data.UID = dataGridView_LTlist.Rows[current_index].Cells["UID"].Value.ToString(); //UID
                data.Quantity = dataGridView_LTlist.Rows[current_index].Cells["Qty"].Value.ToString(); //QTY
                data.Input_type = dataGridView_LTlist.Rows[current_index].Cells["투입형태"].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_LTlist.Rows[current_index].Cells["위치"].Value.ToString(); //위치
                data.Production_date = dataGridView_LTlist.Rows[current_index].Cells["제조일"].Value.ToString(); //제조일
                data.Input_date = dataGridView_LTlist.Rows[current_index].Cells["투입일"].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_LTlist.Rows[current_index].Cells["제조사"].Value.ToString(); //제조사
                data.Inch = dataGridView_LTlist.Rows[current_index].Cells["인치"].Value.ToString(); //인치

                string result = AMM_Main.AMM.Delete_PickReadyinfo_ReelID(AMM_Main.strDefault_linecode, data.UID); //210817_Sangik.choi_ui 삭제 후 db 에서 삭제

                if (result == "OK")
                {
                    dataGridView_longterm.Rows.Add(new object[10] { data.SID, data.LOTID, data.UID, data.Quantity, data.Input_type, data.Tower_no, data.Production_date, data.Input_date, data.Manufacturer, data.Inch });
                    dataGridView_LTlist.Rows.Remove(dataGridView_LTlist.Rows[current_index]);
                    label_count.Text = dataGridView_LTlist.Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("Pick list 삭제 실패. DB 확인 필요");
                    return;

                }

            }

        }   //]210812_Sangik.choi_장기보관관리기능추가(이종명수석님)]


        private void button_addlist_Click(object sender, EventArgs e)
        {

            if (dataGridView_longterm.Rows.Count < 1)
            {
                MessageBox.Show("자재 조회 후 원하는 항목 선택 후 담아주세요");
                return;
            }

            

            int longterm_row_count = dataGridView_longterm.Rows.Count;

            int current_longterm_index = dataGridView_longterm.CurrentCell.RowIndex;

            int nGroup = comboBox_L_group.SelectedIndex + 1;
            string strEquipid = "TWR" + nGroup.ToString();
            
            string user_check = "";            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click_1(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox_sid_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_schedule_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox_sel.Items.Count; i++)
            {
                comboBox_sel.SelectedIndex = i;
                button_dbload_Click(sender, e);
                button_missmatch_Click(sender, e);
             
            }
            //string[] val = AMM_Main.AMM.ReadAutoSync().Split(',');

            ////string date, string time, string Interval, string val, string use
            //Form_schedule s = new Form_schedule(val[0], val[1], val[2], val[3], val[4]);

            //s.Show();
        }

        // 자동 업데이트 기능 추가 
        private void tAutosync_Tick(object sender, EventArgs e)
        {
            try
            {
                //AutoSyncParam = AMM_Main.AMM.ReadAutoSync().Split(',');

                //if (AutoSyncParam[4] == "1") // 사용 여부
                //{
                //    if (AutoSyncParam[2] == "일")
                //    {
                //        DateTime updateday = Convert.ToDateTime(AutoSyncParam[5]);
                //        if((DateTime.Now.Date - updateday.Date).Days > int.Parse(AutoSyncParam[3]))
                //        {
                //            DateTime dt = Convert.ToDateTime(AutoSyncParam[1]);

                //            if (DateTime.Now.Hour > dt.Hour)
                //            {
                //                RunSync(sender, e);
                //            }
                //            else if(DateTime.Now.Hour == dt.Hour && DateTime.Now.Minute >= dt.Minute)

                //            {
                //                RunSync(sender, e);
                //            }
                //        }
                //    }
                //    else if (AutoSyncParam[2] == "주")
                //    {

                //    }
                //    else if (AutoSyncParam[2] == "월")
                //    {

                //    }
                //}
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void RunSync(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox_sel.Items.Count; i++)
            {
                comboBox_sel.SelectedIndex = i;
                button_dbload_Click(sender, e);
                button_missmatch_Click(sender, e);
                
            }

            AMM_Main.AMM.WriteAutoSync(string.Format("update TB_AUTO_SYNC set UPDATE_DAY='{0}' where UPDATE_NO=1", DateTime.Now.ToString("yyyy-MM-dd")));
        }

        string[] AutoSyncParam; //0:date, 1:time, 2:interval, 3:val, 4:use, 5:day
        DateTime bdate = DateTime.Now.AddDays(-1);



        private int GetDay()
        {
            int res = -1;

            if (AutoSyncParam[2] == "일")
            {
                res = int.Parse(AutoSyncParam[3]);
            }
            else if (AutoSyncParam[2] == "주")
            {

            }
            else if (AutoSyncParam[2] == "월")
            {

            }

            return res;
        }

        private void Form_ITS_Load(object sender, EventArgs e)
        {
            //CheckForIllegalCrossThreadCalls = false;

            //dgv_sorter.DefaultCellStyle.SelectionBackColor = Color.Green;
            //dgv_tower.DefaultCellStyle.SelectionBackColor = Color.Green;
            //dgv_fail.DefaultCellStyle.SelectionBackColor = Color.Green;

            dgv_sorter.DoubleBuffered(true);
            dgv_tower.DoubleBuffered(true);
            dgv_fail.DoubleBuffered(true);

            dataGridView_info.DoubleBuffered(true);

            
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        Thread SorterThread;
        Thread TowerThread;
        bool bSorterThread = false;
        bool bTowerThread = false;

        private void button4_Click(object sender, EventArgs e)
        {
            SorterThread = new Thread(ReadSorterData);
            TowerThread = new Thread(ReadTowerData);

            try
            {
                if (bSorterThread == false && bTowerThread == false && bgwSorter.IsBusy == false)
                {
                    if (bSorterThread == false)
                        SorterThread.Start();

                    if (bTowerThread == false)
                        TowerThread.Start();

                    //dgv_fail.Rows.Clear();
                    dgv_fail.BeginInvoke(new Action(() => { dgv_fail.DataSource = null; }));


                    if (bgwSorter.IsBusy == false)
                        bgwSorter.RunWorkerAsync();
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        string AMMDBConnectionString = "server=10.135.200.35;uid=amm;pwd=amm@123;database=ATK4-AMM-DBv1";
        string SORTERDBConnectionString = "server=10.131.15.18;uid=eeuser_r;pwd=AmkorEE123!;database=EE";
        const string SorterCompState_Complete = "1";
        const string SorterCompState_InMiss = "2";
        const string SorterCompState_Fail = "3";


        DataTable SorterData;

        private void ReadSorterData()
        {
            bSorterThread = true;
            string date = "";

            dgv_sorter.BeginInvoke(new Action(() => { dgv_sorter.DataSource = null; }));
            //dgv_sorter.Columns.Clear();
            

            try
            {            
                if (SDTSort.Value == EDTSort.Value)
                {
                    date = string.Format("([DATE] = '{0}')", SDTSort.Value.Date.ToString("yyyyMMdd"));
                }
                else
                {
                    date = string.Format("([DATE] >= '{0}' and [DATE] <= '{1}')", SDTSort.Value.Date.ToString("yyyyMMdd"), EDTSort.Value.Date.ToString("yyyyMMdd"));
                }

                date += "and (";

                if(ch_seq1.Visible == true && ch_seq1.Checked == true)
                {

                    date += "[Seq]=1";
                }

                if (ch_seq2.Visible == true && ch_seq2.Checked == true)
                {
                    if(ch_seq1.Visible == true && ch_seq1.Checked == true)
                    {
                        date += " OR ";
                    }
                    date += "[Seq]=2";
                }

                if (ch_seq3.Visible == true && ch_seq3.Checked == true)
                {
                    if (ch_seq2.Visible == true && ch_seq2.Checked == true)
                    {
                        date += " OR ";
                    }
                    else if (ch_seq1.Visible == true && ch_seq1.Checked == true)
                    {
                        date += " OR ";
                    }

                    date += "[Seq]=3";
                }

                if (ch_seq4.Visible == true && ch_seq4.Checked == true)
                {
                    if (ch_seq3.Visible == true && ch_seq3.Checked == true)
                    {
                        date += " OR ";
                    }
                    else if (ch_seq2.Visible == true && ch_seq2.Checked == true)
                    {
                        date += " OR ";
                    }
                    else if (ch_seq1.Visible == true && ch_seq1.Checked == true)
                    {
                        date += "OR";
                    }

                        date += "[Seq]=4";
                }                               

                date += ")";

                string sql = string.Format("select [SID], [RID], [QTY], [size], [target], [End], [Seq] from vReelSorterResult with(Nolock) where {0} order by[RID]", date);

                SorterData = SearchData(SORTERDBConnectionString, sql);
                SorterData.Columns.Add();

                SorterData.Columns["RID"].ColumnName = "UID";
                dgv_sorter.BeginInvoke(new Action(() => 
                {
                    dgv_sorter.DataSource = SorterData;

                    dgv_sorter.Columns[0].Visible = false;
                    dgv_sorter.Columns[SorterData.Columns.Count - 1].Visible = false;
                    dgv_sorter.Columns[SorterData.Columns.Count - 1].ReadOnly = false;

                    dgv_sorter.Columns[2].Width = 70;
                    dgv_sorter.Columns[3].Width = 30;
                    dgv_sorter.Columns[4].Width = 50;
                    dgv_sorter.Columns[6].Width = 50;
                }));

                //SorterData.Columns.Add();
                //dgv_sorter.DataSource = SorterData;

                

                bSorterThread = false;
            }
            catch (Exception ex)
            {

            }

        }


        DataTable TowerData;
        string tt = "";

        private void ReadTowerData()
        {
            bTowerThread = true;
            string date = "";
            //dgv_tower.Columns.Clear();

            dgv_tower.BeginInvoke(new Action(() => { dgv_tower.DataSource = null; }));
            

            try
            {
                if (SDTTower.Value == EDTTower.Value)
                {
                    date = string.Format("[DATETIME] like '{0}%'", SDTTower.Value.Date.ToString("yyyyMMdd"));
                }
                else
                {
                    date = string.Format("([DATETIME] >= '{0}000000' AND [DATETIME] <= '{1}999999')", SDTTower.Value.Date.ToString("yyyyMMdd"), EDTTower.Value.Date.ToString("yyyyMMdd"));
                }

                string sql = string.Format("select [UID], [QTY], [INCH_INFO], [EQUIP_ID], [DATETIME] from TB_PICK_INOUT_HISTORY with(NOLOCK) where {0} and [STATUS]='IN' order by [UID]", date);

                TowerData = SearchData(AMMDBConnectionString, sql);
                
                TowerData.Columns.Add();
                TowerData.Columns[5].ReadOnly = false;

                //dgv_tower.DataSource = TowerData;

                dgv_tower.BeginInvoke(new Action(() => 
                {
                    dgv_tower.DataSource = TowerData;

                    dgv_tower.Columns[5].Visible = false;
                    dgv_tower.Columns[1].Width = 70;
                    dgv_tower.Columns[2].Width = 30;
                    dgv_tower.Columns[3].Width = 65;
                }));
                


                bTowerThread = false;
            }
            catch (Exception ex)
            {
                
            }
        }

        DataTable FailData;

        private void bgwSorter_DoWork(object sender, DoWorkEventArgs e)
        {
            // Thread.Sleep(3000);

            while (true)
            {                
                try
                {
                    if (bSorterThread == false && bTowerThread == false)
                    {
                        dgv_fail.BeginInvoke(new Action(()=> { dgv_fail.DataSource = null; }));

                        FailData = SorterData.Clone();
                        for(int i = 0; i < SorterData.Rows.Count; i++)
                        {
                            DataRow srow = SorterData.Rows[i];

                            for (int j = 0; j < TowerData.Rows.Count; j++)
                            {
                                DataRow trow = TowerData.Rows[j];

                                if (srow[1].ToString() == trow[0].ToString() &&     // UID 검사
                                    srow[2].ToString() == trow[1].ToString() &&     // QTY 검사
                                    srow[3].ToString() == trow[2].ToString())       // SIZE 검사
                                {
                                    if (srow[4].ToString().Substring(2, (srow[4].ToString().Length - 2))
                                        == trow[3].ToString().Substring(3, (trow[3].ToString().Length - 3)))    // Tower 입고 위치 검사
                                    {
                                        //srow.ReadOnly = false;
                                        dgv_sorter.Rows[i].DefaultCellStyle.BackColor = Color.Blue;
                                        dgv_tower.Rows[j].DefaultCellStyle.BackColor = Color.Blue;

                                        SorterData.Rows[i][SorterData.Columns.Count - 1] = SorterCompState_Complete;
                                        dgv_tower.Rows[j].Cells[5].Value = SorterCompState_Complete;
                                                                                
                                        break;
                                    }
                                    else
                                    {
                                        //srow.ReadOnly = false;
                                        dgv_sorter.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                                        dgv_tower.Rows[j].DefaultCellStyle.BackColor = Color.Yellow;

                                        SorterData.Rows[i][SorterData.Columns.Count - 1] = SorterCompState_InMiss;
                                        dgv_tower.Rows[j].Cells[5].Value = SorterCompState_InMiss;
                                        break;
                                    }
                                }
                                
                            }

                            if (SorterData.Rows[i][SorterData.Columns.Count-1].ToString() == "")
                            {
                                //srow.ReadOnly = false;
                                dgv_sorter.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                SorterData.Rows[i][SorterData.Columns.Count - 1] = SorterCompState_Fail;

                                FailData.ImportRow(SorterData.Rows[i]);
                                //dgv_fail.BeginInvoke(new Action(() => {
                                //    dgv_fail.Rows.Add(srow.Cells[0].Value.ToString(), srow.Cells[1].Value.ToString(), srow.Cells[2].Value.ToString(), srow.Cells[3].Value.ToString(), srow.Cells[4].Value.ToString(), srow.Cells[5].Value.ToString());
                                //}));
                                //dgv_fail.Rows.Add(srow.Cells[0].Value.ToString(), srow.Cells[1].Value.ToString(), srow.Cells[2].Value.ToString(), srow.Cells[3].Value.ToString(), srow.Cells[4].Value.ToString(), srow.Cells[5].Value.ToString());
                            }
                            
                        }


                        //FailData.Columns["RID"].ColumnName = "UID";
                        dgv_fail.BeginInvoke(new Action(() => { dgv_fail.DataSource = FailData; }));
                        MessageBox.Show("검사가 완료되었습니다.");
                        return;
                    }

                    Thread.Sleep(500);
                }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    
            }
        }

        private void InitGrid()
        {
            dgv_sorter.Rows.Clear();
            dgv_tower.Rows.Clear();
        }

        private void RunSqlCMD(string ConnectionString, string sql)
        {
            int res = -1;
            try
            {
                using (SqlConnection c = new SqlConnection(ConnectionString))
                {
                    c.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, c))
                    {
                        

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;

                        res = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        private DataTable SearchData(string ConnectionString, string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection c = new SqlConnection(ConnectionString))
                {
                    c.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, c))
                    {
                        using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return dt;
        }

        int nBSearchUID = -1;

        private void dgv_sorter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DatagridClickRowIndex = e.RowIndex;
            if (e.ColumnIndex == 1 && e.RowIndex != -1)
            {
                string UIDVal = dgv_sorter.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                if(nBSearchUID != -1)
                    dgv_tower.Rows[nBSearchUID].Selected = false;

                nBSearchUID = -1;
                for(int i = 0; i< dgv_tower.RowCount; i++)
                {
                    if (dgv_tower.Rows[i].Cells[0].Value.ToString() == UIDVal)
                    {
                        dgv_tower.Rows[i].Selected = true;
                        nBSearchUID = i;

                        dgv_tower.DefaultCellStyle.SelectionBackColor = Color.Green;
                        dgv_tower.FirstDisplayedScrollingRowIndex = i;
                        dgv_tower.CurrentCell = dgv_tower.Rows[i].Cells[0];
                        
                        break;
                    }
                }

                if(nBSearchUID == -1)
                {
                    MessageBox.Show("UID가 없습니다.");
                }
            }
        }

        int DatagridClickRowIndex = -1;

        private void dgv_fail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dgv_fail_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == (Keys.Control | Keys.C))
                //Clipboard.SetText(dgv_fail.Rows[FailClickRowIndex].Cells["UID"].Value.ToString());
        }


        private void dgv_fail_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void dgv_tower_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DatagridClickRowIndex = e.RowIndex;
        }

        private void dgv_fail_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.ColumnIndex == 1 && e.RowIndex != -1)
            {

                string UIDVal = dgv_fail.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                if (nBSearchUID != -1)
                    dgv_tower.Rows[nBSearchUID].Selected = false;

                nBSearchUID = -1;
                for (int i = 0; i < dgv_tower.RowCount; i++)
                {
                    if (dgv_tower.Rows[i].Cells[0].Value.ToString() == UIDVal)
                    {
                        dgv_tower.Rows[i].Selected = true;
                        nBSearchUID = i;

                        dgv_tower.DefaultCellStyle.SelectionBackColor = Color.Green;
                        dgv_tower.FirstDisplayedScrollingRowIndex = i;
                        break;
                    }
                }

                if (nBSearchUID == -1)
                {
                    MessageBox.Show("UID가 없습니다.");
                }
            }
        }

        private void SDTSort_MouseDown(object sender, MouseEventArgs e)
        {
         
        }

        private void SDTSort_ValueChanged(object sender, EventArgs e)
        {
            GetTotalSEQ();
        }

        private void GetTotalSEQ()
        {
            string date = "";
            string res = "";

            if (SDTSort.Value <= EDTSort.Value)
            {
                if (SDTSort.Value == EDTSort.Value)
                {
                    date = string.Format("[DATE] = '{0}'", SDTSort.Value.Date.ToString("yyyyMMdd"));
                }
                else
                {
                    date = string.Format("[DATE] >= '{0}' and [DATE] <= '{1}'", SDTSort.Value.Date.ToString("yyyyMMdd"), EDTSort.Value.Date.ToString("yyyyMMdd"));
                }

                string sql = string.Format("select [Seq] from vReelSorterResult with(Nolock) where {0} group by [Seq]", date);

                SorterData = SearchData(SORTERDBConnectionString, sql);
                
                for(int i = 0; i< SorterData.Rows.Count; i++)
                {
                    res += SorterData.Rows[i][0].ToString() + ",";
                }

                if (res.Contains("1") == true)
                {
                    ch_seq1.Visible = true;
                    ch_seq1.Checked = true;
                }
                else
                {
                    ch_seq1.Visible = false;
                    ch_seq1.Checked = false;
                }

                if (res.Contains("2") == true)
                {
                    ch_seq2.Visible = true;
                    ch_seq2.Checked = true;
                }
                else
                {
                    ch_seq2.Visible = false;
                    ch_seq2.Checked = false;
                }

                if (res.Contains("3") == true)
                {
                    ch_seq3.Visible = true;
                    ch_seq3.Checked = true;
                }
                else
                {
                    ch_seq3.Visible = false;
                    ch_seq3.Checked = false;
                }

                if (res.Contains("4") == true)
                {
                    ch_seq4.Visible = true;
                    ch_seq4.Checked = true;
                }
                else
                {
                    ch_seq4.Visible = false;
                    ch_seq4.Checked = false;
                }
            }            
        }

        private void EDTSort_ValueChanged(object sender, EventArgs e)
        {
            GetTotalSEQ();
        }

        bool isClick = false; 

        private void comboBox_type_MouseClick(object sender, MouseEventArgs e)
        {
            isClick = true;
        }

        private void comboBox_type_MouseDown(object sender, MouseEventArgs e)
        {
            isClick = true;
        }

        private void comboBox_group_MouseDown(object sender, MouseEventArgs e)
        {
            isClick = true;
        }

        DataConn MDBConn = null;
        string MDBPath = "";

        private void getMDBConn()
        {
            if (MDBConn == null)
            {
                MDBConn = new DataConn();

                DataTable dt = new DataTable();
                using (SqlConnection c = new SqlConnection("server=10.135.200.35;database=ATK4-AMM-DBv1;user id=amm;password=amm@123"))
                {
                    c.Open();

                    using (SqlCommand cmd = new SqlCommand($"SELECT [NAME],[VALUE],[TYPE] from [PUBLIC_SETTINGS] with(nolock) where NAME='TOWER{comboBox_sel.SelectedIndex + 1}_PATH'", c))
                    {
                        using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                }
                MDBPath = @"C:\SMDTowerSQL\BackupDB.MDB";//dt.Rows[0][1].ToString();
            }

            if (Tower_serial.Count == 0)
                GetTowerSerial();
        }

        private void btn_MakeOutList_Click(object sender, EventArgs e)
        {
            try
            {
                Synclog.Info("MakeOutList button Click");

                if(nDbUpdate == 0)
                {
                    Synclog.Info("DB 조회가 되지 않았습니다. DB 조회를 먼저 진행 하십시오.");
                    MessageBox.Show("DB 조회가 되지 않았습니다. DB 조회를 먼저 진행 하십시오.");
                    return;
                }
                if(nDbUpdate == 1)
                {
                    Synclog.Info("Database 비교를 진행 하지 않았습니다.\n비교 진행 후 진행 바랍니다.");
                    MessageBox.Show("Database 비교를 진행 하지 않았습니다.\n비교 진행 후 진행 바랍니다.");
                    return;
                }


                nDbUpdate = 0;

                string temp = "";
                List<DataGridViewRow> ASMRows = new List<DataGridViewRow>();
                int nGroup = int.Parse(comboBox_sel.Text.Substring(1, 1));
                string strGroup = "TWR" + nGroup;
                string ID = "";
                   
                bool isUse = false;
                                
                DataTable dt_Status = AMM_Main.AMM.GetStatus(AMM_Main.strDefault_linecode, strGroup);                
                
                string strStatus = dt_Status.Rows[0]["TYPE"].ToString(); strStatus = strStatus.Trim();

                
                if (!(strStatus == "READY" || strStatus == ""))
                {
                    isUse = true;
                }

                if (isUse == true)
                {
                    Synclog.Info("Ready 상태일 때만 동기화 가능 합니다. 잠시 후 다시 시도 하세요.");
                    string str = string.Format("Ready 상태일 때만 동기화 가능 합니다. \n잠시 후 다시 시도 하세요.");

                    Form_Progress fp = new Form_Progress();
                    fp.Form_Show(str, 1000);

                    while (fp.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                    return;
                }

                if (DialogResult.OK == InputBox("사번입력", "사번 입력 ", ref ID))
                {   
                    string strName = AMM_Main.AMM.User_check(ID);
                    strName = strName.Trim();
                

                    if (strName == "NO_INFO")
                    {
                        Synclog.Info(string.Format("등록되지 않은 사용자 : {0}", ID));
                        string str = string.Format("등록 되지 않은 사용자 입니다.\n등록 후 사용 하세요.", 1000);

                        Form_Progress fp = new Form_Progress();
                        fp.Form_Show(str, 1000);

                        while (fp.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                        return;
                    }

                    Synclog.Info(string.Format("등록된 사용자 : {0}", ID));

                    //if(cb_SyncExcel.Checked == true)
                    //{
                    //    SyncListExcelOut();                        
                    //}
                    //else
                    //{
                    //    SyncListCSVOut();
                    //}
                    getMDBConn();
                    string q = "";
                    foreach (DataGridViewRow row in dataGridView_missmatch.Rows)
                    {
                        q = $"insert into [TB_SYNC_INFO] ([DATETIME], [EQUIP_ID], [TOWER_NO], [UID], [SID], [LOTID], [QTY], [INCH_INFO], [SYNC_INFO], [EMPLOYEE_NO])" +
                            $"VALUES (GETDATE(), '{strGroup}', '{row.Cells["위치"].Value.ToString()}', '{row.Cells["UID"].Value.ToString()}', '{row.Cells["SID"].Value.ToString()}'," +
                            $"'{row.Cells["LOTID"].Value.ToString()}', {(row.Cells["Qty"].Value.ToString() == "" ? "0" : row.Cells["Qty"].Value.ToString().Replace(",", ""))}, '{row.Cells["인치"].Value.ToString()}', " +
                            $"{(row.Cells["위치"].Value.ToString().Contains("T0") == true ? "'배출명령생성'" : "'위치정보 삭제'") },'{ID}')";
                        RunSqlCMD(AMMDBConnectionString, q);
                    }

                    if (nGroup <= 3)
                    {
                        if (dataGridView_missmatch.RowCount > 0)
                        {
                            Fnc_Get_PickID(strGroup);

                            List<DataGridViewRow> willMakeReel = dataGridView_missmatch.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["miss"].Value.ToString() == "TOWER").ToList();

                            /*
                                0 dataGridView_missmatch.Columns.Add("NO", "NO");
                                1 dataGridView_missmatch.Columns.Add("SID", "SID");
                                2 dataGridView_missmatch.Columns.Add("LOTID", "LOTID#");
                                3 dataGridView_missmatch.Columns.Add("UID", "UID");
                                4 dataGridView_missmatch.Columns.Add("Qty", "Qty");
                                5 dataGridView_missmatch.Columns.Add("투입형태", "투입형태");
                                6 dataGridView_missmatch.Columns.Add("위치", "위치");
                                7 dataGridView_missmatch.Columns.Add("제조일", "제조일");
                                8 dataGridView_missmatch.Columns.Add("투입일", "투입일");
                                9 dataGridView_missmatch.Columns.Add("제조사", "제조사");
                                10 dataGridView_missmatch.Columns.Add("인치", "인치");
                                11 dataGridView_missmatch.Columns.Add("MISS", "MISS");           
                             */


                            foreach (DataGridViewRow row in willMakeReel)
                            {
                                //row.Cells["UID"].Value = row.Cells["UID"].Value.ToString() == "" ? GetRandomString(10) : row.Cells["UID"].Value.ToString();
                                //row.Cells["SID"].Value = row.Cells["SID"].Value.ToString() == "" ? GetRandomString(10) : row.Cells["SID"].Value.ToString();

                                //row.Cells["LOTID"].Value = GetRandomString(5);
                                row.Cells["Qty"].Value = 1;
                                row.Cells["투입형태"].Value = "SYNC";
                                row.Cells["제조일"].Value = DateTime.Now.ToString("yyyyMMdd");
                                row.Cells["투입일"].Value = DateTime.Now.ToString("yyyyMMdd");
                                row.Cells["제조사"].Value = "SYNC";
                                row.Cells["인치"].Value = "Unknown";

                                if (row.Cells["MISS"].Value.ToString() != "AMM" && row.Cells["UID"].Value.ToString() != "")
                                {
                                    AMM_Main.AMM.SyncDataInsert(
                                    $"{nGroup}",
                                    row.Cells["위치"].Value.ToString(),
                                    row.Cells["UID"].Value.ToString(),
                                    row.Cells["SID"].Value.ToString(),
                                    row.Cells["LOTID"].Value.ToString(),
                                    row.Cells["Qty"].Value.ToString(),
                                    row.Cells["제조사"].Value.ToString(),
                                    row.Cells["제조일"].Value.ToString(),
                                    row.Cells["인치"].Value.ToString(),
                                    row.Cells["투입형태"].Value.ToString()
                                    );
                                }
                            }

                            bool ListSend = false;

                            for (int i = 0; i < dataGridView_missmatch.RowCount; i++)
                            {
                                //if (dataGridView_missmatch.Rows[i].DefaultCellStyle.ForeColor == Color.Blue)
                                {
                                    //string[] MissReelInfo = dataGridView_missmatch.Rows[i].Cells[0].Value.ToString().Split(';');
                                    //ASMRows.Add(dataGridView_missmatch.Rows[i]);

                                    if ((dataGridView_missmatch.Rows[i].Cells["MISS"].Value.ToString() == "TOWER" && dataGridView_missmatch.Rows[i].Cells["UID"].Value.ToString() != ""))
                                    {
                                        temp = AMM_Main.AMM.SetPicking_Readyinfo(
                                            AMM_Main.strDefault_linecode,
                                            strGroup,
                                            label_pickid_LT.Text,
                                            dataGridView_missmatch.Rows[i].Cells[3].Value.ToString(),
                                            ID,
                                            dataGridView_missmatch.Rows[i].Cells[6].Value.ToString(),
                                            dataGridView_missmatch.Rows[i].Cells[1].Value.ToString(),
                                            dataGridView_missmatch.Rows[i].Cells[2].Value.ToString(),
                                            dataGridView_missmatch.Rows[i].Cells[4].Value.ToString(),
                                            dataGridView_missmatch.Rows[i].Cells[9].Value.ToString(),
                                            dataGridView_missmatch.Rows[i].Cells[7].Value.ToString(),
                                            dataGridView_missmatch.Rows[i].Cells[10].Value.ToString(),
                                            dataGridView_missmatch.Rows[i].Cells[5].Value.ToString(),
                                            "SYNC");

                                        AMM_Main.AMM.GetPickingListinfo(dataGridView_missmatch.Rows[i].Cells[3].Value.ToString());

                                        ListSend = true;
                                    }
                                    
                                }
                            }

                            if (label_pickid_LT.Text != "" &&  ListSend == true) 
                            {
                                Fnc_Picklist_Comfirm();
                                //Fnc_Save_TowerUseInfo();

                                label_count.Text = dataGridView_missmatch.RowCount.ToString();
                                textBox_badge.Text = ID;

                                Fnc_Picklist_Send(AMM_Main.strDefault_linecode, strGroup, label_pickid_LT.Text);
                            }

                            if (ListSend == true)
                            {
                                Form_Progress frm = new Form_Progress();
                                frm.Form_Show("배출 명령이 생성 되었습니다.", 0);
                            }
                            else
                            {
                                Form_Progress frm = new Form_Progress();
                                frm.Form_Show("Sync 로그 기록 되어 있습니다.", 0);
                            }
                        }
                        else
                        {
                            Form_Progress frm = new Form_Progress();
                            frm.Form_Show("목록이 비어 있습니다.", 0);
                        }

                        dataGridView_missmatch.Rows.Clear();
                    }
                    else    // Tower Group #3 이상
                    {
                        int update = 0;
                        int ok = 0;
                        int fail = 0;

                        if(dataGridView_missmatch.RowCount > 0)
                        {
                            

                            bool LostTower = false;
                            
                            string res = "";

                            int loop = dataGridView_missmatch.RowCount;
                            int AMMLostCnt = 0;

                            Fnc_Get_PickID(strGroup);

                            bool tower = false;
                            int MissRows = dataGridView_missmatch.RowCount;

                            Debug.WriteLine($"total miss = {dataGridView_missmatch.RowCount}");

                            for(int i = 0; i < MissRows; i++)
                            {
                                //Thread.Sleep(100);
                                Debug.WriteLine($"cnt = {i}");
                                
                                try
                                {
                                    if (dataGridView_missmatch.Rows[0].Cells["MISS"].Value.ToString() == "AMM")
                                    {
                                        tower = true;

                                        if (dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString().Contains("T0") == true)
                                        {
                                            //res = AMM_Main.AMM.SetLoadComplete(AMM_Main.strDefault_linecode, strGroup, string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                                            //dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["SID"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["LOTID"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["Qty"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["제조사"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["제조일"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["인치"].Value.ToString(),
                                            //"SYNC"
                                            //), true
                                            //);

                                            AMM_Main.AMM.SyncDataInsert(
                                                strGroup,
                                                dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["SID"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["LOTID"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["Qty"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["제조사"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["제조일"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["인치"].Value.ToString(),
                                                "SYNC"
                                                );

                                            BackUpInsert(dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString());

                                            AMM_Main.AMM.SetPicking_Readyinfo(AMM_Main.strDefault_linecode, strGroup, label_pickid_LT.Text,
                                                dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(),
                                                ID,
                                                dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["SID"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["LOTID"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["Qty"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["제조사"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["제조일"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["인치"].Value.ToString(),
                                                "SYNC",
                                                "SYNC");

                                            AMMLostCnt++;
                                            dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                            LostTower = true;
                                        }
                                        else
                                        {
                                            Synclog.Info(string.Format("{0} Databse Delete UID : {1}", strGroup, dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString()));

                                            DeleteMycronicTower(nGroup, dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString());

                                            dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                        }
                                    }
                                    else if(dataGridView_missmatch.Rows[0].Cells["MISS"].Value.ToString() == "Backup")
                                    {
                                        if (dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString().Contains("T0") == false)
                                        {
                                            Synclog.Info(string.Format("{0} Databse Delete UID : {1}", strGroup, dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString()));

                                            MDBConn.DeleteData(dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(), MDBPath);
                                            DeleteMycronicTower(nGroup, dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString());

                                            dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                        }
                                        else
                                        {
                                         


                                            int cnt = MDBConn.TouchRow($"select * from Carrier where Carrier='{dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString()}'", MDBPath);

                                            if (cnt == 0)
                                            {//insert
                                                BackUpInsert1(dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString());
                                            }
                                            else
                                            {//update
                                                BackupUpdate(dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString());
                                            }

                                            AMM_Main.AMM.SetPicking_Readyinfo(AMM_Main.strDefault_linecode, strGroup, label_pickid_LT.Text,
                                                   dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(),
                                                   ID,
                                                   dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString(),
                                                   dataGridView_missmatch.Rows[0].Cells["SID"].Value.ToString(),
                                                   dataGridView_missmatch.Rows[0].Cells["LOTID"].Value.ToString(),
                                                   dataGridView_missmatch.Rows[0].Cells["Qty"].Value.ToString(),
                                                   dataGridView_missmatch.Rows[0].Cells["제조사"].Value.ToString(),
                                                   dataGridView_missmatch.Rows[0].Cells["제조일"].Value.ToString(),
                                                   dataGridView_missmatch.Rows[0].Cells["인치"].Value.ToString(),
                                                   "SYNC",
                                                   "SYNC");

                                            AMMLostCnt++;
                                            dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                            LostTower = true;
                                        }
                                        
                                    }
                                    else if(dataGridView_missmatch.Rows[0].Cells["MISS"].Value.ToString() == "Local_DB")
                                    {
                                        //if (dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString().Contains("T0") == true)
                                        //{
                                        //    dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                        //}
                                        //else
                                        {
                                            Synclog.Info(string.Format("{0} Databse Delete UID : {1}", strGroup, dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString()));

                                            MDBConn.DeleteData(dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(), MDBPath);

                                            dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                        }
                                    }
                                    else if(dataGridView_missmatch.Rows[0].Cells["MISS"].Value.ToString() == "TOWER")
                                    {
                                        // 타워에서 정상적로 입고 처리가 안된 것
                                        tower = true;

                                        if (dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString().Contains("T0") == true)
                                        {
                                            //res = AMM_Main.AMM.SetLoadComplete(AMM_Main.strDefault_linecode, strGroup, string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                                            //dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["SID"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["LOTID"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["Qty"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["제조사"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["제조일"].Value.ToString(),
                                            //dataGridView_missmatch.Rows[0].Cells["인치"].Value.ToString(),
                                            //"SYNC"
                                            //), true
                                            //);

                                         

                                            //BackUpInsert(dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString());

                                            AMM_Main.AMM.SetPicking_Readyinfo(AMM_Main.strDefault_linecode, strGroup, label_pickid_LT.Text,
                                                dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString(),
                                                ID,
                                                dataGridView_missmatch.Rows[0].Cells["위치"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["SID"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["LOTID"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["Qty"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["제조사"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["제조일"].Value.ToString(),
                                                dataGridView_missmatch.Rows[0].Cells["인치"].Value.ToString(),
                                                "SYNC",
                                                "SYNC");

                                            AMMLostCnt++;
                                            dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                            LostTower = true;
                                        }
                                        else
                                        {
                                            Synclog.Info(string.Format("{0} Databse Delete UID : {1}", strGroup, dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString()));

                                            //DeleteMycronicTower(nGroup, dataGridView_missmatch.Rows[0].Cells["UID"].Value.ToString());

                                            dataGridView_missmatch.Rows.Remove(dataGridView_missmatch.Rows[0]);
                                        }
                                    }
                                    else
                                    {
                                        //if (tower == true)
                                        //{
                                        //    dataGridView_asm.Rows.Clear();
                                        //    int n = comboBox_sel.SelectedIndex;

                                        //    if (n == 0)
                                        //    {
                                        //        Fnc_Process_GetMaterials_Tower1();
                                        //        Fnc_Process_GetAMMinfo("TWR1");
                                        //    }
                                        //    else if (n == 1)
                                        //    {
                                        //        Fnc_Process_GetMaterials_Tower2();
                                        //        Fnc_Process_GetAMMinfo("TWR2");
                                        //    }
                                        //    else if (n == 2)
                                        //    {
                                        //        Fnc_Process_GetMaterials_Tower3();
                                        //        Fnc_Process_GetAMMinfo("TWR3");
                                        //    }
                                        //    else
                                        //    {
                                        //        GetMycronicTower(n + 1);
                                        //        Fnc_Process_GetAMMinfo("TWR" + (n + 1).ToString());
                                        //    }

                                        //    tower = false;
                                        //}

                                        //bool UID = false;

                                        //foreach (DataGridViewRow row in dataGridView_asm.Rows)
                                        //{
                                        //    if (row.Cells["UID"].Value.ToString() == mrow.Cells["UID"].Value.ToString())
                                        //    {
                                        //        UID = true;
                                        //        break;
                                        //    }
                                        //}

                                        //if (UID == false)
                                        //{
                                        //    LostTower = true;

                                        //    RunSqlCMD(AMMDBConnectionString, $"update [TB_MTL_INFO] set [INPUT_TYPE]='SYNC' where UID='{mrow.Cells["UID"].Value.ToString()}'");
                                        //    Thread.Sleep(10);

                                        //    res = AMM_Main.AMM.SetPicking_Readyinfo(
                                        //        AMM_Main.strDefault_linecode,
                                        //        strGroup, label_pickid_LT.Text,
                                        //        mrow.Cells[3].Value.ToString(),
                                        //        ID,
                                        //        mrow.Cells[6].Value.ToString(),
                                        //        mrow.Cells[1].Value.ToString(),
                                        //        mrow.Cells[2].Value.ToString(),
                                        //        mrow.Cells[4].Value.ToString().Replace(",", ""),
                                        //        mrow.Cells[9].Value.ToString(),
                                        //        mrow.Cells[7].Value.ToString(),
                                        //        mrow.Cells[10].Value.ToString(),
                                        //        mrow.Cells[5].Value.ToString(), "SYNC");
                                        //    //AMM_Main.AMM.GetPickingListinfo(dataGridView_missmatch.Rows[0].Cells[3].Value.ToString());

                                        //    AMMLostCnt++;
                                        //}
                                    }

                                    //if (res == "OK")
                                    //    dataGridView_missmatch.Rows.RemoveAt(0);

                                    
                                    dataGridView_missmatch.Update();
                                    Application.DoEvents();

                                }
                                catch (Exception ex)
                                {

                                }
                            }

                            

                            if (LostTower == true)
                            {
                                if (label_pickid_LT.Text != "")
                                {
                                    Fnc_Picklist_Comfirm();
                                    //Fnc_Save_TowerUseInfo();

                                    label_count.Text = AMMLostCnt.ToString();
                                    textBox_badge.Text = ID;

                                    Fnc_PicklistSync_Send(AMM_Main.strDefault_linecode, strGroup, label_pickid_LT.Text);
                                }

                                Form_Progress frm = new Form_Progress();
                                frm.Form_Show("배출 명령이 생성 되었습니다.", 0);
                            }
                            else
                            {
                                Synclog.Info(string.Format("{0} Database에서 삭제가 완료 되었습니다.", strGroup));

                                Form_Progress frm = new Form_Progress();
                                //frm.Form_Show($"Insert : {ok}, Update : {update}, Fail : {fail}", 0);
                                frm.Form_Show(string.Format("{0} Database에서 삭제가 완료 되었습니다.", strGroup),0);
                            }
                        }
                        else
                        {
                            Synclog.Info("dataGridView_missmatch.rowscount <= 0 ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
        }

        private void BackupUpdate(string UID)
        {
            getMDBConn();

            DataSet LocalData = GetMycronicData(comboBox_sel.SelectedIndex + 1, $"select * from TCarrier with(nolock) where Carrier='{UID}'");

            int cnt = MDBConn.TouchRow($"select * from Carrier where Carrier='{UID}'", MDBPath);

            if (cnt == 1)//Backup DB에 있으면 update
            {
                string q1 = "Update Carrier set";              
                string val = " values(";

                for (int i = 0; i < LocalData.Tables[0].Columns.Count; i++)
                {                    
                    if (LocalData.Tables[0].Columns[i].ColumnName == "Depot")
                    {
                        val += $"{LocalData.Tables[0].Columns[i].ColumnName}=";
                        val += $"'Tower {Tower_serial[LocalData.Tables[0].Rows[0][i].ToString().Split('.')[0]]}, " +
                            $"Magazine {Dec2Alpa[int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(0, 1))]}{int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(1, 1))}, " +
                            $"Slot {LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(2, 2)}',";
                    }
                    else if (LocalData.Tables[0].Columns[i].ColumnName.Contains("Date") == true)
                    {
                        val += $"{LocalData.Tables[0].Columns[i].ColumnName}=";
                        val += $"{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("yyyy-MM-dd")},";

                        val += $"{LocalData.Tables[0].Columns[i].ColumnName.Replace("Date", "") + "Time,"}=";
                        val += $"'{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("hh:mm")}',";
                    }
                    else if (LocalData.Tables[0].Columns[i].ColumnName == "Article" || LocalData.Tables[0].Columns[i].ColumnName == "Stock" || LocalData.Tables[0].Columns[i].ColumnName == "StockMin" ||
                        LocalData.Tables[0].Columns[i].ColumnName == "StockNew" || LocalData.Tables[0].Columns[i].ColumnName == "StockUsed" || LocalData.Tables[0].Columns[i].ColumnName == "StockTPSys" ||
                        LocalData.Tables[0].Columns[i].ColumnName == "OutTime" || LocalData.Tables[0].Columns[i].ColumnName == "Duration" || LocalData.Tables[0].Columns[i].ColumnName == "Frequency" ||
                        LocalData.Tables[0].Columns[i].ColumnName == "Amplitude" || LocalData.Tables[0].Columns[i].ColumnName == "Core" || LocalData.Tables[0].Columns[i].ColumnName == "TapeHeight" ||
                        LocalData.Tables[0].Columns[i].ColumnName == "Guessed" || LocalData.Tables[0].Columns[i].ColumnName == "Unleaded" || LocalData.Tables[0].Columns[i].ColumnName == "PriceP" ||
                        LocalData.Tables[0].Columns[i].ColumnName == "Cycles" || LocalData.Tables[0].Columns[i].ColumnName == "Height" || LocalData.Tables[0].Columns[i].ColumnName == "HCode" ||
                        LocalData.Tables[0].Columns[i].ColumnName == "Diameter" || LocalData.Tables[0].Columns[i].ColumnName == "StepWidth" || LocalData.Tables[0].Columns[i].ColumnName == "MSLWatch"
                        )
                    {
                        val += $"{LocalData.Tables[0].Columns[i].ColumnName}=";
                        val += $"{LocalData.Tables[0].Rows[0][i].ToString()},";
                    }
                    else if (LocalData.Tables[0].Columns[i].ColumnName == "ID" || LocalData.Tables[0].Columns[i].ColumnName == "ArticleName" || LocalData.Tables[0].Columns[i].ColumnName == "Enabled" || LocalData.Tables[0].Columns[i].ColumnName == "InitialOuttime")
                    {

                    }
                    else if (LocalData.Tables[0].Columns[i].ColumnName == "Expiry")
                    {
                        val += $"{LocalData.Tables[0].Columns[i].ColumnName}=";
                        val += $"1900-01-01,";
                    }
                    else
                    {
                        val += $"{LocalData.Tables[0].Columns[i].ColumnName}=";
                        val += $"'{LocalData.Tables[0].Rows[0][i].ToString().Trim()}',";
                    }
                }
                
                val = val.Substring(0, val.LastIndexOf(',')) + ")";

                q1 += val + $" where Carrier='{UID}'";

                MDBConn.RunQuary(
                // $"Magazine {Dec2Alpa[int.Parse(LocalData.Tables[0].Rows[0][7].ToString().Split('.')[1].Substring(0, 1))]}{int.Parse(LocalData.Tables[0].Rows[0][7].ToString().Split('.')[1].Substring(1, 1))}, " +
                //$"Slot {LocalData.Tables[0].Rows[0][7].ToString().Split('.')[1].Substring(2, 2)}'," +
                q1
                ,
                MDBPath);
            }
        }

        private void BackUpInsert(string UID)
        {
            getMDBConn();
            
            DataSet LocalData = GetMycronicData(comboBox_sel.SelectedIndex + 1, $"select * from TCarrier with(nolock) where Carrier='{UID}'");
                        
            string q1 = "INSERT INTO Carrier";
            string col = "(";
            string val = " values(";

            if (LocalData.Tables.Count == 1)
            {

            }
            else
            {
                for (int i = 0; i < LocalData.Tables[0].Columns.Count; i++)
                {
                    if (LocalData.Tables[0].Columns[i].ColumnName.Contains("Date") == true)
                    {
                        col += $"{LocalData.Tables[0].Columns[i].ColumnName},{LocalData.Tables[0].Columns[i].ColumnName.Replace("Date", "") + "Time,"}";
                    }
                    else if (LocalData.Tables[0].Columns[i].ColumnName == "ID" || LocalData.Tables[0].Columns[i].ColumnName == "ArticleName" || LocalData.Tables[0].Columns[i].ColumnName == "Enabled" || LocalData.Tables[0].Columns[i].ColumnName == "InitialOuttime")
                    {
                    }
                    else
                    {
                        col += $"{LocalData.Tables[0].Columns[i].ColumnName},";
                    }


                    switch (LocalData.Tables[0].Columns[i].ColumnName)
                    {
                        case "Depot":
                            val += $"'Tower {Tower_serial[LocalData.Tables[0].Rows[0][i].ToString().Split('.')[0]]}, " +
                            $"Magazine {Dec2Alpa[int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(0, 1))]}{int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(1, 1))}, " +
                            $"Slot {LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(2, 2)}',";
                            break;
                        case var _ when LocalData.Tables[0].Columns[i].ColumnName.Contains("Date") == true:
                            val += $"{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("yyyy-MM-dd")},";
                            val += $"'{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("hh:mm")}',";
                            break;
                        case "Expiry":
                            val += $"1900-01-01,";
                            break;
                        case "InitialOuttime":
                        case "Enabled":
                        case "ArticleName":
                        case "ID":
                            break;
                        case "Guessed":
                        case "Article":
                        case "Cycles":
                        case "Outtime":
                        case "Diameter":
                        case "Amplitude":
                        case "Stock":
                        case "StockNew":
                        case "StockUsed":
                        case "Duration":
                        case "Core":
                        case "Unleaded":
                        case "Height":
                        case "StepWidth":
                        case "StockMin":
                        case "StockTPSys":
                        case "Frequency":
                        case "TapeHeight":
                        case "PriceP":
                        case "HCode":                            
                        case "MSLWatch":
                            val += $"{LocalData.Tables[0].Rows[0][i].ToString()},";
                            break;
                        default:
                            val += $"'{LocalData.Tables[0].Rows[0][i].ToString().Trim()}',";
                            break;
                    }

                    #region column if else
                    //if (LocalData.Tables[0].Columns[i].ColumnName == "Depot")
                    //{
                    //    val += $"'Tower {Tower_serial[LocalData.Tables[0].Rows[0][i].ToString().Split('.')[0]]}, " +
                    //        $"Magazine {Dec2Alpa[int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(0, 1))]}{int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(1, 1))}, " +
                    //        $"Slot {LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(2, 2)}',";
                    //}
                    //else if(LocalData.Tables[0].Columns[i].ColumnName.Contains("Date") == true)
                    //{
                    //    val += $"{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("yyyy-MM-dd")},";
                    //    val += $"'{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("hh:mm")}',";
                    //}
                    //else if (LocalData.Tables[0].Columns[i].ColumnName == "Article" || LocalData.Tables[0].Columns[i].ColumnName == "Stock" || LocalData.Tables[0].Columns[i].ColumnName == "StockMin" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "StockNew" || LocalData.Tables[0].Columns[i].ColumnName == "StockUsed" || LocalData.Tables[0].Columns[i].ColumnName == "StockTPSys" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "OutTime" || LocalData.Tables[0].Columns[i].ColumnName == "Duration" || LocalData.Tables[0].Columns[i].ColumnName == "Frequency" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Amplitude" || LocalData.Tables[0].Columns[i].ColumnName == "Core" || LocalData.Tables[0].Columns[i].ColumnName == "TapeHeight" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Guessed" || LocalData.Tables[0].Columns[i].ColumnName == "Unleaded" || LocalData.Tables[0].Columns[i].ColumnName == "PriceP" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Cycles" || LocalData.Tables[0].Columns[i].ColumnName == "Height" || LocalData.Tables[0].Columns[i].ColumnName == "HCode"||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Diameter" || LocalData.Tables[0].Columns[i].ColumnName == "StepWidth" || LocalData.Tables[0].Columns[i].ColumnName == "MSLWatch" 
                    //    )
                    //{
                    //    val += $"{LocalData.Tables[0].Rows[0][i].ToString()},";
                    //}
                    //else if(LocalData.Tables[0].Columns[i].ColumnName == "ID" || LocalData.Tables[0].Columns[i].ColumnName == "ArticleName" || LocalData.Tables[0].Columns[i].ColumnName == "Enabled" || LocalData.Tables[0].Columns[i].ColumnName == "InitialOuttime")
                    //{

                    //}
                    //else if(LocalData.Tables[0].Columns[i].ColumnName == "Expiry")
                    //{
                    //    val += $"1900-01-01,";
                    //}
                    //else
                    //{
                    //    val += $"'{LocalData.Tables[0].Rows[0][i].ToString().Trim()}',";
                    //}
                    #endregion
                }

                col = col.Substring(0, col.LastIndexOf(',')) + ")";
                val = val.Substring(0, val.LastIndexOf(',')) + ")";

                q1 += col + val;

                MDBConn.RunQuary(q1, MDBPath);
            }
            
            
        }

        private void BackUpInsert1(string UID)
        {
            getMDBConn();

            DataSet LocalData = GetMycronicData(comboBox_sel.SelectedIndex + 1, $"select * from TCarrier with(nolock) where Carrier='{UID}'");

            string q1 = "INSERT INTO Carrier";
            string col = "(";
            string val = " values(";

            if (LocalData.Tables.Count == 0)
            {

            }
            else
            {
                for (int i = 0; i < LocalData.Tables[0].Columns.Count; i++)
                {
                    if (LocalData.Tables[0].Columns[i].ColumnName.Contains("Date") == true)
                    {
                        col += $"{LocalData.Tables[0].Columns[i].ColumnName},{LocalData.Tables[0].Columns[i].ColumnName.Replace("Date", "") + "Time,"}";
                    }
                    else if (LocalData.Tables[0].Columns[i].ColumnName == "ID" || LocalData.Tables[0].Columns[i].ColumnName == "ArticleName" || LocalData.Tables[0].Columns[i].ColumnName == "Enabled" || LocalData.Tables[0].Columns[i].ColumnName == "InitialOuttime")
                    {
                    }
                    else
                    {
                        col += $"{LocalData.Tables[0].Columns[i].ColumnName},";
                    }


                    switch (LocalData.Tables[0].Columns[i].ColumnName)
                    {
                        case "Depot":
                            val += $"'Tower {Tower_serial[LocalData.Tables[0].Rows[0][i].ToString().Split('.')[0]]}, " +
                            $"Magazine {Dec2Alpa[int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(0, 1))]}{int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(1, 1))}, " +
                            $"Slot {LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(2, 2)}',";
                            break;
                        case var _ when LocalData.Tables[0].Columns[i].ColumnName.Contains("Date") == true:
                            val += $"{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("yyyy-MM-dd")},";
                            val += $"'{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("hh:mm")}',";
                            break;
                        case "Expiry":
                            val += $"1900-01-01,";
                            break;
                        case "InitialOuttime":
                        case "Enabled":
                        case "ArticleName":
                        case "ID":
                            break;
                        case "Guessed":
                        case "Article":
                        case "Cycles":
                        case "Outtime":
                        case "Diameter":
                        case "Amplitude":
                        case "Stock":
                        case "StockNew":
                        case "StockUsed":
                        case "Duration":
                        case "Core":
                        case "Unleaded":
                        case "Height":
                        case "StepWidth":
                        case "StockMin":
                        case "StockTPSys":
                        case "Frequency":
                        case "TapeHeight":
                        case "PriceP":
                        case "HCode":
                        case "MSLWatch":
                            val += $"{LocalData.Tables[0].Rows[0][i].ToString()},";
                            break;
                        default:
                            val += $"'{LocalData.Tables[0].Rows[0][i].ToString().Trim()}',";
                            break;
                    }

                    #region column if else
                    //if (LocalData.Tables[0].Columns[i].ColumnName == "Depot")
                    //{
                    //    val += $"'Tower {Tower_serial[LocalData.Tables[0].Rows[0][i].ToString().Split('.')[0]]}, " +
                    //        $"Magazine {Dec2Alpa[int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(0, 1))]}{int.Parse(LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(1, 1))}, " +
                    //        $"Slot {LocalData.Tables[0].Rows[0][i].ToString().Split('.')[1].Substring(2, 2)}',";
                    //}
                    //else if(LocalData.Tables[0].Columns[i].ColumnName.Contains("Date") == true)
                    //{
                    //    val += $"{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("yyyy-MM-dd")},";
                    //    val += $"'{Convert.ToDateTime(LocalData.Tables[0].Rows[0][i].ToString()).ToString("hh:mm")}',";
                    //}
                    //else if (LocalData.Tables[0].Columns[i].ColumnName == "Article" || LocalData.Tables[0].Columns[i].ColumnName == "Stock" || LocalData.Tables[0].Columns[i].ColumnName == "StockMin" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "StockNew" || LocalData.Tables[0].Columns[i].ColumnName == "StockUsed" || LocalData.Tables[0].Columns[i].ColumnName == "StockTPSys" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "OutTime" || LocalData.Tables[0].Columns[i].ColumnName == "Duration" || LocalData.Tables[0].Columns[i].ColumnName == "Frequency" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Amplitude" || LocalData.Tables[0].Columns[i].ColumnName == "Core" || LocalData.Tables[0].Columns[i].ColumnName == "TapeHeight" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Guessed" || LocalData.Tables[0].Columns[i].ColumnName == "Unleaded" || LocalData.Tables[0].Columns[i].ColumnName == "PriceP" ||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Cycles" || LocalData.Tables[0].Columns[i].ColumnName == "Height" || LocalData.Tables[0].Columns[i].ColumnName == "HCode"||
                    //    LocalData.Tables[0].Columns[i].ColumnName == "Diameter" || LocalData.Tables[0].Columns[i].ColumnName == "StepWidth" || LocalData.Tables[0].Columns[i].ColumnName == "MSLWatch" 
                    //    )
                    //{
                    //    val += $"{LocalData.Tables[0].Rows[0][i].ToString()},";
                    //}
                    //else if(LocalData.Tables[0].Columns[i].ColumnName == "ID" || LocalData.Tables[0].Columns[i].ColumnName == "ArticleName" || LocalData.Tables[0].Columns[i].ColumnName == "Enabled" || LocalData.Tables[0].Columns[i].ColumnName == "InitialOuttime")
                    //{

                    //}
                    //else if(LocalData.Tables[0].Columns[i].ColumnName == "Expiry")
                    //{
                    //    val += $"1900-01-01,";
                    //}
                    //else
                    //{
                    //    val += $"'{LocalData.Tables[0].Rows[0][i].ToString().Trim()}',";
                    //}
                    #endregion
                }

                col = col.Substring(0, col.LastIndexOf(',')) + ")";
                val = val.Substring(0, val.LastIndexOf(',')) + ")";

                q1 += col + val;

                MDBConn.RunQuary(q1, MDBPath);
            }
        }

        private void SyncListCSVOut()
        {
            string filepath = System.Environment.CurrentDirectory + $"\\Sync_List\\SYNC_List_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.csv";

            if (Directory.Exists(System.Environment.CurrentDirectory + $"\\Sync_List") == false)
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + $"\\Sync_List");
            }

            System.IO.FileStream fs = new FileStream(filepath, FileMode.Create);

            fs.Write(Encoding.UTF8.GetBytes("Sync List\n"), 0, "Sync List\n".Length);

            string header = "DATETIME,EQUIP_ID,TOWER_NO,UID,SID,LOTID,QTY,INCH_INFO,SYNC_INFO\n";

            fs.Write(Encoding.UTF8.GetBytes(header), 0, header.Length);

            for (int i = 0; i < dataGridView_missmatch.Rows.Count; i++)
            {
                if (dataGridView_missmatch.Rows[i].Cells["UID"].Value.ToString() == "")
                {
                    dataGridView_missmatch.Rows.RemoveAt(i);
                    --i;
                }
            }

            string wstr = "";

            for (int rowNo = 0; rowNo < dataGridView_missmatch.Rows.Count; rowNo++)
            {
                wstr = $"{DateTime.Now.ToString("yyyyMMdd hhmmss")},TOWER{comboBox_sel.SelectedIndex + 1},{dataGridView_missmatch.Rows[rowNo].Cells["위치"].Value.ToString()},{dataGridView_missmatch.Rows[rowNo].Cells["UID"].Value.ToString()}" +
                    $",{dataGridView_missmatch.Rows[rowNo].Cells["SID"].Value.ToString()},{dataGridView_missmatch.Rows[rowNo].Cells["LOTID"].Value.ToString()},{dataGridView_missmatch.Rows[rowNo].Cells["Qty"].Value.ToString().Replace(",", "")}," +
                    $"{dataGridView_missmatch.Rows[rowNo].Cells["인치"].Value.ToString()},Out List Create\n";
                fs.Write(Encoding.UTF8.GetBytes(wstr), 0, wstr.Length);
            }

            fs.Flush();
            fs.Close();
            fs.Dispose();

            if (DialogResult.Yes == MessageBox.Show("파일을 여시겠습니까?", "file open?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                ProcessStartInfo info = new ProcessStartInfo("excel.exe", filepath);
                Process.Start(info);
            }
        }

        private void SyncListExcelOut()
        {
            string filepath = System.Environment.CurrentDirectory + $"\\Sync_Excel\\SYNC_List_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.xlsx";

            if(Directory.Exists(System.Environment.CurrentDirectory + $"\\Sync_Excel") == false)
            {
                Directory.CreateDirectory(System.Environment.CurrentDirectory + $"\\Sync_Excel");
            }

            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Add();// Filename: string.Format("{0}\\{1}", System.Environment.CurrentDirectory, @"\WaferReturn\WaferReturnOutTemp.xlsx"));

            Excel.Worksheet worksheet1 = workbook.Worksheets.get_Item(1);
            object misValue = System.Reflection.Missing.Value;

            application.Visible = false;


            worksheet1.Name = "SyncList";

            //System.Data.DataTable MtlList = SearchData(temp).Tables[0];//(System.Data.DataTable)dgv_ReturnWafer.DataSource;

            if (dataGridView_missmatch.Rows.Count != 0)
            {
                string[,] item = new string[dataGridView_missmatch.Rows.Count, 9];
                string[] columns = new string[dataGridView_missmatch.Columns.Count];

                Excel.Range rd = worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[1, 12]];
                rd.Merge();
                rd.Value2 = "Sync List";
                rd.Font.Bold = true;
                rd.Font.Size = 12.0;
                worksheet1.get_Range("A1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;



                for(int i = 0 ; i < dataGridView_missmatch.Rows.Count ; i++)
                {
                    if(dataGridView_missmatch.Rows[i].Cells["UID"].Value.ToString() == "")
                    {
                        dataGridView_missmatch.Rows.RemoveAt(i);
                        --i;
                    }
                }


                if (dataGridView_missmatch.Rows.Count > 0)
                {
                    for (int c = 0; c < dataGridView_missmatch.Columns.Count; c++)
                    {
                        //컬럼 위치값을 가져오기
                        columns[c] = ExcelColumnIndexToName(c);
                    }

                    for (int rowNo = 0; rowNo < dataGridView_missmatch.Rows.Count; rowNo++)
                    {
                        item[rowNo, 0] = DateTime.Now.ToString();
                        item[rowNo, 1] = $"TOWER{comboBox_sel.SelectedIndex + 1}";
                        item[rowNo, 2] = dataGridView_missmatch.Rows[rowNo].Cells["위치"].Value.ToString();
                        item[rowNo, 3] = dataGridView_missmatch.Rows[rowNo].Cells["UID"].Value.ToString();
                        item[rowNo, 4] = dataGridView_missmatch.Rows[rowNo].Cells["SID"].Value.ToString();
                        item[rowNo, 5] = dataGridView_missmatch.Rows[rowNo].Cells["LOTID"].Value.ToString();
                        item[rowNo, 6] = dataGridView_missmatch.Rows[rowNo].Cells["Qty"].Value.ToString();
                        item[rowNo, 7] = dataGridView_missmatch.Rows[rowNo].Cells["인치"].Value.ToString();
                        item[rowNo, 8] = "배출 명령 생성 완료";


                        //if (dataGridView_missmatch.Rows[rowNo].Cells["UID"].Value.ToString() != "")
                        //{
                        //    for (int colNo = 0; colNo < dataGridView_missmatch.Columns.Count; colNo++)
                        //    {
                        //        if (colNo == 0)
                        //        {

                        //        }
                        //        else
                        //        {
                        //            item[rowNo, colNo] = dataGridView_missmatch.Rows[rowNo].Cells[colNo].Value.ToString();
                        //        }
                        //    }
                        //}
                    }
                }


                //해당위치에 컬럼명을 담기
                //worksheet1.get_Range("A1", columns[MtlList.Columns.Count - 1] + "1").Value2 = headers;
                //해당위치부터 데이터정보를 담기
                               
                //for(int i = 0; i < dataGridView_missmatch.Columns.Count; i++)
                //{
                //    worksheet1.get_Range($"{(char)(0x41 + i)}3").Value = dataGridView_missmatch.Columns[i].HeaderText.ToString();
                //    worksheet1.get_Range($"{(char)(0x41 + i)}3").HorizontalAlignment = HorizontalAlignment.Center;
                //}

                worksheet1.get_Range("A3").Value = "DATETIME";
                worksheet1.get_Range("A3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("B3").Value = "EQUIP_ID";
                worksheet1.get_Range("B3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("C3").Value = "TOWER_NO";
                worksheet1.get_Range("C3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("D3").Value = "UID";
                worksheet1.get_Range("D3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("E3").Value = "SID";
                worksheet1.get_Range("E3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("F3").Value = "LOTID";
                worksheet1.get_Range("F3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("G3").Value = "QTY";
                worksheet1.get_Range("G3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("H3").Value = "INCH_INFO";
                worksheet1.get_Range("H3").HorizontalAlignment = HorizontalAlignment.Center;

                worksheet1.get_Range("I3").Value = "동기화 처리 내역";
                worksheet1.get_Range("I3").HorizontalAlignment = HorizontalAlignment.Center;


                worksheet1.get_Range("A4", columns[8] + (dataGridView_missmatch.Rows.Count + 3).ToString()).Value = item;
                worksheet1.get_Range("A4", columns[8] + (dataGridView_missmatch.Rows.Count + 3).ToString()).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                worksheet1.Cells.NumberFormat = @"@";
                worksheet1.Columns.AutoFit();


                if (filepath != "")
                {                    
                    workbook.SaveAs(filepath, Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                }
                else
                {
                    filepath = string.Format("{0}\\WaferReturnOut_{1}.xlsx", System.Environment.CurrentDirectory + "\\WaferReturn", DateTime.Now.ToString("yyyyMMddhhmmss"));
                    workbook.SaveAs(filepath, Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                }


                workbook.Close();
                application.Quit();

                releaseObject(application);
                releaseObject(worksheet1);
                releaseObject(workbook);


                if (DialogResult.Yes == MessageBox.Show("파일을 여시겠습니까?", "file open?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    ProcessStartInfo info = new ProcessStartInfo("excel.exe", filepath);
                    Process.Start(info);
                }



            }
            else
            {
                MessageBox.Show("데이터가 없습니다.");
            }
        }


        public string GetRandomString(int digit)
        {
            int numDigits = digit;

            StringBuilder result = new StringBuilder();
            Random rand = new Random();

            for (int i = 0; i < numDigits; i++)
            {
                int randNum = rand.Next(36);
                char randChar = randNum < 10 ? (char)('0' + randNum) : (char)('a' + randNum - 10);
                result.Append(randChar);
            }

            return result.ToString();
        }



        //]210810_Sangik.choi_장기보관관리기능추가(이종명수석님)
        public List<ASM_StorageData> GetSIMMMaterialList(string TowerLocation, string tid)
        {
            List<ASM_StorageData> list = new List<ASM_StorageData>();

            try
            {
                DataTable dt;

                dt = MSSql.GetData(GetMaterialListSIMMQuery(TowerLocation, tid));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ASM_StorageData data = new ASM_StorageData();
                    data.UID = dt.Rows[i]["UID"].ToString();
                    data.SID = dt.Rows[i]["Component"].ToString();
                    data.Quantity = dt.Rows[i]["Quantity"].ToString();
                    data.LotID = dt.Rows[i]["SupplierLotID"].ToString();
                    data.Date_Input = dt.Rows[i]["BookedToLocation"].ToString();
                    data.Productiondate = dt.Rows[i]["ProductionDate"].ToString();
                    data.Manufacturer = dt.Rows[i]["SupplierName"].ToString();

                    if (data.UID != "")
                        list.Add(data);
                }

                list.Sort(CompareStorageData_ASM);
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
                //Log.WriteLog(Log4net.EnumLogLevel.ERROR, ex.ToString());
            }

            return list;
        }

        private void comboBox_sel_SelectedIndexChanged(object sender, EventArgs e)
        {
            nDbUpdate = 0;
            dataGridView_asm.Rows.Clear();
            dataGridView_amm.Rows.Clear();
            dataGridView_missmatch.Rows.Clear();
            dgv_backup.Rows.Clear();
            MycronicTowerIP = "";

            if(comboBox_sel.SelectedIndex < 3)
            {
                dataGridView_asm.Size = new Size(325, 539);
                l_backUp.Visible = false;
                dgv_backup.Visible = false;
            }
            else
            {
                dataGridView_asm.Size = new Size(325, 294);
                l_backUp.Visible = true;
                dgv_backup.Visible = true;
            }

            btn_MakeOutList.Text = String.Format("Tower {0:0} 동기화", comboBox_sel.SelectedIndex + 1);

            Synclog.Info(string.Format("Tower group selectedm change : {0}", comboBox_sel.Text));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SyncListCSVOut();
        }

        

        private void button5_Click_1(object sender, EventArgs e)
        {
            frm_SyncHistory _SyncHistory = new frm_SyncHistory();

            _SyncHistory.ShowDialog();
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            Form_LongtimeReport report = new Form_LongtimeReport();

            report.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            longTermExcelExport();
            //longTermExcelOut();
        }

        private void longTermExcelExport()
        {
            string filePath = "";
            WorkBook workbook = new WorkBook();
            workbook.NumSheets = 1;
            workbook.setSheetName(0, "장기보관 릴 리스트");
            workbook.Sheet = 0;
            
            RangeStyle r = workbook.getRangeStyle();
            r.MergeCells = true;
            r.FontName = "Arial";
            r.FontBold = true;
            r.FontSize = 16*20;
            r.HorizontalAlignment = RangeStyle.HorizontalAlignmentCenter;
            workbook.setRangeStyle(r, 0, 0, 0, 10);
            workbook.setText(0, 0, "장기보관 릴 리스트");

            r.FontBold = false;
            r.FontSize = 10;
            workbook.setRangeStyle(r, 1, 0, 1, 1);
            workbook.setText(1, 0, $"Total : {dataGridView_longterm.Rows.Count}");
            
            workbook.setRangeStyle(r, 1, 9, 1, 10);
            workbook.setText(1, 9, $"Date : {DateTime.Now.ToString("yyyy-MM-dd")}");

            r = workbook.getRangeStyle(2, 0, 2, 10);
            r.TopBorder = RangeStyle.BorderThick;
            r.BottomBorder = RangeStyle.BorderThick;
            workbook.setRangeStyle(r, 2, 0, 2, 10);

            workbook.setText(2, 0, "No");
            workbook.setText(2, 1, "SID");
            workbook.setText(2, 2, "Batch");
            workbook.setText(2, 3, "UID");
            workbook.setText(2, 4, "QTY");
            workbook.setText(2, 5, "투입형태");
            workbook.setText(2, 6, "위치");
            workbook.setText(2, 7, "제조일");
            workbook.setText(2, 8, "투입일");
            workbook.setText(2, 9, "제조사");
            workbook.setText(2, 10, "인치");

            


            for(int i = 0; i < dataGridView_longterm.RowCount ; i++)
            {
                workbook.setText(3 + i, 0, $"{i+1}");
                workbook.setText(3 + i, 1, $"{dataGridView_longterm.Rows[i].Cells["SID"].Value.ToString()}");
                workbook.setText(3 + i, 2, $"{dataGridView_longterm.Rows[i].Cells["Batch#"].Value.ToString()}");
                workbook.setText(3 + i, 3, $"{dataGridView_longterm.Rows[i].Cells["UID"].Value.ToString()}");
                workbook.setText(3 + i, 4, $"{dataGridView_longterm.Rows[i].Cells["Qty"].Value.ToString()}");
                workbook.setText(3 + i, 5, $"{dataGridView_longterm.Rows[i].Cells["투입형태"].Value.ToString()}");
                workbook.setText(3 + i, 6, $"{dataGridView_longterm.Rows[i].Cells["위치"].Value.ToString()}");
                workbook.setText(3 + i, 7, $"{dataGridView_longterm.Rows[i].Cells["제조일"].Value.ToString()}");
                workbook.setText(3 + i, 8, $"{dataGridView_longterm.Rows[i].Cells["투입일"].Value.ToString()}");
                workbook.setText(3 + i, 9, $"{dataGridView_longterm.Rows[i].Cells["제조사"].Value.ToString()}");
                workbook.setText(3 + i, 10, $"{dataGridView_longterm.Rows[i].Cells["인치"].Value.ToString()}");                
            }


            if (Properties.Settings.Default.LongTermReelReportPath != "")
            {
                filePath = $"{Properties.Settings.Default.LongTermReelReportPath}\\LongTermReel_Over{comboBox_month.SelectedIndex + 1}Mon_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx";

                if (Directory.Exists(Properties.Settings.Default.LongTermReelReportPath) == false)
                    Directory.CreateDirectory(Properties.Settings.Default.LongTermReelReportPath);

                workbook.writeXLSX(filePath);
            }
            else
            {
                filePath = $"{System.Environment.CurrentDirectory + "\\LongTermReel"}\\LongTermReel_Over{comboBox_month.SelectedIndex + 1}Mon_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx";

                if (Directory.Exists(System.Environment.CurrentDirectory + "\\LongTermReel") == false)
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\LongTermReel");

                Properties.Settings.Default.LongTermReelReportPath = System.Environment.CurrentDirectory + "\\LongTermReel";
                Properties.Settings.Default.Save();

                workbook.writeXLSX(filePath);
            }
        }

        private void longTermExcelOut()
        {
            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = application.Workbooks.Add();// Filename: string.Format("{0}\\{1}", System.Environment.CurrentDirectory, @"\WaferReturn\WaferReturnOutTemp.xlsx"));

            Worksheet worksheet1 = workbook.Worksheets[0];
            object misValue = System.Reflection.Missing.Value;

            
            worksheet1.Name = "장기보관 Reel 리스트";


            if (dataGridView_longterm.Rows.Count != 0)
            {
                string[,] item = new string[dataGridView_longterm.Rows.Count, dataGridView_longterm.Columns.Count + 1];
                string[] columns = new string[dataGridView_longterm.Columns.Count + 1];


                Range rd = worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[1, 11]];
                rd.Merge();
                rd.Value2 = "장기보관 Reel 리스트";
                rd.Font.Bold = true;
                rd.Font.Size = 16.0;                
                rd.HorizontalAlignment = HorizontalAlignment.Center;
                worksheet1.get_Range("A1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                rd = worksheet1.Range[worksheet1.Cells[2, 1], worksheet1.Cells[2, 2]];
                rd.Merge();
                rd.Value2 = $"Total : {dataGridView_longterm.RowCount}";
                rd.HorizontalAlignment = HorizontalAlignment.Center;
                worksheet1.get_Range("A2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                rd = worksheet1.Range[worksheet1.Cells[2, 10], worksheet1.Cells[2, 11]];
                rd.Merge();
                rd.Value2 = $"Date : {DateTime.Now.ToShortDateString()}";
                rd.HorizontalAlignment = HorizontalAlignment.Center;
                worksheet1.get_Range("J2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                //worksheet1.get_Range("A1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                //rd = worksheet1.Range[worksheet1.Cells[3, 3], worksheet1.Cells[4, 11]];
                //rd.Font.Color = Color.Red;
                //rd.Font.Size = 20.0;
                //rd.Merge();
                //rd.HorizontalAlignment = HorizontalAlignment.Center;
                //rd.Value2 = "★고객 요청 사항 확인★";
                //worksheet1.get_Range("D3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;


                //rd = worksheet1.Range[worksheet1.Cells[4, 12], worksheet1.Cells[4, 12]];
                //rd.Font.Color = Color.Red;
                //rd.Font.Size = 20.0;
                ////rd.Merge();
                //rd.HorizontalAlignment = HorizontalAlignment.Center;
                //rd.Value2 = "Total QTY";
                //worksheet1.get_Range("D3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                int QtyCnt = 0;


                if (dataGridView_longterm.Rows.Count > 0)
                {
                    
                    for (int c = 0; c < dataGridView_longterm.Columns.Count+1; c++)
                    {
                        //컬럼 위치값을 가져오기
                        columns[c] = ExcelColumnIndexToName(c);
                    }

                    for (int rowNo = 0; rowNo < dataGridView_longterm.Rows.Count; rowNo++)
                    {
                        for (int colNo = 0; colNo < dataGridView_longterm.Columns.Count +1; colNo++)
                        {
                            if(colNo == 0)
                            {
                                item[rowNo, colNo] = (rowNo + 1).ToString();
                            }
                            else
                            {
                                item[rowNo, colNo] = dataGridView_longterm.Rows[rowNo].Cells[colNo-1].Value.ToString().Trim();
                            }
                            
                        }                        
                    }
                }

                //해당위치에 컬럼명을 담기
                //worksheet1.get_Range("A1", columns[MtlList.Columns.Count - 1] + "1").Value2 = headers;
                //해당위치부터 데이터정보를 담기

                //rd = worksheet1.Range[worksheet1.Cells[4, 13], worksheet1.Cells[4, 14]];
                //rd.Font.Color = Color.Black;
                //rd.Font.Size = 20.0;
                //rd.Merge();
                //rd.HorizontalAlignment = HorizontalAlignment.Center;
                //rd.Value2 = QtyCnt;
                //worksheet1.get_Range("M4").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;


                /**
            *dataGridView_longterm.Columns.Add("SID", "SID");
            *dataGridView_longterm.Columns.Add("Batch#", "Batch#");
            *dataGridView_longterm.Columns.Add("UID", "UID");
            *dataGridView_longterm.Columns.Add("Qty", "Qty");
            *dataGridView_longterm.Columns.Add("투입형태", "투입형태");
            *dataGridView_longterm.Columns.Add("위치", "위치");
            *dataGridView_longterm.Columns.Add("제조일", "제조일");
            *dataGridView_longterm.Columns.Add("투입일", "투입일");
            *dataGridView_longterm.Columns.Add("제조사", "제조사");
            *dataGridView_longterm.Columns.Add("인치", "인치");
             */

                
                worksheet1.get_Range("A3").Value2 = "No";
                worksheet1.get_Range("B3").Value2 = "SID";
                worksheet1.get_Range("C3").Value2 = "Batch";
                worksheet1.get_Range("D3").Value2 = "UID";
                worksheet1.get_Range("E3").Value2 = "QTY";
                worksheet1.get_Range("F3").Value2 = "투입형태";
                worksheet1.get_Range("G3").Value2 = "위치";
                worksheet1.get_Range("H3").Value2 = "제조일";
                worksheet1.get_Range("I3").Value2 = "투입일";
                worksheet1.get_Range("J3").Value2 = "제조사";
                worksheet1.get_Range("K3").Value2 = "인치";
                

                rd = worksheet1.Range["A3", "K3"];
                //rd.BorderAround2(XlLineStyle.xlDash);
                //rd.Borders[XlBordersIndex.xlDiagonalUp].LineStyle = Excel.XlLineStyle.xlContinuous;
                //rd.Borders[XlBordersIndex.xlDiagonalDown].LineStyle = Excel.XlLineStyle.xlContinuous;

                rd.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                rd.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;
                rd.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = XlBorderWeight.xlThick;

                worksheet1.get_Range("A4", columns[dataGridView_longterm.Columns.Count - 0] + (dataGridView_longterm.Rows.Count + 3).ToString()).Value = item;
                worksheet1.get_Range("A4", columns[dataGridView_longterm.Columns.Count - 0] + (dataGridView_longterm.Rows.Count + 3).ToString()).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                worksheet1.Cells.NumberFormat = @"@";
                worksheet1.Columns.AutoFit();

                worksheet1.PageSetup.PrintArea = string.Format("A1:{0}", columns[dataGridView_longterm.Columns.Count - 3] + (dataGridView_longterm.Rows.Count + 5).ToString());
                worksheet1.PageSetup.Zoom = false;
                worksheet1.PageSetup.FitToPagesWide = 1;        // Zoom이 False일 때만 적용 됨

                string filePath = "";


                if (Properties.Settings.Default.LongTermReelReportPath != "")
                {
                    filePath = $"{Properties.Settings.Default.LongTermReelReportPath}\\LongTermReel_Over{comboBox_month.SelectedIndex +1}Mon_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx";

                    if (Directory.Exists(Properties.Settings.Default.LongTermReelReportPath) == false)
                        Directory.CreateDirectory(Properties.Settings.Default.LongTermReelReportPath);

                    workbook.SaveAs(filePath, Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                }
                else
                {
                    filePath = $"{System.Environment.CurrentDirectory + "\\LongTermReel"}\\LongTermReel_Over{comboBox_month.SelectedIndex +1}Mon_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx";

                    if (Directory.Exists(System.Environment.CurrentDirectory + "\\LongTermReel") == false)
                        Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\LongTermReel");

                    Properties.Settings.Default.LongTermReelReportPath = System.Environment.CurrentDirectory + "\\LongTermReel";
                    Properties.Settings.Default.Save();

                    workbook.SaveAs(filePath, Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                }
                workbook.Close();
                application.Quit();

                releaseObject(application);
                releaseObject(worksheet1);
                releaseObject(workbook);

                if (isBackground == false)
                {
                    if (DialogResult.Yes == MessageBox.Show("파일을 여시겠습니까?", "file open?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        ProcessStartInfo info = new ProcessStartInfo("excel.exe", filePath);
                        Process.Start(info);
                    }
                }
            }
            else
            {
                MessageBox.Show("데이터가 없습니다.");
            }
        }

        public string GetMaterialListSIMMQuery(string towerLocation, string tid)
        {
            try
            {
                string[] split = towerLocation.Split('.');
                string twrLocation = split[split.Length - 1];

                string query = string.Format(@"
                SELECT
                RANK() OVER (ORDER BY FLOT.ID) AS IDX,
                FLOT.ID AS UID, FLOT.MaterialID AS Component, FLOT.Quantity, FLOT.SupplierLotID, FLOT.BookedToLocation, FLOT.ProductionDate, FLOT.SupplierName
                FROM 
                (
	                SELECT ID FROM FactsLocation WITH (NOLOCK)
	                WHERE Name='{0}'
                ) FLOC
                JOIN 
                (
	                SELECT ID, MaterialID, Quantity, Customer1, LocationID, SupplierLotID, BookedToLocation,ProductionDate,SupplierName
	                FROM FactsLot WITH (NOLOCK)
	                WHERE Customer1='{1}'
                ) FLOT
                ON FLOC.ID = FLOT.LocationID",
                     twrLocation, tid);

                return query;
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
                //Log.WriteLog(Log4net.EnumLogLevel.ERROR, ex.ToString());
            }
            return "";
        }
        int CompareStorageData(StorageData_Compare obj1, StorageData_Compare obj2)
        {
            return obj1.UID.CompareTo(obj2.UID);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form_LongtimeReport report = new Form_LongtimeReport();
            report.MakeExcelReportEvent += Report_MakeExcelReportEvent;
            report.GetDataGridRowCountEvent += Report_GetDataGridRowCountEvent;
            
            report.ShowDialog();
        }

        private int Report_GetDataGridRowCountEvent()
        {
            return dataGridView_longterm.RowCount;
        }

        private void Report_MakeExcelReportEvent(int month)
        {
            comboBox_month.SelectedIndex = month;
            comboBox_L_group.SelectedIndex = comboBox_L_group.Items.Count - 1;
            

            button_display_Click(new object(), new EventArgs());
            longTermExcelOut();


        }

        int CompareStorageData_ASM(ASM_StorageData obj1, ASM_StorageData obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
        }

        int CompareStorageData_AMM(AMM_StorageData obj1, AMM_StorageData obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
        }


        bool isBackground = false;
        public bool SetLongTermReport()
        {
            try
            {
                isBackground = true;
                comboBox_month.SelectedIndex = Properties.Settings.Default.LongTimeReelReportMonth - 1;
                comboBox_L_group.SelectedIndex = comboBox_L_group.Items.Count - 1;

                button_display_Click(new object(), new EventArgs());


                button6_Click(new object(), new EventArgs());

                return true;
            }
            catch (Exception ex)
            {
                return false;                
            }            
        }


        public static DialogResult InputBox(string title, string content, ref string value)
        {
            Form form = new Form();
            PictureBox picture = new PictureBox();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.ClientSize = new Size(300, 100);
            form.Controls.AddRange(new Control[] { label, picture, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            form.Text = title;
            //picture.Image = Properties.Resources.Clogo;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;
            label.Text = content;
            textBox.Text = value;
            buttonOk.Text = "확인";
            buttonCancel.Text = "취소";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            picture.SetBounds(10, 10, 50, 50);
            label.SetBounds(65, 17, 100, 20);
            textBox.SetBounds(65, 40, 220, 20);
            buttonOk.SetBounds(135, 70, 70, 20);
            buttonCancel.SetBounds(215, 70, 70, 20);

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
        }
    }
}

public class StorageData
{
    public string Linecode = "";
    public string Equipid = "";
    public string Input_date = "";
    public string Tower_no = "";
    public string UID = "";
    public string SID = "";
    public string LOTID = "";
    public string Quantity = "";
    public string Manufacturer = "";
    public string Production_date = "";
    public string Inch = "";
    public string Input_type = "";
    public string Requestor = "";
}

public class Inchdata
{
    public string Equipid = "";
    public string Inch_7_cnt = "";
    public string Inch_13_cnt = "";
    public string Inch_7_capa = "";
    public string Inch_13_capa = "";
    public string Inch_7_rate = "";
    public string Inch_13_rate = "";

}

public class StorageData2
{
    public string Creation_date = "";
    public string Equipid = "";
    public string pickid = "";
    public string UID = "";
    public string SID = "";
    public string Status = "";
    public string Tower_no = "";
    public string LOTID = "";
    public string Quantity = "";
    public string Manufacturer = "";
    public string Production_date = "";
    public string Inch = "";
    public string Input_type = "";
    public string Requestor = "";
}

public class ASM_StorageData
{
    public string SID = "";
    public string LotID = "";
    public string UID = "";
    public string Quantity = "";
    public string Date_Input = "";
    public string Productiondate = "";
    public string Manufacturer = "";
}

public class AMM_StorageData
{
    public string Linecode = "";
    public string Equipid = "";
    public string Input_date = "";
    public string Tower_no = "";
    public string UID = "";
    public string SID = "";
    public string LOTID = "";
    public string Quantity = "";
    public string Manufacturer = "";
    public string Production_date = "";
    public string Inch = "";
    public string Input_type = "";
    public string Requestor = "";
}

public class StorageData_Compare
{
    public string Linecode = "";
    public string Equipid = "";
    public string Input_date = "";
    public string Tower_no = "";
    public string UID = "";
    public string SID = "";
    public string LOTID = "";
    public string Quantity = "";
    public string Manufacturer = "";
    public string Production_date = "";
    public string Inch = "";
    public string Input_type = "";
    public string Requestor = "";
    public string Miss = "";
}

public static class ExtensionMethods
{
    public static void DoubleBuffered(this DataGridView dgv, bool setting)

    {

        Type dgvType = dgv.GetType();

        PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",

            BindingFlags.Instance | BindingFlags.NonPublic);

        pi.SetValue(dgv, setting, null);

    }

}
public class SharedAPI
{
    // 구조체 선언
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NETRESOURCE
    {
        public uint dwScope;
        public uint dwType;
        public uint dwDisplayType;
        public uint dwUsage;
        public string lpLocalName;
        public string lpRemoteName;
        public string lpComment;
        public string lpProvider;
    }

    // API 함수 선언
    [DllImport("mpr.dll", CharSet = CharSet.Auto)]
    public static extern int WNetUseConnection(
                IntPtr hwndOwner,
                [MarshalAs(UnmanagedType.Struct)] ref NETRESOURCE lpNetResource,
                string lpPassword,
                string lpUserID,
                uint dwFlags,
                StringBuilder lpAccessName,
                ref int lpBufferSize,
                out uint lpResult);

    // API 함수 선언 (공유해제)
    [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Auto)]
    public static extern int WNetCancelConnection2A(string lpName, int dwFlags, int fForce);

    // 공유연결
    public int ConnectRemoteServer(string server)
    {
        int capacity = 64;
        uint resultFlags = 0;
        uint flags = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder(capacity);
        NETRESOURCE ns = new NETRESOURCE();
        ns.dwType = 1;              // 공유 디스크
        ns.lpLocalName = null;   // 로컬 드라이브 지정하지 않음
        ns.lpRemoteName = server;
        ns.lpProvider = null;
        int result = 0;

        result = WNetUseConnection(IntPtr.Zero, ref ns, "Siplace.1", "Administrator", flags, sb, ref capacity, out resultFlags);

        return result;
    }

    // 공유해제
    public void CencelRemoteServer(string server)
    {
        WNetCancelConnection2A(server, 1, 0);
    }
}


class DataConn
{
    public DataSet GetDataset(string quary, string DBpath)
    {
        string connStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBpath + ";Jet OLEDB:Database Password=";


        OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr);
        DataSet ds = new DataSet();
        OleDbDataAdapter adp = new OleDbDataAdapter(quary, conn);
        adp.Fill(ds);
        return ds;
    }

    public int TouchRow(string quary, string DBPath)
    {
        int res = -1;

        string connStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath + ";Jet OLEDB:Database Password=";
        
        OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr);
        DataSet ds = new DataSet();
        OleDbDataAdapter adp = new OleDbDataAdapter(quary, conn);
        adp.Fill(ds);

        return ds.Tables[0].Rows.Count;
    }

    public int RunQuary(string quary, string DBPath)
    {
        try
        {
            string connStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath + ";Jet OLEDB:Database Password=";

            OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr);
            DataSet ds = new DataSet();
            OleDbDataAdapter adp = new OleDbDataAdapter(quary, conn);
            adp.Fill(ds);

            return ds.Tables.Count> 0 ? ds.Tables[0].Rows.Count : 1;
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }

    public int  DeleteData(string UID, string DBPath)
    {
        int res = -1;

        try
        {
            string connStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath + ";Jet OLEDB:Database Password=";

            OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(string.Format("DELETE * from Carrier WHERE Carrier='{0}'", UID), conn);

             res = cmd.ExecuteNonQuery();

            return res;
        }
        catch (Exception ex)
        {
            
        }

        return res;
    }

    
}