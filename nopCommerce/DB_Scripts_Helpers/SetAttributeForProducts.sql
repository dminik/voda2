begin transaction

INSERT INTO [dbo].[Product_SpecificationAttribute_Mapping]
           ([ProductId]
           ,[SpecificationAttributeOptionId]
           , [CustomValue]
           , [AllowFiltering]
           , [ShowOnProductPage]
           , [DisplayOrder])
     
           SELECT pr.Id
           ,(     SELECT opt.Id    
				  FROM [dbo].[SpecificationAttributeOption] opt 
					join [dbo].[SpecificationAttribute] attr on attr.Id = opt.SpecificationAttributeId  
					where attr.Name like '%Производитель%'		-- Attribute name
						and opt.Name like '%Atoll%'				-- Attribute option
		    )
           , null -- [CustomValue]
           , 1 --[AllowFiltering]
           , 1 --[ShowOnProductPage]
           , 1 --[DisplayOrder]))
		   from Product pr where pr.Name like '%Atoll%'			-- Products names




SELECT pr.Name
	,attr.Name
	,opt.Name
    
  FROM [Product_SpecificationAttribute_Mapping] ps
		join Product pr on pr.Id = ps.ProductId
		join [dbo].[SpecificationAttributeOption] opt on ps.SpecificationAttributeOptionId = opt.Id 
		join [dbo].[SpecificationAttribute] attr on attr.Id = opt.SpecificationAttributeId 	
	where attr.Name like '%Производитель%'
		and opt.Name like '%Atoll%'
rollback;

/*

select Id
	,pr.Name
from Product pr
where pr.Name like '%Atoll%'

SELECT opt.Id
	,attr.Name
	,opt.Name
    
  FROM [dbo].[SpecificationAttributeOption] opt 
	join [dbo].[SpecificationAttribute] attr on attr.Id = opt.SpecificationAttributeId  
	where attr.Name like '%Производитель%'
		and opt.Name like '%Atoll%'
*/