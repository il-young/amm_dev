﻿using System;
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
    public partial class Form_Monitor : Form
    {
        System.Diagnostics.Stopwatch sw_AliveTime_1 = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sw_AliveTime_2 = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sw_AliveTime_3 = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sw_AliveTime_4 = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sw_AliveTime_5 = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sw_AliveTime_6 = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch sw_AliveTime_7 = new System.Diagnostics.Stopwatch(); //210823_Sangik.choi_타워그룹추가

        //[210823_Sangik.choi_타워그룹추가
/*        bool[] bStopwatch_Alve = { false, false, false, false, false, false };
        bool[] bAliveAlarm = { false, false, false, false, false, false};
        int[] nAlive = { -1, -1, -1, -1, -1, -1 };
        string[] strAlive = { "", "", "", "", "", ""};*/

        bool[] bStopwatch_Alve = { false, false, false, false, false, false ,false };
        bool[] bAliveAlarm = { false, false, false, false, false, false, false };

        int[] nAlive = { -1, -1, -1, -1, -1, -1 , -1};
        string[] strAlive = { "", "", "", "", "", "" , ""};
        //]210823_Sangik.choi_타워그룹추가

        public Form_Monitor()
        {
            InitializeComponent();           
        }

        public void Fnc_Init()
        {
            Fnc_GetStatus();
            timer1.Start();
        }

        public void Fnc_Status_Load(string strGroup, string strSid, string strUid, string strType)
        {
            if (!strGroup.Contains("TWR"))
                return;

            int nGroup = Int32.Parse(strGroup.Substring(3, strGroup.Length - 3));
            AMM_Main.bTAlarm[nGroup - 1] = false;

            if (strType == "LOAD")
               strType = "CART";

            if (strGroup == "TWR1")
            {
                label_Status_G1.BackColor = Color.DarkGreen;
                label_Status_G1.ForeColor = Color.White;    
                if (strType == "LOAD")
                    label_Status_G1.Text = "LOAD";
                else
                    label_Status_G1.Text = "LOAD-"+ strType;                

                label_Info_G1.Text = "SID: " + strSid + "\n" + "UID: " + strUid;
            }
            else if(strGroup == "TWR2")
            {
                label_Status_G2.BackColor = Color.DarkGreen;
                label_Status_G2.ForeColor = Color.White;
                if (strType == "LOAD")
                    label_Status_G2.Text = "LOAD";
                else
                    label_Status_G2.Text = "LOAD-" + strType;

                label_Info_G2.Text = "SID: " + strSid + "\n" + "UID: " + strUid;
            }
            else if (strGroup == "TWR3")
            {
                label_Status_G3.BackColor = Color.DarkGreen;
                label_Status_G3.ForeColor = Color.White;
                if (strType == "LOAD")
                    label_Status_G3.Text = "LOAD";
                else
                    label_Status_G3.Text = "LOAD-" + strType;

                label_Info_G3.Text = "SID: " + strSid + "\n" + "UID: " + strUid;
            }
            else if (strGroup == "TWR4")
            {
                label_Status_G4.BackColor = Color.DarkGreen;
                label_Status_G4.ForeColor = Color.White;
                if (strType == "LOAD")
                    label_Status_G4.Text = "LOAD";
                else
                    label_Status_G4.Text = "LOAD-" + strType;

                label_Info_G4.Text = "SID: " + strSid + "\n" + "UID: " + strUid;
            }
            else if (strGroup == "TWR5")
            {
                label_Status_G5.BackColor = Color.DarkGreen;
                label_Status_G5.ForeColor = Color.White;
                if (strType == "LOAD")
                    label_Status_G5.Text = "LOAD";
                else
                    label_Status_G5.Text = "LOAD-" + strType;

                label_Info_G5.Text = "SID: " + strSid + "\n" + "UID: " + strUid;
            }
            else if (strGroup == "TWR6")
            {
                label_Status_G6.BackColor = Color.DarkGreen;
                label_Status_G6.ForeColor = Color.White;
                if (strType == "LOAD")
                    label_Status_G6.Text = "LOAD";
                else
                    label_Status_G6.Text = "LOAD-" + strType;

                label_Info_G6.Text = "SID: " + strSid + "\n" + "UID: " + strUid;
            }
            //[210823_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                label_Status_G7.BackColor = Color.DarkGreen;
                label_Status_G7.ForeColor = Color.White;
                if (strType == "LOAD")
                    label_Status_G7.Text = "LOAD";
                else
                    label_Status_G7.Text = "LOAD-" + strType;

                label_Info_G7.Text = "SID: " + strSid + "\n" + "UID: " + strUid;
            }        
            //]210823_Sangik.choi_타워그룹추가

        }

        public void Fnc_Status_Unload(string strGroup, string strRequestor, string strPickid, string strUid)
        {
            if (!strGroup.Contains("TWR"))
                return;

            int nGroup = Int32.Parse(strGroup.Substring(3, strGroup.Length - 3));
            AMM_Main.bTAlarm[nGroup - 1] = false;

            if (strGroup == "TWR1")
            {
                label_Status_G1.BackColor = Color.DarkGreen;
                label_Status_G1.ForeColor = Color.White;
                label_Status_G1.Text = "UNLOAD";
                //label_Info_G1.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid + "\n" + "UID: " + strUid;
                label_Info_G1.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid;
            }
            else if (strGroup == "TWR2")
            {
                label_Status_G2.BackColor = Color.DarkGreen;
                label_Status_G2.ForeColor = Color.White;
                label_Status_G2.Text = "UNLOAD";
                //label_Info_G2.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid + "\n" + "UID: " + strUid;
                label_Info_G2.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid;
            }
            else if (strGroup == "TWR3")
            {
                label_Status_G3.BackColor = Color.DarkGreen;
                label_Status_G3.ForeColor = Color.White;
                label_Status_G3.Text = "UNLOAD";
                //label_Info_G3.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid + "\n" + "UID: " + strUid;
                label_Info_G3.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid;
            }
            else if (strGroup == "TWR4")
            {
                label_Status_G4.BackColor = Color.DarkGreen;
                label_Status_G4.ForeColor = Color.White;
                label_Status_G4.Text = "UNLOAD";
                //label_Info_G4.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid + "\n" + "UID: " + strUid;
                label_Info_G4.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid;
            }
            else if (strGroup == "TWR5")
            {
                label_Status_G5.BackColor = Color.DarkGreen;
                label_Status_G5.ForeColor = Color.White;
                label_Status_G5.Text = "UNLOAD";
                //label_Info_G5.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid + "\n" + "UID: " + strUid;
                label_Info_G5.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid;
            }
            else if (strGroup == "TWR6")
            {
                label_Status_G6.BackColor = Color.DarkGreen;
                label_Status_G6.ForeColor = Color.White;
                label_Status_G6.Text = "UNLOAD";
                //label_Info_G6.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid + "\n" + "UID: " + strUid;
                label_Info_G6.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid;
            }

            //[210823_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                label_Status_G7.BackColor = Color.DarkGreen;
                label_Status_G7.ForeColor = Color.White;
                label_Status_G7.Text = "UNLOAD";
                //label_Info_G6.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid + "\n" + "UID: " + strUid;
                label_Info_G7.Text = "REQ: " + strRequestor + "\n" + "ID: " + strPickid;
            }
            //]210823_Sangik.choi_타워그룹추가

        }
        public void Fnc_Status_Idle(string strGroup)
        {
            if (!strGroup.Contains("TWR"))
                return;

            int nGroup = Int32.Parse(strGroup.Substring(3, strGroup.Length - 3));
            AMM_Main.bTAlarm[nGroup - 1] = false;

            if (strGroup == "TWR1")
            {
                label_Status_G1.BackColor = Color.FromArgb(150, 150, 150);
                label_Status_G1.ForeColor = Color.White;
                label_Status_G1.Text = "IDLE";
                label_Info_G1.Text = "";
            }
            else if (strGroup == "TWR2")
            {
                label_Status_G2.BackColor = Color.FromArgb(150, 150, 150);
                label_Status_G2.ForeColor = Color.White;
                label_Status_G2.Text = "IDLE";
                label_Info_G2.Text = "";
            }
            else if (strGroup == "TWR3")
            {
                label_Status_G3.BackColor = Color.FromArgb(150, 150, 150);
                label_Status_G3.ForeColor = Color.White;
                label_Status_G3.Text = "IDLE";
                label_Info_G3.Text = "";
            }
            else if (strGroup == "TWR4")
            {
                label_Status_G4.BackColor = Color.FromArgb(150, 150, 150);
                label_Status_G4.ForeColor = Color.White;
                label_Status_G4.Text = "IDLE";
                label_Info_G4.Text = "";
            }
            else if (strGroup == "TWR5")
            {
                label_Status_G5.BackColor = Color.FromArgb(150, 150, 150);
                label_Status_G5.ForeColor = Color.White;
                label_Status_G5.Text = "IDLE";
                label_Info_G5.Text = "";
            }
            else if (strGroup == "TWR6")
            {
                label_Status_G6.BackColor = Color.FromArgb(150, 150, 150);
                label_Status_G6.ForeColor = Color.White;
                label_Status_G6.Text = "IDLE";
                label_Info_G6.Text = "";
            }

            //[210823_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                label_Status_G7.BackColor = Color.FromArgb(150, 150, 150);
                label_Status_G7.ForeColor = Color.White;
                label_Status_G7.Text = "IDLE";
                label_Info_G7.Text = "";
            }
            //]210823_Sangik.choi_타워그룹추가

        }
        public void Fnc_Status_Ready(string strGroup)
        {
            if (!strGroup.Contains("TWR"))
                return;

            int nGroup = Int32.Parse(strGroup.Substring(3, strGroup.Length - 3));
            AMM_Main.bTAlarm[nGroup - 1] = false;

            if (strGroup == "TWR1")
            {
                label_Status_G1.BackColor = Color.FromArgb(225, 225, 225);
                label_Status_G1.ForeColor = Color.Black;
                label_Status_G1.Text = "READY";
                label_Info_G1.Text = "";
            }
            else if (strGroup == "TWR2")
            {
                label_Status_G2.BackColor = Color.FromArgb(225, 225, 225);
                label_Status_G2.ForeColor = Color.Black;
                label_Status_G2.Text = "READY";
                label_Info_G2.Text = "";
            }
            else if (strGroup == "TWR3")
            {
                label_Status_G3.BackColor = Color.FromArgb(225, 225, 225);
                label_Status_G3.ForeColor = Color.Black;
                label_Status_G3.Text = "READY";
                label_Info_G3.Text = "";
            }
            else if (strGroup == "TWR4")
            {
                label_Status_G4.BackColor = Color.FromArgb(225, 225, 225);
                label_Status_G4.ForeColor = Color.Black;
                label_Status_G4.Text = "READY";
                label_Info_G4.Text = "";
            }
            else if (strGroup == "TWR5")
            {
                label_Status_G5.BackColor = Color.FromArgb(225, 225, 225);
                label_Status_G5.ForeColor = Color.Black;
                label_Status_G5.Text = "READY";
                label_Info_G5.Text = "";
            }
            else if (strGroup == "TWR6")
            {
                label_Status_G6.BackColor = Color.FromArgb(225, 225, 225);
                label_Status_G6.ForeColor = Color.Black;
                label_Status_G6.Text = "READY";
                label_Info_G6.Text = "";
            }

            //[210823_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                label_Status_G7.BackColor = Color.FromArgb(225, 225, 225);
                label_Status_G7.ForeColor = Color.Black;
                label_Status_G7.Text = "READY";
                label_Info_G7.Text = "";
            }
            //]210823_Sangik.choi_타워그룹추가

        }
        public void Fnc_Status_Alarm(string strGroup)
        {
            if (!strGroup.Contains("TWR"))
                return;

            int nGroup = Int32.Parse(strGroup.Substring(3, strGroup.Length - 3));
            AMM_Main.bTAlarm[nGroup - 1] = true;

            if (strGroup == "TWR1")
            {
                label_Status_G1.BackColor = Color.Red;
                label_Status_G1.ForeColor = Color.White;
                label_Status_G1.Text = "ALARM";
                label_Info_G1.Text = "";
            }
            else if (strGroup == "TWR2")
            {
                label_Status_G2.BackColor = Color.Red;
                label_Status_G2.ForeColor = Color.White;
                label_Status_G2.Text = "ALARM";
                label_Info_G2.Text = "";
            }
            else if (strGroup == "TWR3")
            {
                label_Status_G3.BackColor = Color.Red;
                label_Status_G3.ForeColor = Color.White;
                label_Status_G3.Text = "ALARM";
                label_Info_G3.Text = "";
            }
            else if (strGroup == "TWR4")
            {
                label_Status_G4.BackColor = Color.Red;
                label_Status_G4.ForeColor = Color.White;
                label_Status_G4.Text = "ALARM";
                label_Info_G4.Text = "";
            }
            else if (strGroup == "TWR5")
            {
                label_Status_G5.BackColor = Color.Red;
                label_Status_G5.ForeColor = Color.White;
                label_Status_G5.Text = "ALARM";
                label_Info_G5.Text = "";
            }
            else if (strGroup == "TWR6")
            {
                label_Status_G6.BackColor = Color.Red;
                label_Status_G6.ForeColor = Color.White;
                label_Status_G6.Text = "ALARM";
                label_Info_G6.Text = "";
            }

            //[210823_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                label_Status_G7.BackColor = Color.Red;
                label_Status_G7.ForeColor = Color.White;
                label_Status_G7.Text = "ALARM";
                label_Info_G7.Text = "";
            }
            //]210823_Sangik.choi_타워그룹추가

        }

        public void Fnc_Status_Stop(string strGroup)
        {
            if (!strGroup.Contains("TWR"))
                return;

            int nGroup = Int32.Parse(strGroup.Substring(3, strGroup.Length - 3));
            AMM_Main.bTAlarm[nGroup - 1] = true;

            if (strGroup == "TWR1")
            {
                label_Status_G1.BackColor = Color.Red;
                label_Status_G1.ForeColor = Color.White;
                label_Status_G1.Text = "ALARM";
                label_Info_G1.Text = "";
            }
            else if (strGroup == "TWR2")
            {
                label_Status_G2.BackColor = Color.Red;
                label_Status_G2.ForeColor = Color.White;
                label_Status_G2.Text = "ALARM";
                label_Info_G2.Text = "";
            }
            else if (strGroup == "TWR3")
            {
                label_Status_G3.BackColor = Color.Red;
                label_Status_G3.ForeColor = Color.White;
                label_Status_G3.Text = "ALARM";
                label_Info_G3.Text = "";
            }
            else if (strGroup == "TWR4")
            {
                label_Status_G4.BackColor = Color.Red;
                label_Status_G4.ForeColor = Color.White;
                label_Status_G4.Text = "ALARM";
                label_Info_G4.Text = "";
            }
            else if (strGroup == "TWR5")
            {
                label_Status_G5.BackColor = Color.Red;
                label_Status_G5.ForeColor = Color.White;
                label_Status_G5.Text = "ALARM";
                label_Info_G5.Text = "";
            }
            else if (strGroup == "TWR6")
            {
                label_Status_G6.BackColor = Color.Red;
                label_Status_G6.ForeColor = Color.White;
                label_Status_G6.Text = "ALARM";
                label_Info_G6.Text = "";
            }

            //[210823_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                label_Status_G7.BackColor = Color.Red;
                label_Status_G7.ForeColor = Color.White;
                label_Status_G7.Text = "ALARM";
                label_Info_G7.Text = "";
            }
            //]210823_Sangik.choi_타워그룹추가

        }

        public void Fnc_Status_Noinfo(string strGroup)
        {
            if (strGroup == "TWR1")
            {
                label_Status_G1.BackColor = Color.White;
                label_Status_G1.ForeColor = Color.Black;
                label_Status_G1.Text = "Not connected";
                label_Info_G1.Text = "";
            }
            else if (strGroup == "TWR2")
            {
                label_Status_G2.BackColor = Color.White;
                label_Status_G2.ForeColor = Color.Black;
                label_Status_G2.Text = "Not connected";
                label_Info_G2.Text = "";
            }
            else if (strGroup == "TWR3")
            {
                label_Status_G3.BackColor = Color.White;
                label_Status_G3.ForeColor = Color.Black;
                label_Status_G3.Text = "Not connected";
                label_Info_G3.Text = "";
            }
            else if (strGroup == "TWR4")
            {
                label_Status_G4.BackColor = Color.White;
                label_Status_G4.ForeColor = Color.Black;
                label_Status_G4.Text = "Not connected";
                label_Info_G4.Text = "";
            }
            else if (strGroup == "TWR5")
            {
                label_Status_G5.BackColor = Color.White;
                label_Status_G5.ForeColor = Color.Black;
                label_Status_G5.Text = "Not connected";
                label_Info_G5.Text = "";
            }
            else if (strGroup == "TWR6")
            {
                label_Status_G6.BackColor = Color.White;
                label_Status_G6.ForeColor = Color.Black;
                label_Status_G6.Text = "Not connected";
                label_Info_G6.Text = "";
            }

            //[210824_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                label_Status_G7.BackColor = Color.White;
                label_Status_G7.ForeColor = Color.Black;
                label_Status_G7.Text = "Not connected";
                label_Info_G7.Text = "";
            }
            //]210824_Sangik.choi_타워그룹추가

        }
        public void Fnc_Status_Alive(string strGroup, int nValue)
        {
            if (strGroup == "TWR1")
            {
                if (!bAliveAlarm[0])
                {
                    if (nValue == 0)
                        label_alive1.BackColor = Color.FromArgb(0, 190, 0);
                    else
                        label_alive1.BackColor = Color.FromArgb(0, 190, 0);
                }
                else
                    Fnc_Status_Alarm(strGroup);

                strAlive[0] = nValue.ToString();

                if (strAlive[0] == "0")
                {
                    if (nAlive[0] == 1)
                    {
                        bStopwatch_Alve[0] = false;
                        sw_AliveTime_1.Stop();
                        sw_AliveTime_1.Reset();

                        bAliveAlarm[0] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[0])
                        {
                            sw_AliveTime_1.Start();
                            bStopwatch_Alve[0] = true;
                            nAlive[0] = 0;
                        }
                    }
                }
                else if (strAlive[0] == "1")
                {
                    if (nAlive[0] == 0)
                    {
                        bStopwatch_Alve[0] = false;
                        sw_AliveTime_1.Stop();
                        sw_AliveTime_1.Reset();
                        bAliveAlarm[0] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[0])
                        {
                            sw_AliveTime_1.Start();
                            bStopwatch_Alve[0] = true;
                            nAlive[0] = 1;
                        }
                    }
                }
            }
            else if (strGroup == "TWR2")
            {
                if (!bAliveAlarm[1])
                {
                    if (nValue == 0)
                        label_alive2.BackColor = Color.FromArgb(0, 190, 0);
                    else
                        label_alive2.BackColor = Color.FromArgb(0, 190, 0);
                }
                else
                    Fnc_Status_Alarm(strGroup);

                strAlive[1] = nValue.ToString();

                if (strAlive[1] == "0")
                {
                    if (nAlive[1] == 1)
                    {
                        bStopwatch_Alve[1] = false;
                        sw_AliveTime_2.Stop();
                        sw_AliveTime_2.Reset();

                        bAliveAlarm[1] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[1])
                        {
                            sw_AliveTime_2.Start();
                            bStopwatch_Alve[1] = true;
                            nAlive[1] = 0;
                        }
                    }
                }
                else if (strAlive[1] == "1")
                {
                    if (nAlive[1] == 0)
                    {
                        bStopwatch_Alve[1] = false;
                        sw_AliveTime_2.Stop();
                        sw_AliveTime_2.Reset();
                        bAliveAlarm[1] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[1])
                        {
                            sw_AliveTime_2.Start();
                            bStopwatch_Alve[1] = true;
                            nAlive[1] = 1;
                        }
                    }
                }
            }
            else if (strGroup == "TWR3")
            {
                if (!bAliveAlarm[2])
                {
                    if (nValue == 0)
                        label_alive3.BackColor = Color.FromArgb(0, 190, 0);
                    else
                        label_alive3.BackColor = Color.FromArgb(0, 190, 0);
                }
                else
                    Fnc_Status_Alarm(strGroup);

                strAlive[2] = nValue.ToString();

                if (strAlive[2] == "0")
                {
                    if (nAlive[2] == 1)
                    {
                        bStopwatch_Alve[2] = false;
                        sw_AliveTime_3.Stop();
                        sw_AliveTime_3.Reset();

                        bAliveAlarm[2] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[2])
                        {
                            sw_AliveTime_3.Start();
                            bStopwatch_Alve[2] = true;
                            nAlive[2] = 0;
                        }
                    }
                }
                else if (strAlive[2] == "1")
                {
                    if (nAlive[2] == 0)
                    {
                        bStopwatch_Alve[2] = false;
                        sw_AliveTime_3.Stop();
                        sw_AliveTime_3.Reset();
                        bAliveAlarm[2] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[2])
                        {
                            sw_AliveTime_3.Start();
                            bStopwatch_Alve[2] = true;
                            nAlive[2] = 1;
                        }
                    }
                }
            }
            else if (strGroup == "TWR4")
            {
                if (!bAliveAlarm[3])
                {
                    if (nValue == 0)
                        label_alive4.BackColor = Color.FromArgb(0, 190, 0);
                    else
                        label_alive4.BackColor = Color.FromArgb(0, 190, 0);
                }
                else
                    Fnc_Status_Alarm(strGroup);

                strAlive[3] = nValue.ToString();

                if (strAlive[3] == "0")
                {
                    if (nAlive[3] == 1)
                    {
                        bStopwatch_Alve[3] = false;
                        sw_AliveTime_4.Stop();
                        sw_AliveTime_4.Reset();

                        bAliveAlarm[3] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[3])
                        {
                            sw_AliveTime_4.Start();
                            bStopwatch_Alve[3] = true;
                            nAlive[3] = 0;
                        }
                    }
                }
                else if (strAlive[3] == "1")
                {
                    if (nAlive[3] == 0)
                    {
                        bStopwatch_Alve[3] = false;
                        sw_AliveTime_4.Stop();
                        sw_AliveTime_4.Reset();
                        bAliveAlarm[3] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[3])
                        {
                            sw_AliveTime_4.Start();
                            bStopwatch_Alve[3] = true;
                            nAlive[3] = 1;
                        }
                    }
                }
            }
            else if (strGroup == "TWR5")
            {
                if (!bAliveAlarm[4])
                {
                    if (nValue == 0)
                        label_alive5.BackColor = Color.FromArgb(0, 190, 0);
                    else
                        label_alive5.BackColor = Color.FromArgb(0, 190, 0);
                }
                else
                    Fnc_Status_Alarm(strGroup);

                strAlive[4] = nValue.ToString();

                if (strAlive[4] == "0")
                {
                    if (nAlive[4] == 1)
                    {
                        bStopwatch_Alve[4] = false;
                        sw_AliveTime_5.Stop();
                        sw_AliveTime_5.Reset();

                        bAliveAlarm[4] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[4])
                        {
                            sw_AliveTime_5.Start();
                            bStopwatch_Alve[4] = true;
                            nAlive[4] = 0;
                        }
                    }
                }
                else if (strAlive[4] == "1")
                {
                    if (nAlive[4] == 0)
                    {
                        bStopwatch_Alve[4] = false;
                        sw_AliveTime_5.Stop();
                        sw_AliveTime_5.Reset();
                        bAliveAlarm[4] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[4])
                        {
                            sw_AliveTime_5.Start();
                            bStopwatch_Alve[4] = true;
                            nAlive[4] = 1;
                        }
                    }
                }
            }
            else if (strGroup == "TWR6")
            {
                if (!bAliveAlarm[5])
                {
                    if (nValue == 0)
                        label_alive6.BackColor = Color.FromArgb(0, 190, 0);
                    else
                        label_alive6.BackColor = Color.FromArgb(0, 190, 0);
                }
                else
                    Fnc_Status_Alarm(strGroup);

                strAlive[5] = nValue.ToString();

                if (strAlive[5] == "0")
                {
                    if (nAlive[5] == 1)
                    {
                        bStopwatch_Alve[5] = false;
                        sw_AliveTime_6.Stop();
                        sw_AliveTime_6.Reset();

                        bAliveAlarm[5] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[5])
                        {
                            sw_AliveTime_6.Start();
                            bStopwatch_Alve[5] = true;
                            nAlive[5] = 0;
                        }
                    }
                }
                else if (strAlive[5] == "1")
                {
                    if (nAlive[5] == 0)
                    {
                        bStopwatch_Alve[5] = false;
                        sw_AliveTime_6.Stop();
                        sw_AliveTime_6.Reset();
                        bAliveAlarm[5] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[5])
                        {
                            sw_AliveTime_6.Start();
                            bStopwatch_Alve[5] = true;
                            nAlive[5] = 1;
                        }
                    }
                }
            }

            //[210824_Sangik.choi_타워그룹추가
            else if (strGroup == "TWR7")
            {
                if (!bAliveAlarm[6])
                {
                    if (nValue == 0)
                        label_alive7.BackColor = Color.FromArgb(0, 190, 0); //210915_Sangik.Choi_색표기 오류로 수정하였음.
                    else
                        label_alive7.BackColor = Color.FromArgb(0, 190, 0);
                }
                else
                    Fnc_Status_Alarm(strGroup);

                strAlive[6] = nValue.ToString();

                if (strAlive[6] == "0")
                {
                    if (nAlive[6] == 1)
                    {
                        bStopwatch_Alve[6] = false;
                        sw_AliveTime_7.Stop();
                        sw_AliveTime_7.Reset();

                        bAliveAlarm[6] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[6])
                        {
                            sw_AliveTime_7.Start();
                            bStopwatch_Alve[6] = true;
                            nAlive[6] = 0;
                        }
                    }
                }
                else if (strAlive[6] == "1")
                {
                    if (nAlive[6] == 0)
                    {
                        bStopwatch_Alve[6] = false;
                        sw_AliveTime_7.Stop();
                        sw_AliveTime_7.Reset();
                        bAliveAlarm[6] = false;
                    }
                    else
                    {
                        if (!bStopwatch_Alve[6])
                        {
                            sw_AliveTime_7.Start();
                            bStopwatch_Alve[6] = true;
                            nAlive[6] = 1;
                        }
                    }
                }
            }
            //]210824_Sangik.choi_타워그룹추가

        }

        public void Fnc_Close()
        {
            timer1.Stop();
        }

        public void Fnc_GetStatus()
        {
            //for (int n = 1; n < 7; n++) //210824_Sangik.choi_타워그룹추가


            for (int n = 1; n < 8; n++)
            {
                // GetStatus - query = string.Format(@"SELECT * FROM TB_STATUS WITH (NOLOCK) WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);
                string strGroup = "TWR" + n.ToString();
                DataTable dt_Status = AMM_Main.AMM.GetStatus(AMM_Main.strDefault_linecode, strGroup);
                int ncount = dt_Status.Rows.Count;
                if (ncount == 0)
                    Fnc_Status_Noinfo(strGroup);
                else
                {
                    string strStatus = dt_Status.Rows[0]["STATUS"].ToString(); strStatus = strStatus.Trim();
                    string strType = dt_Status.Rows[0]["TYPE"].ToString(); strType = strType.Trim();
                    string strAlive = dt_Status.Rows[0]["ALIVE"].ToString(); strAlive = strAlive.Trim();

                    if (strGroup == "TWR4" && (strAlive == null || strAlive == ""))  // TWR4 ??? 
                        strAlive = "0";

                    if (strAlive != null && strAlive != "")
                        Fnc_Status_Alive(strGroup, Int32.Parse(strAlive));

                    if (strStatus == "IDLE")
                    {
                        Fnc_Status_Idle(strGroup);
                    }
                    else if (strStatus == "READY")
                    {
                        Fnc_Status_Ready(strGroup);
                    }
                    else if (strStatus == "ALARM")
                    {
                        Fnc_Status_Alarm(strGroup);
                    }
                    else if (strStatus == "STOP")
                    {
                        Fnc_Status_Stop(strGroup);
                    }
                    else if (strStatus == "RUN")
                    {
                        if (strType == "LOAD" || strType == "RETURN" || strType == "CART")
                        {
                            string strSid = dt_Status.Rows[0]["DEPARTURE"].ToString(); strSid = strSid.Trim();
                            string strUid = dt_Status.Rows[0]["ARRIVAL"].ToString(); strUid = strUid.Trim();
                            Fnc_Status_Load(strGroup, strSid, strUid, strType);
                        }
                        else if (strType == "UNLOAD")
                        {
                            string strPickid = dt_Status.Rows[0]["DEPARTURE"].ToString(); strPickid = strPickid.Trim();
                            string strUid = dt_Status.Rows[0]["ARRIVAL"].ToString(); strUid = strUid.Trim();

                            string strRequest = AMM_Main.AMM.GetPickingID_Pickid(strPickid); strRequest = strRequest.Trim();
                            string strName = AMM_Main.AMM.User_check(strRequest); strName = strName.Trim();

                            //strName = strName + " (" + strRequest + ")";                           

                            Fnc_Status_Unload(strGroup, strName, strPickid, strUid);
                        }
                        else if (strType == "READY" || strType == "COMPLETE")
                        {
                            Fnc_Status_Ready(strGroup);
                        }
                    }
                    else
                    {
                        Fnc_Status_Noinfo(strGroup);
                    }
                }
            }
            Fnc_Alivecheck();
        }
        
        public void Fnc_Alivecheck()
        {

            if (sw_AliveTime_1.ElapsedMilliseconds > 3 * 60 * 1000) //10분, 600초  ==> 옵션 입력 추가
             {
                if (!bAliveAlarm[0])
                {
                    label_alive1.BackColor = System.Drawing.Color.Red;
                    bAliveAlarm[0] = true;                    
                }
            }

            if (sw_AliveTime_2.ElapsedMilliseconds > 3 * 60 * 1000) //10분, 600초  ==> 옵션 입력 추가
            {
                if (!bAliveAlarm[1])
                {
                    label_alive2.BackColor = System.Drawing.Color.Red;

                    bAliveAlarm[1] = true;
                }
            }

            if (sw_AliveTime_3.ElapsedMilliseconds > 3 * 60 * 1000) //10분, 600초  ==> 옵션 입력 추가
            {
                if (!bAliveAlarm[2])
                {
                    label_alive3.BackColor = System.Drawing.Color.Red;

                    bAliveAlarm[2] = true;
                }
            }

            if (sw_AliveTime_4.ElapsedMilliseconds > 3 * 60 * 1000) //10분, 600초  ==> 옵션 입력 추가
            {
                if (!bAliveAlarm[3])
                {
                    label_alive4.BackColor = System.Drawing.Color.Red;

                    bAliveAlarm[3] = true;
                }
            }

            if (sw_AliveTime_5.ElapsedMilliseconds > 3 * 60 * 1000) //10분, 600초  ==> 옵션 입력 추가
            {
                if (!bAliveAlarm[4])
                {
                    label_alive5.BackColor = System.Drawing.Color.Red;

                    bAliveAlarm[4] = true;
                }
            }

            if (sw_AliveTime_6.ElapsedMilliseconds > 3 * 60 * 1000) //10분, 600초  ==> 옵션 입력 추가
            {
                if (!bAliveAlarm[5])
                {
                    label_alive6.BackColor = System.Drawing.Color.Red;

                    bAliveAlarm[5] = true;
                }
            }
            //[210824_Sangik.choi_타워그룹추가
            if (sw_AliveTime_7.ElapsedMilliseconds > 3 * 60 * 1000) //10분, 600초  ==> 옵션 입력 추가
            {
                if (!bAliveAlarm[6])
                {
                    label_alive7.BackColor = System.Drawing.Color.Red;

                    bAliveAlarm[6] = true;
                }
            }
            //]210824_Sangik.choi_타워그룹추가

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Fnc_GetStatus();
        }        
    }
}
