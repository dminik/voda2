/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [Id]
      ,[CreatedOnUtc]
      ,[ShortMessage] as '66666666666666666666666666666668888888888888888888888866666666666666666666666666666'
      ,[FullMessage]
      ,[IpAddress]
      ,[CustomerId]
      ,[PageUrl]
      ,[ReferrerUrl]
      
  FROM [nopCommerceVoda2].[dbo].[Log]
  --where [ShortMessage] like '%PARSE CATEGORY %'
  order by  [Id] desc


