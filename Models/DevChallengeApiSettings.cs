﻿using System;
namespace Challenge_DEV_2023.Models
{
    public class DevChallengeApiSettings
    {
        private static readonly Lazy<DevChallengeApiSettings> instance = new Lazy<DevChallengeApiSettings>(() => new DevChallengeApiSettings());

        public static DevChallengeApiSettings Instance => instance.Value;
        private readonly IConfiguration _config;
        public readonly string BaseUrl = "https://devchallenge.winsysgroup.com/api";
        private string _email;

        public string Token
        {
            get => _config["DevChallengeApiSettings:Token"]!;
            set
            {
                _config["DevChallengeApiSettings:Token"] = value;
            }
        }

        public string Email
        {
            get => _email;
        }

        public DevChallengeApiSettings()
        {
            // Initialize configuration
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>(true)
                .Build();
            _email = _config["DevChallengeApiSettings:Email"]!;
        }
    }
}

