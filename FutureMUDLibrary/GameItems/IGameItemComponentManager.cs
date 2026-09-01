using MudSharp.Accounts;
using MudSharp.Framework;
using System;
using System.Collections.Generic;

namespace MudSharp.GameItems
{
    public interface IGameItemComponentManager
    {
        IEnumerable<string> PrimaryTypes { get; }
        IEnumerable<GameItemComponentTypeHelpInfo> TypeHelpInfo { get; }

		IEnumerable<GameItemComponentTypeHelpInfo> GetTypeHelpInfo(bool showModern, bool showFuturistic);

        void AddBuilderLoader(string name, bool primary,
            Func<IFuturemud, IAccount, IGameItemComponentProto> initialiser);

        void AddDatabaseLoader(string name,
            Func<MudSharp.Models.GameItemComponentProto, IFuturemud, IGameItemComponentProto> initialiser);

        void AddTypeHelpInfo(string name, string blurb, string help);
		void AddModernTypeHelpInfo(string name, string blurb, string help);
		void AddFuturisticTypeHelpInfo(string name, string blurb, string help);

        IGameItemComponentProto GetProto(string name, IFuturemud gameworld, IAccount account);
        IGameItemComponentProto GetProto(MudSharp.Models.GameItemComponentProto dbproto, IFuturemud gameworld);
    }
}
