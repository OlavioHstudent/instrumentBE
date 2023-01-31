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

static void ClearLine(string text)
{
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.WriteLine(text);
}

static string ArgSwitch(string[] CurrentArg){
    int inc = 0;

    while (true){
        ClearLine("");
        Console.Write(CurrentArg[inc]);
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        
        if (keyInfo.Key == ConsoleKey.Tab) {
            inc += 1;
            ClearLine("");
            Console.SetCursorPosition(0, Console.CursorTop-2);
            
            if (CurrentArg[inc] == CurrentArg[CurrentArg.Length - 1]) {
                inc = 0;
            }
            continue;
        }
        else if (keyInfo.Key == ConsoleKey.Enter) {
            Console.WriteLine($"\nYou selected: {CurrentArg[inc]}");
            
            if (inc == 0){
                Console.WriteLine("TCP port:");
                int ConnectThroughTCPport = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("asdf");
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
static void WriteOnBottomLine(string text){
    int x = Console.CursorLeft;
    int y = Console.CursorTop;
    Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
    Console.Write(text);
    // Restore previous position
    Console.SetCursorPosition(x, y);
}

