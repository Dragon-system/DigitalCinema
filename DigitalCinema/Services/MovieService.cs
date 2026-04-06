namespace DigitalCinema.Services
{
    public enum MovieImgType
    {
        MainImg,
        SubImg

    }
    public class MovieService
    {
        public string CreateFile(IFormFile Img, MovieImgType movieImgType = MovieImgType.MainImg)
        {
            var fileName =
                    $"{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}-{Guid.NewGuid().ToString()}{Path.GetExtension(Img.FileName)}";
            // 31290-fjkdsfhsd-32131.png

            var filePath = string.Empty;

            if (movieImgType == MovieImgType.MainImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", fileName);
            }
            else if (movieImgType == MovieImgType.SubImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie\\SubImgs", fileName);
            }

            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }

            return fileName;
        }

        public string GetOldFilePath(string oldFileName, MovieImgType movieImgType = MovieImgType.MainImg)
        {
            var filePath = string.Empty;

            if (movieImgType == MovieImgType.MainImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie", oldFileName);
            }
            else if (movieImgType == MovieImgType.SubImg)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Movie\\SubImgs", oldFileName);
            }

            return filePath;
    
        }
    }
}
