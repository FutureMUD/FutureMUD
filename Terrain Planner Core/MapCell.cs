using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MudSharp.Framework;

namespace Terrain_Planner_Tool
{
    public partial class MainWindow
    {
        public class MapCell : INotifyPropertyChanged
        {
            private Terrain _terrain;

            public Terrain Terrain {
                get { return _terrain; }
                set {
                    if (Equals(value, _terrain)) return;
                    _terrain = value;
                    OnPropertyChanged();
                }
            }

            private readonly List<string> _features = new List<string>();

            public List<string> Features => _features;

            public uint X { get; set; }
            public uint Y { get; set; }

            public IEnumerable<MapCell> Neighbors { get; set; }

            #region Overrides of Object

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return $"MapCell with Terrain {Terrain?.Name ?? "None"} and Features {Features.DefaultIfEmpty("None").ListToString()}";
            }

            #endregion

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
