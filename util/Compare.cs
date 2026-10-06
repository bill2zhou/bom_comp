using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BOM_compare.util
{
    /// <summary>
    /// 单条差异项（与 Tauri 版 compare.rs 的 DiffItem 一致）
    /// </summary>
    internal class DiffItem
    {
        /// <summary>位号</summary>
        public string RefDes { get; set; }

        /// <summary>BOM 1 中的料号（已排序）</summary>
        public List<string> PnList1 { get; set; } = new List<string>();

        /// <summary>BOM 2 中的料号（已排序）</summary>
        public List<string> PnList2 { get; set; } = new List<string>();

        /// <summary>差异类型: Part Changed / Only in Bom1 / Only in Bom2</summary>
        public string Type { get; set; }

        /// <summary>两侧是否至少存在一个相同的料号（用于整行高亮判断）</summary>
        public bool HasCommonPart()
        {
            foreach (string pn in PnList1)
            {
                if (PnList2.Contains(pn)) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 对比结果（与 Tauri 版 compare.rs 的 CompareResult 一致）
    /// </summary>
    internal class CompareResult
    {
        public List<DiffItem> PartChanges { get; set; } = new List<DiffItem>();
        public List<DiffItem> OnlyInBom1 { get; set; } = new List<DiffItem>();
        public List<DiffItem> OnlyInBom2 { get; set; } = new List<DiffItem>();

        /// <summary>BOM 1 文件名（不含扩展名）</summary>
        public string Filename1 { get; set; }

        /// <summary>BOM 2 文件名（不含扩展名）</summary>
        public string Filename2 { get; set; }

        /// <summary>差异总数</summary>
        public int TotalDifferences
        {
            get { return PartChanges.Count + OnlyInBom1.Count + OnlyInBom2.Count; }
        }
    }

    internal class Compare
    {
        // 预编译正则表达式，提高性能
        private static readonly Regex PartLineRegex = new Regex(@"^\|\s\d\s\d{4}\s(\w+)", RegexOptions.Compiled);
        private static readonly Regex RefLineRegex = new Regex(@"^\|\s{10}\d{4}\s(\w+)", RegexOptions.Compiled);

        // 定义需要过滤的关键词 (不区分大小写)
        // 这些关键词将用于匹配位号 (RefDes)，例如 "PCB1", "DAX-TEST" 等
        private static readonly string[] FilterKeywords = { "ZZZ", "DAX", "PCB" };

        // 报告列宽
        private const int ColIdxWidth = 4;
        private const int ColLocWidth = 15;
        private const int ColPnWidth = 30;

        /// <summary>
        /// BOM数据结构
        /// </summary>
        private class BomData
        {
            // Key: "位号_料号", 用于唯一标识一个元件实例
            public HashSet<string> UniqueComponents = new HashSet<string>();

            // Key: "位号", Value: 该位号出现的次数
            public Dictionary<string, int> RefCountMap = new Dictionary<string, int>();

            // Key: "位号", Value: 该位号对应的所有唯一料号集合
            public Dictionary<string, HashSet<string>> RefToPartsSet = new Dictionary<string, HashSet<string>>();

            // Key: "位号", Value: 主要料号 (取最后一次出现的，用于简单展示或排序参考)
            public Dictionary<string, string> RefToPartMap = new Dictionary<string, string>();

            public void AddComponent(string refDes, string partNumber)
            {
                string key = string.Format("{0}_{1}", refDes, partNumber);
                UniqueComponents.Add(key);

                if (!RefCountMap.ContainsKey(refDes))
                {
                    RefCountMap[refDes] = 0;
                    RefToPartsSet[refDes] = new HashSet<string>();
                }

                RefCountMap[refDes]++;
                RefToPartsSet[refDes].Add(partNumber);
                RefToPartMap[refDes] = partNumber;
            }
        }

        /// <summary>
        /// 检查字符串是否包含需要过滤的关键词
        /// </summary>
        private static bool IsFiltered(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            foreach (string keyword in FilterKeywords)
            {
                if (text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 解析单个BOM文件
        /// </summary>
        private static BomData ParseBom(string filePath)
        {
            BomData data = new BomData();
            string currentPartNumber = "";

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("BOM file not found: {0}", filePath));
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    Match partMatch = PartLineRegex.Match(line);
                    if (partMatch.Success)
                    {
                        currentPartNumber = partMatch.Groups[1].Value;
                        continue;
                    }

                    Match refMatch = RefLineRegex.Match(line);
                    if (refMatch.Success)
                    {
                        string refDes = refMatch.Groups[1].Value;

                        if (!string.IsNullOrEmpty(currentPartNumber))
                        {
                            // 过滤逻辑应用于位号：包含 ZZZ / DAX / PCB (不区分大小写) 则跳过该元件
                            if (!IsFiltered(refDes))
                            {
                                data.AddComponent(refDes, currentPartNumber);
                            }
                        }
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 解析两个 BOM 并计算差异（不生成文件，供界面直接展示）
        /// </summary>
        public static CompareResult Analyze(string bom1, string bom2)
        {
            // 1. 解析 (此时已自动过滤掉位号包含特定字符的元件)
            BomData data1 = ParseBom(bom1);
            BomData data2 = ParseBom(bom2);

            CompareResult result = new CompareResult
            {
                Filename1 = Path.GetFileNameWithoutExtension(bom1),
                Filename2 = Path.GetFileNameWithoutExtension(bom2)
            };

            // 2. 分类差异
            HashSet<string> allRefs = new HashSet<string>();
            allRefs.UnionWith(data1.RefToPartMap.Keys);
            allRefs.UnionWith(data2.RefToPartMap.Keys);

            foreach (string refDes in allRefs)
            {
                bool in1 = data1.RefToPartMap.ContainsKey(refDes);
                bool in2 = data2.RefToPartMap.ContainsKey(refDes);

                if (in1 && in2)
                {
                    // 两边都存在，检查料号集合是否完全一致
                    HashSet<string> set1 = data1.RefToPartsSet[refDes];
                    HashSet<string> set2 = data2.RefToPartsSet[refDes];

                    if (!set1.SetEquals(set2))
                    {
                        result.PartChanges.Add(new DiffItem
                        {
                            RefDes = refDes,
                            PnList1 = set1.OrderBy(p => p, StringComparer.Ordinal).ToList(),
                            PnList2 = set2.OrderBy(p => p, StringComparer.Ordinal).ToList(),
                            Type = "Part Changed"
                        });
                    }
                }
                else if (in1)
                {
                    result.OnlyInBom1.Add(new DiffItem
                    {
                        RefDes = refDes,
                        PnList1 = data1.RefToPartsSet[refDes].OrderBy(p => p, StringComparer.Ordinal).ToList(),
                        PnList2 = new List<string>(),
                        Type = "Only in Bom1"
                    });
                }
                else
                {
                    result.OnlyInBom2.Add(new DiffItem
                    {
                        RefDes = refDes,
                        PnList1 = new List<string>(),
                        PnList2 = data2.RefToPartsSet[refDes].OrderBy(p => p, StringComparer.Ordinal).ToList(),
                        Type = "Only in Bom2"
                    });
                }
            }

            // 3. 排序
            Comparison<DiffItem> sort = (a, b) => string.Compare(a.RefDes, b.RefDes, StringComparison.OrdinalIgnoreCase);
            result.PartChanges.Sort(sort);
            result.OnlyInBom1.Sort(sort);
            result.OnlyInBom2.Sort(sort);

            return result;
        }

        /// <summary>
        /// 根据对比结果生成 txt 报告内容（与 Tauri 版 build_diff_text 一致）
        /// </summary>
        public static string BuildDiffText(CompareResult result)
        {
            string filename1 = result.Filename1;
            string filename2 = result.Filename2;

            StringBuilder sb = new StringBuilder();
            string header = "".PadRight(ColIdxWidth) + "Location".PadRight(ColLocWidth)
                + filename1.PadRight(ColPnWidth) + filename2.PadRight(ColPnWidth);
            int i = 1;

            // --- 第二部分：仅在 Bom1 ---
            sb.AppendLine("=== Only in " + filename1 + " ===");
            sb.AppendLine(header.Replace(filename2.PadRight(ColPnWidth), "".PadRight(ColPnWidth)));
            foreach (DiffItem item in result.OnlyInBom1)
            {
                WriteAlignedRows(sb, i.ToString(), item.RefDes, item.PnList1, item.PnList2);
                i++;
            }
            if (result.OnlyInBom1.Count == 0) sb.AppendLine("(None)");
            sb.AppendLine();

            // --- 第三部分：仅在 Bom2 ---
            sb.AppendLine("=== Only in " + filename2 + " ===");
            sb.AppendLine(header.Replace(filename1.PadRight(ColPnWidth), "".PadRight(ColPnWidth)));
            foreach (DiffItem item in result.OnlyInBom2)
            {
                WriteAlignedRows(sb, i.ToString(), item.RefDes, item.PnList1, item.PnList2);
                i++;
            }
            if (result.OnlyInBom2.Count == 0) sb.AppendLine("(None)");

            // --- 第一部分：料号变更 ---
            sb.AppendLine("=== Part Number Differences (Exist in Both) ===");
            sb.AppendLine(header);
            foreach (DiffItem item in result.PartChanges)
            {
                WriteAlignedRows(sb, i.ToString(), item.RefDes, item.PnList1, item.PnList2);
                i++;
            }
            if (result.PartChanges.Count == 0) sb.AppendLine("(None)");
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// 对比两个 BOM 并生成报告文件后用记事本打开（保留原有行为）
        /// </summary>
        public void comp(string bom1, string bom2)
        {
            try
            {
                CompareResult result = Analyze(bom1, bom2);

                string diffPath = Path.Combine(
                    Environment.CurrentDirectory,
                    string.Format("diff_{0}_{1}.txt", result.Filename1, result.Filename2));

                File.WriteAllText(diffPath, BuildDiffText(result), new UTF8Encoding(false));

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = "\"" + diffPath + "\"",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 辅助方法：将两个料号列表按行对齐写入
        /// </summary>
        private static void WriteAlignedRows(StringBuilder sb, string index, string location, List<string> pnList1, List<string> pnList2)
        {
            int maxRows = Math.Max(pnList1.Count, pnList2.Count);
            if (maxRows == 0) maxRows = 1;

            for (int r = 0; r < maxRows; r++)
            {
                string idxStr = (r == 0) ? index.PadRight(ColIdxWidth) : "".PadRight(ColIdxWidth);
                string locStr = (r == 0) ? location.PadRight(ColLocWidth) : "".PadRight(ColLocWidth);

                string pn1 = (r < pnList1.Count) ? pnList1[r] : "";
                string pn2 = (r < pnList2.Count) ? pnList2[r] : "";

                sb.Append(idxStr).Append(locStr)
                  .Append(pn1.PadRight(ColPnWidth))
                  .Append(pn2.PadRight(ColPnWidth))
                  .AppendLine();
            }
        }
    }
}
