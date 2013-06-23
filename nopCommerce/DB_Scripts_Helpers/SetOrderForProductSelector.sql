SELECT      TOP (200) Id, Name, DisplayOrder
FROM          SpecificationAttribute
WHERE      (Name IN ('Тип фильтра', 'Отдельный кран', 'Минерализатор', 'Помпа для повышения давления', 'Производитель', 
                        'Рекомендуемая производительность', 'Способы очистки', 'Ступеней очистки'))
ORDER BY DisplayOrder