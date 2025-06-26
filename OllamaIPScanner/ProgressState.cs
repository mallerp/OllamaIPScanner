using System.Collections.Generic;

namespace OllamaIPScanner
{
    public class ProgressState
    {
        public int Completed { get; set; }
        public int Total { get; set; }
        public Dictionary<string, List<string>> Results { get; set; }
        public List<string> RemainingIPs { get; set; }
    }
} 