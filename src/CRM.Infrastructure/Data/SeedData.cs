namespace CRM.Infrastructure.Data;

using CRM.Domain.Common;
using CRM.Infrastructure.Security;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class SeedData
{
    public static async Task InitialiseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }

        if (await context.Users.AnyAsync())
            return;

        logger.LogInformation("Seeding database with initial data...");

        // ── Users ──────────────────────────────────────────────────────────────
        var users = new[]
        {
            new User { Id = Guid.NewGuid(), Email = "admin@crm.demo",  PasswordHash = PasswordHasher.Hash("Admin123!"),  Role = Roles.Admin,  CreatedAt = DateTime.UtcNow },
            new User { Id = Guid.NewGuid(), Email = "sales@crm.demo",  PasswordHash = PasswordHasher.Hash("Sales123!"),  Role = Roles.Sales,  CreatedAt = DateTime.UtcNow },
            new User { Id = Guid.NewGuid(), Email = "viewer@crm.demo", PasswordHash = PasswordHasher.Hash("Viewer123!"), Role = Roles.Viewer, CreatedAt = DateTime.UtcNow },
        };
        await context.Users.AddRangeAsync(users);

        // ── Companies ──────────────────────────────────────────────────────────
        var companies = new[]
        {
            new Company { Id = Guid.NewGuid(), Name = "Acme Corp",        Industry = "Technology",    Size = "200-500",   Website = "https://acme.example.com",    CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Globex Inc",       Industry = "Manufacturing", Size = "500-1000",  Website = "https://globex.example.com",  CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Initech Solutions", Industry = "Finance",      Size = "50-200",    Website = "https://initech.example.com", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Umbrella Ltd",     Industry = "Healthcare",    Size = "1000+",     Website = "https://umbrella.example.com",CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Stark Industries",  Industry = "Aerospace",    Size = "1000+",     Website = "https://stark.example.com",   CreatedAt = DateTime.UtcNow },
        };
        await context.Companies.AddRangeAsync(companies);

        // ── Contacts ───────────────────────────────────────────────────────────
        var contacts = new[]
        {
            new Contact { Id = Guid.NewGuid(), FirstName = "Alice",   LastName = "Johnson", Email = "alice@acme.example.com",    Phone = "+1-555-0101", CompanyId = companies[0].Id, Notes = "Key decision maker",          CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Bob",     LastName = "Smith",   Email = "bob@acme.example.com",      Phone = "+1-555-0102", CompanyId = companies[0].Id, Notes = "Evaluating Q3 budget",        CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Carol",   LastName = "Davis",   Email = "carol@globex.example.com",  Phone = "+1-555-0103", CompanyId = companies[1].Id, Notes = "Primary procurement contact", CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "David",   LastName = "Wilson",  Email = "david@globex.example.com",  Phone = "+1-555-0104", CompanyId = companies[1].Id, Notes = "Technical lead",              CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Eve",     LastName = "Martinez",Email = "eve@initech.example.com",   Phone = "+1-555-0105", CompanyId = companies[2].Id, Notes = "CFO",                         CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Frank",   LastName = "Brown",   Email = "frank@initech.example.com", Phone = "+1-555-0106", CompanyId = companies[2].Id, Notes = "IT manager",                  CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Grace",   LastName = "Taylor",  Email = "grace@umbrella.example.com",Phone = "+1-555-0107", CompanyId = companies[3].Id, Notes = "Procurement director",        CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Henry",   LastName = "Anderson",Email = "henry@umbrella.example.com",Phone = "+1-555-0108", CompanyId = companies[3].Id, Notes = "CTO",                         CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Isabella",LastName = "Thomas",  Email = "isabella@stark.example.com",Phone = "+1-555-0109", CompanyId = companies[4].Id, Notes = "Head of R&D",                 CreatedAt = DateTime.UtcNow },
            new Contact { Id = Guid.NewGuid(), FirstName = "Jack",    LastName = "Jackson", Email = "jack@stark.example.com",    Phone = "+1-555-0110", CompanyId = companies[4].Id, Notes = "CEO",                         CreatedAt = DateTime.UtcNow },
        };
        await context.Contacts.AddRangeAsync(contacts);

        // ── Opportunities ──────────────────────────────────────────────────────
        var opportunities = new[]
        {
            new Opportunity { Id = Guid.NewGuid(), Title = "Acme Platform Licence",     Amount = 85_000m,  Stage = PipelineStage.Proposal,     Probability = 60, ExpectedCloseDate = DateTime.UtcNow.AddDays(30),  ContactId = contacts[0].Id, CompanyId = companies[0].Id, CreatedAt = DateTime.UtcNow },
            new Opportunity { Id = Guid.NewGuid(), Title = "Globex ERP Upgrade",         Amount = 240_000m, Stage = PipelineStage.Negotiation,  Probability = 75, ExpectedCloseDate = DateTime.UtcNow.AddDays(14),  ContactId = contacts[2].Id, CompanyId = companies[1].Id, CreatedAt = DateTime.UtcNow },
            new Opportunity { Id = Guid.NewGuid(), Title = "Initech Analytics Suite",    Amount = 42_500m,  Stage = PipelineStage.Qualified,    Probability = 40, ExpectedCloseDate = DateTime.UtcNow.AddDays(60),  ContactId = contacts[4].Id, CompanyId = companies[2].Id, CreatedAt = DateTime.UtcNow },
            new Opportunity { Id = Guid.NewGuid(), Title = "Umbrella Compliance Module", Amount = 130_000m, Stage = PipelineStage.Lead,         Probability = 20, ExpectedCloseDate = DateTime.UtcNow.AddDays(90),  ContactId = contacts[6].Id, CompanyId = companies[3].Id, CreatedAt = DateTime.UtcNow },
            new Opportunity { Id = Guid.NewGuid(), Title = "Stark Cloud Migration",      Amount = 500_000m, Stage = PipelineStage.ClosedWon,    Probability = 100, ExpectedCloseDate = DateTime.UtcNow.AddDays(-7), ContactId = contacts[8].Id, CompanyId = companies[4].Id, CreatedAt = DateTime.UtcNow },
        };
        await context.Opportunities.AddRangeAsync(opportunities);

        // ── Activities ─────────────────────────────────────────────────────────
        var activities = new[]
        {
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Call,    Subject = "Initial discovery call",        Description = "Discussed requirements and timeline.",       ActivityDate = DateTime.UtcNow.AddDays(-20), ContactId = contacts[0].Id, OpportunityId = opportunities[0].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Email,   Subject = "Proposal follow-up",            Description = "Sent revised pricing document.",             ActivityDate = DateTime.UtcNow.AddDays(-15), ContactId = contacts[0].Id, OpportunityId = opportunities[0].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Meeting, Subject = "ERP demo session",              Description = "Live demo with Globex technical team.",      ActivityDate = DateTime.UtcNow.AddDays(-10), ContactId = contacts[2].Id, OpportunityId = opportunities[1].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Call,    Subject = "Negotiation check-in",          Description = "Aligned on contract terms.",                  ActivityDate = DateTime.UtcNow.AddDays(-5),  ContactId = contacts[3].Id, OpportunityId = opportunities[1].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Email,   Subject = "Analytics use-case overview",   Description = "Shared case studies relevant to Initech.",   ActivityDate = DateTime.UtcNow.AddDays(-30), ContactId = contacts[4].Id, OpportunityId = opportunities[2].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Meeting, Subject = "Compliance requirements review", Description = "Workshop with Umbrella legal and IT teams.", ActivityDate = DateTime.UtcNow.AddDays(-45), ContactId = contacts[6].Id, OpportunityId = opportunities[3].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Call,    Subject = "Kick-off planning call",         Description = "Agreed migration phases and go-live dates.", ActivityDate = DateTime.UtcNow.AddDays(-3),  ContactId = contacts[8].Id, OpportunityId = opportunities[4].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Email,   Subject = "Contract signed confirmation",   Description = "Sent fully executed contract to Stark.",     ActivityDate = DateTime.UtcNow.AddDays(-6),  ContactId = contacts[9].Id, OpportunityId = opportunities[4].Id, CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Meeting, Subject = "Quarterly business review",      Description = "Reviewed pipeline with Acme leadership.",    ActivityDate = DateTime.UtcNow.AddDays(-60), ContactId = contacts[1].Id, OpportunityId = null,                CreatedAt = DateTime.UtcNow },
            new Activity { Id = Guid.NewGuid(), Type = ActivityType.Call,    Subject = "Cold outreach — Globex CFO",    Description = "Left voicemail; follow up next week.",        ActivityDate = DateTime.UtcNow.AddDays(-7),  ContactId = contacts[3].Id, OpportunityId = null,                CreatedAt = DateTime.UtcNow },
        };
        await context.Activities.AddRangeAsync(activities);

        await context.SaveChangesAsync();
        logger.LogInformation("Database seeded successfully.");
    }
}
