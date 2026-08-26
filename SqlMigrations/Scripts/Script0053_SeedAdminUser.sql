IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'admin@ezbori.ba')
BEGIN
    INSERT INTO [dbo].[Users] ([Email], [UserName], [FirstName], [LastName], [Password], [UserRole], [UserVerified], [CreatedAt])
    VALUES (
        'admin@ezbori.ba',
        'admin@ezbori.ba',
        'Admin',
        'eZbori',
        'AQAAAAIAAYagAAAAEEThH6Lb9IQUwYCZl5BCOD579o3uWdL81PhmvIsPC3DZwiZVL+kGwdblC46sxUYCSg==',
        2,
        1,
        GETUTCDATE()
    )
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'korisnik@ezbori.ba')
BEGIN
    INSERT INTO [dbo].[Users] ([Email], [UserName], [FirstName], [LastName], [Password], [UserRole], [UserVerified], [CreatedAt])
    VALUES (
        'korisnik@ezbori.ba',
        'korisnik@ezbori.ba',
        'Test',
        'Korisnik',
        'AQAAAAIAAYagAAAAEL6xKKJF8ZJ3ea/4AAX5wRg+olWbQiDb4+gYIr3X7IHNSqcMGjlVfB+rO0nygbSwNA==',
        1,
        1,
        GETUTCDATE()
    )
END
