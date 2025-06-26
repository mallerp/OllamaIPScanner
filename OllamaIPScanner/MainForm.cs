using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OllamaIPScanner
{
    public partial class MainForm : Form
    {
        private Scanner scanner;
        private string ipFilePath = string.Empty;
        private string progressFilePath = string.Empty;

        public MainForm()
        {
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "Ollama-IP扫描器";
            this.Size = new Size(800, 600);

            Button btnLoadIP = new Button { Text = "选择IP文件", Location = new Point(20, 20), Size = new Size(100, 30) };
            Button btnStart = new Button { Text = "开始扫描", Location = new Point(140, 20), Size = new Size(100, 30) };
            Button btnPause = new Button { Text = "暂停", Location = new Point(260, 20), Size = new Size(100, 30) };
            Button btnResume = new Button { Text = "继续", Location = new Point(380, 20), Size = new Size(100, 30) };
            Button btnSave = new Button { Text = "保存进度", Location = new Point(500, 20), Size = new Size(100, 30) };
            Button btnLoadProgress = new Button { Text = "加载进度", Location = new Point(620, 20), Size = new Size(100, 30) };

            ProgressBar progressBar = new ProgressBar { Location = new Point(20, 60), Size = new Size(700, 20) };
            ListBox logBox = new ListBox { Location = new Point(20, 100), Size = new Size(740, 400) };
            Label statusLabel = new Label { Location = new Point(20, 520), Size = new Size(740, 30), Text = "状态: 等待操作" };

            this.Controls.Add(btnLoadIP);
            this.Controls.Add(btnStart);
            this.Controls.Add(btnPause);
            this.Controls.Add(btnResume);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnLoadProgress);
            this.Controls.Add(progressBar);
            this.Controls.Add(logBox);
            this.Controls.Add(statusLabel);

            // 事件绑定
            btnLoadIP.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog { Filter = "IP文件|*.txt" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ipFilePath = ofd.FileName;
                    statusLabel.Text = $"已选择IP文件: {ipFilePath}";
                }
            };
            btnStart.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(ipFilePath) || !File.Exists(ipFilePath))
                {
                    MessageBox.Show("请先选择有效的IP文件！");
                    return;
                }
                scanner = new Scanner(ipFilePath, logBox, progressBar, statusLabel);
                scanner.StartScan();
            };
            btnPause.Click += (s, e) => { scanner?.PauseScan(); };
            btnResume.Click += (s, e) => { scanner?.ResumeScan(); };
            btnSave.Click += (s, e) => { scanner?.SaveProgress(); };
            btnLoadProgress.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog { Filter = "进度文件|*.json" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    progressFilePath = ofd.FileName;
                    scanner = new Scanner(ipFilePath, logBox, progressBar, statusLabel, progressFilePath);
                    scanner.LoadProgressAndContinue();
                }
            };
        }
    }
} 