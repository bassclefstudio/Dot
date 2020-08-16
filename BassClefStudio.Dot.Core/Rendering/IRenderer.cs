using BassClefStudio.Dot.Core.Levels;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Dot.Core.Rendering
{
    public interface IRenderer<in T>
    {
        bool CanRender(T value);

        void Render(T value, SKCanvas canvas);
    }

    public abstract class SegmentRenderer : IRenderer<Segment>    
    {
        public SegmentType SegmentType { get; }

        public SegmentRenderer(SegmentType segmentType)
        {
            SegmentType = segmentType;
        }

        public bool CanRender(Segment value)
        {
            return value != null && value.Type == SegmentType;
        }

        public abstract void Render(Segment value, SKCanvas canvas);
    }
}
