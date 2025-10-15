var db = new Database();
var now = DateTime.Now;
var discount = (now.DayOfWeek == DayOfWeek.Friday) ? 25f : 0f;

Console.WriteLine("Vad vill du köpa?");
var item = Console.ReadLine();

var price = db.GetItemPrice(item);

Console.WriteLine("Hur många?");
var quantity = float.Parse(Console.ReadLine());

var total = price * quantity * (1 - (discount / 100));

Console.WriteLine($"""
                   ********************************
                   KVITTO
                   {item}({price}) x {quantity}
                   rabatt: {discount}%
                   Total price: {total}:-
                   ~~~~~~~~~~~~~~~~~~~~~
                   {now}
                   ********************************
                   """);

Console.ReadLine();

public class Database
{
    public float GetItemPrice(string id) => new Random().Next(1, 10);
}











/* Steps
 * 1. identify problems
 *      a. All code in main
 *      b. Database dependency
 *      c. DateTime.now
 *      d. Console.WriteLine
 *
 * 2. Extract code to PrintReceipt
 *
 * 3. Solve Database
 *      a. Adding interface
 *      b. Adding mock of interface
 *
 * 4. Solve DateTime.Now
 *      a. Add wrapper class and Interface
 *      b. Create mock of this Interface
 *
 * 5. Solve Console.WriteLine
 *      a. Extract to Printer class
 *      b. Implement IPrinter
 *      c. Mock Iprinter
 *
 */