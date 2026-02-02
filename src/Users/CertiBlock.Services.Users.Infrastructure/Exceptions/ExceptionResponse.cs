using System.Net;

namespace CertiBlock.Services.Users.Infrastructure.Exceptions;

public record ExceptionResponse(object Response, HttpStatusCode StatusCode);