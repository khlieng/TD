using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using TImport = TDContentProcessors.Map;

namespace TDContentProcessors
{
    [ContentImporter(".map", DisplayName = "Map Importer", DefaultProcessor = "")]
    public class MapImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            TImport map = new TImport();
            using (Stream stream = File.OpenRead(filename))
            {
                map.Data = new byte[stream.Length];
                stream.Read(map.Data, 0, (int)stream.Length);
            }
            return map;
        }
    }
}
