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

// Variables
string LogFile = "log.txt";
string[] ArgsRotation = new string[] {"Configure TCP port to use for FE connections",           // 0
                                      "Configure IP address and tcp port to instrumentDataDB",  // 1
                                      "Run program in background - NOT WORKING - ",             // 2
                                      "Log to file",                                            // 3
                                      "This index is here because I can't access index 3 without it somehow :/ "};

// Program
appInterface.ArgSwitch(ArgsRotation, LogFile);


class appInterface{
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
                                ConnectionPoint(connectThroughTCPPort, CurrentArg);}

                            Console.WriteLine("\nTAB to cycle through configurations, ENTER to select:");
                            CurrentArg[1] = "Reconfigure IP to use for instrumentDataDB";
                            break;  }}}

                if (inc == 2) {
                    //How the fuck do I make it run in the bg? Minimize maybe?
                }

                // Log to file
                if (inc == 3) {
                    WriteToLogFile("someText");}}}
    }

    static async Task ConnectionPoint(int port, string[] ArgsRotation, string LogFile = "") {                   
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();                        

        Task listeningTask = Task.Run(async () => {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while (!cancellationTokenSource.Token.IsCancellationRequested) {
                TcpClient client = await listener.AcceptTcpClientAsync(cancellationTokenSource.Token);

                //This is where it happens when connected
                if (client.Connected) {
                    ClearLine("", 1,1);
                    Console.WriteLine("Connection established");
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine(response); //At the moment prints "hello server"
                    Console.WriteLine(SerialCommand("readscaled"));}
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

    static string InstrumentToBEConnection(string comPort, 
                                           string bitRate,
                                           string command){
        string[] ComPorts = System.IO.Ports.SerialPort.GetPortNames();
        Console.WriteLine("The following COM ports exist:");

        foreach (string port in ComPorts){
            Console.WriteLine(port);}

        Console.WriteLine("Enter portName");
        string portName = Console.ReadLine();

        Console.WriteLine("Enter baud rate:");
        int baudRate = Convert.ToInt32(Console.ReadLine());
        SerialPort serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();

        Console.WriteLine("Enter message to send to Arduino:");
        string message = Console.ReadLine();
        serialPort.WriteLine(message);
        Console.WriteLine("Message sent. Waiting for response");

        string response = serialPort.ReadLine();
        Console.WriteLine($"Response received: {response}");
        Console.ReadKey();
        serialPort.Close();
        return response;
    }
    static string SerialCommand(string command){
        SerialPort serialPort = new SerialPort("COM3", 9600);
        serialPort.Open();
        serialPort.WriteLine(command);
        string inoResponse = serialPort.ReadLine();
        serialPort.Close();
        return inoResponse;
    }

    private static void WriteToLogFile(string logText) {
        string fileName = "log.txt";
        string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
        // Open the file for writing
        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read)) {
            using (StreamWriter sw = new StreamWriter(fs)) {
                sw.WriteLine("" + System.DateTime.Now + " " + logText + "\r\n");
            }
            fs.Close();
        }
    }
}
