using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Communication
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmMain = new FormMain();
            Application.Run(frmMain);
        }
        public static FormMain frmMain;
    }
}
