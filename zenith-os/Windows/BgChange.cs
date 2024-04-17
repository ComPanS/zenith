using Cosmos.System.Graphics;
using System.Security.Cryptography.X509Certificates;
using zenithos.Controls;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using IL2CPU.API.Attribs;

namespace zenithos.Windows
{
    internal class BgChange : Window
    {
        public Button clickButton = new("Click for change Bg", 20, 50, Color.Green, Kernel.defFont);
        public BgChange() : base(100, 100, 200, 100, "BgChange", Kernel.defFont, false)
        {
            controls.Add(clickButton);
        }

        bool newBg;
        
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);

            if(clickButton.clickedOnce)
            {
                newBg = !newBg;
            }

            if (newBg)
            {
                zenithos.Kernel.bg = null;
            }
            
        }
    }
}
