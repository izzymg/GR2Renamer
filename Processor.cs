using System;
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

        private Queue<string> filenames = new Queue<string>();

        /// <summary>
        /// Adds a file to the queued files to be processed.
        /// </summary>
        public void QueueFile(string filename)
        {
            this.filenames.Enqueue(filename);
        }

        /// <summary>
        /// Processes the queued filenames, reading them as GR2 data
        /// and returning new filenames for the files.
        /// </summary>
        public IEnumerable<Processed> Process()
        {
            while (this.filenames.Count > 0)
            {
                string filename = this.filenames.Dequeue();
                Processed processed = new Processed
                {
                    CurrentName = filename,
                };

                try
                {
                    processed.NewName = GranReader.GetGR2Filename(filename);
                }
                catch (Exception e)
                {
                    processed.Err = new Exception($"Failed to process {filename}: {e.Message}");
                }

                yield return processed;
            }
        }
    }
}
