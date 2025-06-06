﻿using Customers.Management.Domain.Entities;
using Customers.Management.Domain.Enums;
using Customers.Management.Domain.Interfaces.Repositories;
using Customers.Management.Infra.Tests.Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Priority;

namespace Customers.Management.Infra.Tests;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class CustomerRepositoryTests : IClassFixture<FixtureServiceProvider>
{
    private static bool _mocked;
    private readonly ICustomerRepository _customerRepository;

    public CustomerRepositoryTests(FixtureServiceProvider fixtureServiceProvider)
    {
        _customerRepository = fixtureServiceProvider.GetService<ICustomerRepository>();

        InsertMock();
    }

    [Fact, Priority(1)]
    public async Task GetAllAsync_ShouldReturnCustomers()
    {
        var customers = await _customerRepository.GetAllAsync(new CancellationToken());

        customers.Should().NotBeEmpty();
        customers.Count().Should().Be(3);
    }

    [Fact, Priority(2)]
    public async Task GetByIdAsync_ShouldReturnCustomer()
    {
        var customer = await _customerRepository.GetByIdAsync(Guid.Parse("5f1a5ccf-4066-472f-9606-bb00acdcb5b0"), new CancellationToken());

        customer.Should().NotBeNull();
        customer.Name.Should().Be("João Alberto");
        customer.TaxId.Should().Be("12345678944");
    }

    [Fact, Priority(3)]
    public async Task GetByTaxIdAsync_ShouldReturnCustomer()
    {
        var customer = await _customerRepository.GetByTaxIdAsync("12345678901", new CancellationToken());

        customer.Should().NotBeNull();
        customer.Name.Should().Be("Anderson J.");
        customer.TaxId.Should().Be("12345678901");
    }

    [Fact, Priority(4)]
    public async Task InsertAsync_ShouldInsertCustomer()
    {
        var newCustomer = new Customer(
            "Teste Insert",
            "12345678999",
            DateOnly.FromDateTime(DateTime.Now),
            "Rua xpto, 99",
            "Bluville",
            "89045-123",
            "Santa Catarina",
            "Brasil",
            SignupChannel.Website
        );
        typeof(Customer).GetProperty("Id")?.SetValue(newCustomer, Guid.Parse("5fab0735-04d3-4c6d-a4e9-4f3cd4e05af5"));

        await _customerRepository.InsertAsync(newCustomer, new CancellationToken());
        await _customerRepository.CommitAsync();

        var customer = await _customerRepository.GetByIdAsync(Guid.Parse("5fab0735-04d3-4c6d-a4e9-4f3cd4e05af5"), new CancellationToken());

        customer.Should().NotBeNull();
        customer.Name.Should().Be("Teste Insert");
        customer.TaxId.Should().Be("12345678999");
    }

    [Fact, Priority(5)]
    public async Task Update_ShouldUpdateCustomer()
    {
        var customer = await _customerRepository.GetByIdAsync(Guid.Parse("4411dbce-cf01-4565-a307-6dc237c777b6"), new CancellationToken());

        typeof(Customer).GetProperty("TaxId")?.SetValue(customer, "99999999999");

        await _customerRepository.Update(customer!, new CancellationToken());
        await _customerRepository.CommitAsync();

        var customerNewTaxId = await _customerRepository.GetByIdAsync(Guid.Parse("4411dbce-cf01-4565-a307-6dc237c777b6"), new CancellationToken());

        customerNewTaxId.Should().NotBeNull();
        customerNewTaxId.TaxId.Should().Be("99999999999");
    }

    [Fact, Priority(6)]
    public async Task Delete_ShouldDeleteCustomer()
    {
        var customer = await _customerRepository.GetByIdAsync(Guid.Parse("4411dbce-cf01-4565-a307-6dc237c777b6"), new CancellationToken());

        await _customerRepository.DeleteAsync(customer!, new CancellationToken());
        await _customerRepository.CommitAsync();

        var customerNull = await _customerRepository.GetByIdAsync(Guid.Parse("4411dbce-cf01-4565-a307-6dc237c777b6"), new CancellationToken());

        customerNull.Should().BeNull();
    }

    private void InsertMock()
    {
        if (_mocked)
            return;

        var customer1 = new Customer(
            "Anderson J.",
            "12345678901",
            DateOnly.FromDateTime(DateTime.Now),
            "Rua xpto, 22",
            "Bluville",
            "89045-123",
            "Santa Catarina",
            "Brasil",
            SignupChannel.Website
        );
        typeof(Customer).GetProperty("Id")?.SetValue(customer1, Guid.Parse("a57c8ca4-99c0-4f07-8e72-0656dc060c2e"));
        var customer2 = new Customer(
            "João Alberto",
            "12345678944",
            DateOnly.FromDateTime(DateTime.Now),
            "Rua xpto, 44",
            "Bluville",
            "89045-123",
            "Santa Catarina",
            "Brasil",
            SignupChannel.Website
        );
        typeof(Customer).GetProperty("Id")?.SetValue(customer2, Guid.Parse("5f1a5ccf-4066-472f-9606-bb00acdcb5b0"));
        var customer3 = new Customer(
            "Francisco",
            "12345678955",
            DateOnly.FromDateTime(DateTime.Now),
            "Rua xpto, 78",
            "Bluville",
            "89045-123",
            "Santa Catarina",
            "Brasil",
            SignupChannel.Website
        );
        typeof(Customer).GetProperty("Id")?.SetValue(customer3, Guid.Parse("4411dbce-cf01-4565-a307-6dc237c777b6"));

        _customerRepository.InsertAsync(customer1, new CancellationToken()).Wait();
        _customerRepository.InsertAsync(customer2, new CancellationToken()).Wait();
        _customerRepository.InsertAsync(customer3, new CancellationToken()).Wait();
        _customerRepository.CommitAsync().Wait();

        _mocked = true;
    }
}
