using Application.UseCases.Schools.Commands;
using Application.UseCases.Schools.Queries;
using Application.UseCases.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnitTest.Integration;

public class SchoolCqrsIntegrationTests
{
    [Fact]
    public async Task CqrsHandlers_Should_Create_Update_And_Delete_School_UsingRealDbContext()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase($"schools-cqrs-{Guid.NewGuid():N}")
            .Options;

        await using var db = new SchoolDbContext(options);
        var repository = new SchoolRepository(db);
        var service = new SchoolService(repository, NullLogger<SchoolService>.Instance);

        var createHandler = new CreateSchoolCommandHandler(service);
        var getByIdHandler = new GetSchoolByIdQueryHandler(service);
        var getAllHandler = new GetAllSchoolsQueryHandler(service);
        var updateHandler = new UpdateSchoolCommandHandler(service);
        var deleteHandler = new DeleteSchoolCommandHandler(service);

        var created = await createHandler.HandleAsync(new CreateSchoolCommand("sc01", "School 1", "BCN", false, null));

        Assert.True(created.Id > 0);
        Assert.Equal("SC01", created.Code);

        var found = await getByIdHandler.HandleAsync(new GetSchoolByIdQuery(created.Id));
        Assert.NotNull(found);
        Assert.Equal("School 1", found!.Name);

        var updated = await updateHandler.HandleAsync(new UpdateSchoolCommand(created.Id, "sc02", "School 2", "GIR", true, null));
        Assert.True(updated);

        var all = (await getAllHandler.HandleAsync(new GetAllSchoolsQuery())).ToList();
        Assert.Single(all);
        Assert.Equal("SC02", all[0].Code);
        Assert.Equal("School 2", all[0].Name);

        var deleted = await deleteHandler.HandleAsync(new DeleteSchoolCommand(created.Id));
        Assert.True(deleted);

        await Assert.ThrowsAsync<Domain.DomainExceptions.NotFoundException>(
            async () => await getByIdHandler.HandleAsync(new GetSchoolByIdQuery(created.Id)));
    }
}
