"""Bounded normal chargen UI, with explicit external answers and durable assertions."""
import json
import os
import time
import xml.etree.ElementTree as ET
from pathlib import Path


def run(host, admin, phase, data, mapped, flush):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s6_chargen']
    account = int(cfg['account_id'])
    report = receipt.setdefault(cfg.get('report_key', 's6_chargen'), {'status': 'FAIL', 'limits': cfg['limits'], 'steps': []})

    def state():
        actors = sql(mapped(f'SELECT Id,BodyId,ChargenId,CAST(IsAdminAvatar AS UNSIGNED) FROM Characters WHERE AccountId={account} ORDER BY Id'))
        result = {'applications': sql(mapped(f'SELECT Id,Status,ApprovedById,ApprovalTime FROM Chargens WHERE AccountId={account} ORDER BY Id')), 'characters': actors}
        if actors:
            actor = int(actors[0][0])
            result.update(names=sql(mapped(f'SELECT HEX(NameInfo) FROM Characters WHERE Id={actor}'), (0,)),
                approval_notes=sql(mapped(f"SELECT AuthorId,Subject,Text FROM AccountNotes WHERE AccountId={account} AND Subject LIKE '% - Approval Note' ORDER BY Id")),
                roles=sql(mapped(f'SELECT ChargenRoleId FROM CharactersChargenRoles WHERE CharacterId={actor} ORDER BY ChargenRoleId')),
                merits=sql(mapped(f'SELECT MeritId FROM PerceiverMerits WHERE CharacterId={actor} ORDER BY MeritId')),
                acquired=sql(mapped(f'SELECT MagicSpellId,ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} ORDER BY MagicSpellId')),
                skills=sql(mapped(f'SELECT TraitDefinitionId,Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId IN ({','.join(str(int(x)) for x in cfg['expected_skills'])}) ORDER BY TraitDefinitionId')),
                enrolment=sql(mapped(f'SELECT MagicCapabilityId,CompletedStartingGrantVersion FROM CharacterCastingEnrolments WHERE CharacterId={actor} ORDER BY MagicCapabilityId')),
                reserve=sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={int(cfg['resource'])}')),
                operations=sql(mapped(f'SELECT HEX(Id),Stage,MagicCapabilityId,MagicSpellId,TraitDefinitionId,HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} ORDER BY Id'), (5,)),
                opportunities=sql(mapped(f'SELECT TraitDefinitionId,NextOpportunityUtc FROM CharacterMagicSkillOpportunities WHERE CharacterId={actor} ORDER BY TraitDefinitionId')))
        return result

    if phase == 'cold':
        require(state() == report['saved'], 'Cold process boot changed actual UI-created character/class enrolment or duplicated magic state.')
        report['cold_status'] = 'PASS'
        assertions.append('S6 chargen: exact real UI-created identity, class merit, roots, supports and saved reserve/progress survive cold restart without replay')
        return
    require(not state()['applications'] and not state()['characters'], 'Applicant account inherited a character or application.')
    authority = sql(f'SELECT g.AuthorityLevel FROM Accounts a JOIN AuthorityGroups g ON g.Id=a.AuthorityGroupId WHERE a.Id={account}')
    require(authority == [['2']], 'Applicant is not an ordinary Player.')
    path = Path(cfg.get('commands_file', os.environ['FUTUREMUD_S6_UI_COMMANDS'])).resolve()
    prompt_path = path.with_suffix('.prompt.json')
    player = host['helper'].MudSocket('127.0.0.1', int(receipt['processes'][-1]['endpoint'].rsplit(':', 1)[1]), host['SECRETS'])
    started = time.monotonic()
    try:
        player.read_for(1)
        for command in ('l', cfg['account']): player.send(command, read_seconds=.7)
        output = player.send(host['PASSWORD'], secret=True, read_seconds=1)
        output += player.send('n', read_seconds=1)
        for index in range(160):
            flush()
            applications = state()['applications']
            if applications and applications[0][1] == '1': break
            require(not applications or applications[0][1] == '0', 'UI unexpectedly bypassed normal pending review.')
            prompt_path.write_text(json.dumps({'index': index, 'output': host['redact'](output), 'elapsed_seconds': round(time.monotonic()-started, 1)}, indent=2), encoding='utf-8')
            while True:
                require(time.monotonic() - started < 1500, 'Full UI exceeded the explicit25-minute decision budget.')
                require(path.exists(), 'Explicit UI commands input is missing.')
                commands = json.loads(path.read_text(encoding='utf-8-sig'))
                require(isinstance(commands, list) and all(isinstance(x, str) for x in commands), 'UI answers must be an explicit ordered text array.')
                if index < len(commands): break
                time.sleep(.4)
            command = commands[index]
            report['steps'].append({'index': index, 'prompt': host['redact'](output), 'answer': command})
            output = player.send(command, read_seconds=.5)
        else: raise RuntimeError('Full UI exceeded160 explicit commands.')
        submitted = state()
        require(len(submitted['applications']) == 1 and submitted['applications'][0][1] == '1' and not submitted['characters'], 'Normal UI did not submit exactly one pending application before any character/body was created.')
        application = int(submitted['applications'][0][0])
        report['submitted'] = submitted
        review = admin.send(f'application review {application}', read_seconds=.5)
        require(cfg['role_name'] in review, 'Normal staff review did not show the selected qualification Class: ' + host['redact'](review))
        approval = admin.send('accept', read_seconds=.5)
        require('Please enter any comments you have to the applicant.' in approval and
                'entering an editor' in approval, 'Normal staff approval did not enter its comment editor: ' + host['redact'](approval))
        approval += admin.send('Approved for the isolated S6 Class qualification.', read_seconds=.5)
        approval += admin.send('@', read_seconds=1)
        report['approval_editor_completed'] = True
        flush()
        saved = state()
        report['after_approval'] = saved
        require(len(saved['characters']) == 1 and saved['applications'][0][1] == '5' and
                all(x not in ('NULL', '') for x in saved['applications'][0][2:]), 'Normal staff approval did not finalise one real character: ' + host['redact'](approval))
        actor, body, source, avatar = saved['characters'][0]
        # Production approval creates this account's only character but does not populate legacy Characters.ChargenId.
        require(source in ('NULL', str(application)) and int(body) > 0 and avatar == '0', 'Approval created the wrong identity/body or an admin avatar.')
        authored_name = ET.fromstring(sql(mapped(f'SELECT HEX(Definition) FROM Chargens WHERE Id={application}'), (0,))[0][0]).find('SelectedName/Name')
        actual_name = ET.fromstring(saved['names'][0][0]).find('PersonalName/Name')
        require(authored_name is not None and actual_name is not None and authored_name.attrib == actual_name.attrib and
                [(x.attrib, x.text) for x in authored_name] == [(x.attrib, x.text) for x in actual_name],
                'The real character did not retain the exact application naming culture and selected name elements.')
        require(len(saved['approval_notes']) == 1 and saved['approval_notes'][0][0] == saved['applications'][0][2] and
                saved['approval_notes'][0][2] == 'Approved for the isolated S6 Class qualification.',
                'Normal approval did not persist the exact reviewer and comment audit.')
        require([str(cfg['role'])] in saved['roles'] and [str(cfg['merit'])] in saved['merits'], 'Actual selected Class and permanent qualification capability merit were not retained.')
        require(sorted(int(x[0]) for x in saved['acquired']) == sorted(cfg['roots']) and all(x[1] == '1' for x in saved['acquired']), 'Class finalisation did not grant exactly the authored roots at grade one.')
        require({str(x[0]): float(x[1]) for x in saved['skills']} == cfg['expected_skills'], 'UI finalisation raw starting/casting/support skills differ from selected-mode Class grants.')
        require(any(int(x[0]) == int(cfg['capability']) and int(x[1]) > 0 for x in saved['enrolment']), 'Selected Class hook did not persist selected full-policy enrolment.')
        require(all(float(x[0]) == 0 for x in saved['reserve']) and not saved['opportunities'], 'UI-created caster received reserve or paid progression opportunity work without earning it.')
        policy = ET.fromstring(sql(mapped(f'SELECT HEX(Definition) FROM MagicCapabilities WHERE Id={int(cfg["capability"])}'), (0,))[0][0]).find('Casting')
        permitted = {'EnrolmentRecorded':'Enrolment', 'SkillCapRecorded':'SkillCap',
                     'SupportGranted':'SupportGrant', 'CappedSupportGranted':'SupportGrant'}
        starting = [x for x in policy.findall('Admission') if x.get('starting') == 'true']
        supports = {x.get('key'): x for x in policy.findall('SupportGrant') if x.get('starting') == 'true'}
        cap_traits = {int(x.get('trait', policy.get('trait'))) for x in starting if x.get('rawCap') is not None}
        require(saved['enrolment'] == [[str(cfg['capability']), policy.get('startingVersion')]],
                'Class creation must retain exactly its own completed starting-grant version.')
        recorded_caps, recorded_supports = [], []
        enrolments = [x for x in saved['operations'] if x[1] == 'EnrolmentRecorded']
        require(len(enrolments) == 1, 'Selected Class must record exactly one durable authored-enrolment audit.')
        for audit in saved['operations']:
            payload = ET.fromstring(audit[5])
            require(audit[1] in permitted and payload.tag == permitted[audit[1]] and payload.get('version') == '1' and
                    payload.get('identity') == policy.get('identity') and int(audit[2]) == int(cfg['capability']) and
                    (int(audit[3]) == 0 or int(audit[3]) in cfg['roots']) and
                    (int(audit[4]) == 0 or str(audit[4]) in cfg['expected_skills']),
                    'Class creation contains foreign authorisation or unexpected paid casting work: '+str(audit[:5]))
            if audit[1] == 'EnrolmentRecorded':
                require(int(audit[3]) == 0 and int(audit[4]) == 0 and
                        f'Class role {int(cfg["role"])}' in payload.findtext('Provenance', ''), 'Authored enrolment does not identify the exact selected Class.')
            elif payload.tag == 'SupportGrant':
                support = supports.get(payload.get('key'))
                require(support is not None and int(audit[3]) == 0 and int(audit[4]) == int(support.get('trait')) and
                        payload.get('trait') == support.get('trait') and payload.get('rawCap') == support.get('rawCap') and
                        float(payload.get('opening')) == float(support.get('opening')) == cfg['expected_skills'][str(audit[4])] and
                        (audit[1] == 'CappedSupportGranted') == (support.get('rawCap') is not None),
                        'Class support audit does not retain its authored opening.')
                recorded_supports.append(payload.get('key'))
            else:
                admission = next((x for x in starting if x.get('key') == payload.get('admission')), None)
                require(admission is not None and int(audit[3]) == int(admission.get('spell')) and
                        int(audit[4]) == int(admission.get('trait', policy.get('trait'))) and
                        payload.get('rawCap') == admission.get('rawCap'), 'Class cap audit differs from its starting admission.')
                recorded_caps.append(int(audit[4]))
        require(len(recorded_caps) == len(cap_traits) and set(recorded_caps) == cap_traits and
                len(recorded_supports) == len(supports) and set(recorded_supports) == supports.keys(),
                'Class creation lacks exact durable starting cap/support authorisations.')
        report.update(status='PASS', actor=int(actor), body=int(body), application=application, saved=saved, review=host['redact'](review), approval=host['redact'](approval))
        assertions.append('S6 chargen: empty ordinary account completes actual UI, selects isolated Class, submits pending application and receives one real approved identity/body with full-policy merit, authored grade-one roots/support openings and zero mana')
    finally:
        (Path(receipt['runtime_paths'][-1]) / cfg.get('transcript_name', 's6-chargen-transcript.txt')).write_text(player.full_transcript(), encoding='utf-8')
        player.close()

    return state
