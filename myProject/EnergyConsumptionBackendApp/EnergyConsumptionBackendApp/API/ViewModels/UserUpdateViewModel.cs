﻿namespace EnergyConsumptionBackendApp.API.ViewModels
{
    public class UserUpdateViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
    }
}
