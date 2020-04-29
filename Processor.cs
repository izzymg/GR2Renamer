using System;
using System.IO;
using System.Collections.Generic;
namespace Steak
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
        /// If a file already has the correct name, it will be skipped.
        /// If the new name already exists, it will be renamed with its original
        /// name as a suffix.
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

                string pureFilename = Path.GetFileNameWithoutExtension(filePath);

                try
                {
                    processed.NewName = GranReader.GetGR2Filename(filePath);
                    if (processed.NewName != processed.CurrentName)
                    {
                        try
                        {
                            File.Move(processed.CurrentName, Path.Combine(outputDirectory, processed.NewName));
                        }
                        catch (IOException)
                        {
                            processed.NewName = $"{processed.CurrentName}_{pureFilename}";
                            File.Move(processed.CurrentName, Path.Combine(outputDirectory, processed.NewName));
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
