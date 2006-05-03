using System;
using System.Windows.Forms;
using System.Drawing;

namespace CustomControls
{
    public class ScrollablePanel : Panel
    {
        public Panel Contents
        {
            get { return contents; }
        }

        Panel contents;
        VScrollBar vScroll;

        public ScrollablePanel()
        {
            this.vScroll = new VScrollBar();
            this.vScroll.Parent = this;
            this.vScroll.Visible = true;
            this.vScroll.Minimum = 0;
            this.vScroll.SmallChange = 20;
            this.vScroll.ValueChanged += new EventHandler (this.scrollbar_ValueChanged);
            this.contents = new Panel();
            this.contents.Parent = this;
            this.contents.Width = this.ClientSize.Width - this.vScroll.Size.Width;
            this.contents.SendToBack();  

        }

        public void scrollbar_ValueChanged (object o, EventArgs e)
        {
            if (o == this.vScroll)
            {
                //By decreasing the top y coordinate
                //the contents panel appears to scroll
                this.contents.Top = -this.vScroll.Value;
                this.Update();
            }
        }

        void CheckScrollBars()
        {
            this.vScroll.Value =  this.vScroll.Minimum;
            this.vScroll.Visible = this.contents.Size.Height > this.ClientSize.Height;
        }

        protected override void OnResize (EventArgs e)
        {
            this.contents.Width = this.ClientSize.Width - this.vScroll.Size.Width;

            this.vScroll.Bounds = new Rectangle (this.ClientSize.Width - this.vScroll.Size.Width, 0, this.vScroll.Size.Width, 
                                                                                   this.ClientSize.Height);

            if (this.ClientSize.Height >= 0) {  this.vScroll.LargeChange = this.ClientSize.Height; }

            CheckScrollBars();
        }

        public void SetScrollHeight(int height)
        {
            this.contents.Height = height;

            this.vScroll.Maximum = this.contents.Size.Height;

            CheckScrollBars();

            this.vScroll.Left =  this.Contents.Width;
        }


    }
}