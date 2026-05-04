using Vectantic.Core.Exceptions;

namespace Vectantic.Semantic.Internal.Utilities;

internal static class SemanticExceptionHandler {
    internal static T Handle<T>(Func<T> func, Func<Exception, VectanticException> wrap) {
        try { return func.Invoke(); }
        catch (VectanticException) { throw; }
        catch (Exception ex) { throw wrap(ex); }
    }
}