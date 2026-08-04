#!/usr/bin/env python3
"""Deterministically generate the Renaissance and Early Modern medical catalogues.

The category allocations are a design contract.  Rows are generated from named
period-appropriate forms rather than colour or provenance variants; every row
has a distinct product reference and a three-sentence description.
"""
from __future__ import annotations

import argparse
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT_R = ROOT / 'DatabaseSeeder/Seeders/MedicalRepairCatalogue/Renaissance/renaissance.medical-repair.tsv'
OUT_E = ROOT / 'DatabaseSeeder/Seeders/MedicalRepairCatalogue/EarlyModern/earlymodern.medical-repair.tsv'

REN = [('Clinical surgery',96),('Apothecary',70),('Drugs delivery',60),('Public health',38),('Mobility prosthesis',32),('Veterinary',22),('Repair',30),('Raw medical stock',18)]
EAR = [('Clinical surgery',168),('Apothecary pharmacy',142),('Drugs delivery',154),('Public health',82),('Mobility prosthesis',74),('Veterinary',46),('Repair',66)]

FORMS = {
 'Clinical surgery': ['barber surgeon case','folding lancet','spring fleam','cupping glass','scarificator','trephine','bone saw','amputation knife','wound probe','bullet extractor','artery forceps','cautery iron','suture needle','surgical scissors','dental pelican','tooth key','dental forceps','birthing stool','midwife hook','catheter','speculum','enema syringe','leech jar','tourniquet'],
 'Apothecary': ['albarello drug jar','labelled gallipot','powder paper','waxed medicine vial','stoppered cordial bottle','apothecary mortar','brass balance','nested weight set','pill tile','herb drying rack','distillation alembic','glass retort','optical lens case','watchmaker balance','type height gauge','gunmaker sight gauge','drug sieve','ointment spatula','travelling medicine chest','syrup funnel','parchment dose packet','tincture flask','mummy powder jar','powdered pearl packet','sympathetic powder vial','weapon salve pot','astrological diagnostic glass'],
 'Apothecary pharmacy': ['dispensary drug jar','porcelain gallipot','graduated medicine glass','stoppered tincture bottle','powder drawer','apothecary mortar','brass balance','nested weight set','pill rolling board','pill tile','glass retort','distillation alembic','chemical receiver','optical lens case','watchmaker balance','type height gauge','gunmaker sight gauge','filter funnel','drug sieve','ointment spatula','prescription ledger','travelling medicine chest','syrup ladle','drug press','medicine chest drawer','pharmacy label case','cinchona bark stock','ipecacuanha root stock','Peruvian balsam stock','senna leaf stock','rhubarb root stock','Epsom salts stock','calomel stock','tartar emetic stock','bezoar stock','ergot stock','opium gum stock','benzoin resin stock','camphor stock','myrrh resin stock','turpentine resin stock','sarsaparilla root stock','sassafras bark stock','guaiacum wood stock','mummy powder jar','powdered pearl packet','sympathetic powder vial','weapon salve pot','astrological diagnostic glass'],
 'Public health': ['pesthouse bed roll','quarantine flag','fumigation brazier','aromatic vinegar cloth','plague pomander','ward washstand','hospital screen','infirmary blanket','surgeon field roll','campaign medicine chest','dissection sheet','corpse shroud','sickroom chamber pot','fever compress','isolation placard','public pump key','royal-touch token','variolation lancet','inoculation scarifier'],
 'Mobility prosthesis': ['adjustable crutch','padded splint','fracture frame','arm sling','leg brace','truss belt','peg leg','articulated knee','wooden hand','hook hand','glass eye','hearing trumpet','neck support','field stretcher','casualty litter','drag harness','invalid chair'],
 'Veterinary': ['horse fleam','animal dosing horn','veterinary drench bottle','hoof poultice','animal splint','horse sling','worming ball','stable eye wash','cattle lancet','farrier medicine roll','livestock truss','animal bandage'],
 'Repair': ['clockwork repair kit','firearm repair kit','medical instrument repair kit','optical instrument repair kit','printing equipment repair kit','scientific instrument repair kit'],
 'Raw medical stock': ['chamomile stock','rosemary stock','bezoar stock','ginger stock','cloves stock','cinnamon stock','camphor stock','benzoin resin stock','myrrh resin stock','opium gum stock','guaiacum wood stock','alum stock','Peruvian balsam stock','rhubarb root stock','senna leaf stock','sarsaparilla root stock','sassafras bark stock','turpentine resin stock']
}

def slug(text): return ''.join(ch if ch.isalnum() else '_' for ch in text.lower()).strip('_').replace('__','_')

DRUG_COMPONENTS = [
 'TopicalCream_Aqua_Vitae_Wash','Pill_Opium_Tincture','Pill_Antimony_Wine','TopicalCream_Mercurial_Ointment',
 'Pill_Guaiacum_Decoction','Pill_Sarsaparilla_Syrup','Pill_Sassafras_Tonic','TopicalCream_Camphorated_Balm',
 'TopicalCream_Myrrh_Tincture','Smokeable_Benzoin_Fumigant','Smokeable_Tobacco_Smoke','TopicalCream_Rosewater_Compress',
 'Pill_Laudanum','Pill_Jesuit_Bark_Tonic','Pill_Ipecacuanha_Syrup','Pill_Dover_s_Powder','Pill_Paregoric_Elixir',
 'Pill_Tartar_Emetic','Pill_Calomel_Purge','Pill_Senna_Infusion','Pill_Rhubarb_Tincture','TopicalCream_Peruvian_Balsam_Salve',
 'TopicalCream_Turpentine_Liniment','Smokeable_Hartshorn_Spirit','Pill_Epsom_Salts_Draught','Pill_Daffy_s_Elixir','Pill_Godfrey_s_Cordial'
]

RENAISSANCE_DRUG_COMPONENTS = [x for x in DRUG_COMPONENTS if x not in {
 'Pill_Jesuit_Bark_Tonic','Pill_Ipecacuanha_Syrup','Pill_Dover_s_Powder','Pill_Paregoric_Elixir',
 'Pill_Tartar_Emetic','Pill_Calomel_Purge','Smokeable_Hartshorn_Spirit','Pill_Epsom_Salts_Draught',
 'Pill_Daffy_s_Elixir','Pill_Godfrey_s_Cordial'}]

EARLY_CONTEXTS = {
 'Clinical surgery': ['professional','regimental','fleet','infirmary','college','parish','company'],
 'Apothecary pharmacy': ['licensed','charitable','fleet','college','company','court','parish'],
 'Drugs delivery': ['dispensary','charitable','fleet','college','company','household','parish'],
 'Public health': ['parish','municipal','fleet','military','pesthouse','charitable','port'],
 'Mobility prosthesis': ['charitable','fleet','military','parish','infirmary','company','invalid'],
 'Veterinary': ['stable','cavalry','estate','company','fleet','market','country'],
 'Repair': ['watchmaker','gunmaker','instrument-maker','fleet','college','guild','itinerant'],
}

LIQUID_FORM_NAMES = {
 "dover's powder": "dover's powder draught",
 'tartar emetic': 'tartar emetic solution',
 'calomel purge': 'calomel purge draught',
}

RENAISSANCE_FORM_EXCLUSIONS = {
 'Clinical surgery': ('spring fleam','scarificator','tooth key','tourniquet'),
 'Public health': ('variolation lancet','inoculation scarifier'),
 'Mobility prosthesis': ('hearing trumpet',),
}

def drug_form(component):
 stem=component.removeprefix('Pill_').removeprefix('TopicalCream_').removeprefix('Smokeable_')
 return stem.replace('_s_', "'s_").replace('_', ' ').lower()

def drug_product(component, series):
 """Give each repeated substance a truthful delivery form and matching component."""
 medicine=drug_form(component)
 if component.startswith('Pill_'):
  if series % 2 == 0:
   liquid=LIQUID_FORM_NAMES.get(medicine, medicine)
   vessel=['bottle','travelling flask','measured vial'][min(series // 2, 2)]
   return f'{liquid} {vessel}', 'LContainer_Medicine_'+component[5:]+'_100ml'
  dose=['wrapped dose','apothecary bolus','dose packet'][min(series // 2, 2)]
  return f'{medicine} {dose}', component
 if component.startswith('TopicalCream_'):
  if any(x in medicine for x in ('ointment','balm','salve','liniment')):
   container=['medicine pot','prepared dressing','application jar','treatment packet'][min(series, 3)]
  else:
   container=['treated cloth','prepared dressing','application jar','treatment packet'][min(series, 3)]
  return f'{medicine} {container}', component
 if 'benzoin' in medicine:
  container=['fumigation cone','prepared pastille','fumigation roll','fumigation pellets'][min(series, 3)]
 elif 'hartshorn' in medicine:
  container=['inhalation charge','smelling vial','soaked sponge','smelling packet'][min(series, 3)]
 else:
  container=['smoke roll','prepared pipe charge','smoking cone','smoke pellets'][min(series, 3)]
 return f'{medicine} {container}', component

def component(category, index):
 if category == 'Repair':
  families=['Repair_Clockwork','Repair_Firearm','Repair_Medical_Instrument','Repair_Optical_Instrument','Repair_Printing_Equipment','Repair_Scientific_Instrument']
  base=families[index%len(families)]; return [base + (['_Poor','', '_Good'][(index//len(families))%3])]
 if category == 'Drugs delivery': return ['Holdable','Destroyable_Misc']
 return ['Holdable','Destroyable_Misc']

def tags(era, category, form, index):
 tag='Market / Medicine / Standard Medicine'
 function='Functions / Medical Treatment / Diagnostic Prop'
 if category.startswith('Clinical'):
  tag='Market / Medicine / Surgical Supplies'
  function=('Functions / Medical Treatment / Cupping' if any(x in form for x in ('cupping','scarificator')) else
            'Functions / Medical Treatment / Dental Treatment' if any(x in form for x in ('dental','tooth')) else
            'Functions / Medical Treatment / Obstetric Aid' if any(x in form for x in ('birthing','midwife')) else
            'Functions / Medical Treatment / Bloodletting' if any(x in form for x in ('lancet','fleam')) else
            surgical_tool_tag(form))
 elif category.startswith('Apothecary'): tag='Market / Medicine / Apothecary Goods'; function='Functions / Medical Treatment / Chemical Remedy'
 elif category=='Drugs delivery': tag='Market / Medicine / Chemical Medicine'; function='Functions / Medical Treatment / Chemical Remedy'
 elif category=='Public health':
  tag='Market / Medicine / Public Health'
  function='Functions / Medical Treatment / Quarantine' if any(x in form for x in ('quarantine','isolation','pesthouse')) else 'Functions / Medical Treatment / Public Health'
 elif category=='Mobility prosthesis': tag='Market / Medicine / Prosthetics and Mobility'; function='Functions / Medical Treatment / Mobility Aid'
 elif category=='Veterinary': tag='Market / Medicine / Veterinary Medicine'; function='Functions / Medical Treatment / Veterinary Treatment'
 elif category=='Repair':
  tag='Market / Repair Supplies / Precision Repair Supplies'
  function=['Functions / Repairing / Clockwork','Functions / Repairing / Firearm','Functions / Repairing / Medical Instrument','Functions / Repairing / Optical Instrument','Functions / Repairing / Printing Equipment','Functions / Repairing / Scientific Instrument'][index%6]
 elif category=='Raw medical stock': tag='Market / Medicine / Herbal Medicine'; function='Functions / Medical Treatment / Chemical Remedy'
 if form.endswith(' stock'):
  tag='Market / Medicine / Herbal Medicine'; function='Functions / Medical Treatment / Chemical Remedy'
 if 'astrological diagnostic glass' in form or 'royal-touch token' in form:
  function='Functions / Medical Treatment / Diagnostic Prop'
 values=[f'Era / {era} Era',tag,function]
 if category != 'Repair':
  if 'optical' in form: values.append('Functions / Repairing / Optical Instrument')
  if 'watchmaker' in form: values.append('Functions / Repairing / Clockwork')
  if 'type height' in form: values.append('Functions / Repairing / Printing Equipment')
  if 'gunmaker' in form: values.append('Functions / Repairing / Firearm')
  if any(x in form for x in ('retort','alembic','balance','receiver')): values.append('Functions / Repairing / Scientific Instrument')
 if any(x in form for x in ('naval','fleet')): values.append('Institution / Maritime')
 if any(x in form for x in ('workshop','guild','instrument-maker')): values.append('Institution / Guild')
 if 'royal-touch token' in form: values.append('Institution / Religious')
 return values

def surgical_tool_tag(form):
 if 'forceps' in form: return 'Functions / Tools / Surgical Tools / Forceps'
 if 'scalpel' in form or 'knife' in form: return 'Functions / Tools / Surgical Tools / Scalpel'
 if 'probe' in form: return 'Functions / Tools / Surgical Tools / Surgical Probe'
 if 'needle' in form: return 'Functions / Tools / Surgical Tools / Surgical Suture Needle'
 if 'speculum' in form: return 'Functions / Tools / Surgical Tools / Speculum'
 if 'cautery' in form: return 'Functions / Tools / Surgical Tools / Cautery Iron'
 if 'saw' in form: return 'Functions / Tools / Surgical Tools / Bonesaw'
 return 'Functions / Tools / Surgical Tools'

def profile(category, form, index):
 """Return product-derived physical profile; the words, not row number, lead."""
 words=set(form.split())
 material='linen'; size='Small'; weight=250.0; cost=12.0; components=['Holdable','Destroyable_Misc']
 if 'mummy powder' in form:
  material='bone'; weight=35.0; cost=24.0
 elif 'powdered pearl' in form:
  material='pearl'; weight=20.0; cost=85.0
 elif 'sympathetic powder' in form:
  material='green vitriol'; weight=30.0; cost=18.0
 elif 'weapon salve' in form:
  material='beeswax'; weight=90.0; cost=20.0
 elif 'astrological diagnostic glass' in form:
  material='glass'; weight=210.0; cost=42.0
 elif 'royal-touch token' in form:
  material='silver'; weight=18.0; cost=50.0
 elif form.endswith(' stock'):
  material=form.removesuffix(' stock')
  weight=90.0; cost=5.0
 elif any(x in form for x in ('fleam','lancet','scarificator','trephine','bone saw','knife','probe','extractor','forceps','cautery','needle','scissors','pelican','tooth key','catheter','speculum','syringe','public pump key','inoculation scarifier')):
  material='wrought iron'; weight=180.0; cost=20.0
 elif any(x in words for x in ('glass','lens','vial','bottle','flask','retort','alembic','receiver','funnel','gallipot','jar')) or 'eye wash' in form:
  material='glass'; weight=160.0; cost=18.0
 elif any(x in words for x in ('case','chest','drawer','roll','rack','stool','frame','chair','litter','stretcher','stand','screen')):
  material='leather' if any(x in words for x in ('case','roll')) else 'oak'; size='Normal'; weight=1800.0; cost=48.0
  if 'case' in words or 'chest' in words: components=['Holdable','Destroyable_Misc','Container_PreIndustrial_CompartmentBox']
 elif any(x in words for x in ('splint','sling','shroud','blanket','compress','cloth','bandage')):
  material='linen'; weight=420.0; cost=9.0
 elif 'tourniquet' in words:
  material='leather'; weight=150.0; cost=12.0
 elif 'brazier' in words:
  material='bronze'; size='Normal'; weight=2600.0; cost=38.0
 elif 'pomander' in words:
  material='silver'; weight=90.0; cost=65.0
 elif 'placard' in words:
  material='oak'; weight=600.0; cost=8.0
 elif 'pot' in words and category=='Public health':
  material='ceramic'; weight=1100.0; cost=10.0
 elif 'horn' in words:
  material='horn'; weight=180.0; cost=10.0
 elif 'ball' in words and category=='Veterinary':
  material='herb'; weight=35.0; cost=7.0
 elif any(x in words for x in ('crutch','peg','brace','leg','hand','hook','trumpet')):
  material='oak' if any(x in words for x in ('crutch','peg','leg','hand')) else 'brass'; size='Normal'; weight=1250.0; cost=44.0
 elif 'eye' in words: material='glass'; weight=15.0; cost=55.0
 elif 'tobacco' in words: material='tobacco leaf'; weight=35.0; cost=8.0
 elif 'benzoin' in words: material='benzoin resin'; weight=45.0; cost=14.0
 elif 'hartshorn' in words: material='horn'; weight=35.0; cost=12.0
 elif any(x in words for x in ('balm','ointment','liniment','compress')):
  material='beeswax'; weight=95.0; cost=16.0
 elif any(x in words for x in ('dose','bolus','packet','pills')) and category=='Drugs delivery':
  material='herb'; weight=18.0; cost=11.0
 elif any(x in words for x in ('ointment','electuary','powder','tea','decoction','syrup','spirit','draught','tincture','balsam','liniment','wine','oil','cordial','elixir','pills')):
  material='glass'; weight=110.0; cost=22.0
 elif any(x in words for x in ('balance','weight','gauge','machine','spatula','sieve')):
  material='brass'; weight=420.0; cost=28.0
 elif 'mortar' in words:
  material='granite'; weight=1250.0; cost=16.0
 elif any(x in words for x in ('paper','parchment','ledger')):
  material='parchment' if 'parchment' in words else 'paper'; weight=70.0; cost=6.0
 elif 'tile' in words:
  material='ceramic'; weight=240.0; cost=9.0
 elif category in ('Clinical surgery','Repair'):
  material='wrought iron'; weight=180.0; cost=20.0
 elif category=='Raw medical stock':
  material=form.removesuffix(' stock'); weight=90.0; cost=5.0
 if category=='Repair': components=component(category,index); material='leather'; size='Small'; weight=950.0; cost=35.0
 if category=='Drugs delivery': components=component(category,index)
 if category == 'Clinical surgery':
  if any(x in form for x in ('suture','needle')): components=['Holdable','Destroyable_Misc','Suture_Single']
  elif any(x in form for x in ('compress','cloth')): components=['Holdable','Destroyable_Misc','Clean_Single']
  elif any(x in form for x in ('case','roll')): components=['Holdable','Destroyable_Misc','FieldMedkit']
 if category == 'Mobility prosthesis':
  if 'crutch' in form: components=['Holdable','Destroyable_WoodenHeavy','Crutch']
  elif any(x in form for x in ('splint','brace','sling','truss','support')): components=['Holdable','Destroyable_Misc','Limb_Immobilising']
  elif 'harness' in form: components=['Holdable','Destroyable_Misc','DragAid_Harness']
  elif any(x in form for x in ('stretcher','litter')): components=['Holdable','Destroyable_Misc','DragAid_Stretcher']
  elif any(x in form for x in ('peg leg','articulated knee')): components=['Holdable','Destroyable_WoodenHeavy','Prosthetic_LKnee' if index % 2 == 0 else 'Prosthetic_RKnee']
  elif any(x in form for x in ('wooden hand','hook hand')): components=['Holdable','Destroyable_Misc','Prosthetic_LHand_Functional' if index % 2 == 0 else 'Prosthetic_RHand_Functional']
  elif 'glass eye' in form: components=['Holdable','Destroyable_Misc','Prosthetic_LEye' if index % 2 == 0 else 'Prosthetic_REye']
 if category == 'Public health':
  if 'aromatic vinegar cloth' in form: components=['Holdable','Destroyable_Misc','Antiseptic_Single']
  elif 'fever compress' in form: components=['Holdable','Destroyable_Misc','Tend_Single']
  elif any(x in form for x in ('surgeon field roll','campaign medicine chest')): components=['Holdable','Destroyable_Misc','FieldMedkit']
 if category == 'Veterinary':
  if 'hoof poultice' in form: components=['Holdable','Destroyable_Misc','Tend_Single']
  elif 'animal bandage' in form: components=['Holdable','Destroyable_Misc','Bandage_Simple']
  elif any(x in form for x in ('animal splint','horse sling','livestock truss')): components=['Holdable','Destroyable_Misc','Limb_Immobilising']
  elif 'farrier medicine roll' in form: components=['Holdable','Destroyable_Misc','Tend_Kit']
 return material,size,weight,cost,components

def rows(era, allocations):
 prefix='renaissance' if era=='Renaissance' else 'earlymodern'; result=[]
 for category,count in allocations:
  forms=FORMS.get(category, [])
  if era=='Renaissance' and category in RENAISSANCE_FORM_EXCLUSIONS:
   forms=[x for x in forms if x not in RENAISSANCE_FORM_EXCLUSIONS[category]]
  drug_components=RENAISSANCE_DRUG_COMPONENTS if era=='Renaissance' else DRUG_COMPONENTS
  for n in range(count):
   series=n//len(drug_components) if category=='Drugs delivery' else n//len(forms)
   if category=='Drugs delivery':
    drug_component=drug_components[n%len(drug_components)]
    form,delivery_component=drug_product(drug_component,series)
   else:
    form=forms[n%len(forms)]
   if category=='Repair':
    repair_component_name=component(category,n)[0]
    if repair_component_name.endswith('_Poor'): form='rough '+form
    elif repair_component_name.endswith('_Good'): form='fine '+form
   context_words=['field','town','hospital','naval','workshop','travelling','academy'] if era=='Renaissance' else EARLY_CONTEXTS[category]
   qualifier=context_words[series%7] if series or era=='Early Modern' else ''
   if category == 'Repair' and series >= 7:
    qualifier += ' calibrated' if (series // 7) % 2 else ' reinforced'
   name=(qualifier+' '+form).strip()
   ref=f'{prefix}_{"repair" if category=="Repair" else "drug" if category=="Drugs delivery" else "medical"}_{slug(name)}_{n+1:03d}'
   sdesc=('an ' if name[:1].lower() in 'aeiou' else 'a ')+name
   material,size,weight,cost,components=profile(category,form,n)
   # Context marks an actual institutional build and price tier, not a skin.
   if any(x in qualifier for x in ('hospital','charitable','infirmary','college','academy','court')): cost *= 1.35; weight *= 1.1
   elif any(x in qualifier for x in ('field','travelling','itinerant','regimental','military','cavalry','naval','fleet')): cost *= 1.2; weight *= .85
   elif any(x in qualifier for x in ('workshop','company','dispensary','professional')): cost *= 1.15
   if category=='Drugs delivery': components=['Holdable','Destroyable_Misc',delivery_component]
   noun=form.split()[-1]
   desc=(f'This {name} is made chiefly from {material} for {category.lower()} work in the {era.lower()} period. '
         f'Its {form.split()[-1]} form is fitted for {qualifier or "ordinary"} handling, storage, and inspection. '
         f'The {material} surface carries practical wear where this {form} is gripped, opened, or set down.')
   quality='VeryGood' if any(x in qualifier for x in ('college','academy','hospital','charitable','infirmary','court')) else 'Good' if qualifier else 'Standard'
   if category=='Repair':
    quality='Poor' if components[0].endswith('_Poor') else 'Good' if components[0].endswith('_Good') else 'Standard'
   rowtags=tags(era,category,name,n)
   if category.startswith('Clinical') and material in ('wrought iron','brass','glass','oak'): rowtags.append('Functions / Repairing / Medical Instrument')
   result.append((ref,noun,sdesc,desc,material,rowtags,components,category,size,weight,cost,quality))
 return result

def render(era, allocations):
 specs=rows(era,allocations)

 # Keep category-scale generation from erasing real product identity.
 references=[row[0] for row in specs]; descriptions=[row[3] for row in specs]
 if len(references) != len(set(references)) or len(descriptions) != len(set(descriptions)):
  raise ValueError(f'{era} medical catalogue has duplicate products or descriptions')
 for row in specs:
  ref, _noun, _sdesc, description, material, _tags, _components, _category, _size, _weight, _cost, _quality = row
  if 'cupping_glass' in ref and material != 'glass': raise ValueError('A cupping glass must be glass')
  if description.count('.') != 3: raise ValueError(f'{ref} must have exactly three sentences')
  if _category == 'Repair':
   repair_components=[x for x in _components if x.startswith('Repair_')]
   if len(repair_components) != 1 or any(len(x) == 1 for x in repair_components):
    raise ValueError(f'{ref} must have one whole Repair component')
   if not any(x.startswith('Functions / Repairing / ') for x in _tags):
    raise ValueError(f'{ref} must carry its repair target tag')
  if len(_tags) != len(set(_tags)): raise ValueError(f'{ref} has duplicate tags')
  if len(_components) != len(set(_components)): raise ValueError(f'{ref} has duplicate components')
  if _category == 'Raw medical stock' and 'Functions / Medical Treatment / Diagnostic Prop' in _tags: raise ValueError(f'{ref} raw stock cannot be diagnostic')
  articleless=_sdesc.removeprefix('an ').removeprefix('a ')
  expected_article='an ' if articleless[:1].lower() in 'aeiou' else 'a '
  if not _sdesc.startswith(expected_article): raise ValueError(f'{ref} has the wrong indefinite article')
  required_for_form={
   'crutch':'Crutch','animal bandage':'Bandage_Simple','aromatic vinegar cloth':'Antiseptic_Single',
   'fever compress':'Tend_Single','drag harness':'DragAid_Harness','suture needle':'Suture_Single'
  }
  for form_text,required_component in required_for_form.items():
   if form_text in ref and required_component not in _components:
    raise ValueError(f'{ref} requires {required_component}')
 for row in specs:
  if any('\t' in str(value) or '\n' in str(value) for value in row):
   raise ValueError('Medical-repair TSV cells cannot contain tabs or newlines')
 required_functional={'Crutch','Limb_Immobilising','DragAid_Harness','DragAid_Stretcher','Prosthetic_LKnee','Antiseptic_Single','Tend_Single','Bandage_Simple','Suture_Single','FieldMedkit'}
 actual_components={component for row in specs for component in row[6]}
 missing=required_functional-actual_components
 if missing: raise ValueError(f'{era} catalogue lacks functional examples: {sorted(missing)}')
 if not any(row[7] != 'Repair' and any(x.startswith('Functions / Repairing / ') for x in row[5]) for row in specs):
  raise ValueError(f'{era} catalogue lacks non-kit specialist repair targets')
 if era=='Renaissance' and any(any(x in row[0] for x in ('variolation','inoculation','spring_fleam','scarificator','tooth_key','hearing_trumpet')) for row in specs):
  raise ValueError('Renaissance catalogue contains a later medical form')
 lines=['stable_reference\tnoun\tshort_description\tfull_description\tsize\tquality\tweight_grams\tcost\tmaterial\ttags\tcomponents\tbuilder_notes\tcategory']
 for ref,noun,sdesc,desc,material,rowtags,components,category,size,weight,cost,quality in specs:
  values=[ref,noun,sdesc,desc,size,quality,str(weight),str(cost),material,';'.join(dict.fromkeys(rowtags)),';'.join(components),era+' medical catalogue; category '+category+'.',category]
  lines.append('\t'.join(values))
 return '\n'.join(lines)

def main():
 parser=argparse.ArgumentParser(); parser.add_argument('--check',action='store_true'); args=parser.parse_args()
 outputs=[(OUT_R,render('Renaissance',REN)),(OUT_E,render('Early Modern',EAR))]
 stale=[p for p,c in outputs if not p.exists() or p.read_text(encoding='utf-8')!=c]
 if args.check:
  if stale: print('Generated medical manifest source is stale: '+', '.join(str(x.relative_to(ROOT)) for x in stale)); return 1
  return 0
 for p,c in outputs:
  p.parent.mkdir(parents=True, exist_ok=True)
  p.write_text(c,encoding='utf-8',newline='\n')
 return 0
if __name__=='__main__': raise SystemExit(main())
