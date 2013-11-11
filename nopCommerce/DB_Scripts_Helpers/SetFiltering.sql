USE [nopCommerceVoda2]
GO

SELECT distinct
	  pm.Id,
      s.Name,
	  so.Name as Value,
	  so.DisplayOrder,
      AllowFiltering
            
  FROM [dbo].[Product_SpecificationAttribute_Mapping] pm
	join [dbo].[SpecificationAttributeOption] so on so.Id = pm.SpecificationAttributeOptionId
	join [dbo].[SpecificationAttribute] s on s.Id = so.SpecificationAttributeId
where s.Name in ('Äטאדמםאכü, ה‏יל')
GO



update [dbo].[Product_SpecificationAttribute_Mapping] 
set [AllowFiltering] = 1
where Id in (
	select pm.Id
  FROM [dbo].[Product_SpecificationAttribute_Mapping] pm
	join [dbo].[SpecificationAttributeOption] so on so.Id = pm.SpecificationAttributeOptionId
	join [dbo].[SpecificationAttribute] s on s.Id = so.SpecificationAttributeId
where s.Name = 'Äטאדמםאכü, ה‏יל'

)


UPDATE [dbo].[SpecificationAttributeOption]
   SET [DisplayOrder] = 0
 WHERE [DisplayOrder] = 777

