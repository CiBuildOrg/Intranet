﻿namespace Intranet.Web.Authentication.Contracts
{
    public interface IUser
    {
        string DisplayName { get; set; }
        bool IsAdmin { get; set; }
        string Username { get; set; }
    }
}