using JetBrains.Annotations;
using MudSharp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Terrain_Planner_Blazor_WASM.Data
{
    public class MapCell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int X { get; init; }
        public int Y { get; init; }

        private Terrain? _terrain;
        public Terrain? Terrain
        {
            get => _terrain;
            set
            {
                if (Equals(value, _terrain))
                {
                    return;
                }

                _terrain = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<MapCell> Neighbors { get; set; } = Enumerable.Empty<MapCell>();
    }
}
