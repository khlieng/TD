using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = TDContentProcessors.Map;

namespace TDContentProcessors
{
    [ContentTypeWriter]
    public class MapWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.Data);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "TD.Map+MapReader, TD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        }
    }
}
