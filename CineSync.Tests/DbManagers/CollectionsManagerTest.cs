using System.Linq.Expressions;
using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using JetBrains.Annotations;
using Moq;

namespace CineSync.Tests.DbManagers;

[TestSubject(typeof(CollectionsManager))]
public class CollectionsManagerTest
{

    [Fact]
    public async Task InitializeUserCollectionsAsync_ValidUser_CreatesDefaultCollections()
    {
        var mockUserRepo = new Mock<IRepositoryAsync<ApplicationUser>>();
        var mockUnitOfWork = new Mock<IUnitOfWorkAsync>();
        var logger = new Mock<ILoggerStrategy>();

        var user = new ApplicationUser { Id = "user1", Collections = new List<MovieCollection>() };
        mockUserRepo.Setup(repo => repo.GetFirstByConditionAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync(user);
        mockUnitOfWork.Setup(uow => uow.GetRepositoryAsync<ApplicationUser>()).Returns(mockUserRepo.Object);
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(true);

        var manager = new CollectionsManager(mockUnitOfWork.Object, logger.Object);

        var result = await manager.InitializeUserCollectionsAsync("user1");

        Assert.True(result);
        Assert.Equal(4, user.Collections.Count);
        Assert.Contains(user.Collections, c => c.Name == "Favorites");
        Assert.Contains(user.Collections, c => c.Name == "Watched");
        Assert.Contains(user.Collections, c => c.Name == "Classified");
        Assert.Contains(user.Collections, c => c.Name == "Watch Later");
    }
    
    [Fact]
    public async Task InitializeUserCollectionsAsync_UserNotFound_ThrowsException()
    {
        var mockUserRepo = new Mock<IRepositoryAsync<ApplicationUser>>();
        var mockUnitOfWork = new Mock<IUnitOfWorkAsync>();
        var logger = new Mock<ILoggerStrategy>();

        mockUserRepo.Setup(repo => repo.GetFirstByConditionAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
            .ReturnsAsync((ApplicationUser)null);
        mockUnitOfWork.Setup(uow => uow.GetRepositoryAsync<ApplicationUser>()).Returns(mockUserRepo.Object);

        var manager = new CollectionsManager(mockUnitOfWork.Object, logger.Object);


        await Assert.ThrowsAsync<Exception>(() => manager.InitializeUserCollectionsAsync("invalidUserId"));
    } 
}