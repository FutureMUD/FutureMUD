"""Bounded S6 ordinary-player integration. S5 owns paid practice/funding evidence.

The player is a disposable copied-body fixture, not a chargen-UI certificate.
Canonical availability stays false; only a separate full Earth policy is enabled.
"""
import time
import importlib.util


def run(host, admin, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s5_progression']
    actor, cap, resource = (int(cfg[k]) for k in ('actor', 'capability', 'resource'))
    report = receipt.setdefault('s6_integration', {'status': 'FAIL', 'mode': cfg['mode'],
        'earlier_classes': data['classes_enabled'], 'qualification_capability': cap,
        'limits': 'Separate complete Earth capability; copied-body Player fixture. Selected-Class enrolment and natural branch boundaries use named shared tests; no complete chargen approval or UI campaign.'})

    def flush():
        out = admin.send('impdebug flush', read_seconds=.4)
        deadline = time.monotonic() + 6
        while 'All queued saves have been flushed' not in out and time.monotonic() < deadline:
            out += admin.read_for(.4)
        require('All queued saves have been flushed' in out, 'S6 administrator save flush failed.')

    def state():
        return {
            'acquired': sql(mapped(f'SELECT MagicSpellId,ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} ORDER BY MagicSpellId')),
            'skills': sql(mapped(f'SELECT TraitDefinitionId,Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId IN ({','.join(str(int(x)) for x in cfg['skill_ids'])}) ORDER BY TraitDefinitionId')),
            'enrolment': sql(mapped(f'SELECT MagicCapabilityId,CompletedStartingGrantVersion FROM CharacterCastingEnrolments WHERE CharacterId={actor} ORDER BY MagicCapabilityId')),
            'opportunities': sql(mapped(f'SELECT TraitDefinitionId,NextOpportunityUtc,StateVersion FROM CharacterMagicSkillOpportunities WHERE CharacterId={actor} ORDER BY TraitDefinitionId')),
            'operations': sql(mapped(f'SELECT HEX(Id),Stage,HEX(Definition),CreatedUtc FROM MagicCastingOperations WHERE CharacterId={actor} ORDER BY CreatedUtc,Id'), (0, 2))}

    authority = sql(f'SELECT CAST(c.IsAdminAvatar AS UNSIGNED),g.AuthorityLevel FROM Characters c JOIN Accounts a ON a.Id=c.AccountId JOIN AuthorityGroups g ON g.Id=a.AuthorityGroupId WHERE c.Id={actor}')
    require(authority == [['0', '2']], 'S6 player must have ordinary Player authority and non-admin avatar.')
    for gate in ('use_gate', 'craft_gate'):
        require(sql(f'SELECT HEX(FunctionText) FROM FutureProgs WHERE Id={int(cfg[gate])}', (0,)) == [['return false']], 'S6 enabled a stock availability gate.')
    if phase == 'first':
        initial = state()
        require(not initial['acquired'] and not initial['skills'] and not initial['enrolment'] and not initial['operations'], 'S6 player inherited magic state.')
    else:
        require(state() == report['saved'], 'S6 cold pre-login changed exact acquisitions, skills, enrolment, opportunities or operations.')
        assertions.append('S6 integration: cold pre-login retains exact starting knowledge, raw skills and enrolment without production or opportunity replay')

    player = host['helper'].MudSocket('127.0.0.1', int(receipt['processes'][-1]['endpoint'].rsplit(':', 1)[1]), host['SECRETS'])
    try:
        player.read_for(1)
        for command in ('l', cfg['account']): player.send(command, read_seconds=1)
        player.send(host['PASSWORD'], secret=True, read_seconds=1)
        player.send('c', read_seconds=1)
        player.send('1', read_seconds=2)
        looked = player.send('look', read_seconds=1)
        require('Guest Lounge' in looked, 'Ordinary S6 player did not enter the world.')
        denied = player.send('impdebug flush', read_seconds=.3)
        require('All queued saves have been flushed' not in denied, 'S6 player acquired administrator authority.')
        if phase == 'first':
            admin.send(f'magic casting enrol participant {cap} isolated S6 integration qualification', read_seconds=.5)
            flush()
            acquired = state()
            require(sorted(int(x[0]) for x in acquired['acquired']) == sorted(cfg['roots']) and all(x[1] == '1' for x in acquired['acquired']), 'S6 explicit enrolment did not grant exactly starting roots at grade one.')
            require({str(x[0]): float(x[1]) for x in acquired['skills']} == cfg['starting_skills'], 'S6 enrolment did not open the native selected-mode skills.')
            admin.send(f'magic casting enrol participant {cap} repeat S6 enrolment must be idempotent', read_seconds=.4)
            flush()
            require(state() == acquired, 'Repeated S6 enrolment changed acquisition, raw skill or opportunity state.')
            assertions.append('S6 integration: normal permanent-merit enrolment grants exact stock roots at grade one and selected-mode raw openings; repeat is idempotent')
        before = state()
        # No energy is injected or awaited: this bounded check verifies admission and
        # refusal ordering. Accepted S5 receipts own actual rest funding/paid progress.
        out = player.send(f'armearth practice "Earthen Ward" grade 1 via {cap}', read_seconds=.4)
        flush()
        reserve = float(sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}),0)'))[0][0])
        require(reserve < 50 and f'Insufficient {cfg['resource_name']} for the combined cost.' in out, 'S6 did not reach the native insufficient-reserve admission check: ' + host['redact'](out))
        require(state() == before, 'Unfunded S6 ordinary-player practice created progress/payment work: ' + host['redact'](out))
        assertions.append('S6 integration: ordinary Player login, denied administrator command and unfunded target-free practice refusal preserve progress')
        gather_spec = importlib.util.spec_from_file_location('s6_gathering', host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS6GatheringSmoke.py')
        gather_module = importlib.util.module_from_spec(gather_spec)
        gather_spec.loader.exec_module(gather_module)
        gather_module.run(host, admin, player, phase, data, mapped, flush)
        if host['os'].environ.get('FUTUREMUD_S6_READINESS_SMOKE_ONLY') == '1':
            def load(name):
                spec = importlib.util.spec_from_file_location(name, host['REPO'] / ('tests/ArmageddonPreparedSeederNativeHarness/' + name + '.py'))
                module = importlib.util.module_from_spec(spec)
                spec.loader.exec_module(module)
                return module
            load('PreparedS6ChargenSmoke').run(host, admin, phase, data, mapped, flush)
            ash = load('PreparedS6AshExpirySmoke')
            if phase == 'first': ash.expire(host, admin, player, data, mapped, flush)
            else: ash.cold(host, player)
            load('PreparedS6UncertaintySmoke').run(host, admin, phase, data, mapped, flush)
            report['limits'] = 'Separate full Earth Player enrolment plus actual empty-account chargen UI/finalisation, real ash expiry, native MySQL provider fault and cold conservation; explicit readiness lane. Original stock gates disabled.'
        if phase == 'cold': report['cold_status'] = 'PASS'
        else: report['status'] = 'PASS'
    finally:
        try:
            player.send('quit', read_seconds=1)
            flush()
            if phase == 'first' and report['status'] == 'PASS': report['saved'] = state()
        finally:
            (host['Path'](receipt['runtime_paths'][-1])/'s6-player-transcript.txt').write_text(player.full_transcript(), encoding='utf-8')
            player.close()
