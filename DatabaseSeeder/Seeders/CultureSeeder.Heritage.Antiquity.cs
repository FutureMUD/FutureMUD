#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
    public void SeedRomanHeritage()
    {
        SeedRomanHeritageEthnicities();
        SeedRomanHeritageCultures();
    }

    private void SeedRomanHeritageEthnicities()
    {
        AddEthnicity(_humanRace, "Roman", "Italian", "O-A High Negative", 0, 0,
            description:
            "This ethnicity is for people born in the city of Rome itself; true born children of the eternal city. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Etruscan", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Etruscans are a people in north western Italia just north of Rome, with a proud history and unique language and culture. Etruscans are citizens of Rome. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Sabine", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Sabines are an Italic people who live in the Mountains to the east and north of Rome. They have long been citizens of Rome. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Oscan", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Oscans are an Italic people who live in the southeastern portion of mainland Italia. Their culture is known for lascivious festivals, games and plays. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Umbrian", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Umbrians are an Italic people in the central portion of Italia, south of Rome. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Samnite", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Samnites are a tribal, pastoral people who long resisted the Romans until conquered by Sulla. They have a distinctive culture, and are known to be very superstituous. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Latin", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Latins are of broadly the same tribal origins as the Romans, but lacking the prestige of being born in the eternal city. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Sardinian", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Sardinians are a largely rural people that live on the island of Sardinia. They have at various times been part of Carthaginian and now Roman empires. They have a distinctive culture and language but are thought of as quite rustic. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Corsican", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Corsicans are a largely rural people that live on the island of Corsica. They have at various times been part of Carthaginian and now Roman empires. They have a distinctive culture and language but are thought of as quite rustic. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Sicilian", "Italian", "O-A High Negative", 0, 0,
            description:
            "The Sicilians are the inhabitants of the province of Sicily. This island has long been a melting pot of Greek, Italian and Carthaginian peoples and cultures and is rich, fertile and very prosperous. They are typically characterised by olive skin, dark hair and dark eyes.");

        AddEthnicity(_humanRace, "Cisalpine Gaul", "Celtic", "Majority O Minor A", subgroup: "Cisalpine Gaul",
            tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Cisalpine gauls are gauls that live on the Italian side of the Alps. They include tribes such as the Ligones, Senones, Boii, Cenomani, Insubres, Lepontii, Taurini and Salassi. Most of these peoples are thoroughly romanised with few living in traditional Celtic manner. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Alpine Gaul", "Celtic", "Majority O Minor A", subgroup: "Cisalpine Gaul",
            tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Alpine gauls live in the Alps, between Italia and Gaul. They include tribes such as the Helvetii and Alobroges. They are a fierce people with a history of rebellion and defiance. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Aquitaine Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
            tempFloor: 0, tempCeiling: 0,
            description:
            "The Aquitanian Gauls are Transalpine gauls from Southwestern Gaul, bordering Iberia. Their lands are not especially fertile but productive silver and gold mines have driven urbanisation and trade in this area. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Celtic Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
            tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Celtic Gauls live in the province of Gallia Lugdunensis, also known as Celtica, in central Gaul. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Belgican Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
            tempFloor: 0, tempCeiling: 0,
            description:
            "The Belgica Gauls live in Belgica, in Northern Gaul. They include tribes like the Treveri, Mediomatrici, Leuci, Sequani and the eponymous Belgae. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Narbonese Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
            tempFloor: 0, tempCeiling: 0,
            description:
            "The Narbonese Gauls live in Gallia Narbonensis, the original province of Roman Transalpine Gaul. It had strong influence from a Greek colony Massalia and has been under Roman rule long enough to become fairly romanised. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Iberian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Iberians are a celtic people who live in the east of Iberia. They have long lived alongside Phoenicians, Greeks, Carthaginians and now Romans in their coastal cities. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Lusitanian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Lusitanians are a celtic people who live in the far west of the Iberian peninusla. Their lands are rich in gold and silver but this has been more of a curse than a blessing to their people, as they have fallen under the rule of successive empires who rule them brutally and export their wealth elsewhere. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Gallaecian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Gallacians are a celtic people who live in the northwest of the Iberian peninsular. They were a wealthy tribe and fierce allies of Carthage, but have been much reduced in both wealth and prestige under Roman rule. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Turdetanian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Turdetanians are a celtic people who live in the far south of the Iberian peninsular in the province of Hispania Baetica. Roman historian Strabo described them as the most civilised of all the peoples of Iberia, and they had long lived in an ordered society governed by written laws, with much cultural contact with the Greeks and Carthaginians. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Brittanic", "Celtic", "Majority O Minor A", subgroup: "Insular", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Brittanians are a celtic people who live in the province of Brittania. They are culturally close to to the Gauls but have a distinct language and religious structure. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Hibernian", "Celtic", "Majority O Minor A", subgroup: "Insular", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Hibernians are a celtic people who live on the island of Hibernia, to the west of Brittania. While they have contact with the Romans, they are beyond their Imperium. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
        AddEthnicity(_humanRace, "Caledonian", "Celtic", "Majority O Minor A", subgroup: "Insular", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Caledonians are a celtic people who belong to a tribal confederation north of Brittania that has come together to resist Roman expansion. They are a fierce and proud people and are considered by the Romans to be too warlike to occupy directly, so far from Rome. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");

        AddEthnicity(_humanRace, "Ingvaeon", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The Ingvaeones are a West Germanic cultural group living in the Northern Germania along the North Sea coast in the areas of Jutland, Holstein, and Frisia. Tribes in this area include the Angles, Frisii, Chauci, Saxons, and Jutes. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Istvaeon", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The Istvaeones are a West Germanic cultural group that is defined more by what they are not than what they are - generally speaking they are any group that is not Ingvaeones or Irminon. They occupy the eastern bank of the Rhine river in the more northerly parts of Germania. They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Irminon", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The Irminon are a West Germanic cultural group that lives in the interior of Germania, around the Elbe river. Notable tribes include the Chatti, Cherusci and Hermunduri They are typically characteristised by fair skin, fair hair and light eyes.");
        AddEthnicity(_humanRace, "Suebi", "Germanic", "O-A High Negative", 0, 0,
            description:
            "The Suebi are a West Germanic cultural group that lives on the farthest side of Germania from the Rhine, and included such tribes as the Marcomanni, Semnones, Langobards and Angles. They are typically characteristised by fair skin, fair hair and light eyes.");

        AddEthnicity(_humanRace, "Illyrian", "Illyrian", "O-A High Negative", 0, 0,
            description:
            "The Illyrians are a broad group of many different tribes in the province of Illyricum. They generally identify as their particular tribal grouping, but the Greeks and Romans both refer to them all by the grouping of Illyrian. They have been under Roman rule for a long time, but prior to Roman subjugation they had a reputation as excellent pirates and mercenaries. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Pannonian", "Illyrian", "O-A High Negative", 0, 0,
            description:
            "The Pannonians are an admixture of Illyrians and Celts, forming a distinct tribal identity. Pannonia is north of Dalmatia and borders the Danube, and so is an important frontier of the Roman empire. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Liburnian", "Illyrian", "O-A High Negative", 0, 0,
            description:
            "The Liburnians occupy a small portion of the northwestern coast of Illyrium in the Adriatic sea. They are a strong seafaring peoples and maintain large fleets of fishing and trading vessels, as well as being major contributors to Roman naval efforts. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Venetian", "Illyrian", "O-A High Negative", 0, 0,
            description:
            "The Venetians are an Illyrian-related tribe that occupy the western and northern coasts of the Adriatic north of Italia. They have been under Roman rule for a long time and are mostly latinised in the urban areas, but are still most famous for their fabulous horses. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Dacian", "Thracian", "O-A High Negative", 0, 0,
            description:
            "The Dacians, also called the Getae by the Greeks, are a barbarian people that live in and around the Carpathain mountains. In recent memory they had a kingdom that occupied most of Illyricum, but it has long since fallen. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
        AddEthnicity(_humanRace, "Thracian", "Thracian", "O-A High Negative", 0, 0,
            description:
            "The Thracians are a warlike barbarian people found both to the North of Greece and Macedon and on Anatolia as well. Their culture has a strong tradition of tattooing amongst both men and women. They are also known for being unsophisticated but having excellent music. They are typically characterised by fair skin, fair hair and either light or dark eyes.");

        AddEthnicity(_humanRace, "Scythian", "Scythian", "Majority O Minor A", 0, 0,
            description:
            "The Scythians are a renowned equestrian people originally from central asia who migrated into the Pontic Steppes and all around the Black Sea. They are divided into both sedentary populations of hellenised settlements, mostly along the silk road, and the larger bulk of patriarchal, nomadic pastoralists. The Scythians ride horses without saddles or stirrups. They are typically characterised by olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Sarmatian", "Scythian", "Majority O Minor A", 0, 0,
            description:
            "The Sarmatians are a large confederation of broadly Scythian tribes that nonetheless have a distinct cultural identity and exist in conflict with the traditional Scythian rulers. The Sarmatians are a more egalitarian society on gender grounds than the Scythians in general and women have more freedom and rights. The Sarmatians also ride horses with a saddle and stirrups. They are typically characterised by olive skin, dark hair and dark eyes.");

        AddEthnicity(_humanRace, "Achaean", "Hellenic", "O-A High Negative", subgroup: "Central Greek", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Achaeans were one of the four major tribes into which the Greeks divided themselves (along with the Aeolians, Ionians and Dorians). According to the foundation myth formalized by Hesiod, their name comes from Achaeus, the mythical founder of the Achaean tribe, who was supposedly one of the sons of Xuthus, and brother of Ion, the founder of the Ionian tribe. Xuthus was in turn the son of Hellen, the mythical patriarch of the Greek (Hellenic) nation.\n\nHistorically, the members of the Achaean tribe inhabited the region of Achaea in the northern Peloponnese. The Achaeans played an active role in the Greek colonization of southern Italy, founding the city of Kroton in 710 BC. The city was to gain fame later as the place where the Pythagorean School was founded. Unlike the other major tribes (Ionians, Dorians and Aeolians), the Achaeans did not have a separate dialect in the Classical period, instead using a form of Doric.\n\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Aeolian", "Hellenic", "O-A High Negative", subgroup: "Central Greek", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Aeolians were one of the four major tribes in which Greeks divided themselves in the ancient period (along with the Achaeans, Dorians and Ionians). Their name mythologically derives from Aeolus, the mythical ancestor of the Aeolians and son of Hellen, the mythical patriarch of the Greek nation. The dialect of ancient Greek they spoke is referred to as Aeolic.\n\nOriginating in Thessaly, a part of which was called Aeolis, the Aeolians often appear as the most numerous amongst the other Hellenic tribes of early times. The Boeotians, a subgroup of the Aeolians, were driven from Thessaly by the Thessalians and moved their location to Boeotia. Aeolian peoples were spread in many other parts of Greece such as Aetolia, Locris, Corinth, Elis and Messinia. During the Dorian invasion, Aeolians from Thessaly fled across the Aegean Sea to the island of Lesbos and the region of Aeolis, called as such after them, in Asia Minor.\n\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Ionian", "Hellenic", "O-A High Negative", subgroup: "Eastern Greek", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Ionians were one of the four major tribes that the Greeks considered themselves to be divided into during the ancient period; the other three being the Dorians, Aeolians, and Achaeans. The Ionian dialect was one of the three major linguistic divisions of the Hellenic world, together with the Dorian and Aeolian dialects.\n\nAccording to the foundation myth the Ionians were named after Ion, son of Xuthus, who lived in the north Peloponnesian region of Aigialeia. When the Dorians invaded the Peloponnese they expelled the Achaeans from the Argolid and Lacedaemonia. The displaced Achaeans moved into Aigialeia (thereafter known as Achaea), in turn expelling the Ionians from Aigialeia. The Ionians moved to Attica and mingled with the local population of Attica, and many later emigrated to the coast of Asia Minor founding the historical region of Ionia.\n\nUnlike the austere and militaristic Dorians, the Ionians are renowned for their love of philosophy, art, democracy, and pleasure – Ionian traits that were most famously expressed by the Athenians.\n\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Dorian", "Hellenic", "O-A High Negative", subgroup: "Western Greek", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Dorians were one of the four major ethnic groups into which the Hellenes (or Greeks) of Classical Greece divided themselves (along with the Aeolians, Achaeans, and Ionians). They are almost always referred to as just \"the Dorians\", as they are called in the earliest literary mention of them in the Odyssey, where they already can be found inhabiting the island of Crete.\\n\\nThey were diverse in way of life and social organization, varying from the populous trade center of the city of Corinth, known for its ornate style in art and architecture, to the isolationist, military state of Sparta. And yet, all Hellenes knew which localities were Dorian, and which were not. Dorian states at war could more likely, but not always, count on the assistance of other Dorian states. Dorians were distinguished by the Doric Greek dialect and by characteristic social and historical traditions.\\n\\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Italiote", "Hellenic", "O-A High Negative", subgroup: "Italian", tempFloor: 0,
            tempCeiling: 0,
            description:
            "Italiote Greeks are those who live in the Greek-ethnicity city states in former Magna Graecia, Sicilia, and northern Italia. Once powerful city states in their own right, they have long since been subsumed into the Roman Imperium but they do retain a distinct cultural identity. They are mostly of Dorian Greek descent. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Cypriot", "Hellenic", "O-A High Negative", subgroup: "Aegean Greek", tempFloor: 0,
            tempCeiling: 0,
            description:
            "Cypriots are natives of the island of Cyprus. They are descended from an Achaean diaspora. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Cretan", "Hellenic", "O-A High Negative", subgroup: "Aegean Greek", tempFloor: 0,
            tempCeiling: 0,
            description:
            "Cretans are natives of the island of Crete. They are a mixture of a pre-hellenic Minoan culture as well as later Greek immigrants to the island. They are a proud people with the cultural memory of two great empires - the Minoans and the Mycenaens. Now subjects of the Roman empire, they have nonetheless flourished as a centre of eastern Mediterranean commerce. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Macedonian", "Hellenic", "O-A High Negative", subgroup: "Western Greek", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Macedonians are a branch of Dorian Greeks that nonetheless have a distinct cultural identity, most notably because of their association with Alexander the Great. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Phrygian", "Hellenic", "O-A High Negative", subgroup: "Asian", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Phrygians are Hellenised peoples who live in central Anatolia, east of Greece proper. The Trojans were a tribe of Phrygians. These people spent a long period of time under Persian rule, before being conquered by Alexander the Great, then the Seleucids, then most recently Rome. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Anatolian", "Hellenic", "O-A High Negative", subgroup: "Asian", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Anatolians are a broad grouping of Hellenised peoples in coastal and eastern Anatolia. They descend from a variety of ancient kingdoms but now mostly draw their identity from the city states to which they belong. They are typically characterised by fair to olive skin, dark hair and dark eyes.");

        AddEthnicity(_humanRace, "Punic", "Punic", "Majority O", 0, 0,
            description:
            "The term Punic is used to refer to those Phoenician people of the western Mediterranean who descend from Carthage proper, Roman Provincia Africanus. Until recently, their people were the preeminent empire in the Mediterranean and an ancient enemy of Rome. Now, though much reduced, they still play a prominent role in the empire as traders and sailors. They are characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Libyan", "Punic", "Majority O", 0, 0,
            description:
            "The Libyans are a Punic people in the regions of North Africa to the Southeast of Carthage, including Tripolitania and Cyrenaica. They are used to intermixing with Greek city states, although they maintain a seperate Punic identity. They are characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Mauretanian", "Punic", "Majority O", 0, 0,
            description:
            "The Mauretanians are a Punic people in most of the western parts of North Africa, to the west of Numidia, and sometimes including the southern parts of Hispania. They have a great deal of cultural contact with their nomadic cousins in the more southerly regions and less contact with Greek city states. They are characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Numidian", "Punic", "Majority O", 0, 0,
            description:
            "The Numidians are a people in North Africa to the west of Carthage. They were a client state and ally of Carthage but during the Second Punic War they managed to unify into an independent Kingdom. They then became a client state of Rome until they were later annexed. They are characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Egyptian", "Egyptian", "Majority O", 0, 0,
            description:
            "The Egyptians are the native peoples of the land of Egypt; once a powerful empire that was one of the world's pre-eminent empires for thousands of years but now ruled by a Hellenised ruling class called the Ptolemaics. They are characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Berber", "North African", "Majority O", 0, 0,
            description:
            "The Berbers are the tribal, nomadic peoples of the interior of the province of Mauretania. They are related to most of the more civilised, sedentary Punic peoples on the coast but maintain a distinct cultural identity, even under Roman rule. They are characterised by olive or light brown skin, dark hair and light eyes.");

        AddEthnicity(_humanRace, "Arabian", "Levantine", "Majority O Minor A", 0, 0,
            description:
            "The Arabians are a broad grouping of both settled and nomadic peoples that live on the Arabian peninsular and in the Kingdom of Nabatea. Their peninsular is the gateway to the Indes and an enormous volume of trade passes through their lands, which are exceptionally wealthy on account of the commerce. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Syrian", "Levantine", "Majority O Minor A", 0, 0,
            description:
            "The Syrians are the Phoenician inhabitants of the Roman province of Syria, which sits south and east of Asia Minor and north and east of Egypt. The province is fabulously wealthy, with the trade of east and west coming through here. The Syrians are only recently additions to the Roman Empire, conquered by Pompey Magnus. The Syrian people are known for (to the Romans) exotic religious and cultural practices, and there is a great deal of fascination with the 'oriental'. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Judean", "Levantine", "Majority O Minor A", subgroup: "Semitic", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Judeans are a broad ethnic group including Judeans and Israelites. They have a strong sense of distinct cultural identity from their neighbours and follow a monotheistic god called Yahweh. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Samaritan", "Levantine", "Majority O Minor A", subgroup: "Semitic", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Samaritans are the people of the Kingdom of Samaria, akin to the Judeans but distinct from them. Unlike their neighbours they had gone through a period of Hellenisation after being Seleucid clients, though they retained their monotheistic religion. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
        AddEthnicity(_humanRace, "Gallilean", "Levantine", "Majority O Minor A", subgroup: "Semitic", tempFloor: 0,
            tempCeiling: 0,
            description:
            "The Gallileans are a Semitic people to the north of the Israelites. Of the various Semitic tribes, they are the most influenced by the Phoenician culture and also tend to be the most tolerant of other religions. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");


        AddEthnicityVariable("Cisalpine Gaul", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Cisalpine Gaul", "Ears", "All Ears");
        AddEthnicityVariable("Cisalpine Gaul", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Cisalpine Gaul", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Cisalpine Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Cisalpine Gaul", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Cisalpine Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Cisalpine Gaul", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Cisalpine Gaul", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Cisalpine Gaul", "Nose", "All Noses");
        AddEthnicityVariable("Cisalpine Gaul", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Cisalpine Gaul", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Alpine Gaul", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Alpine Gaul", "Ears", "All Ears");
        AddEthnicityVariable("Alpine Gaul", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Alpine Gaul", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Alpine Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Alpine Gaul", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Alpine Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Alpine Gaul", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Alpine Gaul", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Alpine Gaul", "Nose", "All Noses");
        AddEthnicityVariable("Alpine Gaul", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Alpine Gaul", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Aquitaine Gaul", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Aquitaine Gaul", "Ears", "All Ears");
        AddEthnicityVariable("Aquitaine Gaul", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Aquitaine Gaul", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Aquitaine Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Aquitaine Gaul", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Aquitaine Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Aquitaine Gaul", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Aquitaine Gaul", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Aquitaine Gaul", "Nose", "All Noses");
        AddEthnicityVariable("Aquitaine Gaul", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Aquitaine Gaul", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Celtic Gaul", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Celtic Gaul", "Ears", "All Ears");
        AddEthnicityVariable("Celtic Gaul", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Celtic Gaul", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Celtic Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Celtic Gaul", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Celtic Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Celtic Gaul", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Celtic Gaul", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Celtic Gaul", "Nose", "All Noses");
        AddEthnicityVariable("Celtic Gaul", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Celtic Gaul", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Belgican Gaul", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Belgican Gaul", "Ears", "All Ears");
        AddEthnicityVariable("Belgican Gaul", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Belgican Gaul", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Belgican Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Belgican Gaul", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Belgican Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Belgican Gaul", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Belgican Gaul", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Belgican Gaul", "Nose", "All Noses");
        AddEthnicityVariable("Belgican Gaul", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Belgican Gaul", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Narbonese Gaul", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Narbonese Gaul", "Ears", "All Ears");
        AddEthnicityVariable("Narbonese Gaul", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Narbonese Gaul", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Narbonese Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Narbonese Gaul", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Narbonese Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Narbonese Gaul", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Narbonese Gaul", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Narbonese Gaul", "Nose", "All Noses");
        AddEthnicityVariable("Narbonese Gaul", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Narbonese Gaul", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Iberian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Iberian", "Ears", "All Ears");
        AddEthnicityVariable("Iberian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Iberian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Iberian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Iberian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Iberian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Iberian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Iberian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Iberian", "Nose", "All Noses");
        AddEthnicityVariable("Iberian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Iberian", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Lusitanian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Lusitanian", "Ears", "All Ears");
        AddEthnicityVariable("Lusitanian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Lusitanian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Lusitanian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Lusitanian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Lusitanian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Lusitanian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Lusitanian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Lusitanian", "Nose", "All Noses");
        AddEthnicityVariable("Lusitanian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Lusitanian", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Gallaecian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Gallaecian", "Ears", "All Ears");
        AddEthnicityVariable("Gallaecian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Gallaecian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Gallaecian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Gallaecian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Gallaecian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Gallaecian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Gallaecian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Gallaecian", "Nose", "All Noses");
        AddEthnicityVariable("Gallaecian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Gallaecian", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Turdetanian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Turdetanian", "Ears", "All Ears");
        AddEthnicityVariable("Turdetanian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Turdetanian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Turdetanian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Turdetanian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Turdetanian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Turdetanian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Turdetanian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Turdetanian", "Nose", "All Noses");
        AddEthnicityVariable("Turdetanian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Turdetanian", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Brittanic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Brittanic", "Ears", "All Ears");
        AddEthnicityVariable("Brittanic", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Brittanic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Brittanic", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Brittanic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Brittanic", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Brittanic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Brittanic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Brittanic", "Nose", "All Noses");
        AddEthnicityVariable("Brittanic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Brittanic", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Hibernian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Hibernian", "Ears", "All Ears");
        AddEthnicityVariable("Hibernian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Hibernian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Hibernian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Hibernian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Hibernian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Hibernian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Hibernian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Hibernian", "Nose", "All Noses");
        AddEthnicityVariable("Hibernian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Hibernian", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Caledonian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Caledonian", "Ears", "All Ears");
        AddEthnicityVariable("Caledonian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Caledonian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Caledonian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Caledonian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Caledonian", "Hair Colour", "brown_blonde_red_grey_hair");
        AddEthnicityVariable("Caledonian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Caledonian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Caledonian", "Nose", "All Noses");
        AddEthnicityVariable("Caledonian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Caledonian", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Roman", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Roman", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Roman", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Roman", "Ears", "All Ears");
        AddEthnicityVariable("Roman", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Roman", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Roman", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Roman", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Roman", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Roman", "Nose", "All Noses");
        AddEthnicityVariable("Roman", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Roman", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Etruscan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Etruscan", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Etruscan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Etruscan", "Ears", "All Ears");
        AddEthnicityVariable("Etruscan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Etruscan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Etruscan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Etruscan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Etruscan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Etruscan", "Nose", "All Noses");
        AddEthnicityVariable("Etruscan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Etruscan", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Sabine", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Sabine", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Sabine", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Sabine", "Ears", "All Ears");
        AddEthnicityVariable("Sabine", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sabine", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Sabine", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sabine", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Sabine", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Sabine", "Nose", "All Noses");
        AddEthnicityVariable("Sabine", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Sabine", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Oscan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Oscan", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Oscan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Oscan", "Ears", "All Ears");
        AddEthnicityVariable("Oscan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Oscan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Oscan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Oscan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Oscan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Oscan", "Nose", "All Noses");
        AddEthnicityVariable("Oscan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Oscan", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Umbrian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Umbrian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Umbrian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Umbrian", "Ears", "All Ears");
        AddEthnicityVariable("Umbrian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Umbrian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Umbrian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Umbrian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Umbrian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Umbrian", "Nose", "All Noses");
        AddEthnicityVariable("Umbrian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Umbrian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Samnite", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Samnite", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Samnite", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Samnite", "Ears", "All Ears");
        AddEthnicityVariable("Samnite", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Samnite", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Samnite", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Samnite", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Samnite", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Samnite", "Nose", "All Noses");
        AddEthnicityVariable("Samnite", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Samnite", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Latin", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Latin", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Latin", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Latin", "Ears", "All Ears");
        AddEthnicityVariable("Latin", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Latin", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Latin", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Latin", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Latin", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Latin", "Nose", "All Noses");
        AddEthnicityVariable("Latin", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Latin", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Sardinian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Sardinian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Sardinian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Sardinian", "Ears", "All Ears");
        AddEthnicityVariable("Sardinian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sardinian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Sardinian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sardinian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Sardinian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Sardinian", "Nose", "All Noses");
        AddEthnicityVariable("Sardinian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Sardinian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Corsican", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Corsican", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Corsican", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Corsican", "Ears", "All Ears");
        AddEthnicityVariable("Corsican", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Corsican", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Corsican", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Corsican", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Corsican", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Corsican", "Nose", "All Noses");
        AddEthnicityVariable("Corsican", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Corsican", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Sicilian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Sicilian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Sicilian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Sicilian", "Ears", "All Ears");
        AddEthnicityVariable("Sicilian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sicilian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Sicilian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sicilian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Sicilian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Sicilian", "Nose", "All Noses");
        AddEthnicityVariable("Sicilian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Sicilian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Ingvaeon", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Ingvaeon", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Ingvaeon", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Ingvaeon", "Ears", "All Ears");
        AddEthnicityVariable("Ingvaeon", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Ingvaeon", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Ingvaeon", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Ingvaeon", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Ingvaeon", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Ingvaeon", "Nose", "All Noses");
        AddEthnicityVariable("Ingvaeon", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Ingvaeon", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Istvaeon", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Istvaeon", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Istvaeon", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Istvaeon", "Ears", "All Ears");
        AddEthnicityVariable("Istvaeon", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Istvaeon", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Istvaeon", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Istvaeon", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Istvaeon", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Istvaeon", "Nose", "All Noses");
        AddEthnicityVariable("Istvaeon", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Istvaeon", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Irminon", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Irminon", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Irminon", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Irminon", "Ears", "All Ears");
        AddEthnicityVariable("Irminon", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Irminon", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Irminon", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Irminon", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Irminon", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Irminon", "Nose", "All Noses");
        AddEthnicityVariable("Irminon", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Irminon", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Suebi", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Suebi", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Suebi", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Suebi", "Ears", "All Ears");
        AddEthnicityVariable("Suebi", "Facial Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Suebi", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Suebi", "Hair Colour", "brown_blonde_grey_hair");
        AddEthnicityVariable("Suebi", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Suebi", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Suebi", "Nose", "All Noses");
        AddEthnicityVariable("Suebi", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Suebi", "Skin Colour", "fair_skin");

        AddEthnicityVariable("Achaean", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Achaean", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Achaean", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Achaean", "Ears", "All Ears");
        AddEthnicityVariable("Achaean", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Achaean", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Achaean", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Achaean", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Achaean", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Achaean", "Nose", "All Noses");
        AddEthnicityVariable("Achaean", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Achaean", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Aeolian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Aeolian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Aeolian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Aeolian", "Ears", "All Ears");
        AddEthnicityVariable("Aeolian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Aeolian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Aeolian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Aeolian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Aeolian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Aeolian", "Nose", "All Noses");
        AddEthnicityVariable("Aeolian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Aeolian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Ionian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Ionian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Ionian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Ionian", "Ears", "All Ears");
        AddEthnicityVariable("Ionian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ionian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Ionian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Ionian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Ionian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Ionian", "Nose", "All Noses");
        AddEthnicityVariable("Ionian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Ionian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Dorian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Dorian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Dorian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Dorian", "Ears", "All Ears");
        AddEthnicityVariable("Dorian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Dorian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Dorian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Dorian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Dorian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Dorian", "Nose", "All Noses");
        AddEthnicityVariable("Dorian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Dorian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Italiote", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Italiote", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Italiote", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Italiote", "Ears", "All Ears");
        AddEthnicityVariable("Italiote", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Italiote", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Italiote", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Italiote", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Italiote", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Italiote", "Nose", "All Noses");
        AddEthnicityVariable("Italiote", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Italiote", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Cypriot", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Cypriot", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Cypriot", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Cypriot", "Ears", "All Ears");
        AddEthnicityVariable("Cypriot", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Cypriot", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Cypriot", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Cypriot", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Cypriot", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Cypriot", "Nose", "All Noses");
        AddEthnicityVariable("Cypriot", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Cypriot", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Cretan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Cretan", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Cretan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Cretan", "Ears", "All Ears");
        AddEthnicityVariable("Cretan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Cretan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Cretan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Cretan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Cretan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Cretan", "Nose", "All Noses");
        AddEthnicityVariable("Cretan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Cretan", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Macedonian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Macedonian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Macedonian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Macedonian", "Ears", "All Ears");
        AddEthnicityVariable("Macedonian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Macedonian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Macedonian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Macedonian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Macedonian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Macedonian", "Nose", "All Noses");
        AddEthnicityVariable("Macedonian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Macedonian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Phrygian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Phrygian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Phrygian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Phrygian", "Ears", "All Ears");
        AddEthnicityVariable("Phrygian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Phrygian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Phrygian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Phrygian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Phrygian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Phrygian", "Nose", "All Noses");
        AddEthnicityVariable("Phrygian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Phrygian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Anatolian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Anatolian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Anatolian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Anatolian", "Ears", "All Ears");
        AddEthnicityVariable("Anatolian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Anatolian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Anatolian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Anatolian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Anatolian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Anatolian", "Nose", "All Noses");
        AddEthnicityVariable("Anatolian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Anatolian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Illyrian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Illyrian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Illyrian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Illyrian", "Ears", "All Ears");
        AddEthnicityVariable("Illyrian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Illyrian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Illyrian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Illyrian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Illyrian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Illyrian", "Nose", "All Noses");
        AddEthnicityVariable("Illyrian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Illyrian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Pannonian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Pannonian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Pannonian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Pannonian", "Ears", "All Ears");
        AddEthnicityVariable("Pannonian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Pannonian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Pannonian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Pannonian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Pannonian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Pannonian", "Nose", "All Noses");
        AddEthnicityVariable("Pannonian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Pannonian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Liburnian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Liburnian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Liburnian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Liburnian", "Ears", "All Ears");
        AddEthnicityVariable("Liburnian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Liburnian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Liburnian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Liburnian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Liburnian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Liburnian", "Nose", "All Noses");
        AddEthnicityVariable("Liburnian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Liburnian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Venetian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Venetian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Venetian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Venetian", "Ears", "All Ears");
        AddEthnicityVariable("Venetian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Venetian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Venetian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Venetian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Venetian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Venetian", "Nose", "All Noses");
        AddEthnicityVariable("Venetian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Venetian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Dacian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Dacian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Dacian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Dacian", "Ears", "All Ears");
        AddEthnicityVariable("Dacian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Dacian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Dacian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Dacian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Dacian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Dacian", "Nose", "All Noses");
        AddEthnicityVariable("Dacian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Dacian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Thracian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Thracian", "Eye Colour", "brown_green_blue_eyes");
        AddEthnicityVariable("Thracian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Thracian", "Ears", "All Ears");
        AddEthnicityVariable("Thracian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Thracian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Thracian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Thracian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Thracian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Thracian", "Nose", "All Noses");
        AddEthnicityVariable("Thracian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Thracian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Punic", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Punic", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Punic", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Punic", "Ears", "All Ears");
        AddEthnicityVariable("Punic", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Punic", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Punic", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Punic", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Punic", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Punic", "Nose", "All Noses");
        AddEthnicityVariable("Punic", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Punic", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Libyan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Libyan", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Libyan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Libyan", "Ears", "All Ears");
        AddEthnicityVariable("Libyan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Libyan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Libyan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Libyan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Libyan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Libyan", "Nose", "All Noses");
        AddEthnicityVariable("Libyan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Libyan", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Mauretanian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Mauretanian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Mauretanian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Mauretanian", "Ears", "All Ears");
        AddEthnicityVariable("Mauretanian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mauretanian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Mauretanian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Mauretanian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Mauretanian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Mauretanian", "Nose", "All Noses");
        AddEthnicityVariable("Mauretanian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Mauretanian", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Numidian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Numidian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Numidian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Numidian", "Ears", "All Ears");
        AddEthnicityVariable("Numidian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Numidian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Numidian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Numidian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Numidian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Numidian", "Nose", "All Noses");
        AddEthnicityVariable("Numidian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Numidian", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Egyptian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Egyptian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Egyptian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Egyptian", "Ears", "All Ears");
        AddEthnicityVariable("Egyptian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Egyptian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Egyptian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Egyptian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Egyptian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Egyptian", "Nose", "All Noses");
        AddEthnicityVariable("Egyptian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Egyptian", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Berber", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Berber", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Berber", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Berber", "Ears", "All Ears");
        AddEthnicityVariable("Berber", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Berber", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Berber", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Berber", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Berber", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Berber", "Nose", "All Noses");
        AddEthnicityVariable("Berber", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Berber", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Scythian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Scythian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Scythian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Scythian", "Ears", "All Ears");
        AddEthnicityVariable("Scythian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Scythian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Scythian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Scythian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Scythian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Scythian", "Nose", "All Noses");
        AddEthnicityVariable("Scythian", "Ears", "All Ears");
        AddEthnicityVariable("Scythian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Scythian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Sarmatian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Sarmatian", "Eye Colour", "brown_green_eyes");
        AddEthnicityVariable("Sarmatian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Sarmatian", "Ears", "All Ears");
        AddEthnicityVariable("Sarmatian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sarmatian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Sarmatian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Sarmatian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Sarmatian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Sarmatian", "Nose", "All Noses");
        AddEthnicityVariable("Sarmatian", "Ears", "All Ears");
        AddEthnicityVariable("Sarmatian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Sarmatian", "Skin Colour", "fair_olive_skin");

        AddEthnicityVariable("Arabian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Arabian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Arabian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Arabian", "Ears", "All Ears");
        AddEthnicityVariable("Arabian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Arabian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Arabian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Arabian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Arabian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Arabian", "Nose", "All Noses");
        AddEthnicityVariable("Arabian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Arabian", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Syrian", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Syrian", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Syrian", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Syrian", "Ears", "All Ears");
        AddEthnicityVariable("Syrian", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Syrian", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Syrian", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Syrian", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Syrian", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Syrian", "Nose", "All Noses");
        AddEthnicityVariable("Syrian", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Syrian", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Judean", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Judean", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Judean", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Judean", "Ears", "All Ears");
        AddEthnicityVariable("Judean", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Judean", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Judean", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Judean", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Judean", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Judean", "Nose", "All Noses");
        AddEthnicityVariable("Judean", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Judean", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Samaritan", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Samaritan", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Samaritan", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Samaritan", "Ears", "All Ears");
        AddEthnicityVariable("Samaritan", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Samaritan", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Samaritan", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Samaritan", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Samaritan", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Samaritan", "Nose", "All Noses");
        AddEthnicityVariable("Samaritan", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Samaritan", "Skin Colour", "swarthy_skin");

        AddEthnicityVariable("Gallilean", "Distinctive Feature", "All Distinctive Features");
        AddEthnicityVariable("Gallilean", "Eye Colour", "brown_blue_eyes");
        AddEthnicityVariable("Gallilean", "Eye Shape", "All Eye Shapes");
        AddEthnicityVariable("Gallilean", "Ears", "All Ears");
        AddEthnicityVariable("Gallilean", "Facial Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Gallilean", "Facial Hair Style", "All Facial Hair Styles");
        AddEthnicityVariable("Gallilean", "Hair Colour", "black_brown_grey_hair");
        AddEthnicityVariable("Gallilean", "Hair Style", "All Hair Styles");
        AddEthnicityVariable("Gallilean", "Humanoid Frame", "All Frames");
        AddEthnicityVariable("Gallilean", "Nose", "All Noses");
        AddEthnicityVariable("Gallilean", "Person Word", "Weighted Person Words");
        AddEthnicityVariable("Gallilean", "Skin Colour", "swarthy_skin");
    }

    private string ResolveAntiquityNameCulture(string preferred, string fallback)
    {
        if (_context.NameCultures.AsEnumerable().Any(x => x.Name.Equals(preferred, StringComparison.OrdinalIgnoreCase)))
        {
            return preferred;
        }

        if (_context.NameCultures.AsEnumerable().Any(x => x.Name.Equals(fallback, StringComparison.OrdinalIgnoreCase)))
        {
            return fallback;
        }

        return "Simple";
    }

    private void SeedRomanHeritageCultures()
    {

		AddCulture("Roman", "Roman",
			@"Roman culture is the civic culture of the city and its citizens, shaped by Latin speech, household gods, public law, military service, patronage and the honour of belonging to the Roman people.

It is at once urban and agricultural, practical and ritual-conscious, with sharp expectations around family authority, public reputation, debt, oath, inheritance and duty to the state.");

		AddCulture("Patrician", "Roman",
			@"Patrician culture belongs to the old aristocratic houses of Rome, where ancestry, public honour, priestly obligation and command of dependants stand at the centre of social life.

Its members are raised among patrons, clients, household cults, marriage alliances and public offices, and are expected to understand Roman tradition as both privilege and responsibility.");

		AddCulture("Latin Italic", "Roman",
			@"Latin and Italic culture belongs to the towns and peoples of Italia whose civic life is closely tied to Rome while still preserving local memories, cults, dialects and aristocratic traditions.

It is a culture of municipal pride, farming estates, allied service, local patrons and the steady negotiation between Roman public forms and older Italic identity.");

		AddCulture("Provincial Roman", "Roman",
			@"Provincial Roman culture belongs to established families outside Italia whose public lives are shaped by Roman law, Latin civic forms and imperial patronage while remaining rooted in their own cities and regions.

Its members move between local kinship, temple and council traditions on one side and Roman contracts, offices, manners and status symbols on the other.");

		AddCulture("Romanised Barbarian", "Roman",
			@"Romanised barbarian culture is found among people of non-Roman origin who have adopted Roman names, language, law, dress and habits of public life.

It is often ambitious and outward-looking, using Roman forms to cross boundaries of trade, service and status while still carrying traces of another household tradition.");

		AddCulture("Freed Slave", "Roman",
			@"Freedperson culture is shaped by the transition from enslavement into dependent liberty, especially in the households, workshops and commercial networks of Roman towns.

It values patronage, skilled work, careful respectability, family formation and the visible signs of a status that has been won but remains closely watched.");

		AddCulture("Slave", "Slave",
			@"Enslaved Mediterranean culture is not an ancestry but a shared condition formed in estates, workshops, mines, ships, farms and urban households across the ancient world.

People within it may come from many peoples, but daily life is shaped by imposed service, household hierarchy, survival, private loyalties and the customs preserved among fellow captives.");

		AddCulture("Etruscan", "Etruscan",
			@"Etruscan culture belongs to the old cities of northern and central Italia, where aristocratic houses, temple ritual, divination, feasting and tomb memory remain central to public identity.

It shares many exchanges with Rome and the wider Mediterranean while retaining a strong sense of local city tradition and ancestral prestige.");

		AddCulture("Hellenic", "Hellenic",
			@"Hellenic culture belongs to Greek-speaking cities and settlements joined by shared language, civic identity, public cult, athletic display, theatre, philosophy and memory of the old poleis.

It is diverse rather than uniform, ranging from island and coastal traders to inland aristocracies, but it commonly treats education, public speech, honour and city life as markers of refinement.");

		AddCulture("Anatolian", "Anatolian",
			@"Anatolian culture belongs to the cities, valleys and uplands of Asia Minor where older local peoples live among Greek civic forms, Persian inheritances and long-distance trade.

Its identity is layered, with local cults and dynastic memories existing beside Hellenic city life, caravan routes, fortified towns and the influence of neighbouring empires.");

		AddCulture("Punic", "Punic",
			@"Punic culture belongs to the Phoenician-descended cities and coastal communities of the western Mediterranean, especially those shaped by Carthage and its maritime world.

It is urban, commercial and seafaring, with strong traditions of merchant households, harbour life, Semitic speech, temple service and exchange with North African, Iberian, Sicilian and Greek neighbours.");

		AddCulture("Numidian-Mauretanian", ResolveAntiquityNameCulture("Numidian-Mauretanian", "Punic"),
			@"Numidian and Mauretanian culture belongs to the inland and frontier peoples of North Africa who live between pastoral, mounted, village and urban worlds.

Its communities are tied to local kings, tribal confederations, seasonal movement, horse culture and exchange with Punic cities, Roman provinces and desert-edge peoples.");

		AddCulture("Egyptian", "Egyptian",
			@"Egyptian culture belongs to the Nile valley, where ancient temple traditions, river agriculture, scribal memory and local village life remain deeply rooted despite foreign dynasties and Greek-speaking cities.

It is ordered around the river, the fields, the gods, the dead and the enduring prestige of a land with a very long memory of kingship and sacred custom.");

		AddCulture("Kushite", "Kushite",
			@"Kushite culture belongs to the Nile lands south of Egypt, shaped by royal courts, temple centres, river routes, pastoral wealth, ironworking and exchange across the Red Sea and African interior.

It shares old connections with Egypt while maintaining its own dynastic, religious and artistic traditions.");

		AddCulture("Persian", "Ancient Persian",
			@"Persian culture belongs to the Iranian courts, towns and estates shaped by royal service, cavalry traditions, noble households, formal dress, gardens, roads and imperial memory.

It values loyalty, hospitality, horsemanship, lineage and the disciplined manners of people accustomed to living within large kingdoms and long-distance administration.");

		AddCulture("Scythian-Sarmatian", "Scythian-Sarmatian",
			@"Scythian-Sarmatian culture belongs to the mounted peoples of the steppe and Black Sea frontier, where herds, horses, wagons, kin groups and warrior reputation shape daily life.

It is mobile, martial and prestige-conscious, with strong traditions of riding, feasting, gift exchange, animal ornament and negotiated relationships with settled towns and empires.");

		AddCulture("Celtic", "Celtic Male", "Celtic Female",
			@"Celtic culture belongs to a broad family of western and central peoples joined by related speech, aristocratic households, warrior reputation, feasting, oath bonds and skilled metalwork.

It varies sharply by region, from Atlantic communities to inland chiefdoms and Roman-border towns, but it commonly values kinship, honour, hospitality and the standing of local leaders.");

		AddCulture("Gaulish", "Gaulish Male", "Gaulish Female",
			@"Gaulish culture belongs to the communities of Gaul and the Alpine borderlands, where towns, hillforts, farms, sanctuaries and aristocratic warbands link local peoples into wider tribal confederations.

It is a culture of patrons, clients, feasts, crafts, trade roads and regional rivalries, with strong memories of independence even where Roman influence is already close.");

		AddCulture("Iberian Celtic", "Celtic Male", "Celtic Female",
			@"Iberian Celtic culture belongs to the western and interior peoples of Iberia, where fortified settlements, pastoral wealth, mining regions, local warrior elites and Mediterranean trade meet.

Its communities are shaped by kinship, hill country, metal wealth, old law, local cults and long contact with Phoenician, Greek, Carthaginian and Roman neighbours.");

		AddCulture("Insular Celtic", "Celtic Male", "Celtic Female",
			@"Insular Celtic culture belongs to the peoples of Britain, Hibernia and the northern islands, where kin groups, cattle wealth, local kings, druids, bards and warrior retinues carry public memory.

It is less urban than the Mediterranean world but rich in oral tradition, status display, hospitality, feuding, sacred places and regional identities.");

		AddCulture("Germanic", "Germanic Male", "Germanic Female",
			@"Germanic culture belongs to the peoples of the northern forests, coasts and riverlands beyond and along the Roman frontier, where households, free assemblies, retinues and war leaders hold public life together.

It is a broad and varied world rather than a single nation, marked by kinship, oath loyalty, seasonal farming, herding, trading contacts and a high value placed on personal reputation.");

		AddCulture("Illyrian-Pannonian", ResolveAntiquityNameCulture("Illyrian-Pannonian", "Hellenic"),
			@"Illyrian and Pannonian culture belongs to the Adriatic, Balkan and Danubian peoples whose communities bridge coastal trade, mountain herding, river frontiers and martial service.

Its identity is local and tribal before it is imperial, with strong ties to clan, district, harbour, hill settlement and the mixed influences of Greek, Roman, Celtic and inland Balkan neighbours.");

		AddCulture("Thracian-Dacian", ResolveAntiquityNameCulture("Thracian-Dacian", "Hellenic"),
			@"Thracian and Dacian culture belongs to the peoples north and east of the Greek and Macedonian world, where hill forts, horsemen, goldwork, tattooing traditions, music, local kings and warrior followings are prominent.

It is a frontier culture of villages, sanctuaries and strong chiefs, shaped by contact with Greeks, Persians, Celts, Scythians and Rome while retaining its own regional customs.");

		AddCulture("Levantine", ResolveAntiquityNameCulture("Antiquity Levantine", "Hellenic"),
			@"Levantine culture belongs to the cities, ports and inland towns of Syria, Phoenicia, Arabia and neighbouring lands, where caravan trade, temples, local dynasties and Greek civic forms meet.

It is multilingual, mercantile and cosmopolitan, with households and cities shaped by Semitic speech, Hellenistic public life, desert routes, sea trade and old local cults.");

		AddCulture("Judean", "Jewish Male", "Jewish Female",
			@"Judean culture belongs to the communities centred on the God of Israel, ancestral law, Sabbath observance, purity custom, scripture, household discipline and the memory of kingdom and exile.

It is at home in villages, towns and diaspora communities alike, maintaining a strong boundary between itself and neighbouring peoples while also living through trade, empire and local accommodation.");


	}

	private (Liquid BloodLiquid, Liquid SweatLiquid, Material
        DriedBlood, Material DriedSweat) CreateBloodAndSweat(string racialDescriptor)
    {
        Material driedBlood = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Materials,
            $"Dried {racialDescriptor} Blood",
            x => x.Name,
            () =>
            {
                Material created = new();
                _context.Materials.Add(created);
                return created;
            });
        driedBlood.Name = $"Dried {racialDescriptor} Blood";
        driedBlood.MaterialDescription = "dried blood";
        driedBlood.Density = 1520;
        driedBlood.Organic = true;
        driedBlood.Type = 0;
        driedBlood.BehaviourType = 19;
        driedBlood.ThermalConductivity = 0.2;
        driedBlood.ElectricalConductivity = 0.0001;
        driedBlood.SpecificHeatCapacity = 420;
        driedBlood.IgnitionPoint = 555.3722;
        driedBlood.HeatDamagePoint = 412.0389;
        driedBlood.ImpactFracture = 1000;
        driedBlood.ImpactYield = 1000;
        driedBlood.ImpactStrainAtYield = 2;
        driedBlood.ShearFracture = 1000;
        driedBlood.ShearYield = 1000;
        driedBlood.ShearStrainAtYield = 2;
        driedBlood.YoungsModulus = 0.1;
        driedBlood.SolventId = 1;
        driedBlood.SolventVolumeRatio = 4;
        driedBlood.ResidueDesc = "It is covered in {0}dried blood";
        driedBlood.ResidueColour = "red";
        driedBlood.Absorbency = 0;

        Liquid blood = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Liquids,
            $"{racialDescriptor} Blood",
            x => x.Name,
            () =>
            {
                Liquid created = new();
                _context.Liquids.Add(created);
                return created;
            });
        blood.Name = $"{racialDescriptor} Blood";
        blood.Description = "blood";
        blood.LongDescription = "a virtually opaque dark red fluid";
        blood.TasteText = "It has a sharply metallic, umami taste";
        blood.VagueTasteText = "It has a metallic taste";
        blood.SmellText = "It has a metallic, coppery smell";
        blood.VagueSmellText = "It has a faintly metallic smell";
        blood.TasteIntensity = 200;
        blood.SmellIntensity = 10;
        blood.AlcoholLitresPerLitre = 0;
        blood.WaterLitresPerLitre = 0.8;
        blood.DrinkSatiatedHoursPerLitre = 6;
        blood.FoodSatiatedHoursPerLitre = 4;
        blood.Viscosity = 1;
        blood.Density = 1;
        blood.Organic = true;
        blood.ThermalConductivity = 0.609;
        blood.ElectricalConductivity = 0.005;
        blood.SpecificHeatCapacity = 4181;
        blood.FreezingPoint = -20;
        blood.BoilingPoint = 100;
        blood.DisplayColour = "bold red";
        blood.DampDescription = "It is damp with blood";
        blood.WetDescription = "It is wet with blood";
        blood.DrenchedDescription = "It is drenched with blood";
        blood.DampShortDescription = "(blood damp)";
        blood.WetShortDescription = "(bloody)";
        blood.DrenchedShortDescription = "(blood drenched)";
        blood.SolventId = 1;
        blood.SolventVolumeRatio = 5;
        blood.InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement;
        blood.ResidueVolumePercentage = 0.05;
        blood.DriedResidue = driedBlood;

        Material driedSweat = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Materials,
            $"Dried {racialDescriptor} Sweat",
            x => x.Name,
            () =>
            {
                Material created = new();
                _context.Materials.Add(created);
                return created;
            });
        driedSweat.Name = $"Dried {racialDescriptor} Sweat";
        driedSweat.MaterialDescription = "dried sweat";
        driedSweat.Density = 1520;
        driedSweat.Organic = true;
        driedSweat.Type = 0;
        driedSweat.BehaviourType = 19;
        driedSweat.ThermalConductivity = 0.2;
        driedSweat.ElectricalConductivity = 0.0001;
        driedSweat.SpecificHeatCapacity = 420;
        driedSweat.IgnitionPoint = 555.3722;
        driedSweat.HeatDamagePoint = 412.0389;
        driedSweat.ImpactFracture = 1000;
        driedSweat.ImpactYield = 1000;
        driedSweat.ImpactStrainAtYield = 2;
        driedSweat.ShearFracture = 1000;
        driedSweat.ShearYield = 1000;
        driedSweat.ShearStrainAtYield = 2;
        driedSweat.YoungsModulus = 0.1;
        driedSweat.SolventId = 1;
        driedSweat.SolventVolumeRatio = 3;
        driedSweat.ResidueDesc = "It is covered in {0}dried sweat";
        driedSweat.ResidueColour = "yellow";
        driedSweat.Absorbency = 0;

        Liquid sweat = SeederRepeatabilityHelper.EnsureNamedEntity(
            _context.Liquids,
            $"{racialDescriptor} Sweat",
            x => x.Name,
            () =>
            {
                Liquid created = new();
                _context.Liquids.Add(created);
                return created;
            });
        sweat.Name = $"{racialDescriptor} Sweat";
        sweat.Description = "sweat";
        sweat.LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor";
        sweat.TasteText = "It tastes like a pungent, salty lick of someone's underarms";
        sweat.VagueTasteText = "It tastes very unpleasant, like underarm stench";
        sweat.SmellText = "It has the sharp, pungent smell of body odor";
        sweat.VagueSmellText = "It has the sharp, pungent smell of body odor";
        sweat.TasteIntensity = 200;
        sweat.SmellIntensity = 200;
        sweat.AlcoholLitresPerLitre = 0;
        sweat.WaterLitresPerLitre = 0.95;
        sweat.DrinkSatiatedHoursPerLitre = 5;
        sweat.FoodSatiatedHoursPerLitre = 0;
        sweat.Viscosity = 1;
        sweat.Density = 1;
        sweat.Organic = true;
        sweat.ThermalConductivity = 0.609;
        sweat.ElectricalConductivity = 0.005;
        sweat.SpecificHeatCapacity = 4181;
        sweat.FreezingPoint = -20;
        sweat.BoilingPoint = 100;
        sweat.DisplayColour = "yellow";
        sweat.DampDescription = "It is damp with sweat";
        sweat.WetDescription = "It is wet and smelly with sweat";
        sweat.DrenchedDescription = "It is soaking wet and smelly with sweat";
        sweat.DampShortDescription = "(sweat-damp)";
        sweat.WetShortDescription = "(sweaty)";
        sweat.DrenchedShortDescription = "(sweat-drenched)";
        sweat.SolventId = 1;
        sweat.SolventVolumeRatio = 5;
        sweat.InjectionConsequence = (int)LiquidInjectionConsequence.Harmful;
        sweat.ResidueVolumePercentage = 0.05;
        sweat.DriedResidue = driedSweat;
        _context.SaveChanges();

        return (blood, sweat, driedBlood, driedSweat);
    }
}
