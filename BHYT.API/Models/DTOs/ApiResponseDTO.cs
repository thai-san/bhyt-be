﻿namespace BHYT.API.Models.DTOs
{
    public class ApiResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
