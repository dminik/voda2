// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Parser.Tests
{	
	using Moq;

	using NUnit.Framework;

	using Nop.Core;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.SiteParsers;
	using Nop.Services.YandexMarket;

	[TestFixture]
	public class TestYandexMarketSpecMapper
	{
		private YandexMarketSpecMapper _mapperUnderTest;

		[SetUp]
		public void Init()
		{
			var yandexMarketSpecServiceMoq = new Mock<IYandexMarketSpecService>();
			var shopSpecServiceMoq = new Mock<ISpecificationAttributeService>();

			// set fake return value
			yandexMarketSpecServiceMoq.Setup(
				foo =>
				foo.GetByCategory(
					It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())) // input params
							 .Returns(() => (YaSpecTestData()));

			shopSpecServiceMoq.Setup(
				foo =>
				foo.GetSpecificationAttributes()) // input params
							 .Returns(() => (ShopSpecTestData()));

			_mapperUnderTest = new YandexMarketSpecMapper(yandexMarketSpecServiceMoq.Object, shopSpecServiceMoq.Object);
		}

		#region Tests

		[Test]
		public void GetSumOfYandexSpecsAndShopSpecs_Success()

		{
			// Arrange
			const int categoryId = 1;

			// Act 
			// Возвращает спецификаций магазина, дополненные из Яндекса
			var resultAllShopSpecs = _mapperUnderTest.GetSumOfYandexSpecsAndShopSpecs(categoryId);

			// Assert 
			Assert.That(resultAllShopSpecs.Count, Is.EqualTo(3)); // Color, Size, Sexy

			var colorSpec = resultAllShopSpecs.FirstOrDefault(x => x.Name == "Color");
			Assert.That(colorSpec != null);
			Assert.That(colorSpec.SpecificationAttributeOptions.Count, Is.EqualTo(3)); // Red, White, Black

			var sizeSpec = resultAllShopSpecs.FirstOrDefault(x => x.Name == "Size");
			Assert.That(sizeSpec != null);
			Assert.That(sizeSpec.SpecificationAttributeOptions.Count, Is.EqualTo(2)); // Small, Big
		}

		[Test]
		public void GetNewYandexSpecsOnly_Success()
		{
			// Act
			var result = _mapperUnderTest.GetNewYandexSpecsOnly(SumOfYandexSpecsAndShopSpecs());

			// Assert 
			Assert.That(result.Count, Is.EqualTo(2)); // Color, Size

			var colorSpec = result.FirstOrDefault(x => x.Name == "Color");
			Assert.That(colorSpec != null);
			Assert.That(colorSpec.SpecificationAttributeOptions.Count, Is.EqualTo(2)); // Red, Black

			var sizeSpec = result.FirstOrDefault(x => x.Name == "Size");
			Assert.That(sizeSpec != null);
			Assert.That(sizeSpec.SpecificationAttributeOptions.Count, Is.EqualTo(2)); // Small, Big
		}

		#endregion


		#region Private methods

		private IList<SpecificationAttribute> ShopSpecTestData()
		{
			var result = new List<SpecificationAttribute>()
				{
					new SpecificationAttributeTest()
						{
							Id = 1,
							Name = "Color",
							SpecificationAttributeOptions_Test =
								new List<SpecificationAttributeOption>()
									{
										new SpecificationAttributeOption() { Name = "Red", Id = 1 },
										new SpecificationAttributeOption() { Name = "White", Id = 2 }
									}
						},
					new SpecificationAttributeTest()
						{
							Id = 2,
							Name = "Sexy",
							SpecificationAttributeOptions_Test =
								new List<SpecificationAttributeOption>()
									{
										new SpecificationAttributeOption() { Name = "Yes", Id = 3 },
										new SpecificationAttributeOption() { Name = "No", Id = 4 }
									}
						}
				};

			return result;
		}

		private IPagedList<YandexMarketSpecRecord> YaSpecTestData()
		{
			var result = new List<YandexMarketSpecRecord>()
				{
					new YandexMarketSpecRecord("Color", "Red"),
					new YandexMarketSpecRecord("Color", "Black"),
					new YandexMarketSpecRecord("Size", "Small"),
					new YandexMarketSpecRecord("Size", "Big")
				};

			return new PagedList<YandexMarketSpecRecord>(result, 0, int.MaxValue);
		}

		private IList<SpecificationAttribute> SumOfYandexSpecsAndShopSpecs()
		{
			var result = new List<SpecificationAttribute>()
				{
					new SpecificationAttributeTest()
						{
							Id = 1,
							Name = "Color",
							SpecificationAttributeOptions_Test =
								new List<SpecificationAttributeOption>()
									{
										new SpecificationAttributeOption() { Name = "Red", Id = 1 },
										new SpecificationAttributeOption() { Name = "White", Id = 2 },
										new SpecificationAttributeOption() { Name = "Red" },
										new SpecificationAttributeOption() { Name = "Black" },
									}
						},
					new SpecificationAttributeTest()
						{
							Id = 2,
							Name = "Sexy",
							SpecificationAttributeOptions_Test =
								new List<SpecificationAttributeOption>()
									{
										new SpecificationAttributeOption() { Name = "Yes", Id = 3 },
										new SpecificationAttributeOption() { Name = "No", Id = 4 }
									}
						},
					new SpecificationAttributeTest()
						{
							Name = "Size",
							SpecificationAttributeOptions_Test =
								new List<SpecificationAttributeOption>()
									{
										new SpecificationAttributeOption() { Name = "Small" },
										new SpecificationAttributeOption() { Name = "Big" }
									}
						}
				};

			return result;
		}

		class SpecificationAttributeTest : SpecificationAttribute
		{
			public ICollection<SpecificationAttributeOption> SpecificationAttributeOptions_Test
			{
				get { return base.SpecificationAttributeOptions; }
				set { base.SpecificationAttributeOptions = value; }
			}

		}

		#endregion

	}

}
// ReSharper restore InconsistentNaming