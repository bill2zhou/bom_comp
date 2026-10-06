using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BOM_compare.ui;
using BOM_compare.util;

namespace BOM_compare
{
    public partial class Form1 : Form
    {
        private const int OuterMargin = 28;     // 上下留白

        private readonly int contentWidth;      // 内容区宽度（与 .container 的 900 对应，已含缩放）
        private string bom1Path = "";
        private string bom2Path = "";
        private CompareResult lastResult;
        private int currentTab;                 // 0: 料号变更  1: 仅 BOM 1  2: 仅 BOM 2
        private readonly Timer toastTimer;
        private readonly Font boldCellFont;

        public Form1()
        {
            InitializeComponent();

            contentWidth = container.Width;
            toastTimer = new Timer(this.components) { Interval = 3000 };
            toastTimer.Tick += ToastTimer_Tick;
            boldCellFont = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Bold);

            Icon = LoadAppIcon();
            ClampToScreen();
            WireEvents();
        }

        /// <summary>加载内嵌的 app.ico 作为窗口/任务栏图标</summary>
        private static Icon LoadAppIcon()
        {
            try
            {
                using (Stream stream = typeof(Form1).Assembly.GetManifestResourceStream("BOM_compare.app.ico"))
                {
                    if (stream == null) return null;
                    // 取 32x32：任务栏为原生尺寸，标题栏由系统按比例缩小
                    return new Icon(stream, 32, 32);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>保证窗口不会超出当前屏幕工作区</summary>
        private void ClampToScreen()
        {
            Rectangle workArea = Screen.FromPoint(System.Windows.Forms.Cursor.Position).WorkingArea;
            int maxWidth = Math.Max(480, workArea.Width - 40);
            int maxHeight = Math.Max(360, workArea.Height - 80);

            System.Drawing.Size size = ClientSize;
            if (size.Width > maxWidth) size.Width = maxWidth;
            if (size.Height > maxHeight) size.Height = maxHeight;
            ClientSize = size;

            MinimumSize = new System.Drawing.Size(
                Math.Min(MinimumSize.Width, maxWidth),
                Math.Min(MinimumSize.Height, maxHeight));
        }

        private void WireEvents()
        {
            btnSelect1.Click += (s, e) => SelectFile(1);
            btnSelect2.Click += (s, e) => SelectFile(2);
            btnCompare.Click += Compare_Click;
            btnSaveReport.Click += SaveReport_Click;

            tabChanged.Click += (s, e) => SwitchTab(0);
            tabOnly1.Click += (s, e) => SwitchTab(1);
            tabOnly2.Click += (s, e) => SwitchTab(2);

            scrollPanel.Resize += (s, e) => LayoutUi();
            Shown += (s, e) => LayoutUi();
        }

        // ==================== 布局 ====================

        /// <summary>窗口/结果区变化时重新定位居中容器与 Toast</summary>
        private void LayoutUi()
        {
            if (scrollPanel == null || container == null) return;

            int available = scrollPanel.ClientSize.Width;
            if (available <= 0) return;

            container.Left = Math.Max(0, (available - contentWidth) / 2);
            container.Top = OuterMargin;

            int bottom = btnCompare.Bottom;
            if (resultCard.Visible) bottom = resultCard.Bottom;
            container.Height = bottom + OuterMargin;

            if (toast != null && toast.Visible)
            {
                toast.Left = Math.Max(0, (ClientSize.Width - toast.Width) / 2);
                toast.Top = Math.Max(0, ClientSize.Height - toast.Height - 24);
            }
        }

        // ==================== 选择文件 ====================

        private void SelectFile(int target)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "BOM Files|*.txt;*.csv;*.xls;*.xlsx|All Files|*.*";
                dialog.Title = target == 1 ? "选择 BOM 1 文件" : "选择 BOM 2 文件";

                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                if (target == 1)
                {
                    bom1Path = dialog.FileName;
                    ApplyFilePath(fileLabel1, fileBox1, bom1Path);
                }
                else
                {
                    bom2Path = dialog.FileName;
                    ApplyFilePath(fileLabel2, fileBox2, bom2Path);
                }
            }
        }

        /// <summary>对应 CSS 中 .file-input.has-file 的效果</summary>
        private static void ApplyFilePath(Label label, RoundedPanel box, string path)
        {
            label.Text = path;
            label.ForeColor = UiTheme.Text;
            box.BackColor = UiTheme.PrimaryLight;
            box.BorderColor = UiTheme.Primary;
            box.BorderDash = System.Drawing.Drawing2D.DashStyle.Solid;
            box.Invalidate();
        }

        // ==================== 开始对比 ====================

        private async void Compare_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bom1Path) || string.IsNullOrEmpty(bom2Path))
            {
                ShowToast("请先选择两个 BOM 文件！", true);
                return;
            }

            string file1 = bom1Path;
            string file2 = bom2Path;
            SetBusy(true);

            try
            {
                CompareResult result = await Task.Run(() => Compare.Analyze(file1, file2));

                lastResult = result;
                numChanged.Text = result.PartChanges.Count.ToString();
                numOnly1.Text = result.OnlyInBom1.Count.ToString();
                numOnly2.Text = result.OnlyInBom2.Count.ToString();

                // 表头改为文件名（不含扩展名）
                colPn1.HeaderText = result.Filename1;
                colPn2.HeaderText = result.Filename2;

                btnSaveReport.Visible = true;
                currentTab = 0;
                UpdateTabs();
                RenderTable();

                resultCard.Visible = true;
                LayoutUi();

                ShowToast(string.Format("对比完成，共 {0} 处差异", result.TotalDifferences));
            }
            catch (Exception ex)
            {
                ShowToast("对比失败: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            btnCompare.Enabled = !busy;
            btnCompare.Text = busy ? "对比中..." : "开始对比";
            btnSelect1.Enabled = !busy;
            btnSelect2.Enabled = !busy;
            UseWaitCursor = busy;
        }

        // ==================== 标签页与表格 ====================

        private void SwitchTab(int index)
        {
            if (currentTab == index) return;
            currentTab = index;
            UpdateTabs();
            RenderTable();
        }

        private void UpdateTabs()
        {
            tabChanged.Active = currentTab == 0;
            tabOnly1.Active = currentTab == 1;
            tabOnly2.Active = currentTab == 2;
        }

        private void RenderTable()
        {
            grid.Rows.Clear();
            if (lastResult == null) return;

            List<DiffItem> items;
            if (currentTab == 0) items = lastResult.PartChanges;
            else if (currentTab == 1) items = lastResult.OnlyInBom1;
            else items = lastResult.OnlyInBom2;

            if (items.Count == 0)
            {
                grid.Visible = false;
                emptyLabel.Visible = true;
                emptyLabel.BringToFront();
                return;
            }

            emptyLabel.Visible = false;
            grid.Visible = true;

            grid.SuspendLayout();
            for (int i = 0; i < items.Count; i++)
            {
                DiffItem item = items[i];
                int rowIndex = grid.Rows.Add(
                    (i + 1).ToString(),
                    item.RefDes,
                    JoinParts(item.PnList1),
                    JoinParts(item.PnList2));

                // 料号变更中，两侧没有相同料号的位号整行高亮
                if (currentTab == 0 && !item.HasCommonPart())
                {
                    DataGridViewRow row = grid.Rows[rowIndex];
                    row.DefaultCellStyle.BackColor = UiTheme.DangerLight;
                    row.DefaultCellStyle.ForeColor = UiTheme.DangerText;
                    row.DefaultCellStyle.SelectionBackColor = UiTheme.DangerBorder;
                    row.DefaultCellStyle.SelectionForeColor = UiTheme.DangerText;
                    row.DefaultCellStyle.Font = boldCellFont;
                }
            }
            grid.ResumeLayout();
        }

        private static string JoinParts(List<string> parts)
        {
            if (parts == null || parts.Count == 0) return "";
            return string.Join(Environment.NewLine, parts.Where(p => !string.IsNullOrEmpty(p)));
        }

        // ==================== 保存报告 ====================

        private void SaveReport_Click(object sender, EventArgs e)
        {
            if (lastResult == null) return;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "HTML 报告 (*.html)|*.html|文本文件 (*.txt)|*.txt";
                dialog.FilterIndex = 1;
                dialog.DefaultExt = "html";
                dialog.FileName = string.Format("BOM_diff_{0}_vs_{1}.html", lastResult.Filename1, lastResult.Filename2);

                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    // 默认输出与软件界面同款样式的 HTML 报告，另存 .txt 时回退为纯文本差异表
                    bool asHtml = !string.Equals(Path.GetExtension(dialog.FileName), ".txt", StringComparison.OrdinalIgnoreCase);
                    string content = asHtml ? HtmlReport.Build(lastResult) : Compare.BuildDiffText(lastResult);

                    File.WriteAllText(dialog.FileName, content, new UTF8Encoding(false));
                    ShowToast("报告已保存到: " + dialog.FileName);
                }
                catch (Exception ex)
                {
                    ShowToast("保存失败: " + ex.Message, true);
                }
            }
        }

        // ==================== Toast ====================

        private void ShowToast(string message, bool isError = false)
        {
            int maxWidth = Math.Max(200, ClientSize.Width - 40);
            int textWidth = TextRenderer.MeasureText(message, toastLabel.Font).Width;
            toast.Width = Math.Min(Math.Max(200, textWidth + 48), maxWidth);

            toastLabel.Text = message;
            toast.BackColor = isError ? UiTheme.Danger : UiTheme.ToastBg;
            toast.Visible = true;
            toast.BringToFront();
            LayoutUi();

            toastTimer.Stop();
            toastTimer.Start();
        }

        private void ToastTimer_Tick(object sender, EventArgs e)
        {
            toastTimer.Stop();
            toast.Visible = false;
        }
    }
}
