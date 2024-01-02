using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace Amkor_Material_Manager
{
    public partial class frm_SyncHistory : Form
    {
        string dirPath = System.Environment.CurrentDirectory + "\\Sync_Excel\\";
        string filepath = System.Environment.CurrentDirectory + $"\\Sync_Excel\\SYNC_List_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.xlsx";

        public frm_SyncHistory()
        {
            InitializeComponent();
        }

        private DataTable SearchData( string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection c = new SqlConnection("server=10.135.200.35;uid=amm;pwd=amm@123;database=ATK4-AMM-DBv1"))
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


        private void frm_SyncHistory_Load(object sender, EventArgs e)
        {
            

            if (Properties.Settings.Default.SyncOUtExcelPath == "")
            {
                Properties.Settings.Default.SyncOUtExcelPath = dirPath;
                Properties.Settings.Default.Save();
            }
            else
            {
                dirPath = Properties.Settings.Default.SyncOUtExcelPath;
            }


            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }

            dtp_from.Format = DateTimePickerFormat.Short;
            dtp_to.Format = DateTimePickerFormat.Short;

            dtp_from.MinDate = DateTime.Parse("2023.10.26 00:00");
            dtp_to.MinDate = DateTime.Parse("2023.10.26 00:00");

            dtp_from.MaxDate = DateTime.Now;
            dtp_to.MaxDate = DateTime.Now;
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            string q = $"select  [DATETIME],[EQUIP_ID],[TOWER_NO],[UID],[SID],[LOTID],[QTY],[INCH_INFO],[SYNC_INFO],[EMPLOYEE_NO] " +
                $"from [TB_SYNC_INFO] with(nolock) where [DATETIME] >= '{dtp_from.Value.Date.ToString("yyyyMMdd")}' and [DATETIME] <= '{dtp_to.Value.Date.ToString("yyyyMMdd")} 23:59:59'";

            DataTable dt= SearchData(q);


            foreach(System.Data.DataRow row  in dt.Rows)
            {
                dgv_SyncHistory.Rows.Add(new object[] { row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString(), row[7].ToString(), row[8].ToString(), row[9].ToString() });
            }
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            SyncListExcelOut();
        }

        private void SyncListExcelOut()
        {
            try
            {
                string filepath = dirPath + $"\\SYNC_List_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.xlsx";


                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = application.Workbooks.Add();// Filename: string.Format("{0}\\{1}", System.Environment.CurrentDirectory, @"\WaferReturn\WaferReturnOutTemp.xlsx"));

                Worksheet worksheet1 = workbook.Worksheets.get_Item(1);
                object misValue = System.Reflection.Missing.Value;

                application.Visible = false;


                worksheet1.Name = "SyncList";

                //System.Data.DataTable MtlList = SearchData(temp).Tables[0];//(System.Data.DataTable)dgv_ReturnWafer.DataSource;

                

                if (dgv_SyncHistory.Rows.Count != 0)
                {
                    string[,] item = new string[dgv_SyncHistory.Rows.Count, 10];
                    string[] columns = new string[dgv_SyncHistory.Columns.Count];

                    for (int c = 0; c < dgv_SyncHistory.Columns.Count; c++)
                    {
                        //컬럼 위치값을 가져오기
                        columns[c] = ExcelColumnIndexToName(c);
                    }

                    Range rd = worksheet1.Range[worksheet1.Cells[1, 1], worksheet1.Cells[1, 12]];
                    rd.Merge();
                    rd.Value2 = "Sync List";
                    rd.Font.Bold = true;
                    rd.Font.Size = 12.0;
                    worksheet1.get_Range("A1").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    for (int rowNo = 0; rowNo < dgv_SyncHistory.Rows.Count; rowNo++)
                    {
                        item[rowNo, 0] = dgv_SyncHistory.Rows[rowNo].Cells[0].Value.ToString();
                        item[rowNo, 1] = dgv_SyncHistory.Rows[rowNo].Cells[1].Value.ToString();
                        item[rowNo, 2] = dgv_SyncHistory.Rows[rowNo].Cells[2].Value.ToString();
                        item[rowNo, 3] = dgv_SyncHistory.Rows[rowNo].Cells[3].Value.ToString();
                        item[rowNo, 4] = dgv_SyncHistory.Rows[rowNo].Cells[4].Value.ToString();
                        item[rowNo, 5] = dgv_SyncHistory.Rows[rowNo].Cells[5].Value.ToString();
                        item[rowNo, 6] = dgv_SyncHistory.Rows[rowNo].Cells[6].Value.ToString();
                        item[rowNo, 7] = dgv_SyncHistory.Rows[rowNo].Cells[7].Value.ToString();
                        item[rowNo, 8] = dgv_SyncHistory.Rows[rowNo].Cells[8].Value.ToString();
                        item[rowNo, 9] = dgv_SyncHistory.Rows[rowNo].Cells[9].Value.ToString();
                    }
                    //해당위치에 컬럼명을 담기
                    //worksheet1.get_Range("A1", columns[MtlList.Columns.Count - 1] + "1").Value2 = headers;
                    //해당위치부터 데이터정보를 담기

                    //for(int i = 0; i < dgv_SyncHistory.Columns.Count; i++)
                    //{
                    //    worksheet1.get_Range($"{(char)(0x41 + i)}3").Value = dgv_SyncHistory.Columns[i].HeaderText.ToString();
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

                    worksheet1.get_Range("J3").Value = "사번";
                    worksheet1.get_Range("J3").HorizontalAlignment = HorizontalAlignment.Center;

                    

                    worksheet1.get_Range("A4", columns[dgv_SyncHistory.ColumnCount-1] + (dgv_SyncHistory.Rows.Count + 3).ToString()).Value = item;                   
                    worksheet1.get_Range("A4", columns[dgv_SyncHistory.ColumnCount-1] + (dgv_SyncHistory.Rows.Count + 3).ToString()).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    worksheet1.Cells.NumberFormat = @"@";
                    worksheet1.Columns.AutoFit();


                    if (filepath != "")
                    {
                        workbook.SaveAs(filepath, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                    }
                    else
                    {
                        
                    }


                    workbook.Close();
                    application.Quit();

                    releaseObject(application);
                    releaseObject(worksheet1);
                    releaseObject(workbook);


                    if (DialogResult.Yes == MessageBox.Show("파일을 여시겠습니까?", "file open?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        ProcessStartInfo info = new ProcessStartInfo("excel.exe", "\"" + filepath + "\"");
                        Process.Start(info);
                    }



                }
                else
                {
                    MessageBox.Show("데이터가 없습니다.");
                }
            }
            catch (Exception ex )
            {

                throw;
            }
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

        private void btn_directory_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = dirPath;

            if(DialogResult.OK ==  folderBrowserDialog1.ShowDialog())
            {
                dirPath = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.SyncOUtExcelPath = dirPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
