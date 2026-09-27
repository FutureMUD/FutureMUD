"""Process-ownership regressions; no server or database is started."""
import json
import pathlib
import tempfile
import unittest
from unittest.mock import Mock

from native import stop_owned_mysql


class CleanupTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory(prefix='authored-cleanup-')
        self.addCleanup(self.directory.cleanup)
        self.root = pathlib.Path(self.directory.name)
        self.instance = {'data': str(self.root / 'data'), 'client': ['mysql.exe', '--batch']}

    def test_failed_identity_still_stops_newly_spawned_child(self):
        process = Mock()
        process.poll.return_value = None
        query = Mock(side_effect=RuntimeError('startup client unavailable'))
        run = Mock()
        stop_owned_mysql(self.instance, process, query, run, self.root)
        process.terminate.assert_called_once()
        process.wait.assert_called_once_with(timeout=30)
        run.assert_not_called()
        self.assertTrue(json.loads((self.root / 'cleanup.json').read_text())['mysqlStopped'])

    def test_unverifiable_resumed_instance_is_not_terminated(self):
        run = Mock()
        with self.assertRaisesRegex(RuntimeError, 'no process was terminated'):
            stop_owned_mysql(self.instance, None, Mock(side_effect=RuntimeError('wrong identity')), run, self.root)
        run.assert_not_called()
        self.assertFalse(json.loads((self.root / 'cleanup.json').read_text())['mysqlStopped'])

    def test_verified_shutdown_uses_admin_and_waits_without_termination(self):
        process = Mock()
        run = Mock(return_value=Mock(returncode=0))
        stop_owned_mysql(self.instance, process, Mock(), run, self.root)
        self.assertEqual(run.call_args.args[0], ['mysqladmin.exe', 'shutdown'])
        process.terminate.assert_not_called()
        process.wait.assert_called_once_with(timeout=30)
        self.assertTrue(json.loads((self.root / 'cleanup.json').read_text())['mysqlStopped'])


if __name__ == '__main__':
    unittest.main()
