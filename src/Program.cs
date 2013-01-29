using System;
using System.Windows.Forms;
using GLEED2D.src;


namespace GLEED2D
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
 
            Logger.Instance.log("Application started.");
            
            bool debug = false;
            #if DEBUG
                debug=true;
            #endif

            if (!debug)
            {
                try
                {
                    runApp();
                }
                catch (Exception e)
                {
                    Logger.Instance.log("Exception caught: \n\n " + e.Message + "\n\n" + e.StackTrace);
                    if (e.InnerException != null) Logger.Instance.log("Inner Exception: " + e.InnerException.Message);
                    MessageBox.Show("An exception was caught. Application will end. Please check the file log.txt.");
                }
                finally
                {
                    Logger.Instance.log("Application ended.");
                }
            }
            else
            {
                runApp();
            }

        }
        private static void runApp()
        {
            Application.EnableVisualStyles();

            MainForm form = new MainForm();
            form.Show();

            Logger.Instance.log("Creating Game1 object.");
            using (Game1 game = new Game1(form.getHandle()))
            {
                game.Run();
            }
        }
    }
}

