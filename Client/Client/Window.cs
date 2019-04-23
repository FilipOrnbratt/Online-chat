using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Client
{
    class Window : Form
    {
        private int FPS = 60; // 0 for unlimited
        private string FPSText;
        private int offX = 0, offY = 0;
        private Connection server;

        public static void Main()
        {
            Window window = new Window(400, 400);
        }

        public Window(int width, int height)
        {
            this.Size = new Size(width, height);
            this.DoubleBuffered = true;
            server = new Connection(ServerResponse);
            while (true)
            {
                String msg = Console.ReadLine();
                server.SendMessage(msg);
            }
        }

        public void ServerResponse(string message)
        {
            Console.WriteLine(message);
        }

        private void DisplayForm()
        {
            this.ShowDialog();
        }
    }
}
