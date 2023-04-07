using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Models;

namespace TerrainEditorBlazor.Data
{
	#nullable enable
	public class MapGrid : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		private int _height;
		private int _width;

		public int Height
		{
			get => _height;
			set
			{
				if (Equals(value, _height))
				{
					return;
				}

				_height = value;
				OnPropertyChanged();
			}
		}

		public int Width
		{
			get => _width;
			set
			{
				if (Equals(value, _width))
				{
					return;
				}

				_width = value;
				OnPropertyChanged();
			}
		}

		public MapCell CellAtCoordinate(int x, int y)
		{
			return MapCells[(int) (x + y * _width)];
		}

		public List<MapCell> MapCells { get; set; } = new List<MapCell>();

		public void SetCellFill(int x, int y, Terrain? terrain)
		{
			MapCells[(int) (x + y * _width)].Terrain = terrain;
		}

		public bool ImportCells(string importText, List<Terrain> terrainList)
		{
			var terrains = importText.Split(',');
			if (terrains.Length != Height * Width)
			{
				return false;
			}

			IEnumerable<long> terrainIds;
			try
			{
				terrainIds = terrains.Select(x => long.Parse(x)).ToList();
			}
			catch
			{
				return false;
			}

			MapCells.Clear();
			// Set up the cells first
			for (var y = 0; y < _height; y++)
			{
				for (var x = 0; x < _width; x++)	
				{
					MapCells.Add(new MapCell {X = x, Y = y, Terrain = terrainList.FirstOrDefault(item => item.Id == terrainIds.ElementAt(x + y * _width))});
				}
			}

			// Set up neighbours
			for (var x = 0; x < _width; x++)
			{
				for (var y = 0; y < _height; y++)
				{
					var neighbours = new List<MapCell>();
					if (x != 0)
					{
						neighbours.Add(MapCells[(int)(x+y*_width)-1]);
					}

					if (y != 0)
					{
						neighbours.Add(MapCells[(int)(x+y*_width)-(int)_width]);
					}

					if (x < (_width - 1))
					{
						neighbours.Add(MapCells[(int)(x+y*_width)+1]);
					}

					if (y < (_height - 1))
					{
						neighbours.Add(MapCells[(int)(x+y*_width)+(int)_width]);
					}

					MapCells[(int) (x + y * _width)].Neighbors = neighbours;
				}
			}

			return true;
		}

		public void RedoGrid(bool reuseOldValues)
		{
			var oldCells = MapCells.ToList();
			MapCells.Clear();

			// Set up the cells first
			for (var y = 0; y < _height; y++)
			
			{
				for (var x = 0; x < _width; x++)
				{
					var old = reuseOldValues ? oldCells.FirstOrDefault(item => item.X == x && item.Y == y) : default;
					MapCells.Add(new MapCell {X = x, Y = y, Terrain = old?.Terrain});
				}
			}

			// Set up neighbours
			for (var x = 0; x < _width; x++)
			{
				for (var y = 0; y < _height; y++)
				{
					var neighbours = new List<MapCell>();
					if (x != 0)
					{
						neighbours.Add(MapCells[(int)(x+y*_width)-1]);
					}

					if (y != 0)
					{
						neighbours.Add(MapCells[(int)(x+y*_width)-(int)_width]);
					}

					if (x < (_width - 1))
					{
						neighbours.Add(MapCells[(int)(x+y*_width)+1]);
					}

					if (y < (_height - 1))
					{
						neighbours.Add(MapCells[(int)(x+y*_width)+(int)_width]);
					}

					MapCells[(int) (x + y * _width)].Neighbors = neighbours;
				}
			}
		}
	}
}
