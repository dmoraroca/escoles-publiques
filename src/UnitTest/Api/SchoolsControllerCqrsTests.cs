using Api.Controllers;
using Api.Contracts;
using Application.Interfaces.Cqrs;
using Application.UseCases.Schools.Commands;
using Application.UseCases.Schools.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Api;

public class SchoolsControllerCqrsTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var getAll = new Mock<IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>>>();
        getAll.Setup(x => x.HandleAsync(It.IsAny<GetAllSchoolsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<School> { new() { Id = 1, Code = "SC1", Name = "School", CreatedAt = DateTime.UtcNow } });

        var controller = CreateController(getAll.Object, Mock.Of<IQueryHandler<GetSchoolByIdQuery, School?>>(), Mock.Of<ICommandHandler<CreateSchoolCommand, School>>(), Mock.Of<ICommandHandler<UpdateSchoolCommand, bool>>(), Mock.Of<ICommandHandler<DeleteSchoolCommand, bool>>());

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenNull()
    {
        var getById = new Mock<IQueryHandler<GetSchoolByIdQuery, School?>>();
        getById.Setup(x => x.HandleAsync(It.IsAny<GetSchoolByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School?)null);

        var controller = CreateController(Mock.Of<IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>>>(), getById.Object, Mock.Of<ICommandHandler<CreateSchoolCommand, School>>(), Mock.Of<ICommandHandler<UpdateSchoolCommand, bool>>(), Mock.Of<ICommandHandler<DeleteSchoolCommand, bool>>());

        var result = await controller.Get(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var create = new Mock<ICommandHandler<CreateSchoolCommand, School>>();
        create.Setup(x => x.HandleAsync(It.IsAny<CreateSchoolCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new School { Id = 7, Code = "SC7", Name = "S7", CreatedAt = DateTime.UtcNow });

        var controller = CreateController(Mock.Of<IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>>>(), Mock.Of<IQueryHandler<GetSchoolByIdQuery, School?>>(), create.Object, Mock.Of<ICommandHandler<UpdateSchoolCommand, bool>>(), Mock.Of<ICommandHandler<DeleteSchoolCommand, bool>>());

        var result = await controller.Create(new SchoolDto(null, "sc7", "S7", null, false, null));

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenHandlerReturnsFalse()
    {
        var update = new Mock<ICommandHandler<UpdateSchoolCommand, bool>>();
        update.Setup(x => x.HandleAsync(It.IsAny<UpdateSchoolCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = CreateController(Mock.Of<IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>>>(), Mock.Of<IQueryHandler<GetSchoolByIdQuery, School?>>(), Mock.Of<ICommandHandler<CreateSchoolCommand, School>>(), update.Object, Mock.Of<ICommandHandler<DeleteSchoolCommand, bool>>());

        var result = await controller.Update(11, new SchoolDto(11, "sc", "n", null, false, null));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var delete = new Mock<ICommandHandler<DeleteSchoolCommand, bool>>();
        delete.Setup(x => x.HandleAsync(It.IsAny<DeleteSchoolCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var controller = CreateController(Mock.Of<IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>>>(), Mock.Of<IQueryHandler<GetSchoolByIdQuery, School?>>(), Mock.Of<ICommandHandler<CreateSchoolCommand, School>>(), Mock.Of<ICommandHandler<UpdateSchoolCommand, bool>>(), delete.Object);

        var result = await controller.Delete(5);

        Assert.IsType<NoContentResult>(result);
    }

    private static SchoolsController CreateController(
        IQueryHandler<GetAllSchoolsQuery, IEnumerable<School>> getAll,
        IQueryHandler<GetSchoolByIdQuery, School?> getById,
        ICommandHandler<CreateSchoolCommand, School> create,
        ICommandHandler<UpdateSchoolCommand, bool> update,
        ICommandHandler<DeleteSchoolCommand, bool> delete)
        => new(getAll, getById, create, update, delete);
}
