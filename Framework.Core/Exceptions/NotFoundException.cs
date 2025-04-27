using System.Collections.ObjectModel;
using System.Net;

namespace Framework.Core.Exceptions;
public class NotFoundException : GeneralException
{
    public NotFoundException(string message)
        : base(message, new Collection<string>(), HttpStatusCode.NotFound)
    {
    }
}
