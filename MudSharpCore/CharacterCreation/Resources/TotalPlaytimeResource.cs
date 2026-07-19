using ExpressionEngine;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.CharacterCreation.Resources;

internal class TotalPlaytimeResource : ChargenResourceBase
{
    public TimeSpan AwardInterval => MinimumTimeBetweenAwards;
    public double AwardAmount => MaximumNumberAwardedPerAward;

    public TotalPlaytimeResource(IFuturemud gameworld, ChargenResource resource) : base(gameworld, resource)
    {
    }

    public TotalPlaytimeResource(IFuturemud gameworld, string name, string plural, string alias) : base(gameworld, name, plural, alias)
    {
        MaximumNumberAwardedPerAward = 5;
        MinimumTimeBetweenAwards = TimeSpan.FromMinutes(15);
        MaximumResourceExpression = new Expression("1000");
        DoDatabaseInsert("Playtime");
    }

    public override void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes)
    {
        if (!character.IsPlayerCharacter || character.IsGuest)
        {
            return;
        }

        TimeSpan oldMinutesTS = TimeSpan.FromMinutes(oldMinutes);
        TimeSpan newMinutesTS = TimeSpan.FromMinutes(newMinutes);
        double intervalCount = newMinutesTS / MinimumTimeBetweenAwards - oldMinutesTS / MinimumTimeBetweenAwards;

        if (intervalCount > 0 &&
            character.Account.AccountResources[this] < GetMaximum(character.Account) &&
            ControlProg?.ExecuteBool(character) != false
            )
        {
            character.Account.AccountResources[this] += intervalCount;
            character.Account.AccountResourcesLastAwarded[this] = DateTime.UtcNow;
            Account dbaccount = FMDB.Context.Accounts.Find(character.Account.Id);
            if (dbaccount == null)
            {
                return;
            }

            AccountsChargenResources dbaccountresource =
                dbaccount.AccountsChargenResources.FirstOrDefault(
                    x => x.ChargenResourceId == Id);
            if (dbaccountresource == null)
            {
                dbaccountresource = new AccountsChargenResources();
                dbaccount.AccountsChargenResources.Add(dbaccountresource);
                dbaccountresource.Amount = 0;
                dbaccountresource.ChargenResourceId = Id;
            }

            dbaccountresource.Amount = character.Account.AccountResources[this];
            dbaccountresource.LastAwardDate = character.Account.AccountResourcesLastAwarded[this].Value;
        }
    }

    /// <inheritdoc />
    public override string TypeName => "Total Playtime";
}