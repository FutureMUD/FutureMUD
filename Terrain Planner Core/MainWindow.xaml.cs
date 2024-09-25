using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;
using Microsoft.Win32;
using MudSharp.Framework;
using MySql.Data.MySqlClient;

namespace Terrain_Planner_Tool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly List<MapCell> _cells = new List<MapCell>();
		private readonly List<Terrain> _terrains = new List<Terrain>();
		private Terrain _currentBrush;
		private bool _fillMode;
		public MainWindow()
		{
			InitializeComponent();
			GridHeightTextbox.Text = "5";
			GridWidthTextbox.Text = "5";
			SetupTerrain();
			RedoGrid();
		}

		public void SetupTerrain() {
			_terrains.Add(new Terrain{Colour = Colors.DarkGray, Id = 0, Name = "None"});
			
			foreach (var terrain in _terrains)
			{
				var button = new Button {
					DataContext = terrain,
					Content = terrain.Name,
					Background = new SolidColorBrush(terrain.Colour)
				};
				button.Click += (sender, args) => {
					_currentBrush = (Terrain)((Button)sender).DataContext;
				};
				PalettePanel.Children.Add(button);
			}

			_currentBrush = _terrains.First();
		}

		private bool _drawingWithMouse;

		public void RedoGrid()
		{
			var height = uint.TryParse(GridHeightTextbox.Text, out var hvalue) ? hvalue : 1;
			var width = uint.TryParse(GridWidthTextbox.Text, out var wvalue) ? wvalue : 1;
			_cells.Clear();
			DisplayGrid.Children.Clear();
			DisplayGrid.Columns = (int)width;
			DisplayGrid.Rows = (int)height;
			for (var i = 0; i < height * width; i++)
			{
				var cell = new MapCell
				{
					Terrain = _terrains.First(), 
					X = (uint)(i % width), 
					Y = (uint) (i / width)
				};
				_cells.Add(cell);
				var button = new Label
				{
					DataContext = cell,
					BorderBrush = Brushes.Black,
					BorderThickness = new Thickness(0.1),
					ToolTip = new ToolTip(),
					HorizontalContentAlignment = HorizontalAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Center
				};
				button.SetBinding(BackgroundProperty, new Binding
				{
					Path = new PropertyPath("Terrain.Colour"),
					Converter = new ColorToSolidColorBrushConverter()
				});
				button.SetBinding(ContentProperty, new Binding
				{
					Path = new PropertyPath("Terrain.Text")
				});
				button.SetBinding(MinWidthProperty,
								  new Binding {
									  Path = new PropertyPath("ActualHeight"),
									  RelativeSource = new RelativeSource(RelativeSourceMode.Self)
								  });
				button.SetBinding(MinHeightProperty,
								  new Binding {
									  Path = new PropertyPath("ActualWidth"),
									  RelativeSource = new RelativeSource(RelativeSourceMode.Self)
								  });
				button.SetBinding(ToolTipProperty, new Binding
				{
					Path = new PropertyPath("Terrain"),
					Converter = new NullableTerrainNameConverter()
				});
				button.Height = 30;
				DisplayGrid.Children.Add(button);
				button.MouseEnter += (sender, args) => {
					InteractWithMapCell(((MapCell)((Label)sender).DataContext));
				};
				button.MouseLeftButtonDown += (sender, args) => {
					_drawingWithMouse = true;
					InteractWithMapCell(((MapCell)((Label)sender).DataContext));
				};
				button.MouseLeftButtonUp += (sender, args) => {
					_drawingWithMouse = false;
				};
			}

			for (var i = 0; i < width; i++)
			{
				for (var j = 0; j < height; j++)
				{
					var neighbours = new List<MapCell>();
					if (i != 0)
					{
						neighbours.Add(_cells[(int)(i+j*width)-1]);
					}

					if (j != 0)
					{
						neighbours.Add(_cells[(int)(i+j*width)-(int)width]);
					}

					if (i < (width - 1))
					{
						neighbours.Add(_cells[(int)(i+j*width)+1]);
					}

					if (j < (height - 1))
					{
						neighbours.Add(_cells[(int)(i+j*width)+(int)width]);
					}

					_cells[(int) (i + j * width)].Neighbors = neighbours;
				}
			}
		}

		private void InteractWithMapCell(MapCell cell)
		{
			if (_drawingWithMouse)
			{
				if (_fillMode)
				{
					var consideredCells = new HashSet<MapCell>();
					var cellQueue = new Queue<MapCell>();
					cellQueue.Enqueue(cell);
					var target = cell.Terrain;
					while (true)
					{
						var thisCell = cellQueue.Dequeue();
						if (consideredCells.Contains(thisCell))
						{
							if (!cellQueue.Any())
							{
								break;
							}

							continue;
						}

						thisCell.Terrain = _currentBrush;
						consideredCells.Add(thisCell);
						foreach (var other in thisCell.Neighbors)
						{
							if (other.Terrain == target)
							{
								cellQueue.Enqueue(other);
							}
						}

						if (!cellQueue.Any())
						{
							break;
						}
					}

					return;
				}
				cell.Terrain = _currentBrush;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			RedoGrid();
		}

		private void Button_Click_Export(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(_cells.Select(x => x.Terrain.Id.ToString()).Aggregate((x, y) => $"{x},{y}"));
			MessageBox.Show("Copied the arguments to the clipboard. Paste these as your \"mask\" in the MUD.");
		}

		private void Button_Click_Upload(object sender, RoutedEventArgs e) {
			var mask = Clipboard.GetText();
			var split = mask.Split(',');
			if (split.Length != _cells.Count) {
				MessageBox.Show($"The number of values in your mask was {split.Length}, which doesn't match the size of your grid. Set the size of your grid appropriately first.");
				return;
			}

			for (var i = 0; i < split.Length; i++) {
				if (!long.TryParse(split[i], out var value))
				{
					MessageBox.Show($"The {(i+1).ToOrdinal()} value in the mask was not a valid ID number.");
					return;
				}

				var terrain = _terrains.FirstOrDefault(x => x.Id == value);
				if (terrain == null) {
					MessageBox.Show($"The {(i + 1).ToOrdinal()} value in the mask did not map to a valid terrain type.");
					return;
				}

				_cells[i].Terrain = terrain;
			}

			MessageBox.Show("Your mask has been reloaded.");
		}

		private void Button_Click_Paint(object sender, RoutedEventArgs e)
		{
			_fillMode = false;
			FillToolButton.FontWeight = FontWeights.Regular;
			PaintToolButton.FontWeight = FontWeights.Bold;
		}

		private void Button_Click_Fill(object sender, RoutedEventArgs e)
		{
			_fillMode = true;
			FillToolButton.FontWeight = FontWeights.Bold;
			PaintToolButton.FontWeight = FontWeights.Regular;
		}

		private class ImportTerrain
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string TerrainEditorColour { get; set; }
			public string TerrainEditorText { get; set; }

			public Terrain Terrain
			{
				get
				{
					return new Terrain
					{
						Id = Id,
						Name = Name,
						Text = TerrainEditorText ?? string.Empty,
						Colour = TerrainEditorColour != null ? (Color) ColorConverter.ConvertFromString(TerrainEditorColour) : Colors.Brown
					};
				}
			}
		}

		private async void Button_Click_Import_Terrains_From_API(object sender, RoutedEventArgs e)
		{
			await using var fs = new FileStream("apiaddress.config", FileMode.OpenOrCreate, FileAccess.ReadWrite);
			using var reader = new StreamReader(fs);

			var address = await reader.ReadLineAsync();
			if (string.IsNullOrWhiteSpace(address))
			{
				MessageBox.Show(
					"There was no information specified in the apiaddress.config file pertaining to the web api address. Please fill in this information and try again.");
				return;
			}
			var client = new HttpClient();
			var response = await client.GetAsync(address);
			if (response.IsSuccessStatusCode)
			{
				var deserialise = await response.Content.ReadFromJsonAsync<List<ImportTerrain>>();
				if (deserialise == null)
				{
					MessageBox.Show("Invalid terrain information returned");
				}
				_terrains.Clear();
				_terrains.Add(new Terrain {Colour = Colors.DarkGray, Id = 0, Name = "None", Text = ""});
				foreach (var terrain in deserialise)
				{
					_terrains.Add(terrain.Terrain);
				}

				foreach (var cell in _cells)
				{
					cell.Terrain = _terrains.First();
				}

				PalettePanel.Children.Clear();
				foreach (var terrain in _terrains)
				{
					var button = new Button {
						DataContext = terrain,
						Content = terrain.Name,
						Background = new SolidColorBrush(terrain.Colour)
					};
					button.Click += (senderinternal, args) => {
						_currentBrush = (Terrain)((Button)senderinternal).DataContext;
					};
					PalettePanel.Children.Add(button);
				}

				_currentBrush = _terrains.First();
				MessageBox.Show("Successfully imported terrains.");
				return;
			}

			MessageBox.Show(response.ReasonPhrase);

		}

		private void Button_Click_Import_Terrains(object sender, RoutedEventArgs e)
		{
			try
			{
				var json = Clipboard.GetText();
				var deserialise =
					JsonSerializer.Deserialize<List<ImportTerrain>>(json);
				_terrains.Clear();
				_terrains.Add(new Terrain {Colour = Colors.DarkGray, Id = 0, Name = "None", Text = ""});
				foreach (var terrain in deserialise)
				{
					_terrains.Add(terrain.Terrain);
				}

				foreach (var cell in _cells)
				{
					cell.Terrain = _terrains.First();
				}

				PalettePanel.Children.Clear();
				foreach (var terrain in _terrains)
				{
					var button = new Button {
						DataContext = terrain,
						Content = terrain.Name,
						Background = new SolidColorBrush(terrain.Colour)
					};
					button.Click += (senderinternal, args) => {
						_currentBrush = (Terrain)((Button)senderinternal).DataContext;
					};
					PalettePanel.Children.Add(button);
				}

				_currentBrush = _terrains.First();
				MessageBox.Show("Successfully imported terrains.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Encountered a problem with importing terrains from your clipboard information.\n\n{ex.Message}");
			}
		}
	}
}
