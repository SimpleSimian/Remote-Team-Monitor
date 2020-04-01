using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_Team_Monitor
{
    public partial class NotificationPopup : Form
    {
        int MinimumPosition = 0;
        int MaxPosition = Screen.PrimaryScreen.WorkingArea.Height;

        int VisibleTime = 5000;
        String displayMessage = "";

        public NotificationPopup(string Message)
        {
            InitializeComponent();
            displayMessage = Message;
        }

        private void Hide_Tick(object sender, EventArgs e)
        {
            if (this.Location.Y < MaxPosition)
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width, this.Location.Y + 5);
            else if (this.Location.Y > MinimumPosition)
                this.Close();

            if (this.Location.X < Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width)
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width, this.Location.Y);
        }

        private void Reveal_Tick(object sender, EventArgs e)
        {
            VisibleTime -= 10;
            if (this.Location.Y > MinimumPosition)
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width, this.Location.Y - 5);
            else if (this.Location.Y < MinimumPosition)
                this.Location = new Point(this.Location.X, MinimumPosition);

            if (this.Location.X < Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width)
                this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width, this.Location.Y);

            if (VisibleTime < 0)
            {
                Reveal.Enabled = false;
                Hide.Enabled = true;
            }
        }

        private void NotificationPopup_Load(object sender, EventArgs e)
        {
            MinimumPosition = Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height;
            Reveal.Enabled = true;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width, Screen.PrimaryScreen.WorkingArea.Height);
            lblMessage.Text = displayMessage;
        }
    }
}
