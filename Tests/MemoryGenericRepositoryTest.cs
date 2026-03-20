using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Models;
using AppCore.Repositories;
using AppCore.Enums;
using Infrastructure.Memory;
using Xunit;

using Assert = Xunit.Assert;

namespace Tests;

public class MemoryGenericRepositoryTest
{
    private readonly IGenericRepositoryAsync<Person> _repo;

    public MemoryGenericRepositoryTest()
    {
        _repo = new MemoryGenericRepository<Person>();
    }

    [Fact]
    public async Task AddPersonTestAsync()
    {
        var expected = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam@test.com",
            Phone = "123456789",
            Gender = Gender.Male,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        
        var added = await _repo.AddAsync(expected);
        
        var actual = await _repo.FindByIdAsync(expected.Id);
        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
    }

    [Fact]
    public async Task FindById_ReturnsNull_WhenNotFound()
    {
        
        var found = await _repo.FindByIdAsync(Guid.NewGuid());
        
        Assert.Null(found);
    }

    [Fact]
    public async Task FindAllTestAsync()
    {
        
        var person1 = new Person { Id = Guid.NewGuid(), FirstName = "Anna" };
        var person2 = new Person { Id = Guid.NewGuid(), FirstName = "Piotr" };
        
        await _repo.AddAsync(person1);
        await _repo.AddAsync(person2);
        
        var all = await _repo.FindAllAsync();
        
        Assert.NotNull(all);
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task FindPagedTestAsync()
    {
        for (int i = 1; i <= 25; i++)
        {
            await _repo.AddAsync(new Person 
            { 
                Id = Guid.NewGuid(), 
                FirstName = $"Person{i}" 
            });
        }
        
        var page = await _repo.FindPagedAsync(2, 10);
        
        Assert.NotNull(page);
        Assert.Equal(10, page.Items.Count);
        Assert.Equal(25, page.TotalCount);
        Assert.Equal(2, page.Page);
        Assert.Equal(10, page.PageSize);
    }

    [Fact]
    public async Task UpdateTestAsync()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = "Maria",
            LastName = "Kowalska"
        };
        await _repo.AddAsync(person);
        
        person.FirstName = "Maria Updated";
        
        var updated = await _repo.UpdateAsync(person);
        
        var found = await _repo.FindByIdAsync(person.Id);
        Assert.NotNull(found);
        Assert.Equal("Maria Updated", found.FirstName);
    }

    [Fact]
    public async Task RemoveByIdTestAsync()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "Tomasz" };
        await _repo.AddAsync(person);
        
        await _repo.RemoveByIdAsync(person.Id);
        
        var found = await _repo.FindByIdAsync(person.Id);
        Assert.Null(found);
    }

    [Fact]
    public async Task RemoveById_ThrowsException_WhenNotFound()
    {
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _repo.RemoveByIdAsync(Guid.NewGuid()));
    }
}