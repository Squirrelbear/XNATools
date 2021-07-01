using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;

using XNATools.WndCore;

namespace KinectLibraryTest
{
    public class WndFactory
    {
        public enum WndCodes { MainMenu = 0, TestDialog = 1, GripWndSample = 2, MenuWndSample = 3, 
                                PongWnd = 4, NumberGameWnd = 5, LightsaberWnd = 6, DrawingGameWnd = 7 };

        public static int getCode(WndCodes code)
        {
            return (int)code;
        }

        public static T createWnd<T>(Rectangle displayRect, WndGroup parent) where T : WndHandle
        {
            try
            {
                Type type = typeof(T);
                ConstructorInfo ctor = type.GetConstructor(new[] { typeof(Rectangle), typeof(WndGroup) });
                return (T)ctor.Invoke(new object[] { displayRect, parent });
            }
            catch
            {
                return default(T);
            }
        }

        public static WndHandle createWnd(WndCodes code, Rectangle displayRect, WndGroup parent)
        {
            WndHandle handle = null;
            switch (code)
            {
                case WndCodes.MainMenu:
                    handle = new MainMenu(displayRect, parent);
                    break;
                case WndCodes.TestDialog:
                    handle = new TestDialog(displayRect, parent);
                    break;
                case WndCodes.GripWndSample:
                    handle = new GripWndSample(displayRect, parent);
                    break;
                case WndCodes.MenuWndSample:
                    handle = new MenuWndSample(displayRect, parent);
                    break;
                case WndCodes.PongWnd:
                    handle = new PongWnd(displayRect, parent);
                    break;
            }

            return handle;
        }
    }
}
