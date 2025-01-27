﻿using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharedKernel.Testing.Acceptance.Exceptions
{
    public class ErrorResponseExceptionHandler
    {
        #region Atributes

        private const string ErrorsStartString = "{\"errors\":{";
        private readonly HttpResponseMessage _responseMessage;
        private string _error;
        private Dictionary<string, string[]> _errors;

        #endregion

        #region Constructors

        public ErrorResponseExceptionHandler(HttpResponseMessage responseMessage)
        {
            _responseMessage = responseMessage;
        }

        #endregion

        #region Public Methods

        public async Task<ErrorResponseExceptionHandler> Build()
        {
            _responseMessage.StatusCode.Should().BeOneOf(new List<HttpStatusCode>
                {HttpStatusCode.BadRequest, HttpStatusCode.NotAcceptable});

            var message = await _responseMessage.Content.ReadAsStringAsync();

            _errors = new Dictionary<string, string[]>();

            if (!message.StartsWith(ErrorsStartString))
            {
                _error = JsonConvert.DeserializeObject<string>(message);
                return this;
            }

            var jsonResponse = JsonConvert.DeserializeObject<Root>(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            if (jsonResponse == default)
                return this;

            foreach (var property in jsonResponse.Errors.Properties())
            {
                var valor = property.Value.ToObject<string[]>();
                if (string.IsNullOrWhiteSpace(property.Name))
                    _error = valor?.First();
                else
                    _errors.Add(FirstCharToUpper(property.Name), valor);
            }

            return this;
        }

        public void Should(string message)
        {
            _error.Should().Contain(message);
        }

        public void Should(string propertyName, string message)
        {
            _errors.Should().ContainKey(propertyName);
            _errors[propertyName].Should().Contain(x => x.Replace("carácteres", "caracteres").Contains(message));
        }

        #endregion

        #region Private

        private string FirstCharToUpper(string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($@"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        // ReSharper disable UnusedMember.Local
        private class Root
        {
            public string Type { get; set; }
            public string Title { get; set; }
            public int Status { get; set; }
            public string TraceId { get; set; }
            public JObject Errors { get; set; }
        }

        #endregion
    }
}
