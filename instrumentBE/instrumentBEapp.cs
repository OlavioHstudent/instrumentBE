// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

string[] ArgsRotation = new string[] {"Configure TCP port to use for FE connections",           // 0
                                      "Configure IP address and tcp port to instrumentDataDB",  // 1
                                      "Run program in background",                    // 2
                                      "Log to file"};                                 // 3

static string ArgSwitch(string[] CurrentArg){
    int inc = 0;
    while (true)
    {
        Console.Clear();
        Console.WriteLine("Cycle through arguments by pressing <TAB>, and select using ENTER:");
        Console.CursorLeft = 0;
        Console.CursorTop = 1;
        Console.Write(CurrentArg[inc]);

        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        if (keyInfo.Key == ConsoleKey.Tab) {
            inc += 1;

            if (CurrentArg[inc] == CurrentArg[CurrentArg.Length - 1]) {
                inc = 0;
            }
            continue;
        }
        else if (keyInfo.Key == ConsoleKey.Enter) {
            Console.WriteLine($"You selected: {CurrentArg[inc]}");
            continue;
        }
    }
}

ArgSwitch(ArgsRotation);