using System;
using System.Collections.Generic;
using System.Text;

namespace BOM_compare.util
{
    /// <summary>
    /// 生成与软件界面风格一致的独立 HTML 报告。
    /// 样式完全对应 Tauri 版 src/styles.css（同款配色、卡片、标签页、表格与高亮规则）。
    /// </summary>
    internal static class HtmlReport
    {
        /// <summary>生成报告 HTML 文本（只包含「对比结果」部分）</summary>
        public static string Build(CompareResult result)
        {
            string name1 = Esc(result.Filename1);
            string name2 = Esc(result.Filename2);

            StringBuilder sb = new StringBuilder(96 * 1024);

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"zh-CN\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\">");
            sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine(string.Format("<title>BOM 对比报告 - {0} vs {1}</title>", name1, name2));
            sb.AppendLine(string.Format("<link rel=\"icon\" href=\"{0}\">", FaviconDataUri));
            sb.AppendLine("<style>");
            sb.AppendLine(Css);
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"container\">");

            // ---------- 对比结果 ----------
            sb.AppendLine("<section class=\"card-block\">");
            sb.AppendLine("<div class=\"result-header\">");
            sb.AppendLine("<h2>对比结果</h2>");
            sb.AppendLine("<button class=\"btn-link\" id=\"print-btn\" type=\"button\">打印 / 导出 PDF</button>");
            sb.AppendLine("</div>");

            // 统计卡
            sb.AppendLine("<div class=\"summary-cards\">");
            AppendSummaryCard(sb, "card-changed", result.PartChanges.Count, "料号变更");
            AppendSummaryCard(sb, "card-only1", result.OnlyInBom1.Count, "仅 BOM 1");
            AppendSummaryCard(sb, "card-only2", result.OnlyInBom2.Count, "仅 BOM 2");
            sb.AppendLine("</div>");

            // 标签页
            sb.AppendLine("<div class=\"tabs\">");
            sb.AppendLine("<button class=\"tab active\" type=\"button\" data-tab=\"changed\">料号变更</button>");
            sb.AppendLine("<button class=\"tab\" type=\"button\" data-tab=\"only1\">仅 BOM 1</button>");
            sb.AppendLine("<button class=\"tab\" type=\"button\" data-tab=\"only2\">仅 BOM 2</button>");
            sb.AppendLine("</div>");

            // 三个面板（对应界面上的三个标签页）
            AppendPanel(sb, "changed", "料号变更", result.PartChanges, true, name1, name2);
            AppendPanel(sb, "only1", "仅 BOM 1", result.OnlyInBom1, false, name1, name2);
            AppendPanel(sb, "only2", "仅 BOM 2", result.OnlyInBom2, false, name1, name2);

            sb.AppendLine("</section>");

            sb.AppendLine("</div>");
            sb.AppendLine("<script>");
            sb.AppendLine(Script);
            sb.AppendLine("</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        // ==================== 片段 ====================

        private static void AppendSummaryCard(StringBuilder sb, string cssClass, int count, string label)
        {
            sb.AppendLine(string.Format("<div class=\"card {0}\">", cssClass));
            sb.AppendLine(string.Format("<div class=\"card-num\">{0}</div>", count));
            sb.AppendLine(string.Format("<div class=\"card-label\">{0}</div>", Esc(label)));
            sb.AppendLine("</div>");
        }

        private static void AppendPanel(StringBuilder sb, string id, string title, List<DiffItem> items,
            bool active, string name1, string name2)
        {
            sb.AppendLine(string.Format("<div class=\"panel{0}\" id=\"panel-{1}\">", active ? " active" : "", id));
            sb.AppendLine(string.Format("<h3 class=\"panel-title\">{0}</h3>", Esc(title)));

            sb.AppendLine("<div class=\"table-wrap\">");
            sb.AppendLine("<table class=\"result-table\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th class=\"col-idx\">#</th>");
            sb.AppendLine("<th class=\"col-loc\">Location</th>");
            sb.AppendLine(string.Format("<th>{0}</th>", name1));
            sb.AppendLine(string.Format("<th>{0}</th>", name2));
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            if (items.Count == 0)
            {
                sb.AppendLine("<tr><td colspan=\"4\" class=\"empty-row\">(None) — 无差异</td></tr>");
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    DiffItem item = items[i];
                    string rowClass = (active && !item.HasCommonPart()) ? " class=\"row-no-common\"" : "";

                    sb.AppendLine(string.Format("<tr{0}>", rowClass));
                    sb.AppendLine(string.Format("<td class=\"col-idx\">{0}</td>", i + 1));
                    sb.AppendLine(string.Format("<td class=\"col-loc\">{0}</td>", Esc(item.RefDes)));
                    sb.AppendLine(string.Format("<td class=\"pn-cell\">{0}</td>", BuildPartCell(item.PnList1)));
                    sb.AppendLine(string.Format("<td class=\"pn-cell\">{0}</td>", BuildPartCell(item.PnList2)));
                    sb.AppendLine("</tr>");
                }
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
        }

        private static string BuildPartCell(List<string> parts)
        {
            if (parts == null || parts.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;
                sb.Append(string.Format("<span class=\"pn-item\">{0}</span>", Esc(part)));
            }
            return sb.ToString();
        }

        private static string Esc(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("&", "&amp;")
                       .Replace("<", "&lt;")
                       .Replace(">", "&gt;")
                       .Replace("\"", "&quot;");
        }

        // ==================== 样式（对应 styles.css） ====================

        /// <summary>内嵌 SVG 站点图标（与软件图标同款：蓝底白条），避免报告依赖外部文件</summary>
        private const string FaviconDataUri =
            "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'%3E"
            + "%3Crect width='64' height='64' rx='14' fill='%232563eb'/%3E"
            + "%3Crect x='14' y='20' width='36' height='5' rx='2.5' fill='white'/%3E"
            + "%3Crect x='14' y='30' width='24' height='5' rx='2.5' fill='white'/%3E"
            + "%3Crect x='14' y='40' width='30' height='5' rx='2.5' fill='white'/%3E%3C/svg%3E";

        private const string Css = @"
* { margin: 0; padding: 0; box-sizing: border-box; }

:root {
  --primary: #2563eb;
  --primary-dark: #1d4ed8;
  --danger: #dc2626;
  --warning: #d97706;
  --success: #16a34a;
  --bg: #f1f5f9;
  --card: #ffffff;
  --border: #e2e8f0;
  --text: #0f172a;
  --muted: #64748b;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Microsoft YaHei', sans-serif;
  background: var(--bg);
  color: var(--text);
  min-height: 100vh;
}

.container { max-width: 900px; margin: 0 auto; padding: 28px 24px; }

/* 卡片 */
.card-block {
  background: var(--card);
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
}

/* 结果头部 */
.result-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.result-header h2 { font-size: 18px; }
.btn-link {
  background: transparent;
  border: none;
  color: var(--primary);
  padding: 6px 12px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  border-radius: 8px;
}
.btn-link:hover { text-decoration: underline; }

/* 统计卡 */
.summary-cards { display: flex; gap: 12px; margin-bottom: 18px; }
.card { flex: 1; border-radius: 10px; padding: 16px; text-align: center; border: 1px solid var(--border); }
.card-num { font-size: 30px; font-weight: 800; }
.card-label { font-size: 13px; margin-top: 4px; color: var(--muted); }
.card-changed { background: #fef2f2; border-color: #fecaca; }
.card-changed .card-num { color: var(--danger); }
.card-only1 { background: #fffbeb; border-color: #fde68a; }
.card-only1 .card-num { color: var(--warning); }
.card-only2 { background: #f0fdf4; border-color: #bbf7d0; }
.card-only2 .card-num { color: var(--success); }

/* 标签页 */
.tabs { display: flex; gap: 8px; margin-bottom: 14px; border-bottom: 2px solid var(--border); }
.tab {
  background: transparent;
  border: none;
  padding: 10px 16px;
  font-size: 14px;
  color: var(--muted);
  cursor: pointer;
  border-bottom: 2px solid transparent;
  margin-bottom: -2px;
  font-weight: 600;
  font-family: inherit;
}
.tab:hover { color: var(--text); }
.tab.active { color: var(--primary); border-bottom-color: var(--primary); }

.panel { display: none; }
.panel.active { display: block; }
.panel-title { display: none; font-size: 16px; margin-bottom: 10px; }

/* 表格 */
.table-wrap { max-height: 520px; overflow: auto; border: 1px solid var(--border); border-radius: 8px; }
.result-table { width: 100%; border-collapse: collapse; font-size: 13px; }
.result-table thead { position: sticky; top: 0; background: #f8fafc; }
.result-table th, .result-table td { padding: 9px 12px; text-align: left; border-bottom: 1px solid var(--border); }
.result-table th {
  font-weight: 700;
  color: var(--muted);
  font-size: 12px;
  background: #f8fafc;
  white-space: nowrap;
}
.result-table tbody tr:hover { background: #f8fafc; }
.col-idx { width: 40px; color: var(--muted); }
.col-loc { width: 160px; font-weight: 600; }

.pn-cell .pn-item { display: block; padding: 2px 0; }

/* 料号变更：两侧没有相同料号的位号，整行高亮 */
.result-table tbody tr.row-no-common { background: #fee2e2; }
.result-table tbody tr.row-no-common td { color: #b91c1c; font-weight: 600; }
.result-table tbody tr.row-no-common td.col-loc { box-shadow: inset 3px 0 0 var(--danger); }
.result-table tbody tr.row-no-common:hover { background: #fecaca; }

.empty-row { text-align: center; color: var(--muted); padding: 24px; }

/* 打印 / 导出 PDF：展开全部内容 */
@media print {
  body { background: #ffffff; }
  .container { max-width: none; padding: 0; }
  .card-block, .table-wrap { box-shadow: none; }
  .btn-link { display: none; }
  .tabs { display: none; }
  .panel { display: block !important; }
  .panel + .panel { margin-top: 24px; }
  .panel-title { display: block; }
  .table-wrap { max-height: none; overflow: visible; }
  .result-table { page-break-inside: auto; }
  .result-table tr { page-break-inside: avoid; }
  .result-table thead { display: table-header-group; }
}
";

        private const string Script = @"
(function () {
  var tabs = document.querySelectorAll('.tab');
  var panels = document.querySelectorAll('.panel');

  for (var i = 0; i < tabs.length; i++) {
    tabs[i].addEventListener('click', function () {
      for (var j = 0; j < tabs.length; j++) { tabs[j].classList.remove('active'); }
      for (var k = 0; k < panels.length; k++) { panels[k].classList.remove('active'); }
      this.classList.add('active');
      var target = document.getElementById('panel-' + this.getAttribute('data-tab'));
      if (target) { target.classList.add('active'); }
    });
  }

  var printBtn = document.getElementById('print-btn');
  if (printBtn) {
    printBtn.addEventListener('click', function () { window.print(); });
  }
})();
";
    }
}
