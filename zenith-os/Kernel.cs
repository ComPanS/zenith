using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using zenithos.Controls;
using zenithos.Windows;
using System.Linq;
using Sys = Cosmos.System;
using Cosmos.Core.Memory;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.Audio;

namespace zenithos
{
    public class Kernel : Sys.Kernel
    {
        public static readonly Color bgCol = Color.FromArgb(31, 32, 33);
        public static readonly Color mainCol = Color.FromArgb(57, 64, 69);
        public static readonly Color highlightCol = Color.FromArgb(16, 136, 227);
        public static readonly Color textColLight = Color.FromArgb(59, 71, 79);
        public static readonly Color textColDark = Color.FromArgb(200, 200, 204);
        public static VBECanvas canv;
        public static Font defFont;
        public static List<Window> windows = new();
        static List<Application> applications = new();
        static List<Num> nums = new();
        List<Button> applicationsButtons = new();
        List<Button> numsButtons = new();
        public static List<Text.T> strList = new();
        public static string copyValue;
        Button mainButton;
        Button calendarButton;
        public static int activeIndex = -1;
        bool mainBar;
        bool lookCalendar;
        public static int bgCount = 1;
        string time;
        string spaces = "";

        [ManifestResourceStream(ResourceName = "zenithos.Resource.bg.bmp")]
        public static readonly byte[] bgBytes;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.cur.bmp")]
        static readonly byte[] curBytes;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.zenith.bmp")]
        static readonly byte[] logoBytes;
      
        [ManifestResourceStream(ResourceName = "zenithos.Resource.startup.wav")]
        static readonly byte[] sampleAudioBytes;

        public static Bitmap bg,cursor,logo;

        void DrawTopbar()
        {
            time = DateTime.Now.ToString("dddd, MMM d, yyyy. HH:mm");

            canv.DrawFilledRectangle(bgCol, 0, 0, (int)canv.Mode.Width, 30);
            //calendarButton = new Button(time, (int)canv.Mode.Width - 20 - defFont.Width * time.Length, 0, mainCol, defFont, 7);

            mainButton.Update(0, 0);
            calendarButton.Update(0, 0);

            canv.DrawString(activeIndex != -1 && windows.Count != 0 && activeIndex < windows.Count ? windows[activeIndex].title:"",defFont,textColDark,170,8);
        }

        void DrawMainBar()
        {
            canv.DrawFilledRectangle(bgCol, 10, 40, 300, applicationsButtons.Count*50+40);
            canv.DrawString("Welcome to Timur", defFont, textColDark, 40, 70 - defFont.Height);
            for(int i =0;i<applicationsButtons.Count;i++)
            {
                applicationsButtons[i].Update(10, 40);
                if (applicationsButtons[i].clickedOnce)
                {
                    Window instance = applications[i].constructor();
                    windows.Add(instance);
                    mainBar = false;
                    break;
                }
            }
        }

        void DrawCalendar()
        {

            time = DateTime.Now.ToString("dddd, MMM d, yyyy. HH:mm");
            //calendarButton = new Button(time, (int)canv.Mode.Width - 20 - defFont.Width * time.Length, 0, mainCol, defFont, 7);

            canv.DrawFilledRectangle(bgCol, (int)canv.Mode.Width - 100 - defFont.Width * time.Length, 40, 400, 5 * 50 + 40);
            canv.DrawString("Mn", defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 30, 70 - defFont.Height);
            canv.DrawString("Td", defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 70, 70 - defFont.Height);
            canv.DrawString("Wd", defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 110, 70 - defFont.Height);
            canv.DrawString("Th", defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 150, 70 - defFont.Height);
            canv.DrawString("Fr", defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 190, 70 - defFont.Height);
            canv.DrawString("St", defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 230, 70 - defFont.Height);
            canv.DrawString("Sn", defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 270, 70 - defFont.Height);

            canv.DrawString(DateTime.Now.ToString("MMMMMMMM, yyyy"), defFont, textColDark, (int)canv.Mode.Width - 100 - defFont.Width * time.Length + 30, 300);

            int counter = 0;

            for (int i = 0; i < numsButtons.Count; i++)
            {
                if (counter > 6)
                {
                    counter = 0;
                }

                if (i > 6 && i <= 13)
                {
                    numsButtons[i].Update(30 + 40 * counter, 80);
                    counter++;
                }
                else if(i > 13  && i <=  20)
                {
                    numsButtons[i].Update(30 + 40 * counter, 120);
                    counter++;
                }
                else if(i > 20 && i <= 27)
                {
                    numsButtons[i].Update(30 + 40 * counter, 160);
                    counter++;
                }
                else if(i > 27)
                {
                    numsButtons[i].Update(30 + 40 * counter, 200);
                    counter++;
                }
                else
                {
                    numsButtons[i].Update(30 + 40 * counter, 40);
                    counter++;
                }

            }
        }

        public static void ThrowError(string content,string title="Error")
        {
            windows.Add(new Error(title,content));
            activeIndex = windows.Count - 1;
        }

        class Application
        {
            public Func<Window> constructor;
            public Bitmap logo;
            public string name;
            public Application(Func<Window> constructor, string name,Bitmap logo)
            {
                this.constructor = constructor;
                this.name = name;
                this.logo = logo;
            }
        }

        class Num
        {
            public string name;

            public Num(string name)
            {
                this.name = name;
            }
        }

        /*class T
        {
            public string a;
            public T(string a)
            {
                this.a = a;
            }
        }*/

        protected override void BeforeRun()
        {
            canv = new VBECanvas();
            defFont = PCScreenFont.Default;
            MouseManager.ScreenWidth = canv.Mode.Width;
            MouseManager.ScreenHeight = canv.Mode.Height;
            MouseManager.X = MouseManager.ScreenWidth / 2;
            MouseManager.Y = MouseManager.ScreenHeight / 2;

            bg = new Bitmap(bgBytes);
            cursor = new Bitmap(curBytes);
            logo = new Bitmap(logoBytes);

            time = DateTime.Now.ToString("dddd, MMM d, yyyy. HH:mm");
            IEnumerable<string> repeatStrings = Enumerable.Repeat(" ", time.Length);
            foreach (string repeatString in repeatStrings)
            {
                spaces += repeatString;
            }

            mainButton = new Button("Timur", 0, 0, mainCol, defFont,7,logo);
            calendarButton = new Button(spaces, (int)canv.Mode.Width - 20 - defFont.Width * time.Length, 0, mainCol, defFont,7);
          

            applications.Add(new Application(() => new Calc(), "Calculator",new Calc().logo));
            applications.Add(new Application(() => new TestWindow(), "Test Window",new TestWindow().logo));
            applications.Add(new Application(() => new UITest(), "UI Test", new UITest().logo));
            applications.Add(new Application(() => new About(), "About Timur...", new About().logo));
            applications.Add(new Application(() => new BgChange(), "BgChange", new Windows.Power().logo));
            applications.Add(new Application(() => new ShowText(), "Show txt Files",new ShowText().logo));
            applications.Add(new Application(() => new Windows.Power(), "Power...",new Windows.Power().logo));

            for(int i = 1; i <= 30; i++)
            {
                nums.Add(new Num(Convert.ToString(i)));
            }


            for (int i = 0; i < applications.Count; i++)
            {
                applicationsButtons.Add(new Button(applications[i].name, 30, 40 + i * 50, mainCol, defFont, 10, applications[i].logo,240));
            }

            for (int i = 0;i < nums.Count;i++)
            {
                if(time.Split(' ', ',')[3] == Convert.ToString(i + 1))
                {
                    numsButtons.Add(new Button(nums[i].name, (int)canv.Mode.Width - 105 - defFont.Width * time.Length, 40, Color.CornflowerBlue, defFont, 10, null, 28));
                }
                else
                {
                    numsButtons.Add(new Button(nums[i].name, (int)canv.Mode.Width - 105 - defFont.Width * time.Length, 40, mainCol, defFont, 10, null, 28));
                }
            }
        }

        public void DrawCursor(uint x, uint y)
        {
            int xPos = (int)x;
            int yPos = (int)y;

            if(yPos>canv.Mode.Height- 16)
            {
                yPos = (int)canv.Mode.Height - 16;
            }

            canv.DrawImageAlpha(cursor, xPos, yPos);
        }
        
        protected override void Run()
        {
            canv.Clear();
            canv.DrawImage(bg, 0, 0);
            DrawTopbar();
            time = DateTime.Now.ToString("dddd, MMM d, yyyy. HH:mm");
            canv.DrawString(time, defFont, textColDark, (int)canv.Mode.Width - 10 - defFont.Width * time.Length, 7);
            //calendarButton = new Button(time, (int)canv.Mode.Width - 20 - defFont.Width * time.Length, 0, mainCol, defFont, 7);

            if (mainButton.clickedOnce)
            {
                mainBar = !mainBar;
            }
            if(calendarButton.clickedOnce)
            {
                lookCalendar = !lookCalendar;
            }

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            int dmx = MouseManager.DeltaX;
            int dmy = MouseManager.DeltaY;
            for(int i =0;i<windows.Count;i++)
            {
                if(i != activeIndex)
                    windows[i].Update(canv,mx,my, MouseManager.MouseState == MouseState.Left, dmx, dmy);
            }
            if(activeIndex != -1 && windows.Count > 0)
                windows[activeIndex].Update(canv, mx, my, MouseManager.MouseState == MouseState.Left, dmx, dmy);

            if (mainBar) DrawMainBar();
            if (lookCalendar) DrawCalendar();

            DrawCursor(MouseManager.X,MouseManager.Y);
            canv.Display();
            Heap.Collect();
        }
    }
}
