// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

string[] ArgsRotation = new string[] {"Configure TCP port to use for FE connections",           // 0
                                      "Configure IP address and tcp port to instrumentDataDB",  // 1
                                      "Run program in background",                    // 2
                                      "Log to file"};                                 // 3

ArgSwitch(ArgsRotation);

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
static string ArgSwitch(string[] CurrentArg){
    int inc = 0;
    Console.WriteLine("Press TAB to switch between arguments, ENTER to select");
    while (true){
        ClearLine("", 0);
        Console.Write(CurrentArg[inc]);
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        
        if (keyInfo.Key == ConsoleKey.Tab) {
            inc += 1;
            ClearLine("", 0);
            Console.SetCursorPosition(0, Console.CursorTop-2);
            if (CurrentArg[inc] == CurrentArg[CurrentArg.Length - 1]) {
                inc = 0;
            }
            continue;
        }
        else if (keyInfo.Key == ConsoleKey.Enter) {
            Console.WriteLine($"\nYou selected to {CurrentArg[inc]}");
            
            while (inc == 0){
                Console.WriteLine("\nTCP port:");
                int connectThroughTCPPort;
                bool isValidInput = int.TryParse(Console.ReadLine(), out connectThroughTCPPort);

                if (!isValidInput) {
                    ClearLine("", 1, 4);
                    Console.WriteLine("No port number was input, please try again.");
                }
                else {
                    CurrentArg[0] = "Reconfigure TCP port to use for FE connections";
                    break;
                }
            }
            if (inc == 1)
            {

            }
            if (inc == 2)
            {

            }
            if (inc == 3)
            {

            }
        }
    }
    return CurrentArg[inc];
}

