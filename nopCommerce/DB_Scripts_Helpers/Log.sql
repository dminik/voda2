/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 100 [Id]
      ,[CreatedOnUtc]
      ,[ShortMessage] as '66666666666666666666666666666668888888888888888888888866666666666666666666666666666'
      ,[FullMessage]
      ,[IpAddress]
      ,[CustomerId]
      ,[PageUrl]
      ,[ReferrerUrl]
      
  FROM [nopCommerceVoda2].[dbo].[Log]
  order by  [Id] desc
