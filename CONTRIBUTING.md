# Contributing to Each

First off, thank you for considering contributing to Each! It's people like you that make Each such a great library.

## Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When you create a bug report, include as many details as possible:

- **Use a clear and descriptive title**
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples** (code snippets, test cases)
- **Describe the expected behavior**
- **Include your environment details** (.NET version, OS, etc.)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear and descriptive title**
- **Provide a detailed description of the proposed feature**
- **Explain why this enhancement would be useful**
- **Include code examples** if applicable

### Pull Requests

1. Fork the repository
2. Create a new branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Run tests (`dotnet test`)
5. Commit your changes (`git commit -m 'Add some amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## Development Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/Taiizor/Each.git
   cd Each
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run tests:
   ```bash
   dotnet test
   ```

## Coding Standards

- Follow the existing code style
- Use meaningful variable and method names
- Write XML documentation for all public APIs
- Include unit tests for new functionality
- Keep methods focused and small
- Use the existing validation patterns

### Example: Adding a New Type

When adding a new type to the library:

1. Create the type class in `src/Each/Types/`
2. Follow the existing pattern (see `Email.cs` as reference):
   - Inherit from `ValueObject<T>` if applicable
   - Implement `Create`, `TryCreate`, `Validate`, and `IsValid` methods
   - Add XML documentation for all public members
   - Include implicit conversion operators
3. Add unit tests in `test/Each.Tests/`
4. Update the README.md with documentation

### XML Documentation

All public APIs must include XML documentation:

```csharp
/// <summary>
/// Creates a new instance from the specified value.
/// </summary>
/// <param name="value">The value to create from.</param>
/// <returns>A new instance.</returns>
/// <exception cref="ValidationException">Thrown when the value is invalid.</exception>
public static MyType Create(string value)
```

## Testing

- Write tests for all new functionality
- Tests should be clear and focused
- Use descriptive test method names
- Follow the existing test patterns
- Ensure all tests pass before submitting PR

## Questions?

Feel free to open an issue with your question or reach out to the maintainers.

Thank you for contributing! 🎉