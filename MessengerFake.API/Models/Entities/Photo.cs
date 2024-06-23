﻿using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerFake.API.Models.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public Guid Id { get; set; }
        public string? Url { get; set; }
        public bool IsMain { get; set; }
        public string? PublicId { get; set; }
        public bool IsApproved { get; set; } = false;

        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}