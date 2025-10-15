namespace TestabilityDemo.Core;

public class ReceiptService
{
    public void PrintReceipt(string item, float quantity, IClock clock, IDatabase db, IPrinter printer)
    {
        var now = clock.Now;
        var discount = (now.DayOfWeek == DayOfWeek.Friday) ? 25f : 0f;
        var price = db.GetItemPrice(item);
        var total = price * quantity * (1 - (discount / 100));

        printer.Print($"""
                      ********************************
                      KVITTO
                      {item}({price}) x {quantity}
                      rabatt: {discount}%
                      Total price: {total}:-
                      ~~~~~~~~~~~~~~~~~~~~~
                      {now}
                      ********************************
                      """);
    }
}