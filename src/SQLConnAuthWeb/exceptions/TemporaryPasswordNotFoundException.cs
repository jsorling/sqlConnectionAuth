namespace Sorling.SqlConnAuthWeb.exceptions;

public class TemporaryPasswordNotFoundException : Exception
{
   public TemporaryPasswordNotFoundException()
       : base("Temporary password not found.") {
   }
   public TemporaryPasswordNotFoundException(string message)
       : base(message) {
   }
   public TemporaryPasswordNotFoundException(string message, Exception inner)
       : base(message, inner) {
   }
}
