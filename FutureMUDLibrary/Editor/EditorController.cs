using System;
using System.Collections;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Editor;

public class EditorController : IControllable {
        private readonly IEditor _editor;
        private readonly IControllable _originalController;

        public EditorController(IControllable originalController, object[] suppliedArguments,
            Action<string, IOutputHandler, object[]> postAction, Action<IOutputHandler, object[]> cancelAction,
            EditorOptions options, double charLengthMultiplier = 1.0, string recallText = null) {
            _originalController = originalController;
            _editor = new Editor(suppliedArguments, postAction, cancelAction, options, charLengthMultiplier, recallText);
        }

        #region ITimeout Members

        public int Timeout => _originalController.Timeout;

        #endregion

        #region IControllable Members

        bool IControllable.HasPrompt => true;

        string IControllable.Prompt => $"]";

        public IControllable SubContext { get; private set; }

        public IControllable NextContext { get; private set; }

        public bool HandleSubContext(string command) {
            return false;
        }

        public ICharacterController Controller { get; private set; }

        IController IControllable.Controller => Controller;

        public void AssumeControl(IController controller) {
            Controller = (ICharacterController) controller;
            OutputHandler = controller.OutputHandler;
            _editor.Register(OutputHandler);
            NextContext = null;
        }

        public void SilentAssumeControl(IController controller) {
            AssumeControl(controller);
        }

        public bool ExecuteCommand(string command) {
            _editor.HandleCommand(command);
            if ((_editor.Status == EditorStatus.Cancelled || _editor.Status == EditorStatus.Submitted) && NextContext is null) {
                NextContext = _originalController;
            }

            return true;
        }

        public void LoseControl(IController controller) {
            Controller = null;
            OutputHandler = null;
        }

        #endregion

        #region IHandleOutput Members

        public IOutputHandler OutputHandler { get; private set; }

        public void Register(IOutputHandler handler) {
            OutputHandler = handler;
        }

        #endregion
    }