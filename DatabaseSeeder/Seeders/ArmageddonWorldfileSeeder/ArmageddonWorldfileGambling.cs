#nullable enable

using System;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallGamblingSpecials()
	{
		var games = new[] { "jindo_dice", "gypsy_game_two", "gypsy_game_three" };
		if (!_source.Npcs.Any(x => x.Programs.Any(p => games.Any(g => p == "C bet " + g)))) return;
		// Keep outcome rules in executable progs, so the seeded rules themselves can be verified exhaustively.
		var dice = Prog("ArmageddonWorldfileDiceOutcome", """
if (@point == 0)
	if (@roll == 7 or @roll == 11)
		return 1
	end if
	return 0
end if
if (@roll == 7 or @roll == 11)
	return -1
end if
if (@roll == @point)
	return 1
end if
return 0
""", ProgVariableTypes.Number, ("point", ProgVariableTypes.Number), ("roll", ProgVariableTypes.Number));
		var pebbles = Prog("ArmageddonWorldfilePebbleOutcome", """
if (@player > @dealer)
	return 1
end if
if (@player < @dealer)
	return -1
end if
return 0
""", ProgVariableTypes.Number, ("player", ProgVariableTypes.Number), ("dealer", ProgVariableTypes.Number));
		var wheel = Prog("ArmageddonWorldfileWheelOutcome", """
if (@cardimage == @wheelimage)
	if (@cardcolour == @wheelcolour)
		return 3
	end if
	return 2
end if
if (@cardcolour == @wheelcolour)
	return 0
end if
return -1
""", ProgVariableTypes.Number, ("cardcolour", ProgVariableTypes.Number), ("cardimage", ProgVariableTypes.Number),
			("wheelcolour", ProgVariableTypes.Number), ("wheelimage", ProgVariableTypes.Number));
		foreach (var game in games)
		{
			var sources = _source.Npcs.Where(x => x.Programs.Contains("C bet " + game)).Select(x => x.Vnum).ToArray();
			if (sources.Length == 0) continue;
			var isWheel = game == "gypsy_game_three";
			var intro = game switch
			{
				"gypsy_game_two" => "force @dealer \"say This game is one of skill. You and I will each throw ten pebbles at the plate. Whichever of us lands the most pebbles wins.\"\n",
				"gypsy_game_three" => "force @dealer \"say I will deal you a card, which has a colored image on it. If the card I deal you matches the colored image which the wheel behind me is spun to, then you win.\"\n",
				_ => ""
			};
			var play = game switch
			{
				"jindo_dice" => $"""
var firstfaces = splittext("Peasant|Soldier|Noble|Templar|King|Dragon", "|")
var secondfaces = splittext("Bug|Beetle|Kank|Erdlu|Inix|Mekillot", "|")
var first = random(1, 6)
var second = random(1, 6)
sendloc(@actor.location, "$0 roll|rolls the " + getindex(@firstfaces, @first - 1) + "'s " + getindex(@secondfaces, @second - 1) + ".", @actor)
var point = @first + @second
var outcome = @{dice.FunctionName}(0, @point)
while (@outcome == 0)
	first = random(1, 6)
	second = random(1, 6)
	sendloc(@actor.location, "$0 roll|rolls the " + getindex(@firstfaces, @first - 1) + "'s " + getindex(@secondfaces, @second - 1) + ".", @actor)
	outcome = @{dice.FunctionName}(@point, @first + @second)
end while
""",
				"gypsy_game_two" => $"""
var player = random(1, 10)
var house = random(3, 10)
sendloc(@actor.location, "$0 toss|tosses a handful of pebbles at the plate, causing " + @player + " to land on it.", @actor)
sendloc(@actor.location, "$0 toss|tosses a handful of pebbles at the plate, causing " + @house + " to land on it.", @dealer)
var outcome = @{pebbles.FunctionName}(@player, @house)
""",
				_ => $"""
var colours = splittext("Gold|Black", "|")
var images = splittext("Cliff|Coin|Drum|Dagger|Mask|Hand|Foot|Gwoshi|Sword|Hammer|Tree|Lute|Shovel|Tregil|Bow|Tamborine|Eye|Arrow|Turaal|Quirri|Cliffshrew|Dancer|Rat|Jug|Horta|Wagon|Ring|Dune|Belshun|Moon", "|")
var cardcolour = random(1, 2)
var cardimage = random(1, 30)
var wheelcolour = random(1, 2)
var wheelimage = random(1, 30)
send(@actor, "You get dealt the " + getindex(@colours, @cardcolour - 1) + " " + getindex(@images, @cardimage - 1) + ".")
sendloc(@actor.location, "$0 spin|spins the wheel, it eventually stops on the " + getindex(@colours, @wheelcolour - 1) + " " + getindex(@images, @wheelimage - 1) + ".", @dealer)
var outcome = @{wheel.FunctionName}(@cardcolour, @cardimage, @wheelcolour, @wheelimage)
"""
			};
			var prog = NpcServiceProg("ArmageddonWorldfileGame" + game, $"""
var dealer = tocharacter(@npc)
if (not(@actor.isawake))
	return false
end if
if (@args.Count(word, true) == 0)
	{intro}force @dealer "say How much do you wish to bet?"
	return true
end if
var bet = tonumber(getindex(@args, 0))
if (@bet < 20)
	force @dealer "say You must bet at least 20 coins."
	return true
end if
if (@bet >= 501)
	force @dealer "say The maximum bet I will cover is 500."
	return true
end if
bet = floor(@bet)
var refusal = ""
var currency = tocurrency({_currency.Id})
if (countaccessiblecurrency(@dealer, @currency) < @bet * {(isWheel ? 5 : 1)})
	refusal = "say I cannot cover that bet."
end if
if (countaccessiblecurrency(@actor, @currency) < @bet)
	refusal = "say You cannot cover that bet."
end if
if (@refusal != "")
	force @dealer @refusal
	return true
end if
if (not(cantransfercurrency(@currency, @bet, @actor, @dealer)) or not(cantransfercurrency(@currency, @bet * {(isWheel ? 3 : 1)}, @dealer, @actor)))
	force @dealer "say We cannot exchange the coins for that wager just now."
	return true
end if
{play}
if (@outcome > 0)
	if (not(transfercurrency(@currency, @bet * @outcome, @dealer, @actor)))
		force @dealer "say We could not settle that wager. No payment was made."
		return true
	end if
	{(isWheel ? "if (@outcome == 3)\nforce @dealer \"say You have matched both color and image!\"\nelse\nforce @dealer \"say You have matched the images!\"\nend if" : "force @dealer \"say You have won!\"")}
	sendloc(@actor.location, "$0 hand|hands $1 some winnings.", @dealer, @actor)
else
	if (@outcome < 0)
		if (not(transfercurrency(@currency, @bet, @actor, @dealer)))
			force @dealer "say We could not settle that wager. No payment was made."
			return true
		end if
		force @dealer "say {(isWheel ? "You matched neither color nor image..." : "You have lost...")}"
		sendloc(@actor.location, "$0 hand|hands $1 some coins.", @actor, @dealer)
	else
		force @dealer "say {(isWheel ? "You have matched the color, you break even." : "It is a tie...")}"
	end if
end if
return true
""");
			InstallNpcInputHook("Gambling " + game, "bet", prog, sources);
			Record("npc-game." + game, nameof(FutureProg), prog.Id, null, new
			{ Sources = sources, Minimum = 20, Maximum = 500, BankrollMultiple = isWheel ? 5 : 1,
				Disposition = "Approved source random rules and net payouts, settled from actual held cash without minted replacements or upfront escrow. Native exact denomination, custody, capacity and proximity preflight applies before rolling; settlement revalidates. Conditional input progs handle only source bet associations. Monetary shop functionality is unchanged." });
		}
		Flush();
	}
}
