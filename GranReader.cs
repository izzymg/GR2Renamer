using LSLib.Granny.GR2;
using LSLib.Granny.Model;
using System.IO;
using System.Diagnostics;

namespace GR2Renamer
{
    public static class GranReader
    {

        /// <summary>
        /// Opens filename, reading it as a GR2 file and returning back a new suitable name.
        /// Returns an empty string if no suitable name was found.
        /// </summary>
        public static string GetGR2Name(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                GR2Reader gr2Reader = new GR2Reader(fs);
                Root root = new Root();
                gr2Reader.Read(root);
                gr2Reader.Dispose();

                if (root.Meshes?.Count > 0)
                {
                    return root.Meshes[0].Name;
                }
                else if (root.Animations?.Count > 0)
                {
                    return $"ANIM_{root.Animations[0].Name}";
                }
                else if (root.Skeletons?.Count > 0)
                {
                    return $"SKELETONS_{root.Skeletons.Count}:{root.Skeletons[0].Bones?.Count}";
                }
                return "";
            }
        }
    }
}
