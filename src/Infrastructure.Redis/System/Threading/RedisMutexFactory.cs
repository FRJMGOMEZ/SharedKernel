﻿using Medallion.Threading.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using SharedKernel.Application.System.Threading;
using StackExchange.Redis;

namespace SharedKernel.Infrastructure.Redis.System.Threading;

internal class RedisMutexFactory : IMutexFactory
{
    private readonly IOptions<RedisCacheOptions> _options;

    public RedisMutexFactory(IOptions<RedisCacheOptions> options)
    {
        _options = options;
    }

    public IMutex Create(string key)
    {
        var @lock = new RedisDistributedLock(key, GetDatabase());
        var sqlDistributedLockHandle = @lock.Acquire();
        return new RedisMutex(sqlDistributedLockHandle);
    }

    public async Task<IMutex> CreateAsync(string key, CancellationToken cancellationToken)
    {
        var @lock = new RedisDistributedLock(key, GetDatabase());
        var sqlDistributedLockHandle = await @lock.AcquireAsync(cancellationToken: cancellationToken);
        return new RedisMutex(sqlDistributedLockHandle);
    }

    private IDatabase GetDatabase()
    {
        var redis = ConnectionMultiplexer.Connect(_options.Value.Configuration!);
        return redis.GetDatabase();
    }
}
