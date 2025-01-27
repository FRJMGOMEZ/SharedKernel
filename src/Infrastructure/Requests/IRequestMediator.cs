﻿using SharedKernel.Domain.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Requests;

/// <summary>  </summary>
public interface IRequestMediator
{
    /// <summary>  </summary>
    Task Execute(string requestSerialized, Type type, string method, CancellationToken cancellationToken);

    /// <summary>  </summary>
    Task Execute(string requestSerialized, Request request, Type requestHandler, string method, CancellationToken cancellationToken);
}
