using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Users.Core.Exceptions;

public class UserRoleNotExistException() : CustomException("This role don't exist");