using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Communication
{
    public enum DrawingSize
    {
        Scribble,
        Doodle,
        Figure,
        Sketch,
        Picture,
        Poster,
        Mural
    }
    
    public interface IDrawing : ICanBeRead
    {
        DrawingSize DrawingSize { get; }
        string ShortDescription { get; }
        string FullDescription { get; }
        double DrawingSkill { get; }
        IDrawing Copy();
    }
}
