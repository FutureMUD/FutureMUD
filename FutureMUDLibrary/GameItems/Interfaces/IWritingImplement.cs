using System;
using MudSharp.Form.Colour;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces {
    public enum WritingImplementType {
        Pencil,
        Biro,
        Quill,
        Stylus,
        ComputerStylus,
        Brush,
        Chisel,
        Crayon
    }

    public static class WritingImplementTypeExtensions {
        public static string Describe(this WritingImplementType type) {
            switch (type) {
                case WritingImplementType.Biro:
                    return "Biro";
                case WritingImplementType.Brush:
                    return "Brush";
                case WritingImplementType.ComputerStylus:
                    return "Computer Stylus";
                case WritingImplementType.Pencil:
                    return "Pencil";
                case WritingImplementType.Quill:
                    return "Quill";
                case WritingImplementType.Stylus:
                    return "Stylus";
                case WritingImplementType.Chisel:
                    return "Chisel";
                case WritingImplementType.Crayon:
                    return "Crayon";
                default:
                    throw new ApplicationException($"Unknown WritingImplementType {type} in WritingImplementTypeExtensions.Describe");
            }
        }

        public static string Describe(this WritingImplementType type, IColour colour, ANSIColour ansiColour = null) {
            switch (type) {
                case WritingImplementType.Biro:
                    return $"{colour?.Name?.Colour(ansiColour)} ink";
                case WritingImplementType.Brush:
                    return $"{colour?.Name?.Colour(ansiColour)} paint";
                case WritingImplementType.ComputerStylus:
                    return $"stylus";
                case WritingImplementType.Pencil:
                    return $"{colour?.Name?.Colour(ansiColour)} pencil";
                case WritingImplementType.Quill:
                    return $"{colour?.Name?.Colour(ansiColour)} ink";
                case WritingImplementType.Crayon:
                    return $"{colour?.Name?.Colour(ansiColour)} crayon";
                case WritingImplementType.Stylus:
                    return "stylus";
                case WritingImplementType.Chisel:
                    return "chisel";
                default:
                    throw new ApplicationException($"Unknown WritingImplementType {type} in WritingImplementTypeExtensions.Describe");
            }
        }
    }

    public interface IWritingImplement : IGameItemComponent {
        WritingImplementType WritingImplementType { get; }
        bool Primed { get; }
        void Use(int uses);
        IColour WritingImplementColour { get; }
    }
}
