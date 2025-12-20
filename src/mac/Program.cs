using System.Diagnostics;
using System.Reflection;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0])) return Help(Array.Empty<string>());

        var cmd = args[0];

        return cmd switch
        {
            "help" => Help(args[1..]),
            "version" or "--version" or "-V" => PrintVersion(),
            "list" or "plugins" => List(args[1..]),
            _ => ExecPlugin(cmd, args[1..])
        };
    }

    private static bool IsHelp(string s)
    {
        return s is "-h" or "--help";
    }

    private static int PrintVersion()
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        Console.WriteLine($"mac {v}");
        return 0;
    }

    private static int Help(string[] args)
    {
        // UNIX/CLI INTENT:
        // - PREFER STYLED HELP PLUGIN IF PRESENT (mac-help)
        // - FALL BACK TO BUILT-IN HELP OTHERWISE
        //
        // BEHAVIOR:
        // - mac help            -> styled overview (if plugin exists) else built-in help
        // - mac help plugins    -> plugin list (handled by mac-help plugin if present)
        // - mac help <cmd>      -> executes plugin help via router (this function), if plugin exists.

        if (TryExecHelpPlugin(args, out var exitCode))
            return exitCode;

        // FALLBACK: BUILT-IN HELP (MINIMAL)
        if (args.Length == 0)
        {
            PrintHelp();
            return 0;
        }

        // mac help <cmd> -> run <cmd> --help (plugin or built-in)
        var target = args[0];
        if (target is "help" or "version" or "list" or "plugins")
        {
            PrintBuiltinHelp(target);
            return 0;
        }

        return ExecPlugin(target, new[] { "--help" });
    }

    private static bool TryExecHelpPlugin(string[] args, out int exitCode)
    {
        // ONLY EXECUTE mac-help IF IT EXISTS AS A PLUGIN.
        // THIS KEEPS THE ROUTER MINIMAL AND ALLOWS STYLED UX IN SHELL.
        var pluginPath = ResolvePluginPath("help");
        if (pluginPath is null)
        {
            exitCode = 0;
            return false;
        }

        exitCode = RunProcess(pluginPath, args, GetMacRoot());
        return true;
    }

    private static int List(string[] args)
    {
        // mac list            -> built-ins + plugins
        // mac list plugins    -> plugins only
        var pluginsOnly = args.Length > 0 && args[0] == "plugins";

        if (!pluginsOnly)
        {
            Console.WriteLine("Built-ins:");
            Console.WriteLine("  help");
            Console.WriteLine("  version");
            Console.WriteLine("  list");
            Console.WriteLine();
        }

        var plugins = DiscoverPlugins();
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

    private static int ExecPlugin(string cmd, string[] passthroughArgs)
    {
        var pluginPath = ResolvePluginPath(cmd);

        if (pluginPath is null)
        {
            Console.Error.WriteLine($"mac: unknown command: {cmd}");
            Console.Error.WriteLine("Try 'mac help' or 'mac list'.");
            return 127;
        }

        if (IsExecutable(pluginPath)) return RunProcess(pluginPath, passthroughArgs, GetMacRoot());
        Console.Error.WriteLine($"mac: mac-{cmd}: not executable");
        return 126;

    }

    private static string? ResolvePluginPath(string cmd)
    {
        var pluginName = $"mac-{cmd}";
        var macroot = GetMacRoot();

        // 1) REPO-LOCAL: <MACROOT>/plugins/mac-<cmd>
        if (!string.IsNullOrWhiteSpace(macroot))
        {
            var repoPlugin = Path.Combine(macroot, "plugins", pluginName);
            if (File.Exists(repoPlugin))
                return repoPlugin;
        }

        // 2) PATH: mac-<cmd>
        return FindOnPath(pluginName);
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

        // FORWARD MACROOT FOR PLUGINS
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
        // 1) EXPLICIT ENV WINS
        var env = Environment.GetEnvironmentVariable("MACROOT");
        if (!string.IsNullOrWhiteSpace(env) && Directory.Exists(env))
            return env;

        // 2) RESOLVE FROM EXECUTABLE LOCATION
        var exe = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(exe))
            return null;

        var exeDir = Path.GetDirectoryName(exe);
        if (string.IsNullOrWhiteSpace(exeDir))
            return null;

        // DEV MODE: <repo>/.bin/mac -> repo is parent of .bin
        if (Path.GetFileName(exeDir) == ".bin")
        {
            var repo = Directory.GetParent(exeDir);
            if (repo is not null && Directory.Exists(Path.Combine(repo.FullName, "plugins")))
                return repo.FullName;
        }

        // INSTALLED MODE (OPTIONAL): <root>/plugins next to executable
        if (Directory.Exists(Path.Combine(exeDir, "plugins")))
            return exeDir;

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

    private static List<string> DiscoverPlugins()
    {
        var results = new HashSet<string>(StringComparer.Ordinal);
        var macroot = GetMacRoot();

        // 1) REPO PLUGINS
        if (!string.IsNullOrWhiteSpace(macroot))
        {
            var dir = Path.Combine(macroot, "plugins");
            if (Directory.Exists(dir))
                foreach (var file in Directory.EnumerateFiles(dir, "mac-*"))
                {
                    if (!IsExecutable(file))
                        continue;

                    var name = Path.GetFileName(file);
                    if (name.StartsWith("mac-", StringComparison.Ordinal))
                        results.Add(name["mac-".Length..]);
                }
        }

        // 2) PATH PLUGINS
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
                        if (!IsExecutable(file))
                            continue;

                        var name = Path.GetFileName(file);
                        if (name.StartsWith("mac-", StringComparison.Ordinal))
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

    private static void PrintBuiltinHelp(string name)
    {
        switch (name)
        {
            case "help":
                Console.WriteLine("mac help [command]\n  Show help. Prefers styled plugin if installed.");
                return;
            case "version":
                Console.WriteLine("mac version\n  Print version.");
                return;
            case "list" or "plugins":
                Console.WriteLine("mac list [plugins]\n  List built-ins and plugins. Use 'plugins' to list plugins only.");
                break;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("""
                          mac - macstack command line

                          Usage:
                            mac <command> [args]
                            mac help <command>
                            mac <command> --help

                          Built-ins:
                            help            Show help (prefers styled plugin if present)
                            version         Print version
                            list            List commands (built-ins + plugins)

                          Examples:
                            mac list
                            mac list plugins
                            mac help export
                          """);
    }
}
