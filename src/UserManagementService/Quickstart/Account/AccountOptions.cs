// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace IdentityServer4.Quickstart.UI
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin { get; set; } = true;
        public static bool AllowRememberLogin { get; set; } = true;
        public static TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt { get; set; } = true;
        public static bool AutomaticRedirectAfterSignOut { get; set; } = false;

        // specify the Windows authentication scheme being used
        public static string WindowsAuthenticationSchemeName { get; } = Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
        // if user uses windows auth, should we load the groups from windows
        public static bool IncludeWindowsGroups { get; set; } = false;

        public static string InvalidCredentialsErrorMessage { get; set; } = "Invalid username or password";
    }
}
