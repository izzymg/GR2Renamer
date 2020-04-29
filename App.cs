using System.IO;
using System;
using System.Diagnostics;
using System.Linq;

namespace GR2Renamer
{

    /// <summary>
    /// Formatted console io wrapper.
    /// </summary>
    public static class Logger
    {
        public static void Log(string text, bool isError = false)
        {
            string formatted = $"{DateTime.Now.ToShortTimeString()} GR2Renamer: {text}";
            if (isError)
            {
                Console.Error.WriteLine($"{formatted}");
                return;
            }
            Console.WriteLine(formatted);
        }
    }

    /// <summary>
    /// gr2renamer - uses LSLib to process and rename GR2 files according
    /// to their internal data.
    /// </summary>
    public class App
    {

        private const string ChangeLogFilename = "change_log.txt";
        private const string ErrorLogFilename = "error.txt";

        /// <summary>
        /// App configuration.
        /// </summary>
        private struct Config
        {
            public bool HasFileLimit;
            public int FileLimit;
            public string Directory;
        }

        /// <summary>
        /// Populate a config struct based on command line arguments.
        /// Throws readable exception strings.
        /// </summary>
        private static Config GetConfig(string[] cmdlArgs)
        {
            if (cmdlArgs.Length < 1)
            {
                throw new Exception("Usage: gr2renamer [directory] [max files]?");
            }

            var cfg = new Config
            {
                Directory = cmdlArgs[0],
                HasFileLimit = cmdlArgs.Length > 1,
            };

            try
            {
                if (cfg.HasFileLimit)
                {
                    cfg.FileLimit = int.Parse(cmdlArgs[1]);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Invalid maximum file count, exiting");
            }
            return cfg;
        }

        /// <summary>
        /// gr2renamer entry point.
        /// </summary>
        private static void Run(string[] cmdlineArgs)
        {
            Config cfg = App.GetConfig(cmdlineArgs);
            Logger.Log($"Using {cfg.Directory}...");

            Processor processor = new Processor();
            int queued = processor.QueueDirectory(cfg.Directory, cfg.HasFileLimit ? cfg.FileLimit : 0);
            Logger.Log($"Queued {queued} files...");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var errorLog = File.CreateText(App.ErrorLogFilename))
            { 
                using (var changeLog = File.CreateText(App.ChangeLogFilename))
                {

                    var query = processor.Process(cfg.Directory).Select((Processor.Processed item, int index) =>
                    {
                        if(index % 500 == 0)
                        {
                            Logger.Log($"Progress: {index}/{queued}, {stopwatch.ElapsedMilliseconds}ms");
                        }
                        if (item.Err != null)
                        {
                            errorLog.WriteLine($"{item.CurrentName}: {item.Err.Message}");
                            return 1;
                        }
                        changeLog.WriteLine($"{item.CurrentName} -> {item.NewName}");
                        return 0;
                    });

                    int errors = query.Sum();

                    stopwatch.Stop();
                    Logger.Log($"Done. Completed with {errors} errored files in {stopwatch.ElapsedMilliseconds}ms.");
                }
            }

        }

        private static void Main(string[] args)
        {
            try
            {
                App.Run(args);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, true);
            }
        }
    }
}
