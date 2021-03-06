﻿using MiCam.Api.Client.Entities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MiCam.Api.Client
{
    public class BaseCamClient : IBaseCamClient, IBaseApiRequestor
    {
        #region Properties
        /// <summary>
        /// Gets the underlying HTTP client.
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Gets or sets the number of tries a request should be attempted before failing.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the time that should be waited between each request.
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(3);
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to get the value for.</param>
        public Task<ResponseEntity> GetAsync(string propertyName)
        {
            return RequestAsync("get", propertyName);
        }

        /// <summary>
        /// Makes a request and handles retry logic.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        public async Task<ResponseEntity> RequestAsync(string uri)
        {
            HttpResponseMessage response = null;

            for (var i = 0; i < RetryCount; i++)
            {
                try
                {
                    response = await RawRequestAsync(uri).ConfigureAwait(false);

                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(RetryDelay).ConfigureAwait(false);
                }
            }

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            responseString = responseString?.Trim();

            var breakdown = responseString.Split('\n');

            var responseEntity = new ResponseEntity
            {
                RawResponse = responseString,
                Success = breakdown[1].Equals("ok", StringComparison.CurrentCultureIgnoreCase),
                Value = breakdown[0]
            };

            if (breakdown.Length >= 3)
                responseEntity.PropertyName = breakdown[2].Split('=')[0];

            return responseEntity;
        }

        /// <summary>
        /// Makes a request and handles retry logic.
        /// </summary>
        /// <param name="action">The request action (get/set).</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to set the property to.</param>
        public async Task<ResponseEntity> RequestAsync(string action, string propertyName, object value = null)
        {
            HttpResponseMessage response = null;

            for (var i = 0; i < RetryCount; i++)
            {
                try
                {
                    response = await RawRequestAsync(action, propertyName, value).ConfigureAwait(false);

                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(RetryDelay).ConfigureAwait(false);
                }
            }

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            responseString = responseString?.Trim();

            var breakdown = responseString.Split('\n');

            return new ResponseEntity
            {
                PropertyName = breakdown.Length >= 3 ? breakdown[2].Split('=')[0] : propertyName,
                RawResponse = responseString,
                Success = breakdown[1].Equals("ok", StringComparison.CurrentCultureIgnoreCase),
                Value = breakdown[0]
            };
        }

        /// <summary>
        /// Makes a request to the dashcam.
        /// </summary>
        /// <param name="uri">The request URI.</param>
        public async Task<HttpResponseMessage> RawRequestAsync(string uri)
        {
            var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new WebException();

            return response;
        }

        /// <summary>
        /// Makes a request to the dashcam.
        /// </summary>
        /// <param name="action">The request action (get/set).</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to set the property to.</param>
        public async Task<HttpResponseMessage> RawRequestAsync(string action, string propertyName, object value = null)
        {
            var requestUri = $"http://192.72.1.1/cgi-bin/Config.cgi?action={action}&property={propertyName}";

            if (value != null)
                requestUri += $"&value={value}";

            var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri)).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new WebException();

            return response;
        }

        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">The value to assign to the property.</param>
        public Task<ResponseEntity> SetAsync(string propertyName, string value)
        {
            return RequestAsync("set", propertyName, value);
        }
        #endregion

        #region Constructors
        public BaseCamClient()
        {
            Client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }
        #endregion
    }
}
