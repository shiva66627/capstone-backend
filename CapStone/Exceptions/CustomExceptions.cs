using System;

namespace CapStone.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message) { }
    }

    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string message) : base(message) { }
    }

    public class DuplicateProductNameException : Exception
    {
        public DuplicateProductNameException(string message) : base(message) { }
    }

    public class DuplicateTransactionException : Exception
    {
        public DuplicateTransactionException(string message) : base(message) { }
    }

    public class BusinessRuleViolationException : Exception
    {
        public BusinessRuleViolationException(string message) : base(message) { }
    }

    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message) { }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message) : base(message) { }
    }

    public class InactiveUserException : Exception
    {
        public InactiveUserException(string message) : base(message) { }
    }

    public class ClaimAmountExceedsCoverageException : BusinessRuleViolationException
    {
        public ClaimAmountExceedsCoverageException(string message) : base(message) { }
    }

    public class ClaimModificationException : BusinessRuleViolationException
    {
        public ClaimModificationException(string message) : base(message) { }
    }
}
