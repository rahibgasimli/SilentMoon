using SlientMoon.Domain.Enums;
using System;

namespace SlientMoon.Application.DTOs.Narrators
{
    public class NarratorListItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public int TrackCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
