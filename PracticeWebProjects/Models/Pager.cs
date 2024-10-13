namespace PracticeWebProjects.Models
{
    public class Pager
    {
        public Pager()
        {
                
        }

        public Pager(int totalItems, int page, int pageSize = 4)
        {
            TotalItems = totalItems;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)TotalItems / (decimal)pageSize);
            CurrentPage = page;

            // Handle boundaries for startPage and endPage
            StartPage = CurrentPage - 1;
            EndPage = CurrentPage + 2;

            if (StartPage <= 0)
            {
                EndPage = EndPage - (StartPage - 1);
                StartPage = 1;
            }

            if (EndPage > TotalPages)
            {
                EndPage = TotalPages;
                if (EndPage > 4)
                {
                    StartPage = EndPage - 3;
                }
            }
        }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int StartPage { get; set; }

        public int EndPage { get; set; }
    }
}
