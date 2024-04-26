using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Terrain_Planner_Tool
{
    public partial class MainWindow
    {
        public class Terrain : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #region Overrides of Object

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return $"Terrain ID {Id} - {Name}";
            }

            #endregion

            private long _id;

            public long Id {
                get { return _id; }
                init {
                    if (value == _id) return;
                    _id = value;
                    OnPropertyChanged();
                }
            }

            private string _name;

            public string Name {
                get { return _name; }
                init {
                    if (value == _name) return;
                    _name = value;
                    OnPropertyChanged();
                }
            }

            private Color _colour;

            public Color Colour {
                get { return _colour; }
                init {
                    if (value.Equals(_colour)) return;
                    _colour = value;
                    OnPropertyChanged();
                }
            }

            private string _text;

            public string Text
            {
                get { return _text; }
                set
                {
                    if (value == _text)
                    {
                        return;
                    }

                    _text = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
