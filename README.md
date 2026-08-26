# eZbori

Platforma za analizu izbornih rezultata u Bosni i Hercegovini.  
Seminar projekt — Fakultet informacijskih tehnologija, Sarajevo.

## Arhitektura

| Sloj | Tehnologija |
|---|---|
| Backend API | .NET 10, CQRS / MediatR, EF Core |
| Baza podataka | SQL Server 2022 |
| Poruke | RabbitMQ 3.13 |
| Email worker | .NET Worker Service (`eZbori.Sender`) |
| Mobilna aplikacija | Flutter (Android) |
| Desktop admin | Flutter (Windows) |

---

## Pokretanje putem Dockera

### 1. Kopirati `.env.example` u `.env` i popuniti vrijednosti

```bash
cp .env.example .env
```

| Varijabla | Opis |
|---|---|
| `SA_PASSWORD` | Lozinka za SQL Server SA nalog (min. 8 znakova, velika+mala+broj+specijal) |
| `RABBITMQ_USER` | RabbitMQ korisničko ime |
| `RABBITMQ_PASS` | RabbitMQ lozinka |
| `JWT_SECRET` | Tajni ključ za JWT potpis (min. 32 znaka) |
| `JWT_ISSUER` | JWT izdavač (npr. `eZbori`) |
| `JWT_AUDIENCE` | JWT publika (npr. `eZboriApp`) |
| `JWT_EXPIRY_MINUTES` | Trajanje access tokena u minutama (npr. `60`) |
| `SMTP_HOST` | SMTP host (npr. `smtp.gmail.com`) |
| `SMTP_PORT` | SMTP port (npr. `587`) |
| `SMTP_USER` | SMTP korisničko ime / email |
| `SMTP_PASS` | SMTP lozinka / app password |

### 2. Pokrenuti sve servise

```bash
docker compose up --build
```

Pokreće se:
- **SQL Server** na portu `1433`
- **RabbitMQ** na portovima `5672` (AMQP) i `15672` (Management UI)
- **eZbori API** na portu `5000` — `http://localhost:5000/api`
- **eZbori Sender** (email worker) — nema HTTP porta, konzumira RabbitMQ redove

### 3. Provjera

- API Swagger: `http://localhost:5000/swagger`
- RabbitMQ Management: `http://localhost:15672`

---

## Pristup servisima i kredencijali

| Servis | URL / Pokretanje | Korisnik | Lozinka |
|---|---|---|---|
| **API (Swagger)** | `http://localhost:5000/swagger` | — | — |
| **RabbitMQ Management** | `http://localhost:15672` | vrijednost `RABBITMQ_USER` iz `.env` | vrijednost `RABBITMQ_PASS` iz `.env` |
| **Admin desktop app** | pokrenuti EXE iz build foldera ili `flutter run -d windows` | `admin@ezbori.ba` | `Admin123!` |
| **Mobilna app** | instalirati APK na AVD ili pokrenuti `flutter run` | `korisnik@ezbori.ba` | `User123!` |

> Svi servisi zahtijevaju da je `docker compose up` aktivan na host mašini.

---

## Pokretanje iz source koda

### Mobilna aplikacija (Flutter — Android)

```bash
cd Presentation/Mobile
flutter pub get
flutter run --profile --dart-define=API_BASE_URL=http://10.0.2.2:5000/api
```

Za fizički uređaj zamijeniti `10.0.2.2` IP adresom računara u lokalnoj mreži.

### Desktop admin aplikacija (Flutter — Windows)

```bash
cd Presentation/Admin
flutter pub get
flutter run -d windows --dart-define=API_BASE_URL=http://localhost:5000/api
```

---

## Instalacija APK-a na Android emulator

1. Pokrenuti Android emulator (Android Virtual Device — AVD)
2. Osigurati da je `docker compose up` aktivan
3. Prevući `app-release.apk` fajl u prozor emulatora — instalacija počinje automatski
4. Otvoriti aplikaciju "eZbori" na emulátoru
5. Adresa `10.0.2.2` je standardna AVD adresa za host mašinu — ne mijenjati

---

## Testiranje funkcionalnosti

### Mobilna aplikacija

#### Registracija
1. Na ekranu za prijavu tapnuti "Registruj se"
2. Unijeti ime, prezime, email i lozinku (min. 8 znakova)
3. Potvrda → automatski prelaz na prijavu
4. Prijaviti se s novim kredencijalima

#### Prijava
- Koristiti: `korisnik@ezbori.ba` / `User123!`
- Pri uspješnoj prijavi: prelaz na početni ekran s kategorijama analize

#### Zaboravljena lozinka
1. Ekran za prijavu → "Zaboravili ste lozinku?"
2. Unijeti email registrovanog korisnika → "Pošalji kod"
3. Otvoriti inbox → primiti email s 8-znakastim hex kodom (npr. `A3F1C9B2`)
4. Unijeti kod → unijeti novu lozinku → potvrda → "Lozinka uspješno promijenjena."
5. Prijaviti se s novom lozinkom
6. Pokušati iskoristiti isti kod ponovo → "Kod je nevažeći ili je istekao."

#### Analiza izbornih rezultata
1. Na početnom ekranu odabrati kategoriju (npr. "Opšti izbori" ili "Lokalni izbori")
2. Odabrati godinu:
   - Opšti izbori: **2018**, **2022**
   - Lokalni izbori: **2016**, **2020**, **2024**
3. Odabrati nivo analize: predsjednički, državni, entitetski, kantonalni ili općinski
4. Ovisno o odabiru, popuniti padajuće liste (entitet, kanton, općina)
5. Tapnuti "Pretraži" → tabela s rezultatima se prikazuje
6. Tapnuti na red stranke → detalji za tu stranku/kandidata

#### Sačuvane pretrage
1. Izvršiti pretragu → tapnuti ikonu bookmarka (gornji desni ugao) → "Pretraga spašena."
2. Pokušati sačuvati iste parametre ponovo → "Pretraga je već spašena." (duplikat blokiran)
3. Izvršiti 2–3 različite pretrage s bookmarkovima
4. Navigirati na "Sačuvane pretrage" u meniju → sve pretrage su vidljive
5. Tapnuti na spašenu pretragu → direktno otvara rezultate s tim parametrima

#### Preporuke
1. Nakon bookmarkovanja nekoliko pretraga: navigirati na "Preporučeno"
2. Lista kartica — svaka ima podnaslov s razlogom (npr. "Na osnovu spašenih pretraga" ili "Kantonalni parlament")
3. Pretrage koje odgovaraju spašenim temama prikazuju se više u listi
4. Na novom nalogu bez historije: prikazuju se opšte preporuke bazirane na popularnosti

#### Export u CSV
1. Izvršiti pretragu → tapnuti ikonu eksporta u gornjoj traci
2. Fajl se sprema na uređaj (`Android/data/com.example.ezbori_mobile/files/`)
3. Otvoriti Files aplikaciju → pronaći `ezbori_export_*.csv`

#### Profil i promjena lozinke
1. Donji meni → "Profil"
2. "Uredi profil" → promijeniti ime ili općinu → sačuvati → promjena vidljiva
3. "Promjena lozinke" → unijeti trenutnu + novu + potvrdu → "Lozinka promijenjena."

---

### Desktop admin panel

#### Korisnici
1. Prijaviti se kao `admin@ezbori.ba` / `Admin123!`
2. Navigirati na "Upravljanje korisnicima" (drawer ili kartica na dashboardu)
3. Lista korisnika se učitava; u polju za pretragu filtrirati po imenu ili emailu
4. Klik na ikonu zamjene uloge → dijalog za potvrdu → uloga promijenjena
5. Klik na ikonu brisanja → dijalog za potvrdu → korisnik obrisan iz liste
6. PDF ikona u AppBaru → otvara se PDF sa svim korisnicima (ime, email, uloga, status)

#### Pozivanje novog korisnika (RabbitMQ)
1. "Upravljanje korisnicima" → "Pozovi korisnika"
2. Unijeti ime, prezime, email, ulogu i opcionalnu poruku → Pošalji
3. "Pozivnica uspješno poslana." → na navedeni email stiže poruka s privremenom lozinkom
4. Otvoriti `http://localhost:15672` → provjera Connections (treba biti samo 1 konekcija — singleton)

#### Izborni ciklusi
1. Navigirati na "Izborni ciklusi" → 5 postojećih ciklusa vidljivo
2. U polju za pretragu unijeti godinu ili tip → filtrira listu
3. PDF ikona → otvara PDF sa svim ciklusima
4. **Test CRUD-a (preporučeni redosljed):**
   - Zapisati podatke 2024 lokalnih izbora u Notepad:
     - Godina: `2024`
     - Tip: `2` (Lokalni izbori)
     - API URL: `https://www.izbori.ba/api_2018`
     - Result Key: `WebResult_2024MUNI_2024_9_19_20_28_13`
   - Obrisati red za 2024 → dijalog za potvrdu → obrisan
   - Pritisnuti FAB (+) → unijeti zapisane vrijednosti → Sačuvati
   - 2024 lokalni izbori ponovo vidljivi u listi

#### Bootstrap (uvoz podataka)
1. Admin → "Bootstrap podataka"
2. U sekciji "Brzi uvoz" odabrati tip (npr. Opšti izbori) i godinu (npr. 2022)
3. Klik "Uvoz podataka" → siva poruka "Uvoz je pokrenut u pozadini" → dugme se odmah odblokira
4. Uvoz se izvršava u pozadini na serveru (5–15 minuta za opšte izbore, ~1 minuta za lokalne)
5. Podaci su vidljivi u mobilnoj aplikaciji pri analizi te godine — restart mobilne aplikacije može biti potreban
> **Napomena:** Potrebno je imati aktivan internet za pristup izbori.ba API-u.

#### Općine
1. Navigirati na "Općine" → lista sa poljem za pretragu
2. Unijeti naziv u polje za pretragu → lista se filtrira
3. Klik na ikonu olovke → unijeti novo ime ili populaciju → "Općina ažurirana."
4. Klik na ikonu kante → dijalog za potvrdu → obrisana iz liste

---

## Konfiguracija okoline

Datoteka `.env` nije commitovana (u `.gitignore`). Struktura je u `.env.example`.  
Šifrirana kopija tajni nalazi se u `.env-tajne.zip` u korijenu repozitorija.

Za lokalnu konfiguraciju API-a bez Dockera, editovati `eZbori.WebAPI/appsettings.Development.json`.
