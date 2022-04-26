# Introducing The MJBLogger Class Library
MJBLogger is a configurable, simple-to-use text file logging class library intended to allow you to add logging functionality to your .NET applications without much fuss.

## Creating a Simple Application Log

To add logging to your application:

```
namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Instantiate the log oject
            MJBLog Log = new MJBLog();

            //Write an introductory message to the log
            Log.Banner();

            //Write an informational message to the log:
            Log.Info(@"Hello World!");
        }
    }
}
```

After executing your application, you will find a "logs" folder created in the bin directory. By default, the log files created within are named after the assembly (e.g. "Sample_1.log"). The contents of the log generated by the program above is as follows:

```
==================================================
Sample -- invoked by MYPC\mikeb -- 07/13/2020 11:54:50.791 
==================================================

07/13/2020 11:54:50.799 <Info   >  [Program.Main] Hello World!

```

## Customizing the Log Format

MJB Logger includes many options to customize log appearance and behavior

```
MJBLog Log = new MJBLog()
{
    //Write log messages to the output window of console applications..
    ConsoleEcho = true,

    //..but only if the criticality is at least Informational
    ConsoleEchoLevel = LogLevel.Info
};

Log.Critical(@"Oh no! A critical error occurred!");

//Save screen space by: 
//Using shortened criticality labels:
Log.UseChibiLevelLabels = true;

//...And ommitting the datestamp
Log.IncludeDateStamps = false;

Log.Critical(@"Oops! Another error..");
```

When this program executes, the log entries are written to the output window as well as the log file (so long as this is a console application). Also, the appearance of the second log entry reflects the properties we changed:

```
07/13/2020 12:13:35.444 <Critical     >  [Program.Main] Oh no! A critical error occurred!
12:13:35.457 <Crit   >  [Program.Main] Oops! Another error..
```


## Object Property Reports

MJBLogger allows you to log a dump a justified list of the names and values of all string, int and bool-type properties of any object using a single statement:

```
    public class Fruit
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MJBLog Log = new MJBLog();
            
            Fruit Orange = new Fruit()
            {
                Name = nameof(Orange),
                Type = @"Citris",
                Quantity = 20
            };

            Log.PropertyReport(Orange);
        }
    }
```

In the log file, we find:

```
07/13/2020 12:20:36.606 <Info         >  [Program.Main] Sample.Fruit properties:
          Name     : Orange
          Type     : Citris
          Quantity : 20
```

Full documentation for the MJBLogger class library can be found at [X509Crypto.org](http://www.x509crypto.org)
