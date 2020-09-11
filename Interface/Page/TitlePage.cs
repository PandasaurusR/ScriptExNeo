using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptExNeo.Assets;

namespace ScriptExNeo.Interface.Page {
    public static class TitlePage {
        public static void Display() {
            Terminal.WriteAt(25, 1, Asset.TitleSplashText);
            Console.SetCursorPosition(0, 14);
        }
    }
}
