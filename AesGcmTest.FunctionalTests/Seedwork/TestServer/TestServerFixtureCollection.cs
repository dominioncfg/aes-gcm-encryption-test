using Xunit;

namespace AesGcmTest.FunctionalTests.Seedwork;

[CollectionDefinition(nameof(TestServerFixtureCollection))]
public class TestServerFixtureCollection : ICollectionFixture<TestServerFixture> { }
