using FSPBook.Data;
using FSPBook.Data.Entities;
using FSPBook.Pages;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FSPTest
{
    public class IndexModelTests
    {
        private readonly HttpClient _httpClientMock;
        private readonly Context _dbContextMock;
        private readonly IndexModel _indexModelMock;

        public IndexModelTests()
        {
            _httpClientMock = Substitute.For<HttpClient>();

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContextMock = new Context(options);

            _indexModelMock = Substitute.ForPartsOf<IndexModel>(_dbContextMock, _httpClientMock);
        }

        [Fact]
        public async Task OnGet_ShouldPopulatePostViewModel()
        {
            // Arrange
            var author = new Profile { Id = 1, FirstName = "Test", LastName = "Data" };

            _dbContextMock.Post.AddRange(
                new Post { Content = "Post 1", Author = author, AuthorId = 1, DateTimePosted = DateTime.Now.AddHours(-1) },
                new Post { Content = "Post 2", Author = author, AuthorId = 1, DateTimePosted = DateTime.Now.AddHours(-2) }
            );
            _dbContextMock.SaveChanges();

            var mockFeeds = new List<Feed>
            {
                new Feed { Title = "Feed 1", Description = "Description 1", Url = "http://example.com/feed1" },
                new Feed { Title = "Feed 2", Description = "Description 2", Url = "http://example.com/feed2" }
            };


            _indexModelMock.GetFeeds().Returns(mockFeeds);

            // Act
            await _indexModelMock.OnGet();

            // Assert
            Assert.NotNull(_indexModelMock.PostViewModel);
            Assert.Equal(2, _indexModelMock.PostViewModel.Posts.Count);
            Assert.Equal(2, _indexModelMock.PostViewModel.Feeds.Count);
        }

    }
}