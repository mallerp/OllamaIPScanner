using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace OllamaIPScanner
{
    public class Scanner
    {
        private readonly string ipFilePath;
        private readonly ListBox logBox;
        private readonly ProgressBar progressBar;
        private readonly Label statusLabel;
        private readonly string progressFilePath;
        private List<string> ipList = new List<string>();
        private int total;
        private int completed;
        private bool paused = false;
        private Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
        private int targetPort = 11434;
        private int maxThreads = 50;
        private int timeout = 5;
        private CancellationTokenSource cts;
        private System.Windows.Forms.Timer progressTimer;

        public Scanner(string ipFilePath, ListBox logBox, ProgressBar progressBar, Label statusLabel, string progressFilePath = null)
        {
            this.ipFilePath = ipFilePath;
            this.logBox = logBox;
            this.progressBar = progressBar;
            this.statusLabel = statusLabel;
            this.progressFilePath = progressFilePath;
        }

        public void StartScan()
        {
            Task.Run(async () =>
            {
                ipList = ReadTargets(ipFilePath);
                total = ipList.Count;
                completed = 0;
                results.Clear();
                cts = new CancellationTokenSource();
                StartProgressTimer();
                UpdateProgress();
                await ScanAllAsync();
                StopProgressTimer();
                UpdateProgress();
            });
        }

        public void PauseScan()
        {
            paused = true;
            statusLabel.Invoke((Action)(() => statusLabel.Text = "状态: 已暂停"));
        }

        public void ResumeScan()
        {
            paused = false;
            statusLabel.Invoke((Action)(() => statusLabel.Text = "状态: 扫描中..."));
        }

        public void SaveProgress()
        {
            var state = new ProgressState
            {
                Completed = completed,
                Total = total,
                Results = results,
                RemainingIPs = ipList.Skip(completed).ToList()
            };
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Progress");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string fileName = $"scan_progress_{DateTime.Now:yyyyMMdd-HHmmss}.json";
            string file = Path.Combine(dir, fileName);
            File.WriteAllText(file, JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true }));
            logBox.Invoke((Action)(() => logBox.Items.Add($"进度已保存到: {file}")));
        }

        public void LoadProgressAndContinue()
        {
            if (string.IsNullOrEmpty(progressFilePath)) return;
            string file = progressFilePath;
            if (!Path.IsPathRooted(file))
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Progress", file);
            if (!File.Exists(file)) return;
            var state = JsonSerializer.Deserialize<ProgressState>(File.ReadAllText(file));
            completed = state.Completed;
            total = state.Total;
            results = state.Results ?? new Dictionary<string, List<string>>();
            ipList = state.RemainingIPs ?? new List<string>();
            cts = new CancellationTokenSource();
            StartProgressTimer();
            UpdateProgress();
            Task.Run(async () => { await ScanAllAsync(); StopProgressTimer(); UpdateProgress(); });
        }

        private List<string> ReadTargets(string filename)
        {
            var targets = new List<string>();
            foreach (var line in File.ReadAllLines(filename))
            {
                var l = line.Trim();
                if (string.IsNullOrEmpty(l) || l.StartsWith("#")) continue;
                if (l.Contains("/")) // CIDR
                {
                    try
                    {
                        var net = IPNetwork.Parse(l);
                        int count = 0;
                        foreach (var ip in net.ListIPAddress())
                        {
                            if (count >= 256) break;
                            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                targets.Add(ip.ToString());
                                count++;
                            }
                        }
                    }
                    catch { }
                }
                else // 单个IP
                {
                    if (IPAddress.TryParse(l, out var ip) && ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        targets.Add(ip.ToString());
                    }
                }
            }
            return targets;
        }

        private async Task ScanAllAsync()
        {
            statusLabel.Invoke((Action)(() => statusLabel.Text = "状态: 扫描中..."));
            progressBar.Invoke((Action)(() => { progressBar.Maximum = total; progressBar.Value = completed; }));
            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };
            await Parallel.ForEachAsync(ipList.Skip(completed), options, async (ip, ct) =>
            {
                while (paused) await Task.Delay(200);
                logBox.Invoke((Action)(() => logBox.Items.Add($"正在扫描: {ip}")));
                try
                {
                    while (paused) await Task.Delay(200);
                    if (await PortOpen(ip, targetPort, timeout))
                    {
                        while (paused) await Task.Delay(200);
                        var models = await FetchOllamaModels(ip, targetPort, timeout);
                        if (models.Count > 0)
                        {
                            lock (results)
                            {
                                results[$"{ip}:{targetPort}"] = models;
                            }
                            logBox.Invoke((Action)(() => logBox.Items.Add($"发现服务: {ip}:{targetPort}")));
                        }
                    }
                }
                catch { }
                finally
                {
                    Interlocked.Increment(ref completed);
                }
            });
            SaveResult();
            statusLabel.Invoke((Action)(() => statusLabel.Text = $"扫描完成，发现 {results.Count} 个有效服务"));
        }

        private void UpdateProgress()
        {
            progressBar.Invoke((Action)(() => {
                progressBar.Maximum = total;
                progressBar.Value = Math.Min(completed, total);
            }));
            statusLabel.Invoke((Action)(() => statusLabel.Text = $"进度: {completed}/{total}"));
        }

        private async Task<bool> PortOpen(string ip, int port, int timeoutSec)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var task = client.ConnectAsync(ip, port);
                    if (await Task.WhenAny(task, Task.Delay(timeoutSec * 1000)) == task && client.Connected)
                        return true;
                }
            }
            catch { }
            return false;
        }

        private async Task<List<string>> FetchOllamaModels(string ip, int port, int timeoutSec)
        {
            var models = new List<string>();
            try
            {
                using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSec) })
                {
                    var resp = await http.GetAsync($"http://{ip}:{port}/api/tags");
                    if (resp.IsSuccessStatusCode)
                    {
                        var json = await resp.Content.ReadAsStringAsync();
                        using (var doc = JsonDocument.Parse(json))
                        {
                            if (doc.RootElement.TryGetProperty("models", out var arr))
                            {
                                foreach (var model in arr.EnumerateArray())
                                {
                                    string name = model.GetProperty("name").GetString();
                                    string digest = model.GetProperty("digest").GetString();
                                    models.Add($"{name} (digest: {digest.Substring(0, 8)})");
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return models;
        }

        private void SaveResult()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Result");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string fileName = $"scan_result_{DateTime.Now:yyyyMMdd-HHmmss}.json";
            string file = Path.Combine(dir, fileName);
            File.WriteAllText(file, JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
            logBox.Invoke((Action)(() => logBox.Items.Add($"结果已保存到: {file}")));
        }

        private void StartProgressTimer()
        {
            if (progressTimer == null)
            {
                progressTimer = new System.Windows.Forms.Timer();
                progressTimer.Interval = 200; // 200ms 刷新一次
                progressTimer.Tick += (s, e) => UpdateProgress();
            }
            progressTimer.Start();
        }

        private void StopProgressTimer()
        {
            if (progressTimer != null)
            {
                progressTimer.Stop();
            }
        }
    }

    // IPNetwork 工具类（支持CIDR解析）
    public class IPNetwork
    {
        public IPAddress Network { get; set; }
        public int Cidr { get; set; }
        public IPAddress FirstUsable { get; set; }
        public IPAddress LastUsable { get; set; }
        public int TotalHosts { get; set; }

        public static IPNetwork Parse(string cidr)
        {
            var parts = cidr.Split('/');
            if (parts.Length != 2) throw new FormatException();
            var ip = IPAddress.Parse(parts[0]);
            int prefix = int.Parse(parts[1]);
            uint mask = 0xffffffff << (32 - prefix);
            uint ipInt = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
            uint network = ipInt & mask;
            var networkIP = new IPAddress(BitConverter.GetBytes(network).Reverse().ToArray());
            var first = network + 1;
            var last = (network | ~mask) - 1;
            var firstIP = new IPAddress(BitConverter.GetBytes(first).Reverse().ToArray());
            var lastIP = new IPAddress(BitConverter.GetBytes(last).Reverse().ToArray());
            int total = (int)(last - first + 1);
            return new IPNetwork { Network = networkIP, Cidr = prefix, FirstUsable = firstIP, LastUsable = lastIP, TotalHosts = total };
        }
        public IEnumerable<IPAddress> ListIPAddress()
        {
            uint first = BitConverter.ToUInt32(FirstUsable.GetAddressBytes().Reverse().ToArray(), 0);
            uint last = BitConverter.ToUInt32(LastUsable.GetAddressBytes().Reverse().ToArray(), 0);
            for (uint i = first; i <= last; i++)
            {
                yield return new IPAddress(BitConverter.GetBytes(i).Reverse().ToArray());
            }
        }
    }
} 