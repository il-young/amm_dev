﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Amkor_Material_Manager
{
    public partial class Form_Set : Form
    {
        public Form_Set()
        {
            InitializeComponent();
            Fnc_Init();
        }

        public void Fnc_Init()
        {
            comboBox_twrNo.Refresh();
            
            for(int n = 1; n < 10; n++)//210825_Sangik.choi_타워그룹추가 7 -> 8 //220823_ilyoung_타워그룹추가
            {
                for(int m = 1; m < 5; m++)
                {
                    string strName = string.Format("T0{0}0{1}", n, m);
                    comboBox_twrNo.Items.Add(strName);
                }
            }
        }

        public void Fnc_View(int n)
        {
            if (n == 0) //Request
            {
                panel2.Visible = false;
                panel1.Visible = true;
                textBox_sid.Focus();
            }
            else
            {
                panel1.Visible = false;
                panel2.Visible = true;
                Fnc_View_Request();

                textBox_line.Text = AMM_Main.strDefault_linecode;
                textBox_group.Text = AMM_Main.strDefault_Group;

                comboBox_startup.SelectedIndex = Int32.Parse(AMM_Main.strDefault_Start);

                if(AMM_Main.strSMSearchEnable == "FALSE")
                {
                    comboBox_smsearch.SelectedIndex = 0;
                }
                else
                {
                    comboBox_smsearch.SelectedIndex = 1;
                }

                if (AMM_Main.strMatchTab == "FALSE")
                {
                    comboBox_match.SelectedIndex = 0;
                }
                else
                {
                    comboBox_match.SelectedIndex = 1;
                }

                if (AMM_Main.strNumberPad == "TRUE")
                {
                    comboBox_pad.SelectedIndex = 1;
                }
                else
                {
                    comboBox_pad.SelectedIndex = 0;
                }
            }
        }

        public void Fnc_Send_Request(string strSid, string strName)
        {
            //string strJudge = AMM_Main.AMM.SetUserRequest(strSid, strName);

            //if (strJudge == "OK")
            //{
            //    Fnc_Send_Email(strSid, strName, 0);
            //    MessageBox.Show("사용 요청이 정상적으로 등록 되었습니다.");
            //}
            //else
            //{
            //    MessageBox.Show("요청 실패!", "사용 요청");
            //}
            string res = AMM_Main.AMM.User_Register(strSid, strName);

            if (res == "OK")
                MessageBox.Show("정상적으로 등록 되었습니다.");
            else
                MessageBox.Show("등록이 실패 했습니다.");
        }

        public void Fnc_Send_Email(string strSid, string strName, int nType)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

            message.To.Add("HaeYoung.Jeong@amkor.co.kr");
            message.To.Add("ByeongUn.Choi@amkor.co.kr");
            //message.To.Add("BockWon.Kang@amkor.co.kr");
            message.To.Add("HyunChol.Lee@amkor.co.kr");
            message.To.Add("chayoun.lee@amkor.co.kr");
            message.To.Add("eunhwa.jeong@amkor.co.kr");
            message.To.Add("yuri.seo@amkor.co.kr");
            message.To.Add("eunjung.kim@amkor.co.kr");
            message.To.Add("hyemin.pak@amkor.co.kr");

            if (nType == 0)
            {
                message.Subject = string.Format("[AMM 사용등록요청] 사번:{0} 이름:{1}", strSid, strName);
                message.From = new System.Net.Mail.MailAddress("Amkor.AMM@amkor.co.kr");
                message.Body = string.Format("AMM Reel 청구 사용자 등록 요청 메일 입니다. 관리자께서는 검토 후 등록 해 주시길 바랍니다.\n\n 사번:{0} \n 이름:{1}", strSid, strName);
            }
            else if(nType == 1)
            {
                message.Subject = string.Format("[AMM 등록완료] 이름:{0}", strName);
                message.From = new System.Net.Mail.MailAddress("Amkor.AMM@amkor.co.kr");
                message.Body = string.Format("AMM Reel 청구 사용자 등록이 완료 되었습니다.\n\n 사번:{0} \n 이름:{1}", strSid, strName);
            }
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("10.101.10.6");
            smtp.Credentials = new System.Net.NetworkCredential("", "");
            smtp.Port = 25;
            smtp.Send(message);
        }

        public void Fnc_SendEmail(string strRequestor)
        {
            string strMsg = string.Format("{0} 님께서 AMM 사용 권한을 요청 하셨습니다. 확인 후 승인 하여 주십시오.", strRequestor);
        }

        private void button_request_Click(object sender, EventArgs e)
        {
            if (textBox_sid.Text != "" && textBox_name.Text != "")
                Fnc_Send_Request(textBox_sid.Text, textBox_name.Text);
            else
            {
                MessageBox.Show("정보가 모두 입력 되지 않았습니다. SID, 이름을 모두 입력 하십시오.");
            }

            textBox_sid.Text = "";
            textBox_name.Text = "";
            textBox_sid.Focus();
        }

        private void button_View_Click(object sender, EventArgs e)
        {
            Fnc_View_Request();
        }

        private void Fnc_View_Request()
        {
            dataGridView_List.Columns.Clear();
            dataGridView_List.Rows.Clear();
            dataGridView_List.Refresh();

            dataGridView_List.Columns.Add("요청일자", "요청일자");
            dataGridView_List.Columns.Add("SID", "SID");
            dataGridView_List.Columns.Add("이름", "이름");

            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            dataGridView_List.Columns.Add(chk);
            chk.HeaderText = "선택";
            chk.Name = "chk";

            var dt_List = AMM_Main.AMM.GetUserRequest();

            for (int n = 0; n < dt_List.Rows.Count; n++)
            {
                string strdate = dt_List.Rows[n]["DATETIME"].ToString();

                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                string strSid = dt_List.Rows[n]["USER_SID"].ToString(); strSid = strSid.Trim();
                string strName = dt_List.Rows[n]["USER_NAME"].ToString(); strName = strName.Trim();

                dataGridView_List.Rows.Add(new object[3] { strdate, strSid, strName });
                dataGridView_List.Rows[n].Cells[3].Value = true;
            }
        }

        private void button_accept_Click(object sender, EventArgs e)
        {
            if (dataGridView_List.Rows.Count < 1)
                return;

            DialogResult dialogResult1 = MessageBox.Show("승인 하시겠습니끼?", "승인", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            int nCount = dataGridView_List.Rows.Count;

            string str = "", strSumSid = "", strSumName = "";

            for (int n = 0; n < nCount; n++)
            {
                var Value = dataGridView_List.Rows[n].Cells[3].Value.ToString();

                if (Value == "True")
                {
                    string strSid = dataGridView_List.Rows[n].Cells[1].Value.ToString();
                    string strName = dataGridView_List.Rows[n].Cells[2].Value.ToString();

                    string strJudge = AMM_Main.AMM.User_Register(strSid, strName);
                    if (strJudge == "NG")
                    {
                        str = string.Format("등록 실패! {0} {1}", strSid, strName);
                        MessageBox.Show(str);
                        return;
                    }
                    else
                    {
                        strJudge = AMM_Main.AMM.Delete_UserRequest(strSid);
                        if (strJudge != "OK")
                        {
                            str = string.Format("등록 실패! {0} {1}", strSid, strName);
                            MessageBox.Show(str);
                        }
                    }
                    
                    if (strSumSid == "")
                        strSumSid = strSid + ";";
                    else
                        strSumSid = strSumSid + strSid + ";";

                    if (strSumName == "")
                        strSumName = strName + ";";
                    else
                        strSumName = strSumName + strName + ";";
                }
            }

            Fnc_Send_Email(strSumSid, strSumName, 1);

            str = string.Format("등록이 완료 되었습니다.");
            MessageBox.Show(str);
            Fnc_View_Request();
        }

        private void toolStripMenuItem_Refuse_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("반려 하시겠습니끼?", "반려", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            if (dataGridView_List.Rows.Count < 1)
                return;

            int nIndex = dataGridView_List.CurrentCell.RowIndex;

            if (nIndex < 0)
                return;

            string strSid = dataGridView_List.Rows[nIndex].Cells[1].Value.ToString();
            string strName = dataGridView_List.Rows[nIndex].Cells[2].Value.ToString();

            string strJudge = AMM_Main.AMM.Delete_UserRequest(strSid);
            if (strJudge == "OK")
            {
                string str = string.Format("반려 되었습니다. {0} {1}", strSid, strName);
                MessageBox.Show(str);
            }

            Fnc_View_Request();
        }

        private void toolStripMenuItem_accept_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("승인 하시겠습니끼?", "승인", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            if (dataGridView_List.Rows.Count < 1)
                return;

            int nIndex = dataGridView_List.CurrentCell.RowIndex;

            if (nIndex < 0)
                return;

            string strSid = dataGridView_List.Rows[nIndex].Cells[1].Value.ToString();
            string strName = dataGridView_List.Rows[nIndex].Cells[2].Value.ToString();

            string strJudge = AMM_Main.AMM.User_Register(strSid, strName);
            if (strJudge == "NG")
            {
                string str = string.Format("등록 실패! {0} {1}", strSid, strName);
                MessageBox.Show(str);
                return;
            }
            else
            {
                strJudge = AMM_Main.AMM.Delete_UserRequest(strSid);
                if (strJudge == "OK")
                {
                    string str = string.Format("등록이 완료 되었습니다. {0} {1}", strSid, strName);
                    MessageBox.Show(str);
                }
            }

            Fnc_Send_Email(strSid, strName, 1);

            Fnc_View_Request();
        }

        private void textBox_sid_KeyPress(object sender, KeyPressEventArgs e)
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

        private void button_Save_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("저장 하시겠습니끼?", "저장", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            Fnc_Update_Config();
        }

        public void Fnc_Update_Config()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration
                    (ConfigurationUserLevel.None);

            config.AppSettings.Settings.Remove("Lincode");
            config.AppSettings.Settings.Add("Lincode", textBox_line.Text);
            AMM_Main.strDefault_linecode = textBox_line.Text;

            config.AppSettings.Settings.Remove("Group");
            config.AppSettings.Settings.Add("Group", textBox_group.Text);
            AMM_Main.strDefault_Group = textBox_group.Text;
            AMM_Main.nDefaultGroup = Int32.Parse(AMM_Main.strDefault_Group);

            int nIndex = comboBox_startup.SelectedIndex;
            config.AppSettings.Settings.Remove("Startup");
            config.AppSettings.Settings.Add("Startup", nIndex.ToString());            

            nIndex = comboBox_smsearch.SelectedIndex;
            string strEnable = "";

            if (nIndex == 0)
                strEnable = "FALSE";
            else
                strEnable = "TRUE";

            config.AppSettings.Settings.Remove("SM_Enable");
            config.AppSettings.Settings.Add("SM_Enable", strEnable);
            AMM_Main.strSMSearchEnable = strEnable;

            nIndex = comboBox_match.SelectedIndex;
            strEnable = "";

            if (nIndex == 0)
                strEnable = "FALSE";
            else
                strEnable = "TRUE";

            config.AppSettings.Settings.Remove("Match_Tab");
            config.AppSettings.Settings.Add("Match_Tab", strEnable);
            AMM_Main.strMatchTab = strEnable;

            nIndex = comboBox_pad.SelectedIndex;
            strEnable = "";

            if (nIndex == 0)
                strEnable = "FALSE";
            else
                strEnable = "TRUE";

            config.AppSettings.Settings.Remove("Number_Pad");
            config.AppSettings.Settings.Add("Number_Pad", strEnable);
            AMM_Main.strNumberPad = strEnable;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

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

        private void button_twrSave_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("저장 하시겠습니끼?", "저장", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            string strTwrName = comboBox_twrNo.Text;
            int nUse = comboBox_twrUse.SelectedIndex;
            string strUse = "";

            if (nUse == 0)
                strUse = "USE";
            else
                strUse = "NO";

            AMM_Main.AMM.Set_Twr_Use(strTwrName, strUse);
        }

        private void comboBox_twrNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strTwrName = comboBox_twrNo.Text;
            string strUse = AMM_Main.AMM.Get_Twr_Use(strTwrName);

            if(strUse == "USE")
            {
                comboBox_twrUse.SelectedIndex = 0;
            }
            else
                comboBox_twrUse.SelectedIndex = 1;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DefaultSaftyTime = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
        }

        private void Form_Set_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Properties.Settings.Default.DefaultSaftyTime;
        }

        private void Form_Set_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
