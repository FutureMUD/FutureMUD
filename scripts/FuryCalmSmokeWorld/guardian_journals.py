"""Lossless transport for the nineteen explicit lifecycle journal columns."""
import json

JOURNAL_COLUMNS = ('Id,SpellId,Grade,CreatorId,HEX(Family),Mode,CreatedUtc,DeadlineUtc,'
                   'HEX(Provenance),State,Reason,DeathObservedUtc,RemainsItemId,'
                   'RemainsRemovalRequestedUtc,RemainsNotificationAttemptedUtc,'
                   'RemainsNotificationCompletedUtc,UpdatedUtc,Version,HEX(Diagnostic)')


def parse_journals(output):
    """JSON_ARRAY preserves nulls/empty fields even if SQL stdout is stripped."""
    result = {}
    for line in output.splitlines():
        fields = json.loads(line)
        if not isinstance(fields, list) or len(fields) != 19:
            raise ValueError('Expected all nineteen lifecycle journal fields')
        identifier = fields[0]
        if not isinstance(identifier, str) or not identifier or identifier in result:
            raise ValueError('Expected one unique lifecycle identity per journal row')
        result[identifier] = tuple(fields)
    return result
