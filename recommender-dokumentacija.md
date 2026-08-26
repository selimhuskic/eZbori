# eZbori — Dokumentacija sistema preporuka

## Pregled

Sistem preporuka kombinuje dva algoritma: **popularnostni rangiranjem** (engl. popularity-based ranking) i **personalizaciju na osnovu historije pretraga**. Svaka preporuka nosi i objašnjenje zašto je predložena korisnik.

---

## Algoritam rangiranja (prvi nivo)

### Signali relevantnosti

Za svaku kombinaciju (tip podataka, godina izbora) sistem izračunava ocjenu relevantnosti (0–4) koristeći dvije dimenzije:

**1. Recency (svježi podataka) — osnova**

| Pozicija godine u sortiranom nizu | Ocjena |
|---|---|
| Najnovija godina (posljednja u nizu) | +2 |
| Pretposljednja godina | +1 |
| Starije godine | +0 |

Izračun se vrši unutar `SearchSuggestionRelevanceGenerator.GetRelevance()` koji prima sortiran rastući niz svih dostupnih godina.

**2. Election level (nivo izbora) — multiplikator**

| Nivo | Primjeri | Multiplikator |
|---|---|---|
| Državni parlament / Predsjednički | Državni parlament BiH, Predsjedništvo | ×2 |
| Entitetski parlament | Entitetski parlamenti (FBiH, RS) | ×1.5 |
| Kantonalni parlament | Kantonalna skupštine | ×1 |
| Općinski nivo | Općinski vijećnici, kandidati | ×0.75 |

Mapiranje tipova na nivo definirano je u `RankingService._typeToSubject` (vrijednosti: 1=Državni, 5=Predsjednički, 9=Entitetski, 15=Kantonalni, 20/22=Općinski).

### Dedupliciranje

Svaki par (nivo izbora, godina) pojavljuje se maksimalno jednom u finalnoj listi — vrši se grupiranje i uzima se zapis s najvišom ocjenom iz grupe.

---

## Personalizacija (drugi nivo)

Ako je korisnik prijavljen, sistem učitava njegovu historiju spašenih pretraga (`ISavedSearchRepository.GetByUserAsync`) i primjenjuje boost:

| Podudaranje | Boost |
|---|---|
| Isti nivo izbora I ista godina kao spašena pretraga | +3 |
| Isti nivo izbora (bilo koja godina) | +2 |
| Ista godina (bilo koji nivo) | +1 |

Boost se sabira na osnovnu ocjenu relevantnosti, čime spašene pretrage prirodno iskaču na vrh liste.

---

## Objašnjiivost (Reason)

Svaka preporuka sadrži polje `Reason` koje se prikazuje korisniku u mobilnoj aplikaciji ispod naziva pretrage. Moguće vrijednosti:

| Razlog | Kada |
|---|---|
| `Na osnovu spašenih pretraga` | Korisnik ima spašenu pretragu istog nivoa ili iste godine |
| `Predsjednički izbori` | Predsjednički nivo, bez podudaranja |
| `Državni parlament` | Državni nivo, bez podudaranja |
| `Entitetski parlament` | Entitetski nivo, bez podudaranja |
| `Kantonalni parlament` | Kantonalni nivo, bez podudaranja |
| `Općinski kandidati` / `Općinsko vijeće` | Općinski nivo, bez podudaranja |

---

## Implementacijske napomene

- Pet repozitorija se pozivaju **paralelno** putem `Task.WhenAll` (poboljšanje performansi)
- Anonimni korisnici dobivaju isključivo popularnostne preporuke (bez personalizacije)
- Spašene pretrage s `AnalysisSubject = null` ne sudjeluju u subject-boostanju, ali sudjeluju u year-boostanju
- Broj preporuka je konfigurabiln putem query parametra `top` (default: 10)
