﻿namespace JWT_API.Models.Custom
{
    public class AutorizacionResponse
    {
        public string? Token { get; set; }
        public string? refreshToken { get; set; }
        public bool Resultado { get; set; }
        public string? Msg { get; set; }
    }
}
