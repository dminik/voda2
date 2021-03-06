
-- Проставить всем отрибутам AllowFiltering, если они не в списке
update Product_SpecificationAttribute_Mapping
set AllowFiltering = 0
where Id not in (
  SELECT ps.Id	    
  FROM [Product_SpecificationAttribute_Mapping] ps
	join [dbo].[SpecificationAttributeOption] opt on ps.SpecificationAttributeOptionId = opt.Id  
	join [dbo].[SpecificationAttribute] attr on attr.Id = opt.SpecificationAttributeId  
	where attr.Name in ('Тип фильтра',					
					'Отдельный кран',
					'Минерализатор',					
					'Помпа для повышения давления',
					'Производитель',
					'Рекомендуемая производительность',
					'Способы очистки',
					'Ступеней очистки'))

SELECT ps.Id
	,attr.Name
      ,[AllowFiltering]      
  FROM [Product_SpecificationAttribute_Mapping] ps
	join [dbo].[SpecificationAttributeOption] opt on ps.SpecificationAttributeOptionId = opt.Id  
	join [dbo].[SpecificationAttribute] attr on attr.Id = opt.SpecificationAttributeId  
	where attr.Name in ('Тип фильтра',
					'Число ступеней очистки',
					'Отдельный кран',
					'Минерализатор',					
					'Помпа для повышения давления',
					'Производитель',
					'Рекомендуемая производительность',
					'Способы очистки',
					'Ступеней очистки')