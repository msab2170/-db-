
namespace UpdateMailInfo
{
    partial class UpdateMailInfo
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.TotalCount = new System.Windows.Forms.Label();
            this.ReadCount = new System.Windows.Forms.Label();
            this.WillReadCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TotalCount
            // 
            this.TotalCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TotalCount.AutoSize = true;
            this.TotalCount.Location = new System.Drawing.Point(413, 30);
            this.TotalCount.Margin = new System.Windows.Forms.Padding(0);
            this.TotalCount.Name = "TotalCount";
            this.TotalCount.Size = new System.Drawing.Size(15, 15);
            this.TotalCount.TabIndex = 6;
            this.TotalCount.Text = "0";
            this.TotalCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ReadCount
            // 
            this.ReadCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReadCount.AutoSize = true;
            this.ReadCount.Location = new System.Drawing.Point(209, 30);
            this.ReadCount.Margin = new System.Windows.Forms.Padding(0);
            this.ReadCount.Name = "ReadCount";
            this.ReadCount.Size = new System.Drawing.Size(15, 15);
            this.ReadCount.TabIndex = 5;
            this.ReadCount.Text = "0";
            this.ReadCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WillReadCount
            // 
            this.WillReadCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WillReadCount.AutoSize = true;
            this.WillReadCount.Location = new System.Drawing.Point(5, 30);
            this.WillReadCount.Margin = new System.Windows.Forms.Padding(0);
            this.WillReadCount.Name = "WillReadCount";
            this.WillReadCount.Size = new System.Drawing.Size(15, 15);
            this.WillReadCount.TabIndex = 3;
            this.WillReadCount.Text = "0";
            this.WillReadCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UpdateMailInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 67);
            this.Controls.Add(this.TotalCount);
            this.Controls.Add(this.ReadCount);
            this.Controls.Add(this.WillReadCount);
            this.Name = "UpdateMailInfo";
            this.Text = "update the mail information";
            this.Load += new System.EventHandler(this.UpdateMailInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label TotalCount;
        private System.Windows.Forms.Label ReadCount;
        private System.Windows.Forms.Label WillReadCount;
    }
}

