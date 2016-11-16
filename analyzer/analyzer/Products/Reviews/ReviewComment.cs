using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Reviews
{
    public class ReviewComment
    {
        public ReviewComment(int id, int reviewId, string content)
        {
            Id = id;
            ReviewId = reviewId;
            Content = content;
        }

        public int Id { get; }
        public int ReviewId { get; }
        public string Content { get; }
    }
}


