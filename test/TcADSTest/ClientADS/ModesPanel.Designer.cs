
namespace ClientADS
{
    partial class ModesPanel
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtRun = new System.Windows.Forms.Button();
            this.BtStop = new System.Windows.Forms.Button();
            this.BtRearm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtRun
            // 
            this.BtRun.Location = new System.Drawing.Point(167, 21);
            this.BtRun.Name = "BtRun";
            this.BtRun.Size = new System.Drawing.Size(75, 37);
            this.BtRun.TabIndex = 0;
            this.BtRun.Text = "Run";
            this.BtRun.UseVisualStyleBackColor = true;
            this.BtRun.Click += new System.EventHandler(this.button1_Click);
            // 
            // BtStop
            // 
            this.BtStop.Location = new System.Drawing.Point(248, 21);
            this.BtStop.Name = "BtStop";
            this.BtStop.Size = new System.Drawing.Size(75, 37);
            this.BtStop.TabIndex = 1;
            this.BtStop.Text = "Stop";
            this.BtStop.UseVisualStyleBackColor = true;
            // 
            // BtRearm
            // 
            this.BtRearm.Location = new System.Drawing.Point(167, 64);
            this.BtRearm.Name = "BtRearm";
            this.BtRearm.Size = new System.Drawing.Size(156, 37);
            this.BtRearm.TabIndex = 2;
            this.BtRearm.Text = "Rearm";
            this.BtRearm.UseVisualStyleBackColor = true;
            // 
            // ModesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BtRearm);
            this.Controls.Add(this.BtStop);
            this.Controls.Add(this.BtRun);
            this.Name = "ModesPanel";
            this.Size = new System.Drawing.Size(343, 235);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtRun;
        private System.Windows.Forms.Button BtStop;
        private System.Windows.Forms.Button BtRearm;
    }
}
