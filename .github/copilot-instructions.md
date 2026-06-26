# Copilot Instructions

## Architecture
- Follow SOLID, DRY, KISS, and YAGNI principles.
- Prefer composition over inheritance.

## Development Preferences
- Target .NET 10 and the latest ASP.NET Core.
- Optimize commands and file paths for Ubuntu Linux or Windows.
- Use modern C# 14 features when they improve readability.
- Prefer async/await over synchronous APIs when available.
- Prefer dependency injection over manual service instantiation.
- Prefer early returns over deep nesting.
- Respect nullable reference types.
- Use descriptive and meaningful names.
- Prefer LINQ when it improves readability.
- Handle errors explicitly. Do not swallow exceptions.
- Never hardcode secrets, passwords, tokens, or connection strings.

## Code Generation Rules
- Use existing APIs only. Do not invent methods or libraries.
- Keep solutions simple and avoid unnecessary abstractions.
- Preserve existing user comments.
- Write new code comments in English.
- When modifying code, show only changed blocks unless full file output is requested.
- Don't put dots at the end of comment lines. Use complete sentences without periods.

## Communication
- Respond in Russian unless explicitly requested otherwise.

## Commit Messages
- Follow the Conventional Commits specification.
- Output exactly one line.
- Output only the commit message text.
- Do not include markdown, code blocks, quotes, or explanations.
- Use English only.
- Use imperative mood.
- Use lowercase for type and scope.
- Scope should represent the affected module, component, or folder.
- Do not end the description with a period.

### Format
- type(scope): description

### Allowed Types
- feat
- fix
- docs
- style
- refactor
- test
- chore

### Examples
- feat(auth): add login validation
- fix(database): resolve migration timeout
- refactor(tasks): simplify command parser
- docs(readme): update installation guide