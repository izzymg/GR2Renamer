using System;
using System.IO;
using System.Collections.Generic;
namespace GR2Renamer
{
    public class Processor
    {

        public struct Processed
        {
            public string CurrentName;
            public string NewName;
            public Exception Err;
        }

        private Queue<string> filepaths = new Queue<string>();

        /// <summary>
        /// Recurses a directory, queuing all gr2 files for processing.
        /// If limit is greater than 0, queues only limit files.
        /// Returns number of files queued.
        /// </summary>
        public int QueueDirectory(string directory, int limit)
        {
            int queued = 0;
            foreach (var filename in Directory.EnumerateFiles(directory, "*.gr2", SearchOption.AllDirectories))
            {
                queued++;
                this.filepaths.Enqueue(filename);
                if (limit > 0 && queued > limit)
                {
                    break;
                }
            }
            return queued;
        }

        /// <summary>
        /// Processes the queued filenames, reading them as GR2 data,
        /// renaming them to an appropriate name, and returning data about
        /// the rename.
        /// </summary>
        public IEnumerable<Processed> Process(string outputDirectory)
        {
            while (this.filepaths.Count > 0)
            {
                string filePath = this.filepaths.Dequeue();
                Processed processed = new Processed
                {
                    CurrentName = filePath,
                };
                try
                {
                    string generatedName = GranReader.GetGR2Name(filePath);
                    if (generatedName.Length > 0)
                    {
                        string originalFilename = Path.GetFileNameWithoutExtension(filePath);
                        string from = filePath;
                        processed.NewName = Path.Combine(outputDirectory, $"{generatedName}_{originalFilename}.gr2");
                        if (processed.CurrentName != processed.NewName)
                        {
                            File.Move(processed.CurrentName, processed.NewName);
                        }
                    }
                }
                catch (Exception e)
                {
                    processed.Err = new Exception($"Failed to process {filePath}: {e.Message}");
                }

                yield return processed;
            }
        }
    }
}
