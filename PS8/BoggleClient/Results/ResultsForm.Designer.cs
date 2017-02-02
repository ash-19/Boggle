namespace BoggleClient
{
    partial class ResultsForm
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.listTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.remotePlayerTotalScoreLabel = new System.Windows.Forms.Label();
            this.homePlayerTotalScoreLabel = new System.Windows.Forms.Label();
            this.remotePlayerListTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.remotePlayerScoresListBox = new System.Windows.Forms.ListBox();
            this.remotePlayerWordListBox = new System.Windows.Forms.ListBox();
            this.remotePlayerResultsLabel = new System.Windows.Forms.Label();
            this.homePlayerResultsLabel = new System.Windows.Forms.Label();
            this.homePlayerListTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.homePlayerWordListBox = new System.Windows.Forms.ListBox();
            this.homePlayerScoresListBox = new System.Windows.Forms.ListBox();
            this.doneButtonResults = new System.Windows.Forms.Button();
            this.winnerLabel = new System.Windows.Forms.Label();
            this.mainTableLayout.SuspendLayout();
            this.listTableLayout.SuspendLayout();
            this.remotePlayerListTableLayout.SuspendLayout();
            this.homePlayerListTableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.listTableLayout, 0, 1);
            this.mainTableLayout.Controls.Add(this.doneButtonResults, 0, 2);
            this.mainTableLayout.Controls.Add(this.winnerLabel, 0, 0);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 3;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainTableLayout.Size = new System.Drawing.Size(564, 704);
            this.mainTableLayout.TabIndex = 1;
            // 
            // listTableLayout
            // 
            this.listTableLayout.ColumnCount = 2;
            this.listTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.listTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.listTableLayout.Controls.Add(this.remotePlayerTotalScoreLabel, 1, 2);
            this.listTableLayout.Controls.Add(this.homePlayerTotalScoreLabel, 0, 2);
            this.listTableLayout.Controls.Add(this.remotePlayerListTableLayout, 1, 1);
            this.listTableLayout.Controls.Add(this.remotePlayerResultsLabel, 1, 0);
            this.listTableLayout.Controls.Add(this.homePlayerResultsLabel, 0, 0);
            this.listTableLayout.Controls.Add(this.homePlayerListTableLayout, 0, 1);
            this.listTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listTableLayout.Location = new System.Drawing.Point(3, 71);
            this.listTableLayout.Name = "listTableLayout";
            this.listTableLayout.RowCount = 3;
            this.listTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.listTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.listTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.listTableLayout.Size = new System.Drawing.Size(558, 570);
            this.listTableLayout.TabIndex = 0;
            // 
            // remotePlayerTotalScoreLabel
            // 
            this.remotePlayerTotalScoreLabel.AutoSize = true;
            this.remotePlayerTotalScoreLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remotePlayerTotalScoreLabel.Font = new System.Drawing.Font("Segoe UI", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remotePlayerTotalScoreLabel.ForeColor = System.Drawing.Color.LightSlateGray;
            this.remotePlayerTotalScoreLabel.Location = new System.Drawing.Point(282, 507);
            this.remotePlayerTotalScoreLabel.Name = "remotePlayerTotalScoreLabel";
            this.remotePlayerTotalScoreLabel.Size = new System.Drawing.Size(273, 63);
            this.remotePlayerTotalScoreLabel.TabIndex = 5;
            this.remotePlayerTotalScoreLabel.Text = "0";
            this.remotePlayerTotalScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // homePlayerTotalScoreLabel
            // 
            this.homePlayerTotalScoreLabel.AutoSize = true;
            this.homePlayerTotalScoreLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homePlayerTotalScoreLabel.Font = new System.Drawing.Font("Segoe UI", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homePlayerTotalScoreLabel.ForeColor = System.Drawing.Color.LightSlateGray;
            this.homePlayerTotalScoreLabel.Location = new System.Drawing.Point(3, 507);
            this.homePlayerTotalScoreLabel.Name = "homePlayerTotalScoreLabel";
            this.homePlayerTotalScoreLabel.Size = new System.Drawing.Size(273, 63);
            this.homePlayerTotalScoreLabel.TabIndex = 4;
            this.homePlayerTotalScoreLabel.Text = "0";
            this.homePlayerTotalScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // remotePlayerListTableLayout
            // 
            this.remotePlayerListTableLayout.ColumnCount = 2;
            this.remotePlayerListTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.remotePlayerListTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.remotePlayerListTableLayout.Controls.Add(this.remotePlayerScoresListBox, 0, 0);
            this.remotePlayerListTableLayout.Controls.Add(this.remotePlayerWordListBox, 1, 0);
            this.remotePlayerListTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remotePlayerListTableLayout.Location = new System.Drawing.Point(282, 61);
            this.remotePlayerListTableLayout.Name = "remotePlayerListTableLayout";
            this.remotePlayerListTableLayout.RowCount = 1;
            this.remotePlayerListTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.remotePlayerListTableLayout.Size = new System.Drawing.Size(273, 443);
            this.remotePlayerListTableLayout.TabIndex = 3;
            // 
            // remotePlayerScoresListBox
            // 
            this.remotePlayerScoresListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.remotePlayerScoresListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remotePlayerScoresListBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remotePlayerScoresListBox.FormattingEnabled = true;
            this.remotePlayerScoresListBox.ItemHeight = 32;
            this.remotePlayerScoresListBox.Location = new System.Drawing.Point(3, 3);
            this.remotePlayerScoresListBox.Name = "remotePlayerScoresListBox";
            this.remotePlayerScoresListBox.Size = new System.Drawing.Size(46, 437);
            this.remotePlayerScoresListBox.TabIndex = 2;
            // 
            // remotePlayerWordListBox
            // 
            this.remotePlayerWordListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.remotePlayerWordListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remotePlayerWordListBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remotePlayerWordListBox.FormattingEnabled = true;
            this.remotePlayerWordListBox.ItemHeight = 32;
            this.remotePlayerWordListBox.Location = new System.Drawing.Point(55, 3);
            this.remotePlayerWordListBox.Name = "remotePlayerWordListBox";
            this.remotePlayerWordListBox.Size = new System.Drawing.Size(215, 437);
            this.remotePlayerWordListBox.TabIndex = 1;
            // 
            // remotePlayerResultsLabel
            // 
            this.remotePlayerResultsLabel.AutoSize = true;
            this.remotePlayerResultsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remotePlayerResultsLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remotePlayerResultsLabel.ForeColor = System.Drawing.Color.DimGray;
            this.remotePlayerResultsLabel.Location = new System.Drawing.Point(282, 0);
            this.remotePlayerResultsLabel.Name = "remotePlayerResultsLabel";
            this.remotePlayerResultsLabel.Size = new System.Drawing.Size(273, 58);
            this.remotePlayerResultsLabel.TabIndex = 1;
            this.remotePlayerResultsLabel.Text = "Remote Player";
            this.remotePlayerResultsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // homePlayerResultsLabel
            // 
            this.homePlayerResultsLabel.AutoSize = true;
            this.homePlayerResultsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homePlayerResultsLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homePlayerResultsLabel.ForeColor = System.Drawing.Color.DimGray;
            this.homePlayerResultsLabel.Location = new System.Drawing.Point(3, 0);
            this.homePlayerResultsLabel.Name = "homePlayerResultsLabel";
            this.homePlayerResultsLabel.Size = new System.Drawing.Size(273, 58);
            this.homePlayerResultsLabel.TabIndex = 0;
            this.homePlayerResultsLabel.Text = "Home Player";
            this.homePlayerResultsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // homePlayerListTableLayout
            // 
            this.homePlayerListTableLayout.ColumnCount = 2;
            this.homePlayerListTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.homePlayerListTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.homePlayerListTableLayout.Controls.Add(this.homePlayerWordListBox, 0, 0);
            this.homePlayerListTableLayout.Controls.Add(this.homePlayerScoresListBox, 1, 0);
            this.homePlayerListTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homePlayerListTableLayout.Location = new System.Drawing.Point(3, 61);
            this.homePlayerListTableLayout.Name = "homePlayerListTableLayout";
            this.homePlayerListTableLayout.RowCount = 1;
            this.homePlayerListTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.homePlayerListTableLayout.Size = new System.Drawing.Size(273, 443);
            this.homePlayerListTableLayout.TabIndex = 2;
            // 
            // homePlayerWordListBox
            // 
            this.homePlayerWordListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.homePlayerWordListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homePlayerWordListBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homePlayerWordListBox.FormattingEnabled = true;
            this.homePlayerWordListBox.ItemHeight = 32;
            this.homePlayerWordListBox.Location = new System.Drawing.Point(3, 3);
            this.homePlayerWordListBox.Name = "homePlayerWordListBox";
            this.homePlayerWordListBox.Size = new System.Drawing.Size(215, 437);
            this.homePlayerWordListBox.TabIndex = 0;
            // 
            // homePlayerScoresListBox
            // 
            this.homePlayerScoresListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.homePlayerScoresListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.homePlayerScoresListBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homePlayerScoresListBox.FormattingEnabled = true;
            this.homePlayerScoresListBox.ItemHeight = 32;
            this.homePlayerScoresListBox.Location = new System.Drawing.Point(224, 3);
            this.homePlayerScoresListBox.Name = "homePlayerScoresListBox";
            this.homePlayerScoresListBox.Size = new System.Drawing.Size(46, 437);
            this.homePlayerScoresListBox.TabIndex = 1;
            // 
            // doneButtonResults
            // 
            this.doneButtonResults.BackColor = System.Drawing.Color.LemonChiffon;
            this.doneButtonResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doneButtonResults.FlatAppearance.BorderSize = 0;
            this.doneButtonResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.doneButtonResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.doneButtonResults.ForeColor = System.Drawing.SystemColors.GrayText;
            this.doneButtonResults.Location = new System.Drawing.Point(3, 647);
            this.doneButtonResults.Name = "doneButtonResults";
            this.doneButtonResults.Size = new System.Drawing.Size(558, 54);
            this.doneButtonResults.TabIndex = 1;
            this.doneButtonResults.Text = "Sweet! (if won) Oh well (if lost)";
            this.doneButtonResults.UseVisualStyleBackColor = false;
            this.doneButtonResults.Click += new System.EventHandler(this.doneButtonResults_Click);
            this.doneButtonResults.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.doneButtonResults_KeyPress);
            // 
            // winnerLabel
            // 
            this.winnerLabel.AutoSize = true;
            this.winnerLabel.BackColor = System.Drawing.Color.LightPink;
            this.winnerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winnerLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.winnerLabel.ForeColor = System.Drawing.Color.PaleVioletRed;
            this.winnerLabel.Location = new System.Drawing.Point(3, 0);
            this.winnerLabel.Name = "winnerLabel";
            this.winnerLabel.Size = new System.Drawing.Size(558, 68);
            this.winnerLabel.TabIndex = 2;
            this.winnerLabel.Text = "You won! or You Lost!";
            this.winnerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 704);
            this.Controls.Add(this.mainTableLayout);
            this.Name = "ResultsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Results";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ResultsForm_FormClosed);
            this.mainTableLayout.ResumeLayout(false);
            this.mainTableLayout.PerformLayout();
            this.listTableLayout.ResumeLayout(false);
            this.listTableLayout.PerformLayout();
            this.remotePlayerListTableLayout.ResumeLayout(false);
            this.homePlayerListTableLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.TableLayoutPanel listTableLayout;
        private System.Windows.Forms.Label homePlayerResultsLabel;
        private System.Windows.Forms.Label remotePlayerResultsLabel;
        private System.Windows.Forms.TableLayoutPanel remotePlayerListTableLayout;
        private System.Windows.Forms.TableLayoutPanel homePlayerListTableLayout;
        private System.Windows.Forms.Label remotePlayerTotalScoreLabel;
        private System.Windows.Forms.Label homePlayerTotalScoreLabel;
        private System.Windows.Forms.ListBox homePlayerWordListBox;
        private System.Windows.Forms.ListBox remotePlayerWordListBox;
        private System.Windows.Forms.ListBox homePlayerScoresListBox;
        private System.Windows.Forms.ListBox remotePlayerScoresListBox;
        private System.Windows.Forms.Button doneButtonResults;
        private System.Windows.Forms.Label winnerLabel;
    }
}