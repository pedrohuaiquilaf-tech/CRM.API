namespace CRM.UnitTests.Features.Contacts.Queries;

using CRM.Application.Features.Contacts.Queries;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using FluentAssertions;
using Moq;

public class GetContactByIdQueryHandlerTests
{
    private readonly Mock<IRepository<Contact>> _repositoryMock = new();
    private readonly GetContactByIdQueryHandler _sut;

    public GetContactByIdQueryHandlerTests()
    {
        _sut = new GetContactByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingContact_ReturnsMappedDto()
    {
        var contactId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var contact = new Contact
        {
            Id = contactId,
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Phone = "+1234567890",
            Notes = "VIP",
            CompanyId = companyId,
            CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        var result = await _sut.Handle(new GetContactByIdQuery(contactId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(contactId);
        result.FirstName.Should().Be("Alice");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("alice@example.com");
        result.Phone.Should().Be("+1234567890");
        result.Notes.Should().Be("VIP");
        result.CompanyId.Should().Be(companyId);
        result.CompanyName.Should().BeNull(); // no navigation property loaded
        result.CreatedAt.Should().Be(contact.CreatedAt);
    }

    [Fact]
    public async Task Handle_NonExistentContact_ReturnsNull()
    {
        var missingId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact?)null);

        var result = await _sut.Handle(new GetContactByIdQuery(missingId), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ExistingContact_CallsGetByIdAsyncOnce()
    {
        var contactId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact?)null);

        await _sut.Handle(new GetContactByIdQuery(contactId), CancellationToken.None);

        _repositoryMock.Verify(
            r => r.GetByIdAsync(contactId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ContactWithCompany_MapsCompanyName()
    {
        var contactId = Guid.NewGuid();
        var company = new Company { Id = Guid.NewGuid(), Name = "Acme Corp" };
        var contact = new Contact
        {
            Id = contactId,
            FirstName = "Bob",
            LastName = "Jones",
            Email = "bob@example.com",
            CompanyId = company.Id,
            Company = company,
            CreatedAt = DateTime.UtcNow,
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(contactId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        var result = await _sut.Handle(new GetContactByIdQuery(contactId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.CompanyName.Should().Be("Acme Corp");
    }
}
