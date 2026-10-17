"""Focused no-database checks for the bounded owned SQL retry in the live boot helper."""

import ast
import hashlib
import os
from pathlib import Path
import re
import subprocess
from types import SimpleNamespace
import unittest


class CatalogueSqlRetryTests(unittest.TestCase):
    def setUp(self):
        source = Path(__file__).with_name('PreparedCatalogueBootSmoke.py').read_text(encoding='utf-8')
        method = next(node for node in ast.parse(source).body
                      if isinstance(node, ast.FunctionDef) and node.name == 'sql')
        self.responses = []
        self.calls = []
        def run(command, **kwargs):
            self.calls.append(command)
            return self.responses.pop(0)
        def require(value, message):
            if not value:
                raise RuntimeError(message)
        self.receipt = {}
        self.scope = {
            'subprocess': SimpleNamespace(run=run, PIPE=subprocess.PIPE, CREATE_NO_WINDOW=0),
            'os': os, 're': re, 'hashlib': hashlib, 'Path': Path,
            'mysql': Path('mysql'), 'port': 12345, 'user': 'owned', 'database': 'owned_db',
            'sql_password': 'test', 'expected_uuid': 'owned-uuid',
            'expected_datadir': os.path.normcase(os.path.normpath('C:\\owned')),
            'receipt': self.receipt, 'marker': None, 'redact': lambda value: value,
            'require': require, 'batch_cell': lambda value: value
        }
        exec(compile(ast.Module(body=[method], type_ignores=[]), '<boot-helper-sql>', 'exec'), self.scope)

    def response(self, stdout='', returncode=0, stderr=''):
        return SimpleNamespace(stdout=stdout, returncode=returncode, stderr=stderr)

    def test_empty_success_retries_once_and_retains_first_anomaly(self):
        marker = 'a' * 40
        self.responses = [self.response(),
                          self.response('owned-uuid\t12345\tC:/owned\n' + marker + '\n1\n')]
        self.assertEqual([['1']], self.scope['sql']('SELECT 1', context='arm.spell.earth_mount:NpcTemplate:1'))
        self.assertEqual(2, len(self.calls))
        self.assertEqual(marker, self.scope['marker'])
        self.assertEqual('arm.spell.earth_mount:NpcTemplate:1',
                         self.receipt['sql_output_anomalies'][0]['context'])
        self.assertEqual(0, self.receipt['sql_output_anomalies'][0]['first_rows'])
        self.assertEqual(3, self.receipt['sql_output_anomalies'][0]['retry_rows'])
        self.assertEqual(self.calls[0], self.calls[1])

    def test_repeated_empty_success_fails_with_both_attempts_recorded(self):
        self.responses = [self.response(), self.response()]
        with self.assertRaisesRegex(RuntimeError, 'Missing owned SQL descriptor/marker'):
            self.scope['sql']('SELECT 1', context='earth-mount-reference')
        self.assertEqual(2, len(self.calls))
        self.assertEqual(0, self.receipt['sql_output_anomalies'][0]['retry_rows'])

    def test_sql_error_does_not_retry(self):
        self.responses = [self.response(returncode=1, stderr='syntax error')]
        with self.assertRaisesRegex(RuntimeError, 'Owned SQL failed'):
            self.scope['sql']('SELECT 1')
        self.assertEqual(1, len(self.calls))
        self.assertNotIn('sql_output_anomalies', self.receipt)

    def test_mutation_short_output_fails_closed_without_repeating_write(self):
        self.responses = [self.response()]
        with self.assertRaisesRegex(RuntimeError, 'Missing owned SQL descriptor/marker'):
            self.scope['sql']("UPDATE StaticConfigurations SET Definition='off' WHERE SettingName='EmailServer'; SELECT ROW_COUNT()")
        self.assertEqual(1, len(self.calls))
        self.assertNotIn('retry_rows', self.receipt['sql_output_anomalies'][0])

    def test_retry_still_requires_exact_owned_descriptor(self):
        self.responses = [self.response(),
                          self.response('wrong-uuid\t12345\tC:/owned\n' + 'a' * 40 + '\n1\n')]
        with self.assertRaisesRegex(RuntimeError, 'Exact owned MySQL descriptor mismatch'):
            self.scope['sql']('SELECT 1')
        self.assertEqual(2, len(self.calls))


if __name__ == '__main__':
    unittest.main()
