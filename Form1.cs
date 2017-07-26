using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;

namespace fileProcessWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public static List<double> CH4 = new List<double>();//第8列 甲烷
        public static List<double> voltage = new List<double>();//第10列 电压
        public static List<double> humidity = new List<double>();//第11列 湿度
        public static List<double> temperature = new List<double>();//第12列 温度
        public static String THHI_GAS_ID;//设备ID
        public static String directoryName;//要解析的文件夹的名称

        public static List<string> ymd_deviceStart = new List<string>();//设备启动年月日
        public static List<string> hms_deviceStart = new List<string>();//设备启动时分秒
        public static List<DateTime> time_deviceStart = new List<DateTime>();//存储设备启动时间
        private void button1_Click(object sender, EventArgs e)
        {
            //点击弹出对话框
            OpenFileDialog ofd = new OpenFileDialog();
            //设置对话框的标题
            ofd.Title = "请选择要处理的文本文件";
            //设置对话框可以多选
            ofd.Multiselect = true;
            //设置对话框的初始目录
            ofd.InitialDirectory = @"C:\Users\Zhang\Desktop";
            //设置对话框的文件类型
            ofd.Filter = "处理文件|*.dat|所有文件|*.*";
            //展示对话框
            //ofd.ShowDialog();


            //如果用户点击了选择文件按钮
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //获得在打开对话框中选中文件的路径
                string path = ofd.FileName;
                THHI_GAS_ID = Path.GetFileNameWithoutExtension(path).Substring(17, 3);
                //label1.Text = Path.GetFileNameWithoutExtension(path);
                //获取处理文件所在的文件夹的名称
                directoryName = Path.GetDirectoryName(path);
                directoryName = directoryName.Substring(directoryName.LastIndexOf('\\') + 1);

                //处理文件，取出相应的数据
                processDirectory(path);

            }
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 绘图选择，分别绘制四幅图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    initChart(temperature, "温度");
                    //温度有数据，才获取温度最大值
                    if (temperature.Count != 0)
                    {
                        double temMax = getTemMax(temperature);
                        textBox1.Text = temMax.ToString();
                    }
                    break;
                case 1:
                    //清空温度数据
                    if (textBox1.Text != null)
                    {
                        textBox1.Text = null;
                    }
                    initChart(humidity, "湿度");
                    break;
                case 2:
                    if (textBox1.Text != null)
                    {
                        textBox1.Text = null;
                    }
                    initChart(voltage, "电压");

                    break;
                case 3:
                    if (textBox1.Text != null)
                    {
                        textBox1.Text = null;
                    }
                    initChart(CH4, "甲烷");
                    break;
                default: break;
            }
            //initChart();
        }

        /// <summary>
        /// 绘制单幅图形
        /// </summary>
        /// <param name="target"></param>
        /// <param name="str"></param>
        private void initChart(List<double> target, string str)
        {
            //判断文件是否进行处理
            if (target.Count == 0)
            {
                MessageBox.Show("请先进行文件处理，再进行绘图操作");
                return;
            }
            this.chart1.Series.Clear();
            this.chart1.ChartAreas.Clear();
            this.chart1.Titles.Clear();
            Series se = new Series(str);

            //设置series格式
            se.ChartType = SeriesChartType.Line;
            se.Color = Color.Cyan;
            se.BorderWidth = 2;
            se.MarkerColor = Color.Black;
            se.MarkerSize = 5;
            se.MarkerStyle = MarkerStyle.Square;
            se.IsValueShownAsLabel = false;
            se.ChartArea = "area1";


            //设置ChartArea格式
            ChartArea area1 = new ChartArea("area1");

            area1.AxisX.Title = "日期";
            area1.AxisY.Title = "波动";
            area1.AxisX.Interval = 12;
            area1.AxisX.ScrollBar.Enabled = true;
            area1.Position.Width = 85;
            area1.Position.Height = 90;
            area1.Position.X = 5;
            area1.Position.Y = 10;
            area1.AxisY.Minimum = -1;
            area1.AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm:ss";

            //area1.AxisY.Maximum = 100;
            //area1.AxisY.MaximumAutoSize = 20;

            //设置标题
            string titleStr = "设备 " + THHI_GAS_ID + " 在日期 " + directoryName + " 时的数据信息";
            Title title = new Title(titleStr);

            this.chart1.Titles.Add(title);
            this.chart1.ChartAreas.Add(area1);



            for (int i = 0; i < target.Count; i++)
            {
                se.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), target[i]);
                //se.Points.AddXY(time_deviceStart[i].ToString(), target[i]);
                // se.Points.AddXY(time_deviceStart[i], target[i]);
                //se.Points.AddXY(DateTime.Parse(time_deviceStart[i].ToOADate().ToString()), target[i]);
            }

            this.chart1.Series.Add(se);
        }

        /// <summary>
        /// 绘制温度信息图
        /// </summary>
        private void initTemChart()
        {
            this.chart1.Series.Clear();
            Series tem = new Series("温度");

            tem.ChartType = SeriesChartType.Spline;
            tem.Color = Color.Cyan;
            tem.MarkerColor = Color.Black;
            tem.MarkerSize = 3;
            tem.MarkerStyle = MarkerStyle.Circle;
            tem.IsValueShownAsLabel = false;//是否显示值
            //tem.ToolTip="温度："



            this.chart1.ChartAreas[0].AxisX.Title = "日期";
            this.chart1.ChartAreas[0].AxisY.Title = "波动";
            this.chart1.ChartAreas[0].AxisX.Interval = 12;
            this.chart1.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //this.chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm:ss";
            this.chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;



            for (int i = 0; i < temperature.Count; i++)
            {
                //tem.Points.AddXY(time_deviceStart[i].ToOADate(), temperature[i]);
                tem.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), temperature[i]);
            }

            this.chart1.Series.Add(tem);
        }



        /*
         *将字符串转换为需要的时间格式    
         */
        public static DateTime strToDate(string str)
        {
            //DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            //dtFormat.ShortDatePattern = "yyyyMMddhhmmss";
            //DateTime dt = Convert.ToDateTime(str, dtFormat);
            //HH：  代表小时（24小时制）
            DateTime dt = DateTime.ParseExact(str, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            return dt;

        }

        /// <summary>
        /// 获取温度的最大值
        /// </summary>
        /// <param name="temper"></param>
        private double getTemMax(List<double> temper)
        {

            double max = temper[0];
            for (int i = 0; i < temper.Count; i++)
            {
                if (max < temper[i])
                {
                    max = temper[i];
                }
            }
            return max;
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //设置窗体的大小
            this.Size = new Size(900, 600);
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            textBox2.Text = @"C:\Users\Zhang\Desktop\燃气二代-数据";
            //textBox2.Text = Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyyMMdd");
        }

        /// <summary>
        /// 同时绘制四幅图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            initAllChart();
        }

        /// <summary>
        /// 同时画出甲烷、电压、湿度、温度信息图
        /// </summary>
        private void initAllChart()
        {
            if (time_deviceStart.Count == 0)
            {
                MessageBox.Show("请先进行文件处理，再进行绘图操作");
                return;
            }
            this.chart1.Series.Clear();
            this.chart1.ChartAreas.Clear();
            this.chart1.Titles.Clear();


            //设置ChartArea格式
            ChartArea area1 = new ChartArea("area1");
            ChartArea area2 = new ChartArea("area2");

            area1.AxisX.Title = "日期";
            area1.AxisY.Title = "波动";
            area1.AxisX.Interval = 12;
            area1.AxisX.ScrollBar.Enabled = true;
            area1.Position.Width = 85;
            area1.Position.Height = 45;
            area1.Position.X = 3;
            area1.Position.Y = 10;
            area1.AxisY.Minimum = -1;

            //此段代码对放大缩小的控制效果不好
            ////chart 控件之滚动条的放大与缩小功能
            //area1.AxisX.ScaleView.Zoom(2, 3);
            //area1.AxisX.ScaleView.Zoomable = true;
            //area1.CursorX.IsUserEnabled = true;
            //area1.CursorX.IsUserSelectionEnabled = true;
            ////将滚动内嵌到坐标轴中
            //area1.AxisX.ScrollBar.IsPositionedInside = true;

            //// 设置滚动条的大小
            //area1.AxisX.ScrollBar.Size = 10;

            //// 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
            //area1.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;

            //// 设置自动放大与缩小的最小量
            //area1.AxisX.ScaleView.SmallScrollMinSize = 2;
            //area1.AxisX.ScaleView.SmallScrollSize = double.NaN;



            area2.AxisX.Title = "日期";
            area2.AxisY.Title = "波动";
            area2.AxisX.Interval = 12;
            area2.AxisX.ScrollBar.Enabled = true;
            area2.Position.Width = 85;
            area2.Position.Height = 45;
            area2.Position.X = 3;
            area2.Position.Y = area2.Position.Height + 10;
            area2.AxisY.Minimum = -1;

            //添加画图区域
            this.chart1.ChartAreas.Add(area1);
            this.chart1.ChartAreas.Add(area2);

            //设置图表样式
            Series seCH4 = new Series("甲烷");
            seCH4.ChartType = SeriesChartType.Line;
            seCH4.ChartArea = "area1";
            seCH4.Color = Color.Crimson;
            seCH4.BorderWidth = 3;
            seCH4.MarkerStyle = MarkerStyle.Star4;
            seCH4.MarkerSize = 4;
            seCH4.MarkerColor = Color.Black;
            seCH4.IsValueShownAsLabel = false;
            seCH4.ToolTip = "甲烷值：#VALX\r\n日期：#VALY";
            seCH4["PieLabelStyle"] = "Outside";
            seCH4["PieLineColor"] = "Black";
            seCH4.IsValueShownAsLabel = false;


            Series seVoltage = new Series("电压");
            seVoltage.IsValueShownAsLabel = false;
            seVoltage.ChartType = SeriesChartType.Line;
            seVoltage.ChartArea = "area1";
            seVoltage.Color = Color.Lime;
            seVoltage.BorderWidth = 3;
            seVoltage.MarkerColor = Color.Black;
            seVoltage.MarkerSize = 4;
            seVoltage.MarkerStyle = MarkerStyle.Diamond;

            Series seHumidity = new Series("湿度");
            seHumidity.IsValueShownAsLabel = false;
            seHumidity.ChartType = SeriesChartType.Line;
            seHumidity.ChartArea = "area2";
            seHumidity.Color = Color.Cyan;
            seHumidity.BorderWidth = 3;
            seHumidity.MarkerStyle = MarkerStyle.Circle;
            seHumidity.MarkerSize = 4;
            seHumidity.MarkerColor = Color.Black;

            Series seTemperature = new Series("温度");
            seTemperature.IsValueShownAsLabel = false;
            seTemperature.ChartType = SeriesChartType.Line;
            seTemperature.ChartArea = "area2";
            seTemperature.Color = Color.Blue;
            seTemperature.BorderWidth = 3;
            seTemperature.MarkerColor = Color.Black;
            seTemperature.MarkerSize = 4;
            seTemperature.MarkerStyle = MarkerStyle.Square;


            //设置标题
            string titleStr = "设备 " + THHI_GAS_ID + " 在日期 " + directoryName + " 时的数据信息";
            Title title = new Title(titleStr);

            this.chart1.Titles.Add(title);
            //this.chart1.Legends[0].BorderWidth = 3;



            //获取温度最大值
            double temMax = getTemMax(temperature);
            textBox1.Text = temMax.ToString();

            for (int i = 0; i < temperature.Count; i++)
            {
                seCH4.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), CH4[i]);
                seVoltage.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), voltage[i]);
                seHumidity.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), humidity[i]);
                seTemperature.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), temperature[i]);
            }

            this.chart1.Series.Add(seCH4);
            this.chart1.Series.Add(seVoltage);
            this.chart1.Series.Add(seHumidity);
            this.chart1.Series.Add(seTemperature);

        }

        /// <summary>
        /// 选择处理文件的根路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"C:\Users\Zhang\Desktop\燃气二代-数据";


            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = fbd.SelectedPath;
            }
        }

        /// <summary>
        /// 处理选择的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            //文件根目录
            string rootPath = textBox2.Text;
            //文件夹名称
            string direcName = Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyyMMdd");
            //设备ID
            string gasId = comboBox2.Text;

            //赋值给静态变量
            directoryName = direcName;
            THHI_GAS_ID = gasId;

            if (rootPath == null)
            {
                MessageBox.Show("请选择要处理的文件的根目录");
                return;
            }
            if (direcName == null)
            {
                MessageBox.Show("请选择要处理的文件的日期");
                return;
            }

            if (gasId == null)
            {
                MessageBox.Show("请选择要处理的文件设备的id");
                return;
            }


            DirectoryInfo dirD = new DirectoryInfo(rootPath);

            //返回目录下的文件夹信息
            FileSystemInfo[] fileTime = dirD.GetFileSystemInfos();

            //文件路径
            string direcPath = null;
            foreach (FileSystemInfo fsi in fileTime)
            {
                if (direcName == fsi.Name)
                {
                    //文件路径
                    direcPath = rootPath + '\\' + direcName;
                    break;
                }
            }

            if (direcPath == null)
            {
                MessageBox.Show("不包含所查找的日期 "+direcName+" 的信息，请重新选择日期");
                return;
            }

            DirectoryInfo dirData = new DirectoryInfo(direcPath);
            //FileSystemInfo[] filesData = dirData.GetFileSystemInfos();
            FileInfo[] filesData = dirData.GetFiles();

            //最终dat路径
            string path = null;
            foreach (FileInfo fileData in filesData)
            {
                //获得在打开对话框中选中文件的路径
                string gasIdJudge = fileData.Name;
                gasIdJudge = gasIdJudge.Substring(gasIdJudge.LastIndexOf('.') - 7, 3);

                //设备ID满足要求，则取出文件的路径
                if (gasId == gasIdJudge)
                {
                    path = direcPath + '\\' + fileData.Name;
                    break;
                }

            }

            if (path == null)
            {
                MessageBox.Show("所选择的设备ID "+gasId+" 不存在，请重新选择设备ID");
                return;
            }
            //处理文件，取出相应的数据
            processDirectory(path);
           
        }

        /// <summary>
        /// 单个dat文件处理
        /// </summary>
        /// <param name="path"></param>
        private void processDirectory(string path)
        {
            //清除静态变量所包含的数据
            if (CH4.Count != 0)
            {
                CH4.Clear();
            }
            if (voltage.Count != 0)
            {
                voltage.Clear();
            }
            if (humidity.Count != 0)
            {
                humidity.Clear();
            }
            if (temperature.Count != 0)
            {
                temperature.Clear();
            }
            if (ymd_deviceStart.Count != 0)
            {
                ymd_deviceStart.Clear();
            }
            if (hms_deviceStart.Count != 0)
            {
                hms_deviceStart.Clear();
            }
            if (time_deviceStart.Count != 0)
            {
                time_deviceStart.Clear();
            }
            //读取文件中的数据
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string strLine = null;
                    //文件数据中前13行为不必要的数据，要剔除
                    int index = 0;
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        index++;
                        if (index > 13)
                        {
                            //处理每一行数据
                            string[] strProcess = strLine.Split(new char[] { ' ', ',', '\t' }, StringSplitOptions.None);

                            ymd_deviceStart.Add(strProcess[3]);
                            hms_deviceStart.Add(strProcess[4]);

                            //将yyyyMMddhhmmss转换为时间格式
                            string dtStr = strProcess[3] + strProcess[4];
                            time_deviceStart.Add(strToDate(dtStr));

                            //利用绝对位置取值
                            //CH4.Add(Convert.ToDouble(strProcess[7]));
                            //voltage.Add(Convert.ToDouble(strProcess[9]));
                            //humidity.Add(Convert.ToDouble(strProcess[10]));
                            //temperature.Add(Convert.ToDouble(strProcess[11]));

                            //利用相对位置取值
                            int dataLength = strProcess.Length;
                            CH4.Add(Convert.ToDouble(strProcess[dataLength - 5]));
                            voltage.Add(Convert.ToDouble(strProcess[dataLength - 3]));
                            humidity.Add(Convert.ToDouble(strProcess[dataLength - 2]));
                            temperature.Add(Convert.ToDouble(strProcess[dataLength - 1]));
                        }

                    }
                }
            }
            //文件处理完成之后，出现提示信息
            String hintInfo = "文件处理结束 \n\n 1、请点击‘绘制单幅数据图’按钮画出单幅图，要配合‘绘图选择’指定画哪一副图 \n\n 2、请点击‘绘制所有数据图’按钮，绘制出所有图形";
            MessageBox.Show(hintInfo);
        }

        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// 获取图表上点的信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart1_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                this.Cursor = Cursors.Cross;
                int i = e.HitTestResult.PointIndex;
                DataPoint dp = e.HitTestResult.Series.Points[i];
                DateTime dt = DateTime.FromOADate(dp.XValue);//将double的xvalue转换为datetime形式
                //e.Text = String.Format("数值：{1：F3}" + "\n日期：{0}", dp.XValue.ToString("yyyy-MM-dd HH:mm:ss"), dp.YValues[0]);
                e.Text = string.Format("日期:{0} \n数值:{1:F2} ", time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), dp.YValues[0]);
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 把图形分为左右两边显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            initLeftRightChart();
        }

        /// <summary>
        /// 图形左右两边显示过程
        /// </summary>
        private void initLeftRightChart()
        {
            if (time_deviceStart.Count == 0)
            {
                MessageBox.Show("请先进行文件处理，再进行绘图操作");
                return;
            }
            this.chart1.Series.Clear();
            this.chart1.ChartAreas.Clear();
            this.chart1.Titles.Clear();


            //设置ChartArea格式
            ChartArea area1 = new ChartArea("area1");
            ChartArea area2 = new ChartArea("area2");

            area1.AxisX.Title = "日期";
            area1.AxisY.Title = "波动";
            area1.AxisX.Interval = 12;
            area1.AxisX.ScrollBar.Enabled = true;
            area1.Position.Width = 45;
            area1.Position.Height = 85;
            area1.Position.X = 2;
            area1.Position.Y = 10;
            area1.AxisY.Minimum = -1;

            area2.AxisX.Title = "日期";
            area2.AxisY.Title = "波动";
            area2.AxisX.Interval = 12;
            area2.AxisX.ScrollBar.Enabled = true;
            area2.Position.Width = 45;
            area2.Position.Height = 85;
            area2.Position.X = area1.Position.Width;
            area2.Position.Y = 10;
            area2.AxisY.Minimum = -1;

            //添加画图区域
            this.chart1.ChartAreas.Add(area1);
            this.chart1.ChartAreas.Add(area2);

            //设置图表样式
            Series seCH4 = new Series("甲烷");
            seCH4.ChartType = SeriesChartType.Line;
            seCH4.ChartArea = "area1";
            seCH4.Color = Color.Crimson;
            seCH4.BorderWidth = 3;
            seCH4.MarkerStyle = MarkerStyle.Star4;
            seCH4.MarkerSize = 4;
            seCH4.MarkerColor = Color.Black;
            seCH4.IsValueShownAsLabel = false;
            seCH4.ToolTip = "甲烷值：#VALX\r\n日期：#VALY";
            seCH4["PieLabelStyle"] = "Outside";
            seCH4["PieLineColor"] = "Black";
            seCH4.IsValueShownAsLabel = false;


            Series seVoltage = new Series("电压");
            seVoltage.IsValueShownAsLabel = false;
            seVoltage.ChartType = SeriesChartType.Line;
            seVoltage.ChartArea = "area1";
            seVoltage.Color = Color.Lime;
            seVoltage.BorderWidth = 3;
            seVoltage.MarkerColor = Color.Black;
            seVoltage.MarkerSize = 4;
            seVoltage.MarkerStyle = MarkerStyle.Diamond;

            Series seHumidity = new Series("湿度");
            seHumidity.IsValueShownAsLabel = false;
            seHumidity.ChartType = SeriesChartType.Line;
            seHumidity.ChartArea = "area2";
            seHumidity.Color = Color.Cyan;
            seHumidity.BorderWidth = 3;
            seHumidity.MarkerStyle = MarkerStyle.Circle;
            seHumidity.MarkerSize = 4;
            seHumidity.MarkerColor = Color.Black;

            Series seTemperature = new Series("温度");
            seTemperature.IsValueShownAsLabel = false;
            seTemperature.ChartType = SeriesChartType.Line;
            seTemperature.ChartArea = "area2";
            seTemperature.Color = Color.Blue;
            seTemperature.BorderWidth = 3;
            seTemperature.MarkerColor = Color.Black;
            seTemperature.MarkerSize = 4;
            seTemperature.MarkerStyle = MarkerStyle.Square;


            //设置标题
            string titleStr = "设备 " + THHI_GAS_ID + " 在日期 " + directoryName + " 时的数据信息";
            Title title = new Title(titleStr);

            this.chart1.Titles.Add(title);
            //this.chart1.Legends[0].BorderWidth = 3;



            //获取温度最大值
            double temMax = getTemMax(temperature);
            textBox1.Text = temMax.ToString();

            for (int i = 0; i < temperature.Count; i++)
            {
                seCH4.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), CH4[i]);
                seVoltage.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), voltage[i]);
                seHumidity.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), humidity[i]);
                seTemperature.Points.AddXY(time_deviceStart[i].ToString("yyyy-MM-dd HH:mm:ss"), temperature[i]);
            }

            this.chart1.Series.Add(seCH4);
            this.chart1.Series.Add(seVoltage);
            this.chart1.Series.Add(seHumidity);
            this.chart1.Series.Add(seTemperature);
        }
    }
}
