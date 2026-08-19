namespace EnterpriseApi.Application.Exceptions;

public sealed class RequestValidationException(string message) : Exception(message);
