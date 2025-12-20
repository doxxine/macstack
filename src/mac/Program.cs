using System.Diagnostics;
using System.Reflection;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            PrintHelp();
            return 0;
        }

        var cmd = args[0];

        return cmd switch
        {
            "help" => Help(args[1..]),
            "version" or "--version" or "-V" => PrintVersion(),
            "list" or "plugins" => ListCommands(),
            _ => ExecPlugin(cmd, args[1..])
        };
    }

    private static bool IsHelp(string s)
    {
        return s is "-h" or "--help";
    }

    private static int Help(string[] args)
    {
        if (args.Length != 0) return ExecPlugin(args[0], ["--help"]);
        PrintHelp();
        return 0;
    }

    private static int ListCommands()
    {
        // BUILT-INS
        Console.WriteLine("Built-ins:");
        Console.WriteLine("  help");
        Console.WriteLine("  version");
        Console.WriteLine("  list");

        // PLUGINS
        var plugins = DiscoverPlugins();
        Console.WriteLine();
        Console.WriteLine("Plugins:");

        if (plugins.Count == 0)
        {
            Console.WriteLine("  (none found)");
            return 0;
        }

        foreach (var p in plugins)
            Console.WriteLine($"  {p}");

        return 0;
    }

    private static List<string> DiscoverPlugins()
    {
        var results = new HashSet<string>(StringComparer.Ordinal);
        var macroot = GetMacRoot();

        if (!string.IsNullOrWhiteSpace(macroot))
        {
            var dir = Path.Combine(macroot, "plugins");
            if (Directory.Exists(dir))
                foreach (var file in Directory.EnumerateFiles(dir, "mac-*"))
                {
                    var name = Path.GetFileName(file);
                    if (name.StartsWith("mac-", StringComparison.Ordinal) && IsExecutable(file))
                        results.Add(name["mac-".Length..]);
                }
        }

        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(path)) return results.OrderBy(x => x, StringComparer.Ordinal).ToList();
        {
            foreach (var dir in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
                    continue;

                try
                {
                    foreach (var file in Directory.EnumerateFiles(dir, "mac-*"))
                    {
                        var name = Path.GetFileName(file);
                        if (name.StartsWith("mac-", StringComparison.Ordinal) && IsExecutable(file))
                            results.Add(name["mac-".Length..]);
                    }
                }
                catch
                {
                    // IGNORE UNREADABLE DIRECTORIES
                }
            }
        }

        return results.OrderBy(x => x, StringComparer.Ordinal).ToList();
    }

    private static int PrintVersion()
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        Console.WriteLine($"mac {v}");
        return 0;
    }

    private static int ExecPlugin(string cmd, string[] passthroughArgs)
    {
        var pluginName = $"mac-{cmd}";
        var macroot = GetMacRoot();

        var repoPluginPath = macroot is null
            ? null
            : Path.Combine(macroot, "plugins", pluginName);

        var pluginPath =
            repoPluginPath is not null && File.Exists(repoPluginPath) ? repoPluginPath : FindOnPath(pluginName);

        if (pluginPath is null)
        {
            Console.Error.WriteLine($"mac: unknown command: {cmd}");
            Console.Error.WriteLine("Try 'mac --help'.");
            return 127; // UNIX: command not found
        }

        if (!IsExecutable(pluginPath))
        {
            Console.Error.WriteLine($"mac: {pluginName}: not executable");
            return 126; // UNIX: found but cannot execute
        }

        return RunProcess(pluginPath, passthroughArgs, macroot);
    }

    private static int RunProcess(string fileName, string[] args, string? macroot)
    {
        using var p = new Process();
        p.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            UseShellExecute = false
        };

        foreach (var a in args)
            p.StartInfo.ArgumentList.Add(a);

        if (!string.IsNullOrWhiteSpace(macroot))
            p.StartInfo.Environment["MACROOT"] = macroot;

        try
        {
            p.Start();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"mac: failed to run {Path.GetFileName(fileName)}: {ex.Message}");
            return 126;
        }

        p.WaitForExit();
        return p.ExitCode;
    }

    private static bool IsExecutable(string path)
    {
        try
        {
            var mode = File.GetUnixFileMode(path);
            return mode.HasFlag(UnixFileMode.UserExecute)
                   || mode.HasFlag(UnixFileMode.GroupExecute)
                   || mode.HasFlag(UnixFileMode.OtherExecute);
        }
        catch
        {
            return false;
        }
    }

    private static string? GetMacRoot()
    {
        var env = Environment.GetEnvironmentVariable("MACROOT");
        if (!string.IsNullOrWhiteSpace(env) && Directory.Exists(env))
            return env;

        var exe = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(exe))
            return null;

        var dir = Path.GetDirectoryName(exe);
        if (string.IsNullOrWhiteSpace(dir))
            return null;

        var parent = Directory.GetParent(dir);
        if (parent is not null && Path.GetFileName(dir) == ".bin")
            return parent.FullName;

        return null;
    }

    private static string? FindOnPath(string name)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(path))
            return null;

        foreach (var dir in path.Split(Path.PathSeparator))
        {
            if (string.IsNullOrWhiteSpace(dir))
                continue;

            var candidate = Path.Combine(dir, name);
            if (File.Exists(candidate))
                return candidate;
        }

        return null;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("""
                          mac — macstack command line

                          Usage:
                            mac <command> [args]
                            mac help <command>
                            mac <command> --help

                          Built-ins:
                            version         Print version
                            help            Show help for a command

                          Plugins:
                            mac <cmd> runs plugins named mac-<cmd> (repo plugins/ first, then PATH)

                          Options:
                            -h, --help      Show help
                          """);
    }
}
