﻿
using System.ComponentModel.DataAnnotations;



namespace JsonDeserializator.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
