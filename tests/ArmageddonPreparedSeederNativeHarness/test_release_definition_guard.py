"""Bounded regression checks for the original-stock native acceptance guard."""
import unittest

from PreparedReleaseRegionalSmoke import release_definition_matches


class ReleaseDefinitionGuardTests(unittest.TestCase):
    def setUp(self):
        self.seed = ('<Definition version="1">\r\n  <NaturalRepairPerMinute>0</NaturalRepairPerMinute>\r\n'
                     '  <Outputs>\r\n    <Output resource="10"><Maximum>0</Maximum></Output>\r\n'
                     '  </Outputs>\r\n  <Organic version="1" />\r\n</Definition>')
        self.exported = self.seed.replace('  <Outputs>',
            '  <DecayReferenceUtc>1970-01-01T00:00:00.0000000+00:00</DecayReferenceUtc>\r\n'
            '  <DecayIntegral>0</DecayIntegral>\r\n  <Outputs>', 1)

    def test_unchanged_definitions_remain_exact_in_both_phases(self):
        for phase in ('first', 'cold'):
            self.assertTrue(release_definition_matches('MagicGenerators', self.seed, self.seed, phase))

    def test_only_cold_accepts_exact_implicit_default_export(self):
        self.assertFalse(release_definition_matches('MagicGenerators', self.seed, self.exported, 'first'))
        self.assertTrue(release_definition_matches('MagicGenerators', self.seed, self.exported, 'cold'))

    def test_normalization_does_not_hide_formula_or_resource_edits(self):
        for actual in (self.exported.replace('<Maximum>0', '<Maximum>1'),
                       self.exported.replace('resource="10"', 'resource="11"'),
                       self.exported.replace('<NaturalRepairPerMinute>0', '<NaturalRepairPerMinute>1')):
            self.assertFalse(release_definition_matches('MagicGenerators', self.seed, actual, 'cold'))

    def test_nondefault_or_duplicate_timeline_is_rejected(self):
        for actual in (self.exported.replace('<DecayIntegral>0', '<DecayIntegral>1'),
                       self.exported.replace('1970-01-01', '1970-01-02'),
                       self.exported.replace('  <Outputs>', '  <DecayIntegral>0</DecayIntegral>\r\n  <Outputs>')):
            self.assertFalse(release_definition_matches('MagicGenerators', self.seed, actual, 'cold'))

    def test_other_tables_and_non_environmental_generators_stay_strict(self):
        self.assertFalse(release_definition_matches('MagicCapabilities', self.seed, self.exported, 'cold'))
        self.assertFalse(release_definition_matches('MagicGenerators', self.seed.replace('<Organic ', '<Other '),
                                                   self.exported.replace('<Organic ', '<Other '), 'cold'))

    def test_explicit_timeline_is_not_overwritten(self):
        self.assertFalse(release_definition_matches('MagicGenerators', self.exported,
                                                   self.exported.replace('<DecayIntegral>0', '<DecayIntegral>1'), 'cold'))


if __name__ == '__main__':
    unittest.main()
