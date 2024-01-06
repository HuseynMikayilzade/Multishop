using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;

namespace MultiShop.Areas.Manage.Utilities.Extentions
{
    public static class TotalPageCount
    {
        //public static async Task<IActionResult> CheckPageAndCount<T>( int page, int pageSize, IQueryable<T> queryable)
        //{
        //    if (page <= 0)
        //        throw new Exception("bad request");

        //    double count = await queryable.CountAsync();

        //    if (count <= 0)
        //        throw new Exception("Not Found");

        //    double totalPage = Math.Ceiling(count / pageSize);

        //    if (page > totalPage)
        //        throw new Exception("bad request");

        //    return null;
        //}
    }
}
