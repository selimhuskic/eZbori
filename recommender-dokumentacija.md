# eZbori — Dokumentacija sistema preporuka

## Pregled

Sistem preporuka kombinuje dva algoritma: **popularnostni rangiranjem** (engl. popularity-based ranking) i **personalizaciju na osnovu historije pretraga**. Svaka preporuka nosi i objašnjenje zašto je predložena korisnik.

---

## Algoritam rangiranja (prvi nivo)

### Signali relevantnosti

Za svaku kombinaciju (tip podataka, godina izbora) sistem izračunava ocjenu relevantnosti koristeći `SearchSuggestionRelevanceGenerator.GetRelevance(godina, sveGodine, tip)`:

- Ocjena je **fiksni rang važnosti tipa podatka (1–4)**, definiran u četiri statičke liste tipova (`relevanceTypes4/3/2/1`): rezultati najvišeg nivoa (predsjednički rezultati, entitetski/državni izborni rezultati po stranci) nose 4; pregledi istog nivoa nose 3; općinski/municipalni rezultati nose 2; općinski/municipalni pregledi i preostali tipovi nose 1.
- Ova ocjena se dodjeljuje **isključivo najnovijoj godini** u tom skupu podataka (`sveGodine.LastOrDefault() == godina`) — svaka starija godina dobija ocjenu **0** i time se efektivno isključuje iz rangiranja.
- Ne postoji multi-nivo recency skala niti multiplikator po nivou izbora — ocjena je direktno ovaj fiksni rang, bez množenja.

Mapiranje tipova na "subject" (nivo prikaza korisniku) definirano je odvojeno u `RankingService._typeToSubject` (vrijednosti: 1=Državni, 5=Predsjednički, 9=Entitetski, 15=Kantonalni, 20/22=Općinski) i koristi se samo za grupiranje/deduplikaciju i personalizacijske boostove, ne za samu ocjenu relevantnosti.

### Dedupliciranje

Svaki par (subject, godina) pojavljuje se maksimalno jednom u finalnoj listi — vrši se grupiranje (`RankingService.GetSuggestedSearchesRankedAsync`) i uzima se zapis s **najvišom ocjenom relevantnosti** iz grupe (`g.OrderByDescending(x => x.Relevance).First()`), prije primjene personalizacijskog boosta.

---

## Personalizacija (drugi nivo)

Ako je korisnik prijavljen, sistem učitava njegovu historiju spašenih pretraga (`ISavedSearchRepository.GetByUserIncludingDeletedAsync`) i primjenjuje boost. **Brisanje spašene pretrage je soft-delete upravo zato da ta historija i dalje ostane signal recommenderu** — obrisane pretrage se stoga namjerno uključuju u ovaj upit (za razliku od `GetByUserAsync`, koji izostavlja obrisane i koristi se isključivo za prikaz liste spašenih pretraga korisniku).

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

- Pet repozitorija se pozivaju **sekvencijalno** (jedan `await` za drugim), namjerno — paralelno pozivanje putem `Task.WhenAll` je ranije uzrokovalo `InvalidOperationException` (HTTP 500), jer EF Core `DbContext` nije thread-safe za paralelne upite nad istom instancom. Ne vraćati na paralelno izvršavanje bez uvođenja odvojenog `DbContext`-a po pozivu.
- Anonimni korisnici dobivaju isključivo popularnostne preporuke (bez personalizacije)
- Spašene pretrage s `AnalysisSubject = null` ne sudjeluju u subject-boostanju, ali sudjeluju u year-boostanju
- Broj preporuka je konfigurabiln putem query parametra `top` (default: 10)
