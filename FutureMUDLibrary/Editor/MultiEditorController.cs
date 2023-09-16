using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Editor;

public class MultiEditorController : IControllable
    {
#nullable enable
	private IEditor? _editor;
        private readonly Queue<IEditor> _editors;
	private readonly Queue<string> _editorTexts;
        private readonly IControllable _originalController;
	private readonly object[] _suppliedArguments;
	private readonly Action<IEnumerable<string>, IOutputHandler, object[]> _postAction;
	private readonly Action<IOutputHandler, object[]> _cancelAction;
	private readonly List<string> _editorResults = new();

	public MultiEditorController(IControllable originalController, object[] suppliedArguments,
		Action<IEnumerable<string>, IOutputHandler, object[]> postAction, Action<IOutputHandler, object[]> cancelAction,
		EditorOptions options,
		IEnumerable<string> editorTexts,
		double charLengthMultiplier = 1.0, string? recallText = null)
	{
		_originalController = originalController;
		_suppliedArguments = suppliedArguments;
		_postAction = postAction;
		_cancelAction = cancelAction;
		_editors = new Queue<IEditor>(from text in editorTexts
				   select new Editor(suppliedArguments, null, null, options, charLengthMultiplier, recallText));
		_editorTexts = new Queue<string>(editorTexts);

	}

	#region ITimeout Members

	public int Timeout => _originalController.Timeout;

	#endregion

	#region IControllable Members

	bool IControllable.HasPrompt => true;

	string IControllable.Prompt => $"]";

	public IControllable? SubContext { get; private set; }

	public IControllable? NextContext { get; private set; }

	public bool HandleSubContext(string command)
	{
		return false;
	}

	public ICharacterController? Controller { get; private set; }

	IController IControllable.Controller => Controller;

	public void AssumeControl(IController controller)
	{
		Controller = (ICharacterController)controller;
		OutputHandler = controller.OutputHandler;
		NextContext = null;
		_editor = _editors.Dequeue();
		_editor!.Register(OutputHandler);
		OutputHandler.Send("\n" + _editorTexts.Dequeue());
	}

	public void SilentAssumeControl(IController controller)
	{
		AssumeControl(controller);
	}

	public bool ExecuteCommand(string command)
	{
		_editor!.HandleCommand(command);
		if (_editor.Status == EditorStatus.Cancelled)
		{
			if (_cancelAction is not null)
			{
				_cancelAction(OutputHandler!, _suppliedArguments);
			}

			NextContext = _originalController;
			return true;
		}

		if (_editor.Status == EditorStatus.Submitted)
		{
			_editorResults.Add(_editor.FinalText);
			if (_editors.Count < 1)
			{
				if (_postAction is not null)
				{
					_postAction(_editorResults, OutputHandler!, _suppliedArguments);
				}

				NextContext = _originalController;
				return true;
			}

			_editor = _editors.Dequeue();
			_editor.Register(OutputHandler);
			OutputHandler!.Send(_editorTexts.Dequeue());
		}

		return true;
	}

	public void LoseControl(IController controller)
	{
		Controller = null;
		OutputHandler = null;
	}

	#endregion

	#region IHandleOutput Members

	public IOutputHandler? OutputHandler { get; private set; }

	public void Register(IOutputHandler handler)
	{
		OutputHandler = handler;
	}

	#endregion
}
