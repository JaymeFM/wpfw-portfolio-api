DECLARE @Email nvarchar(256) = 'editor@example.com';
DECLARE @Rol nvarchar(256) = 'Editor';

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
JOIN AspNetRoles r ON r.NormalizedName = UPPER(@Rol)
WHERE u.NormalizedEmail = UPPER(@Email);
