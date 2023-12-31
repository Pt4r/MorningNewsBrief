﻿namespace MorningNewsBrief.Common.Configuration {
    public class GeneralSettings {
        /// <summary>
        /// The name is used to mark the section found inside a configuration file.
        /// </summary>
        public static readonly string Name = "General";
        /// <summary>
        /// Url that the app is hosted under.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// The URL for the IdentityServer.
        /// </summary>
        public string Authority { get; set; }
        /// <summary>
        /// The name of the app. Usually used for the Layout page Title inside an HTML header. 
        /// </summary>
        public string ApplicationName { get; set; } = "My App name";
        /// <summary>
        /// A description for the app.
        /// </summary>
        public string ApplicationDescription { get; set; } = "My App description.";
        /// <summary>
        /// A flag that indicates whether to enable the Swagger UI.
        /// </summary>
        public bool EnableSwagger { get; set; }
        /// <summary>
        /// A flag that indicates whether to register mock implementations of services in the DI.
        /// </summary>
        public bool MockServices { get; set; }
        /// <summary>A list of endpoints used throughout the application.</summary>
        public Dictionary<string, EndpointSettings> Endpoints { get; set; }
    }

    public class EndpointSettings {
        /// <summary>They API endpoint address</summary>
        public string Address { get; set; } = "https://xxx";
        /// <summary>They API secret key.</summary>
        public string ClientId { get; set; } = "xxxxxxx";
        /// <summary>They API secret key.</summary>
        public string ClientSecret { get; set; } = "xxxxxxx";
    }
}
