using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.RPG.Law
{
	public static class CrimeExtensions
	{
		public static bool IsMajorCrime(this CrimeTypes type)
		{
			switch (type)
			{
				case CrimeTypes.Murder:
				case CrimeTypes.Manslaughter:
				case CrimeTypes.Treason:
				case CrimeTypes.EscapeCaptivity:
				case CrimeTypes.Sedition:
				case CrimeTypes.ContemptOfCourt:
					return true;
				default:
					return false;
			}
		}

		public static bool IsViolentCrime(this CrimeTypes type)
		{
			switch (type)
			{
				case CrimeTypes.Assault:
				case CrimeTypes.Battery:
				case CrimeTypes.AttemptedMurder:
				case CrimeTypes.Murder:
				case CrimeTypes.Manslaughter:
				case CrimeTypes.Torture:
				case CrimeTypes.GreviousBodilyHarm:
				case CrimeTypes.Intimidation:
				case CrimeTypes.ResistArrest:
					return true;
				default:
					return false;
			}
		}

		public static bool IsMoralCrime(this CrimeTypes type)
		{
			switch (type)
			{
				case CrimeTypes.Indecency:
				case CrimeTypes.Immorality:
				case CrimeTypes.PublicIntoxication:
				case CrimeTypes.Blasphemy:
				case CrimeTypes.Apostacy:
				case CrimeTypes.Profanity:
					return true;
				default:
					return false;
			}
		}

		public static string StandardDisableIllegalFlagText =>
			$"You must disable your protection against taking illegal actions by typing {"set lawful".MXPSend("set lawful")}.";

		/// <summary>
		/// This function will check if an action would be a crime, and if it is and their account settings are for lawful actions will warn them.
		///
		/// If not, and it is a crime, it will handle the crim-flagging
		/// </summary>
		/// <param name="criminal">The potential criminal</param>
		/// <param name="crime">The crime they may have committed</param>
		/// <param name="victim">The potential victim of their crime</param>
		/// <param name="target">An item that is a part of their crime</param>
		/// <param name="additionalInformation">Crime-specific additional information</param>
		/// <returns>True if the action was stopped because of lawful behaviour, false if it proceeded anyway</returns>
		public static bool HandleCrimesAndLawfulActing(ICharacter criminal, CrimeTypes crime, ICharacter victim = null,
			IGameItem target = null, string additionalInformation = "")
		{
			if (criminal.IsAdministrator())
			{
				return false;
			}

			if (CheckWouldBeACrime(crime, criminal, victim, target, additionalInformation))
			{
				if (criminal.Account.ActLawfully)
				{
					criminal.OutputHandler.Send(
						$"That action would be a crime.\n{StandardDisableIllegalFlagText}");
					return true;
				}

				CheckPossibleCrimeAllAuthorities(criminal, crime, victim, target, additionalInformation);
			}

			return false;
		}

		public static bool CheckWouldBeACrime(this CrimeTypes type, ICharacter actor, ICharacter victim = null,
			IGameItem target = null, string additionalInformation = "")
		{
			foreach (var authority in actor.Gameworld.LegalAuthorities)
			{
				if (authority.WouldBeACrime(actor, type, victim, target, additionalInformation))
				{
					return true;
				}
			}

			return false;
		}

		public static void CheckPossibleCrimeAllAuthorities(ICharacter criminal, CrimeTypes crime, ICharacter victim, IGameItem item,
			string additionalInformation)
		{
			foreach (var authority in criminal.Gameworld.LegalAuthorities)
			{
				authority.CheckPossibleCrime(criminal, crime, victim, item, additionalInformation);
			}
		}

		public static bool ShowMercyToIncapacitatedTarget(this EnforcementStrategy strategy)
		{
			switch (strategy)
			{
				case EnforcementStrategy.KillOnSight:
					return false;
				default:
					return true;
			}
		}

		public static bool IsArrestable(this EnforcementStrategy strategy)
		{
			switch (strategy)
			{
				case EnforcementStrategy.ArrestAndDetainedUnarmedOnly:
				case EnforcementStrategy.ArrestAndDetainIfArmed:
				case EnforcementStrategy.ArrestAndDetain:
				case EnforcementStrategy.ArrestAndDetainNoWarning:
				case EnforcementStrategy.LethalForceArrestAndDetain:
				case EnforcementStrategy.LethalForceArrestAndDetainNoWarning:
					return true;
				default:
					return false;
			}
		}

		public static bool IsKillable(this EnforcementStrategy strategy)
		{
			switch (strategy)
			{
				case EnforcementStrategy.KillOnSight:
				case EnforcementStrategy.LethalForceArrestAndDetain:
				case EnforcementStrategy.LethalForceArrestAndDetainNoWarning:
					return true;
				default:
					return false;
			}
		}
	}
}
