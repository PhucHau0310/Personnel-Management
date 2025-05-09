﻿namespace PersonnelManagement.Models.DTOs
{
    public class ResponseDto<T>
    {
        public bool Success {  get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
