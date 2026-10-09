"""Run with python -B scripts/FuryCalmSmokeWorld/test_guardian_journals.py."""
import json
import unittest

from guardian_journals import parse_journals


class GuardianJournalTransportTests(unittest.TestCase):
    @staticmethod
    def row(identifier='old', diagnostic=''):
        return [identifier, 22, 2, 2, 'FAMILYHEX', 2, 'created', 'deadline', 'PROVENANCEHEX',
                3, 0, 'death', None, None, None, None, 'updated', 7, diagnostic]

    def test_old_last_empty_diagnostic_survives_new_sorted_row_and_stdout_strip(self):
        old = json.dumps(self.row())
        before = parse_journals((old + '\n').strip())
        after = parse_journals((old + '\n' + json.dumps(self.row('z-new')) + '\n').strip())
        self.assertEqual(before, {key: row for key, row in after.items() if key in before})
        self.assertEqual('', after['old'][-1])

    def test_null_and_empty_diagnostic_remain_distinct(self):
        self.assertNotEqual(parse_journals(json.dumps(self.row(diagnostic=None))),
                            parse_journals(json.dumps(self.row(diagnostic=''))))

    def test_change_in_any_known_field_is_detected(self):
        original = self.row()
        before = parse_journals(json.dumps(original))
        for index in range(19):
            with self.subTest(column=index):
                changed = original.copy()
                changed[index] = 'changed'
                self.assertNotEqual(before, parse_journals(json.dumps(changed)))

    def test_missing_or_additional_field_is_rejected(self):
        for fields in [self.row()[:-1], self.row() + ['extra']]:
            with self.assertRaises(ValueError):
                parse_journals(json.dumps(fields))

    def test_duplicate_or_missing_identity_is_rejected(self):
        with self.assertRaises(ValueError):
            parse_journals(json.dumps(self.row()) + '\n' + json.dumps(self.row()))
        with self.assertRaises(ValueError):
            parse_journals(json.dumps(self.row(identifier=None)))


if __name__ == '__main__':
    unittest.main()
