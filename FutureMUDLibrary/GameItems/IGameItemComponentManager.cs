using System;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Framework;

namespace MudSharp.GameItems
{
    public interface IGameItemComponentManager
    {
        IEnumerable<string> PrimaryTypes { get; }
        IEnumerable<(string Name, string Blurb, string Help)> TypeHelpInfo {get;}

        void AddBuilderLoader(string name, bool primary,
            Func<IFuturemud, IAccount, IGameItemComponentProto> initialiser);

        void AddDatabaseLoader(string name,
            Func<MudSharp.Models.GameItemComponentProto, IFuturemud, IGameItemComponentProto> initialiser);

        void AddTypeHelpInfo(string name, string blurb, string help);

        IGameItemComponentProto GetProto(string name, IFuturemud gameworld, IAccount account);
        IGameItemComponentProto GetProto(MudSharp.Models.GameItemComponentProto dbproto, IFuturemud gameworld);
    }
}