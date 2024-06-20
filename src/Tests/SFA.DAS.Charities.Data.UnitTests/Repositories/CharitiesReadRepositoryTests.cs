using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using SFA.DAS.Charities.Data.Repositories;
using FluentAssertions;
using SFA.DAS.Charities.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace SFA.DAS.Charities.Data.UnitTests.Repositories
{
    [TestFixture]
    public class CharitiesReadRepositoryTests
    {
        private CharitiesDataContext _context;
        private Fixture _fixture;
        private CharitiesReadRepository _sut;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            var options = new DbContextOptionsBuilder<CharitiesDataContext>()
                .UseInMemoryDatabase("CharitiesDataContext" + Guid.NewGuid()).Options;
            
            _context = new CharitiesDataContext(options);

            _sut = new CharitiesReadRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetCharityById_ShouldReturnCharity_WhenCharityExists()
        {
            // Arrange
            var registrationNumber = 12345;
            var expectedCharity = _fixture.Build<Charity>()
                                          .With(c => c.RegistrationNumber, registrationNumber)
                                          .With(c => c.LinkedCharityId, 0)
                                          .Create();

            await _context.Charities.AddAsync(expectedCharity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetCharityById(registrationNumber);

            // Assert
            result.Should().BeEquivalentTo(expectedCharity);
        }

        [Test]
        public async Task GetCharityById_ShouldReturnNull_WhenCharityDoesNotExist()
        {
            // Arrange
            var registrationNumber = 12345;

            // Act
            var result = await _sut.GetCharityById(registrationNumber);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task SearchCharities_ShouldReturnCharities_WhenSearchTermMatches()
        {
            // Arrange
            var searchTerm = "charity";
            var expectedCharities = _fixture.Build<Charity>()
                                            .With(c => c.RegistrationStatus, RegistrationStatus.Registered)
                                            .With(c => c.LinkedCharityId, 0)
                                            .With(c => c.Name, $"Test {searchTerm} Name")
                                            .CreateMany(3)
                                            .ToList();

            await _context.Charities.AddRangeAsync(expectedCharities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.SearchCharities(searchTerm);

            // Assert
            result.Should().BeEquivalentTo(expectedCharities);
        }

        [Test]
        public async Task SearchCharities_ShouldReturnEmptyList_WhenSearchTermIsWhitespace()
        {
            // Act
            var result = await _sut.SearchCharities(" ");

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task SearchCharities_ShouldReturnEmptyList_WhenNoCharitiesMatchSearchTerm()
        {
            // Arrange
            var searchTerm = "nonexistent";

            // Act
            var result = await _sut.SearchCharities(searchTerm);

            // Assert
            result.Should().BeEmpty();
        }

    }
}
