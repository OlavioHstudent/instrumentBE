// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

// Variables
string LogFile = "log.txt";
string[] ArgsRotation = new string[] {"Configure TCP port to use for FE connections",           // 0
                                      "Configure IP address and tcp port to instrumentDataDB",  // 1
                                      "Run program in background - NOT WORKING - ",             // 2
                                      "Log to file",                                            // 3
                                      "This index is here because I can't access index 3 without it somehow :/ "};

// Program
ArgSwitch(ArgsRotation, LogFile);


// Methods
static void ClearLine(string text, int MoveBack, int xLines = 0) {

    if (MoveBack == 0) {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.WriteLine(text);
    }
    if (MoveBack == 1) {
        Console.SetCursorPosition(0, Console.CursorTop - xLines);
        for (int i = 0; i < xLines; i++) {
            Console.WriteLine(new string(' ', Console.BufferWidth));
        }
        Console.SetCursorPosition(0, Console.CursorTop - xLines);
    }
}
static string ArgSwitch(string[] CurrentArg, string LogFile){
    int inc = 0;
    Console.WriteLine("TAB to cycle through configurations. ENTER to select:");

    while (true){
        ClearLine("", 0);
        Console.Write(CurrentArg[inc]);
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        
        if (keyInfo.Key == ConsoleKey.Tab) {
            inc += 1;
            ClearLine("", 0);
            Console.SetCursorPosition(0, Console.CursorTop-2);

            if (CurrentArg[inc] == CurrentArg[CurrentArg.Length-1]) {
                inc = 0;
            }
            continue;
        }
        else if (keyInfo.Key == ConsoleKey.Enter) {
            Console.WriteLine($"\nYou selected to {CurrentArg[inc]}. Continune below.");

            while (inc == 0){
                Console.WriteLine("\nTCP port:");

                int connectThroughTCPPort;
                bool isValidInput = int.TryParse(Console.ReadLine(), out connectThroughTCPPort);


                if (!isValidInput) {
                    ClearLine("", 1, 4);
                    Console.WriteLine("No port number was input, please choose a port.");
                }
                else {
                    Console.WriteLine($"\nPort {connectThroughTCPPort} selected");
                    Console.WriteLine("\nTAB to cycle through configurations, ENTER to select:");
                    CurrentArg[0] = "Reconfigure TCP port to use for instrumentFE connections";
                    break;
                }
            }
            if (inc == 1)
            {
                while (inc == 1) {
                    Console.WriteLine("\nIP address:");

                    string connectThroughIP = Console.ReadLine();
                    if (string.IsNullOrEmpty(connectThroughIP)) {
                        ClearLine("", 1, 4);
                        Console.WriteLine("No IP address was input, please enter an IP address.");
                    }
                    else {
                        Console.WriteLine($"\nIP address {connectThroughIP} selected");
                        Console.WriteLine("\nTAB to cycle through configurations, ENTER to select:");
                        CurrentArg[1] = "Reconfigure IP to use for instrumentDataDB";
                        break;
                    }
                }
            }
            if (inc == 2)
            {
                //How the fuck do I make it run in the bg? Minimize maybe?
             }
            if (inc == 3)
            {
                File.WriteAllText(LogFile, string.Empty);
                using (StreamWriter LogToFile = File.AppendText("log.txt")) {
                    LogToFile.WriteLine("Logging this text");
                }
                string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), LogFile);
                Console.WriteLine($"Sensor Data logged to log.txt in this directory:\n{LogFilePath}");
            }
        }
    }
    return CurrentArg[inc];
}

bool TryParse(string? v, out string connectThroughTCPPort) {
    throw new NotImplementedException();
}