﻿using BankAccounts.Api;
using BankAccounts.Infrastructure.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Testing.Acceptance.WebApplication;

namespace BankAccounts.Acceptance.Tests.Shared
{
    public class BankAccountClientFactory : WebApplicationFactoryBase<Startup>
    {
        protected override DbContext CreateScopeReturnDbContext()
        {
            return Services.CreateScope().ServiceProvider.GetRequiredService<BankAccountDbContext>();
        }

        public DbContext CreateNewDbContext()
        {
            return Services.CreateScope().ServiceProvider.GetRequiredService<BankAccountDbContext>();
        }
    }
}