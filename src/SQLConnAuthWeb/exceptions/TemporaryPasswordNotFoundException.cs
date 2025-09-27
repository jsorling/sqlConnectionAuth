namespace Sorling.SqlConnAuthWeb.exceptions;

/// <summary>
/// Exception thrown when a requested temporary password could not be found in the password store.
/// </summary>
public class TemporaryPasswordNotFoundException : Exception
{
   /// <summary>
   /// Initializes a new instance of the <see cref="TemporaryPasswordNotFoundException"/> class with a default message.
   /// </summary>
   public TemporaryPasswordNotFoundException()
       : base("Temporary password not found.") {
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="TemporaryPasswordNotFoundException"/> class with a specified error message.
   /// </summary>
   /// <param name="message">The message that describes the error.</param>
   public TemporaryPasswordNotFoundException(string message)
       : base(message) {
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="TemporaryPasswordNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
   /// </summary>
   /// <param name="message">The error message that explains the reason for the exception.</param>
   /// <param name="inner">The exception that is the cause of the current exception.</param>
   public TemporaryPasswordNotFoundException(string message, Exception inner)
       : base(message, inner) {
   }
}
