﻿using ArtStation.Core.Resources;
using ArtStation_Dashboard.Resource;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels
{
    public class CreatedCategory
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public string NameAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public string NameEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public IFormFile PhotoFile { get; set; }
        public string? Image { get; set; }
    }
}
