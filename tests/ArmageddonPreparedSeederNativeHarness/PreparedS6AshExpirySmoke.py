"""Native real-wall-clock retirement; the existing gathering lane owns payment assertions."""
import datetime
import time
import xml.etree.ElementTree as ET

ASH = 'Grey, lifeless ash collects on the ground.'
MESSAGE = 'The grey ash on the ground dissipates.'


def expire(host, admin, player, data, mapped, flush):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    gathering = receipt['s6_gathering']
    cfg = data['s6_gathering']
    before = gathering['saved']
    registers = {row[0]: ET.fromstring(row[1]).text for row in before['registers']}
    deadline = datetime.datetime.fromisoformat(registers['armageddon_ash_until'].replace('Z', '+00:00'))
    effects = ET.fromstring(before['room_effects'][0][0])
    timed = [x for x in effects.iter('Effect') if x.findtext('AdditionalText') == ASH]
    require(len(timed) == 1 and datetime.datetime.fromisoformat(timed[0].findtext('ExpiresUtc').replace('Z', '+00:00')) == deadline,
            'Native ash effect did not persist the exact source register deadline.')
    report = receipt.setdefault('s6_ash_expiry', {'status': 'FAIL', 'deadline_utc': deadline.isoformat(),
        'nominal_first_land_seconds': 270, 'additional_two_mana_seconds': 540,
        'limits': 'Actual unaccelerated wall clock and one local observer; no ecological restoration.'})
    observer_start = datetime.datetime.now(datetime.timezone.utc)
    require(observer_start < deadline - datetime.timedelta(seconds=5), 'UI consumed the live ash observation window; no exact deadline qualification is possible.')
    output = player.read_for(.3)
    require(MESSAGE not in output, 'Ash announced expiry before its additive UTC deadline.')
    report['observer_started_utc'] = observer_start.isoformat()
    start = time.monotonic()
    while datetime.datetime.now(datetime.timezone.utc) < deadline + datetime.timedelta(seconds=3):
        remaining = (deadline + datetime.timedelta(seconds=3) - datetime.datetime.now(datetime.timezone.utc)).total_seconds()
        chunk = player.read_for(min(1, max(.1, remaining)))
        output += chunk
        if MESSAGE in output and 'message_observed_utc' not in report:
            observed = datetime.datetime.now(datetime.timezone.utc)
            require(observed >= deadline, 'Native ash expired before its persisted additive deadline.')
            report['message_observed_utc'] = observed.isoformat()
        require(time.monotonic() - start < 830, 'Real ash wait exceeded the explicit source-duration budget.')
    flush()
    output += player.read_for(1)
    require(output.count(MESSAGE) == 1, 'Live ash expiry must announce exactly once to its local observer: ' + host['redact'](output))
    require('message_observed_utc' in report, 'Live ash announcement has no bounded receive-time observation.')
    looked = player.send('look', read_seconds=.5)
    require(ASH not in looked and MESSAGE not in looked, 'Expired ash remained visible or replayed its broadcast.')
    room = int(cfg['room'])
    saved = sql(mapped(f'SELECT HEX(EffectData) FROM Rooms WHERE Id={room}'), (0,))
    require(not any(x.text == ASH for x in ET.fromstring(saved[0][0]).iter('AdditionalText')), 'Live expiry retained the ash addendum in persisted room effects.')
    actor, resource, field = (int(cfg[k]) for k in ('actor', 'resource', 'field'))
    checks = {
        'reserve': sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}')),
        'field': sql(mapped(f'SELECT Pasture,HEX(Definition) FROM AgricultureFields WHERE Id={field}'), (1,)),
        'ecology': sql(mapped(f'SELECT ScarDamage,RecentPressure FROM RoomEnvironmentalStates WHERE RoomId={room}'))}
    for key, value in checks.items(): require(value == before[key], 'Ash expiry restored or changed ' + key)
    gathering['saved']['room_effects'] = saved
    report.update(status='PASS', observed_messages=1, waited_seconds=round(time.monotonic()-start, 3), room_effects=saved,
                  expired_look=host['redact'](looked), conservation=checks)
    assertions.append('S6 ash: actual270+540-second additive deadline expires once locally, removes persisted presentation and leaves life/scars/reserve unchanged')


def cold(host, player):
    report = host['receipt']['s6_ash_expiry']
    output = player.send('look', read_seconds=.6)
    host['require'](ASH not in output and MESSAGE not in output, 'Cold boot resurrected expired ash or replayed dissipation.')
    report['cold_status'] = 'PASS'
    host['assertions'].append('S6 ash: cold process restart retains retirement without a second announcement or ecological repair')
