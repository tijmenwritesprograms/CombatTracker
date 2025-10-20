# Functional Specification Document  
## Application: D&D Combat Tracker  

---

### 1. Overview  
The **D&D Combat Tracker** is a web application designed to manage and streamline combat encounters in tabletop RPGs, primarily *Dungeons & Dragons (5e)*. It allows users to manage player characters, add monsters, track initiative and turn order, and automate descriptive combat narration using AI-assisted features.  

---

### 2. Objectives  
- Simplify combat management for DMs.  
- Automate initiative tracking and turn progression.  
- Quickly add monsters and NPCs via statblock parsing.  
- Enhance immersion with AI-generated combat narration.  

---

### 3. Core Features  

#### 3.1 Party Management  
- **Create, edit, and delete parties.**  
  - Create multiple party profiles for different campaigns
  - Edit party names
  - Delete entire parties including all characters
- **Create, edit, and delete party members (characters).**  
  - Fields: Name, Class, Level, HP (current/max), AC, Initiative Modifier, and Notes
  - Add new characters to any party
  - Edit character details inline
  - Remove characters from parties
- **Party data persistence**
  - In-memory storage using singleton service (session-based)
  - Data persists during active session
  - Future enhancement: Persist to database using Entity Framework Core
- **Multiple party profiles** supported for different campaigns
- **Character validation**
  - Required fields: Name, Class, Level, HP Max, HP Current, AC
  - Level range: 1-20 (D&D 5e standard)
  - Initiative modifier range: -5 to +10
  - AC range: 1-30  

#### 3.2 Combat Setup  
- **Start New Combat:**  
  - Select an existing party or add individual adventurers.  
  - Add one or more monsters or monster groups.  
  - Manually enter or edit initiatives for all combatants.  
  - Optionally roll initiative automatically (using modifiers).  

- **Add Monsters:**  
  - Enter manually via form: Name, HP, AC, Attack Bonus, Damage, Initiative Modifier, etc.  
  - OR paste a full monster **statblock** into a textarea.  
  - App calls the **AI Statblock Parser (LLM)** to extract relevant fields (HP, AC, attacks, damage, etc.) and prefill monster data.  
  - Users can confirm or edit parsed data before adding.  

#### 3.3 Combat Tracking  
- **Initiative Order Management:**  
  - Sort all combatants by initiative (descending).  
  - Allow manual reordering if needed.  
  - Display current round number and turn tracker.  

- **Turn Progression:**  
  - Highlight active combatant.  
  - “Next Turn” and “Previous Turn” buttons.  
  - Automatically skip dead/unconscious combatants if flagged.  
  - Option to delay/ready actions.  

- **Hit Point Tracking:**  
  - Increment/decrement HP manually or via quick actions (“Take X damage,” “Heal X HP”).  
  - Automatically mark combatant as unconscious/dead when HP ≤ 0.  

#### 3.4 AI-Assisted Features  

##### 3.4.1 Statblock Parsing  
- **Input:** User pastes a full monster statblock (from official or homebrew source).  
- **Process:**  
  - LLM extracts:  
    - Name  
    - Type  
    - HP (max and formula)  
    - AC  
    - Attack bonuses and damage values  
    - Speed, saving throws, and special abilities (optional)  
  - Return structured JSON or prefilled form fields.  
- **Output:** User can review and confirm the monster entry before adding to combat.  

##### 3.4.2 Combat Narration  
- **Trigger:** User selects “Generate Narration” during a combatant’s turn.  
- **Input Parameters:**  
  - Combatant name  
  - Target name  
  - Attack type (melee/ranged/spell)  
  - Result (hit, miss, critical)  
  - Optional context (location, weapon, current HP of both parties)  
- **Process:**  
  - LLM generates 1–2 sentence immersive narration describing the attack outcome.  
  - Example:  
    - *Hit:* “Eldrid thrusts his windspear, piercing the ogre’s flank with a spray of blood.”  
    - *Miss:* “Gwion’s arrow whistles past the goblin’s ear, embedding in the cave wall.”  
    - *Critical:* “Breaker’s hammer crashes into the skeleton, shattering bone and scattering shards across the floor.”  

---

### 4. User Interface  

#### 4.1 Navigation
The application uses a responsive sidebar navigation with the following main sections:
- **Home**: Landing page with overview of application features and quick access to main sections
- **Party Management**: Interface for creating and managing adventuring parties
- **Combat Setup**: Interface for configuring and starting combat encounters

The navigation is implemented using Blazor Server routing with a collapsible sidebar for mobile/tablet devices.

#### 4.2 Screens / Views  
1. **Home / Dashboard:**  
   - Welcome message and application overview
   - Quick access cards for Party Management and Combat Setup
   - Link to documentation and project information
   - Responsive card layout using Bootstrap grid system

2. **Party Management Screen:**  
   - "Create New Party" button to add new parties
   - Card-based layout displaying all parties
   - Each party card shows:
     - Party name with edit/delete buttons
     - List of characters in table format (Name, Class, Level, HP, AC, Initiative Modifier)
     - Character management buttons (Edit, Delete)
     - "Add Character" button
   - Modal form for adding/editing characters with full validation
   - Party creation/edit form with inline validation
   - Empty state message when no parties exist

3. **Combat Setup Screen:**  
   - Add/remove combatants.  
   - Enter or roll initiatives.  
   - Launch combat tracking view.

4. **Combat Tracker Screen:**  
   - Initiative order list.  
   - Current turn indicator.  
   - Controls: Next Turn, Previous Turn, Edit HP, Add Monster, Generate Narration.  
   - Round counter.  
   - Log panel (for turn history, narration, and HP changes).

#### 4.3 Design Principles
- **Responsive Layout**: Uses Bootstrap CSS framework for mobile-first responsive design
- **Accessibility**: Semantic HTML elements with proper ARIA labels
- **Consistent Theme**: D&D themed color scheme with purple/blue gradient sidebar
- **Clean Navigation**: Clear visual hierarchy with icon-based navigation items

---

### 5. System Architecture  

#### 5.1 Frontend  
- Framework: ASP.NET Core Blazor Server
- State management: Blazor component state and service-based state management
- UI Library: Bootstrap CSS framework
- Routing: Blazor Server routing with `@page` directives  

#### 5.2 Backend  
- Backend: ASP.NET Core with C#
- Database: Entity Framework Core with SQLite or SQL Server
- Data Storage: Local database with Entity Framework Core
- Authentication: ASP.NET Core Identity (optional for multi-user scenarios)  

#### 5.3 AI Integration  
- LLM endpoints:  
  - `/api/parse-statblock` → Extract monster data.  
  - `/api/generate-narration` → Create dynamic narration.  
- Each endpoint uses prompt templates tailored to the D&D 5e format.  

---

### 6. Data Model (simplified)  

#### Entities  

**Party**  
- id  
- name  
- members: [Character]  

**Character**  
- id  
- name  
- class  
- level  
- hpCurrent  
- hpMax  
- ac  
- initiativeModifier  
- notes  

**Monster**  
- id  
- name  
- type  
- hp  
- ac  
- attacks: [Attack]  
- initiativeModifier  

**Combat**  
- id  
- partyId  
- monsters: [Monster]  
- combatants: [CombatantInstance]  
- round  
- turnIndex  

**CombatantInstance**  
- referenceId (character or monster id)  
- initiative  
- hpCurrent  
- status (alive/unconscious/dead)  

---

### 7. Non-Functional Requirements  
- Responsive design (usable on desktop/tablet).  
- Offline persistence for local sessions.  
- Fast initiative sorting and state updates (<100ms).  
- Secure LLM API usage (tokenized requests).  
- Export combat log to text or JSON.  

---

### 8. Future Enhancements (Optional)  
- Condition tracking (poisoned, stunned, etc.).  
- Automated dice roller integration.  
- Map or token view.  
- Voice narration output using TTS.  
- Integration with D&D Beyond or Foundry VTT.  
