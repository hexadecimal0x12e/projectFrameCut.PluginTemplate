using projectFrameCut.Render.RenderAPIBase.EffectAndMixture;
using projectFrameCut.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace nobody
{
    /// <summary>
    /// A example effect that converts any frame to a blank frame.
    /// </summary>
    public class ToBlankFrameEffect : IEffect
    {
        public string FromPlugin => "nobody.MyExamplePlugin";

        public string TypeName => "ToBlankFrameEffect";

        public string Name { get; set; }

        public Dictionary<string, object> Parameters => new();

        public bool Enabled { get; set; }
        public int Index { get; set; }

        public List<string> ParametersNeeded => new();

        public Dictionary<string, string> ParametersType => new();

        public string? NeedComputer => null;

        public int RelativeWidth { get; set; }
        public int RelativeHeight { get; set; }

        public IPicture Render(IPicture source, IComputer? computer, int targetWidth, int targetHeight)
        {
            if (source.bitPerPixel == 8) return Picture8bpp.GenerateSolidColor(targetWidth, targetHeight, (byte)0, (byte)0, (byte)0, null);
            else if (source.bitPerPixel == 16) return Picture16bpp.GenerateSolidColor(targetWidth, targetHeight, (ushort)0, (ushort)0, (ushort)0, null);
            else throw new NotSupportedException("Specific mode is unsupported.");
        }

        public IEffect WithParameters(Dictionary<string, object> parameters)
        {
            return new ToBlankFrameEffect();
        }
    }
}
