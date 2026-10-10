"""Real installer failure qualification on the existing stopped disposable world."""
import hashlib
import importlib.util
import json
import pathlib

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)

s.report['scope'] = 'real MySQL transaction interruption, lost acknowledgement and stable recovery'
s.report['qualificationMarker'] = 'fury-calm-installer-failures-passed.json'
s.report['inputs'][str(pathlib.Path(__file__).relative_to(s.repo))] = hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest()
try:
    prior = json.loads((s.root / 'fury-calm-slice-passed.json').read_text())
    s.check('paid persistence prerequisite passed', prior['status'] == 'PASS')
    result = s.fixture('failure-cases')
    s.report['installer'] = result
    s.check('native rollback and lost-acknowledgement recovery', result['FailureCases']['RetryStable'] and
            result['FailureCases']['OriginalMappingRestored'] and result['FailureCases']['PlayerAndOtherTablesStable'])
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
finally:
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
