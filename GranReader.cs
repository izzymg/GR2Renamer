using LSLib.Granny.GR2;
using LSLib.Granny.Model;
using System.IO;

namespace Steak
{
    public static class GranReader
    {

        /// <summary>
        /// Opens filename, reading it as a GR2 file and returning back a new suitable name.
        /// </summary>
        public static string GetGR2Filename(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                GR2Reader gr2Reader = new GR2Reader(fs);
                Root root = new Root();
                gr2Reader.Read(root);

                string name = root.FromFileName;
                int meshs = root.Meshes != null ? root.Meshes.Count : 0;
                int animations = root.Animations != null ? root.Animations.Count : 0;
                int skeletons = root.Skeletons != null ? root.Skeletons.Count : 0;

                gr2Reader.Dispose();

                string newName = name;
                if (meshs > 0)
                {
                    newName = root.Meshes[0].Name;
                }
                else if (animations > 0)
                {
                    newName = $"ANIM_{root.Animations[0].Name}";
                }
                else if (skeletons > 0)
                {
                    newName = $"SKELETON_{root.Skeletons[0].Name}";
                }

                return $"{newName}.gr2";
            }
        }
    }
}
}
