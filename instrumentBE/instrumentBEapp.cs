// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.IO.Ports;
using System.IO;
using System;
using static appInterface;
using System.Runtime.InteropServices;

// Variables
string LogFile = "log.txt";

string[] ArgsRotation = new string[] {"Configure TCP port to use for FE connections",           // 0
                                      "Configure IP address and tcp port to instrumentDataDB",  // 1
                                      "Run program in background",             // 2
                                      "Log to file",                                            // 3
                                      "This index is here because I can't access index 3 without it somehow :/ "};

// Program
appInterface.ArgSwitch(ArgsRotation, LogFile);


class appInterface{
    private static SerialPort serialPort = new SerialPort();
    public int connectedBitrate = 0;
    public string connectedCOMPort = "";
    static void ClearLine(string text, int MoveBack, int xLines = 0) {

        if (MoveBack == 0) {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine(text);    }

        if (MoveBack == 1) {
            Console.SetCursorPosition(0, Console.CursorTop - xLines);

            for (int i = 0; i < xLines; i++) {
                Console.WriteLine(new string(' ', Console.BufferWidth));}

            Console.SetCursorPosition(0, Console.CursorTop - xLines);}
    }
    public static string ArgSwitch(string[] CurrentArg, string LogFile) {
        int inc = 0;
        int connectThroughTCPPort = 0;
        Console.WriteLine("TAB to cycle through configurations. ENTER to select:");
        RunInBGorFG(5);

        while (true) {
            ClearLine("", 0);
            Console.Write(CurrentArg[inc]);
            ConsoleKeyInfo keyPress = Console.ReadKey(true);

            if (keyPress.Key == ConsoleKey.Tab) {
                inc += 1;
                ClearLine("", 0);
                Console.SetCursorPosition(0, Console.CursorTop - 2);

                if (CurrentArg[inc] == CurrentArg[CurrentArg.Length - 1]) {
                    inc = 0;}
                
                continue;   }

            else if (keyPress.Key == ConsoleKey.Enter) {
                Console.WriteLine($"\nYou selected to {CurrentArg[inc]}. Continue below.");

                while (inc == 0) {
                    Console.WriteLine("\nTCP port:");
                    bool isValidInput = int.TryParse(Console.ReadLine(), out connectThroughTCPPort);

                    if (!isValidInput) {
                        ClearLine("", 1, 4);
                        Console.WriteLine("No port number was input, please choose a port.");}

                    else {
                        Console.WriteLine($"\nPort {connectThroughTCPPort} selected");
                        Console.WriteLine("\nTAB to cycle through configurations, ENTER to select:");
                        CurrentArg[0] = "Reconfigure TCP port to use for instrumentFE connections";
                        break;  }}

                if (inc == 1) {
                    while (inc == 1) {
                        Console.WriteLine("\nIP address:");
                        string connectThroughIP = Console.ReadLine();
                        if (string.IsNullOrEmpty(connectThroughIP)) {
                            ClearLine("", 1, 4);
                            Console.WriteLine("No IP address was input, please enter an IP address.");}
                        else {
                            Console.WriteLine($"\nIP address {connectThroughIP} selected, press enter to listen on port {connectThroughTCPPort}");
                            if (connectThroughTCPPort > 0) {
                                ConnectionPoint(connectThroughIP, connectThroughTCPPort, CurrentArg);}

                            Console.WriteLine("\nTAB to cycle through configurations, ENTER to select:");
                            CurrentArg[1] = "Reconfigure IP to use for instrumentDataDB";
                            break;  }}}

                if (inc == 2) {
                    RunInBGorFG(0);
                }

                    // Log to file
                    if (inc == 3) {
                    WriteToLogFile("someText");}}}
    }

    static void RunInBGorFG(int frontOrBack) {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        IntPtr hWnd = GetConsoleWindow();
        ShowWindow(hWnd, frontOrBack);
    }
    static async Task ConnectionPoint(string ip, int port, string[] ArgsRotation, string LogFile = "") {                   
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task listeningTask = Task.Run(async () => {
            IPAddress ipAddress = IPAddress.Parse(ip);
            TcpListener listener = new TcpListener(ipAddress, port);
            listener.Start();
            string COMPortFromRequest = "3";
            int bitRateFromRequest = 9600;

            while (!cancellationTokenSource.Token.IsCancellationRequested) {
                TcpClient client = await listener.AcceptTcpClientAsync(cancellationTokenSource.Token);

                //This is where it happens when connected
                if (client.Connected) {
                    RunInBGorFG(0);
                    ClearLine("", 1,1);
                    Console.WriteLine("Connection established");
                    NetworkStream stream = client.GetStream();
                    DataTransfer dataTransfer = new DataTransfer(stream);
                    byte[] buffer = new byte[1024];
                    string connectionStatus = "";
                    string[] portNameList= SerialPort.GetPortNames();

                    string availablePorts = string.Join(",", portNameList);
                    dataTransfer.SendMessage(availablePorts);

                    while (true) {
                        int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                        string request = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                        //Connect to instrument
                        if (request.Substring(0, 6) == "COMcon") {
                            COMPortFromRequest = $"COM{request.Substring(7, 1)}";
                            bitRateFromRequest = Convert.ToInt32(request.Substring(11));
                            connectionStatus = ConnectSerialPort(COMPortFromRequest, bitRateFromRequest);

                            if (connectionStatus == "Connected") {
                                dataTransfer.SendMessage("Connection success");}

                            if (connectionStatus == "Connection error") {
                                dataTransfer.SendMessage("Connection Error. Please try again, a different port or consider restarting software.");}
                            continue;
                        }

                        //Disconnect from instrument
                        if (request.Substring(0, 6) == "COMdis") {
                            Console.WriteLine("Disconnecting from COM port");
                            connectionStatus = DisconnectSerialPort();

                            if (connectionStatus == "Disconnected") {
                                dataTransfer.SendMessage("Disconnected from instrument");}

                            else if (connectionStatus == "Error disconnecting") {
                                dataTransfer.SendMessage("Error disconnecting. Try again or consider violently shutting down system");}
                            
                            else if (connectionStatus == "Already closed") {
                                dataTransfer.SendMessage("Instrument already disconnected");}
                            continue;
                        }

                        //readconf
                        if (request.Substring(0, 8) == "readconf") {
                            RequestDataFromIno(request, dataTransfer);
                            continue;
                        }

                        if (request.Substring(0, 9) == "writeconf") {
                            RequestDataFromIno("readconf", dataTransfer);
                            continue;
                        }
                        if (request.Substring(0, 10) == "readscaled") {
                            RequestDataFromIno(request, dataTransfer);
                            continue;
                        }
                        if (request.Substring(0, 4) == "EXIT") { 
                            RunInBGorFG(5);
                            Console.WriteLine("Do you want to exit? Y/N");
                            string exit = Console.ReadLine();
                            if (exit == "Y") {
                                Environment.Exit(0);
                                }
                            continue;
                        }
                        continue;
                    }
                }
                if (!client.Connected) {
                    Environment.Exit(0);
                    break;
                }
            }
            Console.WriteLine("Stopped listening");
            listener.Stop();    } 

        , cancellationTokenSource.Token);

        Console.WriteLine($"Listening on port {port}");
        Console.WriteLine("Press Esc to stop listening");
        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        cancellationTokenSource.Cancel();
        await listeningTask;
    }

    static void RequestDataFromIno(string request, DataTransfer dataTransfer) {
        serialPort.WriteLine(request);
        string inoResponse = serialPort.ReadLine();
        dataTransfer.SendMessage(inoResponse);
    }
    static string ConnectSerialPort(string portName, int bitrate) {
        try {
            serialPort.PortName = portName;
            serialPort.BaudRate = bitrate;
            serialPort.Open();
            Console.WriteLine($"Connected to serial port {portName} at {bitrate} bps.");
            return "Connected";
            
        }
        catch (Exception ex) {
            Console.WriteLine($"Error connecting to serial port {portName}: {ex.Message}");
            return "Connection error";
        }
    }
    static string DisconnectSerialPort() {
        if (serialPort.IsOpen) {
            try {
                serialPort.Close();

                if (!serialPort.IsOpen) {
                    Console.WriteLine("Disconnected from serial port.");
                }
                return "Disconnected";
            }
            catch (Exception ex) {
                Console.WriteLine($"Error disconnecting from serial port: {ex.Message}");
                return "Error disconnecting";
            }
        }
        else {
            Console.WriteLine("Serial port is not open.");
            return "Already closed";
        }
    }
    
    private static void WriteToLogFile(string logText) {
        string fileName = "log.txt";
        string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read)) {
            using (StreamWriter sw = new StreamWriter(fs)) {
                sw.WriteLine("" + System.DateTime.Now + " " + logText + "\r\n");
            }
            fs.Close();}}
}
public class DataTransfer {
    private NetworkStream stream;

    public DataTransfer(NetworkStream stream) {
        this.stream = stream;
    }

    public void SendMessage(string message) {
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }
}

