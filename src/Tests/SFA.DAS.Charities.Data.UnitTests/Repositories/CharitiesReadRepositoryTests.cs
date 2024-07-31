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
            var registrationNumber = 12345;
            var expectedCharity = _fixture.Build<Charity>()
                                          .With(c => c.RegistrationNumber, registrationNumber)
                                          .With(c => c.LinkedCharityId, 0)
                                          .Create();

            await _context.Charities.AddAsync(expectedCharity);
            await _context.SaveChangesAsync();
         
            var result = await _sut.GetCharityById(registrationNumber);        
            result.Should().BeEquivalentTo(expectedCharity);
        }

        [Test]
        public async Task GetCharityById_ShouldReturnNull_WhenCharityDoesNotExist()
        {          
            var registrationNumber = 12345;  
            var result = await _sut.GetCharityById(registrationNumber);
            result.Should().BeNull();
        }

        [Test]
        public async Task SearchCharities_ShouldReturnCharities_WhenSearchTermMatches()
        {       
            var searchTerm = "charity";
            var expectedCharities = _fixture.Build<Charity>()
                                            .With(c => c.RegistrationStatus, RegistrationStatus.Registered)
                                            .With(c => c.LinkedCharityId, 0)
                                            .With(c => c.Name, $"Test {searchTerm} Name")
                                            .CreateMany(3)
                                            .ToList();

            await _context.Charities.AddRangeAsync(expectedCharities);
            await _context.SaveChangesAsync();
            
            var result = await _sut.SearchCharities(searchTerm);           
            result.Should().BeEquivalentTo(expectedCharities);
        }

        [Test]
        public async Task SearchCharities_ShouldReturnEmptyList_WhenSearchTermIsWhitespace()
        {            
            var result = await _sut.SearchCharities(" ");
            result.Should().BeEmpty();
        }

        [Test]
        public async Task SearchCharities_ShouldReturnEmptyList_WhenNoCharitiesMatchSearchTerm()
        {            
            var searchTerm = "nonexistent";   
            var result = await _sut.SearchCharities(searchTerm);
            result.Should().BeEmpty();
        }
    }
}
