using Together.DTOs.Blog;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class BlogService
    {
        private readonly BlogRepo _blogRepo;
        private readonly CloudinaryService _cloudinaryService;
        private readonly AccountRepo _accountRepo;

        public BlogService(BlogRepo blogRepo, CloudinaryService cloudinaryService, AccountRepo accountRepo)
        {
            _blogRepo = blogRepo;
            _cloudinaryService = cloudinaryService;
            _accountRepo = accountRepo;
        }

        public async Task<List<ViewBlogDto>> GetAllBlogPostsAsync()
        {
            var blogs = await _blogRepo.GettAll();
            return blogs.Select(blog => new ViewBlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Subtitle = blog.Subtitle,
                Excerpt = blog.Excerpt,
                ImageUrl1 = blog.ImageUrl1,
                ImageUrl2 = blog.ImageUrl2,
                ImageUrl3 = blog.ImageUrl3,
                ImageUrl4 = blog.ImageUrl4,
                ImageUrl5 = blog.ImageUrl5,
                Paragraph1 = blog.Paragraph1,
                Paragraph2 = blog.Paragraph2,
                Paragraph3 = blog.Paragraph3,
                Paragraph4 = blog.Paragraph4,
                Paragraph5 = blog.Paragraph5,
                FeaturedImageUrl = blog.FeaturedImageUrl,
                AuthorId = blog.AuthorId,
                AuthorName = blog.Author.Name,
                OrganizationId = blog.OrganizationId,
                OrganizationName = blog.Organization?.Name,
                PublishedDate = blog.PublishedDate,
                UpdatedDate = blog.UpdatedDate,
                Status = blog.Status
            }).ToList();
        }

        public async Task<ViewBlogDto?> GetBlogPostByIdAsync(int id)
        {
            var blog = await _blogRepo.GetByIdAsync(id);
            if (blog == null)
            {
                return null;
            }
            return new ViewBlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Subtitle = blog.Subtitle,
                Excerpt = blog.Excerpt,
                ImageUrl1 = blog.ImageUrl1,
                ImageUrl2 = blog.ImageUrl2,
                ImageUrl3 = blog.ImageUrl3,
                ImageUrl4 = blog.ImageUrl4,
                ImageUrl5 = blog.ImageUrl5,
                Paragraph1 = blog.Paragraph1,
                Paragraph2 = blog.Paragraph2,
                Paragraph3 = blog.Paragraph3,
                Paragraph4 = blog.Paragraph4,
                Paragraph5 = blog.Paragraph5,
                FeaturedImageUrl = blog.FeaturedImageUrl,
                AuthorId = blog.AuthorId,
                AuthorName = blog.Author.Name,
                OrganizationId = blog.OrganizationId,
                OrganizationName = blog.Organization?.Name,
                PublishedDate = blog.PublishedDate,
                UpdatedDate = blog.UpdatedDate,
                Status = blog.Status
            };
        }

        public async Task<(bool Success, string Message, int? BlogId)> CreateBlog(CreateBlogDto dto)
        {
            string featuredImageUrl = null;
            var contentImageUrls = new List<string>();

            try
            {
                if (dto.FeaturedImageUrl != null && dto.FeaturedImageUrl.Length > 0)
                {
                    featuredImageUrl = await _cloudinaryService.UploadImageAsync(dto.FeaturedImageUrl);
                }

                var image1Url = dto.ImageUrl1 != null ?
                    await _cloudinaryService.UploadImageAsync(dto.ImageUrl1) : null;
                var image2Url = dto.ImageUrl2 != null ?
                    await _cloudinaryService.UploadImageAsync(dto.ImageUrl2) : null;
                var image3Url = dto.ImageUrl3 != null ?
                    await _cloudinaryService.UploadImageAsync(dto.ImageUrl3) : null;
                var image4Url = dto.ImageUrl4 != null ?
                    await _cloudinaryService.UploadImageAsync(dto.ImageUrl4) : null;
                var image5Url = dto.ImageUrl5 != null ?
                    await _cloudinaryService.UploadImageAsync(dto.ImageUrl5) : null;

                if (image1Url != null) contentImageUrls.Add(image1Url);
                if (image2Url != null) contentImageUrls.Add(image2Url);
                if (image3Url != null) contentImageUrls.Add(image3Url);
                if (image4Url != null) contentImageUrls.Add(image4Url);
                if (image5Url != null) contentImageUrls.Add(image5Url);

                var blog = new BlogPost
                {
                    Title = dto.Title,
                    Subtitle = dto.Subtitle,
                    Excerpt = dto.Excerpt,

                    ImageUrl1 = image1Url,
                    ImageUrl2 = image2Url,
                    ImageUrl3 = image3Url,
                    ImageUrl4 = image4Url,
                    ImageUrl5 = image5Url,

                    Paragraph1 = dto.Paragraph1,
                    Paragraph2 = dto.Paragraph2,
                    Paragraph3 = dto.Paragraph3,
                    Paragraph4 = dto.Paragraph4,
                    Paragraph5 = dto.Paragraph5,

                    FeaturedImageUrl = featuredImageUrl,

                    AuthorId = dto.AuthorId,
                    OrganizationId = dto.OrganizationId,

                    PublishedDate = DateTime.UtcNow,
                    Status = true
                };

                await _blogRepo.AddAsync(blog);

                return (true, "Blog created successfully", blog.Id);
            }
            catch (Exception ex)
            {
                await RollbackUploads(featuredImageUrl, contentImageUrls);

                return (false, $"Error creating blog: {ex.Message}", null);
            }
        }

        private async Task RollbackUploads(string featuredImageUrl, List<string> contentImageUrls)
        {
            try
            {
                var urlsToDelete = new List<string>();

                if (!string.IsNullOrEmpty(featuredImageUrl))
                    urlsToDelete.Add(featuredImageUrl);

                urlsToDelete.AddRange(contentImageUrls);

                if (urlsToDelete.Any())
                {
                    foreach (var url in urlsToDelete)
                    {
                        await _cloudinaryService.DeleteImageAsync(url);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Failed to cleanup uploaded images");
            }
        }

        public async Task<(bool Success, string Message)> UpdateBlog(int blogId, UpdateBlogDto dto, int currentUserId)
        {
            var blog = await _blogRepo.GetByIdAsync(blogId);
            if (blog == null)
                return (false, "Blog not found");

            if (blog.AuthorId != currentUserId)
            {
                var currentUser = await _accountRepo.GetByIdAsync(currentUserId);
                if (currentUser?.Role != AccountRole.Admin)
                    return (false, "You can only edit your own blogs");
            }

            var imagesToDelete = new List<string>();

            try
            {

                if (dto.RemoveFeaturedImage == true && !string.IsNullOrEmpty(blog.FeaturedImageUrl))
                {
                    imagesToDelete.Add(blog.FeaturedImageUrl);
                    blog.FeaturedImageUrl = null;
                }
                else if (dto.FeaturedImageFile != null)
                {
                    if (!string.IsNullOrEmpty(blog.FeaturedImageUrl))
                        imagesToDelete.Add(blog.FeaturedImageUrl);

                    blog.FeaturedImageUrl = await _cloudinaryService.UploadImageAsync(dto.FeaturedImageFile);
                }

                await UpdateBlogImage(blog, dto.ImageFile1, dto.RemoveImage1, "ImageUrl1", imagesToDelete);
                await UpdateBlogImage(blog, dto.ImageFile2, dto.RemoveImage2, "ImageUrl2", imagesToDelete);
                await UpdateBlogImage(blog, dto.ImageFile3, dto.RemoveImage3, "ImageUrl3", imagesToDelete);
                await UpdateBlogImage(blog, dto.ImageFile4, dto.RemoveImage4, "ImageUrl4", imagesToDelete);
                await UpdateBlogImage(blog, dto.ImageFile5, dto.RemoveImage5, "ImageUrl5", imagesToDelete);

                if (dto.Title != null) blog.Title = dto.Title;
                if (dto.Subtitle != null) blog.Subtitle = dto.Subtitle;
                if (dto.Excerpt != null) blog.Excerpt = dto.Excerpt;

                if (dto.Paragraph1 != null) blog.Paragraph1 = dto.Paragraph1;
                if (dto.Paragraph2 != null) blog.Paragraph2 = dto.Paragraph2;
                if (dto.Paragraph3 != null) blog.Paragraph3 = dto.Paragraph3;
                if (dto.Paragraph4 != null) blog.Paragraph4 = dto.Paragraph4;
                if (dto.Paragraph5 != null) blog.Paragraph5 = dto.Paragraph5;

                if (dto.Status.HasValue) blog.Status = dto.Status.Value;

                blog.UpdatedDate = DateTime.UtcNow;

                await _blogRepo.UpdateAsync(blog);

                await DeleteOldImages(imagesToDelete);

                return (true, "Blog updated successfully");
            }
            catch (Exception ex)
            {
                await RollbackNewUploads(blog, imagesToDelete);
                return (false, $"Error updating blog: {ex.Message}");
            }
        }

        private async Task UpdateBlogImage(BlogPost blog, IFormFile? newImageFile, bool? removeImage,
            string propertyName, List<string> imagesToDelete)
        {
            var property = typeof(BlogPost).GetProperty(propertyName);
            var currentUrl = (string?)property?.GetValue(blog);

            if (removeImage == true && !string.IsNullOrEmpty(currentUrl))
            {
                imagesToDelete.Add(currentUrl);
                property?.SetValue(blog, null);
            }
            else if (newImageFile != null)
            {
                if (!string.IsNullOrEmpty(currentUrl))
                    imagesToDelete.Add(currentUrl);

                var newUrl = await _cloudinaryService.UploadImageAsync(newImageFile);
                property?.SetValue(blog, newUrl);
            }
        }

        private async Task DeleteOldImages(List<string> imagesToDelete)
        {
            foreach (var url in imagesToDelete)
            {
                try
                {
                    await _cloudinaryService.DeleteImageAsync(url);
                }
                catch
                {
                    Console.WriteLine($"Failed to delete image: {url}");
                }
            }
        }

        private async Task RollbackNewUploads(BlogPost blog, List<string> attemptedDeletions)
        {
            var currentUrls = new List<string?>
            {
                blog.FeaturedImageUrl,
                blog.ImageUrl1,
                blog.ImageUrl2,
                blog.ImageUrl3,
                blog.ImageUrl4,
                blog.ImageUrl5
            }.Where(url => !string.IsNullOrEmpty(url)).ToList();

            foreach (var url in currentUrls)
            {
                if (!attemptedDeletions.Contains(url))
                {
                    await _cloudinaryService.DeleteImageAsync(url);
                }
            }
        }

        public async Task<(bool Success, string Message)> DeleteBlog(int blogId, int currentUserId)
        {
            var blog = await _blogRepo.GetByIdAsync(blogId);
            if (blog == null)
                return (false, "Blog not found");

            if (blog.AuthorId != currentUserId)
            {
                var currentUser = await _accountRepo.GetByIdAsync(currentUserId);
                if (currentUser?.Role != AccountRole.Admin)
                    return (false, "You can only delete your own blogs");
            }

            var imagesToDelete = new List<string>
            {
                blog.FeaturedImageUrl,
                blog.ImageUrl1,
                blog.ImageUrl2,
                blog.ImageUrl3,
                blog.ImageUrl4,
                blog.ImageUrl5
            }.Where(url => !string.IsNullOrEmpty(url)).ToList();

            try
            {
                await _blogRepo.DeleteAsync(blog);
                await DeleteOldImages(imagesToDelete);
                return (true, "Blog deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting blog: {ex.Message}");
            }
        }
    }
}
