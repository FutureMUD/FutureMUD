#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

public class FileSignalGeneratorGameItemComponent : PoweredMachineBaseGameItemComponent, IFileSignalGenerator
{
	private FileSignalGeneratorGameItemComponentProto _prototype;
	private readonly ComputerMutableFileSystem _fileSystem;
	private ComputerSignal _currentSignal;
	private double _parsedSignalValue;
	private string _fileStatus = "No signal file present.";

	public FileSignalGeneratorGameItemComponent(FileSignalGeneratorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
		_fileSystem = new ComputerMutableFileSystem(proto.FileCapacityInBytes);
		_fileSystem.FileChanged += FileSystemOnFileChanged;
		EnsureSignalFileExists();
		RefreshFileState();
	}

	public FileSignalGeneratorGameItemComponent(MudSharp.Models.GameItemComponent component,
		FileSignalGeneratorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_fileSystem = new ComputerMutableFileSystem(proto.FileCapacityInBytes);
		_fileSystem.FileChanged += FileSystemOnFileChanged;
		LoadRuntimeState(XElement.Parse(component.Definition));
		RefreshFileState();
	}

	public FileSignalGeneratorGameItemComponent(FileSignalGeneratorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_fileSystem = new ComputerMutableFileSystem(rhs._fileSystem.CapacityInBytes);
		_fileSystem.FileChanged += FileSystemOnFileChanged;
		_fileSystem.LoadFiles(rhs._fileSystem.MutableFiles.Select(x => new ComputerMutableTextFile
		{
			FileName = x.FileName,
			TextContents = x.TextContents,
			CreatedAtUtc = x.CreatedAtUtc,
			LastModifiedAtUtc = x.LastModifiedAtUtc,
			PubliclyAccessible = x.PubliclyAccessible
		}));
		_parsedSignalValue = rhs._parsedSignalValue;
		_fileStatus = rhs._fileStatus;
		_currentSignal = rhs._currentSignal;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public long FileOwnerId => Id;
	public IComputerFileSystem? FileSystem => _fileSystem;
	public string SignalFileName => _prototype.SignalFileName;
	public double ParsedSignalValue => _parsedSignalValue;
	public bool FileValueValid => string.IsNullOrEmpty(GetFileParseError());
	public string FileStatus => _fileStatus;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new FileSignalGeneratorGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override int DecorationPriority => 1000;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var file = _fileSystem.GetFile(SignalFileName);
		return
			$"{description}\n\nIts file-driven signal generator is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}, reading {SignalFileName.ColourCommand()} and currently outputting {CurrentValue.ToString("N2", voyeur).ColourValue()}.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (FileSignalGeneratorGameItemComponentProto)newProto;
		_fileSystem.CapacityInBytes = _prototype.FileCapacityInBytes;
		EnsureSignalFileExists();
		RefreshFileState();
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(ComputerMutableOwnerXmlPersistence.SaveFiles(_fileSystem.MutableFiles));
		return root;
	}

	public override void Login()
	{
		base.Login();
		RefreshFileState();
		ApplyLiveSignalState();
	}

	public override void Quit()
	{
		base.Quit();
	}

	public override void Delete()
	{
		_fileSystem.FileChanged -= FileSystemOnFileChanged;
		base.Delete();
	}

	protected override void OnPowerCutInAction()
	{
		RefreshFileState();
		ApplyLiveSignalState();
	}

	protected override void OnPowerCutOutAction()
	{
		SetSignal(default);
	}

	private void LoadRuntimeState(XElement root)
	{
		_fileSystem.LoadFiles(ComputerMutableOwnerXmlPersistence.LoadFiles(root.Element("Files")));
		EnsureSignalFileExists();
	}

	private void FileSystemOnFileChanged(IComputerFileSystem fileSystem, ComputerFileSystemChange change)
	{
		Changed = true;
		if (!change.FileName.Equals(SignalFileName, StringComparison.InvariantCultureIgnoreCase))
		{
			return;
		}

		RefreshFileState();
		ApplyLiveSignalState();
	}

	private void EnsureSignalFileExists()
	{
		if (_fileSystem.FileExists(SignalFileName))
		{
			return;
		}

		_fileSystem.WriteFile(SignalFileName, _prototype.InitialFileContents);
		_fileSystem.SetFilePubliclyAccessible(SignalFileName, _prototype.PubliclyAccessibleByDefault);
	}

	private void RefreshFileState()
	{
		var file = _fileSystem.GetFile(SignalFileName);
		if (file is null)
		{
			_parsedSignalValue = 0.0;
			_fileStatus = $"missing file {SignalFileName.ColourCommand()}";
			return;
		}

		var trimmed = (file.TextContents ?? string.Empty).Trim();
		if (string.IsNullOrWhiteSpace(trimmed))
		{
			_parsedSignalValue = 0.0;
			_fileStatus = $"{SignalFileName.ColourCommand()} is empty";
			return;
		}

		if (!double.TryParse(trimmed, out var value) || double.IsNaN(value) || double.IsInfinity(value))
		{
			_parsedSignalValue = 0.0;
			_fileStatus = $"{SignalFileName.ColourCommand()} does not currently contain a valid number";
			return;
		}

		_parsedSignalValue = value;
		_fileStatus = $"{SignalFileName.ColourCommand()} currently contains {value.ToString("N2").ColourValue()}";
	}

	private string GetFileParseError()
	{
		var file = _fileSystem.GetFile(SignalFileName);
		if (file is null)
		{
			return "missing";
		}

		var trimmed = (file.TextContents ?? string.Empty).Trim();
		if (string.IsNullOrWhiteSpace(trimmed))
		{
			return "empty";
		}

		return double.TryParse(trimmed, out var value) && !double.IsNaN(value) && !double.IsInfinity(value)
			? string.Empty
			: "invalid";
	}

	private void ApplyLiveSignalState()
	{
		if (!SwitchedOn || !IsPowered)
		{
			SetSignal(default);
			return;
		}

		SetSignal(new ComputerSignal(_parsedSignalValue, null, null));
	}

	private void SetSignal(ComputerSignal signal)
	{
		if (SignalComponentUtilities.SignalsEqual(_currentSignal, signal))
		{
			return;
		}

		_currentSignal = signal;
		SignalChanged?.Invoke(this, signal);
	}
}
