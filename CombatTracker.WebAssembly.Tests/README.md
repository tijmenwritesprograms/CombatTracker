# Combat Tracker Test Suite

This directory contains the comprehensive test suite for the Combat Tracker application.

## Test Organization

### Directory Structure

```
CombatTracker.WebAssembly.Tests/
├── ModelValidation/          # Data model validation tests
│   ├── CharacterValidationTests.cs
│   ├── MonsterValidationTests.cs
│   ├── PartyValidationTests.cs
│   └── AbilityScoresTests.cs
├── Services/                 # Service layer unit tests
│   ├── PartyStateServiceTests.cs
│   ├── CombatStateServiceTests.cs
│   ├── StorageStateServiceTests.cs
│   ├── ApiKeyServiceTests.cs
│   └── OpenAIStatblockParserServiceTests.cs
├── Components/               # Blazor component tests (bUnit)
│   ├── Shared/              # Shared UI components
│   ├── CombatTracker/       # Combat tracker specific components
│   └── Layout/              # Layout components
├── Pages/                    # Page component tests
│   ├── CombatSetupPageTests.cs
│   ├── CombatTrackerPageTests.cs
│   ├── PartyManagementPageTests.cs
│   └── HomePageTests.cs
├── TestDataBuilders.cs       # Fluent builders for test data
└── TestHelpers.cs            # Helper methods for tests
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

### Generate Coverage Report
```bash
# Install report generator (one time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"TestResults/coveragereport" \
  -reporttypes:"Html;TextSummary"

# View text summary
cat TestResults/coveragereport/Summary.txt

# Open HTML report (Linux/Mac)
xdg-open TestResults/coveragereport/index.html

# Open HTML report (Windows)
start TestResults/coveragereport/index.html
```

### Run Specific Test Categories
```bash
# Model validation tests
dotnet test --filter "FullyQualifiedName~ModelValidation"

# Service tests
dotnet test --filter "FullyQualifiedName~Services"

# Component tests
dotnet test --filter "FullyQualifiedName~Components"

# Page tests
dotnet test --filter "FullyQualifiedName~Pages"
```

### Run Tests in Watch Mode
```bash
dotnet watch test
```

## Test Framework Stack

- **xUnit**: Test framework for unit and integration tests
- **bUnit**: Testing library for Blazor components
- **Moq**: Mocking framework for dependencies
- **Coverlet**: Code coverage collection
- **ReportGenerator**: Code coverage report generation

## Writing Tests

### Using Test Data Builders

Test data builders provide a fluent API for creating test objects with sensible defaults:

```csharp
using static CombatTracker.WebAssembly.Tests.TestDataBuilders;

// Create a character
var character = CreateCharacter()
    .WithName("Eldrid")
    .WithClass("Wizard")
    .WithLevel(5)
    .WithHpMax(30)
    .WithAC(13)
    .Build();

// Create a monster
var goblin = CreateMonster()
    .WithName("Goblin")
    .WithAC(15)
    .WithHp(7)
    .WithInitiativeModifier(2)
    .Build();

// Create a party
var party = CreateParty()
    .WithName("The Brave Adventurers")
    .AddCharacter(character)
    .Build();

// Use common fixtures
var fighter = TestFixtures.CreateFighter();
var wizard = TestFixtures.CreateWizard();
var orc = TestFixtures.CreateOrc();
var standardParty = TestFixtures.CreateStandardParty();
```

### Model Validation Tests

Test data annotation validation attributes:

```csharp
[Fact]
public void Character_ValidData_ShouldPassValidation()
{
    // Arrange
    var character = new Character
    {
        Name = "Eldrid",
        Class = "Fighter",
        Level = 5,
        HpMax = 50,
        AC = 18
    };

    // Act
    var results = ValidateModel(character);

    // Assert
    Assert.Empty(results);
}
```

### Component Tests with bUnit

Test Blazor components:

```csharp
public class StatusBadgeTests : TestContext
{
    [Fact]
    public void StatusBadge_ShouldShowAliveBadge_WhenStatusIsAlive()
    {
        // Arrange & Act
        var cut = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, Status.Alive));

        // Assert
        Assert.Contains("badge bg-success", cut.Markup);
        Assert.Contains("Alive", cut.Markup);
    }
}
```

### Service Tests with Mocks

Test services with mocked dependencies:

```csharp
[Fact]
public async Task GetOpenAIApiKeyAsync_ShouldReturnKey_WhenSet()
{
    // Arrange
    var mockJsRuntime = new Mock<IJSRuntime>();
    mockJsRuntime
        .Setup(js => js.InvokeAsync<string>("localStorage.getItem", It.IsAny<object[]>()))
        .ReturnsAsync("test-api-key");
    
    var service = new ApiKeyService(mockJsRuntime.Object);

    // Act
    var result = await service.GetOpenAIApiKeyAsync();

    // Assert
    Assert.Equal("test-api-key", result);
}
```

## Test Best Practices

### 1. Follow AAA Pattern
- **Arrange**: Set up test data and dependencies
- **Act**: Execute the code being tested
- **Assert**: Verify the expected outcome

### 2. Use Descriptive Test Names
- Format: `MethodName_Scenario_ExpectedBehavior`
- Example: `Character_InvalidLevel_ShouldFailValidation`

### 3. Test One Thing Per Test
Each test should verify a single behavior or scenario.

### 4. Use Test Data Builders
Reduces boilerplate and makes tests more maintainable.

### 5. Mock External Dependencies
- Mock JSInterop for browser APIs
- Mock services for unit testing
- Use bUnit's TestContext for component tests

### 6. Test Edge Cases
- Null values
- Empty collections
- Boundary values (min/max)
- Invalid input
- Error scenarios

### 7. Keep Tests Fast
- Avoid unnecessary delays
- Mock slow operations
- Use in-memory data structures

## Current Test Coverage

- **Total Tests**: 316
- **Line Coverage**: 45.9%
- **Branch Coverage**: 44.2%
- **Models**: 100% coverage
- **Well-tested areas**: All data models, core shared components, API key service

## Coverage Goals

- **Target**: >80% line coverage on business logic
- **Priority areas for improvement**:
  - Service layer (PartyStateService, StorageStateService)
  - Page components with business logic
  - Complex component interactions

## CI/CD Integration

Tests run automatically on every pull request:

1. Restore dependencies
2. Build solution
3. Run tests with coverage
4. Generate coverage report
5. Upload coverage artifacts
6. Display coverage summary in workflow log

## Troubleshooting

### Tests Fail Locally
- Ensure all dependencies are restored: `dotnet restore`
- Clean and rebuild: `dotnet clean && dotnet build`
- Check for uncommitted changes that might affect tests

### Coverage Report Issues
- Ensure ReportGenerator is installed: `dotnet tool install -g dotnet-reportgenerator-globaltool`
- Delete TestResults directory and regenerate: `rm -rf TestResults && dotnet test --collect:"XPlat Code Coverage"`

### JSInterop Test Failures
- Ensure proper mocking of IJSRuntime
- Use TestHelpers.CreateMockKeyboardShortcutService() for keyboard shortcut tests
- Check that localStorage methods are properly mocked

## Contributing

When adding new features:
1. Write tests first (TDD approach recommended)
2. Ensure tests pass locally
3. Run coverage report to verify adequate coverage
4. Update this README if adding new test categories
5. Use test data builders for consistency

## Resources

- [xUnit Documentation](https://xunit.net/)
- [bUnit Documentation](https://bunit.dev/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Blazor Testing Best Practices](https://learn.microsoft.com/en-us/aspnet/core/blazor/test)
