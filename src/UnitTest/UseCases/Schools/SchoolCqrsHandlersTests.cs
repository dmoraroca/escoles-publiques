using Application.Interfaces;
using Application.UseCases.Schools.Commands;
using Application.UseCases.Schools.Queries;
using Domain.Entities;
using Moq;

namespace UnitTest.UseCases.Schools;

public class SchoolCqrsHandlersTests
{
    [Fact]
    public async Task QueryHandlers_DelegateToService()
    {
        var service = new Mock<ISchoolService>();
        service.Setup(s => s.GetAllSchoolsAsync()).ReturnsAsync(new List<School>());
        service.Setup(s => s.GetSchoolByIdAsync(1)).ReturnsAsync(new School { Id = 1, Code = "S", Name = "N" });
        service.Setup(s => s.GetSchoolByCodeAsync("S")).ReturnsAsync(new School { Id = 1, Code = "S", Name = "N" });

        var allHandler = new GetAllSchoolsQueryHandler(service.Object);
        var byIdHandler = new GetSchoolByIdQueryHandler(service.Object);
        var byCodeHandler = new GetSchoolByCodeQueryHandler(service.Object);

        _ = await allHandler.HandleAsync(new GetAllSchoolsQuery());
        _ = await byIdHandler.HandleAsync(new GetSchoolByIdQuery(1));
        _ = await byCodeHandler.HandleAsync(new GetSchoolByCodeQuery("S"));

        service.Verify(s => s.GetAllSchoolsAsync(), Times.Once);
        service.Verify(s => s.GetSchoolByIdAsync(1), Times.Once);
        service.Verify(s => s.GetSchoolByCodeAsync("S"), Times.Once);
    }

    [Fact]
    public async Task CommandHandlers_CreateUpdateDelete_DelegateToService()
    {
        var service = new Mock<ISchoolService>();
        service.Setup(s => s.CreateSchoolAsync(It.IsAny<School>()))
            .ReturnsAsync((School s) =>
            {
                s.Id = 10;
                return s;
            });
        service.Setup(s => s.GetSchoolByIdAsync(10)).ReturnsAsync(new School { Id = 10, Code = "A", Name = "B" });

        var createHandler = new CreateSchoolCommandHandler(service.Object);
        var updateHandler = new UpdateSchoolCommandHandler(service.Object);
        var deleteHandler = new DeleteSchoolCommandHandler(service.Object);

        var created = await createHandler.HandleAsync(new CreateSchoolCommand("A", "B", null, false, null));
        var updated = await updateHandler.HandleAsync(new UpdateSchoolCommand(10, "A1", "B1", null, true, null));
        var deleted = await deleteHandler.HandleAsync(new DeleteSchoolCommand(10));

        Assert.Equal(10, created.Id);
        Assert.True(updated);
        Assert.True(deleted);
        service.Verify(s => s.UpdateSchoolAsync(It.IsAny<School>()), Times.Once);
        service.Verify(s => s.DeleteSchoolAsync(10), Times.Once);
    }
}
