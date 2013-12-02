using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace LCDDisplay
{
    public partial class Form1 : Form
    {
        private int count;
        private DateTime ti;
        private rssService myrss = new rssService("http://www.nytimes.com/services/xml/rss/nyt/GlobalHome.xml", 10);
        private timerState timer1State;
        private int newsNum;

        private enum timerState
        {
            hello,
            news,
            time
        }

        public Form1()
        {
            InitializeComponent();
            try
            {
                serialPort1.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message+"\n程序退出。");              
                Environment.Exit(0);
            }
            
            timer1.Enabled = false;
            timer1.Interval = 300;
            //timer2.Enabled = false;
            //timer2.Interval = 1000;
            count = 0;
            newsNum = 0;
            resetDisplay();
           // writeString(0, 5, "WELCOME!");
            displayOneByOne(0, 5, "WELCOME!", 40);
            displayOneByOne(1, 6, "cgsv!", 40);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            resetDisplay();
            //timer2.Stop();
            timer1.Interval = 300;
            timer1.Start();
            timer1State = timerState.hello;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                timer1.Enabled = false;
                //timer2.Enabled = false;
                resetDisplay();
            //    writeString(0, 5, "GOODBYE!");
                displayOneByOne(0, 5, "GOODBYE!", 40);
                serialPort1.Close();
            }
        }

        private void resetDisplay()
        {
            byte[] cmd = { 0x1F };
            serialPort1.Write(cmd, 0, 1);
        }

        private void setCursor(int row,int column)
        {
            if (row==0)
            {
                byte[] cur={0x10,(byte)(column+1)};
                serialPort1.Write(cur, 0, 2);
            }
            else if (row==1)
            {
                byte[] cur1 = { 0x10, (byte)(column + 21) };
                serialPort1.Write(cur1, 0, 2);
            }
        }

        private void writeString(int row,int column,string str)
        {
            
            if (column<0)
            {
                setCursor(row, 0);
                serialPort1.Write(str.Substring(-column));
                return;
            }
            setCursor(row, column);
            serialPort1.Write(str);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (timer1State)
            {
                case timerState.hello:
                    resetDisplay();
                    string s = "hello, world";
                    int num=s.Length<(20-count)?s.Length:(19-count);
                    writeString(0, count, s.Substring(0,num));
                    count++;
                    if (count == 20)
                    {
                        count = -s.Length + 1;
                    }
            	break;
                case timerState.time:
                    ti = DateTime.Now;
                    writeString(0, 0, ti.ToString());
                    if(ti.Second%10==0)
                    {
                        writeString(1, 0, " GOOD GOOD STUDY ");
                    }else if(ti.Second%5==0)
                    {
                        writeString(1, 0, "   DAY DAY UP!   ");
                    }
                break;
                case timerState.news:
                    showNews(newsNum++);
                break;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resetDisplay();
            //timer1.Stop();
            timer1.Interval = 1000;
            timer1.Start();
            timer1State = timerState.time;
        }

/*
        private void timer2_Tick(object sender, EventArgs e)
        {
            / *
            ti = DateTime.Now;
                        writeString(0, 0, ti.ToString());
                        if(ti.Second%10==0)
                        {
                            writeString(1, 0, " GOOD GOOD STUDY ");
                        }else if(ti.Second%5==0)
                        {
                            writeString(1, 0, "   DAY DAY UP!   ");
                        }* /
            
        }*/


        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
           // timer2.Stop();
            resetDisplay();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                MessageBox.Show("No text");
                return;
            }
            serialPort1.Write(textBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte t;
            try
            {
                t = byte.Parse(textBox2.Text);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            byte[] cmd = { t };
            serialPort1.Write(cmd, 0, 1);
        }

        private void displayOneByOne(int row,int column,string str,int inteval)
        {
            setCursor(row, column);
            int n = str.Length;
            for (int i = 0; i < n; i++ )
            {
                serialPort1.Write(str.Substring(i, 1));
                System.Threading.Thread.Sleep(inteval);
                Application.DoEvents();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1_FormClosing(null,null);
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetDisplay();
            timer1.Stop();
           // timer2.Stop();
            setCursor(0, -1);
            writeLongString(Properties.Resources.longStr, 2000);
        }

        private void writeLongString(string str,int inteval)
        {
            int i = 0;
            int len = str.Length;
            int count = len / 39;
            for (i = 0; i < count;i++ )
            {
                setCursor(0, -1);
                serialPort1.Write(str.Substring(i * 39, 39));
                System.Threading.Thread.Sleep(inteval);
            }
            setCursor(0, -1);
            resetDisplay();
            serialPort1.Write(str.Substring(i * 39));
        }

        private void showNews(int num)
        {
            ArrayList newsList=myrss.getRssFeed();
            int n = newsList.Count;
            /*
            for (int i = 0; i < n;i++ )
                        {
                            resetDisplay();
                            writeLongString((string)newsList[i], 2000);
                            while (!Delay(6)) ;
                        }*/
            resetDisplay();
            string str=(string)newsList[num % n];
            writeString(0,0,str.Substring(0,38));
            if (str.Length>38)
            {
                Delay(3);
                resetDisplay();
                writeString(0,0,str.Substring(38));
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            resetDisplay();
            writeString(0, 0, "Getting the news...  Please wait...");
          //  timer1.Stop();
            timer1.Interval = 6000;
            timer1State = timerState.news;
            timer1.Start();
           // showNews();
        }

        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }

        private void changeRSSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rssUrl urlForm = new rssUrl();
            urlForm.ShowDialog();
            if (!urlForm.url.Equals(""))
            {
                myrss.rssUrl = urlForm.url;
            }
        }

    }
}
