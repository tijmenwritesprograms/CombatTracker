# Copilot Instructions

This project is a **D&D Combat Tracker** web application designed to manage and streamline combat encounters in tabletop RPGs, primarily Dungeons & Dragons (5e). It allows users to manage player characters, add monsters, track initiative and turn order, and automate descriptive combat narration using AI-assisted features.

## Tech Stack

- **Frontend**: React (or similar modern SPA framework)
- **State Management**: Redux/Zustand or equivalent
- **UI Library**: TailwindCSS or ShadCN UI
- **Backend**: Node.js + Express or .NET API (cloud-backed option) or local storage (IndexedDB/browser storage for local app)
- **Database**: SQLite or PostgreSQL (for cloud-backed) or IndexedDB (for local)
- **AI Integration**: LLM endpoints for statblock parsing and combat narration

## Coding Standards

- Use camelCase for variable and function names
- Use PascalCase for React component names and class names
- Use single quotes for strings in JavaScript/TypeScript
- Use 2 spaces for indentation
- Use arrow functions for callbacks
- Use async/await for asynchronous code
- Use const for constants and let for variables that will be reassigned
- Use destructuring for objects and arrays
- Use template literals for strings that contain variables
- Utilize ES6+ features whenever possible

## Project Structure

### Core Entities

- **Party**: Represents a group of player characters with persistent data
- **Character**: Player character with stats (name, class, level, HP, AC, initiative modifier, etc.)
- **Monster**: Enemy creature with combat stats and attacks
- **Combat**: Active combat session tracking combatants, initiative order, rounds, and turns
- **CombatantInstance**: Individual combatant in a combat with current state (initiative, HP, status)

### Key Features

1. **Party Management**: Create, edit, delete party members with persistent data
2. **Combat Setup**: Start combat with party selection, monster addition, and initiative rolling
3. **Combat Tracking**: Initiative order management, turn progression, HP tracking
4. **AI-Assisted Features**:
   - Statblock parsing: Extract monster data from pasted text
   - Combat narration: Generate immersive descriptions for attack outcomes

## AI Integration Guidelines

### Statblock Parsing

- Endpoint: `/api/parse-statblock`
- Extract: Name, Type, HP (max and formula), AC, Attack bonuses, Damage values, Speed, Saving throws, Special abilities
- Return structured JSON or prefilled form fields
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

- Write tests for all new features and bug fixes
- Test AI integration endpoints with mock responses
- Test combat logic thoroughly (initiative order, turn progression, HP changes)
- Test data persistence (local storage or database)
- Use appropriate testing frameworks for the chosen tech stack (Jest for React, Vitest, etc.)

## Security Practices

- Never hardcode API keys or secrets
- Use environment variables for LLM API endpoints and credentials
- Secure LLM API usage with tokenized requests
- Validate all user inputs, especially for statblock parsing
- Sanitize AI-generated content before displaying

## Performance Requirements

- Fast initiative sorting and state updates (<100ms)
- Responsive design (usable on desktop/tablet)
- Offline persistence for local sessions
- Optimize AI calls to minimize latency

## Documentation

- Document public functions and components
- Use JSDoc for JavaScript/TypeScript
- Keep functional specifications up to date in `/spec/functionalspecs.md`
- Document AI prompt templates and expected responses

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
- Condition tracking (poisoned, stunned, etc.)
- Automated dice roller integration
- Map or token view
- Voice narration output using TTS
- Integration with D&D Beyond or Foundry VTT
- Export combat log to text or JSON
