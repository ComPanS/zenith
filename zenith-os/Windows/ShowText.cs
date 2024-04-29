using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zenithos.Controls;

namespace zenithos.Windows
{
    internal class ShowText : Window
    {
        public Button addTxt = new("New txt File", 20, 20, Color.Green, Kernel.defFont);
        public Button lastTxt = new("The Last txt", 20, 60, Color.FromArgb(57, 64, 69), Kernel.defFont);
        public ShowText() : base(100, 100, 200, 100, "Show txt Files", Kernel.defFont, true)
        {
            controls.Add(addTxt);
            controls.Add(lastTxt);
        }

        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);

            if (addTxt.clickedOnce)
            {
                Window instance = new AddText();
                Kernel.windows.Remove(Kernel.windows.Last());
                zenithos.Kernel.windows.Add(instance);
            }

            if (lastTxt.clickedOnce)
            {
                Window instance = new TheLastTxt();
                Kernel.windows.Remove(Kernel.windows.Last());
                zenithos.Kernel.windows.Add(instance);
            }

        }
    }
}
