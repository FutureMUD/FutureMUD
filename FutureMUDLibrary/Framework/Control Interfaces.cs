using System;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.PerceptionEngine;

namespace MudSharp.Framework {
    public interface IMonitor : IHandleOutput {
        void AddObservee(IMonitorable observee);
        void RemoveObservee(IMonitorable observee);

        void UpdateObservers();
    }

    public interface IMonitorable {
        void AddObserver(IMonitor observer);
        void RemoveObserver(IMonitor observer);
    }

    public interface IControllable : IHandleOutput, ITimeout {
        IControllable SubContext { get; }

        IControllable NextContext { get; }

        public IController Controller { get; }

        public bool HasPrompt { get; }
        public string Prompt { get; }

        public bool HandleSubContext(string command);

        public void AssumeControl(IController controller);
        public void SilentAssumeControl(IController controller);
        public bool ExecuteCommand(string command);
        public void LoseControl(IController controller);
    }

    public interface IArbitrarilyControllable : IControllable {
        void OutOfContextExecuteCommand(string command);
    }

    public interface IController : IHandleOutput {
        public void Close();
        public void SetContext(IControllable context);
    }

    public interface ISubContextController : IController {
        public void CloseSubContext();
    }

    public interface IHandleCommands {
        void HandleCommand(string command);
    }

    public interface ITimeout {
        public int Timeout { get; }
    }

    public interface IHaveFuturemud {
        IFuturemud Gameworld { get; }
    }

    public interface IAccountController : IHandleCommands, IController, IDisposable, IMonitorable, IMonitor {
        IAccount Account { get; }

        void BindAccount(IAccount account);
        void DetachConnection();
    }

    public interface IFuturemudAccountController : IAccountController, IHaveFuturemud {
    }

    public interface ICharacterController : IHandleCommands, IController {
        ICharacter Actor { get; }

        /// <summary>
        ///     Gets tags related to Control Context information such as (Idle) (Disconnected) etc.
        /// </summary>
        string LDescAdditionalTags { get; } 
        long InactivityMilliseconds { get; }
        void UpdateControlFocus(ICharacter newFocus);
    }

    public interface IPlayerController : ICharacterController, IAccountController, ITimeout, IHandleOutput {
        bool Closing { get; }

        string IPAddress { get; }

        void CuePrompt();
    }

    public interface IFuturemudPlayerController : IPlayerController, IHaveFuturemud {
    }
}