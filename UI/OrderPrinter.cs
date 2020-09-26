using ConsoleTables;
using Gluh.TechnicalTest.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.UI
{
    public interface IOrderPrinter
    {
        void Print(IEnumerable<IPurchaseOrder> orders);
        void Print(IUnfulfilledOrder unfulfilledOrder);
    }

    public class OrderPrinter : IOrderPrinter
    {
        public void Print(IEnumerable<IPurchaseOrder> orders)
        {
            PrintHeader("Order summary");
            var table = new ConsoleTable(nameof(IPurchaseOrder.Supplier.Name), nameof(IPurchaseOrder.SubTotal), nameof(IPurchaseOrder.Shipping), nameof(IPurchaseOrder.Total));
            foreach (var order in orders)
            {
                table.AddRow(order.Supplier.Name, order.SubTotal.ToString("c"), order.Shipping.ToString("c"), order.Total.ToString("c"));
            }
            table.AddRow("Total", orders.Sum(x=>x.SubTotal).ToString("c") , orders.Sum(x => x.Shipping).ToString("c"), orders.Sum(x => x.Total).ToString("c"));
            table.Write();
            Console.WriteLine();

            PrintHeader("Order details");
            foreach (var order in orders)
            {
                Console.WriteLine($"Supplier: {order.Supplier.Name}\tSubtotal: {order.SubTotal:c}\tShipping: {order.Shipping:c}\tTotal: {order.Total:c}");
                table = new ConsoleTable(nameof(IPurchaseOrderLine.Product), nameof(IPurchaseOrderLine.Price), nameof(IPurchaseOrderLine.Quantity), nameof(IPurchaseOrderLine.Total));
                foreach (var line in order.Lines)
                {
                    table.AddRow(line.Product, line.Price.ToString("c"), line.Quantity.ToString("n0"), line.Total.ToString("c"));
                }
                //table.AddRow("Total",order.SubTotal ,"", order.Total);
                table.Write();
                Console.WriteLine();
            }
        }

        public void Print(IUnfulfilledOrder unfulfilledOrder)
        {
            PrintHeader("Unfulfilled summary");
            var table = new ConsoleTable(nameof(IOrderLineBase.Product), nameof(IOrderLineBase.Quantity));
            foreach (var line in unfulfilledOrder.Lines)
            {
                table.AddRow(line.Product, line.Quantity.ToString("n0"));
            }
            table.Write();
            Console.WriteLine();
        }

        private void PrintHeader(string header)
        {
            Console.WriteLine();
            WritePadded('*');
            WritePadded('-', header);
            WritePadded('*');
        }
        private void WritePadded(char padChar, string text = null)
        {
            if (text == null)
            {
                Console.WriteLine($" {new string(padChar, Console.BufferWidth-2)} ");
                return;
            }
            var padlength = (Console.BufferWidth - text.Length - 6) / 2;
            var padStr = new string(padChar, padlength);
            Console.WriteLine($"  {padStr} {text} {padStr}  ");
        }
    }
}
