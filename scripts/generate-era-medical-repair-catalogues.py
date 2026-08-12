#!/usr/bin/env python3
"""Generate the curated Renaissance and Early Modern medical catalogues.

Each row is a distinct implement, preparation, or repair kit.  The catalogue
intentionally does not clone the same item for an infirmary, college, fleet, or
other institution: builders can place the one well-described stock item where
it belongs.  Period differences are retained only when a form, material,
quality, or supplied component actually differs.
"""
from __future__ import annotations

import argparse
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT_R = ROOT / 'DatabaseSeeder/Seeders/MedicalRepairCatalogue/Renaissance/renaissance.medical-repair.tsv'
OUT_E = ROOT / 'DatabaseSeeder/Seeders/MedicalRepairCatalogue/EarlyModern/earlymodern.medical-repair.tsv'

# Forms are deliberately listed once.  A different delivery component, repair
# grade, or physically distinct regional medicine is a valid separate row;
# institutional ownership, colour, and imagined price tiers are not.
REN = {
 'Clinical surgery': ['barber surgeon case','folding lancet','cupping glass','trephine','bone saw','amputation knife','wound probe','bullet extractor','artery forceps','cautery iron','suture needle','surgical scissors','dental pelican','dental forceps','birthing stool','midwife hook','catheter','speculum','enema syringe','leech jar'],
 'Apothecary': ['albarello drug jar','labelled gallipot','powder paper','waxed medicine vial','stoppered cordial bottle','apothecary mortar','brass balance','nested weight set','pill tile','herb drying rack','distillation alembic','glass retort','drug sieve','ointment spatula','travelling medicine chest','syrup funnel','parchment dose packet','tincture flask','mummy powder jar','powdered pearl packet','sympathetic powder vial','weapon salve pot','astrological diagnostic glass'],
 'Drugs delivery': ['opium tincture bottle','antimony wine bottle','mercurial ointment pot','guaiacum decoction flask','sarsaparilla syrup bottle','sassafras tonic bottle','camphorated balm pot','myrrh tincture vial','benzoin fumigant cone','tobacco smoke roll','rosewater compress','aqua vitae wash bottle'],
 'Public health': ['pesthouse bed roll','quarantine flag','fumigation brazier','aromatic vinegar cloth','plague pomander','ward washstand','hospital screen','washable sickbed blanket','surgeon field roll','campaign medicine chest','dissection sheet','corpse shroud','sickroom chamber pot','fever compress','isolation placard','public pump key','royal-touch token'],
 'Mobility prosthesis': ['adjustable crutch','padded splint','fracture frame','arm sling','leg brace','truss belt','peg leg','wooden hand','hook hand','glass eye','neck support','field stretcher','casualty litter','drag harness','invalid chair'],
 'Veterinary': ['horse fleam','animal dosing horn','veterinary drench bottle','hoof poultice','animal splint','horse sling','worming ball','stable eye wash','cattle lancet','farrier medicine roll','livestock truss','animal bandage'],
 'Repair': ['rough clockwork repair kit','clockwork repair kit','fine clockwork repair kit','rough firearm repair kit','firearm repair kit','fine firearm repair kit','rough medical instrument repair kit','medical instrument repair kit','fine medical instrument repair kit','rough optical instrument repair kit','optical instrument repair kit','fine optical instrument repair kit','rough printing equipment repair kit','printing equipment repair kit','fine printing equipment repair kit','rough scientific instrument repair kit','scientific instrument repair kit','fine scientific instrument repair kit'],
 'Raw medical stock': ['chamomile stock','rosemary stock','ginger stock','cloves stock','cinnamon stock','camphor stock','benzoin resin stock','myrrh resin stock','opium gum stock','guaiacum wood stock','alum stock','Peruvian balsam stock','rhubarb root stock','senna leaf stock','sarsaparilla root stock','sassafras bark stock','turpentine resin stock']
}

EAR = {
 'Clinical surgery': REN['Clinical surgery'] + ['spring fleam','scarificator','tooth key','tourniquet','variolation lancet','inoculation scarifier','watch-spring artery forceps'],
 'Apothecary pharmacy': ['partitioned drug jar','porcelain gallipot','graduated medicine glass','stoppered tincture bottle','powder drawer','apothecary mortar','brass balance','nested weight set','pill rolling board','pill tile','glass retort','distillation alembic','chemical receiver','filter funnel','drug sieve','ointment spatula','prescription ledger','travelling medicine chest','syrup ladle','drug press','medicine chest drawer','pharmacy label case','cinchona bark stock','ipecacuanha root stock','Peruvian balsam stock','senna leaf stock','rhubarb root stock','Epsom salts stock','calomel stock','tartar emetic stock','bezoar stock','ergot stock','opium gum stock','benzoin resin stock','camphor stock','myrrh resin stock','turpentine resin stock','sarsaparilla root stock','sassafras bark stock','guaiacum wood stock','mummy powder jar','powdered pearl packet','sympathetic powder vial','weapon salve pot','astrological diagnostic glass'],
 'Drugs delivery': ['opium tincture bottle','antimony wine bottle','mercurial ointment pot','guaiacum decoction flask','sarsaparilla syrup bottle','sassafras tonic bottle','camphorated balm pot','myrrh tincture vial','benzoin fumigant cone','tobacco smoke roll','rosewater compress','aqua vitae wash bottle','laudanum bottle','Jesuit bark tonic flask','ipecacuanha syrup bottle',"Dover's powder paper",'paregoric elixir bottle','tartar emetic solution bottle','calomel purge packet','senna infusion flask','rhubarb tincture vial','Peruvian balsam salve pot','turpentine liniment bottle','hartshorn spirit smelling vial','Epsom salts draught bottle',"Daffy's elixir bottle","Godfrey's cordial bottle"],
 'Public health': REN['Public health'],
 'Mobility prosthesis': REN['Mobility prosthesis'] + ['articulated knee','hearing trumpet'],
 'Veterinary': REN['Veterinary'],
 'Repair': REN['Repair']
}

REN_FORMS = {form.casefold() for forms in REN.values() for form in forms}
EARLY_MODERN_ADDITIONS = {
	category: [form for form in forms if form.casefold() not in REN_FORMS]
	for category, forms in EAR.items()
}

DRUG_COMPONENTS = {
 'opium tincture':'LContainer_Medicine_Opium_Tincture_100ml', 'antimony wine':'LContainer_Medicine_Antimony_Wine_100ml',
 'mercurial ointment':'TopicalCream_Mercurial_Ointment', 'guaiacum decoction':'LContainer_Medicine_Guaiacum_Decoction_100ml',
 'sarsaparilla syrup':'LContainer_Medicine_Sarsaparilla_Syrup_100ml', 'sassafras tonic':'LContainer_Medicine_Sassafras_Tonic_100ml',
 'camphorated balm':'TopicalCream_Camphorated_Balm', 'myrrh tincture':'TopicalCream_Myrrh_Tincture',
 'benzoin fumigant':'Smokeable_Benzoin_Fumigant', 'tobacco smoke':'Smokeable_Tobacco_Smoke', 'rosewater compress':'TopicalCream_Rosewater_Compress',
 'aqua vitae wash':'TopicalCream_Aqua_Vitae_Wash', 'laudanum':'LContainer_Medicine_Laudanum_100ml', 'jesuit bark tonic':'LContainer_Medicine_Jesuit_Bark_Tonic_100ml',
 'ipecacuanha syrup':'LContainer_Medicine_Ipecacuanha_Syrup_100ml', "dover's powder":'LContainer_Medicine_Dover_s_Powder_100ml',
 'paregoric elixir':'LContainer_Medicine_Paregoric_Elixir_100ml', 'tartar emetic solution':'LContainer_Medicine_Tartar_Emetic_100ml',
 'calomel purge':'Pill_Calomel_Purge', 'senna infusion':'LContainer_Medicine_Senna_Infusion_100ml', 'rhubarb tincture':'LContainer_Medicine_Rhubarb_Tincture_100ml',
 'peruvian balsam salve':'TopicalCream_Peruvian_Balsam_Salve', 'turpentine liniment':'TopicalCream_Turpentine_Liniment',
 'hartshorn spirit':'Smokeable_Hartshorn_Spirit', 'epsom salts draught':'LContainer_Medicine_Epsom_Salts_Draught_100ml',
 "daffy's elixir":'LContainer_Medicine_Daffy_s_Elixir_100ml', "godfrey's cordial":'LContainer_Medicine_Godfrey_s_Cordial_100ml'
}

def slug(text):
 return ''.join(ch if ch.isalnum() else '_' for ch in text.lower()).strip('_').replace('__','_')

def article(text): return ('an ' if text[:1].lower() in 'aeiou' else 'a ') + text

def repair_component(form):
 family = next(x for x in ('clockwork','firearm','medical instrument','optical instrument','printing equipment','scientific instrument') if x in form)
 component = 'Repair_' + family.title().replace(' ', '_')
 return component + ('_Poor' if form.startswith('rough ') else '_Good' if form.startswith('fine ') else '')

def material_and_profile(category, form):
 words = form.lower()
 material, size, weight, cost = 'linen', 'Small', 250.0, 12.0
 if category == 'Repair': return 'leather', 'Small', 950.0, 35.0
 if 'mummy powder' in words: return 'bone', size, 35.0, 24.0
 if 'powdered pearl' in words: return 'pearl', size, 20.0, 85.0
 if 'sympathetic powder' in words: return 'green vitriol', size, 30.0, 18.0
 if 'weapon salve' in words: return 'beeswax', size, 90.0, 20.0
 if 'astrological diagnostic glass' in words: return 'glass', size, 210.0, 42.0
 if 'royal-touch token' in words: return 'silver', size, 18.0, 50.0
 if words.endswith(' stock'): return form[:-6], size, 90.0, 5.0
 if any(x in words for x in ('lancet','fleam','scarificator','trephine','bone saw','knife','probe','extractor','forceps','cautery','needle','scissors','pelican','tooth key','catheter','speculum','syringe','pump key')): return 'wrought iron', size, 180.0, 20.0
 if any(x in words for x in ('glass','vial','bottle','flask','retort','alembic','receiver','funnel','gallipot','jar')): return 'glass', size, 160.0, 18.0
 if any(x in words for x in ('case','chest','drawer','roll','rack','stool','frame','chair','litter','stretcher','stand','screen')): return ('leather' if any(x in words for x in ('case','roll')) else 'oak'), 'Normal', 1800.0, 48.0
 if any(x in words for x in ('splint','sling','shroud','blanket','compress','cloth','bandage')): return 'linen', size, 420.0, 9.0
 if 'tourniquet' in words: return 'leather', size, 150.0, 12.0
 if 'brazier' in words: return 'bronze', 'Normal', 2600.0, 38.0
 if 'pomander' in words: return 'silver', size, 90.0, 65.0
 if 'placard' in words: return 'oak', size, 600.0, 8.0
 if 'chamber pot' in words: return 'ceramic', size, 1100.0, 10.0
 if 'horn' in words: return 'horn', size, 180.0, 10.0
 if 'worming ball' in words: return 'herb', size, 35.0, 7.0
 if any(x in words for x in ('crutch','peg leg','brace','wooden hand','hook hand','hearing trumpet')): return ('brass' if 'trumpet' in words else 'oak'), 'Normal', 1250.0, 44.0
 if 'glass eye' in words: return 'glass', size, 15.0, 55.0
 if 'tobacco' in words: return 'tobacco leaf', size, 35.0, 8.0
 if 'benzoin' in words: return 'benzoin resin', size, 45.0, 14.0
 if 'hartshorn' in words: return 'horn', size, 35.0, 12.0
 if any(x in words for x in ('balm','ointment','liniment')): return 'beeswax', size, 95.0, 16.0
 if any(x in words for x in ('paper','packet')): return ('parchment' if 'parchment' in words else 'paper'), size, 35.0, 6.0
 if any(x in words for x in ('mortar',)): return 'granite', size, 1250.0, 16.0
 if any(x in words for x in ('balance','weight','spatula','sieve','press')): return 'brass', size, 420.0, 28.0
 if 'tile' in words: return 'ceramic', size, 240.0, 9.0
 return material, size, weight, cost

def components(category, form):
 if category == 'Repair': return ['Holdable','Destroyable_Misc',repair_component(form)]
 if category == 'Drugs delivery':
  component = next(value for stem, value in DRUG_COMPONENTS.items() if stem in form.lower())
  return ['Holdable','Destroyable_Misc',component]
 if category == 'Clinical surgery':
  if 'suture needle' in form: return ['Holdable','Destroyable_Misc','Suture_Single']
  if 'case' in form: return ['Holdable','Destroyable_Misc','FieldMedkit']
 if category == 'Public health':
  if 'aromatic vinegar cloth' in form: return ['Holdable','Destroyable_Misc','Antiseptic_Single']
  if 'fever compress' in form: return ['Holdable','Destroyable_Misc','Tend_Single']
  if any(x in form for x in ('field roll','medicine chest')): return ['Holdable','Destroyable_Misc','FieldMedkit']
 if category == 'Mobility prosthesis':
  if 'crutch' in form: return ['Holdable','Destroyable_WoodenHeavy','Crutch']
  if any(x in form for x in ('splint','brace','sling','truss','support')): return ['Holdable','Destroyable_Misc','Limb_Immobilising']
  if 'harness' in form: return ['Holdable','Destroyable_Misc','DragAid_Harness']
  if any(x in form for x in ('stretcher','litter')): return ['Holdable','Destroyable_Misc','DragAid_Stretcher']
  if any(x in form for x in ('peg leg','articulated knee')): return ['Holdable','Destroyable_WoodenHeavy','Prosthetic_LKnee']
  if any(x in form for x in ('wooden hand','hook hand')): return ['Holdable','Destroyable_Misc','Prosthetic_LHand_Functional']
  if 'glass eye' in form: return ['Holdable','Destroyable_Misc','Prosthetic_LEye']
 if category == 'Veterinary':
  if 'hoof poultice' in form: return ['Holdable','Destroyable_Misc','Tend_Single']
  if 'animal bandage' in form: return ['Holdable','Destroyable_Misc','Bandage_Simple']
  if any(x in form for x in ('animal splint','horse sling','livestock truss')): return ['Holdable','Destroyable_Misc','Limb_Immobilising']
  if 'farrier medicine roll' in form: return ['Holdable','Destroyable_Misc','Tend_Kit']
 return ['Holdable','Destroyable_Misc']

def tags(era, category, form):
 if category == 'Clinical surgery':
  function = 'Functions / Medical Treatment / Bloodletting' if any(x in form for x in ('lancet','fleam')) else 'Functions / Medical Treatment / Obstetric Aid' if any(x in form for x in ('birthing','midwife')) else 'Functions / Medical Treatment / Dental Treatment' if any(x in form for x in ('dental','tooth')) else surgical_tool_tag(form)
  market = 'Market / Medicine / Surgical Supplies'
 elif category.startswith('Apothecary') or category == 'Raw medical stock': market, function = 'Market / Medicine / Apothecary Goods', 'Functions / Medical Treatment / Chemical Remedy'
 elif category == 'Drugs delivery': market, function = 'Market / Medicine / Chemical Medicine', 'Functions / Medical Treatment / Chemical Remedy'
 elif category == 'Public health': market, function = 'Market / Medicine / Public Health', 'Functions / Medical Treatment / Quarantine' if any(x in form for x in ('quarantine','isolation','pesthouse')) else 'Functions / Medical Treatment / Public Health'
 elif category == 'Mobility prosthesis': market, function = 'Market / Medicine / Prosthetics and Mobility', 'Functions / Medical Treatment / Mobility Aid'
 elif category == 'Veterinary': market, function = 'Market / Medicine / Veterinary Medicine', 'Functions / Medical Treatment / Veterinary Treatment'
 else:
  market = 'Market / Repair Supplies / Precision Repair Supplies'
  family = next(x for x in ('Clockwork','Firearm','Medical Instrument','Optical Instrument','Printing Equipment','Scientific Instrument') if x.lower() in form)
  function = 'Functions / Repairing / ' + family
 if form.endswith(' stock'): market = 'Market / Medicine / Herbal Medicine'
 if any(x in form for x in ('astrological diagnostic glass','royal-touch token')): function = 'Functions / Medical Treatment / Diagnostic Prop'
 return [f'Era / {era} Era', market, function]

def surgical_tool_tag(form):
 if 'forceps' in form: return 'Functions / Tools / Surgical Tools / Forceps'
 if 'knife' in form: return 'Functions / Tools / Surgical Tools / Scalpel'
 if 'probe' in form: return 'Functions / Tools / Surgical Tools / Surgical Probe'
 if 'needle' in form: return 'Functions / Tools / Surgical Tools / Surgical Suture Needle'
 if 'speculum' in form: return 'Functions / Tools / Surgical Tools / Speculum'
 if 'cautery' in form: return 'Functions / Tools / Surgical Tools / Cautery Iron'
 if 'bone saw' in form: return 'Functions / Tools / Surgical Tools / Bonesaw'
 return 'Functions / Tools / Surgical Tools'

def description(category, form, material):
 words = form.lower()
 if category == 'Repair':
  grade = 'rough-cut and plainly serviceable' if words.startswith('rough ') else 'carefully fitted, with bright finished edges' if words.startswith('fine ') else 'orderly and workmanlike'
  return f'This {form} is a {grade} leather roll secured by two wrap straps. Its labelled pockets hold small {material}-bound supplies sized for the named mechanism rather than a general assortment of tools. Creases at the fold and darkened strap holes show that it has been opened repeatedly at a bench.'
 if words.endswith(' stock'):
  stock = form[:-6]
  return f'This {stock} stock is kept as a small, unprepared apothecary measure rather than a finished medicine. Its {material} body is visibly sorted into dry pieces, grains, or resinous fragments whose colour and smell distinguish it from neighbouring jars. A tied paper label leaves room for a dispenser to record a source or dose without claiming a cure.'
 if category == 'Drugs delivery':
  return f'This {form} is prepared as a distinct ready-to-use remedy rather than loose pharmacy stock. The {material} container or wrapper is sized for a single course, with its closure arranged to keep the preparation from spilling or drying out. Its surface bears handling marks around the lid, stopper, or fold where a user would open it.'
 if any(x in words for x in ('lancet','fleam','knife','saw','forceps','needle','scissors','probe','trephine','cautery','pelican','tooth key','catheter','speculum','syringe','extractor')):
  return f'This {form} has a compact {material} working end and a grip shaped to keep the hand clear of the task. Its edges, jaws, or point are deliberately exposed enough to show how it is used, while the remaining surfaces are plain and easy to wipe clean. Fine scratches concentrate around the moving joint or tip rather than being scattered as decorative wear.'
 if any(x in words for x in ('crutch','splint','brace','sling','truss','peg leg','hand','eye','stretcher','litter','harness','chair','frame','support')):
  return f'This {form} is built from {material} in broad, practical members that spread weight across the body or a bearer. Its contact points are padded, bound, or rounded where they would otherwise chafe, while the supporting structure remains visibly sturdy. Scuffs and polished edges make its intended direction of use immediately legible.'
 if category == 'Apothecary' or category == 'Apothecary pharmacy':
  return f'This {form} is an apothecary working piece made chiefly from {material}. Its shape makes the contents, measure, or working surface easy to inspect, with a deliberately plain finish that will not hide residue. Small stains, powder traces, or rubbed corners give it the settled look of a tool kept within reach of a dispensing bench.'
 if category == 'Public health':
  return f'This {form} is made chiefly from {material} for visible, repeated use in a shared sickroom or street. Its proportions favour clear handling and easy recognition over ornament, whether it is hung, spread, carried, or set beside a bed. The exposed surfaces show practical wear at the places where attendants would grip, fold, or clean it.'
 if category == 'Veterinary':
  return f'This {form} is made from {material} in a larger, sturdier pattern suited to animals that do not hold still. Its working parts are simple enough to be managed with gloved or dirty hands, and its corners are softened where they meet hide, hoof, or tack. Old rub marks and dried staining make it look like stable equipment rather than a human surgical tool.'
 return f'This {form} is made chiefly from {material} and has a practical, period-appropriate form. Its silhouette is clear at a glance, with the useful end, opening, or bearing surface left unobscured by decoration. Wear is concentrated on the areas a handler would touch, lift, or set down.'

def rows(era, catalogue):
 prefix = 'renaissance' if era == 'Renaissance' else 'earlymodern'
 result = []
 for category, forms in catalogue.items():
  for form in forms:
   material, size, weight, cost = material_and_profile(category, form)
   row_components = components(category, form)
   quality = 'Poor' if form.startswith('rough ') else 'Good' if form.startswith('fine ') else 'Standard'
   ref = f'{prefix}_{"repair" if category == "Repair" else "drug" if category == "Drugs delivery" else "medical"}_{slug(form)}'
   noun = form.split()[-1]
   result.append((ref, noun, article(form), description(category, form, material), size, quality, weight, cost, material, tags(era, category, form), row_components, category))
 return result

def render(era, catalogue):
 specs = rows(era, catalogue)
 refs, descriptions = [x[0] for x in specs], [x[3] for x in specs]
 if len(refs) != len(set(refs)) or len(descriptions) != len(set(descriptions)): raise ValueError(f'{era} has duplicate references or descriptions')
 if any(x[3].count('.') != 3 for x in specs): raise ValueError(f'{era} descriptions must contain three sentences')
 if era == 'Renaissance' and any(any(x in row[0] for x in ('cinchona','ipecacuanha','variolation','inoculation','spring_fleam','scarificator','tooth_key','tourniquet','hearing_trumpet')) for row in specs): raise ValueError('Renaissance catalogue contains later forms')
 lines = ['stable_reference\tnoun\tshort_description\tfull_description\tsize\tquality\tweight_grams\tcost\tmaterial\ttags\tcomponents\tbuilder_notes\tcategory']
 for ref, noun, sdesc, desc, size, quality, weight, cost, material, rowtags, rowcomponents, category in specs:
  if len(rowtags) != len(set(rowtags)) or len(rowcomponents) != len(set(rowcomponents)): raise ValueError(f'{ref} repeats a tag or component')
  values = [ref,noun,sdesc,desc,size,quality,str(weight),str(cost),material,';'.join(rowtags),';'.join(rowcomponents),era+' medical catalogue; category '+category+'.',category]
  lines.append('\t'.join(values))
 return '\n'.join(lines)

def main():
 parser = argparse.ArgumentParser(); parser.add_argument('--check', action='store_true'); args = parser.parse_args()
 outputs = [(OUT_R, render('Renaissance', REN)), (OUT_E, render('Early Modern', EARLY_MODERN_ADDITIONS))]
 stale = [p for p, content in outputs if not p.exists() or p.read_text(encoding='utf-8') != content]
 if args.check:
  if stale: print('Generated medical manifest source is stale: ' + ', '.join(str(x.relative_to(ROOT)) for x in stale)); return 1
  return 0
 for path, content in outputs: path.write_text(content, encoding='utf-8', newline='\n')
 return 0

if __name__ == '__main__': raise SystemExit(main())
