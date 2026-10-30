"""Small grammar/semantic fixtures; no historical source tree or server required."""
import importlib.util
from pathlib import Path
import tempfile
import unittest

SPEC = importlib.util.spec_from_file_location("armageddon_package", Path(__file__).parents[1] / "armageddon-worldfile-package.py")
PACKAGE = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(PACKAGE)


class PackageTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)

    def reader(self, text):
        root = Path(self.directory.name)
        path = root / "fixture"
        path.write_bytes(text.encode("latin-1"))
        return PACKAGE.Reader(path, root)

    def test_same_line_room_headers_preserve_zero_and_multiline_prose(self):
        reader = self.reader("n|A café|0|3|0|0|\r\nep~\r\nFirst line\r\nSecond line~\r\nd|0|1|gate|1|0|A gate~\r\ne|sand|A dune~\r\nn|Other|1|3|0|0|\r\nep~\r\nElsewhere~\r\nx|")
        rooms = PACKAGE.rooms(reader, 4)
        self.assertEqual([0, 1], [x["Vnum"] for x in rooms])
        self.assertEqual("A café", rooms[0]["Name"])
        self.assertEqual("First line\nSecond line", rooms[0]["Description"])
        self.assertEqual(0, rooms[0]["Exits"][0]["Key"])
        self.assertEqual("A dune", rooms[0]["ExtraDescriptions"][0]["Description"])

    def test_legacy_object_flags_insert_only_the_missing_state_field(self):
        reader = self.reader("#0 2\nkey~\na key~\nA key lies here.~\nA tiny key.~\n18 0 1 11\n0 0 0 0 0 0\n2 3 4\n0 0 0 0\nep~\nA 1 2\nE\nrune~\nAn old rune~\n$")
        item = PACKAGE.prototypes(reader, False)[0]
        self.assertEqual([18, 0, 1, 0, 11], item["Flags"])
        self.assertEqual([[1, 2]], item["Affects"])
        self.assertEqual("An old rune", item["ExtraDescriptions"][0]["Description"])

    def test_reset_string_payload_does_not_eat_the_next_command(self):
        reader = self.reader("X 1 1\nrunes~\nTwo lines\nof runes~\nM 0 10 2 0\nS")
        commands = PACKAGE.resets(reader, 0)
        self.assertEqual(["X", "M"], [x["Opcode"] for x in commands])
        self.assertEqual(["runes", "Two lines\nof runes"], commands[0]["Strings"])
        self.assertEqual([0, 10, 2, 0], commands[1]["Arguments"])

    def shop(self, producing, new_items):
        return "1\n" + producing + "\n" + new_items + "\n1.0 1.1 1.2 1.3 1.4\n0.5 0.6 0.7 0.8 0.9\n1 2 3 4 5\n5 4 3 2 1\n" + "message~\n" * 8 + "100 6 22\n"

    def test_wrapped_shop_arrays_and_declared_tail(self):
        reader = self.reader(self.shop("1 2 3 4 5 6 7 8 9 10\n11 12 13 14 15", " ".join(["0"] * 20)) + "archive-only shop tail")
        shop = PACKAGE.shops(reader)[0]
        self.assertEqual(list(range(1, 16)), shop["Producing"])
        self.assertEqual([], shop["Repairs"])
        self.assertEqual(100, shop["Keeper"])
        self.assertEqual("ShopTailOutsideDeclaredCount", reader.diagnostics[0]["Type"])

    def test_missing_shop_new_items_is_an_explicit_repair(self):
        # The historical defect is a missing whole row before decimal multipliers.
        text = self.shop(" ".join(["-1"] * 15), "")
        reader = self.reader(text)
        shop = PACKAGE.shops(reader)[0]
        self.assertEqual([0] * 20, shop["NewItems"])
        self.assertEqual("NewItems", shop["Repairs"][0]["Field"])
        self.assertEqual(20, shop["Repairs"][0]["MissingSlots"])

    def test_real_extra_shop_values_are_not_silently_discarded(self):
        reader = self.reader(self.shop(" ".join(["-1"] * 15 + ["99"]), " ".join(["0"] * 20)))
        with self.assertRaisesRegex(ValueError, "ambiguous Producing"):
            PACKAGE.shops(reader)

    def test_first_loaded_duplicate_wins_with_both_source_locations(self):
        first = {"Vnum": 0, "Source": {"Path": "z1.obj", "Line": 2}, "Name": "first"}
        second = {"Vnum": 0, "Source": {"Path": "z2.obj", "Line": 8}, "Name": "later"}
        diagnostics = []
        self.assertEqual([first], PACKAGE.first_loaded([first, second], "Items", diagnostics))
        self.assertEqual(first["Source"], diagnostics[0]["Winner"])
        self.assertEqual(second["Source"], diagnostics[0]["Discarded"])

    def test_c_integer_division_is_applied_before_distribution_mean(self):
        distribution = PACKAGE.integer_distribution("(17 + dice(3, 4)) / 20")
        self.assertEqual({1}, set(distribution))
        self.assertEqual({-1: 1}, PACKAGE.integer_distribution("-7 / 4"))

    def test_generator_evaluator_rejects_executable_or_unreviewed_syntax(self):
        for text in ["__import__('os')", "dice(x, 4)", "2 ** 999", "dice(1000000, 6)"]:
            with self.subTest(text=text), self.assertRaises(ValueError):
                PACKAGE.integer_distribution(text)

    def test_generator_default_replaces_the_initial_scalar(self):
        source = """char_init_height(int race)
{
int ht = 72;
switch (race) {
case RACE_KANK:
ht = (84 + dice(2, 12)); break;
default: ht = (66 + dice(2, 8)); break;
}
return ht;
}
char_init_weight(int race)
{
int wt = 7;
switch (race) {
case RACE_KANK: wt = (700 + dice(2,100)) / 20; break;
default: wt = (180 + dice(6,10)) / 20; break;
}
return wt;
}
"""
        rows = PACKAGE.physical_generators(source, "#define RACE_KANK 29", [29, 999])
        self.assertEqual(97, rows[0]["Height"]["Mean"])
        self.assertEqual(75, rows[1]["Height"]["Mean"])
        self.assertGreater(rows[0]["Weight"]["Mean"], rows[1]["Weight"]["Mean"])


if __name__ == "__main__":
    unittest.main()
