// See https://aka.ms/new-console-template for more information
int FE_tcp = 0;
int FE_ip  = 0;

int DataDB_tcp = 0;
int DataDB_ip  = 0;


// Establish connection with instrumentFE
Console.WriteLine("instrumentFE connection:\n");
Console.WriteLine("-------------------------\n");

Console.WriteLine("TCP port:");
FE_tcp = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("IP address:");
FE_ip = Convert.ToInt32(Console.ReadLine());


// Establish connection with instrumentDataDB
Console.WriteLine("instrumentDataDB connection:\n");
Console.WriteLine("-------------------------\n");

Console.WriteLine("TCP port:");
DataDB_tcp = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("IP address:");
DataDB_ip = Convert.ToInt32(Console.ReadLine());
