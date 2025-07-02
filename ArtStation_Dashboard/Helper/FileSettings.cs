namespace ArtStation_Dashboard.Helper
{
    
        public static class FileSettings
        {
            public static async Task<string> UploadFile(IFormFile file, string folderName, string webRootPath)
            {
                string folderPath = Path.Combine(webRootPath, "Uploads", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return fileName;
            }

            public static void DeleteFile(string folderName, string fileName, string webRootPath)
            {
                string filepath = Path.Combine(webRootPath, "Uploads", folderName, fileName);
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
        }
    }
