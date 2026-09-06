IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('dbo.FaqItems'))
BEGIN
    CREATE TABLE [dbo].[FaqItems]
    (
        [Id]        INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
        [Question]  NVARCHAR(500) NOT NULL,
        [Answer]    NVARCHAR(MAX) NOT NULL,
        [SortOrder] INT NOT NULL
    )

    INSERT INTO [dbo].[FaqItems] ([Question], [Answer], [SortOrder])
    VALUES
        (N'Što je eZbori?',
         N'eZbori je platforma za pregled i analizu rezultata izbora u Bosni i Hercegovini. Omogućuje uvid u izborne rezultate na svim razinama vlasti — od predsjedništva do općinskih vijeća.',
         1),
        (N'Koji su izbori dostupni?',
         N'Dostupni su opšti izbori (predsjedništvo, Parlamentarna skupština, entitetski parlamenti i kantoni) te lokalni izbori (općinska vijeća i načelnici) za sve dostupne izborne godine.',
         2),
        (N'Da li trebam račun za pregled podataka?',
         N'Osnovni pregled rezultata dostupan je i bez registracije. Registracijom dobivate pristup naprednim analizama i usporednim pregledima.',
         3),
        (N'Odakle dolaze podaci?',
         N'Podaci se preuzimaju iz službenih izvora Centralne izborne komisije Bosne i Hercegovine (CIK BiH) i redovito se ažuriraju.',
         4),
        (N'Kako se tumači izlaznost?',
         N'Izlaznost je postotak registriranih birača koji su glasali na izborima. Na primjer, izlaznost od 52 % znači da je glasalo 52 od 100 registriranih birača.',
         5),
        (N'Kako mogu prijaviti grešku u podacima?',
         N'Greške možete prijaviti putem kontakt forme u odjeljku Profil, ili direktno na našu e-mail adresu podrške.',
         6)
END
