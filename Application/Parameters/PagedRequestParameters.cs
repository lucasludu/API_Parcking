namespace Application.Parameters
{
    public class PagedRequestParameters
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }


        public PagedRequestParameters()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public PagedRequestParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }

    }
}
