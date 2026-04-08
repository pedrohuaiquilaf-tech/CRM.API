namespace CRM.UnitTests.Features.Contacts.Commands;

using CRM.Application.Features.Contacts.Commands;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Moq;

public class CreateContactCommandHandlerTests
{
    private readonly Mock<IRepository<Contact>> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateContactCommandHandler _sut;

    public CreateContactCommandHandlerTests()
    {
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact c, CancellationToken _) => c);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new CreateContactCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNonEmptyGuid()
    {
        var command = new CreateContactCommand("Alice", "Smith", "alice@example.com", null, null, null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsAddAsyncOnce()
    {
        var command = new CreateContactCommand("Alice", "Smith", "alice@example.com", null, null, null);

        await _sut.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChangesOnce()
    {
        var command = new CreateContactCommand("Alice", "Smith", "alice@example.com", null, null, null);

        await _sut.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_MapsFieldsCorrectly()
    {
        var companyId = Guid.NewGuid();
        var command = new CreateContactCommand("Bob", "Jones", "bob@example.com", "+1234567890", "VIP", companyId);
        Contact? captured = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
            .Callback<Contact, CancellationToken>((c, _) => captured = c)
            .ReturnsAsync((Contact c, CancellationToken _) => c);

        await _sut.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.FirstName.Should().Be("Bob");
        captured.LastName.Should().Be("Jones");
        captured.Email.Should().Be("bob@example.com");
        captured.Phone.Should().Be("+1234567890");
        captured.Notes.Should().Be("VIP");
        captured.CompanyId.Should().Be(companyId);
    }
}
