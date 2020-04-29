using LSLib.Granny.GR2;
using LSLib.Granny.Model;
using System.IO;
using System.Diagnostics;

namespace Steak
{
    public static class GranReader
    {

        /// <summary>
        /// Opens filename, reading it as a GR2 file and returning back a new suitable name (no extension).
        /// Returns an empty string if no suitable name was found.
        /// </summary>
        public static string GetGR2Filename(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                GR2Reader gr2Reader = new GR2Reader(fs);
                Root root = new Root();
                gr2Reader.Read(root);
                gr2Reader.Dispose();

                if (root.Meshes != null && root.Meshes.Count > 0)
                {
                    return root.Meshes[0].Name;
                }
                else if (root.Animations != null && root.Animations.Count > 0)
                {
                    return $"ANIM_{root.Animations[0].Name}";
                }
                else if (root.Skeletons != null && root.Skeletons.Count > 0)
                {
                    return $"SKELETON_{root.Skeletons[0].Name}";
                }
                return "";
            }
        }
    }
}
