# Copilot Instructions

This project is a **D&D Combat Tracker** web application designed to manage and streamline combat encounters in tabletop RPGs, primarily Dungeons & Dragons (5e). It allows users to manage player characters, add monsters, track initiative and turn order, and automate descriptive combat narration using AI-assisted features.

## Tech Stack

- **Framework**: ASP.NET Core Blazor WebAssembly (Standalone)
- **Frontend**: Blazor components with Razor syntax running client-side via WebAssembly
- **State Management**: Client-side service-based state management with browser localStorage
- **UI Library**: Bootstrap 5
- **Backend**: ASP.NET Core API service for future AI features
- **Orchestration**: .NET Aspire for development and deployment
- **Testing**: bUnit and xUnit for component and unit tests

## Coding Standards

- Use PascalCase for class names, method names, and public properties
- Use camelCase for private fields (with underscore prefix: `_fieldName`)
- Use PascalCase for Blazor component file names and component classes
- Use 4 spaces for indentation (standard C# convention)
- Use async/await for asynchronous operations
- Use `var` for local variables when the type is obvious, otherwise use explicit types
- Follow C# naming conventions and .NET coding standards
- Use nullable reference types to prevent null reference exceptions
- Prefer dependency injection for service registration and consumption
- Use browser localStorage for data persistence via JSInterop

## Project Structure

### Core Entities

- **Party**: Represents a group of player characters with persistent data
- **Character**: Player character with stats (name, class, level, HP, AC, initiative modifier, etc.)
- **Monster**: Enemy creature with full D&D 5e statblock support including:
  - Basic info: name, size, type, subtype, alignment
  - Defense: AC, armor type, HP, HP formula
  - Movement: speed, fly speed, swim speed, climb speed, burrow speed
  - Ability scores: STR, DEX, CON, INT, WIS, CHA with automatic modifier calculation
  - Proficiencies: saving throws, skills
  - Resistances: damage vulnerabilities, resistances, immunities, condition immunities
  - Senses: darkvision, blindsight, passive perception, etc.
  - Languages: spoken and understood languages
  - Challenge rating: CR and XP value, proficiency bonus
  - Traits: special abilities (e.g., "Aggressive", "Pack Tactics")
  - Actions: regular actions, bonus actions, reactions
  - Legendary actions: legendary actions and actions per round
  - Lair actions: environment-based actions
- **MonsterTrait**: Special trait or ability with name and description
- **MonsterAction**: Action with full attack details (attack type, attack bonus, reach/range, damage formula, damage type)
- **AbilityScores**: Six D&D ability scores with automatic modifier calculation
- **Combat**: Active combat session tracking combatants, initiative order, rounds, and turns
- **CombatantInstance**: Individual combatant in a combat with current state (initiative, HP, status)

### Key Features

1. **Party Management**: Create, edit, delete party members with client-side state and localStorage persistence
2. **Combat Setup**: Start combat with party selection, monster addition, and initiative rolling through Blazor forms
3. **Combat Tracking**: Initiative order management, turn progression, HP tracking with real-time UI updates via Blazor's client-side state management
4. **AI-Assisted Features** (planned):
   - Statblock parsing: Extract monster data from pasted text using HTTP calls to API service
   - Combat narration: Generate immersive descriptions for attack outcomes

## AI Integration Guidelines

### Statblock Parsing

- Endpoint: `/api/parse-statblock`
- Extract all D&D 5e statblock properties:
  - Basic: name, size, type, subtype, alignment
  - Defense: AC, armor type, HP, HP formula
  - Movement: all speed types (walk, fly, swim, climb, burrow)
  - Ability scores: STR, DEX, CON, INT, WIS, CHA
  - Proficiencies: saving throws, skills
  - Resistances: vulnerabilities, resistances, immunities, condition immunities
  - Senses and languages
  - Challenge rating, XP, proficiency bonus
  - Traits: special abilities with names and descriptions
  - Actions: all action types (actions, bonus actions, reactions, legendary, lair)
  - For each action: name, description, attack type, attack bonus, reach/range, damage formula and type
- Return structured JSON matching Monster model
- User should review and confirm before adding to combat

### Combat Narration

- Endpoint: `/api/generate-narration`
- Input: Combatant name, target name, attack type (melee/ranged/spell), result (hit/miss/critical), optional context
- Output: 1-2 sentence immersive narration
- Examples:
  - Hit: "Eldrid thrusts his windspear, piercing the ogre's flank with a spray of blood."
  - Miss: "Gwion's arrow whistles past the goblin's ear, embedding in the cave wall."
  - Critical: "Breaker's hammer crashes into the skeleton, shattering bone and scattering shards across the floor."

## Testing Requirements

- Write unit tests for all new features and bug fixes using xUnit
- Test AI integration endpoints with mock HTTP responses
- Test combat logic thoroughly (initiative order, turn progression, HP changes)
- Test data persistence with localStorage mocking
- Use Moq for mocking dependencies
- Use bUnit for Blazor component testing
- Aim for high code coverage on business logic and services

## Security Practices

- Never hardcode API keys or secrets; use User Secrets for development and Azure Key Vault for production
- Use configuration from appsettings.json and environment variables for LLM API endpoints
- Secure LLM API usage with proper authentication tokens
- Validate all user inputs using data annotations and client-side validation
- Sanitize AI-generated content before rendering in Blazor components
- Use proper authorization and authentication if user accounts are needed

## Performance Requirements

- Fast initiative sorting and state updates (<100ms)
- Responsive design (usable on desktop/tablet) using Bootstrap
- Optimize WebAssembly bundle size (consider lazy loading for large assemblies)
- Minimize localStorage operations for better performance
- Minimize API call latency with proper async patterns

## Documentation

- Document public classes, methods, and properties using XML documentation comments (///)
- Keep functional specifications up to date in `/spec/functionalspecs.md`
- Document AI prompt templates and expected responses
- Follow standard C# documentation practices for clarity

## D&D 5e Context

When working with D&D mechanics:
- Initiative uses d20 + modifier
- Hit Points (HP) represent health; 0 or below means unconscious/dead
- Armor Class (AC) is the target number for attacks
- Combat proceeds in rounds with turn order based on initiative
- Common statuses: alive, unconscious, dead
- Future enhancements may include conditions (poisoned, stunned, etc.)

## Future Enhancements

Consider these planned features when designing:
- Condition tracking (poisoned, stunned, etc.) with Blazor state management
- Automated dice roller integration with real-time updates
- Map or token view using Blazor components or JavaScript interop
- Voice narration output using Text-to-Speech APIs
- Integration with D&D Beyond or other APIs
- Export combat log to text or JSON format
- Progressive Web App (PWA) support for offline functionality
- Service worker for background sync and caching
