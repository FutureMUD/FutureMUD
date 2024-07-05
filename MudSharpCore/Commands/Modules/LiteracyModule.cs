using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using Org.BouncyCastle.Asn1.X509;
using Drawing = MudSharp.Communication.Drawing;

namespace MudSharp.Commands.Modules;

public class LiteracyModule : Module<ICharacter>
{
	private LiteracyModule()
		: base("Literacy")
	{
		IsNecessary = true;
	}

	public static LiteracyModule Instance { get; } = new();

	[PlayerCommand("Scripts", "scripts")]
	[HelpInfo("Scripts", @"The #3scripts#0 command is used to show all of the scripts (systems of writing) that you know, and which one you are currently using when you write.

The syntax is simply #3scripts#0.", AutoHelp.HelpArg)]
	protected static void Scripts(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate. You don't know anything about fancy squiggly lines.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("You know the following scripts:");
		foreach (var script in actor.Scripts)
		{
			sb.AppendLine(
				$"\t{script.Name.Colour(Telnet.Cyan)}: {script.KnownScriptDescription}{(script == actor.CurrentScript ? $" ({"Current".Colour(Telnet.Green)})" : "")}");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Read", "read")]
	[HelpInfo("Read", @"The #3read#0 command is used to read written text, like a book or a sheet of paper. 

You can use the syntax #3read <thing>#0 to see a list of writings and drawings on the thing. You can then use #3read <thing> <##>#0 to read a specific piece of text on the thing.

For graffiti, see the #3look#0 command instead.", AutoHelp.HelpArgOrNoArg)]
	protected static void Read(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to read?");
			return;
		}

		var target = actor.TargetItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that to read.");
			return;
		}

		var targetAsReadable = target.GetItemType<IReadable>();
		if (targetAsReadable == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be read.");
			return;
		}

		var targetAsOpenable = target.GetItemType<IOpenable>();
		if (targetAsOpenable != null && !targetAsOpenable.IsOpen)
		{
			actor.Send($"{target.HowSeen(actor, true)} must first be opened before it can be read.");
			return;
		}

		var sb = new StringBuilder();
		if (ss.IsFinished)
		{
			var textNumber = 1;
			foreach (var item in targetAsReadable.Readables)
			{
				if (item is IWriting writingItem)
				{
					sb.AppendLine($"Writing #{textNumber++:N0}: {actor.GetWritingHeader(writingItem)}"
						.ColourIncludingReset(Telnet.Yellow));
					if (actor.CanRead(writingItem))
					{
						sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
						sb.AppendLine(writingItem.ParseFor(actor));
					}
					else
					{
						sb.AppendLine($"***{actor.WhyCannotRead(writingItem)}***".Colour(Telnet.Red));
					}
				}
				else if (item is IDrawing drawingItem)
				{
					sb.AppendLine(
						$"Drawing #{(textNumber++).ToString("N0", actor)}: {drawingItem.ShortDescription} ({drawingItem.DrawingSize.DescribeEnum()})"
							.ColourIncludingReset(Telnet.Yellow));
					sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
					sb.AppendLine(drawingItem.ParseFor(actor));
				}

				sb.AppendLine();
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (!int.TryParse(ss.Pop(), out var value))
		{
			actor.Send(
				"If you specify an additional argument for read, it must be the number of the writing or drawing on that item that you wish to view.");
			return;
		}

		var readable = targetAsReadable.Readables.ElementAtOrDefault(value - 1);
		if (readable == null)
		{
			actor.Send(
				$"There are only {targetAsReadable.Readables.Count():N0} writings or drawings available to read on that item.");
			return;
		}

		if (readable is IWriting writing)
		{
			sb.AppendLine($"Writing #{value:N0}: {actor.GetWritingHeader(writing)}");
			if (actor.CanRead(writing))
			{
				sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
				sb.AppendLine(readable.ParseFor(actor));
			}
			else
			{
				sb.AppendLine($"***{actor.WhyCannotRead(writing)}***".Colour(Telnet.Red));
			}
		}
		else if (readable is IDrawing drawing)
		{
			sb.AppendLine(
				$"Drawing #{value.ToString("N0", actor)}: {drawing.ShortDescription} ({drawing.DrawingSize.DescribeEnum()})");
			sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
			sb.AppendLine(drawing.ParseFor(actor));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Write", "write")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("Write", @"This command is used to write on things designed to have writing on them, like books or sheets of paper.

You will use the language, script and handwriting style you have set with the #3script#0 command and #3set writing#0 command. You must have a writing implement to write something.

The syntax is as follows:

	#3write <thing>#0 - writes on a book or paper or similar", AutoHelp.HelpArgOrNoArg)]
	protected static void Write(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate. You won't be writing anything.");
			return;
		}

		if (actor.CurrentScript == null)
		{
			actor.Send("You must first decide what script you will use when writing, before you can write.");
			actor.Send(
				$"See {"help script".FluentTagMXP("send", "href='help script'")} for more information".Colour(
					Telnet.Yellow));
			return;
		}

		if (actor.CurrentWritingLanguage == null)
		{
			actor.Send("You must first decide what language you will use when writing, before you can write.");
			actor.Send(
				$"See {"help script".FluentTagMXP("send", "href='help script'")} for more information".Colour(
					Telnet.Yellow));
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What do you want to write on?");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anything like that to write on.");
			return;
		}

		var targetAsWritable = target.GetItemType<IWriteable>();
		if (targetAsWritable == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be written on.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var implement = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<IWritingImplement>())
		                     .FirstOrDefault();
		if (implement == null)
		{
			actor.Send(
				"You must be holding a writing implement of some description that matches the thing you want to write on.");
			return;
		}

		if (!targetAsWritable.CanWrite(actor, implement, null))
		{
			actor.Send(targetAsWritable.WhyCannotWrite(actor, implement, null));
			return;
		}

		actor.EditorMode((text, handler, pars) =>
			{
				actor.RemoveAllEffects(x => x.IsEffectType<StoredEditorText>());
				actor.AddEffect(new StoredEditorText(actor, text), TimeSpan.FromMinutes(30));

				// First, check they still have the writing implement and can still see the writeable
				if (!actor.Body.HeldOrWieldedItems.Contains(implement.Parent))
				{
					handler.Send(
						"You seem to have lost your writing implement while you were pondering what to write.");
					return;
				}

				if ((!actor.CanSee(target) && target.TrueLocations.All(x => x.Location != actor.Location)) ||
				    target.Destroyed)
				{
					handler.Send("The thing you were writing on is no longer there.");
					return;
				}

				var writing = new SimpleWriting(actor.Gameworld, actor, text)
				{
					WritingColour = implement.WritingImplementColour, ImplementType = implement.WritingImplementType
				};
				if (!targetAsWritable.CanWrite(actor, implement, writing))
				{
					handler.Send(targetAsWritable.WhyCannotWrite(actor, implement, writing));
					actor.Gameworld.SaveManager.Abort(writing);
					return;
				}

				targetAsWritable.Write(actor, implement, writing);
				handler.Handle(new EmoteOutput(new Emote("@ write|writes on $0 with $1.", actor, target,
					implement.Parent)));
			}, (handler, pars) => { handler.Send("You decide not to write anything."); },
			actor.CurrentScript.DocumentLengthModifier);
	}

	[PlayerCommand("Graffiti", "graffiti")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("graffiti", @"The graffiti command is used to draw or write things or locations and create images and text that others can then read. Graffiti may be a crime that is automatically enforced if you're putting graffiti in a public place or on items in a public place.

You must have a writing implement to make graffiti, and it will use your current writing language and script for any language in the text.

The syntax is as follows:

	#3graffiti here ""<short description>"" [<specific location description>]#0 - create graffiti in the room
	#3graffiti <thing> <short description>#0 - create graffiti on an item

Both of these will drop you into an editor to describe your graffiti and any text on it.", AutoHelp.HelpArgOrNoArg)]
	protected static void Graffiti(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What do you want to draw graffiti on?");
			return;
		}

		if (actor.CurrentWritingLanguage is null)
		{
			actor.OutputHandler.Send("You must have set a current writing language in order to draw graffiti.");
			return;
		}

		if (actor.CurrentScript is null)
		{
			actor.OutputHandler.Send("You must have set a current script in order to order draw graffiti.");
			return;
		}

		var targetText = ss.PopSpeech();
		if (targetText.EqualTo("here"))
		{
			GraffitiRoom(actor, ss);
			return;
		}

		var target = actor.TargetItem(targetText);
		if (target is null)
		{
			actor.OutputHandler.Send("You don't see anything like that here.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a short description for your graffiti, in a format like #E\"an image of a black lotus\"#0".SubstituteANSIColour());
			return;
		}

		var sdesc = ss.SafeRemainingArgument;
		if (sdesc.Length > 80)
		{
			actor.OutputHandler.Send("The maximum length for short descriptions is 80 characters.");
			return;
		}

		var implement = actor.Body.HeldOrWieldedItems
		                     .SelectNotNull(x => x.GetItemType<IWritingImplement>())
		                     .FirstOrDefault();
		if (implement is null)
		{
			actor.Send("You must be holding a writing implement in order to make some graffiti.");
			return;
		}

		if (!implement.Primed)
		{
			actor.OutputHandler.Send($"You must prime {implement.Parent.HowSeen(actor)} before you can use it.");
			return;
		}

		actor.OutputHandler.Send($@"Please enter the description of the graffiti that you are creating.
You are drawing on {target.HowSeen(actor)} and you have given a graffiti short description of {sdesc.Colour(Telnet.BoldCyan)}.

Anything you enter in between double quotes (e.g. "") will be interpreted as writing within the drawing and parsed appropriately.

You are writing with these settings:

Language: {actor.CurrentWritingLanguage.Name.ColourValue()}
Script: {actor.CurrentScript.Name.ColourValue()}
Style: {actor.WritingStyle.Describe().ColourValue()}
");
		actor.EditorMode((text, handler, pars) =>
		{
			actor.RemoveAllEffects(x => x.IsEffectType<StoredEditorText>());
			actor.AddEffect(new StoredEditorText(actor, text), TimeSpan.FromMinutes(30));
			// First, check they still have the writing implement and can still see the writeable
			if (!actor.Body.HeldOrWieldedItems.Contains(implement.Parent))
			{
				handler.Send("You seem to have lost your writing implement while you were pondering what to graffiti.\n#3Note: the text you wrote can be recalled in the editor for the next 30 minutes with the *recall command#0".SubstituteANSIColour());
				return;
			}

			if ((!actor.CanSee(target) && target.TrueLocations.All(x => x.Location != actor.Location)) ||
			    target.Destroyed)
			{
				handler.Send("The thing you were drawing on is no longer there.\n#3Note: the text you wrote can be recalled in the editor for the next 30 minutes with the *recall command#0".SubstituteANSIColour());
				return;
			}

			var graffiti = new CompositeWriting(actor.Gameworld, actor, implement, text, sdesc);
			actor.Gameworld.Add(graffiti);
			target.AddEffect(new GraffitiEffect(actor, graffiti, RoomLayer.GroundLevel, string.Empty));
			handler.Handle(new EmoteOutput(new Emote("@ draw|draws graffiti on $0 with $1.", actor, target, implement.Parent)));
			handler.Send(new EmoteOutput(new Emote("You draw graffiti on $0 with $1.", actor, target, implement.Parent)));
		}, (handler, pars) => { handler.Send("You decide not to graffiti anything."); }, 1.0);
	}

	private static void GraffitiRoom(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a short description for your graffiti, in a format like #E\"an image of a black lotus\"#0".SubstituteANSIColour());
			return;
		}

		var sdesc = ss.PopSpeech();
		if (sdesc.Length > 80)
		{
			actor.OutputHandler.Send("The maximum length for short descriptions is 80 characters.");
			return;
		}

		var localDescription = string.Empty;
		if (!ss.IsFinished)
		{
			localDescription = ss.SafeRemainingArgument;
		}

		var implement = actor.Body.HeldOrWieldedItems
							 .SelectNotNull(x => x.GetItemType<IWritingImplement>())
							 .FirstOrDefault();
		if (implement is null)
		{
			actor.Send("You must be holding a writing implement in order to make some graffiti.");
			return;
		}

		if (!implement.Primed)
		{
			actor.OutputHandler.Send($"You must prime {implement.Parent.HowSeen(actor)} before you can use it.");
			return;
		}
		actor.OutputHandler.Send($@"Please enter the description of the graffiti that you are creating.
You are drawing on the location you are at and you have given a graffiti short description of {sdesc.Colour(Telnet.BoldCyan)}.

Anything you enter in between double quotes (e.g. "") will be interpreted as writing within the drawing and parsed appropriately.

You are writing with these settings:

Language: {actor.CurrentWritingLanguage.Name.ColourValue()}
Script: {actor.CurrentScript.Name.ColourValue()}
Style: {actor.WritingStyle.Describe().ColourValue()}
");

		actor.EditorMode((text, handler, pars) =>
		{
			actor.RemoveAllEffects(x => x.IsEffectType<StoredEditorText>());
			actor.AddEffect(new StoredEditorText(actor, text), TimeSpan.FromMinutes(30));
			// First, check they still have the writing implement and can still see the writeable
			if (!actor.Body.HeldOrWieldedItems.Contains(implement.Parent))
			{
				handler.Send("You seem to have lost your writing implement while you were pondering what to graffiti.\n#3Note: the text you wrote can be recalled in the editor for the next 30 minutes with the *recall command#0".SubstituteANSIColour());
				return;
			}

			var graffiti = new CompositeWriting(actor.Gameworld, actor, implement, text, sdesc);
			actor.Gameworld.Add(graffiti);
			actor.Location.AddEffect(new GraffitiEffect(actor, graffiti, actor.RoomLayer, localDescription));
			handler.Handle(new EmoteOutput(new Emote($"@ draw|draws graffiti here{(string.IsNullOrEmpty(localDescription) ? "" : $" {localDescription}")} with $1.", actor, actor, implement.Parent)));
			handler.Send(new EmoteOutput(new Emote($"You draw graffiti here{(string.IsNullOrEmpty(localDescription) ? "" : $" {localDescription}")} with $1.", actor, actor, implement.Parent)));
		}, (handler, pars) => { handler.Send("You decide not to graffiti anything."); }, 1.0);
	}

	[PlayerCommand("DrawPicture", "drawpicture")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("DrawPicture", @"The #3DrawPicture#0 command is used to create a drawing on a writing object. This is essentially just a description of something that your character is drawing. Other players will be able to view the picture and also will see how good your drawing skill is.

You must have a writing implement and something to write on in order to draw a picture.

The syntax for this command is as follows:

	#3drawpicture <writeable> <size> ""<sdesc>""#0 - drops you into an editor to describe your drawing

To draw on locations or things that aren't normally meant for writing and drawing, see the #3graffiti#0 command.", AutoHelp.HelpArgOrNoArg)]
	protected static void DrawPicture(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What do you want to draw on?");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anything like that to draw on.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var targetAsWritable = target.GetItemType<IWriteable>();
		if (targetAsWritable == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be drawn on.");
			return;
		}

		var implement = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<IWritingImplement>())
		                     .FirstOrDefault();
		if (implement == null)
		{
			actor.Send(
				"You must be holding a writing implement of some description that matches the thing you want to draw on.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a size of the drawing that you want to draw. The valid sizes and their equivalent amount of document length consumption are: {Enum.GetValues(typeof(DrawingSize)).OfType<DrawingSize>().Select(x => $"{x.DescribeEnum().ColourName()} ({x.DocumentLength().ToString("N0", actor).ColourValue()})").ListToString()}.");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum<DrawingSize>(out var size))
		{
			actor.OutputHandler.Send(
				$"That is not a valid drawing size. The valid sizes and their equivalent amount of document length consumption are: {Enum.GetValues(typeof(DrawingSize)).OfType<DrawingSize>().Select(x => $"{x.DescribeEnum().ColourName()} ({x.DocumentLength().ToString("N0", actor).ColourValue()})").ListToString()}.");
			return;
		}

		var dummy = new DummyDrawing
			{ DrawingSize = size, ImplementType = implement.WritingImplementType, Author = actor };

		if (!targetAsWritable.CanDraw(actor, implement, dummy))
		{
			actor.Send(targetAsWritable.WhyCannotDraw(actor, implement, dummy));
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a short description for your picture, in a format like #E\"a picture of a beautiful flower\"#0".SubstituteANSIColour());
			return;
		}

		var sdesc = ss.PopSpeech();
		if (sdesc.Length > 80)
		{
			actor.OutputHandler.Send("The maximum length for short descriptions is 80 characters.");
			return;
		}

		actor.EditorMode((text, handler, pars) =>
		{
			actor.RemoveAllEffects(x => x.IsEffectType<StoredEditorText>());
			actor.AddEffect(new StoredEditorText(actor, text), TimeSpan.FromMinutes(30));
			// First, check they still have the writing implement and can still see the writeable
			if (!actor.Body.HeldOrWieldedItems.Contains(implement.Parent))
			{
				handler.Send("You seem to have lost your writing implement while you were pondering what to draw.");
				return;
			}

			if ((!actor.CanSee(target) && target.TrueLocations.All(x => x.Location != actor.Location)) ||
			    target.Destroyed)
			{
				handler.Send("The thing you were drawing on is no longer there.");
				return;
			}

			var drawing = new Drawing(actor,
				actor.TraitValue(actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("DrawingTraitId"))), sdesc,
				text, implement.WritingImplementType, size);
			if (!targetAsWritable.CanDraw(actor, implement, drawing))
			{
				handler.Send(targetAsWritable.WhyCannotDraw(actor, implement, drawing));
				actor.Gameworld.SaveManager.Abort(drawing);
				return;
			}

			targetAsWritable.Draw(actor, implement, drawing);
			handler.Handle(new EmoteOutput(new Emote("@ draw|draws on $0 with $1.", actor, target, implement.Parent)));
		}, (handler, pars) => { handler.Send("You decide not to draw anything."); }, 1.0);
	}

	[PlayerCommand("Title", "title")]
	[HelpInfo("Title", @"The #3title#0 command is used to set the title of a book or other writing object. Titles are visible in many contexts like short descriptions, and help differentiate different written items.

Texts can have no title, but once a title has been given they can only be changed, not revoked.

The syntax is #3title <item> <title>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void Title(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate, you don't know anything about titling works.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send($"The syntax for this command is {"title <item> <title>".Colour(Telnet.Yellow)}");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anything like that to title.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var targetAsWritable = target.GetItemType<IWriteable>();
		if (targetAsWritable == null)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is not something that can be given titles.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What title do you want to give to {target.HowSeen(actor)}?");
			return;
		}

		if (!targetAsWritable.CanGiveTitle(actor, ss.SafeRemainingArgument))
		{
			actor.Send(targetAsWritable.WhyCannotGiveTitle(actor, ss.SafeRemainingArgument));
			return;
		}

		var oldTitle = targetAsWritable.Title ?? string.Empty;
		targetAsWritable.GiveTitle(actor, ss.SafeRemainingArgument);
		if (oldTitle.Equals(string.Empty))
		{
			actor.OutputHandler.Send($"You give the title of #F\"{targetAsWritable.Title}\"#0 to {target.HowSeen(actor)}".SubstituteANSIColour());
		}
		else
		{
			actor.OutputHandler.Send(
				$"You change the title from #F\"{oldTitle}\"#0 to #F\"{targetAsWritable.Title}\"#0".SubstituteANSIColour());
		}
	}
}