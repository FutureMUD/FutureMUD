"""Source fidelity and conventional-default regressions for the historical exporter."""
import importlib.util
from pathlib import Path
import sys
import tempfile
import unittest
from unittest.mock import patch

SCRIPT = Path(__file__).resolve().parents[1] / "generate-item-seeder-outfit-manifests.py"
SPEC = importlib.util.spec_from_file_location("outfit_generator", SCRIPT)
generator = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = generator
SPEC.loader.exec_module(generator)


class HistoricalOutfitSourceTests(unittest.TestCase):
	def fixture(self, full='"A complete robe, with a (folded) hem."'):
		return ('new("medieval_test", "robe", "a robe", null, ' + full +
			', SizeCategory.Normal, ItemQuality.Standard, 300, 2.5m, true, false, '
			'"linen", ["Clothing", "Religious"], ["Holdable", "Variable_Colour"], null, null, null, null)')

	def read_fixture(self, text):
		with tempfile.TemporaryDirectory() as directory:
			path = Path(directory) / "source.cs"
			path.write_text(text, encoding="utf-8-sig")
			return generator.extract_create_item_calls(path, "new", preserve_authored=True)

	def test_complete_normal_and_verbatim_literals_are_preserved(self):
		for literal, expected in [
			('"A complete robe, with a (folded) hem."', 'A complete robe, with a (folded) hem.'),
			(r'"A \"fold\", a slash \\, and (stitches)."', 'A "fold", a slash \\, and (stitches).'),
			('@"A ""fold"", a slash \\, and (stitches)."', 'A "fold", a slash \\, and (stitches).'),
		]:
			with self.subTest(literal=literal):
				item = self.read_fixture(self.fixture(literal))["medieval_test"]
				self.assertEqual(expected, item.full_description)
				self.assertTrue(item.use_authored_full_description)
				self.assertEqual(("Holdable", "Variable_Colour"), item.components)
				self.assertEqual("2.5", item.cost)

	def test_duplicate_and_unterminated_sources_fail(self):
		with self.assertRaisesRegex(ValueError, "duplicate item source medieval_test"):
			self.read_fixture(self.fixture() + ",\n" + self.fixture())
		with self.assertRaisesRegex(ValueError, r"source.cs:2: unterminated new call"):
			self.read_fixture("\n" + self.fixture()[:-1])

	def test_shared_records_are_the_exact_medieval_first_definitions(self):
		shared = generator.extract_create_item_calls(generator.HISTORICAL_CLOTHING_SOURCE, "new", preserve_authored=True)
		self.assertEqual(35, len(shared))
		medieval = {key: value for key, value in shared.items() if key.startswith("medieval_")}
		self.assertEqual(33, len(medieval))
		merged = generator.medieval_first_definition_items()
		for key, value in medieval.items():
			self.assertEqual(value, merged[key])
			self.assertTrue(value.use_authored_full_description)
		with patch.object(generator, "extract_create_item_calls", return_value=medieval):
			with self.assertRaisesRegex(ValueError, "Duplicate direct/shared Medieval item source"):
				generator.medieval_first_definition_items()

	def test_explicit_entry_choices_override_conventions_without_mutating_source(self):
		default = generator.OutfitManifestItem("medieval_latin_amice")
		override = generator.OutfitManifestItem("medieval_latin_amice", load_arguments="colour=blue")
		unrelated = generator.OutfitManifestItem("unrelated", load_arguments="colour=green")
		outfit = generator.Outfit("example", "Example", "Example ensemble", (default, override, unrelated))
		rendered = "\n".join(generator.render_manifest_array("Example", [outfit]))
		for choice in ("colour=white", "colour=blue", "colour=green"):
			self.assertIn(f'LoadArguments = "{choice}"', rendered)
		self.assertEqual("", default.load_arguments)
		self.assertEqual("colour=blue", override.load_arguments)

	def test_generated_file_is_current_and_contains_authored_source_verbatim(self):
		output = generator.generate()
		self.assertTrue(generator.OUTPUT.read_text(encoding="utf-8-sig") == output, "Generated file is stale")
		shared = generator.extract_create_item_calls(generator.HISTORICAL_CLOTHING_SOURCE, "new", preserve_authored=True)
		for key, item in shared.items():
			if key.startswith("medieval_") and f'new("{key}", "{item.noun}",' in output:
				self.assertTrue(generator.cs(item.full_description) in output, key)

	def test_institutional_admissions_use_the_complete_authored_override(self):
		overrides = generator.renaissance_item_overrides()
		admissions = generator.renaissance_admission_items()
		keys = [key for key in overrides if key.startswith("renaissance_institution_") and key in admissions]
		self.assertEqual(5, len(keys))
		output = generator.generate()
		for key in keys:
			with self.subTest(key=key):
				expected = generator.item_from_9_cell_row(overrides[key])
				self.assertEqual(expected, admissions[key])
				self.assertTrue(expected.use_authored_full_description)
				self.assertIn("$colour", expected.full_description)
				self.assertEqual(1, expected.components.count("Variable_Colour"))
				self.assertTrue(generator.cs(expected.full_description) in output)
		self.assertNotIn("Variable_Colour", admissions["renaissance_institution_preaching_gown"].components)

	def test_alb_generic_catalogue_override_has_authored_prose_and_outfit_only_white(self):
		key = "renaissance_institution_liturgical_alb"
		item = generator.item_from_9_cell_row(generator.renaissance_item_overrides()[key])
		self.assertEqual("a long $colour linen alb", item.short_description)
		self.assertIn("Wear_Robe_Layer_0_5_NonBulky", item.components)
		self.assertIn("Variable_BasicColour", item.components)
		self.assertTrue(item.use_authored_full_description)
		self.assertIn(generator.cs(item.full_description), generator.generate())
		self.assertEqual("colour=white", generator.HISTORICAL_OUTFIT_COLOUR_DEFAULTS[key])

	def test_conflicting_institutional_overrides_fail_with_source_location(self):
		row = generator.renaissance_item_overrides()["renaissance_institution_academic_robe"]
		original = "| " + " | ".join(row) + " |"
		conflicting = original.replace("1200.0g", "1300.0g")
		self.assertNotEqual(original, conflicting)
		with tempfile.TemporaryDirectory() as directory:
			path = Path(directory) / "overrides.md"
			path.write_text(original + "\n" + conflicting, encoding="utf-8")
			with patch.object(generator, "RENAISSANCE_DOCS", (path,)):
				with self.assertRaisesRegex(ValueError, r"overrides.md:2: Conflicting Renaissance stock overrides"):
					generator.renaissance_item_overrides()

	def test_judicial_skin_preserves_authored_variable_prose_and_unset_quality(self):
		skin = next(skin for skin in generator.parse_documented_skins(generator.EARLY_MODERN_DOC)
			if skin.stable_reference == "earlymodern_skin_judicial_full_sleeved_robe")
		self.assertEqual("null", skin.quality)
		self.assertIn("$colour", skin.short_description)
		self.assertIn("$colour", skin.full_description)
		rendered = "\n".join(generator.render_skin_array("Example", [skin]))
		self.assertIn(generator.cs(skin.full_description) + ", null)", rendered)
		self.assertNotIn("ItemQuality.null", rendered)

	def test_outfit_specific_default_precedence_does_not_leak_to_other_outfits(self):
		key = "renaissance_institution_academic_robe"
		default = generator.OutfitManifestItem(key)
		judicial = generator.Outfit("earlymodern_outfit_0884", "Judicial", "Example", (default,))
		rendered = "\n".join(generator.render_manifest_array("Example", [judicial]))
		self.assertIn('LoadArguments = "colour=black"', rendered)
		other = generator.replace(judicial, key="other_outfit")
		self.assertNotIn("LoadArguments", "\n".join(generator.render_manifest_array("Example", [other])))
		explicit = generator.replace(judicial, items=(generator.replace(default, load_arguments="colour=blue"),))
		rendered = "\n".join(generator.render_manifest_array("Example", [explicit]))
		self.assertIn('LoadArguments = "colour=blue"', rendered)
		self.assertNotIn("colour=black", rendered)
		self.assertEqual("", default.load_arguments)


if __name__ == "__main__":
	unittest.main()
