using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Logging;

public class HealingLogger : ICustomLogger
{
    public string FileName { get; init; }
    private StringBuilder _pendingLogEntries = new();

    public HealingLogger()
    {
        _pendingLogEntries.AppendLine("id,name,severity,damage,original,current,pain,stun,result,roll,healing");
    }

    #region Implementation of ICustomLogger

    public void HandleLog(LogEntryType type, params object[] data)
    {
        switch (type)
        {
            case LogEntryType.HealingTick:
                HandleHealingTick(data);
                return;
        }
    }

    private void HandleHealingTick(object[] data)
    {
        ICharacter owner = (ICharacter)data[0];
        WoundSeverity severity = (WoundSeverity)data[1];
        double originalDamage = (double)data[2];
        double currentDamage = (double)data[3];
        double currentPain = (double)data[4];
        double currentStun = (double)data[5];
        DamageType damageType = (DamageType)data[6];
        WoundHealingTickResult tickResult = (WoundHealingTickResult)data[7];
        switch (tickResult)
        {
            case WoundHealingTickResult.NoHealBleeding:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal bleeding");
                break;
            case WoundHealingTickResult.NoHealInCombat:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal in combat");
                break;
            case WoundHealingTickResult.NoHealLodged:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal lodged");
                break;
            case WoundHealingTickResult.NoHealCantBreathe:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal can't breathe");
                break;
            case WoundHealingTickResult.NoHealInfected:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal infection");
                break;
            case WoundHealingTickResult.NoHealNecrotic:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal necrotic");
                break;
            case WoundHealingTickResult.NoHealNotSutured:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal not sutured");
                break;
            case WoundHealingTickResult.NoHealNotSuturedAutoClosed:
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},no heal not sutured (auto closed)");
                break;
            case WoundHealingTickResult.Healed:
                Outcome result = (Outcome)data[8];
                double healing = (double)data[9];
                _pendingLogEntries.AppendLine(
                    $"{owner.Id},\"{owner.PersonalName.GetName(NameStyle.FullName)}\",{severity.Describe()},{damageType.Describe()},{originalDamage:N3},{currentDamage:N3},{currentPain:N3},{currentStun:N3},healed,{result.Describe()},{healing:N3}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SaveLog()
    {
        if (_pendingLogEntries.Length <= 0)
        {
            return;
        }

        using (StreamWriter writer = new(new FileStream(FileName, FileMode.Append)))
        {
            writer.Write(_pendingLogEntries.ToString());
        }

        _pendingLogEntries = new StringBuilder();
    }

    #endregion
}
