USE [nopCommerceVoda2]
GO

alter PROCEDURE [dbo].[GetPriceAmountForFilter]
(
	@CategoryIds nvarchar(100),	--a list of category IDs (comma-separated list). e.g. 1,2,3						
	@FilteredSpecs nvarchar(100),	--filter by attributes (comma-separated list). e.g. 14,15,16	
	@ShowWithPositiveQuantity bit	
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
		Price int 
	)

	SET @sql = '
	INSERT INTO #DisplayOrderTmp ([Price])
	SELECT pv.Price
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
		p.Deleted = 0
		and pv.Price > 0'
	
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
	
	
	SELECT pi.Price
	FROM #DisplayOrderTmp [pi]
	
			
	DROP TABLE #DisplayOrderTmp	
 
	
			
END


go

---------------------------------------------------------------------------------------------------------------------------------------------



DECLARE @RC int
DECLARE @CategoryIds nvarchar(max)
DECLARE @FilteredSpecs nvarchar(max)
DECLARE @ShowWithPositiveQuantity bit

set @CategoryIds = N'58'
set @FilteredSpecs = null
set @ShowWithPositiveQuantity = 0

EXECUTE @RC = [dbo].[GetPriceAmountForFilter] 
   @CategoryIds
  ,@FilteredSpecs
  ,@ShowWithPositiveQuantity

   
GO