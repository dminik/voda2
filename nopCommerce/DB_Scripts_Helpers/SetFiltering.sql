USE [nopCommerceVoda2]
GO

SELECT distinct
	  pm.Id,
      s.Name,
	  so.Name
      ,[AllowFiltering]
            
  FROM [dbo].[Product_SpecificationAttribute_Mapping] pm
	join [dbo].[SpecificationAttributeOption] so on so.Id = pm.SpecificationAttributeOptionId
	join [dbo].[SpecificationAttribute] s on s.Id = so.SpecificationAttributeId
where s.Name in ('Тип плеера',
					'Комплект акустических систем',
					'Мощность суммарная',
					'Функция караоке')
GO



update [dbo].[Product_SpecificationAttribute_Mapping] 
set [AllowFiltering] = 1
where Id in (
	select pm.Id
  FROM [dbo].[Product_SpecificationAttribute_Mapping] pm
	join [dbo].[SpecificationAttributeOption] so on so.Id = pm.SpecificationAttributeOptionId
	join [dbo].[SpecificationAttribute] s on s.Id = so.SpecificationAttributeId
where s.Name = 'Объем памяти'

)

