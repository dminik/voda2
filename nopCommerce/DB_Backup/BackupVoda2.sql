DECLARE @MyFileName varchar(200)
SELECT @MyFileName=N'D:\!Work\voda2\nopCommerce\DB_Backup\' + REPLACE(convert(nvarchar(20),GetDate(),120),':','-') + '.bak'

BACKUP DATABASE [nopCommerceVoda2] TO  DISK = @MyFileName WITH NOFORMAT, NOINIT,  NAME = N'nopCommerceVoda2-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
GO
