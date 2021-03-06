USE [nopCommerceVoda2]
GO
/****** Object:  StoredProcedure [dbo].[ProductLoadAllPaged2]    Script Date: 08.09.2013 18:36:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[ProductLoadAllPaged2]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3				
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,		
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16	
	@ShowWithPositiveQuantity bit = 0,
	@PositiveQuantityCount	int = null OUTPUT
)
AS
BEGIN
	
	DECLARE @sql nvarchar(max)

	SET NOCOUNT ON
	
	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',')
	DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecs)
		
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	SET @sql = '
	INSERT INTO #DisplayOrderTmp ([ProductId])
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		LEFT JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	
	SET @sql = @sql + '
	LEFT JOIN ProductVariant pv with (NOLOCK)
		ON p.Id = pv.ProductId
	WHERE
		p.Deleted = 0'
	
	--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId IN (SELECT CategoryId FROM #FilteredCategoryIds)'				
	END
	
	SET @sql = @sql + '
	AND p.Published = 1
	AND pv.Published = 1
	AND pv.Deleted = 0
	AND (getutcdate() BETWEEN ISNULL(pv.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(pv.AvailableEndDateTimeUtc, ''1/1/2999''))'
	

	--show with quantity > 0
	IF @ShowWithPositiveQuantity = 1
	BEGIN
		SET @sql = @sql + 'AND pv.AvailableForPreOrder = 0'   
	END
	
	--min price
	IF @PriceMin > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')
				)
			)'
	END
	
	--max price
	IF @PriceMax > 0
	BEGIN
		SET @sql = @sql + '
		AND (
				(
					--special price (specified price and valid date range)
					(pv.SpecialPrice IS NOT NULL AND (getutcdate() BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.SpecialPrice <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
				OR 
				(
					--regular price (price isnt specified or date range isnt valid)
					(pv.SpecialPrice IS NULL OR (getutcdate() NOT BETWEEN isnull(pv.SpecialPriceStartDateTimeUtc, ''1/1/1900'') AND isnull(pv.SpecialPriceEndDateTimeUtc, ''1/1/2999'')))
					AND
					(pv.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')
				)
			)'
	END
	
	
	SET @sql = @sql + '
	AND (p.SubjectToAcl = 0)'
	
	
		
	--filter by specs
	IF @SpecAttributesCount > 0
	BEGIN
		SET @sql = @sql + '
		AND NOT EXISTS (
			SELECT 1 FROM #FilteredSpecs [fs]
			WHERE
				[fs].SpecificationAttributeOptionId NOT IN (
					SELECT psam.SpecificationAttributeOptionId
					FROM Product_SpecificationAttribute_Mapping psam
					WHERE psam.AllowFiltering = 1 AND psam.ProductId = p.Id
				)
			)'
	END

	
	-- PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
	
	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp
	GROUP BY ProductId
	ORDER BY min([Id])

		
	DROP TABLE #DisplayOrderTmp

	
	--return products
	--SELECT p.*
	--FROM
	--	#PageIndex [pi]
	--	INNER JOIN Product p on p.Id = [pi].[ProductId]
	


	--return products
	--SELECT sao.Name, psam.SpecificationAttributeOptionId, count(psam.SpecificationAttributeOptionId)
	--FROM #PageIndex [pi]
	--		INNER JOIN Product p on p.Id = [pi].[ProductId]
	--		inner join Product_SpecificationAttribute_Mapping psam on psam.ProductId = p.Id
	--		inner join SpecificationAttributeOption sao on sao.Id = psam.SpecificationAttributeOptionId
	--WHERE psam.AllowFiltering = 1 
	--group by sao.Name, psam.SpecificationAttributeOptionId

	SELECT	1 as Id,
			psam.SpecificationAttributeOptionId as SpecificationAttributeOptionId,
			count(psam.SpecificationAttributeOptionId) as ProductCount
	FROM #PageIndex [pi]
			INNER JOIN Product p on p.Id = [pi].[ProductId]
			inner join Product_SpecificationAttribute_Mapping psam on psam.ProductId = p.Id			
	WHERE psam.AllowFiltering = 1	
	group by psam.SpecificationAttributeOptionId
		
 
	SELECT @PositiveQuantityCount = count(*)
	FROM #PageIndex [pi]
			JOIN Product p on p.Id = [pi].[ProductId]
			JOIN ProductVariant pv with (NOLOCK)
		ON p.Id = pv.ProductId
	where pv.AvailableForPreOrder = 0
			
		
	DROP TABLE #PageIndex
END


go

---------------------------------------------------------------------------------------------------------------------------------------------

DECLARE @RC int
DECLARE @CategoryIds nvarchar(max)
DECLARE @PriceMin decimal(18,4)
DECLARE @PriceMax decimal(18,4)
DECLARE @FilteredSpecs nvarchar(max)
DECLARE @ShowWithPositiveQuantity bit
declare @PositiveQuantityCount int

set @CategoryIds = '23'
set @PriceMin = 1000
set @PriceMax = 1999
set @FilteredSpecs = '697, 699'
set @ShowWithPositiveQuantity = 1

EXECUTE @RC = [dbo].[ProductLoadAllPaged2] 
   @CategoryIds
  ,@PriceMin
  ,@PriceMax
  ,@FilteredSpecs
  ,@ShowWithPositiveQuantity
  ,@PositiveQuantityCount out


  select @PositiveQuantityCount
 
GO