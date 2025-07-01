using MultiShop.Models;
using MultiShop.ViewModels;
using System.Text;

namespace MultiShop.Utilities.Extentions
{
    public static class EmailCreator
    {
        public static string EmailBody(Order order)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h2>Hörmətli müştəri,</h2>");
            sb.AppendLine("<p>Sizin aşağıdakı məhsullar uğurla alınmışdır:</p>");

            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 8px; text-align:left;'>Məhsul Adı</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 8px; text-align:right;'>Miqdar</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 8px; text-align:right;'>Qiymət (AZN)</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 8px; text-align:right;'>Cəm (AZN)</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            decimal totalPrice = 0;

            foreach (var p in order.BasketItems)
            {
                decimal totalProductPrice = p.Price * p.Count;
                totalPrice += totalProductPrice;

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px;'>{p.Product.Name}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px; text-align:right;'>{p.Count}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px; text-align:right;'>{p.Price:F2}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px; text-align:right;'>{totalProductPrice:F2}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("<tfoot>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align:right; font-weight:bold;'>Ümumi Məbləğ:</td>");
            sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 8px; text-align:right; font-weight:bold;'>{totalPrice:F2} AZN</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</tfoot>");
            sb.AppendLine("</table>");

            sb.AppendLine("<p>Alışınız üçün təşəkkür edirik! <br/> MultiShop komandası</p>");

            return sb.ToString();
        }
    }
}
