﻿namespace Identity.Domain.Models
{
    /// <summary>
    /// Holds the key and uri required for setting up user authenticator
    /// </summary>
    public class EnableAuthenticatorModel
    {
        /// <summary>
        /// Formatted Key for the user authenticator
        /// </summary>
        public string SharedKey { get; set; } = string.Empty;

        /// <summary>
        /// QR code Uri
        /// </summary>
        public string AuthenticatorUri { get; set; } = string.Empty;
    }
}
