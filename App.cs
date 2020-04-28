using System.IO;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Steak
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
            Logger.Log($"Reading {cfg.Directory}...");

            // Recurse through the configured directory, queueing the files for processing.
            int queued = 0;
            Processor processor = new Processor();
            foreach (var filename in Directory.EnumerateFiles(cfg.Directory, "*.gr2", SearchOption.AllDirectories))
            {
                processor.QueueFile(filename);
                queued++;
                if (cfg.HasFileLimit && queued > cfg.FileLimit)
                {
                    break;
                }
            }

            // Process all the files in the queue
            Logger.Log($"Queued {queued} files...");
            Queue<Processor.Processed> sucessfullyProcessed = new Queue<Processor.Processed>(queued);
            List<Exception> errors = new List<Exception>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int processedCount = 0;
            foreach (var result in processor.Process())
            {
                // TODO: log error to file
                if(result.Err != null)
                {
                    errors.Add(result.Err);
                    continue;
                }
                sucessfullyProcessed.Enqueue(result);
                processedCount++;
                if (processedCount % 500 == 0)
                {
                    Logger.Log($"Progress: {processedCount}/{queued} with {errors.Count} errored files");
                }
            };

            using (var sw = File.CreateText(App.ChangeLogFilename))
            {
                foreach (var item in sucessfullyProcessed)
                {
                    sw.WriteLine($"{item.CurrentName} -> {item.NewName}");
                }
            }
            using (var sw = File.CreateText(App.ErrorLogFilename))
            {
                foreach (var item in errors)
                {
                    sw.WriteLine(item.Message);
                }
            }

            stopwatch.Stop();
            Logger.Log($"Done. Processed {sucessfullyProcessed.Count} files with {errors.Count} errored in {stopwatch.ElapsedMilliseconds}ms.");
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
