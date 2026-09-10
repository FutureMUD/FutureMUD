"""Materialise reviewed editorial changes from preserved source, never alter handoff inputs."""
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
CORPUS = ROOT / 'Design Documents/Seeding/CultureSeederOriginalCorpus'
STAGING = ROOT / 'Design Documents/Verification/CultureSeeder_Redesign_Source_Staging.json'
OUTPUT = ROOT / 'DatabaseSeeder/Seeders/CultureSeeder/CultureToolkit/ReviewedSourceProse.json'
modules = ['earthantiquity', 'earthdarkagesandmedieval', 'earthrenaissanceeurope', 'earthrenaissanceworldexpansion']
rows = []


def add(module, entity, identity, field, before, after):
    if before != after:
        rows.append(dict(Module=module, Entity=entity, Identity=identity, Field=field, Original=before,
                         Replacement=after, Action='correct', Reason='Contemporary voice; original retained in source corpus. No new historical attestation.'))


specific = {
    'The Scanian dialect, historically Danish, though spoken in what is now southern Sweden, bridging Nordic varieties':
        'The Scanian dialect of the southern Scandinavian lands, bridging neighbouring Nordic varieties',
    'The Upper Carniolan dialect, centered around the area north of Ljubljana, historically influential in later literary tradition':
        'The Upper Carniolan dialect, centred around the area north of Ljubljana',
    'The Western Aramaic dialect, once widespread in the Levant but greatly diminished by the 16th century, still surviving in a few isolated communities':
        'The Western Aramaic dialect spoken in communities of the Levant',
    'The Sahidic dialect, historically spoken in Upper Egypt, once a literary standard before Bohairic':
        'The Sahidic dialect of Upper Egypt, with an established literary tradition',
    'The Zenaga dialect, spoken historically in parts of Mauritania and the southern Sahara':
        'The Zenaga dialect spoken in parts of the southern Sahara',
    'The Bartian dialect, one of the Western Baltic dialects of the Old Prussian language, spoken in the region historically associated with the Bartians':
        'The Bartian dialect, one of the Western Baltic dialects of the Prussian language, spoken in the lands of the Bartians',
    'The Cilician dialect, a Western variety historically associated with the Armenian Kingdom of Cilicia':
        'The Cilician dialect, a western Armenian variety associated with Cilicia',
}
phrases = {
    'Medieval ': '', 'medieval ': '', 'late-medieval migrants': 'migrants',
    'Renaissance cultural exchange': 'cultural exchange', 'Renaissance-era cultural exchanges': 'cultural exchanges',
    'cultural exchange and Renaissance influence': 'cultural exchange', 'Renaissance Europe': 'Europe',
}
name_guidance = {
    'Kushite names are seeded as single Meroitic and Napatan-style personal names, drawing heavily from royal names because those are the best-attested stock source.':
        'Choose a single Kushite personal name in a Meroitic or Napatan tradition.',
    "The matronym is a genitive form of the mother's personal name, included so female stock profiles do not draw lineage names from the male pool.":
        "The matronym identifies your mother through a genitive form of her personal name.",
    "The patronym is a genitive form of the father's personal name, used here as a compact stock approximation of the attested Celtic patronymic formulas.":
        "The patronym identifies your father through a genitive form of his personal name.",
    "Punic inscriptions commonly identify people by filiation. The seeded patronyms use the readable Ben- prefix as a stock 'child of' form.":
        'Your patronym identifies your parent.',
    'The family name is the gentilicium or clan name. Full Etruscan inscriptions may add patronymics, matronymics, or marriage identifiers, but this stock profile keeps the reusable core two-name form.':
        'The family name is the gentilicium or clan name.',
    'Anatolian names cover Lydian, Phrygian, Carian, Lycian, Cappadocian, and Hellenistic Anatolian stock names used by the antiquity item cultures.':
        'Choose a personal name from the Lydian, Phrygian, Carian, Lycian, Cappadocian or Hellenistic communities of Anatolia.',
    'Scythian and Sarmatian names use Greek and Latin source forms for Iranic steppe peoples around the Black Sea and Pontic-Caspian frontier.':
        'Choose a personal name from a Scythian or Sarmatian household of the Black Sea or Pontic-Caspian steppe.',
    'Numidian and Mauretanian names are drawn from the Libyco-Berber and North African royal and tribal naming horizon as it appears in Punic, Greek and Latin sources. Common masculine examples include Masinissa, Micipsa, Jugurtha, Adherbal, Hiempsal and Bocchus; feminine examples are necessarily broader and include Sophonisba, Eunoe, Masinissa-derived and tribal feminine forms suitable for the same setting.':
        'Choose a personal name from a Numidian or Mauretanian household.',
    'The same stock is shared by nobles, clergy, townspeople and many tenants.':
        'These names are used by nobles, clergy, townspeople and many tenants.',
    'the same name may appear in several transliterations': 'the same name may have several spellings',
}
for module in modules:
    tables = json.loads((CORPUS / f'{module}.json').read_text(encoding='utf-8-sig'))['Tables']
    language_names = {language['Id']: language['Name'] for language in tables['Language']}
    for accent in tables['Accent']:
        for field in ['Description', 'Suffix', 'VagueSuffix']:
            before = accent[field]
            after = specific.get(before, before)
            for old, new in phrases.items():
                after = after.replace(old, new)
            add(module, 'Accent', language_names[accent['LanguageId']] + ':' + accent['Name'], field, before, after)
    for culture in tables['NameCulture']:
        before = culture['Definition']
        after = before.replace('tenth-century Carpathian Basin', 'Carpathian Basin')
        after = after.replace('while Christian baptismal names become more prominent in later generations', 'and Christian baptismal names are also used')
        after = after.replace('fixed modern surname', 'fixed family name').replace('modern surname', 'family name')
        after = after.replace('Ancient Egyptian personal names are seeded as single names, many of them theophoric or royal-administrative names known from Egyptian and Graeco-Roman sources.',
                              'Choose a single personal name. Names may honour a deity or reflect royal and administrative traditions.')
        after = after.replace('Ancient Persian personal names here use the familiar Greek and Latin transliterations of Achaemenid and early Iranian names.',
                              'Choose a Persian personal name, using the spelling you want other people to use.')
        for old, new in name_guidance.items():
            after = after.replace(old, new)
        after = re.sub(r'\r?\n\r?\nYou can use the following generator if you are unsure: https://www\.fantasynamegenerators\.com/celtic-gaul-names\.php\r?\n#1Note:[^<]*?#0', '', after)
        add(module, 'NameCulture', culture['Name'], 'Definition', before, after)

appearance = '\n\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.'
ethnic = {
    'Achaean': 'The Achaeans are one of the four major Greek peoples, alongside the Aeolians, Ionians and Dorians. Their foundation myth, told by Hesiod, traces their name to Achaeus, son of Xuthus and brother of Ion. Xuthus is a son of Hellen, the mythical patriarch of the Hellenes.\n\nAchaean communities inhabit Achaea in the northern Peloponnese and maintain ties with settlements in southern Italy, including Kroton. They use a form of Doric speech.' + appearance,
    'Aeolian': 'The Aeolians are one of the four major Greek peoples, alongside the Achaeans, Dorians and Ionians. They trace their name to Aeolus, a son of Hellen, and speak Aeolic Greek.\n\nTheir communities have roots in Thessaly and extend through Boeotia and other Greek lands, including Aetolia, Locris, Corinth, Elis and Messinia. Traditions of migration connect them with Lesbos and the coast of Aeolis across the Aegean.' + appearance,
    'Ionian': 'The Ionians are one of the four major Greek peoples, alongside the Dorians, Aeolians and Achaeans. Ionian speech forms one of the major linguistic divisions of the Hellenic world.\n\nTheir foundation myth names Ion, son of Xuthus, as their ancestor. Traditions of displacement from Aigialeia connect them with Attica and with the Ionian towns on the coast of Asia Minor. Their towns, including Athens, are renowned for philosophy, art and public debate.' + appearance,
    'Dorian': 'The Dorians are one of the four major Greek peoples, alongside the Aeolians, Achaeans and Ionians. The Odyssey speaks of Dorians in Crete.\n\nDorian communities vary greatly in their way of life, from the populous trading city of Corinth, known for ornate art and architecture, to the military institutions of Sparta. Doric speech and shared traditions connect them, though common identity does not guarantee common allegiance in war.' + appearance,
}
for stage in json.loads(STAGING.read_text(encoding='utf-8-sig')):
    if stage['SourcePack'] != 'earthantiquity':
        continue
    for ethnicity in stage['Ethnicities']:
        if ethnicity['Name'] in ethnic:
            add(stage['SourcePack'], 'Ethnicity', ethnicity['Name'], 'ChargenBlurb',
                ethnicity['ChargenBlurb'], ethnic[ethnicity['Name']])
OUTPUT.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'{len(rows)} source-qualified editorial changes written; original files untouched.')
