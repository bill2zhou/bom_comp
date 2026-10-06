using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BOM_compare.ui;

namespace BOM_compare
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// 界面结构与 Tauri 版 src/index.html 一一对应：
        /// header(标题/副标题) -> file-section(文件卡片) -> action-section(主按钮) -> result-section(结果卡片)
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.scrollPanel = new BufferedPanel();
            this.container = new BufferedPanel();

            this.titleLabel = new Label();
            this.subtitleLabel = new Label();

            this.fileCard = new RoundedPanel();
            this.btnSelect1 = new RoundedButton();
            this.fileBox1 = new RoundedPanel();
            this.fileLabel1 = new Label();
            this.btnSelect2 = new RoundedButton();
            this.fileBox2 = new RoundedPanel();
            this.fileLabel2 = new Label();

            this.btnCompare = new RoundedButton();

            this.resultCard = new RoundedPanel();
            this.resultTitle = new Label();
            this.btnSaveReport = new RoundedButton();
            this.cardChanged = new RoundedPanel();
            this.numChanged = new Label();
            this.lblChanged = new Label();
            this.cardOnly1 = new RoundedPanel();
            this.numOnly1 = new Label();
            this.lblOnly1 = new Label();
            this.cardOnly2 = new RoundedPanel();
            this.numOnly2 = new Label();
            this.lblOnly2 = new Label();
            this.tabChanged = new TabButton();
            this.tabOnly1 = new TabButton();
            this.tabOnly2 = new TabButton();
            this.tabLine = new Panel();
            this.grid = new DataGridView();
            this.colIdx = new DataGridViewTextBoxColumn();
            this.colLoc = new DataGridViewTextBoxColumn();
            this.colPn1 = new DataGridViewTextBoxColumn();
            this.colPn2 = new DataGridViewTextBoxColumn();
            this.emptyLabel = new Label();

            this.toast = new RoundedPanel();
            this.toastLabel = new Label();

            this.scrollPanel.SuspendLayout();
            this.container.SuspendLayout();
            this.fileCard.SuspendLayout();
            this.fileBox1.SuspendLayout();
            this.fileBox2.SuspendLayout();
            this.resultCard.SuspendLayout();
            this.cardChanged.SuspendLayout();
            this.cardOnly1.SuspendLayout();
            this.cardOnly2.SuspendLayout();
            this.toast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();

            // ==================== 滚动容器 ====================
            // scrollPanel
            this.scrollPanel.AutoScroll = true;
            this.scrollPanel.BackColor = UiTheme.Bg;
            this.scrollPanel.Dock = DockStyle.Fill;
            this.scrollPanel.Location = new Point(0, 0);
            this.scrollPanel.Name = "scrollPanel";
            this.scrollPanel.Size = new Size(944, 721);
            this.scrollPanel.TabIndex = 0;

            // container （对应 .container，最大宽度 900 居中）
            this.container.BackColor = UiTheme.Bg;
            this.container.Location = new Point(22, 28);
            this.container.Name = "container";
            this.container.Size = new Size(900, 400);
            this.container.TabIndex = 0;

            // ==================== 头部 ====================
            // titleLabel
            this.titleLabel.BackColor = Color.Transparent;
            this.titleLabel.Font = new Font(UiTheme.FontFamily, 19.5F, FontStyle.Bold);
            this.titleLabel.ForeColor = UiTheme.Text;
            this.titleLabel.Location = new Point(0, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new Size(900, 40);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "BOM 对比工具";
            this.titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // subtitleLabel
            this.subtitleLabel.BackColor = Color.Transparent;
            this.subtitleLabel.Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Regular);
            this.subtitleLabel.ForeColor = UiTheme.Muted;
            this.subtitleLabel.Location = new Point(0, 42);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new Size(900, 24);
            this.subtitleLabel.TabIndex = 0;
            this.subtitleLabel.Text = "对比两个 BOM 文件的位号与料号差异";
            this.subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // ==================== 文件选择卡片 ====================
            // fileCard
            this.fileCard.BackColor = UiTheme.Card;
            this.fileCard.BorderColor = UiTheme.Border;
            this.fileCard.BorderWidth = 1f;
            this.fileCard.Location = new Point(0, 94);
            this.fileCard.Name = "fileCard";
            this.fileCard.Padding = new Padding(20);
            this.fileCard.Radius = 12;
            this.fileCard.Size = new Size(900, 142);
            this.fileCard.TabIndex = 0;

            // btnSelect1
            this.btnSelect1.Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Bold);
            this.btnSelect1.Location = new Point(20, 20);
            this.btnSelect1.Name = "btnSelect1";
            this.btnSelect1.Radius = 8;
            this.btnSelect1.Size = new Size(130, 44);
            this.btnSelect1.TabIndex = 0;
            this.btnSelect1.Text = "选择 BOM 1";
            this.btnSelect1.Variant = ButtonVariant.Select;

            // fileBox1
            this.fileBox1.BackColor = UiTheme.Surface;
            this.fileBox1.BorderColor = UiTheme.Border;
            this.fileBox1.BorderDash = DashStyle.Dash;
            this.fileBox1.BorderWidth = 1f;
            this.fileBox1.Location = new Point(162, 20);
            this.fileBox1.Name = "fileBox1";
            this.fileBox1.Padding = new Padding(14, 2, 14, 2);
            this.fileBox1.Radius = 8;
            this.fileBox1.Size = new Size(718, 44);
            this.fileBox1.TabIndex = 0;

            // fileLabel1
            this.fileLabel1.AutoEllipsis = true;
            this.fileLabel1.BackColor = Color.Transparent;
            this.fileLabel1.Dock = DockStyle.Fill;
            this.fileLabel1.Font = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Regular);
            this.fileLabel1.ForeColor = UiTheme.Muted;
            this.fileLabel1.Location = new Point(14, 2);
            this.fileLabel1.Name = "fileLabel1";
            this.fileLabel1.Size = new Size(690, 40);
            this.fileLabel1.TabIndex = 0;
            this.fileLabel1.Text = "尚未选择 BOM 1 文件";
            this.fileLabel1.TextAlign = ContentAlignment.MiddleLeft;

            // btnSelect2
            this.btnSelect2.Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Bold);
            this.btnSelect2.Location = new Point(20, 78);
            this.btnSelect2.Name = "btnSelect2";
            this.btnSelect2.Radius = 8;
            this.btnSelect2.Size = new Size(130, 44);
            this.btnSelect2.TabIndex = 1;
            this.btnSelect2.Text = "选择 BOM 2";
            this.btnSelect2.Variant = ButtonVariant.Select;

            // fileBox2
            this.fileBox2.BackColor = UiTheme.Surface;
            this.fileBox2.BorderColor = UiTheme.Border;
            this.fileBox2.BorderDash = DashStyle.Dash;
            this.fileBox2.BorderWidth = 1f;
            this.fileBox2.Location = new Point(162, 78);
            this.fileBox2.Name = "fileBox2";
            this.fileBox2.Padding = new Padding(14, 2, 14, 2);
            this.fileBox2.Radius = 8;
            this.fileBox2.Size = new Size(718, 44);
            this.fileBox2.TabIndex = 1;

            // fileLabel2
            this.fileLabel2.AutoEllipsis = true;
            this.fileLabel2.BackColor = Color.Transparent;
            this.fileLabel2.Dock = DockStyle.Fill;
            this.fileLabel2.Font = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Regular);
            this.fileLabel2.ForeColor = UiTheme.Muted;
            this.fileLabel2.Location = new Point(14, 2);
            this.fileLabel2.Name = "fileLabel2";
            this.fileLabel2.Size = new Size(690, 40);
            this.fileLabel2.TabIndex = 0;
            this.fileLabel2.Text = "尚未选择 BOM 2 文件";
            this.fileLabel2.TextAlign = ContentAlignment.MiddleLeft;

            this.fileCard.Controls.Add(this.btnSelect1);
            this.fileCard.Controls.Add(this.fileBox1);
            this.fileCard.Controls.Add(this.btnSelect2);
            this.fileCard.Controls.Add(this.fileBox2);
            this.fileBox1.Controls.Add(this.fileLabel1);
            this.fileBox2.Controls.Add(this.fileLabel2);

            // ==================== 主按钮 ====================
            // btnCompare
            this.btnCompare.Font = new Font(UiTheme.FontFamily, 11.25F, FontStyle.Bold);
            this.btnCompare.Location = new Point(370, 256);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Radius = 8;
            this.btnCompare.Size = new Size(160, 45);
            this.btnCompare.TabIndex = 2;
            this.btnCompare.Text = "开始对比";
            this.btnCompare.Variant = ButtonVariant.Primary;

            // ==================== 结果卡片 ====================
            // resultCard
            this.resultCard.BackColor = UiTheme.Card;
            this.resultCard.BorderColor = UiTheme.Border;
            this.resultCard.BorderWidth = 1f;
            this.resultCard.Location = new Point(0, 325);
            this.resultCard.Name = "resultCard";
            this.resultCard.Padding = new Padding(20);
            this.resultCard.Radius = 12;
            this.resultCard.Size = new Size(900, 550);
            this.resultCard.TabIndex = 3;
            this.resultCard.Visible = false;

            // resultTitle
            this.resultTitle.AutoSize = true;
            this.resultTitle.BackColor = Color.Transparent;
            this.resultTitle.Font = new Font(UiTheme.FontFamily, 13.5F, FontStyle.Bold);
            this.resultTitle.ForeColor = UiTheme.Text;
            this.resultTitle.Location = new Point(20, 20);
            this.resultTitle.Name = "resultTitle";
            this.resultTitle.Size = new Size(80, 25);
            this.resultTitle.TabIndex = 0;
            this.resultTitle.Text = "对比结果";

            // btnSaveReport
            this.btnSaveReport.Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Bold);
            this.btnSaveReport.Location = new Point(752, 16);
            this.btnSaveReport.Name = "btnSaveReport";
            this.btnSaveReport.Radius = 8;
            this.btnSaveReport.Size = new Size(130, 34);
            this.btnSaveReport.TabIndex = 3;
            this.btnSaveReport.Text = "保存报告文件";
            this.btnSaveReport.Variant = ButtonVariant.Link;
            this.btnSaveReport.Visible = false;

            // cardChanged
            this.cardChanged.BackColor = UiTheme.DangerLight;
            this.cardChanged.BorderColor = UiTheme.DangerBorder;
            this.cardChanged.BorderWidth = 1f;
            this.cardChanged.Location = new Point(20, 66);
            this.cardChanged.Name = "cardChanged";
            this.cardChanged.Radius = 10;
            this.cardChanged.Size = new Size(278, 92);
            this.cardChanged.TabIndex = 0;

            // numChanged
            this.numChanged.BackColor = Color.Transparent;
            this.numChanged.Font = new Font(UiTheme.FontFamily, 22.5F, FontStyle.Bold);
            this.numChanged.ForeColor = UiTheme.Danger;
            this.numChanged.Location = new Point(0, 12);
            this.numChanged.Name = "numChanged";
            this.numChanged.Size = new Size(278, 44);
            this.numChanged.TabIndex = 0;
            this.numChanged.Text = "0";
            this.numChanged.TextAlign = ContentAlignment.MiddleCenter;

            // lblChanged
            this.lblChanged.BackColor = Color.Transparent;
            this.lblChanged.Font = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Regular);
            this.lblChanged.ForeColor = UiTheme.Muted;
            this.lblChanged.Location = new Point(0, 58);
            this.lblChanged.Name = "lblChanged";
            this.lblChanged.Size = new Size(278, 22);
            this.lblChanged.TabIndex = 0;
            this.lblChanged.Text = "料号变更";
            this.lblChanged.TextAlign = ContentAlignment.MiddleCenter;

            this.cardChanged.Controls.Add(this.numChanged);
            this.cardChanged.Controls.Add(this.lblChanged);

            // cardOnly1
            this.cardOnly1.BackColor = UiTheme.WarningLight;
            this.cardOnly1.BorderColor = UiTheme.WarningBorder;
            this.cardOnly1.BorderWidth = 1f;
            this.cardOnly1.Location = new Point(310, 66);
            this.cardOnly1.Name = "cardOnly1";
            this.cardOnly1.Radius = 10;
            this.cardOnly1.Size = new Size(278, 92);
            this.cardOnly1.TabIndex = 0;

            // numOnly1
            this.numOnly1.BackColor = Color.Transparent;
            this.numOnly1.Font = new Font(UiTheme.FontFamily, 22.5F, FontStyle.Bold);
            this.numOnly1.ForeColor = UiTheme.Warning;
            this.numOnly1.Location = new Point(0, 12);
            this.numOnly1.Name = "numOnly1";
            this.numOnly1.Size = new Size(278, 44);
            this.numOnly1.TabIndex = 0;
            this.numOnly1.Text = "0";
            this.numOnly1.TextAlign = ContentAlignment.MiddleCenter;

            // lblOnly1
            this.lblOnly1.BackColor = Color.Transparent;
            this.lblOnly1.Font = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Regular);
            this.lblOnly1.ForeColor = UiTheme.Muted;
            this.lblOnly1.Location = new Point(0, 58);
            this.lblOnly1.Name = "lblOnly1";
            this.lblOnly1.Size = new Size(278, 22);
            this.lblOnly1.TabIndex = 0;
            this.lblOnly1.Text = "仅 BOM 1";
            this.lblOnly1.TextAlign = ContentAlignment.MiddleCenter;

            this.cardOnly1.Controls.Add(this.numOnly1);
            this.cardOnly1.Controls.Add(this.lblOnly1);

            // cardOnly2
            this.cardOnly2.BackColor = UiTheme.SuccessLight;
            this.cardOnly2.BorderColor = UiTheme.SuccessBorder;
            this.cardOnly2.BorderWidth = 1f;
            this.cardOnly2.Location = new Point(600, 66);
            this.cardOnly2.Name = "cardOnly2";
            this.cardOnly2.Radius = 10;
            this.cardOnly2.Size = new Size(278, 92);
            this.cardOnly2.TabIndex = 0;

            // numOnly2
            this.numOnly2.BackColor = Color.Transparent;
            this.numOnly2.Font = new Font(UiTheme.FontFamily, 22.5F, FontStyle.Bold);
            this.numOnly2.ForeColor = UiTheme.Success;
            this.numOnly2.Location = new Point(0, 12);
            this.numOnly2.Name = "numOnly2";
            this.numOnly2.Size = new Size(278, 44);
            this.numOnly2.TabIndex = 0;
            this.numOnly2.Text = "0";
            this.numOnly2.TextAlign = ContentAlignment.MiddleCenter;

            // lblOnly2
            this.lblOnly2.BackColor = Color.Transparent;
            this.lblOnly2.Font = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Regular);
            this.lblOnly2.ForeColor = UiTheme.Muted;
            this.lblOnly2.Location = new Point(0, 58);
            this.lblOnly2.Name = "lblOnly2";
            this.lblOnly2.Size = new Size(278, 22);
            this.lblOnly2.TabIndex = 0;
            this.lblOnly2.Text = "仅 BOM 2";
            this.lblOnly2.TextAlign = ContentAlignment.MiddleCenter;

            this.cardOnly2.Controls.Add(this.numOnly2);
            this.cardOnly2.Controls.Add(this.lblOnly2);

            // tabChanged
            this.tabChanged.Active = true;
            this.tabChanged.Location = new Point(20, 176);
            this.tabChanged.Name = "tabChanged";
            this.tabChanged.Size = new Size(100, 40);
            this.tabChanged.TabIndex = 0;
            this.tabChanged.Text = "料号变更";

            // tabOnly1
            this.tabOnly1.Location = new Point(128, 176);
            this.tabOnly1.Name = "tabOnly1";
            this.tabOnly1.Size = new Size(100, 40);
            this.tabOnly1.TabIndex = 1;
            this.tabOnly1.Text = "仅 BOM 1";

            // tabOnly2
            this.tabOnly2.Location = new Point(236, 176);
            this.tabOnly2.Name = "tabOnly2";
            this.tabOnly2.Size = new Size(100, 40);
            this.tabOnly2.TabIndex = 2;
            this.tabOnly2.Text = "仅 BOM 2";

            // tabLine （对应 .tabs 的 border-bottom，贯穿整行；置于标签页下层）
            this.tabLine.BackColor = UiTheme.Border;
            this.tabLine.Location = new Point(20, 214);
            this.tabLine.Name = "tabLine";
            this.tabLine.Size = new Size(860, 2);
            this.tabLine.TabIndex = 9;

            // grid
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeColumns = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this.grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.grid.BackgroundColor = Color.White;
            this.grid.BorderStyle = BorderStyle.FixedSingle;
            this.grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            this.grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.Surface;
            this.grid.ColumnHeadersDefaultCellStyle.Font = new Font(UiTheme.FontFamily, 9F, FontStyle.Bold);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Muted;
            this.grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            this.grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiTheme.Surface;
            this.grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = UiTheme.Muted;
            this.grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            this.grid.ColumnHeadersHeight = 36;
            this.grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.Columns.AddRange(new DataGridViewColumn[] {
                this.colIdx, this.colLoc, this.colPn1, this.colPn2});
            this.grid.DefaultCellStyle.BackColor = Color.White;
            this.grid.DefaultCellStyle.Font = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Regular);
            this.grid.DefaultCellStyle.ForeColor = UiTheme.Text;
            this.grid.DefaultCellStyle.Padding = new Padding(10, 8, 10, 8);
            this.grid.DefaultCellStyle.SelectionBackColor = UiTheme.PrimaryLight;
            this.grid.DefaultCellStyle.SelectionForeColor = UiTheme.Text;
            this.grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.grid.EnableHeadersVisualStyles = false;
            this.grid.GridColor = UiTheme.Border;
            this.grid.Location = new Point(20, 230);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 34;
            this.grid.ScrollBars = ScrollBars.Vertical;
            this.grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new Size(860, 300);
            this.grid.TabIndex = 3;
            this.grid.TabStop = false;

            // colIdx
            this.colIdx.DefaultCellStyle.ForeColor = UiTheme.Muted;
            this.colIdx.HeaderText = "#";
            this.colIdx.Name = "colIdx";
            this.colIdx.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.colIdx.Width = 46;

            // colLoc
            this.colLoc.DefaultCellStyle.Font = new Font(UiTheme.FontFamily, 9.75F, FontStyle.Bold);
            this.colLoc.HeaderText = "Location";
            this.colLoc.Name = "colLoc";
            this.colLoc.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.colLoc.Width = 160;

            // colPn1
            this.colPn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.colPn1.HeaderText = "BOM 1 料号";
            this.colPn1.Name = "colPn1";
            this.colPn1.SortMode = DataGridViewColumnSortMode.NotSortable;

            // colPn2
            this.colPn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.colPn2.HeaderText = "BOM 2 料号";
            this.colPn2.Name = "colPn2";
            this.colPn2.SortMode = DataGridViewColumnSortMode.NotSortable;

            // emptyLabel （对应 .empty-row 空态）
            this.emptyLabel.BackColor = Color.White;
            this.emptyLabel.BorderStyle = BorderStyle.FixedSingle;
            this.emptyLabel.Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Regular);
            this.emptyLabel.ForeColor = UiTheme.Muted;
            this.emptyLabel.Location = new Point(20, 230);
            this.emptyLabel.Name = "emptyLabel";
            this.emptyLabel.Size = new Size(860, 300);
            this.emptyLabel.TabIndex = 4;
            this.emptyLabel.Text = "(None) — 无差异";
            this.emptyLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.emptyLabel.Visible = false;

            this.resultCard.Controls.Add(this.resultTitle);
            this.resultCard.Controls.Add(this.btnSaveReport);
            this.resultCard.Controls.Add(this.cardChanged);
            this.resultCard.Controls.Add(this.cardOnly1);
            this.resultCard.Controls.Add(this.cardOnly2);
            this.resultCard.Controls.Add(this.tabChanged);
            this.resultCard.Controls.Add(this.tabOnly1);
            this.resultCard.Controls.Add(this.tabOnly2);
            this.resultCard.Controls.Add(this.tabLine);
            this.resultCard.Controls.Add(this.grid);
            this.resultCard.Controls.Add(this.emptyLabel);
            this.tabLine.SendToBack();

            // ==================== 组装 ====================
            this.container.Controls.Add(this.titleLabel);
            this.container.Controls.Add(this.subtitleLabel);
            this.container.Controls.Add(this.fileCard);
            this.container.Controls.Add(this.btnCompare);
            this.container.Controls.Add(this.resultCard);

            this.scrollPanel.Controls.Add(this.container);

            // toast （对应 .toast，固定显示在窗口底部居中）
            this.toast.BackColor = UiTheme.ToastBg;
            this.toast.BorderWidth = 0f;
            this.toast.Location = new Point(322, 650);
            this.toast.Name = "toast";
            this.toast.Radius = 8;
            this.toast.Size = new Size(300, 44);
            this.toast.TabIndex = 0;
            this.toast.Visible = false;

            // toastLabel
            this.toastLabel.AutoEllipsis = true;
            this.toastLabel.BackColor = Color.Transparent;
            this.toastLabel.Dock = DockStyle.Fill;
            this.toastLabel.Font = new Font(UiTheme.FontFamily, 10.5F, FontStyle.Regular);
            this.toastLabel.ForeColor = Color.White;
            this.toastLabel.Location = new Point(14, 0);
            this.toastLabel.Name = "toastLabel";
            this.toastLabel.Padding = new Padding(14, 0, 14, 0);
            this.toastLabel.Size = new Size(272, 44);
            this.toastLabel.TabIndex = 0;
            this.toastLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.toast.Controls.Add(this.toastLabel);

            // ==================== Form1 ====================
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            // 采用固定像素布局，避免字体度量差异导致整体缩放错位（高分屏由系统统一缩放）
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = UiTheme.Bg;
            this.ClientSize = new Size(944, 700);
            this.Controls.Add(this.scrollPanel);
            this.Controls.Add(this.toast);
            this.DoubleBuffered = true;
            this.Font = new Font(UiTheme.FontFamily, 9F, FontStyle.Regular);
            this.MinimumSize = new Size(960, 640);
            this.Name = "Form1";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "BOM 对比工具";

            this.scrollPanel.ResumeLayout(false);
            this.container.ResumeLayout(false);
            this.fileCard.ResumeLayout(false);
            this.fileBox1.ResumeLayout(false);
            this.fileBox2.ResumeLayout(false);
            this.resultCard.ResumeLayout(false);
            this.resultCard.PerformLayout();
            this.cardChanged.ResumeLayout(false);
            this.cardOnly1.ResumeLayout(false);
            this.cardOnly2.ResumeLayout(false);
            this.toast.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private BufferedPanel scrollPanel;
        private BufferedPanel container;

        private Label titleLabel;
        private Label subtitleLabel;

        private RoundedPanel fileCard;
        private RoundedButton btnSelect1;
        private RoundedPanel fileBox1;
        private Label fileLabel1;
        private RoundedButton btnSelect2;
        private RoundedPanel fileBox2;
        private Label fileLabel2;

        private RoundedButton btnCompare;

        private RoundedPanel resultCard;
        private Label resultTitle;
        private RoundedButton btnSaveReport;
        private RoundedPanel cardChanged;
        private Label numChanged;
        private Label lblChanged;
        private RoundedPanel cardOnly1;
        private Label numOnly1;
        private Label lblOnly1;
        private RoundedPanel cardOnly2;
        private Label numOnly2;
        private Label lblOnly2;
        private TabButton tabChanged;
        private TabButton tabOnly1;
        private TabButton tabOnly2;
        private Panel tabLine;
        private DataGridView grid;
        private DataGridViewTextBoxColumn colIdx;
        private DataGridViewTextBoxColumn colLoc;
        private DataGridViewTextBoxColumn colPn1;
        private DataGridViewTextBoxColumn colPn2;
        private Label emptyLabel;

        private RoundedPanel toast;
        private Label toastLabel;
    }
}
