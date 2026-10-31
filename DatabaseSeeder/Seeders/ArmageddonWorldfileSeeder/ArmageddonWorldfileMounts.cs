#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal const string MountOwnerRegister = "arm_mount_owner_identity";
	internal const string MountApprovedRidersRegister = "arm_mount_approved_riders";
	internal const string MountTrainedRegister = "arm_mount_trained";
	private static readonly HashSet<int> ArmageddonMountableRaces = [10, 13, 28, 29, 38, 44, 45, 60, 62, 71, 106, 113, 129, 138];
	private const int MountFlag = 1 << 26;
	private const int AggressiveFlag = 1 << 5;

	private const string MountPermitProg = """
		if (getregister(@mount, "arm_mount_trained") != 1)
			return false
		end if
		var owner = getregister(@mount, "arm_mount_owner_identity")
		if (@owner > 0 and @owner != @rider.identityid and not(contains(@mount.riders, @rider)))
			var approved = getregister(@mount, "arm_mount_approved_riders")
			if (not(contains(@approved, @rider.identityid)))
				return false
			end if
		end if
		if (@mount.riders.Count(occupant, true) > 0 and not(contains(@mount.riders, @rider)))
			return false
		end if
		if (not(@ArmageddonWorldfileMountHandlerMatch(@mount, @rider)))
			return false
		end if
		return true
		""";

	private void EnsureMountRegisters()
	{
		EnsureMountRegister(ProgVariableTypes.Character, MountOwnerRegister, ProgVariableTypes.Number, "<var>0</var>");
		EnsureMountRegister(ProgVariableTypes.Character, MountApprovedRidersRegister,
			ProgVariableTypes.Number | ProgVariableTypes.Collection, "<var></var>");
		EnsureMountRegister(ProgVariableTypes.Character, MountTrainedRegister, ProgVariableTypes.Number, "<var>0</var>");
	}

	private void EnsureMountRegister(ProgVariableTypes ownerType, string name, ProgVariableTypes valueType, string defaultValue)
	{
		var owner = ownerType.ToStorageString();
		var localDefinition = _db.VariableDefinitions.Local.SingleOrDefault(x => x.OwnerTypeDefinition == owner && x.Property == name);
		var storedDefinition = _db.VariableDefinitions.Where(x => x.OwnerTypeDefinition == owner && x.Property == name).FirstOrDefault();
		if (localDefinition is not null && storedDefinition is not null && !ReferenceEquals(localDefinition, storedDefinition) ||
			(localDefinition ?? storedDefinition) is { } existing && existing.ContainedTypeDefinition != valueType.ToStorageString())
			throw new InvalidOperationException($"Mount register {name} has an incompatible or ambiguous definition.");
		if (localDefinition is null && storedDefinition is null)
			_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = name, ContainedTypeDefinition = valueType.ToStorageString() });
		var localDefault = _db.VariableDefaults.Local.SingleOrDefault(x => x.OwnerTypeDefinition == owner && x.Property == name);
		var storedDefault = _db.VariableDefaults.Where(x => x.OwnerTypeDefinition == owner && x.Property == name).FirstOrDefault();
		if (localDefault is not null && storedDefault is not null && !ReferenceEquals(localDefault, storedDefault))
			throw new InvalidOperationException($"Mount register {name} has ambiguous defaults.");
		if (localDefault is null && storedDefault is null)
			_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = name, DefaultValue = defaultValue });
	}

	private void InstallArmageddonMountPolicies(ArtificialIntelligence mountAi, long mountAiId)
	{
		EnsureMountRegisters();
		var rangerRole = OwnedBehaviourId("arm.class.ranger", nameof(ChargenRole))
			?? throw new InvalidOperationException("Armageddon mount taming requires the installed Ranger role.");
		var riding = _db.TraitDefinitions.SingleOrDefault(x => x.Name.Equals("Riding", StringComparison.OrdinalIgnoreCase) && x.Type == 0)
			?? throw new InvalidOperationException("Armageddon mount taming requires the installed Riding skill.");
		var standing = EngineCompiler::MudSharp.Body.Position.PositionStates.PositionStanding.Instance.Id;
		Prog("ArmageddonWorldfileMountCurrentHandler", """
			var current = @mount
			var remaining = @mount.location.characters.Count + 1
			while (@remaining > 0)
				if (@current.location != @mount.location or @current.layer != @mount.layer)
					return null("character")
				end if
				if (@current.riders.any)
					return @current.riders.first
				end if
				if (not(@current.npc))
					return @current
				end if
				if (isnull(@current.following))
					return null("character")
				end if
				current = @current.following
				remaining = @remaining - 1
			end while
			return @mount
			""", ProgVariableTypes.Character, ("mount", ProgVariableTypes.Character));
		var handlerMatch = Prog("ArmageddonWorldfileMountHandlerMatch", """
			var handler = @ArmageddonWorldfileMountCurrentHandler(@mount)
			return isnull(@handler) or @handler == @rider
			""", ProgVariableTypes.Boolean, ("mount", ProgVariableTypes.Character), ("rider", ProgVariableTypes.Character));
		var followerCount = Prog("ArmageddonWorldfileMountLeadCount", """
			var count = 0
			foreach (candidate in @handler.location.characters)
				if (@candidate.layer == @handler.layer and not(isnull(@candidate.following)) and getregister(@candidate, "arm_mount_trained") == 1 and @ArmageddonWorldfileMountCurrentHandler(@candidate) == @handler)
					count = @count + 1
				end if
			end foreach
			return @count
			""", ProgVariableTypes.Number, ("handler", ProgVariableTypes.Character));
		var sourceMounts = _source.Npcs.Where(x => (unchecked((uint)x.Flags[0]) & MountFlag) != 0).ToArray();
		foreach (var source in sourceMounts)
		{
			var templateIds = _npcFamilyIds.GetValueOrDefault(source.Vnum, [_npcs[source.Vnum].Id]);
			var definition = XElement.Parse(_npcs[source.Vnum].Definition);
			var onLoadId = (long)definition.Element("OnLoadProg")!;
			var onLoad = _progs.Single(x => x.Id == onLoadId);
			var line = string.Concat(templateIds.Select(templateId =>
				$"if (@npc.npctemplateid == {templateId})\nsetregister @npc \"{MountTrainedRegister}\" 1\nend if\n"));
			var position = onLoad.FunctionText.LastIndexOf("return true", StringComparison.OrdinalIgnoreCase);
			if (position < 0) throw new InvalidOperationException($"Mount NPC {source.Vnum} has an on-load prog without a successful return.");
			onLoad.FunctionText = onLoad.FunctionText.Insert(position, line);
		}

		var eligible = _source.Npcs.Where(x => ArmageddonMountableRaces.Contains(x.Identity[6]) &&
			(unchecked((uint)x.Flags[0]) & (MountFlag | AggressiveFlag)) == 0).ToArray();
		var taming = Prog("ArmageddonWorldfileTameMount", $$"""
			if (@args.Count(word, true) != 1)
				return false
			end if
			var animal = targetcharacter(@actor, getindex(@args, 0))
			if (isnull(@animal) or @animal != tocharacter(@npc))
				return false
			end if
			if (not(@actor.canmove) or not(isnull(@actor.mount)))
				return false
			end if
			if (getregister(@animal, "{{MountTrainedRegister}}") == 1 and not(@{{handlerMatch.FunctionName}}(@animal, @actor)) and not(contains(@animal.riders, @actor)))
				send(@actor, "Someone else is already handling that animal.")
				return true
			end if
			if (positionid(@actor) != {{standing}} or not({{(rangerRole is { } role ? $"contains(@actor.roles, torole({role}))" : "false")}}))
				return false
			end if
			var skill = totrait({{riding.Id}})
			if (gettrait(@actor, @skill) <= 60)
				return false
			end if
			if (getregister(@animal, "{{MountTrainedRegister}}") == 1)
				var owner = getregister(@animal, "{{MountOwnerRegister}}")
				var approved = getregister(@animal, "{{MountApprovedRidersRegister}}")
				if (@owner == 0 or @owner == @actor.identityid or contains(@approved, @actor.identityid))
					return false
				end if
				if (not(@animal.canmove) or not(@animal.isawake))
					return false
				end if
				if (check(@actor, @skill, 5) > 0)
					additem approved @actor.identityid
					setregister @animal "{{MountApprovedRidersRegister}}" @approved
					sendloc(@actor.location, "$0 earns $1's trust as a capable handler.", @actor, @animal)
					return false
				end if
				send(@actor, "The animal refuses to accept your handling.")
				return true
			end if
			if (not(@animal.canmove) or not(@animal.isawake))
				return false
			end if
			if (check(@actor, @skill, 5) <= 0)
				var attackcommand = "kill " + bestkeyword(@animal, @actor)
				force @animal @attackcommand
				send(@actor, "The animal lashes out at you!")
				return true
			end if
			if (not(setnpcai(@animal, {{mountAiId}}, true)))
				return true
			end if
			setregister @animal "{{MountOwnerRegister}}" @actor.identityid
			setregister @animal "{{MountTrainedRegister}}" 1
			var approved as number collection
			approved = getregister(@animal, "{{MountApprovedRidersRegister}}")
			additem approved @actor.identityid
			setregister @animal "{{MountApprovedRidersRegister}}" @approved
			sendloc(@actor.location, "$0 takes a careful approach to $1 until it finally settles.", @actor, @animal)
			return false
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		foreach (var source in eligible.Concat(sourceMounts).DistinctBy(x => x.Vnum))
		{
			InstallNpcInputHook("Mount Handling " + source.Vnum, "mount", taming, [source.Vnum]);
			InstallNpcInputHook("Mount Handling Ride " + source.Vnum, "ride", taming, [source.Vnum]);
		}

		var mountLeadRoles = new[] { rangerRole, OwnedBehaviourId("arm.class.merchant", nameof(ChargenRole)) }
			.Where(x => x.HasValue).Select(x => x!.Value).ToArray();
		var halfElfRaceIds = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0]
			.Equals("Half-Elf", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
		var mountLeadPredicates = mountLeadRoles.Select(x => $"contains(@actor.roles, torole({x}))")
			.Concat(halfElfRaceIds.Select(x => $"@actor.race.id == {x}")).ToArray();
		var mountLeadPredicate = mountLeadPredicates.Length switch
		{
			0 => "false",
			1 => mountLeadPredicates[0],
			_ => "(" + string.Join(" or ", mountLeadPredicates) + ")"
		};
		var ownerCommands = Prog("ArmageddonWorldfileMountAccess", $$"""
			if (not(@actor.isawake))
				return false
			end if
			if (@args.Count(word, true) != 3 or targetcharacter(@actor, getindex(@args, 0)) != tocharacter(@npc))
				return false
			end if
			var action = lowercase(getindex(@args, 1))
			var person = targetcharacter(@actor, getindex(@args, 2))
			var mount = tocharacter(@npc)
			if (isnull(@person) or getregister(@mount, "{{MountOwnerRegister}}") != @actor.identityid)
				send(@actor, "Only the owner can change this animal's approved riders.")
				return true
			end if
			var approved as number collection
			var updated as number collection
			approved = getregister(@mount, "{{MountApprovedRidersRegister}}")
			if (@action == "approve" or @action == "allow")
				if (not(contains(@approved, @person.identityid)))
					additem approved @person.identityid
					setregister @mount "{{MountApprovedRidersRegister}}" @approved
				end if
				send(@actor, "That rider may now handle your animal.")
				return true
			elseif (@action == "revoke" or @action == "deny")
				foreach (identity in @approved)
					if (@identity != @person.identityid)
						additem updated @identity
					end if
				end foreach
				setregister @mount "{{MountApprovedRidersRegister}}" @updated
				send(@actor, "That rider's permission has been removed.")
				return true
			elseif (@action == "transfer")
				setregister @mount "{{MountOwnerRegister}}" @person.identityid
				additem updated @person.identityid
				setregister @mount "{{MountApprovedRidersRegister}}" @updated
				send(@actor, "You transfer ownership of the animal.")
				return true
			end if
			return false
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hitch = Prog("ArmageddonWorldfileMountHitch", $$"""
			if (not(@actor.canmove) or @actor.incombat)
				return false
			end if
			if (@args.Count(word, true) < 1 or @args.Count(word, true) > 2 or targetcharacter(@actor, getindex(@args, 0)) != tocharacter(@npc))
				return false
			end if
			var mount = tocharacter(@npc)
			if (getregister(@mount, "{{MountTrainedRegister}}") != 1)
				return false
			end if
			if (@args.Count == 2 and isnull(targetcharacter(@actor, getindex(@args, 1))))
				return false
			end if
			if (not(@mount.isawake) or @mount.incombat)
				send(@actor, "You cannot lead the animal just now.")
				return true
			end if
			var owner = getregister(@mount, "{{MountOwnerRegister}}")
			var approved = getregister(@mount, "{{MountApprovedRidersRegister}}")
			if (@owner > 0 and @owner != @actor.identityid and not(contains(@approved, @actor.identityid)))
				send(@actor, "The animal does not accept you as its handler.")
				return true
			end if
			if (@mount.riders.Count(occupant, true) > 0 or not(@{{handlerMatch.FunctionName}}(@mount, @actor)))
				send(@actor, "Someone else is already handling that animal.")
				return true
			end if
			var leader = @actor
			if (@args.Count(word, true) == 2)
				leader = targetcharacter(@actor, getindex(@args, 1))
				if (isnull(@leader) or (@leader != @actor and (getregister(@leader, "{{MountTrainedRegister}}") != 1 or @ArmageddonWorldfileMountCurrentHandler(@leader) != @actor)))
					send(@actor, "You can only lead an animal yourself or through an animal already following you.")
					return true
				end if
			end if
			var maxlead = 1
			if ({{mountLeadPredicate}})
				maxlead = 2
			end if
			if (@{{followerCount.FunctionName}}(@actor) >= @maxlead and (isnull(@mount.following) or @ArmageddonWorldfileMountCurrentHandler(@mount) != @actor))
				send(@actor, "You are already leading as many animals as you can handle.")
				return true
			end if
			if (not(setfollowing(@mount, @leader)) or not(setnpccompanion(@mount, @actor)))
				send(@actor, "You cannot get the animal to follow that lead.")
				return true
			end if
			sendloc(@actor.location, "$0 take|takes the lead of $1.", @actor, @mount)
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var unhitch = Prog("ArmageddonWorldfileMountUnhitch", $$"""
			if (not(@actor.canmove) or @actor.incombat)
				return false
			end if
			var mount = tocharacter(@npc)
			if (@args.Count(word, true) != 1 or targetcharacter(@actor, getindex(@args, 0)) != @mount or isnull(@mount.following))
				return false
			end if
			if (not(@{{handlerMatch.FunctionName}}(@mount, @actor)))
				send(@actor, "That animal is being handled by someone else.")
				return true
			end if
			setnpccompanion(@mount, null("character"))
			return setfollowing(@mount, null("character"))
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var mounts = sourceMounts.Concat(eligible).DistinctBy(x => x.Vnum).Select(x => x.Vnum).ToArray();
		foreach (var vnum in mounts)
		{
			InstallNpcInputHook("Mount Ownership " + vnum, "mountaccess", ownerCommands, [vnum]);
			InstallNpcInputHook("Mount Hitch " + vnum, "hitch", hitch, [vnum]);
			InstallNpcInputHook("Mount Unhitch " + vnum, "unhitch", unhitch, [vnum]);
		}
		Record("mounts.owner-riders", "ArmageddonMountPolicy", null, null, new
		{
			OwnerRegister = MountOwnerRegister, ApprovedRidersRegister = MountApprovedRidersRegister, TrainedRegister = MountTrainedRegister,
			TamingProg = taming.Id, MountAi = mountAi.Id, OwnerProg = ownerCommands.Id, HitchProg = hitch.Id, UnhitchProg = unhitch.Id,
			Disposition = "Durable owner and approved-rider character identities are kept separately from native active rider/follower state. Purchases set their owner; the first successful wild tame claims ownership. An unowned, source-trained mount remains public. Native MountAI enforces permission and prevents a second active rider/controller; native size and movement checks still apply."
		});
		_counts["mount relationship progs"] = 8;
	}

	private void InstallMountWhistle()
	{
		const int vnum = 64129;
		var source = _source.Items.SingleOrDefault(x => x.Vnum == vnum && x.Programs.Contains("C whistle mount_whistle"));
		if (source is null || !_items.TryGetValue(vnum, out var prototype)) return;
		var riding = _db.TraitDefinitions.SingleOrDefault(x => x.Name.Equals("Riding", StringComparison.OrdinalIgnoreCase) && x.Type == 0);
		if (riding is null) throw new InvalidOperationException("The mount whistle requires the installed Riding skill.");
		var body = $$"""
			if (not(@actor.isawake))
				return false
			end if
			if (not(contains(@actor.helditems, @item)) and not(contains(@actor.wieldeditems, @item)))
				return false
			end if
			var skill = totrait({{riding.Id}})
			var success = gettrait(@actor, @skill) >= 50
			if (not(@success))
				success = check(@actor, @skill, 5) > 0
			end if
			sendloc(@actor.location, "$0 blow|blows a clear note on $1.", @actor, @item)
			foreach (mount in loadedcharacters())
				if (not(isnull(@mount.location)) and (@mount.location == @actor.location or @mount.location.exits.Any(exit, @exit.destination == @actor.location)))
					if (getregister(@mount, "{{MountTrainedRegister}}") == 1 and @mount.following == @actor and @mount.isawake and @mount.riders.Count(occupant, true) == 0 and canhearnoise(@mount, @actor, 4, 5))
						if (@success)
							force @mount "stand"
							var destination = @actor.location
							if (@mount.location != @destination)
								foreach (passage in @mount.location.exits)
									if (@passage.destination == @destination)
										force @mount lowercase(@passage.direction)
										break
									end if
								end foreach
							end if
						else
							send(@actor, "You hear a distant answering nicker, but no mount comes closer.")
						end if
					end if
				end if
			end foreach
			return true
			""";
		var prog = Prog("ArmageddonWorldfileMountWhistle", body, ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile Mount Whistle", Category = "Worldfile", Type = "HookOnInput",
			TargetEventType = (int)EventType.CommandInput, Definition = new XElement("Definition", new XElement("TargetCommand", "whistle"),
				new XElement("UseProgResultForHandling", true), new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook);
		foreach (var proto in new[] { prototype }.Concat(_resetItemVariants.Where(x => x.Key.StartsWith(vnum + ":", StringComparison.Ordinal)).Select(x => x.Value)).DistinctBy(x => x.Id))
			AddItemOnLoadProg(proto, "MountWhistle", $"addhook(@item, {hook.Id})\nreturn true");
		Record("item-special.mount-whistle", nameof(Hooks), hook.Id, null, new { Source = vnum, Prog = prog.Id,
			Disposition = "Unplaced source item remains unplaced. Held or wielded use only. Native Riding success threshold, awake handler-associated mounts in the same or directly adjacent room, with normal safe movement and no teleporting. The source checks a primary or secondary hand; this item is not worn in an ear." });
	}
}
