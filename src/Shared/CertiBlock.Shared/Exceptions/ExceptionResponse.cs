using System.Net;

namespace CertiBlock.Shared.Exceptions;

public record ExceptionResponse(object Response, HttpStatusCode StatusCode);
