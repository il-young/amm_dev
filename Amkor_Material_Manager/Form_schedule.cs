using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amkor_Material_Manager
{
    public partial class Form_schedule : Form
    {
        string sDate = "";
        string sTime = "";
        string sUse = "";
        string sVal = "";
        string sInterval = "";


        public Form_schedule()
        {
            InitializeComponent();
        }

        public Form_schedule(string date, string time, string Interval, string val, string use)
        {
            sDate = date;
            sTime = time;
            sInterval = Interval;
            sVal = val;
            sUse = use;

            

            InitializeComponent();
        }

        private void Form_schedule_Load(object sender, EventArgs e)
        {
            dateTimePicker2.Format = DateTimePickerFormat.Time;
            dateTimePicker2.ShowUpDown = true;
            groupBox1.Enabled = sUse == "1" ? true : false;
            checkBox1.Checked = sUse == "1" ? true : false;

            comboBox1.SelectedIndex = 0;
            //if (sInterval == "일")
            //{
            //    comboBox1.SelectedIndex = 0;
            //}
            //else if(sInterval == "주")
            //{
            //    comboBox1.SelectedIndex = 1;
            //}
            //else if(sInterval == "월")
            //{
            //    comboBox1.SelectedIndex = 2;
            //}
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select_index = comboBox1.SelectedIndex;
            domainUpDown1.Items.Clear();
            //domainUpDown1.SelectedItem = sVal;
            

            dateTimePicker2.Enabled = true;
            dateTimePicker2.Value = Convert.ToDateTime(sTime);

            if (select_index == 0) // 일
            {
                for (int i = 1; i < 28; i++)
                {
                    domainUpDown1.Items.Add(i.ToString());
                }

            
                domainUpDown1.Enabled = true;
                domainUpDown1.SelectedIndex = int.Parse(sVal);
                domainUpDown1.Text = domainUpDown1.Items[int.Parse(sVal)].ToString();
            }
            else if(select_index == 1)  // 주
            {
                domainUpDown1.Items.Add("월");
                domainUpDown1.Items.Add("화");
                domainUpDown1.Items.Add("수");
                domainUpDown1.Items.Add("목");
                domainUpDown1.Items.Add("금");
                domainUpDown1.Items.Add("토");
                domainUpDown1.Items.Add("일");

            
                domainUpDown1.Enabled = true;

                if(int.Parse(sVal) < 0)
                {
                    sVal = "0";
                }
                else if(int.Parse(sVal) > 6)
                {
                    sVal = "6";
                }

                domainUpDown1.SelectedIndex = int.Parse(sVal);
                domainUpDown1.Text = domainUpDown1.Items[int.Parse(sVal)].ToString();
            }
            else if(select_index == 2)  // 월
            {
                domainUpDown1.Enabled = false;            
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = checkBox1.Checked;
            sUse = checkBox1.Checked == true ? "1" : "0";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("저장 하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                sTime = dateTimePicker2.Value.ToString("HH:mm");

                if(comboBox1.SelectedIndex == 0 )
                {
                    sInterval = "일";
                    sVal = domainUpDown1.SelectedIndex.ToString();
                }
                else if(comboBox1.SelectedIndex == 1)
                {
                    sInterval = "주";
                    sVal = domainUpDown1.SelectedIndex.ToString();
                }

                string sql = string.Format("update TB_AUTO_SYNC set UPDATE_DATE='{0}', UPDATE_TIME='{1}', UPDATE_INTERVAL='{2}', UPDATE_VAL='{3}', UPDATE_USE='{4}' where UPDATE_NO=1",
                sDate, sTime, sInterval, sVal, sUse);
                int res = AMM_Main.AMM.WriteAutoSync(sql);

                if (res == 0)
                    MessageBox.Show("저장에 실패 했습니다." + System.Environment.NewLine + "재시도 하세요.", "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    Close();              
            }
        }
    }
}
