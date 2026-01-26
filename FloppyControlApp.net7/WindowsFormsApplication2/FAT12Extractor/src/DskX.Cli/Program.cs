using DskX.Core;

namespace DskX.Cli;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return 0;
        }

        string command = args[0].ToLower();

        try
        {
            return command switch
            {
                "l" or "list" => ListCommand(args[1..]),
                "x" or "extract" => ExtractCommand(args[1..]),
                "--help" or "-h" or "help" => ShowHelp(),
                "--version" or "-v" => ShowVersion(),
                _ => ShowError($"Unknown command: {command}")
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static int ListCommand(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Error: Missing required argument <image>");
            Console.Error.WriteLine("Usage: dskx l <image> [--deleted] [--deleted-only]");
            return 1;
        }

        string imagePath = args[0];
        bool includeDeleted = Array.IndexOf(args, "--deleted") >= 0;
        bool deletedOnly = Array.IndexOf(args, "--deleted-only") >= 0;

        using var reader = new DiskImageReader();
        reader.Open(imagePath);

        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions
        {
            IncludeDeleted = includeDeleted,
            DeletedOnly = deletedOnly
        };

        foreach (var entry in extractor.EnumerateFiles(options))
        {
            Console.WriteLine($"{entry.FullPath,-50} {entry.Size,10:N0} bytes");
        }

        return 0;
    }

    static int ExtractCommand(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: Missing required arguments");
            Console.Error.WriteLine("Usage: dskx x <image> <dest> [--deleted] [--deleted-only] [--file <path>]");
            return 1;
        }

        string imagePath = args[0];
        string destPath = args[1];
        bool includeDeleted = Array.IndexOf(args, "--deleted") >= 0;
        bool deletedOnly = Array.IndexOf(args, "--deleted-only") >= 0;
        
        string? specificFile = null;
        int fileIndex = Array.IndexOf(args, "--file");
        if (fileIndex >= 0 && fileIndex + 1 < args.Length)
        {
            specificFile = args[fileIndex + 1];
        }

        using var reader = new DiskImageReader();
        reader.Open(imagePath);

        var extractor = new Fat12Extractor(reader);
        var options = new ExtractionOptions
        {
            IncludeDeleted = includeDeleted,
            DeletedOnly = deletedOnly,
            SpecificFilePath = specificFile
        };

        extractor.ExtractAll(destPath, options);

        return 0;
    }

    static int ShowHelp()
    {
        Console.WriteLine("DskX - FAT12 Disk Image Extractor");
        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("  dskx l <image> [options]              List files in disk image");
        Console.WriteLine("  dskx x <image> <dest> [options]       Extract files from disk image");
        Console.WriteLine();
        Console.WriteLine("LIST OPTIONS:");
        Console.WriteLine("  --deleted          Include deleted files");
        Console.WriteLine("  --deleted-only     List only deleted files");
        Console.WriteLine();
        Console.WriteLine("EXTRACT OPTIONS:");
        Console.WriteLine("  --deleted          Include deleted files");
        Console.WriteLine("  --deleted-only     Extract only deleted files");
        Console.WriteLine("  --file <path>      Extract specific file (e.g., \"DOCS\\README.TXT\")");
        Console.WriteLine();
        Console.WriteLine("EXAMPLES:");
        Console.WriteLine("  dskx l \"disk.dsk\"");
        Console.WriteLine("  dskx x \"disk.dsk\" ./output");
        Console.WriteLine("  dskx x \"disk.dsk\" ./output --deleted");
        Console.WriteLine("  dskx x \"disk.dsk\" ./output --file \"README.TXT\"");
        return 0;
    }

    static int ShowVersion()
    {
        Console.WriteLine("DskX v1.0.0");
        return 0;
    }

    static int ShowError(string message)
    {
        Console.Error.WriteLine(message);
        Console.Error.WriteLine("Run 'dskx --help' for usage information.");
        return 1;
    }
}
