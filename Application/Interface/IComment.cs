using Application.DTO;
using Domain;
using Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;using Application.Helper;

public interface IComment
{
    Task<AdminCommentsViewModel> GetCommentsForAdminAsync();
    Task ApproveCommentAsync(int id);
    Task DeleteCommentAsync(int id);
    Task<List<CommentListDto>> GetAll();

    Task AddCommentAsync(AddCommentDto command);
    Task<bool> ToggleVoteAsync(int commentId, int userId, bool isLike);
    Task<List<CommentForDetailDto>> GetHomeTestimonialsAsync();
    Task<List<ProductCommentForViewDto>> GetProductCommentsAsync(int productId);
    


}


 public class CommentService : IComment
    {
        private readonly AppDbContext _mydb;

        public CommentService(AppDbContext context)
        {
            _mydb = context;
        }

        public async Task<AdminCommentsViewModel> GetCommentsForAdminAsync()
        {

            var pending = await _mydb.Msg4Products
                .Include(c => c.prdmsg)
              //  .Include(c => c.User)
                .Where(c => !c.IsConfirmed)   // ✅ Pending
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            var approved = await _mydb.Msg4Products
                .Include(c => c.prdmsg)
         //       .Include(c => c.User)
                .Where(c => c.IsConfirmed)    // ✅ Approved
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return new AdminCommentsViewModel
            {

                Pending = pending.Select(c => new CommentDto
                {
                    
                    Id = c.Id,
                    Content = c.Content ?? "", 
                    ProductId = c.ProductId,

                    ProductName = c.prdmsg != null ? c.prdmsg.Title : "محصول نامشخص", 
                    

                 //   UserName = (c.User != null && !string.IsNullOrEmpty(c.User.Username)) ? c.User.Username : "ناشناس",
                    
                    Title = c.Title,
                
                    IsConfirmed = c.IsConfirmed
                    
                }).ToList(),


                Approved = approved.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content ?? "",
                    ProductId = c.ProductId,
                    ProductName = c.prdmsg != null ? c.prdmsg.Title : "محصول نامشخص",
               //     UserName = (c.User != null && !string.IsNullOrEmpty(c.User.Username)) ? c.User.Username : "ناشناس",
                    
             
                    IsConfirmed = c.IsConfirmed
                }).ToList()
            };


        }



        public async Task ApproveCommentAsync(int id)
        {
            var comment = await _mydb.Msg4Products.FindAsync(id);
            if (comment == null) return;

            comment.IsConfirmed = true;
            await _mydb.SaveChangesAsync();
        }


        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _mydb.Msg4Products.FindAsync(id);
            if (comment != null)
            {
                _mydb.Msg4Products.Remove(comment);
                await _mydb.SaveChangesAsync();
            }
        }

        public async Task AddCommentAsync(AddCommentDto dto)
        {
            var comment = new ProductComment()
            {
                ProductId = dto.ProductId,
                Name = dto.Name,               
                Title = dto.Title,             
                IsRecommended = dto.IsRecommended, 
                Content = dto.Content,
                

                CreateTime = DateTime.Now,
                IsConfirmed = false, 
                Like = 0,
                Dislike = 0,

            };

            _mydb.Msg4Products.Add(comment);
            await _mydb.SaveChangesAsync();
        }
        public async Task<bool> ToggleVoteAsync(int commentId, int userId, bool isLike)
        {

            var comment = await _mydb.Msg4Products.FindAsync(commentId);
            if (comment == null) return false;


            var existingVote = await _mydb.CommentVotes
                .FirstOrDefaultAsync(v => v.CommentId == commentId );

            if (existingVote != null)
            {

                if (existingVote.IsLike == isLike)
                {

                    _mydb.CommentVotes.Remove(existingVote);
            
                    if (isLike) comment.Like = Math.Max(0, (comment.Like ?? 0) - 1);
                    else comment.Dislike = Math.Max(0, (comment.Dislike ?? 0) - 1);
                }
                else
                {

                    existingVote.IsLike = isLike;

                    if (isLike)
                    {
                        comment.Dislike = Math.Max(0, (comment.Dislike ?? 0) - 1);
                        comment.Like++;
                    }
                    else
                    {
                        comment.Dislike = Math.Max(0, (comment.Dislike ?? 0) - 1);
                        comment.Dislike++;
                    }
                }
            }
            else
            {

                var newVote = new CommentVote
                {
                    CommentId = commentId,
                 //   UserId = userId,
                    IsLike = isLike
                };
                _mydb.CommentVotes.Add(newVote);

                if (isLike) comment.Like++;
                else comment.Dislike++;
            }

            await _mydb.SaveChangesAsync();
            return true;
        }
        public async Task<List<CommentForDetailDto>> GetHomeTestimonialsAsync()
        {
            return await _mydb.Msg4Products
               // .Include(c => c.User)

                .OrderByDescending(c => c.Id)
                .Take(10) 
                .Select(c => new CommentForDetailDto
                {
                    Id = c.Id,
                    Content = c.Content,
                //    UserName = (c.User != null && !string.IsNullOrEmpty(c.User.Username)) ? c.User.Username : "کاربر ناشناس",

                })
                .ToListAsync();
        }
        public async Task<List<CommentListDto>> GetAll()
        {
            return await _mydb.Msg4Products
                .OrderByDescending(c => c.Id)
                .Select(b => new CommentListDto
                {
                    Id = b.Id,
                    Content = b.Content,
                    ProductName = b.prdmsg != null ? b.prdmsg.Title : "محصول حذف شده",
                 //   UserName = b.User != null ? b.User.Username : "کاربر ناشناس",
            

                })
                .ToListAsync();
        }

        public async Task<List<ProductCommentForViewDto>> GetProductCommentsAsync(int productId)
        {
            var comments = await _mydb.Msg4Products
                .Where(c => c.ProductId == productId && c.IsConfirmed) // فقط تایید شده‌ها
                .OrderByDescending(c => c.Id)
                .Select(c => new 
                {
                    c.Id, c.Name, c.Title, c.Content, c.IsRecommended, c.CreateTime, c.Like, c.Dislike
                })
                .ToListAsync();


            return comments.Select(c => new ProductCommentForViewDto
            {
                Id = c.Id,
                Name = string.IsNullOrEmpty(c.Name) ? "کاربر ناشناس" : c.Name,
                Title = c.Title,
                Content = c.Content,
                IsRecommended = c.IsRecommended ?? true,

                Date = c.CreateTime.Value.ToShamsi(), 
                Like = c.Like ?? 0,
                Dislike = c.Dislike ?? 0
            }).ToList();
        }
     
    }